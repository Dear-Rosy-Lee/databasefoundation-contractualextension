using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media.Imaging;
using YuLinTu;
using YuLinTu.Appwork;
using YuLinTu.Data;
using YuLinTu.Windows;
using YuLinTu.Windows.Wpf;
using YuLinTu.Windows.Wpf.Metro;
using YuLinTu.Windows.Wpf.Metro.Components;

namespace YuLinTu.Appwork.Apps.Samples.Examples.Others
{
    [View(typeof(ImageResourcesViewerPage))]
    public partial class ImageResourcesViewerPageViewModel : ViewModelObject, IDisposable
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

        public CatalogDescriptor CurrentCatalog
        {
            get { return _CurrentCatalog; }
            set { _CurrentCatalog = value; NotifyPropertyChanged(() => CurrentCatalog); OnRefresh(null); }
        }
        private CatalogDescriptor _CurrentCatalog;

        public string FilterKey
        {
            get { return _FilterKey; }
            set { _FilterKey = value; NotifyPropertyChanged(() => FilterKey); }
        }
        private string _FilterKey;

        public int ResourcesCount
        {
            get { return _ResourcesCount; }
            set { _ResourcesCount = value; NotifyPropertyChanged(() => ResourcesCount); }
        }
        private int _ResourcesCount;

        public int FilterResourceCount
        {
            get { return _FilterResourceCount; }
            set { _FilterResourceCount = value; NotifyPropertyChanged(() => FilterResourceCount); }
        }
        private int _FilterResourceCount;


        public ObservableCollection<ResourceItem> Items
        {
            get { return _Items; }
            set { _Items = value; NotifyPropertyChanged(() => Items); }
        }
        private ObservableCollection<ResourceItem> _Items = new ObservableCollection<ResourceItem>();

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

        public DelegateCommand CommandRefresh { get { return _CommandRefresh ?? (_CommandRefresh = new DelegateCommand(args => OnRefresh(args), args => OnCanRefresh(args))); } }
        private DelegateCommand _CommandRefresh;

        private bool OnCanRefresh(object args)
        {
            return true;
        }

        private void OnRefresh(object args)
        {
            //为了提示加载进度，获取到工作页的实例，这种写法有违背 MVVM 的嫌疑，
            //但这里为了方便，提示进度的代码允许这样写，其余功能不允许这样写。
            var page = Workpage.Page.Content as Page;

            // 由于数据量大，在上一次加载数据的过程中，用户由触发了加载数据的过程，
            // 那么应该先取消上一次的加载过程，让下面的 IsStopPending 的值变为 true。
            tq.Cancel();
            tq.Do(go =>
            {
                var list = GetResources(page, go);
                go.Instance.Argument.UserState = list;

            }, completed =>
            {
                if (completed.Instance.IsStopPending)
                    return;

                Items = completed.Result as ObservableCollection<ResourceItem>;
                ApplyFilter(Items, FilterKey);

            }, error =>
            {
                Workpage.Page.ShowDialog(new YuLinTu.Windows.Wpf.Metro.Components.MessageDialog()
                {
                    Header = "错误",
                    Message = string.Format("加载失败，错误详细信息为 {0}", error.Exception),
                    MessageGrade = eMessageGrade.Error
                });

            }, progressChanged =>
            {
                page.RaiseProgressChanged(progressChanged.Percent);
                ResourcesCount = Convert.ToInt32(progressChanged.UserState);

            }, started: started =>
            {
                Workpage.Page.IsBusy = true;
                IsRefreshing = true;
                page.RaiseProgressBegin();
                Items.Clear();
                ResourcesCount = 0;
                FilterResourceCount = 0;

            }, ended: ended =>
            {
                page.RaiseProgressEnd();
                IsRefreshing = false;
                Workpage.Page.IsBusy = false;
            });
        }

        private ObservableCollection<ResourceItem> GetResources(Page page, TaskGoEventArgs go)
        {
            var list = new List<ResourceItem>();

            var path = CurrentCatalog == null ? string.Empty : CurrentCatalog.Path;
            var resources = YuLinTu.Resources.ResourcesHelper.GetResources();

            double index = 0;
            int cnt = 0;
            var lastPercent = -1;

            foreach (var item in resources)
            {
                var percent = (int)(index++ / resources.Count * 100);

                if (lastPercent != percent)
                    go.Instance.ReportProgress(lastPercent = percent, cnt);

                if (!item.Key.LocalPath.StartsWith(path))
                    continue;

                try
                {
                    list.Add(CreateResourceItem(item));
                    cnt++;
                }
                catch { }
            }

            go.Instance.ReportProgress(100, list.Count);

            list = list.OrderBy(c => c.Name).ThenBy(c => c.UriPath).ToList();

            var oc = new ObservableCollection<ResourceItem>();
            list.ForEach(c => oc.Add(c));
            return oc;
        }

