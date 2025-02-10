using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using YuLinTu.Spatial;
using Geometry = YuLinTu.Spatial.Geometry;

namespace YuLinTu.Component.CoordinateTransformTask
{
    /// <summary>
    /// 数据平移
    /// </summary>
    [TaskDescriptor(IsLanguageName = false, Name = "数据平移", Gallery = "矢量数据处理",
        UriImage16 = "pack://application:,,,/YuLinTu.Resources;component/Images/16/card-export.png",
        UriImage24 = "pack://application:,,,/YuLinTu.Resources;component/Images/24/card-export.png")]
    public class LevelMoveTask : Task
    {
        #region Fields

        private OperateShapeFile operateShape;

        private LevelMoveArgument args;
        #endregion

        #region Ctor
        public LevelMoveTask()
        {
            Name = "数据平移";
            Description = "将矢量数据进行平移处理";
        }
        #endregion

        #region Methods - Override
        protected override void OnGo()
        {
            args = Argument as LevelMoveArgument;
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
            var listfolder = new List<string>();
            foreach (var item in args.OldShapePath)
            {
                listfolder.Add(Path.GetDirectoryName(item));
            }
            if (listfolder.Distinct().Any(a => a == args.NewShapePath))
            {
                this.ReportError(" 新Shape文件的保存路径不能和原路径相同");
                return false;
            }
            this.ReportInfomation(string.Format("导出参数设置正确。"));

            return true;
        }

        #endregion

        #region Methods - Private

        private void OperateShapeFiles()
        {
            this.ReportProgress(0, "开始");
            this.ReportInfomation(string.Format("开始转换Shape文件。"));

            this.ReportProgress(30, "数据转换中。。。");
            operateShape.ReadShapeData((layer, shpLandItem, dataCount) =>
            {
                var geo = YuLinTu.ObjectExtension.GetPropertyValue(shpLandItem, "Shape") as YuLinTu.Spatial.Geometry;
                if (geo == null) return;
                Geometry geometry = null;
                if (geo.GeometryType.ToString().Contains("Polygon"))
                    geometry = MoveDataPolygon(geo, args);
                else
                    geometry = MoveData(geo, args);

                operateShape.WriteShapeFile(layer, shpLandItem, geometry);
            },
            () =>
            {
                operateShape.Dispose();
                OperateShapeFile.DeleteShapeFile(args.NewShapePath);
                this.ReportError(string.Format("shape数据无效，无法导入"));
            });
            //复制PRJ文件至目标路径
            var errorInfo = operateShape.CopyToPrjFile();
            if (!string.IsNullOrEmpty(errorInfo) &&
                errorInfo.ToUpper() != "TRUE")
                this.ReportError(errorInfo);

            operateShape.Dispose();
            this.ReportProgress(100, "完成");
            this.ReportInfomation(string.Format("Shape文件转换完成。"));
        }

        private Geometry MoveData(Geometry geo, LevelMoveArgument args)
        {
            double X2, Y2;

            Coordinate[] coord = geo.ToCoordinates();
            List<Coordinate> newCoordinates = new List<Coordinate>();
            foreach (Coordinate t in coord)
            {
                //原坐标加上平移量
                X2 = t.X + args.XMoveNumber;
                Y2 = t.Y + args.YMoveNumber;

                newCoordinates.Add(new Coordinate(X2, Y2));
            }
            return operateShape.ConvertCoordinatesToGeometry(newCoordinates, geo);
        }

        /// <summary>
        /// 移动图形
        /// </summary>
        private Geometry MoveDataPolygon(Geometry geo, LevelMoveArgument args)
        {
            double x1;
            double X2, Y2;
            Geometry newgeo = null;
            var geometrys = geo.ToSingleGeometries();

            List<Coordinate[]> coordinates;
            List<List<Coordinate>> coordinateList = new List<List<Coordinate>>();
            var holes = new List<List<Coordinate>>();
            var geos = new List<Geometry>();

            foreach (var g in geometrys)
            {
                coordinates = g.ToGroupCoordinates();
                List<Coordinate> newCoordinates = new List<Coordinate>();
                for (int i = 0; i < coordinates.Count; i++)
                {
                    var coorData = coordinates[i];
                    if (i > 0)//有洞的数据
                    {
                        var holeCoordinates = new List<Coordinate>();
                        holeCoordinates.AddRange(coorData.Select(t => new Coordinate(t.X + args.XMoveNumber, t.Y + args.YMoveNumber)));

                        holes.Add(holeCoordinates);
                        continue;
                    }

                    foreach (Coordinate t in coorData)
                    {
                        //原坐标加上平移量
                        X2 = t.X + args.XMoveNumber;
                        Y2 = t.Y + args.YMoveNumber;

                        newCoordinates.Add(new Coordinate(X2, Y2));
                    }
                }

                var ng = YuLinTu.Spatial.Geometry.CreatePolygon(newCoordinates, holes);
                if (newgeo == null)
                    newgeo = ng;
                else
                    newgeo = newgeo.Union(ng);
            }
            return newgeo;
        }

        #endregion
    }
}
