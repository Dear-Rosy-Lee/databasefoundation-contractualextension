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

namespace YuLinTu.Library.Business
{
    /// <summary>
    /// 承包方数据参数类型
    /// </summary>
    public enum ePersonArgType
    {
        #region Fields

        /// <summary>
        /// 导出数据EXCEL
        /// </summary>
        ExportExcel = 1,

        /// <summary>
        /// 导出数据WORD
        /// </summary>
        ExportWord = 2,

        /// <summary>
        /// 导入数据
        /// </summary>
        ImportData = 3,

        /// <summary>
        /// 导出户主声明书
        /// </summary>
        ExportApply = 4,

        /// <summary>
        /// 导出委托书
        /// </summary>
        ExportDelegate = 5,

        /// <summary>
        /// 导出无异议书
        /// </summary>
        ExportIdea = 6,

        /// <summary>
        /// 导出测绘申请书
        /// </summary>
        ExportSurvey = 7,

        /// <summary>
        /// 批量导入数据
        /// </summary>
        VolumnImport = 8,

        /// <summary>
        /// 初始化承包方信息
        /// </summary>
        InitialVirtualPerson=9,

        #endregion
    }
}