using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using YuLinTu.Data;
using YuLinTu.Library.Business;
using YuLinTu.Library.Entity;
using YuLinTu.Library.WorkStation;
using YuLinTu.Windows;

namespace YuLinTu.Component.FuSui
{
    public class TaskExportContractInformationOperationFusui : Task
    {
        public TaskExportContractInformationOperationFusui()
        {
        }

        #region Field

        private string openFilePath;  //打开文件路径
        private SystemSetDefine SystemSetDefine = SystemSetDefine.GetIntence();
        private Zone currentZone;
        private IDbContext dbContext;
        private bool returnValue;

        #endregion Field

        protected override void OnGo()
        {
            try
            {
                TaskAccountFiveTableArgument metadata = Argument as TaskAccountFiveTableArgument;
                currentZone = metadata.CurrentZone;
                dbContext = metadata.Database;
                openFilePath = metadata.FileName;
                string fileName = openFilePath;
                if (currentZone == null)
                {
                    this.ReportError("未选择导出数据的地域!");
                    return;
                }
                string excelName = GetMarkDesc(currentZone);
                var vpStation = metadata.Database.CreateVirtualPersonStation<LandVirtualPerson>();
                List<VirtualPerson> vps = vpStation.GetByZoneCode(currentZone.FullCode);

                string tempPath = TemplateHelper.ExcelTemplate(TemplateFile.ContractInformationExcelFuSui);
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
                string filePath = string.Empty;
                ExportContractInformationExcelFuSui export = new ExportContractInformationExcelFuSui(dbContext, openFilePath);

                #region 通过反射等机制定制化具体的业务处理类

                var temp = WorksheetConfigHelper.GetInstance(export);
                if (temp != null && temp.TemplatePath != null)
                {
                    if (temp is ExportContractInformationExcelFuSui)
                    {
                        export = (ExportContractInformationExcelFuSui)temp;
                    }
                    tempPath = Path.Combine(TheApp.GetApplicationPath(), temp.TemplatePath);
                }

                #endregion 通过反射等机制定制化具体的业务处理类

                double percent = 95 / (double)metadata.SelfAndSubsZones.Count;
                string savePath1 = fileName;
                export.SaveFilePath = openFilePath;
                export.CurrentZone = currentZone;
                export.Familys = vps;
                export.ExcelName = SystemSetDefine.GetTBDWStr(currentZone);
                export.UnitName = SystemSetDefine.GetTableHeaderStr(currentZone);
                export.TableType = metadata.TableType;
                export.DictionList = metadata.DictList;
                export.LandArrays = landArrays;
                export.Percent = percent;
                export.CurrentPercent = percent;
                export.ConcordCollection = listConcords;
                export.BookColletion = listBooks;
                export.PostProgressEvent += export_PostProgressEvent;
                export.PostErrorInfoEvent += export_PostErrorInfoEvent;
                export.BeginExcel(currentZone.FullCode.ToString(), tempPath);

                this.ReportProgress(100, "完成");
            }
            catch (Exception ex)
            {
                this.ReportError(ex.Message);
                YuLinTu.Library.Log.Log.WriteException(this, "ExportDataExcel(导出数据到Excel表)", ex.Message + ex.StackTrace);
            }
        }

        public override void OpenResult()
        {
            System.Diagnostics.Process.Start(openFilePath);
            base.OpenResult();
        }

        private string GetMarkDesc(Zone zone)
        {
            Zone parent = GetParent(zone);  //获取上级地域
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
                Zone parentTowm = GetParent(parent);
                excelName = parentTowm.Name + parent.Name + zone.Name;
            }
            return excelName;
        }

        public Zone GetParent(Zone zone)
        {
            ModuleMsgArgs arg = new ModuleMsgArgs();
            arg.Datasource = dbContext;
            arg.Parameter = zone;
            arg.Name = ZoneMessage.ZONE_PARENT_ZONE;
            TheBns.Current.Message.Send(this, arg);
            return (arg.ReturnValue as Zone);
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

        private void export_PostProgressEvent(int progress, object userState)
        {
            this.ReportProgress(progress, userState);
        }

        private void export_PostErrorInfoEvent(string message)
        {
            if (!string.IsNullOrEmpty(message))
            {
                this.ReportError(message);
            }
        }
    }
}