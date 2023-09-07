/*
 * (C) 2015  鱼鳞图公司版权所有,保留所有权利 
 */
using System;
using System.Collections;
using System.Collections.Generic;
using YuLinTu.Data;
using YuLinTu.Data.Dynamic;
using YuLinTu.Data.Shapefile;
using YuLinTu.Library.Business;
using YuLinTu.Library.Entity;
using YuLinTu.Library.WorkStation;
using YuLinTuQuality.Business.TaskBasic;

namespace YuLinTu.Component.MapFoundation
{
    /// <summary>
    /// 导入基本农田保护区图斑任务
    /// </summary>
    public class TaskImportFarmLandConserveShapeOperation : Task
    {
        #region Ctor

        /// <summary>
        /// 构造函数
        /// </summary>
        public TaskImportFarmLandConserveShapeOperation()
        { }

        #endregion

        #region Method - Override

        /// <summary>
        /// 开始任务
        /// </summary>
        protected override void OnGo()
        {
            TaskImportFarmLandConserveShapeArgument metadata = Argument as TaskImportFarmLandConserveShapeArgument;
            if (metadata == null)
            {
                return;
            }
            if (metadata.Database == null)
            {
                this.ReportError(DataBaseSourceWork.ConnectionError);
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
                var flConserveStation = metadata.Database.CreateFarmLandConserveWorkStation();
                int count = flConserveStation.Count(c => c.ZoneCode.StartsWith(metadata.CurrentZone.FullCode));
                flConserveStation.Delete(c => c.ZoneCode.StartsWith(metadata.CurrentZone.FullCode));
                this.ReportProgress(100);
                this.ReportInfomation("清空" + count + "条基本农田保护区数据");
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
            ImportFlConserveShape(columnNameList, landShapeList, metadata);
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
        /// 导入基本农田保护区图层shp文件
        /// </summary>
        private void ImportFlConserveShape(List<string> columnNameList, IList landShapeList, TaskImportFarmLandConserveShapeArgument metadata)
        {
            double percent = 98 / (double)landShapeList.Count;  //百分比
            int index = 1;   //索引
            this.ReportProgress(0, "开始");
            metadata.Database.OpenConnection();
            var flConserveStation = metadata.Database.CreateFarmLandConserveWorkStation();
            FarmLandConserve farmLand = null;
            metadata.Database.BeginTransaction();

            var schema = ObjectContext.Create(typeof(FarmLandConserve)).Schema;
            var elementName = ObjectContext.Create(typeof(FarmLandConserve)).TableName;
            var spatialReference = metadata.Database.CreateSchema().GetElementSpatialReference(schema, elementName);

            try
            {
                string codeValue = string.Empty;  //标识码值
                string featureCodeValue = string.Empty;  //要素代码值
                string conserveNumberValue = string.Empty; //保护区编号值
                string farmLandAreaValue = string.Empty;   //保护区面积值
                foreach (var shpLandItem in landShapeList)
                {
                    farmLand = new FarmLandConserve();
                    try
                    {
                        if (metadata.ImportConserveDefine.Code != "None")
                        {
                            codeValue = GetproertValue(shpLandItem, metadata.ImportConserveDefine.Code);
                            //YuLinTu.ObjectExtension.GetPropertyValue(shpLandItem, metadata.ImportConserveDefine.Code).ToString();
                            if (string.IsNullOrEmpty(codeValue))
                            {
                                farmLand.Code = null;
                            }
                            else
                            {
                                int result = 0;
                                int.TryParse(codeValue, out result);
                                farmLand.Code = result;
                            }
                        }
                        else
                        {
                            farmLand.Code = QuantityValue.ProtectArea;
                        }
                    }
                    catch
                    {
                        this.ReportError(string.Format("标识码字段中无法导入{0}", codeValue));
                        metadata.Database.RollbackTransaction();
                        return;
                    }

                    if (metadata.ImportConserveDefine.FeatureCode != "None")
                    {
                        featureCodeValue = GetproertValue(shpLandItem, metadata.ImportConserveDefine.FeatureCode);
                        //YuLinTu.ObjectExtension.GetPropertyValue(shpLandItem, metadata.ImportConserveDefine.FeatureCode).ToString();
                        farmLand.FeatureCode = featureCodeValue;
                    }
                    else
                    {
                        farmLand.FeatureCode = "251100";
                    }

                    if (metadata.ImportConserveDefine.ConserveNumber != "None")
                    {
                        conserveNumberValue = GetproertValue(shpLandItem, metadata.ImportConserveDefine.ConserveNumber);
                        //YuLinTu.ObjectExtension.GetPropertyValue(shpLandItem, metadata.ImportConserveDefine.ConserveNumber).ToString();
                        farmLand.ConserveNumber = conserveNumberValue;
                    }

                    if (metadata.ImportConserveDefine.FarmLandArea != "None")
                    {
                        try
                        {
                            farmLandAreaValue = GetproertValue(shpLandItem, metadata.ImportConserveDefine.FarmLandArea);
                            //YuLinTu.ObjectExtension.GetPropertyValue(shpLandItem, metadata.ImportConserveDefine.FarmLandArea).ToString();
                            if (string.IsNullOrEmpty(farmLandAreaValue))
                            {
                                farmLand.FarmLandArea = null;
                            }
                            else
                            {
                                double result = 0;
                                double.TryParse(farmLandAreaValue, out result);
                                farmLand.FarmLandArea = result;
                            }
                        }
                        catch
                        {
                            this.ReportError(string.Format("基本农田面积字段中无法导入{0}", farmLandAreaValue));
                            metadata.Database.RollbackTransaction();
                            return;
                        }
                    }

                    try
                    {
                        farmLand.ZoneCode = metadata.CurrentZone.FullCode;
                        farmLand.ZoneName = metadata.CurrentZone.FullName;
                        var g = YuLinTu.ObjectExtension.GetPropertyValue(shpLandItem, "Shape") as YuLinTu.Spatial.Geometry;
                        var shapevalue = YuLinTu.Spatial.Geometry.FromBytes(g.AsBinary(), 0);
                        farmLand.Shape = shapevalue == null ? null : shapevalue as YuLinTu.Spatial.Geometry;
                        
                        farmLand.Shape.SpatialReference = new Spatial.SpatialReference(spatialReference.WKID, spatialReference.WKT);
                        flConserveStation.Add(farmLand);
                    }
                    catch (Exception ex)
                    {
                        this.ReportError(string.Format("第{0}条shape数据无效，无法导入" + ex.Message, index));
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
                this.ReportError("导入基本农田保护区图斑数据时发生错误！");
                metadata.Database.RollbackTransaction();
                YuLinTu.Library.Log.Log.WriteException(this, "ImportFlConserveShape(导入基本农田保护区图斑数据失败!)", ex.Message + ex.StackTrace);
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
