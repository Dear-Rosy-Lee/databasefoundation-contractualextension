using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YuLinTu.Library.Basic
{
    public class ToolCom
    {
        public static void ReleaseComObject(Object objCom)
        {
            if (objCom == null)
                return;

            System.Runtime.InteropServices.Marshal.ReleaseComObject(objCom);
        }
    }
}
