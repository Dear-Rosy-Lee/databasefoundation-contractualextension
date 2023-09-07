using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using YuLinTu;
using YuLinTu.Appwork;
using YuLinTu.Data;
using YuLinTu.Windows;
using YuLinTu.Windows.Wpf;
using YuLinTu.Windows.Wpf.Metro;
using YuLinTu.Windows.Wpf.Metro.Components;

namespace YuLinTu.Appwork.Apps.Samples.Examples.Dialog
{
    [View(typeof(WindowDialogPage))]
    public partial class WindowDialogPageViewModel : ViewModelObject, IDisposable
    {
        #region Properties

        #region Properties - Common

        private string _Description = "WindowDialogPage Content";
        public string Description
        {
            get { return _Description; }
            set { _Description = value; NotifyPropertyChanged("Description"); }
        }

        #endregion

        #region Properties - System

        public ITheWorkpage Workpage { get; private set; }

        #endregion

        #endregion

        #region Commands

        #region Commands - Loaded

        public DelegateCommand CommandLoaded { get { return _CommandLoaded ?? (_CommandLoaded = new DelegateCommand(obj => OnLoaded(obj))); } }
        private DelegateCommand _CommandLoaded;

        #endregion

        #region Commands - ShowDialog

        public DelegateCommand CommandShowDialog { get { return _CommandShowDialog ?? (_CommandShowDialog = new DelegateCommand(args => OnShowDialog(args), args => OnCanShowDialog(args))); } }
        private DelegateCommand _CommandShowDialog;

        private bool OnCanShowDialog(object args)
        {
            return true;
        }

        private void OnShowDialog(object args)
        {
            var dlg = new BlankWindowDialogViewModel().CreateView() as WindowDialog;
            dlg.ShowDialog();
        }

        #endregion

        #region Commands - ShowModelessDialog

        public DelegateCommand CommandShowModelessDialog { get { return _CommandShowModelessDialog ?? (_CommandShowModelessDialog = new DelegateCommand(args => OnShowModelessDialog(args), args => OnCanShowModelessDialog(args))); } }
        private DelegateCommand _CommandShowModelessDialog;

        private bool OnCanShowModelessDialog(object args)
        {
            return true;
        }

        private void OnShowModelessDialog(object args)
        {
            var dlg = new BlankWindowDialogViewModel().CreateView() as WindowDialog;
            dlg.Show();
        }

        #endregion

        #endregion

        #region Ctor

        public WindowDialogPageViewModel(ITheWorkpage workpage) : base(workpage.Message)
        {
            Workpage = workpage;
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

        #region Methods - System

        public void Dispose()
        {
            Workpage = null;
        }

        #endregion

        #endregion
    }
}
