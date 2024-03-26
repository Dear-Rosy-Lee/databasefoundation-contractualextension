using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YuLinTu.Data;
using YuLinTu.Library.Entity;
using YuLinTu.Library.WorkStation;
using YuLinTu.Windows;

namespace YuLinTu.Library.Business
{
    public class TaskExportLandVerifyExcelOperation : Task
    {
        #region Property

        private bool returnValue;

        public bool IsGroup { get; set; }

        public SystemSetDefine SystemSet = SystemSetDefine.GetIntence();

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
        public TaskExportLandVerifyExcelOperation()
        {
        }

        #endregion Ctor

        #region Method—Override

        /// <summary>
        /// 开始执行任务
        /// </summary>
        protected override void OnGo()
        {
            TaskExportLandVerifyExcelArgument argument = Argument as TaskExportLandVerifyExcelArgument;
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
                var currentZone = argument.CurrentZone;
                string fileName = argument.FileName;
                string excelName = GetMarkDesc(argument.CurrentZone, argument.DbContext);
                var dicStation = argument.DbContext.CreateDictWorkStation();
                List<Dictionary> DictList = dicStation.Get();
                string tempPath = TemplateHelper.ExcelTemplate("农村土地二轮承包到期后再延长三十年摸底核实表");
                string zoneName = GetUinitName(currentZone);
                if (SystemSetDefine.CountryTableHead)
                {
                    var zoneStation = dbContext.CreateZoneWorkStation();
                    zoneName = zoneStation.GetVillageName(currentZone);
                }
                var landStation = dbContext.CreateContractLandWorkstation();
                List<ContractLand> landArrays = landStation.GetCollection(currentZone.FullCode);
                landArrays.LandNumberFormat(SystemSetDefine);
                var concordStation = dbContext.CreateConcordStation();
                var bookStation = dbContext.CreateRegeditBookStation();
                var listConcords = concordStation.GetContractsByZoneCode(currentZone.FullCode);
                var listBooks = bookStation.GetByZoneCode(currentZone.FullCode, eSearchOption.Precision);
                ExportLandVerifyExcelTable export = new ExportLandVerifyExcelTable();
                string savePath = argument.FileName + excelName + "农村土地二轮承包到期后再延长三十年摸底核实表" + ".xls";
                
                #region 通过反射等机制定制化具体的业务处理类

                var temp = WorksheetConfigHelper.GetInstance(export);
                if (temp != null && temp.TemplatePath != null)
                {
                    if (temp is ExportLandVerifyExcelTable)
                    {
                        export = (ExportLandVerifyExcelTable)temp;
                    }
                    tempPath = Path.Combine(TheApp.GetApplicationPath(), temp.TemplatePath);
                }

                #endregion 通过反射等机制定制化具体的业务处理类

                export.Familys = vps;
                export.ExcelName = SystemSet.GetTBDWStr(zone);
                export.UnitName = SystemSet.GetTableHeaderStr(zone);
                export.DictionList = DictList;
                export.LandArrays = landArrays;
                export.ConcordCollection = listConcords;
                export.BookColletion = listBooks;
                export.PostProgressEvent += export_PostProgressEvent;
                export.PostErrorInfoEvent += export_PostErrorInfoEvent;
                result = export.BeginExcel(zone.FullCode.ToString(), tempPath);
                if (argument.IsShow)
                    savePath = export.SaveFilePath;
                    export.PrintView(savePath);

                this.ReportProgress(100, "完成");
                this.ReportInfomation(string.Format("{0}导出{1}户调查信息数据", excelName, vps.Count));
            }
            catch (Exception ex)
            {
                this.ReportError(ex.Message);
                YuLinTu.Library.Log.Log.WriteException(this, "ExportDataExcel(导出数据到Excel表)", ex.Message + ex.StackTrace);
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

        public string GetUinitName(Zone zone)
        {
            ModuleMsgArgs arg = new ModuleMsgArgs();
            arg.Datasource = dbContext;
            arg.Parameter = zone;
            arg.Name = VirtualPersonMessage.VIRTUALPERSON_UNITNAME;
            TheBns.Current.Message.Send(this, arg);
            return arg.ReturnValue.ToString();
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