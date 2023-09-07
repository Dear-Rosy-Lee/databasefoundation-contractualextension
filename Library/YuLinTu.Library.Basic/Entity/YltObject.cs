using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YuLinTu.Library.Basic
{
    [Serializable]
    public abstract class YltObject : ICloneable, IDisposable, IComparable
    {
        #region Methods

        public virtual object Clone()
        {
            return MemberwiseClone();
        }

        public virtual void Dispose()
        {
        }

        public virtual int CompareTo(object obj)
        {
            return 0;
        }

        #endregion
    }
}
