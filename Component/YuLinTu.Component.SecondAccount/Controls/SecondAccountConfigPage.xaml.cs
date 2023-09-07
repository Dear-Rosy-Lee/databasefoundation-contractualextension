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

namespace YuLinTu.Component.SecondAccount
{
    /// <summary>
    /// 二轮台账系统配置
    /// </summary>
    public partial class SecondAccountConfigPage : WorkpageOptionsEditor
    {
        #region Ctor

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="workpage"></param>
        public SecondAccountConfigPage(IWorkpage workpage)
            : base(workpage)
        {
            InitializeComponent();
        }

        #endregion

        #region Fields

        private SecondTableExportDefine secondTableDefine;
        private SettingsProfileCenter systemCenter;
        private SecondTableExportDefine config;

        #endregion

        #region Properties

        /// <summary>
        /// 二轮台账表格输出配置实体属性
        /// </summary>
        public SecondTableExportDefine SecondTableDefine
        {
            get { return secondTableDefine; }
            set
            {
                secondTableDefine = value;
                this.DataContext = SecondTableDefine;
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
                systemCenter = TheApp.Current.GetSystemSettingsProfileCenter();
                var profile = systemCenter.GetProfile<SecondTableExportDefine>();
                var section = profile.GetSection<SecondTableExportDefine>();
                config = section.Settings as SecondTableExportDefine;
                SecondTableDefine = config.Clone() as SecondTableExportDefine;
            }));
        }

        /// <summary>
        /// 保存
        /// </summary>
        protected override void OnSave()
        {
            Dispatcher.Invoke(new Action(() =>
            {
                config.CopyPropertiesFrom(SecondTableDefine);
                systemCenter.Save<SecondTableExportDefine>();
            }));
        }

        #endregion

    }
}
