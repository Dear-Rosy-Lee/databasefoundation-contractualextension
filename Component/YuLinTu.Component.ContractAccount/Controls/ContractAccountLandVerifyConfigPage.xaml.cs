using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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
using YuLinTu.Library.Business;
using YuLinTu.Library.Controls;
using YuLinTu.Windows;
using YuLinTu.Windows.Wpf.Metro.Components;

namespace YuLinTu.Component.ContractAccount
{
    /// <summary>
    /// ContractAccountLandVerifyConfigPage.xaml 的交互逻辑
    /// </summary>
    public partial class ContractAccountLandVerifyConfigPage : WorkpageOptionsEditor
    {
        public ContractAccountLandVerifyConfigPage(IWorkpage workpage)
            : base(workpage)
        {
            InitializeComponent();
        }

        #region Fields

        private LandVerifyDefine config;
        private LandVerifyDefine otherDefine;
        private SettingsProfileCenter systemCenter;

        #endregion Fields

        #region Properties

        /// <summary>
        /// 数据汇总设置实体属性
        /// </summary>
        public LandVerifyDefine OtherDefine
        {
            get { return otherDefine; }
            set
            {
                otherDefine = value;
                this.ProGrid.Object = OtherDefine;
            }
        }

        /// <summary>
        /// 常用配置页
        /// </summary>
        public PropertyGrid ProGrid
        { get { return this.propertyGrid; } }

        #endregion Properties

        #region Method-Override

        protected override void OnShown()
        {
            var are = new AutoResetEvent(false);

            Dispatcher.Invoke(new Action(() =>
            {
                var propertyCount = typeof(LandVerifyDefine).GetProperties().Count();
                ProGrid.Properties["index"] = CommonConfigSelector.GetConfigColumnInfo(propertyCount); //获取定义的数据源

                systemCenter = TheApp.Current.GetSystemSettingsProfileCenter();  //系统配置
                var profile = systemCenter.GetProfile<LandVerifyDefine>();
                var section = profile.GetSection<LandVerifyDefine>();  //得到section部分 此方法已经判断了section为空的情况，为空就用默认构造
                config = (section.Settings as LandVerifyDefine);   //得到经反序列化后的对象

                ProGrid.InitializeEnd += (s, e) => { are.Set(); };
                OtherDefine = config.Clone() as LandVerifyDefine;
            }));

            are.WaitOne();
        }

        /// <summary>
        /// 装载
        /// </summary>
        protected override void OnLoad()
        {
        }

        /// <summary>
        /// 保存
        /// </summary>
        protected override void OnSave()
        {
            Dispatcher.Invoke(new Action(() =>
            {
                config.CopyPropertiesFrom(OtherDefine);
                systemCenter.Save<LandVerifyDefine>();
            }));
        }

        #endregion Method-Override
    }
}