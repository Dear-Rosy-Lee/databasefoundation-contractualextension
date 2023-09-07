/*
 * (C) 2015  鱼鳞图公司版权所有,保留所有权利
 * 查询界址点的空间信息
 */
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using YuLinTu;
using YuLinTu.Spatial;
using YuLinTu.tGIS.Client;
using YuLinTu.Windows;
using YuLinTu.Windows.Wpf;
using YuLinTu.Windows.Wpf.Metro.Components;
using NetTopologySuite.Geometries;

namespace YuLinTu.Component.ZoneInvestigation
{
    public class MapControlActionQueryGeoPoint : MapControlAction
    {
        #region Properties

        private DetentionReporter Reporter
        {
            get
            {
                if (_Reporter == null)
                    _Reporter = DetentionReporterDispatcher.Create(
                        MapControl.Dispatcher, c => StartQuery((System.Windows.Point)c.Value), 100, 100);

                return _Reporter;
            }
        }


        #endregion

        #region Fields

        private GeometryFinderCenter finderLast = null;
        private DetentionReporter _Reporter = null;
        private MapControlActionQueryGeoPointShell dlg = null;
        private MapControlActionQueryGeoPointDataSource maqp = null;
        private int groupNum = 1; 
        #endregion

        #region Ctor

        public MapControlActionQueryGeoPoint(MapControl map)
            : base(map)
        {

        }

        #endregion

        #region Methods

        #region Methods - Override

        protected override void OnStartup()
        {
            var pWheelZoom = MapControl.GetPlugin<MapControlPluginWheelZoom>();
            if (pWheelZoom != null)
                pWheelZoom.Enabled = true;

            var pDragger = MapControl.GetPlugin<MapControlPluginDragger>();
            if (pDragger != null)
            {
                pDragger.EnableLeft(true);
                pDragger.EnableMiddle(true);
            }

            MapControl.MouseClick += MapControl_MouseClick;

        }

        protected override void OnShutdown()
        {
            var pWheelZoom = MapControl.GetPlugin<MapControlPluginWheelZoom>();
            if (pWheelZoom != null)
                pWheelZoom.Enabled = false;

            var pDragger = MapControl.GetPlugin<MapControlPluginDragger>();
            if (pDragger != null)
            {
                pDragger.EnableLeft(false);
                pDragger.EnableMiddle(false);
            }

            MapControl.MouseClick -= MapControl_MouseClick;
            if (dlg != null)
            {
                dlg.OnUninstallDisplayCoordinate();
            }
        }

        #endregion

        #region Methods - Events

        private void MapControl_MouseClick(object sender, MouseButtonEventArgs e)
        {
            if (MapControl == null || MapControl.IsAnimating)
                return;

            Reporter.Set(MapControl.PointToScreen(e));
        }

        #endregion

        #region Methods - Private
        //获取到被点击的对象
        private void StartQuery(System.Windows.Point geo)
        {
            if (MapControl == null)
                return;

            var map = MapControl;

            lock (MapControl.SynchronousObject)
            {
                if (finderLast != null)
                    finderLast.Cancel = true;

                var layers = MapControl.Layers.ToList();
                layers.Reverse();

                finderLast = new GeometryFinderCenter(MapControl, layers);
                finderLast.Tolerance = MapControl.SelectTolerance;
                finderLast.FindSelectable = true;
                finderLast.FindVisible = true;

                var task = TaskThreadDispatcher.Create(MapControl.Dispatcher,
                go =>
                {
                    go.Instance.Argument.Properties["Results"] = finderLast.FindTop(geo);
                }, null, null,
                start =>
                {
                    map.BusyUp();
                },
                end =>
                {
                    map.BusyDown();
                },
                completed =>
                {
                    var finder = completed.Argument.Properties.GetValue<GeometryFinderCenter>("Finder");
                    if (finder.Cancel)
                        return;

                    var results = completed.Argument.Properties.GetValue<List<Graphic>>("Results");
                    if (results == null || results.Count == 0)
                        return;

                    var workpage = map.Properties["Workpage"] as IWorkpage;
                    if (workpage == null)
                        return;

                    Show(workpage, map, results[0]);
                });

                task.Argument.Properties["Finder"] = finderLast;
                task.Start();
            }
        }

