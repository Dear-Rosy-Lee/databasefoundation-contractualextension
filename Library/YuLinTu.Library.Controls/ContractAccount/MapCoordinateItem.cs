/*
 * (C) 2016  鱼鳞图公司版权所有,保留所有权利 
 */
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;
using YuLinTu.Library.Entity;

namespace YuLinTu.Library.Controls
{
    /// <summary>
    /// 地图坐标点界面显示实体
    /// </summary>
    public class MapCoordinateItem : NotifyCDObject
    {
        #region Property

        [DisplayLanguage("序号")]
        public string OrderID
        {
            get { return _OrderID; }
            set { _OrderID = value; NotifyPropertyChanged("OrderID"); }
        }
        private string _OrderID;

        [DisplayLanguage("X坐标")]
        public string XCoordinate
        {
            get { return _XCoordinate; }
            set { _XCoordinate = value; NotifyPropertyChanged("XCoordinate"); }
        }
        private string _XCoordinate;

        [DisplayLanguage("Y坐标")]
        public string YCoordinate
        {
            get { return _YCoordinate; }
            set { _YCoordinate = value; NotifyPropertyChanged("YCoordinate"); }
        }
        private string _YCoordinate;

        /// <summary>
        /// 坐标点实体
        /// </summary>
        public GeoAPI.Geometries.Coordinate CoordEntity { get; set; }

        #endregion

        #region Ctor

        /// <summary>
        /// 构造方法
        /// </summary>
        public MapCoordinateItem()
        {
        }

        #endregion
    }
}
