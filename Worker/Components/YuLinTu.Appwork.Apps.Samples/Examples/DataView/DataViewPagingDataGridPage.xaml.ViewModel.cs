using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
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
    [View(typeof(DataViewPagingDataGridPage))]
    public partial class DataViewPagingDataGridPageViewModel : ViewModelObject, IDisposable
    {
        #region Properties

        #region Properties - Common

        public IDataPagerProvider DataSource
        {
            get { return _DataSource; }
            set { _DataSource = value; NotifyPropertyChanged(() => DataSource); }
        }
        private IDataPagerProvider _DataSource = new DataPagerProviderEmployeeDynamic();

        public object SelectedItem
        {
            get { return _SelectedItem; }
            set { _SelectedItem = value; NotifyPropertyChanged(() => SelectedItem); }
        }
        private object _SelectedItem;

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

        #region Commands - InitializeColumn

        public DelegateCommand CommandInitializeColumn { get { return _CommandInitializeColumn ?? (_CommandInitializeColumn = new DelegateCommand(args => OnInitializeColumn(args), args => OnCanInitializeColumn(args))); } }
        private DelegateCommand _CommandInitializeColumn;

        private bool OnCanInitializeColumn(object args)
        {
            return true;
        }

        private void OnInitializeColumn(object args)
        {
            var cp = args as CommandParameterEx;
            var e = cp.EventArgs as InitializeDataGridColumnEventArgs;
            var sender = cp.Sender as PagableDataGrid;

            var col = new DataGridTextColumn();
            col.Header = e.Property.AliasName;

            var b = new Binding(string.Format("{0}", e.Property.ColumnName));
            col.Binding = b;

            if (e.Property.ColumnName == "Photo")
            {
                var ct = new DataGridTemplateColumn();
                ct.CellTemplate = sender.TryFindResource("PhotoColumnCellTemplate") as DataTemplate;
                ct.Header = e.Property.ColumnName;
                e.Column = ct;
                return;
            }
            else if (e.Property.ColumnName == "ShipCountry")
            {
                var ct = new DataGridTemplateColumn();
                ct.CellTemplate = sender.TryFindResource("CountryColumnCellTemplate") as DataTemplate;
                ct.Header = e.Property.ColumnName;
                ct.SortMemberPath = e.Property.ColumnName;
                e.Column = ct;
                return;
            }
            else if (e.Property.ColumnName == "Freight")
            {
                var ha = HorizontalAlignment.Right; // 内容右对齐

                var style = new Style(typeof(TextBlock));
                style.Setters.Add(new Setter(TextBlock.HorizontalAlignmentProperty, ha));
                col.ElementStyle = style;

                var style1 = new Style(typeof(DataGridColumnHeader));
                style1.BasedOn = Application.Current.TryFindResource("Metro_DataGrid_HeaderContainer_Style") as Style;
                style1.Setters.Add(new Setter(DataGridColumnHeader.HorizontalContentAlignmentProperty, ha));
                DataGridAttacher.SetColumnHeaderHorizontalContentAlignment(col, ha);
                col.HeaderStyle = style1;

                b.StringFormat = "{0:C}";
            }

            if (e.Property.ColumnType == eDataType.Decimal ||
                e.Property.ColumnType == eDataType.Double ||
                e.Property.ColumnType == eDataType.Float ||
                e.Property.ColumnType == eDataType.Int16 ||
                e.Property.ColumnType == eDataType.Int32 ||
                e.Property.ColumnType == eDataType.Int64 ||
                e.Property.ColumnType == eDataType.Byte)
            {
                var ha = HorizontalAlignment.Right; // 内容右对齐

                var style = new Style(typeof(TextBlock));
                style.Setters.Add(new Setter(TextBlock.HorizontalAlignmentProperty, ha));
                col.ElementStyle = style;

                var style1 = new Style(typeof(DataGridColumnHeader));
                style1.BasedOn = Application.Current.TryFindResource("Metro_DataGrid_HeaderContainer_Style") as Style;
                style1.Setters.Add(new Setter(DataGridColumnHeader.HorizontalContentAlignmentProperty, ha));
                DataGridAttacher.SetColumnHeaderHorizontalContentAlignment(col, ha);
                col.HeaderStyle = style1;
            }
            else if (e.Property.ColumnType == eDataType.DateTime)
            {
                var ha = HorizontalAlignment.Center; // 内容居中

                var style = new Style(typeof(TextBlock));
                style.Setters.Add(new Setter(TextBlock.HorizontalAlignmentProperty, ha));
                col.ElementStyle = style;

                var style1 = new Style(typeof(DataGridColumnHeader));
                style1.BasedOn = Application.Current.TryFindResource("Metro_DataGrid_HeaderContainer_Style") as Style;
                style1.Setters.Add(new Setter(DataGridColumnHeader.HorizontalContentAlignmentProperty, ha));
                DataGridAttacher.SetColumnHeaderHorizontalContentAlignment(col, ha);
                col.HeaderStyle = style1;

                b.StringFormat = "yyyy年MM月dd日";
            }

            e.Column = col;
        }

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
            var dg = args as PagableDataGrid;
            dg.Refresh();
        }

        #endregion

        #endregion

        #region Ctor

        public DataViewPagingDataGridPageViewModel(ITheWorkpage workpage) : base(workpage.Message)
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
