/*
 * (C) 2022 鱼鳞图公司版权所有,保留所有权利 
 */
using YuLinTu.Appwork;
using YuLinTu.Appwork.Task;
using YuLinTu.Windows;

namespace YuLinTu.Component.CoordinateTransformTask
{
    /// <summary>
    /// 通用信息插件上下文
    /// </summary>
    public class WorkspaceContext : TheWorkspaceContext
    {
        #region Ctor

        /// <summary>
        /// 构造函数
        /// </summary>
        public WorkspaceContext(IWorkspace workspace)
            : base(workspace)
        {
            //Register<YuLinTuMapFoundation, WorkpageContextExtention>();
            Register<TaskPage, TaskPageContext>();
        }

        #endregion

        #region method

        /// <summary>
        /// 注册一张WPF页面，配置主页面
        /// </summary>
        protected override void OnInstallNew(object sender, InstallNewEventArgs e)
        {
            //e.Items.RemoveAll(c => c.Type == typeof(MapPage));
        }
        #endregion
    }
}
