using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YuLinTu.Library.Basic
{
    [Serializable()]
    public class YltEntityIDName : YltEntityID
    {
        #region Properties

        public string Name { get; set; }

        #endregion

        #region Ctor

        public YltEntityIDName()
        {
            Name = string.Empty;
        }

        #endregion

        #region Methods

        public override string ToString()
        {
            return Name;
        }

        public override int CompareTo(object obj)
        {
            YltEntityIDName entity = obj as YltEntityIDName;
            if (entity == null)
                return 1;

            bool srcNull = string.IsNullOrEmpty(Name);
            bool tgtNull = string.IsNullOrEmpty(entity.Name);

            if (srcNull && tgtNull)
                return 0;

            if (srcNull && !tgtNull)
                return -1;

            if (!srcNull && tgtNull)
                return 1;

            return Name.CompareTo(entity.Name);
        }

        #endregion
    }
}
