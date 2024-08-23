/*
 * (C) 2024  鱼鳞图公司版权所有,保留所有权利 
 */
using MediaDevices;
using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using YuLinTu.Appwork;
using YuLinTu.Library.Command;
using YuLinTu.Library.Controls;
using YuLinTu.Windows.Wpf.Metro;

namespace YuLinTu.Component.PadDataHandle
{
    /// <summary>
    /// 任务包管理主界面
    /// </summary>
    [Newable(true,
        Order = 0,
        IsLanguageName = false,
        Name = "数据包管理",
        Description = "平板数据包管理",
        Category = "工具",
        Icon = "pack://application:,,,/YuLinTu.Resources;component/Images/16/map.png",
        Image = "pack://application:,,,/YuLinTu.Resources;component/Images/Galleries/Apps/Zone78.png",
        IsCreateShortcut = true,
        FontIcon = "\uf27c",
        IsNeedAuthenticated = true)]
    public partial class DataPakgeFramePage : WorkpageFrame
    {
        #region Fields

        DeviceHelper deviceHelper;

        /// <summary>
        /// 命令集合
        /// </summary>
        private ZoneCommand command;
        private DispatcherTimer _timer;
        private MediaDevice selectmediaDevice;


        /// <summary>
        /// 是否需要授权
        /// </summary>
        public override bool IsNeedAuthenticated
        {
            get
            {
                return true;
            }
        }

        #endregion

        #region Ctor

        public DataPakgeFramePage()
        {
            InitializeComponent();
            localMgrPanel.MenuEnable += SetControlsEnable;
            command = new ZoneCommand();
            SingleInstance = true;
            deviceHelper = new DeviceHelper();
            deviceHelper.DeviceChage += DeviceChage;
            Setpanelvisible(); 
        }

        #endregion

        #region Methods

        /// <summary>
        /// 初始化
        /// </summary>
        protected override void OnInstallComponents()
        {
            SetCommandToControl(); 
            string rootZone = NavigateZone.GetRootZoneCode(); 
            deviceHelper.RefreshDeviceList();
            var items = deviceHelper.MediaDevices;
            localMgrPanel.deviceview.ItemsSource = items;
            netdiskMgrPanel.Workpage= this.Workpage;
        } 

        /// <summary>
        /// 绑定命令到控件上
        /// </summary>
        private void SetCommandToControl()
        { 
            SetCommandBinding(mbtnRefresh, command.Refresh, command.RefreshBind);
            SetCommandBinding(mbtnUpToService, command.UpToService, command.UpToServiceBind);
        }

        /// <summary>
        /// 创建命令绑定
        /// </summary>
        private void SetCommandBinding(MetroButton button, RoutedCommand cmd, CommandBinding bind)
        {
            bind.CanExecute += CommandBinding_CanExecute;
            bind.Executed += CommandBinding_Executed;
            button.CommandBindings.Add(bind);
            button.Command = cmd;
            button.CommandParameter = cmd.Name;
        }

        /// <summary>
        /// 是否可以执行命令
        /// </summary>
        private void CommandBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (e.Source != null)
            {
                e.CanExecute = true;
            }
            else
            {
                e.CanExecute = false;
            }
        }

        /// <summary>
        /// 执行命令
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        { 
        }

        /// <summary>
        /// 设置控件可用性
        /// </summary>
        public void SetControlsEnable(bool isEnable = true)
        { 
        }

        #endregion

        /// <summary>
        /// 设备变化
        /// </summary>
        public void DeviceChage()
        {
            if (localMgrPanel.deviceview.SelectedItem==null)
            {
                localMgrPanel.softview.ItemsSource = null;
                return;
            }
            MediaDevice mediaDevice = (MediaDevice)localMgrPanel.deviceview.SelectedItem;
            if (!deviceHelper.MediaDevices.Contains(selectmediaDevice))
            {
                localMgrPanel.softview.ItemsSource = null;
            }
        } 

        private void mbtnAdd_Checked(object sender, RoutedEventArgs e)
        {
            if (mbtnEdit != null)
            {
                mbtnEdit.IsChecked = !mbtnAdd.IsChecked.Value;
            }
            Setpanelvisible();
        }

        private void mbtnEdit_Checked(object sender, RoutedEventArgs e)
        {
            if (mbtnAdd != null)
            {
                mbtnAdd.IsChecked = !mbtnEdit.IsChecked.Value;
            }
            Setpanelvisible();
        }

        /// <summary>
        /// 设置中间控件可见性
        /// </summary>
        public void Setpanelvisible()
        {
            var chk = mbtnAdd.IsChecked.Value;
            if (localMgrPanel != null)
                localMgrPanel.Visibility = chk ? Visibility.Visible : Visibility.Collapsed;
            if (netdiskMgrPanel != null)
                netdiskMgrPanel.Visibility = chk ? Visibility.Collapsed : Visibility.Visible;
        }
    }
}
