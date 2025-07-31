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
using YuLinTu.Component.VectorDataLinkageTask.Core;
using YuLinTu.Data;
using YuLinTu.Windows;
using YuLinTu.Windows.Wpf;
using YuLinTu.Windows.Wpf.Metro;
using YuLinTu.Windows.Wpf.Metro.Components;

namespace YuLinTu.Component.VectorDataLinkageTask
{
    [Newable(true,
        Order = 10000,
        IsLanguageName = false,
        Name = "OptionsEditorAuthentication",
        Description = "OptionsEditorAuthentication Description#Feature 1#Feature 2#Feature 3#Feature 4#Feature 5",
        Category = "Page Category",
        Icon = "pack://application:,,,/YuLinTu.Resources;component/Images/16/Document.png",
        Image = "pack://application:,,,/YuLinTu.Resources;component/Images/78/Information78.png",
        IsNeedAuthenticated = false)]
    [ViewModel(typeof(OptionsEditorAuthenticationViewModel))]
    public partial class OptionsEditorAuthentication :OptionsEditor
    {
        #region Properties

        #endregion

        #region Fields

        #endregion

        #region Ctor

       

       
        public OptionsEditorAuthentication(IWorkspace Workspace)
          : base(Workspace)
        {
            InitializeComponent();
        }
        #endregion


        #region Methods

        protected override void OnInstall()
        {
            Dispatcher.Invoke(new Action(() =>
            {
                var m = ViewModelObject.ResolveModel<OptionsEditorAuthentication>(Workspace);
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
            var center = TheApp.Current.GetSystemSettingsProfileCenter();
            var profile = center.GetProfile<VectorDataLinkWorkpageConfig>();
            var section = profile.GetSection<VectorDataLinkWorkpageConfig>();

            Dispatcher.Invoke(new Action(() =>
            {
                (DataContext as OptionsEditorAuthenticationViewModel).PageConfig =
                    section.Settings.Clone() as VectorDataLinkWorkpageConfig;
            }));
        }

        protected override void OnSave()
        {
            var center = TheApp.Current.GetSystemSettingsProfileCenter();
            var profile = center.GetProfile<VectorDataLinkWorkpageConfig>();
            var section = profile.GetSection<VectorDataLinkWorkpageConfig>();

            Dispatcher.Invoke(new Action(() =>
            {
                section.Settings = (DataContext as OptionsEditorAuthenticationViewModel).PageConfig;
                center.Save<VectorDataLinkWorkpageConfig>();
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
