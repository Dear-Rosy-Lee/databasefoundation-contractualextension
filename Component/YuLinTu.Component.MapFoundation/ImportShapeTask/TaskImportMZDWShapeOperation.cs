/*
 * (C) 2025  鱼鳞图公司版权所有,保留所有权利 
 */
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YuLinTu.Data;
using YuLinTu.Data.Dynamic;
using YuLinTu.Data.Shapefile;
using YuLinTu.Library.Entity;
using YuLinTu.Library.Repository;
using YuLinTu.Library.WorkStation;

namespace YuLinTu.Component.MapFoundation
{
    public class TaskImportMZDWShapeOperation : Task
    {
        public TaskImportMZDWShapeOperation()
        {

        }

        /// <summary>
        /// 开始执行子任务
        /// </summary>
        protected override void OnGo()
        {
            TaskImportMZDWShapeArgument metadata = Argument as TaskImportMZDWShapeArgument;
            if (metadata == null)
            {
                return;
            }
            if (metadata.CurrentZone == null)
            {
                this.ReportError("当前未选择地域");
                return;
            }
            if (metadata.Type == "Del")
            {
                this.ReportProgress(10);
                IMZDWWorkStation mzdwstation = new ContainerFactory(metadata.Database).CreateWorkstation<IMZDWWorkStation, IMZDWRepository>();
                int count = mzdwstation.Count(c => c.ZoneCode.StartsWith(metadata.CurrentZone.FullCode));

                mzdwstation.Delete(c => c.ZoneCode.StartsWith(metadata.CurrentZone.FullCode));
                this.ReportProgress(100);
                this.ReportInfomation("清空" + count + "条面状地物数据");
                return;
            }
            //获取当前路径下shape数据
            IList landShapeList = null;
            List<string> columnNameList = new List<string>();
            try
            {
                string filepath = System.IO.Path.GetDirectoryName(metadata.FileName);
                string filename = System.IO.Path.GetFileNameWithoutExtension(metadata.FileName);
                var ds = ProviderShapefile.CreateDataSource(filepath, false) as IDbContext;
                var dq = new DynamicQuery(ds);
                var importShpType = ds.DataSource as IProviderShapefile;
                if (importShpType.GetGeometryType(filename) != Spatial.eGeometryType.Polygon)
                {
                    this.ReportError("当前Shape文件不为面文件，请重新选择面文件导入");
                    return;
                }
                var landdata = dq.Get(null, filename).Result as IList;
                landShapeList = landdata;
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

                s.ForEach(t => columnNameList.Add(t.ColumnName));
            }
            catch
            {
                this.ReportError("当前Shape文件信息有误，请检查相关文件名称");
                return;
            }
            ImportMZDWShape(columnNameList, landShapeList, metadata);

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
        /// 导入面状地物图层shp文件
        /// </summary>
        private void ImportMZDWShape(List<string> columnNameList, IList landShapeList, TaskImportMZDWShapeArgument metadata)
        {
            double Percent = 0.0;  //百分比
            Percent = 98 / (double)landShapeList.Count;
            int index = 0;   //索引
            this.ReportProgress(0, "开始");

            IMZDWWorkStation mzdwstation = new ContainerFactory(metadata.Database).CreateWorkstation<IMZDWWorkStation, IMZDWRepository>();
            MZDW mzdw = null;
            metadata.Database.BeginTransaction();
            try
            {
                var targetSpatialReference = metadata.Database.CreateSchema().GetElementSpatialReference(
                 ObjectContext.Create(typeof(MZDW)).Schema,
                 ObjectContext.Create(typeof(MZDW)).TableName);
                string bsmvalue = string.Empty;
                string areavalue = string.Empty;
                foreach (var shpLandItem in landShapeList)
                {
                    mzdw = new MZDW();
                    try
                    {
                        if (metadata.importMZDWDefine.BSM != "None")
                        {
                            bsmvalue = GetproertValue(shpLandItem, metadata.importMZDWDefine.BSM);
                            //YuLinTu.ObjectExtension.GetPropertyValue(shpLandItem, metadata.importMZDWDefine.BSM).ToString();
                            if (bsmvalue == "" || bsmvalue == null)
                            {
                                mzdw.BSM = null;
                            }
                            else
                            {
                                mzdw.BSM = int.Parse(bsmvalue);
                            }
                        }
                    }
                    catch (Exception)
                    {
                        this.ReportError(string.Format("标识码字段中无法导入{0}", bsmvalue));
                        metadata.Database.RollbackTransaction();
                        return;
                    }
                    if (metadata.importMZDWDefine.YSDM != "None")
                    {
                        var ysdmvalue = GetproertValue(shpLandItem, metadata.importMZDWDefine.YSDM);
                        //YuLinTu.ObjectExtension.GetPropertyValue(shpLandItem, metadata.importMZDWDefine.YSDM).ToString();
                        mzdw.YSDM = ysdmvalue;
                    }

                    if (metadata.importMZDWDefine.DWMC != "None")
                    {
                        var dwmcvalue = GetproertValue(shpLandItem, metadata.importMZDWDefine.DWMC);
                        //YuLinTu.ObjectExtension.GetPropertyValue(shpLandItem, metadata.importMZDWDefine.DWMC).ToString();
                        mzdw.DWMC = dwmcvalue;
                    }

                    if (metadata.importMZDWDefine.MJ != "None")
                    {
                        try
                        {
                            areavalue = GetproertValue(shpLandItem, metadata.importMZDWDefine.MJ);
                            //YuLinTu.ObjectExtension.GetPropertyValue(shpLandItem, metadata.importMZDWDefine.MJ).ToString();
                            if(areavalue!="")
                               mzdw.Area = double.Parse(areavalue);
                        }
                        catch (Exception)
                        {
                            this.ReportError(string.Format("面积字段中无法导入{0}", areavalue));
                            metadata.Database.RollbackTransaction();
                            return;
                        }
                    }

                    if (metadata.importMZDWDefine.Comment != "None")
                    {
                        var bzvalue = GetproertValue(shpLandItem, metadata.importMZDWDefine.Comment);
                        //YuLinTu.ObjectExtension.GetPropertyValue(shpLandItem, metadata.importMZDWDefine.Comment).ToString();
                        mzdw.Comment = bzvalue;
                    }

                    try
                    {
                        mzdw.ZoneCode = metadata.CurrentZone.FullCode;
                        mzdw.ZoneName = metadata.CurrentZone.FullName;
                        var g = YuLinTu.ObjectExtension.GetPropertyValue(shpLandItem, "Shape") as YuLinTu.Spatial.Geometry;
                        var shapevalue = YuLinTu.Spatial.Geometry.FromBytes(g.AsBinary(), 0);
                        mzdw.Shape = shapevalue == null ? null : shapevalue as YuLinTu.Spatial.Geometry;
                        mzdw.Shape.SpatialReference = targetSpatialReference;
                        mzdwstation.Add(mzdw);

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
