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
    [View(typeof(CustomPersonDialog))]
    public partial class CustomPersonDialogViewModel : ViewModelObject,
        IMetroDialogLifetimeHandlerInitialize,
        IMetroDialogLifetimeHandlerConfirm,
        IMetroDialogLifetimeHandlerCancel
    {
        #region Properties

        #region Properties - Common

        private string _Description = "CustomPersonDialog Content";
        public string Description
        {
            get { return _Description; }
            set { _Description = value; NotifyPropertyChanged("Description"); }
        }

        public FormValidationPerson Person
        {
            get { return _Person; }
            set { _Person = value; NotifyPropertyChanged(() => Person); }
        }
        private FormValidationPerson _Person = new FormValidationPerson();

        public PersonOptionsPanelViewModel PersonOptionsPanelViewModel
        {
            get { return _PersonOptionsPanelViewModel; }
            set { _PersonOptionsPanelViewModel = value; NotifyPropertyChanged(() => PersonOptionsPanelViewModel); }
        }
        private PersonOptionsPanelViewModel _PersonOptionsPanelViewModel;

        #endregion

        #region Properties - System

        public IDialogOwner DialogOwner { get; private set; }

        #endregion

        #endregion

        #region Commands

        #region Commands - Loaded

        public DelegateCommand CommandLoaded { get { return _CommandLoaded ?? (_CommandLoaded = new DelegateCommand(obj => OnLoaded(obj))); } }
        private DelegateCommand _CommandLoaded;

        #endregion

        #endregion

        #region Ctor

        public CustomPersonDialogViewModel(IDialogOwner owner)
        {
            DialogOwner = owner;
        }

        #endregion

        #region Methods

        #region Methods - Lifetime

        public void OnInitializeGo()
        {
        }

        public void OnInitializeCompleted()
        {
        }

        public void OnInitializeTerminated(Exception ex)
        {
        }

        public void OnInitializeStarted()
        {
        }

        public void OnInitializeEnded()
        {
        }

        public bool OnConfirmGo()
        {
            return true;
        }

        public void OnConfirmCompleted()
        {
        }

        public bool OnConfirmTerminated(Exception ex)
        {
            return false;
        }

        public void OnConfirmStarted()
        {
        }

        public void OnConfirmEnded()
        {
        }

        public bool OnCancelGo()
        {
            return true;
        }

        public void OnCancelCompleted()
        {
        }

        public bool OnCancelTerminated(Exception ex)
        {
            return false;
        }

        public void OnCancelStarted()
        {
        }

        public void OnCancelEnded()
        {
        }

        #endregion

        #region Methods - Public

        #endregion

        #region Methods - Override

        protected override void OnStartup()
        {
            base.OnStartup();
            PersonOptionsPanelViewModel = new PersonOptionsPanelViewModel(Person);
            PersonOptionsPanelViewModel.Startup();
        }

        protected override void OnShutdown()
        {
            PersonOptionsPanelViewModel.Shutdown();
            base.OnShutdown();
        }

        #endregion

        #region Methods - Private

        private void OnLoaded(object obj)
        {
            // 一般不使用该方法处理对话框的加载过程，请使用
            // OnInitialize 系列方法初始化界面。
        }

        #endregion

        #region Methods - System

        #endregion

        #endregion
    }
}
