using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Media.Imaging;
using YuLinTu;
using YuLinTu.Appwork;
using YuLinTu.Data;
using YuLinTu.Windows;
using YuLinTu.Windows.Wpf;
using YuLinTu.Windows.Wpf.Metro;
using YuLinTu.Windows.Wpf.Metro.Components;

namespace YuLinTu.Appwork.Apps.Samples.Examples
{
    public partial class OptionsNavigatorPanelViewModel :
        YuLinTu.Messages.Workspace.IMessageHandlerInstallAccountData,
        YuLinTu.Messages.Workspace.IMessageHandlerUninstallAccountData,
        YuLinTu.Messages.Workpage.IMessageHandlerInstallWorkpageContent,
        YuLinTu.Messages.Workpage.IMessageHandlerUninstallWorkpageContent,
        YuLinTu.Messages.Workpage.IMessageHandlerInitializeWorkpageContentCompleted,
        YuLinTu.Messages.Workpage.IMessageHandlerInstallNavigateItem,
        YuLinTu.Messages.Workpage.IMessageHandlerNavigateTo,
        YuLinTu.Messages.Workpage.IMessageHandlerInstallNavigatorOptionsEditor
    {
        #region Methods

        #region Methods - Message

        public void InstallWorkpageContent(object sender, InstallWorkpageContentEventArgs e)
        {
            Workpage.Properties["DefaultOptionsButtonVisible"] = false;
            Workpage.Properties["DefaultTasksButtonVisible"] = false;
        }

        public void UninstallWorkpageContent(object sender, UninstallWorkpageContentEventArgs e)
        {
        }

        public void InitializeWorkpageContentCompleted(object sender, InitializeWorkpageContentCompletedEventArgs e)
        {
            var navigator = Workpage.Properties.TryGetValue<Navigator>("Navigator", null);
            if (navigator == null)
                return;

            //navigator.NavigatorRowHeight = new System.Windows.GridLength(200);
            //navigator.NavigatorRowMinHeight = 100;
            //navigator.NavigatorRowMaxHeight = 500;

            //List<NavigateItem> list = new List<NavigateItem>();
            //list.Add(CreateDirectoryItem(@"I:\Projects\Atomic"));
            ////list.Add(CreateDirectoryItem(@"I:\Projects\Atomic\Framework\bin\SailfishCore\YuLinTu.Components.Ultilities"));
            ////list.Add(CreateDirectoryItem(@"I:\Projects\Atomic\Framework\bin\SailfishCore\YuLinTu.Components.Ultilities\build"));
            ////list.Add(CreateFileItem(@"I:\Projects\Atomic\Framework\bin\SailfishCore\YuLinTu.Components.Ultilities\build\YuLinTu.Components.Ultilities.props"));

            //navigator.ExpandTo(list);
        }

        public void InstallAccountData(object sender, AccountEventArgs e)
        {
        }

        public void UninstallAccountData(object sender, AccountEventArgs e)
        {
        }

        public void InstallNavigateItem(object sender, InstallNavigateItemMsgEventArgs e)
        {
            if (e.Instance.Root == null || e.Instance.Root.Object == null)
                return;

            if (e.Instance.Root.Object.ToString() == "ROOT")
            {
                var center = Workpage.Workspace.GetUserSettingsProfileCenter();
                var profile = center.GetProfile<OptionsEditorNavigatorPathConfiguration>();
                var section = profile.GetSection<OptionsEditorNavigatorPathConfiguration>();
                if (section.Settings == null || section.Settings.Path.TrimSafe().IsNullOrBlank())
                    return;

                e.Instance.Items.Add(CreateDirectoryItem(section.Settings.Path));
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
        }

        public void InstallNavigatorOptionsEditor(object sender, InstallNavigatorOptionsEditorEventArgs e)
        {
            Workpage.Workspace.Window.Dispatcher.Invoke(new Action(() =>
            {
                e.Editors.Add(new WorkpageOptionsEditorMetadata()
                {
                    Name = "路径",
                    Editor = new OptionsEditorNavigatorPath(Workpage),
                });
            }));
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
