using YuLinTu.Windows;
using YuLinTu.Appwork;
using YuLinTu.Component.ContractAccount;

namespace YuLinTu.Component.FuSui
{
    public class WorkspaceContext : TheWorkspaceContext
    {
        #region Ctor

        /// <summary>
        /// 构造函数
        /// </summary>
        public WorkspaceContext(IWorkspace workspace)
            : base(workspace)
        {
            Register<ContractAccountFramePage, WorkpageContext>();
        }

        #endregion Ctor

    }
}