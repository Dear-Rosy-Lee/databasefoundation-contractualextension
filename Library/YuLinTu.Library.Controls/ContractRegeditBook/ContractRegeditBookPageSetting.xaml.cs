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
    /// ContractRegeditBookPageSetting.xaml 的交互逻辑-权证打印设置-证书分页设置
    /// </summary>
    public partial class ContractRegeditBookPageSetting : InfoPageBase
    {
        #region Properties

        /// <summary>
        /// 日期设置工作页
        /// </summary>
        public IWorkpage WorkPage { get; set; }

        /// <summary>
        /// 证书共有人数设置
        /// </summary>
        public int? BookPersonNum { get; set; }

        /// <summary>
        /// 证书地块数设置
        /// </summary>
        public int? BookLandNum { get; set; }
        /// <summary>
        /// 证书编号设置
        /// </summary>
        public string BookNumSetting { get; set; }

        #endregion

        public ContractRegeditBookPageSetting()
        {
            InitializeComponent();

            DataContext = this;

            //序列化过后，如果序列化后获取的是0，默认的数字设定
            ContractRegeditBookPreviewSetInfo xmlSet = ContractRegeditBookExtend.DeserializeSelectedSetInfo();
            if (xmlSet.CRBLandCount == 0)
            {
                caBookLandNum.Value = 11;
            }
            else
            {
                caBookLandNum.Value = xmlSet.CRBLandCount;
            };

            if (xmlSet.CRBSharePersonCount == 0)
            {
                caBookPersonNum.Value = 12;
            }
            else
            {
                caBookPersonNum.Value = xmlSet.CRBSharePersonCount;
            };

            if (xmlSet.CRBBookNumerSetting.IsNullOrEmpty())
            {
                caBookNumSettingBox.Text = "NO.J";
            }
            else
            {
                caBookNumSettingBox.Text = xmlSet.CRBBookNumerSetting;
            }
        }

        /// <summary>
        /// 证书共有人数设置
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void caBookPersonNumBox_Click(object sender, RoutedEventArgs e)
        {
            caBookPersonNum.IsEnabled = (bool)caBookPersonNumBox.IsChecked;
        }

        /// <summary>
        /// 证书地块数设置
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void caBookLandNumBox_Click(object sender, RoutedEventArgs e)
        {
            caBookLandNum.IsEnabled = (bool)caBookLandNumBox.IsChecked;
        }
        /// <summary>
        /// 证书编码样式设置
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void caBookNumSetting_Click(object sender, RoutedEventArgs e)
        {
            caBookNumSettingBox.IsEnabled = (bool)caBookNumSetting.IsChecked;
        }

        ///</summary>
        /// 确定
        /// </summary>
        private void mbtnDateOK_Click(object sender, RoutedEventArgs e)
        {
            #region 代码注释-之前的业务逻辑

            //if ((bool)(caBookPersonNumBox.IsChecked) && caBookPersonNum.Value != null)
            //{
            //    BookPersonNum = caBookPersonNum.Value;
            //}
            //if ((bool)(caBookLandNumBox.IsChecked) && caBookLandNum.Value != null)
            //{
            //    BookLandNum = caBookLandNum.Value;
            //}

            #endregion

            if (caBookPersonNum.Value != null)
            {
                BookPersonNum = caBookPersonNum.Value;
            }
            if (caBookLandNum.Value != null)
            {
                BookLandNum = caBookLandNum.Value;
            }
            if (!caBookNumSettingBox.Text.IsNullOrEmpty())
            {
                BookNumSetting=caBookNumSettingBox.Text;
            }
            ContractRegeditBookPreviewSetInfo xmlSet = new ContractRegeditBookPreviewSetInfo();
            xmlSet.CRBLandCount = BookLandNum == null ? 0 : BookLandNum.Value;
            xmlSet.CRBSharePersonCount = BookPersonNum == null ? 0 : BookPersonNum.Value;
            xmlSet.CRBBookNumerSetting = BookNumSetting.IsNullOrEmpty() ? "" : BookNumSetting;
            ContractRegeditBookExtend.SerializeSelectedSetInfo(xmlSet);
            Close(true);
        }

        /// <summary>
        /// 消息提示框
        /// </summary>
        private void ShowBox(string title, string msg, eMessageGrade type = eMessageGrade.Error)
        {
            WorkPage.Page.ShowMessageBox(new TabMessageBoxDialog()
            {
                Header = title,
                Message = msg,
                MessageGrade = type,
                CancelButtonText = "取消",
            });
        }

    }
}
