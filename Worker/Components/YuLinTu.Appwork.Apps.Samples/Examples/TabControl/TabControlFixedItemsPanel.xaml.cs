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
    [ViewModel(typeof(TabControlFixedItemsPanelViewModel))]
    [Example(Order = 0, IsNew = true, Name = "Simple TabControl", Catalog = "TabControl",
        Image = "pack://application:,,,/YuLinTu.Resources;component/Images/16/ui-tab-content.png", Files = new string[] {
        "TabControlConfiguration.cs"})]
    public partial class TabControlFixedItemsPanel : UserControl
    {
        #region Ctor

        public TabControlFixedItemsPanel()
        {
            InitializeComponent();
        }

        #endregion
    }
}
