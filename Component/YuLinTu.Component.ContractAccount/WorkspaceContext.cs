/*
 * (C) 2025  鱼鳞图公司版权所有,保留所有权利
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YuLinTu.Appwork;
using YuLinTu.Windows;
using YuLinTu.Appwork.Task;

namespace YuLinTu.Component.ContractAccount
{
    /// <summary>
    /// 承包台账插件工作空间上下文
    /// </summary>
    public class WorkspaceContext : TheWorkspaceContext
    {
        #region Ctor

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="workspace">工作空间</param>
        public WorkspaceContext(IWorkspace workspace)
            : base(workspace)
        {
            Register<ContractAccountFramePage, ContractAccountPageContext>();
            //Register<BusinessProcessWizardFramePage, ContractAccountPageContext>();
        }

        #endregion Ctor

        #region Method

        /// <summary>
        /// 不处理消息
        /// </summary>
        /// <returns></returns>
        //protected override bool NeedHandleMessage()
        //{
        //    return TheApp.Current.GetIsAuthenticated();
        //}

        /// <summary>
        /// 注册了一张页面，即承包台账主页面
        /// </summary>
        protected override void OnInstallNew(object sender, InstallNewEventArgs e)
        {
            e.Items.Add(new NewMetadata { Type = typeof(ContractAccountFramePage) });
        }

        #endregion Method
    }
}