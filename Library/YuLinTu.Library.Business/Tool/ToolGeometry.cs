/*
 * (C) 2015  鱼鳞图公司版权所有,保留所有权利 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YuLinTu.Library.Entity;
using YuLinTu.Spatial;
using System.Windows;
using YuLinTu.NetAux;

namespace YuLinTu.Library.Business
{
    /// <summary>
    /// 图形工具
    /// </summary>
    public static class ToolGeometry
    {
        #region 生成界址点(点集合)

        /// <summary>
        /// 由地块生成界址点
        /// </summary>
        /// <param name="geoland">承包地块</param>
        /// <returns>界址点集合</returns>
        public static List<BuildLandBoundaryAddressDot> InitialDotShape(this ContractLand geoland)
        {
            if (geoland == null || geoland.Shape == null)
            {
                return null;
            }
            List<BuildLandBoundaryAddressDot> dotCollection = new List<BuildLandBoundaryAddressDot>();
            var geoShape = geoland.Shape;
            List<Coordinate> points = geoShape.ToCoordinates().ToList();
            //points.RemoveAt(points.Count - 1);
            int i = 1;
            foreach (var point in points)
            {
                YuLinTu.Spatial.Coordinate cordPoint = new Spatial.Coordinate(point.X, point.Y);
                var geoPoint = YuLinTu.Spatial.Geometry.CreatePoint(cordPoint, 0);
                BuildLandBoundaryAddressDot dot = new BuildLandBoundaryAddressDot()
                {
                    DotNumber = "J" + i.ToString(),
                    ZoneCode = geoland.LocationCode,
                    LandID = geoland.ID,
                    LandNumber = geoland.LandNumber,
                    Description = "",
                    Founder = "Admin",
                    CreationTime = DateTime.Now,
                    Shape = geoPoint,
                };
                dotCollection.Add(dot);
                i++;
            }
            return dotCollection;
        }

        /// <summary>
        /// 由图行生成点集合
        /// </summary>
        /// <param name="polygon">图行</param>
        /// <returns>点集合</returns>
        public static List<GeoAPI.Geometries.IPoint> InitialPointShape(this GeoAPI.Geometries.IPolygon polygon)
        {
            if (polygon == null)
            {
                return null;
            }
            GeoAPI.Geometries.Coordinate[] points = polygon.Coordinates;
            NetTopologySuite.Geometries.GeometryFactory gf = new NetTopologySuite.Geometries.GeometryFactory();
            List<GeoAPI.Geometries.IPoint> pointList = new List<GeoAPI.Geometries.IPoint>();
            foreach (var point in points)
            {
                GeoAPI.Geometries.IPoint apiPoint = gf.CreatePoint(point);
                if (apiPoint != null)
                    pointList.Add(apiPoint);
            }
            return pointList;
        }

        #endregion

        #region 生成界址线(线集合)

        /// <summary>
        /// 由界址点生成界址线
        /// </summary>  
        /// <param name="dots">界址点集合</param>
        /// <returns>界址线集合</returns>
        public static List<BuildLandBoundaryAddressCoil> InitialCoilShape(this List<BuildLandBoundaryAddressDot> dots)
        {
            if (dots == null || dots.Count == 0)
            {
                return null;
            }
            List<BuildLandBoundaryAddressCoil> coilCollection = new List<BuildLandBoundaryAddressCoil>();
            int numberEnd = 0;
            int startNumber = 0;
            int endNumber = 0;
            for (int i = 0; i < dots.Count(); i++)
            {
                GeoAPI.Geometries.IPoint dotInstanceStart = null;
                if (dots[i].Shape != null)
                    dotInstanceStart = dots[i].Shape.Instance as GeoAPI.Geometries.IPoint;
                GeoAPI.Geometries.IPoint dotInstanceEnd = null;
                if (i == dots.Count - 1)
                {
                    numberEnd = 0;
                    if (dots[numberEnd].Shape != null)
                        dotInstanceEnd = dots[numberEnd].Shape.Instance as GeoAPI.Geometries.IPoint;
                }
                else
                {
                    numberEnd = i + 1;
                    if (dots[numberEnd].Shape != null)
                        dotInstanceEnd = dots[numberEnd].Shape.Instance as GeoAPI.Geometries.IPoint;
                }
                List<Spatial.Coordinate> spCoordinates = new List<Spatial.Coordinate>();
                if (dotInstanceStart != null)
                {
                    YuLinTu.Spatial.Coordinate coordinateStart = new Spatial.Coordinate(dotInstanceStart.X, dotInstanceStart.Y);
                    spCoordinates.Add(coordinateStart);
                }
                if (dotInstanceEnd != null)
                {
                    YuLinTu.Spatial.Coordinate coordinateEnd = new Spatial.Coordinate(dotInstanceEnd.X, dotInstanceEnd.Y);
                    spCoordinates.Add(coordinateEnd);
                }
                Spatial.Geometry geoLine = YuLinTu.Spatial.Geometry.CreatePolyline(spCoordinates, 0);
                startNumber = i + 1;
                endNumber = numberEnd + 1;
                BuildLandBoundaryAddressCoil coil = new BuildLandBoundaryAddressCoil()
                {
                    LandID = dots[i].LandID,
                    LandNumber = dots[i].LandNumber,
                    ZoneCode = dots[i].ZoneCode,
                    CoilLength = ToolMath.CutNumericFormat(geoLine.Length(), 4),
                    Founder = "Admin",
                    CreationTime = DateTime.Now,
                    Description = "",
                    StartNumber = "J" + startNumber.ToString(),
                    EndNumber = "J" + endNumber.ToString(),
                    StartPointID = dots[i].ID,
                    EndPointID = dots[numberEnd].ID,
                    Shape = geoLine,
                };
                coilCollection.Add(coil);
            }
            return coilCollection;
        }

        /// <summary>
        /// 由图行生成线集合
        /// </summary>
        /// <param name="polygon">图行</param>
        /// <returns>线集合</returns>
        public static List<Spatial.Geometry> IntialSegmentLineShape(this GeoAPI.Geometries.IPolygon polygon)
        {
            if (polygon == null)
            {
                return null;
            }
            List<Spatial.Geometry> lineCollection = new List<Geometry>();
            Spatial.Geometry polygonShape = polygon as Spatial.Geometry;
            if (polygonShape != null)
            {
                Spatial.Geometry[] geoLines = polygonShape.ToSegmentLines();

                foreach (var line in geoLines)
                {
                    lineCollection.Add(line);
                }
            }
            return lineCollection;
        }

        #endregion

        #region 由生成线

        /// <summary>
        /// 有点生成线
        /// </summary>
        public static Geometry CreateLineByPoint(List<Geometry> geoList, int srid)
        {
            if (geoList == null || geoList.Count == 0)
                return null;
            List<Coordinate> coordList = new List<Coordinate>();
            foreach (var item in geoList)
            {
                if (item == null)
                    continue;
                Coordinate[] coords = item.ToCoordinates();
                for (int i = 0; i < coords.Length; i++)
                {
                    coordList.Add(coords[i]);
                }
            }
            if (coordList == null || coordList.Count == 0)
                return null;
            return YuLinTu.Spatial.Geometry.CreatePolyline(coordList, srid);
        }

        #endregion

        #region 生成图形西北角开始的坐标点集合

        /// <summary>
        /// 获取从西北角开始的坐标点集合
        /// </summary>
        public static KeyValue<bool, List<GeoAPI.Geometries.Coordinate>> GetWNOrderCoordinates(this Spatial.Geometry geo)
        {
            if (geo == null)
                return null;
            KeyValue<bool, List<GeoAPI.Geometries.Coordinate>> orderCoords = new KeyValue<bool, List<GeoAPI.Geometries.Coordinate>>();
            var g = geo.Instance;
            if (g is NetTopologySuite.Geometries.Polygon)
            {
                orderCoords = ParseCoordinates(g as NetTopologySuite.Geometries.Polygon);
            }
            else if (g is NetTopologySuite.Geometries.MultiPolygon)
            {
                var mg = g as NetTopologySuite.Geometries.MultiPolygon;
                foreach (var gs in mg.Geometries)
                {
                    var coords = ParseCoordinates(gs as NetTopologySuite.Geometries.Polygon);
                    if (coords == null)
                        continue;
                    orderCoords.Key = coords.Key;
                    orderCoords.Value.AddRange(coords.Value);
                }
            }
            return orderCoords;
        }

        /// <summary>
        /// 坐标点排序
        /// </summary>
        public static KeyValue<bool, List<GeoAPI.Geometries.Coordinate>> ParseCoordinates(NetTopologySuite.Geometries.Polygon polygon)
        {
            if (polygon == null)
                return null;
            KeyValue<bool, List<GeoAPI.Geometries.Coordinate>> orderCoords = new KeyValue<bool, List<GeoAPI.Geometries.Coordinate>>();
            orderCoords.Value = new List<GeoAPI.Geometries.Coordinate>();
            var coords = ParseCoordinates(polygon.Shell.Coordinates);
            if (coords != null)
            {
                orderCoords.Key = coords.Key;
                orderCoords.Value.AddRange(coords.Value);
            }
            foreach (var hole in polygon.Holes)
            {
                var cdts = ParseCoordinates(hole.Coordinates);
                if (cdts == null)
                    continue;
                orderCoords.Key = cdts.Key;
                orderCoords.Value.AddRange(cdts.Value);
            }
            return orderCoords;
        }

        /// <summary>
        /// 坐标点排序
        /// </summary>
        public static KeyValue<bool, List<GeoAPI.Geometries.Coordinate>> ParseCoordinates(GeoAPI.Geometries.Coordinate[] coords)
        {
            bool isCCW = false;
            bool fClosed = CglHelper.IsSame2(coords[0], coords[coords.Length - 1], 0.05 * 0.05);
            var coords1 = GeometryHelper.SortCoordsByWNOrder(coords, fClosed, out isCCW);
            if (isCCW)
            {
                for (int i = coords1.Length - 1, j = 0; i >= 0; --i, ++j)
                {
                    coords[j] = coords1[i];
                }
                isCCW = false;
            }
            else
            {
                for (int i = 0; i < coords1.Length; ++i)
                {
                    coords[i] = coords1[i];
                }
            }
            return new KeyValue<bool, List<GeoAPI.Geometries.Coordinate>> { Key = isCCW, Value = coords.ToList() };
        }

        #endregion
    }
}
