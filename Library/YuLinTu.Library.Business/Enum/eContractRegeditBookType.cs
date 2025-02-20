/*
 * (C) 2025  鱼鳞图公司版权所有,保留所有权利 
 */
namespace YuLinTu.Library.Business
{
    /// <summary>
    /// 承包权证数据参数类型
    /// </summary>
    public enum eContractRegeditBookType
    {
        /// <summary>
        /// 权证登记
        /// </summary>
        InitialWarrantData = 20,

        #region 工具

        /// <summary>
        /// 导出权证-证书数据处理
        /// </summary>
        ExportWarrant = 1,

        /// <summary>
        /// 批量导出权证-证书数据处理
        /// </summary>
        BatchExportWarrant = 12,

        /// <summary>
        /// 权证数据汇总表
        /// </summary>
        ExportWarrentSummaryData = 2,

        /// <summary>
        /// 批量权证数据汇总表
        /// </summary>
        BatchExportWarrentSummaryData = 13,

        /// <summary>
        /// 颁证清册
        /// </summary>
        ExportAwareInventoryData = 3,

        /// <summary>
        /// 批量颁证清册
        /// </summary>
        BatchExportAwareInventoryData = 14,

        /// <summary>
        /// 单户确认表
        /// </summary>
        ExportSingleFamilyConfirmData = 4,

        /// <summary>
        /// 批量单户确认表
        /// </summary>
        BatchExportSingleFamilyConfirmData = 15,

        /// <summary>
        /// 批量导出登记簿
        /// </summary>
        BatchExportRegeditBookData = 10,

        /// <summary>
        /// 导出登记簿
        /// </summary>
        ExportRegeditBookData = 11,

        #endregion

    }
}
