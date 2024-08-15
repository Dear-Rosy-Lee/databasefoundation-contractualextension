/*
 * (C) 2024  鱼鳞图公司版权所有,保留所有权利 
 */
using MediaDevices;
using System;
using System.Collections.Generic;
using System.Linq;
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
            //_timer = new DispatcherTimer();
            //_timer.Interval = TimeSpan.FromSeconds(5);
            //_timer.Tick += Timer_Tick;
            //_timer.Start();
        }

        #endregion

        #region Methods

        /// <summary>
        /// 初始化
        /// </summary>
        protected override void OnInstallComponents()
        {
            SetCommandToControl();
            //zoneMgrPanel.ThePage = Workpage;
            //zoneMgrPanel.ShowTaskViewer += () =>
            //{
            //    Workpage.Message.Send(this, new MsgEventArgs(EdCore.langRequestShowTaskViewer));
            //};
            string rootZone = NavigateZone.GetRootZoneCode();
            //zoneMgrPanel.RootZoneCode = rootZone;
            //zoneMgrPanel.SetControlData();

            deviceHelper.RefreshDeviceList();
            var items = deviceHelper.MediaDevices;
            localMgrPanel.deviceview.ItemsSource = items;
        }

        ///// <summary>
        ///// 加载左侧内容栏
        ///// </summary>
        //protected override void OnInstalLeftSidebarTabItems(object sender, InstallUIElementsEventArgs e)
        //{
        //    if (Navigator == null)
        //    {
        //        return;
        //    }
        //    Navigator.RootItemAutoExpand = false;
        //    panel.TheWorkPage = Workpage;
        //    panel.OwerShipPanel.CurrentMapControl = MapControl;

        //    LandPanelControl = panel.OwerShipPanel.LandPanelControl;
        //    LandPanelControl.CurrentMapControl = MapControl;

        //    e.Items.Add(new MetroListTabItem()
        //    {
        //        Name = "",
        //        Header = new ImageTextItem()
        //        {
        //            ImagePosition = eDirection.Top,
        //            Text = "权属",
        //            ToolTip = "权属",  //LanguageAttribute.GetLanguage("lang3070005")
        //            Image = BitmapFrame.Create(new Uri("pack://application:,,,/YuLinTu.Resources;component/Images/32/Population32.png"))
        //        },
        //        Content = panel,
        //    });

        //    OnInstallDotTabItems(e);
        //    OnInstallCoilTabItem(e);
        //    OnInstallPointTabItem(e);
        //    OnInstallToolTabItem(e);

        //    var dic = new ResourceDictionary() { Source = new Uri("pack://application:,,,/YuLinTu.Library.Controls;component/Navigation/Res.xaml") };
        //    var key = new DataTemplateKey(typeof(NavigateZoneItem));
        //    if (Navigator != null)
        //    {
        //        Navigator.RegisterItemTemplate(typeof(NavigateZoneItem), dic[key] as DataTemplate);
        //    }
        //    var menu = dic["TreeViewNavigator_Menu_Zone"] as ContextMenu;
        //    Navigator.RegisterContextMenu(typeof(Zone), menu);
        //    Navigator.AddCommandBinding(ZoneNavigatorCommands.CopyCommandBinding);
        //}

        /// <summary>
        /// 绑定命令到控件上
        /// </summary>
        private void SetCommandToControl()
        {
            //SetCommandBinding(mbtnAdd, command.Add, command.AddBind);
            //SetCommandBinding(mbtnDel, command.Del, command.DelBind);
            //SetCommandBinding(mbtnEdit, command.Edit, command.EditBind);
            //SetCommandBinding(mbtnExportData, command.ExportData, command.ExportDataBind);
            //SetCommandBinding(mbtnExportPackage, command.ExportPackage, command.ExportPackageBind);
            //SetCommandBinding(mbtnExportShape, command.ExportShape, command.ExportShapeBind);
            //SetCommandBinding(mbtnImportShape, command.ImportShape, command.ImportShapeBind);
            //SetCommandBinding(mbtnImportData, command.ImportData, command.ImportDataBind);
            //SetCommandBinding(mbtnClear, command.Clear, command.ClearBind);
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
            //if (zoneMgrPanel == null)
            //{
            //    return;
            //}
            string parameter = e.Parameter.ToString();
            switch (parameter)
            {
                //case ZoneCommand.AddName:
                //    zoneMgrPanel.Add();
                //    break;
                //case ZoneCommand.EditName:
                //    zoneMgrPanel.Edit();
                //    break;
                //case ZoneCommand.DelName:
                //    zoneMgrPanel.Del();
                //    break;
                //case ZoneCommand.ImportDataName:
                //    zoneMgrPanel.ImportData();
                //    break;
                //case ZoneCommand.ImportShapeName:
                //    zoneMgrPanel.ImportShape();
                //    break;
                //case ZoneCommand.ExportDataName:
                //    zoneMgrPanel.ExportData();
                //    break;
                //case ZoneCommand.ExportShapeName:
                //    zoneMgrPanel.ExportShape();
                //    break;
                //case ZoneCommand.ExportPackageName:
                //    zoneMgrPanel.ExportPackage();
                //    break;
                //case ZoneCommand.ClearName:
                //    zoneMgrPanel.Clear();
                //    break;
                //case ZoneCommand.RefreshName:
                //    zoneMgrPanel.Refresh();
                //    break;
                //case ZoneCommand.UpToServiceName:
                //    zoneMgrPanel.UpdateToServie();
                //    break;
            }
        }

        /// <summary>
        /// 设置控件可用性
        /// </summary>
        public void SetControlsEnable(bool isEnable = true)
        {
            System.Windows.Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                //mbtnAdd.IsEnabled = isEnable;
                //mbtnEdit.IsEnabled = isEnable;
                //mbtnDel.IsEnabled = isEnable;
                //mbtnClear.IsEnabled = isEnable;
                //mbtnExportData.IsEnabled = isEnable;
                //mbtnExportPackage.IsEnabled = isEnable;
                //mbtnExportShape.IsEnabled = isEnable;
                //mbtnImportShape.IsEnabled = isEnable;
                //mbtnImportData.IsEnabled = isEnable;
            }));
        }

        #endregion

        /// <summary>
        /// 设备变化
        /// </summary>
        public void DeviceChage()
        {
            if (localMgrPanel.view.SelectedItem == null)
            {
                localMgrPanel.softview.ItemsSource = null;
                return;
            }
            MediaDevice mediaDevice = (MediaDevice)localMgrPanel.view.SelectedItem;
            if (!deviceHelper.MediaDevices.Contains(selectmediaDevice))
            {
                localMgrPanel.softview.ItemsSource = null;
            }
        }

        //private void Timer_Tick(object sender, EventArgs e)
        //{
        //    if (view.SelectedItem == null)
        //    {
        //        softview.ItemsSource = null;
        //        return;
        //    }
        //    MediaDevice mediaDevice = (MediaDevice)view.SelectedItem;
        //    if (mediaDevice == null) return;
        //    var folderlist = deviceHelper.GetFolderList(mediaDevice);
        //    softview.ItemsSource = folderlist;
        //} 

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
