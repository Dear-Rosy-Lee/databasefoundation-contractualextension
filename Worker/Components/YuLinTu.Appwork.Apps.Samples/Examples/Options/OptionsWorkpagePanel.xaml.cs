using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Timers;
using System.Windows;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using YuLinTu.Appwork;
using YuLinTu.Windows;
using YuLinTu.Windows.Wpf;
using YuLinTu.Windows.Wpf.Metro;
using YuLinTu.Windows.Wpf.Metro.Components;

namespace YuLinTu.Appwork.Apps.Samples.Examples
{
    [Newable(false,
        Order = 10000,
        IsLanguageName = false,
        Name = "OptionsWorkpagePanel",
        Description = "OptionsWorkpagePanel Description#Feature 1#Feature 2#Feature 3#Feature 4#Feature 5",
        Category = "Page Category",
        Icon = "pack://application:,,,/YuLinTu.Resources;component/Images/16/Document.png",
        Image = "pack://application:,,,/YuLinTu.Resources;component/Images/78/Information78.png",
        IsNeedAuthenticated = false)]
    [ViewModel(typeof(OptionsWorkpagePanelViewModel))]
    [Example(Order = 0, IsNew = true, Name = "Workpage Options", Catalog = "Options",
        Image = "pack://application:,,,/YuLinTu.Resources;component/Images/16/ui-check-boxes-series.png", Files = new string[] {
        "OptionsWorkpagePanel.xaml.Message.cs",
        "OptionsEditorWorkpagePerson.xaml",
        "OptionsEditorWorkpagePerson.xaml.cs",
        "OptionsEditorWorkpagePerson.xaml.ViewModel.cs",
        "FormValidationPerson.cs",
        "eGender.cs",
        "IcnValidationAttribute.cs",
        "IcnHelper.cs"})]
    public partial class OptionsWorkpagePanel : YuLinTu.Appwork.Page, INavigatableExtend
    {
        #region Properties

        public eNavigatorType NavigatorType { get { return eNavigatorType.None; } }
        public string NavigatorTitle { get { return null; } }

        #endregion

        #region Fields

        #endregion

        #region Ctor

        public OptionsWorkpagePanel()
        {
            InitializeComponent();
        }

        #endregion

        #region Methods

        #region Methods - Public

        #endregion

        #region Methods - Override

        protected override void OnWorkpageChanged()
        {
            base.OnWorkpageChanged();

            var m = ViewModelObject.ResolveModel<OptionsWorkpagePanel>(Workpage);
            m.Startup();
            DataContext = m;
        }

        public override void Dispose()
        {
            base.Dispose();

            var m = DataContext as ViewModelObject;
            if (m != null)
                m.Shutdown();

            var d = DataContext as IDisposable;
            if (d != null)
                d.Dispose();

            DataContext = null;
        }

        #endregion

        #region Methods - Events

        #endregion

        #region Methods - Private

        #endregion

        #endregion
    }
}

