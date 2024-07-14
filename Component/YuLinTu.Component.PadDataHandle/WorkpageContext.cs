/*
 * (C) 2024  鱼鳞图公司版权所有,保留所有权利 
 */
using System;
using System.Windows;
using YuLinTu.Appwork;
using YuLinTu.Windows;

namespace YuLinTu.Component.PadDataHandle
{
    /// <summary>
    /// 工作页面上下文
    /// </summary>
    public class WorkpageContext : BusinessNavigatableWorkpageContext
    {
        #region Fields

        #endregion

        #region Ctor

        public WorkpageContext(IWorkpage workpage)
            : base(workpage)
        {
        }

        #endregion

        #region Methods

        /// <summary>
        /// 登录触发
        /// </summary>
        protected override void OnInstallAccountData(object sender, AccountEventArgs e)
        {
            DataPakgeFramePage page = PageContent as DataPakgeFramePage;
            if (page == null)
                return;
            page.mbtnUpToService.Visibility = Visibility.Collapsed;
            Guid? guid = TheApp.Current.GetWorkspaceUserSessionCode(Workpage.Workspace);
            if (guid != null)
            {
                page.mbtnUpToService.Visibility = Visibility.Visible;
            }
        }

        [MessageHandler(ID = EdCore.langInstallWorkpageOptionsEditor)]
        private void langInstallWorkpageOptionsEditor(object sender, InstallWorkpageOptionsEditorEventArgs e)
        {
            Workpage.Workspace.Window.Dispatcher.Invoke(new Action(() =>
            {
                e.Editors.Add(new WorkpageOptionsEditorMetadata()
                {
                    Name = "常规",
                    Editor = new ZoneConfigPage(Workpage),
                });
            }));
        }

        /// <summary>
        /// 当配置中根级地域发生变化时触发该事件
        /// </summary> 
        protected override void OnSettingsChanged(object sender, SettingsProfileChangedEventArgs e)
        {
            if (e.Profile.Name == "CURRENTROOTCHANGE")
            {
                Workpage.Workspace.Window.Dispatcher.Invoke(new Action(() =>
                {
                    var page = PageContent as DataPakgeFramePage;
                    if (page != null)
                    {
                        //page.zoneMgrPanel.Refresh();
                    }
                }));
            }
        }

        #endregion
    }
}
