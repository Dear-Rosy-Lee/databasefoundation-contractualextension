/*
 * (C) 2015  鱼鳞图公司版权所有,保留所有权利 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media.Imaging;
using YuLinTu.Appwork;
using YuLinTu.Appwork.Task;
using YuLinTu.Windows;
using YuLinTu.Windows.Wpf.Metro;

namespace YuLinTu.Component.Common
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

        [MessageHandler(ID = EdCore.langValidateAuthenticatedKey)]
        public virtual void ValidateAuthenticatedKey(object sender, AuthenticatedKeyEventArgs e)
        {
            var codeZone = Workspace.Properties.TryGetValue<string>("CurrentZoneCode", null);

            e.ReturnValue = TheApp.Current.GetIsAuthenticated();

            if (!e.ReturnValue)
                return;
            //if (TheApp.Current.GetValue<bool>("__TokenAuthenticated__"))
            //    return;

            var extends = TheApp.Current.GetAuthenticatedKeyExtends();

            e.ReturnValue = e.ReturnValue && (
                codeZone == null ||
                codeZone == "86" ||
                extends.Contains("86") || (
                codeZone != null &&
                extends.Any(c => codeZone.StartsWith(c) || c.StartsWith(codeZone))));
        }

        protected override void OnWorkspaceWindowLoad(object sender, WorkspaceEventArgs e)
        {
            try
            {
                (e.Workspace.Window as IMetroWindow).Icon = BitmapFrame.Create(new Uri("pack://application:,,,/YuLinTu.Resources;component/Icons/SkyDriver.ico"));
                (e.Workspace.Window as IMetroWindow).WindowIcon = BitmapFrame.Create(new Uri("pack://application:,,,/YuLinTu.Resources;component/Icons/16/SkyDriver.ico"));
            }
            catch
            {

            }

        }

        #endregion
    }
}
