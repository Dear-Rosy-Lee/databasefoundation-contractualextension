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
    /// 地图自定义命令：比例缩小
    /// </summary>
    public class ScaleZoomOutCommand
    {
        #region Fields

        MapControl mapControl;

        #endregion

        #region Ctor

        public ScaleZoomOutCommand(MapControl map)
        {
            mapControl = map;
        }

        #endregion

        #region Methods

        #region Methods - public

        /// <summary>
        /// 命令启动
        /// </summary>
        public void Run()
        {
            Envelope envelope = mapControl.Extend.Clone() as Envelope;
            double xyRatio = (mapControl.FullExtend.MaxX - mapControl.FullExtend.MinX) / (mapControl.FullExtend.MaxY - mapControl.FullExtend.MinY);
            envelope.MinX -= (mapControl.Extend.MaxX - mapControl.Extend.MinX) / 2;
            envelope.MinY -= (mapControl.Extend.MaxY - mapControl.Extend.MinY) / 2;
            envelope.MaxX += (mapControl.Extend.MaxX - mapControl.Extend.MinX) / 2;
            envelope.MaxY += (mapControl.Extend.MaxY - mapControl.Extend.MinY) / 2;
            mapControl.ZoomTo(envelope);   
       
        }


        #endregion

        #endregion
    }
}
