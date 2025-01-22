using Microsoft.Win32;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using YuLinTu.Windows.Wpf.Metro.Components;

namespace YuLinTu.Component.CoordinateTransformTask
{
    /// <summary>
    /// UserControl1.xaml 的交互逻辑
    /// </summary>
    public partial class SplitFileBrowserTextBox : MetroTextBox
    {
        #region Properties

        public string Filter { get; set; } = "矢量文件(*.shp)|*.shp";
        public bool Multiselect { get; set; } = true;

        /// <summary>
        /// 绑定参数
        /// </summary>
        public List<string> SelectdFiles
        {
            get { return (List<string>)GetValue(SelectdFilesProperty); }
            set { SetValue(SelectdFilesProperty, value); }
        }

        public static readonly DependencyProperty SelectdFilesProperty =
            DependencyProperty.Register("SelectdFiles", typeof(List<string>), typeof(SplitFileBrowserTextBox));

        #endregion

        #region Events

        #endregion

        #region Ctor

        public SplitFileBrowserTextBox()
        {
            InitializeComponent();
            VerticalContentAlignment = System.Windows.VerticalAlignment.Center;
        }

        #endregion

        #region Methods

        #region Methods - Events

        private void ImageButton_Click_1(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.FileName = Text.Trim();
            ofd.Filter = Filter;
            ofd.Multiselect = Multiselect;
            if (SelectdFiles != null && SelectdFiles.Count > 0)
            {
                ofd.InitialDirectory = System.IO.Path.GetDirectoryName(SelectdFiles[0]);
            }
            var b = ofd.ShowDialog();
            if (b == null || !b.Value)
                return;

            SetFiles(ofd.FileNames.ToList());

            this.Focus();
        }

        public void SetFiles(List<string> fileList)
        {
            if (fileList.Count > 20)
            {
                MessageBox.Show("最多添加20个文件，对多余的文件将忽略！", "请注意", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            if (fileList.Count > 1)
            {
                string files = string.Empty;
                foreach (var item in fileList)
                {
                    files += $"\"{System.IO.Path.GetFileName(item)}\";";
                }
                Text = files.Substring(0, files.Length - 1);
            }
            else
            {
                Text = fileList[0];
            }

            if (fileList.Count > 20)
            {
                if (SelectdFiles == null)
                {
                    SelectdFiles = new List<string>();
                }
                for (int i = 0; i < 20; i++)
                {
                    SelectdFiles.Add(fileList[i]);
                }
            }
            else
            {
                this.SelectdFiles = fileList;
            }
        }
        #endregion

        #endregion
    }
}
