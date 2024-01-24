/*
 * (C) 2015  鱼鳞图公司版权所有,保留所有权利 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YuLinTu.Data;
using YuLinTu.Library.Entity;

namespace YuLinTu.Component.XiZangLZ
{
    /// <summary>
    /// 批量导出界址点成果表任务参数
    /// </summary>
    public class TaskTopGropExportDotResultArgument : TaskArgument
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
        /// 文件路径
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// 类型
        /// </summary>
        public eVirtualType VirtualType { get; set; }

        /// <summary>
        /// 导出承包方集合(当前地域及其子级地域)
        /// </summary>
        public List<VirtualPerson> ListPerson { get; set; }

        /// <summary>
        /// 导出空间地块集合(当前地域及其子级地域)
        /// </summary>
        public List<ContractLand> ListGeoLand { get; set; }

        /// <summary>
        /// 所有地域集合(包括当前地域)
        /// </summary>
        public List<Zone> AllZones { get; set; }

        #endregion

        #region Ctor

        /// <summary>
        /// 构造函数
        /// </summary>
        public TaskTopGropExportDotResultArgument()
        {
            VirtualType = eVirtualType.Land;
        }

        #endregion
    }
}
