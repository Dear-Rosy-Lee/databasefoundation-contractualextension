using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YuLinTu.tGIS.Client;

namespace YuLinTu.Library.Map
{
    /// <summary>
    /// 地图命令
    /// </summary>
    public class MapCommand
    {
        #region Fields

        /// <summary>
        /// 地图控件
        /// </summary>
        private MapControl mapControl;


        #endregion

        #region Method
        
        /// <summary>
        /// 地图命令：加载影像
        /// </summary>
        public void LoadImage()
        {
            var comand = new LoadImageComand(mapControl);
            comand.Run();
        }

        /// <summary>
        /// 地图命令：全图
        /// </summary>
        public void Global()
        {
            var comand = new GlobalComand(mapControl);
            comand.Run();
        }

        /// <summary>
        /// 地图命令：固定缩小
        /// </summary>
        public void ScaleZoomOut()
        {
            var comand = new ScaleZoomOutCommand(mapControl);
            comand.Run();
        }

        /// <summary>
        /// 地图命令：固定放大
        /// </summary>
        public void ScaleZoomIn()
        {
            var comand = new ScaleZoomInCommand(mapControl);
            comand.Run();
        }

        #endregion

        #region Ctor

        public MapCommand(MapControl mc) 
        {
            mapControl = mc;
        }
        
        #endregion

    }
}