        /// <summary>
        /// 分解并显示获取得几何对象
        /// </summary>
        /// <param name="workpage"></param>
        /// <param name="map"></param>
        /// <param name="graphic"></param>
        private void Show(IWorkpage workpage, MapControl map, Graphic graphic)
        {
            map.SelectedItems.Clear();

            if (graphic.Layer.IsSelectable())
                map.SelectedItems.Add(graphic);

            if (!graphic.Layer.IsEditable())
                return;

            var unitlist = RefreshMapControlSpatialUnit(graphic);
            var lengthunit = unitlist[0].ToString();
            var areaunit = unitlist[1].ToString();            

            //获取到所有组别及二维坐标
            var getCoordinates = ToCoordinates(graphic);

            dlg = new MapControlActionQueryGeoPointShell();
            dlg.map = MapControl;
            dlg.OnInstallDisplayCoordinate();

            //长度、面积、组别获取
            dlg.GeoLength.Text = string.Format("{0:F2}  {1}", graphic.Geometry.Length(), lengthunit);
            dlg.GeoArea.Text = string.Format("{0:F2}  {1}", graphic.Geometry.Area(), areaunit);

            var area = graphic.Geometry.Area();
            var areamu = 0.0;
            switch (areaunit)
            {
                case "平方千米":                  
                    areamu = area * 1500;
                    break;
                case "平方米":
                    areamu = area * 0.0015;
                    break;
                default:
                    areamu = 0;
                    break;
            }
            dlg.GeoAreaMu.Text = string.Format("{0:F2}",areamu);
            dlg.GeoGroupsCount.Text = getCoordinates.Length.ToString();
            groupNum = int.Parse(getCoordinates.Length.ToString());
            dlg.nowGroup.Text = "1";
            dlg.nowGroup.MaxLength = groupNum.ToString().Length;     
            dlg.nowGroup.PreviewKeyUp += nowGroup_PreviewKeyUp;           
           
            dlg.GeoInfoGoTo.Click += GeoInfoGoTo_Click;

            if (!MapControl.InternalLayers.Contains(dlg.layerEndDisplay))
            {
                MapControl.InternalLayers.Add(dlg.layerEndDisplay);
            }

            maqp = new MapControlActionQueryGeoPointDataSource(getCoordinates);
            dlg.dgQueryGeoPoint.DataSource = maqp;
            workpage.Page.ShowMessageBox(dlg, (b, r) =>
            {
                if (MapControl.InternalLayers.Contains(dlg.layerEndDisplay))
                {
                    dlg.layerEndDisplay.Graphics.Clear();
                }
            });
        }

        /// <summary>
        /// 组别改变时，只能输入范围内的整数
        /// </summary>
        private void nowGroup_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            TextBox txt = sender as TextBox;
            if (e.Key >= Key.D0 && e.Key <= Key.D9 || e.Key == Key.Back || e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9)
            {
                try
                {
                    e.Handled = false;
                    if (txt.Text.ToString() == "")
                    {
                        txt.Text = "1";
                    }                    
                    var nowgroupnum = int.Parse(txt.Text.ToString());
                    if (nowgroupnum < 1)
                    {
                        txt.Text = "1";
                    }
                  
                    if (txt.Text[0].ToString() == "0")
                    {                      
                       txt.Text = txt.Text.Remove(0,1);
                    }
                    if (nowgroupnum > groupNum)
                    {
                        txt.Text = groupNum.ToString();
                    }
                }
                catch 
                {
                    return;
                }
            }
            else
            {
                txt.Text = "1";
                e.Handled = true;
            }
        }       
        
