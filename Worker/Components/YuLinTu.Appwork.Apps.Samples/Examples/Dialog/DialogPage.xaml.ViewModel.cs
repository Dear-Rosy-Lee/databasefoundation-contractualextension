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

namespace YuLinTu.Appwork.Apps.Samples.Examples.Dialog
{
    [View(typeof(DialogPage))]
    public partial class DialogPageViewModel : ViewModelObject, IDisposable
    {
        #region Properties

        #region Properties - Common

        private string _Description = "DialogPage Content";
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
            // 这里使用 ViewModel 来实例化对话框
            var vm = new BlankDialogViewModel(Workpage.Page);
            Workpage.Page.ShowDialog(vm.CreateView());
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
            // 这里使用 ViewModel 来实例化对话框
            var vm = new BlankDialogViewModel(Workpage.Page);
            Workpage.Page.Show(vm.CreateView());
        }

        #endregion

        #region Commands - ShowBackstageDialog

        public DelegateCommand CommandShowBackstageDialog { get { return _CommandShowBackstageDialog ?? (_CommandShowBackstageDialog = new DelegateCommand(args => OnShowBackstageDialog(args), args => OnCanShowBackstageDialog(args))); } }
        private DelegateCommand _CommandShowBackstageDialog;

        private bool OnCanShowBackstageDialog(object args)
        {
            return true;
        }

        private void OnShowBackstageDialog(object args)
        {
            var vm = new BlankBackstageDialogViewModel(Workpage.Page);
            Workpage.Page.ShowDialog(vm.CreateView());
        }

        #endregion


        #region Commands - ShowSelfAdaptionDialog

        public DelegateCommand CommandShowSelfAdaptionDialog { get { return _CommandShowSelfAdaptionDialog ?? (_CommandShowSelfAdaptionDialog = new DelegateCommand(args => OnShowSelfAdaptionDialog(args), args => OnCanShowSelfAdaptionDialog(args))); } }
        private DelegateCommand _CommandShowSelfAdaptionDialog;

        private bool OnCanShowSelfAdaptionDialog(object args)
        {
            return true;
        }

        private void OnShowSelfAdaptionDialog(object args)
        {
            var vm = new AutoScaleDialogViewModel(Workpage.Page);
            Workpage.Page.ShowDialog(vm.CreateView());
        }

        #endregion

        #region Commands - ShowDialogWorkspace

        public DelegateCommand CommandShowDialogWorkspace { get { return _CommandShowDialogWorkspace ?? (_CommandShowDialogWorkspace = new DelegateCommand(args => OnShowDialogWorkspace(args), args => OnCanShowDialogWorkspace(args))); } }
        private DelegateCommand _CommandShowDialogWorkspace;

        private bool OnCanShowDialogWorkspace(object args)
        {
            return true;
        }

        private void OnShowDialogWorkspace(object args)
        {
            var vm = new BlankDialogViewModel(Workpage.Page);
            Workpage.Workspace.Window.ShowDialog(vm.CreateView());
        }

        #endregion

        #region Commands - ShowModelessDialogWorkspace

        public DelegateCommand CommandShowModelessDialogWorkspace { get { return _CommandShowModelessDialogWorkspace ?? (_CommandShowModelessDialogWorkspace = new DelegateCommand(args => OnShowModelessDialogWorkspace(args), args => OnCanShowModelessDialogWorkspace(args))); } }
        private DelegateCommand _CommandShowModelessDialogWorkspace;

        private bool OnCanShowModelessDialogWorkspace(object args)
        {
            return true;
        }

        private void OnShowModelessDialogWorkspace(object args)
        {
            var vm = new BlankDialogViewModel(Workpage.Page);
            Workpage.Workspace.Window.Show(vm.CreateView());
        }

        #endregion



        #region Commands - ShowMessageDialog

        public DelegateCommand CommandShowMessageDialog { get { return _CommandShowMessageDialog ?? (_CommandShowMessageDialog = new DelegateCommand(args => OnShowMessageDialog(args), args => OnCanShowMessageDialog(args))); } }
        private DelegateCommand _CommandShowMessageDialog;

        private bool OnCanShowMessageDialog(object args)
        {
            return true;
        }

        private void OnShowMessageDialog(object args)
        {
            new MessageDialog()
            {
                Header = "Message Dialog",
                Message = "This is a test message.",
                MessageGrade = eMessageGrade.Warn,

            }.ShowDialog(Workpage.Page);
        }

        #endregion

        #region Commands - ShowMessageDialog2

        public DelegateCommand CommandShowMessageDialog2 { get { return _CommandShowMessageDialog2 ?? (_CommandShowMessageDialog2 = new DelegateCommand(args => OnShowMessageDialog2(args), args => OnCanShowMessageDialog2(args))); } }
        private DelegateCommand _CommandShowMessageDialog2;

        private bool OnCanShowMessageDialog2(object args)
        {
            return true;
        }

        private void OnShowMessageDialog2(object args)
        {
            new MessageDialog()
            {
                Header = "Message Dialog",
                Message = "This is a test message.",
                MessageGrade = eMessageGrade.Infomation,

                ConfirmButtonVisibility = System.Windows.Visibility.Collapsed,
                CancelButtonText = "关闭",

            }.ShowDialog(Workpage.Page);
        }

        #endregion

        #endregion

        #region Ctor

        public DialogPageViewModel(ITheWorkpage workpage) : base(workpage.Message)
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
