/*
 * (C) 2015  鱼鳞图公司版权所有,保留所有权利
 */

using System;
using System.Collections.Generic;
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

namespace YuLinTu.Component.VirtualPerson
{
    /// <summary>
    /// 承包方管理主界面
    /// </summary>
    [Newable(true,
        Order = 0,
        IsLanguageName = false,
        Name = "承包方",
        Description = "承包方管理",
        Category = "应用",
        Icon = "pack://application:,,,/YuLinTu.Resources;component/Images/16/map.png",
        Image = "pack://application:,,,/YuLinTu.Resources;component/Images/Galleries/Apps/人口管理78.png",
        IsCreateShortcut = true,
        FontIcon = "\uf007",
        IsNeedAuthenticated = true)]
    public partial class PersonFramePage : NavigatableWorkpageFrame
    {
        #region Fields

        /// <summary>
        /// 承包方命令
        /// </summary>
        private VirtualPersonCommand command = new VirtualPersonCommand();

        /// <summary>
        /// 当前地域
        /// </summary>
        private Zone currentZone;

        #endregion Fields

        #region Properties

        /// <summary>
        /// 当前地域
        /// </summary>
        public Zone CurrentZone
        {
            get { return currentZone; }
            set
            {
                currentZone = value;
                personPanel.CurrentZone = value;
            }
        }

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

        #endregion Properties

        #region Ctor

        public PersonFramePage()
        {
            InitializeComponent();
            personPanel.MenuEnable += SetControlsEnable;
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
            personPanel.ThePage = Workpage;
            personPanel.ShowEqualColum = false;
            personPanel.ShowTableColum = false;
            personPanel.ShowTaskViewer += () =>
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
            SetCommandBinding(mbtnImport, command.Import, command.ImportBind);
            SetCommandBinding(mbtnExcelTemplate, command.ExcelTemplate, command.ExcelTemplateBind);
            SetCommandBinding(mbtnWordTemplate, command.WordTemplate, command.WordTemplateBind);
            SetCommandBinding(btnExportExcel, command.ExportExcel, command.ExportExcelBind);
            SetCommandBinding(btnExportTable, command.ExportTable, command.ExportTableBind);
            SetCommandBinding(mbtnClear, command.Clear, command.ClearBind);
            SetCommandBinding(mbtnRefresh, command.Refresh, command.RefreshBind);
            SetCommandBinding(btnApplyPreview, command.ApplyPreview, command.ApplyPreviewBind);
            SetCommandBinding(btnApplyExport, command.ApplyExport, command.ApplyExportBind);
            SetCommandBinding(btnApplyPrint, command.ApplyPrint, command.ApplyPrintBind);
            SetCommandBinding(btnDelegatePreview, command.DelegatePreview, command.DelegatePreviewBind);
            SetCommandBinding(btnDelegateExport, command.DelegateExport, command.DelegateExportBind);
            SetCommandBinding(btnDelegatePrint, command.DelegatePrint, command.DelegatePrintBind);
            SetCommandBinding(btnIdeaPreview, command.IdeaPreview, command.IdeaPreviewBind);
            SetCommandBinding(btnIdeaExport, command.IdeaExport, command.IdeaExportBind);
            SetCommandBinding(btnIdeaPrint, command.IdeaPrint, command.IdeaPrintBind);
            SetCommandBinding(btnSurveyPreview, command.SurveyPreview, command.SurveyPreviewBind);
            SetCommandBinding(btnSurveyExport, command.SurveyExport, command.SurveyExportBind);
            SetCommandBinding(btnSurveyPrint, command.SurveyPrint, command.SurveyPrintBind);
            SetCommandBinding(mbtnSplitFamily, command.SplitFamily, command.SplitFamilyBind);
            SetCommandBinding(mbtnCombineFamily, command.CombineFamily, command.CombineFamilyBind);
            SetCommandBinding(mbtnLockedFamily, command.LockedFamily, command.LockedFamilyBind);
            SetCommandBinding(mbtnSetFamily, command.SetFamily, command.SetFamilyBind);
            SetCommandBinding(mbtnSearchData, command.SearchData, command.SearchDataBind);
            SetCommandBinding(mbtnInitiallData, command.InitiallData, command.InitiallDataBind);
            SetCommandBinding(btnRelationCheck, command.RelationCheck, command.RelationCheckBind);
            SetCommandBinding(btnRelationReplace, command.RelationReplace, command.RelationReplaceBind);

            SetCommandBinding(btnSharePersonRepair, command.SharePersonRepair, command.SharePersonRepairBind);

            SetCommandBinding(btnLandType, command.Contract, command.ContractBind);
            SetCommandBinding(btnWoodType, command.Wood, command.WoodBind);
            SetCommandBinding(btnHouseType, command.House, command.HouseBind);
            SetCommandBinding(btnYardType, command.Yard, command.YardBind);
            SetCommandBinding(btnCollectiveType, command.Collective, command.CollectiveBind);
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

        /// </summary>
        private void Export_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = e.Source != null && personPanel.GetSelectItem() == null;
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
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            btntemplate.IsOpen = false;
            btntable.IsOpen = false;
            btnmaterial.IsOpen = false;
            btndelegate.IsOpen = false;
            btnIdea.IsOpen = false;
            btnsurvey.IsOpen = false;
            string parameter = e.Parameter.ToString();
            switch (parameter)
            {
                case VirtualPersonCommand.AddName:
                    personPanel.AddVirtualPerson();
                    break;

                case VirtualPersonCommand.RefreshName:
                    personPanel.Refresh();
                    break;

                case VirtualPersonCommand.DelName:
                    personPanel.DelPerson();
                    break;

                case VirtualPersonCommand.EditName:
                    personPanel.EditPerson();
                    break;

                case VirtualPersonCommand.ClearName:
                    personPanel.Clear();
                    break;

                case VirtualPersonCommand.ImportName:
                    personPanel.ImportData();
                    break;

                case VirtualPersonCommand.SplitFamilyName:
                    personPanel.VirtualPersonSplit();
                    break;

                case VirtualPersonCommand.SetFamilyName:
                    personPanel.VirtualPersonSet();
                    break;

                case VirtualPersonCommand.LockedFamilyName:
                    personPanel.VirtualPersonLock();
                    break;

                case VirtualPersonCommand.CombineFamilyName:
                    personPanel.VirtualPersonCombine();
                    break;

                case VirtualPersonCommand.ExcelTemplateName:
                    personPanel.ExportExcelTemplate();
                    break;

                case VirtualPersonCommand.WordTemplateName:
                    personPanel.ExportWordTemplate();
                    break;

                case VirtualPersonCommand.InitiallDataName:
                    personPanel.VirtualPersonInitialize();
                    break;

                case VirtualPersonCommand.SearchDataName:
                    personPanel.VirtualPersonSearch();
                    break;

                case VirtualPersonCommand.ExportExcelName:
                    personPanel.ExportVirtualPersonExcel();
                    break;

                case VirtualPersonCommand.ExportTableName:
                    personPanel.ExportVirtualPersonWord();
                    break;

                case VirtualPersonCommand.IdeaExportName:
                    personPanel.ExportIdeaBook();
                    break;

                case VirtualPersonCommand.IdeaPreviewName:
                    personPanel.IdeaPreviewBook();
                    break;

                case VirtualPersonCommand.ApplyExportName:
                    personPanel.ExportApplyBook();
                    break;

                case VirtualPersonCommand.ApplyPreviewName:
                    personPanel.ApplyPreviewBook();
                    break;

                case VirtualPersonCommand.DelegateExportName:
                    personPanel.ExportDelegateBook();
                    break;

                case VirtualPersonCommand.DelegatePreviewName:
                    personPanel.DelegatePreviewBook();
                    break;

                case VirtualPersonCommand.SurveyExportName:
                    personPanel.ExportSurveyBook();
                    break;

                case VirtualPersonCommand.SurveyPreviewName:
                    personPanel.SurveyPreviewBook();
                    break;

                case VirtualPersonCommand.ContractName:
                    SetMainPagePersonType(sender, eVirtualType.Land);
                    break;

                case VirtualPersonCommand.WoodName:
                    SetMainPagePersonType(sender, eVirtualType.Wood);
                    break;

                case VirtualPersonCommand.YardName:
                    SetMainPagePersonType(sender, eVirtualType.Yard);
                    break;

                case VirtualPersonCommand.CollectiveName:
                    SetMainPagePersonType(sender, eVirtualType.CollectiveLand);
                    break;

                case VirtualPersonCommand.HouseName:
                    SetMainPagePersonType(sender, eVirtualType.House);
                    break;

                case VirtualPersonCommand.RelationReplaceName:
                    personPanel.RelationReplace();
                    break;

                case VirtualPersonCommand.RelationCheckName:
                    personPanel.RelationCheck();
                    break;

                case VirtualPersonCommand.SharePersonRepairName:
                    personPanel.SharePersonRepair();
                    break;
            }
        }

