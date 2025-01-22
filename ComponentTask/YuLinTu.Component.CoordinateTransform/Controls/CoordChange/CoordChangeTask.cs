using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Common.CShapeExport;
using DotSpatial.Projections;
using NetTopologySuite.IO;
using YuLinTu.Data;
using YuLinTu.Data.Dynamic;
using YuLinTu.Data.Shapefile;
using YuLinTu.Spatial;

namespace YuLinTu.Component.CoordinateTransformTask
{
    /// <summary>
    /// 更改坐标系
    /// </summary>
    [TaskDescriptor(IsLanguageName = false, Name = "更改坐标系", Gallery = "矢量数据处理",
        UriImage16 = "pack://application:,,,/YuLinTu.Resources;component/Images/16/store.png",
        UriImage24 = "pack://application:,,,/YuLinTu.Resources;component/Images/24/store.png")]
    public class CoordChangeTask : Task
    {
        #region Fields   
        private CoordChangeArgument args;
        private object lockobj = new object();
        #endregion

        #region Ctor
        public CoordChangeTask()
        {
            Name = "更改坐标系";
            Description = "更改矢量文件原有的坐标系为指定坐标系";
        }
        #endregion

        #region Methods - Override
        protected override void OnGo()
        {
            args = Argument as CoordChangeArgument;

            if (!ValidateArgs())
                return;
            int index = 0;
            foreach (var sp in args.OldShapePath)
            {
                TransformNoArgs(sp, args.NewShapePath, args.ChangeDestinationPrj, args.OldShapePath.Count, index, ChageDataCoordnate);
                index++;
            }
            this.ReportProgress(100, "完成");
        }
        #endregion

        #region Methods - Validate

        //参数检查
        private bool ValidateArgs()
        {
            if (args.OldShapePrj.CreateProjectionInfo().Name.Equals(
            args.ChangeDestinationPrj.CreateProjectionInfo().Name))
            {
                this.ReportError("原坐标系不能与目标坐标系相同！");
                return false;
            }
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
            this.ReportInfomation(string.Format("参数设置正确。"));

            return true;
        }

        #endregion

        #region Methods - Private

        public void TransformNoArgs(string oldPath, string newPath, SpatialReference destprj, int shapecount, int index,
            Func<List<object>, SpatialReference, List<string>> action)
        {
            this.ReportInfomation(string.Format("开始转换Shape文件: {0}。", Path.GetFileName(oldPath)));

            string filepath = System.IO.Path.GetDirectoryName(oldPath);
            string oldShpNname = System.IO.Path.GetFileNameWithoutExtension(oldPath);
            string shpFullName = System.IO.Path.Combine(newPath, oldShpNname + ".shp");

            string errormsg = OperateShapeFile.DeleteShapeFile(shpFullName);
            if (!string.IsNullOrEmpty(errormsg))
            {
                this.ReportError(errormsg);
                return;
            }

            ShapefileDataReader sdr = new ShapefileDataReader(oldPath, NetTopologySuite.Geometries.GeometryFactory.Default);
            int count = sdr.RecordCount;
            if (count == 0)
            {
                this.ReportAlert("", "shape文件为空！");
                return;
            }
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
            var shpType = sdr.ShapeHeader.ShapeType;
            sdr.Dispose();
            var singecount = 100 / shapecount;
            var shpecount = index * singecount;
            var ds = ProviderShapefile.CreateDataSource(filepath, false) as IDbContext;
            var dq = new DynamicQuery(ds);
            var result = TransData(dq, oldShpNname, destprj, shpFullName, count, shpType, fieldList, shpecount, singecount, action);
            if (result)
            {
                this.ReportInfomation(string.Format("Shape文件转换完成。"));
            }
        }

        /// <summary>
        /// 转换数据
        /// </summary>
        private bool TransData(DynamicQuery dq, string oldShpNname, SpatialReference destprj, string shpFullName,
            int count, ShapeGeometryType shpType, List<FieldInfo> fieldList, int processcount, int singecount,
            Func<List<object>, SpatialReference, List<string>> action)
        {
            var result = true;
            ShapeFileExporter<object> writer = new ShapeFileExporter<object>();
            ShapeLayer layer = null;
            string infostr = destprj.ToEsriString();
            if (string.IsNullOrEmpty(infostr))
            {
                ProjectionInfo info = destprj.CreateProjectionInfo();
                if (info != null)
                    infostr = info.ToEsriString();
            }
            layer = writer.InitiallLayer(shpFullName, (Common.CShapeExport.EShapeType)shpType, infostr, fieldList);
            if (layer == null)
            {
                result = false;
                this.ReportError(string.Format("创建图层失败，无法转换"));
                return false;
            }

            List<object> datalist = new List<object>(1000);
            int trfmCount = 0;
            try
            {
                dq.ForEach(null, oldShpNname, (i, j, o) =>
                {
                    datalist.Add(o);
                    if (datalist.Count == 1000)
                    {
                        action(datalist, destprj);
                        writer.WriteVectorFile(layer, datalist);
                        datalist.Clear();
                    }
                    this.ReportProgress((int)(processcount + (++trfmCount * singecount) / (double)count), "正在转换...");
                    return true;
                });

                if (datalist.Count > 0)
                {
                    action(datalist, destprj);
                    writer.WriteVectorFile(layer, datalist);
                    datalist.Clear();
                }

                if (layer != null)
                    layer.Dispose();
            }
            catch
            {
                layer.Dispose();
            }
            return result;
        }

        /// <summary>
        /// 数据转换
        /// </summary>
        public List<string> ChageDataCoordnate(List<object> datalist, SpatialReference destprj)
        {
            Parallel.ForEach(datalist, d =>
            {
                var geo = YuLinTu.ObjectExtension.GetPropertyValue(d, "Shape") as YuLinTu.Spatial.Geometry;
                if (geo == null)
                {
                    this.ReportError(string.Format("shape数据无效，无法转换"));
                    return;
                }
                try
                {
                    var geometry = geo.Project(destprj);
                    YuLinTu.ObjectExtension.SetPropertyValue(d, "Shape", geometry);
                }
                catch (Exception ex)
                {
                    var geostr = geo.ToText();
                    this.ReportError(string.Format("转换失败，请确定数据是否可以执行转换：{0} \n目标坐标系: {1} \n原始坐标: {2} ",
                        ex.Message, destprj.ToEsriString(), geostr.Length > 50 ? geostr.Substring(0, 50) : geostr));
                    return;
                }
            });
            return new List<string>();
        }
        #endregion
    }
}
