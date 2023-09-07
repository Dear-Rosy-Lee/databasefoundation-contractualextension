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
    [ViewModel(typeof(NumericPanelViewModel))]
    [Example(Order = 13, Name = "Numeric", Catalog = @"Input",
        Image = "pack://application:,,,/YuLinTu.Resources;component/images/office/2013/16/webcontrollistbox.png")]
    public partial class NumericPanel : UserControl
    {
        #region Ctor

        public NumericPanel()
        {
            InitializeComponent();
        }

        #endregion
    }
}
