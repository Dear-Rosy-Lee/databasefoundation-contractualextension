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
    /// 初始化是否基本农田任务参数
    /// </summary>
    public class TaskInitialIsFarmerArgument : TaskArgument
    {
        #region  Properties - 初始地块是否为基本农田

        /// <summary>
        /// 当前行政地域
        /// </summary>
        public Zone CurrentZone { get; set; }

        /// <summary>
        /// 数据库连接
        /// </summary>
        public IDbContext DbContext { get; set; }

        /// <summary>
        /// 当前地域下的所有空间地块集合
        /// </summary>
        public List<ContractLand> ListGeoLand { get; set; }

        /// <summary>
        /// Shape文件路径
        /// </summary>
        public string ShapeFileName { get; set; }

        /// <summary>
        /// 类型
        /// </summary>
        public eVirtualType VirtualType { get; set; }

        #endregion

        #region Ctor

        /// <summary>
        /// 构造函数
        /// </summary>
        public TaskInitialIsFarmerArgument()
        {
            VirtualType = eVirtualType.Land;
        }

        #endregion
    }
}

