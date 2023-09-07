using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YuLinTu.Library.Basic
{
    [Serializable]
    public class STTMetadata
    {
        #region Properties

        public virtual string Name { get; set; }
        public virtual double Value { get; set; }

        #endregion

        #region Methods

        #region Methods - Override

        public override string ToString()
        {
            return string.Format("{0} : {1}", Name, Value);
        }

        #endregion

        #endregion
    }
}
