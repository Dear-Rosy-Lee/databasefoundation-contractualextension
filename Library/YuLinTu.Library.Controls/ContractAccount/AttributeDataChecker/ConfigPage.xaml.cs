/*
 * (C)2014 - 2015 公司版权所有,保留所有权利
*/
using System.Windows;
using System.Windows.Media;
using YuLinTu.Windows;
using YuLinTu.Windows.Wpf.Metro.Components;

namespace YuLinTu.Library.Controls
{
    /// <summary>
    /// 质检配置页
    /// </summary>
    public partial class ConfigPage : TabMessageBox
    {

        #region Fields

        /// <summary>
        /// 绑定检查项
        /// </summary>
        public ConfigBaseInfo deployInfo;

        /// <summary>
        /// 临时检查项
        /// </summary>
        public ConfigBaseInfo tempInfo;

        private int checkType;

        private ConfigManagerCentre ctc;

        #endregion

        #region Properties

        /// <summary>
        /// 配置信息
        /// </summary>
        public ConfigBaseInfo DeployInfo
        {
            get
            {
                return deployInfo;
            }
            set
            {
                deployInfo = value;
                tempInfo = deployInfo.Clone() as ConfigBaseInfo;
                ctc.CreatControlUi(tempInfo.TermSet, this.tcInfo);
                this.tcInfo.DataContext = tempInfo;
            }
        }


        public IWorkpage Page { get; set; }

        
        #endregion

        #region Ctor

        public ConfigPage()
        {
            InitializeComponent();
            ctc = new ConfigManagerCentre();
            
            checkType = 1;
        }
        /// <summary>
        /// 保存
        /// </summary>
        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            
        }

        /// <summary>
        /// 退出
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void canc_Click(object sender, RoutedEventArgs e)
        {
            //Page.Page.ShowMessageBox(
            //    new MessageDialog()
            //    {
            //        Header = "信息",
            //        Content = @"   确定退出配置界面?",
            //        Message = "sdfdsf",
            //        MessageGrade = YuLinTu.eMessageGrade.Infomation,
            //        CancelButtonVisibility = Visibility.Visible,
            //        ConfirmButtonVisibility = Visibility.Visible,
            //    }, (b, es) =>
            //    {
            //        if (!(bool)b)
            //        {
            //            e.Handled = false;
            //        }
            //        else
            //        {
            //            e.Handled = false;
            //        }
            //    });
        }
        #endregion

    }
}
