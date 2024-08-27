/*
 * (C) 2024  鱼鳞图公司版权所有,保留所有权利 
 */
using YuLinTu.Appwork;
using YuLinTu.Windows;

namespace YuLinTu.Component.Concord
{
    /// <summary>
    /// 工作空间上下文
    /// </summary>
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
            Register<ConcordFramePage, ConcordPageContext>();
        }

        #endregion

        #region Methods

        /// <summary>
        /// 不处理消息
        /// </summary>
        /// <returns></returns>
        //protected override bool NeedHandleMessage()
        //{
        //    return TheApp.Current.GetIsAuthenticated();
        //}

        /// <summary>
        /// 添加项
        /// </summary>
        protected override void OnInstallNew(object sender, InstallNewEventArgs e)
        {
            e.Items.Add(new NewMetadata { Type = typeof(ConcordFramePage) });
        }

        #endregion
    }
}
