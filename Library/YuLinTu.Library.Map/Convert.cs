using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media.Imaging;
using YuLinTu.tGIS.Client;

namespace YuLinTu.Library.Map
{
//    public class LayerIsRenderToLayerIconVisibilityConverter : IValueConverter
//    {
//        #region Methods

//        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
//        {
//            if (!(value is bool))
//                return Visibility.Hidden;

//            bool val = (bool)value;
//            return val ? Visibility.Hidden : Visibility.Visible;
//        }

//        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
//        {
//            throw new NotImplementedException();
//        }

//        #endregion
//    }

//    public class LayerIsRenderToLayerLoadIconVisibilityConverter : IValueConverter
//    {
//        #region Methods

//        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
//        {
//            if (!(value is bool))
//                return Visibility.Hidden;

//            bool val = (bool)value;
//            return !val ? Visibility.Hidden : Visibility.Visible;
//        }

//        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
//        {
//            throw new NotImplementedException();
//        }

//        #endregion
//    }

    /// <summary>
    /// 根据图层类型变更图标
    /// </summary>
    public class LayerToIconConverter : IValueConverter
    {
        #region Methods

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (!(value is Layer))
                return null;

            var layerGroup = value as IGroupLayer;
            if (layerGroup != null)
                return BitmapFrame.Create(new Uri(@"pack://application:,,,/YuLinTu.Resources;component/Images/24/inbox-document.png"));

            return BitmapFrame.Create(new Uri(@"pack://application:,,,/YuLinTu.Resources;component/Images/24/map.png"));
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
