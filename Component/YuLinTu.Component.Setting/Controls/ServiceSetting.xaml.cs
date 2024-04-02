/*
 * (C) 2015  鱼鳞图公司版权所有,保留所有权利
 */

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using YuLinTu.Windows;
using YuLinTu.Windows.Wpf.Metro.Components;
using YuLinTu.Appwork;

using YuLinTu.Windows;

using YuLinTu.Library.Business;
using AutoUpdaterDotNET;
using System.Net.Http;
using System.Net.Http.Headers;

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
                ServiceSettingDefine = serviceSet.Clone() as ServiceSetDefine;
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
        }

        private async void CheckUpdate_Click(object sender, RoutedEventArgs e)
        {
            string updateUrl = "http://192.168.20.16:8080/view/%E6%89%BF%E5%8C%85%E7%BB%8F%E8%90%A5%E6%9D%83%E7%A1%AE%E6%9D%83%E5%B7%A5%E5%85%B7/job/%E6%89%BF%E5%8C%85%E7%BB%8F%E8%90%A5%E6%9D%83%E5%BB%B6%E5%8C%85%E5%BB%BA%E5%BA%93%E5%B7%A5%E5%85%B7/ws/update.xml";
            await VerifyLink(updateUrl);
        }

        private async System.Threading.Tasks.Task VerifyLink(string updateUrl)
        {
            string username = "admin";
            string password = "jenkins@Admin2023";

            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(System.Text.Encoding.ASCII.GetBytes($"{username}:{password}")));

            HttpResponseMessage response = await client.GetAsync(updateUrl);

            if (response.IsSuccessStatusCode)
            {
                string xmlContent = response.Content.ReadAsStringAsync().Result;

                // 设置AutoUpdater的更新URL并启动

                AutoUpdater.Start(updateUrl);
                // Parse the XML response and handle update logic here
            }
        }

        private void AutoUpdaterOnCheckForUpdateEvent(UpdateInfoEventArgs args)
        {
            if (args != null)
            {
                if (args.IsUpdateAvailable)
                {
                    Console.WriteLine("A new version is available!");
                    Console.WriteLine($"New version number: {args.CurrentVersion}");
                    Console.WriteLine($"Download URL: {args.DownloadURL}");
                    // You can add more handling logic here, such as displaying a dialog to the user.
                }
                else
                {
                    Console.WriteLine("Your application is up to date.");
                }
            }
        }

        #endregion Methods - Override
    }
}