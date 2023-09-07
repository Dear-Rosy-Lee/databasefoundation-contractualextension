/*
 * (C) 2016 鱼鳞图公司版权所有,保留所有权利
 * 区域赋值Converter 20161114 陈泽林
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using YuLinTu.tGIS.Client;
using YuLinTu.Windows;

namespace YuLinTu.Component.MapFoundation
{
    class MapControlActionMapZoneAssignmentConverter : IValueConverter
    {
        public IWorkpage TheWorkPage { get; set; }
        public MapControl map { get; set; }
        #region Methods

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var action = value as MapActionMeasureAreaAssignment;
            if (action == null)
                return false;

            return true;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (!(value is bool))
                return null;

            bool isChecked = (bool)value;

            return isChecked ? new MapActionMeasureAreaAssignment(map, TheWorkPage) : null;
        }

        #endregion
    }
}
