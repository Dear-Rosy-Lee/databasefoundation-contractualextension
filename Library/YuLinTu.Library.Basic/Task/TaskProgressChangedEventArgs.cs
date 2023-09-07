using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YuLinTu.Library.Basic
{
    public delegate void TaskProgressChangedEventHandler(object sender, TaskProgressChangedEventArgs e);

    [Serializable]
    public class TaskProgressChangedEventArgs : EventArgs
    {
        #region Properties

        public int Percent { get; set; }
        public object UserState { get; set; }
        public bool ShowPersent { get; set; }
        public string Description { get; set; }

        #endregion

        #region Ctor

        public TaskProgressChangedEventArgs(int percent, object userState)
        {
            Percent = percent;
            UserState = userState;
            ShowPersent = true;
            Description = "";
        }

        public TaskProgressChangedEventArgs(int percent, object userState, bool showPersent)
        {
            Percent = percent;
            UserState = userState;
            ShowPersent = showPersent;
        }

        public TaskProgressChangedEventArgs(int percent, object userState, bool showPersent, string description)
        {
            Percent = percent;
            UserState = userState;
            ShowPersent = showPersent;
            Description = description;
        }

        #endregion
    }
}
