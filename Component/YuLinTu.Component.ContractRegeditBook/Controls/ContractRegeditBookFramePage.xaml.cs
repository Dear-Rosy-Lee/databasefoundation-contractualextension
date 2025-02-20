/*
 * (C) 2025  鱼鳞图公司版权所有,保留所有权利 
 */
using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using YuLinTu.Appwork;
using YuLinTu.Library.Business;
using YuLinTu.Library.Command;
using YuLinTu.Library.Controls;
using YuLinTu.Library.Entity;
using YuLinTu.Library.Log;
using YuLinTu.Windows;
using YuLinTu.Windows.Wpf.Metro;
using YuLinTu.Windows.Wpf.Metro.Components;

namespace YuLinTu.Component.ContractRegeditBook
{
    /// <summary>
    /// 承包权证管理主界面
    /// </summary>
    [Newable(true,
        Order = 0,
        IsLanguageName = false,
        Name = "承包权证",
        Description = "承包权证管理",
        Category = "应用",
        Icon = "pack://application:,,,/YuLinTu.Component.ContractRegeditBook;component/Resources/map.png",
        Image = "pack://application:,,,/YuLinTu.Resources;component/Images/Galleries/Apps/Regedit78.png",
        IsCreateShortcut = true,
        FontIcon = "\uf02d",
        IsNeedAuthenticated = true)]
    public partial class ContractRegeditBookFramePage : NavigatableWorkpageFrame
    {
        #region Fields

        /// <summary>
        /// 承包合同命令
        /// </summary>        
        private ContractRegeditBookCommand command = new ContractRegeditBookCommand();

        /// <summary>
        /// 当前地域
        /// </summary>
        private Zone currentZone;

        #endregion

        #region Properties

        /// <summary>
        /// 是否需要授权
        /// </summary>
        public override bool IsNeedAuthenticated
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// 当前地域
        /// </summary>
        public Zone CurrentZone
        {
            get { return currentZone; }
            set
            {
                currentZone = value;
                contractRegeditBookPanel.CurrentZone = value;
            }
        }

        #endregion

        #region Ctor

        public ContractRegeditBookFramePage()
        {
            InitializeComponent();
            contractRegeditBookPanel.MeunEnable += SetControlsEnable;
            SingleInstance = true;
            NavigatorType = eNavigatorType.TreeView;
        }

        #endregion

        #region Methods

        /// <summary>
        /// 初始化
        /// </summary>
        protected override void OnInstallComponents()
        {
            base.OnInstallComponents();
            SetCommandToControl();
            contractRegeditBookPanel.ThePage = Workpage;
            contractRegeditBookPanel.ShowTaskViewer += () =>
            {
                Workpage.Message.Send(this, new MsgEventArgs(EdCore.langRequestShowTaskViewer));
            };
        }

        /// <summary>
        /// 绑定命令到控件上
        /// </summary>
        private void SetCommandToControl()
        {
            SetCommandBinding(mbtnAdd, command.Add, command.AddBind);
            SetCommandBinding(mbtnEdit, command.Edit, command.EditBind);
            SetCommandBinding(mbtnDel, command.Del, command.DelBind);
            SetCommandBinding(mbtnInitalizeData, command.InitalizeData, command.InitalizeDataBind);
            //SetCommandBinding(mbtnBookSetting, command.BookSetting, command.BookSettingBind);
            SetCommandBinding(mbtnPrivewRegeditBook, command.PrivewRegeditBook, command.PrivewRegeditBookBind);
            SetCommandBinding(mbtnExportRegeditBook, command.ExportRegeditBook, command.ExportRegeditBookBind);
            //SetCommandBinding(mbtnPrintRegeditBook, command.PrintRegeditBook, command.PrintRegeditBookBind);
            SetCommandBinding(mbtnBookPrintSetting, command.PrintSetting, command.PrintSettingBind);
            SetCommandBinding(mbtnPrintViewWarrant, command.PrintViewWarrant, command.PrintViewWarrantBind);
            SetCommandBinding(mbtnExportWarrant, command.ExportWarrant, command.ExportWarrantBind);
            //SetCommandBinding(mbtnPrintWarrant, command.PrintWarrant, command.PrintWarrantBind);
            SetCommandBinding(mbtnWarrantSummery, command.ExportWarrantSummeryTable, command.ExportWarrantSummeryTableBind);
            SetCommandBinding(mbtnFamilyTable, command.ExportFamilyTable, command.ExportFamilyTableBind);
            SetCommandBinding(mbtnExportAwareTable, command.ExportAwareTable, command.ExportAwareTableBind);
            SetCommandBinding(mbtnClear, command.Clear, command.ClearBind);
            SetCommandBinding(mbtnRefresh, command.Refresh, command.RefreshBind);

            SetCommandBinding(mbtnResetSerialNumber, command.ResetSerialNumber, command.ResetSerialNumberBinding);
            SetCommandBinding(mbtnRelationSerialNumber, command.RelationSerialNumber, command.RelationSerialNumberBinding);
        }

