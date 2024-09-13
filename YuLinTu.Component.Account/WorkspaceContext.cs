using YuLinTu.Appwork;
using YuLinTu.Windows;

namespace YuLinTu.Component.Account
{
    /// <summary>
    /// 工作空间上下文
    /// </summary>
    [Workspace(null)]
    public class WorkspaceContext : TheWorkspaceContext
    {
        #region Ctor

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="workspace"></param>
        public WorkspaceContext(IWorkspace workspace)
            : base(workspace)
        {
        }

        #endregion Ctor

        #region Methods

        /// <summary>
        /// 授权验证
        /// </summary>
        /// <returns></returns>
        protected override bool NeedHandleMessage()
        {
            return TheApp.Current.GetIsAuthenticated();
        }

        #endregion Methods
    }
}