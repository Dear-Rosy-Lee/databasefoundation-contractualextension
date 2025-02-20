/*
 * (C) 2025  鱼鳞图公司版权所有,保留所有权利 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;
using YuLinTu.Library.Entity;

namespace YuLinTu.Library.Controls
{
    /// <summary>
    /// 地域类别名称转换类
    /// </summary>
    public class ZoneLevelConvert : IValueConverter
    {
        /// <summary>
        /// 转换值
        /// </summary>
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string name = string.Empty;
            eZoneLevel level = (eZoneLevel)value;
            name = EnumNameAttribute.GetDescription(level);
            return name;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return null;
        }
    }
}
