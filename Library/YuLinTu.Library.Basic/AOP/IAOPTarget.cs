using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YuLinTu.Library.Basic
{
    public interface IAOPTarget
    {
        #region Properties

        Log LastErrorLog { get; set; }
        Exception LastException { get; set; }

        #endregion
    }
}
