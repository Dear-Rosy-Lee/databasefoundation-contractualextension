/*
 * (C) 2025  鱼鳞图公司版权所有,保留所有权利 
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
    /// 批量初始化地块是否基本农田任务参数
    /// </summary>
    public class TaskGroupInitialIsFarmerArgument : TaskArgument
    {
        #region Properties

        /// <summary>
        /// 数据库连接
        /// </summary>
        public IDbContext DbContext { get; set; }

        /// <summary>
        /// 当前地域
        /// </summary>
        public Zone CurrentZone { get; set; }

        /// <summary>
        /// 全部子级地域(包括当前地域)
        /// </summary>
        public List<Zone> AllZones { get; set; }

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
        public TaskGroupInitialIsFarmerArgument()
        {
            VirtualType = eVirtualType.Land;
        }

        #endregion
    }
}
