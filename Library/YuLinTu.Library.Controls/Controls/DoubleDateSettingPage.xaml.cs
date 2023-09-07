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
using YuLinTu.Windows.Wpf.Metro.Components;

namespace YuLinTu.Library.Controls
{
    /// <summary>
    /// 两个日期设置界面
    /// </summary>
    public partial class DoubleDateSettingPage : InfoPageBase
    {
        #region Properties

        /// <summary>
        /// 设置声明日期
        /// </summary>
        public DateTime? SetDecTime { get; set; }

        /// <summary>
        /// 设置公示日期
        /// </summary>
        public DateTime? SetPubTime { get; set; }

        #endregion

        #region Ctor

        /// <summary>
        /// 构造函数
        /// </summary>
        public DoubleDateSettingPage()
        {
            InitializeComponent();
            txt_BirthdayPub.Value = DateTime.Now;
            txt_BirthdayDec.Value = DateTime.Now;
        }

        #endregion

        #region Event

        /// <summary>
        /// 执行确定按钮
        /// </summary>
        private void btnExcuteImport_Click(object sender, RoutedEventArgs e)
        {
            if (txt_BirthdayDec.Value < txt_BirthdayPub.Value)
            {
                ShowBox(VirtualPersonInfo.ExportIdeaBook, VirtualPersonInfo.ExportIdeaBookDateSettingError);
                return;
            }
            SetDecTime = txt_BirthdayDec.Value;
            SetPubTime = txt_BirthdayPub.Value;
            Workpage.Page.CloseMessageBox(true);
        }

        /// <summary>
        /// 消息提示框
        /// </summary>
        private void ShowBox(string title, string msg, eMessageGrade type = eMessageGrade.Error)
        {
            Workpage.Page.ShowMessageBox(new TabMessageBoxDialog()
            {
                Header = title,
                Message = msg,
                MessageGrade = type,
                CancelButtonText = "取消",
            });
        }

        #endregion
    }
}
