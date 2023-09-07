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
    /// 导出承包方Excel调查表任务参数
    /// </summary>
    public class TaskExportVirtualPersonExcelArgument : TaskArgument
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
        /// 类型
        /// </summary>
        public eVirtualType VirtualType { get; set; }

        /// <summary>
        /// 选择文件路径
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// 承包方其它配置
        /// </summary>
        public FamilyOtherDefine FamilyOtherSet { get; set; }

        /// <summary>
        /// 承包方导出设置
        /// </summary>
        public FamilyOutputDefine FamilyOutputSet { get; set; }

        /// <summary>
        /// 系统设置
        /// </summary>
        public SystemSetDefine SystemSet { get; set; }
        /// <summary>
        /// 是否有子级地域
        /// </summary>
        public bool hasChildren { get; set; }

        #endregion

        #region Ctor

        /// <summary>
        /// 构造函数
        /// </summary>
        public TaskExportVirtualPersonExcelArgument()
        {
            VirtualType = eVirtualType.Land;
        }

        #endregion
    }
}
