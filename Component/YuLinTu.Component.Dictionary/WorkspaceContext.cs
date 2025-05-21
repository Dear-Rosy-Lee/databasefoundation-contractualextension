/*
 * (C) 2025  鱼鳞图公司版权所有,保留所有权利 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YuLinTu.Appwork;
using YuLinTu.Windows;

namespace YuLinTu.Component.Dictionary
{
    /// <summary>
    /// 数据字典插件工作空间上下文
    /// </summary>
    public class WorkspaceContext:TheWorkspaceContext
    {
        #region Ctor

        /// <summary>
        /// 构造函数—用于传入工作空间参数,并注册数据字典窗体和数据字典工作页
        /// </summary>
        /// <param name="workspace">工作空间</param>
        public WorkspaceContext(IWorkspace workspace):base(workspace)
        {
            Register<DictFramePage,DictPageContext>();
        }

        #endregion

        #region method

        /// <summary>
        /// 不处理消息
        /// </summary>
        /// <returns></returns>
        //protected override bool NeedHandleMessage()
        //{
        //    return TheApp.Current.GetIsAuthenticated();
        //}

        /// <summary>
        /// 注册一张WPF页面，数据字典主页面
        /// </summary>
        protected override void OnInstallNew(object sender, InstallNewEventArgs e)
        {
            e.Items.Add(new NewMetadata { Type=typeof(DictFramePage)}); 
        }
 
        #endregion
    }
}
