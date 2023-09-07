using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Data;
using YuLinTu;
using YuLinTu.Appwork;
using YuLinTu.Data;
using YuLinTu.Windows;
using YuLinTu.Windows.Wpf;
using YuLinTu.Windows.Wpf.Metro;
using YuLinTu.Windows.Wpf.Metro.Components;

namespace YuLinTu.Appwork.Apps.Samples.Examples.DataView
{
    [View(typeof(DataViewNavigateDataGridPage))]
    public partial class DataViewNavigateDataGridPageViewModel : ViewModelObject, IDisposable
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

        public object NavigateObject
        {
            get { return _NavigateObject; }
            set { _NavigateObject = value; NotifyPropertyChanged(() => NavigateObject); OnRefresh(null); }
        }
        private object _NavigateObject;

        public string FilterKey
        {
            get { return _FilterKey; }
            set { _FilterKey = value; NotifyPropertyChanged(() => FilterKey); }
        }
        private string _FilterKey;

        public int EmployeeCount
        {
            get { return _EmployeeCount; }
            set { _EmployeeCount = value; NotifyPropertyChanged(() => EmployeeCount); }
        }
        private int _EmployeeCount;

        public ObservableCollection<EmployeeItem> Items
        {
            get { return _Items; }
            set { _Items = value; NotifyPropertyChanged(() => Items); }
        }
        private ObservableCollection<EmployeeItem> _Items = new ObservableCollection<EmployeeItem>();

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
            Items.Clear();
            EmployeeCount = 0;

            Workpage.Page.IsBusy = true;
            IsRefreshing = true;

            //为了提示加载进度，获取到工作页的实例，这种写法有违背 MVVM 的嫌疑，
            //但这里为了方便，提示进度的代码允许这样写，其余功能不允许这样写。
            var page = Workpage.Page.Content as Page;
            page.RaiseProgressBegin();

            tq.Do(go =>
            {
                var list = GetEmployee(page, NavigateObject);
                go.Instance.Argument.UserState = list;

            }, completed =>
            {
                var list = completed.Result as ObservableCollection<EmployeeItem>;
                Items = list;

                EmployeeCount = list.Count;

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

        private ObservableCollection<EmployeeItem> GetEmployee(Page page, object root)
        {
            var list = new ObservableCollection<EmployeeItem>();

            var orders = Xceed.Wpf.Samples.SampleData.SampleDataProvider.GetOrders();

            double index = 0;

            foreach (var item in orders)
            {
                page.RaiseProgressChanged((int)(index++ / orders.Count * 100));

                var country = root as CountryItem;
                if (country != null && country.Name != item.ShipCountry)
                    continue;
                var city = root as CityItem;
                if (city != null && city.Name != item.ShipCity)
                    continue;
                if (city == null && country == null)
                    continue;

                var employee = item.ConvertTo<EmployeeItem>();
                employee.Name = $"{item.Employee.FirstName} {item.Employee.LastName}";
                employee.Photo = item.Employee.SmallPhoto;

                list.Add(employee);
            }

            page.RaiseProgressChanged(100);

            return list;
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

        #endregion

        #region Fields

        private TaskQueue tq = new TaskQueueDispatcher();
        private DetentionReporter reporter = null;

        #endregion

        #region Ctor

        public DataViewNavigateDataGridPageViewModel(ITheWorkpage workpage) : base(workpage.Message)
        {
            Workpage = workpage;

            reporter = DetentionReporterDispatcher.Create(
                System.Windows.Application.Current.Dispatcher,
                c => FilterTreeGrid(c), 1000, 1000);
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
            OnRefresh(obj);
        }

        private void FilterTreeGrid(DetentionElapsedEventArgs c)
        {
            var cp = c.Value as CommandParameterEx;
            var key = (cp.Sender as TextBox).Text;
            var dg = cp.Parameter as DataGrid;

            var view = CollectionViewSource.GetDefaultView(Items);
            view.Filter = obj =>
            {
                if (key.IsNullOrBlank())
                    return true;

                var objE = obj as EmployeeItem;
                if (objE != null)
                    return objE.Name.Contains(key) || objE.ShipCity.Contains(key);

                return false;
            };
        }

        #endregion

        #region Methods - System

        public void Dispose()
        {
            Workpage = null;
            tq = null;

            reporter.Dispose();
            reporter = null;
        }

        #endregion

        #endregion
    }
}
