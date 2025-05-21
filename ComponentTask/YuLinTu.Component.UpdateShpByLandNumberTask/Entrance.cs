using YuLinTu.Windows;

namespace YuLinTu.Component.UpdateShpByLandNumberTask
{
    public class Entrance : EntranceBase
    {
        protected override void OnConnect()
        {
            RegisterWorkspaceContext<WorkspaceContext>();
        }
    }
}
        

