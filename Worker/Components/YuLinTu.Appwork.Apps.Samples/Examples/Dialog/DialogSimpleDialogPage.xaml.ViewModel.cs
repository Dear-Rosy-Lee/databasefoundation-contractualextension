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
    [View(typeof(DialogSimpleDialogPage))]
    public partial class DialogSimpleDialogPageViewModel : ViewModelObject, IDisposable
    {
        #region Properties

        #region Properties - Common

        private string _Description;
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
            var vm = new SimpleDialogViewModel(Workpage.Page, this);
            Workpage.Page.ShowDialog(vm.CreateView(), (b, r) =>
            {
                AppendDescription($"Dialog Closed Callback. Result is {b}, Reason is {r}.");
            });
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
            var vm = new SimpleDialogViewModel(Workpage.Page, this);
            Workpage.Page.Show(vm.CreateView(), (b, r) =>
            {
                AppendDescription($"Dialog Closed Callback. Result is {b}, Reason is {r}.");
            });
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
            var dlg = new MessageDialog()
            {
                Header = "Message Dialog",
                Message = "This is a test message.",
                MessageGrade = eMessageGrade.Warn,
            };

            dlg.ConfirmStart += (s, a) =>
            {
                if (dlg.Dispatcher.CheckAccess())
                    AppendDescription($"Message Dialog Confirm Start on main thread.");
                else
                    AppendDescription($"Message Dialog Confirm Start on child thread.");
            };
            dlg.ConfirmEnd += (s, a) =>
            {
                if (dlg.Dispatcher.CheckAccess())
                    AppendDescription($"Message Dialog Confirm End on main thread.");
                else
                    AppendDescription($"Message Dialog Confirm End on child thread.");
            };
            dlg.Confirm += (s, a) =>
            {
                if (dlg.Dispatcher.CheckAccess())
                    AppendDescription($"Message Dialog Confirm Go on main thread.");
                else
                    AppendDescription($"Message Dialog Confirm Go on child thread.");
            };
            dlg.ConfirmCompleted += (s, a) =>
            {
                if (dlg.Dispatcher.CheckAccess())
                    AppendDescription($"Message Dialog Confirm Completed on main thread.");
                else
                    AppendDescription($"Message Dialog Confirm Completed on child thread.");
            };
            dlg.ConfirmTerminated += (s, a) =>
            {
                if (dlg.Dispatcher.CheckAccess())
                    AppendDescription($"Message Dialog Confirm Terminated on main thread.");
                else
                    AppendDescription($"Message Dialog Confirm Terminated on child thread.");
            };

            dlg.CancelStart += (s, a) =>
            {
                if (dlg.Dispatcher.CheckAccess())
                    AppendDescription($"Message Dialog Cancel Start on main thread.");
                else
                    AppendDescription($"Message Dialog Cancel Start on child thread.");
            };
            dlg.CancelEnd += (s, a) =>
            {
                if (dlg.Dispatcher.CheckAccess())
                    AppendDescription($"Message Dialog Cancel End on main thread.");
                else
                    AppendDescription($"Message Dialog Cancel End on child thread.");
            };
            dlg.Cancel += (s, a) =>
            {
                if (dlg.Dispatcher.CheckAccess())
                    AppendDescription($"Message Dialog Cancel Go on main thread.");
                else
                    AppendDescription($"Message Dialog Cancel Go on child thread.");
            };
            dlg.CancelCompleted += (s, a) =>
            {
                if (dlg.Dispatcher.CheckAccess())
                    AppendDescription($"Message Dialog Cancel Completed on main thread.");
                else
                    AppendDescription($"Message Dialog Cancel Completed on child thread.");
            };
            dlg.CancelTerminated += (s, a) =>
            {
                if (dlg.Dispatcher.CheckAccess())
                    AppendDescription($"Message Dialog Cancel Terminated on main thread.");
                else
                    AppendDescription($"Message Dialog Cancel Terminated on child thread.");
            };

            dlg.ShowDialog(Workpage.Page, (b, r) =>
            {
                Description += $"Message Dialog Closed Callback. Result is {b}, Reason is {r}.\r\n";
            });
        }

        #endregion

        #region Commands - Clear

        public DelegateCommand CommandClear { get { return _CommandClear ?? (_CommandClear = new DelegateCommand(args => OnClear(args), args => OnCanClear(args))); } }
        private DelegateCommand _CommandClear;

        private bool OnCanClear(object args)
        {
            return true;
        }

        private void OnClear(object args)
        {
            Description = null;
        }

        #endregion


        #endregion

        #region Ctor

        public DialogSimpleDialogPageViewModel(ITheWorkpage workpage) : base(workpage.Message)
        {
            Workpage = workpage;
        }

        #endregion

        #region Methods

        #region Methods - Public

        public void AppendDescription(string txt)
        {
            Description += $"{txt}\r\n";
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
