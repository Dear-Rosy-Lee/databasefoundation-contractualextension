/*
 * (C) 2025  鱼鳞图公司版权所有,保留所有权利 
 */
using System.Collections.Generic;
using YuLinTu.Data;
using YuLinTu.Library.Entity;

namespace YuLinTu.Library.Business
{
    /// <summary>
    /// 初始化空间地块面积任务参数
    /// </summary>
    public class TaskInitialAreaArgument : TaskArgument
    {
        #region Properties - 初始地块实测面积和确权面积

        /// <summary>
        /// 当前行政地域
        /// </summary>
        public Zone CurrentZone { get; set; }
        /// <summary>
        /// 小数位数
        /// </summary>
        public int ToAreaNumeric { get; set; }

        /// <summary>
        /// 小数点处理模式
        /// </summary>
        public int ToAreaModule { get; set; }

        /// <summary>
        /// 数据库连接
        /// </summary>
        public IDbContext DbContext { get; set; }

        /// <summary>
        /// 是否平面面积到实测面积
        /// </summary>
        public bool ToActualArea { get; set; }

        /// <summary>
        /// 是否平面面积到确权面积
        /// </summary>
        public bool ToAwareArea { get; set; }

        /// <summary>
        /// 类型
        /// </summary>
        public eVirtualType VirtualType { get; set; }

        /// <summary>
        /// 当前地域下的所有空间地块集合
        /// </summary>
        public List<ContractLand> ListGeoLand { get; set; }
        public bool InstallEmpty { get; set; }
        public bool InstallContract { get; set; }

        #endregion

        #region Ctor

        /// <summary>
        /// 构造函数
        /// </summary>
        public TaskInitialAreaArgument()
        {
            VirtualType = eVirtualType.Land;
        }

        #endregion
    }
}

