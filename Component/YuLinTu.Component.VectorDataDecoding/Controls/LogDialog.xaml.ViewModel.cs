using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;
using YuLinTu;
using YuLinTu.Appwork;
using YuLinTu.Component.VectorDataDecoding.Core;
using YuLinTu.Component.VectorDataDecoding.JsonEntity;
using YuLinTu.Data;
using YuLinTu.Windows;
using YuLinTu.Windows.Wpf;
using YuLinTu.Windows.Wpf.Metro;
using YuLinTu.Windows.Wpf.Metro.Components;

namespace YuLinTu.Component.VectorDataDecoding.Controls
{
    [View(typeof(LogDialog))]
    public partial class LogDialogViewModel : ViewModelObject,
        IMetroDialogLifetimeHandlerInitialize,
        IMetroDialogLifetimeHandlerConfirm,
        IMetroDialogLifetimeHandlerCancel
    {
        #region Properties

        #region Properties - Common

        public ObservableCollection<LogEn> Items
        {
            get { return _Items; }
            set { _Items = value; NotifyPropertyChanged(() => Items); }
        }
        private ObservableCollection<LogEn> _Items = new ObservableCollection<LogEn>();
        public LogEn SelectedItem
        {
            get { return _SelectedItem; }
            set { _SelectedItem = value; NotifyPropertyChanged(() => SelectedItem); }
        }
        private LogEn _SelectedItem;

        public string BatchCode { get; set; }



        #endregion

        #region Properties - System

        public IDialogOwner DialogOwner { get; private set; }

        public bool IsBusy
        {
            get { return IsBusy; }
            set { _IsBusy = value; NotifyPropertyChanged(() => IsBusy); }
        }
        private bool _IsBusy=true;

        #endregion

        #endregion

        #region Commands

        #region Commands - Loaded

        public DelegateCommand CommandLoaded { get { return _CommandLoaded ?? (_CommandLoaded = new DelegateCommand(obj => OnLoaded(obj))); } }



        private DelegateCommand _CommandLoaded;

        #endregion

        #endregion

        #region Ctor

        public LogDialogViewModel(IDialogOwner owner)
        {
           

            DialogOwner = owner;
          
        }

        #endregion

        #region Methods

        #region Methods - Lifetime

        public void OnInitializeGo()
        {
            System.Threading.Thread.Sleep(1000);
        }

        public void OnInitializeCompleted()
        {
            Items =new VectorService().QueryLogsByBatchCode(BatchCode);
        
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
            System.Threading.Thread.Sleep(1000);
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
