using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YuLinTu.Library.Basic;

namespace YuLinTu.Library.Basic
{
    public class TraceDeviceConsole : ITraceDevice
    {
        #region Methods

        public void Write(Log log)
        {
            Console.Write(log);
        }

        public void WriteLine(Log log)
        {
            Console.WriteLine(log);
        }

        public void Clear()
        {
            Console.Clear();
        }

        #endregion
    }
}
