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

namespace YuLinTu.Component.SecondAccount
{
    /// <summary>
    /// 导航类工作上下文
    /// </summary>
    public class SecondAccountPageContext : BusinessNavigatableWorkpageContext
    {
        #region Fields

        /// <summary>
        /// 导航类
        /// </summary>
        private ZoneNavigator nav = new ZoneNavigator();

        #endregion

        #region Ctor

        public SecondAccountPageContext(IWorkpage workpage)
            : base(workpage)
        { }

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

            SecondAccountFramePage page = PageContent as SecondAccountFramePage;

            if (page != null)
            {
                IDbContext dbContext = DataBaseSource.GetDataBaseSource();
                page.secondAccountPanel.PersonBusiness = new VirtualPersonBusiness(dbContext);    //这里要更改二轮台账相关业务

                page.secondAccountPanel.DbContext = dbContext;
                page.secondAccountPanel.TableLandBusiness = new SecondTableLandBusiness(dbContext);
                page.secondAccountPanel.VirtualType = eVirtualType.SecondTable;

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

        protected override void OnInstallAccountData(object sender, AccountEventArgs e)
        {
            base.OnInstallAccountData(sender, e);
        }

        /// <summary>
        /// 界面消息   相关地域的
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
                case SecondTableLandMessage.SECONDPERSON_SET_COMPLATE:
                    RefreshUi();
                    break;
                case SecondTableLandMessage.SECONDPERSON_ADD_COMPLETE:
                    RefreshUi();
                    break;
                case SecondTableLandMessage.SECONDPERSON_DELT_COMPLETE:
                    RefreshUi();
                    break;
                case SecondTableLandMessage.SECONDPERSON_EDIT_COMPLATE:
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
                SecondAccountFramePage page = PageContent as SecondAccountFramePage;
                //Navigator.Refresh();
                if (page != null)
                {
                    page.secondAccountPanel.Refresh();  //
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
        protected override void OnNavigateTo(object sender, NavigateToMsgEventArgs e)
        {
            base.OnNavigateTo(sender, e);

            if (e.Object == null)
            {
                SecondAccountFramePage senderPage = PageContent as SecondAccountFramePage;
                if (senderPage != null)
                    senderPage.CurrentZone = null;
                return;
            }
            if (e.Object.Object is Zone)
            {
                Zone zone = e.Object.Object as Zone;
                SecondAccountFramePage senderPage = PageContent as SecondAccountFramePage;
                if (senderPage != null)
                {
                    senderPage.CurrentZone = zone;
                    IDbContext dbContext = DataBaseSource.GetDataBaseSource();
                    senderPage.secondAccountPanel.PersonBusiness = new VirtualPersonBusiness(dbContext);    //这里要更改二轮台账相关业务
                    senderPage.secondAccountPanel.DbContext = dbContext;
                    senderPage.secondAccountPanel.TableLandBusiness = new SecondTableLandBusiness(dbContext);
                    senderPage.secondAccountPanel.VirtualType = eVirtualType.SecondTable;
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
                    Editor = new SecondAccountConfigPage(Workpage)
                });
            }));
        }

        #endregion

        #endregion
    }
}
