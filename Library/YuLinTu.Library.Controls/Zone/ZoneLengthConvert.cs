using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace YuLinTu.Library.Controls
{
    /// <summary>
    /// 根据区域级别转化相应图片标识
    /// </summary>
    public class ZoneLengthConvert : IValueConverter
    {
        /// <summary>
        /// 根据区域级别转化相应图片标识
        /// </summary>
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
            {
                return ZoneDataItemHelper.imgGroup;
            }
            string name = (string)value;
            var length = name.GetLiteralLength();
            if (length == 2 )
            {
                if (name == "86")
                {
                    return ZoneDataItemHelper.imgCountry;
                }
                else
                    return ZoneDataItemHelper.imgProvince;
                
            }
            else if (length == 4)
            {
                return ZoneDataItemHelper.imgCity;
            }
            else if (length == 6)
            {
                return ZoneDataItemHelper.imgCounty;
            }
            else if (length == 9)
            {
                return ZoneDataItemHelper.imgTown;
            }
            else if (length == 12)
            {
                return ZoneDataItemHelper.imgVillage;
            }
            else 
            {
                return ZoneDataItemHelper.imgGroup;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
