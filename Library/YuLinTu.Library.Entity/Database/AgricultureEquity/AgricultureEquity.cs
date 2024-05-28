// (C) 2015 鱼鳞图公司版权所有，保留所有权利
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YuLinTu;
using YuLinTu.Data;
using YuLinTu.Spatial;

namespace YuLinTu.Library.Entity
{
    [DataTable("AgricultureEquity")]
    [Serializable]
    public class AgricultureEquity : NameableObject  
    {
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
                if (string.IsNullOrEmpty(_CadastralNumber))
                    return;
                _CadastralNumber = _CadastralNumber.Trim();
            }
        }

        /// <summary>
        ///地块名称
        /// </summary>
        [DataColumn("Name", ColumnType = eDataType.String)]
        public new string Name
        {
            get;
            set;
        }

        /// <summary>
        ///地块编号
        /// </summary>
        [DataColumn("LandNumber", ColumnType = eDataType.String)]
        public string LandNumber
        {
            get { return _LandNumber; }
            set
            {
                _LandNumber = value;
                if (string.IsNullOrEmpty(_LandNumber))
                    return;
                _LandNumber = _LandNumber.Trim();
            }
        }

        /// <summary>
        ///地类编码
        /// </summary>
        [DataColumn("LandCode", ColumnType = eDataType.String)]
        public string LandCode
        {
            get { return _landCode; }
            set
            {
                _landCode = value;
                if (string.IsNullOrEmpty(_landCode))
                    return;
                _landCode = _landCode.Trim();
            }
        }

        /// <summary>
        ///地类名称
        /// </summary>
        [DataColumn("LandName", ColumnType = eDataType.String)]
        public string LandName
        {
            get;
            set;
        }

        /// <summary>
        ///土地等级
        /// </summary>
        [DataColumn("LandLevel", ColumnType = eDataType.Int32)]
        public eContractLandLevel LandLevel { get; set; }

        /// <summary>
        ///四至
        /// </summary>
        [DataColumn("LandNeighbor", ColumnType = eDataType.String)]
        public string LandNeighbor { get; set; }

        /// <summary>
        ///权属单位代码
        /// </summary>
        [DataColumn("OwnRightCode", ColumnType = eDataType.String)]
        public string OwnRightCode
        {
            get { return _OwnRightCode; }
            set
            {
                _OwnRightCode = value;
                if (string.IsNullOrEmpty(_OwnRightCode))
                    return;
                _OwnRightCode = _OwnRightCode.Trim();
            }
        }

        /// <summary>
        ///权属单位名称
        /// </summary>
        [DataColumn("OwnRightName", ColumnType = eDataType.String)]
        public string OwnRightName { get; set; }

        /// <summary>
        ///座落单位代码
        /// </summary>
        [DataColumn("ZoneCode", ColumnType = eDataType.String)]
        public string LocationCode
        {
            get { return _LocationCode; }
            set
            {
                _LocationCode = value;
                if (string.IsNullOrEmpty(_LocationCode))
                    return;
                _LocationCode = _LocationCode.Trim();
            }
        }

        /// <summary>
        ///座落单位名称
        /// </summary>
        [DataColumn("ZoneName", ColumnType = eDataType.String)]
        public string LocationName { get; set; }

        /// <summary>
        ///二轮台账面积
        /// </summary>
        [DataColumn("TableArea", ColumnType = eDataType.Decimal)]
        public double? TableArea { get; set; }

        /// <summary>
        ///实测面积
        /// </summary>
        [DataColumn("ActualArea", ColumnType = eDataType.Decimal)]
        public double ActualArea { get; set; }

        /// <summary>
        ///发证面积
        /// </summary>
        [DataColumn("AwareArea", ColumnType = eDataType.Decimal)]
        public double AwareArea { get; set; }

        /// <summary>
        /// 占有股份数
        /// </summary>
        [DataColumn("EquityNumber", ColumnType = eDataType.Decimal)]
        public double EquityNumber { get; set; }

        /// <summary>
        ///是否基本农田
        /// </summary>
        [DataColumn("IsFarmerLand", ColumnType = eDataType.Boolean)]
        public bool? IsFarmerLand { get; set; }

        /// <summary>
        ///土地用途
        /// </summary>
        [DataColumn("Purpose", ColumnType = eDataType.Int32)]
        public eLandPurposeType Purpose { get; set; }

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
        ///备用字段A 已用于
        ///1、宗地座落方位描述
        /// </summary>
        [DataColumn("ExtendA", ColumnType = eDataType.String)]
        public string ExtendA { get; set; }

        /// <summary>
        ///备用字段B(宗地统一编码)
        /// </summary>
        [DataColumn("ExtendB", ColumnType = eDataType.String)]
        public string ExtendB { get; set; }

        /// <summary>
        ///备用字段C
        /// </summary>
        [DataColumn("ExtendC", ColumnType = eDataType.String)]
        public string ExtendC { get; set; }

        /// <summary>
        ///扩展信息
        /// </summary>
        [DataColumn("ExtendInformation", ColumnType = eDataType.String)]
        public string ExtendInformation { get; set; }

        #endregion

        #region Fields

        private string _landCode;
        private string _CadastralNumber;
        private string _LandNumber;
        private string _OwnRightCode;
        private string _LocationCode;

        #endregion

        #region Ctor

        public AgricultureEquity()
        {
            CreationTime = DateTime.Now;
            ModifiedTime = DateTime.Now;
            Modifier = "Admin";
            Founder = "Admin";
            LandLevel = eContractLandLevel.UnKnow;
            Purpose = eLandPurposeType.Planting;
            LandCode = "XX";
            ActualArea = 0.0;
            AwareArea = 0.0;
            TableArea = 0.0;
        }

        #endregion
    }
}