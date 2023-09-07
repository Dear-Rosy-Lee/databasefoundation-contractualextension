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

namespace YuLinTu.Appwork.Apps.Samples.Examples.Dialog
{
    [ViewModel(typeof(PersonOptionsPanelViewModel))]
    public partial class PersonOptionsPanel : UserControl
    {
        #region Ctor

        public PersonOptionsPanel()
        {
            InitializeComponent();
        }

        #endregion
    }
}
