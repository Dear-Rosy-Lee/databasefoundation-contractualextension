using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using YuLinTu.Spatial;
using YuLinTu.tGIS.Client;

namespace YuLinTu.Library.Map
{
    /// <summary>
    /// 地图自定义命令：拉框缩小当前视图范围到指定区域范围
    /// </summary>
    public class ZoomOutTool : MapControlAction
    {
        #region Fields  
         
        private DrawRectangle drawRectangle = null;

        #endregion

        #region Ctor

        public ZoomOutTool(MapControl map)
            : base(map)
        {
        }

        #endregion

        #region Methods

        #region Methods - Private

        /// <summary>
        /// 命令启动
        /// </summary>
        protected override void OnStartup()
        {
            drawRectangle = new DrawRectangle(MapControl);
            drawRectangle.SnapMode = eSnapMode.None;
            drawRectangle.LeftButtonEnabled = true;
            drawRectangle.RightButtonEnabled = false;
            drawRectangle.FillSymbol = Application.Current.TryFindResource(UserMapActionConst.DefaultUISymbol_Fill) as FillSymbol;
            drawRectangle.End += drawRectangle_End;
            drawRectangle.Activate();
        }


        /// <summary>
        /// 命令卸载时
        /// </summary>
        protected override void OnShutdown()
        {
            drawRectangle.End -= drawRectangle_End;
            drawRectangle.Deactivate();
            drawRectangle = null;
        }


        #endregion

        #region Event

        YuLinTu.Spatial.Geometry ge;
        private void drawRectangle_End(object sender, EditGeometryEndEventArgs e)
        {
            if (e.Geometry == null)
            {
                return;
            }
            double ratio = MapControl.Extend.Area() / e.Geometry.Area();
            Envelope envelope = MapControl.Extend.Clone() as Envelope;
            envelope.MinX -= (MapControl.Extend.MaxX - MapControl.Extend.MinX) / ratio;
            envelope.MinY -= (MapControl.Extend.MaxY - MapControl.Extend.MinY) / ratio;
            envelope.MaxX += (MapControl.Extend.MaxX - MapControl.Extend.MinX) / ratio;
            envelope.MaxY += (MapControl.Extend.MaxY - MapControl.Extend.MinY) / ratio;
            MapControl.ZoomTo(envelope);
            ge = e.Geometry;
        }

        #endregion


        #endregion
    }
}
