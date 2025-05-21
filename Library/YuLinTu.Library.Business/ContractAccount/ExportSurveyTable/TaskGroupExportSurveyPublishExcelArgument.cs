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
    /// 批量导出调查信息公示表任务参数
    /// </summary>
    public class TaskGroupExportSurveyPublishExcelArgument : TaskArgument
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
        /// 导出公示调查表日期设置
        /// </summary>
        public DateSetting PublishDateSetting { get; set; }

  

        #endregion

        #region Ctor

        /// <summary>
        /// 构造函数
        /// </summary>
        public TaskGroupExportSurveyPublishExcelArgument()
        {
            VirtualType = eVirtualType.Land;
        }

        #endregion
    }
}
