/*
 * (C) 2015  鱼鳞图公司版权所有,保留所有权利
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
            AutoUpdater.CheckForUpdateEvent += AutoUpdaterOnCheckForUpdateEvent;
            //AutoUpdater.ParseUpdateInfoEvent += AutoUpdaterOnParseUpdateInfoEvent;
            //AutoUpdater.ApplicationExitEvent += AutoUpdater_ApplicationExitEvent;
            AutoUpdater.RemindLaterTimeSpan = RemindLaterFormat.Days;
            Thread.CurrentThread.CurrentCulture = Thread.CurrentThread.CurrentUICulture = System.Globalization.CultureInfo.CreateSpecificCulture("zh");
            AutoUpdater.AppTitle = "升级更新";
            AutoUpdater.RemindLaterAt = 2;
            AutoUpdater.Synchronous = true;
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
                AutoUpdater.InstallationPath = AppDomain.CurrentDomain.BaseDirectory;
                BasicAuthentication basicAuthentication = new BasicAuthentication(username, password);
                AutoUpdater.BasicAuthXML = AutoUpdater.BasicAuthDownload = AutoUpdater.BasicAuthChangeLog = basicAuthentication;
                AutoUpdater.OpenDownloadPage = true;
                AutoUpdater.Start(updateUrl);
            }
        }

        /// <summary>
        /// 更新软件
        /// </summary>
        private void AutoUpdaterOnParseUpdateInfoEvent(ParseUpdateInfoEventArgs args)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadFromString(args.RemoteData);
            var node = xmlDoc.SelectSingleNode("item");
            //dynamic json = JsonConvert.DeserializeObject(args.RemoteData);
            args.UpdateInfo = new UpdateInfoEventArgs
            {
                CurrentVersion = node.SelectSingleNode("version").InnerText,
                ChangelogURL = node.SelectSingleNode("changelog").InnerText,//json.changelog,
                DownloadURL = node.SelectSingleNode("url").InnerText,//json.url,
                Mandatory = new Mandatory
                {
                    Value = false,//json.mandatory.value,
                    UpdateMode = Mode.ForcedDownload,
                    MinimumVersion = node.SelectSingleNode("version").InnerText
                },
                CheckSum = new CheckSum
                {
                    Value = "E5F59E50FC91A9E52634FFCB11F32BD37FE0E2F1",
                    HashingAlgorithm = "SHA1"
                }
            };
        }

        private void AutoUpdaterOnCheckForUpdateEvent(UpdateInfoEventArgs args)
        {
            if (args == null)
            {
                return;
            }
            if (!args.IsUpdateAvailable)
            {
                System.Windows.MessageBox.Show($@"当前无可用的更新版本", @"可用更新", MessageBoxButton.OK, MessageBoxImage.Information);
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
                dialogResult = System.Windows.MessageBox.Show($@"当前最新版本 {args.CurrentVersion} 可用. 这是个必要的更新，点击确定开始更新程序...", @"可用更新",
                        MessageBoxButton.OK,
                        MessageBoxImage.Question);
            }
            else
            {
                dialogResult = System.Windows.MessageBox.Show($@"当前最新版本 {args.CurrentVersion} 可用. 是否要下载更新?
更新内容：
{stringBuilder.ToString()}", @"可用更新",
                        MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.Yes);
            }
            //AutoUpdater.DownloadUpdate(args);

            //DialogResult result = (DialogResult)System.Windows.MessageBox.Show("当前软件有更新，是否下载", "提示", MessageBoxButton.YesNo);
            if (dialogResult == MessageBoxResult.Yes || dialogResult == MessageBoxResult.OK)
            {
                if (AutoUpdater.DownloadUpdate(args))
                {
                    Thread.Sleep(5000);
                    System.Windows.Application.Current.Shutdown();
                }
            }
            else
            {
                System.Windows.MessageBox.Show("稍后更新！");
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