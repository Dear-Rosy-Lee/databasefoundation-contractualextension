using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YuLinTu.Library.Basic
{
    public delegate void TaskQueueProgressChangedEventHandler(object sender, TaskQueueProgressChangedEventArgs e);

    [Serializable]
    public class TaskQueueProgressChangedEventArgs : TaskProgressChangedEventArgs
    {
        #region Properties

        public int CurrentTaskIndex { get; set; }
        public int CountTask { get; set; }

        #endregion

        #region Ctor


        public TaskQueueProgressChangedEventArgs(int indexCurrent, int count, TaskBase taskCurrent)
            : base((int)ToolMath.GetPercent(indexCurrent, count), taskCurrent)
        {
            CurrentTaskIndex = indexCurrent;
            CountTask = count;
        }

        #endregion
    }
}
