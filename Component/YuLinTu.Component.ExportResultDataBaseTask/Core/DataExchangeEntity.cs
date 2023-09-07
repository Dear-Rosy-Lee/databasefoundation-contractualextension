/*
 * (C) 2015 -2016 鱼鳞图公司版权所有，保留所有权利
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YuLinTu.Data;
using YuLinTu.Spatial;
using YuLinTuQuality.Business.Entity;

namespace YuLinTu.Component.ExportResultDataBaseTask
{
    #region 交换实体

    public class ExchangeRightEntity : ComplexRightEntity
    {
        /// <summary>
        /// 数据库
        /// </summary>
        public new List<CBDKXX> DKXX { get; set; }

        /// <summary>
        /// 空间地块
        /// </summary>
        public List<DKEX> KJDK { get; set; }
    }

    #endregion

    #region 界址线

    /// <summary>
    /// 界址线
    /// </summary>
    [Serializable()]
    [DataTable(TableName = "JZX")]
    public class SqliteJZX
    {
        #region Field

        /// <summary>
        /// 表格名称
        /// </summary>
        public const string TableName = "JZX";

        /// <summary>
        /// 表名
        /// </summary>
        public const string TableNameCN = "界址线";

        #endregion

        #region Property

        /// <summary>
        /// 标识码(M)
        /// </summary>
        [DataColumn(PrimaryKey = false)]
        public int BSM { get; set; }

        /// <summary>
        /// 要素代码(M)(eFeatureType)
        /// </summary>
        public string YSDM { get; set; }

        /// <summary>
        /// 界线性质(O)(eJXXZ)
        /// </summary>
        public string JXXZ { get; set; }

        /// <summary>
        /// 界址线类别(O)(eQSJXLB)
        /// </summary>
        public string JZXLB { get; set; }

        /// <summary>
        /// 界址线位置(M)(eQSJXWZ)
        /// </summary>
        public string JZXWZ { get; set; }

        /// <summary>
        /// 界址线说明(M)
        /// </summary>
        public string JZXSM { get; set; }

        /// <summary>
        /// 毗邻地物权利人(M)
        /// </summary>
        public string PLDWQLR { get; set; }

        /// <summary>
        /// 毗邻地物指界人(M)
        /// </summary>
        public string PLDWZJR { get; set; }

        /// <summary>
        /// 图形
        /// </summary>
        public Geometry Shape { get; set; }

        /// <summary>
        /// 起界址点号
        /// </summary>
        public string QJZDH { get; set; }

        /// <summary>
        /// 止界址点号
        /// </summary>
        public string ZJZDH { get; set; }

        /// <summary>
        /// 起界址点ID
        /// </summary>
        public int FK_QJZDID { get; set; }

        /// <summary>
        /// 止界址点ID
        /// </summary>
        public int FK_ZJZDID { get; set; }

        /// <summary>
        /// 地块编码
        /// </summary>
        public string DKBM { get; set; }

        /// <summary>
        /// 界址线号
        /// </summary>
        public string JZXH { get; set; }

        #endregion
    }

    #endregion

    #region 界址点

    /// <summary>
    /// 界址点
    /// </summary>
    [Serializable]
    [DataTable(TableName = "JZD")]
    public class SqliteJZD
    {
        #region Field

        /// <summary>
        /// 表格名称
        /// </summary>
        public const string TableName = "JZD";

        /// <summary>
        /// 表名
        /// </summary>
        public const string TableNameCN = "界址点";

        #endregion

        #region Property

        /// <summary>
        /// 标识码（M）
        /// </summary>
        [DataColumn(PrimaryKey = false)]
        public int BSM { get; set; }

        /// <summary>
        /// 要素代码(M)(eFeatureType)
        /// </summary>
        public string YSDM { get; set; }

        /// <summary>
        /// 界址点号(M)
        /// </summary>
        public string JZDH { get; set; }

        /// <summary>
        /// 界标类型(O)(eJBLX)
        /// </summary>
        public string JBLX { get; set; }

        /// <summary>
        /// 界址点类型(O)(eJZDLX)
        /// </summary>
        public string JZDLX { get; set; }

        /// <summary>
        /// 地块编码
        /// </summary>
        public string DKBM { get; set; }

        /// <summary>
        /// 图形
        /// </summary>
        public Geometry Shape { get; set; }

        /// <summary>
        /// X坐标值
        /// </summary>
        public double XZBZ { get; set; }

        /// <summary>
        /// Y坐标值
        /// </summary>
        public double YZBZ { get; set; }

        #endregion
    }

    /// <summary>
    /// 地块
    /// </summary>
    [Serializable]
    [DataTable(TableName = "KJDK")]
    public class SqliteDK
    {
        #region ConstName

        /// <summary>
        /// 表名
        /// </summary>
        public const string TableName = "KJDK";

        /// <summary>
        /// 表名
        /// </summary>
        public const string TableNameCN = "地块";

        #endregion

        #region Property

        /// <summary>
        /// 标识码(M)
        /// </summary>
        [DataColumn(PrimaryKey = false)]
        public int BSM { get; set; }

        /// <summary>
        /// 要素代码(M)(eFeatureType)
        /// </summary>
        public string YSDM { get; set; }

        /// <summary>
        /// 地块编码(M)
        /// </summary>
        public string DKBM { get; set; }

        /// <summary>
        /// 地块名称(M)
        /// </summary>
        public string DKMC { get; set; }

        /// <summary>
        /// 所有权性质(O)(eSYQXZ)
        /// </summary>
        public string SYQXZ { get; set; }

        /// <summary>
        /// 地块类别(M)(eDKLB)
        /// </summary>
        public string DKLB { get; set; }

        /// <summary>
        /// 土地利用类型(O)
        /// </summary>
        public string TDLYLX { get; set; }

        /// <summary>
        /// 地力等级(M)(eDLDJ)
        /// </summary>
        public string DLDJ { get; set; }

        /// <summary>
        /// 土地用途(M)(eTDYT)
        /// </summary>
        public string TDYT { get; set; }

        /// <summary>
        /// 是否基本农田(M)(eWhether)
        /// </summary>
        public string SFJBNT { get; set; }

        /// <summary>
        /// 实测面积(M)
        /// </summary>
        public double SCMJ { get; set; }

        /// <summary>
        /// 实测面积亩（o）
        /// </summary>
        public double? SCMJM { get; set; }

        /// <summary>
        /// 地块东至(O)
        /// </summary>
        public string DKDZ { get; set; }

        /// <summary>
        /// 地块西至(O)
        /// </summary>
        public string DKXZ { get; set; }

        /// <summary>
        /// 地块南至(O)
        /// </summary>
        public string DKNZ { get; set; }

        /// <summary>
        /// 地块北至(O)
        /// </summary>
        public string DKBZ { get; set; }

        /// <summary>
        /// 地块备注信息
        /// </summary>
        public string DKBZXX { get; set; }

        /// <summary>
        /// 指界人姓名
        /// </summary>
        public string ZJRXM { get; set; }

        /// <summary>
        /// 图形
        /// </summary>
        public Geometry Shape { get; set; }

        /// <summary>
        /// 空间坐标
        /// </summary>
        public string KJZB { get; set; }

        /// <summary>
        /// 空间坐标ID(界址点BSM集合)
        /// </summary>
        public string FK_KJZBID { get; set; }

        #endregion

        #region Ctor

        /// <summary>
        /// 构造方法
        /// </summary>
        public SqliteDK()
        {
        }

        #endregion
    }

    #endregion

    /// <summary>
    /// 元数据
    /// </summary>
    public static class VictorExtand
    {
        /// <summary>
        /// 元数据初始化
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static VictorData InitiallClass(this VictorData data)
        {
            data.Cont_Info = new VicContInfo();
            data.Data_Info = new VicdataIdInfo();
            data.DQ_Info = new VicdqInfo();
            data.Sys_Info = new VicrefSysInfo();
            data.Data_Info.Title = "XX市XX县农村土地承包经营权数据库";
            data.Data_Info.Date = DateTime.Now.ToString("yyyyMMdd");
            data.Data_Info.GeoID = "";
            data.Data_Info.DataEdition = "1.0";
            data.Data_Info.DataLang = "zh";
            data.Data_Info.IdAbs = "图上内容包括:地块相关、地域相关数据";
            data.Data_Info.Status = "001";
            data.Data_Info.Ending = "";
            data.Data_Info.RpOrgName = "";
            data.Data_Info.RpCnt = "";
            data.Data_Info.VoiceNum = "";
            data.Data_Info.CntAddress = "";
            data.Data_Info.CntCode = "";
            data.Data_Info.ClassCode = "005";

            data.Sys_Info.CentralMer = "";
            data.Sys_Info.CoorFDKD = "002";
            data.Sys_Info.CoorRSID = "002";
            data.Sys_Info.EastFAL = "";
            data.Sys_Info.NorthFAL = "";

            data.Cont_Info.LayerName = "本数据包含村级区域、地块、点状地物、基本农田保护区、界址点、界址线、控制点、面状底物、区域界线、县级行政区、现状地物、注记等图层";
            data.Cont_Info.CatFetTyps = "";
            data.Cont_Info.AttrTypList = "";

            data.DQ_Info.DqStatement = "数据库已通过县级验收，数据质量良好，其中家庭成员表中某些身份证号码因历史原因不符合现行身份证号编制规则，已核对身份证原件，确认无误。";
            data.DQ_Info.DqLineage = "依据农村土地承包经营权相关行业标准，依托高分辨率数字正射影像来生产，最终建立符合标准规范的农村土地承包经营权数据库。";
            return data;
        }
    }
}
