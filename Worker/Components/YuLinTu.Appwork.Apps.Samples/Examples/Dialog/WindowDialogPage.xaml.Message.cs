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

namespace YuLinTu.Appwork.Apps.Samples.Examples.Dialog
{
    public partial class WindowDialogPageViewModel :
        YuLinTu.Messages.Workspace.IMessageHandlerInstallAccountData,
        YuLinTu.Messages.Workspace.IMessageHandlerUninstallAccountData,
        YuLinTu.Messages.Workpage.IMessageHandlerInstallWorkpageContent,
        YuLinTu.Messages.Workpage.IMessageHandlerUninstallWorkpageContent,
        YuLinTu.Messages.Workpage.IMessageHandlerInitializeWorkpageContentCompleted,
        YuLinTu.Messages.Workpage.IMessageHandlerInstallNavigateItem,
        YuLinTu.Messages.Workpage.IMessageHandlerNavigateTo
    {
        #region Methods

        #region Methods - Message

        public void InstallWorkpageContent(object sender, InstallWorkpageContentEventArgs e)
        {
        }

        public void UninstallWorkpageContent(object sender, UninstallWorkpageContentEventArgs e)
        {
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
        }

        public void NavigateTo(object sender, NavigateToMsgEventArgs e)
        {
        }

        #endregion

        #endregion
    }
}
