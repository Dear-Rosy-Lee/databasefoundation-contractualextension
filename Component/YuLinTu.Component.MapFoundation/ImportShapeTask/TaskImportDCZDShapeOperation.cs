/*
 * (C) 2015  鱼鳞图公司版权所有,保留所有权利 
 */
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using YuLinTu.Data;
using YuLinTu.Data.Dynamic;
using YuLinTu.Data.Shapefile;
using YuLinTu.Library.Entity;
using YuLinTu.Library.Repository;
using YuLinTu.Library.WorkStation;

namespace YuLinTu.Component.MapFoundation
{
    public class TaskImportDCZDShapeOperation : Task
    {
        public TaskImportDCZDShapeOperation()
        {

        }

        /// <summary>
        /// 开始执行子任务
        /// </summary>
        protected override void OnGo()
        {
            TaskImportDCZDShapeArgument metadata = Argument as TaskImportDCZDShapeArgument;
            if (metadata == null)
            {
                return;
            }
            if(metadata.Database==null)
            {
                this.ReportError("数据库连接失败");
                this.ReportProgress(100);
                return;
            }
            if(metadata.Type=="Del")
            {
                IDCZDWorkStation dczdstation = new ContainerFactory(metadata.Database).CreateWorkstation<IDCZDWorkStation, IDCZDRepository>();
                this.ReportProgress(10);
                int count = dczdstation.Count();
                dczdstation.Delete();
                this.ReportProgress(100);
                this.ReportInfomation("清空"+count+"条调查宗地数据");
                return;
            }
            IList landShapeList = null;
            List<string> columnNameList = new List<string>();
            try
            {
                //获取当前路径下shape数据

                string filepath = System.IO.Path.GetDirectoryName(metadata.FileName);
                string filename = System.IO.Path.GetFileNameWithoutExtension(metadata.FileName);
                var ds = ProviderShapefile.CreateDataSource(filepath, false) as IDbContext;
                var dq = new DynamicQuery(ds);
                var landdata = dq.Get(null, filename).Result as IList;
                landShapeList = landdata;
                var importShpType = ds.DataSource as IProviderShapefile;

                #region 数据检查
                if (importShpType.GetGeometryType(filename) != Spatial.eGeometryType.Polygon)
                {
                    this.ReportError("当前Shape文件不为面文件，请重新选择面文件导入");
                    return;
                }
                if (landShapeList.Count == 0 || landShapeList == null)
                {
                    this.ReportError("当前Shape文件中没有数据");
                    return;
                }
                var s = dq.GetElementProperties(null, filename);
                var ss = s.FindAll(c => c.ColumnName == "Shape");

                if (ss == null || ss.Count == 0)
                {
                    this.ReportError("当前Shape文件中没有Shape字段");
                    return;
                }
                #endregion

                s.ForEach(t => columnNameList.Add(t.ColumnName));
            }
            catch
            {
                this.ReportError("当前Shape文件信息有误，请检查相关文件名称");
                return;
            }
            switch (metadata.SelectLayerName)
            {
                case "调查宗地":
                    ImportDCZDShape(columnNameList, landShapeList, metadata);
                    break;
                case "点状地物":
                    ImportDZDWShape(columnNameList, landShapeList, metadata);
                    break;
                case "线状地物":
                    ImportXZDWShape(columnNameList, landShapeList, metadata);
                    break;
                case "面状地物":
                    ImportMZDWShape(columnNameList, landShapeList, metadata);
                    break;
            }

        }

