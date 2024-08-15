/*
 * (C) 2024  鱼鳞图公司版权所有,保留所有权利
 */

using MediaDevices;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using YuLinTu.Library.Business;
using YuLinTu.Library.Entity;
using YuLinTu.Windows;
using YuLinTu.Windows.Wpf.Metro.Components;

namespace YuLinTu.Component.PadDataHandle
{
    /// <summary>
    /// 任务包管理界面
    /// </summary>
    public partial class DataManagerPanel : UserControl
    {
        #region Fields

        /// <summary>
        /// 定义后台线程
        /// </summary>
        private BackgroundWorker worker;
        /// <summary>
        /// 定义委托
        /// </summary>
        public delegate void TaskViewerShowDelegate();

        private MediaDevice selectmediaDevice;

        #endregion Fields

        #region Properties

        /// <summary>
        /// 工作空间
        /// </summary>
        public IWorkpage ThePage { get; set; }


        public DeviceHelper DeviceHelper { get; set; }

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


        #endregion Properties

        #region Ctor

        public DataManagerPanel()
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
        /// 任务完成
        /// </summary>
        private void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                MenuEnable();
                view.ItemsSource = null;
                view.Items.Clear();
                List<Zone> zoneList = e.Result as List<Zone>;
                if (zoneList != null && zoneList.Count > 0)
                {
                    InitializeControl(zoneList);
                }
                else
                {
                    view.Items.Add(new
                    {
                        Name = "未能显示数据,请检查数据源连接及数据库数据!",
                        Img = new BitmapImage(new Uri("pack://application:,,,/YuLinTu.Library.Resources;component/Resources/Identify16.png"))
                    });
                }
            }
            catch (Exception ex)
            {
                YuLinTu.Library.Log.Log.WriteException(ex.Source, "行政地域获取", ex.StackTrace);
            }
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
            worker.RunWorkerAsync(string.IsNullOrEmpty(code) ? "" : code);
        }

        #endregion Method-public

        #region Methods - Private

        /// <summary>
        /// 初始化控件
        /// </summary>
        private void InitializeControl(List<Zone> list)
        {
            if (list == null || list.Count == 0)
                return;

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
        private void view_SelectedChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (view.SelectedItem == null)
                return;
            selectmediaDevice = (MediaDevice)view.SelectedItem;
            if (selectmediaDevice == null) return;
            var folderlist = DeviceHelper.GetFolderList(selectmediaDevice);
            softview.ItemsSource = folderlist;
            //if (folderlist.Count > 0)
            //    softview.SelectedIndex = 0;
        }

        private void view_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {

        }

        private void deviceview_SelectedChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            var sview = ((System.Windows.Controls.Primitives.Selector)softview);
            if (sview.SelectedItem == null)
                return;
            DeviceFolder deviceFolder = (DeviceFolder)sview.SelectedItem;
            if (deviceFolder == null) return;
            var list = DeviceHelper.GetDataList(deviceFolder, (MediaDevice)view.SelectedItem);
            view.ItemsSource = list;
        }
    }
}