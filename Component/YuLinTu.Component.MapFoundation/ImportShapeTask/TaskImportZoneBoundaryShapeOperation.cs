/*
 * (C) 2025  鱼鳞图公司版权所有,保留所有权利 
 */
using Quality.Business.TaskBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using YuLinTu.Data;
using YuLinTu.Data.Dynamic;
using YuLinTu.Data.Shapefile;
using YuLinTu.Library.Business;
using YuLinTu.Library.Entity;
using YuLinTu.Library.WorkStation;

namespace YuLinTu.Component.MapFoundation
{
    /// <summary>
    /// 导入区域界线图斑任务
    /// </summary>
    public class TaskImportZoneBoundaryShapeOperation : Task
    {
        #region Ctor

        /// <summary>
        /// 构造函数
        /// </summary>
        public TaskImportZoneBoundaryShapeOperation()
        { }

        #endregion

        #region Method - Override

        /// <summary>
        /// 开始任务
        /// </summary>
        protected override void OnGo()
        {
            TaskImportZoneBoundaryShapeArgument metadata = Argument as TaskImportZoneBoundaryShapeArgument;
            if (metadata == null)
            {
                return;
            }
            if (metadata.Database == null)
            {
                this.ReportError(DataBaseSourceWork.ConnectionError);
                return;
            }
            if (metadata.Type == "Del")
            {
                var zoneBoundaryStation = metadata.Database.CreateZoneBoundaryWorkStation();
                this.ReportProgress(10);
                int count = zoneBoundaryStation.Count();
                zoneBoundaryStation.Delete();
                this.ReportProgress(100);
                this.ReportInfomation("清空" + count + "条区域界线数据！");
                return;
            }
            //if (metadata.CurrentZone == null)
            //{
            //    this.ReportError("当前未选择地域");
            //    return;
            //}
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
                if (importShpType.GetGeometryType(filename) != Spatial.eGeometryType.Polyline)
                {
                    this.ReportError("当前Shape文件不为线文件，请重新选择线文件导入");
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
            ImportZbShape(columnNameList, landShapeList, metadata);
        }

        #endregion

        #region Method - Private
        private string GetproertValue(object shapeData, string Value)
        {
            object obj = YuLinTu.ObjectExtension.GetPropertyValue(shapeData, Value);
            if (obj == null)
                return "";
            return obj.ToString();
        }
        /// <summary>
        /// 导入区域界线图层shp文件
        /// </summary>
        private void ImportZbShape(List<string> columnNameList, IList landShapeList, TaskImportZoneBoundaryShapeArgument metadata)
        {
            double percent = 98 / (double)landShapeList.Count;  //百分比
            int index = 1;   //索引
            this.ReportProgress(0, "开始");
            metadata.Database.OpenConnection();
            var zoneBoundaryStation = metadata.Database.CreateZoneBoundaryWorkStation();
            ZoneBoundary zoneBoundary = null;
            metadata.Database.BeginTransaction();
            try
            {
                string codeValue = string.Empty;  //标识码值
                string featureCodeValue = string.Empty;  //要素代码值
                string boundaryLineTypeValue = string.Empty; //界线类型值
                string boundaryLineNatureValue = string.Empty; //界线性质值

                string farmLandAreaValue = string.Empty;   //保护区面积值
                foreach (var shpLandItem in landShapeList)
                {
                    zoneBoundary = new ZoneBoundary();
                    try
                    {
                        if (metadata.ImportZoneBoundaryDefine.Code != "None")
                        {
                            codeValue = GetproertValue(shpLandItem, metadata.ImportZoneBoundaryDefine.Code);
                            //YuLinTu.ObjectExtension.GetPropertyValue(shpLandItem, metadata.ImportZoneBoundaryDefine.Code).ToString();
                            if (string.IsNullOrEmpty(codeValue))
                            {
                                zoneBoundary.Code = null;
                            }
                            else
                            {
                                int result = 0;
                                int.TryParse(codeValue, out result);
                                zoneBoundary.Code = result;
                            }
                        }
                        else
                        {
                            zoneBoundary.Code = QuantityValue.AreaLine;
                        }
                    }
                    catch
                    {
                        this.ReportError(string.Format("标识码字段中无法导入{0}", codeValue));
                        metadata.Database.RollbackTransaction();
                        return;
                    }

                    if (metadata.ImportZoneBoundaryDefine.FeatureCode != "None")
                    {
                        featureCodeValue = GetproertValue(shpLandItem, metadata.ImportZoneBoundaryDefine.FeatureCode);
                        //YuLinTu.ObjectExtension.GetPropertyValue(shpLandItem, metadata.ImportZoneBoundaryDefine.FeatureCode).ToString();
                        zoneBoundary.FeatureCode = featureCodeValue;
                    }
                    else
                    {
                        zoneBoundary.FeatureCode = "161051";
                    }

                    if (metadata.ImportZoneBoundaryDefine.BoundaryLineType != "None")
                    {
                        boundaryLineTypeValue = GetproertValue(shpLandItem, metadata.ImportZoneBoundaryDefine.BoundaryLineType);
                        //YuLinTu.ObjectExtension.GetPropertyValue(shpLandItem, metadata.ImportZoneBoundaryDefine.BoundaryLineType).ToString();
                        zoneBoundary.BoundaryLineType = boundaryLineTypeValue;
                    }
                    else
                    {
                        zoneBoundary.BoundaryLineType = "670900";
                    }

                    if (metadata.ImportZoneBoundaryDefine.BoundaryLineNature != "None")
                    {
                        boundaryLineNatureValue = GetproertValue(shpLandItem, metadata.ImportZoneBoundaryDefine.BoundaryLineNature);
                        //YuLinTu.ObjectExtension.GetPropertyValue(shpLandItem, metadata.ImportZoneBoundaryDefine.BoundaryLineNature).ToString();
                        zoneBoundary.BoundaryLineNature = boundaryLineNatureValue;
                    }
                    else
                    {
                        zoneBoundary.BoundaryLineNature = "600009";
                    }
                    if (metadata.ImportZoneBoundaryDefine.ZoneCode != "None")
                    {
                        var zoneCode = GetproertValue(shpLandItem, metadata.ImportZoneBoundaryDefine.ZoneCode);
                        zoneBoundary.ZoneCode = zoneCode;
                    }
                    if (metadata.ImportZoneBoundaryDefine.ZoneName != "None")
                    {
                        var zonename = GetproertValue(shpLandItem, metadata.ImportZoneBoundaryDefine.ZoneName);
                        zoneBoundary.ZoneName = zonename;
                    }
                    try
                    {
                        //zoneBoundary.SenderCode = metadata.CurrentZone.FullCode;
                        //zoneBoundary.SenderName = metadata.CurrentZone.FullName;
                        var g = YuLinTu.ObjectExtension.GetPropertyValue(shpLandItem, "Shape") as YuLinTu.Spatial.Geometry;
                        var shapevalue = YuLinTu.Spatial.Geometry.FromBytes(g.AsBinary(), 0);
                        zoneBoundary.Shape = shapevalue == null ? null : shapevalue as YuLinTu.Spatial.Geometry;
                        var targetSpatialReference = metadata.Database.CreateSchema().GetElementSpatialReference(ObjectContext.Create(typeof(ZoneBoundary)).Schema,
                           ObjectContext.Create(typeof(ZoneBoundary)).TableName);
                        zoneBoundary.Shape.SpatialReference = targetSpatialReference;
                        zoneBoundaryStation.Add(zoneBoundary);
                    }
                    catch
                    {
                        this.ReportError(string.Format("第{0}条shape数据无效，无法导入", index));
                        metadata.Database.RollbackTransaction();
                        return;
                    }
                    this.ReportProgress((int)(percent * (index++)), string.Format("导入第{0}条数据", index.ToString()));
                }
                metadata.Database.CommitTransaction();
                this.ReportProgress(100, "导入完成.");
                this.ReportInfomation(string.Format("共导入{0}条Shape数据", (index - 1).ToString()));
            }
            catch (Exception ex)
            {
                this.ReportError("导入区域界线图斑数据时发生错误！");
                metadata.Database.RollbackTransaction();
                YuLinTu.Library.Log.Log.WriteException(this, "ImportZbShape(导入区域界线图斑数据失败!)", ex.Message + ex.StackTrace);
            }
            finally
            {
                if (metadata.Database != null)
                {
                    metadata.Database.CloseConnection();
                }
                metadata.Dispose();
                metadata = null;
                columnNameList.Clear();
                columnNameList = null;
                landShapeList.Clear();
                landShapeList = null;

                GC.Collect();
            }
        }

        #endregion
    }
}
