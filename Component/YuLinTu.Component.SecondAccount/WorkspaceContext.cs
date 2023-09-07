/*
 * (C) 2015  鱼鳞图公司版权所有,保留所有权利 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using YuLinTu.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using YuLinTu.Appwork;
using YuLinTu.Appwork.New;

namespace YuLinTu.Component.SecondAccount
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
        /// <param name="workspace">工作空间</param>
        public WorkspaceContext(IWorkspace workspace)
            : base(workspace)
        {
            Register<SecondAccountFramePage, SecondAccountPageContext>();
        }

        #endregion

        #region method

        /// <summary>
        /// 不处理消息
        /// </summary>
        /// <returns></returns>
        protected override bool NeedHandleMessage()
        {
            return TheApp.Current.GetIsAuthenticated();
        }

        /// <summary>
        /// 注册了一张页面，即二轮台账的主界面
        /// </summary>
        protected override void OnInstallNew(object sender, InstallNewEventArgs e)
        {
            e.Items.Add(new NewMetadata { Type = typeof(SecondAccountFramePage) });
        }

        #endregion
    }
}
