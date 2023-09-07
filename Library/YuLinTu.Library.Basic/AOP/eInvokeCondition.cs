using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YuLinTu.Library.Basic
{
    public enum eInvokeCondition
    {
        InvokeBegin = 1,
        InvokeEnd = 2,
        InvokeException = 4,
        InvokeOverride = 8,
        InvokeOverrideException = 16,
    }
}