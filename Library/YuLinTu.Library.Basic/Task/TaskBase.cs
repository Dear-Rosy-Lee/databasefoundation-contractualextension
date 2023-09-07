using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YuLinTu.Library.Basic
{
    [Serializable]
    public abstract class TaskBase : IDisposable, ICloneable
    {
        #region Properties

        public virtual TaskMetadata Argument { get; set; }
        public virtual bool ReportTime { get; set; }

        public bool IsBusy { get; private set; }

        public Guid ID { get; private set; }
        public string Name { get; set; }

        protected object SyncObject { get; private set; }

        protected bool HasError { get; set; }
        protected bool HasWarning { get; set; }
        protected bool HasStopped { get; set; }

        private bool isStopped;

        #endregion

        #region Events

        public event TaskProgressChangedEventHandler ProgressChanged;
        public event TaskAlertEventHandler Alert;
        public event TaskCompletedEventHandler Completed;

        #endregion

        #region Ctor

        static TaskBase()
        {
            LanguageAttribute.AddLanguage(Properties.Resources.langChs_Task);
        }

        public TaskBase()
        {
            ID = Guid.NewGuid();
            Name = string.Empty;
            SyncObject = new object();
        }

        ~TaskBase()
        {
            Dispose(false);
        }

        #endregion

        #region Methods

        #region Methods - Public

        public void Do(TaskMetadata meta)
        {
            if (IsBusy)
                return;

            IsBusy = true;

            Argument = meta;
            HasError = false;
            HasWarning = false;
            HasStopped = false;
            isStopped = false;

            if (ReportTime)
                ReportAlert(eLogGrade.Infomation, null,
                    string.Format(LanguageAttribute.GetLanguage("lang29010"), DateTime.Now));

            try { OnDo(meta); }
            catch (Exception ex) { ReportException(ex); }
            finally { try { Dispose(); } catch { } }

            if (ReportTime)
                ReportAlert(eLogGrade.Infomation, null,
                    string.Format(LanguageAttribute.GetLanguage("lang29011"), DateTime.Now));


            ReportCompleted();

            IsBusy = false;
        }

        public void Do()
        {
            Do(Argument);
        }

        public void Stop()
        {
            OnStop();
            isStopped = true;
        }

        public void Dispose()
        {
            Dispose(true);
        }

        #endregion

        #region Methods - Override

        public virtual object Clone()
        {
            TaskBase newTask = Activator.CreateInstance(this.GetType()) as TaskBase;
            ToolReflection.CopyMember(this, newTask);

            return newTask;
        }

        protected virtual void Dispose(bool disposing)
        {
        }

        protected virtual void OnDo(object arg)
        {
        }

        protected virtual void OnStop()
        {
        }

        #endregion

        #region Methods - Protected

        protected void ReportProgress(int percent, object userState)
        {
            TaskProgressChangedEventArgs e = new TaskProgressChangedEventArgs(percent, userState);
            ReportProgress(e);
        }

        protected void ReportProgress(int percent, object userState, bool showPersent)
        {
            TaskProgressChangedEventArgs e = new TaskProgressChangedEventArgs(percent, userState, showPersent);
            ReportProgress(e);
        }

        protected void ReportProgress(int percent, object userState, bool showPersent, string description)
        {
            TaskProgressChangedEventArgs e = new TaskProgressChangedEventArgs(percent, userState, showPersent, description);
            ReportProgress(e);
        }

        protected void ReportProgress(TaskProgressChangedEventArgs e)
        {
            if (isStopped)
                throw new TaskStopException();

            if (ProgressChanged != null)
                ProgressChanged(this, e);
        }

        protected bool ReportException(object userState, string description)
        {
            return ReportAlert(eLogGrade.Exception, userState, description);
        }

        protected void ReportException(TaskAlertEventArgs e)
        {
            e.Grade = eLogGrade.Exception;
            ReportAlert(e);
        }

        protected void ReportException(Exception ex)
        {
            if (ex is TaskStopException)
            {
                HasStopped = true;
                return;
            }

            string msg = Id.GetName(Id.exUnhandledTaskException);
            msg = string.Format(msg, ToolException.GetExceptionMessage(ex));
            ReportException(ex, msg);
        }

        protected bool ReportAlert(object userState, string description)
        {
            TaskAlertEventArgs e = new TaskAlertEventArgs(userState, description);
            ReportAlert(e);

            return e.IsCancel;
        }

        protected bool ReportAlert(eLogGrade grade, object userState, string description)
        {
            TaskAlertEventArgs e = new TaskAlertEventArgs(grade, userState, description);
            ReportAlert(e);

            return e.IsCancel;
        }

        protected void ReportAlert(TaskAlertEventArgs e)
        {
            if (Alert != null)
                Alert(this, e);

            if (e.Grade == eLogGrade.Warn)
                HasWarning = true;

            if (e.Grade == eLogGrade.Error || e.Grade == eLogGrade.Exception)
                HasError = true;

            if (e.Grade != eLogGrade.Exception)
                return;

            string msg = string.Format(LanguageAttribute.GetLanguage("ex20106"), e.UserState);
            Trace.WriteLine(new Log()
            {
                Description = msg,
                EventID = 20106,
                Grade = eLogGrade.Exception,
                Source = this.GetType().FullName
            });
        }

        #endregion

        #region Methods - Private

        private void ReportCompleted()
        {
            if (Completed != null)
                Completed(this, new TaskCompletedEventArgs()
                {
                    HasError = HasError,
                    HasWarning = HasWarning,
                    HasStopped = HasStopped
                });
        }

        #endregion

        #endregion
    }
}
