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
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using YuLinTu.Library.Business;
using YuLinTu.Windows;

namespace YuLinTu.Library.Controls
{
    /// <summary>
    /// 数据导入页面
    /// </summary>
    public partial class ImportDataPage : InfoPageBase
    {
        #region Field

        private string filter;

        #endregion

        #region Property

        /// <summary>
        /// 导入文件名称
        /// </summary>
        public string FileName { get; private set; }

        /// <summary>
        /// 导入类型
        /// </summary>
        public eImportTypes ImportType { get; private set; }

        #endregion

        #region Ctor

        public ImportDataPage(IWorkpage page, string header, string desc = "", string filter = "")
        {
            InitializeComponent();

            DataContext = this;
            this.Header = header;
            this.Workpage = page;
            this.filter = filter;
        }

        #endregion

        #region Methods

        /// <summary>
        /// 选择文件
        /// </summary>
        private void btnFileSelect_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog ofd = new Microsoft.Win32.OpenFileDialog();
            ofd.Multiselect = false;
            ofd.Filter = string.IsNullOrEmpty(filter) ? "文件类型(*.xls,*.xlsx)|*.xls;*.xlsx" : filter;
            var val = ofd.ShowDialog();
            if (val == null || !val.Value)
            {
                return;
            }
            txt_file.Text = ofd.FileName;
        }

        /// <summary>
        /// Changed事件
        /// </summary>
        private void txt_file_TextChanged_1(object sender, TextChangedEventArgs e)
        {
            string name = txt_file.Text.Trim();
            if (string.IsNullOrEmpty(name) || (!string.IsNullOrEmpty(name) && !System.IO.File.Exists(name)))
            {
                FileName = string.Empty;
                btnExcuteImport.IsEnabled = false;
            }
            else
            {
                FileName = name;
                btnExcuteImport.IsEnabled = true;
            }
        }

        /// <summary>
        /// 执行导入
        /// </summary>
        private void btnExcuteImport_Click_1(object sender, RoutedEventArgs e)
        {
            ComboBoxItem cbi = cbtype.SelectedItem as ComboBoxItem;
            string typestr = cbi.Tag.ToString();
            if (typestr == eImportTypes.Clear.ToString())
            {
                ImportType = eImportTypes.Clear;
            }
            if (typestr == eImportTypes.Over.ToString())
            {
                ImportType = eImportTypes.Over;
            }
            if (typestr == eImportTypes.Ignore.ToString())
            {
                ImportType = eImportTypes.Ignore;
            }
            Workpage.Page.CloseMessageBox(true);
        }

        #endregion
    }
}
