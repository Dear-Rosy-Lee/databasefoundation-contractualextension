/*
 * (C) 2025  鱼鳞图公司版权所有,保留所有权利 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YuLinTu.Appwork;
using YuLinTu.Appwork.Task;
using YuLinTu.Component.MapFoundation;
using YuLinTu.Components.Diagrams;
using YuLinTu.Components.tGIS;
using YuLinTu.Components.Visio;
using YuLinTu.Windows;

namespace YuLinTu.Component.DiagramFoundation
{
    internal class WorkspaceContext : TheWorkspaceContext
    {
        /// <summary>
        /// 注册鱼鳞图功能页面
        /// </summary>     
        public WorkspaceContext(IWorkspace workspace)
            : base(workspace)
        {
            //Register<YuLinTuDiagramFoundation, DiagramsPageContext>();
            Register<YuLinTuDiagramFoundation, VisioModePageContext>();
            Register<YuLinTuMapFoundation, MapPageContext>();
        }

        /// <summary>
        /// 不处理消息
        /// </summary>
        /// <returns></returns>
        //protected override bool NeedHandleMessage()
        //{
        //    return TheApp.Current.GetIsAuthenticated();
        //}

        /// <summary>
        /// 在主界面添加鱼鳞图功能模块
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected override void OnInstallNew(object sender, InstallNewEventArgs e)
        {
            e.Items.RemoveAll(c => c.Type == typeof(DiagramsPage) || c.Type == typeof(VisioPage));
        }
    }
}
