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

namespace YuLinTu.Component.BusinessProcessWizard
{
    /// <summary>
    ///业务向导主框架
    /// </summary>
    [Newable(true,
        Order = -999,
        IsLanguageName = false,
        Name = "业务向导",
        Description = "业务流程向导",
        Category = "工具",
        Icon = "pack://application:,,,/YuLinTu.Component.Concord;component/Resources/map.png",
        Image = "pack://application:,,,/YuLinTu.Resources;component/Images/Galleries/Apps/Wizzard78.png",
        IsNeedAuthenticated = true)]
    public partial class BusinessProcessWizardFramePage:NavigatableWorkpageFrame
    {
        #region Fields

        /// <summary>
        /// 承包合同命令
        /// </summary>        
        private ConcordCommand command = new ConcordCommand();

        /// <summary>
        /// 当前地域
        /// </summary>
        private YuLinTu.Library.Entity.Zone currentZone;

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
        public YuLinTu.Library.Entity.Zone CurrentZone
        {
            get { return currentZone; }
            set
            {
                currentZone = value;
                BusinessProcessWizardPanel.CurrentZone = value;
            }
        }

        #endregion

        #region Ctor

        public BusinessProcessWizardFramePage()
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

            BusinessProcessWizardPanel.ThePage = Workpage;
            BusinessProcessWizardPanel.ShowTaskViewer += () =>
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
