using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YuLinTu.Library.Basic
{
    public delegate void TaskCompletedEventHandler(object sender, TaskCompletedEventArgs e);

    [Serializable]
    public class TaskCompletedEventArgs : EventArgs
    {
        #region Properties

        public bool HasError { get; set; }
        public bool HasWarning { get; set; }
        public bool HasStopped { get; set; }

        #endregion

        #region Ctor

        public TaskCompletedEventArgs()
        {
        }

        #endregion
    }
}
