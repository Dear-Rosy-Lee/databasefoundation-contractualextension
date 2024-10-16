/*
 * (C) 2024  鱼鳞图公司版权所有,保留所有权利 
 */
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using YuLinTu.Windows;

namespace YuLinTu.Library.Controls
{
    /// <summary>
    /// 导出数据选择界面
    /// </summary>
    public partial class ExportDataPage : InfoPageBase
    {
        #region Filds

        private string description = "请选择保存文件的目录";

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
        public ExportDataPage(string Name, IWorkpage page, string header, string desc = "", string btnName = "",
            string lbPathName = "")
        {
            InitializeComponent();
            DataContext = this;
            zoneName.Text = Name;
            Workpage = page;
            this.Header = header;
            description = string.IsNullOrEmpty(desc) ? description : desc;
            if (!string.IsNullOrEmpty(btnName))
                btnCommit.Text = btnName;
            if (!string.IsNullOrEmpty(lbPathName))
                lbPath.Content = lbPathName;
            btnExcuteImport.IsEnabled = false;
        }

        ///// <summary>
        ///// 执行
        ///// </summary>
        //private void btnExcuteImport_Click(object sender, RoutedEventArgs e)
        //{
        //    Workpage.Page.CloseMessageBox(true);
        //}

        /// <summary>
        /// 文件浏览
        /// </summary>
        private void FileBrowser_Click(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            fbd.Description = description; ;
            if (fbd.ShowDialog() == DialogResult.OK)
            {
                txt_file.Text = fbd.SelectedPath;
            }
        }

        /// <summary>
        /// 文件路径选择
        /// </summary>
        private void txt_file_TextChanged_1(object sender, TextChangedEventArgs e)
        {
            if (txt_file.Text.Trim() != "")
            {
                FileName = txt_file.Text;
                btnExcuteImport.IsEnabled = true;
            }
            else
            {
                btnExcuteImport.IsEnabled = false;
            }
        }

        #endregion
    }
}
