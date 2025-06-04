using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YuLinTu;

namespace YuLinTu.Component.VectorDataDecoding
{
    [AddINotifyPropertyChangedInterface]
    [Serializable]
    public class VectorDecodeMode : NotifyCDObject
    {
        #region Properties

        public string ShapeFileName { get; set; }
        public string ZoneCode { get; set; }
        public string UplaodTime { get; set; }
        public int DataCount { get; set; }
      

        #endregion

        #region Ctor

        public VectorDecodeMode()
        {
           
        }

        #endregion
    }
}
