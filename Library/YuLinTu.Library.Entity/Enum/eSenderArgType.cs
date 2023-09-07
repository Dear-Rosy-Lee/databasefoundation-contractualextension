/*
 * (C) 2015  鱼鳞图公司版权所有,保留所有权利 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using YuLinTu.Library.Entity;
using YuLinTu;
using YuLinTu.Data;

namespace YuLinTu.Library.Entity
{
    /// <summary>
    /// 发包方数据参数类型
    /// </summary>
    public enum eSenderArgType
    {
        #region Fields

        /// <summary>
        /// 导出Word
        /// </summary>
        ExportWord = 1,

        /// <summary>
        /// 导出Excel
        /// </summary>
        ExportExcel = 2,

        /// <summary>
        /// 导入数据
        /// </summary>
        ImportData = 3,

        /// <summary>
        /// 导出word模板
        /// </summary>
        WordTemplate = 4,

        /// <summary>
        /// 导入Excel模板
        /// </summary>
        ExcelTemplate = 5,

        /// <summary>
        /// 初始化发包方
        /// </summary>
        InitialSender = 6,

        #endregion
    }
}