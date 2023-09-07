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
    [ViewModel(typeof(ButtonSuperButtonDefaultPanelViewModel))]
    [Example(Order = 2, Name = "Default Super Button", Catalog = @"Buttons\Super Button",
        Image = "pack://application:,,,/YuLinTu.Resources;component/Images/16/ui-button.png")]
    public partial class ButtonSuperButtonDefaultPanel : UserControl
    {
        #region Ctor

        public ButtonSuperButtonDefaultPanel()
        {
            InitializeComponent();
        }

        #endregion
    }
}
