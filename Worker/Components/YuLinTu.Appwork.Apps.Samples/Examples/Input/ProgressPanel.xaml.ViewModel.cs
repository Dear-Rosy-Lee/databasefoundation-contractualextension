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

namespace YuLinTu.Appwork.Apps.Samples.Examples.Progress
{
    [View(typeof(ProgressPanel))]
    public partial class ProgressPanelViewModel : ViewModelObject
    {
        #region Properties

        #region Properties - Common

        private string _Description = "ProgressPanel Content";
        public string Description
        {
            get { return _Description; }
            set { _Description = value; NotifyPropertyChanged("Description"); }
        }

        #endregion

        #region Properties - System

        #endregion

        #endregion

        #region Commands

        #endregion

        #region Ctor

        public ProgressPanelViewModel()
        {
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

        #endregion

        #endregion
    }
}
