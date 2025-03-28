/*
 * (C) 2025  鱼鳞图公司版权所有,保留所有权利 
 */
using System.Collections.Generic;
using YuLinTu.Data;
using YuLinTu.Library.Entity;

namespace YuLinTu.Library.Business
{
    /// <summary>
    /// 批量初始化地块基本信息任务参数
    /// </summary>
    public class TaskGroupInitialLandInfoArgument : TaskInitialLandInfoArgument
    {
        #region Properties

        /// <summary>
        /// 数据库连接
        /// </summary>
        public IDbContext DbContext { get; set; }

        /// <summary>
        /// 全部子级地域(包括当前地域)
        /// </summary>
        public List<Zone> AllZones { get; set; }

        #endregion

        #region Ctor

        /// <summary>
        /// 构造函数
        /// </summary>
        public TaskGroupInitialLandInfoArgument()
        {
            VirtualType = eVirtualType.Land;
        }

        #endregion
    }
}
