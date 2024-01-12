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
    /// 承包台账数据参数类型
    /// </summary>
    public enum eContractAccountType
    {
        #region Fields - 导入导出

        /// <summary>
        /// 导入数据
        /// </summary>
        ImportData = 1,

        /// <summary>
        /// 批量导入数据
        /// </summary>
        VolumnImport = 2,

        /// <summary>
        /// 导出公示结果归户表
        /// </summary>
        ExportSurveyTable = 52,

        /// <summary>
        /// 批量导出公示结果归户表
        /// </summary>
        VolumnExportSurveyTable = 3,

        /// <summary>
        /// 导出地块调查表Word
        /// </summary>
        ExportLandSurveyWord = 25,

        /// <summary>
        /// 批量导出地块调查表
        /// </summary>
        VolumnExportLandSurveyTable = 4,

        /// <summary>
        /// 台账报表-导出4个同样表的excel数据
        /// </summary>
        ExportLandSurveyTable = 5,

        /// <summary>
        /// 界址调查表
        /// </summary>
        ExportBoundaryInfoExcel = 53,

        /// <summary>
        /// 批量界址调查表
        /// </summary>
        VolumnExportBoundaryInfoExcel = 54,

        /// <summary>
        /// 公示调查表
        /// </summary>
        ExportPublishTable = 6,

        /// <summary>
        /// 批量导出公示调查表
        /// </summary>
        VolumnExportPublishTable = 7,

        /// <summary>
        /// 导出承包方Word调查表
        /// </summary>
        ExportContractorTable = 51,

        /// <summary>
        /// 批量导出承包方Word调查表
        /// </summary>
        VolumnExportContractorTable = 8,

        /// <summary>
        /// 导出发包方Word
        /// </summary>
        ExportSendTableWord = 9,

        /// <summary>
        /// 批量导出发包方Word
        /// </summary>
        VolumnExportSenderTableWord = 19,

        /// <summary>
        /// 导出发包方Excel20
        /// </summary>
        ExportSendTableExcel = 30,

        /// <summary>
        /// 导出承包方调查表Excel
        /// </summary>
        ExportVirtualPersonExcel = 31,

        /// <summary>
        /// 批量导出承包方调查表Excel
        /// </summary>
        VolumnExportVirtualPersonExcel = 32,

        /// <summary>
        /// 台账报表-导出5个表的excel数据
        /// </summary>
        ExportContractAccountExcel = 10,

        /// <summary>
        /// 台账报表-导出单户确认表的excel数据
        /// </summary>
        ExportSingleFamilyConfirmExcel = 11,

        /// <summary>
        /// 台账报表-批量导出单户确认表的excel数据
        /// </summary>
        VolumnExportSingleFamilyConfirmExcel = 17,

        /// <summary>
        /// 台账报表-批量导出5个表的excel数据
        /// </summary>
        VolumnExportContractAccountExcel = 18,

        /// <summary>
        /// 导出数据汇总表
        /// </summary>
        ExportSummaryExcel = 12,

        /// <summary>
        /// 批量导出数据汇总表
        /// </summary>
        VolumnExportSummaryExcel = 21,

        /// <summary>
        /// 导出村组公示公告
        /// </summary>
        ExportVillageDeclare = 13,

        /// <summary>
        /// 批量导出村组公示公告
        /// </summary>
        VolumnExportVillageDeclare = 43,

        /// <summary>
        /// 台账报表-导出单户调查表的excel数据
        /// </summary>
        ExportSingleFamilySurveyExcel = 14,

        /// <summary>
        /// 台账报表-批量导出单户调查表的excel数据
        /// </summary>
        VolumnExportSingleFamilySurveyExcel = 20,

        /// <summary>
        /// 导出地块示意图
        /// </summary>
        ExportMultiParcelOfFamily = 33,

        /// <summary>
        /// 批量导出地块示意图
        /// </summary>
        VolumnExportMultiParcelOfFamily = 34,

        /// <summary>
        /// 导出地块示意图小页图示意图
        /// </summary>
        ExportSamllMultiParcelOfFamily = 35,

        /// <summary>
        /// 批量导出地块示意图小页图示意图
        /// </summary>
        VolumnExportSamllMultiParcelOfFamily = 36,

        /// <summary>
        /// 导入承包地图斑shape数据
        /// </summary>
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
        ExportConcordInformationTable = 22,

        /// <summary>
        /// 权证数据汇总表
        /// </summary>
        ExportWarrentSummaryTable = 23,

        #endregion 承包合同或承包权证

        /// <summary>
        /// 查找四至
        /// </summary>
        SeekLandNeighbor = 24,

        /// <summary>
        /// 导出合同信息表
        /// </summary>
        ExportContractInformationExcel = 110,

        /// <summary>
        /// 批量导出合同信息表
        /// </summary>
        VolumnExportContractInformationExcel = 111
    }
}