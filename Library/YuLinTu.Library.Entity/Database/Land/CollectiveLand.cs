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
    /// <summary>
    /// 集体土地
    /// </summary>
    [DataTable("CollectiveLand")]
    [Serializable]
    public class CollectiveLand : CommonLand  
    {
        #region Fields

        private string _ZoneCode = string.Empty;
        private string _OwnUnitCode = string.Empty;

        /// <summary>
        /// 空间字段
        /// </summary>
        private Geometry shape;

        #endregion

        #region Properties

        /// <summary>
        ///
        /// </summary>
        [DataColumn("ID", ColumnType = eDataType.Guid)]
        public Guid ID
        {
            get;
            set;
        }

        /// <summary>
        ///上级主管部门
        /// </summary>
        [DataColumn("Administration", ColumnType = eDataType.String)]
        public string Administration { get; set; }

        ///// <summary>
        /////对应地域
        ///// </summary>
        //[DataColumn("ZoneCode", ColumnType = eDataType.String)]
        //public string ZoneCode
        //{
        //    get { return _ZoneCode; }
        //    set
        //    {
        //        _ZoneCode = value;
        //        if (!string.IsNullOrEmpty(_ZoneCode))
        //            _ZoneCode = _ZoneCode.Trim();
        //    }
        //}

        ///// <summary>
        /////地号
        ///// </summary>
        //[DataColumn("LandNumber", ColumnType = eDataType.String)]
        //public string LandNumber { get; set; }

        ///// <summary>
        /////宗地统一编号
        ///// </summary>
        //[DataColumn("ParcelNumber", ColumnType = eDataType.String)]
        //public string ParcelNumber { get; set; }

        /// <summary>
        ///图号
        /// </summary>
        [DataColumn("ImageNumber", ColumnType = eDataType.String)]
        public string ImageNumber { get; set; }

        /// <summary>
        ///承包方ID
        /// </summary>
        [DataColumn("OwnUnitID", ColumnType = eDataType.Guid)]
        public Guid OwnUnitID { get; set; }

        /// <summary>
        ///总面积
        /// </summary>
        [DataColumn("Area", ColumnType = eDataType.Decimal)]
        public double? Area { get; set; }

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

        ///// <summary>
        /////备注
        ///// </summary>
        //[DataColumn("Comment", ColumnType = eDataType.String)]
        //public string Comment { get; set; }

        ///// <summary>
        /////权属单位代码
        ///// </summary>
        //[DataColumn("ZoneCode", ColumnType = eDataType.String)]
        //public string ZoneCode
        //{
        //    get { return _OwnUnitCode; }
        //    set
        //    {
        //        _OwnUnitCode = value;
        //        if (!string.IsNullOrEmpty(_OwnUnitCode))
        //            _OwnUnitCode = _OwnUnitCode.Trim();
        //    }
        //}

        /// <summary>
        ///地籍区座落代码
        /// </summary>
        [DataColumn("CadastralZoneCode", ColumnType = eDataType.String)]
        public string CadastralZoneCode
        {
            get;
            set;
        }

        /// <summary>
        ///土地所有权人
        /// </summary>
        [DataColumn("OwnUnitName", ColumnType = eDataType.String)]
        public string OwnUnitName { get; set; }

        /// <summary>
        ///座落
        /// </summary>
        [DataColumn("LandLocated", ColumnType = eDataType.String)]
        public string LandLocated { get; set; }

        /// <summary>
        ///耕地面积
        /// </summary>
        [DataColumn("InfieldArea", ColumnType = eDataType.Decimal)]
        public double InfieldArea { get; set; }

        /// <summary>
        ///园地面积
        /// </summary>
        [DataColumn("GardenPlotArea", ColumnType = eDataType.Decimal)]
        public double GardenPlotArea { get; set; }

        /// <summary>
        ///林地面积
        /// </summary>
        [DataColumn("WoodLandArea", ColumnType = eDataType.Decimal)]
        public double WoodLandArea { get; set; }

        /// <summary>
        ///牧草地面积
        /// </summary>
        [DataColumn("GrassLand", ColumnType = eDataType.Decimal)]
        public double GrassLand { get; set; }

        /// <summary>
        ///其他未利用地面积
        /// </summary>
        [DataColumn("OtherUndueArea", ColumnType = eDataType.Decimal)]
        public double OtherUndueArea { get; set; }

        /// <summary>
        ///建设用地面积
        /// </summary>
        [DataColumn("BuildLandArea", ColumnType = eDataType.Decimal)]
        public double BuildLandArea { get; set; }

        /// <summary>
        ///未利用地面积
        /// </summary>
        [DataColumn("UndueArea", ColumnType = eDataType.Decimal)]
        public double UndueArea { get; set; }

        /// <summary>
        ///四至
        /// </summary>
        [DataColumn("LandNeighbor", ColumnType = eDataType.String)]
        public string LandNeighbor { get; set; }

        /// <summary>
        ///土地所有权人性质
        /// </summary>
        [DataColumn("LandUsePersonType", ColumnType = eDataType.Int32)]
        public eLandUsePersonType LandUsePersonType { get; set; }

        /// <summary>
        ///法人代表姓名
        /// </summary>
        [DataColumn("RepresentName", ColumnType = eDataType.String)]
        public string RepresentName { get; set; }

        /// <summary>
        ///法人代表证件类型
        /// </summary>
        [DataColumn("RepresentType", ColumnType = eDataType.Int32)]
        public eCredentialsType RepresentType { get; set; }

        /// <summary>
        ///法人代表证件号
        /// </summary>
        [DataColumn("RepresentNumber", ColumnType = eDataType.String)]
        public string RepresentNumber { get; set; }

        /// <summary>
        ///法人代表电话号码
        /// </summary>
        [DataColumn("RepresentTelphone", ColumnType = eDataType.String)]
        public string RepresentTelphone { get; set; }

        /// <summary>
        ///代理人姓名
        /// </summary>
        [DataColumn("AgentName", ColumnType = eDataType.String)]
        public string AgentName { get; set; }

        /// <summary>
        ///代理人证件类型
        /// </summary>
        [DataColumn("AgentCrdentialType", ColumnType = eDataType.Int32)]
        public eCredentialsType AgentCrdentialType { get; set; }

        /// <summary>
        ///代理人证件号
        /// </summary>
        [DataColumn("AgentCrdentialNumber", ColumnType = eDataType.String)]
        public string AgentCrdentialNumber { get; set; }

        /// <summary>
        ///代理人电话号码
        /// </summary>
        [DataColumn("AgentTelphone", ColumnType = eDataType.String)]
        public string AgentTelphone { get; set; }

        /// <summary>
        ///土地权属性质
        /// </summary>
        [DataColumn("OwnRightType", ColumnType = eDataType.Int32)]
        public eLandPropertyType OwnRightType { get; set; }

        /// <summary>
        ///农用地面积
        /// </summary>
        [DataColumn("FarmArea", ColumnType = eDataType.Decimal)]
        public double FarmArea { get; set; }

        /// <summary>
        ///重要界址点点位说明
        /// </summary>
        [DataColumn("EmphasisDotComment", ColumnType = eDataType.String)]
        public string EmphasisDotComment { get; set; }

        /// <summary>
        ///主要权属界线走向说明
        /// </summary>
        [DataColumn("MainCoilComment", ColumnType = eDataType.String)]
        public string MainCoilComment { get; set; }

        /// <summary>
        ///宗地被线状国有或其它农民集体土地分割的说明
        /// </summary>
        [DataColumn("DivisionComment", ColumnType = eDataType.String)]
        public string DivisionComment { get; set; }

        /// <summary>
        ///其他说明
        /// </summary>
        [DataColumn("OtherComment", ColumnType = eDataType.String)]
        public string OtherComment { get; set; }

        /// <summary>
        ///土地权属界址调查记事及调查员意见
        /// </summary>
        [DataColumn("InvestigateComment", ColumnType = eDataType.String)]
        public string InvestigateComment { get; set; }

        /// <summary>
        ///调查人员
        /// </summary>
        [DataColumn("InvestigatePerson", ColumnType = eDataType.String)]
        public string InvestigatePerson { get; set; }

        /// <summary>
        ///堪丈日期
        /// </summary>
        [DataColumn("InvestigateDate", ColumnType = eDataType.DateTime)]
        public DateTime? InvestigateDate { get; set; }

        /// <summary>
        ///状态 登记10、审核20、核发权证30
        /// </summary>
        [DataColumn("Status", ColumnType = eDataType.Int32)]
        
        public int Status { get; set; }
        /// <summary>
        /// 空间字段
        /// </summary>
        public Geometry Shape
        {
            get { return shape; }
            set
            {
                shape = value;
                NotifyPropertyChanged("Shape");
            }
        }

        #endregion

        #region Ctor

        static CollectiveLand()
        {
            LanguageAttribute.AddLanguage(Properties.Resources.langChs_eLandPropertyType);
            LanguageAttribute.AddLanguage(Properties.Resources.langChs_eCredentialsType);
        }

        public CollectiveLand()
        {
            CreationTime = DateTime.Now;
            ModifiedTime = DateTime.Now;
            InvestigateDate = DateTime.Now;
        }

        #endregion

        #region Methods

        public string[] GetLandNeighbor()
        {
            if (LandNeighbor == null || LandNeighbor.Length < 4)
                return new string[] { "", "", "", "" };

            string[] neighbors = LandNeighbor.Split(new char[] { '\r' });
            if (neighbors == null || neighbors.Length != 4)
                return new string[] { "", "", "", "" };

            return neighbors;
        }

        #endregion
    }
}