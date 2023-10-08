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
    public partial class BatchUpdatePage : InfoPageBase
    {

        #region Properties

        /// <summary>
        /// 设置声明日期
        /// </summary>
        public DateTime? SetStartTime { get; set; }

        /// <summary>
        /// 设置公示日期
        /// </summary>
        public DateTime? SetFinishTime { get; set; }

        #endregion

        #region Ctor

        public BatchUpdatePage()
        {
            InitializeComponent();
            txt_BirthdayStart.Value = DateTime.Now;
            txt_BirthdayFinish.Value = DateTime.Now;
        }

        #endregion

        private void btnExcuteImport_Click(object sender, RoutedEventArgs e)
        {
            if (txt_BirthdayStart.Value < txt_BirthdayFinish.Value)
            {
                ShowBox("批量编辑", "结束日期不能小于开始日期!");
                return;
            }
            SetStartTime = txt_BirthdayStart.Value;
            SetFinishTime = txt_BirthdayFinish.Value;
            Workpage.Page.CloseMessageBox(true);
        }
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
    }
}
