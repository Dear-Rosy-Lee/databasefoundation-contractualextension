using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using YuLinTu;
using YuLinTu.Appwork;
using YuLinTu.Windows;

namespace YuLinTu.Appwork.Apps.Samples.Examples
{
    [Workspace(null)]
    public class WorkspaceContext_OptionsEditorService : TheWorkspaceContext,
        YuLinTu.Messages.Workspace.IMessageHandlerInstallOptionsEditorServices
    {
        #region Fields

        #endregion

        #region Ctor

        public WorkspaceContext_OptionsEditorService(IWorkspace workspace)
            : base(workspace)
        {
        }

        #endregion

        #region Methods

        #region Methods - Override

        protected override void OnInstallComponent(object sender, InstallComponentEventArgs e)
        {
        }

        protected override void OnInstallAccountData(object sender, AccountEventArgs e)
        {
        }

        protected override void OnUninstallAccountData(object sender, AccountEventArgs e)
        {
        }

        public void InstallOptionsEditorServices(object sender, InstallUIElementsEventArgs e)
        {
            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                e.Items.Add(new OptionsEditorWorkspaceGeneral(Workspace));
            }));
        }

        #endregion

        #region Methods - Private

        #endregion

        #endregion
    }
}
