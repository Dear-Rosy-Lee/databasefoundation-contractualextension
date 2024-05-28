/*
 * (C) 2015  鱼鳞图公司版权所有,保留所有权利 
 */
using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using YuLinTu.Appwork;
using YuLinTu.Library.Business;
using YuLinTu.Library.Command;
using YuLinTu.Library.Controls;
using YuLinTu.Library.Entity;
using YuLinTu.Library.Log;
using YuLinTu.Windows;
using YuLinTu.Windows.Wpf.Metro;
using YuLinTu.Windows.Wpf.Metro.Components;

namespace YuLinTu.Component.Sender
{
    /// <summary>
    /// 地域管理主界面
    /// </summary>
    [Newable(true,
        Order = 0,
        IsLanguageName = false,
        Name = "发包方",
        Description = "发包方管理",
        Category = "应用",
        Icon = "pack://application:,,,/YuLinTu.Resources;component/Images/16/map.png",
        Image = "pack://application:,,,/YuLinTu.Resources;component/Images/Galleries/Apps/News78.png",
        IsCreateShortcut = true,
        FontIcon = "\uf19c",
        IsNeedAuthenticated = true)]
    public partial class SenderFramePage : NavigatableWorkpageFrame
    {
        #region Fields

        /// <summary>
        /// 发包方命令
        /// </summary>        
        private SenderCommand command = new SenderCommand();

        /// <summary>
        /// 当前地域
        /// </summary>
        private Zone currentZone;

        #endregion

        #region Properties

        /// <summary>
        /// 当前地域
        /// </summary>
        public Zone CurrentZone
        {
            get { return currentZone; }
            set
            {
                currentZone = value;
                senderPanel.SetControlData(currentZone);
            }
        }

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

        public SenderFramePage()
        {
            InitializeComponent();
            senderPanel.MenuEnable += SetControlsEnable;
            SingleInstance = true;
        }

        #endregion

        #region Methods

        /// <summary>
        /// 初始化
        /// </summary>
        protected override void OnInstallComponents()
        {
            base.OnInstallComponents();
            SetCommandToControl();
            senderPanel.ThePage = Workpage;
            senderPanel.ShowTaskViewer += () =>
            {
                Workpage.Message.Send(this, new MsgEventArgs(EdCore.langRequestShowTaskViewer));
            };
        }

        /// <summary>
        /// 绑定命令到控件上
        /// </summary>
        private void SetCommandToControl()
        {
            SetCommandBinding(mbtnAdd, command.Add, command.AddBind);
            SetCommandBinding(mbtnDel, command.Del, command.DelBind);
            SetCommandBinding(mbtnEdit, command.Edit, command.EditBind);
            SetCommandBinding(mbtnImportData, command.ImportData, command.ImportDataBind);
            SetCommandBinding(mbtnExportExcel, command.ExportExcel, command.ExportExcelBind);
            SetCommandBinding(mbtnExportWord, command.ExportWord, command.ExportWordBind);
            SetCommandBinding(mbtnExcelTemplate, command.ExportExcelTemplate, command.ExcelTemplateBind);
            SetCommandBinding(mbtnWordTemplate, command.ExportWordTemplate, command.WordTemplateBind);
            SetCommandBinding(mbtnRefresh, command.Fresh, command.FreshBind);
            SetCommandBinding(mbtnInitial, command.Initialize, command.InitializeBind);
            SetCommandBinding(mbtnCombine, command.combinesender, command.CombineSenderData);
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
            btnexport.IsOpen = false;
            btntemplate.IsOpen = false;
            string parameter = e.Parameter.ToString();
            switch (parameter)
            {
                case SenderCommand.AddName:
                    senderPanel.Add();
                    break;
                case SenderCommand.EditName:
                    senderPanel.Edit();
                    break;
                case SenderCommand.DelName:
                    senderPanel.Del();
                    break;
                case SenderCommand.ImportDataName:
                    senderPanel.ImportData();
                    break;
                case SenderCommand.ExcelTemplateName:
                    senderPanel.ExportExcelTemplate();
                    break;
                case SenderCommand.WordTemplateName:
                    senderPanel.ExportWordTemplate();
                    break;
                case SenderCommand.ExportExcelName:
                    senderPanel.ExportExcelData();
                    break;
                case SenderCommand.ExportWordName:
                    senderPanel.ExportWord();
                    break;
                case SenderCommand.FreshName:
                    senderPanel.Refresh();
                    break;
                case SenderCommand.InitilizeName:
                    senderPanel.InitializeSender();
                    break;

                case SenderCommand.CombineData:
                    senderPanel.CombinSenderData();
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
                mbtnImportData.IsEnabled = isEnable;
                btnexport.IsEnabled = isEnable;
                btntemplate.IsEnabled = isEnable;
            }));
        }

        #endregion
    }
}
