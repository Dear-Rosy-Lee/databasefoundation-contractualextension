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
    /// 导出颁证清册任务类
    /// </summary>
    public class TaskExportAwareTableOperation : Task
    {
        #region Ctor

        /// <summary>
        /// 构造函数
        /// </summary>
        public TaskExportAwareTableOperation()
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
            TaskExportAwareTableArgument argument = Argument as TaskExportAwareTableArgument;
            if (argument == null)
            {
                return;
            }
            var dbContext = argument.DbContext;
            var zone = argument.CurrentZone;
            try
            {
                var personStation = dbContext.CreateVirtualPersonStation<LandVirtualPerson>();
                var concordStation = dbContext.CreateConcordStation();
                var bookStation = dbContext.CreateRegeditBookStation();
                var listPerson = personStation.GetByZoneCode(zone.FullCode, eLevelOption.Self);
                var listConcord = concordStation.GetByZoneCode(zone.FullCode);
                var listBook = bookStation.GetByZoneCode(zone.FullCode, eSearchOption.Fuzzy);
                if (listPerson == null || listPerson.Count == 0)
                {
                    this.ReportProgress(100, null);
                    this.ReportWarn(string.Format("{0}未获取承包方数据!", zone.FullName));
                    return;
                }
                if (listConcord == null || listConcord.Count == 0)
                {
                    this.ReportProgress(100, null);
                    this.ReportWarn(string.Format("{0}未获取合同数据!", zone.FullName));
                    return;
                }
                if(listBook==null || listBook.Count==0)
                {
                    this.ReportProgress(100, null);
                    this.ReportWarn(string.Format("{0}未获取权证数据!", zone.FullName));
                    return;
                }
                bool canOpen = ExportAwareTable(argument, listPerson, listConcord);
                if (canOpen)
                    CanOpenResult = true;
            }
            catch (Exception ex)
            {
                YuLinTu.Library.Log.Log.WriteException(this, "TaskExportAwareTableOperation(导出办证清册任务)", ex.Message + ex.StackTrace);
                this.ReportException(ex, "导出颁证清册出现异常!");
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
        /// 导出颁证清册
        /// </summary>
        public bool ExportAwareTable(TaskExportAwareTableArgument argument, List<VirtualPerson> listPerson, List<ContractConcord> listConcord)
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
                string tempPath = TemplateHelper.ExcelTemplate(TemplateFile.AwareInventoryTable);  //获取模板
                openFilePath = argument.FileName;
                using (ExportAwareInventoryTable awareExport = new ExportAwareInventoryTable(argument.DbContext))
                {
                    string savePath= openFilePath + @"\" + excelName + "颁证清册" + ".xls";
                    awareExport.SaveFilePath = savePath; 
                    awareExport.CurrentZone = argument.CurrentZone;
                    awareExport.ListPerson = listPerson == null ? new List<VirtualPerson>() : listPerson;
                    awareExport.ListConcord = listConcord == null ? new List<ContractConcord>() : listConcord;
                    awareExport.StatuDes = excelName + "颁证清册";
                    awareExport.Percent = 99;
                    awareExport.CurrentPercent = 1;
                    awareExport.ZoneDesc = excelName;
                    awareExport.UnitName = argument.UnitName;
                    awareExport.PostProgressEvent += export_PostProgressEvent;
                    awareExport.PostErrorInfoEvent += export_PostErrorEvent;
                    awareExport.BeginToZone(tempPath);   //开始导表
                    if (argument.IsShow)
                        awareExport.PrintView(savePath);
                }
                export_PostProgressEvent(100, null);
                this.ReportInfomation(string.Format("{0}导出{1}户承包方数据", excelName, personCount));
                result = true;

                listPerson.Clear();
                listPerson = null;
                listConcord.Clear();
                listConcord = null;
                GC.Collect();
            }
            catch (Exception ex)
            {
                result = false;
                this.ReportError("导出颁证清册失败!");
                YuLinTu.Library.Log.Log.WriteException(this, "ExportAwareTable(导出颁证清册到Excel)", ex.Message + ex.StackTrace);
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