        #region privateMethod
        private string GetproertValue(object shapeData, string Value)
        {
            object obj = YuLinTu.ObjectExtension.GetPropertyValue(shapeData, Value);
            if (obj == null)
                return "";
            return obj.ToString();
        }
        /// <summary>
        /// 导入调查宗地图层shp文件
        /// </summary>
        private void ImportDCZDShape(List<string> columnNameList, IList landShapeList, TaskImportDCZDShapeArgument metadata)
        {
            double Percent = 0.0;  //百分比
            Percent = 98 / (double)landShapeList.Count;
            int index = 0;   //索引
            this.ReportProgress(0, "开始");

            metadata.Database.BeginTransaction();
            try
            {
                var targetSpatialReference = metadata.Database.CreateSchema().GetElementSpatialReference(
                ObjectContext.Create(typeof(DCZD)).Schema,
                ObjectContext.Create(typeof(DCZD)).TableName);
                IDCZDWorkStation dczdstation = new ContainerFactory(metadata.Database).CreateWorkstation<IDCZDWorkStation, IDCZDRepository>();
                DCZD dcZDLand = null;
                foreach (var shpLandItem in landShapeList)
                {
                    dcZDLand = new DCZD();
                    try
                    {
                        var g = YuLinTu.ObjectExtension.GetPropertyValue(shpLandItem, "Shape") as YuLinTu.Spatial.Geometry;
                        var shapevalue = YuLinTu.Spatial.Geometry.FromBytes(g.AsBinary(), 0);
                        dcZDLand.Shape = shapevalue == null ? null : shapevalue as YuLinTu.Spatial.Geometry;
                        dcZDLand.Shape.SpatialReference = targetSpatialReference;
                        dczdstation.Add(dcZDLand);
                    }
                    catch (Exception)
                    {
                        this.ReportError(string.Format("第{0}条shape数据无效，无法导入", index + 1));
                        metadata.Database.RollbackTransaction();
                        return;
                    }
                    index++;
                    this.ReportProgress((int)(Percent * index), string.Format("导入第{0}个数据", index.ToString()));
                }
                metadata.Database.CommitTransaction();
                this.ReportProgress(100, "导入完成.");
                this.ReportInfomation(string.Format("共导入{0}个Shape数据", index.ToString()));
            }
            catch (Exception ex)
            {
                metadata.Database.RollbackTransaction();
                this.ReportError("导入图斑数据时发生错误！");
                YuLinTu.Library.Log.Log.WriteException(this, "ImportShape", ex.Message + ex.StackTrace);
            }

        }

        /// <summary>
        /// 导入点状地物图层shp文件
        /// </summary>
        private void ImportDZDWShape(List<string> columnNameList, IList landShapeList, TaskImportDCZDShapeArgument metadata)
        {
            double Percent = 0.0;  //百分比
            Percent = 98 / (double)landShapeList.Count;
            int index = 0;   //索引
            this.ReportProgress(1, "开始");

            IDZDWWorkStation dzdwstation = new ContainerFactory(metadata.Database).CreateWorkstation<IDZDWWorkStation, IDZDWRepository>();
            DZDW dzdw = null;

            var hasBSMCol = columnNameList.Contains("BSM");
            if (!hasBSMCol) this.ReportInfomation(string.Format("当前数据中无'BSM'字段"));

            var hasYSDMCol = columnNameList.Contains("YSDM");
            if (!hasYSDMCol) this.ReportInfomation(string.Format("当前数据中无'YSDM'字段"));

            var hasDWMCCol = columnNameList.Contains("DWMC");
            if (!hasDWMCCol) this.ReportInfomation(string.Format("当前数据中无'DWMC'字段"));

            var hasBZCol = columnNameList.Contains("BZ");
            if (!hasBZCol) this.ReportInfomation(string.Format("当前数据中无'BZ'字段"));

            foreach (var shpLandItem in landShapeList)
            {
                dzdw = new DZDW();

                if (hasBSMCol)
                {
                    var bsmvalue = GetproertValue(shpLandItem, "BSM");
                    //YuLinTu.ObjectExtension.GetPropertyValue(shpLandItem, "BSM").ToString();
                    if (bsmvalue == "" || bsmvalue == null)
                    {
                        dzdw.BSM = null;
                    }
                    else
                    {
                        dzdw.BSM = int.Parse(bsmvalue);
                    }
                }
                if (hasYSDMCol)
                {
                    var ysdmvalue = GetproertValue(shpLandItem, "YSDM");
                    //YuLinTu.ObjectExtension.GetPropertyValue(shpLandItem, "YSDM").ToString();
                    dzdw.YSDM = ysdmvalue;
                }
                if (hasDWMCCol)
                {
                    var dwmcvalue = GetproertValue(shpLandItem, "DWMC");
                    //YuLinTu.ObjectExtension.GetPropertyValue(shpLandItem, "DWMC").ToString();
                    dzdw.DWMC = dwmcvalue;
                }
                if (hasBZCol)
                {
                    var bzvalue = GetproertValue(shpLandItem, "BZ");
                    //YuLinTu.ObjectExtension.GetPropertyValue(shpLandItem, "BZ").ToString();
                    dzdw.Comment = bzvalue;
                }
                dzdw.ZoneCode = metadata.CurrentZone.FullCode;
                dzdw.ZoneName = metadata.CurrentZone.FullName;

                var shapevalue = YuLinTu.ObjectExtension.GetPropertyValue(shpLandItem, "Shape");
                dzdw.Shape = shapevalue == null ? null : shapevalue as YuLinTu.Spatial.Geometry;
                dzdw.Shape.Srid = 0;
                dzdwstation.Add(dzdw);
                index++;
                this.ReportProgress((int)(Percent * index), string.Format("导入第{0}个数据", index.ToString()));
            }
            this.ReportProgress(100, "导入完成.");
            this.ReportInfomation(string.Format("共导入{0}个Shape数据", index.ToString()));

        }

