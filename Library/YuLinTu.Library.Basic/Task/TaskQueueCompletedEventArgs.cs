using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YuLinTu.Library.Basic
{
    public delegate void TaskQueueCompletedEventHandler(object sender, TaskQueueCompletedEventArgs e);

    [Serializable]
    public class TaskQueueCompletedEventArgs : EventArgs
    {
        #region Properties

        #endregion

        #region Ctor

        public TaskQueueCompletedEventArgs()
        {
        }

        #endregion
    }
}
