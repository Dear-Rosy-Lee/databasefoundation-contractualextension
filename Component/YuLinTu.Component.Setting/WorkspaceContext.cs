/*
 * (C) 2015  鱼鳞图公司版权所有,保留所有权利 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YuLinTu.Appwork;
using YuLinTu.Windows;

namespace YuLinTu.Component.Setting
{
    /// <summary>
    /// 配置插件工作空间上下文
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
        }

        #endregion

        #region method

        /// <summary>
        /// 注册一张WPF页面，配置主页面
        /// </summary>
        protected override void OnInstallNew(object sender, InstallNewEventArgs e)
        {
        }

        /// <summary>
        /// 系统管理选项卡
        /// </summary>
        [MessageHandler(ID = EdCore.langInstallOptionsEditor)]
        private void langInstallOptionsEditor(object sender, InstallOptionsEditorEventArgs e)
        {
            Workspace.Window.Dispatcher.Invoke(new Action(() =>
            {
                e.Editors.Insert(2,new OptionsEditorMetadata()
                {
                    Name = "数据库",
                    Editor = new SpatialReferenceSetting(Workspace),
                });
                e.Editors.Insert(3, new OptionsEditorMetadata()
                {
                    Name = "设置",
                    Editor = new SystemSetting(Workspace),
                });
            }));

            //Workspace.Window.Dispatcher.Invoke(new Action(() =>
            //{
            //    e.Editors.Add(new OptionsEditorMetadata()
            //    {
            //        Name = "服务",
            //        Editor = new ServiceSetting(Workspace),
            //    });
            //}));
            //Workspace.Window.Dispatcher.Invoke(new Action(() =>
            //{
            //    e.Editors.Add(new OptionsEditorMetadata()
            //    {
            //        Name = "坐标系",
            //        Editor = new SpatialReferenceSetting(Workspace),
            //    });
            //}));
        }

        [MessageHandler(ID = EdCore.langInstallOptionsEditorGeneral)]
        private void langInstallOptionsEditorGeneral(object sender, InstallUIElementsEventArgs e)
        {
            Workspace.Window.Dispatcher.Invoke(new Action(() =>
            {
                e.Items.Add(new CommonBusinessSetting(Workspace));
                //e.Items.Add(new SystemSetting(Workspace));
            }));

        }

        [MessageHandler(ID = EdCore.langInstallOptionsEditorServices)]
        private void langInstallOptionsEditorServices(object sender, InstallUIElementsEventArgs e)
        {
            Workspace.Window.Dispatcher.Invoke(new Action(() =>
            {
                e.Items.Add(new ServiceSetting(Workspace));
            }));

        }

        #endregion
    }
}
