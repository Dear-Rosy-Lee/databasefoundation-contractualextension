using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using YuLinTu;
using YuLinTu.Appwork;
using YuLinTu.Data;
using YuLinTu.DF;
using YuLinTu.DF.Data;
using YuLinTu.DF.Zones;
using YuLinTu.Windows;
using YuLinTu.Windows.Wpf;
using YuLinTu.Windows.Wpf.Metro;
using YuLinTu.Windows.Wpf.Metro.Components;

namespace YuLinTu.Component.VectorDataDecoding
{
    [View(typeof(VectorDataDecodePage))]
    public partial class VectorDataDecodeViewModel : ViewModelObject, IDisposable
    {
        #region Properties

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

        #endregion

        #region Properties - Common

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

        public ITheWorkpage Workpage { get; private set; }

        #endregion

        #endregion

        #region Commands

        #region Commands - Loaded

        public DelegateCommand CommandLoaded { get { return _CommandLoaded ?? (_CommandLoaded = new DelegateCommand(obj => OnLoaded(obj))); } }
        private DelegateCommand _CommandLoaded;

        #endregion

        #region Commands - Refresh

   
     
        #endregion
        #endregion

        #region Ctor

        public VectorDataDecodeViewModel(ITheWorkpage workpage) : base(workpage.Message)
        {
       

            Workpage = workpage;
            //Items=new ObservableCollection<VectorDecodeBatchModel>();
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
            var chr = new VectorDecodeMode
            {
                ShapeFileName = "5101001012025060401.shp",
                ZoneCode = "510100101001",
                UplaodTime = DateTime.Now.ToLongDateString(),
                DataCount = 1030,

            };
            var chr2 = new VectorDecodeMode
            {
                DataCount = 1000,
                ShapeFileName = "5101001012025060402.shp",
                UplaodTime = DateTime.Now.ToLongDateString(),
            };

            var temp2 = new VectorDecodeBatchModel
            {
                BatchCode = "5101001012025060401",
                ZoneCode = "510100101001",
                UplaodTime = DateTime.Now.ToLongDateString(),
                DataCount = 3000,
                DecodeProgress = "已脱密",
                DecodeStaus = "是",
                NumbersOfDownloads = 0,

            };
            var chr3 = new VectorDecodeMode
            {
                ShapeFileName = "5101001012025060403.shp",
                ZoneCode = "510100101001",
                UplaodTime = DateTime.Now.ToLongDateString(),
                DataCount = 2000,

            };
            var chr4 = new VectorDecodeMode
            {
                DataCount = 1000,
                ShapeFileName = "5101001012025060404.shp",
                UplaodTime = DateTime.Now.ToLongDateString(),
            };
            temp.Children.Add(chr); temp.Children.Add(chr2);
            temp2.Children.Add(chr3); temp2.Children.Add(chr4);
            _Items.Add(temp);
            _Items.Add(temp2);
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
