using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using YuLinTu.Data;
using YuLinTu.Library.Entity;
using YuLinTu.Library.WorkStation;
using YuLinTu.Windows;

namespace YuLinTu.Library.Business
{
    public class TaskExportVerifyExcel : Task
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
        public TaskExportVerifyExcel()
        {
        }

        #endregion Ctor

        #region Method—Override

        /// <summary>
        /// 开始执行任务
        /// </summary>
        protected override void OnGo()
        {
            var argument = Argument as TaskExportLandVerifyExcelArgument;
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
                YuLinTu.Library.Log.Log.WriteException(this, "TaskExportSurveyPublishExcelOperation(导出摸底核实表任务)", ex.Message + ex.StackTrace);
                this.ReportException(ex, "导出摸底核实表出现异常!");
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
        public bool ExportLandVerifyExcel(TaskExportLandVerifyExcelArgument argument, List<VirtualPerson> vps, List<ContractLand> lands)
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
                var stockLand = new AccountLandBusiness(dbContext).GetStockRightLand(zone);
                var qglands = argument.DbContext.CreateVirtualPersonStation<LandVirtualPerson>().GetRelationByZone(zone.FullCode, eLevelOption.Self);//确股的地块
                var delLands = argument.DbContext.CreateContractLandWorkstation().GetDelLandByZone(zone.FullCode);

                foreach (VirtualPerson vp in vps)
                {
                    var landCollection = lands == null ? new List<ContractLand>() : lands.FindAll(c => c.OwnerId == vp.ID);
                    var landDelCollection = delLands == null ? new List<ContractLand_Del>() : delLands.FindAll(c => c.CBFID == vp.ID);
                    ContractAccountLandFamily accountLandFamily = new ContractAccountLandFamily();
                    accountLandFamily.CurrentFamily = vp;
                    accountLandFamily.Persons = vp.SharePersonList;
                    accountLandFamily.LandCollection = landCollection;
                    accountLandFamily.LandDelCollection = landDelCollection;
                    accountFamilyCollection.Add(accountLandFamily);
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
                }
                string excelName = GetMarkDesc(argument.CurrentZone, argument.DbContext);
                string tempPath = TemplateHelper.ExcelTemplate("农村土地二轮承包到期后再延长三十年摸底核实表");
                var zoneStation = argument.DbContext.CreateZoneWorkStation();
                var tissueStation = argument.DbContext.CreateSenderWorkStation();
                string zoneName = zoneStation.GetZoneName(argument.CurrentZone);
                CollectivityTissue tissue = tissueStation.GetByCode(zone.FullCode);
                if (tissue == null)
                {
                    var tis = tissueStation.GetTissues(zone.FullCode);
                    tissue = tis.FirstOrDefault(t => t.Code.Equals(zone.FullCode.PadRight(14, '0')));
                }
                if (tissue == null)
                {
                    this.ReportError($"未获取到{zone.FullName}下的发包方数据！");
                    return false;
                }
                if (string.IsNullOrEmpty(tissue.LawyerName) || string.IsNullOrEmpty(tissue.LawyerAddress) ||
                    string.IsNullOrEmpty(tissue.LawyerTelephone) || string.IsNullOrEmpty(tissue.LawyerCartNumber))
                {
                    this.ReportError($"未获取到{zone.FullName}下的发包方数据不完整：代表姓名({tissue.LawyerName} )、代表证件号({tissue.LawyerCartNumber} )、地址({tissue.LawyerAddress} )、电话({tissue.LawyerTelephone} )");
                    return false;
                }
                if (tissue != null)
                    zoneName = tissue.Name;
                GetDelDataToExport(accountFamilyCollection, argument.CurrentZone.FullCode);
                openFilePath = argument.FileName;
                int personCount = vps == null ? 0 : vps.Count;
                var export = new ExportRelationLandVerifyExcel();
                IConcordWorkStation ConcordStation = argument.DbContext.CreateConcordStation();
                string savePath = openFilePath + @"\" + excelName + "摸底核实表" + ".xls";

                export.SaveFilePath = savePath;
                export.CurrentZone = argument.CurrentZone;
                export.DbContext = argument.DbContext;
                export.TemplateFile = tempPath;
                export.AccountLandFamily = accountFamilyCollection;
                export.Tissue = tissue;
                export.ZoneDesc = excelName;
                export.PostProgressEvent += export_PostProgressEvent;
                export.PostErrorInfoEvent += export_PostErrorEvent;
                result = export.BeginToZone(tempPath);
                export.SaveAs(export.SaveFilePath);
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
                this.ReportError("导出摸底核实表失败");
                YuLinTu.Library.Log.Log.WriteException(this, "ExportVirtualPersonExcel(导出数据到摸底核实表)", ex.Message + ex.StackTrace);
            }
            return result;
        }

        /// <summary>
        /// 获取删除数据进行导出
        /// </summary>
        private void GetDelDataToExport(List<ContractAccountLandFamily> accountFamilyCollection, string zonecode)
        {
            var query = dbContext.CreateQuery<VirtualPerson_Del>();
            var delvps = query.Where(t => t.ZoneCode == zonecode).ToList();

            var dquery = dbContext.CreateQuery<ContractLand_Del>();
            var dellds = dquery.Where(t => t.DYBM == zonecode).ToList();

            foreach (var item in delvps)
            {
                var vp = item.ConvertTo<VirtualPerson>();
                vp.Status = eVirtualPersonStatus.Bad;
                var landDelCollection = dellds.FindAll(c => c.CBFID == vp.ID);
                ContractAccountLandFamily accountLandFamily = new ContractAccountLandFamily();
                accountLandFamily.CurrentFamily = vp;
                accountLandFamily.Persons = vp.SharePersonList;
                accountLandFamily.LandDelCollection = landDelCollection;
                accountFamilyCollection.Add(accountLandFamily);
            }
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