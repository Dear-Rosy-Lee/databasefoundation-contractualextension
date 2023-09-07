using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace YuLinTu.Library.Basic
{
    public class HashtableHasDefault : Hashtable
    {
        #region Properties

        public object Default { get; set; }

        public new object this[object key]
        {
            get
            {
                if (this.ContainsKey(key))
                    return base[key];
                else
                    return Default;
            }
            set { base[key] = value; }
        }

        #endregion
    }
}
