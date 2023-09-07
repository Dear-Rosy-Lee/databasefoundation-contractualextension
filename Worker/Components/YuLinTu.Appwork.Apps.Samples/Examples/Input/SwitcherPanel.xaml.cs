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

namespace YuLinTu.Appwork.Apps.Samples.Examples.Buttons.Switcher
{
    [ViewModel(typeof(SwitcherPanelViewModel))]
    [Example(Order = 10, Name = "Switcher", Catalog = @"Input",
        Image = "pack://application:,,,/YuLinTu.Resources;component/images/16/ui-button-toggle.png")]
    public partial class SwitcherPanel : UserControl
    {
        #region Ctor

        public SwitcherPanel()
        {
            InitializeComponent();
        }

        #endregion
    }
}
