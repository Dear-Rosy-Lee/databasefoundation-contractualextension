/*
 * (C) 2015  鱼鳞图公司版权所有,保留所有权利 
 */
using System.Windows;
using YuLinTu.Windows;

namespace YuLinTu.Library.Controls
{
    /// <summary>
    /// 导出数据选择界面
    /// </summary>
    public partial class ConfirmInfoPage : InfoPageBase
    {
        #region Filds

        #endregion

        #region Property 

        #endregion

        #region Methods

        /// <summary>
        /// 构造方法
        /// </summary>
        public ConfirmInfoPage(IWorkpage page, string header, string message = "")
        {
            InitializeComponent();
            DataContext = this;
            Workpage = page;
            this.Header = header;
            txtMessage.Text = message;
        }
        /// <summary>
        /// 取消
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MetroButton_Click(object sender, RoutedEventArgs e)
        {
            Workpage.Page.CloseMessageBox(false, eCloseReason.Confirm);
        }

        /// <summary>
        /// 确定
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnExcuteImport_Click(object sender, RoutedEventArgs e)
        {
            Workpage.Page.CloseMessageBox(true);
        }
        #endregion


    }
}
