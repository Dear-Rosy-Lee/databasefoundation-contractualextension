using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Documents;
using System.Windows.Forms;
using YuLinTu;
using YuLinTu.Appwork;
using YuLinTu.Appwork.Task;
using YuLinTu.Component.VectorDataDecoding.Task;
using YuLinTu.Data;
using YuLinTu.DF;
using YuLinTu.DF.Data;
using YuLinTu.DF.Enums;
using YuLinTu.DF.Tasks;
using YuLinTu.DF.Zones;
using YuLinTu.Windows;
using YuLinTu.Windows.Wpf;
using YuLinTu.Windows.Wpf.Metro;
using System.Windows;
using YuLinTu.Windows.Wpf.Metro.Components;
using YuLinTu.DF.Controls.Messages;

namespace YuLinTu.Component.VectorDataDecoding
{
    [View(typeof(VectorDataDecodePage))]
    public partial class VectorDataDecodeViewModel : ViewModelObject, IDisposable
    {
  

        #region Properties - Common

        public bool IsRefreshing
        {
            get { return _IsRefreshing; }
            set { _IsRefreshing = value; NotifyPropertyChanged(() => IsRefreshing); }
        }
        private bool _IsRefreshing;

        public object SelectedItem
        {
            get { return _SelectedItem; }
            set { _SelectedItem = value; NotifyPropertyChanged(() => SelectedItem); }
        }
        private object _SelectedItem;

        public VectorDecodeMode SelectedItemChild
        {
            get { return _SelectedItemChild; }
            set { _SelectedItemChild = value; NotifyPropertyChanged(() => _SelectedItemChild); }
        }
        private VectorDecodeMode _SelectedItemChild;

        public string FilterKey
        {
            get { return _FilterKey; }
            set { _FilterKey = value; NotifyPropertyChanged(() => FilterKey); }
        }
        private string _FilterKey;

      
        public ObservableCollection<VectorDecodeBatchModel> Items
        {
            get { return _Items; }
            set { _Items = value; NotifyPropertyChanged(() => Items); }
        }
        private ObservableCollection<VectorDecodeBatchModel> _Items = new ObservableCollection<VectorDecodeBatchModel>();




        protected IDbContext DbContext { get => Db.GetInstance(); }

        private string _Description = "VectorDataDecode Content";
        public string Description
        {
            get { return _Description; }
            set { _Description = value; NotifyPropertyChanged("Description"); }
        }

        public object NavigateObject
        {
            get { return _NavigateObject; }
            set { _NavigateObject = value; NotifyPropertyChanged(() => NavigateObject); }
        }
        private object _NavigateObject;

        #endregion

        #region Properties - System

        #region Fileds
        public ITheWorkpage Workpage { get; private set; }
        /// <summary>
        /// 当前行政地域
        /// </summary>
        public IZone CurrentZone { get; set; } 
        #endregion

        #endregion

        protected readonly TaskQueue tq = new TaskQueueDispatcher();

        #region Commands - Refresh

        public DelegateCommand CommandRefresh { get { return _CommandRefresh ?? (_CommandRefresh = new DelegateCommand(args => OnRefresh(args), args => OnCanRefresh(args))); } }
        private DelegateCommand _CommandRefresh;

        private bool OnCanRefresh(object args)
        {
            return true;
        }

        [MessageHandler(ID = IdMsg.Refresh)]
        private void OnRefresh(object args)
        {
            Items.Clear();
           

            Workpage.Page.IsBusy = true;
            IsRefreshing = true;

            //为了提示加载进度，获取到工作页的实例，这种写法有违背 MVVM 的嫌疑，
            //但这里为了方便，提示进度的代码允许这样写，其余功能不允许这样写。
            var page = Workpage.Page.Content as Page;
            page.RaiseProgressBegin();

            tq.Do(go =>
            {
                System.Windows.Application.Current.Dispatcher.Invoke(() =>
                {
                    var temp = new VectorDecodeBatchModel
                    {
                        BatchCode = "5101001012025060401",
                        ZoneCode = "510100101001",
                        UplaodTime = DateTime.Now.ToLongDateString(),
                        DataCount = 2030,
                        DecodeProgress = "已上传",
                        DecodeStaus = "否",
                        NumbersOfDownloads = 0,

                    };
                    temp.Children = DbContext.CreateQuery<VectorDecodeMode>().Where(t => t.BatchCode.Equals(temp.BatchCode)).ToObservableCollection<VectorDecodeMode>();
                    var list = new ObservableCollection<VectorDecodeBatchModel>();
                    list.Add(temp);
                    //var list = GetEmployee(page, NavigateObject);
                    go.Instance.Argument.UserState = list;
                });
                

            }, completed =>
            {
                var list = completed.Result as ObservableCollection<VectorDecodeBatchModel>;
                Items = list;

              //  EmployeeCount = list.Count;

            }, error =>
            {
                Workpage.Page.ShowDialog(new YuLinTu.Windows.Wpf.Metro.Components.MessageDialog()
                {
                    Header = "错误",
                    Message = string.Format("加载失败，错误详细信息为 {0}", error.Exception),
                    MessageGrade = eMessageGrade.Error
                });

            }, ended: ended =>
            {
                page.RaiseProgressEnd();
                IsRefreshing = false;
                Workpage.Page.IsBusy = false;
            });
        }



        #endregion


        #region Commands - Loaded

        public DelegateCommand CommandLoaded { get { return _CommandLoaded ?? (_CommandLoaded = new DelegateCommand(obj => OnLoaded(obj))); } }
        private DelegateCommand _CommandLoaded;

        #endregion

   


   
     
        


        #region Ctor

