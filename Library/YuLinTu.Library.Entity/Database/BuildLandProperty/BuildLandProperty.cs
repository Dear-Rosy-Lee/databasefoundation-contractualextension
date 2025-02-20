// (C) 2025 鱼鳞图公司版权所有，保留所有权利
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YuLinTu.Data;
using System.Text.RegularExpressions;
using YuLinTu;

namespace YuLinTu.Library.Entity
{
    /// <summary>
    /// 集体建设用地使用权
    /// </summary>
    [DataTable("BuildLandProperty")]
    [Serializable]
    public class BuildLandProperty : NameableObject 
    {
        #region Ctor

        static BuildLandProperty()
        {
            LanguageAttribute.AddLanguage(Properties.Resources.langChs_eLandPropertyType);
            LanguageAttribute.AddLanguage(Properties.Resources.langChs_eLandUseRightType);
            LanguageAttribute.AddLanguage(Properties.Resources.langChs_eLandLevel);
        }

        #endregion

        #region Fields

        private string _CadastralNumber = string.Empty;
        private string _LandLocatedCode = string.Empty;
        private string _OwnUnitCode = string.Empty;

        #endregion

        #region Properties

        /// <summary>
        ///标识码
        /// </summary>
        [DataColumn("ID", ColumnType = eDataType.Guid)]
        public Guid ID
        {
            get;
            set;
        }

        /// <summary>
        ///地籍号
        /// </summary>
        [DataColumn("CadastralNumber", ColumnType = eDataType.String)]
        public string CadastralNumber
        {
            get { return _CadastralNumber; }
            set
            {
                _CadastralNumber = value;
                if (!string.IsNullOrEmpty(_CadastralNumber))
                    _CadastralNumber = _CadastralNumber.Trim();
            }
        }

        /// <summary>
        ///承包方名称
        /// </summary>
        [DataColumn("HouseHolderName", ColumnType = eDataType.String)]
        public string HouseHolderName { get; set; }

        /// <summary>
        ///承包方ID
        /// </summary>
        [DataColumn("HouseHolderID", ColumnType = eDataType.Guid)]
        public Guid? HouseHolderID { get; set; }

        /// <summary>
        ///宗地号
        /// </summary>
        [DataColumn("LandNumber", ColumnType = eDataType.String)]
        public string LandNumber { get; set; }

        /// <summary>
        ///宗地四至
        /// </summary>
        [DataColumn("LandNeighbor", ColumnType = eDataType.String)]
        public string LandNeighbor { get; set; }

        /// <summary>
        ///通讯地址
        /// </summary>
        [DataColumn("Address", ColumnType = eDataType.String)]
        public string Address { get; set; }

        /// <summary>
        ///土地座落
        /// </summary>
        [DataColumn("LandLocated", ColumnType = eDataType.String)]
        public string LandLocated { get; set; }

        /// <summary>
        ///土地座落代码
        /// </summary>
        [DataColumn("LandLocatedCode", ColumnType = eDataType.String)]
        public string LandLocatedCode
        {
            get { return _LandLocatedCode; }
            set
            {
                _LandLocatedCode = value;
                if (!string.IsNullOrEmpty(_LandLocatedCode))
                    _LandLocatedCode = _LandLocatedCode.Trim();
            }
        }

        /// <summary>
        ///权属单位名称
        /// </summary>
        [DataColumn("OwnUnitName", ColumnType = eDataType.String)]
        public string OwnUnitName { get; set; }

        /// <summary>
        ///权属单位代码
        /// </summary>
        [DataColumn("ZoneCode", ColumnType = eDataType.String)]
        public string ZoneCode
        {
            get { return _OwnUnitCode; }
            set
            {
                _OwnUnitCode = value;
                if (!string.IsNullOrEmpty(_OwnUnitCode))
                    _OwnUnitCode = _OwnUnitCode.Trim();
            }
        }

        /// <summary>
        ///权属性质
        /// </summary>
        [DataColumn("OwnRightType", ColumnType = eDataType.Int32)]
        public eLandPropertyType OwnRightType { get; set; }

        /// <summary>
        ///使用权类型
        /// </summary>
        [DataColumn("UseRightType", ColumnType = eDataType.Int32)]
        public LandUseRightType UseRightType { get; set; }

        /// <summary>
        ///土地用途
        /// </summary>
        [DataColumn("LandPurpose", ColumnType = eDataType.String)]
        public string LandPurpose { get; set; }

        /// <summary>
        ///实测面积，已废弃
        ///请使用 SelfArea
        /// </summary>
        [DataColumn("ActualArea", ColumnType = eDataType.Decimal)]
        public double ActualArea { get; set; }

        /// <summary>
        ///发证面积，已废弃
        ///请使用PublicArea
        /// </summary>
        [DataColumn("AwareArea", ColumnType = eDataType.Decimal)]
        public double? AwareArea { get; set; }

        /// <summary>
        ///合计面积
        /// </summary>
        [DataColumn("CountArea", ColumnType = eDataType.Decimal)]
        public double? CountArea { get; set; }

        /// <summary>
        ///批准农村宅基地面积
        /// </summary>
        [DataColumn("ApproveLandArea", ColumnType = eDataType.Decimal)]
        public double? ApproveLandArea { get; set; }

        /// <summary>
        ///建房日期
        /// </summary>
        [DataColumn("BuildHouseDate", ColumnType = eDataType.DateTime)]
        public DateTime? BuildHouseDate { get; set; }

        /// <summary>
        ///建筑容积率
        /// </summary>
        [DataColumn("FrameCapacityRatio", ColumnType = eDataType.Decimal)]
        public double? FrameCapacityRatio { get; set; }

