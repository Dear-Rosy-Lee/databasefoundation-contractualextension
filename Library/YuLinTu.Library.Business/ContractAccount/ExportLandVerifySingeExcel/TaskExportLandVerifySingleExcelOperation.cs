using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using YuLinTu.Data;
using YuLinTu.Library.Entity;
using YuLinTu.Library.WorkStation;

namespace YuLinTu.Library.Business
{
    public class TaskExportLandVerifySingleExcelOperation : Task
    {
        #region Property

        public bool IsGroup { get; set; }

        #endregion Property

        #region Field

        private string openFilePath;  //打开文件路径
        private SystemSetDefine SystemSetDefine = SystemSetDefine.GetIntence();
        private IDbContext dbContext;
        private Zone zone;

        #endregion Field

        #region Ctor

        /// <summary>
        /// 构造函数
        /// </summary>
        public TaskExportLandVerifySingleExcelOperation()
        {
        }

        #endregion Ctor

        #region Method—Override

        /// <summary>
        /// 开始执行任务
        /// </summary>
        protected override void OnGo()
        {
            var argument = Argument as TaskExportLandVerifySingleExcelArgument;
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
                bool canOpen = ExportLandVerifyExcel(argument, listPerson, listLand);
                if (canOpen)
                    CanOpenResult = true;
            }
            catch (Exception ex)
            {
                YuLinTu.Library.Log.Log.WriteException(this, "TaskExportSurveyPublishExcelOperation(导出单户摸底核实表任务)", ex.Message + ex.StackTrace);
                this.ReportException(ex, "导出单户摸底核实表出现异常!");
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

        #endregion Method—Override

        #region Method—ExportBusiness

        /// <summary>
        /// 导出调查信息公示表
        /// </summary>
        public bool ExportLandVerifyExcel(TaskExportLandVerifySingleExcelArgument argument, List<VirtualPerson> vps, List<ContractLand> lands)
        {
            bool result = false;
            try
            {
                if (argument.CurrentZone == null)
                {
                    this.ReportError("未选择导出数据的地域!");
                    return result;
                }

                string tempPath = TemplateHelper.ExcelTemplate("土地延包农户基本情况摸底调查表");
                var zoneStation = argument.DbContext.CreateZoneWorkStation();
                var tissueStation = argument.DbContext.CreateSenderWorkStation();
                string zoneName = zoneStation.GetZoneName(argument.CurrentZone);
                CollectivityTissue tissue = tissueStation.Get(zone.ID);
                if (tissue == null)
                {
                    var tis = tissueStation.GetTissues(zone.FullCode);
                    tissue = tis.FirstOrDefault(t => t.Code.Equals(zone.FullCode.PadRight(14, '0')));
                }
                if (tissue != null)
                    zoneName = tissue.Name;

                openFilePath = argument.FileName;
                int personCount = vps == null ? 0 : vps.Count;
                IConcordWorkStation ConcordStation = argument.DbContext.CreateConcordStation();

                var stockLand = new AccountLandBusiness(dbContext).GetStockRightLand(zone);
                var qglands = argument.DbContext.CreateVirtualPersonStation<LandVirtualPerson>().GetRelationByZone(zone.FullCode, eLevelOption.Self);//确股的地块
                foreach (VirtualPerson vp in vps)
                {
                    var landCollection = lands == null ? new List<ContractLand>() : lands.FindAll(c => c.OwnerId == vp.ID);
                    var accountLandFamily = new ContractAccountLandFamily();
                    accountLandFamily.CurrentFamily = vp;
                    accountLandFamily.Persons = vp.SharePersonList;
                    accountLandFamily.LandCollection = landCollection;
                    var ralations = qglands.FindAll(o => o.VirtualPersonID == vp.ID);
                    if (ralations.Count > 0)
                    {
                        foreach (var r in ralations)
                        {
                            var stockland = stockLand.Find(t => t.ID == r.LandID).Clone() as ContractLand;
                            if (stockland == null)
                                continue;
                            stockland.AwareArea = r.QuanficationArea;
                            accountLandFamily.LandCollection.Add(stockland);
                        }
                    }
                    var export = new ExportLandVerifySingleExcelTable();
                    export.SaveFilePath = Path.Combine(openFilePath, $"{vp.FamilyNumber}-{vp.Name}-单户摸底调查表.xls");
                    export.CurrentZone = argument.CurrentZone;
                    export.DbContext = argument.DbContext;
                    export.TemplateFile = tempPath;
                    export.AccountLandFamily = accountLandFamily;
                    export.Tissue = tissue;
                    export.PostProgressEvent += export_PostProgressEvent;
                    export.PostErrorInfoEvent += export_PostErrorEvent;
                    result = export.BeginToZone(tempPath);
                    export.SaveAs(export.SaveFilePath);
                }

                if (result)
                {
                    export_PostProgressEvent(100, null);
                }
                vps.Clear();
                vps = null;
                lands.Clear();
                lands = null;
            }
            catch (Exception ex)
            {
                result = false;
                this.ReportError("导出摸底核实表失败");
                YuLinTu.Library.Log.Log.WriteException(this, "ExportVirtualPersonExcel(导出数据到摸底核实表)", ex.Message + ex.StackTrace);
            }
            return result;
        }

        #endregion Method—ExportBusiness

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

        #endregion Method—Private

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

        #endregion Method—Helper
    }
}