        /// <summary>
        /// 获取点击跳转组别事件
        /// </summary>       
        private void GeoInfoGoTo_Click(object sender, RoutedEventArgs e)
        {
            if (maqp == null || dlg == null || maqp.geoCoordinate == null) return;
            if (dlg.nowGroup.Text.ToString() == null || dlg.nowGroup.Text.ToString() == "") return;

            var groupindex = int.Parse(dlg.nowGroup.Text.ToString());
            if (groupindex > maqp.geoCoordinate.Length || groupindex <= 0) return;
            maqp.resourcePage = groupindex - 1;
            dlg.dgQueryGeoPoint.Refresh();
        }

        /// <summary>
        /// 坐标系发生改变时获取坐标单位，具体实现
        /// </summary>
        private List<string> RefreshMapControlSpatialUnit(Graphic grahpic)
        {
            List<string> unitlist = new List<string>();
            string lengthunit = "未知";
            string areaunit = "未知";
            try
            {
                if (grahpic.Geometry.SpatialReference.IsPROJCS())
                {
                    var projectionInfo = YuLinTu.Spatial.SpatialReferences.CreateProjectionInfo(grahpic.Geometry.SpatialReference);

                    if (projectionInfo == null) return null;
                    switch (projectionInfo.Unit.Name)
                    {
                        case "Kilometer":
                            lengthunit = "千米";
                            areaunit = "平方千米";
                            break;
                        case "Meter":
                            lengthunit = "米";
                            areaunit = "平方米";
                            break;
                        default:
                            lengthunit = "未知";
                            areaunit = "未知";
                            break;
                    }
                }
                else if (MapControl.SpatialReference.IsGEOGCS() || !MapControl.SpatialReference.IsValid())
                {
                    lengthunit = "未知";
                    areaunit = "未知";
                }
                unitlist.Add(lengthunit);
                unitlist.Add(areaunit);
                return unitlist;
            }
            catch (Exception error)
            {
                lengthunit = "未知";
                areaunit = "未知";
                unitlist.Add(lengthunit);
                unitlist.Add(areaunit);

                return unitlist;
            }
        }

        /// <summary>
        /// 获取点位列表
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        private Coordinate[][] ToCoordinates(Graphic source)
        {
            Coordinate[][] geos = null;
            YuLinTu.Spatial.Geometry geosource = source.Geometry;

            if (geosource.Instance is LineString)
                geos = PolylineToCoordinates(geosource.Instance as LineString);
            else if (geosource.Instance is MultiLineString)
                geos = MultiPolylineToCoordinates(geosource.Instance as MultiLineString);
            else if (geosource.Instance is Polygon)
                geos = PolygonToCoordinates(geosource.Instance as Polygon);
            else if (geosource.Instance is MultiPolygon)
                geos = MultiPolygonToCoordinates(geosource.Instance as MultiPolygon);
            else if (geosource.Instance is NetTopologySuite.Geometries.Point)
                geos = PointToCoordinates(geosource.Instance as NetTopologySuite.Geometries.Point);
            else if (geosource.Instance is MultiPoint)
                geos = MultiPointToCoordinates(geosource.Instance as MultiPoint);
            else if (geosource.Instance is GeometryCollection)
                geos = GeometryCollectionToCoordinates(geosource.Instance as GeometryCollection);
            else
                throw new NotSupportedException();

            return geos;
        }

        /// <summary>
        /// 返回各种选中要素的二维数组点坐标
        /// </summary> 
        #region 返回各种选中要素的二维数组点坐标
        private static Coordinate[][] PolygonToCoordinates(Polygon polygon)
        {
            List<Coordinate[]> list = new List<Coordinate[]>();
            List<Coordinate> geos = new List<Coordinate>();
            List<Coordinate> geoshole = new List<Coordinate>();

            var pts = polygon.Shell.Coordinates.ToList();
            foreach (var item in pts)
            {
                geos.Add(new Coordinate(item.X, item.Y));
            }
            YuLinTu.Spatial.Coordinate[] cor = geos.ToArray();
            list.Add(cor);

            YuLinTu.Spatial.Coordinate[] corhole = geos.ToArray();
            foreach (var hole in polygon.Holes)
            {
                var ptshole = hole.Coordinates.ToList();
                foreach (var itemhole in ptshole)
                {
                    geoshole.Add(new Coordinate(itemhole.X, itemhole.Y));
                }
                corhole = geoshole.ToArray();
                list.Add(corhole);
            }

            return list.ToArray();
        }

