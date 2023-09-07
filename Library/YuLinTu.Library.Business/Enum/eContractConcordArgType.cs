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
    /// 承包合同数据参数类型
    /// </summary>
    public enum eContractConcordArgType
    {
        #region 家庭承包方式

        /// <summary>
        /// 预览单户申请书
        /// </summary>
        SinglePrintRequireBook = 1,

        /// <summary>
        /// 导出单户申请书
        /// </summary>
        SingleExportRequireBook = 2,

        /// <summary>
        /// 批量导出单户申请书
        /// </summary>
        BatchSingleExportRequireBook = 3,

        /// <summary>
        /// 其他承包方式导出单户申请书
        /// </summary>
        ExportApplicationByOther = 4,

        /// <summary>
        /// 其他承包方式批量导出单户申请书
        /// </summary>
        BatchExportApplicationByOther = 5,

        /// <summary>
        /// 导出集体申请书
        /// </summary>
        ExportApplicationBook = 6,

        /// <summary>
        /// 批量导出集体申请书
        /// </summary>
        BatchExportApplicationBook = 7,

        #endregion

        #region 合同数据处理

        /// <summary>
        /// 签订合同
        /// </summary>
        InitialConcordData = 20,

        /// <summary>
        /// 导出合同
        /// </summary>
        ExportConcordData = 21,

        #endregion

        #region 工具

        /// <summary>
        /// 合同明细表
        /// </summary>
        ExportConcordInformationTable = 22,

        /// <summary>
        /// 批量导出合同明细表
        /// </summary>
        BatchExportConcordInformationTable = 23,
        #endregion
    }
}
