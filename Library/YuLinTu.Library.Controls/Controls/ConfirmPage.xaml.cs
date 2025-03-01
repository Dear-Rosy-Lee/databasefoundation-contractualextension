/*
 * (C) 2025  鱼鳞图公司版权所有,保留所有权利 
 */
using YuLinTu.Windows;

namespace YuLinTu.Library.Controls
{
    /// <summary>
    /// 导出数据选择界面
    /// </summary>
    public partial class ConfirmPage : InfoPageBase
    {
        #region Filds

        #endregion

        #region Property

        /// <summary>
        /// 导出目录文件名称
        /// </summary>
        public string FileName { get; private set; }

        #endregion

        #region Methods

        /// <summary>
        /// 构造方法
        /// </summary>
        public ConfirmPage(IWorkpage page, string header, string message = "")
        {
            InitializeComponent();
            DataContext = this;
            Workpage = page;
            this.Header = header;
            txtMessage.Text = message;
        }

        /// <summary>
        /// 执行
        /// </summary>
        //private void btnExcuteImport_Click(object sender, RoutedEventArgs e)
        //{
        //    Workpage.Page.CloseMessageBox(true);
        //}

        #endregion
    }
}
