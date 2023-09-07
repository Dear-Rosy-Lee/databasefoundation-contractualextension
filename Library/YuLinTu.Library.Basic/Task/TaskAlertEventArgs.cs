using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YuLinTu.Library.Basic
{
    public delegate void TaskAlertEventHandler(object sender, TaskAlertEventArgs e);

    [Serializable]
    public class TaskAlertEventArgs : EventArgs
    {
        #region Properties

        public eLogGrade Grade { get; set; }
        public bool IsCancel { get; set; }
        public string Description { get; set; }
        public object UserState { get; set; }

        #endregion

        #region Ctor

        public TaskAlertEventArgs()
        {
            Grade = eLogGrade.Error;
            IsCancel = false;
        }

        public TaskAlertEventArgs(object userState, string description)
        {
            Grade = eLogGrade.Error;
            UserState = userState;
            Description = description;
            IsCancel = false;
        }

        public TaskAlertEventArgs(eLogGrade grade, object userState, string description)
        {
            Grade = grade;
            UserState = userState;
            Description = description;
            IsCancel = false;
        }

        #endregion
    }

    [Serializable]
    public class TaskAlertCollection : List<TaskAlertEventArgs>
    {
    }
}
