/*
 * (C) 2025  鱼鳞图公司版权所有,保留所有权利 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YuLinTu.Data;
using YuLinTu.Library.Aux;
using YuLinTu.Library.Entity;

namespace YuLinTu.Library.Business
{
    /// <summary>
    /// 初始化界址点线任务参数
    /// </summary>
    public class TaskInitializeLandDotCoilArgument : TaskArgument
    {
        #region Fields

        #endregion

        #region Properties

        /// <summary>
        /// 数据库
        /// </summary>
        public IDbContext Database { get; set; }

        /// <summary>
        /// 数据库
        /// </summary>
        public IDbContext TempDatabase { get; set; }

        /// <summary>
        /// 当前地域
        /// </summary>
        public Zone CurrentZone { get; set; }

        /// <summary>
        /// 是否初始化
        /// </summary>
        public bool IsInstall { get; set; }

        /// <summary>
        /// 是否赋值
        /// </summary>
        public bool IsValueSet { get; set; }

        /// <summary>
        /// 初始化参数
        /// </summary>
        public InstallDotCoilArg InstallArg { get; set; }

        /// <summary>
        /// 赋值参数
        /// </summary>
        public SetDotCoilArg SetArg { get; set; }

        ///// <summary>
        ///// 查找毗邻权利人到村
        ///// </summary>
        //public bool IsNeighborExportVillageLevel { set; get; }


        ///// <summary>
        ///// 界址线说明是否使用界址点统编号
        ///// </summary>
        //public bool IsUseDotUniteDotNumber { set; get; }

        ///// <summary>
        ///// 查找毗邻权利人
        ///// </summary>
        //public bool IsNeighbor { set; get; }
        ///// <summary>
        ///// 是否全部地块初始化
        ///// </summary>
        //public bool IsAllLands { get; set; }

        ///// <summary>
        ///// 是否只初始化没有界址信息的地块
        ///// </summary>
        //public bool IsLandsWithoutInfo { get; set; }

        ///// <summary>
        ///// 是否只初始化选择的地块
        ///// </summary>
        //public bool IsSelectedLands { get; set; }

        /// <summary>
        /// 是否单个地块初始化
        /// </summary>
        public bool IsSingleLand { get; set; }

        /// <summary>
        /// 待初始化承包地块
        /// </summary>
        public ContractLand SingleLand { get; set; }

        ///// <summary>
        ///// 是否根据四至查找，界址线毗邻权利人为空的情况，使用输入的自定义的权利人名称
        ///// </summary>
        //public bool IsLineVpNameUseImport { get; set; }

        ///// <summary>
        ///// 是否根据四至查找，界址线毗邻权利人为空的情况，使用输入的自定义的权利人名称-值
        ///// </summary>
        //public string LineVpNameUseImportValue { get; set; }

        ///// <summary>
        ///// 排完序的坐标点
        ///// </summary>
        //public KeyValue<bool, List<GeoAPI.Geometries.Coordinate>> OrderCoordsKv { get; set; }

        ///// <summary>
        ///// 选中权利人
        ///// </summary>
        //public List<VirtualPerson> SelectedObligees { get; set; }

        ///// <summary>
        ///// 当前地域下的含空间数据地块集合
        ///// </summary>
        //public List<ContractLand> CurrentZoneLandList { get; set; }

        ///// <summary>
        ///// 子级地域集合(包括当前地域)
        ///// </summary>
        //public List<Zone> AllZones { get; set; }


        #endregion

        #region Ctor

        /// <summary>
        /// 构造函数
        /// </summary>
        public TaskInitializeLandDotCoilArgument()
        {

        }

        #endregion

    }

    /// <summary>
    /// 初始化需要的参数
    /// </summary>
    public class InstallDotCoilArg
    {
        /// <summary>
        /// 界址点类型
        /// </summary>
        public string AddressDotType { get; set; }

        /// <summary>
        /// 界标类型
        /// </summary>
        public string AddressDotMarkType { get; set; }

        /// <summary>
        /// 是否过滤界址点
        /// </summary>
        public bool IsFilterDot { get; set; }

        /// <summary>
        /// 最小过滤角度值
        /// </summary>
        public double? MinAngleFileter { get; set; }

        /// <summary>
        /// 最大过滤角度值
        /// </summary>
        public double? MaxAngleFilter { get; set; }

        /// <summary>
        /// 是否使用增补算法
        /// </summary>
        public bool UseAddAlgorithm { get; set; }

        /// <summary>
        /// 界线性质
        /// </summary>
        public string AddressLineType { get; set; }

        /// <summary>
        /// 界址线类型
        /// </summary>
        public string AddressLineCatalog { get; set; }

        /// <summary>
        /// 界址线位置
        /// </summary>
        public string AddressLinePosition { get; set; }

        /// <summary>
        /// 邻宗地距离
        /// </summary>
        public double AddressLinedbiDistance { get; set; }

        /// <summary>
        /// 界址标识
        /// </summary>
        public string AddressPointPrefix { get; set; }

        /// <summary>
        /// 界址线说明初始化的类型
        /// </summary>
        public EnumDescription LineDescription { set; get; }

        /// <summary>
        /// 是否初始化时设置界址线位置为中和内//就是将找出外的改为内。
        /// </summary>
        public bool IsSetAddressLinePosition { get; set; }

        /// <summary>
        /// 界址线说明是否全域统编
        /// </summary>
        public bool IsUnit { set; get; }
        
        /// <summary>
        /// 界址线类别字典
        /// </summary>
        public List<Dictionary> Jzxlbdics { set; get; }
    }

    /// <summary>
    /// 赋值需要的参数
    /// </summary>
    public class SetDotCoilArg
    {
        /// <summary>
        /// 是否设置界址线性质
        /// </summary>
        public bool IsSetLinePropery { get; set; }

        /// <summary>
        /// 是否设置界址线类别
        /// </summary>
        public bool IsSetLineType { get; set; }

        /// <summary>
        /// 是否设置界址线位置
        /// </summary>
        public bool IsSetLinePosition { get; set; }

        /// <summary>
        /// 是否设置界址线说明
        /// </summary>
        public bool IsSetLineDescription { set; get; }

        /// <summary>
        /// 界址线说明初始化的类型
        /// </summary>
        public EnumDescription SetLineDescription { set; get; }

        /// <summary>
        /// 是否设置界址权利人
        /// </summary>
        public bool IsReplaceLinePerson { set; get; }

        /// <summary>
        /// 是否设置指界人
        /// </summary>
        public bool IsReplaceLineRefer { set; get; }

        /// <summary>
        /// 替换源
        /// </summary>
        public string ReplaceLinePersonFrom { set; get; }

        /// <summary>
        /// 替换目标
        /// </summary>
        public string ReplaceLinePersonTo { set; get; }

        /// <summary>
        /// 是否替换有洞面内部环界址线毗邻权利人内容
        /// </summary>
        public bool IsReplaceLinePersonFromMult { set; get; }

        /// <summary>
        /// 替换有洞面内部环界址线毗邻权利人内容
        /// </summary>
        public string ReplaceLinePersonFromMultTo { set; get; }
        
        /// <summary>
        /// 替换源-指界人
        /// </summary>
        public string ReplaceLineReferFrom { set; get; }

        /// <summary>
        /// 替换目标
        /// </summary>
        public string ReplaceLineReferTo { set; get; }

        /// <summary>
        /// 是否设置前缀
        /// </summary>
        public bool IsSetDotLinePrefix { set; get; }

        /// <summary>
        /// 是否根据四至更新界址线信息
        /// </summary>
        public bool InitialJZXInfoUseSN { set; get; }

        /// <summary>
        /// 是否根据四至更新界址线信息
        /// </summary>
        public string InitialJZXInfoSet { set; get; }

        /// <summary>
        /// 界线性质
        /// </summary>
        public string SetAddressLineType { get; set; }

        /// <summary>
        /// 界址线类型
        /// </summary>
        public string SetAddressLineCatalog { get; set; }

        /// <summary>
        /// 界址线位置
        /// </summary>
        public string SetAddressLinePosition { get; set; }

        /// <summary>
        /// 是否初始化时设置界址线位置为中和内
        /// </summary>
        public bool IsSetAddressLinePosition { get; set; }

        /// <summary>
        /// 界址线说明是否全域统编
        /// </summary>
        public bool IsUnit { set; get; }
    }

   
}
