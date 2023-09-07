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
using System.Windows.Controls;
using YuLinTu.Windows.Wpf.Metro.Components;
using YuLinTu.Appwork;
using YuLinTu.Windows.Wpf.Metro;
using Xceed.Wpf.Toolkit;
using Microsoft.Win32;
using System.Windows;

namespace YuLinTu.Component.Zone
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
            ZoneFramePage page = PageContent as ZoneFramePage;
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
                    var page = PageContent as ZoneFramePage;
                    if (page != null)
                    {
                        page.zoneMgrPanel.Refresh();
                    }
                }));
            }
        }

        #endregion
    }
}
