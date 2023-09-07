using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using YuLinTu.tGIS.Client;
using YuLinTu.Library.Business;
using YuLinTu.Windows.Wpf.Metro.Components;

namespace YuLinTu.Component.MapFoundation
{
    /// <summary>
    /// 地图操作工具
    /// </summary>
    public partial class MapToolPanel : UserControl
    {
        #region Ctor

        /// <summary>
        /// 构造函数
        /// </summary>
        public MapToolPanel(MapControl mapControl)
        {
            MapControl = mapControl;
            InitializeComponent();
        }

        /// <summary>
        /// 初始化
        /// </summary>
        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);
            SetToggleButtonBinding();
        }

        /// <summary>
        /// 设置按钮绑定
        /// </summary>
        private void SetToggleButtonBinding()
        {
            var binding = new Binding("Action");
            binding.Source = MapControl;
            binding.Converter = new MapActionToMeasureAreaButtonIsCheckedConverter();
            binding.Mode = BindingMode.TwoWay;
            mtbMeasureArea.SetBinding(MetroToggleButton.IsCheckedProperty, binding);

            binding = new Binding("Action");
            binding.Source = MapControl;
            binding.Converter = new MapActionToMeasureLengthButtonIsCheckedConverter();
            binding.Mode = BindingMode.TwoWay;
            mtbMeasureLength.SetBinding(MetroToggleButton.IsCheckedProperty, binding);

            binding = new Binding("Action");
            binding.Source = MapControl;
            binding.Converter = new MapActionToMapClipAngleByLineButtonIsCheckedConverter();
            binding.Mode = BindingMode.TwoWay;
            angleClipped.SetBinding(MetroToggleButton.IsCheckedProperty, binding);
        }

        #endregion

        #region Field

        private const int scaleRation = 100;
        private const double tolerance = 0.0001;

        #endregion

        #region Property

        /// <summary>
        /// 地图控件
        /// </summary>
        public MapControl MapControl { get; set; }

        #endregion

        #region Events

        /// <summary>
        /// 上一视图
        /// </summary>
        private void previousView_Click(object sender, RoutedEventArgs e)
        {
            MapControl.PreviousView();
        }

        /// <summary>
        /// 下一视图
        /// </summary>
        private void nextView_Click(object sender, RoutedEventArgs e)
        {
            MapControl.NextView();
        }

        /// <summary>
        /// 固定比例放大
        /// </summary>
        private void fastenZoonIn_Click(object sender, RoutedEventArgs e)
        {
            double scale = MapControl.Scale - scaleRation;
            if (scale < tolerance)
                MapControl.Scale = tolerance;
            else
                MapControl.Scale = scale;
        }

        /// <summary>
        /// 固定比例缩小
        /// </summary>
        private void fastenZoonOut_Click(object sender, RoutedEventArgs e)
        {
            MapControl.Scale += scaleRation;
        }

        /// <summary>
        /// 修角
        /// </summary>
        private void angleClipped_Click(object sender, RoutedEventArgs e)
        {
            MapControl.Action = new MapControlActionQueryMapClipAngleByLine(MapControl);
        }

        /// <summary>
        /// 设置分割
        /// </summary>
        private void landClippedByMode_Click(object sender, RoutedEventArgs e)
        {
            MapControl.Action = new MapControlActionQueryMapClipBySetting(MapControl);
        }

        /// <summary>
        /// 测量面积
        /// </summary>
        private void mtbMeasureArea_Click(object sender, RoutedEventArgs e)
        {
            MapControl.Action = new MapControlActionMeasureArea(MapControl);
        }

        /// <summary>
        /// 测量长度
        /// </summary>
        private void mtbMeasureLength_Click(object sender, RoutedEventArgs e)
        {
            MapControl.Action = new MapControlActionMeasureLength(MapControl);
        }

        #endregion
    }
}