        public VectorDataDecodeViewModel(ITheWorkpage workpage) : base(workpage.Message)
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
            OnRefresh(obj);
        }

        #endregion

        #region Commands - StartSingleTask
        
        public DelegateCommand CommandStartSingleTask { get { return _CommandStartSingleTask ?? (_CommandStartSingleTask = new DelegateCommand(args => OnStartSingleTask(args), args => OnCanStartSingleTask(args))); } }
        private DelegateCommand _CommandStartSingleTask;

        private bool OnCanStartSingleTask(object args)
        {
            if(SelectedItem==null) return false;
            return true;
        }

        private void OnStartSingleTask(object args)
        {
            
            var task = new UploadVectorDataTolocalDB();
           var arg = new UploadVectorDataTolocalDBArgument();
           
           if(SelectedItem is VectorDecodeBatchModel)
            {
                arg.BatchCode = (SelectedItem as VectorDecodeBatchModel).BatchCode;
            }else if(SelectedItem is VectorDecodeMode)
            {
                arg.BatchCode = (SelectedItem as VectorDecodeMode).BatchCode;
            }

            task.Argument = arg;

        var obj = task.Argument;

            var editor = new YuLinTu.Component.VectorDataDecoding.PropertyEditorCom();
            editor.pg.Properties["Workpage"] = Workpage;
            editor.Header = task.Name;

          
            editor.pg.Object = obj;
            editor.pgTask.Object = task;
            editor.pg.DataContext = obj;
            this.Workpage.TaskCenter.Add(task);
            task.Completed += new TaskCompletedEventHandler((o, t) =>
            {
             
                    //OnRefresh(args);

                Workpage.Workspace.Message.Send(this, new RefreshEventArgs(IdMsg.Refresh));



            });
            Workpage.Page.ShowMessageBox(editor, (v, r) =>
            {
                editor.pg.Object = null;
                editor.pgTask.Object = null;
                if(r== eCloseReason.Confirm)
                {
                    task.StartAsync();
                    Workpage.Message.Send(this, new MsgEventArgs(EdCore.langRequestShowTaskViewer));
                }
            });
          
      
        }
        #region Methods - System

        public void Dispose()
        {
            Workpage = null;
        }

        #endregion

        #endregion
        #region Commands - DeletFileTask

        public DelegateCommand CommandDeletFileTask { get { return _CommandDeletFileTask ?? (_CommandDeletFileTask = new DelegateCommand(args => OnDeletFileTask(args), args => OnCanDeletFileTask(args))); } }
        private DelegateCommand _CommandDeletFileTask;

        private bool OnCanDeletFileTask(object args)
        {
            if (SelectedItem == null||!(SelectedItem is VectorDecodeMode)) return false;
            return true;
        }

        private void OnDeletFileTask(object args)
        {
       
            var item = SelectedItem as VectorDecodeMode;      
            DbContext.BeginTransaction();
            var landQ = DbContext.CreateQuery<SpaceLandEntity>();
            var fileQ = DbContext.CreateQuery<VectorDecodeMode>();
            landQ.Where(t => t.FileID.Equals(item.FileID)).Delete().Save();
            fileQ.Where(t => t.FileID.Equals(item.FileID)).Delete().Save();
            DbContext.CommitTransaction();
            //System.Windows.Application.Current.Dispatcher.Invoke(new Action(() =>
            //{
                OnRefresh(args);
            //}));
        }
        #region Methods - System



        #endregion

        #endregion

        #region Commands - DeletFileTask

        public DelegateCommand CommandUploadVectorDataToSeverTask { get { return _CommandUploadVectorDataToSeverTask ?? (_CommandUploadVectorDataToSeverTask = new DelegateCommand(args => OnUploadVectorDataToSeverTask(args), args => OnCanUploadVectorDataToSeverTask(args))); } }
        private DelegateCommand _CommandUploadVectorDataToSeverTask;

        private bool OnCanUploadVectorDataToSeverTask(object args)
        {
            if (SelectedItem == null || !(SelectedItem is VectorDecodeBatchModel)) return false;
            return true;
        }

        private void OnUploadVectorDataToSeverTask(object args)
        {

            var task = new UploadVectorDataToSever();
            var arg = new UploadVectorDataToSeverArgument();

            if (SelectedItem is VectorDecodeBatchModel)
            {
                arg.BatchCode = (SelectedItem as VectorDecodeBatchModel).BatchCode;
            }
           

            task.Argument = arg;

            var obj = task.Argument;

            var editor = new YuLinTu.Component.VectorDataDecoding.PropertyEditorCom();
            editor.pg.Properties["Workpage"] = Workpage;
            editor.Header = task.Name;


            editor.pg.Object = obj;
            editor.pgTask.Object = task;
            editor.pg.DataContext = obj;
            this.Workpage.TaskCenter.Add(task);
            task.Completed += new TaskCompletedEventHandler((o, t) =>
            {

                //OnRefresh(args);

                Workpage.Workspace.Message.Send(this, new RefreshEventArgs(IdMsg.Refresh));



            });
            Workpage.Page.ShowMessageBox(editor, (v, r) =>
            {
                editor.pg.Object = null;
                editor.pgTask.Object = null;
                if (r == eCloseReason.Confirm)
                {
                    task.StartAsync();
                    Workpage.Message.Send(this, new MsgEventArgs(EdCore.langRequestShowTaskViewer));
                }
            });

        }
        #region Methods - System



        #endregion

        #endregion

        [MessageHandler(ID = IdMsg.NavigateZoneTo)]
        protected virtual void OnNavigateToZone(object sender, NavigateZoneToEventArgs e)
        {
            CurrentZone = e.Parameter as IZone;
            OnRefresh(null);
        }
        #endregion
    }
}
