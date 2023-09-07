using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;
using YuLinTu.Spatial;
using YuLinTu.tGIS.Client;

namespace YuLinTu.Library.Map
{
    /// <summary>
    /// 定位导航地图工具
    /// </summary>
    public class LocatorTool : MapControlAction
    {
        #region Field

        private GraphicsLayer graphicsLayer;

        #endregion

        #region Ctor


        public LocatorTool(MapControl map)
            : base(map)
        {            
        }

        #endregion

        #region Properties


        #endregion

        #region Method--Public

        /// <summary>
        /// 定位到指定坐标点 同时在地图上显示这个点
        /// </summary>
        protected override void OnStartup()
        {
            LocationParameterDialog lpd=new LocationParameterDialog();
            lpd.FullExtend = MapControl.FullExtend;
            if(lpd.ShowDialog()==System.Windows.Forms.DialogResult.Cancel)
            {
                return;
            }
            Coordinate cd=new Coordinate(lpd.X,lpd.Y);
            Geometry point = Geometry.CreatePoint(cd, MapControl.SpatialReference.WKID);

            graphicsLayer = new GraphicsLayer();
            graphicsLayer.Name = "LocatorResult";
            graphicsLayer.SpatialReference = MapControl.SpatialReference;
            Graphic g = new Graphic();
            g.Geometry = point;
            g.Symbol = Application.Current.TryFindResource(UserMapActionConst.DefaultUISymbol_Mark) as MarkSymbol;
            graphicsLayer.Graphics.Add(g);
            MapControl.Layers.Add(graphicsLayer);
        }

        protected override void OnShutdown()
        {
            MapControl.Layers.Remove(graphicsLayer);
        }

        #endregion

        #region Method--Private

        #endregion
    }
}
