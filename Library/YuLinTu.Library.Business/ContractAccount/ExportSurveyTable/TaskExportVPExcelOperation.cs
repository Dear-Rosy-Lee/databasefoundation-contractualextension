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
    /// 导出承包方excel调查表任务类
    /// </summary>
    public class TaskExportVPExcelOperation : Task
    {
        #region Ctor

        /// <summary>
        /// 构造函数
        /// </summary>
        public TaskExportVPExcelOperation()
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
            TaskExportVPExcelArgument argument = Argument as TaskExportVPExcelArgument;
            if (argument == null)
            {
                return;
            }
            var dbContext = argument.DbContext;
            var zone = argument.CurrentZone;
            try
            {
                var personStation = dbContext.CreateVirtualPersonStation<LandVirtualPerson>();
                var tableStation = dbContext.CreateVirtualPersonStation<TableVirtualPerson>();
                var landStation = dbContext.CreateContractLandWorkstation();
                var listPerson = personStation.GetByZoneCode(zone.FullCode, eLevelOption.Self);
                var tablePersons = tableStation.GetByZoneCode(zone.FullCode, eLevelOption.Self);
                if (listPerson == null || listPerson.Count == 0)
                {
                    this.ReportProgress(100, null);
                    this.ReportWarn(string.Format("{0}未获取承包方数据!", zone.FullName));
                    return;
                }
                var ownerIds = new List<Guid>();
                listPerson.ForEach(c => ownerIds.Add(c.ID));
                var listLand = landStation.GetCollection(ownerIds);
                bool canOpen = ExportVirtualPersonExcel(argument, listPerson, tablePersons, listLand);
                if (canOpen)
                    CanOpenResult = true;
            }
            catch (Exception ex)
            {
                YuLinTu.Library.Log.Log.WriteException(this, "TaskExportVPExcelOperation(导出承包方excel调查表任务)", ex.Message + ex.StackTrace);
                this.ReportException(ex, "导出承包方excel调查表出现异常!");
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
        /// 导出承包方Excel调查表
        /// </summary>
        public bool ExportVirtualPersonExcel(TaskExportVPExcelArgument argument, List<VirtualPerson> vps, List<VirtualPerson> tbVps, List<ContractLand> landList)
        {
            bool result = false;
            try
            {
                if (argument.CurrentZone == null)
                {
                    this.ReportError("未选择导出数据的地域!");
                    return result;
                }
                int personCount = vps == null ? 0 : vps.Count;
                if (personCount > 0)
                {
                    vps.Sort((a, b) =>
                    {
                        long aNumber = Convert.ToInt64(a.FamilyNumber);
                        long bNumber = Convert.ToInt64(b.FamilyNumber);
                        return aNumber.CompareTo(bNumber);
                    });
                }
                string excelName = GetMarkDesc(argument.CurrentZone, argument.DbContext);
                string tempPath = TemplateHelper.ExcelTemplate(TemplateFile.VirtualPersonSurveyExcel);
                var zoneStation = argument.DbContext.CreateZoneWorkStation();
                string zoneName = zoneStation.GetZoneName(argument.CurrentZone);
                string villageName = zoneStation.GetVillageName(argument.CurrentZone);
                openFilePath = argument.FileName;
                using (ExportContractorExcel export = new ExportContractorExcel())
                {
                    string savePath = openFilePath + @"\" + excelName + TemplateFile.VirtualPersonSurveyExcel + ".xls";
                    export.SaveFilePath = savePath;
                    export.PersonType = argument.VirtualType;
                    export.CurrentZone = argument.CurrentZone;
                    export.FamilyList = vps == null ? new List<VirtualPerson>() : vps;
                    export.TableFamilyList = tbVps == null ? new List<VirtualPerson>() : tbVps;
                    export.LandList = landList;
                    //export.UnitName = zoneName;
                    //export.UnitVillageName = villageName;
                    export.UnitName = argument.SystemSet.GetTableHeaderStr(argument.CurrentZone);
                    export.UnitVillageName = argument.SystemSet.GetTBDWStr(argument.CurrentZone);
                    export.ZoneDesc = excelName;
                    export.PostProgressEvent += export_PostProgressEvent;
                    export.PostErrorInfoEvent += export_PostErrorEvent;
                    result = export.BeginExcel(tempPath);
                    if (result)
                    {
                        if (argument.IsShow)
                            export.PrintView(savePath);
                        export_PostProgressEvent(100, null);
                        this.ReportInfomation(string.Format("{0}导出{1}户承包方数据", excelName, personCount));
                    }
                }
                vps = null;
                tbVps = null;
            }
            catch (Exception ex)
            {
                result = false;
                this.ReportError("导出承包方Excel调查表失败");
                YuLinTu.Library.Log.Log.WriteException(this, "ExportVirtualPersonExcel(导出数据到承包方Excel表)", ex.Message + ex.StackTrace);
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
