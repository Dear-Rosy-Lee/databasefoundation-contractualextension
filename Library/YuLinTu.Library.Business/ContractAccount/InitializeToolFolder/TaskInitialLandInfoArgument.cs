/*
 * (C) 2025  鱼鳞图公司版权所有,保留所有权利 
 */
using System.Collections.Generic;
using YuLinTu.Data;
using YuLinTu.Library.Entity;

namespace YuLinTu.Library.Business
{
    /// <summary>
    /// 初始化地块基本信息任务参数
    /// </summary>
    public class TaskInitialLandInfoArgument : TaskArgument
    {
        #region Properties - 初始地块属性信息

        /// <summary>
        /// 数据库
        /// </summary>
        public IDbContext Database { get; set; }

        /// <summary>
        /// 当前地域
        /// </summary>
        public Zone CurrentZone { get; set; }

        /// <summary>
        /// 类型
        /// </summary>
        public eVirtualType VirtualType { get; set; }

        /// <summary>
        /// 土地利用类型
        /// </summary>
        public Dictionary LandName { get; set; }

        /// <summary>
        /// 土地等级
        /// </summary>
        public Dictionary LandLevel { get; set; }

        /// <summary>
        /// 土地用途
        /// </summary>
        public Dictionary LandPurpose { get; set; }

        /// <summary>
        /// 是否基本农田
        /// </summary>
        public bool? IsFamer { get; set; }

        /// <summary>
        /// 确权面积等于
        /// </summary>
        public bool AwareAreaEqualActual { get; set; }

        /// <summary>
        /// 是否初始化地块类别
        /// </summary>
        public bool InitialLandName { get; set; }
        /// <summary>
        /// 所有权性质
        /// </summary>
        public Dictionary QSXZ { get; set; }
        /// <summary>
        /// 是否初始化所有权性质
        /// </summary>
        public bool InitialQSXZ { get; set; }

        /// <summary>
        /// 是否初始化地力等级
        /// </summary>
        public bool InitialLandLevel { get; set; }

        /// <summary>
        /// 是否初始化基本农田
        /// </summary>
        public bool InitialIsFamer { get; set; }

        /// <summary>
        /// 是否初始化确权面积等于
        /// </summary>
        public bool InitialAwareArea { get; set; }

        /// <summary>
        /// 是否初始化土地用途
        /// </summary>
        public bool InitialLandPurpose { get; set; }

        /// <summary>
        /// 是否初始化地块编码
        /// </summary>
        public bool InitialLandNumber { get; set; }

        /// <summary>
        /// 是否初始化确权地块编码
        /// </summary>
        public bool InitialLandOldNumber { get; set; }

        /// <summary>
        /// 是否初始化地块编码-从上往下
        /// </summary>
        public bool InitialLandNumberByUpDown { get; set; }

        /// <summary>
        /// 是否只处理承包地块
        /// </summary>
        public bool HandleContractLand { get; set; }

        /// <summary>
        /// 地块编码是否按组合方式生成
        /// </summary>
        public bool IsCombination { get; set; }

        /// <summary>
        /// 地块编码是否按统一重新生成
        /// </summary>
        public bool IsNew { get; set; }

        /// <summary>
        /// 地块编码是否重新生成不匹配部分
        /// </summary>
        public bool IsNewPart { get; set; }

        /// <summary>
        /// 承包地块扩展信息
        /// </summary>
        public AgricultureLandExpand LandExpand { get; set; }

        /// <summary>
        /// 是否初始化图幅编号
        /// </summary>
        public bool InitialMapNumber { get; set; }

        /// <summary>
        /// 是否初始化调查员
        /// </summary>
        public bool InitialSurveyPerson { get; set; }

        /// <summary>
        /// 是否初始化调查日期
        /// </summary>
        public bool InitialSurveyDate { get; set; }

        /// <summary>
        /// 是否初始化调查记事
        /// </summary>
        public bool InitialSurveyInfo { get; set; }

        /// <summary>
        /// 是否初始化审核员
        /// </summary>
        public bool InitialCheckPerson { get; set; }

        /// <summary>
        /// 是否初始化审核日期
        /// </summary>
        public bool InitialCheckDate { get; set; }

        /// <summary>
        /// 是否初始化审核意见
        /// </summary>
        public bool InitialCheckInfo { get; set; }

        /// <summary>
        /// 是否初始化指界人
        /// </summary>
        public bool InitialReferPerson { get; set; }

        /// <summary>
        /// 以地块当前承包方为指界人
        /// </summary>
        public bool InitialReferPersonByOwner { get; set; }

        /// <summary>
        /// 按村级进行户号、地块编码统一初始及签订
        /// </summary>
        public bool VillageInlitialSet { get; set; }

        /// <summary>
        /// 是否获取(前的四至信息
        /// </summary>
        public bool InitialLandNeighbor { get; set; }

        /// <summary>
        /// 是否地块周边地块信息
        /// </summary>
        public bool InitialLandNeighborInfo { get; set; }

        /// <summary>
        /// 按村级进行户号、地块编码统一初始及签订记录编号
        /// </summary>
        public int[] CombinationLandNumber { get; set; }

        /// <summary>
        /// 当前地域下的地块集合
        /// </summary>
        public List<ContractLand> CurrentZoneLandList { get; set; }
        /// <summary>
        /// 是否只初始化空项
        /// </summary>
        public bool InitialNull;

        /// <summary>
        /// 是否初始化地块备注
        /// </summary>
        public bool InitLandComment
        {
            get;
            set;
        }
        /// <summary>
        /// 地块备注
        /// </summary>
        public string LandComment { get; set; }

        /// <summary>
        /// 初始化起始编号
        /// </summary>
        public int InitiallStartNum { get; set; }

        #endregion

        #region Ctor

        /// <summary>
        /// 构造函数
        /// </summary>
        public TaskInitialLandInfoArgument()
        {
            VirtualType = eVirtualType.Land;
        }

        #endregion
    }
}
