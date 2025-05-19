using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using DotSpatial.Projections;
using GeoAPI.Geometries;
using NetTopologySuite.Geometries;
using YuLinTu.Data;
using YuLinTu.Library.Log;
using YuLinTu.Spatial;
using YuLinTu.tGISCNet;
using File = System.IO.File;
using SpatialReference = YuLinTu.Spatial.SpatialReference;

namespace YuLinTu.Component.VectorDataTreatTask
{
    public class VectorDataProgress
    {
        /// <summary>
        /// 图形SRID
        /// </summary>
        private int srid;

        //private Dictionary<string, int> codeIndex;

        private string dkShapeFilePath;//用于判断是否读取了不同的地块shp

        /// <summary>
        /// 参数
        /// </summary>
        public VectorDataUpdateArgument DataArgument { get; set; }

        public static string ErrorInfo { get; private set; }

        public IDataSource Source { get; set; }

        public static SpatialReference GetByFile(string fileName)
        {
            var prjFile = Path.ChangeExtension(fileName, ".prj");
            if (!File.Exists(prjFile))
                throw new FileNotFoundException("获取坐标系失败", prjFile);
            var result = new SpatialReference(File.ReadAllText(prjFile));
            if (result.WKID == 0)
            {
                var pi = ProjectionInfo.Open(prjFile);
                var file = "";
                if (pi.Name != null)
                    file = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                        "Data/SpatialReferences/Projected Coordinate Systems/Gauss Kruger/CGCS2000",
                        pi.Name.Replace("_", " ") + ".prj");
                if (File.Exists(file))
                {
                    return new SpatialReference(File.ReadAllText(file));
                }
                result.WKID = 4490;
            }
            return result;
        }

        static private string CheckField(ShapeFile shp)
        {
            string err = "";
            var infoArray = typeof(QCDK).GetProperties();
            for (int i = 0; i < infoArray.Length; i++)
            {
                var info = infoArray[i];
                var index = shp.FindField(info.Name);
                switch (info.Name)
                {
                    case "CBFBM":
                        if (index == -1)
                        {
                            err = "shp文件未包含CBFBM字段；";
                        }
                        break;
                    case "DKBM":
                        if (index == -1)
                        {
                            err += "shp文件未包含DKBM字段；";
                        }
                        break;
                    case "QQDKBM":
                        if (index == -1)
                        {
                            err += "shp文件未包含QQDKBM字段；";
                        }
                        break;
                    case "Shape":
                        //if (index == -1)
                        //{
                        //    ErrorInfo += "shp文件未包含Shape字段；";
                        //}
                        break;
                }
            }
            return err;
        }

        static public void InitiallShapeLandList(string filePath, int srid, Action<int, List<QCDK>> DataAction, string zoneCode = "")
        {
            var dkList = new List<QCDK>();

            if (filePath == null || string.IsNullOrEmpty(filePath))
            {
                return;
            }
            //codeIndex.Clear();
            using (var shp = new ShapeFile())
            {
                var err = shp.Open(filePath);
                if (!string.IsNullOrEmpty(err))
                {
                    Log.WriteError(null, "", "读取地块Shape文件发生错误" + err);
                    return;
                }
                var codeIndex = new Dictionary<string, int>();

                ErrorInfo = CheckField(shp);
                if (!string.IsNullOrEmpty(ErrorInfo))
                    throw new Exception(filePath + ErrorInfo);
                int count = shp.GetRecordCount();
                foreach (var dk in ForEnumRecord<QCDK>(shp, filePath, codeIndex, srid, QCDK.CDKBM, zoneCode))
                {
                    dkList.Add(dk);
                    if (dkList.Count == 5000)
                    {
                        DataAction(count, dkList);
                        dkList.Clear();
                    }
                }
                if (dkList.Count > 0)
                {
                    DataAction(count, dkList);
                    dkList.Clear();
                }
            }
        }

