/*
 * (C) 2015  鱼鳞图公司版权所有,保留所有权利
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

namespace YuLinTu.Component.Concord
{
    /// <summary>
    /// 承包合同管理主界面
    /// </summary>
    [Newable(true,
        Order = 0,
        IsLanguageName = false,
        Name = "承包合同",
        Description = "承包合同管理",
        Category = "应用",
        Icon = "pack://application:,,,/YuLinTu.Component.Concord;component/Resources/map.png",
        Image = "pack://application:,,,/YuLinTu.Resources;component/Images/Galleries/Apps/ccc78.png",
        IsCreateShortcut = true,
        FontIcon = "\uf09d",
        IsNeedAuthenticated = true)]
    public partial class ConcordFramePage : NavigatableWorkpageFrame
    {
        #region Fields

        /// <summary>
        /// 承包合同命令
        /// </summary>
        private ConcordCommand command = new ConcordCommand();

        /// <summary>
        /// 当前地域
        /// </summary>
        private Zone currentZone;

        #endregion Fields

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
                concordPanel.CurrentZone = value;
            }
        }

        #endregion Properties

        #region Ctor

        public ConcordFramePage()
        {
            InitializeComponent();
            concordPanel.MenueEnableMethod += SetControlsEnable;
            SingleInstance = true;
            NavigatorType = eNavigatorType.TreeView;
        }

        #endregion Ctor

        #region Methods

        /// <summary>
        /// 初始化
        /// </summary>
        protected override void OnInstallComponents()
        {
            base.OnInstallComponents();
            SetCommandToControl();
            concordPanel.ThePage = Workpage;
            concordPanel.ShowTaskViewer += () =>
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
            SetCommandBinding(mbtnClear, command.Clear, command.ClearBind);
            SetCommandBinding(mbtnRefresh, command.Refresh, command.RefreshBind);

            SetCommandBinding(mbtnContactConcord, command.ContactConcord, command.ContactConcordBind);
            SetCommandBinding(mbtnPreviewConcord, command.PreviewConcord, command.PreviewConcordBind);
            SetCommandBinding(mbtnExportConcord, command.ExportConcord, command.ExportConcordBind);
            SetCommandBinding(mbtnPrintConcord, command.PrintConcord, command.PrintConcordBind);

            SetCommandBinding(mbtnPrintViewApplication, command.PrintViewApplication, command.PrintViewApplicationBind);
            SetCommandBinding(mbtnExportApplicationBook, command.ExportApplicationBook, command.ExportApplicationBookBind);
            SetCommandBinding(mbtnPrintApplication, command.PrintApplication, command.PrintApplicationBind);

            SetCommandBinding(mbtnPrintRequireBook, command.PrintRequireBook, command.PrintRequireBookBind);
            SetCommandBinding(mbtnExportApplicationByFamily, command.ExportApplicationByFamily, command.ExportApplicationByFamilyBind);
            SetCommandBinding(mbtnBatchPrintAppFamily, command.BatchPrintAppFamily, command.BatchPrintAppFamilyBind);

            SetCommandBinding(mbtnPrintViewOtherApplication, command.PrintViewOtherApplication, command.PrintViewOtherApplicationBind);
            SetCommandBinding(mbtnExportApplicationByOther, command.ExportApplicationByOther, command.ExportApplicationByOtherBind);
            SetCommandBinding(mbtnPrintOtherApplication, command.PrintOtherApplication, command.PrintOtherApplicationBind);

            //SetCommandBinding(mbtnSender, command.Sender, command.SenderBind);
            //SetCommandBinding(mbtnPublicityResultTable, command.PublicityResultTable, command.PublicityResultTableBind);
            SetCommandBinding(mbtnConcordInformation, command.ConcordInformation, command.ConcordInformationBind);
            //SetCommandBinding(mbtnRegionModel, command.BatchRegionEdit, command.BatchRegionBind);
            //SetCommandBinding(mbtnMultipleModel, command.BatchMultipleEdit, command.BatchMultipleBind);
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
            string parameter = e.Parameter.ToString();
            mbtnCollectBook.IsOpen = false;
            mbtnOtherSingleBook.IsOpen = false;
            mbtnSingleBook.IsOpen = false;
            switch (parameter)
            {
                case ConcordCommand.AddName:
                    concordPanel.AddConcord();
                    break;

                case ConcordCommand.RefreshName:
                    concordPanel.Refresh();
                    break;

                case ConcordCommand.DelName:
                    concordPanel.DelConcord();
                    break;

                case ConcordCommand.EditName:
                    concordPanel.EditConcord();
                    break;

                case ConcordCommand.ClearName:
                    concordPanel.Clear();
                    break;

                case ConcordCommand.ContactConcordName:
                    concordPanel.InitialContractConcord();
                    break;

                case ConcordCommand.PreviewConcordName:
                    //预览合同
                    concordPanel.PrintViewConcord();
                    break;

                case ConcordCommand.ExportConcordName:
                    //导出合同
                    concordPanel.ExportConcord();
                    break;

                case ConcordCommand.PrintRequireBookName:
                    //单户申请书预览
                    concordPanel.PrintRequireBook();
                    break;

                case ConcordCommand.ExportApplicationByFamilyName:
                    //单户申请书导出
                    concordPanel.ExportApplicationByFamily();
                    break;

                case ConcordCommand.PrintViewOtherApplicationName:
                    //单户申请书预览(其他)
                    concordPanel.PrintViewOtherApplication();
                    break;

                case ConcordCommand.ExportApplicationByOtherName:
                    //单户申请书导出(其他)
                    concordPanel.ExportApplicationByOther();
                    break;

                case ConcordCommand.PrintViewApplicationName:
                    //集体申请书预览
                    concordPanel.PrintViewApplication();
                    break;

                case ConcordCommand.ExportApplicationBookName:
                    //集体申请书导出
                    concordPanel.ExportApplicationBook();
                    break;

                case ConcordCommand.ConcordInformationName:
                    concordPanel.ConcordInformationTable();
                    break;

                case ConcordCommand.BatchRegionModel:
                    concordPanel.OnUpdate(sender, e);
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
               mbtnContactConcord.IsEnabled = isEnable;
               mbtnPreviewConcord.IsEnabled = isEnable;
               mbtnExportConcord.IsEnabled = isEnable;
               mbtnPrintConcord.IsEnabled = isEnable;
               mbtnCollectBook.IsEnabled = isEnable;
               mbtnOtherSingleBook.IsEnabled = isEnable;
               mbtnSingleBook.IsEnabled = isEnable;
               mbtnConcordInformation.IsEnabled = isEnable;
               //mbtnRegionModel.IsEnabled = isEnable;
               //mbtnMultipleModel.IsEnabled = isEnable;
           }));
        }

        #endregion Methods
    }
}