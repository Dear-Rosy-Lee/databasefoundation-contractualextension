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
using YuLinTu;
using YuLinTu.Appwork;
using YuLinTu.Data;
using YuLinTu.Windows;
using YuLinTu.Windows.Wpf;
using YuLinTu.Windows.Wpf.Metro;
using YuLinTu.Windows.Wpf.Metro.Components;

namespace YuLinTu.Appwork.Apps.Samples.Examples
{
    [ViewModel(typeof(OptionsEditorWorkspaceGeneralViewModel))]
    public partial class OptionsEditorWorkspaceGeneral : OptionsEditor
    {
        #region Ctor

        public OptionsEditorWorkspaceGeneral(IWorkspace workspace)
            : base(workspace)
        {
            InitializeComponent();
        }

        #endregion

        #region Methods

        protected override void OnInstall()
        {
            Dispatcher.Invoke(new Action(() =>
            {
                var m = ViewModelObject.ResolveModel<OptionsEditorWorkspaceGeneral>(Workspace);
                m.Startup();
                DataContext = m;
            }));
        }

        protected override void OnUninstall()
        {
            Dispatcher.Invoke(new Action(() =>
            {
                var m = DataContext as ViewModelObject;
                if (m != null)
                    m.Shutdown();

                var d = DataContext as IDisposable;
                if (d != null)
                    d.Dispose();

                DataContext = null;
            }));
        }

        protected override void OnLoad()
        {
            Dispatcher.Invoke(new Action(() =>
            {
            }));
        }

        protected override void OnSave()
        {
            Dispatcher.Invoke(new Action(() =>
            {
            }));
        }

        protected override void OnShown()
        {
            Dispatcher.Invoke(new Action(() =>
            {
            }));
        }

        #endregion
    }
}
