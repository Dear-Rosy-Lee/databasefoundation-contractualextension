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
using YuLinTu.Library.Business;
using YuLinTu.Library.Controls;
using YuLinTu.Library.Entity;
using YuLinTu.Windows;
using YuLinTu.Windows.Wpf.Metro;
using YuLinTu.Windows.Wpf.Metro.Components;

namespace YuLinTu.Component.Sender
{
    public class SenderPageContext : BusinessNavigatableWorkpageContext
    {
        #region Fields

        /// <summary>
        /// 导航类
        /// </summary>
        private ZoneNavigator nav = new ZoneNavigator();

        #endregion

        #region Ctor

        public SenderPageContext(IWorkpage workpage)
            : base(workpage)
        { }

        #endregion

        #region Methods

        #region Methods-Message

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
                SenderFramePage page = PageContent as SenderFramePage;
                Navigator.Refresh();
                if (page != null)
                {
                    page.senderPanel.Refresh();
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
        /// 点击节点发送消息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected override void OnNavigateTo(object sender, NavigateToMsgEventArgs e)
        {
            base.OnNavigateTo(sender, e);

            if (e.Object == null)
            {
                SenderFramePage senderPage = PageContent as SenderFramePage;
                if (senderPage != null)
                    senderPage.CurrentZone = null;
                return;
            }
            if (e.Object.Object is Zone)
            {
                Zone zone = e.Object.Object as Zone;

                SenderFramePage senderPage = PageContent as SenderFramePage;
                senderPage.CurrentZone = zone;
            }
        }

        #endregion

        #endregion

    }
}
