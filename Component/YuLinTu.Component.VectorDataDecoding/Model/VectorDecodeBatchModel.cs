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
    public class VectorDecodeBatchModel : NotifyCDObject
    {
        #region Properties

 

  
        public string BatchCode { get;set;   }
        public string ZoneCode { get; set; }
        public string UplaodTime { get; set; }
        public int DataCount { get; set; }
        public string DecodeProgress { get; set; }
        public string DecodeStaus { get; set; }
        public string DataStaus { get; set; }
        public int NumbersOfDownloads { get; set; }
        public string SupportingMaterials { get; set; }


        public System.Collections.ObjectModel.ObservableCollection<VectorDecodeMode> Children { get; set; }
        #endregion

        #region Ctor

        public VectorDecodeBatchModel()
        {
            Children = new System.Collections.ObjectModel.ObservableCollection<VectorDecodeMode>();

        }

        #endregion
    }
}
