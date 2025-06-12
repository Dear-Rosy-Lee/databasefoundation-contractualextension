using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YuLinTu;
using YuLinTu.Data;
using YuLinTu.DF;

namespace YuLinTu.Component.VectorDataDecoding
{
    [AddINotifyPropertyChangedInterface]
    [Serializable]
    [DataTable("FileInfomation", AliasName = "矢量文件信息表")]
    public class VectorDecodeMode : NotifyCDObject
    {
        #region Properties
        [DataColumn("ShapeFileName", AliasName = "矢量文件名")]
        public string ShapeFileName { get; set; }
        public string ZoneCode { get; set; }
        [DataColumn("UplaodTime", AliasName = "矢量文件名")]
        public DateTime? UplaodTime { get; set; }
        public int DataCount { get; set; }
        [DataColumn("FileID", AliasName = "文件标识")]
        public Guid FileID { get; set; }
        public string BatchCode { get; set; }

        public string FilePath { get; set; }
        #endregion

        #region Ctor

        public VectorDecodeMode()
        {
           
        }

        #endregion
    }
}
