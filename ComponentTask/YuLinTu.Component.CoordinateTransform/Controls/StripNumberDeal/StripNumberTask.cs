using NetTopologySuite.IO;
using System;
using System.Collections.Generic;
using System.IO;
using YuLinTu.Spatial;
using Geometry = YuLinTu.Spatial.Geometry;

namespace YuLinTu.Component.CoordinateTransformTask
{
    /// <summary>
    /// 带号处理
    /// </summary>
    [TaskDescriptor(IsLanguageName = false, Name = "带号处理", Gallery = "矢量数据处理",
        UriImage16 = "pack://application:,,,/YuLinTu.Resources;component/Images/16/store.png",
        UriImage24 = "pack://application:,,,/YuLinTu.Resources;component/Images/24/store.png")]
    public class StripNumberTask : Task
    {
        #region Fields

        private OperateShapeFile operateShape;

        private StripNumberArgument args;
        #endregion

        #region Ctor
        public StripNumberTask()
        {
            Name = "带号处理";
            Description = "对矢量文件中的进行添加或去除带号的处理";
        }
        #endregion

        #region Methods - Override
        protected override void OnGo()
        {
            args = Argument as StripNumberArgument;

            if (!ValidateArgs())
                return;
            foreach (var sp in args.OldShapePath)
            {
                var newshp = Path.Combine(args.NewShapePath, Path.GetFileName(sp));
                operateShape = new OperateShapeFile(sp, newshp);
                OperateShapeFiles();
            }
        }
        #endregion

        #region Methods - Validate

        //参数检查
        private bool ValidateArgs()
        {
            if (args.OperateStripNumber <= 0 || args.OperateStripNumber > 2)
            {
                this.ReportError("请选择正确的操作!");
                return false;
            }
            if (args.OldShapePath == null || args.OldShapePath.Count == 0)
            {
                this.ReportError("请选择源Shape文件路径!");
                return false;
            }
            string filepath = System.IO.Path.GetDirectoryName(args.OldShapePath[0]);
            string oldShpNname = System.IO.Path.GetFileNameWithoutExtension(args.OldShapePath[0]) + ".prj";
            if (!File.Exists($"{filepath}\\{oldShpNname}"))
            {
                this.ReportError("源Shape文件路径中，不存在Prj文件，请确认文件路径!");
                return false;
            }
            if (string.IsNullOrEmpty(args.NewShapePath))
            {
                this.ReportError("请选择新Shape文件的保存路径!");
                return false;
            }
            this.ReportInfomation(string.Format("导出参数设置正确。"));
            bool haveCode = false;
            var datanumber = 0;
            ShapefileDataReader sdr = new ShapefileDataReader(args.OldShapePath[0],
                NetTopologySuite.Geometries.GeometryFactory.Default);
            if (sdr.RecordCount == 0)
            {
                this.ReportWarn("矢量文件无数据，无需去除带号");
                return false;
            }
            while (sdr.Read())
            {
                var geo = sdr.Geometry.Coordinates;
                var x = geo[0].X;
                if (Convert.ToInt32(x).ToString().Length == 8)
                {
                    haveCode = true;
                }
            }
            sdr.Close();
            sdr.Dispose();
            if (!haveCode && args.OperateStripNumber == 1)//无带号&&删除代号
            {
                this.ReportWarn("矢量文件数据无带号，无需去除带号");
                return false;
            }
            return true;
        }

        #endregion

        #region Methods - Private

        private void OperateShapeFiles()
        {
            this.ReportProgress(0, "开始");
            this.ReportInfomation(string.Format("开始转换Shape文件。"));
            Geometry geometry = null;
            this.ReportProgress(30, "数据转换中。。。");
            operateShape.ReadShapeData((layer,shpLandItem, dataCount) =>
            {
                try
                {
                    var geo = YuLinTu.ObjectExtension.GetPropertyValue(shpLandItem, "Shape") as YuLinTu.Spatial.Geometry;
                    if (geo == null) return;

                    if (geo.GeometryType.ToString().Contains("Polygon"))
                        geometry = StripDealPolygon(geo);
                    else
                        geometry = StripDeal(geo, args);

                    if (!operateShape.WriteShapeFile(layer, shpLandItem, geometry))
                    {
                        operateShape.Dispose();
                        OperateShapeFile.DeleteShapeFile(args.NewShapePath);
                        this.ReportError(string.Format("shape数据无效，无法转换"));
                        return;
                    }
                }
                catch (Exception ex)
                {
                    operateShape.Dispose();
                    OperateShapeFile.DeleteShapeFile(args.NewShapePath);
                    this.ReportError(string.Format("shape数据无效，无法转换"));
                    return;
                }
            }, () =>
             {
                 operateShape.Dispose();
                 OperateShapeFile.DeleteShapeFile(args.NewShapePath);
                 this.ReportError(string.Format("shape数据无效或无数据，无法转换"));
             });
            //复制PRJ文件至目标路径
            var errorInfo = operateShape.CopyToPrjFile(operateShape.ReadExcel());
            if (!string.IsNullOrEmpty(errorInfo) &&
                errorInfo.ToUpper() != "TRUE")
                this.ReportError(errorInfo);

            operateShape.Dispose();
            this.ReportProgress(100, "完成");
            this.ReportInfomation(string.Format("Shape文件转换完成。"));
        }

