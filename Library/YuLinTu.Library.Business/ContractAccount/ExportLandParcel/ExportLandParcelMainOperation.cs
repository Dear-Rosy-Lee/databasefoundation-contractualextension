using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using YuLinTu.NetAux.CglLib;
using YuLinTu.NetAux;
using YuLinTu.Diagrams;
using System.Windows.Media;
using YuLinTu.Library.Entity;
using YuLinTu.Spatial;
using YuLinTu.tGIS.Data;
using YuLinTu.tGIS;
using YuLinTu.tGIS.Client;

namespace YuLinTu.Library.Business
{
    /// <summary>
    /// 地块示意图主要的方法，与设置挂钩
    /// </summary>
    public class ExportLandParcelMainOperation
    {
        #region private

        private ContractBusinessParcelWordSettingDefine SettingDefine = ContractBusinessParcelWordSettingDefine.GetIntence();

        #endregion private

        public SpatialReference targetSpatialReference { set; get; }

        /// <summary>
        /// 线状地物(当前地域)
        /// </summary>
        public List<XZDW> ListLineFeature { get; set; }

        /// <summary>
        /// 点状地物(当前地域)
        /// </summary>
        public List<DZDW> ListPointFeature { get; set; }

        /// <summary>
        /// 面状地物(当前地域)
        /// </summary>
        public List<MZDW> ListPolygonFeature { get; set; }

        /// <summary>
        /// 逻辑出图的范围宽
        /// </summary>
        public double mapW { set; get; }

        /// <summary>
        /// 逻辑出图的范围高
        /// </summary>
        public double mapH { set; get; }

        public double Scale { get; set; }

        /// <summary>
        /// 当前地块过滤好的界址点集合
        /// </summary>
        public List<YuLinTu.Spatial.Geometry> FilterjzdNodes { set; get; }

        public ExportLandParcelMainOperation()
        {
        }

        #region method

        #region 常规添加图形

