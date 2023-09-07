using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YuLinTu.Library.Basic
{
    [Serializable]
    public class TaskQueue : TaskBase, IList<TaskBase>
    {
        #region Properties

        public TaskBase this[int index]
        {
            get { return Get(index); }
            set { listTask[index] = value; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public int Count
        {
            get { return listTask.Count; }
        }

        public override bool ReportTime
        {
            get { return base.ReportTime; }
            set { SetReportTime(value); }
        }

        #endregion

        #region Fields

        private List<TaskBase> listTask;

        #endregion

        #region Events

        public event TaskAlertEventHandler TaskAlert;
        public event TaskProgressChangedEventHandler TaskProgressChanged;
        public event TaskCompletedEventHandler TaskCompleted;

        #endregion

        #region Ctor

        public TaskQueue()
        {
            listTask = new List<TaskBase>();
        }

        #endregion

        #region Methods

        #region Methods - Initialize

        private void InstallTask(TaskBase task)
        {
            task.ProgressChanged += new TaskProgressChangedEventHandler(OnTaskProgressChanged);
            task.Alert += new TaskAlertEventHandler(OnTaskAlert);
            task.Completed += new TaskCompletedEventHandler(OnTaskCompleted);
        }

        private void UninstallTask(TaskBase task)
        {
            task.ProgressChanged -= new TaskProgressChangedEventHandler(OnTaskProgressChanged);
            task.Alert -= new TaskAlertEventHandler(OnTaskAlert);
            task.Completed -= new TaskCompletedEventHandler(OnTaskCompleted);
        }

        #endregion

        #region Methods - Public

        public void Add(TaskBase task)
        {
            lock (SyncObject)
                listTask.Add(task);
        }

        public TaskBase Get(int index)
        {
            lock (SyncObject)
            {
                if (index >= 0 && index < listTask.Count)
                    return listTask[index];
                return null;
            }
        }

        public void Clear()
        {
            lock (SyncObject)
                listTask.Clear();
        }

        public bool Contains(TaskBase item)
        {
            lock (SyncObject)
                return listTask.Contains(item);
        }

        public void CopyTo(TaskBase[] array, int arrayIndex)
        {
            lock (SyncObject)
                listTask.CopyTo(array, arrayIndex);
        }

        public int IndexOf(TaskBase item)
        {
            lock (SyncObject)
                return listTask.IndexOf(item);
        }

        public void Insert(int index, TaskBase item)
        {
            lock (SyncObject)
                listTask.Insert(index, item);
        }

        public void RemoveAt(int index)
        {
            lock (SyncObject)
                listTask.RemoveAt(index);
        }

        public bool Remove(TaskBase item)
        {
            lock (SyncObject)
                return listTask.Remove(item);
        }

        public IEnumerator<TaskBase> GetEnumerator()
        {
            return listTask.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion

        #region Methods - Override

        protected override void OnDo(object arg)
        {
            DoTask();
        }

        protected override void OnStop()
        {
            for (int i = 0; i < Count; i++)
            {
                TaskBase task = Get(i);
                if (task == null)
                    break;

                task.Stop();
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (!disposing)
                return;

            for (int i = 0; i < Count; i++)
            {
                TaskBase task = Get(i);
                if (task == null)
                    break;

                task.Dispose();
            }
        }

        public override object Clone()
        {
            object obj = base.Clone();
            TaskQueue tq = obj as TaskQueue;

            foreach (TaskBase task in this)
                tq.Add(task.Clone() as TaskBase);

            return obj;
        }

        #endregion

        #region Methods - Events

        private void OnTaskProgressChanged(object sender, TaskProgressChangedEventArgs e)
        {
            ReportProgress(sender as TaskBase, e.Percent, e.ShowPersent, e.Description);
            ReportTaskProgress(sender, e);
        }

        private void OnTaskCompleted(object sender, TaskCompletedEventArgs e)
        {
            HasError = HasError || e.HasError;
            HasWarning = HasWarning || e.HasWarning;
            HasStopped = HasStopped || e.HasStopped;

            ReportTaskCompleted(sender, e);
        }

        private void OnTaskAlert(object sender, TaskAlertEventArgs e)
        {
            ReportTaskAlert(sender, e);
            //ReportAlert(e);
        }

        #endregion

        #region Methods - Private

        private void ReportTaskCompleted(object sender, TaskCompletedEventArgs e)
        {
            if (TaskCompleted != null)
                TaskCompleted(sender, e);
        }

        private void ReportTaskAlert(object sender, TaskAlertEventArgs e)
        {
            if (TaskAlert != null)
                TaskAlert(sender, e);
        }

        private void ReportTaskProgress(object sender, TaskProgressChangedEventArgs e)
        {
            if (TaskProgressChanged != null)
                TaskProgressChanged(sender, e);
        }

        private void ReportProgress(TaskBase task, int percent, bool showPersent, string description)
        {
            int index = listTask.IndexOf(task);

            double perStep = ToolMath.GetPercent(1, Count);
            double perBase = ToolMath.GetPercent(index, Count);
            int per = (int)(perBase + perStep * percent / 100);

            ReportProgress(per, new int[] { index, Count }, showPersent, description);
        }

        private void SetReportTime(bool value)
        {
            base.ReportTime = value;
            for (int i = 0; i < Count; i++)
            {
                TaskBase task = Get(i);
                if (task == null)
                    break;

                task.ReportTime = value;
            }
        }

        private void DoTask()
        {
            for (int i = 0; i < Count; i++)
            {
                TaskBase task = Get(i);
                if (task == null)
                    break;

                InstallTask(task);
                task.Do();
                UninstallTask(task);

                if (HasStopped)
                    throw new TaskStopException();
            }
        }

        #endregion

        #endregion
    }
}
