/*
 * (C) 2025  鱼鳞图公司版权所有,保留所有权利 
 */
using System;
using System.Drawing;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace YuLinTu.Library.Controls
{
    /// <summary>
    /// 承包台账图片转换
    /// </summary>
    public class ContractAccountImgConvert : IValueConverter
    {
        #region Fields

        /// <summary>
        /// 地块
        /// </summary>
        public BitmapImage imgLand { get; set; } //= new BitmapImage(new Uri("pack://application:,,,/YuLinTu.Library.Resources;component/Resources/承包地16.png"));

        /// <summary>
        /// 空间地块
        /// </summary>
        public BitmapImage imgGeoLand { get; set; } // = new BitmapImage(new Uri("pack://application:,,,/YuLinTu.Library.Resources;component/Resources/landGeo.png"));

        /// <summary>
        /// 家庭
        /// </summary>
        public BitmapImage imgFamily { get; set; } // = new BitmapImage(new Uri("pack://application:,,,/YuLinTu.Library.Resources;component/Resources/户主16.png"));

        /// <summary>
        /// 锁定
        /// </summary>
        public BitmapImage imgLock { get; set; } // = new BitmapImage(new Uri("pack://application:,,,/YuLinTu.Library.Resources;component/Resources/锁定承包方_16.png"));

        #endregion

        #region Methods

        public ContractAccountImgConvert()
        {
            imgLand = new BitmapImage(new Uri("pack://application:,,,/YuLinTu.Library.Resources;component/Resources/承包地16.png"));
            imgGeoLand = new BitmapImage(new Uri("pack://application:,,,/YuLinTu.Library.Resources;component/Resources/landGeo.png"));
            imgFamily = new BitmapImage(new Uri("pack://application:,,,/YuLinTu.Library.Resources;component/Resources/户主16.png"));
            imgLock = new BitmapImage(new Uri("pack://application:,,,/YuLinTu.Library.Resources;component/Resources/锁定承包方_16.png"));
            imgLand.CacheOption = BitmapCacheOption.OnLoad;
            imgLand.CreateOptions = BitmapCreateOptions.DelayCreation‌;

            imgGeoLand.CacheOption = BitmapCacheOption.OnLoad;
            imgGeoLand.CreateOptions = BitmapCreateOptions.DelayCreation‌;

            imgFamily.CacheOption = BitmapCacheOption.OnLoad;
            imgFamily.CreateOptions = BitmapCreateOptions.DelayCreation‌;

            imgLock.CacheOption = BitmapCacheOption.OnLoad;
            imgLock.CreateOptions = BitmapCreateOptions.DelayCreation‌;
        }


        /// <summary>
        /// 转换
        /// </summary>
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
                return imgLand;
            int type = (int)value;
            if (type == 0)
            {
                //未被锁定承包方
                return imgFamily;
            }
            else if (type == 1)
            {
                //空间地块
                return imgGeoLand;
            }
            else if (type == 2)
            {
                //没有空间信息地块
                return imgLand;
            }
            else
            {
                //被锁定承包方
                return imgLock;
            }
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
