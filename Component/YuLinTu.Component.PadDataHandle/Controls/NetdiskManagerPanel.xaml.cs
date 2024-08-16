/*
 * (C) 2024  鱼鳞图公司版权所有,保留所有权利
 */

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using YuLinTu.Data;
using YuLinTu.Library.Business;
using YuLinTu.Library.Controls;
using YuLinTu.Library.Entity;
using YuLinTu.Windows;
using YuLinTu.Windows.Wpf.Metro.Components;

namespace YuLinTu.Component.PadDataHandle
{
    /// <summary>
    /// 任务包管理界面
    /// </summary>
    public partial class NetdiskManagerPanel : UserControl
    {
        #region Fields

        /// <summary>
        /// 定义后台线程
        /// </summary>
        private BackgroundWorker worker;

        /// <summary>
        /// 绑定集合
        /// </summary>
        private ObservableCollection<ZoneDataItem> bindList = new ObservableCollection<ZoneDataItem>();

        /// <summary>
        /// 定义委托
        /// </summary>
        public delegate void TaskViewerShowDelegate();

        #endregion Fields

        #region Properties

        /// <summary>
        /// 工作空间
        /// </summary>
        public IWorkpage ThePage { get; set; }

        /// <summary>
        /// 当前选择地域
        /// </summary>
        public Zone CurrentZone { get; set; }

        /// <summary>
        /// 根级地域编码
        /// </summary>
        public string RootZoneCode { get; set; }

        /// <summary>
        /// 显示任务
        /// </summary>
        public TaskViewerShowDelegate ShowTaskViewer { get; set; }

        /// <summary>
        /// 设置控件可用性委托
        /// </summary>
        public delegate void MenuEnableControl(bool isEnable = true);

        /// <summary>
        /// 委托属性
        /// </summary>
        public MenuEnableControl MenuEnable { get; set; }

        /// <summary>
        /// 行政地域常规设置
        /// </summary>
        public ZoneDefine ZoneDefine
        {
            get
            {
                var systemCenter = TheApp.Current.GetSystemSettingsProfileCenter();
                var profile = systemCenter.GetProfile<ZoneDefine>();
                var section = profile.GetSection<ZoneDefine>();
                var config = section.Settings as ZoneDefine;
                return config;
            }
        }

        #endregion Properties

        #region Ctor

        public NetdiskManagerPanel()
        {
            InitializeComponent();

            if (DesignerProperties.GetIsInDesignMode(this))
                return;
            LanguageAttribute.AddLanguage(YuLinTu.Library.Entity.Properties.Resources.langChs_eZoneLevel);
            LanguageAttribute.AddLanguage(YuLinTu.Library.Entity.Properties.Resources.langChs_Zone);
            worker = new BackgroundWorker();
            worker.DoWork += worker_DoWork;
            worker.RunWorkerCompleted += worker_RunWorkerCompleted;
        }

        /// <summary>
        /// 设置控件数据地域编码
        /// </summary>
        public void SetControlData(string zoneCode = "", IWorkpage workPage = null)
        {
            if (!string.IsNullOrEmpty(zoneCode))
            {
                RootZoneCode = zoneCode;
            }
            if (workPage != null)
            {
                this.ThePage = workPage;
            }
            worker.RunWorkerAsync(RootZoneCode);
        }

        /// <summary>
        /// 任务完成
        /// </summary>
        private void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
        }

        /// <summary>
        /// 执行任务
        /// </summary>
        private void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            MenuEnable(false);
        }

        #endregion Ctor

        #region Methods

        #region Method-public 

        /// <summary>
        /// 刷新地域
        /// </summary>
        public void Refresh()
        {
            //CurrentZone = null;
            var system = TheApp.Current.GetSystemSettingsProfileCenter();
            var profile = system.GetProfile<CommonBusinessDefine>();
            var section = profile.GetSection<CommonBusinessDefine>();
            var config = section.Settings as CommonBusinessDefine;
            string code = config.CurrentZoneFullCode;
            RefreshContent(code);
        }

        /// <summary>
        /// 刷新内容
        /// </summary>
        /// <param name="zoneCode"></param>
        public void RefreshContent(string code = "")
        {
            if (worker == null)
            {
                return;
            }
            if (worker.IsBusy)
            {
                return;
            }
            worker.RunWorkerAsync(string.IsNullOrEmpty(code) ? RootZoneCode : code);
        }


        #endregion Method-public

        #region Methods - Private

        /// <summary>
        /// 是否继续
        /// </summary>
        /// <returns></returns>
        private bool CanContinue(string header, string message)
        {
            if (CurrentZone == null)
            {
                ShowBox(header, message);
                return false;
            }
            else
            {
                return true;
            }
        }

        /// <summary>
        /// 获取服务配置
        /// </summary>
        private ServiceSetDefine GetServiceSetting()
        {
            SettingsProfileCenter systemCenter = TheApp.Current.GetSystemSettingsProfileCenter();  //系统配置
            var profile = systemCenter.GetProfile<ServiceSetDefine>();
            var section = profile.GetSection<ServiceSetDefine>();
            return section.Settings;
        }

        #endregion Methods - Private

        #endregion Methods

        #region Event

        /// <summary>
        /// 双击节点事件
        /// </summary>
        private void view_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
        }

        /// <summary>
        /// 选择节点
        /// </summary>
        private void view_SelectedChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
        }

        /// <summary>
        /// 是否显示展开符号
        /// </summary>
        private void view_HasItemsGetter(object sender, MetroViewItemHasItemsEventArgs e)
        {
            var item = e.Object as ZoneDataItem;
            if (item == null)
                return;
            e.HasItems = item.Children.Count > 0;
        }

        #endregion Event

        #region Helper 

        /// <summary>
        /// 消息提示框
        /// </summary>
        private void ShowBox(string title, string msg, eMessageGrade type = eMessageGrade.Error, Action<bool?, eCloseReason> action = null)
        {
            Dispatcher.Invoke(new Action(() =>
            {
                ThePage.Page.ShowDialog(new TabMessageBoxDialog()
                {
                    Header = title,
                    Message = msg,
                    MessageGrade = type,
                    CancelButtonText = "取消",
                }, action);
            }));
        }

        /// <summary>
        /// 发送消息(三种)
        /// </summary>
        private void SendMessasge(ModuleMsgArgs args)
        {
            ThePage.Message.Send(this, args);
            TheBns.Current.Message.Send(this, args);
            ThePage.Workspace.Message.Send(this, args);
        }

        #endregion Helper 
    }
}