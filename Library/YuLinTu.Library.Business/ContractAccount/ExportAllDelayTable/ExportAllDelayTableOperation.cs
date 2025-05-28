using Microsoft.Scripting.Actions;
using NPOI.Util;
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
using static YuLinTu.tGISCNet.SmallAngleCheckWKB;

namespace YuLinTu.Library.Business
{
    public class ExportAllDelayTableOperation : Task
    {


        #region Property

        public bool IsGroup { get; set; }
        public ExportAllDelayTableArgument Arg { get; set; }

        #endregion Property

        #region Field
        private string openFilePath;  //打开文件路径
        
        private SystemSetDefine SystemSetDefine = SystemSetDefine.GetIntence();
        private IDbContext dbContext;
        private Zone zone;

        #endregion Field

        #region Ctor
        public ExportAllDelayTableOperation()
        {

        }
        #endregion Ctor
        #region Method—Override

        /// <summary>
        /// 开始执行任务
        /// </summary>
        protected override void OnGo()
        {
            Arg = Argument as ExportAllDelayTableArgument;
            if (Arg == null)
            {
                return;
            }
            dbContext = Arg.DbContext;
            zone = Arg.CurrentZone;
            try
            {
                var surveyStation = dbContext.CreateSurveyFormStation();
                var surveys = surveyStation.GetByZoneCode(zone.FullCode, eSearchOption.Precision);
                if (surveys == null || surveys.Count == 0)
                {
                    this.ReportProgress(100, null);
                    this.ReportWarn(string.Format("{0}未获取承包方数据!", zone.FullName));
                    return;
                }
                
                //listLand.LandNumberFormat(SystemSetDefine);  //根据系统设置进行地块编码的截取
                bool canOpen = ExportAllDelayTable(surveys);
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
            openFilePath = Arg.SaveFilePath;
            System.Diagnostics.Process.Start(openFilePath);
            base.OpenResult();
        }

        #endregion Method—Override

        #region Method—ExportBusiness

        /// <summary>
        /// 一键导出试点工作统计表
        /// </summary>
        public bool ExportAllDelayTable(List<SurveyForm> surveyForms)
        {
            bool result = false;
            try
            {
                if (Arg.CurrentZone == null)
                {
                    this.ReportError("未选择导出数据的地域!");
                    return result;
                }
                this.ReportProgress(1, "开始");
                string markDesc = GetMarkDesc(zone, dbContext);
                string tempPathCBMJQRB = TemplateHelper.ExcelTemplate("承包共有人及承包面积确认统计表");  //模板文件
                string tempPathRKBHTJB = TemplateHelper.ExcelTemplate("承包共有人新增及减少统计表");  //模板文件
                string tempPathRKBHHZB = TemplateHelper.ExcelTemplate("承包共有人增减情况汇总表");  //模板文件
                string tempPathDKBHTJB = TemplateHelper.ExcelTemplate("地块增减变化统计表");  //模板文件
                string tempPathDKBHHZB = TemplateHelper.ExcelTemplate("地块增减情况汇总表");  //模板文件
                string tempPathMDQKHZB = TemplateHelper.ExcelTemplate("调查摸底情况汇总表");  //模板文件
                string tempPathJTJDDTJB = TemplateHelper.ExcelTemplate("集体机动地统计表");  //模板文件
                string tempPathXWTJB = TemplateHelper.WordTemplate("整户消亡统计表");  //模板文件
                int index = 1;

                var zoneStation = dbContext.CreateZoneWorkStation();
                DictionaryBusiness dictBusiness = new DictionaryBusiness(dbContext);
                List<Dictionary> dictList = dictBusiness.GetAll();
                double vpPercent = 99 / (double)surveyForms.Count;
                openFilePath = Arg.SaveFilePath;

                
                List<Zone> allZones = zoneStation.GetAllZonesToProvince(zone);
                
                if (openFilePath == null)
                {
                    openFilePath = TheApp.GetApplicationPath();
                }
                
                //openFilePath= openFilePath + @"\" + savePath+@"\"+argument.CurrentZone.Name;

                ExoprtCBMJQRBFile("承包共有人及承包面积确认统计表", tempPathCBMJQRB, dictList, openFilePath, surveyForms);
                //ExoprtRKBHTJBFile("承包共有人新增及减少统计表", tempPathRKBHTJB);
                //ExoprtRKBHHZBFile("承包共有人增减情况汇总表", tempPathRKBHHZB);
                //ExoprtDKBHTJBFile("地块增减变化统计表", tempPathDKBHTJB);
                //ExoprtDKBHHZBFile("地块增减情况汇总表", tempPathDKBHHZB);
                //ExoprtMDQKHZBFile("调查摸底情况汇总表", tempPathMDQKHZB);
                //ExoprtJTJDDTJBFile("集体机动地统计表", tempPathJTJDDTJB);
                //ExoprtXWTJBFile("整户消亡统计表", tempPathXWTJB);

                    
                result = true;
                string info = string.Format("{0}导出{1}一户一档完整资料", markDesc, index - 1);
                this.ReportInfomation(info);
                this.ReportProgress(100, "完成");
            }
            catch (Exception ex)
            {
                result = false;
                this.ReportError("导出一户一档完整资料失败");
                YuLinTu.Library.Log.Log.WriteException(this, "导出一户一档完整资料", ex.Message + ex.StackTrace);
            }
            return result;
        }




        #endregion Method—ExportBusiness

        #region Method—Private

        private void ExoprtCBMJQRBFile(string tempName, string tempPath, List<Dictionary> dictList,string saveFilePath,List<SurveyForm> surveyForms)
        {
            bool result = false;
            var export = new ExportCBMJQRBTable();
            export.EmptyReplacement = WorkStationExtend.GetSystemSetReplacement();
            #region 通过反射等机制定制化具体的业务处理类
            var temp = WorksheetConfigHelper.GetInstance(export);
            if (temp != null && temp.TemplatePath != null)
            {
                if (temp is ExportCBMJQRBTable)
                {
                    export = (ExportCBMJQRBTable)temp;
                }
                tempPath = Path.Combine(TheApp.GetApplicationPath(), temp.TemplatePath);
            }
            #endregion
            var dictXB = dictList.FindAll(x => x.GroupCode == DictionaryTypeInfo.XB);
            string excelName = GetMarkDesc(zone, dbContext);
            string savePath = saveFilePath + @"\" + excelName + "承包共有人及承包面积确认统计表" + ".xls";
            export.dictXB = dictXB;
            export.SurveyForms = surveyForms;
            export.ZoneDesc = excelName;
            export.TownNme = excelName;
            result = export.BeginToZone(tempPath);
            export.SaveAs(savePath);
            if (result)
            {
                openFilePath = export.saveFilePath;
                export_PostProgressEvent(100, null);
                this.ReportInfomation(string.Format("导出承包共有人及承包面积确认统计表", excelName));
            }
            
        }
        //private void ExoprtRKBHTJBFile(string tempName, string tempPath)
        //{
        //    var export = new ExportOneDossierWordTable();
        //    export.EmptyReplacement = WorkStationExtend.GetSystemSetReplacement();
        //    #region 通过反射等机制定制化具体的业务处理类
        //    var temp = WorksheetConfigHelper.GetInstance(export);
        //    if (temp != null && temp.TemplatePath != null)
        //    {
        //        if (temp is ExportOneDossierWordTable)
        //        {
        //            export = (ExportOneDossierWordTable)temp;
        //        }
        //        tempPath = Path.Combine(TheApp.GetApplicationPath(), temp.TemplatePath);
        //    }
        //    #endregion
        //    var eplands = lands.FindAll(l => l.OwnerId == vp.ID);
        //    export.CurrentZone = argument.CurrentZone;
        //    export.ZoneList = allZones;
        //    export.LandCollection = eplands;
        //    export.ExportPublicTableDeleteEmpty = argument.ContractSettingDefine.ExportPublicTableDeleteEmpty;
        //    export.ExportPublicAwareArea = argument.ContractSettingDefine.ExportPublicTableUseAwareArea;
        //    export.contractAccountPanel = argument.contractAccountPanel;
        //    export.SystemSet.KeepRepeatFlag = SystemSetDefine.KeepRepeatFlag;
        //    export.Contractor = vp;
        //    export.MarkDesc = markDesc;
        //    export.DictList = dictList;
        //    export.Concord = concord;
        //    export.Book = book;
        //    export.ExportVPTableCountContainsDiedPerson = SystemSetDefine.ExportVPTableCountContainsDiedPerson;
        //    export.IsShare = SystemSetDefine.PersonTable;
        //    export.ConcordNumber = concordnumber;
        //    export.Tissue = tissue == null ? new CollectivityTissue { Name = "" } : tissue;
        //    export.WarrentNumber = bookNumber;
        //    export.OpenTemplate(tempPath);
        //    string familyNuber = Library.WorkStation.ToolString.ExceptSpaceString(vp.FamilyNumber);
        //    string filePath = saveFilePath + @"\" + vp.Name + @"\" + familyNuber + "-" + vp.Name + "-" + tempName;
        //    export.SaveAsDocx(vp, filePath);
        //    //export.SplitDocumentByPage(filePath);

        //}
        //private void ExoprtRKBHHZBFile(string tempName, string tempPath)
        //{
        //    var export = new ExportPublicityWordTable();
        //    export.EmptyReplacement = WorkStationExtend.GetSystemSetReplacement();
        //    #region 通过反射等机制定制化具体的业务处理类
        //    var temp = WorksheetConfigHelper.GetInstance(export);
        //    temp.TemplatePath = tempPath;
        //    if (temp != null && temp.TemplatePath != null)
        //    {
        //        if (temp is ExportPublicityWordTable)
        //        {
        //            export = (ExportPublicityWordTable)temp;
        //        }
        //        tempPath = Path.Combine(TheApp.GetApplicationPath(), temp.TemplatePath);
        //    }
        //    #endregion
        //    var eplands = lands.FindAll(l => l.OwnerId == vp.ID);
        //    export.CurrentZone = argument.CurrentZone;
        //    export.ZoneList = allZones;
        //    export.LandCollection = eplands;
        //    export.ExportPublicTableDeleteEmpty = argument.ContractSettingDefine.ExportPublicTableDeleteEmpty;
        //    export.ExportPublicAwareArea = argument.ContractSettingDefine.ExportPublicTableUseAwareArea;
        //    export.SystemSet.KeepRepeatFlag = SystemSetDefine.KeepRepeatFlag;
        //    export.Contractor = vp;
        //    export.DictList = dictList;
        //    export.Concord = concord;
        //    export.Book = book;
        //    export.Tissue = tissue == null ? new CollectivityTissue { Name = "" } : tissue;
        //    export.OpenTemplate(tempPath);
        //    string familyNuber = Library.WorkStation.ToolString.ExceptSpaceString(vp.FamilyNumber);
        //    string filePath = saveFilePath + @"\" + vp.Name + @"\" + familyNuber + "-" + vp.Name + "-" + tempName;
        //    export.SaveAs(vp, filePath);
        //    //export.SplitDocumentByPage(filePath);

        //}
        //private void ExoprtDKBHTJBFile(string tempName, string tempPath)
        //{
        //    var export = new ExportContractorWordTable();
        //    export.EmptyReplacement = WorkStationExtend.GetSystemSetReplacement();
        //    #region 通过反射等机制定制化具体的业务处理类
        //    var temp = WorksheetConfigHelper.GetInstance(export);
        //    temp.TemplatePath = tempPath;
        //    if (temp != null && temp.TemplatePath != null)
        //    {
        //        if (temp is ExportContractorWordTable)
        //        {
        //            export = (ExportContractorWordTable)temp;
        //        }
        //        tempPath = Path.Combine(TheApp.GetApplicationPath(), temp.TemplatePath);
        //    }
        //    #endregion
        //    var eplands = lands.FindAll(l => l.OwnerId == vp.ID);
        //    export.CurrentZone = argument.CurrentZone;
        //    export.ZoneList = allZones;
        //    export.LandCollection = eplands;
        //    export.SystemSet.KeepRepeatFlag = SystemSetDefine.KeepRepeatFlag;
        //    export.Contractor = vp;
        //    export.DictList = dictList;
        //    export.Concord = concord;
        //    export.Book = book;
        //    export.Tissue = tissue == null ? new CollectivityTissue { Name = "" } : tissue;
        //    export.OpenTemplate(tempPath);
        //    string familyNuber = Library.WorkStation.ToolString.ExceptSpaceString(vp.FamilyNumber);
        //    string filePath = saveFilePath + @"\" + vp.Name + @"\" + familyNuber + "-" + vp.Name + "-" + tempName;
        //    export.SaveAs(vp, filePath);
        //    //export.SplitDocumentByPage(filePath);

        //}
        //private void ExoprtDKBHHZBFile(string tempName, string tempPath)
        //{
        //    var export = new ExportYBSQSTable();
        //    export.EmptyReplacement = WorkStationExtend.GetSystemSetReplacement();
        //    #region 通过反射等机制定制化具体的业务处理类
        //    var temp = WorksheetConfigHelper.GetInstance(export);
        //    temp.TemplatePath = tempPath;
        //    if (temp != null && temp.TemplatePath != null)
        //    {
        //        if (temp is ExportYBSQSTable)
        //        {
        //            export = (ExportYBSQSTable)temp;
        //        }
        //        tempPath = Path.Combine(TheApp.GetApplicationPath(), temp.TemplatePath);
        //    }
        //    #endregion
        //    var eplands = lands.FindAll(l => l.OwnerId == vp.ID);
        //    export.CurrentZone = argument.CurrentZone;
        //    export.ZoneList = allZones;
        //    export.LandCollection = eplands;
        //    export.SystemSet.KeepRepeatFlag = SystemSetDefine.KeepRepeatFlag;
        //    export.Contractor = vp;
        //    export.DictList = dictList;
        //    export.Concord = concord;
        //    export.Book = book;
        //    export.Tissue = tissue == null ? new CollectivityTissue { Name = "" } : tissue;
        //    export.OpenTemplate(tempPath);
        //    string familyNuber = Library.WorkStation.ToolString.ExceptSpaceString(vp.FamilyNumber);
        //    string filePath = saveFilePath + @"\" + vp.Name + @"\" + familyNuber + "-" + vp.Name + "-" + tempName;
        //    export.SaveAs(vp, filePath);
        //    //export.SplitDocumentByPage(filePath);

        //}
        //private void ExoprtMDQKHZBFile(string tempName, string tempPath)
        //{
        //    var export = new ExportYBSQSTable();
        //    export.EmptyReplacement = WorkStationExtend.GetSystemSetReplacement();
        //    #region 通过反射等机制定制化具体的业务处理类
        //    var temp = WorksheetConfigHelper.GetInstance(export);
        //    temp.TemplatePath = tempPath;
        //    if (temp != null && temp.TemplatePath != null)
        //    {
        //        if (temp is ExportYBSQSTable)
        //        {
        //            export = (ExportYBSQSTable)temp;
        //        }
        //        tempPath = Path.Combine(TheApp.GetApplicationPath(), temp.TemplatePath);
        //    }
        //    #endregion
        //    var eplands = lands.FindAll(l => l.OwnerId == vp.ID);
        //    export.CurrentZone = argument.CurrentZone;
        //    export.ZoneList = allZones;
        //    export.LandCollection = eplands;
        //    export.SystemSet.KeepRepeatFlag = SystemSetDefine.KeepRepeatFlag;
        //    export.Contractor = vp;
        //    export.DictList = dictList;
        //    export.Concord = concord;
        //    export.Book = book;
        //    export.Tissue = tissue == null ? new CollectivityTissue { Name = "" } : tissue;
        //    export.OpenTemplate(tempPath);
        //    string familyNuber = Library.WorkStation.ToolString.ExceptSpaceString(vp.FamilyNumber);
        //    string filePath = saveFilePath + @"\" + vp.Name + @"\" + familyNuber + "-" + vp.Name + "-" + tempName;
        //    export.SaveAs(vp, filePath);
        //    //export.SplitDocumentByPage(filePath);
        //}
        //private void ExoprtJTJDDTJBFile(string tempName, string tempPath)
        //{
        //    var export = new ExportLandWordTable();
        //    export.EmptyReplacement = WorkStationExtend.GetSystemSetReplacement();
        //    #region 通过反射等机制定制化具体的业务处理类
        //    var temp = WorksheetConfigHelper.GetInstance(export);
        //    temp.TemplatePath = tempPath;
        //    if (temp != null && temp.TemplatePath != null)
        //    {
        //        if (temp is ExportLandWordTable)
        //        {
        //            export = (ExportLandWordTable)temp;
        //        }
        //        tempPath = Path.Combine(TheApp.GetApplicationPath(), temp.TemplatePath);
        //    }
        //    #endregion
        //    var eplands = lands.FindAll(l => l.OwnerId == vp.ID);
        //    export.CurrentZone = argument.CurrentZone;
        //    export.ZoneList = allZones;
        //    export.LandCollection = eplands;
        //    export.ExportPublicTableDeleteEmpty = argument.ContractSettingDefine.ExportPublicTableDeleteEmpty;
        //    export.ExportPublicAwareArea = argument.ContractSettingDefine.ExportPublicTableUseAwareArea;
        //    export.contractAccountPanel = argument.contractAccountPanel;
        //    export.SystemSet.KeepRepeatFlag = SystemSetDefine.KeepRepeatFlag;
        //    export.Contractor = vp;
        //    export.MarkDesc = markDesc;
        //    export.DictList = dictList;
        //    export.Concord = concord;
        //    export.Book = book;
        //    export.ExportVPTableCountContainsDiedPerson = SystemSetDefine.ExportVPTableCountContainsDiedPerson;
        //    export.IsShare = SystemSetDefine.PersonTable;
        //    export.ConcordNumber = concordnumber;
        //    export.Tissue = tissue == null ? new CollectivityTissue { Name = "" } : tissue;
        //    export.WarrentNumber = bookNumber;
        //    export.OpenTemplate(tempPath);
        //    string familyNuber = Library.WorkStation.ToolString.ExceptSpaceString(vp.FamilyNumber);
        //    string filePath = saveFilePath + @"\" + vp.Name + @"\" + familyNuber + "-" + vp.Name + "-" + tempName;
        //    export.SaveAs(vp, filePath);
        //}
        //private void ExoprtXWTJBFile(string tempName, string tempPath)
        //{
        //    var export = new ExportLandWordTable();
        //    export.EmptyReplacement = WorkStationExtend.GetSystemSetReplacement();
        //    #region 通过反射等机制定制化具体的业务处理类
        //    var temp = WorksheetConfigHelper.GetInstance(export);
        //    temp.TemplatePath = tempPath;
        //    if (temp != null && temp.TemplatePath != null)
        //    {
        //        if (temp is ExportLandWordTable)
        //        {
        //            export = (ExportLandWordTable)temp;
        //        }
        //        tempPath = Path.Combine(TheApp.GetApplicationPath(), temp.TemplatePath);
        //    }
        //    #endregion
        //    var eplands = lands.FindAll(l => l.OwnerId == vp.ID);
        //    export.CurrentZone = argument.CurrentZone;
        //    export.ZoneList = allZones;
        //    export.LandCollection = eplands;
        //    export.ExportPublicTableDeleteEmpty = argument.ContractSettingDefine.ExportPublicTableDeleteEmpty;
        //    export.ExportPublicAwareArea = argument.ContractSettingDefine.ExportPublicTableUseAwareArea;
        //    export.contractAccountPanel = argument.contractAccountPanel;
        //    export.SystemSet.KeepRepeatFlag = SystemSetDefine.KeepRepeatFlag;
        //    export.Contractor = vp;
        //    export.MarkDesc = markDesc;
        //    export.DictList = dictList;
        //    export.Concord = concord;
        //    export.Book = book;
        //    export.ExportVPTableCountContainsDiedPerson = SystemSetDefine.ExportVPTableCountContainsDiedPerson;
        //    export.IsShare = SystemSetDefine.PersonTable;
        //    export.ConcordNumber = concordnumber;
        //    export.Tissue = tissue == null ? new CollectivityTissue { Name = "" } : tissue;
        //    export.WarrentNumber = bookNumber;
        //    export.OpenTemplate(tempPath);
        //    string familyNuber = Library.WorkStation.ToolString.ExceptSpaceString(vp.FamilyNumber);
        //    string filePath = saveFilePath + @"\" + vp.Name + @"\" + familyNuber + "-" + vp.Name + "-" + tempName;
        //    export.SaveAs(vp, filePath);
        //}
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