        private Geometry StripDeal(Geometry geo, StripNumberArgument args)
        {
            double x1;
            double X2, Y2;

            Coordinate[] coord = geo.ToCoordinates();

            List<Coordinate> newCoordinates = new List<Coordinate>();
            decimal Meridian = operateShape.CalculateBH(geo);

            if (args.OperateStripNumber == 2) //添加带号
            {
                foreach (Coordinate t in coord)
                {
                    x1 = t.X;

                    var temp = Convert.ToInt32(x1).ToString();
                    if (temp.Length == 8)
                        continue;

                    //等于6位则加上带号作为前两位
                    X2 = Convert.ToDouble(Convert.ToInt32(Meridian).ToString() + x1.ToString());
                    Y2 = t.Y;

                    newCoordinates.Add(new Coordinate(X2, Y2));
                }
            }
            else if (args.OperateStripNumber == 1) //删除带号
            {
                foreach (Coordinate t in coord)
                {
                    x1 = t.X;

                    var temp = Convert.ToInt32(x1).ToString();
                    if (temp.Length != 8)
                        continue;

                    //大于6位则去掉前两位
                    X2 = Convert.ToDouble(x1.ToString().Substring(2, x1.ToString().Length - 2));
                    Y2 = t.Y;

                    newCoordinates.Add(new Coordinate(X2, Y2));
                }
            }

            return YuLinTu.Spatial.Geometry.CreatePolyline(newCoordinates);
        }

        private Geometry StripDealPolygon(Geometry geo)
        {
            var geometrys = geo.ToSingleGeometries();
            Geometry newgeo = null;
            decimal Meridian = operateShape.CalculateBH(geo);
            foreach (var geos in geometrys)
            {
                List<Coordinate> newCoordinates = new List<Coordinate>();
                var holes = new List<List<Coordinate>>();
                var coordinates = geos.ToGroupCoordinates();
                for (int i = 0; i < coordinates.Count; i++)
                {
                    var coorData = coordinates[i];
                    if (i > 0)//有洞的数据
                    {
                        var holeCoordinates = ProcessData(coorData, Meridian);
                        holes.Add(holeCoordinates);
                        continue;
                    }
                    newCoordinates = ProcessData(coorData, Meridian);
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

        /// <summary>
        /// 处理带号
        /// </summary> 
        private List<Coordinate> ProcessData(Coordinate[] coorData, decimal Meridian)
        {
            var newCoordinates = new List<Coordinate>();
            double x1;
            double X2, Y2;
            if (args.OperateStripNumber == 2) //添加带号
            {
                foreach (Coordinate t in coorData)
                {
                    x1 = t.X;

                    var temp = Convert.ToInt32(x1).ToString();
                    if (temp.Length == 8)
                        continue;

                    //等于6位则加上带号作为前两位
                    X2 = Convert.ToDouble(Convert.ToInt32(Meridian).ToString() + x1.ToString());
                    Y2 = t.Y;

                    newCoordinates.Add(new Coordinate(X2, Y2));
                }
            }
            else if (args.OperateStripNumber == 1) //删除带号
            {
                foreach (Coordinate t in coorData)
                {
                    x1 = t.X;

                    var temp = Convert.ToInt32(x1).ToString();
                    if (temp.Length != 8)
                        continue;

                    //大于6位则去掉前两位
                    X2 = Convert.ToDouble(x1.ToString().Substring(2, x1.ToString().Length - 2));
                    Y2 = t.Y;

                    newCoordinates.Add(new Coordinate(X2, Y2));
                }
            }
            return newCoordinates;
        }
        #endregion
    }
}
