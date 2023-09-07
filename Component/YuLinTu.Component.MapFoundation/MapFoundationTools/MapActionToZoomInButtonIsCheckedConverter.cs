/*
 * (C) 2015  鱼鳞图公司版权所有,保留所有权利
 * 地图放大converter
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
    public class MapActionToZoomInButtonIsCheckedConverter : IValueConverter
    {
        #region Methods
        /// <summary>
        /// 定义地图模式切换,如果是面积放大按下则返回true
        /// </summary>        
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var action = value as MapControlActionZoomIn;
            if (action == null)
                return false;

            return true;
        }
        /// <summary>
        /// 如果放大按钮被按下值为True，则返回放大对象
        /// </summary>      
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (!(value is bool))
                return null;

            bool isChecked = (bool)value;
            return isChecked ? new MapControlActionZoomIn(null) : null;
        }

        #endregion
    }
}
