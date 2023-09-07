using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
namespace YuLinTu.Component.DiagramFoundation
{
   public class MapActionToSelectPolyGonButtonIsCheckedConverter : IValueConverter
    {
        #region Methods
        /// <summary>
        /// 定义地图模式切换,如果是选择矩形框按下则返回true
        /// </summary>        
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var action = value as MapControlActionLayoutSelectPolygon;
            if (action == null)
                return false;

            return true;
        }

        /// <summary>
        /// 如果选择矩形框被按下值为True，则返回测量面积对象
        /// </summary>      
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (!(value is bool))
                return null;

            bool isChecked = (bool)value;
            return isChecked ? new MapControlActionLayoutSelectPolygon(null) : null;
        }

        #endregion
    }
}
