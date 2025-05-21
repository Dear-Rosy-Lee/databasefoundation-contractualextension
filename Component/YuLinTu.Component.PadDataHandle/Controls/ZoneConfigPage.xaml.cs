/*
 * (C) 2024  鱼鳞图公司版权所有,保留所有权利 
 */
using Aspose.Cells.Drawing;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using Xceed.Wpf.Toolkit.Core;
using YuLinTu.Appwork;
using YuLinTu.Library.Business;
using YuLinTu.Windows;
using YuLinTu.Windows.Wpf;

namespace YuLinTu.Component.PadDataHandle
{
    /// <summary>
    /// 任务包系统配置界面
    /// </summary>
    public partial class ZoneConfigPage : WorkpageOptionsEditor
    {
        #region Ctor

        /// <summary>
        /// 构造函数
        /// </summary>
        public ZoneConfigPage(IWorkpage workpage)
            : base(workpage)
        {
            InitializeComponent();
        }

        #endregion

        #region Fields

        private SystemSetDefine config;
        private SystemSetDefine CurrentDefine;
        private SettingsProfileCenter systemCenter;



        #endregion

        #region Properties

        public String DataExchangeDirectory
        {
            get { return CurrentDefine.DataExchangeDirectory; }
            set {
                if (CurrentDefine.DataExchangeDirectory != value)
                {
                    CurrentDefine.DataExchangeDirectory = value;
                    //DataContext = CurrentDefine;
                }
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
                //var profile = systemCenter.GetProfile<SystemSetDefine>();
                //var section = profile.GetSection<SystemSetDefine>();
                config = SystemSetDefine.GetIntence();// (section.Settings);
                CurrentDefine = config.Clone() as SystemSetDefine;
                DataContext = CurrentDefine;
            }));
        }

        /// <summary>
        /// 保存
        /// </summary>
        protected override void OnSave()
        {
            Dispatcher.Invoke(new Action(() =>
            {
                // 获取最新系统设置，替换DataExchangeDirectory再存进去。
                var newDataExchangeDirectory = CurrentDefine.DataExchangeDirectory;
                var profile = systemCenter.GetProfile<SystemSetDefine>();
                var section = profile.GetSection<SystemSetDefine>();
                config = (section.Settings);
                CurrentDefine = config.Clone() as SystemSetDefine;
                CurrentDefine.DataExchangeDirectory = newDataExchangeDirectory;
                config.CopyPropertiesFrom(CurrentDefine);
                systemCenter.Save<SystemSetDefine>();
                // 保存后添加到TheApp的变量中。
                TheApp.Current.Add("DataExchangeDirectory", DataExchangeDirectory);
            }));
        }

        #endregion

        #region Events
        /// <summary>
        /// 选择目录的事件，弹窗选择目录
        /// </summary>
        public void SelectDirectory(object sender, RoutedEventArgs e)
        {
            var dlg = new System.Windows.Forms.FolderBrowserDialog();
            if (dlg.ShowDialog() != System.Windows.Forms.DialogResult.OK)
                return;
            string path = dlg.SelectedPath;
            DataExchangeDirectory = path;
        }

        #endregion

    }
}
