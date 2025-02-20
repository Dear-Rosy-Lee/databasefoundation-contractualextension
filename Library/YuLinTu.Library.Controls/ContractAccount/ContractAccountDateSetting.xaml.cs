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
using YuLinTu.Windows;
using YuLinTu.Windows.Wpf.Metro.Components;
using YuLinTu.Library.Business;

namespace YuLinTu.Library.Controls
{
    /// <summary>
    /// 公示表日期配置
    /// </summary>
    public partial class ContractAccountDateSetting : InfoPageBase
    {

        #region Properties

        /// <summary>
        /// 日期设置工作页
        /// </summary>
        //public IWorkpage WorkPage { get; set; }

        public DateSetting DateTimeSetting { get; set; }

        #endregion

        public ContractAccountDateSetting()
        {
            InitializeComponent();
            DataContext = this;
            DateTimeSetting = new DateSetting();
        }


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

        /// <summary>
        /// 制表日期
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void caCreateTableDateBox_Click(object sender, RoutedEventArgs e)
        {
            caCreateTableDate.IsEnabled = (bool)caCreateTableDateBox.IsChecked;
        }

        /// <summary>
        /// 审核日期
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void caCheckTableDateBox_Click(object sender, RoutedEventArgs e)
        {
            caCheckTableDate.IsEnabled = (bool)caCheckTableDateBox.IsChecked;
        }

        /// <summary>
        /// 确定
        /// </summary>
        private void mbtnDateOK_Click(object sender, RoutedEventArgs e)
        {

            if ((bool)caEndDateBox.IsChecked && (bool)caStartDateBox.IsChecked
                && caEndDate.Value != null && caStartDate.Value != null && caEndDate.Value < caStartDate.Value)
            {
                ShowBox("提示", "公示结束日期不能小于公示开始日期!");
                return;
            }
            if ((bool)caCreateTableDateBox.IsChecked && (bool)caCheckTableDateBox.IsChecked
                && caCheckTableDate.Value != null && caCreateTableDate.Value != null && caCheckTableDate.Value < caCreateTableDate.Value)
            {
                ShowBox("提示", "审核日期不能小于制表日期!");
                return;
            }
            if ((bool)caEndDateBox.IsChecked && (bool)caCreateTableDateBox.IsChecked
                && caEndDate.Value != null && caCreateTableDate.Value != null && caEndDate.Value < caCreateTableDate.Value)
            {
                ShowBox("提示", "公示结束日期不能小于制表日期!");
                return;
            }
            if ((bool)caStartDateBox.IsChecked && (bool)caCreateTableDateBox.IsChecked
               && caStartDate.Value != null && caCreateTableDate.Value != null && caStartDate.Value < caCreateTableDate.Value)
            {
                ShowBox("提示", "公示开始日期不能小于制表日期!");
                return;
            }
            if ((bool)caEndDateBox.IsChecked && (bool)caCheckTableDateBox.IsChecked
              && caEndDate.Value != null && caCheckTableDate.Value != null && caEndDate.Value < caCheckTableDate.Value)
            {
                ShowBox("提示", "公示结束日期不能小于审核日期!");
                return;
            }
            if ((bool)caStartDateBox.IsChecked && (bool)caCheckTableDateBox.IsChecked
               && caStartDate.Value != null && caCheckTableDate.Value != null && caStartDate.Value < caCheckTableDate.Value)
            {
                ShowBox("提示", "公示开始日期不能小于审核日期!");
                return;
            }
            if (DateTimeSetting != null)
            {
                DateTimeSetting.CheckTablePerson = caCheckPerson.Text != null ? caCheckPerson.Text.ToString() : "";
                DateTimeSetting.CreateTablePerson = caCreatePerson.Text != null ? caCreatePerson.Text.ToString() : "";
                DateTimeSetting.CheckTableDate = caCheckTableDate.Value;
                DateTimeSetting.CreateTableDate = caCreateTableDate.Value;
                DateTimeSetting.PublishStartDate = caStartDate.Value;
                DateTimeSetting.PublishEndDate = caEndDate.Value;
            }
            Close(true);
        }

        /// <summary>
        /// 消息提示框
        /// </summary>
        private void ShowBox(string title, string msg, eMessageGrade type = eMessageGrade.Error)
        {
            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                Workpage.Page.ShowMessageBox(new TabMessageBoxDialog()
                {
                    Header = title,
                    Message = msg,
                    MessageGrade = type,
                    CancelButtonText = "取消",
                });
            }));

        }
    }
}
