using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YuLinTu.Library.Basic
{
    [Serializable]
    public class NamedObject : YltObject
    {
        #region Properties

        public string Name { get; set; }

        #endregion

        #region Methods

        public override int CompareTo(object obj)
        {
            NamedObject objName = obj as NamedObject;
            if (objName == null)
                return 0;

            return Name.CompareTo(objName.Name);
        }

        public override void Dispose()
        {
            Name = null;
        }

        #endregion
    }
}
