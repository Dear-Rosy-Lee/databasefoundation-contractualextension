/*
 * (C) 2015  鱼鳞图公司版权所有,保留所有权利
 * 界面显示类，被列表控件使用
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YuLinTu.Spatial;
using YuLinTu.Windows.Wpf.Metro.Components;

namespace YuLinTu.Component.ZoneInvestigation
{
    public class GeoPointUI : NotifyCDObject
    {
        [DisplayLanguage("序号")]
        public double ID
        {
            get { return _ID; }
            set { _ID = value; NotifyPropertyChanged("ID"); }
        }
        private double _ID;

        [DisplayLanguage("X坐标")]
        public double XCoordinate
        {
            get { return _XCoordinate; }
            set { _XCoordinate = value; NotifyPropertyChanged("XCoordinate"); }
        }
        private double _XCoordinate;

        [DisplayLanguage("Y坐标")]
        public double YCoordinate
        {
            get { return _YCoordinate; }
            set { _YCoordinate = value; NotifyPropertyChanged("YCoordinate"); }
        }
        private double _YCoordinate;

        [DisplayLanguage("距离")]
        public double Distance
        {
            get { return _Distance; }
            set { _Distance = value; NotifyPropertyChanged("Distence"); }
        }
        private double _Distance;
        
    }
}
