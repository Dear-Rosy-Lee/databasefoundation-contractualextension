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

namespace YuLinTu.Appwork.Apps.Samples.Examples.Input
{
    [ViewModel(typeof(DateTimePickerPanelViewModel))]
    [Example(Order = 15, Name = "DateTime", Catalog = @"Input",
        Image = "pack://application:,,,/YuLinTu.Resources;component/images/16/clock.png")]
    public partial class DateTimePickerPanel : UserControl
    {
        #region Ctor

        public DateTimePickerPanel()
        {
            InitializeComponent();
        }

        #endregion
    }
}
