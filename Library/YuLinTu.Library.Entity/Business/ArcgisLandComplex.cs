using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using YuLinTu.Data;

namespace YuLinTu.Library.Entity
{
    /// <summary>
    /// 空间农用地数据
    /// </summary>
    [DataTable(ARCGISLANDCOMPLEXSTRING)]
    [XmlRoot(ARCGISLANDCOMPLEXSTRING)]
    public class ArcGisLandComplex
    {
        #region ConstName

        public const string ARCGISLANDCOMPLEXSTRING = "ArcGisLandComplex";//农用地符合实体

        #endregion

        #region Propertys

        /// <summary>
        /// ObjectId
        /// </summary>
        public long OBJECTID { get; set; }

        /// <summary>
        /// 承包地
        /// </summary>
        public ContractLand ContractorLand { get; set; }

        /// <summary>
        /// 地块扩展
        /// </summary>
        public AgricultureLandExpand Expand { get; set; }

        /// <summary>
        /// 身份证号码
        /// </summary>
        public string IcnNumber { get; set; }

        /// <summary>
        /// 是否地块名称
        /// </summary>
        public bool IsName { get; set; }

        /// <summary>
        /// 是否承包方
        /// </summary>
        public bool IsContractor { get; set; }

        /// <summary>
        /// 是否地类编码
        /// </summary>
        public bool IsLandCode { get; set; }

        /// <summary>
        /// 是否地块名称
        /// </summary>
        public bool IsLandName { get; set; }

        /// <summary>
        /// 是否耕保类型
        /// </summary>
        public bool IsPlantType { get; set; }

        /// <summary>
        /// 是否实测面积
        /// </summary>
        public bool IsActualArea { get; set; }

        /// <summary>
        /// 是否确权面积
        /// </summary>
        public bool IsAwareArea { get; set; }

        /// <summary>
        /// 是否台帐面积
        /// </summary>
        public bool IsTableArea { get; set; }

        /// <summary>
        /// 是否机动地面积
        /// </summary>
        public bool IsMotorizeLandArea { get; set; }

        /// <summary>
        /// 是否线状面积
        /// </summary>
        public bool IsLineArea { get; set; }

        /// <summary>
        /// 是否畦数
        /// </summary>
        public bool IsPlotNumber { get; set; }

        /// <summary>
        /// 是否承包类型
        /// </summary>
        public bool IsContractType { get; set; }

        /// <summary>
        /// 是否承包方式
        /// </summary>
        public bool IsContractMode { get; set; }

        /// <summary>
        /// 是否种植类型
        /// </summary>
        public bool IsPlatType { get; set; }

        /// <summary>
        /// 是否飞地
        /// </summary>
        public bool IsFlyLand { get; set; }

        /// <summary>
        /// 是否基本农田
        /// </summary>
        public bool IsFarmerLand { get; set; }

        /// <summary>
        /// 是否土地用途
        /// </summary>
        public bool IsPurpose { get; set; }

        /// <summary>
        /// 是否土地等级
        /// </summary>
        public bool IsLandLevel { get; set; }

        /// <summary>
        /// 经营方式
        /// </summary>
        public bool IsManagerType { get; set; }

        /// <summary>
        /// 是否流转
        /// </summary>
        public bool IsTransfer { get; set; }

        /// <summary>
        /// 是否流转方式
        /// </summary>
        public bool IsTransferType { get; set; }

        /// <summary>
        /// 是否流转期限
        /// </summary>
        public bool IsTransferTime { get; set; }

        /// <summary>
        /// 是否流转面积
        /// </summary>
        public bool IsTransferArea { get; set; }

        /// <summary>
        /// 是否备注
        /// </summary>
        public bool IsComment { get; set; }

        /// <summary>
        /// 是否坐落代码
        /// </summary>
        public bool IsLocationCode { get; set; }

        /// <summary>
        /// 是否地籍区代码
        /// </summary>
        public bool IsCadaCode { get; set; }

        /// <summary>
        /// 是否四至
        /// </summary>
        public bool IsNeighbor { get; set; }

        /// <summary>
        /// 是否高程
        /// </summary>
        public bool IsExpand { get; set; }

        /// <summary>
        /// 界址点集合
        /// </summary>
        public List<BuildLandBoundaryAddressDot> DotCollection { get; set; }

        /// <summary>
        /// 界址线集合
        /// </summary>
        public List<BuildLandBoundaryAddressCoil> LineCollection { get; set; }

        #endregion

        #region Constructor

        /// <summary>
        /// 默认构造方法
        /// </summary>
        public ArcGisLandComplex()
        {
            ContractorLand = new ContractLand();
            Expand = new AgricultureLandExpand();
        }

        #endregion

        #region Methods

        #region SourceList

        /// <summary>
        /// 列表
        /// </summary>
        /// <returns></returns>
        public static ArrayList LandList()
        {
            ArrayList list = new ArrayList();
            list.Add("承包方名称");
            list.Add("证件号码");
            list.Add("地块名称");
            list.Add("地块编码");
            list.Add("实测面积");
            list.Add("确权面积");
            list.Add("台账面积");
            list.Add("利用类型");
            list.Add("地类名称");
            list.Add("基本农田");
            list.Add("地块类别");
            list.Add("地力等级");
            list.Add("承包方式");
            list.Add("四至东");
            list.Add("四至南");
            list.Add("四至西");
            list.Add("四至北");
            list.Add("指界人");
            list.Add("是否飞地");
            list.Add("是否流转");
            list.Add("流转方式");
            list.Add("流转期限");
            list.Add("种植类型");
            list.Add("土地用途");
            list.Add("图幅编号");
            list.Add("海拔高度");
            list.Add("调查员");
            list.Add("调查日期");
            list.Add("调查记事");
            list.Add("审核人");
            list.Add("审核日期");
            list.Add("审核意见");
            list.Add("备注");
            return list;
        }

        #endregion

        #region Mapping

        #endregion

        #endregion
    }
	
}
