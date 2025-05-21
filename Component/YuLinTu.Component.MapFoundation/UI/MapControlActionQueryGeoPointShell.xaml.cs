/*
 * (C) 2025  鱼鳞图公司版权所有,保留所有权利
 * 实现跳转与显示
 */
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
using YuLinTu.Spatial;
using YuLinTu.tGIS.Client;
using YuLinTu.Windows.Wpf;
using YuLinTu.Windows.Wpf.Metro.Components;

namespace YuLinTu.Component.MapFoundation
{
    /// <summary>
    /// MapControlActionQueryGeoPointShell.xaml 的交互逻辑
    /// </summary>
    public partial class MapControlActionQueryGeoPointShell : TabDialog
    {
        public MapControl map;       
        private Graphic graphicEndDisplay = null;
        public GraphicsLayer layerEndDisplay = new GraphicsLayer();

        public MapControlActionQueryGeoPointShell()
        {
            InitializeComponent();                          
        }
        /// <summary>
        /// 注册方法
        /// </summary>
        public void OnInstallDisplayCoordinate()
        {          
          dgQueryGeoPoint.MouseDoubleClick += dgQueryGeoPoint_MouseDoubleClick;
          dgQueryGeoPoint.SelectionChanged += dgQueryGeoPoint_SelectionChanged;          
        }

        /// <summary>
        /// 注销方法
        /// </summary>
        public void OnUninstallDisplayCoordinate()
        {
            dgQueryGeoPoint.MouseDoubleClick -= dgQueryGeoPoint_MouseDoubleClick;
            dgQueryGeoPoint.SelectionChanged -= dgQueryGeoPoint_SelectionChanged;
        }

        /// <summary>
        /// 单击为高亮显示
        /// </summary> 
        private void dgQueryGeoPoint_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
           
            layerEndDisplay.Graphics.Clear();

            if (sender == null) return;
            Coordinate selectpoint = getCoordinate(sender);
            if (selectpoint == null) return;
            var point = YuLinTu.Spatial.Geometry.CreatePoint(selectpoint, map.SpatialReference);            
        
            graphicEndDisplay = new Graphic();
            graphicEndDisplay.Geometry = point;
            graphicEndDisplay.Symbol = Application.Current.TryFindResource("UISymbol_Mark_Measure") as UISymbol;
            layerEndDisplay.Graphics.Add(graphicEndDisplay);
        }

        /// <summary>
        /// 双击跳转，跳转到对应的地图视图
        /// </summary>       
        private void dgQueryGeoPoint_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            
            if (sender == null) return;
            Coordinate selectpoint = getCoordinate(sender);
            if (selectpoint == null) return;
            var point = YuLinTu.Spatial.Geometry.CreatePoint(selectpoint,map.SpatialReference);
            map.PanTo(point);         
        }

        /// <summary>
        /// 获取点击列表后的坐标
        /// </summary>
        /// <param name="sender"></param>
        /// <returns></returns>
        private Coordinate getCoordinate(object sender) 
        {
            var dgcont = sender as PagableDataGrid;
            if (dgcont == null) return null;

            var selitem = dgcont.SelectedItem as GeoPointUI;
            if (selitem == null) return null;

            var selectpointX = selitem.XCoordinate;
            var selectpointY = selitem.YCoordinate;

            if (double.IsNaN(selectpointX) || double.IsNaN(selectpointY)) return new Coordinate(0, 0);

            Coordinate selectpoint = new Coordinate(selectpointX, selectpointY);
            return selectpoint;
        }     

    }
}
