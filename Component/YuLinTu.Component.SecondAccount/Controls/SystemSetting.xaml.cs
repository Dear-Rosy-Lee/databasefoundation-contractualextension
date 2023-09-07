/*
 * (C) 2015  鱼鳞图公司版权所有,保留所有权利 
 */

using System;
using System.Collections.Generic;
using System.Collections;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using YuLinTu.Windows.Wpf;
using YuLinTu.Windows.Wpf.Metro.Components;
using YuLinTu.Appwork;
using YuLinTu.Windows;
using YuLinTu.Library.Business;


namespace YuLinTu.Component.SecondAccount
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
            :base(workspace)
        {
            InitializeComponent();

            DataContext = this;
        }

        #endregion

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
                systemSet.CopyPropertiesFrom(SystemSettingDefine);
                center.Save<SystemSetDefine>();
            }));
        }

        #endregion
    }
}
