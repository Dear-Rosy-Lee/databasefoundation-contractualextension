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
using YuLinTu.Library.Controls;
using YuLinTu.Windows;
using System.Windows.Forms;

namespace YuLinTu.Component.Common
{
    /// <summary>
    /// 选择文件夹文本框界面
    /// </summary>
    public partial class SelectedFolderTextBox : MetroTextBox
    {
        #region Ctor

        /// <summary>
        /// 构造函数
        /// </summary>
        public SelectedFolderTextBox()
        {
            InitializeComponent();
            this.DataContext = this;
        }

        #endregion

        #region Properties

        /// <summary>
        /// 依赖属性文件夹路径
        /// </summary>
        public string FolderName
        {
            get { return (string)GetValue(FolderNameProperty); }
            set { SetValue(FolderNameProperty, value); }
        }

        public static readonly DependencyProperty FolderNameProperty = DependencyProperty.Register("FolderName", typeof(string), typeof(SelectedFolderTextBox), new PropertyMetadata((s, a) =>
        {
            var folderName = a.NewValue as string;
            if (!folderName.IsNullOrBlank())
                (s as MetroTextBox).Text = folderName;
        }));

        /// <summary>
        /// 工作空间属性
        /// </summary>
        public IWorkpage WorkPage { get; set; }

        #endregion

        #region Event

        /// <summary>
        /// 文件选择按钮
        /// </summary>
        private void ImageButton_Click(object sender, RoutedEventArgs e)
        {
            using (FolderBrowserDialog fbd = new FolderBrowserDialog())
            {
                fbd.Description = "请选择文件夹";
                if (fbd.ShowDialog() == DialogResult.OK)
                {
                    FolderName = fbd.SelectedPath;
                }
            }
        }

        #endregion

    }
}