        /// <summary>
        /// 创建临宗-邻宗图形集合
        /// </summary>
        /// <param name="listOwenrFeature"></param>
        /// <param name="map"></param>
        public void GetSetNeighborlandGeos(List<ContractLand> tempLands, YuLinTu.Spatial.Geometry geolandbuffershape, List<FeatureObject> listFeature, MapShapeUI map, int layerCountIndex)
        {
            foreach (var ld in tempLands)
            {
                var templines = ld.Shape.ToSegmentLines();
                foreach (var itemline in templines)
                {
                    if (itemline.Intersects(geolandbuffershape))
                    {
                        FeatureObject fo = new FeatureObject();
                        fo.Object = ld;
                        fo.Geometry = itemline;
                        fo.GeometryPropertyName = "Shape";
                        listFeature.Add(fo);
                    }
                }
            }
            DynamicGeoSource otherGeos = null;   //邻宗数据源
            VectorLayer lyer = null;  //创建邻宗矢量图层
            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                if (map.MapControl.Layers.Count == layerCountIndex)
                {
                    lyer = new VectorLayer();  //创建邻宗矢量图层
                    lyer.DataProcessingMethod = eDataProcessingMethod.Synchronous;
                    lyer.Renderer = new SimpleRenderer(new SimplePolylineSymbol()
                    {
                        StrokeColor = Color.FromArgb(255, 255, 0, 0),
                        StrokeThickness = 1
                    });
                    otherGeos = new DynamicGeoSource();
                    otherGeos.AddRange(listFeature.ToArray());
                    lyer.DataSource = otherGeos;
                    map.MapControl.Layers.Add(lyer);
                }
                else
                {
                    lyer = map.MapControl.Layers[layerCountIndex] as VectorLayer;
                    otherGeos = lyer.DataSource as DynamicGeoSource;
                    otherGeos.Clear();
                    otherGeos.AddRange(listFeature.ToArray());
                }

                var spls = (lyer.Renderer as SimpleRenderer).Symbol as SimplePolylineSymbol;
                if (spls != null)
                {
                    spls.StrokeColor = SettingDefine.NeighborLandColor;
                    spls.StrokeThickness = SettingDefine.NeighborLandBorderThickness;
                }
            }));
        }

        /// <summary>
        /// 创建临宗-本宗图形
        /// </summary>
        /// <param name="listOwenrFeature"></param>
        /// <param name="map"></param>
        public DynamicGeoSource GetSetNeighborlandGeo(ContractLand geoLand, DynamicGeoSource selfGeods, MapShapeUI map, int layerCountIndex, bool isEditUse)
        {
            ////创建本宗图层
            var fot = new FeatureObject();
            fot.Object = geoLand;
            fot.Geometry = geoLand.Shape;
            fot.GeometryPropertyName = "Shape";

            VectorLayer lyerOwner = null;     //创建本宗矢量图层
            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                if (map.MapControl.Layers.Count == layerCountIndex)
                {
                    lyerOwner = new VectorLayer();   //创建本宗矢量图层
                    lyerOwner.DataProcessingMethod = eDataProcessingMethod.Synchronous;
                    lyerOwner.Renderer = new SimpleRenderer(new SimplePolygonSymbol()
                    {
                        BackgroundColor = Colors.Transparent,
                        BorderStrokeColor = Color.FromArgb(255, 255, 0, 0),
                        BorderThickness = 1
                    });
                    selfGeods = new DynamicGeoSource();
                    selfGeods.Add(fot);
                    lyerOwner.DataSource = selfGeods;
                    map.MapControl.Layers.Add(lyerOwner);
                }
                else
                {
                    lyerOwner = map.MapControl.Layers[layerCountIndex] as VectorLayer;
                    selfGeods = lyerOwner.DataSource as DynamicGeoSource;
                    selfGeods.Clear();
                    selfGeods.Add(fot);
                }

                ((lyerOwner.Renderer as SimpleRenderer).Symbol as SimplePolygonSymbol).BorderStrokeColor = SettingDefine.OwnerLandColor;
                ((lyerOwner.Renderer as SimpleRenderer).Symbol as SimplePolygonSymbol).BorderThickness = SettingDefine.OwnerLandBorderThickness;
            }));
            return selfGeods;
        }

        /// <summary>
        /// 创建点线面状地物要素的图形
        /// </summary>
        /// <param name="listFeature"></param>
        /// <param name="tempPoints"></param>
        /// <param name="tempLines"></param>
        /// <param name="tempPolygons"></param>
        /// <param name="map"></param>
        public void GetSetALLdxmzdwGeos(List<FeatureObject> listFeature, List<DZDW> tempPoints, List<XZDW> tempLines, List<MZDW> tempPolygons, MapShapeUI map, int layerCountIndex)
        {
            foreach (var point in tempPoints)
            {
                FeatureObject fo = new FeatureObject();
                fo.Object = point;
                fo.Geometry = point.Shape;
                fo.GeometryPropertyName = "Shape";
                listFeature.Add(fo);
            }
            DynamicGeoSource pointGeos = null;   //点状地物数据源
            VectorLayer pointLayer = null;  //创建点状地物矢量图层
            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                if (map.MapControl.Layers.Count == layerCountIndex)
                {
                    pointLayer = new VectorLayer();  //创建点状地物矢量图层
                    pointLayer.DataProcessingMethod = eDataProcessingMethod.Synchronous;
                    pointLayer.Renderer = new SimpleRenderer(new SimplePointSymbol()
                    {
                        BackgroundColor = Colors.Transparent,
                        BorderStrokeColor = Color.FromArgb(255, 255, 0, 0),
                        BorderThickness = 1,
                    });

                    pointGeos = new DynamicGeoSource();
                    pointGeos.AddRange(listFeature.ToArray());
                    pointLayer.DataSource = pointGeos;
                    map.MapControl.Layers.Add(pointLayer);
                }
                else
                {
                    pointLayer = map.MapControl.Layers[layerCountIndex] as VectorLayer;
                    pointGeos = pointLayer.DataSource as DynamicGeoSource;
                    pointGeos.Clear();
                    pointGeos.AddRange(listFeature.ToArray());
                }
            }));
            listFeature.Clear();
            layerCountIndex++;

            foreach (var line in tempLines)
            {
                FeatureObject fo = new FeatureObject();
                fo.Object = line;
                fo.Geometry = line.Shape;
                fo.GeometryPropertyName = "Shape";
                listFeature.Add(fo);
            }
            DynamicGeoSource lineGeos = null;   //线状地物数据源
            VectorLayer lineLayer = null;  //创建线状地物矢量图层
            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                if (map.MapControl.Layers.Count == layerCountIndex)
                {
                    lineLayer = new VectorLayer();  //创建线状地物矢量图层
                    lineLayer.DataProcessingMethod = eDataProcessingMethod.Synchronous;
                    lineLayer.Renderer = new SimpleRenderer(new SimplePolylineSymbol()
                    {
                        StrokeColor = Color.FromArgb(255, 255, 0, 0),
                        StrokeThickness = 1
                    });
                    lineGeos = new DynamicGeoSource();
                    lineGeos.AddRange(listFeature.ToArray());
                    lineLayer.DataSource = lineGeos;
                    map.MapControl.Layers.Add(lineLayer);
                }
                else
                {
                    lineLayer = map.MapControl.Layers[layerCountIndex] as VectorLayer;
                    lineGeos = lineLayer.DataSource as DynamicGeoSource;
                    lineGeos.Clear();
                    lineGeos.AddRange(listFeature.ToArray());
                }
            }));
            listFeature.Clear();
            layerCountIndex++;

            foreach (var mzdw in tempPolygons)
            {
                FeatureObject fo = new FeatureObject();
                fo.Object = mzdw;
                fo.Geometry = mzdw.Shape;
                fo.GeometryPropertyName = "Shape";
                listFeature.Add(fo);
            }
            DynamicGeoSource mzdwGeos = null;   //面状地物数据源
            VectorLayer mzdwLayer = null;  //创建面状地物矢量图层
            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                if (map.MapControl.Layers.Count == layerCountIndex)
                {
                    mzdwLayer = new VectorLayer();  //创建面状地物矢量图层
                    mzdwLayer.DataProcessingMethod = eDataProcessingMethod.Synchronous;
                    mzdwLayer.Renderer = new SimpleRenderer(new SimplePolygonSymbol()
                    {
                        BackgroundColor = Colors.Transparent,
                        BorderStrokeColor = Color.FromArgb(255, 255, 0, 0),
                        BorderThickness = 1,
                    });
                    mzdwGeos = new DynamicGeoSource();
                    mzdwGeos.AddRange(listFeature.ToArray());
                    mzdwLayer.DataSource = mzdwGeos;
                    map.MapControl.Layers.Add(mzdwLayer);
                }
                else
                {
                    mzdwLayer = map.MapControl.Layers[layerCountIndex] as VectorLayer;
                    mzdwGeos = mzdwLayer.DataSource as DynamicGeoSource;
                    mzdwGeos.Clear();
                    mzdwGeos.AddRange(listFeature.ToArray());
                }
            }));
        }

        #endregion 常规添加图形

        #region 常规添加图形标注

        /// <summary>
        /// 添加界址圈图形
        /// </summary>
        public void AddNodeLayer(ContractLand geoLand, List<ContractLand> listGeoLand, MapShapeUI map, List<BuildLandBoundaryAddressDot> listValidDot, int layerCountIndex)
        {
            List<FeatureObject> fos = new List<FeatureObject>();
            FilterjzdNodes.ForEach(c =>
            {
                var fo = new FeatureObject() { Geometry = c, Object = new { } };
                fos.Add(fo);
            });

            DynamicGeoSource geoSource = null;
            VectorLayer pointLyer = null;
            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                if (map.MapControl.Layers.Count == layerCountIndex)
                {
                    pointLyer = new VectorLayer();
                    pointLyer.DataProcessingMethod = eDataProcessingMethod.Synchronous;
                    pointLyer.Renderer = new SimpleRenderer(new SimplePointSymbol()
                    {
                        BackgroundColor = Colors.White,
                        BorderStrokeColor = Color.FromArgb(255, 255, 0, 0),
                        Size = 4

                        //BackgroundColor = Colors.Red,
                        //BorderStrokeColor = Colors.Red,
                        //Size = 6
                    });
                    geoSource = new DynamicGeoSource();
                    geoSource.AddRange(fos.ToArray());
                    pointLyer.DataSource = geoSource;
                    map.MapControl.Layers.Add(pointLyer);
                }
                else
                {
                    pointLyer = map.MapControl.Layers[layerCountIndex] as VectorLayer;
                    geoSource = pointLyer.DataSource as DynamicGeoSource;
                    geoSource.Clear();
                    geoSource.AddRange(fos.ToArray());
                }
            }));
        }

        /// <summary>
        /// 添加中心标注-isEditUse编辑框需用ListTextShape，不然会出现两个中线
        /// </summary>
        public DiagramBase AddCenterLable(DiagramsView view, DiagramBase diagram, DynamicGeoSource geods, Envelope extent, ContractLand geoLand, int index)
        {
            var geo = geoLand.Shape as YuLinTu.Spatial.Geometry;
            if (geo.IsValid() == false) geo = geo.GetEnvelope().ToGeometry();
            var obj = geods.Get()[0];
            string showCenterText = string.Empty;
            string showLandNumber = string.Empty;
            string showLandName = string.Empty;
            if (SettingDefine.IsUseLandNumber)
            {
                showLandNumber = ObjectExtension.GetPropertyValue<string>(obj.Object, "LandNumber");
                if (showLandNumber.IsNotNullOrEmpty() && showLandNumber.Length == 19)
                {
                    showLandNumber = showLandNumber.Substring(SettingDefine.GetLandMiniNumber, showLandNumber.Length - SettingDefine.GetLandMiniNumber);
                }
                showCenterText = showLandNumber;
            }

            if (SettingDefine.IsUseLandName)
            {
                showLandName = ObjectExtension.GetPropertyValue<string>(obj.Object, "Name");
                showCenterText = string.Format("{0}\n{1}", showCenterText, showLandName);
            }

            if (SettingDefine.IsUseLandAwareArea)
            {
                showCenterText = string.Format("{0}\n{1}", ToolMath.SetNumbericFormat(geoLand.AwareArea.ToString(), 2), showCenterText);
            }

            if (SettingDefine.IsUseLandActualArea)
            {
                showCenterText = string.Format("{0}\n{1}", ToolMath.SetNumbericFormat(geoLand.ActualArea.ToString(), 2), showCenterText);
            }

            if (SettingDefine.IsUseLandTableArea)
            {
                showCenterText = string.Format("{0}\n{1}", ToolMath.SetNumbericFormat(geoLand.TableArea.ToString(), 2), showCenterText);
            }
            //diagram.FontSize = SettingDefine.UseLandLabelFontSize;
            Size sz;
            if (view.Items.Count == index)
            {
                diagram = new ListTextShape()
                {
                    FontSize = 13,
                    FontFamily = "宋体",
                    FontColor = Color.FromRgb(0, 0, 0),
                }.CreateDiagram();
                (diagram.Model as ListTextShape).HorizontalAlignment = HorizontalAlignment.Center;
                (diagram.Model as ListTextShape).VerticalAlignment = VerticalAlignment.Center;
                diagram.FontSize = SettingDefine.UseLandLabelFontSize;
                sz = calcTextSize(showCenterText, diagram);
                diagram.Model.Width = sz.Width;// 100;
                diagram.Model.Height = sz.Height;// 60;
                diagram.Model.BorderWidth = 0;
                view.Items.Add(diagram);
            }
            else
            {
                diagram = view.Items[index];
                diagram.FontSize = SettingDefine.UseLandLabelFontSize;
                sz = calcTextSize(showCenterText, diagram);
                diagram.Model.Width = sz.Width;// 100;
                diagram.Model.Height = sz.Height;// 60;
            }

            (diagram.Model as TextShapeBase).Text = showCenterText;

            //字体设置
            (diagram.Model as TextShapeBase).FontFamily = SettingDefine.UseLandLabelFontSet;
            (diagram.Model as TextShapeBase).FontSize = SettingDefine.UseLandLabelFontSize;
            (diagram.Model as TextShapeBase).FontColor = SettingDefine.UseLandLabelFontColor;
            (diagram.Model as TextShapeBase).IsBold = SettingDefine.UseLandLabelBold;

            var location = GetLocation(extent, mapW, mapH, sz.Width, sz.Height, geo);

            diagram.Model.X = location.X + 0;
            diagram.Model.Y = location.Y + 0;
            return diagram;
        }

        /// <summary>
        /// 添加相邻宗地标注
        /// </summary>
        public DiagramBase AddNeiberLandlabel(DiagramsView view, DiagramBase diagram, Envelope extent, ContractLand tempLand, ContractLand geoLand, int index)
        {
            double Distance = 3;//线段延长的距离；
            var templandgeo = tempLand.Shape.Instance;
            if (!templandgeo.IsValid)
            {
                templandgeo = YuLinTu.tGISCNet.Topology.Simplify(tempLand.Shape).Instance;
            }
            var tempgetitem = templandgeo.Intersection(extent.ToGeometry().Instance);
            if (tempgetitem.IsEmpty) return null;
            var templandcenterptn = new Coordinate(tempgetitem.PointOnSurface.X, tempgetitem.PointOnSurface.Y);//终点

            var geolandgeo = geoLand.Shape.Instance;
            if (!geolandgeo.IsValid)
            {
                geolandgeo = YuLinTu.tGISCNet.Topology.Simplify(geoLand.Shape).Instance;
            }
            var geolandcenterptn = new Coordinate(geolandgeo.PointOnSurface.X, geolandgeo.PointOnSurface.Y);//起点
            List<Coordinate> uselinecdts = new List<Coordinate>();
            uselinecdts.Add(geolandcenterptn);
            uselinecdts.Add(templandcenterptn);
            Spatial.Geometry useline = Spatial.Geometry.CreatePolyline(uselinecdts, tempLand.Shape.Srid);

            var res = Math.Max(extent.Width / mapW, extent.Height / mapH);
            var distancebettwocdt = Math.Sqrt(Math.Pow(geolandcenterptn.X - templandcenterptn.X, 2) + Math.Pow(geolandcenterptn.Y - templandcenterptn.Y, 2));
            var screendistancetwocdt = distancebettwocdt / res;
            var newDistancePtn = new Spatial.Geometry();
            if (screendistancetwocdt < 30)
            {
                newDistancePtn = Deflection_Distance(useline, 0, Distance + 15 * res);//获取新距离下的坐标点；
            }
            else if (screendistancetwocdt > 60)
            {
                newDistancePtn = Deflection_Distance(useline, 0, -Distance + 1);//获取新距离下的坐标点；
            }
            else
            {
                newDistancePtn = Deflection_Distance(useline, 0, Distance);//获取新距离下的坐标点；
            }

            var landOwnerName = tempLand.OwnerName;
            if (SettingDefine.NeighborlandLabelisJTuseLandName && landOwnerName != null && landOwnerName.Contains("集体"))
            {
                landOwnerName = tempLand.LandName != null ? tempLand.LandName : "";
            }

            double elementWidth = 100;
            double elementHeight = 60;

            if (view.Items.Count == index)
            {
                diagram = new TextShape()
                {
                    FontSize = 12,
                    FontFamily = "宋体",
                    FontColor = System.Windows.Media.Color.FromRgb(0, 0, 0),
                }.CreateDiagram();
                diagram.FontSize = SettingDefine.NeighborLandLabelFontSize;
                var sz = calcTextSize(landOwnerName, diagram);//.FontFamily,diagram.FontSize,false);
                sz.Height += 3;
                elementWidth = sz.Width;//100
                elementHeight = sz.Height;//60
                diagram.Model.Width = elementWidth;// sz.Width;// 100;
                diagram.Model.Height = elementHeight;// 60;// sz.Height;// 60;
                diagram.Model.BorderWidth = 0;
                (diagram.Model as TextShape).HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
                (diagram.Model as TextShape).VerticalAlignment = System.Windows.VerticalAlignment.Center;
                view.Items.Add(diagram);
            }
            else
            {
                diagram = view.Items[index];
                diagram.FontSize = SettingDefine.NeighborLandLabelFontSize;
                var sz = calcTextSize(landOwnerName, diagram);//.FontFamily,diagram.FontSize,false);
                sz.Height += 3;
                elementWidth = sz.Width;//100
                elementHeight = sz.Height;//60
                diagram.Model.Width = elementWidth;// sz.Width;// 100;
                diagram.Model.Height = elementHeight;// 60;// sz.Height;// 60;
            }
            (diagram.Model as TextShapeBase).Text = landOwnerName;

            //字体设置
            (diagram.Model as TextShapeBase).FontFamily = SettingDefine.NeighborLandLabelFontSet;
            (diagram.Model as TextShapeBase).FontSize = SettingDefine.NeighborLandLabelFontSize;
            (diagram.Model as TextShapeBase).FontColor = SettingDefine.NeighborLandLabelFontColor;
            (diagram.Model as TextShapeBase).IsBold = SettingDefine.NeighborLandLabelBold;

            var location = GetLocation(extent, mapW, mapH, elementWidth, elementHeight, newDistancePtn);

            diagram.Model.X = location.X + 0;
            diagram.Model.Y = location.Y + 0;

            diagram.Model.Angle = 0;
            return diagram;
        }

        /// <summary>
        /// 添加相邻点状地物标注
        /// </summary>
        public DiagramBase AddNeiberDzdwlabel(DiagramsView view, DiagramBase diagram, Envelope extent, DZDW tempDZDW, ContractLand geoLand, int index)
        {
            double Distance = 3;//线段延长的距离；
            var templandgeo = tempDZDW.Shape.Normalized();
            var tempgetitem = templandgeo.Intersection(extent.ToGeometry());
            if (tempgetitem == null) return null;
            var templandcenterptn = new Coordinate(tempgetitem.Instance.PointOnSurface.X, tempgetitem.Instance.PointOnSurface.Y);//终点

            var geolandgeo = geoLand.Shape.Normalized();
            var geolandcenterptn = new Coordinate(geolandgeo.Instance.PointOnSurface.X, geolandgeo.Instance.PointOnSurface.Y);//起点
            List<Coordinate> uselinecdts = new List<Coordinate>();
            uselinecdts.Add(geolandcenterptn);
            uselinecdts.Add(templandcenterptn);
            Spatial.Geometry useline = Spatial.Geometry.CreatePolyline(uselinecdts, templandgeo.Srid);

            var res = Math.Max(extent.Width / mapW, extent.Height / mapH);
            var distancebettwocdt = Math.Sqrt(Math.Pow(geolandcenterptn.X - templandcenterptn.X, 2) + Math.Pow(geolandcenterptn.Y - templandcenterptn.Y, 2));
            var screendistancetwocdt = distancebettwocdt / res;
            var newDistancePtn = new Spatial.Geometry();
            if (screendistancetwocdt < 30)
            {
                newDistancePtn = Deflection_Distance(useline, 0, Distance + 15 * res);//获取新距离下的坐标点；
            }
            else if (screendistancetwocdt > 60)
            {
                newDistancePtn = Deflection_Distance(useline, 0, -Distance + 1);//获取新距离下的坐标点；
            }
            else
            {
                newDistancePtn = Deflection_Distance(useline, 0, Distance);//获取新距离下的坐标点；
            }

            var landOwnerName = tempDZDW.DWMC;
            double elementWidth = 100;
            double elementHeight = 60;
            if (view.Items.Count == index)
            {
                diagram = new TextShape()
                {
                    FontSize = 12,
                    FontColor = System.Windows.Media.Color.FromRgb(0, 0, 0),
                    //BackgroundColor = Color.FromArgb(100,0, 255,0),
                }.CreateDiagram();

                var sz = calcTextSize(landOwnerName, diagram);//.FontFamily,diagram.FontSize,false);
                elementWidth = sz.Width;//100
                elementHeight = sz.Height;//60

                diagram.Model.Width = elementWidth;// sz.Width;// 100;
                diagram.Model.Height = elementHeight;// 60;// sz.Height;// 60;
                diagram.Model.BorderWidth = 0;
                (diagram.Model as TextShape).HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
                (diagram.Model as TextShape).VerticalAlignment = System.Windows.VerticalAlignment.Center;
                view.Items.Add(diagram);
            }
            else
            {
                diagram = view.Items[index];
                var sz = calcTextSize(landOwnerName, diagram);//.FontFamily,diagram.FontSize,false);
                elementWidth = sz.Width;//100
                elementHeight = sz.Height;//60
                diagram.Model.Width = elementWidth;// sz.Width;// 100;
                diagram.Model.Height = elementHeight;// 60;// sz.Height;// 60;
            }
            (diagram.Model as TextShapeBase).Text = landOwnerName;

            var location = GetLocation(extent, mapW, mapH, elementWidth, elementHeight, newDistancePtn);

            //字体设置
            (diagram.Model as TextShapeBase).FontFamily = SettingDefine.NeighborLandLabelFontSet;
            (diagram.Model as TextShapeBase).FontSize = SettingDefine.NeighborLandLabelFontSize;
            (diagram.Model as TextShapeBase).FontColor = SettingDefine.NeighborLandLabelFontColor;
            (diagram.Model as TextShapeBase).IsBold = SettingDefine.NeighborLandLabelBold;

            diagram.Model.X = location.X + 0;
            diagram.Model.Y = location.Y + 0;
            return diagram;
        }

        /// <summary>
        /// 添加相邻线状地物标注
        /// </summary>
        public DiagramBase AddNeiberXzdwlabel(DiagramsView view, DiagramBase diagram, Envelope extent, XZDW tempXZDW, ContractLand geoLand, int index)
        {
            double Distance = 3;//线段延长的距离；
            var templandgeo = tempXZDW.Shape.Normalized();
            var tempgetitem = templandgeo.Intersection(extent.ToGeometry());
            if (tempgetitem == null) return null;
            var templandcenterptn = new Coordinate(tempgetitem.Instance.PointOnSurface.X, tempgetitem.Instance.PointOnSurface.Y);//终点

            var geolandgeo = geoLand.Shape.Normalized();
            var geolandcenterptn = new Coordinate(geolandgeo.Instance.PointOnSurface.X, geolandgeo.Instance.PointOnSurface.Y);//起点
            List<Coordinate> uselinecdts = new List<Coordinate>();
            uselinecdts.Add(geolandcenterptn);
            uselinecdts.Add(templandcenterptn);
            Spatial.Geometry useline = Spatial.Geometry.CreatePolyline(uselinecdts, templandgeo.Srid);

            var res = Math.Max(extent.Width / mapW, extent.Height / mapH);
            var distancebettwocdt = Math.Sqrt(Math.Pow(geolandcenterptn.X - templandcenterptn.X, 2) + Math.Pow(geolandcenterptn.Y - templandcenterptn.Y, 2));
            var screendistancetwocdt = distancebettwocdt / res;
            var newDistancePtn = new Spatial.Geometry();
            if (screendistancetwocdt < 30)
            {
                newDistancePtn = Deflection_Distance(useline, 0, Distance + 15 * res);//获取新距离下的坐标点；
            }
            else if (screendistancetwocdt > 60)
            {
                newDistancePtn = Deflection_Distance(useline, 0, -Distance + 1);//获取新距离下的坐标点；
            }
            else
            {
                newDistancePtn = Deflection_Distance(useline, 0, Distance);//获取新距离下的坐标点；
            }

            var landOwnerName = tempXZDW.DWMC;
            double elementWidth = 100;
            double elementHeight = 60;
            if (view.Items.Count == index)
            {
                diagram = new TextShape()
                {
                    FontSize = 12,
                    FontColor = System.Windows.Media.Color.FromRgb(0, 0, 0),
                    //BackgroundColor = Color.FromArgb(100,0, 255,0),
                }.CreateDiagram();

                var sz = calcTextSize(landOwnerName, diagram);//.FontFamily,diagram.FontSize,false);
                elementWidth = sz.Width;//100
                elementHeight = sz.Height;//60

                diagram.Model.Width = elementWidth;// sz.Width;// 100;
                diagram.Model.Height = elementHeight;// 60;// sz.Height;// 60;
                diagram.Model.BorderWidth = 0;
                (diagram.Model as TextShape).HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
                (diagram.Model as TextShape).VerticalAlignment = System.Windows.VerticalAlignment.Center;
                view.Items.Add(diagram);
            }
            else
            {
                diagram = view.Items[index];
                var sz = calcTextSize(landOwnerName, diagram);//.FontFamily,diagram.FontSize,false);
                elementWidth = sz.Width;//100
                elementHeight = sz.Height;//60
                diagram.Model.Width = elementWidth;// sz.Width;// 100;
                diagram.Model.Height = elementHeight;// 60;// sz.Height;// 60;
            }
            (diagram.Model as TextShapeBase).Text = landOwnerName;

            var location = GetLocation(extent, mapW, mapH, elementWidth, elementHeight, newDistancePtn);
            //字体设置
            (diagram.Model as TextShapeBase).FontFamily = SettingDefine.NeighborLandLabelFontSet;
            (diagram.Model as TextShapeBase).FontSize = SettingDefine.NeighborLandLabelFontSize;
            (diagram.Model as TextShapeBase).FontColor = SettingDefine.NeighborLandLabelFontColor;
            (diagram.Model as TextShapeBase).IsBold = SettingDefine.NeighborLandLabelBold;

            diagram.Model.X = location.X + 0;
            diagram.Model.Y = location.Y + 0;
            return diagram;
        }

        /// <summary>
        /// 添加相邻面状地物标注
        /// </summary>
        public DiagramBase AddNeiberMzdwlabel(DiagramsView view, DiagramBase diagram, Envelope extent, MZDW tempMZDW, ContractLand geoLand, int index)
        {
            double Distance = 3;//线段延长的距离；
            var templandgeo = tempMZDW.Shape.Normalized();
            var tempgetitem = templandgeo.Intersection(extent.ToGeometry());
            if (tempgetitem == null) return null;
            var templandcenterptn = new Coordinate(tempgetitem.Instance.PointOnSurface.X, tempgetitem.Instance.PointOnSurface.Y);//终点

            var geolandgeo = geoLand.Shape.Normalized();
            var geolandcenterptn = new Coordinate(geolandgeo.Instance.PointOnSurface.X, geolandgeo.Instance.PointOnSurface.Y);//起点
            List<Coordinate> uselinecdts = new List<Coordinate>();
            uselinecdts.Add(geolandcenterptn);
            uselinecdts.Add(templandcenterptn);
            Spatial.Geometry useline = Spatial.Geometry.CreatePolyline(uselinecdts, templandgeo.Srid);

            var res = Math.Max(extent.Width / mapW, extent.Height / mapH);
            var distancebettwocdt = Math.Sqrt(Math.Pow(geolandcenterptn.X - templandcenterptn.X, 2) + Math.Pow(geolandcenterptn.Y - templandcenterptn.Y, 2));
            var screendistancetwocdt = distancebettwocdt / res;
            var newDistancePtn = new Spatial.Geometry();
            if (screendistancetwocdt < 30)
            {
                newDistancePtn = Deflection_Distance(useline, 0, Distance + 15 * res);//获取新距离下的坐标点；
            }
            else if (screendistancetwocdt > 60)
            {
                newDistancePtn = Deflection_Distance(useline, 0, -Distance + 1);//获取新距离下的坐标点；
            }
            else
            {
                newDistancePtn = Deflection_Distance(useline, 0, Distance);//获取新距离下的坐标点；
            }

            var landOwnerName = tempMZDW.DWMC;
            double elementWidth = 100;
            double elementHeight = 60;
            if (view.Items.Count == index)
            {
                diagram = new TextShape()
                {
                    FontSize = 12,
                    FontColor = System.Windows.Media.Color.FromRgb(0, 0, 0),
                    //BackgroundColor = Color.FromArgb(100,0, 255,0),
                }.CreateDiagram();

                var sz = calcTextSize(landOwnerName, diagram);//.FontFamily,diagram.FontSize,false);
                elementWidth = sz.Width;//100
                elementHeight = sz.Height;//60

                diagram.Model.Width = elementWidth;// sz.Width;// 100;
                diagram.Model.Height = elementHeight;// 60;// sz.Height;// 60;
                diagram.Model.BorderWidth = 0;
                (diagram.Model as TextShape).HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
                (diagram.Model as TextShape).VerticalAlignment = System.Windows.VerticalAlignment.Center;
                view.Items.Add(diagram);
            }
            else
            {
                diagram = view.Items[index];
                var sz = calcTextSize(landOwnerName, diagram);//.FontFamily,diagram.FontSize,false);
                elementWidth = sz.Width;//100
                elementHeight = sz.Height;//60
                diagram.Model.Width = elementWidth;// sz.Width;// 100;
                diagram.Model.Height = elementHeight;// 60;// sz.Height;// 60;
            }
            (diagram.Model as TextShapeBase).Text = landOwnerName;

            var location = GetLocation(extent, mapW, mapH, elementWidth, elementHeight, newDistancePtn);
            //字体设置
            (diagram.Model as TextShapeBase).FontFamily = SettingDefine.NeighborLandLabelFontSet;
            (diagram.Model as TextShapeBase).FontSize = SettingDefine.NeighborLandLabelFontSize;
            (diagram.Model as TextShapeBase).FontColor = SettingDefine.NeighborLandLabelFontColor;
            (diagram.Model as TextShapeBase).IsBold = SettingDefine.NeighborLandLabelBold;

            diagram.Model.X = location.X + 0;
            diagram.Model.Y = location.Y + 0;
            return diagram;
        }

        /// <summary>
        /// 添加界址点号标注
        /// </summary>
        public DiagramBase AddNeiberJzdNumberlabel(DiagramsView view, DiagramBase diagram, Envelope extent, BuildLandBoundaryAddressDot jzdShowLabel, ContractLand geoLand, int index, bool isUniteDotNumber = false)
        {
            double Distance = 3;//线段延长的距离；
            var templandgeo = jzdShowLabel.Shape.Normalized();
            var tempgetitem = templandgeo.Intersection(extent.ToGeometry());
            if (tempgetitem == null) return null;
            var templandcenterptn = new Coordinate(tempgetitem.Instance.PointOnSurface.X, tempgetitem.Instance.PointOnSurface.Y);//终点

            var geolandgeo = geoLand.Shape.Normalized();
            var geolandcenterptn = new Coordinate(geolandgeo.Instance.PointOnSurface.X, geolandgeo.Instance.PointOnSurface.Y);//起点
            List<Coordinate> uselinecdts = new List<Coordinate>();
            uselinecdts.Add(geolandcenterptn);
            uselinecdts.Add(templandcenterptn);
            Spatial.Geometry useline = Spatial.Geometry.CreatePolyline(uselinecdts, templandgeo.Srid);

            var res = Math.Max(extent.Width / mapW, extent.Height / mapH);
            var distancebettwocdt = Math.Sqrt(Math.Pow(geolandcenterptn.X - templandcenterptn.X, 2) + Math.Pow(geolandcenterptn.Y - templandcenterptn.Y, 2));
            var screendistancetwocdt = distancebettwocdt / res;
            var newDistancePtn = new Spatial.Geometry();
            if (screendistancetwocdt < 30)
            {
                newDistancePtn = Deflection_Distance(useline, 0, Distance + 15 * res);//获取新距离下的坐标点；
            }
            else if (screendistancetwocdt > 60)
            {
                newDistancePtn = Deflection_Distance(useline, 0, -Distance + 1);//获取新距离下的坐标点；
            }
            else
            {
                newDistancePtn = Deflection_Distance(useline, 0, Distance);//获取新距离下的坐标点；
            }

            var landOwnerName = isUniteDotNumber ? jzdShowLabel.UniteDotNumber : jzdShowLabel.DotNumber;
            double elementWidth = 100;
            double elementHeight = 60;
            if (view.Items.Count == index)
            {
                diagram = new TextShape()
                {
                    FontSize = 12,
                    FontColor = System.Windows.Media.Color.FromRgb(0, 0, 0),
                }.CreateDiagram();

                var sz = calcTextSize(landOwnerName, diagram);//.FontFamily,diagram.FontSize,false);
                elementWidth = sz.Width + 10;//100
                elementHeight = sz.Height + 10;//60

                diagram.Model.Width = elementWidth;// sz.Width;// 100;
                diagram.Model.Height = elementHeight;// 60;// sz.Height;// 60;
                diagram.Model.BorderWidth = 0;
                (diagram.Model as TextShape).HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
                (diagram.Model as TextShape).VerticalAlignment = System.Windows.VerticalAlignment.Center;
                view.Items.Add(diagram);
            }
            else
            {
                diagram = view.Items[index];
                var sz = calcTextSize(landOwnerName, diagram);//.FontFamily,diagram.FontSize,false);
                elementWidth = sz.Width + 10;//100
                elementHeight = sz.Height + 10;//60
                diagram.Model.Width = elementWidth;// sz.Width;// 100;
                diagram.Model.Height = elementHeight;// 60;// sz.Height;// 60;
            }
            (diagram.Model as TextShapeBase).Text = landOwnerName;

            //字体设置
            (diagram.Model as TextShapeBase).FontFamily = SettingDefine.NeighborLandLabelFontSet;
            (diagram.Model as TextShapeBase).FontSize = SettingDefine.NeighborLandLabelFontSize;
            (diagram.Model as TextShapeBase).FontColor = SettingDefine.NeighborLandLabelFontColor;
            (diagram.Model as TextShapeBase).IsBold = SettingDefine.NeighborLandLabelBold;

            var location = GetLocation(extent, mapW, mapH, elementWidth, elementHeight, templandgeo);

            diagram.Model.X = location.X + 0;
            diagram.Model.Y = location.Y + 0;
            return diagram;
        }

        #endregion 常规添加图形标注

        #region 辅助添加，如比例尺、标注线

        /// <summary>
        /// 全宗临宗添加比例尺标注
        /// </summary>
        public DiagramBase AddScaleText(DiagramsView view, DiagramBase diagram, Envelope extent, int index, double result = 0, bool isViewOfAllScaleUse = false)
        {
            var res = Math.Max(extent.Width / mapW, extent.Height / mapH);
            if (result == 0 && isViewOfAllScaleUse == false)
            {
                result = res * 96 * 0.3937008 * 100;
                var integer = Math.Truncate(result * 0.01);
                if (integer == 0)
                    result = 100;
                else
                    result = integer * 100;
            }
            var text = string.Format("1:{0:F0}", result);
            Size sz;

            if (view.Items.Count == index)
            {
                diagram = new TextShape()
                {
                    FontSize = 10,
                    FontColor = Color.FromRgb(0, 0, 0),
                }.CreateDiagram();
                sz = calcTextSize(text, diagram);
                diagram.Model.Width = sz.Width;
                diagram.Model.Height = sz.Height;
                diagram.Model.BorderWidth = 0;
                diagram.Tag = true;
                view.Items.Add(diagram);
            }
            else
            {
                diagram = view.Items[index];
                sz = calcTextSize(text, diagram);
                diagram.Model.Width = sz.Width;
                diagram.Model.Height = sz.Height;
            }
            (diagram.Model as TextShapeBase).Text = text;

            diagram.Model.X = mapW / 2 - 25;
            diagram.Model.Y = mapH - 5;

            if (isViewOfAllScaleUse)
            {
                diagram.Model.X = view.Paper.Model.Width / 2 - 10;
                diagram.Model.Y = view.Paper.Model.Height - 20;
            }

            return diagram;
        }

        /// <summary>
        /// 添加主视图指北针
        /// </summary>
        public void AddMainCompass(DiagramsView view, DiagramBase diagram, int itemindex)
        {
            if (view.Items.Count == itemindex)
            {
                diagram = new CompassShape()
                {
                    FontSize = 2,
                    BorderColor = Color.FromRgb(50, 50, 50),
                }.CreateDiagram();
                diagram.Model.Height = 30;
                diagram.Model.Width = 15;
                diagram.Model.BorderWidth = 1;
                (diagram.Model as CompassShape).BorderWidthN = 1;
                diagram.Model.X = 5;
                diagram.Model.Y = 5;
                view.Items.Add(diagram);
            }
            else
            {
                diagram = view.Items[itemindex];
            }
        }

        /// <summary>
        /// 添加指北针
        /// </summary>
        public void AddCompass(DiagramsView view, DiagramBase diagram, int index)
        {
            if (view.Items.Count == index)
            {
                diagram = new CompassShape()
                {
                    FontSize = 1,
                    BorderColor = Color.FromRgb(50, 50, 50),
                }.CreateDiagram();
                diagram.Model.Height = 25;
                diagram.Model.Width = 15;
                diagram.Model.BorderWidth = 0.5;
                (diagram.Model as CompassShape).BorderWidthN = 1;
                diagram.Model.X = 0;
                diagram.Model.Y = 0;
                view.Items.Add(diagram);
            }
            else
            {
                diagram = view.Items[index];
            }
        }

        #endregion 辅助添加，如比例尺、标注线

        #region 辅助方法，如获取位置，比例尺等

        /// <summary>
        /// 过滤界址点
        /// </summary>
        public List<YuLinTu.Spatial.Geometry> FilterNodeByValidDot(ContractLand geoLand, List<BuildLandBoundaryAddressDot> listValidDot)
        {
            var geo = geoLand.Shape as YuLinTu.Spatial.Geometry;
            var geoPointArray = geo.ToPoints();

            //double prescion = 0.0001;
            List<YuLinTu.Spatial.Geometry> filterGeos = new List<Spatial.Geometry>();
            if (geoLand == null || geoPointArray == null)
                return filterGeos;
            var listGeoPoint = geoPointArray.ToList();
            var listLandValidDot = listValidDot.FindAll(c => c.LandID == geoLand.ID);
            if (listLandValidDot == null || listLandValidDot.Count == 0)
            {
                if (SettingDefine.IsVacuateDotRing)
                {
                    filterGeos = VacuteDotRing(listGeoPoint);
                }
                else
                {
                    listGeoPoint.ForEach(c => filterGeos.Add(c));
                }
            }
            else
            {
                //List<GeoAPI.Geometries.IPoint> listValidDotPoint = new List<GeoAPI.Geometries.IPoint>();
                //listLandValidDot.ForEach(c => listValidDotPoint.Add(c.Shape.Instance as GeoAPI.Geometries.IPoint));

                //if (listValidDotPoint == null || listValidDotPoint.Count == 0)
                //    return filterGeos;
                //foreach (var geoPoint in listGeoPoint)
                //{
                //    var point = geoPoint.Instance as GeoAPI.Geometries.IPoint;
                //    if (listValidDotPoint.Any(c => ToolMath.AlmostEquals(c.X, point.X, prescion) && ToolMath.AlmostEquals(c.Y, point.Y, prescion)))
                //        filterGeos.Add(geoPoint);
                //}

                listLandValidDot.ForEach(c => filterGeos.Add(c.Shape));
            }
            return filterGeos;
        }

        /// <summary>
        /// 根据设置，没有界址信息的情况下，抽稀全部的点
        /// </summary>
        /// <param name="listGeoPoint"></param>
        /// <returns></returns>
        private List<YuLinTu.Spatial.Geometry> VacuteDotRing(List<YuLinTu.Spatial.Geometry> listGeoPoint)
        {
            List<YuLinTu.Spatial.Geometry> filterGeos = new List<Spatial.Geometry>();
            double vacuteDotDistence = SettingDefine.VacuateDotRingSetIndex;
            filterGeos.Add(listGeoPoint[0]);
            for (int i = 1; i < listGeoPoint.Count; i++)
            {
                if (i != listGeoPoint.Count - 1)
                {
                    var ii1distance = listGeoPoint[i].Distance(listGeoPoint[i - 1]);
                    if (ii1distance < vacuteDotDistence)
                    {
                        listGeoPoint.Remove(listGeoPoint[i]);
                        i = i - 1;
                    }
                    else if (ii1distance > vacuteDotDistence)
                    {
                        filterGeos.Add(listGeoPoint[i]);
                    }
                }
                else
                {
                    var ii1distance = listGeoPoint[i].Distance(listGeoPoint[0]);
                    if (ii1distance < vacuteDotDistence)
                    {
                        listGeoPoint.Remove(listGeoPoint[i]);
                    }
                    else if (ii1distance > vacuteDotDistence)
                    {
                        if (filterGeos.Contains(listGeoPoint[i]) == false)
                        {
                            filterGeos.Add(listGeoPoint[i]);
                        }
                    }
                }
            }
            return filterGeos;
        }

        public Spatial.Geometry Deflection_Distance(Spatial.Geometry ls, double angle, double distance)
        {
            Spatial.Geometry ptn;
            var linecdts = ls.ToCoordinates();
            var Startcdt = linecdts[0];
            var Endcdt = linecdts[1];
            double s1_x = Endcdt.X - Startcdt.X;
            double s1_y = Endcdt.Y - Startcdt.Y;
            double d = Math.Sqrt(s1_x * s1_x + s1_y * s1_y);
            if (d == 0)
            {
                ptn = Spatial.Geometry.CreatePoint(Endcdt, ls.Srid);
                return ptn;
            }
            else
            {
                angle = angle * Math.PI / 180.0f;//角度转换为弧度
                double r_normal = 1.0f / d;
                s1_x *= r_normal;
                s1_y *= r_normal;
                double tempx = s1_x * Math.Cos(angle) - s1_y * Math.Sin(angle);
                double tempy = s1_x * Math.Sin(angle) + s1_y * Math.Cos(angle);
                s1_x = tempx * distance;
                s1_y = tempy * distance;
                var retX = Endcdt.X + s1_x;
                var retY = Endcdt.Y + s1_y;

                Coordinate retptn = new Coordinate(retX, retY);
                ptn = Spatial.Geometry.CreatePoint(retptn, ls.Srid);

                return ptn;
            }
        }

        /// <summary>
        /// 获取插入位置
        /// </summary>
        public Point GetLocation(Envelope mapExtent, double mapWidth, double mapHeight, double diagramWidth, double diagramHeight, YuLinTu.Spatial.Geometry geo)
        {
            var res = Math.Max(mapExtent.Width / mapWidth, mapExtent.Height / mapHeight);

            var dWidth = res * mapWidth;
            var dHeight = res * mapHeight;

            var extent = new Envelope()
            {
                MinX = mapExtent.MinX - dWidth / 2 + mapExtent.Width / 2,
                MaxX = mapExtent.MaxX + dWidth / 2 - mapExtent.Width / 2,
                MaxY = mapExtent.MaxY + dHeight / 2 - mapExtent.Height / 2,
                MinY = mapExtent.MinY - dHeight / 2 + mapExtent.Height / 2
            };

            var geo2 = geo.Normalized();

            var center = new Coordinate(geo2.Instance.PointOnSurface.X, geo2.Instance.PointOnSurface.Y);

            var x = (center.X - extent.MinX) / res - diagramWidth / 2;
            var y = (extent.MaxY - center.Y) / res - diagramHeight / 2;

            return new Point(x, y);
        }

        public Size calcTextSize(string text, DiagramBase e)// FontFamily fontFamily, double fontSize, FontWeights fontWeights)
        {
            var typeFace = new Typeface(e.FontFamily,// new System.Windows.Media.FontFamily(fontName),
                          e.FontStyle,// System.Windows.FontStyles.Normal,
                          e.FontWeight,// bold ? System.Windows.FontWeights.Bold : System.Windows.FontWeights.Normal,
                          e.FontStretch);// System.Windows.FontStretches.Normal);

            var fontSize = (double)new System.Windows.FontSizeConverter().ConvertFrom(e.FontSize + "pt");
            var ft = new FormattedText(text, System.Globalization.CultureInfo.CurrentCulture,
                System.Windows.FlowDirection.LeftToRight
                , typeFace, fontSize, System.Windows.Media.Brushes.Black);
            //ft.MaxTextWidth = 190;
            //return new Size(ft.Width, ft.Height * 0.76);
            return new Size(ft.Width, ft.Height);
        }

        /// <summary>
        /// 获取临宗四至是否有临接地块
        /// </summary>
        public void GetlandneighborhasLandInfo(ContractLand targetland, List<ContractLand> neiborlands, Dictionary<string, bool> landneighborhasland)
        {
            var targetlandCenterPoint = targetland.Shape.Instance.PointOnSurface;
            landneighborhasland.Add("东至", true);
            landneighborhasland.Add("南至", true);
            landneighborhasland.Add("西至", true);
            landneighborhasland.Add("北至", true);
            if (targetland.Shape == null || targetlandCenterPoint.IsEmpty)
            {
                return;
            }
            int geoSrid = targetland.Shape.Srid;
            var extendGeo = targetland.Shape.GetEnvelope();

            List<YuLinTu.Spatial.Geometry> currentlandpolyline = targetland.Shape.ToPolylines().ToList();
            var currentlandtargetline = currentlandpolyline[0];//外环

            double bufferDistence = SettingDefine.Neighborlandbufferdistence;//缓冲距离
            //东北框点坐标
            Coordinate extendNorthEastCdt = new Coordinate(extendGeo.MaxX, extendGeo.MaxY);
            //西北框点坐标
            Coordinate extendNorthWestCdt = new Coordinate(extendGeo.MinX, extendGeo.MaxY);
            //东南框点坐标
            Coordinate extendSouthEastCdt = new Coordinate(extendGeo.MaxX, extendGeo.MinY);
            //西南框点坐标
            Coordinate extendSouthWestCdt = new Coordinate(extendGeo.MinX, extendGeo.MinY);

            //当前地块中心点坐标
            Coordinate currentLandExtendCenterCdt = new Coordinate(targetlandCenterPoint.X, targetlandCenterPoint.Y);

            //北至
            //缩小搜索框
            Coordinate newextendNorthinterCdt = new Coordinate(targetlandCenterPoint.X, extendGeo.MaxY + bufferDistence);
            YuLinTu.Spatial.Geometry northtrianglegeo = YuLinTu.Spatial.Geometry.CreatePoint(newextendNorthinterCdt, geoSrid);//北边三角形

            var northintersectlands = neiborlands.FindAll(a => a.Shape.Intersects(northtrianglegeo));
            if (northintersectlands.Count == 0)
            {
                landneighborhasland["北至"] = false;
            }

            //东边
            Coordinate newextendEastinterCdt = new Coordinate(extendGeo.MaxX + bufferDistence, targetlandCenterPoint.Y);
            YuLinTu.Spatial.Geometry easttrianglegeo = YuLinTu.Spatial.Geometry.CreatePoint(newextendEastinterCdt, geoSrid);//东边三角形

            var eastintersectlands = neiborlands.FindAll(a => a.Shape.Intersects(easttrianglegeo));
            if (eastintersectlands.Count == 0)
            {
                landneighborhasland["东至"] = false;
            }

            //西至
            Coordinate newextendWestinterCdt = new Coordinate(extendGeo.MinX - bufferDistence, targetlandCenterPoint.Y);
            YuLinTu.Spatial.Geometry westtrianglegeo = YuLinTu.Spatial.Geometry.CreatePoint(newextendWestinterCdt, geoSrid);//西边三角形
            var westintersectlands = neiborlands.FindAll(a => a.Shape.Intersects(westtrianglegeo));
            if (westintersectlands.Count == 0)
            {
                landneighborhasland["西至"] = false;
            }

            //南至
            Coordinate newextendSouthinterCdt = new Coordinate(targetlandCenterPoint.X, extendGeo.MinY - bufferDistence);
            YuLinTu.Spatial.Geometry southtrianglegeo = YuLinTu.Spatial.Geometry.CreatePoint(newextendSouthinterCdt, geoSrid);//南边三角形

            var southintersectlands = neiborlands.FindAll(a => a.Shape.Intersects(southtrianglegeo));
            if (southintersectlands.Count == 0)
            {
                landneighborhasland["南至"] = false;
            }
        }

        //获取四至临宗的标注
        public List<DiagramBase> GetNeighborDiagramBases(DiagramsView view, ContractLand geoLand, int index, Dictionary<string, bool> landneighborhasland)
        {
            List<DiagramBase> landNeighbors = new List<DiagramBase>();

            if (landneighborhasland["东至"] == false && geoLand.NeighborEast.IsNullOrEmpty() == false)//东
            {
                var diagram = getdbBylandneibor(view, geoLand.NeighborEast, index);
                var sz = calcTextSize(geoLand.NeighborEast, diagram);
                if (SettingDefine.SetNeighborLandWestEastLabelVertical)
                {
                    diagram.Model.X = mapW - diagram.Width;
                    //diagram.Model.Y = mapH - sz.Width - (mapH / 2 - sz.Width / 2);
                    diagram.Model.Y = mapH - diagram.Width - (mapH / 2 - diagram.Width / 2);
                }
                else
                {
                    diagram.Model.X = mapW - sz.Width;
                    diagram.Model.Y = mapH / 2;
                }
                landNeighbors.Add(diagram);
                index++;
            }
            if (landneighborhasland["北至"] == false && geoLand.NeighborNorth.IsNullOrEmpty() == false)//北
            {
                var diagram = getdbBylandneibor(view, geoLand.NeighborNorth, index, false);
                diagram.Model.X = mapW / 2 - 20;
                diagram.Model.Y = 10;
                landNeighbors.Add(diagram);
                index++;
            }
            if (landneighborhasland["南至"] == false && geoLand.NeighborSouth.IsNullOrEmpty() == false)//南
            {
                var diagram = getdbBylandneibor(view, geoLand.NeighborSouth, index, false);
                diagram.Model.X = mapW / 2 - 25;
                diagram.Model.Y = mapH - 28;
                landNeighbors.Add(diagram);
                index++;
            }
            if (landneighborhasland["西至"] == false && geoLand.NeighborWest.IsNullOrEmpty() == false)//西
            {
                var diagram = getdbBylandneibor(view, geoLand.NeighborWest, index);
                diagram.Model.X = 0;
                diagram.Model.Y = mapH / 2;
                if (SettingDefine.SetNeighborLandWestEastLabelVertical)
                {
                    //var sz = calcTextSize(geoLand.NeighborEast, diagram);
                    diagram.Model.Y = mapH - diagram.Width - (mapH / 2 - diagram.Width / 2);
                }
                landNeighbors.Add(diagram);
                index++;
            }

            return landNeighbors;
        }

        //获取四至临宗直接打印调查四至中田埂、道路、沟渠
        public List<DiagramBase> GetNeighborDiagramBaseUSETGDLGQs(DiagramsView view, ContractLand geoLand, int index)
        {
            List<DiagramBase> landNeighbors = new List<DiagramBase>();

            if (geoLand.NeighborEast.IsNullOrEmpty() == false && (geoLand.NeighborEast.Contains("田埂") || geoLand.NeighborEast.Contains("道路") || geoLand.NeighborEast.Contains("沟渠")))//东
            {
                var diagram = getdbBylandneibor(view, geoLand.NeighborEast, index);
                var sz = calcTextSize(geoLand.NeighborEast, diagram);
                diagram.Model.X = mapW - sz.Width;
                diagram.Model.Y = mapH / 2;
                landNeighbors.Add(diagram);
                index++;
            }
            if (geoLand.NeighborNorth.IsNullOrEmpty() == false && (geoLand.NeighborNorth.Contains("田埂") || geoLand.NeighborNorth.Contains("道路") || geoLand.NeighborNorth.Contains("沟渠")))//北
            {
                var diagram = getdbBylandneibor(view, geoLand.NeighborNorth, index, false);
                diagram.Model.X = mapW / 2 - 20;
                diagram.Model.Y = 10;
                landNeighbors.Add(diagram);
                index++;
            }
            if (geoLand.NeighborSouth.IsNullOrEmpty() == false && (geoLand.NeighborSouth.Contains("田埂") || geoLand.NeighborSouth.Contains("道路") || geoLand.NeighborSouth.Contains("沟渠")))//南
            {
                var diagram = getdbBylandneibor(view, geoLand.NeighborSouth, index, false);
                diagram.Model.X = mapW / 2 - 25;
                diagram.Model.Y = mapH - 28;
                landNeighbors.Add(diagram);
                index++;
            }
            if (geoLand.NeighborWest.IsNullOrEmpty() == false && (geoLand.NeighborWest.Contains("田埂") || geoLand.NeighborWest.Contains("道路") || geoLand.NeighborWest.Contains("沟渠")))//西
            {
                var diagram = getdbBylandneibor(view, geoLand.NeighborWest, index);
                diagram.Model.X = 0;
                diagram.Model.Y = mapH / 2;
                landNeighbors.Add(diagram);
                index++;
            }

            return landNeighbors;
        }

        public DiagramBase getdbBylandneibor(DiagramsView view, string neiborname, int index, bool islabelVertical = true)
        {
            DiagramBase diagram = null;
            var landOwnerName = neiborname;
            double elementWidth = 100;
            double elementHeight = 60;
            if (view.Items.Count == index)
            {
                diagram = new TextShape()
                {
                    FontSize = 13,
                    FontFamily = "宋体",
                    FontColor = System.Windows.Media.Color.FromRgb(0, 0, 0),
                    //BackgroundColor = Color.FromArgb(100,0, 255,0),
                }.CreateDiagram();
                diagram.FontSize = SettingDefine.NeighborLandLabelFontSize;
                var sz = calcTextSize(landOwnerName, diagram);//.FontFamily,diagram.FontSize,false);
                elementWidth = sz.Width;//100
                elementHeight = sz.Height;//60

                diagram.Model.Width = elementWidth;// sz.Width;// 100;
                diagram.Model.Height = elementHeight;// 60;// sz.Height;// 60;
                diagram.Model.BorderWidth = 0;
                (diagram.Model as TextShape).HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
                (diagram.Model as TextShape).VerticalAlignment = System.Windows.VerticalAlignment.Center;
                view.Items.Add(diagram);
            }
            else
            {
                diagram = view.Items[index];
                diagram.FontSize = SettingDefine.NeighborLandLabelFontSize;
                var sz = calcTextSize(landOwnerName, diagram);//.FontFamily,diagram.FontSize,false);
                elementWidth = sz.Width;//100
                if (elementWidth > 190)
                {
                    elementWidth = 190;
                }
                elementHeight = sz.Height;//60
                diagram.Model.Width = elementWidth;// sz.Width;// 100;
                diagram.Model.Height = elementHeight;// 60;// sz.Height;// 60;
            }
            (diagram.Model as TextShapeBase).Text = landOwnerName;

            //字体设置
            (diagram.Model as TextShapeBase).FontFamily = SettingDefine.NeighborLandLabelFontSet;
            (diagram.Model as TextShapeBase).FontSize = SettingDefine.NeighborLandLabelFontSize;
            (diagram.Model as TextShapeBase).FontColor = SettingDefine.NeighborLandLabelFontColor;
            (diagram.Model as TextShapeBase).IsBold = SettingDefine.NeighborLandLabelBold;

            if (SettingDefine.SetNeighborLandWestEastLabelVertical && islabelVertical)
            {
                var dmw = diagram.Model.Width;
                diagram.Model.Width = diagram.Model.Height;
                diagram.Model.Height = dmw;
            }

            return diagram;
        }

        /// <summary>
        /// 确保元素完全可见
        /// </summary>
        /// <param name="visibleBounds"></param>
        /// <param name="element"></param>
        public void EnsureFullVisible(CglEnvelope visibleBounds, DiagramBase element)
        {
            var w = visibleBounds.MaxX;
            var h = visibleBounds.MaxY;
            //var leftX = -2;
            var leftX = 0;
            var m = element.Model;
            if (m.X < leftX)
            {
                m.X = leftX;
            }
            else if (m.X + m.Width > w)
            {
                m.X = w - m.Width;
            }
            var topY = 0;
            if (m.Y < topY)
            {
                m.Y = topY;
            }
            else if (m.Y + m.Height > h)
            {
                m.Y = h - m.Height;
            }
        }

        #endregion 辅助方法，如获取位置，比例尺等

        #endregion method
    }

    public class MyElements
    {
        private readonly List<DiagramBase> _lstElements = new List<DiagramBase>();
        private CglEnvelope _visibleBounds;

        public MyElements(CglEnvelope visibleBounds)
        {
            _visibleBounds = visibleBounds;
        }

        public void Clear()
        {
            _lstElements.Clear();
        }

        public void AddElement(DiagramBase element)
        {
            if (_lstElements.Count == 0)
            {
                _lstElements.Add(element);
                element.Background = new SolidColorBrush(Colors.Beige);
                return;
            }
            var m = element.Model;
            //double wi = m.Width * 0.75;// -m.Width * 15.0 / 48;
            double wi = m.Width;// -m.Width * 15.0 / 48;
            var maxX = _visibleBounds.MaxX;
            var maxY = _visibleBounds.MaxY;

            foreach (var e in _lstElements)
            {
                var ie = intersect(e, element);
                if (ie == null)
                    continue;

                var deltaUp = m.Y - (e.Model.Y - m.Height);
                var deltaDown = e.Model.Y + e.Model.Height - m.Y;
                var deltaLeft = m.X - (e.Model.X - wi);// m.Width);
                var deltaRight = e.Model.X + e.Model.Width - m.X;
                if (m.X < (maxX * 0.2) || m.X > (maxX * 0.8))
                {
                    deltaLeft *= 10;
                    deltaRight *= 10;
                }
                if (m.Y < (maxY * 0.2) || m.Y > (maxY * 0.8))
                {
                    deltaUp *= 10;
                    deltaDown *= 10;
                }
                var sa = new double[] { deltaUp, deltaDown, deltaLeft, deltaRight };
                for (int index = 0; index < 4; index++)
                {
                    int i = 0;
                    double minDelta = sa[0];
                    for (int j = 1; j < sa.Length; ++j)
                    {
                        if (sa[j] < minDelta)
                        {
                            minDelta = sa[j];
                            i = j;
                        }
                    }
                    if (i == 0)
                    {
                        m.Y = e.Model.Y - m.Height;
                    }
                    else if (i == 1)
                    {
                        m.Y = e.Model.Y + e.Model.Height;
                    }
                    else if (i == 2)
                    {
                        m.X = e.Model.X - wi;// m.Width;
                    }
                    else
                    {
                        m.X = e.Model.X + e.Model.Width;
                    }
                    // 防止重叠但不超出边界
                    if (m.X >= 0 && (m.X + m.Width) <= maxX && m.Y >= 0 && (m.Y + m.Height) <= maxY)
                    {
                        break;
                    }
                    else
                    {
                        sa[i] = int.MaxValue;
                    }
                }

                //if (m.X < 0)
                //    m.X = 0;//0 - m.Width;
                //if (m.Y < 0)
                //    m.Y = 0 - m.Height;
                //if (m.X < maxX && m.X + m.Width > maxX)
                //    m.X = maxX - m.Width;
                //if (m.Y < maxY && m.Y + m.Height > maxY)
                //    m.Y = maxY - m.Height;
            }
            //bool isinter = false;
            //if (_lstElements.Count > 0)
            //{
            //    foreach (var e in _lstElements)
            //    {
            //        var ie = intersect(e, element);
            //        if (ie == null)
            //        {
            //            continue;
            //        }
            //        else
            //        {
            //            isinter = true;
            //        }
            //    }
            //}
            //if (isinter == false) { _lstElements.Add(element); }
            _lstElements.Add(element);
        }

        public bool IsIntersect(DiagramBase element)
        {
            bool isinter = false;
            if (_lstElements.Count > 0)
            {
                foreach (var e in _lstElements)
                {
                    var ie = intersect(e, element);
                    if (ie == null)
                    {
                        continue;
                    }
                    else
                    {
                        isinter = true;
                    }
                }
            }
            return isinter;
        }

        private static CglEnvelope? intersect(DiagramBase a, DiagramBase b)
        {
            // double wi = a.Model.Width - a.Model.Width * 15.0 / 48;

            double wi = a.Model.Width;
            var ae = new CglEnvelope(a.Model.X, a.Model.Y, a.Model.X + wi, a.Model.Y + a.Model.Height);
            //wi = b.Model.Width - b.Model.Width * 15.0 / 48;

            wi = b.Model.Width;
            var be = new CglEnvelope(b.Model.X, b.Model.Y, b.Model.X + wi, b.Model.Y + b.Model.Height);
            if (!ae.Intersects(be))
                return null;

            double left = Math.Max(ae.MinX, be.MinX);
            double right = Math.Min(ae.MaxX, be.MaxX);
            double top = Math.Max(ae.MinY, be.MinY);
            double bottom = Math.Min(ae.MaxY, be.MaxY);
            return new CglEnvelope(left, top, right, bottom);
        }
    }
}