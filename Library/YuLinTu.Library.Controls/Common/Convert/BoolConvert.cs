using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace YuLinTu.Library.Controls
{
    public class BoolConvert : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return null;
            if ((bool)value)
                return "是";
            return "否";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var str = value as string;
            if (string.IsNullOrEmpty(str))
                return null;
            if (str == "是")
                return true;
            if (str == "否")
                return false;
            return null;

        }
    }
}
