/*
 * (C) 2025  鱼鳞图公司版权所有,保留所有权利
 * 地图裁剪-按面积及份数自动裁剪Converter获取
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
    public class MapActionToMapClipBySetButtonIsCheckedConverter : IValueConverter
    {
        #region Methods
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var action = value as MapControlActionQueryMapClipBySetting;
            if (action == null)
                return false;

            return true;
        }
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (!(value is bool))
                return null;

            bool isChecked = (bool)value;

            return isChecked ? new MapControlActionQueryMapClipBySetting(null) : null;
        }
        #endregion
    }
}
