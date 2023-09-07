/*
 * (C) 2015  鱼鳞图公司版权所有,保留所有权利 
 */

using System;
using System.Windows;
using YuLinTu.Appwork;
using YuLinTu.Windows;
using YuLinTu.Library.Business;


namespace YuLinTu.Component.Setting
{
    /// <summary>
    /// SystemSetting.xaml 的交互逻辑
    /// </summary>
    public partial class SystemSetting : OptionsEditor
    {

        #region Fields

        private SettingsProfileCenter center;

        private SystemSetDefine systemSet;

        #endregion

        #region Properties

        public SystemSetDefine SystemSettingDefine
        {
            get { return (SystemSetDefine)GetValue(SystemSettingDefineProperty); }
            set { SetValue(SystemSettingDefineProperty, value); }
        }

        public static readonly DependencyProperty SystemSettingDefineProperty =
            DependencyProperty.Register("SystemSettingDefine", typeof(SystemSetDefine), typeof(SystemSetting));

        #endregion

        #region Ctor

        public SystemSetting(IWorkspace workspace)
            : base(workspace)
        {
            InitializeComponent();

            DataContext = this;
        }

        #endregion

        #region Methods - Override

        /// <summary>
        /// 
        /// </summary>
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
                var profile = center.GetProfile<SystemSetDefine>();
                var section = profile.GetSection<SystemSetDefine>();
                systemSet = (section.Settings as SystemSetDefine);
                SystemSettingDefine = systemSet.Clone() as SystemSetDefine;

            }));
        }

        protected override void OnSave()
        {
            Dispatcher.Invoke(new Action(() =>
            {
                var p1 = systemSet.BackUpPath;
                var p2 = systemSet.BackDay;

                systemSet.CopyPropertiesFrom(SystemSettingDefine);

                systemSet.BackUpPath = p1;
                systemSet.BackDay = p2;

                center.Save<SystemSetDefine>();
            }));
        }

        #endregion

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            SystemSettingDefine.CityTableHead = false;
            SystemSettingDefine.CountTableHead = false;
            SystemSettingDefine.TownTableHead = false;
            SystemSettingDefine.CountryTableHead = false;
            SystemSettingDefine.GroupTableHead = false;
        }
    }
}
