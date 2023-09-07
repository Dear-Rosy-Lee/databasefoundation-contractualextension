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
    [View(typeof(OptionsEditorNavigatorPath))]
    public partial class OptionsEditorNavigatorPathViewModel : ViewModelObject, IDisposable
    {
        #region Properties

        #region Properties - Common

        private string _Description = "查看和编辑文件夹路径";
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
        private double _TitleWidth = 150;

        public OptionsEditorNavigatorPathConfiguration Config
        {
            get { return _Config; }
            set { _Config = value; NotifyPropertyChanged(() => Config); }
        }
        private OptionsEditorNavigatorPathConfiguration _Config = new OptionsEditorNavigatorPathConfiguration();


        #endregion

        #region Properties - System

        public IWorkpage Workpage { get; private set; }

        #endregion

        #endregion

        #region Commands

        #endregion

        #region Ctor

        public OptionsEditorNavigatorPathViewModel(IWorkpage workpage)
        {
            Workpage = workpage;
        }

        #endregion

        #region Methods

        #region Methods - Public

        internal void Load()
        {
            var center = Workpage.Workspace.GetUserSettingsProfileCenter();
            var profile = center.GetProfile<OptionsEditorNavigatorPathConfiguration>();
            var section = profile.GetSection<OptionsEditorNavigatorPathConfiguration>();
            Config = section.Settings.Clone() as OptionsEditorNavigatorPathConfiguration;
        }

        internal void Save()
        {
            var center = Workpage.Workspace.GetUserSettingsProfileCenter();
            var profile = center.GetProfile<OptionsEditorNavigatorPathConfiguration>();
            var section = profile.GetSection<OptionsEditorNavigatorPathConfiguration>();
            section.Settings = Config;
            center.Save<OptionsEditorNavigatorPathConfiguration>();
        }

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
            Workpage = null;
        }

        #endregion

        #endregion
    }
}
