using System;
using YuLinTu.Data;

namespace YuLinTu.Component.VectorDataDecoding
{
    internal class LandEntity
    {
        public string dkbm { get; set; }

        public string ewkt { get; set; }

        public string qqdkbm { get; set; }

    }

    [Serializable]
    internal class QCDK
    {
        public const string TableName = "DK";

        public const string TableNameCN = "地块";

        public const string CBSM = "BSM";

        public const string CYSDM = "YSDM";

        public const string CDKBM = "DKBM";

        public const string CQQDKBM = "QQDKBM";

        public const string CCBFBM = "CBFBM";

        public const string CDKMC = "DKMC";

        public const string CDKLB = "DKLB";

        public const string CSYQXZ = "SYQXZ";

        public const string CTDLYLX = "TDLYLX";

        public const string CDLDJ = "DLDJ";

        public const string CTDYT = "TDYT";

        public const string CSFJBNT = "SFJBNT";

        public const string CSCMJ = "SCMJ";

        public const string CSCMJM = "SCMJM";

        public const string CDKDZ = "DKDZ";

        public const string CDKXZ = "DKXZ";

        public const string CDKNZ = "DKNZ";

        public const string CDKBZ = "DKBZ";

        public const string CDKBZXX = "DKBZXX";

        public const string CZJRXM = "ZJRXM";

        public const string CKJZB = "KJZB";

        public int BSM { get; set; }

        public string YSDM { get; set; }

        public string DKBM { get; set; }

        public string QQDKBM { get; set; }

        public string CBFBM { get; set; }

        public string DKMC { get; set; }

        public string SYQXZ { get; set; }

        public string DKLB { get; set; }

        public string TDLYLX { get; set; }

        public string DLDJ { get; set; }

        public string TDYT { get; set; }

        public string SFJBNT { get; set; }

        public double SCMJ { get; set; }

        public double? SCMJM { get; set; }

        public string DKDZ { get; set; }

        public string DKXZ { get; set; }

        public string DKNZ { get; set; }

        public string DKBZ { get; set; }

        public string DKBZXX { get; set; }

        public string ZJRXM { get; set; }

        public string KJZB { get; set; }

        public object Shape { get; set; }

        public static string ExpandName()
        {
            string empty = string.Empty;
            return "DK(地块)";
        }
    }

    /// <summary>
    /// json对象
    /// </summary>
    internal class DataJsonEntity
    {
        public string locations { get; set; }
        public string key { get; set; }

        public DataJsonEntity(string locations, string key)
        {
            this.locations = locations;
            this.key = key;
        }
    }

    /// <summary>
    /// 空间数据对象
    /// </summary>
 [DataTable("CBD_DK", AliasName = "承包地")]
    internal class SpaceLandEntity
    {
        /// <summary>
        /// 地块编码
        /// </summary>
        public string DKBM { get; set; }

        ///// <summary>
        ///// 确权地块编码
        ///// </summary>
        //public string QQDKBM { get; set; }

        /// <summary>
        /// 地块名称
        /// </summary>
        public string DKMC { get; set; }

        public string XZDYDM { get; set; }
        /// <summary>
        /// 承包方编码
        /// </summary>
        public string CBFBM { get; set; }

        public string BatchCode { get; set; }
        [DataColumn("FileID", AliasName = "文件标识")]
        public Guid FileID { get; set; }
        /// <summary>
        /// 图形
        /// </summary>
        public YuLinTu.Spatial.Geometry Shape { get; set; }
    }
}
