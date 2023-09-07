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
using YuLinTu.Windows;
using YuLinTu.Windows.Wpf.Metro.Components;
using YuLinTu.Library.Business;

namespace YuLinTu.Library.Controls
{
    /// <summary>
    /// 承包合同用户申请表日期设置
    /// </summary>
    public partial class ContractConcordDateSetting : InfoPageBase
    {
        #region Properties

        /// <summary>
        /// 日期设置工作页
        /// </summary>
        public IWorkpage WorkPage { get; set; }

        public DateSetting DateTimeSetting { get; set; }

        #endregion

        public ContractConcordDateSetting()
        {
            InitializeComponent();

            DataContext = this;
            DateTimeSetting = new DateSetting();
            caStartDate.Value = DateTime.Now;
            caEndDate.Value = DateTime.Now;
        }

        #region Events


        /// <summary>
        /// 公示开始日期
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void caStartDateBox_Click(object sender, RoutedEventArgs e)
        {
            caStartDate.IsEnabled = (bool)caStartDateBox.IsChecked;
        }

        /// <summary>
        /// 公示结束日期
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void caEndDateBox_Click(object sender, RoutedEventArgs e)
        {
            caEndDate.IsEnabled = (bool)caEndDateBox.IsChecked;
        }


        /// 确定
        /// </summary>
        private void mbtnDateOK_Click(object sender, RoutedEventArgs e)
        {

            if ((bool)caEndDateBox.IsChecked && (bool)caStartDateBox.IsChecked
                && caEndDate.Value < caStartDate.Value)
            {
                ShowBox("提示", "审核日期不能小于申请日期!");
                return;
            }

            if (DateTimeSetting != null)
            {
                DateTimeSetting.PublishStartDate = caStartDate.Value;
                DateTimeSetting.PublishEndDate = caEndDate.Value;
            }
            Close(true);
        }

        /// <summary>
        /// 消息提示框
        /// </summary>
        private void ShowBox(string title, string msg, eMessageGrade type = eMessageGrade.Error, Action<bool?, eCloseReason> action = null)
        {
            WorkPage.Page.ShowMessageBox(new TabMessageBoxDialog()
            {
                Header = title,
                Message = msg,
                MessageGrade = type,
                CancelButtonText = "取消"
            }, action);
        }

        #endregion
    }
}
