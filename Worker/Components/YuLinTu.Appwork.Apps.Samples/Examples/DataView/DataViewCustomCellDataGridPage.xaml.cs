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

namespace YuLinTu.Appwork.Apps.Samples.Examples.DataView
{
    [Newable(false,
        Order = 10000,
        IsLanguageName = false,
        Name = "DataViewCustomCellDataGridPage",
        Description = "DataViewCustomCellDataGridPage Description#Feature 1#Feature 2#Feature 3#Feature 4#Feature 5",
        Category = "Page Category",
        Icon = "pack://application:,,,/YuLinTu.Resources;component/Images/16/Document.png",
        Image = "pack://application:,,,/YuLinTu.Resources;component/Images/78/Information78.png",
        IsNeedAuthenticated = false)]
    [ViewModel(typeof(DataViewCustomCellDataGridPageViewModel))]
    [Example(Order = 0.5, Name = "Custom Cell DataGrid", Catalog = "DataView",
        Image = "pack://application:,,,/YuLinTu.Resources;component/Images/16/table.png", Files = new string[] {
        "ScoreItem.cs",
        "EmployeeItem.cs",
        "ScoreToBackgroundConverter.cs"})]
    public partial class DataViewCustomCellDataGridPage : YuLinTu.Appwork.Page, INavigatableExtend
    {
        #region Properties

        public eNavigatorType NavigatorType { get { return eNavigatorType.None; } }
        public string NavigatorTitle { get { return null; } }

        #endregion

        #region Fields

        #endregion

        #region Ctor

        public DataViewCustomCellDataGridPage()
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

            var m = ViewModelObject.ResolveModel<DataViewCustomCellDataGridPage>(Workpage);
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
