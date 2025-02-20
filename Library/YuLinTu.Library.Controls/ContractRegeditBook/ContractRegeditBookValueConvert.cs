/*
 * (C) 2025  鱼鳞图公司版权所有,保留所有权利 
 */
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media.Imaging;
using YuLinTu.Library.Entity;

namespace YuLinTu.Library.Controls
{   
    /// <summary>
    /// 图片数据转换类
    /// </summary>
    public class ContractRegeditBookValueConvert : IValueConverter
    {
        #region Fields

        /// <summary>
        /// 权证
        /// </summary>
        public static BitmapImage imgRegeditBook = new BitmapImage(new Uri("pack://application:,,,/YuLinTu.Library.Resources;component/Resources/证书16.png"));

        /// <summary>
        /// 家庭
        /// </summary>
        public static BitmapImage imgFamily = new BitmapImage(new Uri("pack://application:,,,/YuLinTu.Library.Resources;component/Resources/户主16.png"));

        /// <summary>
        /// 锁定
        /// </summary>
        public static BitmapImage imgLock = new BitmapImage(new Uri("pack://application:,,,/YuLinTu.Library.Resources;component/Resources/锁定承包方_16.png"));

        #endregion

        #region Methods

        /// <summary>
        /// 转换
        /// </summary>
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            BitmapImage img = imgFamily;
            if (value == null)
            {
                return img;
            }
            int imgvalue = System.Convert.ToInt32(value);
            if (imgvalue == 2)
            {
                img = imgRegeditBook;
            }
            else if (imgvalue == 0)
            {
                img = imgLock;
            }
            return img;
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

    /// <summary>
    /// 布尔值数据转换类
    /// </summary>
    public class ContractRegeditBookCheckValueConvert : IValueConverter
    {
        #region Methods

        /// <summary>
        /// 转换
        /// </summary>
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
            {
                return true;
            }
            bool vis = System.Convert.ToBoolean(value);
            return !vis;
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