        /// <summary>
        /// 创建命令绑定
        /// </summary>
        private void SetCommandBinding(MetroButton button, RoutedCommand cmd, CommandBinding bind)
        {
            bind.CanExecute += CommandBinding_CanExecute;
            bind.Executed += CommandBinding_Executed;
            button.CommandBindings.Add(bind);
            button.Command = cmd;
            button.CommandParameter = cmd.Name;
        }

        /// <summary>
        /// 是否可以执行命令
        /// </summary>
        private void CommandBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (e.Source != null)
            {
                e.CanExecute = true;
            }
            else
            {
                e.CanExecute = false;
            }
        }

        /// <summary>
        /// 执行命令
        /// </summary>
        private void CommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            mbtnCollectBook.IsOpen = false;
            string parameter = e.Parameter.ToString();
            switch (parameter)
            {
                case ContractRegeditBookCommand.AddName:
                    contractRegeditBookPanel.AddRegeditBook();
                    break;
                case ContractRegeditBookCommand.RefreshName:
                    contractRegeditBookPanel.Refresh();
                    break;
                case ContractRegeditBookCommand.DelName:
                    contractRegeditBookPanel.DelRegeditBook();
                    break;
                case ContractRegeditBookCommand.EditName:
                    contractRegeditBookPanel.EditRegeditBook();
                    break;
                case ContractRegeditBookCommand.ExportAwareTableName:
                    contractRegeditBookPanel.ExportAwareTable();  //颁证清册
                    break;
                case ContractRegeditBookCommand.ExportFamilyTableName:
                    contractRegeditBookPanel.ExportFamilyTable(); //单户确认表
                    break;
                case ContractRegeditBookCommand.ExportRegeditBookName:
                    contractRegeditBookPanel.ExportRegeditBook(); //导出登记簿
                    break;
                case ContractRegeditBookCommand.ExportWarrantName:
                    contractRegeditBookPanel.ExportWarrant();  //证书数据处理-导出证书
                    break;
                case ContractRegeditBookCommand.ExportWarrantSummeryTableName:
                    contractRegeditBookPanel.ExportWarrentSummaryTable();  //权证数据汇总表
                    break;
                case ContractRegeditBookCommand.InitalizeDataName:
                    contractRegeditBookPanel.InitalizeWarrent();  //权证登记
                    break;
                case ContractRegeditBookCommand.PrintSettingName:
                    contractRegeditBookPanel.PrintSettingName();//证书数据处理-打印设置
                    break;
                case ContractRegeditBookCommand.PrintViewWarrantName:
                    contractRegeditBookPanel.PrintViewWarrant();//证书数据处理-预览证书
                    break;
                //case ContractRegeditBookCommand.PrintWarrantName:
                //  break;
                case ContractRegeditBookCommand.PrivewRegeditBookName:
                    contractRegeditBookPanel.PrivewRegeditBook(); //登记簿预览
                    break;
                case ContractRegeditBookCommand.ClearName:
                    contractRegeditBookPanel.Clear();
                    break;
                case ContractRegeditBookCommand.ResetSerialNumberName:
                    contractRegeditBookPanel.ResetSerialNumber();
                    break;

                case ContractRegeditBookCommand.RelationSerialNumberName:
                    contractRegeditBookPanel.ImportRelaitonExcel();
                    break;
            }
        }

        /// <summary>
        /// 设置控件可用性
        /// </summary>
        public void SetControlsEnable(bool isEnable = true)
        {
            System.Windows.Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                mbtnAdd.IsEnabled = isEnable;
                mbtnEdit.IsEnabled = isEnable;
                mbtnDel.IsEnabled = isEnable;
                mbtnClear.IsEnabled = isEnable;
                mbtnCollectBook.IsEnabled = isEnable;
                mbtnInitalizeData.IsEnabled = isEnable;
                mbtnPrivewRegeditBook.IsEnabled = isEnable;
                mbtnExportRegeditBook.IsEnabled = isEnable;
                mbtnWarrantSummery.IsEnabled = isEnable;
                mbtnBookPrintSetting.IsEnabled = isEnable;
                mbtnPrintViewWarrant.IsEnabled = isEnable;
                mbtnExportWarrant.IsEnabled = isEnable;

            }));
        }


        #endregion
    }
}
