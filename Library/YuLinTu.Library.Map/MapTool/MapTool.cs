using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YuLinTu.tGIS.Client;

namespace YuLinTu.Library.Map
{
    /// <summary>
    /// 自定义地图命令类
    /// </summary>
    public class MapTool : MapControlAction
    {
        #region Properties


        /// <summary>
        /// 地图自定义命令：缩小
        /// </summary>
        public static ZoomInTool MapZoomIn
        {
            get
            {
                return new ZoomInTool(null);
            }
        }

        /// <summary>
        /// 地图自定义命令：放大
        /// </summary>
        public static ZoomOutTool MapZoomOut
        {
            get
            {
                return new ZoomOutTool(null);
            }
        }

        /// <summary>
        /// 地图自定义命令：长度量算（公米）
        /// </summary>
        public static LenghCalculateTool MapLenghCalculate
        {
            get
            {
                return new LenghCalculateTool(null);
            }
        }

        /// <summary>
        /// 地图自定义命令：面积量算（平方米、亩）
        /// </summary>
        public static AreaCalculateTool MapAreaCalculate
        {
            get
            {
                return new AreaCalculateTool(null);
            }
        }

        /// <summary>
        /// 地图自定义命令：定位导航
        /// </summary>
        public static LocatorTool MapLocator
        {
            get
            {
                return new LocatorTool(null);
            }
        }

        #endregion

        #region Ctor

        public MapTool(MapControl map)
            : base(map)
        {
        }

        #endregion

        #region Methods

        #region Methods - Public
                
        #endregion

        #region Methods - Override

        #endregion

        #endregion
        
    }
}
