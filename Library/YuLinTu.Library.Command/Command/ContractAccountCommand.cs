/*
 * (C) 2015  鱼鳞图公司版权所有,保留所有权利 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace YuLinTu.Library.Command
{
    /// <summary>
    /// 承包台账模块命令定义
    /// </summary>
    public class ContractAccountCommand
    {
        #region Files - Const

        /// <summary>
        /// 承包地块添加命令名称
        /// </summary>
        public const string ContractLandAddName = "contractLandAdd";

        /// <summary>
        /// 承包地块编辑命令名称
        /// </summary>
        public const string ContractLandEditName = "contractLandEdit";

        /// <summary>
        /// 承包地块删除命令名称
        /// </summary>
        public const string ContractLandDelName = "contractLandDel";

        /// <summary>
        /// 导入(批量)地块调查表命令名称
        /// </summary>
        public const string ImportLandExcelName = "importLandExcel";

        /// <summary>
        /// 导入(批量)界址调查表命令名称
        /// </summary>
        public const string ImportBoundaryExcelName = "importBoundaryExcel";

        /// <summary>
        /// 导入(批量)矢量调查图斑数据命令名称
        /// </summary>
        public const string ImportVectorName = "importVector";

        /// <summary>
        /// 导入(批量)界址点调查图斑数据命令名称
        /// </summary>
        public const string ImportPointsName = "importPoints";

        /// <summary>
        /// 导入(批量)界址线调查图斑数据命令名称
        /// </summary>
        public const string ImportLinesName = "importLines";

        /// <summary>
        /// 导入(批量)土地调查压缩包数据命令名称
        /// </summary>
        public const string ImportZipName = "importZip";   

        /// <summary>
        /// 导出当前地域下地块Shape数据
        /// </summary>
        public const string ExportLandShapeDataName = "exportLandShapeData";

        /// <summary>
        /// 导出当前地域下地块界址点Shape数据
        /// </summary>
        public const string ExportLandDotShapeDataName = "exportLandDotShapeData";

        /// <summary>
        /// 导出当前地域下地块界址线Shape数据
        /// </summary>
        public const string ExportLandCoilShapeDataName = "exportLandCoilShapeData";

        /// <summary>
        /// 导出发包方调查模板命令名称
        /// </summary>
        public const string SenderExcelTemplateName = "senderExcelTemplate";

        /// <summary>
        /// 导出承包方调查模板命令名称
        /// </summary>
        public const string VPExcelTemplateName = "vPExcelTemplate";

        /// <summary>
        /// 导出承包地块调查模板命令名称
        /// </summary>
        public const string LandExcelTemplateName = "landExcelTemplate";

        /// <summary>
        /// 导出界址信息调查模板命令名称
        /// </summary>
        public const string BoundaryExcelTemplateName = "boundaryExcelTemplate";

        /// <summary>
        /// 导出发包方调查模板(Word)命令名称
        /// </summary>
        public const string SenderWordTemplateName = "senderWordTemplate";

        /// <summary>
        /// 导出承包方调查模板(Word)命令名称
        /// </summary>
        public const string VPWordTemplateName = "vPWordTemplate";

        /// <summary>
        /// 导出承包地块调查模板(Word)命令名称
        /// </summary>
        public const string LandWordTemplateName = "landWordTemplate";

        /// <summary>
        /// 导出(批量)发包方调查表命令名称
        /// </summary>
        public const string ExportSenderExcelName = "exportSenderExcel";

        /// <summary>
        /// 导出(批量)承包方调查表命令名称
        /// </summary>
        public const string ExportVPExcelName = "exportVPExcel";

        /// <summary>
        /// 导出(批量)承包地块调查表命令名称
        /// </summary>
        public const string ExportLandExceleName = "exportLandExcel";

        /// <summary>
        /// 导出(批量)界址信息调查表命令名称
        /// </summary>
        public const string ExportBoundaryExcelName = "exportBoundaryExcel";

        /// <summary>
        /// 导出(批量)调查信息公示表命令名称
        /// </summary>
        public const string ExportPublishExcelName = "exportPublishExcel";

        /// <summary>
        /// 导出(批量)发包方调查表(Word)命令名称
        /// </summary>
        public const string ExportSenderWordName = "exportSenderWord";

        /// <summary>
        /// 导出(批量)承包方调查表(Word)命令名称
        /// </summary>
        public const string ExportVPWordName = "exportVPWord";

        /// <summary>
        /// 导出(批量)承包地块调查表(Word)命令名称
        /// </summary>
        public const string ExportLandWordName = "exportLandWord";

        /// <summary>
        /// 导出(批量)公示结果归户表(Word)命令名称
        /// </summary>
        public const string ExportPublishWordName = "exportPublishWord";

        /// <summary>
        /// 导出(批量)台账调查表命令名称
        /// </summary>
        public const string ExportAccountName = "exportAccount";

        /// <summary>
        /// 导出(批量)单户调查表命令名称
        /// </summary>
        public const string ExportSingleName = "exportSingle";

        /// <summary>
        /// 导出(批量)登记公示表命令名称
        /// </summary>
        public const string ExportPublishName = "exportPublish";

        /// <summary>
        /// 导出(批量)登记签字表命令名称
        /// </summary>
        public const string ExportSignName = "exportSign";

        /// <summary>
        /// 导出(批量)村组公示表命令名称
        /// </summary>
        public const string ExportVillagesPubName = "exportVillagesPub";

        /// <summary>
        /// 导出(批量)村组公告表命令名称
        /// </summary>
        public const string ExportVillagesDeclareName = "exportVillagesDeclare";

        /// <summary>
        /// 导出(批量)单户确认表命令名称
        /// </summary>
        public const string ExportSingleAffirmName = "exportSingleAffirm";

        /// <summary>
        /// 导出(批量)数据汇总表命令名称
        /// </summary>
        public const string ExportSummaryName = "exportSummary";
        /// <summary>
        /// 导出（批量）地块类别汇总表命令名称
        /// </summary>
        public const string ExportCategorySummary = "exportCategory";

        /// <summary>
        /// 查找四至命令名称
        /// </summary>
        public const string SearchNeighorName = "searchNeighor";

        /// <summary>
        /// 导出(批量)界址点成果表命令名称
        /// </summary>
        public const string ResultExcelExpotName = "resultExcelExpot";

        /// <summary>
        /// 示意图设置命令名称
        /// </summary>
        public const string ParcelSettingName = "parcelSetting";

        /// <summary>
        /// 导出地块示意图小页图命令名称
        /// </summary>
        public const string MultiParcelExportSamllName = "multiParcelSmallExport";

        /// <summary>
        /// 导出地块示意图命令名称
        /// </summary>
        public const string MultiParcelExportName = "multiParcelExport";

        /// <summary>
        /// 导出确权确股地块示意图命令
        /// </summary>
        public const string MultiParcelStockExportName = "multiParcelStockExport";

        /// <summary>
        /// 全部地块命令名称
        /// </summary>
        public const string AllLandName = "allLand";

        /// <summary>
        /// 承包地块命令名称
        /// </summary>
        public const string ContractLandName = "contractLand";

        /// <summary>
        /// 自留地块命令名称
        /// </summary>
        public const string OwnerLandName = "ownerLand";

        /// <summary>
        /// 机动地块命令名称
        /// </summary>
        public const string MotorLandName = "motorLand";

        /// <summary>
        /// 开荒地块命令名称
        /// </summary>
        public const string CultivateLandName = "cultivateLand";

        /// <summary>
        /// 其他土地命令名称
        /// </summary>
        public const string OtherLandName = "otherLand";

        /// <summary>
        /// 经济地块命令名称
        /// </summary>
        public const string EconomyLandName = "economyLand";

        /// <summary>
        /// 饲料地块命令名称
        /// </summary>
        public const string FodderLandName = "fodderLand";

        /// <summary>
        /// 撂荒地块命令名称
        /// </summary>
        public const string WasteLandName = "wasteLand";

        /// <summary>
        /// 初始数据命令名称
        /// </summary>
        public const string InitialName = "initial";

        /// <summary>
        /// 界址数据命令名称-初始化
        /// </summary>
        public const string BoundaryDataName = "boundaryData";
        
        /// <summary>
        /// 界线数据命令名称-初始化
        /// </summary>
        public const string LandCoilName = "LandCoil";

        /// <summary>
        /// 图幅编号命令名称
        /// </summary>
        public const string MapNumberName = "mapNumber";

        /// <summary>
        /// 面积量算命令名称
        /// </summary>
        public const string AreaMeasurementName = "areaMeasurement";

        /// <summary>
        /// 面积小数截取命令名称
        /// </summary>
        public const string AreaNumericFormatName = "areaNumericFormat";

        /// <summary>
        /// 基本农田命令名称
        /// </summary>
        public const string FarmerLandName = "farmerLand";

        /// <summary>
        /// 地块名称空值命令名称
        /// </summary>
        public const string LandNameNullName = "landNameNull";

        /// <summary>
        /// 二轮合同面积空值命令名称
        /// </summary>
        public const string ContractAreaNullName = "contractAreaNull";

        /// <summary>
        /// 实测面积空值命令名称
        /// </summary>
        public const string ActualAreaNullName = "actualAreaNull";

        /// <summary>
        /// 确权面积空值命令名称
        /// </summary>
        public const string AwareAreaNullName = "awareAreaNull";

        /// <summary>
        /// 基本农田空值命令名称
        /// </summary>
        public const string FarmerLandNullName = "farmerLandNull";

        /// <summary>
        /// 地力等级空值命令名称
        /// </summary>
        public const string LandLevelNullName = "landLevelNull";

        /// <summary>
        /// 地块图斑空值命令名称
        /// </summary>
        public const string LandShapeNullName = "landShapeNull";

        /// <summary>
        /// 空间查看命令名称
        /// </summary>
        public const string FindName = "find";

        /// <summary>
        /// 清空命令名称
        /// </summary>
        public const string ClearName = "clear";

        /// <summary>
        /// 刷新命令名称
        /// </summary>
        public const string RefreshName = "refresh";

        /// <summary>
        /// 下载数据命令名称
        /// </summary>
        public const string DownLoadName = "download";

        /// <summary>
        /// 上传数据命令名称
        /// </summary>
        public const string UpdateDataName = "updatedata";

        /// <summary>
        /// 导出数据压缩包
        /// </summary>
        public const string ExportPackageName = "exportpackage";

        #endregion

        #region Files - Command


        /// <summary>
        /// 导出数据压缩包
        /// </summary>
        public RoutedCommand ExportPackage = new RoutedCommand(ExportPackageName, typeof(Button));

        /// <summary>
        /// 上传数据
        /// </summary>
        public RoutedCommand UpdateData = new RoutedCommand(UpdateDataName, typeof(Button));

        /// <summary>
        /// 下载数据
        /// </summary>
        public RoutedCommand DownLoad = new RoutedCommand(DownLoadName, typeof(Button));

        /// <summary>
        /// 承包地块添加命令
        /// </summary>
        public RoutedCommand ContractLandAdd = new RoutedCommand(ContractLandAddName, typeof(Button));

        /// <summary>
        /// 承包地块编辑命令
        /// </summary>
        public RoutedCommand ContractLandEdit = new RoutedCommand(ContractLandEditName, typeof(Button));

        /// <summary>
        /// 承包地块删除命令
        /// </summary>
        public RoutedCommand ContractLandDel = new RoutedCommand(ContractLandDelName, typeof(Button));

        /// <summary>
        /// 导入(批量)地块调查表命令
        /// </summary>
        public RoutedCommand ImportLandExcel = new RoutedCommand(ImportLandExcelName, typeof(Button));

        /// <summary>
        /// 导入(批量)界址调查表命令
        /// </summary>
        public RoutedCommand ImportBoundaryExcel = new RoutedCommand(ImportBoundaryExcelName, typeof(Button));

        /// <summary>
        /// 导入(批量)矢量调查图斑数据命令
        /// </summary>
        public RoutedCommand ImportVector = new RoutedCommand(ImportVectorName, typeof(Button));

        /// <summary>
        /// 导入(批量)界址点调查图斑数据命令
        /// </summary>
        public RoutedCommand ImportPoints = new RoutedCommand(ImportPointsName, typeof(Button));

        /// <summary>
        /// 导入(批量)界址线调查图斑数据命令
        /// </summary>
        public RoutedCommand ImportLines = new RoutedCommand(ImportLinesName, typeof(Button));

        /// <summary>
        /// 导入(批量)土地调查压缩包数据命令
        /// </summary>
        public RoutedCommand ImportZip = new RoutedCommand(ImportZipName, typeof(Button));

        /// <summary>
        /// 导出当前地域下地块Shape数据命令
        /// </summary>
        public RoutedCommand ExportVectorShape = new RoutedCommand(ExportLandShapeDataName, typeof(Button));

        /// <summary>
        /// 导出当前地域下地块界址点Shape数据命令
        /// </summary>
        public RoutedCommand ExportVectorDotShape = new RoutedCommand(ExportLandDotShapeDataName, typeof(Button));

        /// <summary>
        /// 导出当前地域下地块界址线Shape数据命令
        /// </summary>
        public RoutedCommand ExportVectorCoilShape = new RoutedCommand(ExportLandCoilShapeDataName, typeof(Button));

        /// <summary>
        /// 导出发包方调查模板命令
        /// </summary>
        public RoutedCommand SenderExcelTemplate = new RoutedCommand(SenderExcelTemplateName, typeof(Button));

        /// <summary>
        /// 导出承包方调查模板命令
        /// </summary>
        public RoutedCommand VPExcelTemplate = new RoutedCommand(VPExcelTemplateName, typeof(Button));

        /// <summary>
        /// 导出承包地块调查模板命令
        /// </summary>
        public RoutedCommand LandExcelTemplate = new RoutedCommand(LandExcelTemplateName, typeof(Button));

        /// <summary>
        /// 导出界址信息调查模板命令
        /// </summary>
        public RoutedCommand BoundaryExcelTemplate = new RoutedCommand(BoundaryExcelTemplateName, typeof(Button));

        /// <summary>
        /// 导出发包方调查模板(Word)命令
        /// </summary>
        public RoutedCommand SenderWordTemplate = new RoutedCommand(SenderWordTemplateName, typeof(Button));

        /// <summary>
        /// 导出承包方调查模板(Word)命令
        /// </summary>
        public RoutedCommand VPWordTemplate = new RoutedCommand(VPWordTemplateName, typeof(Button));

        /// <summary>
        /// 导出承包地块调查模板(Word)命令
        /// </summary>
        public RoutedCommand LandWordTemplate = new RoutedCommand(LandWordTemplateName, typeof(Button));

        /// <summary>
        /// 导出(批量)发包方调查表命令
        /// </summary>
        public RoutedCommand ExportSenderExcel = new RoutedCommand(ExportSenderExcelName, typeof(Button));

        /// <summary>
        /// 导出(批量)承包方调查表命令
        /// </summary>
        public RoutedCommand ExportVPExcel = new RoutedCommand(ExportVPExcelName, typeof(Button));

        /// <summary>
        /// 导出(批量)承包地块调查表命令
        /// </summary>
        public RoutedCommand ExportLandExcel = new RoutedCommand(ExportLandExceleName, typeof(Button));

        /// <summary>
        /// 导出(批量)界址信息调查表命令
        /// </summary>
        public RoutedCommand ExportBoundaryExcel = new RoutedCommand(ExportBoundaryExcelName, typeof(Button));

        /// <summary>
        /// 导出(批量)调查信息公示表命令
        /// </summary>
        public RoutedCommand ExportPublishExcel = new RoutedCommand(ExportPublishExcelName, typeof(Button));

        /// <summary>
        /// 导出(批量)发包方调查表(Word)命令
        /// </summary>
        public RoutedCommand ExportSenderWord = new RoutedCommand(ExportSenderWordName, typeof(Button));

        /// <summary>
        /// 导出(批量)承包方调查表(Word)命令
        /// </summary>
        public RoutedCommand ExportVPWord = new RoutedCommand(ExportVPWordName, typeof(Button));

        /// <summary>
        /// 导出(批量)承包地块调查表(Word)命令
        /// </summary>
        public RoutedCommand ExportLandWord = new RoutedCommand(ExportLandWordName, typeof(Button));

        /// <summary>
        /// 导出(批量)公示结果归户表(Word)命令
        /// </summary>
        public RoutedCommand ExportPublishWord = new RoutedCommand(ExportPublishWordName, typeof(Button));

        /// <summary>
        /// 导出(批量)台账调查表命令
        /// </summary>
        public RoutedCommand ExportAccount = new RoutedCommand(ExportAccountName, typeof(Button));

        /// <summary>
        /// 导出(批量)单户调查表命令
        /// </summary>
        public RoutedCommand ExportSingle = new RoutedCommand(ExportSingleName, typeof(Button));

        /// <summary>
        /// 导出(批量)登记公示表命令
        /// </summary>
        public RoutedCommand ExportPublish = new RoutedCommand(ExportPublishName, typeof(Button));

        /// <summary>
        /// 导出(批量)登记签字表命令名称
        /// </summary>
        public RoutedCommand ExportSign = new RoutedCommand(ExportSignName, typeof(Button));

        /// <summary>
        /// 导出(批量)村组公示表命令
        /// </summary>
        public RoutedCommand ExportVillagesPub = new RoutedCommand(ExportVillagesPubName, typeof(Button));

        /// <summary>
        /// 导出(批量)村组公告表命令
        /// </summary>
        public RoutedCommand ExportVillagesDeclare = new RoutedCommand(ExportVillagesDeclareName, typeof(Button));

        /// <summary>
        /// 导出(批量)单户确认表命令
        /// </summary>
        public RoutedCommand ExportSingleAffirm = new RoutedCommand(ExportSingleAffirmName, typeof(Button));

        /// <summary>
        /// 导出(批量)数据汇总表命令
        /// </summary>
        public RoutedCommand ExportSummary = new RoutedCommand(ExportSummaryName, typeof(Button));
        public RoutedCommand CategorySummary = new RoutedCommand(ExportCategorySummary, typeof(Button));

        /// <summary>
        /// 查找四至命令
        /// </summary>
        public RoutedCommand SearchNeighor = new RoutedCommand(SearchNeighorName, typeof(Button));

        /// <summary>
        /// 导出(批量)界址点成果表命令
        /// </summary>
        public RoutedCommand ResultExcelExpot = new RoutedCommand(ResultExcelExpotName, typeof(Button));

        /// <summary>
        /// 示意图设置命令
        /// </summary>
        public RoutedCommand ParcelSetting = new RoutedCommand(ParcelSettingName, typeof(Button));

        /// <summary>
        /// 导出地块示意图小页图命令
        /// </summary>
        public RoutedCommand MultiParcelSmallExport = new RoutedCommand(MultiParcelExportSamllName, typeof(Button));

        /// <summary>
        /// 导出地块示意图命令
        /// </summary>
        public RoutedCommand MultiParcelExport = new RoutedCommand(MultiParcelExportName, typeof(Button));

        /// <summary>
        /// 导出确权确股地块示意图命令
        /// </summary>
        public RoutedCommand MultiParcelStockExport = new RoutedCommand(MultiParcelStockExportName, typeof(Button)); 

        /// <summary>
        /// 全部地块命令
        /// </summary>
        public RoutedCommand AllLand = new RoutedCommand(AllLandName, typeof(Button));

        /// <summary>
        /// 承包地块命令
        /// </summary>
        public RoutedCommand ContractLand = new RoutedCommand(ContractLandName, typeof(Button));

        /// <summary>
        /// 自留地块命令
        /// </summary>
        public RoutedCommand OwnerLand = new RoutedCommand(OwnerLandName, typeof(Button));

        /// <summary>
        /// 机动地块命令
        /// </summary>
        public RoutedCommand MotorLand = new RoutedCommand(MotorLandName, typeof(Button));

        /// <summary>
        /// 开荒地块命令
        /// </summary>
        public RoutedCommand CultivateLand = new RoutedCommand(CultivateLandName, typeof(Button));

        /// <summary>
        /// 其他土地命令
        /// </summary>
        public RoutedCommand OtherLand = new RoutedCommand(OtherLandName, typeof(Button));

        /// <summary>
        /// 经济地块命令
        /// </summary>
        public RoutedCommand EconomyLand = new RoutedCommand(EconomyLandName, typeof(Button));

        /// <summary>
        /// 饲料地块命令
        /// </summary>
        public RoutedCommand FodderLand = new RoutedCommand(FodderLandName, typeof(Button));

        /// <summary>
        /// 撂荒地块命令
        /// </summary>
        public RoutedCommand WasteLand = new RoutedCommand(WasteLandName, typeof(Button));

        /// <summary>
        /// 初始数据命令
        /// </summary>
        public RoutedCommand Initial = new RoutedCommand(InitialName, typeof(Button));

        /// <summary>
        /// 界址数据命令
        /// </summary>
        public RoutedCommand BoundaryData = new RoutedCommand(BoundaryDataName, typeof(Button));

        /// <summary>
        /// 界址数据命令
        /// </summary>
        public RoutedCommand LandCoilData = new RoutedCommand(LandCoilName, typeof(Button));

        /// <summary>
        /// 图幅编号命令
        /// </summary>
        public RoutedCommand MapNumber = new RoutedCommand(MapNumberName, typeof(Button));

        /// <summary>
        /// 面积量算命令
        /// </summary>
        public RoutedCommand AreaMeasurement = new RoutedCommand(AreaMeasurementName, typeof(Button));

        /// <summary>
        /// 面积小数位截取命令
        /// </summary>
        public RoutedCommand AreaNumericFormat = new RoutedCommand(AreaNumericFormatName, typeof(Button));

        /// <summary>
        /// 基本农田命令
        /// </summary>
        public RoutedCommand FarmerLand = new RoutedCommand(FarmerLandName, typeof(Button));

        /// <summary>
        /// 地块名称空值命令
        /// </summary>
        public RoutedCommand LandNameNull = new RoutedCommand(LandNameNullName, typeof(Button));

        /// <summary>
        /// 二轮合同面积空值命令
        /// </summary>
        public RoutedCommand ContractAreaNull = new RoutedCommand(ContractAreaNullName, typeof(Button));

        /// <summary>
        /// 实测面积空值命令
        /// </summary>
        public RoutedCommand ActualAreaNull = new RoutedCommand(ActualAreaNullName, typeof(Button));

        /// <summary>
        /// 确权面积空值命令
        /// </summary>
        public RoutedCommand AwareAreaNull = new RoutedCommand(AwareAreaNullName, typeof(Button));

        /// <summary>
        /// 基本农田空值命令
        /// </summary>
        public RoutedCommand FarmerLandNull = new RoutedCommand(FarmerLandNullName, typeof(Button));

        /// <summary>
        /// 地力等级空值命令
        /// </summary>
        public RoutedCommand LandLevelNull = new RoutedCommand(LandLevelNullName, typeof(Button));

        /// <summary>
        /// 地块图斑空值命令
        /// </summary>
        public RoutedCommand LandShapeNull = new RoutedCommand(LandShapeNullName, typeof(Button));

        /// <summary>
        /// 空间查看命令
        /// </summary>
        public RoutedCommand Find = new RoutedCommand(FindName, typeof(Button));

        /// <summary>
        /// 清空命令
        /// </summary>
        public RoutedCommand Clear = new RoutedCommand(ClearName, typeof(Button));

        /// <summary>
        /// 刷新命令
        /// </summary>
        public RoutedCommand Refresh = new RoutedCommand(RefreshName, typeof(Button));

        #endregion

        #region Files - Binding

        /// <summary>
        /// 上传数据
        /// </summary>
        public CommandBinding ExportPackageBind = new CommandBinding();

        /// <summary>
        /// 上传数据
        /// </summary>
        public CommandBinding UpdateDataBind = new CommandBinding();

        /// <summary>
        /// 下载数据
        /// </summary>
        public CommandBinding DownLoadBind = new CommandBinding();

        /// <summary>
        /// 承包地块添加绑定
        /// </summary>
        public CommandBinding ContractLandAddBind = new CommandBinding();

        /// <summary>
        /// 承包地块编辑绑定
        /// </summary>
        public CommandBinding ContractLandEditBind = new CommandBinding();

        /// <summary>
        /// 承包地块删除绑定
        /// </summary>
        public CommandBinding ContractLandDelBind = new CommandBinding();

        /// <summary>
        /// 导入(批量)地块调查表绑定
        /// </summary>
        public CommandBinding ImportLandExcelBind = new CommandBinding();

        /// <summary>
        /// 导入(批量)界址调查表绑定
        /// </summary>
        public CommandBinding ImportBoundaryExcelBind = new CommandBinding();

        /// <summary>
        /// 导入(批量)矢量调查图斑数据绑定
        /// </summary>
        public CommandBinding ImportVectorBind = new CommandBinding();

        /// <summary>
        /// 导入(批量)界址点调查图斑数据绑定
        /// </summary>
        public CommandBinding ImportPointsBind = new CommandBinding();

        /// <summary>
        /// 导入(批量)界址线调查图斑数据绑定
        /// </summary>
        public CommandBinding ImportLinesBind = new CommandBinding();

        /// <summary>
        /// 导入(批量)土地调查压缩包数据绑定
        /// </summary>
        public CommandBinding ImportZipBind = new CommandBinding();

        /// <summary>
        /// 导出当前地域下Shape数据绑定
        /// </summary>
        public CommandBinding ExportVectorShapeBind = new CommandBinding();

        /// <summary>
        /// 导出当前地域下界址点Shape数据绑定
        /// </summary>
        public CommandBinding ExportVectorDotShapeBind = new CommandBinding();

        /// <summary>
        /// 导出当前地域下界址线Shape数据绑定
        /// </summary>
        public CommandBinding ExportVectorCoilShapeBind = new CommandBinding();

        /// <summary>
        /// 导出发包方调查模板绑定
        /// </summary>
        public CommandBinding SenderExcelTemplateBind = new CommandBinding();

        /// <summary>
        /// 导出承包方调查模板绑定
        /// </summary>
        public CommandBinding VPExcelTemplateBind = new CommandBinding();

        /// <summary>
        /// 导出承包地块调查模板绑定
        /// </summary>
        public CommandBinding LandExcelTemplateBind = new CommandBinding();

        /// <summary>
        /// 导出界址信息调查模板绑定
        /// </summary>
        public CommandBinding BoundaryExcelTemplateBind = new CommandBinding();

        /// <summary>
        /// 导出发包方调查模板(Word)绑定
        /// </summary>
        public CommandBinding SenderWordTemplateBind = new CommandBinding();

        /// <summary>
        /// 导出承包方调查模板(Word)绑定
        /// </summary>
        public CommandBinding VPWordTemplateBind = new CommandBinding();

        /// <summary>
        /// 导出承包地块调查模板(Word)绑定
        /// </summary>
        public CommandBinding LandWordTemplateBind = new CommandBinding();

        /// <summary>
        /// 导出(批量)发包方调查表绑定
        /// </summary>
        public CommandBinding ExportSenderExcelBind = new CommandBinding();

        /// <summary>
        /// 导出(批量)承包方调查表绑定
        /// </summary>
        public CommandBinding ExportVPExcelBind = new CommandBinding();

        /// <summary>
        /// 导出(批量)承包地块调查表绑定
        /// </summary>
        public CommandBinding ExportLandExcelBind = new CommandBinding();

        /// <summary>
        /// 导出(批量)界址信息调查表绑定
        /// </summary>
        public CommandBinding ExportBoundaryExcelBind = new CommandBinding();

        /// <summary>
        /// 导出(批量)调查信息公示表绑定
        /// </summary>
        public CommandBinding ExportPublishExcelBind = new CommandBinding();

        /// <summary>
        /// 导出(批量)发包方调查表(Word)绑定
        /// </summary>
        public CommandBinding ExportSenderWordBind = new CommandBinding();

        /// <summary>
        /// 导出(批量)承包方调查表(Word)绑定
        /// </summary>
        public CommandBinding ExportVPWordBind = new CommandBinding();

        /// <summary>
        /// 导出(批量)承包地块调查表(Word)绑定
        /// </summary>
        public CommandBinding ExportLandWordBind = new CommandBinding();

        /// <summary>
        /// 导出(批量)公示结果归户表(Word)绑定
        /// </summary>
        public CommandBinding ExportPublishWordBind = new CommandBinding();

        /// <summary>
        /// 导出(批量)台账调查表绑定
        /// </summary>
        public CommandBinding ExportAccountBind = new CommandBinding();

        /// <summary>
        /// 导出(批量)单户调查表绑定
        /// </summary>
        public CommandBinding ExportSingleBind = new CommandBinding();

        /// <summary>
        /// 导出(批量)登记公示表绑定
        /// </summary>
        public CommandBinding ExportPublishBind = new CommandBinding();

        /// <summary>
        ///  导出(批量)登记签字表命令绑定
        /// </summary>
        public CommandBinding ExportSignBind = new CommandBinding();

        /// <summary>
        /// 导出(批量)村组公示表绑定
        /// </summary>
        public CommandBinding ExportVillagesPubBind = new CommandBinding();

        /// <summary>
        /// 导出(批量)村组公告表绑定
        /// </summary>
        public CommandBinding ExportVillagesDeclareBind = new CommandBinding();

        /// <summary>
        /// 导出(批量)单户确认表绑定
        /// </summary>
        public CommandBinding ExportSingleAffirmBind = new CommandBinding();

        /// <summary>
        /// 导出(批量)数据汇总表绑定
        /// </summary>
        public CommandBinding ExportSummaryBind = new CommandBinding();
        public CommandBinding ExportCategoryBind = new CommandBinding();

        /// <summary>
        /// 查找四至绑定
        /// </summary>
        public CommandBinding SearchNeighorBind = new CommandBinding();

        /// <summary>
        /// 导出(批量)界址点成果表绑定
        /// </summary>
        public CommandBinding ResultExcelExpotBind = new CommandBinding();

        /// <summary>
        /// 示意图设置绑定
        /// </summary>
        public CommandBinding ParcelSettingBind = new CommandBinding();

        /// <summary>
        /// 导出地块示意图小页图绑定
        /// </summary>
        public CommandBinding MultiParcelExportSmallBind = new CommandBinding();

        /// <summary>
        /// 导出地块示意图绑定
        /// </summary>
        public CommandBinding MultiParcelExportBind = new CommandBinding();

        /// <summary>
        /// 导出确权确股地块示意图绑定
        /// </summary>
        public CommandBinding MultiParcelStockExportBind = new CommandBinding();

        /// <summary>
        /// 全部地块绑定
        /// </summary>
        public CommandBinding AllLandBind = new CommandBinding();

        /// <summary>
        /// 承包地块绑定
        /// </summary>
        public CommandBinding ContractLandBind = new CommandBinding();

        /// <summary>
        /// 自留地块绑定
        /// </summary>
        public CommandBinding OwnerLandBind = new CommandBinding();

        /// <summary>
        /// 机动地块绑定
        /// </summary>
        public CommandBinding MotorLandBind = new CommandBinding();

        /// <summary>
        /// 开荒地块绑定
        /// </summary>
        public CommandBinding CultivateLandBind = new CommandBinding();

        /// <summary>
        /// 其他土地绑定
        /// </summary>
        public CommandBinding OtherLandBind = new CommandBinding();

        /// <summary>
        /// 经济地块绑定
        /// </summary>
        public CommandBinding EconomyLandBind = new CommandBinding();

        /// <summary>
        /// 饲料地块绑定
        /// </summary>
        public CommandBinding FodderLandBind = new CommandBinding();

        /// <summary>
        /// 撂荒地块绑定
        /// </summary>
        public CommandBinding WasteLandBind = new CommandBinding();

        /// <summary>
        /// 初始数据绑定
        /// </summary>
        public CommandBinding InitialBind = new CommandBinding();

        /// <summary>
        /// 界址数据绑定
        /// </summary>
        public CommandBinding BoundaryDataBind = new CommandBinding();
        
        /// <summary>
        /// 界线数据绑定
        /// </summary>
        public CommandBinding LandCoilBind = new CommandBinding();

        /// <summary>
        /// 图幅编号绑定
        /// </summary>
        public CommandBinding MapNumberBind = new CommandBinding();

        /// <summary>
        /// 面积量算绑定
        /// </summary>
        public CommandBinding AreaMeasurementBind = new CommandBinding();

        /// <summary>
        /// 面积量算绑定
        /// </summary>
        public CommandBinding AreaNumericFormatBind = new CommandBinding();

        /// <summary>
        /// 基本农田绑定
        /// </summary>
        public CommandBinding FarmerLandBind = new CommandBinding();

        /// <summary>
        /// 地块名称空值绑定
        /// </summary>
        public CommandBinding LandNameNullBind = new CommandBinding();

        /// <summary>
        /// 二轮合同面积空值绑定
        /// </summary>
        public CommandBinding ContractAreaNullBind = new CommandBinding();

        /// <summary>
        /// 实测面积空值绑定
        /// </summary>
        public CommandBinding ActualAreaNullBind = new CommandBinding();

        /// <summary>
        /// 确权面积空值绑定
        /// </summary>
        public CommandBinding AwareAreaNullBind = new CommandBinding();

        /// <summary>
        /// 基本农田空值绑定
        /// </summary>
        public CommandBinding FarmerLandNullBind = new CommandBinding();

        /// <summary>
        /// 地力等级空值绑定
        /// </summary>
        public CommandBinding LandLevelNullBind = new CommandBinding();

        /// <summary>
        /// 地块图斑空值绑定
        /// </summary>
        public CommandBinding LandShapeNullBind = new CommandBinding();

        /// <summary>
        /// 空间查看绑定
        /// </summary>
        public CommandBinding FindBind = new CommandBinding();

        /// <summary>
        /// 清空绑定
        /// </summary>
        public CommandBinding ClearBind = new CommandBinding();

        /// <summary>
        /// 刷新绑定
        /// </summary>
        public CommandBinding RefreshBind = new CommandBinding();

        #endregion

        #region Properties

        #endregion

        #region Ctor

        /// <summary>
        /// 构造方法
        /// </summary>
        public ContractAccountCommand()
        {
            InstallCommand();
        }

        #endregion

        #region Install

        /// <summary>
        /// 将命令设置到绑定上
        /// </summary>
        public void InstallCommand()
        {
            UpdateDataBind.Command = UpdateData;
            DownLoadBind.Command = DownLoad;
            ContractLandAddBind.Command = ContractLandAdd;
            ContractLandEditBind.Command = ContractLandEdit;
            ContractLandDelBind.Command = ContractLandDel;
            ImportLandExcelBind.Command = ImportLandExcel;
            ImportBoundaryExcelBind.Command = ImportBoundaryExcel;
            ImportVectorBind.Command = ImportVector;
            ImportPointsBind.Command = ImportPoints;
            ImportLinesBind.Command = ImportLines;
            ImportZipBind.Command = ImportZip;
            ExportVectorShapeBind.Command = ExportVectorShape;
            ExportVectorDotShapeBind.Command = ExportVectorDotShape;
            ExportVectorCoilShapeBind.Command = ExportVectorCoilShape;
            SenderExcelTemplateBind.Command = SenderExcelTemplate;
            VPExcelTemplateBind.Command = VPExcelTemplate;
            LandExcelTemplateBind.Command = LandExcelTemplate;
            BoundaryExcelTemplateBind.Command = BoundaryExcelTemplate;
            SenderWordTemplateBind.Command = SenderWordTemplate;
            VPWordTemplateBind.Command = VPWordTemplate;
            LandWordTemplateBind.Command = LandWordTemplate;
            ExportSenderExcelBind.Command = ExportSenderExcel;
            ExportVPExcelBind.Command = ExportVPExcel;
            ExportLandExcelBind.Command = ExportLandExcel;
            ExportBoundaryExcelBind.Command = ExportBoundaryExcel;
            ExportPublishExcelBind.Command = ExportPublishExcel;
            ExportSenderWordBind.Command = ExportSenderWord;
            ExportVPWordBind.Command = ExportVPWord;
            ExportLandWordBind.Command = ExportLandWord;
            ExportPublishWordBind.Command = ExportPublishWord;
            ExportAccountBind.Command = ExportAccount;
            ExportSingleBind.Command = ExportSingle;
            ExportPublishBind.Command = ExportPublish;
            ExportSignBind.Command = ExportSign;
            ExportVillagesPubBind.Command = ExportVillagesPub;
            ExportVillagesDeclareBind.Command = ExportVillagesDeclare;
            ExportSingleAffirmBind.Command = ExportSingleAffirm;
            ExportSummaryBind.Command = ExportSummary;
            ExportCategoryBind.Command = CategorySummary;
            SearchNeighorBind.Command = SearchNeighor;
            ResultExcelExpotBind.Command = ResultExcelExpot;
            ParcelSettingBind.Command = ParcelSetting;
            MultiParcelExportSmallBind.Command = MultiParcelSmallExport;
            MultiParcelExportBind.Command = MultiParcelExport;
            AllLandBind.Command = AllLand;
            ContractLandBind.Command = ContractLand;
            OwnerLandBind.Command = OwnerLand;
            MotorLandBind.Command = MotorLand;
            CultivateLandBind.Command = CultivateLand;
            OtherLandBind.Command = OtherLand;
            EconomyLandBind.Command = EconomyLand;
            FodderLandBind.Command = FodderLand;
            WasteLandBind.Command = WasteLand;
            InitialBind.Command = Initial;
            BoundaryDataBind.Command = BoundaryData;
            LandCoilBind.Command = LandCoilData;
            MapNumberBind.Command = MapNumber;
            AreaMeasurementBind.Command = AreaMeasurement;
            AreaNumericFormatBind.Command = AreaNumericFormat;
            FarmerLandBind.Command = FarmerLand;
            LandNameNullBind.Command = LandNameNull;
            ContractAreaNullBind.Command = ContractAreaNull;
            ActualAreaNullBind.Command = ActualAreaNull;
            AwareAreaNullBind.Command = AwareAreaNull;
            FarmerLandNullBind.Command = FarmerLandNull;
            LandLevelNullBind.Command = LandLevelNull;
            LandShapeNullBind.Command = LandShapeNull;
            FindBind.Command = Find;
            ClearBind.Command = Clear;
            RefreshBind.Command = Refresh;
            ExportPackageBind.Command = ExportPackage;
            MultiParcelStockExportBind.Command = MultiParcelStockExport;
        }

        #endregion
    }
}
