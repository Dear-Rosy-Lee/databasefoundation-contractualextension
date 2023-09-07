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

namespace YuLinTu.Appwork.Apps.Samples
{
    [ViewModel(typeof(ToolbarPanelViewModel))]
    public partial class ToolbarPanel : UserControl
    {
        #region Ctor

        public ToolbarPanel()
        {
            InitializeComponent();
        }

        #endregion
    }
}
