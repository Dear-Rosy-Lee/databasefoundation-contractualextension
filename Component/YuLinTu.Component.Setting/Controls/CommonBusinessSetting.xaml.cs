/*
 * (C) 2025  鱼鳞图公司版权所有,保留所有权利 
 */
using System;
using System.Windows;
using YuLinTu.Windows.Wpf.Metro.Components;
using YuLinTu.Appwork;
using YuLinTu.Windows;
using YuLinTu.Library.Business;
using YuLinTu.Library.Controls;
using YuLinTu.Data;
using YuLinTu.Library.Entity;

namespace YuLinTu.Component.Setting
{
    /// <summary>
    /// 系统配置界面
    /// </summary>
    public partial class CommonBusinessSetting : OptionsEditor
    {
        #region Fields

        private SettingsProfileCenter center;
        private CommonBusinessDefine commonBusinessSet;
        private string currentDataSource;
        private string tempChangeZone;
        private string tempDateSource;

        #endregion

        #region Properties

        /// <summary>
        /// 依赖属性 绑定CommonBusinessDefine实体
        /// </summary>
        public CommonBusinessDefine CommonBusinessSettingDefine
        {
            get { return (CommonBusinessDefine)GetValue(CommonBusinessDefineProperty); }
            set { SetValue(CommonBusinessDefineProperty, value); }
        }

        public static readonly DependencyProperty CommonBusinessDefineProperty =
            DependencyProperty.Register("CommonBusinessSettingDefine", typeof(CommonBusinessDefine), typeof(CommonBusinessSetting));

        #endregion

        #region Ctor

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="workspace"></param>
        public CommonBusinessSetting(IWorkspace workspace)
            : base(workspace)
        {
            InitializeComponent();
            DataContext = this;
        }

        #endregion

        #region Methods

        #region Methods - Events

        /// <summary>
        /// 选择导航地域
        /// </summary>
        private void btnChangeCommon_Click(object sender, RoutedEventArgs e)
        {
            ZoneSelectorPanel zoneSelectorPanel = new ZoneSelectorPanel();
            zoneSelectorPanel.DbContext = DataSource.Create<IDbContext>(Workspace.Properties.TryGetValue("CurrentDataSourceName", TheBns.Current.GetDataSourceName()));
            zoneSelectorPanel.SelectorZone = new ZoneDataItem() { FullCode = CommonBusinessSettingDefine.CurrentZoneFullCode };
            Workspace.Window.ShowDialog(zoneSelectorPanel, (b, r) =>
            {
                if (!(bool)b)
                {
                    return;
                }
                this.CommonBusinessSettingDefine.CurrentZoneFullName = zoneSelectorPanel.RootZone.FullName;
                this.CommonBusinessSettingDefine.CurrentZoneFullCode = zoneSelectorPanel.RootZone.FullCode;
            });

        }

        /// <summary>
        /// 选择数据连接
        /// </summary>
        private void btnChangeDataSource_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new ConnectionStringsViewerDialog();
            Workspace.Window.ShowDialog(dlg, (b, r) =>
            {
                if (b.HasValue && b.Value && dlg.SelectedConnectionString != null)
                {
                    this.currentDataSource = dlg.SelectedConnectionString.Name;
                    this.CommonBusinessSettingDefine.CurrentDataSource = currentDataSource;
                }
            });
        }

        #endregion

        #region Methods - Override

        protected override void OnInstall()
        {
            Workspace.Message.Received += Message_Received;
        }

        protected override void OnUninstall()
        {
            Workspace.Message.Received -= Message_Received;
        }

        /// <summary>
        /// 加载数据，从配置文件中读取数据
        /// </summary>
        protected override void OnLoad()
        {
            Dispatcher.Invoke(new Action(() =>
            {
                center = TheApp.Current.GetSystemSettingsProfileCenter();
                var profile = center.GetProfile<CommonBusinessDefine>();
                var section = profile.GetSection<CommonBusinessDefine>();
                commonBusinessSet = (section.Settings as CommonBusinessDefine);
                tempChangeZone = commonBusinessSet.CurrentZoneFullCode;
                tempDateSource = commonBusinessSet.CurrentDataSource;
                CommonBusinessSettingDefine = commonBusinessSet.Clone() as CommonBusinessDefine;
            }));
        }

        /// <summary>
        /// 保存更新后设置状态
        /// </summary>
        protected override void OnSave()
        {
            Dispatcher.Invoke(new Action(() =>
            {
                commonBusinessSet.CopyPropertiesFrom(CommonBusinessSettingDefine);
                center.Save<CommonBusinessDefine>();

                if (tempChangeZone != commonBusinessSet.CurrentZoneFullCode)
                {
                    SettingsProfileChangedEventArgs settingsProfileArgs = new SettingsProfileChangedEventArgs() { Profile = new SettingsProfile() { Name = "CURRENTROOTCHANGE" } };
                    Workspace.Message.Send(this, settingsProfileArgs);
                }
            }));
        }

        private void Message_Received(object sender, MsgEventArgs e)
        {
            switch (e.ID)
            {
                case EdCore.langSettingsChanged:
                    var args = e as SettingsProfileChangedEventArgs;
                    if (args == null)
                        break;
                    if (args.Profile.Name == TheBns.stringDataSourceNameChangedMessageKey)
                    {
                        this.CommonBusinessSettingDefine.CurrentZoneFullName = "中国";
                        this.CommonBusinessSettingDefine.CurrentZoneFullCode = "86"; 
                    }
                        break;
                 
                default:
                    break;
            }
        }

        #endregion

        #endregion
    }
}