        /// <summary>
        /// 导入线状地物图层shp文件
        /// </summary>
        private void ImportXZDWShape(List<string> columnNameList, IList landShapeList, TaskImportDCZDShapeArgument metadata)
        {
            double Percent = 0.0;  //百分比
            Percent = 98 / (double)landShapeList.Count;
            int index = 0;   //索引
            this.ReportProgress(1, "开始");

            IXZDWWorkStation xzdwstation = new ContainerFactory(metadata.Database).CreateWorkstation<IXZDWWorkStation, IXZDWRepository>();
            XZDW xzdw = null;

            var hasBSMCol = columnNameList.Contains("BSM");
            if (!hasBSMCol) this.ReportInfomation(string.Format("当前数据中无'BSM'字段"));

            var hasYSDMCol = columnNameList.Contains("YSDM");
            if (!hasYSDMCol) this.ReportInfomation(string.Format("当前数据中无'YSDM'字段"));

            var hasDWMCCol = columnNameList.Contains("DWMC");
            if (!hasDWMCCol) this.ReportInfomation(string.Format("当前数据中无'DWMC'字段"));

            var hasCDCol = columnNameList.Contains("CD");
            if (!hasCDCol) this.ReportInfomation(string.Format("当前数据中无'CD'字段"));

            var hasKDCol = columnNameList.Contains("KD");
            if (!hasKDCol) this.ReportInfomation(string.Format("当前数据中无'KD'字段"));

            var hasBZCol = columnNameList.Contains("BZ");
            if (!hasBZCol) this.ReportInfomation(string.Format("当前数据中无'BZ'字段"));

            foreach (var shpLandItem in landShapeList)
            {
                xzdw = new XZDW();
                if (hasBSMCol)
                {
                    var bsmvalue = GetproertValue(shpLandItem, "BSM");
                    //YuLinTu.ObjectExtension.GetPropertyValue(shpLandItem, "BSM").ToString();
                    if (bsmvalue == "" || bsmvalue == null)
                    {
                        xzdw.BSM = null;
                    }
                    else
                    {
                        xzdw.BSM = int.Parse(bsmvalue);
                    }
                }
                if (hasYSDMCol)
                {
                    var ysdmvalue = GetproertValue(shpLandItem, "YSDM");
                    //YuLinTu.ObjectExtension.GetPropertyValue(shpLandItem, "YSDM").ToString();
                    xzdw.YSDM = ysdmvalue;
                }
                if (hasDWMCCol)
                {
                    var dwmcvalue = GetproertValue(shpLandItem, "DWMC");
                    //YuLinTu.ObjectExtension.GetPropertyValue(shpLandItem, "DWMC").ToString();
                    xzdw.DWMC = dwmcvalue;
                }
                if (hasCDCol)
                {
                    var cdvalue = GetproertValue(shpLandItem, "CD");
                    //YuLinTu.ObjectExtension.GetPropertyValue(shpLandItem, "CD").ToString();
                    if (cdvalue != "")
                        xzdw.CD = double.Parse(cdvalue);
                }
                if (hasKDCol)
                {
                    var kdvalue = GetproertValue(shpLandItem, "KD");
                    //YuLinTu.ObjectExtension.GetPropertyValue(shpLandItem, "KD").ToString();
                    if (kdvalue != "")
                        xzdw.KD = double.Parse(kdvalue);
                }
                if (hasBZCol)
                {
                    var bzvalue = GetproertValue(shpLandItem, "BZ");
                    //YuLinTu.ObjectExtension.GetPropertyValue(shpLandItem, "BZ").ToString();
                    xzdw.Comment = bzvalue;
                }

                xzdw.ZoneCode = metadata.CurrentZone.FullCode;
                xzdw.ZoneName = metadata.CurrentZone.FullName;

                var shapevalue = YuLinTu.ObjectExtension.GetPropertyValue(shpLandItem, "Shape");
                xzdw.Shape = shapevalue == null ? null : shapevalue as YuLinTu.Spatial.Geometry;
                xzdw.Shape.Instance.SRID = 0;
                xzdw.Shape = YuLinTu.Spatial.Geometry.FromInstance(xzdw.Shape.Instance);
                xzdwstation.Add(xzdw);
                index++;
                this.ReportProgress((int)(Percent * index), string.Format("导入第{0}个数据", index.ToString()));
            }
            this.ReportProgress(100, "导入完成.");
            this.ReportInfomation(string.Format("共导入{0}个Shape数据", index.ToString()));

        }

