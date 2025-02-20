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
using YuLinTu.Spatial;

namespace YuLinTu.Component.MapFoundation
{
    public class TaskGenerateQYJXShapeOperation : Task
    {
        public TaskGenerateQYJXShapeOperation()
        {

        }

        /// <summary>
        /// 开始执行子任务
        /// </summary>
        protected override void OnGo()
        {
            TaskGenerateQYJXShapeArgument metadata = Argument as TaskGenerateQYJXShapeArgument;
            if (metadata == null)
            {
                return;
            }
            IDbContext dbContext = DataBaseSourceWork.GetDataBaseSource();
            if (dbContext == null)
            {
                this.ReportError("数据连接失败");
                this.ReportProgress(100);
                return;
            }

            GenerateDataShape(dbContext, metadata);
            this.ReportProgress(100);
        }

        #region privateMethod

        /// <summary>
        /// 导入点状地物图层shp文件
        /// </summary>
        private void GenerateDataShape(IDbContext dbContext, TaskGenerateQYJXShapeArgument metadata)
        {
            IZoneWorkStation zonestation = new ContainerFactory(dbContext).CreateWorkstation<IZoneWorkStation, IZoneRepository>();
            IZoneBoundaryWorkStation zoneboundarystation = new ContainerFactory(dbContext).CreateWorkstation<IZoneBoundaryWorkStation, IZoneBoundaryRepository>();

            List<Zone> zones = zonestation.Get();
            List<ZoneBoundary> zoneboundarys = zoneboundarystation.Get();
            double Percent = 0.0;  //百分比
            Percent = 98 / (double)zones.Count;
            int index = 0;   //索引
            this.ReportProgress(0, "开始");
            dbContext.BeginTransaction();
            List<ZoneBoundary> zoneBoundary = new List<ZoneBoundary>();
            try
            {
                foreach (var zone in zones)
                {
                    if (zone.Shape == null)
                        continue;
                    ZoneBoundary boundary = new ZoneBoundary();
                    zoneboundarystation.Delete(c => c.ZoneCode == zone.FullCode);
                    boundary.BoundaryLineNature = metadata.selectLineNature.Key;
                    boundary.BoundaryLineType = metadata.selectLineType.Key;
                    boundary.Code = 0;
                    boundary.FeatureCode = "161051";
                    boundary.ZoneCode = zone.FullCode;
                    boundary.ZoneName = zone.FullName;
                    Coordinate[] point = zone.Shape.ToCoordinates();
                    List<Coordinate> points = new List<Coordinate>();
                    for (int i = 0; i < point.Count(); i++)
                    {
                        points.Add(point[i]);
                    }
                    int InsertInt = -1;
                    try
                    {
                        boundary.Shape = Geometry.CreatePolyline(points);
                        zoneBoundary.Add(boundary);
                        InsertInt = zoneboundarystation.Add(boundary);
                    }
                    catch (Exception ex)
                    {
                        this.ReportError("" + boundary.ZoneName + "区域界线生成失败,请检查数据是否正确" + ex.Message);
                        //this.ReportError("异常：" + ex.Message);
                        //this.ReportProgress(100, "异常：" + ex.Message);
                        //dbContext.RollbackTransaction();
                        return;
                    }

                    if (InsertInt == -1)
                    {
                        this.ReportProgress(100, "" + boundary.ZoneName + "区域界线生成失败");
                        dbContext.RollbackTransaction();
                    }
                    index++;
                    this.ReportProgress((int)(Percent * index), string.Format("生成第{0}个数据", index.ToString()));
                }
            }
            catch (Exception ex)
            {
                this.ReportError("生成图斑数据时发生错误！" + ex.Message);
                dbContext.RollbackTransaction();
                return;
            }

            dbContext.CommitTransaction();
            this.ReportProgress(100, "生成完成.");
            this.ReportInfomation(string.Format("共导入{0}个Shape数据", index.ToString()));
            //}
            //catch (Exception ex)
            //{
            //    dbContext.RollbackTransaction();
            //    this.ReportError("生成图斑数据时发生错误！");
            //    YuLinTu.Library.Log.Log.WriteException(this, "GenerateShape", ex.Message + ex.StackTrace);
            //}
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
