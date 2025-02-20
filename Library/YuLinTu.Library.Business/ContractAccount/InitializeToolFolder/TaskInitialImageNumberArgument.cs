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
    /// 初始化地块图幅编号任务参数
    /// </summary>
    public class TaskInitialImageNumberArgument : TaskArgument
    {
        #region Properties

        /// <summary>
        /// 当前行政地域
        /// </summary>
        public Zone CurrentZone { get; set; }

        /// <summary>
        /// 数据库连接
        /// </summary>
        public IDbContext DbContext { get; set; }

        /// <summary>
        /// 当前地域下(包括子级地域)的所有空间地块集合
        /// </summary>
        public List<ContractLand> ListGeoLand { get; set; }

        /// <summary>
        /// 图幅类型索引(0是50X50)
        /// </summary>
        public int ScropeIndex { get; set; }

        /// <summary>
        /// 比例尺索引(0是1:2000、1是1:1000、2是1:500)
        /// </summary>
        public int ScalerIndex { get; set; }

        /// <summary>
        /// 是否使用地理坐标系
        /// </summary>
        public bool IsUseYX { get; set; }

        /// <summary>
        /// 是否初始化所有的图幅表哈
        /// </summary>
        public bool IsInitialAllImageNumber { get; set; }

        /// <summary>
        /// 类型
        /// </summary>
        public eVirtualType VirtualType { get; set; }

        #endregion

        #region Ctor

        /// <summary>
        /// 构造函数
        /// </summary>
        public TaskInitialImageNumberArgument()
        {
            VirtualType = eVirtualType.Land;
        }

        #endregion
    }
}
