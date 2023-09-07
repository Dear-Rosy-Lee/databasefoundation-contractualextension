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
using YuLinTu.Data;
using YuLinTu.Library.Business;
using YuLinTu.Library.Controls;
using YuLinTu.Library.Entity;
using YuLinTu.Windows;
using YuLinTu.Windows.Wpf.Metro;
using YuLinTu.Windows.Wpf.Metro.Components;

namespace YuLinTu.Component.Concord
{
    public class ConcordPageContext : BusinessNavigatableWorkpageContext
    {
        #region Fields

        /// <summary>
        /// 导航类
        /// </summary>
        private ZoneNavigator nav = new ZoneNavigator();

        #endregion

        #region Ctor

        public ConcordPageContext(IWorkpage workpage)
            : base(workpage)
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
            ConcordFramePage page = PageContent as ConcordFramePage;
            if (page != null)
            {
                IDbContext dbContext = DataBaseSource.GetDataBaseSource();
                page.concordPanel.DbContext = dbContext;
                page.concordPanel.PersonBusiness = new VirtualPersonBusiness(dbContext);
                page.concordPanel.ConcordBusiness = new ConcordBusiness(dbContext);
                page.concordPanel.LandBusiness = new AccountLandBusiness(dbContext);
                page.concordPanel.VirtualType = eVirtualType.Land;
            }
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
                case ZoneMessage.ZONE_ADD_COMPLETE:
                    RefreshUi();
                    break;
                case ZoneMessage.ZONE_DELETE_COMPLETE:
                    RefreshUi();
                    break;
                case ZoneMessage.ZONE_CLEAR_COMPLETE:
                    RefreshUi();
                    break;
                case ZoneMessage.ZONE_UPDATE_COMPLETE:
                    RefreshUi();
                    break;
                case VirtualPersonMessage.CLEAR_COMPLATE:
                    RefreshUi();
                    break;
                case VirtualPersonMessage.VIRTUALPERSON_COMBINE_COMPLATE:
                    RefreshUi();
                    break;
                case VirtualPersonMessage.VIRTUALPERSON_DEL_COMPLATE:
                    RefreshUi();
                    break;
                case ContractAccountMessage.CONTRACTLAND_ADD_COMPLETE:
                    RefreshUi();
                    break;
                case ContractAccountMessage.CONTRACTACCOUNT_CLEAR_COMPLETE:
                    RefreshUi();
                    break;
                case ContractAccountMessage.CONTRACTLAND_DELETE_COMPLETE:
                    RefreshUi();
                    break;
                case ContractAccountMessage.CONTRACTLAND_EDIT_COMPLETE:
                    RefreshUi();
                    break;
                case VirtualPersonMessage.VIRTUALPERSON_STATUSCHANGE:
                    RefreshUi();
                    break;
                case VirtualPersonMessage.VIRTUALPERSON_INITIAL_COMPLATE:
                    RefreshUi();
                    break;
                case ConcordMessage.CONCORD_REFRESH:
                    RefreshUi();
                    break;
                case ContractAccountMessage.CONTRACTACCOUNT_Refresh:
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
                ConcordFramePage page = PageContent as ConcordFramePage;
                Navigator.Refresh();
                if (page != null)
                {
                    page.concordPanel.Refresh();
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
                ConcordFramePage senderPage = PageContent as ConcordFramePage;
                if (senderPage != null)
                    senderPage.CurrentZone = null;
                return;
            }
            if (e.Object.Object is Zone)
            {
                Zone zone = e.Object.Object as Zone;
                ConcordFramePage senderPage = PageContent as ConcordFramePage;
                if (senderPage != null)
                {
                    senderPage.CurrentZone = zone;
                    IDbContext dbContext = DataBaseSource.GetDataBaseSource();
                    senderPage.concordPanel.DbContext = dbContext;
                    senderPage.concordPanel.PersonBusiness = new VirtualPersonBusiness(dbContext);
                    senderPage.concordPanel.ConcordBusiness = new ConcordBusiness(dbContext);
                    senderPage.concordPanel.LandBusiness = new AccountLandBusiness(dbContext);
                    senderPage.concordPanel.VirtualType = eVirtualType.Land;
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
                e.Editors.Add(new WorkpageOptionsEditorMetadata()
                {
                    Name = "常规",
                    Editor = new ConcordConfigPage(Workpage),
                });
                e.Editors.Add(new WorkpageOptionsEditorMetadata()
                {
                    Name = "合同明细表",
                    Editor = new ConcordDetailTableConfigPage(Workpage),
                });
            }));
        }

        #endregion

        #endregion

    }
}
