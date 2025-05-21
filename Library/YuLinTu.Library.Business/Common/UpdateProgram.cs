using System.Threading;
using System;
using AutoUpdaterDotNET;
using YuLinTu.Windows;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Windows;
using System.Net;
using System.Text;
using System.Xml;

namespace YuLinTu.Library.Business
{
    /// <summary>
    /// 更新程序
    /// </summary>
    public class UpdateProgram
    {
        public static bool ShoweAvailable;
        /// <summary>
        /// 初始化更新
        /// </summary>
        public static void InstallUpdateProgram()
        {
            AutoUpdater.RemindLaterTimeSpan = RemindLaterFormat.Minutes;
            Thread.CurrentThread.CurrentCulture = Thread.CurrentThread.CurrentUICulture = System.Globalization.CultureInfo.CreateSpecificCulture("zh");
            AutoUpdater.AppTitle = "升级更新";
            AutoUpdater.RemindLaterAt = 2;
            AutoUpdater.Synchronous = true;
            AutoUpdater.InstallationPath = AppDomain.CurrentDomain.BaseDirectory;
            AutoUpdater.DownloadPath = AppDomain.CurrentDomain.BaseDirectory;
            AutoUpdater.CheckForUpdateEvent += AutoUpdaterOnCheckForUpdateEvent;
        }

        private static void AutoUpdaterOnCheckForUpdateEvent(UpdateInfoEventArgs args)
        {
            if (args == null)
            {
                return;
            }
            if (!args.IsUpdateAvailable)
            {
                if (ShoweAvailable)
                    System.Windows.MessageBox.Show($@"当前已是最新版本", @"可用更新", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            string username = "admin";
            string password = "yltadmin";
            Thread.CurrentThread.CurrentCulture = Thread.CurrentThread.CurrentUICulture = System.Globalization.CultureInfo.CreateSpecificCulture("zh-CN");
            System.Net.WebClient client = new WebClient();
            string credentials = Convert.ToBase64String(System.Text.Encoding.ASCII.GetBytes(username + ":" + password));
            client.Headers.Add("Authorization", "Basic " + credentials);
            byte[] page = client.DownloadData(args.ChangelogURL);
            string content = System.Text.Encoding.UTF8.GetString(page);
            XmlDocument xml = new XmlDocument();
            xml.LoadFromString(content);
            XmlNode node2 = xml.SelectSingleNode("html//body//ul");
            StringBuilder stringBuilder = new StringBuilder();
            if (node2 != null)
            {
                int index = 1;
                foreach (var item in node2.ChildNodes)
                {
                    stringBuilder.AppendLine($"{index}. {((System.Xml.XmlElement)item).InnerText} ");
                    index++;
                }
            }
            MessageBoxResult dialogResult;
            if (args.Mandatory.Value)
            {
                dialogResult = System.Windows.MessageBox.Show($@"当前最新版本 {args.CurrentVersion} 可用. 这是个必要的更新，点击确定开始更新程序...", @"软件更新",
                        MessageBoxButton.OK,
                        MessageBoxImage.Question);
            }
            else
            {
                dialogResult = System.Windows.MessageBox.Show($@"当前最新版本 {args.CurrentVersion} 可用. 是否要下载更新?
更新内容：
{stringBuilder.ToString()}", @"软件更新",
                        MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.Yes);
            }
            //AutoUpdater.DownloadUpdate(args);

            //DialogResult result = (DialogResult)System.Windows.MessageBox.Show("当前软件有更新，是否下载", "提示", MessageBoxButton.YesNo);
            if (dialogResult == MessageBoxResult.Yes || dialogResult == MessageBoxResult.OK)
            {
                try
                {
                    if (AutoUpdater.DownloadUpdate(args))
                    {
                        Thread.Sleep(5000);
                        System.Windows.Application.Current.Shutdown();
                    }
                }
                catch (Exception ex)
                {
                    System.Windows.MessageBox.Show($"更新失败{ex.Message}", "软件更新", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK);
                }
            }

            //if (args.IsUpdateAvailable)
            //{
            //    // 显示更新提示或对话框
            //    MessageBoxResult result = MessageBox.Show(
            //        "有新版本可用，是否立即更新？",
            //        "更新提示",
            //        MessageBoxButton.YesNo,
            //        MessageBoxImage.Information);

            //    if (result == MessageBoxResult.Yes)
            //    {
            //        // 执行更新操作
            //        AutoUpdater.DownloadUpdate(args);
            //    }
            //}
            //else
            //{
            //    // 无需更新，可以添加相应的逻辑
            //}
        }

        public static async void CheckUpdate(bool showMsg = true)
        {
            ShoweAvailable = showMsg;
            var center = TheApp.Current.GetSystemSettingsProfileCenter();
            var profile = center.GetProfile<ServiceSetDefine>();
            var section = profile.GetSection<ServiceSetDefine>();
            var serviceSet = (section.Settings as ServiceSetDefine);
            string updateUrl = $"{serviceSet.BusinessSecurityAddress}/update.xml";
            string uplogUrl = $"{serviceSet.BusinessSecurityAddress}/changelog.html";
            string username = "admin";
            string password = "yltadmin";
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(System.Text.Encoding.ASCII.GetBytes($"{username}:{password}")));
            try
            {
                HttpResponseMessage response = await client.GetAsync(updateUrl);
                if (response.IsSuccessStatusCode)
                {
                    BasicAuthentication basicAuthentication = new BasicAuthentication(username, password);
                    AutoUpdater.BasicAuthXML = AutoUpdater.BasicAuthDownload = AutoUpdater.BasicAuthChangeLog = basicAuthentication;
                    //AutoUpdater.OpenDownloadPage = true;
                    AutoUpdater.Start(updateUrl);
                }
            }
            catch { 
            }
        }
    }
}
