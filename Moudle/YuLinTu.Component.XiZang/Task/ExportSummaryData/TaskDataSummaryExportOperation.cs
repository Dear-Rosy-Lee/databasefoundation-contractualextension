/*
 * (C) 2015  鱼鳞图公司版权所有,保留所有权利 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using YuLinTu.Library.Entity;
using YuLinTu;
using YuLinTu.Data;
using System.Diagnostics;
using System.IO;
using System.Collections;
using YuLinTu.Data.Dynamic;
using YuLinTu.Library.Office;

using YuLinTu.Library.Business;
using YuLinTu.Diagrams;
using YuLinTu.Library.Controls;
using YuLinTu.Library.WorkStation;

namespace YuLinTu.Component.XiZangLZ
{
    /// <summary>
    /// 导出汇总数据操作
    /// </summary>
    public class TaskDataSummaryExportOperation : Task
    {
        #region Fields

        private Zone currentZone = null;//当前地域
        private List<Zone> allZones = null;//当前地域，村、镇
        private IDbContext dbContext = null;
        private ALLValidateArgument currentZoneExportArgs;//当前地域下需要导出的人地合同权证
        private string vpSavePath = null;//当前承包方保存路径
        private string descZone = null;//到镇的描述
        private string dictoryName = AppDomain.CurrentDomain.BaseDirectory + @"Template\西藏字典.xlsx";
        private ContractBusinessParcelWordSettingDefine ParcelWordSettingDefine = ContractBusinessParcelWordSettingDefine.GetIntence();
        private SystemSetDefine systemSet = SystemSetDefine.GetIntence();
        #endregion

        #region Methods

        #region Methods - Override

        protected override void OnGo()
        {
            var args = Argument as TaskDataSummaryExportArgument;
            currentZoneExportArgs = new ALLValidateArgument();
            dbContext = args.Database;
            if (dbContext == null)
            {
                this.ReportWarn("数据源为空！");
                return;
            }

            if (args.CurrentZone == null)
            {
                this.ReportWarn("未选择行政区域！");
                return;
            }
            currentZone = args.CurrentZone;
            descZone = ExportZoneListDir(currentZone, args.AllZones);
            allZones = args.AllZones;
            this.ReportProgress(0, "开始验证参数...");
            this.ReportAlert(eMessageGrade.Infomation, null, "开始验证参数...");
            if (!ValidateArgument(args))
                return;

            this.ReportProgress(2, "获取导出参数...");
            this.ReportAlert(eMessageGrade.Infomation, null, "获取导出参数...");
            if (!GetALLValidateArgument())
                return;

            this.ReportProgress(5, "开始导出数据...");//导出数据时，将地域图层都导出、承包地图层选择导出村或者组下对应地块
            this.ReportAlert(eMessageGrade.Infomation, null, "开始导出数据...");
            SelectExportMode(args);

            this.ReportProgress(100, "导出完成");
            this.ReportAlert(eMessageGrade.Infomation, null, "导出完成");
        }

        /// <summary>
        /// 结束释放
        /// </summary>
        protected override void OnEnded()
        {
            currentZoneExportArgs = null;
        }

        #endregion

        #region Methods - Private

        /// <summary>
        /// 验证参数
        /// </summary>      
        private bool ValidateArgument(TaskDataSummaryExportArgument args)
        {
            if (currentZone.Level > eZoneLevel.Town)
            {
                this.ReportAlert(eMessageGrade.Error, null, "行政地域不能大于镇级。");
                return false;
            }

            if (args == null)
            {
                this.ReportAlert(eMessageGrade.Error, null, "参数不能为空。");
                return false;
            }

            if (args.FileName.IsNullOrBlank())
            {
                this.ReportAlert(eMessageGrade.Error, null, "目标路径不能为空。");
                return false;
            }

            return true;
        }

        /// <summary>
        /// 获取当前地域下要导出的所有人、地、合同、等参数
        /// </summary>      
        private bool GetALLValidateArgument()
        {
            var VirtualPersonStation = dbContext.CreateVirtualPersonStation<LandVirtualPerson>();
            var contractLandWorkStation = dbContext.CreateContractLandWorkstation();
            var concordStation = dbContext.CreateConcordStation();
            var contractRegeditBookStation = dbContext.CreateRegeditBookStation();
            var senderStation = dbContext.CreateSenderWorkStation();
            var dicStation = dbContext.CreateDictWorkStation();

            List<ContractConcord> concordCollection = concordStation.GetByZoneCode(currentZone.FullCode);
            List<YuLinTu.Library.Entity.ContractRegeditBook> bookCollection = contractRegeditBookStation.GetByZoneCode(currentZone.FullCode, eSearchOption.Precision);
            List<VirtualPerson> familyCollection = VirtualPersonStation.GetByZoneCode(currentZone.FullCode);
            List<ContractLand> AllLandCollection = contractLandWorkStation.GetCollection(currentZone.FullCode, eLevelOption.Self);
            CollectivityTissue tissue = senderStation.Get(currentZone.FullCode, currentZone.FullName);
            List<Dictionary> dicList = dicStation.Get();
            if (tissue == null)
                tissue = senderStation.Get(currentZone.ID);
            currentZoneExportArgs.zonevps = familyCollection;
            currentZoneExportArgs.zoneConcords = concordCollection;
            currentZoneExportArgs.zoneContractLands = AllLandCollection;
            currentZoneExportArgs.zoneContractRegeditBooks = bookCollection;
            currentZoneExportArgs.tissue = tissue;
            currentZoneExportArgs.DicList = dicList;

            return true;
        }

        /// <summary>
        /// 导出数据模式，分多个人，单个人或分地域组导出
        /// </summary>       
        private void SelectExportMode(TaskDataSummaryExportArgument args)
        {
            List<VirtualPerson> exportVitualPersons = new List<VirtualPerson>();

            //导出发包方调查表
            if (args.ExportDataSummaryTableTypes.ExportSenderSurvyTable)
            {
                var result = ExportSenderSurvyTable(currentZoneExportArgs.tissue);
                if (!result)
                {
                    this.ReportInfomation(string.Format("未导出{0}的发包方调查表", currentZone.Name));
                }
            }
            if (args.ExportDataSummaryTableTypes.ExportSurvyInfoPublishTable)
            {
                var result = ExportSurvyInfoPublishTable();
                if (!result)
                {
                    this.ReportInfomation(string.Format("未导出{0}的农村土地承包经营权调查信息公示表", currentZone.Name));
                }
            }

            if (args.SelectContractors != null && args.SelectContractors.Count > 0)
            {
                exportVitualPersons = args.SelectContractors;
            }
            else
            {
                exportVitualPersons = currentZoneExportArgs.zonevps;
            }
            if (exportVitualPersons.Count == 0)
            {
                //this.ReportAlert(eMessageGrade.Warn, null, currentZone.FullName + "下没有承包方。");
                this.ReportProgress(100);
                return;
            }
            ALLValidateArgument vpExportArgs;//获取当前个人的信息          
            int percentOfVP = (90 / exportVitualPersons.Count);
            int percentIndex = 1;
            foreach (var selectvp in exportVitualPersons)
            {
                this.ReportProgress(5 + percentOfVP * percentIndex, "导出" + selectvp.Name + "的汇总文件");
                vpExportArgs = new ALLValidateArgument();
                vpExportArgs.zoneConcords = currentZoneExportArgs.zoneConcords.FindAll(c => c.ContracterId == selectvp.ID);
                vpExportArgs.zoneContractLands = currentZoneExportArgs.zoneContractLands.FindAll(ld => ld.OwnerId == selectvp.ID);
                List<YuLinTu.Library.Entity.ContractRegeditBook> crb = new List<YuLinTu.Library.Entity.ContractRegeditBook>();
                foreach (var cd in vpExportArgs.zoneConcords)
                {
                    crb.Add(currentZoneExportArgs.zoneContractRegeditBooks.Find(rb => rb.ID == cd.ID));
                }
                vpExportArgs.zoneContractRegeditBooks = crb;
                vpExportArgs.tissue = currentZoneExportArgs.tissue;
                if (!Directory.Exists(Path.GetDirectoryName(args.FileName + @"\" + selectvp.FamilyNumber + "-" + selectvp.Name)))
                    Directory.CreateDirectory(Path.GetDirectoryName(args.FileName + @"\" + selectvp.FamilyNumber + "-" + selectvp.Name));
                vpSavePath = args.FileName + @"\" + selectvp.FamilyNumber + "-" + selectvp.Name;
                vpExportArgs.DicList = args.DictList;
                ExportData(selectvp, vpExportArgs);//使用底层开始导出  
                percentIndex++;
            }

            CanOpenResult = true;
        }

        /// <summary>
        /// 打开文件
        /// </summary>
        public override void OpenResult()
        {
            var args = Argument as TaskDataSummaryExportArgument;
            System.Diagnostics.Process.Start(args.FileName);
            base.OpenResult();
        }


        /// <summary>
        /// 导出数据，单个人及其信息
        /// </summary>       
        private void ExportData(VirtualPerson exportVp, ALLValidateArgument vpExportArgs)
        {
            bool result;
            var args = Argument as TaskDataSummaryExportArgument;
            //根据选择的参数导出表格

            #region 导出个人表格集合
            if (args.ExportDataSummaryTableTypes.ExportVPSurvyTable)
            {
                result = ExportVPSurvyTable(exportVp, vpExportArgs);
                if (!result)
                {
                    this.ReportInfomation(string.Format("未导出{0}的承包方调查表", exportVp.Name));
                }
            }

            if (args.ExportDataSummaryTableTypes.ExportLandSurvyTable)
            {
                result = ExportLandSurvyTable(exportVp, vpExportArgs);
                if (!result)
                {
                    this.ReportInfomation(string.Format("未导出{0}的地块调查表", exportVp.Name));
                }
            }

            if (args.ExportDataSummaryTableTypes.ExportPublicDataWord)
            {
                result = result = ExportPublicDataWord(exportVp, vpExportArgs);
                if (!result)
                {
                    this.ReportInfomation(string.Format("未导出{0}的公示结果归户表", exportVp.Name));
                }
            }
            if (args.ExportDataSummaryTableTypes.ExportRegeditBook)
            {
                result = ExportRegeditBook(exportVp, vpExportArgs);
                if (!result)
                {
                    this.ReportInfomation(string.Format("未导出{0}的登记簿", exportVp.Name));
                }
            }

            if (args.ExportDataSummaryTableTypes.ExportDJBZSQSTable)
            {
                result = ExportApplicationBook(exportVp, vpExportArgs);
                if (!result)
                {
                    this.ReportInfomation(string.Format("未导出{0}的登记申请书", exportVp.Name));
                }
            }

            if (args.ExportDataSummaryTableTypes.ExportAttorney)
            {
                result = ExportAgencyBook(exportVp, vpExportArgs);
                if (!result)
                {
                    this.ReportInfomation(string.Format("未导出{0}的委托代理声明书", exportVp.Name));
                }
            }

            if (args.ExportDataSummaryTableTypes.ExportConcord)
            {
                result = ExportConcordBook(exportVp, vpExportArgs);
                if (!result)
                {
                    this.ReportInfomation(string.Format("未导出{0}的承包合同", exportVp.Name));
                }
            }

            if (args.ExportDataSummaryTableTypes.ExportLandParcel)
            {
                result = ExportLandParcelWord(exportVp, vpExportArgs);
                if (!result)
                {
                    this.ReportInfomation(string.Format("未导出{0}的地块示意图", exportVp.Name));
                }
            }

            if (args.ExportDataSummaryTableTypes.ExportStatement)
            {
                result = ExportStatementBook(exportVp, vpExportArgs);
                if (!result)
                {
                    this.ReportInfomation(string.Format("未导出{0}的户主声明书", exportVp.Name));
                }
            }
        }

        #region Methods - exportHelper-单个文件导出底层

        #region 户主声明书

        /// <summary>
        /// 导出户主声明书
        /// </summary>
        /// <param name="exportVp"></param>
        /// <param name="vpExportArgs"></param>
        /// <returns></returns>
        private bool ExportStatementBook(VirtualPerson exportVp, ALLValidateArgument vpExportArgs)
        {
            string templatePath = TemplateHelper.WordTemplate(TemplateFile.VirtualPersonApplyBook);
            string zoneName = currentZone.FullName;
            var args = Argument as TaskDataSummaryExportArgument;
            bool result = true;
            //确定是否导出集体户信息(利用配置文件)
            if (args.FamilyOtherSet.ShowFamilyInfomation && (exportVp.Name.IndexOf("机动地") >= 0 || exportVp.Name.IndexOf("集体") >= 0))
            {
                return false;
            }
            var zonelist = GetParentZone(currentZone);
            string familyNuber = Library.Business.ToolString.ExceptSpaceString(exportVp.FamilyNumber);
            ExportApplyBook exportFamily = new ExportApplyBook(exportVp);
            exportFamily.Date = args.Date;       //获取日期
            exportFamily.ZoneName = zoneName;
            exportFamily.ZoneList = zonelist;
            exportFamily.CurrentZone = currentZone;
            exportFamily.OpenTemplate(templatePath);
            exportFamily.RightName = InitalizeRightType(args.VirtualType);
            exportFamily.SaveAs(exportVp, vpSavePath + @"\" + TemplateFile.VirtualPersonApplyBook + ".doc");
            this.ReportInfomation(string.Format("导出{0}{1}的户主声明书", descZone, exportVp.Name));

            return result;
        }

        #endregion

        #region 承包合同

        /// <summary>
        /// 导出承包合同
        /// </summary>
        /// <param name="exportVp"></param>
        /// <param name="vpExportArgs"></param>
        /// <returns></returns>
        private bool ExportConcordBook(VirtualPerson exportVp, ALLValidateArgument vpExportArgs)
        {
            bool result = false;
            var args = Argument as TaskDataSummaryExportArgument;
            try
            {
                IZoneWorkStation zoneStation = dbContext.CreateZoneWorkStation();
                var concordStation = dbContext.CreateConcordStation();
                string tempPath = TemplateHelper.WordTemplate("西藏农村土地承包经营权农村土地承包合同书");
                string familyNuber = Library.Business.ToolString.ExceptSpaceString(exportVp.FamilyNumber);
                string zoneNameCounty = GetZoneNameByLevel(currentZone.FullCode, eZoneLevel.County, zoneStation);
                string zoneNameTown = GetZoneNameByLevel(currentZone.FullCode, eZoneLevel.Town, zoneStation);
                string zoneNameVillage = GetZoneNameByLevel(currentZone.FullCode, eZoneLevel.Village, zoneStation);
                string zoneNameGroup = GetZoneNameByLevel(currentZone.FullCode, eZoneLevel.Group, zoneStation);
                var concords = concordStation.GetByZoneCode(currentZone.FullCode);
                var vpConcord = concords.Where(s => s.ContracterId == exportVp.ID).FirstOrDefault();
                ExportAgricultureConcord export = new ExportAgricultureConcord(dictoryName);
                export.dbContext = dbContext;
                export.CurrentZone = currentZone;
                export.VirtualPerson = exportVp;
                export.Systemset = systemSet;
                export.Concord = vpConcord;
                export.ZoneNameCounty = string.IsNullOrEmpty(zoneNameCounty) ? "" : zoneNameCounty;
                export.ZoneNameTown = string.IsNullOrEmpty(zoneNameTown) ? "" : zoneNameTown;
                export.ZoneNameVillage = string.IsNullOrEmpty(zoneNameVillage) ? "" : zoneNameVillage;
                export.ZoneNameGroup = string.IsNullOrEmpty(zoneNameGroup) ? "" : zoneNameGroup;
                export.ListLand = vpExportArgs.zoneContractLands.Clone() as List<ContractLand>;
                export.ListDict = vpExportArgs.DicList;

                export.LandCollection = vpExportArgs.zoneContractLands.FindAll(l => l.OwnerId == exportVp.ID);
                export.OpenTemplate(tempPath);
                export.SaveAs(vpConcord, vpSavePath + @"\" + "承包合同.doc");
                result = true;
            }
            catch (Exception ex)
            {
                result = false;
                this.ReportError("导出承包合同失败");
                YuLinTu.Library.Log.Log.WriteException(this, "ExportVirtualPersonExcel(导出承包合同数据到Word表)", ex.Message + ex.StackTrace);
            }
            if (result)
            {
                this.ReportInfomation(string.Format("{0}导出{1}承包合同", descZone, exportVp.Name));
            }
            return result;
        }

        #endregion

        #region 委托代理声明书

        /// <summary>
        /// 导出委托代理声明书
        /// </summary>
        /// <param name="exportVp"></param>
        /// <param name="vpExportArgs"></param>
        /// <returns></returns>
        private bool ExportAgencyBook(VirtualPerson exportVp, ALLValidateArgument vpExportArgs)
        {
            string templatePath = TemplateHelper.WordTemplate(TemplateFile.VirtualPersonDelegateBook);
            string zoneName = currentZone.FullName;
            var args = Argument as TaskDataSummaryExportArgument;
            bool result = true;
            var zoneList = GetParents(currentZone, dbContext);
            //确定是否导出集体户信息(利用配置文件)
            if (args.FamilyOtherSet.ShowFamilyInfomation && (exportVp.Name.IndexOf("机动地") >= 0 || exportVp.Name.IndexOf("集体") >= 0))
            {
                return false;
            }
            string familyNuber = Library.Business.ToolString.ExceptSpaceString(exportVp.FamilyNumber);
            ExportDelegateBook exportFamily = new ExportDelegateBook(exportVp);
            exportFamily.DateValue = args.Date;
            exportFamily.ZoneList = zoneList == null ? new List<Zone>() : zoneList;
            exportFamily.CurrentZone = currentZone;
            exportFamily.OpenTemplate(templatePath);
            exportFamily.RightName = InitalizeRightType(args.VirtualType);
            exportFamily.SaveAs(exportVp, vpSavePath + @"\" + TemplateFile.VirtualPersonDelegateBook + ".doc");
            this.ReportInfomation(string.Format("导出{0}{1}的户主委托书", descZone, exportVp.Name));

            return result;
        }

        #endregion

        #region 登记申请书

        /// <summary>
        /// 导出登记申请书
        /// </summary>
        /// <param name="exportVp"></param>
        /// <param name="vpExportArgs"></param>
        /// <returns></returns>
        private bool ExportApplicationBook(VirtualPerson exportVp, ALLValidateArgument vpExportArgs)
        {
            var args = Argument as TaskDataSummaryExportArgument;
            bool result = true;
            string tempPath = "西藏" + TemplateFile.ContractFamilyRequireBook;
            result = SingleExportRequireBook(currentZone, vpExportArgs, exportVp, tempPath, eConstructMode.Family);
            if (result)
            {
                this.ReportInfomation(string.Format("导出{0}{1}的单户登记申请书(家庭承包)", descZone, exportVp.Name));
            }
            //tempPath = "西藏" + TemplateFile.ContractFamilyRequireBook;
            //result = SingleExportRequireBook(currentZone, vpExportArgs, exportVp, tempPath, eConstructMode.Other);
            //if (result)
            //{
            //    this.ReportInfomation(string.Format("导出{0}{1}的单户登记申请书(非家庭承包)", descZone, exportVp.Name));
            //}
            return result;
        }

        #endregion

        #region 承包方

        /// <summary>
        /// 导出承包方调查表word
        /// </summary>       
        private bool ExportVPSurvyTable(VirtualPerson exportVp, ALLValidateArgument vpExportArgs)
        {
            bool result = false;
            var args = Argument as TaskDataSummaryExportArgument;
            try
            {
                string tempPath = TemplateHelper.WordTemplate("西藏" + TemplateFile.VirtualPersonSurveyWord);
                string familyNuber = Library.Business.ToolString.ExceptSpaceString(exportVp.FamilyNumber);
                List<VirtualPerson> exportVps = new List<VirtualPerson>();
                exportVps.Add(exportVp);
                ExportContractorTable export = new ExportContractorTable(dictoryName);
                export.LandCollection = vpExportArgs.zoneContractLands.FindAll(l => l.OwnerId == exportVp.ID);
                export.OpenTemplate(tempPath);
                export.SaveAs(exportVp, vpSavePath + @"\" + "承包方调查表.doc");
                result = true;
            }
            catch (Exception ex)
            {
                result = false;
                this.ReportError("导出承包方调查表失败");
                YuLinTu.Library.Log.Log.WriteException(this, "ExportVirtualPersonExcel(导出承包方数据到Excel表)", ex.Message + ex.StackTrace);
            }
            if (result)
            {
                this.ReportInfomation(string.Format("{0}导出{1}承包方调查表", descZone, exportVp.Name));
            }
            return result;
        }

        #endregion

        #region 发包方

        /// <summary>
        /// 导出发包方调查表word
        /// </summary>       
        private bool ExportSenderSurvyTable(CollectivityTissue tissue)
        {
            bool result = false;
            if (tissue == null)
            {
                this.ReportInfomation(string.Format("未获取{0}的发包方", currentZone.FullName));
                return result;
            }

            var args = Argument as TaskDataSummaryExportArgument;
            try
            {
                string fileName = TemplateHelper.WordTemplate("西藏" + TemplateFile.SenderSurveyWord);
                ExportSenderTable senderTable = new ExportSenderTable(dictoryName);
                senderTable.OpenTemplate(fileName);
                senderTable.DictList = currentZoneExportArgs.DicList;
                senderTable.SaveAs(tissue, args.FileName + "\\" + tissue.Name + "(" + tissue.Code + ")" + "发包方调查表");

                result = true;
            }
            catch (Exception ex)
            {
                result = false;
                this.ReportError("导出发包方调查表失败");
                YuLinTu.Library.Log.Log.WriteException(this, "ExportVirtualPersonExcel(导出发包方调查表)", ex.Message + ex.StackTrace);
            }
            if (result)
            {
                this.ReportInfomation(string.Format("{0}导出发包方调查表", descZone));
            }
            return result;
        }

        #endregion

        #region 调查信息公示表

        /// <summary>
        /// 导出调查信息公示表
        /// </summary>       
        private bool ExportSurvyInfoPublishTable()
        {
            bool result = false;
            var argument = Argument as TaskDataSummaryExportArgument;
            try
            {
                List<ContractAccountLandFamily> accountFamilyCollection = new List<ContractAccountLandFamily>();
                List<ContractAccountLandFamily> accountFamilyCollectionOther = new List<ContractAccountLandFamily>();
                List<VirtualPerson> vps = new List<VirtualPerson>();
                var orderdVps = currentZoneExportArgs.zonevps.OrderBy(vp =>
                {
                    //排序
                    int num = 0;
                    Int32.TryParse(vp.FamilyNumber, out num);
                    return num;
                });
                foreach (YuLinTu.Library.Entity.VirtualPerson vp in orderdVps)
                {
                    vps.Add(vp);
                }
                int personCount = vps == null ? 0 : vps.Count;
                foreach (VirtualPerson vp in vps)
                {
                    var landCollection = currentZoneExportArgs.zoneContractLands == null ? new List<ContractLand>() : currentZoneExportArgs.zoneContractLands.FindAll(c => c.OwnerId == vp.ID);
                    var landCollectionother = currentZoneExportArgs.zoneContractLands == null ? new List<ContractLand>() : currentZoneExportArgs.zoneContractLands.FindAll(c => c.OwnerId == vp.ID && c.LandCategory != ((int)eLandCategoryType.ContractLand).ToString());
                    ContractAccountLandFamily accountLandFamily = new ContractAccountLandFamily();
                    ContractAccountLandFamily accountLandFamilyOther = new ContractAccountLandFamily();
                    accountLandFamily.CurrentFamily = vp;
                    accountLandFamily.Persons = vp.SharePersonList;
                    accountLandFamily.LandCollection = landCollection;
                    accountFamilyCollection.Add(accountLandFamily);

                    accountLandFamilyOther.CurrentFamily = vp;
                    accountLandFamilyOther.Persons = vp.SharePersonList;
                    accountLandFamilyOther.LandCollection = landCollectionother;
                    accountFamilyCollectionOther.Add(accountLandFamilyOther);
                }
                string excelName = GetMarkDesc(argument.CurrentZone, dbContext);
                string tempPath = TemplateHelper.ExcelTemplate("西藏" + TemplateFile.LandInfomationExcel);
                var zoneStation = dbContext.CreateZoneWorkStation();
                var tissue = currentZoneExportArgs.tissue;
                string senderName = tissue.Name;
                string zoneName = string.Empty;
                if (tissue != null)
                {
                    zoneName = tissue.Name;
                }
                else
                {
                    zoneName = zoneStation.GetZoneName(argument.CurrentZone);
                }
                string villageName = zoneStation.GetVillageName(currentZone);

                using (ExportLandInfomationExcelTable export = new ExportLandInfomationExcelTable())
                {
                    export.SaveFilePath = argument.FileName + @"\" + excelName + TemplateFile.LandInfomationExcel + ".xls";
                    export.CurrentZone = argument.CurrentZone;
                    export.TemplateFile = tempPath;
                    export.SettingDefine = argument.SettingDefine;
                    export.AccountLandFamily = accountFamilyCollection;
                    export.AccountLandFamilyOthers = accountFamilyCollectionOther;
                    export.SenderName = zoneName;
                    export.SenderNameVillage = villageName;
                    export.SystemDefine = argument.SystemSet;

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
                    if (result)
                    {
                        export_PostProgressEvent(100, null);
                        this.ReportInfomation(string.Format("{0}导出{1}户调查信息数据", excelName, personCount));
                    }
                }
            }
            catch (Exception ex)
            {
                result = false;
                this.ReportError("导出农村土地承包经营权调查信息公示表失败");
                YuLinTu.Library.Log.Log.WriteException(this, "ExportVirtualPersonExcel(导出农村土地承包经营权调查信息公示表)", ex.Message + ex.StackTrace);
            }
            if (result)
            {
                this.ReportInfomation(string.Format("{0}导出农村土地承包经营权调查信息公示表", descZone));
            }
            return result;
        }

        #endregion

        #region 公示结果归户表

        /// <summary>
        /// 导出公示结果归户表
        /// </summary>       
        private bool ExportPublicDataWord(VirtualPerson exportVp, ALLValidateArgument vpExportArgs)
        {
            var args = Argument as TaskDataSummaryExportArgument;
            bool result = true;
            List<ContractLand> landArrays = vpExportArgs.zoneContractLands.Clone() as List<ContractLand>;
            landArrays.LandNumberFormat(args.SystemSet);
            try
            {
                string tempPath = TemplateHelper.WordTemplate("西藏农村承包经营权公示结果归户表");
                var vpconcord = vpExportArgs.zoneConcords.Find(dd => dd.ContracterId == exportVp.ID && dd.ArableLandType == "110");
                ExportLandPublicityTable export = new ExportLandPublicityTable(tempPath, dictoryName);
                export.CurrentZone = currentZone;
                export.Concord = vpconcord;
                export.Contractor = exportVp;
                export.LandCollection = landArrays == null ? new List<ContractLand>() : landArrays;  //地块集合
                export.Tissue = vpExportArgs.tissue; //发包方
                string fileName = vpSavePath + @"\" + "公示结果归户表";
                export.OpenTemplate(tempPath);
                export.SaveAs(exportVp, fileName);
                result = true;
                this.ReportInfomation(string.Format("导出{0}{1}的公示结果归户表", descZone, exportVp.Name));
            }
            catch (Exception ex)
            {
                this.ReportError(ex.Message);
                YuLinTu.Library.Log.Log.WriteException(this, "ExportDataWord(导出数据到Word表)", ex.Message + ex.StackTrace);
            }
            return result;

        }

        #endregion

        #region 地块示意图

        /// <summary>
        /// 导出地块示意图
        /// </summary>       
        private bool ExportLandParcelWord(VirtualPerson exportVp, ALLValidateArgument vpExportArgs)
        {
            bool result = false;
            var args = Argument as TaskDataSummaryExportArgument;
            List<ContractLand> listLand = new List<ContractLand>();
            List<ContractLand> listGeoLand = new List<ContractLand>(1000);
            List<XZDW> listLine = new List<XZDW>(1000);
            List<DZDW> listPoint = new List<DZDW>(1000);
            List<MZDW> listPolygon = new List<MZDW>(1000);
            List<Dictionary> dictDKLB = new List<Dictionary>(500);
            List<BuildLandBoundaryAddressDot> listDot = new List<BuildLandBoundaryAddressDot>(1000);
            List<BuildLandBoundaryAddressCoil> listCoil = new List<BuildLandBoundaryAddressCoil>(1000);
            List<BuildLandBoundaryAddressDot> listValidDot = new List<BuildLandBoundaryAddressDot>(1000);
            DiagramsView viewOfAllMultiParcel = null;
            DiagramsView viewOfNeighorParcels = null;
            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                viewOfAllMultiParcel = new DiagramsView();
                viewOfNeighorParcels = new DiagramsView();
            }));
            string fileName = args.FileName;
            var dotStation = dbContext.CreateBoundaryAddressDotWorkStation();
            var lineStation = dbContext.CreateXZDWWorkStation();
            var PointStation = dbContext.CreateDZDWWorkStation();
            var PolygonStation = dbContext.CreateMZDWWorkStation();
            var zoneStation = dbContext.CreateZoneWorkStation();
            var coilStation = dbContext.CreateBoundaryAddressCoilWorkStation();

            var concordStation = dbContext.CreateConcordStation();
            var bookStation = dbContext.CreateRegeditBookStation();
            var senderStation = dbContext.CreateSenderWorkStation();
            var landStation = dbContext.CreateContractLandWorkstation();

            string templatePath = TemplateHelper.WordTemplate(TemplateFile.ParcelWord);
            string savePathOfImage = fileName;
            try
            {
                listLand = currentZoneExportArgs.zoneContractLands.Clone() as List<ContractLand>;
                var VillageZone = zoneStation.Get(currentZone.UpLevelCode);
                listGeoLand = listLand.FindAll(c => c.Shape != null);
                listLine = lineStation.GetByZoneCode(currentZone.FullCode);
                dictDKLB = vpExportArgs.DicList.FindAll(c => !string.IsNullOrEmpty(c.GroupCode) && c.GroupCode == DictionaryTypeInfo.DKLB);

                listDot = dotStation.GetByZoneCode(currentZone.FullCode, eLevelOption.Self);
                listValidDot = listDot.FindAll(c => c.IsValid == true);
                listPoint = PointStation.GetByZoneCode(currentZone.FullCode);
                listPolygon = PolygonStation.GetByZoneCode(currentZone.FullCode);

                Application.Current.Dispatcher.Invoke(new Action(() =>
                {
                    viewOfAllMultiParcel.Paper.Model.Width = 336;
                    viewOfAllMultiParcel.Paper.Model.Height = 357;
                    viewOfAllMultiParcel.Paper.Model.BorderWidth = 0;
                    viewOfAllMultiParcel.Paper.Model.X = 0;
                    viewOfAllMultiParcel.Paper.Model.Y = 0;
                }));

                Application.Current.Dispatcher.Invoke(new Action(() =>
                {
                    viewOfNeighorParcels.Paper.Model.Width = 150;
                    viewOfNeighorParcels.Paper.Model.Height = 150;
                    viewOfNeighorParcels.Paper.Model.BorderWidth = 0;
                    viewOfNeighorParcels.Paper.Model.X = 0;
                    viewOfNeighorParcels.Paper.Model.Y = 0;
                }));
                string savePathOfWord = InitalizeLandImageName(fileName, exportVp);
                //ExportContractLandParcelWord parcelWord = new ExportContractLandParcelWord(dbContext);
                //parcelWord.ViewOfAllMultiParcel = viewOfAllMultiParcel;
                //parcelWord.ViewOfNeighorParcels = viewOfNeighorParcels;
                //parcelWord.CurrentZone = currentZone;
                //parcelWord.VillageZone = VillageZone;
                //parcelWord.DictList = vpExportArgs.DicList;
                //parcelWord.SavePathOfImage = savePathOfImage;
                //parcelWord.SavePathOfWord = savePathOfWord;
                //parcelWord.DictDKLB = dictDKLB;
                //parcelWord.ListGeoLand = listGeoLand;
                //parcelWord.ListLineFeature = listLine;
                //parcelWord.ListPointFeature = listPoint;
                //parcelWord.ListPolygonFeature = listPolygon;
                //parcelWord.ListValidDot = listValidDot;
                //parcelWord.ParcelCheckDate = DateTime.Now;
                //parcelWord.ParcelDrawDate = DateTime.Now;
                //parcelWord.OpenTemplate(templatePath);

                var listConcord = concordStation.GetByZoneCode(currentZone.FullCode);
                var listBook = bookStation.GetByZoneCode(currentZone.FullCode, eSearchOption.Precision);
                var listTissue = landStation.GetTissuesByConcord(currentZone);

                var parcelWord = new ExportContractLandParcelWordLZ(dbContext);
                parcelWord.ViewOfAllMultiParcel = viewOfAllMultiParcel;
                parcelWord.ViewOfNeighorParcels = viewOfNeighorParcels;
                parcelWord.CurrentZone = currentZone;
                parcelWord.SavePathOfImage = savePathOfImage + @"\";
                parcelWord.SavePathOfWord = savePathOfWord;
                parcelWord.DictList = vpExportArgs.DicList; ;
                parcelWord.DictDKLB = dictDKLB;
                parcelWord.ListGeoLand = listGeoLand;
                parcelWord.ListLineFeature = listLine;
                parcelWord.ListConcord = listConcord;
                parcelWord.ListBook = listBook;
                parcelWord.ListTissue = listTissue;
                parcelWord.ListDot = listDot;
                parcelWord.ListValidDot = listValidDot;
                parcelWord.ListCoil = listCoil;
                parcelWord.ParcelCheckDate = DateTime.Now;
                parcelWord.ParcelDrawDate = DateTime.Now;
                parcelWord.SettingDefine = ContractBusinessParcelWordSettingDefine.GetIntence();
                parcelWord.OpenTemplate(templatePath);
                parcelWord.SaveAsMultiFile(exportVp, savePathOfWord, ParcelWordSettingDefine.SaveParcelPCAsPDF);
                result = true;
            }
            catch (Exception exx)
            {
                this.ReportWarn(string.Format("导出{0}的地块示意图.发生错误:{2}", descZone, exx.ToString()));
                result = false;
                return result;
            }
            finally
            {

                listLine.Clear();
                listLine = null;
                dictDKLB.Clear();
                dictDKLB = null;
                listDot.Clear();
                listDot = null;
                listCoil.Clear();
                listCoil = null;
                listValidDot.Clear();
                listValidDot = null;

                Application.Current.Dispatcher.Invoke(new Action(() =>
                {
                    viewOfAllMultiParcel.Dispose();
                    viewOfAllMultiParcel = null;
                    viewOfNeighorParcels.Dispose();
                    viewOfNeighorParcels = null;
                }));

                GC.Collect();
                GC.WaitForPendingFinalizers();
                GC.Collect();
            }

            return result;

        }

        #endregion

        #region 登记薄

        /// <summary>
        /// 导出登记薄
        /// </summary>       
        private bool ExportRegeditBook(VirtualPerson exportVp, ALLValidateArgument vpExportArgs)
        {
            var args = Argument as TaskDataSummaryExportArgument;
            bool result = true;
            List<Dictionary> dictCBFS = args.DictList.FindAll(c => c.GroupCode == DictionaryTypeInfo.CBJYQQDFS);
            var zonelist = GetParentZone(currentZone, dbContext);

            foreach (var concord in vpExportArgs.zoneConcords)
            {
                if (!vpExportArgs.zoneContractRegeditBooks.Exists(c => !string.IsNullOrEmpty(c.RegeditNumber) && c.RegeditNumber == concord.ConcordNumber))
                    continue;
                string typeString = dictCBFS.Find(c => c.Code == concord.ArableLandType).Name;
                IDbContext dbContext = DataBaseSource.GetDataBaseSource();
                List<XZDW> listLine = new List<XZDW>(1000);
                List<DZDW> listPoint = new List<DZDW>(1000);
                List<MZDW> listPolygon = new List<MZDW>(1000);
                List<ContractLand> listGeoLand = new List<ContractLand>(1000);
                List<CollectivityTissue> listTissue = new List<CollectivityTissue>(1000);
                List<Dictionary> dictDKLB = new List<Dictionary>(1000);
                List<BuildLandBoundaryAddressDot> listDot = new List<BuildLandBoundaryAddressDot>(10000);
                List<BuildLandBoundaryAddressCoil> listCoil = new List<BuildLandBoundaryAddressCoil>(10000);
                List<BuildLandBoundaryAddressDot> listValidDot = new List<BuildLandBoundaryAddressDot>(10000);
                DiagramsView viewOfAllMultiParcel = null;
                var concordStation = dbContext.CreateConcordStation();
                var bookStation = dbContext.CreateRegeditBookStation();
                var senderStation = dbContext.CreateSenderWorkStation();
                var landStation = dbContext.CreateContractLandWorkstation();
                var dotStation = dbContext.CreateBoundaryAddressDotWorkStation();
                var coilStation = dbContext.CreateBoundaryAddressCoilWorkStation();
                var lineStation = dbContext.CreateXZDWWorkStation();
                var PointStation = dbContext.CreateDZDWWorkStation();
                var PolygonStation = dbContext.CreateMZDWWorkStation();
                var dicStation = dbContext.CreateDictWorkStation();
                var VillageZone = GetParent(currentZone, dbContext);
                var listDict = dicStation.Get();
                var listLand = landStation.GetCollection(currentZone.FullCode, eLevelOption.Self);
                dictDKLB = listDict.FindAll(c => !string.IsNullOrEmpty(c.GroupCode) && c.GroupCode == DictionaryTypeInfo.DKLB);
                listCoil = coilStation.GetByZoneCode(currentZone.FullCode, eLevelOption.Self);
                listDot = dotStation.GetByZoneCode(currentZone.FullCode, eLevelOption.Self);
                listValidDot = listDot.FindAll(c => c.IsValid == true);
                listGeoLand = listLand.FindAll(c => c.Shape != null);
                listLine = lineStation.GetByZoneCode(currentZone.FullCode);
                listPoint = PointStation.GetByZoneCode(currentZone.FullCode);
                listPolygon = PolygonStation.GetByZoneCode(currentZone.FullCode);
                listLine.RemoveAll(l => l.Shape == null);
                listPoint.RemoveAll(l => l.Shape == null);
                listPolygon.RemoveAll(l => l.Shape == null);
                Application.Current.Dispatcher.Invoke(new Action(() =>
                {
                    viewOfAllMultiParcel = new DiagramsView();
                    viewOfAllMultiParcel.Paper.Model.Width = 336;
                    viewOfAllMultiParcel.Paper.Model.Height = 357;
                    viewOfAllMultiParcel.Paper.Model.BorderWidth = 0;
                    viewOfAllMultiParcel.Paper.Model.X = 0;
                    viewOfAllMultiParcel.Paper.Model.Y = 0;
                }));
                ContractRegeditBookPrinterData data = new ContractRegeditBookPrinterData(concord);
                data.CurrentZone = currentZone;
                data.DbContext = dbContext;
                data.SystemDefine = systemSet;
                data.AccountLandBusiness = new AccountLandBusiness(dbContext); ;
                data.ConcordBusiness = new ConcordBusiness(dbContext);
                data.DictBusiness = new DictionaryBusiness(dbContext);
                data.PersonBusiness = new VirtualPersonBusiness(dbContext);
                data.SystemDefine = systemSet;
                data.InitializeInnerData();
                string tempPath = TemplateHelper.WordTemplate("农村土地承包经营权登记簿");
                ContractRegeditBookWork regeditBookWord = new ContractRegeditBookWork(dbContext);
                regeditBookWord.ViewOfAllMultiParcel = viewOfAllMultiParcel;
                regeditBookWord.CurrentZone = currentZone;
                regeditBookWord.VillageZone = VillageZone;
                regeditBookWord.DictList = listDict;
                regeditBookWord.DictDKLB = dictDKLB;
                regeditBookWord.ListGeoLand = listGeoLand;
                regeditBookWord.ListLineFeature = listLine;
                regeditBookWord.ListPointFeature = listPoint;
                regeditBookWord.ListPolygonFeature = listPolygon;
                regeditBookWord.DictList = vpExportArgs.DicList;
                regeditBookWord.SavePathOfImage = System.IO.Path.GetTempPath();
                regeditBookWord.OpenTemplate(tempPath);
                //string filePath = vpSavePath + @"\" + exportVp.FamilyNumber + "-" + concord.ContracterName.InitalizeFamilyName() + "-" + concord.ConcordNumber + "-" + TemplateFile.PrivewRegeditBookWord;
                string filePath = vpSavePath + @"\" + concord.ContracterName.InitalizeFamilyName() + "-" + concord.ConcordNumber + "-" + "登记簿";
                regeditBookWord.SaveAs(data, filePath);
                data = null;
                this.ReportInfomation(string.Format("导出{0}{1}的登记簿" + "(" + typeString + ")", descZone, concord.ContracterName));
            }
            return result;
        }

        #endregion

        #region 地块调查表

        /// <summary>
        ///按户导出地块调查表word
        /// </summary>       
        private bool ExportLandSurvyTable(VirtualPerson exportVp, ALLValidateArgument vpExportArgs)
        {
            XiZangUserControlSettingDefine xzSetting = XiZangUserControlSettingDefine.GetIntence();
            bool result = true;
            try
            {
                var args = Argument as TaskDataSummaryExportArgument;
                //判断是否显示集体信息-勾选常规设置后就设置为不显示
                if (args.SettingDefine.DisplayCollectUsingCBdata)
                {
                    if (exportVp.Name.IndexOf("机动地") >= 0 || exportVp.Name.IndexOf("集体") >= 0)
                    {
                        result = false;
                        this.ReportInfomation(string.Format("根据系统设置，不显示导出{0}{1}的地块调查表", descZone, exportVp.Name));
                        return result;
                    }
                }
                string zoneName = GetUinitName(currentZone);
                if (args.SystemSet.CountryTableHead)
                {
                    var zoneStation = dbContext.CreateZoneWorkStation();
                    zoneName = zoneStation.GetVillageName(currentZone);
                }
                List<ContractLand> landArrays = vpExportArgs.zoneContractLands.Clone() as List<ContractLand>;
                landArrays.LandNumberFormat(args.SystemSet);
                string markDesc = GetMarkDesc(currentZone, dbContext);
                if (landArrays == null || landArrays.Count == 0)
                {
                    this.ReportWarn(string.Format("{0}未获取承包地块!", markDesc + exportVp.Name));
                }
                ExportLandSurveyWordTable export = new ExportLandSurveyWordTable();
                string tempPath = TemplateHelper.WordTemplate("西藏" + TemplateFile.ContractAccountLandSurveyWord);  //模板文件
                export.Contractor = exportVp;
                export.DictList = vpExportArgs.DicList == null ? new List<Dictionary>() : vpExportArgs.DicList;
                export.CurrentZone = currentZone;
                export.Tissue = vpExportArgs.tissue;
                export.LandCollection = landArrays;
                export.Concord = new ContractConcord();
                export.OpenTemplate(tempPath);
                string filePath = vpSavePath + @"\" + "地块调查表";
                export.SaveAs(exportVp, filePath);
                string strDesc = markDesc + exportVp.Name;
            }
            catch (Exception ex)
            {
                this.ReportError(ex.Message);
                YuLinTu.Library.Log.Log.WriteException(this, "ExportDataExcel(导出数据到Excel表)", ex.Message + ex.StackTrace);
            }
            return result;
        }

        #endregion

        #endregion

        #region Methods-Helper

        /// <summary>
        /// 导出单户申请书-调用
        /// </summary>
        private bool SingleExportRequireBook(Zone zone, ALLValidateArgument vpExportArgs, VirtualPerson vp, string templemPath, eConstructMode typeMode)
        {
            try
            {
                var args = Argument as TaskDataSummaryExportArgument;
                var zoneStation = dbContext.CreateZoneWorkStation();
                string tempPath = TemplateHelper.WordTemplate(templemPath);
                ExportSingleRequireWord printBook = new ExportSingleRequireWord(vp);
                printBook.CurrentZone = zone;
                printBook.ConstructMode = typeMode;
                printBook.RequireDate = args.PublishDateSetting == null ? DateTime.Now : args.PublishDateSetting.PublishStartDate;
                printBook.CheckDate = args.PublishDateSetting == null ? DateTime.Now : args.PublishDateSetting.PublishEndDate;
                printBook.DictList = args.DictList;
                printBook.Concords = vpExportArgs.zoneConcords.Clone() as List<ContractConcord>;
                ContractConcord concord = null;
                var useconcord = printBook.InitalizeConcord(concord);
                if (useconcord == null)
                {
                    string warnNoConcordInfo = typeMode == eConstructMode.Family ? string.Format("当前承包方{0}没有签订家庭承包方式合同!", vp.Name) : string.Format("当前承包方{0}没有签订非家庭承包方式合同!", vp.Name);
                    this.ReportInfomation(warnNoConcordInfo);
                    return false;
                }
                List<ContractLand> landArrays = vpExportArgs.zoneContractLands.Clone() as List<ContractLand>;
                landArrays.LandNumberFormat(args.SystemSet);
                printBook.ConcordLands = landArrays.FindAll(ld => ld.ConcordId == useconcord.ID);
                printBook.Lands = landArrays;
                List<ContractLand> landuse = new List<ContractLand>();
                if (typeMode != eConstructMode.Family)
                {
                    landuse = printBook.Lands.FindAll(l => l.ConstructMode != ((int)eConstructMode.Family).ToString());
                }
                else
                {
                    landuse = printBook.Lands.FindAll(l => l.ConstructMode == ((int)eConstructMode.Family).ToString());
                }
                if (landuse.Count == 0)
                {
                    string warnNoLandInfo = typeMode == eConstructMode.Family ? string.Format("当前承包方{0}没有家庭承包方式的地块!", vp.Name) : string.Format("当前承包方{0}没有非家庭承包方式的地块!", vp.Name);
                    this.ReportInfomation(warnNoLandInfo);
                    return false;
                }
                List<Zone> listZone = GetParentZone(currentZone);
                printBook.CurrentZoneCounty = zoneStation.Get(vp.ZoneCode.Substring(0, Zone.ZONE_COUNTY_LENGTH));
                printBook.CurrentZoneProvince = zoneStation.Get(vp.ZoneCode.Substring(0, Zone.ZONE_PROVICE_LENGTH));
                printBook.CurrentZoneTown = zoneStation.Get(vp.ZoneCode.Substring(0, Zone.ZONE_TOWN_LENGTH));
                printBook.CurrentZoneCity = zoneStation.Get(vp.ZoneCode.Substring(0, Zone.ZONE_CITY_LENGTH));
                printBook.CurrentZoneVillage = zoneStation.Get(vp.ZoneCode.Substring(0, Zone.ZONE_VILLAGE_LENGTH));
                printBook.CurrentZoneGroup = zoneStation.Get(vp.ZoneCode);
                printBook.ZoneList = listZone;
                printBook.OpenTemplate(tempPath);
                string filePath = null;
                if (typeMode == eConstructMode.Family)
                {
                    filePath = vpSavePath + @"\" + "登记申请书(家庭承包)";
                }
                else
                {
                    filePath = vpSavePath + @"\" + "登记申请书(非家庭承包)";
                }
                printBook.SaveAs(vp, filePath);

                GC.Collect();
                return true;
            }
            catch (Exception ex)
            {
                this.ReportError(ex.Message);
                YuLinTu.Library.Log.Log.WriteException(this, "ExportDataWord(导出数据到word表)", ex.Message + ex.StackTrace);
                return false;
            }
        }

        /// <summary>
        /// 获取地域集合
        /// </summary>
        public List<Zone> GetParentZone(Zone zone)
        {
            ModuleMsgArgs arg = new ModuleMsgArgs();
            arg.Datasource = dbContext;
            arg.Parameter = zone;
            arg.Name = ZoneMessage.ZONE_PARENTS_ZONE;
            TheBns.Current.Message.Send(this, arg);
            return (arg.ReturnValue as List<Zone>);
        }

        /// <summary>    
        /// 获取地域集合
        /// </summary>
        public List<Zone> GetParentZone(Zone zone, IDbContext dbContext)
        {
            ModuleMsgArgs arg = new ModuleMsgArgs();
            arg.Datasource = dbContext;
            arg.Parameter = zone;
            arg.Name = ZoneMessage.ZONE_PARENTS_TOPROVINCEZONE;
            TheBns.Current.Message.Send(this, arg);
            return (arg.ReturnValue as List<Zone>);
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
        /// 初始化权属类型
        /// </summary>
        private string InitalizeRightType(eVirtualType virtualType)
        {
            string templateName = "农村土地承包经营权";
            switch (virtualType)
            {
                case eVirtualType.Land:
                    templateName = "农村土地承包经营权";
                    break;
                case eVirtualType.Yard:
                    templateName = "集体建设用地使用权";
                    break;
                case eVirtualType.House:
                    templateName = "房屋所有权";
                    break;
                case eVirtualType.Wood:
                    templateName = "林权";
                    break;
                default:
                    break;
            }
            return templateName;
        }

        /// <summary>
        /// 获取父级地域集合(县级)
        /// </summary>
        private List<Zone> GetParents(Zone zone, IDbContext dbContext)
        {
            List<Zone> zoneList = new List<Zone>();
            if (zone == null || (zone != null && zone.Level >= eZoneLevel.County) || dbContext == null)
            {
                return zoneList;
            }
            try
            {
                var zoneStation = dbContext.CreateZoneWorkStation();
                Zone temZone = zoneStation.Get(zone.UpLevelCode);
                zoneList.Add(temZone);
                while (temZone.Level < eZoneLevel.County)
                {
                    temZone = zoneStation.Get(temZone.UpLevelCode);
                    zoneList.Add(temZone);
                }
            }
            catch (Exception ex)
            {
                YuLinTu.Library.Log.Log.WriteException(this, "GetParents(获取父级地域集合)", ex.Message + ex.StackTrace);
            }
            return zoneList;
        }

        /// <summary>
        /// 进度提示用，导出时获取当前地域的上级地域名称路径到镇级
        /// </summary>       
        private string ExportZoneListDir(Zone zone, List<Zone> allZones)
        {
            string exportzonedir = string.Empty;
            if (zone.Level == eZoneLevel.Group)
            {
                Zone vzone = allZones.Find(t => t.FullCode == zone.UpLevelCode);
                Zone tzone = allZones.Find(t => t.FullCode == vzone.UpLevelCode);
                exportzonedir = tzone.Name + vzone.Name + zone.Name;
            }
            if (zone.Level == eZoneLevel.Village)
            {
                Zone tzone = allZones.Find(t => t.FullCode == zone.UpLevelCode);
                exportzonedir = tzone.Name + zone.Name;
            }
            return exportzonedir;
        }

        /// <summary>
        /// 初始化地块示意图名称
        /// </summary>
        /// <returns></returns>
        private string InitalizeLandImageName(string filePath, VirtualPerson family)
        {
            if (family == null)
            {
                return "";
            }
            string imagePath = vpSavePath + "\\" + "地块示意图";
            if (!Directory.Exists(imagePath))
            {
                Directory.CreateDirectory(imagePath);
            }
            string imageName = imagePath + "\\" + "DKSYT" + family.ZoneCode;
            int number = 0;
            Int32.TryParse(family.FamilyNumber, out number);
            imageName += string.Format("{0:D4}", number);
            imageName += "J";
            return imageName;
        }

        /// <summary>
        /// 获取单位名称
        /// </summary>
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

        /// <summary>
        /// 根据地域编码与级别获取名称
        /// </summary> 
        private string GetZoneNameByLevel(string zoneCode, eZoneLevel level, YuLinTu.Library.WorkStation.IZoneWorkStation zoneStation)
        {
            Zone temp = zoneStation.Get(c => c.FullCode == zoneCode).FirstOrDefault();
            if (temp == null)
                return " ";
            if (temp.Level == level)
                return temp.Name;
            else
                return GetZoneNameByLevel(temp.UpLevelCode, level, zoneStation);
        }


        #endregion

        #endregion

        #endregion
    }

    /// <summary>
    /// 当前地域下导出类需要的所有参数，包括人、地块、权证、合同四类数据
    /// </summary>
    class ALLValidateArgument
    {
        public List<VirtualPerson> zonevps { set; get; }
        public List<ContractConcord> zoneConcords { set; get; }
        public List<YuLinTu.Library.Entity.ContractRegeditBook> zoneContractRegeditBooks { set; get; }
        public List<ContractLand> zoneContractLands { set; get; }
        public CollectivityTissue tissue { set; get; }
        public List<Dictionary> DicList { get; set; }
    }

    #endregion
}
