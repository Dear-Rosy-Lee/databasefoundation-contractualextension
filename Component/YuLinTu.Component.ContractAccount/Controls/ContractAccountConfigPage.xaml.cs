/*
 * (C) 2015  鱼鳞图公司版权所有,保留所有权利 
 */
using System;
using System.Collections.Generic;
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
using YuLinTu.Library.Business;
using YuLinTu.Windows;
using YuLinTu.Windows.Wpf.Metro.Components;

namespace YuLinTu.Component.ContractAccount
{
    /// <summary>
    /// 承包台账常规设置界面
    /// </summary>
    public partial class ContractAccountConfigPage:WorkpageOptionsEditor
    {
        #region Ctor
        
        /// <summary>
        /// 构造函数
        /// </summary>
        public ContractAccountConfigPage(IWorkpage workpage):base(workpage)
        {
            InitializeComponent();
        }

        #endregion

        #region Fields

        private ContractBusinessSettingDefine config;
        private ContractBusinessSettingDefine otherDefine;
        private SettingsProfileCenter systemCenter;

        #endregion

        #region Properties

        /// <summary>
        /// 承包经营权其他设置实体属性
        /// </summary>
        public ContractBusinessSettingDefine OtherDefine
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
                var profile = systemCenter.GetProfile<ContractBusinessSettingDefine>();
                var section = profile.GetSection<ContractBusinessSettingDefine>();
                config = (section.Settings as ContractBusinessSettingDefine);
                OtherDefine = config.Clone() as ContractBusinessSettingDefine;
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
                systemCenter.Save<ContractBusinessSettingDefine>();
            }));
        }

        #endregion

        #region Events

        #endregion
    }
}
