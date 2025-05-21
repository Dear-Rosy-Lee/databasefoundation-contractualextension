/*
 * (C) 2025  鱼鳞图公司版权所有,保留所有权利
 */

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
using YuLinTu.Windows;
using YuLinTu.Windows.Wpf.Metro.Components;

namespace YuLinTu.Component.ContractRegeditBook
{
    /// <summary>
    /// ContractRegeditBookConfigPage.xaml 的交互逻辑-承包权证常规设置页面
    /// </summary>
    public partial class ContractRegeditBookConfigPage : WorkpageOptionsEditor
    {
        #region Ctor

        public ContractRegeditBookConfigPage(IWorkpage workpage)
            : base(workpage)
        {
            InitializeComponent();
        }

        #endregion Ctor

        #region Fields

        private ContractRegeditBookSettingDefine config;
        private ContractRegeditBookSettingDefine otherDefine;
        private SettingsProfileCenter systemCenter;

        #endregion Fields

        #region Properties

        /// <summary>
        /// 数据汇总设置实体属性
        /// </summary>
        public ContractRegeditBookSettingDefine OtherDefine
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
        public PropertyGrid ProGrid { get { return this.propertyGrid; } }

        #endregion Properties

        #region Method-Override

        protected override void OnShown()
        {
            var are = new AutoResetEvent(false);

            Dispatcher.Invoke(new Action(() =>
            {
                systemCenter = TheApp.Current.GetSystemSettingsProfileCenter();  //系统配置
                var profile = systemCenter.GetProfile<ContractRegeditBookSettingDefine>();
                var section = profile.GetSection<ContractRegeditBookSettingDefine>();
                config = section.Settings;

                ProGrid.InitializeEnd += (s, e) => { are.Set(); };
                OtherDefine = config.Clone() as ContractRegeditBookSettingDefine;
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
                if (otherDefine.MaxNumber < otherDefine.MinNumber)
                {
                    ShowBox("系统设置", "流水区间设置最大值不能小于最小值");
                    return;
                }
                config.CopyPropertiesFrom(OtherDefine);
                systemCenter.Save<ContractRegeditBookSettingDefine>();
            }));
        }

        private void ShowBox(string header, string msg, eMessageGrade type = eMessageGrade.Error)
        {
            var showDlg = new TabMessageBoxDialog()
            {
                Header = header,
                Message = msg,
                MessageGrade = type,
                CancelButtonText = "取消",
            };
            Workpage.Page.ShowMessageBox(showDlg);
        }

        #endregion Method-Override

        #region Event

        private void ScrollViewer_PreviewMouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
        {
            var eventArg = new MouseWheelEventArgs(e.MouseDevice, e.Timestamp, e.Delta);
            eventArg.RoutedEvent = UIElement.MouseWheelEvent;
            eventArg.Source = sender;
            scrollViewer.RaiseEvent(eventArg);
        }

        #endregion Event
    }
}