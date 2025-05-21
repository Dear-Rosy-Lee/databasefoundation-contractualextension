using Microsoft.Win32;
using System;
using System.Windows;
using YuLinTu.Windows.Wpf.Metro.Components;

namespace YuLinTu.Component.CoordinateTransformTask.Controls
{
    /// <summary>
    /// FileSelectTestBox.xaml 的交互逻辑
    /// </summary>
    public partial class FileSelectTestBox : MetroTextBox
    {
        #region Properties

        public string Filter { get; set; }
        public bool Multiselect { get; set; }

        #endregion

        #region Events

        public event EventHandler<MsgEventArgs<OpenFileDialog>> FilesSelected;

        #endregion

        public FileSelectTestBox()
        {
            InitializeComponent();

            VerticalContentAlignment = System.Windows.VerticalAlignment.Center;
        }

        #region Methods

        #region Methods - Events

        private void ImageButton_Click_1(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.FileName = Text.Trim();
            ofd.Filter = Filter;
            ofd.Multiselect = Multiselect;
            //if (ofd.FileName.IsNullOrBlank())
            //    ofd.FileName = Environment.CurrentDirectory;

            var b = ofd.ShowDialog();
            if (b == null || !b.Value || ofd.FileNames.Length == 0)
                return;

            if (ofd.FileNames.Length > 1)
            {
                string temp = string.Empty;
                foreach (var item in ofd.FileNames)
                {
                    temp += System.IO.Path.GetFileName(item) + ";";
                }
                Text = temp;
                Tag = ofd.FileNames;
            }
            else
            {
                Text = ofd.FileName;
                Tag = ofd.FileNames;
            }

            if (FilesSelected != null)
                FilesSelected(this, new MsgEventArgs<OpenFileDialog>() { Parameter = ofd });

            this.Focus();
        }

        #endregion

        #endregion

    }
}
