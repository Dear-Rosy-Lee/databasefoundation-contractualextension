/*
 * (C) 2015  鱼鳞图公司版权所有,保留所有权利
 * 添加地图坐标状态栏
 */
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using YuLinTu.tGIS.Client;
using YuLinTu.Spatial;

namespace YuLinTu.Component.MapFoundation
{
    /// <summary>
    /// MapCoordinateStatusbarShell.xaml 的交互逻辑
    /// </summary>
    public partial class MapCoordinateStatusbarShell : UserControl
    {
        #region Properties

        public MapControl Map
        {
            get { return (MapControl)GetValue(MapProperty); }
            set { SetValue(MapProperty, value); }
        }

        public static readonly DependencyProperty MapProperty =
            DependencyProperty.Register("Map", typeof(MapControl), typeof(MapCoordinateStatusbarShell));

        private string projectionUnit = "Unkown";


        #endregion

        #region Ctor

        public MapCoordinateStatusbarShell(MapControl mapControl)
        {
            InitializeComponent();

            DataContext = this;
            Map = mapControl;
            RefreshMapControlSpatialUnit();
        }

        #endregion

        /// <summary>
        /// 注册鼠标移动显示坐标方法
        /// </summary>
        public void OnInstallMouseMoveDisplayCoordinate()
        {
            Map.SpatialReferenceChanged += Map_SpatialReferenceChanged;

            Map.MouseMove += MapControl_MouseMove;
        }

        /// <summary>
        /// 地图投影改变刷新
        /// </summary>       
        private void Map_SpatialReferenceChanged(object sender, EventArgs e)
        {
            RefreshMapControlSpatialUnit();
        }

        /// <summary>
        /// 注销方法
        /// </summary>
        public void OnUninstallMouseMoveDisplayCoordinate()
        {
            Map.SpatialReferenceChanged -= Map_SpatialReferenceChanged;
            Map.MouseMove -= MapControl_MouseMove;
        }

        /// <summary>
        /// 坐标系发生改变时获取坐标单位，具体实现
        /// </summary>
        private void RefreshMapControlSpatialUnit()
        {
            try
            {
                if (Map.SpatialReference.IsPROJCS())
                {
                    var projectionInfo = YuLinTu.Spatial.SpatialReferences.CreateProjectionInfo(Map.SpatialReference);

                    if (projectionInfo == null) return;
                    switch (projectionInfo.Unit.Name)
                    {
                        case "Kilometer":
                            projectionUnit = "千米";
                            break;
                        case "Meter":
                            projectionUnit = "米";
                            break;
                        case "Decimeter":
                            projectionUnit = "分米";
                            break;
                        case "Centimeter":
                            projectionUnit = "厘米";
                            break;
                        case "Millimeter":
                            projectionUnit = "毫米";
                            break;
                        case "Mile":
                            projectionUnit = "英里";
                            break;
                        case "Nautical Mile":
                            projectionUnit = "海哩";
                            break;
                        case "Foot":
                            projectionUnit = "英尺";
                            break;
                        case "Yard":
                            projectionUnit = "码";
                            break;
                        case "Inch":
                            projectionUnit = "英寸";
                            break;
                        default:
                            projectionUnit = "Unkown";
                            break;
                    }
                }
                else if (Map.SpatialReference.IsGEOGCS() || !Map.SpatialReference.IsValid())
                {
                    projectionUnit = "度";
                }
                else if (!Map.SpatialReference.IsValid())
                {
                    projectionUnit = "Unkown";
                }
            }
            catch
            {
                projectionUnit = "Unkown";
            }
        }

        private void Border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
        }

        private void Border_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
        }

        private void textBoxScale_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Return && e.Key != Key.Enter)
                return;

            btnDropDown.Focus();

        }

        /// <summary>
        /// 键盘按键回车，也可以定位
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Border_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                MapPanToCoordinate();
            }
        }

        #region Methods

        #region Methods - Events

        /// <summary>
        /// 定义鼠标移动事件，显示当前鼠标坐标
        /// </summary>       
        private void MapControl_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            var pt = Map.ScreenToMap(e).ToCoordinates()[0];
            textBlockScale.Text = string.Format("{0:F3} , {1:F3}  {2}", pt.X, pt.Y, projectionUnit);

            MouseX.Text = string.Format("{0:F3}", pt.X);
            MouseY.Text = string.Format("{0:F3}", pt.Y);
        }

        /// <summary>
        /// 点击跳转到地图相应位置
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MetroButton_Click(object sender, RoutedEventArgs e)
        {

            MapPanToCoordinate();
        }
        /// <summary>
        /// 地图定位到点
        /// </summary>
        private void MapPanToCoordinate()
        {
            if (MapX.Text == "" || MapY.Text == "") return;

            try
            {
                var x = double.Parse(MapX.Text.ToString());
                var y = double.Parse(MapY.Text.ToString());

                var pointCoordinate = new Coordinate(x, y);
                var geo = YuLinTu.Spatial.Geometry.CreatePoint(pointCoordinate, Map.SpatialReference);
                Map.PanTo(geo);
            }
            catch
            {
                return;
            }
        }

        #endregion

        /// <summary>
        /// 点击弹出框时，获取到中心点坐标
        /// </summary> 
        private void btnDropDown_Checked(object sender, RoutedEventArgs e)
        {

            if (Map.Extend == null) return;
            var extend = Map.Extend;
            Coordinate centerPoint = new Coordinate();
            centerPoint.X = extend.MinX + (extend.MaxX - extend.MinX) / 2;
            centerPoint.Y = extend.MinY + (extend.MaxY - extend.MinY) / 2;

            MapX.Text = string.Format("{0:F3}", centerPoint.X);
            MapY.Text = string.Format("{0:F3}", centerPoint.Y);
        }

        #endregion
    }
}
