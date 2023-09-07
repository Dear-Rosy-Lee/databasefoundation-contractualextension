/*
 * (C) 2015  鱼鳞图公司版权所有,保留所有权利 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using YuLinTu.Library.Entity;
using YuLinTu;
using YuLinTu.Data;

namespace YuLinTu.Library.Business
{
  /// <summary>
  /// 承包权证数据参数
  /// </summary>
  public class TaskGroupInitalizeWarrentArgument : TaskArgument
    {
        #region Properties

        /// <summary>
        /// 文件路径
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// 当前地域
        /// </summary>
        public Zone CurrentZone { get; set; }

        /// <summary>
        /// 数据库
        /// </summary>
        public IDbContext Database { get; set; }

        /// <summary>
        /// 参数类型
        /// </summary>
        public eContractRegeditBookType ArgType { get; set; }

        /// <summary>
        /// 时间
        /// </summary>
        public DateTime? DateValue { get; set; }

        /// <summary>
        /// 公示时间
        /// </summary>
        public DateTime? PubDateValue { get; set; }

        #region Properties - 权证登记

        /// <summary>
        /// 当前地域下承包方集合-未被锁定的
        /// </summary>
        public List<VirtualPerson> listPerson { get; set; }

        /// <summary>
        /// 当前地域下承包合同集合
        /// </summary>
        public List<ContractConcord> Concords { get; set; }
        
        /// <summary>
        /// 承包权证集合(初始化之前)
        /// </summary>
        public List<ContractRegeditBook> ListWarrants { get; set; }          

        /// <summary>
        /// 承包权证集合(初始化之后)
        /// </summary>
        public List<ContractRegeditBook> WarrantsModified { get; set; }

        /// <summary>
        /// 当前地域下的承包地块集合
        /// </summary>
        public List<ContractLand> ListLand { get; set; }

        /// <summary>
        /// 初始化合同信息的地块集合
        /// </summary>
        public List<ContractLand> LandsOfInitialConcord { get; set; }

        /// <summary>
        /// 发包方集合
        /// </summary>
        public List<CollectivityTissue> Senders { get; set; }

        /// <summary>
        /// 系统信息常规设置
        /// </summary>
        public SystemSetDefine SystemSet { get; set; }

        /// <summary>
        /// 所有的地域集合
        /// </summary>
        public List<Zone> AllZones { get; set; }            

        #endregion

        #endregion

        #region Ctor

        /// <summary>
        /// 构造函数
        /// </summary>
        public TaskGroupInitalizeWarrentArgument()
        { }

        #endregion
    }
}
