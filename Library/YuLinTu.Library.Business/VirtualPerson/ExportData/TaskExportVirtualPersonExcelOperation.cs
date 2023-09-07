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
    /// 导出承包方调查表任务类
    /// </summary>
    public class TaskExportVirtualPersonExcelOperation : Task
    {
        #region Ctor

        /// <summary>
        /// 构造函数
        /// </summary>
        public TaskExportVirtualPersonExcelOperation()
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
            TaskExportVirtualPersonExcelArgument argument = Argument as TaskExportVirtualPersonExcelArgument;
            if (argument == null)
            {
                return;
            }
            IDbContext dbContext = argument.DbContext;
            var zone = argument.CurrentZone;
            openFilePath = argument.FileName;
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
                listPerson.Clear();
                listPerson = null;
                tablePersons.Clear();
                tablePersons = null;
            }
            catch (Exception ex)
            {
                YuLinTu.Library.Log.Log.WriteException(this, "TaskExportVirtualPersonExcelOperation(导出承包方数据任务)", ex.Message + ex.StackTrace);
                this.ReportException(ex, "导出承包方调查表出现异常!");
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
        /// 导出数据到Excel表
        /// </summary>
        public bool ExportVirtualPersonExcel(TaskExportVirtualPersonExcelArgument argument, List<VirtualPerson> listPerson, List<VirtualPerson> tablePersons, List<ContractLand> landList)
        {
            bool result = false;
            try
            {
                if (argument.CurrentZone == null)
                {
                    this.ReportError("未选择导出数据的地域!");
                    return result;
                }
                int personCount = listPerson == null ? 0 : listPerson.Count;
                string excelName = GetMarkDesc(argument.CurrentZone, argument.DbContext);
                string tempPath = TemplateHelper.ExcelTemplate(TemplateFile.VirtualPersonSurveyExcel);
                var zoneStation = argument.DbContext.CreateZoneWorkStation();
                string zoneName = zoneStation.GetZoneName(argument.CurrentZone);
                string villageName = zoneStation.GetVillageName(argument.CurrentZone);
                openFilePath = argument.FileName;
                if (listPerson == null)
                    listPerson = new List<VirtualPerson>();
                var orderByPersons = listPerson.OrderBy(c => c.FamilyNumber.PadLeft(4, '0'));
                using (ExportContractorExcel export = new ExportContractorExcel())
                {
                    string saveFilePath= argument.FileName + @"\" + excelName + TemplateFile.VirtualPersonSurveyExcel + ".xls";
                    export.SaveFilePath = saveFilePath;
                    export.CurrentZone = argument.CurrentZone;
                    export.FamilyList = orderByPersons == null ? new List<VirtualPerson>() : orderByPersons.ToList();
                    export.TableFamilyList = tablePersons == null ? new List<VirtualPerson>() : tablePersons;
                    export.LandList = landList;
                    //export.UnitName = zoneName;
                    export.UnitName = argument.SystemSet.GetTableHeaderStr(argument.CurrentZone);
                    //export.UnitVillageName = villageName;
                    export.UnitVillageName = argument.SystemSet.GetTBDWStr(argument.CurrentZone);
                    export.ZoneDesc = excelName;
                    export.PersonType = argument.VirtualType;
                    export.PostProgressEvent += export_PostProgressEvent;
                    export.PostErrorInfoEvent += export_PostErrorEvent;                    
                    result = export.BeginExcel(tempPath);
                    if (!argument.hasChildren)
                        export.PrintView(saveFilePath);
                    if (result)
                    {
                        export_PostProgressEvent(100, null);
                        this.ReportInfomation(string.Format("{0}导出{1}户承包方数据", argument.CurrentZone.FullName, personCount));
                    }                   
                }
            }
            catch (Exception ex)
            {
                result = false;
                this.ReportError("导出承包方调查表失败");
                YuLinTu.Library.Log.Log.WriteException(this, "ExportVirtualPersonExcel(导出承包方数据到Excel表)", ex.Message + ex.StackTrace);
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
