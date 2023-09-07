using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Data;
using YuLinTu.Library.Business;
using YuLinTu.Library.Entity;

namespace YuLinTu.Library.Business
{
    public  class DictionaryConvert : IValueConverter
    {
        protected  string GroupCode { get; set; }


        public  object Convert(object value, Type targetType=null, object parameter=null, CultureInfo culture=null)
        {
            var str = value as string;
            if (string.IsNullOrWhiteSpace(str))
                return string.Empty;
            var dicList = DictionaryHelper.GetDictionaryByGroupCode(GroupCode);
            var dictonary = dicList.Find(o => o.Code == str.Trim());
            return dictonary==null ? null:dictonary.Name;
        }

        public  object ConvertBack(object value, Type targetType=null, object parameter=null, CultureInfo culture=null)
        {
            var dicList = DictionaryHelper.GetDictionaryByGroupCode(GroupCode);
            var str = value as string;
            if (string.IsNullOrWhiteSpace(str))
                return string.Empty;
            var dictonary=dicList.Find(o => o.Name == str.Trim());
            return dictonary==null? null:dictonary.Code;
        }

    }
}
