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
    [ViewModel(typeof(PersonExtendsBackstageDialogViewModel))]
    public partial class PersonExtendsBackstageDialog : BackstageDialog, ICustomValidate
    {
        #region Properties

        #endregion

        #region Ctor

        public PersonExtendsBackstageDialog()
        {
            InitializeComponent();
        }

        #endregion

        #region Methods

        #region Methods - Public

        public void CustomBindingExpressions()
        {
        }

        public bool CustomValidate()
        {
            return tabControl.Validate();
        }

        #endregion

        #region Methods - Events

        private void pgExtends_InitializeEnd(object sender, EventArgs e)
        {
            bgExtends.BindingExpressions.Clear();
            var list = pgExtends.ExtractBindingExpressions();
            list.ForEach(c => bgExtends.BindingExpressions.Add(c));
        }

        #endregion

        #endregion

        private void pgExtends_InitializeProperty(object sender, InitializePropertyDescriptorEventArgs e)
        {

        }
    }
}