        static public List<QCDK> InitiallShapeLandList(string filePath, int srid, string zoneCode = "")
        {
            var dkList = new List<QCDK>();

            if (filePath == null || string.IsNullOrEmpty(filePath))
            {
                return dkList;
            }
            //codeIndex.Clear();
            using (var shp = new ShapeFile())
            {
                var err = shp.Open(filePath);
                if (!string.IsNullOrEmpty(err))
                {
                    Log.WriteError(null, "", "读取地块Shape文件发生错误" + err);
                    return null;
                }
                var codeIndex = new Dictionary<string, int>();

                ErrorInfo = CheckField(shp);
                if (!string.IsNullOrEmpty(ErrorInfo))
                    throw new Exception(filePath + ErrorInfo);

                foreach (var dk in ForEnumRecord<QCDK>(shp, filePath, codeIndex, srid, QCDK.CDKBM, zoneCode))
                {
                    dkList.Add(dk);
                }
            }
            return dkList;
        }

        static public IEnumerable<T> ForEnumRecord<T>(ShapeFile shp, string fileName, Dictionary<string, int> codeIndex,
          int srid, string mainField = "", string zoneCode = "", bool setGeo = true) where T : class, new()
        {
            var infoArray = typeof(T).GetProperties();
            var fieldIndex = new Dictionary<string, int>();
            bool isSelect = (mainField != "" && zoneCode != "") ? true : false;

            int dkbmindex = -1;
            for (int i = 0; i < infoArray.Length; i++)
            {
                var info = infoArray[i];

                var index = shp.FindField(info.Name);

                if (index >= 0)
                {
                    fieldIndex.Add(info.Name, index);
                }

                if (info.Name == mainField)
                {
                    dkbmindex = index;
                }
            }

            if (codeIndex.Count > 0)
            {
                foreach (var item in codeIndex)
                {
                    if (isSelect)
                    {
                        if (dkbmindex < 0)
                            continue;

                        if (!item.Key.StartsWith(zoneCode))
                            continue;
                    }
                    var en = new T();
                    for (int i = 0; i < infoArray.Length; i++)
                    {
                        var info = infoArray[i];
                        if (!fieldIndex.ContainsKey(info.Name))
                            continue;
                        info.SetValue(en, FieldValue(item.Value, fieldIndex[info.Name], shp, info), null);
                    }
                    ObjectExtension.SetPropertyValue(en, "Shape", shp.GetGeometry(item.Value, srid));
                    yield return en;
                }
            }
            else
            {
                var shapeCount = shp.GetRecordCount();
                for (int i = 0; i < shapeCount; i++)
                {
                    var en = new T();

                    if (isSelect)
                    {
                        if (dkbmindex < 0)
                            continue;

                        var strValue = shp.GetFieldString(i, dkbmindex);
                        if (strValue == null)
                        {
                            continue;
                        }
                        if (!codeIndex.ContainsKey(strValue))
                            codeIndex.Add(strValue, i);
                        if (!strValue.StartsWith(zoneCode))
                            continue;
                    }
                    for (int j = 0; j < infoArray.Length; j++)
                    {
                        var info = infoArray[j];
                        if (!fieldIndex.ContainsKey(info.Name))
                            continue;
                        var value = FieldValue(i, fieldIndex[info.Name], shp, info);
                        info.SetValue(en, value, null);

                    }
                    if (setGeo)
                        ObjectExtension.SetPropertyValue(en, "Shape", shp.GetGeometry(i, srid));
                    yield return en;
                }
            }

        }
        /// <summary>
        /// 字段值获取
        /// </summary>
        static private object FieldValue(int row, int colum, ShapeFile dataReader, PropertyInfo info)
        {
            object value = null;
            if (info.Name == "BSM")
            {
                int bsm = 0;
                int.TryParse(dataReader.GetFieldString(row, colum), out bsm);
                value = bsm;
            }
            else if (info.Name.EndsWith("MJ") || info.Name.EndsWith("MJM") || info.Name == "CD")
            {
                double scmj = 0;
                var mjstr = dataReader.GetFieldString(row, colum);
                double.TryParse(mjstr.IsNullOrEmpty() ? "0" : mjstr, out scmj);
                value = scmj;
            }
            else
            {
                value = dataReader.GetFieldString(row, colum);
                value = value == null ? "" : value;
            }
            return value;
        }

