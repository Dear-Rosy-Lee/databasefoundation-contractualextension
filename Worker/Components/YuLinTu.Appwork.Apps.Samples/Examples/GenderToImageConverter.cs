using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows.Media.Imaging;
using Xceed.Wpf.Samples.SampleData;
using YuLinTu;
using YuLinTu.Data;
using YuLinTu.Windows;
using YuLinTu.Windows.Wpf;
using YuLinTu.Windows.Wpf.Metro;
using YuLinTu.Windows.Wpf.Metro.Components;

namespace YuLinTu.Appwork.Apps.Samples
{
    public class GenderToImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (!(value is eGender))
                return BitmapFrame.Create(new Uri("pack://application:,,,/YuLinTu.Resources;component/Images/16/user-silhouette-question.png"));

            switch ((eGender)value)
            {
                case eGender.Male:
                    return BitmapFrame.Create(new Uri("pack://application:,,,/YuLinTu.Resources;component/Images/16/user.png"));
                case eGender.Female:
                    return BitmapFrame.Create(new Uri("pack://application:,,,/YuLinTu.Resources;component/Images/16/user-female.png"));
                case eGender.Unknown:
                default:
                    return BitmapFrame.Create(new Uri("pack://application:,,,/YuLinTu.Resources;component/Images/16/user-silhouette-question.png"));
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
