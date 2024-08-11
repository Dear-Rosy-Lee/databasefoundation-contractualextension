/*
 * (C) 2024  鱼鳞图公司版权所有,保留所有权利 
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

namespace YuLinTu.Component.BusinessProcessWizard
{
    public class BusinessProcessWizardPageContext : BusinessNavigatableWorkpageContext
    {
        #region Fields

        /// <summary>
        /// 导航类
        /// </summary>
        private ZoneNavigator nav = new ZoneNavigator();

        #endregion

        #region Ctor

        public BusinessProcessWizardPageContext(IWorkpage workpage)
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
            BusinessProcessWizardFramePage page = PageContent as BusinessProcessWizardFramePage;
            if (page != null)
            {
                IDbContext dbContext = DataBaseSource.GetDataBaseSource();
                page.BusinessProcessWizardPanel.DbContext = dbContext;
                page.BusinessProcessWizardPanel.PersonBusiness = new VirtualPersonBusiness(dbContext);
                page.BusinessProcessWizardPanel.ConcordBusiness = new ConcordBusiness(dbContext);
                page.BusinessProcessWizardPanel.LandBusiness = new AccountLandBusiness(dbContext);
                page.BusinessProcessWizardPanel.VirtualType = eVirtualType.Land;
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
            Navigator.RegisterContextMenu(typeof(YuLinTu.Library.Entity.Zone), menu);
            Navigator.AddCommandBinding(ZoneNavigatorCommands.CopyCommandBinding);
        }

        /// <summary>
        /// 刷新界面
        /// </summary>
        private void RefreshUi()
        {
            Workpage.Workspace.Window.Dispatcher.Invoke(new Action(() =>
            {
                BusinessProcessWizardFramePage page = PageContent as BusinessProcessWizardFramePage;
                Navigator.Refresh();
                if (page != null)
                {
                    //page.dataSummaryExportPanel.Refresh();
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
                BusinessProcessWizardFramePage senderPage = PageContent as BusinessProcessWizardFramePage;
                if (senderPage != null)
                    senderPage.CurrentZone = null;
                return;
            }
            if (e.Object.Object is YuLinTu.Library.Entity.Zone)
            {
                YuLinTu.Library.Entity.Zone zone = e.Object.Object as YuLinTu.Library.Entity.Zone;
                BusinessProcessWizardFramePage senderPage = PageContent as BusinessProcessWizardFramePage;
                if (senderPage != null)
                {
                    senderPage.CurrentZone = zone;
                    IDbContext dbContext = DataBaseSource.GetDataBaseSource();
                    senderPage.BusinessProcessWizardPanel.DbContext = dbContext;
                    senderPage.BusinessProcessWizardPanel.PersonBusiness = new VirtualPersonBusiness(dbContext);
                    senderPage.BusinessProcessWizardPanel.ConcordBusiness = new ConcordBusiness(dbContext);
                    senderPage.BusinessProcessWizardPanel.LandBusiness = new AccountLandBusiness(dbContext);
                    senderPage.BusinessProcessWizardPanel.VirtualType = eVirtualType.Land;
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
                //e.Editors.Add(new WorkpageOptionsEditorMetadata()
                //{
                //    Name = "常规",
                //    Editor = new ConcordConfigPage(Workpage),
                //});
                //e.Editors.Add(new WorkpageOptionsEditorMetadata()
                //{
                //    Name = "合同明细表",
                //    Editor = new ConcordDetailTableConfigPage(Workpage),
                //});
            }));
        }

        #endregion

        #endregion

    }
}
