/*
 * (C) 2015  鱼鳞图公司版权所有,保留所有权利 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media.Imaging;
using YuLinTu.Library.Entity;
using YuLinTu.Library.Business;

namespace YuLinTu.Library.Controls
{
    public class CadastralNumberToShortConverter : IValueConverter
    {
        #region Methods

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string num = value as string;
            if (num.IsNullOrBlank() || num.Length < 19)
                return string.Empty;

            num = num.Remove(0, 19);
            if (num.Length > 18)
                return num.Remove(0, 18);

            return num;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }

    /// <summary>
    /// 根据性别提取图片
    /// </summary>
    public class GenderToImageConverter : IValueConverter
    {
        #region Methods

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (!(value is eGender))
                return null;

            eGender g = (eGender)value;
            switch (g)
            {
                case eGender.Female:
                    return BitmapFrame.Create(new Uri(
                        "pack://application:,,,/YuLinTu.Resources;component/Images/16/user-female.png"));
                case eGender.Male:
                    return BitmapFrame.Create(new Uri(
                        "pack://application:,,,/YuLinTu.Resources;component/Images/16/user.png"));
                case eGender.Unknow:
                    return BitmapFrame.Create(new Uri(
                        "pack://application:,,,/YuLinTu.Resources;component/Images/16/user-silhouette-question.png"));
                default:
                    return null;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }

    /// <summary>
    /// 根据身份证取图片
    /// </summary>
    public class ICNToImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string ICN = "";
            string pic = "";
            if (value != null)
            {
                ICN = value.ToString();
            }
            int sex = ToolICN.GetGenderInNotCheck(ICN);
            if (string.IsNullOrEmpty(ICN) || sex == -1)
            {
                pic = "pack://application:,,,/YuLinTu.Library.Resources;component/Resources/户主_黑16.png";
            }
            else if (sex == 0)
            {
                pic = "pack://application:,,,/YuLinTu.Library.Resources;component/Resources/女16.png";
            }
            else
            {
                pic = "pack://application:,,,/YuLinTu.Library.Resources;component/Resources/男16.png";
            }
            return pic;
        }
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return null;
        }
    }
}