        private static Coordinate[][] MultiPointToCoordinates(MultiPoint multiPoint)
        {
            List<Coordinate[]> list = new List<Coordinate[]>();
            List<Coordinate> geos = new List<Coordinate>();

            foreach (var item in multiPoint.Geometries)
            {
                if (item is NetTopologySuite.Geometries.Point)
                {
                    var point = item as NetTopologySuite.Geometries.Point;
                    geos.Add(new Coordinate(point.X, point.Y));
                }
                else
                    throw new NotSupportedException();
            }

            YuLinTu.Spatial.Coordinate[] cor = geos.ToArray();
            list.Add(cor);

            return list.ToArray();
        }

        private static Coordinate[][] PointToCoordinates(NetTopologySuite.Geometries.Point point)
        {
            List<Coordinate[]> list = new List<Coordinate[]>();
            List<Coordinate> geos = new List<Coordinate>();
            geos.Add(new Coordinate(point.X, point.Y));
            YuLinTu.Spatial.Coordinate[] cor = geos.ToArray();
            list.Add(cor);
            return list.ToArray();
        }

        private static Coordinate[][] MultiPolygonToCoordinates(MultiPolygon multiPolygon)
        {
            List<Coordinate[]> list = new List<Coordinate[]>();

            foreach (var item in multiPolygon.Geometries)
            {
                if (item is Polygon)
                    list.AddRange(PolygonToCoordinates(item as Polygon));
                else
                    throw new NotSupportedException();
            }
            return list.ToArray();
        }

        private static Coordinate[][] MultiPolylineToCoordinates(MultiLineString multiLineString)
        {
            List<Coordinate[]> list = new List<Coordinate[]>();
            List<Coordinate> geos = new List<Coordinate>();

            foreach (var item in multiLineString.Geometries)
            {
                if (item is LineString)
                {
                    foreach (var itemm in item.Coordinates)
                    {
                        geos.Add(new Coordinate(itemm.X, itemm.Y));
                    }
                    YuLinTu.Spatial.Coordinate[] cor = geos.ToArray();
                    list.Add(cor);
                }
                else
                    throw new NotSupportedException();
            }
            return list.ToArray();
        }

        private static Coordinate[][] PolylineToCoordinates(LineString lineString)
        {
            List<Coordinate[]> list = new List<Coordinate[]>();
            List<Coordinate> geos = new List<Coordinate>();

            foreach (var item in lineString.Coordinates)
                geos.Add(new Coordinate(item.X, item.Y));
            YuLinTu.Spatial.Coordinate[] cor = geos.ToArray();
            list.Add(cor);
            return list.ToArray();
        }

        private static Coordinate[][] GeometryCollectionToCoordinates(GeometryCollection geometryCollection)
        {
            List<Coordinate[]> geos = new List<Coordinate[]>();
            foreach (var item in geometryCollection.Geometries)
            {
                if (item is LineString)
                    geos.AddRange(PolylineToCoordinates(item as LineString));
                else if (item is MultiLineString)
                    geos.AddRange(MultiPolylineToCoordinates(item as MultiLineString));
                else if (item is Polygon)
                    geos.AddRange(PolygonToCoordinates(item as Polygon));
                else if (item is MultiPolygon)
                    geos.AddRange(MultiPolygonToCoordinates(item as MultiPolygon));
                else if (item is NetTopologySuite.Geometries.Point)
                    geos.AddRange(PointToCoordinates(item as NetTopologySuite.Geometries.Point));
                else if (item is MultiPoint)
                    geos.AddRange(MultiPointToCoordinates(item as MultiPoint));
                else
                    throw new NotSupportedException();
            }
            return geos.ToArray();
        }

        #endregion

        #endregion
        #endregion
    }
}
