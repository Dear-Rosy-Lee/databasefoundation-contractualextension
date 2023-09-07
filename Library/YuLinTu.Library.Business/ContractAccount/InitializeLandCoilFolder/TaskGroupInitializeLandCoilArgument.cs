/*
 * (C) 2015  鱼鳞图公司版权所有,保留所有权利 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YuLinTu.Data;
using YuLinTu.Library.Entity;

namespace YuLinTu.Library.Business
{
    /// <summary>
    /// 批量初始化界址点线任务参数
    /// </summary>
    public class TaskGroupInitializeLandCoilArgument : TaskArgument
    {
        #region Fields

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
        /// 界址点类型
        /// </summary>
        public string AddressDotType
        {
            get;
            set;
        }

        /// <summary>
        /// 界标类型
        /// </summary>
        public string AddressDotMarkType
        {
            get;
            set;
        }

        /// <summary>
        /// 界线性质
        /// </summary>
        public string AddressLineType
        {
            get;
            set;
        }

        /// <summary>
        /// 界址线类型
        /// </summary>
        public string AddressLineCatalog
        {
            get;
            set;
        }

        /// <summary>
        /// 界址线位置
        /// </summary>
        public string AddressLinePosition
        {
            get;
            set;
        }

        /// <summary>
        /// 领宗地距离
        /// </summary>
        public double AddressLinedbiDistance
        {
            get;
            set;
        }

        /// <summary>
        /// 界址标识
        /// </summary>
        public string AddressPointPrefix
        {
            get;
            set;
        }

        /// <summary>
        /// 全域统编
        /// </summary>
        public bool IsUnit { set; get; }

        /// <summary>
        /// 自动识别界址线位置
        /// </summary>
        public bool IsPostion { set; get; }

        /// <summary>
        /// 查找毗邻承包方
        /// </summary>
        public bool IsNeighbor { set; get; }

        /// <summary>
        /// 界址线说明填写长度
        /// </summary>
        public bool IsLineDescription { set; get; }

        /// <summary>
        /// 查找毗邻承包方到村
        /// </summary>
        public bool IsNeighborExportVillageLevel { set; get; }

        /// <summary>
        /// 是否根据毗邻承包方过滤界址点
        /// </summary>
        //public bool IsVirtualPersonFilter { get; set; }

        /// <summary>
        /// 是否根据角度过滤界址点
        /// </summary>
        //public bool IsAngleFilter { get; set; }

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
        /// 是否全部地块初始化
        /// </summary>
        public bool IsAllLands { get; set; }

        /// <summary>
        /// 是否只初始化没有界址信息的地块
        /// </summary>
        public bool IsLandsWithoutInfo { get; set; }

        /// <summary>
        /// 是否只初始化选择的地块
        /// </summary>
        public bool IsSelectedLands { get; set; }

        /// <summary>
        /// 选中承包方
        /// </summary>
        public List<VirtualPerson> SelectedObligees { get; set; }

        /// <summary>
        /// 当前地域下的含空间数据地块集合
        /// </summary>
        public List<ContractLand> CurrentZoneLandList { get; set; }

        /// <summary>
        /// 子级地域集合(包括当前地域)
        /// </summary>
        public List<Zone> AllZones { get; set; }

        #endregion

        #region Ctor

        /// <summary>
        /// 构造函数
        /// </summary>
        public TaskGroupInitializeLandCoilArgument()
        {

        }
        #endregion


    }
}
