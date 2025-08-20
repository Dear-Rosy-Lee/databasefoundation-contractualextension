/*
 * (C) 2025  鱼鳞图公司版权所有,保留所有权利
 */

using Aspose.Cells;
using System.ComponentModel.DataAnnotations;

namespace YuLinTu.Library.Business
{
    /// <summary>
    /// 承包台账数据参数类型
    /// </summary>
    public enum eContractAccountType
    {
        #region Fields - 导入导出

        /// <summary>
        /// 导入数据
        /// </summary>
        [Display(Name = "导入数据")]
        ImportData = 1,

        /// <summary>
        /// 批量导入数据
        /// </summary>
        [Display(Name = "批量导入数据")]
        VolumnImport = 2,

        /// <summary>
        /// 导出公示结果归户表
        /// </summary>
        [Display(Name = "导出公示结果归户表")]
        ExportSurveyTable = 52,

        /// <summary>
        /// 批量导出公示结果归户表
        /// </summary>
        [Display(Name = "批量导出公示结果归户表")]
        VolumnExportSurveyTable = 3,

        /// <summary>
        /// 导出地块调查表Word
        /// </summary>
        [Display(Name = "导出地块调查表Word")]
        ExportLandSurveyWord = 25,

        /// <summary>
        /// 批量导出地块调查表
        /// </summary>
        [Display(Name = "批量导出地块调查表")]
        VolumnExportLandSurveyTable = 4,

        /// <summary>
        /// 台账报表-导出4个同样表的excel数据
        /// </summary>
        [Display(Name = "导出4个同样表的excel数据")]
        ExportLandSurveyTable = 5,

        /// <summary>
        /// 界址调查表
        /// </summary>
        [Display(Name = "界址调查表")]
        ExportBoundaryInfoExcel = 53,

        /// <summary>
        /// 批量界址调查表
        /// </summary>
        [Display(Name = "批量界址调查表")]
        VolumnExportBoundaryInfoExcel = 54,

        /// <summary>
        /// 公示调查表
        /// </summary>
        [Display(Name = "公示调查表")]
        ExportPublishTable = 6,

        /// <summary>
        /// 批量导出公示调查表
        /// </summary>
        [Display(Name = "批量导出公示调查表")]
        VolumnExportPublishTable = 7,

        /// <summary>
        /// 公示调查表
        /// </summary>
        [Display(Name = "公示调查表(南宁)")]
        ExportPublishTableNanning = 44,

        /// <summary>
        /// 批量导出公示调查表
        /// </summary>
        [Display(Name = "批量导出公示调查表(南宁)")]
        VolumnExportPublishTableNanning = 45,

        /// <summary>
        /// 导出承包方Word调查表
        /// </summary>
        [Display(Name = "导出承包方Word调查表")]
        ExportContractorTable = 51,

        /// <summary>
        /// 批量导出承包方Word调查表
        /// </summary>
        [Display(Name = "批量导出承包方Word调查表")]
        VolumnExportContractorTable = 8,

        /// <summary>
        /// 导出发包方Word
        /// </summary>
        [Display(Name = "导出发包方Word")]
        ExportSendTableWord = 9,

        /// <summary>
        /// 批量导出发包方Word
        /// </summary>
        [Display(Name = "批量导出发包方Word")]
        VolumnExportSenderTableWord = 19,

        /// <summary>
        /// 导出发包方Excel20
        /// </summary>
        [Display(Name = "导出发包方Excel")]
        ExportSendTableExcel = 30,

        /// <summary>
        /// 导出承包方调查表Excel
        /// </summary>
        [Display(Name = "导出承包方调查表Excel")]
        ExportVirtualPersonExcel = 31,

        /// <summary>
        /// 批量导出承包方调查表Excel
        /// </summary>
        [Display(Name = "批量导出承包方调查表Excel")]
        VolumnExportVirtualPersonExcel = 32,

        /// <summary>
        /// 台账报表-导出承包地块调查表
        /// </summary>
        [Display(Name = "承包地块调查表")]
        ExportContractAccountExcel = 10,

        /// <summary>
        /// 台账报表-导出单户确认表的excel数据
        /// </summary>
        [Display(Name = "导出单户excel确认表")]
        ExportSingleFamilyConfirmExcel = 11,

        /// <summary>
        /// 台账报表-批量导出单户确认表的excel数据
        /// </summary>
        [Display(Name = "批量导出单户确认excel表")]
        VolumnExportSingleFamilyConfirmExcel = 17,

        /// <summary>
        /// 台账报表-批量导出5个表的excel数据
        /// </summary>
        [Display(Name = "批量承包地块调查表")]
        VolumnExportContractAccountExcel = 18,

        /// <summary>
        /// 导出数据汇总表
        /// </summary>
        [Display(Name = "导出数据汇总表")]
        ExportSummaryExcel = 12,

        /// <summary>
        /// 批量导出数据汇总表
        /// </summary>
        [Display(Name = "批量导出数据汇总表")]
        VolumnExportSummaryExcel = 21,

