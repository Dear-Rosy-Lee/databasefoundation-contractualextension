using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YuLinTu.Library.Basic
{
    public delegate void DetentionElapsedEventHandler(object sender, DetentionElapsedEventArgs e);

    public class DetentionElapsedEventArgs : EventArgs
    {
        #region Properites

        public object Value { get; set; }

        #endregion

        #region Ctor

        public DetentionElapsedEventArgs(object value)
        {
            Value = value;
        }

        #endregion
    }
}
