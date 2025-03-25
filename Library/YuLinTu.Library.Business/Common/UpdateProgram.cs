using System.Threading;
using System;
using AutoUpdaterDotNET;
using YuLinTu.Windows;
using System.Net.Http;
using System.Net.Http.Headers;

namespace YuLinTu.Library.Business
{
    /// <summary>
    /// 更新程序
    /// </summary>
    public class UpdateProgram
    {
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
        }

        public static async void CheckUpdate()
        {
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
            HttpResponseMessage response = await client.GetAsync(updateUrl);
            if (response.IsSuccessStatusCode)
            {
                BasicAuthentication basicAuthentication = new BasicAuthentication(username, password);
                AutoUpdater.BasicAuthXML = AutoUpdater.BasicAuthDownload = AutoUpdater.BasicAuthChangeLog = basicAuthentication;
                AutoUpdater.OpenDownloadPage = true;
                AutoUpdater.Start(updateUrl);
            }
        }
    }
}
