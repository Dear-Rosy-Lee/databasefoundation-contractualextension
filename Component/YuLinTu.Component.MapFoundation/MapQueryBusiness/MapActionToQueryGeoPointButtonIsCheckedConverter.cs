/*
 * (C) 2015  鱼鳞图公司版权所有,保留所有权利
 * 查询界址点的空间信息Converter
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media.Imaging;
using YuLinTu.Spatial;
using YuLinTu.tGIS.Client;

namespace YuLinTu.Component.MapFoundation
{
   public class MapActionToQueryGeoPointButtonIsCheckedConverter : IValueConverter
    {
        #region Methods

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var action = value as MapControlActionQueryGeoPoint;
            if (action == null)
                return false;

            return true;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (!(value is bool))
                return null;

            bool isChecked = (bool)value;
           
            return isChecked ? new MapControlActionQueryGeoPoint(null) : null;
        }

        #endregion
    }
}
