/*
 * (C) 2025  鱼鳞图公司版权所有,保留所有权利 
 */
using System.Windows;
using YuLinTu.Windows;
using YuLinTu.Windows.Wpf.Metro.Components;

namespace YuLinTu.Component.Common
{
    /// <summary>
    /// 选择文件文本框界面
    /// </summary>
    public partial class SelectedFileTextBox : MetroTextBox
    {
        public string Filter { get; set; }

        #region Ctor

        /// <summary>
        /// 构造函数
        /// </summary>
        public SelectedFileTextBox()
        {
            InitializeComponent();
            this.DataContext = this;
            Filter = "文件类型(*.sqlite)|*.sqlite";
        }

        #endregion

        #region Properties

        /// <summary>
        /// 依赖属性文件路径
        /// </summary>
        public string FileName
        {
            get { return (string)GetValue(FileNameProperty); }
            set { SetValue(FileNameProperty, value); }
        }

        public static readonly DependencyProperty FileNameProperty = DependencyProperty.Register("FileName", typeof(string), typeof(SelectedFileTextBox), new PropertyMetadata((s, a) =>
        {
            var fileName = a.NewValue as string;
            if (!fileName.IsNullOrBlank())
                (s as MetroTextBox).Text = fileName;
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
            Microsoft.Win32.OpenFileDialog ofd = new Microsoft.Win32.OpenFileDialog();
            ofd.Multiselect = false;
            ofd.Filter = Filter;
            ofd.Multiselect = true;
            var val = ofd.ShowDialog();
            if (val == null || !val.Value)
            {
                return;
            }
            string filename = string.Empty;
            foreach (var item in ofd.FileNames)
            {
                filename += item + ";";
            }
            FileName = filename.TrimEnd(';');
        }

        #endregion
    }
}