        private ResourceItem CreateResourceItem(KeyValue<Uri, object> item)
        {
            var ri = new ResourceItem();
            ri.Name = item.Key.LocalPath.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries).Last();
            ri.Image = BitmapFrame.Create(item.Value as Stream);
            ri.Size = $"{Math.Round(ri.Image.Width)} × {Math.Round(ri.Image.Height)}";
            ri.MaxLength = Math.Max(Math.Round(ri.Image.Width), Math.Round(ri.Image.Height));
            ri.UriPath = $"pack://application:,,,{item.Key.LocalPath}";

            return ri;
        }

        #endregion

        #region Commands - Filter

        public DelegateCommand CommandFilter { get { return _CommandFilter ?? (_CommandFilter = new DelegateCommand(args => OnFilter(args), args => OnCanFilter(args))); } }
        private DelegateCommand _CommandFilter;

        private bool OnCanFilter(object args)
        {
            return true;
        }

        private void OnFilter(object args)
        {
            // 使用延迟通知机制，在1秒后通知开始过滤，
            // 若1秒内多次触发，则以最后一次的为准，
            // 以此避免频繁输入过滤条件引起的界面卡顿。
            if (reporter == null)
                return;

            reporter.Start(args);
        }

        #endregion

        #region Commands - CopyName

        public DelegateCommand CommandCopyName { get { return _CommandCopyName ?? (_CommandCopyName = new DelegateCommand(args => OnCopyName(args), args => OnCanCopyName(args))); } }
        private DelegateCommand _CommandCopyName;

        private bool OnCanCopyName(object args)
        {
            return SelectedItem != null;
        }

        private void OnCopyName(object args)
        {
            if (SelectedItem != null)
                Clipboard.SetText((SelectedItem as ResourceItem).Name);
        }

        #endregion

        #region Commands - CopyUri

        public DelegateCommand CommandCopyUri { get { return _CommandCopyUri ?? (_CommandCopyUri = new DelegateCommand(args => OnCopyUri(args), args => OnCanCopyUri(args))); } }
        private DelegateCommand _CommandCopyUri;

        private bool OnCanCopyUri(object args)
        {
            return SelectedItem != null;
        }

        private void OnCopyUri(object args)
        {
            if (SelectedItem != null)
                Clipboard.SetText((SelectedItem as ResourceItem).UriPath);
        }

        #endregion

        #endregion

        #region Fields

        private TaskQueue tq = new TaskQueueDispatcher();
        private DetentionReporter reporter = null;
        private VirtualizingWrapPanel wrapPanel = null;

        #endregion

        #region Ctor

        public ImageResourcesViewerPageViewModel(ITheWorkpage workpage) : base(workpage.Message)
        {
            Workpage = workpage;

            reporter = DetentionReporterDispatcher.Create(
                System.Windows.Application.Current.Dispatcher,
                c => Filter(c), 600, 600);
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
            var cp = obj as CommandParameterEx;
            wrapPanel = cp.Sender as VirtualizingWrapPanel;
        }

        private void ScrollToTop()
        {
            if (wrapPanel != null)
                wrapPanel.SetVerticalOffset(0);
        }

        private void ApplyFilter(ObservableCollection<ResourceItem> oc, string key)
        {
            FilterResourceCount = 0;

            var view = CollectionViewSource.GetDefaultView(oc);

            key = key.TrimSafe();
            int cnt = 0;

            view.Filter = obj =>
            {
                if (key.IsNullOrBlank())
                {
                    cnt++;
                    return true;
                }

                var objE = obj as ResourceItem;
                if (objE != null && objE.Name.Contains(key))
                {
                    cnt++;
                    return true;
                }

                return false;
            };

            FilterResourceCount = cnt;

            ScrollToTop();
        }

        private void Filter(DetentionElapsedEventArgs c)
        {
            var cp = c.Value as CommandParameterEx;
            var key = (cp.Sender as TextBox).Text;

            ApplyFilter(Items, key);
        }

        #endregion

        #region Methods - System

        public void Dispose()
        {
            Workpage = null;
            reporter.Dispose();
            reporter = null;
            wrapPanel = null;
        }

        #endregion

        #endregion
    }
}
