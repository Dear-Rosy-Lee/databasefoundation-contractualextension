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
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
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
