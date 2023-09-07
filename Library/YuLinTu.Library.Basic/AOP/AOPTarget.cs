using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YuLinTu.Library.Basic
{
    public abstract class AOPTarget : MarshalByRefObject, IAOPTarget
    {
        #region Properties

        public Log LastErrorLog { get; set; }
        public Exception LastException { get; set; }

        #endregion
    }
}
