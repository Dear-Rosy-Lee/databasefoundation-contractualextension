/*
 * (C) 2015  鱼鳞图公司版权所有,保留所有权利 
 */
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel; 
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using YuLinTu.Library.Entity;

namespace YuLinTu.Library.Controls
{
    /// <summary>
    /// 图片数据转换类
    /// </summary>
    public class ImageValueConvert : IValueConverter
    {
        #region Fields

        /// <summary>
        /// 锁定
        /// </summary>
        public static BitmapImage imgLock = new BitmapImage(new Uri("pack://application:,,,/YuLinTu.Library.Resources;component/Resources/锁定承包方_16.png"));

        /// <summary>
        /// 家庭
        /// </summary>
        public static BitmapImage imgFamily = new BitmapImage(new Uri("pack://application:,,,/YuLinTu.Library.Resources;component/Resources/户主16.png"));

        /// <summary>
        /// 男
        /// </summary>
        public static BitmapImage imgMan = new BitmapImage(new Uri("pack://application:,,,/YuLinTu.Library.Resources;component/Resources/男16.png"));

        /// <summary>
        /// 女
        /// </summary>
        public static BitmapImage imgWoman = new BitmapImage(new Uri("pack://application:,,,/YuLinTu.Library.Resources;component/Resources/女16.png"));

        /// <summary>
        /// 未知
        /// </summary>
        public static BitmapImage imgUnKnown = new BitmapImage(new Uri("pack://application:,,,/YuLinTu.Library.Resources;component/Resources/户主_黑16.png"));

        #endregion

        #region Methods

        /// <summary>
        /// 转换
        /// </summary>
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            BitmapImage img = null;
            if (value == null)
                return imgFamily;
            eImage gender = (eImage)value;
            switch (gender)
            {
                case eImage.Family:
                    img = imgFamily;
                    break;
                case eImage.Man:
                    img = imgMan;
                    break;
                case eImage.Woman:
                    img = imgWoman;
                    break;
                case eImage.Unknown:
                    img = imgUnKnown;
                    break;
                case eImage.Lock:
                    img = imgLock;
                    break;
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
    /// 性别图片数据转换类
    /// </summary>
    public class GenderImageConvert : IValueConverter
    {
        #region Fields

        /// <summary>
        /// 男
        /// </summary>
        public static BitmapImage imgMan = new BitmapImage(new Uri("pack://application:,,,/YuLinTu.Library.Resources;component/Resources/男16.png"));

        /// <summary>
        /// 女
        /// </summary>
        public static BitmapImage imgWoman = new BitmapImage(new Uri("pack://application:,,,/YuLinTu.Library.Resources;component/Resources/女16.png"));

        /// <summary>
        /// 未知
        /// </summary>
        public static BitmapImage imgUnKnown = new BitmapImage(new Uri("pack://application:,,,/YuLinTu.Library.Resources;component/Resources/户主_黑16.png"));

        #endregion

        #region Methods

        /// <summary>
        /// 转换
        /// </summary>
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
                return imgUnKnown;
            eGender gender = (eGender)value;
            BitmapImage img = null;
            switch (gender)
            {
                case eGender.Male:
                    img = imgMan;
                    break;
                case eGender.Female:
                    img = imgWoman;
                    break;
                case eGender.Unknow:
                    img = imgUnKnown;
                    break;
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
    /// 性别数据转换类
    /// </summary>
    public class GenderValueConvert : IValueConverter
    {
        #region Methods

        /// <summary>
        /// 转换
        /// </summary>
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string sex = "";
            if (value == null)
                return sex;
            eGender gender = (eGender)value;
            switch (gender)
            {
                case eGender.Male:
                    sex = "男";
                    break;
                case eGender.Female:
                    sex = "女";
                    break;
            }
            return sex;
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
    /// 颜色数据转换类
    /// </summary>
    public class ColorValueConvert : IValueConverter
    {
        #region Methods

        /// <summary>
        /// 转换s
        /// </summary>
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
                return Brushes.DimGray ;
            bool gender = (bool)value;
            return gender ? Brushes.Brown : Brushes.DimGray;
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
    /// 锁定数据转换类
    /// </summary>
    public class LockValueConvert : IValueConverter
    {
        #region Methods

        /// <summary>
        /// 转换
        /// </summary>
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
                return false;
            int status = (int)value;
            return status == 0 ? false : true;
        }

        /// <summary>
        /// 转换回来
        /// </summary>
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return (bool)value;
        }

        #endregion
    }
}
