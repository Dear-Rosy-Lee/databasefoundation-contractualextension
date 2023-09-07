using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YuLinTu.Library.Basic;

namespace YuLinTu.Library.Basic
{
    [Serializable]
    public class STTGallery
    {
        #region Properties

        public virtual string Name { get; set; }
        public virtual string Title { get; set; }
        public List<STTGroup> Items { get; private set; }

        #endregion

        #region Ctor

        public STTGallery()
        {
            Items = new List<STTGroup>();
        }

        #endregion

        #region Methods

        public override string ToString()
        {
            return string.Format("{0} : {1}", Name, Title);
        }

        #endregion
    }
}
