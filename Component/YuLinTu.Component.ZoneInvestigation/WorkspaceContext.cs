/*
 * (C) 2015  鱼鳞图公司版权所有,保留所有权利 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YuLinTu.Appwork;
using YuLinTu.Windows;

namespace YuLinTu.Component.ZoneInvestigation
{
    internal class WorkspaceContext : TheWorkspaceContext
    {
        /// <summary>
        /// 注册鱼鳞图功能页面
        /// </summary>     
        public WorkspaceContext(IWorkspace workspace)
            : base(workspace)
        {
            Register<YuLinTuMapFoundation, MapPageContext>();
        }

        /// <summary>
        /// 在主界面添加鱼鳞图功能模块
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected override void OnInstallNew(object sender, InstallNewEventArgs e)
        {
            e.Items.Add(new NewMetadata { Type = typeof(YuLinTuMapFoundation) });            
        }
    }
}
