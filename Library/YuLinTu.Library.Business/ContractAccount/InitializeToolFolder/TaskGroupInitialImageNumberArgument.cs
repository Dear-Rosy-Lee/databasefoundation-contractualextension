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
    /// 批量初始化地块图幅编号任务参数
    /// </summary>
    public class TaskGroupInitialImageNumberArgument : TaskArgument
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
        /// 是否初始化所有图幅编号
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
        public TaskGroupInitialImageNumberArgument()
        {
            VirtualType = eVirtualType.Land;
        }

        #endregion
    }
}
