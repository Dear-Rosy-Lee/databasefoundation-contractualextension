/*
 * (C) 2015  鱼鳞图公司版权所有,保留所有权利 
 */
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Xceed.Wpf.Toolkit;
using YuLinTu.Appwork;
using YuLinTu.Component.ContractAccount;
using YuLinTu.Data;
using YuLinTu.Library.Business;
using YuLinTu.Library.Controls;
using YuLinTu.Library.Entity;
using YuLinTu.Windows;
using YuLinTu.Windows.Wpf.Metro;
using YuLinTu.Windows.Wpf.Metro.Components;

namespace YuLinTu.Component.XiZangLZ
{
    public class ExportDataSummaryPageContext : BusinessNavigatableWorkpageContext
    {
        #region Fields

        /// <summary>
        /// 导航类
        /// </summary>
        private ZoneNavigator nav = new ZoneNavigator();

        #endregion

        #region Ctor

        public ExportDataSummaryPageContext(IWorkpage workpage) : base(workpage)
        {


        }

        #endregion

        #region Methods

        #region Methods-Message

        /// <summary>
        /// 注册导航模板
        /// </summary>
        protected override void OnInitializeWorkpageContent(object sender, InitializeWorkpageContentEventArgs e)
        {
            if (!e.Value)
            {
                return;
            }
            DataSummaryExportFramePage page = PageContent as DataSummaryExportFramePage;
            if (page != null)
            {
                IDbContext dbContext = DataBaseSource.GetDataBaseSource();
                page.dataSummaryExportPanel.DbContext = dbContext;
                page.dataSummaryExportPanel.PersonBusiness = new VirtualPersonBusiness(dbContext);
                page.dataSummaryExportPanel.ConcordBusiness = new ConcordBusiness(dbContext);
                page.dataSummaryExportPanel.LandBusiness = new AccountLandBusiness(dbContext);
                page.dataSummaryExportPanel.VirtualType = eVirtualType.Land;
            }


        }




        /// <summary>
        /// 消息显示框
        /// </summary>
        private void ShowBox(string header, string msg, eMessageGrade type = eMessageGrade.Error, Action<bool?, eCloseReason> action = null)
        {

            Workpage.Page.ShowMessageBox(new TabMessageBoxDialog
            {
                Header = header,
                Message = msg,
                MessageGrade = type,
                CancelButtonText = "取消",
            }, action);
        }


        protected override void OnInstalLeftSidebarTabItems(object sender, InstallUIElementsEventArgs e)
        {

            if (Navigator == null)
            {
                return;
            }
            Navigator.RootItemAutoExpand = false;
            var dic = new ResourceDictionary() { Source = new Uri("pack://application:,,,/YuLinTu.Library.Controls;component/Navigation/Res.xaml") };
            var key = new DataTemplateKey(typeof(NavigateZoneItem));
            Navigator.RegisterItemTemplate(typeof(NavigateZoneItem), dic[key] as DataTemplate);

            var menu = dic["TreeViewNavigator_Menu_Zone"] as ContextMenu;
            Navigator.RegisterContextMenu(typeof(Zone), menu);
            Navigator.AddCommandBinding(ZoneNavigatorCommands.CopyCommandBinding);

        }


        /// <summary>
        /// 获取账户数据
        /// </summary>
        protected override void OnInstallAccountData(object sender, AccountEventArgs e)
        {
            base.OnInstallAccountData(sender, e);
        }

        /// <summary>
        /// 界面消息
        /// </summary>
        protected override void OnWorkspaceMessageReceived(object sender, MsgEventArgs e)
        {
            switch (e.Name)
            {
                case ZoneMessage.ZONE_IMPORTTABLE_COMPLETE:
                    RefreshUi();
                    break;

                default:
                    base.OnWorkspaceMessageReceived(sender, e);
                    break;
            }
        }

        /// <summary>
        /// 刷新界面
        /// </summary>
        private void RefreshUi()
        {
            Workpage.Workspace.Window.Dispatcher.Invoke(new Action(() =>
            {
                DataSummaryExportFramePage page = PageContent as DataSummaryExportFramePage;
                Navigator.Refresh();
                if (page != null)
                {

                }
            }));
        }

        /// <summary>
        /// 初始化节点
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        [MessageHandler(ID = EdCore.langInstallNavigateItems)]
        private void OnInstallNavigateItems(object sender, InstallNavigateItemMsgEventArgs e)
        {

            e.Instance.Items.AddRange(nav.GetChildren(e.Instance.Root));

        }

        protected override void OnNavigateTo(object sender, NavigateToMsgEventArgs e)
        {
            base.OnNavigateTo(sender, e);

            if (e.Object == null)
            {
                DataSummaryExportFramePage senderPage = PageContent as DataSummaryExportFramePage;
                if (senderPage != null)
                    senderPage.CurrentZone = null;
                return;
            }
            if (e.Object.Object is Zone)
            {
                Zone zone = e.Object.Object as Zone;
                DataSummaryExportFramePage senderPage = PageContent as DataSummaryExportFramePage;
                if (senderPage != null)
                {
                    senderPage.CurrentZone = zone;
                    IDbContext dbContext = DataBaseSource.GetDataBaseSource();
                    senderPage.dataSummaryExportPanel.DbContext = dbContext;
                    senderPage.dataSummaryExportPanel.PersonBusiness = new VirtualPersonBusiness(dbContext);
                    senderPage.dataSummaryExportPanel.ConcordBusiness = new ConcordBusiness(dbContext);
                    senderPage.dataSummaryExportPanel.LandBusiness = new AccountLandBusiness(dbContext);
                    senderPage.dataSummaryExportPanel.VirtualType = eVirtualType.Land;
                }
            }
        }

        /// <summary>
        /// 系统配置
        /// </summary>
        [MessageHandler(ID = EdCore.langInstallWorkpageOptionsEditor)]
        private void langInstallWorkpageOptionsEditor(object sender, InstallWorkpageOptionsEditorEventArgs e)
        {
            Workpage.Workspace.Window.Dispatcher.Invoke(new Action(() =>
            {

            }));
        }

        #endregion

        #endregion

    }
}
