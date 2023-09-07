/*
 * (C) 2015  鱼鳞图公司版权所有,保留所有权利 
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
    public class TaskImportDZDWShapeOperation : Task
    {
        public TaskImportDZDWShapeOperation()
        {

        }

        /// <summary>
        /// 开始执行子任务
        /// </summary>
        protected override void OnGo()
        {
            TaskImportDZDWShapeArgument metadata = Argument as TaskImportDZDWShapeArgument;
            if (metadata == null)
            {
                return;
            }
            if (metadata.CurrentZone == null)
            {
                this.ReportError("当前未选择地域");
                return;
            }
            if(metadata.Type=="Del")
            {
                this.ReportProgress(10);
                IDZDWWorkStation dzdwstation = new ContainerFactory(metadata.Database).CreateWorkstation<IDZDWWorkStation, IDZDWRepository>();
                int count = dzdwstation.Count(c => c.ZoneCode.StartsWith(metadata.CurrentZone.FullCode));
                dzdwstation.Delete(c => c.ZoneCode.StartsWith(metadata.CurrentZone.FullCode));
                this.ReportProgress(100);
                this.ReportInfomation("清空" + count + "条点状地物数据");
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
                var landdata = dq.Get(null, filename).Result as IList;
                landShapeList = landdata;
                var importShpType = ds.DataSource as IProviderShapefile;
                if (importShpType.GetGeometryType(filename) != Spatial.eGeometryType.Point)
                {
                    this.ReportError("当前Shape文件不为点文件，请重新选择点文件导入");
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

                s.ForEach(t => columnNameList.Add(t.ColumnName));
            }
            catch
            {
                this.ReportError("当前Shape文件信息有误，请检查相关文件名称");
                return;
            }
            ImportDZDWShape(columnNameList, landShapeList, metadata);
            this.ReportProgress(100);
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
        /// 导入点状地物图层shp文件
        /// </summary>
        private void ImportDZDWShape(List<string> columnNameList, IList landShapeList, TaskImportDZDWShapeArgument metadata)
        {
            double Percent = 0.0;  //百分比
            Percent = 98 / (double)landShapeList.Count;
            int index = 0;   //索引
            this.ReportProgress(0, "开始");

            IDZDWWorkStation dzdwstation = new ContainerFactory(metadata.Database).CreateWorkstation<IDZDWWorkStation, IDZDWRepository>();
            DZDW dzdw = null;
            metadata.Database.BeginTransaction();
            try
            {
                var targetSpatialReference = metadata.Database.CreateSchema().GetElementSpatialReference(
                 ObjectContext.Create(typeof(DZDW)).Schema,
                 ObjectContext.Create(typeof(DZDW)).TableName);
                string bsmvalue = string.Empty;
                foreach (var shpLandItem in landShapeList)
                {
                    dzdw = new DZDW();
                    try
                    {
                        if (metadata.importDZDWDefine.BSM != "None")
                        {
                            bsmvalue = GetproertValue(shpLandItem, metadata.importDZDWDefine.BSM);
                            //YuLinTu.ObjectExtension.GetPropertyValue(shpLandItem, metadata.importDZDWDefine.BSM).ToString();
                            if (bsmvalue == "" || bsmvalue == null)
                            {
                                dzdw.BSM = null;
                            }
                            else
                            {
                                dzdw.BSM = int.Parse(bsmvalue);
                            }
                        }
                    }
                    catch (Exception)
                    {
                        this.ReportError(string.Format("标识码字段中无法导入{0}", bsmvalue));
                        metadata.Database.RollbackTransaction();
                        return;
                    }
                    if (metadata.importDZDWDefine.YSDM != "None")
                    {
                        var ysdmvalue = GetproertValue(shpLandItem, metadata.importDZDWDefine.YSDM);
                            //YuLinTu.ObjectExtension.GetPropertyValue(shpLandItem, metadata.importDZDWDefine.YSDM).ToString();
                        dzdw.YSDM = ysdmvalue;
                    }
                    if (metadata.importDZDWDefine.DWMC != "None")
                    {
                        var dwmcvalue = GetproertValue(shpLandItem, metadata.importDZDWDefine.DWMC);
                        //YuLinTu.ObjectExtension.GetPropertyValue(shpLandItem, metadata.importDZDWDefine.DWMC).ToString();
                        dzdw.DWMC = dwmcvalue;
                    }
                    if (metadata.importDZDWDefine.Comment != "None")
                    {
                        var bzvalue = GetproertValue(shpLandItem, metadata.importDZDWDefine.Comment);
                        //YuLinTu.ObjectExtension.GetPropertyValue(shpLandItem, metadata.importDZDWDefine.Comment).ToString();
                        dzdw.Comment = bzvalue;
                    }
                    dzdw.ZoneCode = metadata.CurrentZone.FullCode;
                    dzdw.ZoneName = metadata.CurrentZone.FullName;
                    try
                    {
                        var g = YuLinTu.ObjectExtension.GetPropertyValue(shpLandItem, "Shape") as YuLinTu.Spatial.Geometry;
                        var shapevalue = YuLinTu.Spatial.Geometry.FromBytes(g.AsBinary(), 0);
                        dzdw.Shape = shapevalue == null ? null : shapevalue as YuLinTu.Spatial.Geometry;
                        dzdw.Shape.SpatialReference = targetSpatialReference;
                        dzdwstation.Add(dzdw);
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
