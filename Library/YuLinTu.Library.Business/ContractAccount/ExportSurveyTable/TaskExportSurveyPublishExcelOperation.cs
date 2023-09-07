/*
 * (C) 2015  鱼鳞图公司版权所有,保留所有权利 
 */
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using YuLinTu.Data;
using YuLinTu.Library.Entity;
using YuLinTu.Library.WorkStation;
using YuLinTu.Windows;

namespace YuLinTu.Library.Business
{
    /// <summary>
    /// 
    /// </summary>
    public class TaskExportSurveyPublishExcelOperation : Task
    {
        #region Ctor

        public bool IsGroup { get; set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        public TaskExportSurveyPublishExcelOperation()
        { }

        #endregion

        #region Field

        private string openFilePath;  //打开文件路径
        private SystemSetDefine SystemSetDefine = SystemSetDefine.GetIntence();
        private IDbContext dbContext;
        private Zone zone;
        #endregion

        #region Property

        #endregion

        #region Method—Override

        /// <summary>
        /// 开始执行任务
        /// </summary>
        protected override void OnGo()
        {
            TaskExportSurveyPublishExcelArgument argument = Argument as TaskExportSurveyPublishExcelArgument;
            if (argument == null)
            {
                return;
            }
            dbContext = argument.DbContext;
            zone = argument.CurrentZone;
            try
            {
                var personStation = dbContext.CreateVirtualPersonStation<LandVirtualPerson>();
                var landStation = dbContext.CreateContractLandWorkstation();
                var listPerson = personStation.GetByZoneCode(zone.FullCode, eLevelOption.Self);
                var listLand = landStation.GetCollection(zone.FullCode, eLevelOption.Self);
                if (listPerson == null || listPerson.Count == 0)
                {
                    this.ReportProgress(100, null);
                    this.ReportWarn(string.Format("{0}未获取承包方数据!", zone.FullName));
                    return;
                }
                if (listLand == null || listLand.Count == 0)
                {
                    this.ReportProgress(100, null);
                    this.ReportWarn(string.Format("{0}未获取承包地数据!", zone.FullName));
                    return;
                }
                listLand.LandNumberFormat(SystemSetDefine);  //根据系统设置进行地块编码的截取
                bool canOpen = ExportSurveyPublishExcel(argument, listPerson, listLand);
                if (canOpen)
                    CanOpenResult = true;
            }
            catch (Exception ex)
            {
                YuLinTu.Library.Log.Log.WriteException(this, "TaskExportSurveyPublishExcelOperation(导出调查信息公示表任务)", ex.Message + ex.StackTrace);
                this.ReportException(ex, "导出调查信息公示表出现异常!");
            }
        }

        /// <summary>
        /// 打开文件
        /// </summary>
        public override void OpenResult()
        {
            System.Diagnostics.Process.Start(openFilePath);
            base.OpenResult();
        }

        #endregion

        #region Method—ExportBusiness

        /// <summary>
        /// 导出调查信息公示表
        /// </summary>
        public bool ExportSurveyPublishExcel(TaskExportSurveyPublishExcelArgument argument, List<VirtualPerson> vps, List<ContractLand> lands)
        {
            bool result = false;
            try
            {
                if (argument.CurrentZone == null)
                {
                    this.ReportError("未选择导出数据的地域!");
                    return result;
                }
                List<ContractAccountLandFamily> accountFamilyCollection = new List<ContractAccountLandFamily>();
                //var stockLand = new AccountLandBusiness(dbContext).GetStockRightLand(zone);
                foreach (VirtualPerson vp in vps)
                {
                    var landCollection = lands == null ? new List<ContractLand>() : lands.FindAll(c => c.OwnerId == vp.ID);
                    ContractAccountLandFamily accountLandFamily = new ContractAccountLandFamily();
                    accountLandFamily.CurrentFamily = vp;
                    accountLandFamily.Persons = vp.SharePersonList;
                    //if(vp.IsStockFarmer)
                    //    landCollection.AddRange(stockLand);
                    accountLandFamily.LandCollection = landCollection;
                    accountFamilyCollection.Add(accountLandFamily);
                }
                string excelName = GetMarkDesc(argument.CurrentZone, argument.DbContext);
                string tempPath = TemplateHelper.ExcelTemplate(TemplateFile.LandInfomationExcel);
                var zoneStation = argument.DbContext.CreateZoneWorkStation();
                var tissueStation = argument.DbContext.CreateSenderWorkStation();
                string zoneName = zoneStation.GetZoneName(argument.CurrentZone);
                CollectivityTissue tissue = tissueStation.GetTissues(argument.CurrentZone.FullCode).FirstOrDefault();
                if (tissue != null)
                    zoneName = tissue.Name;
                string villageName = zoneStation.GetVillageName(argument.CurrentZone);
                openFilePath = argument.FileName;
                int personCount = vps == null ? 0 : vps.Count;
                ExportLandInfomationExcelTable export = new ExportLandInfomationExcelTable();
                string savePath = openFilePath + @"\" + excelName + TemplateFile.LandInfomationExcel + ".xls";

                #region 通过反射等机制定制化具体的业务处理类
                var temp = WorksheetConfigHelper.GetInstance(export);
                if (temp != null && temp.TemplatePath != null)
                {
                    if (temp is ExportLandInfomationExcelTable)
                    {
                        export = (ExportLandInfomationExcelTable)temp;
                    }
                    tempPath = Path.Combine(TheApp.GetApplicationPath(), temp.TemplatePath);
                }
                #endregion

                export.SaveFilePath = savePath;
                export.CurrentZone = argument.CurrentZone;
                export.TemplateFile = tempPath;
                export.AccountLandFamily = accountFamilyCollection;
                export.SenderName = zoneName;
                if (SystemSetDefine.ExportTableSenderDesToVillage)
                {
                    export.SenderName = GetVillageLevelDesc(argument.CurrentZone, argument.DbContext);
                    string zoneCode = argument.CurrentZone.FullCode.Substring(0, Zone.ZONE_VILLAGE_LENGTH);
                    tissue = tissueStation.GetTissues(zoneCode).FirstOrDefault();
                    if (tissue != null)
                        export.SenderName = tissue.Name;
                }
                export.SenderNameVillage = villageName;
                export.ZoneDesc = excelName;
                export.StartDate = argument.PublishDateSetting.PublishStartDate;
                export.EndDate = argument.PublishDateSetting.PublishEndDate;
                export.DrawPerson = argument.PublishDateSetting.CreateTablePerson;
                export.DrawDate = argument.PublishDateSetting.CreateTableDate;
                export.CheckPerson = argument.PublishDateSetting.CheckTablePerson;
                export.CheckDate = argument.PublishDateSetting.CheckTableDate;
                export.PostProgressEvent += export_PostProgressEvent;
                export.PostErrorInfoEvent += export_PostErrorEvent;
                result = export.BeginToZone(tempPath);
                export.SaveAs(export.SaveFilePath);
                if (argument.IsShow)
                    export.PrintView(savePath);
                if (result)
                {
                    openFilePath = export.SaveFilePath;
                    export_PostProgressEvent(100, null);
                    this.ReportInfomation(string.Format("{0}导出{1}户调查信息数据", excelName, personCount));
                }
                vps.Clear();
                vps = null;
                lands.Clear();
                lands = null;
            }
            catch (Exception ex)
            {
                result = false;
                this.ReportError("导出调查信息公示表失败");
                YuLinTu.Library.Log.Log.WriteException(this, "ExportVirtualPersonExcel(导出数据到调查信息公示表)", ex.Message + ex.StackTrace);
            }
            return result;
        }




        #endregion

        #region Method—Private


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
        /// 查找，到村的描述
        /// </summary>
        /// <param name="zone"></param>
        /// <param name="Database"></param>
        /// <returns></returns>
        private string GetVillageLevelDesc(Zone zone, IDbContext Database)
        {
            Zone parent = GetParent(zone, Database);  //获取上级地域
            string excelName = string.Empty;
            if (zone.Level == eZoneLevel.Town)
            {
                excelName = zone.Name;
            }
            else if (zone.Level == eZoneLevel.Village)
            {
                excelName = parent.Name + zone.Name;
            }
            else if (zone.Level == eZoneLevel.Group)
            {
                Zone parentTowm = GetParent(parent, Database);
                excelName = parentTowm.Name + parent.Name;
            }
            return excelName;
        }






        #endregion

        #region Method—Helper

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
        private void ReportPercent(object sender, TaskProgressChangedEventArgs e)
        {
            if (e != null)
            {
                this.ReportProgress(e.Percent, e.UserState);
            }
        }

        /// <summary>
        ///  进度报告
        /// </summary>
        private void export_PostProgressEvent(int progress, object userState)
        {
            this.ReportProgress(progress, userState);
        }

        /// <summary>
        /// 信息报告
        /// </summary>
        private void export_PostErrorEvent(string error)
        {
            this.ReportError(error);
        }

        #endregion
    }
}
