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

namespace YuLinTu.Appwork.Apps.Samples.Examples.DataView
{
    [View(typeof(DataViewTreeListViewPage))]
    public partial class DataViewTreeListViewPageViewModel : ViewModelObject, IDisposable
    {
        #region Properties

        #region Properties - Common

        public System.Collections.ObjectModel.ObservableCollection<DiskObject> TreeViewItemsSource
        {
            get { return _TreeViewItemsSource; }
            set { _TreeViewItemsSource = value; NotifyPropertyChanged("TreeViewItemsSource"); }
        }
        private System.Collections.ObjectModel.ObservableCollection<DiskObject> _TreeViewItemsSource;

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

        #region Commands - HasItemsGetter

        public DelegateCommand CommandHasItemsGetter { get { return _CommandHasItemsGetter ?? (_CommandHasItemsGetter = new DelegateCommand(args => OnHasItemsGetter(args), args => OnCanHasItemsGetter(args))); } }
        private DelegateCommand _CommandHasItemsGetter;

        private bool OnCanHasItemsGetter(object args)
        {
            return true;
        }

        private void OnHasItemsGetter(object args)
        {
            var cpe = args as CommandParameterEx;
            var e = cpe.EventArgs as Windows.Wpf.Metro.Components.MetroViewItemHasItemsEventArgs;

            var obj = e.Object as DiskObject;
            if (obj == null)
                return;

            e.HasItems = !obj.IsFile;
            if (!e.HasItems)
                return;

            var dirs = System.IO.Directory.GetDirectories(obj.Path);
            e.HasItems = dirs.Length != 0;
            if (e.HasItems)
                return;

            var files = System.IO.Directory.GetFiles(obj.Path);
            e.HasItems = files.Length != 0;
            if (e.HasItems)
                return;
        }

        #endregion

        #region Commands - ItemGetter

        public DelegateCommand CommandItemGetter { get { return _CommandItemGetter ?? (_CommandItemGetter = new DelegateCommand(args => OnItemGetter(args), args => OnCanItemGetter(args))); } }
        private DelegateCommand _CommandItemGetter;

        private bool OnCanItemGetter(object args)
        {
            return true;
        }

        private void OnItemGetter(object args)
        {
            var cpe = args as CommandParameterEx;
            var e = cpe.EventArgs as Windows.Wpf.Metro.Components.MetroViewItemItemsEventArgs;

            var obj = e.Object as DiskObject;
            if (obj == null)
                return;

            var dirs = System.IO.Directory.GetDirectories(obj.Path);
            foreach (var dir in dirs)
            {
                var item = CreateDirectoryObject(dir);
                System.Windows.Application.Current.Dispatcher.Invoke(new Action(() => obj.Children.Add(item)));
            }

            var files = System.IO.Directory.GetFiles(obj.Path);
            foreach (var file in files)
            {
                var item = CreateFileObject(file);
                System.Windows.Application.Current.Dispatcher.Invoke(new Action(() => obj.Children.Add(item)));
            }
        }

        #endregion

        #endregion

        #region Ctor

        public DataViewTreeListViewPageViewModel(ITheWorkpage workpage) : base(workpage.Message)
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

        private DiskObject CreateDirectoryObject(string path)
        {
            var item = new DiskObject();
            item.IsFile = false;
            item.Name = System.IO.Path.GetFileName(path);
            item.Path = path;
            item.DateTimeModified = new System.IO.DirectoryInfo(path).LastWriteTime;

            return item;
        }

        private DiskObject CreateFileObject(string path)
        {
            var item = new DiskObject();
            item.IsFile = true;
            item.Name = System.IO.Path.GetFileName(path);
            item.Path = path;
            item.DateTimeModified = new System.IO.FileInfo(path).LastWriteTime;

            return item;
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
