/*
 * (C) 2017 鱼鳞图公司版权所有,保留所有权利 
 */
using System;
using System.Text;
using YuLinTu.Appwork;
using YuLinTu.Appwork.Task;
using YuLinTu.Windows;

namespace YuLinTu.Component.ExportResultDataBaseTask
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
            Register<TaskPage, TaskPageContext>();
        }

        #endregion

        #region method

        /// <summary>
        /// 注册一张WPF页面，配置主页面
        /// </summary>
        protected override void OnInstallNew(object sender, InstallNewEventArgs e)
        {
        }
        #endregion
    }
}
