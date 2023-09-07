using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace YuLinTu.Library.Basic
{
    [Serializable()]
    public class YltEntity : ICloneable, IComparable
    {
        #region Properties

        #endregion

        #region Fields

        #endregion

        #region Methods

        public virtual int CompareTo(object obj)
        {
            return 0;
        }

        public virtual object Clone()
        {
            return this.MemberwiseClone();
        }

        #endregion
    }
}