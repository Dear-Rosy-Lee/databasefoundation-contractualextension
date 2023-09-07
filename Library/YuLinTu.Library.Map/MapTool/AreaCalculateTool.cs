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
    /// 地图自定义命令：面积量算（平方米、亩）
    /// 重要提示：未考虑地图坐标单位及其换算
    /// </summary>
    public class AreaCalculateTool : MapControlAction
    {
        #region Fields  
        
        private DrawPolygon drawPolygon = null;


        #endregion

        #region Ctor

        public AreaCalculateTool(MapControl map)
            : base(map)
        {
        }

        #endregion

        #region Methods

        /// <summary>
        /// 面积量算
        /// </summary>
        protected override void OnStartup()
        {
            if (MapControl.Layers.Count == 0)
            {
                return;
            }
            drawPolygon = new DrawPolygon(MapControl);
            drawPolygon.SnapMode = eSnapMode.None;
            drawPolygon.TrackingMouseMove = true;
            drawPolygon.FillSymbol = Application.Current.TryFindResource(YuLinTu.Library.Map.UserMapActionConst.DefaultUISymbol_Fill) as FillSymbol;
            drawPolygon.End += drawPolygon_End;
            drawPolygon.Activate();

        }

        
        protected override void OnShutdown()
        {
            drawPolygon.End -= drawPolygon_End;
            drawPolygon.Deactivate();
            drawPolygon = null;
        }

        #region Methods - Private

        void drawPolygon_End(object sender, EditGeometryEndEventArgs e)
        {
            if (e.Geometry == null)
            {
                return;
            }
            MessageBox.Show(string.Format("面积:{0}㎡\n面积:{1}亩", e.Geometry.Area().ToString(), (e.Geometry.Area() * 0.0015).ToString()), "量算结果");
        }


        #endregion


        #endregion
    }
}
