using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
    [View(typeof(OptionsEditorAuthentication))]
    public partial class OptionsEditorAuthenticationViewModel : ViewModelObject, IDisposable
    {
        #region Properties

        #region Properties - Common


        private string _Description = "鉴权信息";
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
        private double _TitleWidth = 50;


        public VectorDataLinkWorkpageConfig PageConfig
        {
            get { return _PageConfig; }
            set { _PageConfig = value; NotifyPropertyChanged(() => PageConfig); }
        }
        private VectorDataLinkWorkpageConfig _PageConfig = new VectorDataLinkWorkpageConfig();


        #endregion

        #region Properties - System

        //public ITheWorkpage Workpage { get; private set; }
        public IWorkspace Workspace { get; private set; }
        #endregion

        #endregion

        #region Commands

        #region Commands - Loaded

        public DelegateCommand CommandLoaded { get { return _CommandLoaded ?? (_CommandLoaded = new DelegateCommand(obj => OnLoaded(obj))); } }
        private DelegateCommand _CommandLoaded;

        #endregion

        #endregion

        #region Ctor

       

        public OptionsEditorAuthenticationViewModel(IWorkspace workspace)
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

        private void OnLoaded(object obj)
        {
            // 一般不使用该方法处理页面的加载过程，请使用 Message 文件中的
            // InstallWorkpageContent 初始化界面组件，
            // InstallAccountData 初始化用户数据。
        }

        #endregion

        #region Methods - Public

        //internal void Load()
        //{
        //    var center = Workpage.Workspace.GetUserSettingsProfileCenter();
        //    var profile = center.GetProfile<VectorDataLinkWorkpageConfig>();
        //    var section = profile.GetSection<VectorDataLinkWorkpageConfig>();
        //    PageConfig = section.Settings.Clone() as VectorDataLinkWorkpageConfig;
        //}

        //internal void Save()
        //{
        //    var center = Workpage.Workspace.GetUserSettingsProfileCenter();
        //    var profile = center.GetProfile<VectorDataLinkWorkpageConfig>();
        //    var section = profile.GetSection<VectorDataLinkWorkpageConfig>();
        //    section.Settings = PageConfig;
        //    center.Save<VectorDataLinkWorkpageConfig>();
        //}

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
