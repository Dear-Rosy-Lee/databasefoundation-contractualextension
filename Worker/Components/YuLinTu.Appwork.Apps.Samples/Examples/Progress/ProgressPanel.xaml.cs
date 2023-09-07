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

namespace YuLinTu.Appwork.Apps.Samples.Examples.Progress
{
    [ViewModel(typeof(ProgressPanelViewModel))]
    [Example(Order = 0, Name = "Progress", Catalog = @"Progress",
        Image = "pack://application:,,,/YuLinTu.Resources;component/images/16/ui-progress-bar.png")]
    public partial class ProgressPanel : UserControl
    {
        #region Ctor

        public ProgressPanel()
        {
            InitializeComponent();
        }

        #endregion
    }
}
