/*
 * (C) 2025  鱼鳞图公司版权所有,保留所有权利
 */

using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows;
using System.Windows.Forms;
using System.Xml;
using AutoUpdaterDotNET;
using OSGeo.GDAL;
using YuLinTu.Appwork;
using YuLinTu.Library.Business;
using YuLinTu.Windows;

namespace YuLinTu.Component.Setting
{
    /// <summary>
    /// 服务配置
    /// </summary>
    public partial class ServiceSetting : OptionsEditor
    {
        #region Fields

        private SettingsProfileCenter center;

        private ServiceSetDefine serviceSet;

        #endregion Fields

        #region Properties

        public ServiceSetDefine ServiceSettingDefine
        {
            get { return (ServiceSetDefine)GetValue(ServiceSettingDefineProperty); }
            set { SetValue(ServiceSettingDefineProperty, value); }
        }

        public static readonly DependencyProperty ServiceSettingDefineProperty =
            DependencyProperty.Register("ServiceSettingDefine", typeof(ServiceSetDefine), typeof(ServiceSetting));

        #endregion Properties

        #region Ctor

        public ServiceSetting(IWorkspace workspace)
            : base(workspace)
        {
            InitializeComponent();
            InitializeData();
            ServiceSettingDefine = ServiceSettingDefine == null ? ServiceSetDefine.GetIntence() : ServiceSettingDefine;
            DataContext = this;
        }

        #endregion Ctor

        #region Methods - Override

        protected override void OnInstall()
        {
        }

        protected override void OnUninstall()
        {
        }

        protected override void OnLoad()
        {
            Dispatcher.Invoke(new Action(() =>
            {
                center = TheApp.Current.GetSystemSettingsProfileCenter();
                var profile = center.GetProfile<ServiceSetDefine>();
                var section = profile.GetSection<ServiceSetDefine>();
                serviceSet = (section.Settings as ServiceSetDefine);
                ServiceSettingDefine.CopyPropertiesFrom(serviceSet);//.Clone() as ServiceSetDefine;
            }));
        }

        protected override void OnSave()
        {
            Dispatcher.Invoke(new Action(() =>
            {
                serviceSet.CopyPropertiesFrom(ServiceSettingDefine);
                center.Save<ServiceSetDefine>();
            }));
        }

        private void InitializeData()
        {
            var asm = Assembly.GetEntryAssembly();
            var ver = asm.GetAttribute<AssemblyFileVersionAttribute>();
            if (ver != null)
                txtVer.Text = ver.Version;
            AutoUpdater.RemindLaterTimeSpan = RemindLaterFormat.Days;
            Thread.CurrentThread.CurrentCulture = Thread.CurrentThread.CurrentUICulture = System.Globalization.CultureInfo.CreateSpecificCulture("zh");
            AutoUpdater.AppTitle = "升级更新";
            AutoUpdater.RemindLaterAt = 2;
            AutoUpdater.Synchronous = true;
            AutoUpdater.InstallationPath = AppDomain.CurrentDomain.BaseDirectory;
            AutoUpdater.DownloadPath = AppDomain.CurrentDomain.BaseDirectory;
        }

        private async void CheckUpdate_Click(object sender, RoutedEventArgs e)
        {
            string updateUrl = $"{ServiceSettingDefine.BusinessSecurityAddress}/update.xml";
            string uplogUrl = $"{ServiceSettingDefine.BusinessSecurityAddress}/changelog.html";
            await VerifyLink(updateUrl);
            //("https://yourdomain.com/updates.xml", new TimeSpan(0, 24, 0));
        }

        private async System.Threading.Tasks.Task VerifyLink(string updateUrl)
        {
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
       
        private void AutoUpdater_ApplicationExitEvent()
        {
            Thread.Sleep(5000);
            System.Windows.Application.Current.Shutdown();

        }
    }

    #endregion Methods - Override
}