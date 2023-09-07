using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YuLinTu.Library.Basic
{
    [Serializable()]
    public class YltEntityID : YltEntity
    {
        #region Properites

        public Guid ID { get; set; }

        #endregion

        #region Ctor

        public YltEntityID()
        {
            ID = Guid.NewGuid();
        }

        #endregion
    }
}