using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YuLinTu.tGIS.Client;
using YuLinTu.tGIS.Data;

namespace YuLinTu.Library.Map
{
    /// <summary>
    /// 地图命令加载影像文件
    /// </summary>
    public class LoadImageComand
    {
        
        #region Field


        /// <summary>
        /// 地图控件
        /// </summary>
        private MapControl mapControl;

       

        #endregion

        #region Ctor


        public LoadImageComand(MapControl mc)
        {
            mapControl = mc;

        }

        #endregion

        #region Properties


        #endregion

        #region Method--Public

        /// <summary>
        /// 加载影像
        /// </summary>
        public void Run()
        {
            if (!mapControl.IsInitialized) 
            {
                mapControl.InitializeEnd += MapControl_InitializeEnd;
            }

            var dlg = new OpenFileDialog();
            dlg.Multiselect = true;
            dlg.Filter = "影像文件(*.img)|*.img";

            var val = dlg.ShowDialog();
            if (val == null || !val.Value)
            {
                return;
            }
            foreach (var item in dlg.FileNames)
            {
                var gs = new RasterSource(item);
                var layer = new RasterLayer(gs);

                layer.Renderer = new SimpleRasterRenderer(new RGBRasterSymbol());
                layer.Name = System.IO.Path.GetFileNameWithoutExtension(item);

                if (layer != null)
                {
                    mapControl.Layers.Add(layer);
                }
            }
        }

        private void MapControl_InitializeEnd(object sender, EventArgs e)
        {
            if (mapControl.Layers.Count == 0)
            {
                return;
            }

            foreach (var layer in mapControl.Layers)
            {
                if (!layer.IsInitialized)
                {
                    return;
                }
            }
            mapControl.NavigateTo(mapControl.Layers[mapControl.Layers.Count-1].FullExtend.ToGeometry().Buffer(10));
            mapControl.InitializeEnd -= MapControl_InitializeEnd;
        }


        #endregion

        #region Method--Private

        #endregion
    }
}
