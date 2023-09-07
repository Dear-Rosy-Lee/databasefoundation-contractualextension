using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace YuLinTu.Library.Basic
{
    public class ToolBasic
    {
        #region Methods

        public static void TraversalEnumerator(ParameterInvokeDelegate method, IEnumerator et)
        {
            if (method == null || et == null)
                return;

            et.Reset();
            while (et.MoveNext())
                method(et.Current);
        }

        public static bool HasObject(IEnumerable eb)
        {
            if (eb == null)
                return false;

            IEnumerator et = eb.GetEnumerator();
            et.Reset();

            return et.MoveNext();
        }

        #endregion
    }
}
