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

namespace YuLinTu.Component.ExportDataSummaryTask
{
    /// <summary>
    /// 汇总数据导出主框架
    /// </summary>
    [Newable(true,
        Order = 0,
        IsLanguageName = false,
        Name = "汇总数据",
        Description = "汇总数据导出",
        Category = "工具",
        Icon = "pack://application:,,,/YuLinTu.Component.Concord;component/Resources/map.png",
        Image = "pack://application:,,,/YuLinTu.Resources;component/Images/Galleries/Apps/Package78.png",
        IsNeedAuthenticated = true)]
    public partial class DataSummaryExportFramePage: NavigatableWorkpageFrame
    {
        #region Fields

        /// <summary>
        /// 承包合同命令
        /// </summary>        
        private ConcordCommand command = new ConcordCommand();

        /// <summary>
        /// 当前地域
        /// </summary>
        private Zone currentZone;

        #endregion

        #region Properties

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

        /// <summary>
        /// 当前地域
        /// </summary>
        public Zone CurrentZone
        {
            get { return currentZone; }
            set
            {
                currentZone = value;
                dataSummaryExportPanel.CurrentZone = value;
            }
        }

        #endregion

        #region Ctor

        public DataSummaryExportFramePage()
        {
            InitializeComponent();          
            SingleInstance = true;
            NavigatorType = eNavigatorType.TreeView;
        }

        #endregion

        #region Methods

        /// <summary>
        /// 初始化
        /// </summary>
        protected override void OnInstallComponents()
        {
            base.OnInstallComponents();

            dataSummaryExportPanel.ThePage = Workpage;
            dataSummaryExportPanel.ShowTaskViewer += () =>
            {
                Workpage.Message.Send(this, new MsgEventArgs(EdCore.langRequestShowTaskViewer));
            };
        }
              
        /// <summary>
        /// 设置控件可用性
        /// </summary>
        public void SetControlsEnable(bool isEnable = true)
        {
            System.Windows.Application.Current.Dispatcher.Invoke(new Action(() =>
           {             

           }));
        }

        #endregion
    }
}
