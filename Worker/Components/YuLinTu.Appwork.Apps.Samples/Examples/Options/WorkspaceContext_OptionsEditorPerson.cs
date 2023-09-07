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
    public class WorkspaceContext_OptionsEditorPerson : TheWorkspaceContext,
        YuLinTu.Messages.Workspace.IMessageHandlerInstallOptionsEditor
    {
        #region Fields

        #endregion

        #region Ctor

        public WorkspaceContext_OptionsEditorPerson(IWorkspace workspace)
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

        public void InstallOptionsEditor(object sender, InstallOptionsEditorEventArgs e)
        {
            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                e.Editors.Add(new OptionsEditorMetadata()
                {
                    Name = "负责人",
                    Editor = new OptionsEditorWorkspacePerson(Workspace),
                });
            }));
        }

        #endregion

        #region Methods - Private

        #endregion

        #endregion
    }
}
