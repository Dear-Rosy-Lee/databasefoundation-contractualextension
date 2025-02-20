/*
 * (C) 2025  鱼鳞图公司版权所有,保留所有权利 
 */
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media.Imaging;
using YuLinTu.Library.Entity;

namespace YuLinTu.Library.Controls
{
    /// <summary>
    /// 地域数据转换类
    /// </summary>
    public class ZoneDataItemHelper
    {
        #region Fields

        /// <summary>
        /// 国
        /// </summary>
        public static BitmapImage imgCountry = new BitmapImage(new Uri("pack://application:,,,/YuLinTu.Library.Resources;component/Resources/点16.png"));

        /// <summary>
        /// 省
        /// </summary>
        public static BitmapImage imgProvince = new BitmapImage(new Uri("pack://application:,,,/YuLinTu.Library.Resources;component/Resources/省16.png"));

        /// <summary>
        /// 市
        /// </summary>
        public static BitmapImage imgCity = new BitmapImage(new Uri("pack://application:,,,/YuLinTu.Library.Resources;component/Resources/市16.png"));

        /// <summary>
        /// 县
        /// </summary>
        public static BitmapImage imgCounty = new BitmapImage(new Uri("pack://application:,,,/YuLinTu.Library.Resources;component/Resources/县16.png"));

        /// <summary>
        /// 乡
        /// </summary>
        public static BitmapImage imgTown = new BitmapImage(new Uri("pack://application:,,,/YuLinTu.Library.Resources;component/Resources/镇16.png"));

        /// <summary>
        /// 村
        /// </summary>
        public static BitmapImage imgVillage = new BitmapImage(new Uri("pack://application:,,,/YuLinTu.Library.Resources;component/Resources/村16.png"));

        /// <summary>
        /// 组
        /// </summary>
        public static BitmapImage imgGroup = new BitmapImage(new Uri("pack://application:,,,/YuLinTu.Library.Resources;component/Resources/组16.png"));

        #endregion

        #region Methods

        /// <summary>
        /// 转换地域为绑定实体
        /// </summary>
        public static ZoneDataItem ConvertToDataItem(Zone zone)
        {
            ZoneDataItem item = new ZoneDataItem();
            item.Code = zone.Code;
            item.Comment = zone.Comment;
            item.FullCode = zone.FullCode;
            item.FullName = zone.FullName;
            item.ID = zone.ID;
            item.Img = GetImgByLevel(zone.Level);
            item.Level = zone.Level;
            item.Name = zone.Name;
            item.Shape = zone.Shape;
            item.UpLevelCode = zone.UpLevelCode;
            item.UpLevelName = zone.UpLevelName;
            item.Visibility = Visibility.Visible;
            item.CreateTime = zone.CreateTime;
            item.CreateUser = zone.CreateUser;
            item.LastModifyTime = zone.LastModifyTime;
            item.LastModifyUser = zone.LastModifyUser;
            return item;
        }

        /// <summary>
        ///设置图片 
        /// </summary>
        /// <returns></returns>
        public static BitmapImage GetImgByLevel(eZoneLevel level)
        {
            BitmapImage img = null;
            switch (level)
            {
                case eZoneLevel.Group:
                    img = imgGroup;
                    break;
                case eZoneLevel.Village:
                    img = imgVillage;
                    break;
                case eZoneLevel.Town:
                    img = imgTown;
                    break;
                case eZoneLevel.County:
                    img = imgCounty;
                    break;
                case eZoneLevel.City:
                    img = imgCity;
                    break;
                case eZoneLevel.Province:
                    img = imgProvince;
                    break;
                case eZoneLevel.State:
                    img = imgCountry;
                    break;
                default:
                    img = imgGroup;
                    break;
            }
            return img;
        }

        /// <summary>
        /// 创建模板地域
        /// </summary>
        /// <param name="upLevel">上级地域等级</param>
        /// <param name="upCode">上级地域编码</param>
        /// <param name="upName">上级地域名称</param>
        /// <returns></returns>
        public static Zone CreateTempZone(eZoneLevel upLevel, string upCode, string upName)
        {
            Zone childZone = new Zone();
            childZone.Level = (eZoneLevel)(upLevel - 1);
            childZone.UpLevelCode = upCode;
            childZone.UpLevelName = upName;
            childZone.FullCode = upCode;
            childZone.FullName = upName;
            return childZone;
        }

        #endregion
    }
}
