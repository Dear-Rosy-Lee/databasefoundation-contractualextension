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
    [ViewModel(typeof(FontIconViewerPanelViewModel))]
    [Example(Order = 2000, IsNew = true, Name = "Font Icons Viewer", Catalog = @"Others",
        Image = "pack://application:,,,/YuLinTu.Resources;component/Images/16/ChangeCase.png", Files = new string[] {
        "FontIconViewerConfiguration.cs",
        "FontCharacterMetadata.cs",
        "FontFamilyMetadata.cs",
        "XamlFormatter.cs",
        "XamlFormat.cs",
        "FontIconTemplate.xml"})]
    public partial class FontIconViewerPanel : UserControl
    {
        #region Ctor

        public FontIconViewerPanel()
        {
            InitializeComponent();
        }

        #endregion
    }
}