        /// <summary>
        /// 导入面状地物图层shp文件
        /// </summary>
        private void ImportMZDWShape(List<string> columnNameList, IList landShapeList, TaskImportDCZDShapeArgument metadata)
        {
            double Percent = 0.0;  //百分比
            Percent = 98 / (double)landShapeList.Count;
            int index = 0;   //索引
            this.ReportProgress(1, "开始");

            IMZDWWorkStation mzdwstation = new ContainerFactory(metadata.Database).CreateWorkstation<IMZDWWorkStation, IMZDWRepository>();
            MZDW mzdw = null;

            var hasBSMCol = columnNameList.Contains("BSM");
            if (!hasBSMCol) this.ReportInfomation(string.Format("当前数据中无'BSM'字段"));

            var hasYSDMCol = columnNameList.Contains("YSDM");
            if (!hasYSDMCol) this.ReportInfomation(string.Format("当前数据中无'YSDM'字段"));

            var hasDWMCCol = columnNameList.Contains("DWMC");
            if (!hasDWMCCol) this.ReportInfomation(string.Format("当前数据中无'DWMC'字段"));

            var hasMJCol = columnNameList.Contains("MJ");
            if (!hasMJCol) this.ReportInfomation(string.Format("当前数据中无'MJ'字段"));

            var hasBZCol = columnNameList.Contains("BZ");
            if (!hasBZCol) this.ReportInfomation(string.Format("当前数据中无'BZ'字段"));

            foreach (var shpLandItem in landShapeList)
            {
                mzdw = new MZDW();
                if (hasBSMCol)
                {
                    var bsmvalue = GetproertValue(shpLandItem, "BSM");
                    //YuLinTu.ObjectExtension.GetPropertyValue(shpLandItem, "BSM").ToString();
                    if (bsmvalue == "" || bsmvalue == null)
                    {
                        mzdw.BSM = null;
                    }
                    else
                    {
                        mzdw.BSM = int.Parse(bsmvalue);
                    }
                }
                if (hasYSDMCol)
                {
                    var ysdmvalue = GetproertValue(shpLandItem, "YSDM");
                    //YuLinTu.ObjectExtension.GetPropertyValue(shpLandItem, "YSDM").ToString();
                    mzdw.YSDM = ysdmvalue;
                }

                if (hasDWMCCol)
                {
                    var dwmcvalue = GetproertValue(shpLandItem, "DWMC");
                    //YuLinTu.ObjectExtension.GetPropertyValue(shpLandItem, "DWMC").ToString();
                    mzdw.DWMC = dwmcvalue;
                }

                if (hasMJCol)
                {
                    var areavalue = GetproertValue(shpLandItem, "MJ");
                    //YuLinTu.ObjectExtension.GetPropertyValue(shpLandItem, "MJ").ToString();
                    if (areavalue != "")
                        mzdw.Area = double.Parse(areavalue);
                }

                if (hasBZCol)
                {
                    var bzvalue = GetproertValue(shpLandItem, "BZ");
                    //YuLinTu.ObjectExtension.GetPropertyValue(shpLandItem, "BZ").ToString();
                    mzdw.Comment = bzvalue;
                }

                mzdw.ZoneCode = metadata.CurrentZone.FullCode;
                mzdw.ZoneName = metadata.CurrentZone.FullName;
                var shapevalue = YuLinTu.ObjectExtension.GetPropertyValue(shpLandItem, "Shape");
                mzdw.Shape = shapevalue == null ? null : shapevalue as YuLinTu.Spatial.Geometry;
                mzdw.Shape.Instance.SRID = 0;
                mzdw.Shape = YuLinTu.Spatial.Geometry.FromInstance(mzdw.Shape.Instance);
                mzdwstation.Add(mzdw);
                index++;
                this.ReportProgress((int)(Percent * index), string.Format("导入第{0}个数据", index.ToString()));
            }
            this.ReportProgress(100, "导入完成.");
            this.ReportInfomation(string.Format("共导入{0}个Shape数据", index.ToString()));

        }


        #endregion




        /// <summary>
        /// 错误信息报告
        /// </summary>
        /// <param name="message"></param>
        private void ReportInfo(object sender, TaskAlertEventArgs e)
        {
            if (e != null)
            {
                this.ReportAlert(e.Grade, e.UserState, e.Description);
            }
        }

        /// <summary>
        /// 进度报告
        /// </summary>
        /// <param name="progress"></param>
        private void ReportPercent(object sender, TaskProgressChangedEventArgs e)
        {
            if (e != null)
            {
                this.ReportProgress(e.Percent, e.UserState);
            }
        }

    }
}
