using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YuLinTu.Library.Basic
{
    public static class ExtensionString
    {
        #region Methods - String

        public static byte[] ToByteArray(this string source)
        {
            return Encoding.ASCII.GetBytes(source);
        }

        #endregion
    }
}
