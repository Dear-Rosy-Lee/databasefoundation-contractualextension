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
using YuLinTu.Library.Business;

namespace YuLinTu.Component.XiZangLZ
{
    /// <summary>
    /// 数据参数
    /// </summary>
  public  class TaskExportDJSPBArgument: TaskArgument
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
        public eContractConcordArgType ArgType { get; set; }    

        #region Properties - 签订合同

        /// <summary>
        /// 地块集合
        /// </summary>
        public List<ContractLand> ALLLands { get; set; }

        /// <summary>
        /// 发包方集合个数
        /// </summary>
        public int SendersCount { get; set; }

        /// <summary>
        /// 承包方集合
        /// </summary>
        public List<YuLinTu.Library.Entity.VirtualPerson> VirtualPersons { get; set; }

        /// <summary>
        /// 地域集合
        /// </summary>
        public List<Zone> AllZones { get; set; }

        #endregion

        #endregion

        #region Ctor

        public TaskExportDJSPBArgument()
        { }

        #endregion
    }
}
