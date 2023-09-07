using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using YuLinTu;
using YuLinTu.Data;
using YuLinTu.Windows;
using YuLinTu.Windows.Wpf;
using YuLinTu.Windows.Wpf.Metro;
using YuLinTu.Windows.Wpf.Metro.Components;

namespace YuLinTu.Appwork.Apps.Samples.Examples.Dialog
{
    [ViewModel(typeof(CustomPersonDialogViewModel))]
    public partial class CustomPersonDialog : MetroDialog, ICustomValidate
    {
        #region Properties

        #endregion

        #region Ctor

        public CustomPersonDialog()
        {
            InitializeComponent();
        }

        #endregion

        #region Methods

        public void CustomBindingExpressions()
        {
            var exps = pp.BindingGroup.BindingExpressions.ToList();
            pp.BindingGroup.BindingExpressions.Clear();
            exps.ForEach(c => bg.BindingExpressions.Add(c));
        }

        public bool CustomValidate()
        {
            return tabControl.Validate();
        }

        #endregion
    }
}
