using YuLinTu.Windows;

namespace YuLinTu.Component.StockRightShuShan
{
    public class Entrance : EntranceBase
    {
        /// <summary>
        /// 重写注册工作空间方法
        /// </summary>
        protected override void OnConnect()
        {
            RegisterWorkspaceContext<WorkspaceContext>();
        }
    }
}
