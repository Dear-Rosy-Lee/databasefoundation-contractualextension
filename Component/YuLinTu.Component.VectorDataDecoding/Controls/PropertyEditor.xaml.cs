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
using YuLinTu.Windows.Wpf;
using YuLinTu.Windows.Wpf.Metro.Components;

namespace YuLinTu.Component.VectorDataDecoding
{
    public partial class PropertyEditorCom : TabMessageBox
    {
        #region Properties

        public bool? IsBusy
        {
            get { return _IsBusy; }
            set { _IsBusy = value; NotifyPropertyChanged("IsBusy"); }
        }

        #endregion

        #region Fields

        private bool? _IsBusy = false;

        #endregion

        #region Ctor

        public PropertyEditorCom()
        {
            InitializeComponent();

            DataContext = this;
            pg.InitializeBegin += pg_InitializeBegin;
            pg.InitializeEnd += pg_InitializeEnd;

            Loaded += PropertyEditor_Loaded;
        }

        #endregion

        #region Methods

        private void pg_InitializeEnd(object sender, EventArgs e)
        {
            IsBusy = false;
        }

        private void pg_InitializeBegin(object sender, EventArgs e)
        {
            IsBusy = true;
        }

        private void PropertyEditor_Loaded(object sender, RoutedEventArgs e)
        {
            DetentionReporter reporter = null;
            reporter = DetentionReporterDispatcher.Create(this.Dispatcher, c =>
            {
                MaxHeight = ActualHeight;
                MinHeight = ActualHeight;
                reporter.Dispose();
            }, 300, 300, false);

            reporter.Start();
        }

        #endregion
    }
}
