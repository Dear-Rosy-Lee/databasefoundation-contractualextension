/*
 * (C) 2015  鱼鳞图公司版权所有,保留所有权利
 * 地图全图
 */
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using YuLinTu.Spatial;
using YuLinTu.tGIS;
using YuLinTu.tGIS.Client;
using YuLinTu.Windows.Wpf;
using YuLinTu.Windows.Wpf.Metro;

namespace YuLinTu.Component.ZoneInvestigation
{
    public class MapControlActionMapAllExtend : MapControlAction
    {      
        #region Ctor

        public MapControlActionMapAllExtend(MapControl map)
            : base(map)
        {
        }

        #endregion

        #region Methods
        /// <summary>
        /// 开始的时候就地图全图
        /// </summary>
        protected override void OnStartup()
        {
            MapControl.ZoomTo(MapControl.FullExtend);
        }    

        #endregion

    }
}