        /// <summary>
        /// 坐标转换
        /// </summary>
        /// <param name="geo"></param>
        /// <param name="sreproject"></param>
        /// <param name="dreproject"></param>
        /// <param name="srid"></param>
        /// <returns></returns>
        public static YuLinTu.Spatial.Geometry ReprojectShape(YuLinTu.Spatial.Geometry geo, ProjectionInfo sreproject, ProjectionInfo dreproject, int srid)
        {
            var geos = geo.ToSingleGeometries();
            YuLinTu.Spatial.Geometry geometry = null;
            foreach (var g in geos)
            {
                GeoAPI.Geometries.Coordinate[] shels = null;
                GeoAPI.Geometries.ILinearRing[] hols = null;
                var list = new List<GeoAPI.Geometries.Coordinate>();
                if (g.Instance is Polygon)
                {
                    var pg = g.Instance as Polygon;
                    shels = pg.Shell.Coordinates;
                    hols = pg.Holes;
                }
                else if (g.Instance is Point)
                {
                    var pg = g.Instance;
                    shels = pg.Coordinates;
                    hols = new LinearRing[0];
                }
                else if (g.Instance is LinearRing)
                {
                    var pg = g.Instance as LinearRing;
                    shels = pg.Coordinates;
                    hols = new LinearRing[0];
                }
                foreach (var sl in shels)
                {
                    var xy = new double[] { sl.X, sl.Y };
                    Reproject.ReprojectPoints(xy, new double[] { 0 }, sreproject, dreproject, 0, 1);
                    list.Add(new GeoAPI.Geometries.Coordinate(xy[0], xy[1]));
                }
                var nholes = new List<GeoAPI.Geometries.ILinearRing>();
                foreach (var ho in hols)
                {
                    var hlist = new List<GeoAPI.Geometries.Coordinate>();
                    foreach (var h in ho.Coordinates)
                    {
                        var xy = new double[] { h.X, h.Y };
                        Reproject.ReprojectPoints(xy, new double[] { 0 }, sreproject, dreproject, 0, 1);
                        hlist.Add(new GeoAPI.Geometries.Coordinate(xy[0], xy[1]));
                    }
                    var hlinearRing = new LinearRing(hlist.ToArray());
                    nholes.Add(hlinearRing);
                }
                var linearRing = new LinearRing(list.ToArray());

                var polygon = new Polygon(linearRing, nholes.ToArray());
                if (geometry == null)
                    geometry = YuLinTu.Spatial.Geometry.FromInstance(polygon);
                else
                    geometry = geometry.Union(YuLinTu.Spatial.Geometry.FromInstance(polygon));
            }
            if (geometry != null)
                geometry.SpatialReference = new SpatialReference(srid);
            return geometry;
        }

