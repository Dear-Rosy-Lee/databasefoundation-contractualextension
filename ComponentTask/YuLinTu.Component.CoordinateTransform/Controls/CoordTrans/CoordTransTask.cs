using Common.CShapeExport;
using DotSpatial.Projections;
using NetTopologySuite.Geometries;
using NetTopologySuite.IO;
using ShapeProcess;
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
    /// <summary>
    /// 坐标系转换
    /// </summary>
    [TaskDescriptor(IsLanguageName = false, Name = "坐标系转换", Gallery = "矢量数据处理",
        UriImage16 = "pack://application:,,,/YuLinTu.Resources;component/Images/16/store.png",
        UriImage24 = "pack://application:,,,/YuLinTu.Resources;component/Images/24/store.png")]
    public class CoordTransTask : Task
    {
        #region Fields
        private Param4 param4;
        private Param7 param7;
        private DataSave dataSave;

        #endregion

        #region Ctor
        public CoordTransTask()
        {
            Name = "坐标系转换";
            Description = "根据选择目标坐标系或输入参数对矢量文件进行坐标系转换";
        }
        #endregion

        #region Methods - Override
        protected override void OnGo()
        {
            CoordTransArgument args = Argument as CoordTransArgument;

            if (!ValidateArgs(args))
                return;

            string savePath = args.NewShapePath;
            if (savePath.Equals(Path.GetDirectoryName(args.OldShapePath[0])))
            {
                savePath = Path.Combine(savePath, "转换后矢量文件");
                this.ReportInfomation("新Shape文件存储位置与原Shape位置相同，新Shape文件存储位置改为：" + savePath);
            }
            if (!Directory.Exists(savePath))
            {
                Directory.CreateDirectory(savePath);
            }
            if (args.EnabledCustomArgs)
            {
                ReadShapeFileToGeometry(args.OldShapePath.ToArray(), savePath, args);
            }
            else
            {
                int index = 0;
                var sr = SpatialReferences.Create(int.Parse(args.DestinationPrj));
                foreach (var sp in args.OldShapePath)
                {
                    var cct = new CoordChangeTask();
                    cct.ProgressChanged += (s, e) => { this.ReportProgress(e); };
                    cct.Alert += (s, e) => { this.ReportAlert(e); };
                    cct.TransformNoArgs(sp, savePath, sr, args.OldShapePath.Count, index, cct.ChageDataCoordnate);
                    index++;
                }
                this.ReportProgress(100, "完成");
            }
        }
        #endregion

        #region Methods - Validate

        //参数检查
        private bool ValidateArgs(CoordTransArgument args)
        {
            if (args.OldShapePath == null || args.OldShapePath.Count == 0)
            {
                this.ReportError("请选择源Shape文件路径!");
                return false;
            }
            if (string.IsNullOrEmpty(args.NewShapePath))
            {
                this.ReportError("请选择新Shape文件的保存路径!");
                return false;
            }

            if (args.EnabledCustomArgs)
            {
                if (string.IsNullOrEmpty(args.PyX.ToString()))
                {
                    this.ReportError("请输入正确的平移参数X.");
                    return false;
                }
                if (string.IsNullOrEmpty(args.PyY.ToString()))
                {
                    this.ReportError("请输入正确的平移参数Y.");
                    return false;
                }
                if (string.IsNullOrEmpty(args.RotateAngleT.ToString()))
                {
                    this.ReportError("请输入正确的旋转参数T.");
                    return false;
                }
                if (string.IsNullOrEmpty(args.Scale.ToString()))
                {
                    this.ReportError("请输入正确的尺度参数K.");
                    return false;
                }
            }
            else
            {
                var wkid = 0;
                int.TryParse(args.DestinationPrj.Trim(), out wkid);
                var sp = SpatialReferences.Create(wkid);
                var name = sp.CreateProjectionInfo().Name;
                if (args.OldShapePrjName.Equals(name))
                {
                    this.ReportError("原坐标系与目标坐标系相同！");
                    return false;
                }
                if (CoordTransArgument.ZBXCWTS == args.OldShapePrjName)
                {
                    this.ReportError(CoordTransArgument.ZBXCWTS);
                    return false;
                }
            }

            this.ReportInfomation(string.Format("导出参数设置正确。"));

            return true;
        }

        #endregion

        #region Methods - Private

        /// <summary>
        /// 参数坐标转换
        /// </summary>
        private void ReadShapeFileToGeometry(string[] shapefiles, string savePath, CoordTransArgument args)
        {
            this.ReportProgress(0, "开始");
            int acount = shapefiles.Length;
            int index = 0;
            foreach (var filepath in shapefiles)
            {
                this.ReportInfomation(string.Format("开始转换Shape文件:" + System.IO.Path.GetFileName(filepath)));
                Geometry geometry = null;

                string newShapeFileName = Path.Combine(savePath, Path.GetFileName(filepath));
                OperateShapeFile operateShape = new OperateShapeFile(filepath, newShapeFileName);

                int trfmCount = 0;
                operateShape.ReadShapeData((shpLandItem, dataCount) =>
                {
                    try
                    {
                        this.ReportProgress((int)(((index / (double)acount) + (++trfmCount / (double)dataCount / (double)acount)) * 100), "正在转换...");

                        var geo = YuLinTu.ObjectExtension.GetPropertyValue(shpLandItem, "Shape") as YuLinTu.Spatial.Geometry;
                        if (geo == null) return;

                        if (geo.GeometryType.ToString().Contains("Polygon"))
                            geometry = Transform4ParamPolygon(geo, args);
                        else
                            geometry = Transform4Param(geo.ToCoordinates(), args);

                        if (!operateShape.WriteShapeFile(shpLandItem, geometry))
                        {
                            operateShape.Dispose();
                            OperateShapeFile.DeleteShapeFile(newShapeFileName);
                            this.ReportError(string.Format("shape数据无效，无法导入"));
                            return;
                        }
                    }
                    catch (Exception ex)
                    {
                        operateShape.Dispose();
                        OperateShapeFile.DeleteShapeFile(newShapeFileName);
                        this.ReportError(string.Format("shape数据无效，无法导入"));
                        return;
                    }
                }, () =>
                 {
                     operateShape.Dispose();
                     OperateShapeFile.DeleteShapeFile(newShapeFileName);
                     this.ReportError(string.Format("shape数据无效，无法导入"));
                 });
                index++;
                operateShape.Dispose();
            }
            this.ReportProgress(100, "完成");
            this.ReportInfomation(string.Format("Shape文件转换完成。"));

        }

        private Geometry Transform4Param(Coordinate[] coord, CoordTransArgument args)
        {
            double x1, y1;
            double X2, Y2;
            double angle, m;

            List<Coordinate> newCoordinates = new List<Coordinate>();
            for (int i = 0; i < coord.Length; i++)
            {
                x1 = coord[i].X;
                y1 = coord[i].Y;
                m = args.Scale;
                angle = args.RotateAngleT;

                //X2 = args.PyX + (x1 * Math.Cos(angle) - y1 * Math.Sin(angle)) + (m * x1 * Math.Cos(angle) - m * y1 * Math.Sin(angle));
                //Y2 = args.PyY + (x1 * Math.Sin(angle) + y1 * Math.Cos(angle)) + (m * x1 * Math.Sin(angle) + m * y1 * Math.Cos(angle));

                X2 = args.PyX + m * x1 * Math.Cos(angle) - m * y1 * Math.Sin(angle);
                Y2 = args.PyY + m * x1 * Math.Sin(angle) + m * y1 * Math.Cos(angle);

                newCoordinates.Add(new Coordinate(X2, Y2));
            }

            return YuLinTu.Spatial.Geometry.CreatePolyline(newCoordinates);
        }

        /// <summary>
        /// 四参数的坐标转换
        /// </summary> 
        private Geometry Transform4ParamPolygon(Geometry geo, CoordTransArgument args)
        {
            double x1, y1;
            double X2, Y2;
            double angle, m;
            var geometrys = geo.ToSingleGeometries();
            Geometry newgeo = null;
            List<Coordinate[]> coordinates;

            foreach (var geos in geometrys)
            {
                List<Coordinate> newCoordinates = new List<Coordinate>();
                coordinates = geos.ToGroupCoordinates();
                var holes = new List<List<Coordinate>>();
                for (int k = 0; k < coordinates.Count; k++)
                {
                    var coorData = coordinates[k];
                    if (k > 0)//有洞的数据
                    {
                        var holeCoordinates = new List<Coordinate>();
                        holeCoordinates.AddRange(coorData.Select(t =>
                            new Coordinate(
                                args.PyX + args.Scale * t.X * Math.Cos(args.RotateAngleT) - args.Scale * t.Y * Math.Sin(args.RotateAngleT),
                                args.PyY + args.Scale * t.X * Math.Sin(args.RotateAngleT) + args.Scale * t.Y * Math.Cos(args.RotateAngleT))));

                        holes.Add(holeCoordinates);
                        continue;
                    }

                    for (int i = 0; i < coorData.Length; i++)
                    {
                        x1 = coorData[i].X;
                        y1 = coorData[i].Y;

                        X2 = args.PyX + args.Scale * x1 * Math.Cos(args.RotateAngleT) - args.Scale * y1 * Math.Sin(args.RotateAngleT);
                        Y2 = args.PyY + args.Scale * x1 * Math.Sin(args.RotateAngleT) + args.Scale * y1 * Math.Cos(args.RotateAngleT);

                        newCoordinates.Add(new Coordinate(X2, Y2));
                    }
                }
                if (newgeo == null)
                {
                    newgeo = YuLinTu.Spatial.Geometry.CreatePolygon(newCoordinates, holes);
                }
                else
                {
                    newgeo = newgeo.Union(YuLinTu.Spatial.Geometry.CreatePolygon(newCoordinates, holes));
                }
            }
            return newgeo;
        }

        private List<Coordinate> Transform7Param(Coordinate[] coord, CoordTransArgument args)
        {
            double x1, y1;
            double X2, Y2;
            double angle, m;

            List<Coordinate> newCoordinates = new List<Coordinate>();
            for (int i = 0; i < coord.Length; i++)
            {
                x1 = coord[i].X;
                y1 = coord[i].Y;
                m = args.Scale;
                angle = args.RotateAngleT;


                //newCoordinates.Add(new Coordinate(X2, Y2));
            }

            return newCoordinates;
        }

        public bool DeleteShapeFile(string shpPath, out string exmsg)
        {
            try
            {
                if (File.Exists(shpPath))
                    File.Delete(shpPath);
                var dbfPath = Path.ChangeExtension(shpPath, ".dbf");
                if (File.Exists(dbfPath))
                    File.Delete(dbfPath);
                dbfPath = Path.ChangeExtension(shpPath, ".shx");
                if (File.Exists(dbfPath))
                    File.Delete(dbfPath);
                dbfPath = Path.ChangeExtension(shpPath, ".sbn");
                if (File.Exists(dbfPath))
                    File.Delete(dbfPath);
                dbfPath = Path.ChangeExtension(shpPath, ".sbx");
                if (File.Exists(dbfPath))
                    File.Delete(dbfPath);
                dbfPath = Path.ChangeExtension(shpPath, ".prj");
                if (File.Exists(dbfPath))
                    File.Delete(dbfPath);
            }
            catch (Exception ex)
            {
                exmsg = ex.Message;
                return false;
            }
            exmsg = null;
            return true;
        }
        private DBFFieldType ConvertToDBFFieldType(Type type)
        {
            if (type == typeof(string))
            {
                return DBFFieldType.FTString;
            }
            if (type == typeof(int) || type == typeof(int?) ||
                type == typeof(long) || type == typeof(long?))
            {
                return DBFFieldType.FTInteger;
            }
            if (type == typeof(double) || type == typeof(double?) ||
                type == typeof(decimal) || type == typeof(decimal?) ||
                type == typeof(float) || type == typeof(float?))
            {
                return DBFFieldType.FTDouble;
            }
            if (type == typeof(DateTime) || type == typeof(DateTime?))
            {
                return DBFFieldType.FTDate;
            }
            if (type == typeof(short) || type == typeof(short?) ||
                type == typeof(byte) || type == typeof(byte?))
            {
                return DBFFieldType.FTShort;
            }
            return DBFFieldType.FTInvalid;
        }
        public static EShapeType GeoTypeConvert(Spatial.eGeometryType geoType)
        {
            switch (geoType)
            {
                case Spatial.eGeometryType.Unknown:
                    return EShapeType.SHPT_NULL;
                case Spatial.eGeometryType.Point:
                    return EShapeType.SHPT_POINT;
                case Spatial.eGeometryType.MultiPoint:
                    return EShapeType.SHPT_MULTIPOINT;
                case Spatial.eGeometryType.Polyline:
                    return EShapeType.SHPT_ARC;
                case Spatial.eGeometryType.MultiPolyline:
                    return EShapeType.SHPT_ARC;
                case Spatial.eGeometryType.Polygon:
                    return EShapeType.SHPT_POLYGON;
                case Spatial.eGeometryType.MultiPolygon:
                    return EShapeType.SHPT_POLYGON;
                case Spatial.eGeometryType.GeometryCollection:
                    return EShapeType.SHPT_NULL;
                default:
                    return EShapeType.SHPT_NULL;
            }
        }

        /// <summary>
        /// 无参数的坐标转换
        /// </summary> 
        private void TransformNoArgs(string[] sourcePaths, string newPath, SpatialReference destprj)
        {
            this.ReportProgress(0, "开始");
            this.ReportInfomation(string.Format("开始转换Shape文件。"));
            int acount = sourcePaths.Length;
            int index = 0;
            foreach (var oldPath in sourcePaths)
            {
                string filepath = System.IO.Path.GetDirectoryName(oldPath);
                string oldShpNname = System.IO.Path.GetFileNameWithoutExtension(oldPath);
                string newShpFullName = Path.Combine(newPath, Path.GetFileName(oldPath));
                string newShpName = System.IO.Path.GetFileNameWithoutExtension(oldPath);

                string errormsg;
                if (!DeleteShapeFile(newShpFullName, out errormsg))
                {
                    this.ReportError("删除文件错误！" + errormsg);
                    return;
                }

                var ds = ProviderShapefile.CreateDataSource(filepath, false) as IDbContext;
                var dq = new DynamicQuery(ds);
                int count = dq.Count(null, oldShpNname);
                if (count == 0)
                {
                    this.ReportAlert("", "shape文件为空！");
                    return;
                }

                ShapefileDataReader sdr = new ShapefileDataReader(oldPath, GeometryFactory.Default);
                var fieldList = new List<FieldInfo>();
                foreach (var item in sdr.DbaseHeader.Fields)
                {
                    fieldList.Add(new FieldInfo()
                    {
                        Name = item.Name,
                        Type = ConvertToDBFFieldType(item.Type),
                        Length = item.Length,
                        Precision = item.DecimalCount
                    });
                }
                sdr.Dispose();

                bool init = false;

                //GDALShapeFileWriter<object> writer = new GDALShapeFileWriter<object>();
                var nsf = new ShapeFileExporter<object>();//创建导出类

                ShapeLayer layer = null;
                int trfmCount = 0;
                dq.ForEach(null, oldShpNname, (i, j, o) =>
                {
                    var geo = YuLinTu.ObjectExtension.GetPropertyValue(o, "Shape") as YuLinTu.Spatial.Geometry;
                    if (geo == null)
                    {
                        this.ReportError(string.Format("shape数据无效，无法导入"));
                        return false;
                    }

                    if (!init)
                    {
                        init = true;
                        //layer = writer.InitiallLayer(newShpFullName, newShpName, OperateShapeFile.GeoTypeConvert(geo.GeometryType), fieldList);
                        layer = nsf.InitiallLayer(newShpFullName, GeoTypeConvert(geo.GeometryType), "prj", fieldList);//5,创建图层
                    }
                    if (layer == null)
                    {
                        this.ReportError(string.Format("创建图层失败！"));
                        return false;
                    }

                    Geometry geometry = geo.Project(destprj);

                    YuLinTu.ObjectExtension.SetPropertyValue(o, "Shape", geometry);
                    nsf.WriteVectorFile(layer, new List<object>() { o });

                    this.ReportProgress((int)(((index / (double)acount) + (++trfmCount / (double)count / (double)acount)) * 100), "正在转换...");
                    return true;
                });
                layer.Dispose();

                index++;
                if (layer == null)
                {
                    return;
                }

                ProjectionInfo info = destprj.CreateProjectionInfo();
                if (info == null)
                    return;

                info.SaveAs(Path.ChangeExtension(newShpFullName, "prj"));
            }

            this.ReportProgress(100, "完成");
            this.ReportInfomation(string.Format("Shape文件转换完成。"));
        }
        #endregion
    }
}
