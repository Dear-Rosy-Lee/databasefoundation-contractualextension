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
    [View(typeof(DialogPersonDialogPage))]
    public partial class DialogPersonDialogPageViewModel : ViewModelObject, IDisposable
    {
        #region Properties

        #region Properties - Common

        private string _Description = "DialogPersonDialogPage Content";
        public string Description
        {
            get { return _Description; }
            set { _Description = value; NotifyPropertyChanged("Description"); }
        }

        public object SelectedItem
        {
            get { return _SelectedItem; }
            set { _SelectedItem = value; NotifyPropertyChanged(() => SelectedItem); }
        }
        private object _SelectedItem;

        public System.Collections.ObjectModel.ObservableCollection<FormValidationPerson> Items
        {
            get { return _Items; }
            set { _Items = value; NotifyPropertyChanged(() => Items); }
        }
        private System.Collections.ObjectModel.ObservableCollection<FormValidationPerson> _Items = new System.Collections.ObjectModel.ObservableCollection<FormValidationPerson>();

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
            var vm = new PersonDialogViewModel(Workpage.Page);
            Workpage.Page.ShowDialog(vm.CreateView(), (s, a) =>
            {
                if (!(s.HasValue && s.Value))
                    return;

                Items.Add(vm.Person);
                SelectedItem = vm.Person;
            });
        }

        #endregion

        #region Commands - ShowDialog2

        public DelegateCommand CommandShowDialog2 { get { return _CommandShowDialog2 ?? (_CommandShowDialog2 = new DelegateCommand(args => OnShowDialog2(args), args => OnCanShowDialog2(args))); } }
        private DelegateCommand _CommandShowDialog2;

        private bool OnCanShowDialog2(object args)
        {
            return true;
        }

        private void OnShowDialog2(object args)
        {
            var vm = new TabPersonDialogViewModel(Workpage.Page);
            Workpage.Page.ShowDialog(vm.CreateView(), (s, a) =>
            {
                if (!(s.HasValue && s.Value))
                    return;

                Items.Add(vm.Person);
                SelectedItem = vm.Person;
            });
        }

        #endregion

        #region Commands - ShowDialog3

        public DelegateCommand CommandShowDialog3 { get { return _CommandShowDialog3 ?? (_CommandShowDialog3 = new DelegateCommand(args => OnShowDialog3(args), args => OnCanShowDialog3(args))); } }
        private DelegateCommand _CommandShowDialog3;

        private bool OnCanShowDialog3(object args)
        {
            return true;
        }

        private void OnShowDialog3(object args)
        {
            var vm = new CustomPersonDialogViewModel(Workpage.Page);
            vm.Startup();
            Workpage.Page.ShowDialog(vm.CreateView(), (s, r) =>
            {
                vm.Shutdown();

                if (!(s.HasValue && s.Value))
                    return;

                Items.Add(vm.Person);
                SelectedItem = vm.Person;
            });
        }

        #endregion

        #region Commands - ShowDialog4

        public DelegateCommand CommandShowDialog4 { get { return _CommandShowDialog4 ?? (_CommandShowDialog4 = new DelegateCommand(args => OnShowDialog4(args), args => OnCanShowDialog4(args))); } }
        private DelegateCommand _CommandShowDialog4;

        private bool OnCanShowDialog4(object args)
        {
            return true;
        }

        private void OnShowDialog4(object args)
        {
            var vm = new PersonExtendsDialogViewModel(Workpage.Page);
            vm.Startup();
            Workpage.Page.ShowDialog(vm.CreateView(), (s, r) =>
            {
                vm.Shutdown();

                if (!(s.HasValue && s.Value))
                    return;

                Items.Add(vm.Person);
                SelectedItem = vm.Person;
            });
        }

        #endregion

        #region Commands - ShowDialog5

        public DelegateCommand CommandShowDialog5 { get { return _CommandShowDialog5 ?? (_CommandShowDialog5 = new DelegateCommand(args => OnShowDialog5(args), args => OnCanShowDialog5(args))); } }
        private DelegateCommand _CommandShowDialog5;

        private bool OnCanShowDialog5(object args)
        {
            return true;
        }

        private void OnShowDialog5(object args)
        {
            var vm = new PersonExtendsBackstageDialogViewModel(Workpage.Page);
            vm.Startup();
            Workpage.Page.ShowDialog(vm.CreateView(), (s, r) =>
            {
                vm.Shutdown();

                if (!(s.HasValue && s.Value))
                    return;

                Items.Add(vm.Person);
                SelectedItem = vm.Person;
            });
        }

        #endregion


        #endregion

        #region Ctor

        public DialogPersonDialogPageViewModel(ITheWorkpage workpage) : base(workpage.Message)
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
