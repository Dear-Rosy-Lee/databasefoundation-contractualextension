/*
 * (C) 2015  鱼鳞图公司版权所有,保留所有权利 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media.Imaging;
using YuLinTu.Data;
using YuLinTu.Library.Business;
using YuLinTu.Library.Controls;
using YuLinTu.Library.Entity;
using YuLinTu.Library.Repository;
using YuLinTu.Library.WorkStation;
using YuLinTu.Windows.Wpf.Metro.Components;

namespace YuLinTu.Component.MapFoundation
{
    public class VirtualPersonNavigator
    {
        #region Fields
        private string tabname = "未知";//当前节点名称
        #endregion

        #region Properties
        public IDbContext dbContext { get; set; }

        /// <summary>
        /// 取子节点
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public List<NavigateItem> GetChildren(NavigateItem item)
        {
            if (item.Object is string && (string)item.Object == "ROOT")
            {
                return GetRoots();
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
            var db = DataBaseSourceWork.GetDataBaseSource();//DataSource.Create<DbContext>(TheBns.Current.GetDataSourceName());
            if (db == null)
            {
                return new List<NavigateItem>();
            }
            ContainerFactory factroy = new ContainerFactory(db);
            string rootZone = NavigateZone.GetRootZoneCode();
            Zone zone = null;
            if (!string.IsNullOrEmpty(rootZone))
            {
                IZoneWorkStation station = factroy.CreateWorkstation<IZoneWorkStation, IZoneRepository>();
                zone = station.Get(rootZone);
            }
            if (zone == null)
            {
                zone = new Zone()
                {
                    FullCode = "86",
                    FullName = "中国",
                    Name = "中国",
                    Code = "86",
                    Level = eZoneLevel.State
                };
            }
            return GetRootChildren(zone);
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
            tabname = root.Name;
            List<NavigateItem> list = new List<NavigateItem>();
            var db = DataBaseSourceWork.GetDataBaseSource();
            if (db == null)
            {
                return null;
            }
            ContainerFactory factroy = new ContainerFactory(db);
            IZoneWorkStation station = factroy.CreateWorkstation<IZoneWorkStation, IZoneRepository>();
            //如果是村或组，读取对应的承包方
            if (root.Level == eZoneLevel.Group)
            {
                list = CreateVirtualPersonNavigateItemZ(root);
                return list;
            }
            if (root.Level == eZoneLevel.Village)
            {
                List<Zone> zoneCollection = station.Get(t => t.UpLevelCode == root.FullCode);
                zoneCollection.Sort((r1, r2) => r1.FullCode.CompareTo(r2.FullCode));
                zoneCollection.ForEach(t => list.Add(CreateNavigateItem(t)));

                list.AddRange(CreateVirtualPersonNavigateItemZ(root));

                return list;
            }
            else
            {
                List<Zone> zoneCollection = station.Get(t => t.UpLevelCode == root.FullCode);
                zoneCollection.Sort((r1, r2) => r1.FullCode.CompareTo(r2.FullCode));
                zoneCollection.ForEach(t => list.Add(CreateNavigateItem(t)));
                return list;
            }
        }


        /// <summary>
        /// 取村或组下承包方子节点
        /// </summary>
        /// <param name="root"></param>
        /// <returns></returns>
        private List<NavigateItem> CreateVirtualPersonNavigateItemZ(Zone root)
        {

            List<NavigateItem> groupPersonlist = new List<NavigateItem>();
            var db = DataBaseSourceWork.GetDataBaseSource();

            //获取到组或村别下的承包方               
            VirtualPersonBusiness vpb = new VirtualPersonBusiness(db);
            vpb.VirtualType = eVirtualType.SecondTable;

            var getVirtualPerson = vpb.GetByZone(root.FullCode);
            if (getVirtualPerson == null) return groupPersonlist;

            NavigateItem person = null;
            foreach (var item in getVirtualPerson)
            {
                person = new NavigateZoneItem();
                person.CanOpen = false;
                person.Object = item;
                person.Name = item.Name;
                person.Gallery = tabname + "承包方";
                person.Image = BitmapFrame.Create(new Uri("pack://application:,,,/YuLinTu.Component.MapFoundation;component/Resources/VirtualPerson.png"));
                person.Image24 = BitmapFrame.Create(new Uri("pack://application:,,,/YuLinTu.Component.MapFoundation;component/Resources/VirtualPerson.png"));
                person.GalleryImage = BitmapFrame.Create(new Uri("pack://application:,,,/YuLinTu.Resources;component/Images/24/inbox-document.png"));
                groupPersonlist.Add(person);
            }
            return groupPersonlist;
        }

        /// <summary>
        /// 创建子节点
        /// </summary>
        /// <param name="zone">地域</param>
        /// <param name="name">上级名称</param>
        /// <returns></returns>
        private NavigateItem CreateNavigateItem(Zone zone)
        {
            NavigateItem item = null;
            item = new NavigateZoneItem()
            {
                CanOpen = true,
                Object = zone,
                Name = zone.Name,
                Image = BitmapFrame.Create(new Uri("pack://application:,,,/YuLinTu.Resources;component/Images/16/marker.png")),
                Image24 = BitmapFrame.Create(new Uri("pack://application:,,,/YuLinTu.Resources;component/Images/24/marker.png")),
                Gallery = tabname,
                GalleryImage = BitmapFrame.Create(new Uri("pack://application:,,,/YuLinTu.Resources;component/Images/24/inbox-document.png")),
            };
            return item;
        }


        #endregion
    }
}