        /// <summary>
        /// 设置承包方类型
        /// </summary>
        private void SetMainPagePersonType(object sender, eVirtualType type)
        {
            btnVirtualPersonType.IsOpen = false;
            personPanel.VirtualType = type;
            Zone zone = personPanel.CurrentZone;
            personPanel.CurrentZone = zone;
            MetroButton btn = sender as MetroButton;
            if (btn == null)
                return;
            ImageTextItem item = btn.Content as ImageTextItem;
            if (item == null)
                return;
            btnvpTypeImg.Image = item.Image;
            btnvpTypeImg.Text = item.Text;
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
                btntemplate.IsEnabled = isEnable;
                btntable.IsEnabled = isEnable;
                btnmaterial.IsEnabled = isEnable;
                btndelegate.IsEnabled = isEnable;
                btnIdea.IsEnabled = isEnable;
                btnsurvey.IsEnabled = isEnable;

                mbtnImport.IsEnabled = isEnable;
                mbtnClear.IsEnabled = isEnable;
                mbtnSplitFamily.IsEnabled = isEnable;
                mbtnCombineFamily.IsEnabled = isEnable;
                mbtnLockedFamily.IsEnabled = isEnable;
                mbtnSetFamily.IsEnabled = isEnable;
                mbtnSearchData.IsEnabled = isEnable;
                mbtnInitiallData.IsEnabled = isEnable;
            }));
        }

        #endregion Methods
    }
}