/*
 * (C) 2015  鱼鳞图公司版权所有,保留所有权利 
 */
using System;
using System.Windows.Input;
using YuLinTu.Appwork;
using YuLinTu.Library.Business;
using YuLinTu.Library.Command;
using YuLinTu.Library.Controls;
using YuLinTu.Windows.Wpf.Metro;

namespace YuLinTu.Component.Zone
{
    /// <summary>
    /// 地域管理主界面
    /// </summary>
    [Newable(true,
        Order = 0,
        IsLanguageName = false,
        Name = "行政区域",
        Description = "行政区域管理",
        Category = "应用",
        Icon = "pack://application:,,,/YuLinTu.Resources;component/Images/16/map.png",
        Image = "pack://application:,,,/YuLinTu.Resources;component/Images/Galleries/Apps/Zone78.png",
        IsCreateShortcut = true,
        FontIcon = "\uf27c",
        IsNeedAuthenticated = false)]
    public partial class ZoneFramePage : WorkpageFrame
    {
        #region Fields

        /// <summary>
        /// 命令集合
        /// </summary>
        private ZoneCommand command;
         
        #endregion

        #region Ctor

        public ZoneFramePage()
        {
            InitializeComponent();
            zoneMgrPanel.MenuEnable += SetControlsEnable;
            SingleInstance = true;
            command = new ZoneCommand();
        }

        #endregion

        #region Methods

        /// <summary>
        /// 初始化
        /// </summary>
        protected override void OnInstallComponents()
        {
            SetCommandToControl();
            zoneMgrPanel.ThePage = Workpage;
            zoneMgrPanel.ShowTaskViewer += () =>
            {
                Workpage.Message.Send(this, new MsgEventArgs(EdCore.langRequestShowTaskViewer));
            };
            string rootZone = NavigateZone.GetRootZoneCode();
            zoneMgrPanel.RootZoneCode = rootZone;
            zoneMgrPanel.SetControlData();
        }

        /// <summary>
        /// 绑定命令到控件上
        /// </summary>
        private void SetCommandToControl()
        {
            SetCommandBinding(mbtnAdd, command.Add, command.AddBind);
            SetCommandBinding(mbtnDel, command.Del, command.DelBind);
            SetCommandBinding(mbtnEdit, command.Edit, command.EditBind);
            SetCommandBinding(mbtnExportData, command.ExportData, command.ExportDataBind);
            SetCommandBinding(mbtnExportPackage, command.ExportPackage, command.ExportPackageBind);
            SetCommandBinding(mbtnExportShape, command.ExportShape, command.ExportShapeBind);
            SetCommandBinding(mbtnImportShape, command.ImportShape, command.ImportShapeBind);
            SetCommandBinding(mbtnImportData, command.ImportData, command.ImportDataBind);
            SetCommandBinding(mbtnClear, command.Clear, command.ClearBind);
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
            if (zoneMgrPanel == null)
            {
                return;
            }
            string parameter = e.Parameter.ToString();
            switch (parameter)
            {
                case ZoneCommand.AddName:
                    zoneMgrPanel.Add();
                    break;
                case ZoneCommand.EditName:
                    zoneMgrPanel.Edit();
                    break;
                case ZoneCommand.DelName:
                    zoneMgrPanel.Del();
                    break;
                case ZoneCommand.ImportDataName:
                    zoneMgrPanel.ImportData();
                    break;
                case ZoneCommand.ImportShapeName:
                    zoneMgrPanel.ImportShape();
                    break;
                case ZoneCommand.ExportDataName:
                    zoneMgrPanel.ExportData();
                    break;
                case ZoneCommand.ExportShapeName:
                    zoneMgrPanel.ExportShape();
                    break;
                case ZoneCommand.ExportPackageName:
                    zoneMgrPanel.ExportPackage();
                    break;
                case ZoneCommand.ClearName:
                    zoneMgrPanel.Clear();
                    break;
                case ZoneCommand.RefreshName:
                    zoneMgrPanel.Refresh();
                    break;
                case ZoneCommand.UpToServiceName:
                    zoneMgrPanel.UpdateToServie();
                    break;
            }
        }

        /// <summary>
        /// 设置控件可用性
        /// </summary>
        public void SetControlsEnable(bool isEnable = true)
        {
            System.Windows.Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                mbtnAdd.IsEnabled = isEnable;
                mbtnEdit.IsEnabled = isEnable;
                mbtnDel.IsEnabled = isEnable;
                mbtnClear.IsEnabled = isEnable;
                mbtnExportData.IsEnabled = isEnable;
                mbtnExportPackage.IsEnabled = isEnable;
                mbtnExportShape.IsEnabled = isEnable;
                mbtnImportShape.IsEnabled = isEnable;
                mbtnImportData.IsEnabled = isEnable;
            }));
        }

        #endregion
    }
}
