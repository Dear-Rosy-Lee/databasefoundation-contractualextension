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
    /// 二轮台账图片数据转换类
    /// </summary>
    public class SecondAccountImgConvert : IValueConverter
    {
        #region Fields

        /// <summary>
        /// 地块
        /// </summary>
        public static BitmapImage imgLand = new BitmapImage(new Uri("pack://application:,,,/YuLinTu.Library.Resources;component/Resources/承包地16.png"));

        /// <summary>
        /// 家庭
        /// </summary>
        public static BitmapImage imgFamily = new BitmapImage(new Uri("pack://application:,,,/YuLinTu.Library.Resources;component/Resources/户主16.png"));

        #endregion

        #region Methods

        /// <summary>
        /// 转换
        /// </summary>
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
                return imgLand;
            bool isFamily = (bool)value;
            if (isFamily)
                return imgFamily;
            return imgLand;
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
