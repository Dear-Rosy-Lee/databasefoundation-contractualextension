using Common.CShapeExport;
using NetTopologySuite.Geometries;
using NetTopologySuite.IO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using YuLinTu.Data;
using YuLinTu.Data.Dynamic;
using YuLinTu.Data.Shapefile;
using YuLinTu.Spatial;
using Geometry = YuLinTu.Spatial.Geometry;

namespace YuLinTu.Component.CoordinateTransformTask
{
    public class OperateShapeFile
    {
        #region Property
        /// <summary>
        /// 中央经线值
        /// </summary>
        private decimal CentralMeridian { get; set; }
        /// <summary>
        /// 坐标系名称
        /// </summary>
        private string CoordName { get; set; }
        /// <summary>
        /// 源shape文件路径
        /// </summary>
        private string OldShapePath { get; set; }
        /// <summary>
        /// 新shape文件存放路径 
        /// </summary>
        private string NewShapePath { get; set; }
        #endregion

        #region Fields
        //private ShapeLayer layer;

        private ShapeFileExporter<object> writer;
        #endregion

        #region Ctor

        public OperateShapeFile(string _oldShapePath, string _newShapePath)
        {
            OldShapePath = _oldShapePath;
            NewShapePath = _newShapePath;
            writer = new ShapeFileExporter<object>();
        }
        #endregion

        public void ReadShapeData(Action<ShapeLayer, object, int> EnumEntity, Action NoData)
        {
            //初始化文件名称
            string filepath = System.IO.Path.GetDirectoryName(OldShapePath);
            string shpFullName = NewShapePath;
            string newShpName = System.IO.Path.GetFileNameWithoutExtension(NewShapePath);

            if (!string.IsNullOrEmpty(OperateShapeFile.DeleteShapeFile(shpFullName)))
                return;
            string oldShpNname = System.IO.Path.GetFileNameWithoutExtension(OldShapePath);

            //var writer = new ShapeFileExporter<object>();
            var olayer = writer.OpenLayer(OldShapePath);
            if (olayer.GetRecordCount() == 0)
                return;

            var ds = ProviderShapefile.CreateDataSource(filepath, false) as IDbContext;
            var dq = new DynamicQuery(ds);
            if (dq.Count(null, oldShpNname) == 0)
                return;
            //实例化metedata
            ShapefileDataReader sdr = new ShapefileDataReader(OldShapePath, GeometryFactory.Default);
            if (sdr.RecordCount == 0)
            {
                NoData();
                return;
            }
            var shpType = sdr.ShapeHeader.ShapeType;
            sdr.Dispose();
            var fieldList = new List<FieldInfo>();
            foreach (var item in sdr.DbaseHeader.Fields)
            {
                fieldList.Add(new FieldInfo()
                {
                    Name = item.Name,
                    Type = item.Type.ConvertType(),
                    Length = item.Length,
                    Precision = item.DecimalCount
                });
            }
            //初始化图层
            //var geo = YuLinTu.Spatial.Geometry.FromInstance(sdr.ShapeHeader.ShapeType); 
            //.GetPropertyValue(landShapeList[0], "Shape") as YuLinTu.Spatial.Geometry; 
            try
            {
                var layer = writer.InitiallLayer(shpFullName, (EShapeType)shpType, "", fieldList);
                if (layer == null)
                    return;
                dq.ForEach(null, oldShpNname, (i, p, en) =>
                {
                    EnumEntity(layer, en, sdr.RecordCount);
                    return true;
                });
                layer.Dispose();
            }
            catch (Exception ex)
            {
                OperateShapeFile.DeleteShapeFile(shpFullName); 
                throw ex;
            }
        }


        public bool WriteShapeFile(ShapeLayer layer, object shpLandItem, Geometry geometry)
        {
            YuLinTu.ObjectExtension.SetPropertyValue(shpLandItem, "Shape", geometry);
            return writer.WriteVectorFile(layer, new List<object>() { shpLandItem });
        }

        public Geometry ConvertCoordinatesToGeometry(List<Coordinate> newCoordinates, Geometry g)
        {
            Geometry geometry = null;
            if (g.GeometryType == eGeometryType.Point)
                geometry = YuLinTu.Spatial.Geometry.CreatePoint(newCoordinates.FirstOrDefault());
            else if (g.GeometryType == eGeometryType.Polyline)
                geometry = YuLinTu.Spatial.Geometry.CreatePolyline(newCoordinates);
            else if (g.GeometryType == eGeometryType.Polygon)
                geometry = YuLinTu.Spatial.Geometry.CreatePolygon(newCoordinates);
            else if (g.GeometryType == eGeometryType.MultiPolygon)
            {
                var geometrys = new List<Geometry> { YuLinTu.Spatial.Geometry.CreatePolygon(newCoordinates) };
                geometry = YuLinTu.Spatial.Geometry.CreateMultiPolygon(geometrys);
            }
            else if (g.GeometryType == eGeometryType.MultiPolyline)
            {
                geometry = YuLinTu.Spatial.Geometry.CreateMultiPolyline(new List<List<Coordinate>> { newCoordinates });
            }
            return geometry;
        }

