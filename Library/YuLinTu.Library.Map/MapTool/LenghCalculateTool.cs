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
    /// 地图自定义命令：长度量算（公米）
    /// 重要提示：未考虑地图坐标单位及其换算
    /// </summary>
    public class LenghCalculateTool : MapControlAction
    {

        #region Fields

        private DrawPolyline drawPolyline = null;

        #endregion

        #region Ctor

        public LenghCalculateTool(MapControl map)
            : base(map)
        {            
        }

        #endregion

        #region Methods

        /// <summary>
        /// 长度量算
        /// </summary>
        protected override void OnStartup()
        {
            if (MapControl.Layers.Count == 0)
            {
                return;
            }
            drawPolyline = new DrawPolyline(MapControl);
            drawPolyline.SnapMode = eSnapMode.None;
            drawPolyline.TrackingMouseMove = true;
            drawPolyline.LineSymbol = Application.Current.TryFindResource(UserMapActionConst.DefaultUISymbol_Line) as LineSymbol;
            drawPolyline.End += drawPolyline_End;
            drawPolyline.Activate();
        }


        protected override void OnShutdown()
        {
            drawPolyline.End -= drawPolyline_End;
            drawPolyline.Deactivate();
            drawPolyline = null;
        }
        #endregion

        #region Methods - Private

        void drawPolyline_End(object sender, EditGeometryEndEventArgs e)
        {
            if (e.Geometry == null)
            {
                return;
            }
            MessageBox.Show(string.Format("长度：{0}米", e.Geometry.Length().ToString()), "量算结果");
        }
        
        #endregion
    }
}
