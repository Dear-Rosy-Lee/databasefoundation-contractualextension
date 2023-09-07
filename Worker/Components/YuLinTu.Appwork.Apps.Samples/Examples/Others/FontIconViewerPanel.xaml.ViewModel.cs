using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Markup;
using System.Windows.Media;
using YuLinTu;
using YuLinTu.Appwork;
using YuLinTu.Data;
using YuLinTu.Windows;
using YuLinTu.Windows.Wpf;
using YuLinTu.Windows.Wpf.Metro;
using YuLinTu.Windows.Wpf.Metro.Components;

namespace YuLinTu.Appwork.Apps.Samples.Examples
{
    [View(typeof(FontIconViewerPanel))]
    public partial class FontIconViewerPanelViewModel : ViewModelObject
    {
        #region Properties

        #region Properties - Common

        public FontIconViewerConfiguration Config
        {
            get { return _Config; }
            set { _Config = value; NotifyPropertyChanged(() => Config); }
        }
        private FontIconViewerConfiguration _Config = new FontIconViewerConfiguration();


        #endregion

        #region Properties - System

        #endregion

        #endregion

        #region Commands

        #endregion

        #region Ctor

        public FontIconViewerPanelViewModel()
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
