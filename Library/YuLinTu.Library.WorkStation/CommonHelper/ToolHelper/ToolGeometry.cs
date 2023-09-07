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

namespace YuLinTu.Library.WorkStation
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
                var dotInstanceStart = dots[i].Shape.Instance as GeoAPI.Geometries.IPoint;
                GeoAPI.Geometries.IPoint dotInstanceEnd = null;
                if (i == dots.Count - 1)
                {
                    numberEnd = 0;
                    dotInstanceEnd = dots[numberEnd].Shape.Instance as GeoAPI.Geometries.IPoint;
                }
                else
                {
                    numberEnd = i + 1;
                    dotInstanceEnd = dots[numberEnd].Shape.Instance as GeoAPI.Geometries.IPoint;
                }
                List<Spatial.Coordinate> spCoordinates = new List<Spatial.Coordinate>();
                YuLinTu.Spatial.Coordinate coordinateStart = new Spatial.Coordinate(dotInstanceStart.X, dotInstanceStart.Y);
                YuLinTu.Spatial.Coordinate coordinateEnd = new Spatial.Coordinate(dotInstanceEnd.X, dotInstanceEnd.Y);
                spCoordinates.Add(coordinateStart);
                spCoordinates.Add(coordinateEnd);
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
    }
}
