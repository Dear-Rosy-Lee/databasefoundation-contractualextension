/*
 * (C) 2025  鱼鳞图公司版权所有,保留所有权利 
 */
using System;
using System.Windows;
using System.Windows.Controls;
using YuLinTu.Appwork;
using YuLinTu.Data;
using YuLinTu.Library.Business;
using YuLinTu.Library.Controls;
using YuLinTu.Library.Entity;
using YuLinTu.Windows;

namespace YuLinTu.Component.ContractRegeditBook
{
    public class ContractRegeditBookPageContext : BusinessNavigatableWorkpageContext
    {
        #region Fields

        /// <summary>
        /// 导航类
        /// </summary>
        private ZoneNavigator nav = new ZoneNavigator();

        #endregion

        #region Ctor

        public ContractRegeditBookPageContext(IWorkpage workpage)
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
            ContractRegeditBookFramePage page = PageContent as ContractRegeditBookFramePage;
            if (page != null)
            {
                IDbContext dbContext = DataBaseSource.GetDataBaseSource();
                page.contractRegeditBookPanel.DbContext = dbContext;
                page.contractRegeditBookPanel.PersonBusiness = new VirtualPersonBusiness(dbContext);
                page.contractRegeditBookPanel.ConcordBusiness = new ConcordBusiness(dbContext);
                page.contractRegeditBookPanel.ContractRegeditBookBusiness = new ContractRegeditBookBusiness(dbContext);
                page.contractRegeditBookPanel.AccountLandBusiness = new AccountLandBusiness(dbContext);
                page.contractRegeditBookPanel.DictBusiness = new DictionaryBusiness(dbContext);
                page.contractRegeditBookPanel.VirtualType = eVirtualType.Land;
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
                case VirtualPersonMessage.VIRTUALPERSON_DEL_COMPLATE:
                    RefreshUi();
                    break;
                case VirtualPersonMessage.VIRTUALPERSON_COMBINE_COMPLATE:
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
                case ConcordMessage.CONCORD_ADD_COMPLATE:
                    RefreshUi();
                    break;
                case ConcordMessage.CONCORD_DELETE_COMPLATE:
                    RefreshUi();
                    break;
                case ConcordMessage.CONCORD_CLEAR_COMPLATE:
                    RefreshUi();
                    break;
                case VirtualPersonMessage.VIRTUALPERSON_STATUSCHANGE:
                    RefreshUi();
                    break;
                case VirtualPersonMessage.VIRTUALPERSON_INITIAL_COMPLATE:
                    RefreshUi();
                    break;
                case ContractRegeditBookMessage.ContractRegeditBook_Refresh:
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
                ContractRegeditBookFramePage page = PageContent as ContractRegeditBookFramePage;
                Navigator.Refresh();
                if (page != null)
                {
                    page.contractRegeditBookPanel.Refresh();
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
                ContractRegeditBookFramePage senderPage = PageContent as ContractRegeditBookFramePage;
                if (senderPage != null)
                    senderPage.CurrentZone = null;
                return;
            }
            if (e.Object.Object is Zone)
            {
                Zone zone = e.Object.Object as Zone;
                ContractRegeditBookFramePage senderPage = PageContent as ContractRegeditBookFramePage;
                if (senderPage != null)
                {
                    senderPage.CurrentZone = zone;
                    IDbContext dbContext = DataBaseSource.GetDataBaseSource();
                    senderPage.contractRegeditBookPanel.DbContext = dbContext;
                    senderPage.contractRegeditBookPanel.PersonBusiness = new VirtualPersonBusiness(dbContext);
                    senderPage.contractRegeditBookPanel.ConcordBusiness = new ConcordBusiness(dbContext);
                    senderPage.contractRegeditBookPanel.ContractRegeditBookBusiness = new ContractRegeditBookBusiness(dbContext);
                    senderPage.contractRegeditBookPanel.AccountLandBusiness = new AccountLandBusiness(dbContext);
                    senderPage.contractRegeditBookPanel.DictBusiness = new DictionaryBusiness(dbContext);
                    senderPage.contractRegeditBookPanel.VirtualType = eVirtualType.Land;
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
                    Editor = new ContractRegeditBookConfigPage(Workpage),
                });
                e.Editors.Add(new WorkpageOptionsEditorMetadata()
                {
                    Name = "数据汇总设置",
                    Editor = new ContractRegeditBookDataSummaryConfigPage(Workpage),
                });
                e.Editors.Add(new WorkpageOptionsEditorMetadata()
                {
                    Name = "二维码配置",
                    Editor = new ContractRegeditBookQRCodeConfig(Workpage)
                });
            }));
        }

        #endregion

        #endregion

    }
}