        /// <summary>
        /// 导出村组公示公告
        /// </summary>
        [Display(Name = "导出村组公示公告")]
        ExportVillageDeclare = 13,

        /// <summary>
        /// 批量导出村组公示公告
        /// </summary>
        [Display(Name = "批量导出村组公示公告")]
        VolumnExportVillageDeclare = 43,

        /// <summary>
        /// 台账报表-导出单户调查表的excel数据
        /// </summary>
        [Display(Name = "导出单户调查表的excel数据")]
        ExportSingleFamilySurveyExcel = 14,

        /// <summary>
        /// 台账报表-批量导出单户调查表的excel数据
        /// </summary>
        [Display(Name = "批量导出单户调查表的excel数据")]
        VolumnExportSingleFamilySurveyExcel = 20,

        /// <summary>
        /// 导出地块示意图
        /// </summary>
        [Display(Name = "导出地块示意图")]
        ExportMultiParcelOfFamily = 33,

        /// <summary>
        /// 批量导出地块示意图
        /// </summary>
        [Display(Name = "批量导出地块示意图")]
        VolumnExportMultiParcelOfFamily = 34,

        /// <summary>
        /// 导出地块示意图小页图示意图
        /// </summary>
        [Display(Name = "导出地块示意图小页图示意图")]
        ExportSamllMultiParcelOfFamily = 35,

        /// <summary>
        /// 批量导出地块示意图小页图示意图
        /// </summary>
        [Display(Name = "导出地块示意图小页图示意图")]
        VolumnExportSamllMultiParcelOfFamily = 36,

        /// <summary>
        /// 导入承包地图斑shape数据
        /// </summary>
        [Display(Name = "导入承包地图斑shape数据")]
        ImportLandShapeData = 15,

        /// <summary>
        /// 导入界址点图斑数据
        /// </summary>
        ImportDotShapeData = 16,

        #endregion Fields - 导入导出

        #region Fields - 初始化工具

        /// <summary>
        /// 初始化承包地块属性信息
        /// </summary>
        InitialLandTool = 100,

        /// <summary>
        /// 初始化承包地块面积
        /// </summary>
        InitialAreaTool = 101,

        /// <summary>
        /// 截取承包地块面积小数位
        /// </summary>
        InitialAreaNumericFormatTool = 102,

        /// <summary>
        /// 初始化承包地块是否基本农田
        /// </summary>
        InitialIsFarmerTool = 103,

        #endregion Fields - 初始化工具

        #region Fields - 检索工具

        /// <summary>
        /// 检索地块名称为空
        /// </summary>
        SearchNameNull = 103,

        /// <summary>
        /// 检索二轮合同面积为空
        /// </summary>
        SearchContractAreaNull = 104,

        /// <summary>
        /// 检索实测面积为空
        /// </summary>
        SearchActualAreaNull = 105,

        /// <summary>
        /// 检索确权面积为空
        /// </summary>
        SearchAwareAreaNull = 106,

        /// <summary>
        /// 检索是否基本农田为空
        /// </summary>
        SearchIsFarmerNull = 107,

        /// <summary>
        /// 检索地力等级为空
        /// </summary>
        SearchLandLevelNull = 108,

        /// <summary>
        /// 检索地块图斑为空
        /// </summary>
        SearchLandShapeNull = 109,

        #endregion Fields - 检索工具

        #region 承包合同或承包权证

        /// <summary>
        /// 合同明细表
        /// </summary>
        [Display(Name = "合同明细表")]
        ExportConcordInformationTable = 22,

        /// <summary>
        /// 权证数据汇总表
        /// </summary>
        [Display(Name = "权证数据汇总表")]
        ExportWarrentSummaryTable = 23,

        #endregion 承包合同或承包权证

        /// <summary>
        /// 查找四至
        /// </summary>
        SeekLandNeighbor = 24,

        /// <summary>
        /// 导出合同信息表
        /// </summary>
        [Display(Name = "导出合同信息表")]
        ExportContractInformationExcel = 110,

        /// <summary>
        /// 批量导出合同信息表
        /// </summary>
        [Display(Name = "批量导出合同信息表")]
        VolumnExportContractInformationExcel = 111,

        /// <summary>
        /// 批量导出摸底核实表
        /// </summary>
        [Display(Name = "批量导出摸底核实表")]
        VolumnExportLandVerifyExcel = 112,

        /// <summary>
        /// 批量导出摸底核实表(打印版)
        /// </summary>
        [Display(Name = "批量导出摸底核实表(打印版)")]
        VolumnExportLandVerifyPrintExcel = 113,

        /// <summary>
        /// 批量导出单户摸底核实表
        /// </summary>
        [Display(Name = "批量导出单户摸底核实表")]
        ExportLandVerifySingeExcel = 114,

        /// <summary>
        /// 一键导出试点工作统计表
        /// </summary>
        [Display(Name = "批量导出单户摸底核实表")]
        ExportAllDelayTable = 115,

        /// <summary>
        /// 批量一键导出试点工作统计表
        /// </summary>
        [Display(Name = "批量导出单户摸底核实表")]
        VolumnExportAllDelayTable = 116,

    }
}