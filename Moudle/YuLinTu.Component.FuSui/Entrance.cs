using YuLinTu.Windows;

namespace YuLinTu.Component.FuSui
{
    public class Entrance : EntranceBase
    {
        #region Methods

        /// <summary>
        /// 重写注册工作空间方法
        /// </summary>
        protected override void OnConnect()
        {
            RegisterWorkspaceContext<WorkspaceContext>();
        }

        #endregion Methods
    }
}