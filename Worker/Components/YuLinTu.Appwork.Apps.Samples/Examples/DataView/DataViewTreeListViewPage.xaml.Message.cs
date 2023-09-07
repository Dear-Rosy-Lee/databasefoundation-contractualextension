using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media.Imaging;
using YuLinTu;
using YuLinTu.Appwork;
using YuLinTu.Data;
using YuLinTu.Windows;
using YuLinTu.Windows.Wpf;
using YuLinTu.Windows.Wpf.Metro;
using YuLinTu.Windows.Wpf.Metro.Components;

namespace YuLinTu.Appwork.Apps.Samples.Examples.DataView
{
    public partial class DataViewTreeListViewPageViewModel :
        YuLinTu.Messages.Workspace.IMessageHandlerInstallAccountData,
        YuLinTu.Messages.Workspace.IMessageHandlerUninstallAccountData,
        YuLinTu.Messages.Workpage.IMessageHandlerInstallWorkpageContent,
        YuLinTu.Messages.Workpage.IMessageHandlerUninstallWorkpageContent,
        YuLinTu.Messages.Workpage.IMessageHandlerInitializeWorkpageContentCompleted,
        YuLinTu.Messages.Workpage.IMessageHandlerInstallNavigateItem,
        YuLinTu.Messages.Workpage.IMessageHandlerNavigateTo
    {
        #region Methods

        #region Methods - Message

        public void InstallWorkpageContent(object sender, InstallWorkpageContentEventArgs e)
        {
        }

        public void UninstallWorkpageContent(object sender, UninstallWorkpageContentEventArgs e)
        {
        }

        public void InitializeWorkpageContentCompleted(object sender, InitializeWorkpageContentCompletedEventArgs e)
        {
        }

        public void InstallAccountData(object sender, AccountEventArgs e)
        {
        }

        public void UninstallAccountData(object sender, AccountEventArgs e)
        {
        }

        public void InstallNavigateItem(object sender, InstallNavigateItemMsgEventArgs e)
        {
            System.Windows.Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                var navigator = Workpage.Properties.TryGetValue<Navigator>("Navigator", null);
                if (navigator == null || !navigator.RootItemAutoExpand)
                    return;

                navigator.RootItemAutoExpand = false;
            }));

            if (e.Instance.Root == null || e.Instance.Root.Object == null)
                return;

            if (e.Instance.Root.Object.ToString() == "ROOT")
            {
                var drives = DriveInfo.GetDrives().Where(c => c.DriveType == DriveType.Fixed).ToList();

                foreach (var drive in drives)
                    e.Instance.Items.Add(CreateDriveItem(drive));

                return;
            }

            var root = e.Instance.Root.Object as DiskObject;
            if (root == null)
                return;

            var dirs = Directory.GetDirectories(root.Path);
            foreach (var dir in dirs)
                e.Instance.Items.Add(CreateDirectoryItem(dir));

            var files = Directory.GetFiles(root.Path);
            foreach (var file in files)
                e.Instance.Items.Add(CreateFileItem(file));
        }

        public void NavigateTo(object sender, NavigateToMsgEventArgs e)
        {
            if (e.Object == null)
            {
                TreeViewItemsSource = null;
                return;
            }

            var obj = e.Object.Object as DiskObject;
            if (obj == null || obj.IsFile)
            {
                TreeViewItemsSource = null;
                return;
            }

            string path = obj.Path;

            var item = new DiskObject();
            item.IsFile = false;
            item.Name = System.IO.Path.GetFileName(path);
            item.Path = path;
            item.DateTimeModified = new DirectoryInfo(path).LastWriteTime;

            TreeViewItemsSource = new ObservableCollection<DiskObject>() { item };
        }

        #endregion

        #region Methods - Private

        private NavigateItem CreateDirectoryItem(string path)
        {
            var obj = new DiskObject();
            obj.IsFile = false;
            obj.Name = System.IO.Path.GetFileName(path);
            obj.Path = path;
            obj.DateTimeModified = new DirectoryInfo(path).LastWriteTime;

            var item = new NavigateItem();
            item.CanOpen = HasItems(obj.Path);
            item.Image = BitmapFrame.Create(new Uri("pack://application:,,,/YuLinTu.Resources;component/Images/16/folder-horizontal-open.png"));
            item.Name = obj.Name;
            item.Object = obj;

            return item;
        }

        private NavigateItem CreateFileItem(string path)
        {
            var obj = new DiskObject();
            obj.IsFile = true;
            obj.Name = System.IO.Path.GetFileName(path);
            obj.Path = path;
            obj.DateTimeModified = new FileInfo(path).LastWriteTime;

            var item = new NavigateItem();
            item.CanOpen = HasItems(obj.Path);
            item.Image = BitmapFrame.Create(new Uri("pack://application:,,,/YuLinTu.Resources;component/Images/16/Document.png"));
            item.Name = obj.Name;
            item.Object = obj;

            return item;
        }

        private NavigateItem CreateDriveItem(DriveInfo drive)
        {
            var obj = new DiskObject();
            obj.IsFile = false;
            obj.Name = drive.Name;
            obj.Path = drive.Name;

            var item = new NavigateItem();
            item.CanOpen = HasItems(obj.Path);
            item.Image = BitmapFrame.Create(new Uri("pack://application:,,,/YuLinTu.Resources;component/Images/16/drive-globe.png"));
            item.Name = obj.Name;
            item.Object = obj;

            return item;
        }

        private bool HasItems(string path)
        {
            try
            {
                var hasItems = false;
                var dirs = Directory.GetDirectories(path);
                hasItems = dirs.Length != 0;
                if (hasItems)
                    return true;

                var files = Directory.GetFiles(path);
                hasItems = files.Length != 0;
                if (hasItems)
                    return true;

                return false;
            }
            catch
            {
                return false;
            }
        }

        #endregion

        #endregion
    }
}