        /// <summary>
        /// 拓扑检查
        /// </summary>
        /// <returns></returns>
        public string CheckTopology(List<QCDK> landlist)
        {
            if (landlist.Count == 0)
                return "";
            StringBuilder stringBuilder = new StringBuilder();
            var distanc = 0.05 * 0.05;
            var replist = new List<CheckGeo>();
            foreach (var item in landlist)
            {
                if (item.DKBM == null || item.DKBM.Length != 19)
                {
                    stringBuilder.AppendLine($"地块编码为{item.DKBM}的数据编码不规范!");
                    continue;
                }
                var dkgeo = item.Shape as YuLinTu.Spatial.Geometry;
                if (dkgeo == null)
                    continue;
                if (!dkgeo.IsValid())
                {
                    stringBuilder.AppendLine($"地块编码为{item.DKBM}的矢量图形无效!");
                    continue;
                }

                if (dkgeo.Area() < 1)
                {
                    stringBuilder.AppendLine($"地块编码为{item.DKBM}的矢量图形是碎面!");
                    continue;
                }

                if (dkgeo.ToSingleGeometries().Count() > 1)
                {
                    stringBuilder.AppendLine($"地块编码为{item.DKBM}的矢量图形包含多个图元!");
                    continue;
                }
                var allcoords = dkgeo.ToCoordinates().ToList();
                allcoords = allcoords.OrderBy(o => o.X).ToList();
                for (int i = 0; i < allcoords.Count - 1; i++)
                {
                    if (allcoords[i].X == allcoords[i + 1].X &&
                        allcoords[i].Y == allcoords[i + 1].Y)
                        continue;
                    if (allcoords[i].X - allcoords[i + 1].X > 0.05)
                        continue;
                    var xd = allcoords[i].X - allcoords[i + 1].X;
                    var yd = allcoords[i].Y - allcoords[i + 1].Y;
                    if (xd * xd + yd * yd < distanc)
                    {
                        stringBuilder.AppendLine($"地块编码为{item.DKBM}的矢量图形节点({allcoords[i].X},{allcoords[i].Y}),({allcoords[i + 1].X},{allcoords[i + 1].Y})距离小于0.05米!");
                    }
                }
                //var geos = dkgeo.ToSingleGeometries();
                List<IGeometry> geocs = new List<IGeometry>();
                var bd = dkgeo.Instance.Boundary;
                if (bd.GeometryType == "LinearRing")
                {
                    geocs.Add(dkgeo.Instance);
                }
                else if (bd.GeometryType == "GeometryCollection")
                {
                    foreach (var g in ((NetTopologySuite.Geometries.GeometryCollection)bd).Geometries)
                    {
                        geocs.Add(dkgeo.Instance);
                    }
                }
                //var geocs = ((NetTopologySuite.Geometries.GeometryCollection)dkgeo.Instance.Boundary).Geometries;

                foreach (var gitem in geocs)
                {
                    var coordArr = gitem.Coordinates.ToList();
                    for (int i = 0; i < coordArr.Count - 2; i++)
                    {
                        var angle = CalcAngle(coordArr[i], coordArr[i + 1], coordArr[i + 2]);
                        if (angle != 0 && (angle < 5 || angle > 355))
                        {
                            stringBuilder.AppendLine($"地块编码为{item.DKBM}的矢量图形存在狭长角,度数为{angle}!");
                            continue;
                        }
                    }
                    var tangle = CalcAngle(coordArr[coordArr.Count - 2], coordArr[0], coordArr[1]);
                    if (tangle != 0 && (tangle < 5 || tangle > 355))
                    {
                        stringBuilder.AppendLine($"地块编码为{item.DKBM}的矢量图形存在狭长角!,度数为{tangle}");
                    }
                }
                replist.Add(new CheckGeo()
                {
                    DKBM = item.DKBM,
                    Shape = (item.Shape as YuLinTu.Spatial.Geometry).Buffer(-0.001),
                    MinX = allcoords.Min(t => t.X),
                    MaxX = allcoords.Max(t => t.X)
                });
            }

            replist = replist.OrderBy(l => l.MinX).ToList();

            for (int i = 0; i < replist.Count - 1; i++)
            {
                for (int j = i + 1; j < replist.Count - 2; j++)
                {
                    if (replist[i].MaxX < replist[j].MinX)
                    {
                        break;
                    }
                    if (replist[i].Shape.Intersects(replist[j].Shape))
                    {
                        stringBuilder.AppendLine($"地块编码为{replist[i].DKBM}的矢量与地块编码为{replist[j].DKBM}的矢量存在重叠！");
                    }
                }

            }
            CheckNodeRepeat(landlist, stringBuilder);
            return stringBuilder.ToString();
        }

        /// <summary>
        /// 检查相邻要素节点重复
        /// </summary>
        private void CheckNodeRepeat(List<QCDK> landlist, StringBuilder stringBuilder)
        {
            AreaNodeRepeatCheck check = new AreaNodeRepeatCheck();
            var geolist = new List<CheckGeometry>();
            for (int i = 0; i < landlist.Count; i++)
            {
                geolist.Add(new CheckGeometry()
                {
                    index = i,
                    Graphic = landlist[i].Shape as YuLinTu.Spatial.Geometry
                });
            }
            check.DoCheck(geolist, 0.05, 0.001, (i, j, x1, y1, x2, y2, len) => { }, (progress) =>
            {
            });
        }

        /// <summary>
        /// 计算角度
        /// </summary> 
        private double CalcAngle(GeoAPI.Geometries.Coordinate coordinate1, GeoAPI.Geometries.Coordinate coordinate2, GeoAPI.Geometries.Coordinate coordinate3)
        {
            double max = coordinate1.X - coordinate2.X;
            double may = coordinate1.Y - coordinate2.Y;
            double mbx = coordinate3.X - coordinate2.X;
            double mby = coordinate3.Y - coordinate2.Y;
            double v1 = (max * mbx) + (may * mby);
            double maval = Math.Sqrt(max * max + may * may);
            double mbval = Math.Sqrt(mbx * mbx + mby * mby);
            double cosM = v1 / (maval * mbval);
            double angleAMB = Math.Acos(cosM) * 180 / Math.PI;
            return angleAMB;
        }

    }

    public class CheckGeo
    {
        public Spatial.Geometry Shape { get; set; }
        public string DKBM { get; set; }

        public double MinX { get; set; }
        public double MaxX { get; set; }
    }
}
