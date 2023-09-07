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
    [View(typeof(SimpleDialog))]
    public partial class SimpleDialogViewModel : ViewModelObject,
        IMetroDialogLifetimeHandlerInitialize,
        IMetroDialogLifetimeHandlerConfirm,
        IMetroDialogLifetimeHandlerCancel
    {
        #region Properties

        #region Properties - Common

        private string _Description = "SimpleDialog Content";
        public string Description
        {
            get { return _Description; }
            set { _Description = value; NotifyPropertyChanged("Description"); }
        }

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

        #region Fields

        private DialogSimpleDialogPageViewModel model = null;

        #endregion

        #region Ctor

        public SimpleDialogViewModel(IDialogOwner owner, DialogSimpleDialogPageViewModel model)
        {
            DialogOwner = owner;
            this.model = model;
        }

        #endregion

        #region Methods

        #region Methods - Lifetime

        public void OnInitializeGo()
        {
            if (System.Windows.Application.Current.Dispatcher.CheckAccess())
                model.AppendDescription($"Dialog Initialize Go on main thread.");
            else
                model.AppendDescription($"Dialog Initialize Go on child thread.");

            System.Threading.Thread.Sleep(1000);
        }

        public void OnInitializeCompleted()
        {
            if (System.Windows.Application.Current.Dispatcher.CheckAccess())
                model.AppendDescription($"Dialog Initialize Completed on main thread.");
            else
                model.AppendDescription($"Dialog Initialize Completed on child thread.");
        }

        public void OnInitializeTerminated(Exception ex)
        {
            if (System.Windows.Application.Current.Dispatcher.CheckAccess())
                model.AppendDescription($"Dialog Initialize Terminated on main thread.");
            else
                model.AppendDescription($"Dialog Initialize Terminated on child thread.");
        }

        public void OnInitializeStarted()
        {
            if (System.Windows.Application.Current.Dispatcher.CheckAccess())
                model.AppendDescription($"Dialog Initialize Started on main thread.");
            else
                model.AppendDescription($"Dialog Initialize Started on child thread.");
        }

        public void OnInitializeEnded()
        {
            if (System.Windows.Application.Current.Dispatcher.CheckAccess())
                model.AppendDescription($"Dialog Initialize Ended on main thread.");
            else
                model.AppendDescription($"Dialog Initialize Ended on child thread.");
        }

        public bool OnConfirmGo()
        {
            if (System.Windows.Application.Current.Dispatcher.CheckAccess())
                model.AppendDescription($"Dialog Confirm Go on main thread.");
            else
                model.AppendDescription($"Dialog Confirm Go on child thread.");

            System.Threading.Thread.Sleep(1000);
            return true;
        }

        public void OnConfirmCompleted()
        {
            if (System.Windows.Application.Current.Dispatcher.CheckAccess())
                model.AppendDescription($"Dialog Confirm Completed on main thread.");
            else
                model.AppendDescription($"Dialog Confirm Completed on child thread.");
        }

        public bool OnConfirmTerminated(Exception ex)
        {
            if (System.Windows.Application.Current.Dispatcher.CheckAccess())
                model.AppendDescription($"Dialog Confirm Terminated on main thread.");
            else
                model.AppendDescription($"Dialog Confirm Terminated on child thread.");
            return false;
        }

        public void OnConfirmStarted()
        {
            if (System.Windows.Application.Current.Dispatcher.CheckAccess())
                model.AppendDescription($"Dialog Confirm Started on main thread.");
            else
                model.AppendDescription($"Dialog Confirm Started on child thread.");
        }

        public void OnConfirmEnded()
        {
            if (System.Windows.Application.Current.Dispatcher.CheckAccess())
                model.AppendDescription($"Dialog Confirm Ended on main thread.");
            else
                model.AppendDescription($"Dialog Confirm Ended on child thread.");
        }

        public bool OnCancelGo()
        {
            if (System.Windows.Application.Current.Dispatcher.CheckAccess())
                model.AppendDescription($"Dialog Cancel Go on main thread.");
            else
                model.AppendDescription($"Dialog Cancel Go on child thread.");
            return true;
        }

        public void OnCancelCompleted()
        {
            if (System.Windows.Application.Current.Dispatcher.CheckAccess())
                model.AppendDescription($"Dialog Cancel Completed on main thread.");
            else
                model.AppendDescription($"Dialog Cancel Completed on child thread.");
        }

        public bool OnCancelTerminated(Exception ex)
        {
            if (System.Windows.Application.Current.Dispatcher.CheckAccess())
                model.AppendDescription($"Dialog Cancel Terminated on main thread.");
            else
                model.AppendDescription($"Dialog Cancel Terminated on child thread.");
            return false;
        }

        public void OnCancelStarted()
        {
            if (System.Windows.Application.Current.Dispatcher.CheckAccess())
                model.AppendDescription($"Dialog Cancel Started on main thread.");
            else
                model.AppendDescription($"Dialog Cancel Started on child thread.");
        }

        public void OnCancelEnded()
        {
            if (System.Windows.Application.Current.Dispatcher.CheckAccess())
                model.AppendDescription($"Dialog Cancel Ended on main thread.");
            else
                model.AppendDescription($"Dialog Cancel Ended on child thread.");
        }

        #endregion

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
            // 一般不使用该方法处理对话框的加载过程，请使用
            // OnInitialize 系列方法初始化界面。
        }

        #endregion

        #region Methods - System

        #endregion

        #endregion
    }
}