        public void Dispose()
        {
        }


        #region 复制PRJ文件

        public List<CoordinateMap> ReadExcel()
        {
            string excelPath = Environment.CurrentDirectory + "\\Template\\坐标系对应EPSG代号、经度范围、中央经线.xls";
            if (!File.Exists(excelPath)) return null;

            ExcelReaderCoordMap readerCoord = new ExcelReaderCoordMap();
            return readerCoord.ReadCoordInfo(excelPath);
        }

        public string CopyToPrjFile(List<CoordinateMap> coordsMap)
        {
            if (coordsMap == null) return "";

            var coords = coordsMap.Where(c => c.CentralMeridian == CentralMeridian).ToList();
            var coord = coords.Where(c => c.CoordinateName.Contains(CoordName.Left(4))).ToList();
            var data = coord.Where(c => c.CoordinateName != CoordName).ToList();
            if (data.Count < 1)
            {
                return "目标PRJ文件名称匹配失败！";
            }
            string targetPrjName = data[0].CoordinateName.Replace("_", " ") + ".prj";
            string targetPrjFullName = "";
            string targetPrjPath = Environment.CurrentDirectory + @"\Data\SpatialReferences\Projected Coordinate Systems\Gauss Kruger\";

            string[] allPrjDir = Directory.GetFiles(targetPrjPath, "*.prj", SearchOption.AllDirectories);
            foreach (var prjPath in allPrjDir)
            {
                string prjName = Path.GetFileNameWithoutExtension(prjPath) + ".prj";
                if (prjName == targetPrjName)
                {
                    targetPrjFullName = prjPath;
                    break;
                }
            }
            if (!File.Exists(targetPrjFullName) || targetPrjFullName.IsNullOrEmpty())
            {
                return "源PRJ路径下不存在该源文件，复制失败！";
            }

            FileInfo fileInfo = new FileInfo(targetPrjFullName);
            fileInfo.CopyTo(Path.ChangeExtension(NewShapePath, ".prj"), true);

            return "true";
        }

        public string CopyToPrjFile()
        {
            string oldPrjPath = Path.ChangeExtension(OldShapePath, ".prj");
            string newPrjPath = Path.ChangeExtension(NewShapePath, ".prj");
            if (!File.Exists(oldPrjPath))
                return "源PRJ路径下不存在该源文件，复制失败！";

            FileInfo fileInfo = new FileInfo(oldPrjPath);
            fileInfo.CopyTo(newPrjPath, true);

            return "true";
        }

        #endregion

        public decimal CalculateBH(Geometry geometry)
        {
            var pro = geometry.SpatialReference.CreateProjectionInfo();
            //获取中央经线
            decimal centralMeridian = pro.CentralMeridian == null ? 0.0M : Convert.ToDecimal(pro.CentralMeridian.GetValueOrDefault());
            CentralMeridian = centralMeridian;
            CoordName = pro.Name;

            return Convert.ToDecimal(centralMeridian / 3.0M);
        }

        /// <summary>
        /// 删除矢量文件
        /// </summary> 
        static public string DeleteShapeFile(string shpPath)
        {
            if (!File.Exists(shpPath))
                return null;
            try
            {
                if (File.Exists(shpPath))
                    File.Delete(shpPath);
                var fpath = Path.ChangeExtension(shpPath, ".dbf");
                if (File.Exists(fpath))
                    File.Delete(fpath);
                fpath = Path.ChangeExtension(shpPath, ".shx");
                if (File.Exists(fpath))
                    File.Delete(fpath);
                fpath = Path.ChangeExtension(shpPath, ".sbn");
                if (File.Exists(fpath))
                    File.Delete(fpath);
                fpath = Path.ChangeExtension(shpPath, ".sbx");
                if (File.Exists(fpath))
                    File.Delete(fpath);
                fpath = Path.ChangeExtension(shpPath, ".prj");
                if (File.Exists(fpath))
                    File.Delete(fpath);
                fpath = Path.ChangeExtension(shpPath, ".shp.xml");
                if (File.Exists(fpath))
                    File.Delete(fpath);
                fpath = Path.ChangeExtension(shpPath, ".cpg");
                if (File.Exists(fpath))
                    File.Delete(fpath);
            }
            catch (Exception ex)
            {
                return string.Format("删除文件{0}出错,{1}", Path.GetFileName(shpPath), ex.Message);
            }
            return null;
        }
    }

    public static class Strhelper
    {
        public static string Left(this string str, int leng)
        {
            if (str.Length < leng)
                throw new ArgumentOutOfRangeException();
            return str.Substring(0, leng);
        }
    }
}
