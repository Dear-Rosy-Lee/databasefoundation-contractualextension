/*
 * (C) 2015  鱼鳞图公司版权所有,保留所有权利 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace YuLinTu.Library.Controls
{
    /// <summary>
    /// 初始化图幅编号界面
    /// </summary>
    public partial class ContractLandInitialImageNumber : InfoPageBase
    {
        #region Ctor

        /// <summary>
        /// 构造函数
        /// </summary>
        public ContractLandInitialImageNumber()
        {
            InitializeComponent();
        }

        #endregion

        #region Fields

        #endregion

        #region Properties

        /// <summary>
        /// 图幅类型索引
        /// </summary>
        public int ScropeIndex { get; set; }

        /// <summary>
        /// 比例尺索引
        /// </summary>
        public int ScalerIndex { get; set; }

        /// <summary>
        /// 是否使用地理坐标系
        /// </summary>
        public bool IsUseYX { get; set; }

        /// <summary>
        /// 是否初始化所有的图幅表哈
        /// </summary>
        public bool IsInitialAllImageNumber { get; set; }

        #endregion

        #region Events

        /// <summary>
        /// 图幅类型改变
        /// </summary>
        private void cmbScrope_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
        }

        /// <summary>
        /// 比例尺改变
        /// </summary>
        private void txtScaler_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //cmbScrope.Items.Clear();
            //if (cmbScaler.SelectedIndex == 3)
            //{
            //    //此时选择的比例尺是 1:500
            //    cmbScrope.Items.Add("25X25");
            //    cmbScrope.Items.Add("50X50");
            //}
            //else
            //{
            //    cmbScrope.Items.Add("50X50");
            //}
            //cmbScrope.SelectedIndex = 0;
        }

        /// <summary>
        /// 确定
        /// </summary>
        private void btnConfirm_Click(object sender, RoutedEventArgs e)
        {
            ScropeIndex = cmbScrope.SelectedIndex;
            ScalerIndex = cmbScaler.SelectedIndex;
            IsUseYX = (bool)cbUseYX.IsChecked;
            IsInitialAllImageNumber = (bool)cbInitialAllNumber.IsChecked;
            Workpage.Page.CloseMessageBox(true);
        }

        #endregion

        #region Methods

        /// <summary>
        /// 初始化控件开始
        /// </summary>
        protected override void OnInitializeGo()
        {
        }

        /// <summary>
        /// 初始化控件完成
        /// </summary>
        protected override void OnInitializeCompleted()
        {
            //比例尺控件
            List<string> scalerString = new List<string>() { "1:2000", "1:1000", "1:500" };
            cmbScaler.ItemsSource = scalerString;
            cmbScaler.SelectedIndex = 0;

            //图幅范围控件
            List<string> scropeString = new List<string>() { "50X50" };
            cmbScrope.ItemsSource = scropeString;
            cmbScrope.SelectedIndex = 0;
            cmbScrope.IsEnabled = false;
        }

        #endregion
    }
}
