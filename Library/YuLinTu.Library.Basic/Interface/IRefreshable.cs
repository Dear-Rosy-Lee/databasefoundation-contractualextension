using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YuLinTu.Library.Basic
{
    public interface IRefreshable
    {
        void Refresh();
        void Reload();
        void Clear();
    }
}
