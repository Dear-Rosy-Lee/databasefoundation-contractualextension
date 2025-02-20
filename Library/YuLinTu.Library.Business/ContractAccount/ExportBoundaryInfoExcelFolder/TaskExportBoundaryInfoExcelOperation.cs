/*
 * (C) 2025  鱼鳞图公司版权所有,保留所有权利 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using YuLinTu.Library.Entity;
using YuLinTu;
using YuLinTu.Data;
using System.Diagnostics;
using System.IO;
using System.Collections;
using YuLinTu.Data.Dynamic;
using YuLinTu.Data.Shapefile;
using YuLinTu.Library.Office;

namespace YuLinTu.Library.Business
{
    /// <summary>
    /// 界址信息表
    /// </summary>
    public class TaskExportBoundaryInfoExcelOperation : Task
    {
        #region Fields

        private bool returnValue;
        private string openFilePath;  //打开文件路径
        private IDbContext dbContext;  //数据源
        private int exportLandCount;  //已导出界址信息的地块数
        private SystemSetDefine SystemSetDefine = SystemSetDefine.GetIntence();
        private bool IsShow;
        #endregion

        #region Properties

        #endregion

        #region Ctor

        /// <summary>
        /// 构造函数
        /// </summary>
        public TaskExportBoundaryInfoExcelOperation()
        {
        }

        #endregion

        #region Methods - Override

        /// <summary>
        /// 开始操作
        /// </summary>
        protected override void OnGo()
        {
            returnValue = true;
            TaskExportBoundaryInfoExcelArgument metadata = Argument as TaskExportBoundaryInfoExcelArgument;
            if (metadata == null)
            {
                return;
            }
            string fileName = metadata.FileName;

            dbContext = metadata.Database;
            Zone zone = metadata.CurrentZone;
            openFilePath = metadata.FileName;

            //导出界址点成果表
            ExportBoundaryInfoExcelByZone(metadata, zone, openFilePath);

            if (returnValue)
            {
                CanOpenResult = true;
            }

            GC.Collect();
        }

        /// <summary>
        /// 打开文件
        /// </summary>
        public override void OpenResult()
        {
            System.Diagnostics.Process.Start(openFilePath);
            base.OpenResult();
        }

        protected override void OnStopped()
        {
            base.OnStopped();
            GC.Collect();
        }

        protected override void OnTerminate(Exception ex)
        {
            base.OnTerminate(ex);
            GC.Collect();
        }

        #endregion

        #region Methods - ExportBusiness

        /// <summary>
        /// 导出界址信息调查表
        /// </summary>
        private void ExportBoundaryInfoExcelByZone(TaskExportBoundaryInfoExcelArgument metadata, Zone currentZone, string fileName)
        {
            if (currentZone == null)
            {
                this.ReportError("未选择导出数据的地域!");
                return;
            }
            this.ReportProgress(1, "开始");
            //this.ReportProgress(5, "正在获取表");
            string descZone = GetMarkDesc(currentZone, dbContext);
            try
            {
                var landStation = dbContext.CreateContractLandWorkstation();
                List<ContractLand> listLand = landStation.GetCollection(currentZone.FullCode, eLevelOption.Self);  //(metadata.CurrentZone.FullCode, eVirtualPersonStatus.Right, eLevelOption.Self);
                if (listLand == null || listLand.Count == 0)
                {
                    //地域下无地块数据数据
                    this.ReportWarn(string.Format("{0}无地块数据", descZone));
                    this.ReportProgress(100, "完成");
                    returnValue = false;
                    return;
                }
                var listGeoLand = listLand.FindAll(c => c.Shape != null);
                if (listGeoLand == null || listGeoLand.Count == 0)
                {
                    //地域下无空间地块数据
                    this.ReportWarn(string.Format("{0}无空间地块数据", descZone));
                    this.ReportProgress(100, "完成");
                    returnValue = false;
                    return;
                }
                int geoLandsCount = listGeoLand.Count;
                var dotStation = dbContext.CreateBoundaryAddressDotWorkStation();
                var coilStation = dbContext.CreateBoundaryAddressCoilWorkStation();
                var currentZoneDots = dotStation.GetByZoneCode(currentZone.FullCode, eLevelOption.Self);
                var currentZoneCoils = coilStation.GetByZoneCode(currentZone.FullCode, eLevelOption.Self);
                //var currentZoneDots = dotStation.GetByZoneCode(currentZone.FullCode, eSearchOption.Precision, ((int)eLandPropertyRightType.AgricultureLand).ToString());
                //var currentZoneCoils = coilStation.GetByZoneCode(currentZone.FullCode, eSearchOption.Precision, eLandPropertyRightType.AgricultureLand);
                if (currentZoneDots == null || currentZoneDots.Count == 0)
                {
                    //地域下无界址点数据
                    this.ReportWarn(string.Format("{0}无界址点数据", descZone));
                    this.ReportProgress(100, "完成");
                    returnValue = false;
                    return;
                }
                if (currentZoneCoils == null || currentZoneCoils.Count == 0)
                {
                    //地域下无界址线数据
                    this.ReportWarn(string.Format("{0}无界址线数据", descZone));
                    this.ReportProgress(100, "完成");
                    returnValue = false;
                    return;
                }
                listGeoLand.LandNumberFormat(SystemSetDefine);
                IsShow = metadata.IsShow;
                returnValue = ExportBoundaryInfoExcel(currentZone, fileName, listGeoLand, currentZoneDots, currentZoneCoils, metadata.DictList, descZone);  //percent, 5 + percent * (indexOfZone));
                if (returnValue)
                {
                    this.ReportProgress(100, "完成");
                    this.ReportInfomation(string.Format("{0}共{1}个空间地块,已导出{1}个地块界址调查信息", descZone, geoLandsCount, exportLandCount));
                }
            }
            catch (Exception ex)
            {
                this.ReportError("导出界址信息调查表失败!");
                YuLinTu.Library.Log.Log.WriteException(this, "ExportBoundaryInfoExcelByZone(导出界址信息到Excel表)", ex.Message + ex.StackTrace);
                returnValue = false;
            }
        }

        /// <summary>
        /// 导出界址信息调查表
        /// </summary>
        public bool ExportBoundaryInfoExcel(Zone zone, string fileName, List<ContractLand> listGeoLand, List<BuildLandBoundaryAddressDot> currentZoneDots,
            List<BuildLandBoundaryAddressCoil> currentZoneCoils, List<Dictionary> dictList, string excelName)
        //, double averagePercent = 0.0, double currentPercent = 0.0)
        {
            try
            {
                if (zone == null)
                {
                    this.ReportError("未选择导出数据的地域!");
                    return false;
                }
                //string excelName = GetMarkDesc(zone);
                string tempPath = TemplateHelper.ExcelTemplate(TemplateFile.BoundarySurveyExcel);
                //string uinitName = GetUinitName(zone);   //单位名称
                //if (metadata.SystemSet.CountryTableHead)
                //{
                //    var zoneStation = dbContext.CreateZoneWorkStation();
                //    uinitName = zoneStation.GetVillageName(zone);
                //}
                using (ExportBoundaryInfoExcel boundaryInfoExcel = new ExportBoundaryInfoExcel())
                {
                    string savePath= fileName + @"\" + excelName + TemplateFile.BoundarySurveyExcel + ".xls";
                    boundaryInfoExcel.SaveFilePath = savePath;
                    boundaryInfoExcel.CurrentZone = zone;
                    boundaryInfoExcel.TempletePath = tempPath;
                    boundaryInfoExcel.CurrentZoneLandList = listGeoLand;
                    boundaryInfoExcel.CurrentZoneDots = currentZoneDots;
                    boundaryInfoExcel.CurrentZoneCoils = currentZoneCoils;
                    if (currentZoneDots.Count > 65535) //超过了03版Excel的容积，存为07版本的
                    {
                        boundaryInfoExcel.SaveFilePath = fileName + @"\" + excelName + TemplateFile.BoundarySurveyExcel + ".xlsx";
                    }
                    boundaryInfoExcel.DictList = dictList;
                    //boundaryInfoExcel.Title = excelName;
                    boundaryInfoExcel.Title = SystemSetDefine.GetTableHeaderStr(zone);
                    boundaryInfoExcel.PostProgressEvent += export_PostProgressEvent;  //new ExportExcelBase.PostProgressDelegate(export_PostProgressEvent;
                    boundaryInfoExcel.PostErrorInfoEvent += export_PostErrorInfoEvent;
                    boundaryInfoExcel.PostExceptionInfoEvent += export_PostWarnInfoEvent;
                    returnValue = boundaryInfoExcel.BeginToZone(zone);   //开始导表
                    if (IsShow)
                        boundaryInfoExcel.PrintView(savePath);
                    exportLandCount = boundaryInfoExcel.ExportLandCount;
                    
                }
                return returnValue;
            }
            catch (Exception ex)
            {
                this.ReportError("导出界址信息调查表失败!");
                YuLinTu.Library.Log.Log.WriteException(this, "ExportBoundaryInfoExcel(导出界址信息到Excel表)", ex.Message + ex.StackTrace);
                return false;
            }
            finally
            {
                listGeoLand = null;
                currentZoneDots = null;
                currentZoneCoils = null;
            }
        }

        #endregion

        #region Methods - 辅助功能

        /// <summary>
        ///  获取上级地域
        /// </summary>
        /// <param name="zone"></param>
        /// <param name="dbContext"></param>
        /// <returns></returns>
        private Zone GetParent(Zone zone, IDbContext dbContext)
        {
            Zone parentZone = null;
            if (zone == null || dbContext == null)
                return parentZone;
            try
            {
                var zoneStation = dbContext.CreateZoneWorkStation();
                parentZone = zoneStation.Get(zone.UpLevelCode);
            }
            catch (Exception ex)
            {
                YuLinTu.Library.Log.Log.WriteException(this, "GetParent(获取父级地域失败!)", ex.Message + ex.StackTrace);
            }
            return parentZone;
        }

        /// <summary>
        /// 根据当前地域获得任务描述信息
        /// </summary>
        private string GetMarkDesc(Zone zone, IDbContext dbContext)
        {
            string excelName = string.Empty;
            if (zone == null || dbContext == null)
                return excelName;
            Zone parent = GetParent(zone, dbContext);  //获取上级地域
            string parentName = parent == null ? "" : parent.Name;
            if (zone.Level == eZoneLevel.County)
            {
                excelName = zone.Name;
            }
            else if (zone.Level == eZoneLevel.Town)
            {
                excelName = zone.Name;
            }
            else if (zone.Level == eZoneLevel.Village)
            {
                excelName = parentName + zone.Name;
            }
            else if (zone.Level == eZoneLevel.Group)
            {
                Zone parentTowm = GetParent(parent, dbContext);
                string parentTownName = parentTowm == null ? "" : parentTowm.Name;
                excelName = parentTownName + parentName + zone.Name;
            }
            return excelName;
        }

        /// <summary>
        /// 进度提示用，导出时获取当前地域的上级地域名称路径到镇级
        /// </summary>       
        //private string ExportZoneListDir(Zone zone, List<Zone> allZones)
        //{
        //    string exportzonedir = string.Empty;
        //    if (zone.Level == eZoneLevel.Group)
        //    {
        //        Zone vzone = allZones.Find(t => t.FullCode == zone.UpLevelCode);
        //        Zone tzone = allZones.Find(t => t.FullCode == vzone.UpLevelCode);
        //        exportzonedir = tzone.Name + vzone.Name + zone.Name;
        //    }
        //    else if (zone.Level == eZoneLevel.Village)
        //    {
        //        Zone tzone = allZones.Find(t => t.FullCode == zone.UpLevelCode);
        //        exportzonedir = tzone.Name + zone.Name;
        //    }
        //    else if (zone.Level == eZoneLevel.Town)
        //    {
        //        exportzonedir = zone.Name;
        //    }
        //    return exportzonedir;
        //}

        #endregion

        #region  Methods - 提示信息

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

        /// <summary>
        ///  错误信息报告
        /// </summary>
        private void export_PostErrorInfoEvent(string message)
        {
            if (!string.IsNullOrEmpty(message))
            {
                this.ReportError(message);
            }
        }

        /// <summary>
        /// 警告信息报告
        /// </summary>
        private void export_PostWarnInfoEvent(string message)
        {
            if (!string.IsNullOrEmpty(message))
            {
                this.ReportWarn(message);
            }
        }

        /// <summary>
        /// 报告进度
        /// </summary>
        private void export_PostProgressEvent(int progress, object userState)
        {
            this.ReportProgress(progress, userState);
        }

        #endregion
    }
}
