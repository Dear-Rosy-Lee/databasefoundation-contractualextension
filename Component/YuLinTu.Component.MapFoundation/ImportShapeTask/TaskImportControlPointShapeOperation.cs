/*
 * (C) 2015  鱼鳞图公司版权所有,保留所有权利 
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
    /// 导入控制点图斑任务
    /// </summary>
    public class TaskImportControlPointShapeOperation : Task
    {
        #region Ctor

        /// <summary>
        /// 构造函数
        /// </summary>
        public TaskImportControlPointShapeOperation()
        { }

        #endregion

        #region Method - Override

        /// <summary>
        /// 开始任务
        /// </summary>
        protected override void OnGo()
        {
            TaskImportControlPointShapeArgument metadata = Argument as TaskImportControlPointShapeArgument;
            if (metadata == null)
            {
                return;
            }
            if (metadata.Database == null)
            {
                this.ReportError(DataBaseSourceWork.ConnectionError);
                return;
            }            
            if(metadata.Type=="Del")
            {
                var controlPointStation = metadata.Database.CreateControlPointWorkStation();
                this.ReportProgress(10);
                int count = controlPointStation.Count();
                controlPointStation.Delete();
                this.ReportProgress(100);
                this.ReportInfomation("清空"+count+"条控制点数据");
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
                if (importShpType.GetGeometryType(filename) != Spatial.eGeometryType.Point)
                {
                    this.ReportError("当前Shape文件不为点文件，请重新选择点文件导入");
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
            ImportCpShape(columnNameList, landShapeList, metadata);
            this.ReportProgress(100);
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
        /// 导入控制点图层shp文件
        /// </summary>
        private void ImportCpShape(List<string> columnNameList, IList landShapeList, TaskImportControlPointShapeArgument metadata)
        {
            double percent = 98 / (double)landShapeList.Count;  //百分比
            int index = 1;   //索引
            this.ReportProgress(0, "开始");
            metadata.Database.OpenConnection();
            var controlPointStation = metadata.Database.CreateControlPointWorkStation();
           
            ControlPoint controlPoint = null;
            metadata.Database.BeginTransaction();
            try
            {
                string codeValue = string.Empty;  //标识码值
                string featureCodeValue = string.Empty;  //要素代码值
                string pointNameValue = string.Empty; //控制点名称值
                string pointNumberValue = string.Empty; //控制点点号值
                string pointTypeValue = string.Empty; //控制点类型值
                string pointRankValue = string.Empty; //控制点等级值
                string bsTypeValue = string.Empty; //标石类型值
                string bzTypeValue = string.Empty; //标志类型值
                string pointStateValue = string.Empty; //控制点状态值
                string dzjValue = string.Empty; //点之记值
                string x2000Value = string.Empty; // X_2000a值
                string y2000Value = string.Empty; //Y_2000a值
                string x80Value = string.Empty; //X(E)_XA80a值
                string y80Value = string.Empty; //Y(E)_XA80a值

                string farmLandAreaValue = string.Empty;   //保护区面积值
                foreach (var shpLandItem in landShapeList)
                {
                    controlPoint = new ControlPoint();
                    try
                    {
                        if (metadata.ImportControlPointDefine.Code != "None")
                        {
                            codeValue = GetproertValue(shpLandItem, metadata.ImportControlPointDefine.Code);
                            if (string.IsNullOrEmpty(codeValue))
                            {
                                controlPoint.Code = null;
                            }
                            else
                            {
                                int result = 0;
                                int.TryParse(codeValue, out result);
                                controlPoint.Code = result;
                            }
                        }
                        else
                        {
                            controlPoint.Code = QuantityValue.ControlPoint;
                        }
                    }
                    catch
                    {
                        this.ReportError(string.Format("标识码字段中无法导入{0}", codeValue));
                        metadata.Database.RollbackTransaction();

                        return;
                    }

                    if (metadata.ImportControlPointDefine.FeatureCode != "None")
                    {
                        featureCodeValue = GetproertValue(shpLandItem, metadata.ImportControlPointDefine.FeatureCode);//YuLinTu.ObjectExtension.GetPropertyValue(shpLandItem, metadata.ImportControlPointDefine.FeatureCode).ToString();
                        controlPoint.FeatureCode = featureCodeValue;
                    }
                    else
                    {
                        controlPoint.FeatureCode = "111000";
                    }

                    if (metadata.ImportControlPointDefine.PointName != "None")
                    {
                        pointNameValue = GetproertValue(shpLandItem, metadata.ImportControlPointDefine.PointName);
                        //YuLinTu.ObjectExtension.GetPropertyValue(shpLandItem, metadata.ImportControlPointDefine.PointName).ToString();
                        controlPoint.PointName = pointNameValue;
                    }

                    if (metadata.ImportControlPointDefine.PointNumber != "None")
                    {
                        pointNumberValue = GetproertValue(shpLandItem, metadata.ImportControlPointDefine.PointNumber);
                        //YuLinTu.ObjectExtension.GetPropertyValue(shpLandItem, metadata.ImportControlPointDefine.PointNumber).ToString();
                        controlPoint.PointNumber = pointNumberValue;
                    }

                    if (metadata.ImportControlPointDefine.PointType != "None")
                    {
                        pointTypeValue = GetproertValue(shpLandItem, metadata.ImportControlPointDefine.PointType);
                        //YuLinTu.ObjectExtension.GetPropertyValue(shpLandItem, metadata.ImportControlPointDefine.PointType).ToString();
                        if(pointTypeValue=="")
                        {
                            this.ReportError(string.Format("第{0}条shape数据的控制点类型不能为空", index));
                            return;
                        }
                        controlPoint.PointType = pointTypeValue;
                    }

                    if (metadata.ImportControlPointDefine.PointRank != "None")
                    {
                        pointRankValue = GetproertValue(shpLandItem, metadata.ImportControlPointDefine.PointRank);
                        //YuLinTu.ObjectExtension.GetPropertyValue(shpLandItem, metadata.ImportControlPointDefine.PointRank).ToString();
                        controlPoint.PointRank = pointRankValue;
                    }

                    if (metadata.ImportControlPointDefine.BsType != "None")
                    {
                        bsTypeValue = GetproertValue(shpLandItem, metadata.ImportControlPointDefine.BsType);
                        //YuLinTu.ObjectExtension.GetPropertyValue(shpLandItem, metadata.ImportControlPointDefine.BsType).ToString();
                        controlPoint.BsType = bsTypeValue;
                    }

                    if (metadata.ImportControlPointDefine.BzType != "None")
                    {
                        bzTypeValue = GetproertValue(shpLandItem, metadata.ImportControlPointDefine.BsType);
                        //YuLinTu.ObjectExtension.GetPropertyValue(shpLandItem, metadata.ImportControlPointDefine.BzType).ToString();
                        controlPoint.BzType = bzTypeValue;
                    }

                    if (metadata.ImportControlPointDefine.PointState != "None")
                    {
                        pointStateValue = GetproertValue(shpLandItem, metadata.ImportControlPointDefine.PointState);
                        //YuLinTu.ObjectExtension.GetPropertyValue(shpLandItem, metadata.ImportControlPointDefine.PointState).ToString();
                        controlPoint.PointState = pointStateValue;
                    }

                    if (metadata.ImportControlPointDefine.Dzj != "None")
                    {
                        dzjValue = GetproertValue(shpLandItem, metadata.ImportControlPointDefine.Dzj);
                        //YuLinTu.ObjectExtension.GetPropertyValue(shpLandItem, metadata.ImportControlPointDefine.Dzj).ToString();
                        controlPoint.Dzj = dzjValue;
                    }

                    if (metadata.ImportControlPointDefine.X2000 != "None")
                    {
                        try
                        {
                            x2000Value = GetproertValue(shpLandItem, metadata.ImportControlPointDefine.X2000);
                            //YuLinTu.ObjectExtension.GetPropertyValue(shpLandItem, metadata.ImportControlPointDefine.X2000).ToString();
                            if (string.IsNullOrEmpty(x2000Value))
                            {
                                controlPoint.X2000 = null;
                            }
                            else
                            {
                                double result = 0;
                                double.TryParse(x2000Value, out result);
                                controlPoint.X2000 = result;
                            }
                        }
                        catch
                        {
                            this.ReportError(string.Format("控制点X_2000a字段中无法导入{0}", x2000Value));
                            metadata.Database.RollbackTransaction();
                            return;
                        }
                    }

                    if (metadata.ImportControlPointDefine.Y2000 != "None")
                    {
                        try
                        {
                            y2000Value = GetproertValue(shpLandItem, metadata.ImportControlPointDefine.Y2000);
                            //YuLinTu.ObjectExtension.GetPropertyValue(shpLandItem, metadata.ImportControlPointDefine.Y2000).ToString();
                            if (string.IsNullOrEmpty(y2000Value))
                            {
                                controlPoint.Y2000 = null;
                            }
                            else
                            {
                                double result = 0;
                                double.TryParse(y2000Value, out result);
                                controlPoint.Y2000 = result;
                            }
                        }
                        catch
                        {
                            this.ReportError(string.Format("控制点Y_2000a字段中无法导入{0}", y2000Value));
                            metadata.Database.RollbackTransaction();
                            return;
                        }
                    }

                    if (metadata.ImportControlPointDefine.X80 != "None")
                    {
                        try
                        {
                            x80Value = GetproertValue(shpLandItem, metadata.ImportControlPointDefine.X80);
                            //YuLinTu.ObjectExtension.GetPropertyValue(shpLandItem, metadata.ImportControlPointDefine.X80).ToString();
                            if (string.IsNullOrEmpty(x80Value))
                            {
                                controlPoint.X80 = null;
                            }
                            else
                            {
                                double result = 0;
                                double.TryParse(x80Value, out result);
                                controlPoint.X80 = result;
                            }
                        }
                        catch
                        {
                            this.ReportError(string.Format("控制点X(E)_XA80a字段中无法导入{0}", x80Value));
                            metadata.Database.RollbackTransaction();
                            return;
                        }
                    }

                    if (metadata.ImportControlPointDefine.Y80 != "None")
                    {
                        try
                        {
                            y80Value = GetproertValue(shpLandItem, metadata.ImportControlPointDefine.Y80);
                            //YuLinTu.ObjectExtension.GetPropertyValue(shpLandItem, metadata.ImportControlPointDefine.Y80).ToString();
                            if (string.IsNullOrEmpty(y80Value))
                            {
                                controlPoint.Y80 = null;
                            }
                            else
                            {
                                double result = 0;
                                double.TryParse(y80Value, out result);
                                controlPoint.Y80 = result;
                            }
                        }
                        catch
                        {
                            this.ReportError(string.Format("控制点Y(E)_XA80a字段中无法导入{0}", y80Value));
                            metadata.Database.RollbackTransaction();
                            return;
                        }
                    }

                    try
                    {
                        //controlPoint.SenderCode = metadata.CurrentZone.FullCode;
                        //controlPoint.SenderName = metadata.CurrentZone.FullName;
                        var g = YuLinTu.ObjectExtension.GetPropertyValue(shpLandItem, "Shape") as YuLinTu.Spatial.Geometry;
                        var shapevalue = YuLinTu.Spatial.Geometry.FromBytes(g.AsBinary(), 0);
                        controlPoint.Shape = shapevalue == null ? null : shapevalue as YuLinTu.Spatial.Geometry;
                        var targetSpatialReference = metadata.Database.CreateSchema().GetElementSpatialReference(ObjectContext.Create(typeof(ControlPoint)).Schema,
                            ObjectContext.Create(typeof(ControlPoint)).TableName);
                        controlPoint.Shape.SpatialReference = targetSpatialReference;
                        if(controlPoint.PointType==null || controlPoint.PointType=="")
                        {
                            this.ReportError(string.Format("第{0}条shape数据的控制点类型不能为空", index));
                            return;
                        }
                        controlPointStation.Add(controlPoint);
                    }
                    catch (Exception ex)
                    {
                        this.ReportError(string.Format("第{0}条shape数据无效，无法导入"+ex.Message, index));
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
                this.ReportError("导入控制点图斑数据时发生错误！");
                metadata.Database.RollbackTransaction();
                YuLinTu.Library.Log.Log.WriteException(this, "ImportCpShape(导入控制点图斑数据失败!)", ex.Message + ex.StackTrace);
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
