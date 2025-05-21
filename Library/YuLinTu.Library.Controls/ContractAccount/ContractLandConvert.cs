/*
 * (C) 2025  鱼鳞图公司版权所有,保留所有权利 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace YuLinTu.Library.Controls
{
    /// <summary>
    /// 承包地数据转换类
    /// </summary>
    public class ContractLandConvert : IValueConverter
    {
        #region Fields

        #endregion

        #region Methods

        /// <summary>
        /// 转换
        /// </summary>
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if ((int)value == 1)
                return false;
            return true;
        }

        /// <summary>
        /// 转换回来
        /// </summary>
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return null;
        }

        #endregion
    }
}
