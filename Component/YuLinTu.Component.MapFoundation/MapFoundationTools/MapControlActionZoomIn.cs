/*
 * (C) 2015  鱼鳞图公司版权所有,保留所有权利
 * 拉框放大地图
 */
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using YuLinTu.Spatial;
using YuLinTu.tGIS;
using YuLinTu.tGIS.Client;
using YuLinTu.Windows.Wpf;
using YuLinTu.Windows.Wpf.Metro;

namespace YuLinTu.Component.MapFoundation
{
    public class MapControlActionZoomIn : MapControlAction
    {
        #region Properties
        
        #endregion

        #region Fields

        private DrawRectangle draw = null;

        #endregion

        #region Events

        #endregion

        #region Ctor

        public MapControlActionZoomIn(MapControl map)
            : base(map)
        {
        }

        #endregion

        #region Methods

        #region Methods - Override

        protected override void OnStartup()
        {
            var pWheelZoom = MapControl.GetPlugin<MapControlPluginWheelZoom>();
            if (pWheelZoom != null)
                pWheelZoom.Enabled = true;

            var pDragger = MapControl.GetPlugin<MapControlPluginDragger>();
            if (pDragger != null)
                pDragger.EnableMiddle(true);

            draw = new DrawRectangle(MapControl);
            draw.Begin += draw_Begin;
            draw.End += draw_End;
            draw.VertexAdded += draw_VertexAdded;

            draw.MarkSymbol = Application.Current.TryFindResource("UISymbol_Mark_Measure") as MarkSymbol;
            draw.LineSymbol = Application.Current.TryFindResource("UISymbol_Line_Measure") as LineSymbol;
            draw.FillSymbol = Application.Current.TryFindResource("UISymbol_Fill_Measure") as FillSymbol;
            draw.TrackingMouseMove = MapControl.TrackingMouseMoveWhenDraw;
            draw.SnapMode = eSnapMode.None;

            draw.Activate();

            MapControl.SnapModeChanged += MapControl_SnapModeChanged;
            MapControl.MouseMove += MapControl_MouseMove;
        }

        protected override void OnShutdown()
        {
            var pWheelZoom = MapControl.GetPlugin<MapControlPluginWheelZoom>();
            if (pWheelZoom != null)
                pWheelZoom.Enabled = false;

            var pDragger = MapControl.GetPlugin<MapControlPluginDragger>();
            if (pDragger != null)
                pDragger.EnableMiddle(false);

            MapControl.SnapModeChanged -= MapControl_SnapModeChanged;
            MapControl.MouseMove -= MapControl_MouseMove;

            draw.Deactivate();
            draw.Begin -= draw_Begin;
            draw.End -= draw_End;
            draw.VertexAdded -= draw_VertexAdded;

            draw = null;          

        }

        #endregion

        #region Methods - Events

        private void MapControl_SnapModeChanged(object sender, EventArgs e)
        {
            //if (draw == null)
            //    return;

            //draw.SnapMode = MapControl.SnapMode;
        }

        //绘制完成后放大到当前范围
        private void draw_End(object sender, EditGeometryEndEventArgs e)
        {
            //放大            
             var geo = e.Geometry;
             MapControl.ZoomTo(geo); 
        }

        private void draw_Begin(object sender, EditGeometryBeginEventArgs e)
        {
            
        }

        private void draw_VertexAdded(object sender, EditGeometryVertexEventArgs e)
        {

        }

        private void MapControl_MouseMove(object sender, MouseEventArgs e)
        {

        }

        #endregion

        #region Methods - Private

        #endregion

        #endregion
    }
}
