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
    /// 签订合同任务参数
    /// </summary>
    public class TaskGroupConcordInitializeArgument : TaskArgument
    {
        #region Properties

        /// <summary>
        /// 当前地域
        /// </summary>
        public Zone CurrentZone { get; set; }

        /// <summary>
        /// 数据库
        /// </summary>
        public IDbContext Database { get; set; }


        /// <summary>
        /// 承包合同集合(待初始化)
        /// </summary>
        public List<ContractConcord> ConcordsModified { get; set; }

        /// <summary>
        /// 初始化合同信息的地块集合
        /// </summary>
        public List<ContractLand> LandsOfInitialConcord { get; set; }

        /// <summary>
        /// 发包方
        /// </summary>
        public CollectivityTissue Sender { get; set; }

        /// <summary>
        /// 是否对承包地块面积进行处理
        /// </summary>
        public bool IsCalculateArea { get; set; }
        
        /// <summary>
        /// 所有的地域集合
        /// </summary>
        public List<Zone> AllZones { get; set; }

        /// <summary>
        /// 按村级进行户号、地块编码统一初始及签订
        /// </summary>
        public bool VillageInlitialSet { get; set; }

        #endregion

    }
}
