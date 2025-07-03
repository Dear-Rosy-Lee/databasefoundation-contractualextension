/*
 * (C) 2025  鱼鳞图公司版权所有,保留所有权利 
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

namespace YuLinTu.Component.VirtualPerson
{
    public class PersonPageContext : BusinessNavigatableWorkpageContext
    {
        #region Fields

        /// <summary>
        /// 导航类
        /// </summary>
        private ZoneNavigator nav = new ZoneNavigator();

        #endregion

        #region Ctor

        public PersonPageContext(IWorkpage workpage)
            : base(workpage)
        {
        }

        #endregion

        #region Methods

        #region Methods-Message

        /// <summary>
        /// 点击节点发送消息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected override void OnNavigateTo(object sender, NavigateToMsgEventArgs e)
        {
            base.OnNavigateTo(sender, e);
            if (e.Object == null)
            {
                PersonFramePage senderPage = PageContent as PersonFramePage;
                if (senderPage != null)
                    senderPage.CurrentZone = null;
                return;
            }
            if (e.Object.Object is Zone)
            {
                Zone zone = e.Object.Object as Zone;

                PersonFramePage senderPage = PageContent as PersonFramePage;
                if (senderPage != null)
                {
                    senderPage.CurrentZone = zone;
                    IDbContext dbContext = DataBaseSource.GetDataBaseSource();
                    senderPage.personPanel.Business = new VirtualPersonBusiness(dbContext); ;
                    senderPage.personPanel.DbContext = dbContext;
                    senderPage.personPanel.VirtualType = eVirtualType.Land;
                    senderPage.btnVirtualPersonType.Visibility = Visibility.Collapsed;
                }
            }
        }

        /// <summary>
        /// 注册导航模板
        /// </summary>
        protected override void OnInitializeWorkpageContent(object sender, InitializeWorkpageContentEventArgs e)
        {
            if (!e.Value)
            {
                return;
            }
            PersonFramePage page = PageContent as PersonFramePage;
            if (page != null)
            {
                IDbContext dbContext = DataBaseSource.GetDataBaseSource();
                page.personPanel.Business = new VirtualPersonBusiness(dbContext); ;
                page.personPanel.DbContext = dbContext;
                page.personPanel.VirtualType = eVirtualType.Land;
                page.btnVirtualPersonType.Visibility = Visibility.Collapsed;
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
                case  ContractAccountMessage.CONTRACTACCOUNT_CLEARLANDANDPERSON_COMPLETE:
                    RefreshUi();
                    break;
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
                case ConcordMessage.CONCORD_CLEAR_COMPLATE:
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
                PersonFramePage page = PageContent as PersonFramePage;
                Navigator.Refresh();
                if (page != null)
                {
                    page.personPanel.Refresh();
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
                    Editor = new VirtualPersonConfigPage(Workpage),
                });

                e.Editors.Add(new WorkpageOptionsEditorMetadata()
                {
                    Name = "导入调查表",
                    Editor = new VirtualPersonImportConfigPage(Workpage),
                });

                e.Editors.Add(new WorkpageOptionsEditorMetadata()
                {
                    Name = "导出调查表",
                    Editor = new VirtualPersonOutputConfigPage(Workpage),
                });
            }));
        }

        #endregion

        #endregion

    }
}
