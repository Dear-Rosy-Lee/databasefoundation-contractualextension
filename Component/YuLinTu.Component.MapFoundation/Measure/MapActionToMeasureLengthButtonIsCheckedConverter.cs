/*
 * (C) 2025  鱼鳞图公司版权所有,保留所有权利
 * 添加测量地图长度模式converter
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
    public class MapActionToMeasureLengthButtonIsCheckedConverter : IValueConverter
    {
        #region Methods
        /// <summary>
        /// 定义地图模式切换,如果是测量长度按下则返回true
        /// </summary>        
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var action = value as MapControlActionMeasureLength;
            if (action == null)
                return false;

            return true;
        }
        /// <summary>
        /// 如果测量按钮被按下值为True，则返回测量长度对象
        /// </summary>      
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (!(value is bool))
                return null;

            bool isChecked = (bool)value;
            return isChecked ? new MapControlActionMeasureLength(null) : null;
        }

        #endregion
    }
}
