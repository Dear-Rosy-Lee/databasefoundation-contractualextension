using YuLinTu.Appwork;
using YuLinTu.Component.StockRightShuShan.Control;
using YuLinTu.Windows;

namespace YuLinTu.Component.StockRightShuShan
{
    public class WorkspaceContext : TheWorkspaceContext 
    {
        public WorkspaceContext(IWorkspace workspace)
            : base(workspace)
        {
            Register<MainFramePage, WorkPageContext>();
        }

        protected override void OnInstallNew(object sender, InstallNewEventArgs e)
        {
            e.Items.Add(new NewMetadata { Type = typeof(MainFramePage) });
        }

    }
}