        /// <summary>
        ///建筑密度
        /// </summary>
        [DataColumn("FrameDensity", ColumnType = eDataType.Decimal)]
        public double? FrameDensity { get; set; }

        /// <summary>
        ///土地级别
        /// </summary>
        [DataColumn("LandLevel", ColumnType = eDataType.Int32)]
        public eLandLevel LandLevel { get; set; }

        /// <summary>
        ///申报地价
        /// </summary>
        [DataColumn("DeclareLandPrice", ColumnType = eDataType.Decimal)]
        public double? DeclareLandPrice { get; set; }

        /// <summary>
        ///取得价格
        /// </summary>
        [DataColumn("RealityPrice", ColumnType = eDataType.Decimal)]
        public double? RealityPrice { get; set; }

        /// <summary>
        ///集体建设用地使用权附件路径
        /// </summary>
        [DataColumn("Path", ColumnType = eDataType.String)]
        public string Path { get; set; }

        /// <summary>
        ///开始审核
        /// </summary>
        [DataColumn("BeginCheck", ColumnType = eDataType.Boolean)]
        public bool BeginCheck { get; set; }

        /// <summary>
        ///审核完成
        /// </summary>
        [DataColumn("EndCheck", ColumnType = eDataType.Boolean)]
        public bool EndCheck { get; set; }

        /// <summary>
        ///是否打证
        /// </summary>
        [DataColumn("IsPrint", ColumnType = eDataType.Boolean)]
        public bool IsPrint { get; set; }

        /// <summary>
        ///状态 登记10、审核20、核发权证30
        /// </summary>
        [DataColumn("Status", ColumnType = eDataType.Int32)]
        public int Status { get; set; }

        /// <summary>
        ///扩展属性A 备用
        ///疑似B.S中已使用
        /// </summary>
        [DataColumn("ExtendA", ColumnType = eDataType.String)]
        public string ExtendA { get; set; }

        /// <summary>
        ///扩展属性B 备用
        ///目前已用户存储 集体建设用地使用权确权审批表编号。
        /// </summary>
        [DataColumn("ExtendB", ColumnType = eDataType.String)]
        public string ExtendB { get; set; }

        /// <summary>
        ///扩展属性C 备用
        ///目前已用户存储 集体建设用地使用权确权审批表年号。
        /// </summary>
        [DataColumn("ExtendC", ColumnType = eDataType.String)]
        public string ExtendC { get; set; }

        /// <summary>
        ///创建者
        /// </summary>
        [DataColumn("Founder", ColumnType = eDataType.String)]
        public string Founder { get; set; }

        /// <summary>
        ///创建时间
        /// </summary>
        [DataColumn("CreationTime", ColumnType = eDataType.DateTime)]
        public DateTime? CreationTime { get; set; }

        /// <summary>
        ///最后修改者
        /// </summary>
        [DataColumn("Modifier", ColumnType = eDataType.String)]
        public string Modifier { get; set; }

        /// <summary>
        ///最后修改时间
        /// </summary>
        [DataColumn("ModifiedTime", ColumnType = eDataType.DateTime)]
        public DateTime? ModifiedTime { get; set; }

        /// <summary>
        ///备注
        /// </summary>
        [DataColumn("Comment", ColumnType = eDataType.String)]
        public string Comment { get; set; }

        /// <summary>
        ///实测面积
        /// </summary>
        [DataColumn("SelfArea", ColumnType = eDataType.Decimal)]
        public double SelfArea { get; set; }

        /// <summary>
        ///确权面积
        /// </summary>
        [DataColumn("PublicArea", ColumnType = eDataType.Decimal)]
        public double PublicArea { get; set; }

        /// <summary>
        ///违法用地面积
        /// </summary>
        [DataColumn("IllegalArea", ColumnType = eDataType.Decimal)]
        public double IllegalArea { get; set; }

        /// <summary>
        ///超占面积
        /// </summary>
        [DataColumn("ExceedArea", ColumnType = eDataType.Decimal)]
        public double ExceedArea { get; set; }

        /// <summary>
        ///地类名称
        /// </summary>
        [DataColumn("LandTypeName", ColumnType = eDataType.String)]
        public string LandTypeName { get; set; }

        /// <summary>
        ///是否可用
        /// </summary>
        [DataColumn("IsValid", ColumnType = eDataType.Boolean)]
        public bool IsValid { get; set; }

        /// <summary>
        /// 地籍区编号
        /// </summary>
        [DataColumn("CadastralZoneCode", ColumnType = eDataType.String)]
        public string CadastralZoneCode { get; set; }

        #endregion

        #region Ctor

        public BuildLandProperty()
        {
            CreationTime = DateTime.Now;
            ModifiedTime = DateTime.Now;
            OwnRightType = eLandPropertyType.Collectived;
            UseRightType = LandUseRightType.AssignHouse;
            LandPurpose = "072";
            LandLevel = eLandLevel.UnKnow;
            BuildHouseDate = DateTime.Now;
            LandTypeName = "农村宅基地";
        }

        #endregion

        #region Methods

        public string[] GetLandNeighbors()
        {
            if (LandNeighbor == null || LandNeighbor.Length < 4)
                return new string[] { "", "", "", "" };

            string[] neighbors = LandNeighbor.Split(new char[] { '\r' });
            if (neighbors == null || neighbors.Length != 4)
            {
                neighbors = LandNeighbor.Split(new char[] { '\n' });//xml中会变成\n
                if (neighbors == null || neighbors.Length != 4)
                    return new string[] { "", "", "", "" };
            }

            return neighbors;
        }

        #endregion
    }
}