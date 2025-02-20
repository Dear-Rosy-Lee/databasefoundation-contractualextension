/*
 * (C) 2025  鱼鳞图公司版权所有,保留所有权利 
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
using YuLinTu.Library.Entity;
using YuLinTu.Windows.Wpf.Metro.Components;

namespace YuLinTu.Library.Controls
{
    /// <summary>
    /// ContractLandInitializeAreaNumericFormat.xaml 的交互逻辑
    /// </summary>
    public partial class ContractLandInitializeAreaNumericFormat: InfoPageBase
    {
        #region Ctor

        /// <summary>
        /// 构造函数
        /// </summary>
        public ContractLandInitializeAreaNumericFormat(bool isBatch = false)
        {
            InitializeComponent();
            this.Header = isBatch ? "批量地块面积小数位计算" : "地块面积小数位计算";
            btnConfirm.IsEnabled = false;
        }

        #endregion

        #region Fields

        private int toAreaNumeric;  //小数位数
        private int toAreaModule;   //面积截取模式
        private bool toAwareArea;   //确权面积
        private bool toActualArea;//实测面积
        private bool toTableArea;//二轮合同面积
        private int ChkIndex = 0;//勾选面积个数

        #endregion

        #region Properties

        /// <summary>
        /// 小数位数
        /// </summary>
        public int ToAreaNumeric
        {
            get { return toAreaNumeric; }
            set
            {
                toAreaNumeric = value;              
            }
        }

        /// <summary>
        /// 面积截取模式
        /// </summary>
        public int ToAreaModule
        {
            get { return toAreaModule; }
            set
            {
                toAreaModule = value;               
            }
        }
        /// <summary>
        /// 确权面积
        /// </summary>
       public bool ToAwareArea
        {
            get { return toAwareArea; }
            set { toAwareArea = value; }
        }
        /// <summary>
        /// 实测面积
        /// </summary>
        public bool ToActualArea
        {
            get { return toActualArea; }
            set { toActualArea = value; }
        }
        /// <summary>
        /// 二轮合同面积
        /// </summary>
        public bool ToTableArea
        {
            get { return toTableArea; }
            set { toTableArea = value; }
        }

        #endregion

        #region Method



        #endregion

        #region Event

        /// <summary>
        /// 确定按钮
        /// </summary>
        private void btnConfirm_Click(object sender, RoutedEventArgs e)
        {
            if (txt_Point.Value == null)
            {
                ShowBox("提示", "小数位数不允许为空");
                return;
            }
            toAreaNumeric = (int)txt_Point.Value;
            toAreaModule = cbModule.SelectedIndex;
            toActualArea = (bool)cbToActualArea.IsChecked;
            toAwareArea = (bool)cbToAwareArea.IsChecked;
            toTableArea = (bool)cbToTableArea.IsChecked;
            Workpage.Page.CloseMessageBox(true);
        }       
        #endregion

        private void cbToChk_Click(object sender, RoutedEventArgs e)
        {
            CheckBox ck = sender as CheckBox;
            if ((bool)ck.IsChecked)
                ChkIndex++;
            else
                ChkIndex--;
            if (ChkIndex > 0)
                btnConfirm.IsEnabled = true;
            else
                btnConfirm.IsEnabled = false;

        }
        /// <summary>
        /// 消息显示框
        /// </summary>
        private void ShowBox(string header, string msg, eMessageGrade type = eMessageGrade.Error)
        {
            Workpage.Page.ShowMessageBox(new TabMessageBoxDialog
            {
                Header = header,
                Message = msg,
                MessageGrade = type,
                CancelButtonText = "取消",
            });
        }
    }
}
