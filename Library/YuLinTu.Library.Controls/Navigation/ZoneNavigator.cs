/*
 * (C) 2025  鱼鳞图公司版权所有,保留所有权利 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media.Imaging;
using Utils.Tool;
using YuLinTu.Data;
using YuLinTu.Library.Business;
using YuLinTu.Library.Entity;
using YuLinTu.Library.Repository;
using YuLinTu.Library.WorkStation;
using YuLinTu.Windows;
using YuLinTu.Windows.Wpf.Metro.Components;

namespace YuLinTu.Library.Controls
{
    /// <summary>
    ///地域导航类
    /// </summary>
    public class ZoneNavigator
    {
        #region Fields

        #endregion

        #region Properties

        /// <summary>
        /// 数据库
        /// </summary>
        public IDbContext dbContext { get; set; }

        #endregion

        #region public

        /// <summary>
        /// 取子节点
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public List<NavigateItem> GetChildren(NavigateItem item)
        {
            if (item.Object is string && (string)item.Object == "ROOT")
            {
                var roots = GetRoots();
                return roots;
            }
            else if (item.Object is Zone)
            {
                return GetChildren((Zone)item.Object);
            }
            else
            {
                return new List<NavigateItem>();
            }
        }

        #endregion

        #region Private

        /// <summary>
        /// 取根节点
        /// </summary>
        /// <returns></returns>
        private List<NavigateItem> GetRoots()
        {
            List<NavigateItem> list = new List<NavigateItem>();
            var db = YuLinTu.Library.Business.DataBaseSource.GetDataBaseSource();          //DataSource.Create<DbContext>(TheBns.Current.GetDataSourceName());
            if (db == null)
            {
                return new List<NavigateItem>();
            }
            ContainerFactory factroy = new ContainerFactory(db);

            //var center = TheApp.Current.GetSystemSettingsProfileCenter();
            //var profile = center.GetProfile<CommonBusinessDefine>();
            //var section = profile.GetSection<CommonBusinessDefine>();
            //var codeZoneNew = section.Settings.CurrentZoneFullCode;
            string rootZone = NavigateZone.GetRootZoneCode();  //NavigateZone.RootZoneCode();
            Zone zone = null;
            IZoneWorkStation station = factroy.CreateWorkstation<IZoneWorkStation, IZoneRepository>();
            if (!string.IsNullOrEmpty(rootZone))
            {
                zone = station.Get(rootZone);
            }
            if (zone == null)
            {
                zone = new Zone()
                {
                    FullCode = "86",
                    FullName = "中国",
                    Name = "中国",
                    Code = "86"
                };
            }
            return new List<NavigateItem>() { CreateNavigateItem(zone, station.Any(c => c.UpLevelCode == zone.FullCode)) };
        }

        /// <summary>
        /// 取子节点
        /// </summary>
        private List<NavigateItem> GetChildren(Zone zone)
        {
            List<NavigateItem> list = new List<NavigateItem>();
            list.AddRange(GetRootChildren(zone));
            return list;
        }

        /// <summary>
        /// 取地域子节点
        /// </summary>
        /// <param name="root"></param>
        /// <returns></returns>
        private List<NavigateItem> GetRootChildren(Zone root)
        {
            var db = DataBaseSourceWork.GetDataBaseSource();//DataSource.Create<DbContext>(TheBns.Current.GetDataSourceName());
            if (db == null)
            {
                return null;
            }
            ContainerFactory factroy = new ContainerFactory(db);
            IZoneWorkStation station = factroy.CreateWorkstation<IZoneWorkStation, IZoneRepository>();
            List<NavigateItem> list = new List<NavigateItem>();
            List<Zone> zoneCollection = station.Get(t => t.UpLevelCode == root.FullCode);
            zoneCollection.Sort((r1, r2) => r1.FullCode.CompareTo(r2.FullCode));
            zoneCollection.ForEach(t => list.Add(CreateNavigateItem(t, station.Any(c => c.UpLevelCode == t.FullCode))));
            return list;
        }

        /// <summary>
        /// 创建子节点
        /// </summary>
        /// <param name="zone">地域</param>
        /// <param name="name">上级名称</param>
        /// <returns></returns>
        private NavigateItem CreateNavigateItem(Zone zone, bool canOpen)
        {
            NavigateItem item = null;
            item = new NavigateZoneItem()
            {
                CanOpen = canOpen,
                Object = zone,
                Name = zone.Name,
                Image = BitmapFrame.Create(new Uri("pack://application:,,,/YuLinTu.Resources;component/Images/16/folder-horizontal-open.png")),
                Image24 = BitmapFrame.Create(new Uri("pack://application:,,,/YuLinTu.Resources;component/Images/24/marker.png")),
                Gallery = zone.UpLevelName,
                GalleryImage = BitmapFrame.Create(new Uri("pack://application:,,,/YuLinTu.Resources;component/Images/24/inbox-document.png")),
            };
            return item;
        }

        #endregion
    }
}
