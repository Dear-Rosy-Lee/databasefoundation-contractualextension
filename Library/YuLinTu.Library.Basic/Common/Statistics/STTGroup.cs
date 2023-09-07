using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YuLinTu.Library.Basic;

namespace YuLinTu.Library.Basic
{
    [Serializable]
    public class STTGroup
    {
        #region Properties

        public virtual string Name { get; set; }
        public List<STTMetadata> Items { get; private set; }

        #endregion

        #region Ctor

        public STTGroup()
        {
            Items = new List<STTMetadata>();
        }

        #endregion

        #region Methods

        public override string ToString()
        {
            return Name;
        }

        #endregion
    }
}
