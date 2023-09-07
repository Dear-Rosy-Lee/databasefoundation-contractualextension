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
    /// 定位导航地图命令类
    /// </summary>
    public class LocatorComand
    {
        #region Field


        /// <summary>
        /// 地图控件
        /// </summary>
        private MapControl mapControl;

        private GraphicsLayer graphicsLayer;

        #endregion

        #region Ctor


        public LocatorComand(MapControl mc)
        {
            mapControl = mc;

        }

        #endregion

        #region Properties


        #endregion

        #region Method--Public

        /// <summary>
        /// 定位到指定坐标点 同时在地图上显示这个点
        /// </summary>
        public void Run()
        {
            LocationParameterDialog lpd=new LocationParameterDialog();
            if(lpd.ShowDialog()==System.Windows.Forms.DialogResult.Cancel)
            {
                return;
            }
            Coordinate cd=new Coordinate(lpd.X,lpd.Y);
            Geometry point = Geometry.CreatePoint(cd, mapControl.SpatialReference.WKID);

            graphicsLayer = new GraphicsLayer();
            graphicsLayer.SpatialReference = mapControl.SpatialReference;
            Graphic g = new Graphic();
            g.Geometry = point;
            g.Symbol = Application.Current.TryFindResource(UserMapActionConst.DefaultUISymbol_Mark) as MarkSymbol;
            graphicsLayer.Graphics.Add(g);
            mapControl.Layers.Add(graphicsLayer);            
            mapControl.NavigateTo(point);
        }


        #endregion

        #region Method--Private

        #endregion
    }
}
