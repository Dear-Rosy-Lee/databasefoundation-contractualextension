/*
 * (C) 2025  鱼鳞图公司版权所有,保留所有权利 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using YuLinTu.Library.Entity;
using YuLinTu;
using YuLinTu.Data;
using System.Collections.ObjectModel;
using System.Collections;

namespace YuLinTu.Library.Business
{
    /// <summary>
    /// 地块类别汇总表
    /// </summary>
   public class TaskExportCategorySummaryExcelArgument : TaskArgument
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
        public string UnitName { get; set; }

        #endregion
        /// <summary>
        /// 构造函数
        /// </summary>
        public TaskExportCategorySummaryExcelArgument()
        {
        }
        
    }
}
