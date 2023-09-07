using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YuLinTu;
using YuLinTu.Appwork;
using YuLinTu.Data;
using YuLinTu.Windows;
using YuLinTu.Windows.Wpf;
using YuLinTu.Windows.Wpf.Metro;
using YuLinTu.Windows.Wpf.Metro.Components;

namespace YuLinTu.Appwork.Apps.Samples.Examples
{
    [View(typeof(OptionsEditorWorkspaceGeneral))]
    public partial class OptionsEditorWorkspaceGeneralViewModel : ViewModelObject, IDisposable
    {
        #region Properties

        #region Properties - Common

        private string _Description = "选项卡描述";
        public string Description
        {
            get { return _Description; }
            set { _Description = value; NotifyPropertyChanged("Description"); }
        }

        public double TitleWidth
        {
            get { return _TitleWidth; }
            set { _TitleWidth = value; NotifyPropertyChanged(() => TitleWidth); }
        }
        private double _TitleWidth = 100;

        #endregion

        #region Properties - System

        public IWorkspace Workspace { get; private set; }

        #endregion

        #endregion

        #region Commands

        #endregion

        #region Ctor

        public OptionsEditorWorkspaceGeneralViewModel(IWorkspace workspace)
        {
            Workspace = workspace;
        }

        #endregion

        #region Methods

        #region Methods - Public

        #endregion

        #region Methods - Override

        protected override void OnStartup()
        {
            base.OnStartup();
        }

        protected override void OnShutdown()
        {
            base.OnShutdown();
        }

        #endregion

        #region Methods - Private

        #endregion

        #region Methods - System

        public void Dispose()
        {
            Workspace = null;
        }

        #endregion

        #endregion
    }
}
