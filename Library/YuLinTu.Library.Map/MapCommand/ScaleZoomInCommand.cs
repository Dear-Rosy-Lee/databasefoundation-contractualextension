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
    /// 地图自定义命令：比例放大
    /// </summary>
    public class ScaleZoomInCommand
    {
        #region Fields

        MapControl mapControl;

        #endregion

        #region Ctor

        public ScaleZoomInCommand(MapControl map)
        {
            mapControl = map;
        }

        #endregion

        #region Methods

        #region Methods - Private

        /// <summary>
        /// 命令启动
        /// </summary>
        public void Run()
        {
            Envelope envelope = mapControl.Extend.Clone() as Envelope;
            envelope.MinX += (mapControl.Extend.MaxX - mapControl.Extend.MinX) / 4;
            envelope.MinY += (mapControl.Extend.MaxY - mapControl.Extend.MinY) / 4;
            envelope.MaxX -= (mapControl.Extend.MaxX - mapControl.Extend.MinX) / 4;
            envelope.MaxY -= (mapControl.Extend.MaxY - mapControl.Extend.MinY) / 4;
            mapControl.ZoomTo(envelope);        
        }

        
        #endregion


        #endregion
    }
}
