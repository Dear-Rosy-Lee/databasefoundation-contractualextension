using System;
using YuLinTu.Appwork;
using YuLinTu.Library.Business;
using YuLinTu.Windows;

namespace YuLinTu.Component.ContractAccount
{
    /// <summary>
    /// DownloadSetting.xaml 的交互逻辑
    /// </summary>
    public partial class DownloadSetting : WorkpageOptionsEditor
    {
        public DownloadSetting(IWorkpage workpage) : base(workpage)
        {
            InitializeComponent();
        }

        #region Fields

        private DownloadSettingDefine config;
        private DownloadSettingDefine otherDefine;
        private SettingsProfileCenter systemCenter;

        #endregion

        #region Properties

        /// <summary>
        /// 承包经营权其他设置实体属性
        /// </summary>
        public DownloadSettingDefine OtherDefine
        {
            get { return otherDefine; }
            set
            {
                otherDefine = value;
                this.DataContext = OtherDefine;
            }
        }

        #endregion

        #region Method-Override

        /// <summary>
        /// 装载
        /// </summary>
        protected override void OnLoad()
        {
            Dispatcher.Invoke(new Action(() =>
            {
                systemCenter = TheApp.Current.GetSystemSettingsProfileCenter();  //系统配置
                var profile = systemCenter.GetProfile<DownloadSettingDefine>();
                var section = profile.GetSection<DownloadSettingDefine>();
                config = (section.Settings as DownloadSettingDefine);
                OtherDefine = config.Clone() as DownloadSettingDefine;
            }));
        }

        /// <summary>
        /// 保存
        /// </summary>
        protected override void OnSave()
        {
            Dispatcher.Invoke(new Action(() =>
            {
                config.CopyPropertiesFrom(OtherDefine);
                systemCenter.Save<DownloadSettingDefine>();
            }));
        }

        #endregion
    }
}
