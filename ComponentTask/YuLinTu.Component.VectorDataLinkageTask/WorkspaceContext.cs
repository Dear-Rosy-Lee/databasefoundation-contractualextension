using System;
using System.Windows;
using YuLinTu.Appwork;
using YuLinTu.Appwork.Task;
using YuLinTu.Windows;

namespace YuLinTu.Component.VectorDataLinkageTask
{
    /// <summary>
    /// 通用信息插件上下文
    /// </summary>
    public class WorkspaceContext : TheWorkspaceContext, YuLinTu.Messages.Workspace.IMessageHandlerInstallOptionsEditor
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

        public void InstallOptionsEditor(object sender, InstallOptionsEditorEventArgs e)
        {
            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                e.Editors.Add(new OptionsEditorMetadata()
                {
                    Name = "密匙",
                    Editor = new OptionsEditorAuthentication(this.Workspace),
                });
            }));
        }

        #endregion Ctor

        #region method

        /// <summary>
        /// 注册一张WPF页面，配置主页面
        /// </summary>
        protected override void OnInstallNew(object sender, InstallNewEventArgs e)
        {
        }

        //[MessageHandler(ID = EdCore.langValidateAuthenticatedKey)]
        //public virtual void ValidateAuthenticatedKey(object sender, AuthenticatedKeyEventArgs e)
        //{
        //    var codeZone = Workspace.Properties.TryGetValue<string>("CurrentZoneCode", null);

        //    e.ReturnValue = TheApp.Current.GetIsAuthenticated();

        //    if (!e.ReturnValue)
        //        return;
        //    if (TheApp.Current.GetValue<bool>("__TokenAuthenticated__"))
        //        return;

        //    var extends = TheApp.Current.GetAuthenticator().GetAuthenticatedKeyExtends();

        //    e.ReturnValue = e.ReturnValue && (
        //        codeZone == null ||
        //        extends.Contains("86") || (
        //        codeZone != null &&
        //        extends.Any(c => codeZone.StartsWith(c))));
        //}

        #endregion method
    }
}