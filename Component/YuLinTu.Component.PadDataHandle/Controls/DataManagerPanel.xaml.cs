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


        public DeviceHelper DvcHelper { get; set; }

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
            DvcHelper = new DeviceHelper();
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
        private void deviceview_SelectedChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (deviceview.SelectedItem == null)
                return;
            selectmediaDevice = (MediaDevice)deviceview.SelectedItem;
            if (selectmediaDevice == null) return;
            var folderlist = DvcHelper.GetFolderList(selectmediaDevice);
            softview.ItemsSource = folderlist;
            //if (folderlist.Count > 0)
            //    softview.SelectedIndex = 0;
        }

        private void view_SelectionChanged(object sender, SelectionChangedEventArgs e)
        { 
            if (softview.SelectedItems.Count == 0)
                return;
            DeviceFolder deviceFolder = (DeviceFolder)softview.SelectedItems[0];
            if (deviceFolder == null) return;
            var list = DvcHelper.GetDataList(deviceFolder, (MediaDevice)deviceview.SelectedItem);
            dataview.ItemsSource = list;
        }
    }
}