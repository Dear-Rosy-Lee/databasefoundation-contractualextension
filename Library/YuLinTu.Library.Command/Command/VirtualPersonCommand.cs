/*
 * (C) 2015  鱼鳞图公司版权所有,保留所有权利
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace YuLinTu.Library.Command
{
    /// <summary>
    /// 发包方模块命令定义
    /// </summary>
    public class VirtualPersonCommand
    {
        #region Files - Const

        /// <summary>
        /// 添加命令名称
        /// </summary>
        public const string AddName = "Add";

        /// <summary>
        /// 编辑命令名称
        /// </summary>
        public const string EditName = "Edit";

        /// <summary>
        /// 删除命令名称
        /// </summary>
        public const string DelName = "Del";

        /// <summary>
        /// 导入调查表命令名称
        /// </summary>
        public const string ImportName = "ImportExcel";

        /// <summary>
        /// 导出Excel模板
        /// </summary>
        public const string ExcelTemplateName = "ExcelTemplate";

        /// <summary>
        /// 导出Word模板
        /// </summary>
        public const string WordTemplateName = "WordTemplate";

        /// <summary>
        /// 导出Excel数据
        /// </summary>
        public const string ExportExcelName = "ExportExcel";

        /// <summary>
        /// 导出Word数据
        /// </summary>
        public const string ExportTableName = "ExportTable";

        /// <summary>
        /// 清理命令名称
        /// </summary>
        public const string ClearName = "Clear";

        /// <summary>
        /// 地域刷新
        /// </summary>
        public const string RefreshName = "Refresh";

        /// <summary>
        /// 声明书预览
        /// </summary>
        public const string ApplyPreviewName = "ApplyPreview";

        /// <summary>
        /// 声明书导出
        /// </summary>
        public const string ApplyExportName = "ApplyExport";

        /// <summary>
        /// 声明书打印
        /// </summary>
        public const string ApplyPrintName = "ApplyPrint";

        /// <summary>
        /// 委托书预览
        /// </summary>
        public const string DelegatePreviewName = "DelegatePreview";

        /// <summary>
        /// 委托书导出
        /// </summary>
        public const string DelegateExportName = "DelegateExport";

        /// <summary>
        /// 委托书打印
        /// </summary>
        public const string DelegatePrintName = "DelegatePrint";

        /// <summary>
        /// 无异议书预览
        /// </summary>
        public const string IdeaPreviewName = "IdeaPreview";

        /// <summary>
        /// 无异议书导出
        /// </summary>
        public const string IdeaExportName = "IdeaExport";

        /// <summary>
        /// 无异议书打印
        /// </summary>
        public const string IdeaPrintName = "IdeaPrint";

        /// <summary>
        /// 测绘书预览
        /// </summary>
        public const string SurveyPreviewName = "SurveyPreview";

        /// <summary>
        /// 测绘书导出
        /// </summary>
        public const string SurveyExportName = "SurveyExport";

        /// <summary>
        /// 测绘书打印
        /// </summary>
        public const string SurveyPrintName = "SurveyPrint";

        /// <summary>
        /// 分户
        /// </summary>
        public const string SplitFamilyName = "SplitFamily";

        /// <summary>
        /// 合户
        /// </summary>
        public const string CombineFamilyName = "CombineFamily";

        /// <summary>
        /// 锁户
        /// </summary>
        public const string LockedFamilyName = "LockedFamily";

        /// <summary>
        /// 设置户主
        /// </summary>
        public const string SetFamilyName = "SetFamily";

        /// <summary>
        /// 查询数据
        /// </summary>
        public const string SearchDataName = "SearchData";

        /// <summary>
        /// 初始数据
        /// </summary>
        public const string InitiallDataName = "InitiallData";

        /// <summary>
        /// 承包经营权
        /// </summary>
        public const string ContractName = "Contract";

        /// <summary>
        /// 林权
        /// </summary>
        public const string WoodName = "Wood";

        /// <summary>
        /// 建设用地
        /// </summary>
        public const string YardName = "Yard";

        /// <summary>
        /// 宅基地
        /// </summary>
        public const string HouseName = "House";

        /// <summary>
        /// 集体土地
        /// </summary>
        public const string CollectiveName = "Collective";

        /// <summary>
        /// 家庭关系检查
        /// </summary>
        public const string RelationCheckName = "RelationCheck";

        /// <summary>
        /// 家庭关系替换
        /// </summary>
        public const string RelationReplaceName = "RelationReplace";

        public const string SharePersonRepairName = "SharePersonRepair";

        #endregion Files - Const

        #region Files - Command

        /// <summary>
        /// 添加命令
        /// </summary>
        public RoutedCommand Add = new RoutedCommand(AddName, typeof(Button));

        /// <summary>
        /// 编辑命令名称
        /// </summary>
        public RoutedCommand Edit = new RoutedCommand(EditName, typeof(Button));

        /// <summary>
        /// 删除命令名称
        /// </summary>
        public RoutedCommand Del = new RoutedCommand(DelName, typeof(Button));

        /// <summary>
        /// 导入调查表命令
        /// </summary>
        public RoutedCommand Import = new RoutedCommand(ImportName, typeof(Button));

        /// <summary>
        /// 导出Excel模板
        /// </summary>
        public RoutedCommand ExcelTemplate = new RoutedCommand(ExcelTemplateName, typeof(Button));

        /// <summary>
        /// 导出Word模板
        /// </summary>
        public RoutedCommand WordTemplate = new RoutedCommand(WordTemplateName, typeof(Button));

        /// <summary>
        /// 导出Excel数据
        /// </summary>
        public RoutedCommand ExportExcel = new RoutedCommand(ExportExcelName, typeof(Button));

        /// <summary>
        /// 导出Word数据
        /// </summary>
        public RoutedCommand ExportTable = new RoutedCommand(ExportTableName, typeof(Button));

        /// <summary>
        /// 清理命令名称
        /// </summary>
        public RoutedCommand Clear = new RoutedCommand(ClearName, typeof(Button));

        /// <summary>
        /// 刷新
        /// </summary>
        public RoutedCommand Refresh = new RoutedCommand(RefreshName, typeof(Button));

        /// <summary>
        /// 声明书预览
        /// </summary>
        public RoutedCommand ApplyPreview = new RoutedCommand(ApplyPreviewName, typeof(Button));

        /// <summary>
        /// 声明书导出
        /// </summary>
        public RoutedCommand ApplyExport = new RoutedCommand(ApplyExportName, typeof(Button));

        /// <summary>
        /// 声明书打印
        /// </summary>
        public RoutedCommand ApplyPrint = new RoutedCommand(ApplyPrintName, typeof(Button));

        /// <summary>
        /// 委托书预览
        /// </summary>
        public RoutedCommand DelegatePreview = new RoutedCommand(DelegatePreviewName, typeof(Button));

        /// <summary>
        /// 委托书导出
        /// </summary>
        public RoutedCommand DelegateExport = new RoutedCommand(DelegateExportName, typeof(Button));

        /// <summary>
        /// 委托书打印
        /// </summary>
        public RoutedCommand DelegatePrint = new RoutedCommand(DelegatePrintName, typeof(Button));

        /// <summary>
        /// 无异议书预览
        /// </summary>
        public RoutedCommand IdeaPreview = new RoutedCommand(IdeaPreviewName, typeof(Button));

        /// <summary>
        /// 无异议书导出
        /// </summary>
        public RoutedCommand IdeaExport = new RoutedCommand(IdeaExportName, typeof(Button));

        /// <summary>
        /// 无异议书打印
        /// </summary>
        public RoutedCommand IdeaPrint = new RoutedCommand(IdeaPrintName, typeof(Button));

        /// <summary>
        /// 测绘书预览
        /// </summary>
        public RoutedCommand SurveyPreview = new RoutedCommand(SurveyPreviewName, typeof(Button));

        /// <summary>
        /// 测绘书导出
        /// </summary>
        public RoutedCommand SurveyExport = new RoutedCommand(SurveyExportName, typeof(Button));

        /// <summary>
        /// 测绘书打印
        /// </summary>
        public RoutedCommand SurveyPrint = new RoutedCommand(SurveyPrintName, typeof(Button));

        /// <summary>
        /// 分户
        /// </summary>
        public RoutedCommand SplitFamily = new RoutedCommand(SplitFamilyName, typeof(Button));

        /// <summary>
        /// 合户
        /// </summary>
        public RoutedCommand CombineFamily = new RoutedCommand(CombineFamilyName, typeof(Button));

        /// <summary>
        /// 锁户
        /// </summary>
        public RoutedCommand LockedFamily = new RoutedCommand(LockedFamilyName, typeof(Button));

        /// <summary>
        /// 设置户主
        /// </summary>
        public RoutedCommand SetFamily = new RoutedCommand(SetFamilyName, typeof(Button));

        /// <summary>
        /// 查询数据
        /// </summary>
        public RoutedCommand SearchData = new RoutedCommand(SearchDataName, typeof(Button));

        /// <summary>
        /// 初始数据
        /// </summary>
        public RoutedCommand InitiallData = new RoutedCommand(InitiallDataName, typeof(Button));

        #region 权利选择

        /// <summary>
        /// 承包经营权
        /// </summary>
        public RoutedCommand Contract = new RoutedCommand(ContractName, typeof(Button));

        /// <summary>
        /// 林权
        /// </summary>
        public RoutedCommand Wood = new RoutedCommand(WoodName, typeof(Button));

        /// <summary>
        /// 建设用地
        /// </summary>
        public RoutedCommand Yard = new RoutedCommand(YardName, typeof(Button));

        /// <summary>
        /// 宅基地
        /// </summary>
        public RoutedCommand House = new RoutedCommand(HouseName, typeof(Button));

        /// <summary>
        /// 集体土地
        /// </summary>
        public RoutedCommand Collective = new RoutedCommand(CollectiveName, typeof(Button));

        /// <summary>
        /// 家庭关系处理
        /// </summary>
        public RoutedCommand RelationCheck = new RoutedCommand(RelationCheckName, typeof(Button));

        public RoutedCommand RelationReplace = new RoutedCommand(RelationReplaceName, typeof(Button));

        /// <summary>
        /// 家庭成员数据修复
        /// </summary>
        public RoutedCommand SharePersonRepair = new RoutedCommand(SharePersonRepairName, typeof(Button));

        #endregion 权利选择

        #endregion Files - Command

        #region Files - Binding

        /// <summary>
        /// 添加命令
        /// </summary>
        public CommandBinding AddBind = new CommandBinding();

        /// <summary>
        /// 编辑命令名称
        /// </summary>
        public CommandBinding EditBind = new CommandBinding();

        /// <summary>
        /// 删除命令名称
        /// </summary>
        public CommandBinding DelBind = new CommandBinding();

        /// <summary>
        /// 导入调查表命令名称
        /// </summary>
        public CommandBinding ImportBind = new CommandBinding();

        /// <summary>
        /// 导出Excel模板
        /// </summary>
        public CommandBinding ExcelTemplateBind = new CommandBinding();

        /// <summary>
        /// 导出Word模板
        /// </summary>
        public CommandBinding WordTemplateBind = new CommandBinding();

        /// <summary>
        /// 导出Excel数据
        /// </summary>
        public CommandBinding ExportExcelBind = new CommandBinding();

        /// <summary>
        /// 导出Word数据
        /// </summary>
        public CommandBinding ExportTableBind = new CommandBinding();

        /// <summary>
        /// 清理命令名称
        /// </summary>
        public CommandBinding ClearBind = new CommandBinding();

        /// <summary>
        /// 刷新
        /// </summary>
        public CommandBinding RefreshBind = new CommandBinding();

        /// <summary>
        /// 声明书预览
        /// </summary>
        public CommandBinding ApplyPreviewBind = new CommandBinding();

        /// <summary>
        /// 声明书导出
        /// </summary>
        public CommandBinding ApplyExportBind = new CommandBinding();

        /// <summary>
        /// 声明书打印
        /// </summary>
        public CommandBinding ApplyPrintBind = new CommandBinding();

        /// <summary>
        /// 委托书预览
        /// </summary>
        public CommandBinding DelegatePreviewBind = new CommandBinding();

        /// <summary>
        /// 委托书导出
        /// </summary>
        public CommandBinding DelegateExportBind = new CommandBinding();

        /// <summary>
        /// 委托书打印
        /// </summary>
        public CommandBinding DelegatePrintBind = new CommandBinding();

        /// <summary>
        /// 无异议书预览
        /// </summary>
        public CommandBinding IdeaPreviewBind = new CommandBinding();

        /// <summary>
        /// 无异议书导出
        /// </summary>
        public CommandBinding IdeaExportBind = new CommandBinding();

        /// <summary>
        /// 无异议书打印
        /// </summary>
        public CommandBinding IdeaPrintBind = new CommandBinding();

        /// <summary>
        /// 测绘书预览
        /// </summary>
        public CommandBinding SurveyPreviewBind = new CommandBinding();

        /// <summary>
        /// 测绘书导出
        /// </summary>
        public CommandBinding SurveyExportBind = new CommandBinding();

        /// <summary>
        /// 测绘书打印
        /// </summary>
        public CommandBinding SurveyPrintBind = new CommandBinding();

        /// <summary>
        /// 分户
        /// </summary>
        public CommandBinding SplitFamilyBind = new CommandBinding();

        /// <summary>
        /// 合户
        /// </summary>
        public CommandBinding CombineFamilyBind = new CommandBinding();

        /// <summary>
        /// 锁户
        /// </summary>
        public CommandBinding LockedFamilyBind = new CommandBinding();

        /// <summary>
        /// 设置户主
        /// </summary>
        public CommandBinding SetFamilyBind = new CommandBinding();

        /// <summary>
        /// 查询数据
        /// </summary>
        public CommandBinding SearchDataBind = new CommandBinding();

        /// <summary>
        /// 初始数据
        /// </summary>
        public CommandBinding InitiallDataBind = new CommandBinding();

        #region 权利选择

        /// <summary>
        /// 承包经营权
        /// </summary>
        public CommandBinding ContractBind = new CommandBinding();

        /// <summary>
        /// 林权
        /// </summary>
        public CommandBinding WoodBind = new CommandBinding();

        /// <summary>
        /// 建设用地
        /// </summary>
        public CommandBinding YardBind = new CommandBinding();

        /// <summary>
        /// 宅基地
        /// </summary>
        public CommandBinding HouseBind = new CommandBinding();

        /// <summary>
        /// 集体土地
        /// </summary>
        public CommandBinding CollectiveBind = new CommandBinding();

        /// <summary>
        /// 家庭关系处理
        /// </summary>
        public CommandBinding RelationCheckBind = new CommandBinding();

        public CommandBinding RelationReplaceBind = new CommandBinding();

        public CommandBinding SharePersonRepairBind = new CommandBinding();

        #endregion 权利选择

        #endregion Files - Binding

        #region Properties

        #endregion Properties

        #region Ctor

        /// <summary>
        /// 构造方法
        /// </summary>
        public VirtualPersonCommand()
        {
            InstallCommand();
        }

        #endregion Ctor

        #region Install

        /// <summary>
        /// 将命令设置到绑定上
        /// </summary>
        public void InstallCommand()
        {
            AddBind.Command = Add;
            EditBind.Command = Edit;
            DelBind.Command = Del;
            ImportBind.Command = Import;
            ExcelTemplateBind.Command = ExcelTemplate;
            WordTemplateBind.Command = WordTemplate;
            ExportExcelBind.Command = ExportExcel;
            ExportTableBind.Command = ExportTable;
            ClearBind.Command = Clear;
            RefreshBind.Command = Refresh;
            ApplyPreviewBind.Command = ApplyPreview;
            ApplyExportBind.Command = ApplyExport;
            ApplyPrintBind.Command = ApplyPrint;
            DelegatePreviewBind.Command = DelegatePreview;
            DelegateExportBind.Command = DelegateExport;
            DelegatePrintBind.Command = DelegatePrint;
            IdeaPreviewBind.Command = IdeaPreview;
            IdeaExportBind.Command = IdeaExport;
            IdeaPrintBind.Command = IdeaPrint;
            SurveyPreviewBind.Command = SurveyPreview;
            SurveyExportBind.Command = SurveyExport;
            SurveyPrintBind.Command = SurveyPrint;
            SplitFamilyBind.Command = SplitFamily;
            CombineFamilyBind.Command = CombineFamily;
            LockedFamilyBind.Command = LockedFamily;
            SetFamilyBind.Command = SetFamily;
            SearchDataBind.Command = SearchData;
            InitiallDataBind.Command = InitiallData;

            #region 权利选择

            ContractBind.Command = Contract;
            WoodBind.Command = Wood;
            YardBind.Command = Yard;
            HouseBind.Command = House;
            CollectiveBind.Command = Collective;
            RelationCheckBind.Command = RelationCheck;
            RelationReplaceBind.Command = RelationReplace;

            SharePersonRepairBind.Command = SharePersonRepair;

            #endregion 权利选择
        }

        #endregion Install
    }
}