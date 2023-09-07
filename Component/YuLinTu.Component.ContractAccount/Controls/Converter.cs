using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media.Imaging;
using YuLinTu.Diagrams;
using YuLinTu.Spatial;
using YuLinTu.tGIS.Client;

namespace YuLinTu.Component.ContractAccount
{
    public class ActionToSelectButtonIsCheckedConverter : IValueConverter
    {
        #region Methods

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var action = value as DiagramsViewActionSelect;
            if (action == null)
                return false;

            return true;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (!(value is bool))
                return null;

            bool isChecked = (bool)value;
            return isChecked ? new DiagramsViewActionSelect(null) : null;
        }

        #endregion
    }

    public class ActionToPanButtonIsCheckedConverter : IValueConverter
    {
        #region Methods

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var action = value as DiagramsViewActionPan;
            if (action == null)
                return false;

            return true;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (!(value is bool))
                return null;

            bool isChecked = (bool)value;
            return isChecked ? new DiagramsViewActionPan(null) : null;
        }

        #endregion
    }

    public class CurrentResolutionToScalePercentConverter : IValueConverter
    {
        #region Methods

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var res = (double)value;

            return 1.0 / res;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var scale = (double)value;
            var view = parameter as DiagramsView;

            var res = 1.0 / scale;
            view.ZoomTo(res);
            return res;
        }

        #endregion
    }
}
