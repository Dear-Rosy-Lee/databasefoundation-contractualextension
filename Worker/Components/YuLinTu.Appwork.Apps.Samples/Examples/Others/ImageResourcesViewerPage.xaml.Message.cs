using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YuLinTu;
using YuLinTu.Appwork;
using YuLinTu.Data;
using YuLinTu.Windows;
using YuLinTu.Windows.Wpf;
using YuLinTu.Windows.Wpf.Metro;
using YuLinTu.Windows.Wpf.Metro.Components;

namespace YuLinTu.Appwork.Apps.Samples.Examples.Others
{
    public partial class ImageResourcesViewerPageViewModel :
        YuLinTu.Messages.Workspace.IMessageHandlerInstallAccountData,
        YuLinTu.Messages.Workspace.IMessageHandlerUninstallAccountData,
        YuLinTu.Messages.Workpage.IMessageHandlerInstallWorkpageContent,
        YuLinTu.Messages.Workpage.IMessageHandlerUninstallWorkpageContent,
        YuLinTu.Messages.Workpage.IMessageHandlerInitializeWorkpageContentCompleted,
        YuLinTu.Messages.Workpage.IMessageHandlerInstallNavigateItem,
        YuLinTu.Messages.Workpage.IMessageHandlerNavigateTo
    {
        #region Fields

        private ResourcesNavigator nav = null;

        #endregion

        #region Methods

        #region Methods - Message

        public void InstallWorkpageContent(object sender, InstallWorkpageContentEventArgs e)
        {
            Workpage.ActionQueue.
                DoAsync(() => { nav = new ResourcesNavigator(Resources.ResourcesHelper.GetResources()); });
        }

        public void UninstallWorkpageContent(object sender, UninstallWorkpageContentEventArgs e)
        {
            nav = null;
        }

        public void InitializeWorkpageContentCompleted(object sender, InitializeWorkpageContentCompletedEventArgs e)
        {
        }

        public void InstallAccountData(object sender, AccountEventArgs e)
        {
        }

        public void UninstallAccountData(object sender, AccountEventArgs e)
        {
        }

        public void InstallNavigateItem(object sender, InstallNavigateItemMsgEventArgs e)
        {
            e.Instance.Items.AddRange(nav.GetChildren(e.Instance.Root));
        }

        public void NavigateTo(object sender, NavigateToMsgEventArgs e)
        {
            CurrentCatalog = e.Object == null ? null : (e.Object.Object as CatalogDescriptor);
        }

        #endregion

        #endregion
    }
}
