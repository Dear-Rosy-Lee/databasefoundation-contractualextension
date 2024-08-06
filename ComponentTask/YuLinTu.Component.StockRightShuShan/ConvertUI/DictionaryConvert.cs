using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Data;
using YuLinTu.Library.Business;

namespace YuLinTu.Component.StockRightShuShan.ConvertUI
{
    public class DictionaryConvert : IValueConverter
    {
        protected string GroupCode { get; set; }


        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var str = value as string;
            if (string.IsNullOrWhiteSpace(str))
                return string.Empty;
            var dicList = DataBaseSource.GetDataBaseSource().CreateDictWorkStation().Get(o => o.GroupCode == GroupCode);
            return dicList.Find(o => o.Code == str.Trim()).Name;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var dicList = DataBaseSource.GetDataBaseSource().CreateDictWorkStation().Get(o => o.GroupCode == GroupCode);
            var str = value as string;
            if (string.IsNullOrWhiteSpace(str))
                return string.Empty;
            return dicList.Find(o => o.Name == str.Trim()).Code;
        }

    }
}
