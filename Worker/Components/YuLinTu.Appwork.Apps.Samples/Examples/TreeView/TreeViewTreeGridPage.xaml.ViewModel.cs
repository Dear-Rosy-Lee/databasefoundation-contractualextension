using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using YuLinTu;
using YuLinTu.Appwork;
using YuLinTu.Data;
using YuLinTu.Windows;
using YuLinTu.Windows.Wpf;
using YuLinTu.Windows.Wpf.Metro;
using YuLinTu.Windows.Wpf.Metro.Components;

namespace YuLinTu.Appwork.Apps.Samples.Examples.TreeView
{
    [View(typeof(TreeViewTreeGridPage))]
    public partial class TreeViewTreeGridPageViewModel : ViewModelObject, IDisposable
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

        public int CountryCount
        {
            get { return _CountryCount; }
            set { _CountryCount = value; NotifyPropertyChanged(() => CountryCount); }
        }
        private int _CountryCount;

        public int EmployeeCount
        {
            get { return _EmployeeCount; }
            set { _EmployeeCount = value; NotifyPropertyChanged(() => EmployeeCount); }
        }
        private int _EmployeeCount;

        public ObservableCollection<CountryItem> Items
        {
            get { return _Items; }
            set { _Items = value; NotifyPropertyChanged(() => Items); }
        }
        private ObservableCollection<CountryItem> _Items = new ObservableCollection<CountryItem>();


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
            CountryCount = 0;
            EmployeeCount = 0;

            Workpage.Page.IsBusy = true;
            IsRefreshing = true;

            //为了提示加载进度，获取到工作页的实例，这种写法有违背 MVVM 的嫌疑，
            //但这里为了方便，提示进度的代码允许这样写，其余功能不允许这样写。
            var page = Workpage.Page.Content as Page;
            page.RaiseProgressBegin();

            tq.Do(go =>
            {
                var countries = GetCountries(page);
                go.Instance.Argument.UserState = countries;

            }, completed =>
            {
                var countries = completed.Result as ObservableCollection<CountryItem>;
                Items = countries;

                CountryCount = countries.Count;
                EmployeeCount = countries.Sum(c => c.Children.Count);

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

        private ObservableCollection<CountryItem> GetCountries(Page page)
        {
            var listCountry = new ObservableCollection<CountryItem>();

            var orders = Xceed.Wpf.Samples.SampleData.SampleDataProvider.GetOrders();

            double index = 0;
            var listEmployee = new List<EmployeeItem>();

            foreach (var item in orders)
            {
                var employee = item.ConvertTo<EmployeeItem>();
                employee.Name = $"{item.Employee.FirstName} {item.Employee.LastName}";
                employee.Photo = item.Employee.SmallPhoto;

                listEmployee.Add(employee);

                page.RaiseProgressChanged((int)(index++ / orders.Count * 100));
            }

            var groups = listEmployee.GroupBy(c => c.ShipCountry).ToList();
            foreach (var item in groups)
            {
                var country = new CountryItem() { Name = item.Key };
                item.ToList().ForEach(c => country.Children.Add(c));

                listCountry.Add(country);
            }

            page.RaiseProgressChanged(100);

            return listCountry;
        }

        #endregion

        #endregion

        #region Fields

        private TaskQueue tq = new TaskQueueDispatcher();

        #endregion

        #region Ctor

        public TreeViewTreeGridPageViewModel(ITheWorkpage workpage) : base(workpage.Message)
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
            OnRefresh(obj);
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
