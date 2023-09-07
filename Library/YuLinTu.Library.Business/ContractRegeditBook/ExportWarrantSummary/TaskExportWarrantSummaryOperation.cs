/*
 * (C) 2015  鱼鳞图公司版权所有,保留所有权利 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YuLinTu.Data;
using YuLinTu.Library.Entity;

namespace YuLinTu.Library.Business
{
    /// <summary>
    /// 导出权证数据汇总表任务类
    /// </summary>
    public class TaskExportWarrantSummaryOperation : Task
    {
        #region Ctor


        /// <summary>
        /// 构造函数
        /// </summary>
        public TaskExportWarrantSummaryOperation()
        { }

        #endregion

        #region Field

        private string openFilePath;  //打开文件路径

        #endregion

        #region Property

        #endregion

        #region Method—Override

        /// <summary>
        /// 开始执行任务
        /// </summary>
        protected override void OnGo()
        {
            TaskExportWarrantSummaryArgument argument = Argument as TaskExportWarrantSummaryArgument;
            if (argument == null)
            {
                return;
            }
            var dbContext = argument.DbContext;
            var zone = argument.CurrentZone;
            try
            {
                var personStation = dbContext.CreateVirtualPersonStation<LandVirtualPerson>();
                var landStation = dbContext.CreateContractLandWorkstation();
                var concordStation = dbContext.CreateConcordStation();
                var bookStation = dbContext.CreateRegeditBookStation();
                var listPerson = personStation.GetByZoneCode(zone.FullCode, eLevelOption.Self);
                var listLand = landStation.GetCollection(zone.FullCode, eLevelOption.Self);
                var listConcord = concordStation.GetByZoneCode(zone.FullCode);
                var listBook = bookStation.GetByZoneCode(zone.FullCode, eSearchOption.Precision);
                if (listPerson == null || listPerson.Count == 0)
                {
                    this.ReportProgress(100, null);
                    this.ReportWarn(string.Format("{0}未获取承包方数据!", zone.FullName));
                    return;
                }
                if (listBook == null || listBook.Count == 0)
                {
                    this.ReportProgress(100, null);
                    this.ReportWarn(string.Format("{0}未获取权证数据!", zone.FullName));
                    return;
                }
                bool canOpen = ExportWarrantSummaryExcel(argument, listPerson, listLand, listConcord, listBook);
                if (canOpen)
                    CanOpenResult = true;
            }
            catch (Exception ex)
            {
                YuLinTu.Library.Log.Log.WriteException(this, "TaskExportWarrantSummaryOperation(导出数据汇总表任务)", ex.Message + ex.StackTrace);
                this.ReportException(ex, "导出数据汇总表出现异常!");
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
        /// 导出权证数据汇总表
        /// </summary>
        public bool ExportWarrantSummaryExcel(TaskExportWarrantSummaryArgument argument, List<VirtualPerson> listPerson, List<ContractLand> listLand,
            List<ContractConcord> listConcord, List<ContractRegeditBook> listBook)
        {
            bool result = false;
            try
            {
                if (argument.CurrentZone == null)
                {
                    this.ReportError("未选择导出数据的地域!");
                    return result;
                }
                export_PostProgressEvent(1, "开始");
                int personCount = listPerson.Count;
                string excelName = GetMarkDesc(argument.CurrentZone, argument.DbContext);
                string tempPath = TemplateHelper.ExcelTemplate(TemplateFile.ContractSummaryExcel);  //获取模板
                var zoneStation = argument.DbContext.CreateZoneWorkStation();
                string unitName = zoneStation.GetZoneName(argument.CurrentZone);
                string villageName = zoneStation.GetVillageName(argument.CurrentZone);
                openFilePath = argument.FileName;
                using (ExportContractorSummaryExcel warrentSummaryExport = new ExportContractorSummaryExcel(argument.DbContext))
                {
                    string savePath= openFilePath + @"\" + excelName + "权证数据汇总表" + ".xls";
                    warrentSummaryExport.SaveFilePath = savePath;
                    warrentSummaryExport.CurrentZone = argument.CurrentZone;
                    warrentSummaryExport.ListPerson = listPerson == null ? new List<VirtualPerson>() : listPerson;
                    warrentSummaryExport.ListConcord = listConcord == null ? new List<ContractConcord>() : listConcord;
                    warrentSummaryExport.ListLand = listLand == null ? new List<ContractLand>() : listLand;
                    warrentSummaryExport.ListBook = listBook == null ? new List<ContractRegeditBook>() : listBook;
                    warrentSummaryExport.UnitName = argument.SystemSet.CountryTableHead ? villageName : unitName;
                    warrentSummaryExport.UnitName = argument.SystemSet.GetTableHeaderStr(argument.CurrentZone);
                    warrentSummaryExport.StatuDes = excelName + "权证数据汇总表";
                    warrentSummaryExport.Percent = 99;
                    warrentSummaryExport.CurrentPercent = 1;
                    warrentSummaryExport.ZoneDesc = excelName;
                    warrentSummaryExport.ArgType = eContractAccountType.ExportWarrentSummaryTable;
                    //warrentSummaryExport.SettingDefine = null;
                    warrentSummaryExport.PostProgressEvent += export_PostProgressEvent;
                    warrentSummaryExport.PostErrorInfoEvent += export_PostErrorEvent;
                    result = warrentSummaryExport.BeginExcel(tempPath);   //开始导表
                    if (result)
                    {
                        if (argument.IsShow)
                            warrentSummaryExport.PrintView(savePath);
                        export_PostProgressEvent(100, null);
                        this.ReportInfomation(string.Format("{0}导出{1}户承包方数据", excelName, personCount));
                    }
                }

                listPerson.Clear();
                listPerson = null;
                listLand.Clear();
                listLand = null;
                listConcord.Clear();
                listConcord = null;
                listBook.Clear();
                listBook = null;
                GC.Collect();
            }
            catch (Exception ex)
            {
                this.ReportError("导出权证数据汇总表失败!");
                YuLinTu.Library.Log.Log.WriteException(this, "ExportWarrentSummaryTable(导出权证数据汇总表到Excel)", ex.Message + ex.StackTrace);
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
