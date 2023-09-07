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
    /// 承包合同模块命令定义
    /// </summary>
    public class ConcordCommand
    {
        #region Files - Const

        #region 基本操作

        /// <summary>
        /// 添加命令名称
        /// </summary>
        public const string AddName = "Add";

        /// <summary>
        /// 编辑命令名称
        /// </summary>
        public const string EditName = "Edit";

        /// <summary>
        /// 合同删除命令名称
        /// </summary>
        public const string DelName = "Del";

        /// <summary>
        /// 清理命令名称
        /// </summary>
        public const string ClearName = "Clear";

        /// <summary>
        /// 合同刷新
        /// </summary>
        public const string RefreshName = "Refresh";

        #endregion

        #region 合同数据处理

        /// <summary>
        /// 签订合同
        /// </summary>
        public const string ContactConcordName = "ContactConcord";

        /// <summary>
        /// 预览合同
        /// </summary>
        public const string PreviewConcordName = "PreviewConcord";

        /// <summary>
        /// 导出合同
        /// </summary>
        public const string ExportConcordName = "ExportConcord";

        /// <summary>
        /// 打印合同
        /// </summary>
        public const string PrintConcordName = "PrintConcord";

        #endregion

        #region 家庭承包方式

        /// <summary>
        /// 预览集体申请书
        /// </summary>
        public const string PrintViewApplicationName = "PrintViewApplication";

        /// <summary>
        /// 导出集体申请书
        /// </summary>
        public const string ExportApplicationBookName = "ExportApplicationBook";

        /// <summary>
        /// 打印集体申请书
        /// </summary>
        public const string PrintApplicationName = "PrintApplication";

        /// <summary>
        /// 预览单户申请书
        /// </summary>
        public const string PrintRequireBookName = "PrintRequireBook";

        /// <summary>
        /// 导出单户申请书
        /// </summary>
        public const string ExportApplicationByFamilyName = "ExportApplicationByFamily";

        /// <summary>
        /// 打印单户申请书
        /// </summary>
        public const string BatchPrintAppFamilyName = "BatchPrintAppFamily";

        #endregion

        #region 其他承包方式

        /// <summary>
        /// 预览单户申请书
        /// </summary>
        public const string PrintViewOtherApplicationName = "PrintViewOtherApplication";

        /// <summary>
        /// 导出单户申请书
        /// </summary>
        public const string ExportApplicationByOtherName = "ExportApplicationByOther";

        /// <summary>
        /// 打印单户申请书
        /// </summary>
        public const string PrintOtherApplicationName = "PrintOtherApplication";

        #endregion

        #region 工具

        /// <summary>
        /// 发包方管理
        /// </summary>
        public const string SenderName = "Sender";

        /// <summary>
        /// 公示归户表
        /// </summary>
        public const string PublicityResultTableName = "PublicityResultTable";

        /// <summary>
        /// 合同明细表
        /// </summary>
        public const string ConcordInformationName = "ConcordInformation";

        #endregion

        #endregion

        #region Files - Command

        #region 基本管理

        /// <summary>
        /// 合同添加
        /// </summary>
        public RoutedCommand Add = new RoutedCommand(AddName, typeof(Button));

        /// <summary>
        /// 合同编辑
        /// </summary>
        public RoutedCommand Edit = new RoutedCommand(EditName, typeof(Button));

        /// <summary>
        /// 合同删除
        /// </summary>
        public RoutedCommand Del = new RoutedCommand(DelName, typeof(Button));

        /// <summary>
        /// 清理
        /// </summary>
        public RoutedCommand Clear = new RoutedCommand(ClearName, typeof(Button));

        /// <summary>
        /// 合同刷新
        /// </summary>
        public RoutedCommand Refresh = new RoutedCommand(RefreshName, typeof(Button));

        #endregion

        #region 合同数据处理

        /// <summary>
        /// 签订合同
        /// </summary>
        public RoutedCommand ContactConcord = new RoutedCommand(ContactConcordName, typeof(Button));

        /// <summary>
        /// 预览合同
        /// </summary>
        public RoutedCommand PreviewConcord = new RoutedCommand(PreviewConcordName, typeof(Button));

        /// <summary>
        /// 导出合同
        /// </summary>
        public RoutedCommand ExportConcord = new RoutedCommand(ExportConcordName, typeof(Button));

        /// <summary>
        /// 打印合同
        /// </summary>
        public RoutedCommand PrintConcord = new RoutedCommand(PrintConcordName, typeof(Button));

        #endregion

        #region 家庭承包方式

        /// <summary>
        /// 预览集体申请书
        /// </summary>
        public RoutedCommand PrintViewApplication = new RoutedCommand(PrintViewApplicationName, typeof(Button));

        /// <summary>
        /// 导出集体申请书
        /// </summary>
        public RoutedCommand ExportApplicationBook = new RoutedCommand(ExportApplicationBookName, typeof(Button));

        /// <summary>
        /// 打印集体申请书
        /// </summary>
        public RoutedCommand PrintApplication = new RoutedCommand(PrintApplicationName, typeof(Button));

        /// <summary>
        /// 预览单户申请书
        /// </summary>
        public RoutedCommand PrintRequireBook = new RoutedCommand(PrintRequireBookName, typeof(Button));

        /// <summary>
        /// 导出单户申请书
        /// </summary>
        public RoutedCommand ExportApplicationByFamily = new RoutedCommand(ExportApplicationByFamilyName, typeof(Button));

        /// <summary>
        /// 打印单户申请书
        /// </summary>
        public RoutedCommand BatchPrintAppFamily = new RoutedCommand(BatchPrintAppFamilyName, typeof(Button));


        #endregion

        #region 其他承包方式

        /// <summary>
        /// 预览单户申请书
        /// </summary>
        public RoutedCommand PrintViewOtherApplication = new RoutedCommand(PrintViewOtherApplicationName, typeof(Button));

        /// <summary>
        /// 导出单户申请书
        /// </summary>
        public RoutedCommand ExportApplicationByOther = new RoutedCommand(ExportApplicationByOtherName, typeof(Button));

        /// <summary>
        /// 打印单户申请书
        /// </summary>
        public RoutedCommand PrintOtherApplication = new RoutedCommand(PrintOtherApplicationName, typeof(Button));

        #endregion

        #region 工具

        /// <summary>
        /// 发包方管理
        /// </summary>
        public RoutedCommand Sender = new RoutedCommand(SenderName, typeof(Button));

        /// <summary>
        /// 公示归户表
        /// </summary>
        public RoutedCommand PublicityResultTable = new RoutedCommand(PublicityResultTableName, typeof(Button));

        /// <summary>
        /// 合同明细表
        /// </summary>
        public RoutedCommand ConcordInformation = new RoutedCommand(ConcordInformationName, typeof(Button));

        #endregion

        #endregion

        #region Files - Binding

        #region 基本管理

        /// <summary>
        /// 合同添加
        /// </summary>
        public CommandBinding AddBind = new CommandBinding();

        /// <summary>
        /// 合同编辑
        /// </summary>
        public CommandBinding EditBind = new CommandBinding();

        /// <summary>
        /// 合同删除
        /// </summary>
        public CommandBinding DelBind = new CommandBinding();

        /// <summary>
        /// 清理
        /// </summary>
        public CommandBinding ClearBind = new CommandBinding();

        /// <summary>
        /// 合同刷新
        /// </summary>
        public CommandBinding RefreshBind = new CommandBinding();

        #endregion

        #region 合同数据处理

        /// <summary>
        /// 签订合同
        /// </summary>
        public CommandBinding ContactConcordBind = new CommandBinding();

        /// <summary>
        /// 预览合同
        /// </summary>
        public CommandBinding PreviewConcordBind = new CommandBinding();

        /// <summary>
        /// 导出合同
        /// </summary>
        public CommandBinding ExportConcordBind = new CommandBinding();

        /// <summary>
        /// 打印合同
        /// </summary>
        public CommandBinding PrintConcordBind = new CommandBinding();

        #endregion

        #region 家庭承包方式

        /// <summary>
        /// 预览集体申请书
        /// </summary>
        public CommandBinding PrintViewApplicationBind = new CommandBinding();

        /// <summary>
        /// 导出集体申请书
        /// </summary>
        public CommandBinding ExportApplicationBookBind = new CommandBinding();

        /// <summary>
        /// 打印集体申请书
        /// </summary>
        public CommandBinding PrintApplicationBind = new CommandBinding();

        /// <summary>
        /// 预览单户申请书
        /// </summary>
        public CommandBinding PrintRequireBookBind = new CommandBinding();

        /// <summary>
        /// 导出单户申请书
        /// </summary>
        public CommandBinding ExportApplicationByFamilyBind = new CommandBinding();

        /// <summary>
        /// 打印单户申请书
        /// </summary>
        public CommandBinding BatchPrintAppFamilyBind = new CommandBinding();

        #endregion

        #region 其他承包方式

        /// <summary>
        /// 预览单户申请书
        /// </summary>
        public CommandBinding PrintViewOtherApplicationBind = new CommandBinding();

        /// <summary>
        /// 导出单户申请书
        /// </summary>
        public CommandBinding ExportApplicationByOtherBind = new CommandBinding();

        /// <summary>
        /// 打印单户申请书
        /// </summary>
        public CommandBinding PrintOtherApplicationBind = new CommandBinding();

        #endregion

        #region 工具

        /// <summary>
        /// 发包方管理
        /// </summary>
        public CommandBinding SenderBind = new CommandBinding();

        /// <summary>
        /// 公示归户表
        /// </summary>
        public CommandBinding PublicityResultTableBind = new CommandBinding();

        /// <summary>
        /// 合同明细表
        /// </summary>
        public CommandBinding ConcordInformationBind = new CommandBinding();

        #endregion

        #endregion

        #region Properties

        #endregion

        #region Ctor

        /// <summary>
        /// 构造方法
        /// </summary>
        public ConcordCommand()
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
            AddBind.Command = Add;
            EditBind.Command = Edit;
            DelBind.Command = Del;
            ClearBind.Command = Clear;
            RefreshBind.Command = Refresh;

            ContactConcordBind.Command = ContactConcord;
            PreviewConcordBind.Command = PreviewConcord;
            ExportConcordBind.Command = ExportConcord;
            PrintConcordBind.Command = PrintConcord;

            PrintViewApplicationBind.Command = PrintViewApplication;
            ExportApplicationBookBind.Command = ExportApplicationBook;
            PrintApplicationBind.Command = PrintApplication;

            PrintRequireBookBind.Command = PrintRequireBook;
            ExportApplicationByFamilyBind.Command = ExportApplicationByFamily;
            BatchPrintAppFamilyBind.Command = BatchPrintAppFamily;
            PrintViewOtherApplicationBind.Command = PrintViewOtherApplication;
            ExportApplicationByOtherBind.Command = ExportApplicationByOther;
            PrintOtherApplicationBind.Command = PrintOtherApplication;

            SenderBind.Command = Sender;
            PublicityResultTableBind.Command = PublicityResultTable;
            ConcordInformationBind.Command = ConcordInformation;

        }

        #endregion
    }
}
