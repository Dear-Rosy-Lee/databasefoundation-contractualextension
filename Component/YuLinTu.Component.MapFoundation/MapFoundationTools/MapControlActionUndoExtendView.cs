/*
 * (C) 2015  鱼鳞图公司版权所有,保留所有权利
 * 地图视图后退
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

namespace YuLinTu.Component.MapFoundation
{
   public class MapControlActionUndoExtendView:MapControlAction
    {
       public List<Envelope> viewExtendList;
       
       /// <summary>
       /// 构造函数，在开始时获取地图范围列表
       /// </summary>       
        public MapControlActionUndoExtendView(MapControl map)
            : base(map)
        {
            viewExtendList = new List<Envelope>();
        }
        /// <summary>
        /// 与已有的地图范围进行比较，获取不重复的视图范围列表
        /// </summary>        
        public void map_ExtendChanged(object sender, ExtendChangedEventArgs e)
        {              
            if (MapControl.Layers.Count == 0) return;

            if (double.IsNaN(e.Value.MaxX) && double.IsNaN(e.Value.MaxY)
                && double.IsNaN(e.Value.MinX) && double.IsNaN(e.Value.MinY))
            {
                return;
            }
            else if(viewExtendList.Count == 0)
            {
                viewExtendList.Add(e.Value);
            }

            //已经与已经有的视图进行判断，如果有重复的，则不添加进入
            if (viewExtendList.Count >= 1) 
            {
               if (MapControl.Extend != null) 
                 {
                   viewExtendList.Add(e.Value);

                   if (viewExtendList[viewExtendList.Count - 2].MaxX == e.Value.MaxX &&
                      viewExtendList[viewExtendList.Count - 2].MaxY == e.Value.MaxY &&
                      viewExtendList[viewExtendList.Count - 2].MinX == e.Value.MinX &&
                      viewExtendList[viewExtendList.Count - 2].MinY == e.Value.MinY &&
                      viewExtendList[viewExtendList.Count - 2].SpatialReference.Equals(e.Value.SpatialReference))
                   {
                       viewExtendList.Remove(viewExtendList[viewExtendList.Count - 1]);
                   }                  

                 }            
              }         
        }
       
        /// <summary>
        /// 返回上一视图
        /// </summary>       
        public void map_ActionUndoExtendView(Envelope backExtend) 
        {
            MapControl.Extend = backExtend;
            MapControl.Refresh();
            MapControl.ExtendChanged += this.map_ExtendChanged;
        }
    }
}
