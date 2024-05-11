/*
 * (C) 2015  鱼鳞图公司版权所有,保留所有权利 
 */
using System;
using System.Collections.Generic;
using YuLinTu.Data;
using YuLinTu.Library.Entity;

namespace YuLinTu.Library.Business
{
    /// <summary>
    /// 台账承包地块参数信息
    /// </summary>
    public class TaskGroupAccountFiveTableArgument:TaskArgument
    {
       #region Fields

        private Dictionary landName;
        private Dictionary landLevel;
        private Dictionary landPurpose;
        private Dictionary isFamer;
        private bool awareAreaEqualActual = true;
        private bool initialLandName;   //是否初始化地块类别
        private bool initialLandLevel;  //是否初始化地力等级
        private bool initialIsFamer;    //是否初始化基本农田
        private bool initialAwareArea;  //是否初始化确权面积等于
        private bool initialLandPurpose;  //是否初始化土地用途
        private bool initialLandNumber;   //是否初始化地块编码
        private bool handleContractLand;  //是否只处理承包地块
        private AgricultureLandExpand landExpand = null;
        private bool isCombination;  //地块编码是否按组合方式生成
        private bool isNew;   //地块编码是否按统一重新生成
        private bool initialMapNumber;     //是否初始化图幅编号
        private bool initialSurveyPerson;   //是否初始化调查员
        private bool initialSurveyDate;    //是否初始化调查日期
        private bool initialSurveyInfo;    //是否初始化调查记事
        private bool initialCheckPerson;   //是否初始化审核员
        private bool initialCheckDate;    //是否初始化审核日期
        private bool initialCheckInfo;    //是否初始化审核意见
        private bool initialReferPerson;   //是否初始化指界人
        private bool initialReferPersonByOwner;//以地块当前承包方为指界人

        #endregion

        #region Properties

        /// <summary>
        /// 数据库
        /// </summary>
        public IDbContext Database { get; set; }

        /// <summary>
        /// 当前地域
        /// </summary>
        public Zone CurrentZone { get; set; }

        /// <summary>
        /// 文件路径
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// 参数类型
        /// </summary>
        public eContractAccountType ArgType { get; set; }

        /// <summary>
        /// 使用者
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// 清空数据
        /// </summary>
        public bool IsClear { get; set; }

        /// <summary>
        /// 类型
        /// </summary>
        public eVirtualType VirtualType { get; set; }

        /// <summary>
        /// 承包台账地块数据业务
        /// </summary>
        public AccountLandBusiness LandBusiness { get; set; }

        /// <summary>
        /// 当前地域下的地块集合
        /// </summary>
        public List<ContractLand> CurrentZoneLandList { get; set; }

        /// <summary>
        /// 是否批量
        /// </summary>
        public bool IsBatch { get; set; }       

        #region Properties - 导入地块图斑

        /// <summary>
        /// 按照地块编码绑定导入-导入地块图斑设置
        /// </summary>
        public bool UseLandCodeBindImport { get; set; }

        /// <summary>
        /// 按照承包方信息绑定导入-导入地块图斑设置
        /// </summary>
        public bool UseContractorInfoImport { get; set; }

        /// <summary>
        /// 读取的shp所有字段名称
        /// </summary>
        public List<KeyValue<int, string>> shapeAllcolNameList { get; set; }

        #endregion

        #region Properties - 导入地块图斑

        /// <summary>
        /// 读取界址点所有字段名称
        /// </summary>
        public List<KeyValue<int, string>> DotAllcolNameList { get; set; }

        #endregion

        #region Properties - 初始地块属性信息

        /// <summary>
        /// 土地利用类型
        /// </summary>
        public Dictionary LandName
        {
            get { return landName; }
            set { landName = value; }
        }

        /// <summary>
        /// 土地等级
        /// </summary>
        public Dictionary LandLevel
        {
            get { return landLevel; }
            set { landLevel = value; }
        }

        /// <summary>
        /// 土地用途
        /// </summary>
        public Dictionary LandPurpose
        {
            get { return landPurpose; }
            set { landPurpose = value; }
        }

        /// <summary>
        /// 是否基本农田
        /// </summary>
        public Dictionary IsFamer
        {
            get { return isFamer; }
            set { isFamer = value; }
        }

        /// <summary>
        /// 确权面积等于
        /// </summary>
        public bool AwareAreaEqualActual
        {
            get { return awareAreaEqualActual; }
            set { awareAreaEqualActual = value; }
        }

        /// <summary>
        /// 是否初始化地块类别
        /// </summary>
        public bool InitialLandName
        {
            get { return initialLandName; }
            set { initialLandName = value; }
        }

        /// <summary>
        /// 是否初始化地力等级
        /// </summary>
        public bool InitialLandLevel
        {
            get { return initialLandLevel; }
            set { initialLandLevel = value; }
        }

        /// <summary>
        /// 是否初始化基本农田
        /// </summary>
        public bool InitialIsFamer
        {
            get { return initialIsFamer; }
            set { initialIsFamer = value; }
        }

