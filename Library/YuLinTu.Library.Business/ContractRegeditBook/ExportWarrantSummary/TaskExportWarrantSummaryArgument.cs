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
    /// 导出权证数据汇总表任务参数
    /// </summary>
    public class TaskExportWarrantSummaryArgument : TaskArgument
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
        /// 选择文件路径
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// 系统设置
        /// </summary>
        public SystemSetDefine SystemSet { get; set; }

        /// <summary>
        /// 汇总表设置实体
        /// </summary>
        public DataSummaryDefine SummaryDefine { get; set; }
        /// <summary>
        /// 是否预览
        /// </summary>
        public bool IsShow { get; set; }

        #endregion

        #region Ctor

        /// <summary>
        /// 构造函数
        /// </summary>
        public TaskExportWarrantSummaryArgument()
        { }

        #endregion
    }
}
