/*
 * (C) 2025  鱼鳞图公司版权所有,保留所有权利 
 */
using System;
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
            //AutoUpdater.CheckForUpdateEvent += AutoUpdaterOnCheckForUpdateEvent;
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
                e.Editors.Insert(2, new OptionsEditorMetadata()
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
        //        private void AutoUpdaterOnCheckForUpdateEvent(UpdateInfoEventArgs args)
        //        {
        //            if (args == null)
        //            {
        //                return;
        //            }
        //            if (!args.IsUpdateAvailable)
        //            {
        //                System.Windows.MessageBox.Show($@"当前已是最新版本", @"可用更新", MessageBoxButton.OK, MessageBoxImage.Information);
        //                return;
        //            }

        //            string username = "admin";
        //            string password = "yltadmin";
        //            Thread.CurrentThread.CurrentCulture = Thread.CurrentThread.CurrentUICulture = System.Globalization.CultureInfo.CreateSpecificCulture("zh-CN");
        //            System.Net.WebClient client = new WebClient();
        //            string credentials = Convert.ToBase64String(System.Text.Encoding.ASCII.GetBytes(username + ":" + password));
        //            client.Headers.Add("Authorization", "Basic " + credentials);
        //            byte[] page = client.DownloadData(args.ChangelogURL);
        //            string content = System.Text.Encoding.UTF8.GetString(page);
        //            XmlDocument xml = new XmlDocument();
        //            xml.LoadFromString(content);
        //            XmlNode node2 = xml.SelectSingleNode("html//body//ul");
        //            StringBuilder stringBuilder = new StringBuilder();
        //            if (node2 != null)
        //            {
        //                int index = 1;
        //                foreach (var item in node2.ChildNodes)
        //                {
        //                    stringBuilder.AppendLine($"{index}. {((System.Xml.XmlElement)item).InnerText} ");
        //                    index++;
        //                }
        //            }

        //            MessageBoxResult dialogResult;
        //            if (args.Mandatory.Value)
        //            {
        //                dialogResult = System.Windows.MessageBox.Show($@"当前最新版本 {args.CurrentVersion} 可用. 这是个必要的更新，点击确定开始更新程序...", @"软件更新",
        //                        MessageBoxButton.OK,
        //                        MessageBoxImage.Question);
        //            }
        //            else
        //            {
        //                dialogResult = System.Windows.MessageBox.Show($@"当前最新版本 {args.CurrentVersion} 可用. 是否要下载更新?
        //更新内容：
        //{stringBuilder.ToString()}", @"软件更新",
        //                        MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.Yes);
        //            }
        //            //AutoUpdater.DownloadUpdate(args);

        //            //DialogResult result = (DialogResult)System.Windows.MessageBox.Show("当前软件有更新，是否下载", "提示", MessageBoxButton.YesNo);
        //            if (dialogResult == MessageBoxResult.Yes || dialogResult == MessageBoxResult.OK)
        //            {
        //                try
        //                {
        //                    if (AutoUpdater.DownloadUpdate(args))
        //                    {
        //                        Thread.Sleep(5000);
        //                        System.Windows.Application.Current.Shutdown();
        //                    }
        //                }
        //                catch (Exception ex)
        //                {
        //                    System.Windows.MessageBox.Show($"更新失败{ex.Message}", "软件更新", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK);
        //                }
        //            }

        //            AutoUpdater.CheckForUpdateEvent -= AutoUpdaterOnCheckForUpdateEvent;
        //        }

        #endregion
    }
}
