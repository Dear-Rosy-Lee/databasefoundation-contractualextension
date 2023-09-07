using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Windows.Media;
using YuLinTu.Spatial;
using YuLinTu.tGIS.Client;
using YuLinTu.tGIS.Data;

namespace YuLinTu.Library.Map
{
    /// <summary>
    /// 图层管理命令
    /// </summary>
    public class LayerCommand
    {
        #region Fields

        #endregion

        #region Ctor

        #endregion

        #region Properties
        

        #endregion

        #region Method

        /// <summary>
        /// 添加数据至地图
        /// </summary>
        public static void AddData(MapControl mapControl)
        {
            #region
            var dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.Multiselect = false;
            dlg.Filter = "Shapefile(*.shp)|*.shp";

            var val = dlg.ShowDialog();
            if (val == null || !val.Value)
            {
                return;
            }

            foreach (var item in dlg.FileNames)
            {
                var gs = new ShapefileGeoSource(item);
                var layer = new VectorLayer(gs);
                var geoType = gs.GeometryType;

                switch (geoType)
                {
                    case eGeometryType.Point:
                        layer.Renderer = new SimpleRenderer(new SimplePointSymbol());
                        layer.Labeler = new SimpleLabeler(new SimpleTextPointSymbol());
                        break;
                    case eGeometryType.MultiPoint:
                        layer.Renderer = new SimpleRenderer(new SimplePointSymbol());
                        layer.Labeler = new SimpleLabeler(new SimpleTextPointSymbol());
                        break;
                    case eGeometryType.Polyline:
                        layer.Renderer = new SimpleRenderer(new SimplePolylineSymbol());
                        layer.Labeler = new SimpleLabeler(new SimpleTextPolylineSymbolPerFeaturePart());
                        break;
                    case eGeometryType.MultiPolyline:
                        layer.Renderer = new SimpleRenderer(new SimplePolylineSymbol());
                        layer.Labeler = new SimpleLabeler(new SimpleTextPolylineSymbolPerFeaturePart());
                        break;
                    case eGeometryType.Polygon:
                        layer.Renderer = new SimpleRenderer(new SimplePolygonSymbol()
                        {
                            BackgroundColor = Color.FromArgb(255, 234, 224, 189),
                            BorderStrokeColor = Color.FromArgb(255, 158, 148, 112),
                            BorderThickness = 1,
                        });
                        layer.Labeler = new SimpleLabeler(new SimpleTextPolygonSymbolPerFeaturePartInView());
                        break;
                    case eGeometryType.MultiPolygon:
                        layer.Renderer = new SimpleRenderer(new SimplePolygonSymbol()
                        {
                            BackgroundColor = Color.FromArgb(255, 234, 224, 189),
                            BorderStrokeColor = Color.FromArgb(255, 158, 148, 112),
                            BorderThickness = 1,
                        });
                        layer.Labeler = new SimpleLabeler(new SimpleTextPolygonSymbolPerFeaturePartInView());
                        break;
                    case eGeometryType.Unknown:
                        break;
                    default:
                        break;
                }
                layer.Name = layer.DataSource.ElementName;
                mapControl.Layers.Add(layer);
            }
            #endregion
        }

        /// <summary>
        /// 显示图层管理窗体
        /// </summary>
        public static void MannagerLayer()
        { 

        }

        /// <summary>
        /// 移除图层
        /// </summary>
        public static void RemoveLayer(MapControl mapControl,Layer layer) 
        {
            if (layer.Parent is MultiVectorLayer)
            {
                (layer.Parent as MultiVectorLayer).Layers.Remove(layer);
            }
            else
            {
                mapControl.Layers.Remove(layer);
            }
        }

        /// <summary>
        /// 缩放至图层
        /// </summary>
        public static void ZoomToLayer(MapControl mapControl, Layer layer)
        {
            mapControl.NavigateTo(layer.FullExtend);
        }


