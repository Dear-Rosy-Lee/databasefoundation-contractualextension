using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Windows.Media.Imaging;
using YuLinTu.Windows.Wpf.Metro.Components;

namespace YuLinTu.Appwork.Apps.Samples
{
    internal class ResourcesNavigator
    {
        #region Fields

        private KeyValueList<Uri, object> res;

        #endregion

        #region Ctor

        public ResourcesNavigator(KeyValueList<Uri, object> resources)
        {
            res = resources;
        }

        #endregion

        #region Methods

        #region Methods - Public

        public List<NavigateItem> GetChildren(NavigateItem item)
        {
            if (item.Object is string && (string)item.Object == "ROOT")
                return GetRoots();
            else if (item.Object is CatalogDescriptor)
                return GetChildren(item.Object as CatalogDescriptor);
            else
                return new List<NavigateItem>();
        }

        #endregion

        #region Methods - Private

        private List<NavigateItem> GetChildren(CatalogDescriptor gd)
        {
            Dictionary<string, bool> dicCanOpen = new Dictionary<string, bool>();

            res.Where(c => c.Key.LocalPath.StartsWith(gd.Path)).ToList().ForEach(c =>
            {
                var relativePath = c.Key.LocalPath.Replace(gd.Path, "");
                var parts = relativePath.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length < 2)
                    return;

                var next = parts[0];
                var canOpen = false;
                if (dicCanOpen.ContainsKey(next))
                    canOpen = dicCanOpen[next];
                if (canOpen)
                    return;

                dicCanOpen[next] = parts.Length > 2;
            });

            var listNV = new List<NavigateItem>();

            foreach (var item in dicCanOpen.OrderBy(c => c.Key).ToList())
                listNV.Add(CreateNavigateItem(gd.Path + item.Key + "/", item.Key, item.Value));

            return listNV;
        }

        private List<NavigateItem> GetRoots()
        {
            return GetChildren(new CatalogDescriptor
            {
                Path = "/YuLinTu.Resources;",
            });
        }

        private NavigateItem CreateNavigateItem(string path, string name, bool canOpen)
        {
            var item = new NavigateItemExample
            {
                CanOpen = canOpen,
                Name = name,
                Image = BitmapFrame.Create(new Uri("pack://application:,,,/YuLinTu.Resources;component/Images/16/folder-horizontal-open.png")),
                Image24 = BitmapFrame.Create(new Uri("pack://application:,,,/YuLinTu.Resources;component/Images/16/folder-horizontal-open.png")),
                Object = new CatalogDescriptor
                {
                    Name = name,
                    Path = path,
                }
            };

            return item;
        }

        #endregion

        #endregion
    }
}
