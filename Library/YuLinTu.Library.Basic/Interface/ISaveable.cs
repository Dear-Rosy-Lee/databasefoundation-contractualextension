using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YuLinTu.Library.Basic
{
    public interface ISaveable
    {
        bool Save();
        bool SaveAs();
    }
}
