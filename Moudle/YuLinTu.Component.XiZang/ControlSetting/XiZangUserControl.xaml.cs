using System;
using System.Linq;
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
using YuLinTu.Appwork;
using YuLinTu.Component.XiZangLZ;
using YuLinTu.Library.Business;
using YuLinTu.Windows;
using YuLinTu.Windows.Wpf.Metro.Components;

namespace YuLinTu.Component.ContractAccount
{
    /// <summary>
    /// HLJUserControl.xaml 的交互逻辑
    /// </summary>
    public partial class XiZangSetting : WorkpageOptionsEditor
    {
        public XiZangSetting(IWorkpage workpage) : base(workpage)
        {
            InitializeComponent();
        }

        #region Fields

        private XiZangUserControlSettingDefine config;
        private XiZangUserControlSettingDefine otherDefine;
        private SettingsProfileCenter systemCenter;

        #endregion

        #region Properties

        /// <summary>
        /// 承包经营权其他设置实体属性
        /// </summary>
        public XiZangUserControlSettingDefine OtherDefine
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
                var profile = systemCenter.GetProfile<XiZangUserControlSettingDefine>();
                var section = profile.GetSection<XiZangUserControlSettingDefine>();
                config = (section.Settings as XiZangUserControlSettingDefine);
                OtherDefine = config.Clone() as XiZangUserControlSettingDefine;

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
                systemCenter.Save<XiZangUserControlSettingDefine>();
            }));
        }

        #endregion
    }
}
