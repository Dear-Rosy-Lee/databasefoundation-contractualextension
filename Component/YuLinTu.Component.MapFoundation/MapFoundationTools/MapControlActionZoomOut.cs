/*
 * (C) 2015  鱼鳞图公司版权所有,保留所有权利
 * 拉框缩小地图
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
    public class MapControlActionZoomOut : MapControlAction
    {
        #region Properties    

        #endregion

        #region Fields

        private DrawRectangle draw = null;

        #endregion

        #region Events

        #endregion

        #region Ctor

        public MapControlActionZoomOut(MapControl map):base(map)
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
            var drawGeoExtend = e.Geometry.GetEnvelope();
            if (drawGeoExtend == null) return;
            if (drawGeoExtend.Width == 0 || drawGeoExtend.Height == 0) return;
            var mapExtend = MapControl.Extend;
            if (mapExtend == null) return;

            var dWidth = mapExtend.Width * mapExtend.Width / drawGeoExtend.Width;
            var dHeight = mapExtend.Height * mapExtend.Height / drawGeoExtend.Height;

            var dXmin = mapExtend.MinX - ((drawGeoExtend.MinX - mapExtend.MinX) * mapExtend.Width / drawGeoExtend.Width);
            var dYmin = mapExtend.MinY - ((drawGeoExtend.MinY - mapExtend.MinY) * mapExtend.Height / drawGeoExtend.Height);
            var dXmax = dXmin + dWidth;
            var dYmax = dYmin + dHeight;

            var extendlist = new List<Coordinate>();
            extendlist.Add(new Coordinate(dXmin,dYmin));
            extendlist.Add(new Coordinate(dXmin,dYmax));
            extendlist.Add(new Coordinate(dXmax,dYmax)); 
            extendlist.Add(new Coordinate(dXmax,dYmin));                       
         
            var geo = YuLinTu.Spatial.Geometry.CreatePolygon(extendlist,MapControl.SpatialReference);            
            MapControl.ZoomTo(geo);
            MapControl.Refresh();           
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
