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

namespace YuLinTu.Component.VirtualPerson
{
    /// <summary>
    /// 承包方系统配置界面
    /// </summary>
    public partial class VirtualPersonConfigPage : WorkpageOptionsEditor
    { 
        #region Ctor

        /// <summary>
        /// 构造函数
        /// </summary>
        public VirtualPersonConfigPage(IWorkpage workpage)
            : base(workpage)
        {
            InitializeComponent();
        }

        #endregion

        #region Fields

        private FamilyOtherDefine config;
        private FamilyOtherDefine otherDefine;
        private SettingsProfileCenter systemCenter;

        #endregion

        #region Properties

        /// <summary>
        /// 承包方其他设置实体属性
        /// </summary>
        public FamilyOtherDefine OtherDefine
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
                var profile = systemCenter.GetProfile<FamilyOtherDefine>();
                var section = profile.GetSection<FamilyOtherDefine>();
                config = (section.Settings as FamilyOtherDefine);
                OtherDefine = config.Clone() as FamilyOtherDefine;
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
                systemCenter.Save<FamilyOtherDefine>();
            }));
        }

        #endregion

        #region Events

        #endregion

    }
}
