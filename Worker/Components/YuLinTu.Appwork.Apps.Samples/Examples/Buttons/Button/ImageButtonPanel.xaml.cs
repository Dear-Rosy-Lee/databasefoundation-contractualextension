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
using YuLinTu.Windows.Wpf;

namespace YuLinTu.Appwork.Apps.Samples.Examples
{
    [ViewModel(typeof(ImageButtonPanelViewModel))]
    [Example(Order = 1, Name = "Image Button", Catalog = @"Buttons\Button",
        Image = "pack://application:,,,/YuLinTu.Resources;component/Images/16/image.png")]
    public partial class ImageButtonPanel : UserControl
    {
        #region Ctor

        public ImageButtonPanel()
        {
            InitializeComponent();
        }

        #endregion
    }
}
