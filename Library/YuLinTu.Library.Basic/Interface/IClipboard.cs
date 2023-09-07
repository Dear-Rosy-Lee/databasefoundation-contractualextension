using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YuLinTu.Library.Basic
{
    public interface IClipboard
    {
        void Copy();
        void Cut();
        void Paste();
    }
}
