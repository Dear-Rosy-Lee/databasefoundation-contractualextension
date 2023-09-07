using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YuLinTu.Library.Basic
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class ExceptionValueAttribute : Attribute
    {
        #region Fields

        private object value;

        #endregion

        #region Properties

        public object Value
        {
            get { return value; }
        }

        #endregion

        #region Ctor

        public ExceptionValueAttribute(object value)
        {
            this.value = value;
        }

        #endregion
    }
}