        /// <summary>
        /// 添加数据至图层组
        /// </summary>
        public static void GroupLayerAddData(Layer groupLayer)
        {
            #region
            var dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.Multiselect = false;
            dlg.Filter = "Shapefile(*.shp)|*.shp";

            var val = dlg.ShowDialog();
            if (val == null || !val.Value)
            {
                return;
            }

            foreach (var item in dlg.FileNames)
            {
                var gs = new ShapefileGeoSource(item);
                var layer = new VectorLayer(gs);
                var geoType = gs.GeometryType;

                switch (geoType)
                {
                    case eGeometryType.Point:
                        layer.Renderer = new SimpleRenderer(new SimplePointSymbol());
                        layer.Labeler = new SimpleLabeler(new SimpleTextPointSymbol());
                        break;
                    case eGeometryType.MultiPoint:
                        layer.Renderer = new SimpleRenderer(new SimplePointSymbol());
                        layer.Labeler = new SimpleLabeler(new SimpleTextPointSymbol());
                        break;
                    case eGeometryType.Polyline:
                        layer.Renderer = new SimpleRenderer(new SimplePolylineSymbol());
                        layer.Labeler = new SimpleLabeler(new SimpleTextPolylineSymbolPerFeaturePart());
                        break;
                    case eGeometryType.MultiPolyline:
                        layer.Renderer = new SimpleRenderer(new SimplePolylineSymbol());
                        layer.Labeler = new SimpleLabeler(new SimpleTextPolylineSymbolPerFeaturePart());
                        break;
                    case eGeometryType.Polygon:
                        layer.Renderer = new SimpleRenderer(new SimplePolygonSymbol()
                        {
                            BackgroundColor = Color.FromArgb(255, 234, 224, 189),
                            BorderStrokeColor = Color.FromArgb(255, 158, 148, 112),
                            BorderThickness = 1,
                        });
                        layer.Labeler = new SimpleLabeler(new SimpleTextPolygonSymbolPerFeaturePartInView());
                        break;
                    case eGeometryType.MultiPolygon:
                        layer.Renderer = new SimpleRenderer(new SimplePolygonSymbol()
                        {
                            BackgroundColor = Color.FromArgb(255, 234, 224, 189),
                            BorderStrokeColor = Color.FromArgb(255, 158, 148, 112),
                            BorderThickness = 1,
                        });
                        layer.Labeler = new SimpleLabeler(new SimpleTextPolygonSymbolPerFeaturePartInView());
                        break;
                    case eGeometryType.Unknown:
                        break;
                    default:
                        break;
                }
                layer.Name = layer.DataSource.ElementName;

                if (groupLayer is IGroupLayer)
                {
                    (groupLayer as IGroupLayer).Layers.Add(layer);
                }
            }
            #endregion
        }

        /// <summary>
        /// 图层组图层全部显示
        /// </summary>
        public static void GroupLayerVisiable(Layer groupLayer)
        {
            if (groupLayer is IGroupLayer)
            {
                foreach (var item in (groupLayer as IGroupLayer).Layers)
                {
                    item.Visible = true;
                }
            }
        }

        /// <summary>
        /// 图层组图层全部隐藏
        /// </summary>
        public static void GroupLayerHide(Layer groupLayer)
        {
            if (groupLayer is IGroupLayer)
            {
                foreach (var item in (groupLayer as IGroupLayer).Layers)
                {
                    item.Visible = false;
                }
            }
        }

        /// <summary>
        /// 移除图层组
        /// </summary>
        public static void GroupLayerRemove(MapControl mapControl, Layer groupLayer)
        {
            if (groupLayer is MultiVectorLayer)
            {
                mapControl.Layers.Remove(groupLayer);
            }
            if (groupLayer is GroupLayer)
            {
                mapControl.Layers.Remove(groupLayer);
            }
        }

        /// <summary>
        /// 图层全部显示
        /// </summary>
        public static void LayerGroupsVisible(MapControl mapControl)
        {
            foreach (var item in mapControl.Layers)
            {
                item.Visible = true;
            }
        }

        /// <summary>
        /// 图层全部隐藏
        /// </summary>
        public static void LayerGroupsHide(MapControl mapControl)
        {
            foreach (var item in mapControl.Layers)
            {
                item.Visible = false;
            }
        }

        #endregion

        /// <summary>
        /// 添加图层组
        /// </summary>
        public static void LayerGroupNew(MapControl mapControl)
        {
            MultiVectorLayer newGroupLayer = new MultiVectorLayer();
            GroupLayerNewDialog newGroupLayerDialog = new GroupLayerNewDialog();

            double minScale = 0;
            double maxScale = 0;
            if (mapControl.Layers.Count > 0) 
            {
                minScale = mapControl.Layers[0].MinimizeScale;
                foreach (var item in mapControl.Layers)
                {
                    if (item.MinimizeScale < minScale)
                    {
                        minScale = item.MinimizeScale;
                    }
                    if (item.MaximizeScale > maxScale)
                    {
                        maxScale = item.MaximizeScale;
                    }
                }
            }
            newGroupLayerDialog.MinScale = minScale;
            newGroupLayerDialog.MaxScale = maxScale;
            if (newGroupLayerDialog.ShowDialog() == DialogResult.Cancel) 
            {
                return;
            }
            newGroupLayer.Name = newGroupLayerDialog.LayerGroupName;
            newGroupLayer.MinimizeScale = newGroupLayerDialog.MinScale;
            newGroupLayer.MaximizeScale = newGroupLayerDialog.MaxScale;
            newGroupLayer.Visible = newGroupLayerDialog.LayerGroupVisiable;
            mapControl.Layers.Add(newGroupLayer);
        }


        /// <summary>
        /// 图层比例尺设置
        /// </summary>
        public static void LayerScaleSet(Layer layer)
        {
            double minScale = 0;
            double maxScale = 0;
            minScale = layer.MinimizeScale;
            maxScale = layer.MaximizeScale;

            LayerScaleSetDialog layerScaleSetDialog = new LayerScaleSetDialog();
            layerScaleSetDialog.MinScale = minScale;
            layerScaleSetDialog.MaxScale = maxScale;
            if (layerScaleSetDialog.ShowDialog() == DialogResult.Cancel) 
            {
                return;
            }
            layer.MinimizeScale = layerScaleSetDialog.MinScale;
            layer.MaximizeScale = layerScaleSetDialog.MaxScale;
        }

    }
}
