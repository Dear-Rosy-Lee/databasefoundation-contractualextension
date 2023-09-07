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
    [ViewModel(typeof(ThemeColorViewerPanelViewModel))]
    [Example(Order = 2001, IsNew = true, Name = "Theme Color Viewer", Catalog = @"Others",
        Image = "pack://application:,,,/YuLinTu.Resources;component/Images/16/color--pencil.png", Files = new string[] {
        "ThemeColorViewerItem.cs",
        "PropertyDescriptorBuilderThemeColorEditor.cs",
        "ThemeColorSelectorComboBox.xaml",
        "ThemeColorSelectorComboBox.xaml.cs",
        "ThemeColorItem.cs",
        "XamlFormat.cs",
        "XamlFormatter.cs",
        "ThemeColorElementTemplate.xml"})]
    public partial class ThemeColorViewerPanel : UserControl
    {
        #region Ctor

        public ThemeColorViewerPanel()
        {
            InitializeComponent();
        }

        #endregion
    }
}
