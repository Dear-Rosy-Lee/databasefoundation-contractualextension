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
    [ViewModel(typeof(FormPanelViewModel))]
    [Example(Order = 0, Name = "Horizontal Form", Catalog = "Input",
        Image = "pack://application:,,,/YuLinTu.Resources;component/Images/16/application-form.png", Files = new string[] {
        "FormValidationPerson.cs",
        "eGender.cs",
        "IcnValidationAttribute.cs",
        "IcnHelper.cs"})]
    public partial class FormPanel : UserControl
    {
        #region Ctor

        public FormPanel()
        {
            InitializeComponent();
        }

        #endregion
    }
}
