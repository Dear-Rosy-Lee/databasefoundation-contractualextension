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
using Microsoft.Win32;
using YuLinTu.Windows.Wpf.Metro.Components;

namespace YuLinTu.Component.MapFoundation
{
    public partial class SaveFileBrowserTextBox : MetroTextBox
    {
        #region Properties

        public string Filter { get; set; }

        #endregion

        #region Events

        public event EventHandler<MsgEventArgs<SaveFileDialog>> FilesSelected;

        #endregion

        #region Ctor

        public SaveFileBrowserTextBox()
        {
            InitializeComponent();
            VerticalContentAlignment = System.Windows.VerticalAlignment.Center;
        }

        #endregion

        #region Methods

        #region Methods - Events

        private void ImageButton_Click_1(object sender, RoutedEventArgs e)
        {
            SaveFileDialog ofd = new SaveFileDialog();
            ofd.FileName = Text.Trim();
            ofd.Filter = Filter;
            //if (ofd.FileName.IsNullOrBlank())
            //    ofd.FileName = Environment.CurrentDirectory;

            var b = ofd.ShowDialog();
            if (b == null || !b.Value)
                return;

            Text = ofd.FileName;

            if (FilesSelected != null)
                FilesSelected(this, new MsgEventArgs<SaveFileDialog>() { Parameter = ofd });

            this.Focus();
        }

        #endregion

        #endregion
    }
}
