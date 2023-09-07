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
    [ViewModel(typeof(FontButtonPanelViewModel))]
    [Example(Order = 2, Name = "Font Button", Catalog = @"Buttons\Button",
        Image = "pack://application:,,,/YuLinTu.Resources;component/Images/16/FontColorPicker.png")]
    public partial class FontButtonPanel : UserControl
    {
        #region Ctor

        public FontButtonPanel()
        {
            InitializeComponent();
        }

        #endregion
    }
}
