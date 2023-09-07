using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using YuLinTu;
using YuLinTu.Data;
using YuLinTu.Windows;
using YuLinTu.Windows.Wpf;
using YuLinTu.Windows.Wpf.Metro;
using YuLinTu.Windows.Wpf.Metro.Components;

namespace YuLinTu.Appwork.Apps.Samples
{
    public class ScoreToBackgroundConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (!(value is double))
                return Brushes.Transparent;

            var score = (double)value;
            if (score >= 90)
                return Application.Current.TryFindResource(ResourceKeys.SuccessBrushKey);
            else if (score >= 80)
                return Application.Current.TryFindResource(ResourceKeys.InformationBrushKey);
            else if (score >= 70)
                return Application.Current.TryFindResource(ResourceKeys.WarnBrushKey);
            else if (score >= 60)
                return Application.Current.TryFindResource(ResourceKeys.ErrorBrushKey);
            else
                return Application.Current.TryFindResource(ResourceKeys.ExceptionBrushKey);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
