/*
 * (C) 2015  鱼鳞图公司版权所有,保留所有权利
 * 地图裁剪-裁剪修角Converter
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
    class MapActionToMapClipAngleByLineButtonIsCheckedConverter : IValueConverter
    {
        #region Methods

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var action = value as MapControlActionQueryMapClipAngleByLine;
            if (action == null)
                return false;

            return true;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (!(value is bool))
                return null;

            bool isChecked = (bool)value;

            return isChecked ? new MapControlActionQueryMapClipAngleByLine(null) : null;
        }

        #endregion
    }
}
