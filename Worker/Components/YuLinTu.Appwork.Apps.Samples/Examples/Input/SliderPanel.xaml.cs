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
    [ViewModel(typeof(SliderPanelViewModel))]
    [Example(Order = 14, Name = "Slider", Catalog = @"Input",
        Image = "pack://application:,,,/YuLinTu.Resources;component/images/16/ui-slider-050.png")]
    public partial class SliderPanel : UserControl
    {
        #region Ctor

        public SliderPanel()
        {
            InitializeComponent();
        }

        #endregion
    }
}
