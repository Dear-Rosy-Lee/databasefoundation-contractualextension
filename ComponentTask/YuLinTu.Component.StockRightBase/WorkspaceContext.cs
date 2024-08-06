using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YuLinTu.Appwork;
using YuLinTu.Component.ContractAccount;
using YuLinTu.Component.StockRightBase.Control;
using YuLinTu.Windows;

namespace YuLinTu.Component.StockRightBase
{
    public class WorkspaceContext : TheWorkspaceContext 
    {
        public WorkspaceContext(IWorkspace workspace)
            : base(workspace)
        {
            Register<MainFramePage, WorkpageContext>();
        }

        protected override void OnInstallNew(object sender, InstallNewEventArgs e)
        {
            e.Items.Add(new NewMetadata { Type = typeof(MainFramePage) });
        }

    }
}
