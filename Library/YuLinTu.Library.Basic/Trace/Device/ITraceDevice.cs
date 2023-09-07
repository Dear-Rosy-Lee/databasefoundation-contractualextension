using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YuLinTu.Library.Basic
{
    public interface ITraceDevice
    {
        #region Methods

        void Write(Log log);
        void WriteLine(Log log);

        void Clear();

        #endregion
    }
}