        /// <summary>
        /// 是否初始化确权面积等于
        /// </summary>
        public bool InitialAwareArea
        {
            get { return initialAwareArea; }
            set { initialAwareArea = value; }
        }

        /// <summary>
        /// 是否初始化土地用途
        /// </summary>
        public bool InitialLandPurpose
        {
            get { return initialLandPurpose; }
            set { initialLandPurpose = value; }
        }

        /// <summary>
        /// 是否初始化地块编码
        /// </summary>
        public bool InitialLandNumber
        {
            get { return initialLandNumber; }
            set { initialLandNumber = value; }
        }

        /// <summary>
        /// 是否只处理承包地块
        /// </summary>
        public bool HandleContractLand
        {
            get { return handleContractLand; }
            set { handleContractLand = value; }
        }

        /// <summary>
        /// 地块编码是否按组合方式生成
        /// </summary>
        public bool IsCombination
        {
            get { return isCombination; }
            set { isCombination = value; }
        }

        /// <summary>
        /// 地块编码是否按统一重新生成
        /// </summary>
        public bool IsNew
        {
            get { return isNew; }
            set { isNew = value; }
        }

        /// <summary>
        /// 承包地块扩展信息
        /// </summary>
        public AgricultureLandExpand LandExpand
        {
            get { return landExpand; }
            set { landExpand = value; }
        }

        /// <summary>
        /// 是否初始化图幅编号
        /// </summary>
        public bool InitialMapNumber
        {
            get { return initialMapNumber; }
            set { initialMapNumber = value; }
        }

        /// <summary>
        /// 是否初始化调查员
        /// </summary>
        public bool InitialSurveyPerson
        {
            get { return initialSurveyPerson; }
            set { initialSurveyPerson = value; }
        }

        /// <summary>
        /// 是否初始化调查日期
        /// </summary>
        public bool InitialSurveyDate
        {
            get { return initialSurveyDate; }
            set { initialSurveyDate = value; }
        }

        /// <summary>
        /// 是否初始化调查记事
        /// </summary>
        public bool InitialSurveyInfo
        {
            get { return initialSurveyInfo; }
            set { initialSurveyInfo = value; }
        }

        /// <summary>
        /// 是否初始化审核员
        /// </summary>
        public bool InitialCheckPerson
        {
            get { return initialCheckPerson; }
            set { initialCheckPerson = value; }
        }

        /// <summary>
        /// 是否初始化审核日期
        /// </summary>
        public bool InitialCheckDate
        {
            get { return initialCheckDate; }
            set { initialCheckDate = value; }
        }

        /// <summary>
        /// 是否初始化审核意见
        /// </summary>
        public bool InitialCheckInfo
        {
            get { return initialCheckInfo; }
            set { initialCheckInfo = value; }
        }

        /// <summary>
        /// 是否初始化指界人
        /// </summary>
        public bool InitialReferPerson
        {
            get { return initialReferPerson; }
            set { initialReferPerson = value; }
        }


        /// <summary>
        /// 以地块当前承包方为指界人
        /// </summary>
        public bool InitialReferPersonByOwner
        {
            get { return initialReferPersonByOwner; }
            set { initialReferPersonByOwner = value; }
        }


        #endregion

        #region Properties - 初始地块实测面积和确权面积

        /// <summary>
        /// 是否平面面积到实测面积
        /// </summary>
        public bool ToActualArea { get; set; }

        /// <summary>
        /// 是否平面面积到确权面积
        /// </summary>
        public bool ToAwareArea { get; set; }

        #endregion

        #region Properties - 截取承包地块面积小数位

        /// <summary>
        /// 小数位数
        /// </summary>
        public int ToAreaNumeric { get; set; }

        /// <summary>
        /// 面积截取模式
        /// </summary>
        public int ToAreaModule { get; set; }

        /// <summary>
        /// 面积种类选择
        /// </summary>
        public int ToAreaSelect { get; set; }

        #endregion

        #region  Properties - 初始地块是否为基本农田

        /// <summary>
        /// Shape文件路径
        /// </summary>
        public string ShapeFileName { get; set; }

        #endregion

        /// <summary>
        /// 表格类型
        /// </summary>
        public int TableType { get; set; }

        /// <summary>
        /// 日期
        /// </summary>
        public DateTime? Date { get; set; }

        /// <summary>
        /// 当前选择的承包方
        /// </summary>
        public List<VirtualPerson> SelectContractor { get; set; }

      

        /// <summary>
        /// 数据字典
        /// </summary>
        public List<Dictionary> DictList{ get; set; }



        /// <summary>
        /// 本地域及以下集合
        /// </summary>
        public List<Zone> SelfAndSubsZones { get; set; }

        /// <summary>
        /// 包括镇、村、组地域集合
        /// </summary>
        public List<Zone> AllZones { get; set; }


        #endregion

        #region Ctor

        /// <summary>
        /// 构造函数
        /// </summary>
        public TaskGroupAccountFiveTableArgument()
        {
            VirtualType = eVirtualType.Land;
        }

        #endregion


    }
}
