using System;
using YuLinTu.Component.Account.Services;
using YuLinTu.Security;
using YuLinTu.Windows;

namespace YuLinTu.Component.Account
{
    /// <summary>
    /// 插件入口
    /// </summary>
    public class Entrance : EntranceBase
    {
        #region Methods

        /// <summary>
        /// 重写注册工作空间方法
        /// </summary>
        protected override void OnConnect()
        {
            TheApp.Current["AccountServiceFactoryCallback"] = new Func<ISecurityQuery>(() => { return new AccountService(); });
            //RegisterWorkspaceContext<WorkspaceContext>();
            //var url = TheApp.Current.GetSystemSection().TryGetValue(
            //   AppParameters.stringDefaultSecurityService,
            //   AppParameters.stringDefaultSecurityServiceValue);
        }

        protected override void OnWorkspaceWindowShown(object sender, WorkspaceEventArgs e)
        {
            base.OnWorkspaceWindowShown(sender, e);
        }

        #endregion Methods
    }
}