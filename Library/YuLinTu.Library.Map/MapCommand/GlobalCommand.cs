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
    /// 地图自定义命令：全图
    /// </summary>
    public class GlobalComand
    {
        #region Fields  
           
        /// <summary>
        /// X最小值
        /// </summary>
        private double minX = 0;

        /// <summary>
        /// X最大值
        /// </summary>
        private double maxX = 0;

        /// <summary>
        /// Y最小值
        /// </summary>
        private double minY = 0;

        /// <summary>
        /// Y最大值
        /// </summary>
        private double maxY = 0;


        private MapControl mapControl;

        #endregion

        #region Ctor

        public GlobalComand(MapControl map)
        {
            mapControl = map;
        }

        #endregion

        #region Methods

        #region Methods - Private

        /// <summary>
        /// 视图导航至所有视图最大值
        /// </summary>
        public void Run()
        {
            if (mapControl.Layers.Count == 0)
            {
                return;
            }
            Envelope envelope = mapControl.FullExtend.Clone() as Envelope;
            //范围最小值以默认第一图层最小值赋值 如果是0 可能找不到最小值
            minX = mapControl.Layers[0].FullExtend.MinX;
            minY = mapControl.Layers[0].FullExtend.MinY;
            foreach (var item in mapControl.Layers)
            {
                if (item.FullExtend.MinX < minX)
                {
                    minX = item.FullExtend.MinX;
                }
                if (item.FullExtend.MaxX > maxX)
                {
                    maxX = item.FullExtend.MaxX;
                }
                if (item.FullExtend.MinY < minY)
                {
                    minY = item.FullExtend.MinY;
                }
                if (item.FullExtend.MinX > maxY)
                {
                    maxY = item.FullExtend.MaxY;
                }
            }

            envelope.MinX = minX;
            envelope.MaxX = maxX;
            envelope.MinY = minY;
            envelope.MaxY = maxY;

            mapControl.ZoomTo(envelope);
        }

        #endregion


        #endregion
    }
}
