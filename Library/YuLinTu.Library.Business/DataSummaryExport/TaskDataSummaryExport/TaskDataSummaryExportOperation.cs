/*
 * (C) 2025  鱼鳞图公司版权所有,保留所有权利
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
using YuLinTu.Data.Shapefile;
using YuLinTu.Library.Office;
using YuLinTu.Diagrams;
using YuLinTu.Library.WorkStation;
using YuLinTu.Windows;
using System.Windows.Controls.Primitives;

namespace YuLinTu.Library.Business
{
    /// <summary>
    /// 导出汇总数据操作
    /// </summary>
    public class TaskDataSummaryExportOperation : Task
    {
        #region Fields

        private Zone currentZone = null;//当前地域

        //private List<Zone> allZones = null;//当前地域，村、镇
        private IDbContext dbContext = null;

        private ALLValidateArgument currentZoneExportArgs;//当前地域下需要导出的人地合同权证
        private string vpSavePath = null;//当前承包方保存路径
        private string descZone = null;//到镇的描述
        private ContractBusinessParcelWordSettingDefine ParcelWordSettingDefine = ContractBusinessParcelWordSettingDefine.GetIntence();
        private ContractConcordSettingDefine concordSet = ContractConcordSettingDefine.GetIntence();

        private SystemSetDefine SystemSet
        {
            get { return SystemSetDefine.GetIntence(); }
        }

        #endregion Fields

        #region Properties

        /// <summary>
        /// 数据字典
        /// </summary>
        public List<Dictionary> DictList
        {
            get
            {
                DictionaryBusiness dictBusiness = new DictionaryBusiness(dbContext);
                List<Dictionary> dictList = dictBusiness.GetAll();
                return dictList;
            }
        }

        #endregion Properties

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
            bool res = SelectExportMode(args);
            if (res)
            {
                this.ReportProgress(100, "导出完成");
                this.ReportAlert(eMessageGrade.Infomation, null, "导出完成");
            }
        }

        /// <summary>
        /// 结束释放
        /// </summary>
        protected override void OnEnded()
        {
            currentZoneExportArgs = null;
        }

        #endregion Methods - Override

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

            List<ContractConcord> concordCollection = concordStation.GetByZoneCode(currentZone.FullCode);
            List<ContractRegeditBook> bookCollection = contractRegeditBookStation.GetByZoneCode(currentZone.FullCode, eSearchOption.Precision);
            List<VirtualPerson> familyCollection = VirtualPersonStation.GetByZoneCode(currentZone.FullCode);
            List<ContractLand> AllLandCollection = contractLandWorkStation.GetCollection(currentZone.FullCode, eLevelOption.Self);
            List<CollectivityTissue> tissues = senderStation.GetTissues(currentZone.FullCode, eLevelOption.Self);
            if (tissues != null && tissues.Count > 0)
                currentZoneExportArgs.tissue = tissues[0];

            currentZoneExportArgs.zonevps = familyCollection;
            currentZoneExportArgs.zoneConcords = concordCollection;
            currentZoneExportArgs.zoneContractLands = AllLandCollection;
            currentZoneExportArgs.zoneContractRegeditBooks = bookCollection;

            return true;
        }

        /// <summary>
        /// 导出数据模式，分多个人，单个人或分地域组导出
        /// </summary>
        private bool SelectExportMode(TaskDataSummaryExportArgument args)
        {
            List<VirtualPerson> exportVitualPersons = new List<VirtualPerson>();

            exportVitualPersons = args.SelectContractors;
            currentZoneExportArgs.zonevps = args.SelectContractors;
            if (exportVitualPersons == null || exportVitualPersons.Count == 0)
            {
                this.ReportAlert(eMessageGrade.Warn, null, currentZone.FullName + "下没有承包方。");
                this.ReportProgress(100);
                return true;
            }
            if (args.SelectContractors == null || args.SelectContractors.Count == 0)
            {
                this.ReportAlert(eMessageGrade.Warn, null, "请至少选择一个承包方进行汇总数据导出!");
                this.ReportProgress(100);
                return true;
            }
            if (currentZoneExportArgs.zoneContractLands != null && currentZoneExportArgs.zoneContractLands.Count != 0)
            {
                this.ReportInfomation(string.Format("导出{0}的承包地块调查表", currentZone.FullName));
                bool res = ExportZoneLandSurvyTable();
                if (res == false)
                {
                    return false;
                }
            }
            if (args.ExportDataSummaryTableTypes.ExportLandWordParcel)
            {
                this.ReportInfomation(string.Format("导出{0}的地块示意图", currentZone.FullName));
                bool res = ExportLandWordParcel(exportVitualPersons);
                if (res == false)
                {
                    return false;
                }
            }
            ALLValidateArgument vpExportArgs;//获取当前个人的信息
            int oldper = 0;
            int percentIndex = 1;
            this.ReportInfomation(string.Format("导出{0}的详细汇总文件", currentZone.FullName));
            foreach (var selectvp in exportVitualPersons)
            {
                if (percentIndex * 90 / exportVitualPersons.Count != oldper)
                {
                    this.ReportProgress(5 + oldper, "导出" + selectvp.Name + "的汇总文件");
                    oldper = percentIndex * 90 / exportVitualPersons.Count;
                }

                vpExportArgs = new ALLValidateArgument();
                vpExportArgs.zoneConcords = currentZoneExportArgs.zoneConcords.FindAll(c => c.ContracterId == selectvp.ID);
                vpExportArgs.zoneContractLands = currentZoneExportArgs.zoneContractLands.FindAll(ld => ld.OwnerId == selectvp.ID);
                List<ContractRegeditBook> crb = new List<ContractRegeditBook>();
                foreach (var cd in vpExportArgs.zoneConcords)
                {
                    ContractRegeditBook rbook = currentZoneExportArgs.zoneContractRegeditBooks.Find(rb => rb.ID == cd.ID);
                    if (rbook != null)
                        crb.Add(rbook);
                }
                vpExportArgs.zoneContractRegeditBooks = crb;
                vpExportArgs.tissue = currentZoneExportArgs.tissue;

                if (!Directory.Exists(Path.GetDirectoryName(args.FileName + @"\" + selectvp.FamilyNumber + "-" + selectvp.Name)))
                    Directory.CreateDirectory(Path.GetDirectoryName(args.FileName + @"\" + selectvp.FamilyNumber + "-" + selectvp.Name));
                vpSavePath = args.FileName + @"\" + selectvp.FamilyNumber + "-" + selectvp.Name;
                bool res = ExportData(selectvp, vpExportArgs);//使用底层开始导出
                if (res == false) continue;
                percentIndex++;
            }

            CanOpenResult = true;
            return true;
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
        private bool ExportData(VirtualPerson exportVp, ALLValidateArgument vpExportArgs)
        {
            bool result = true;
            var args = Argument as TaskDataSummaryExportArgument;
            //根据选择的参数导出表格

            #region 导出个人表格集合

            try
            {
                if (args.ExportDataSummaryTableTypes.ExportContractConcord)
                {
                    result = ExportContractConcord(exportVp, vpExportArgs);
                    if (!result)
                    {
                        this.ReportInfomation(string.Format("未导出{0}的承包合同", exportVp.Name));
                    }
                }
                if (args.ExportDataSummaryTableTypes.ExportContractRegeditBook)
                {
                    result = ExportContractRegeditBook(exportVp, vpExportArgs);
                    if (!result)
                    {
                        this.ReportInfomation(string.Format("未导出{0}的承包权证", exportVp.Name));
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
                    result = ExportPublicDataWord(exportVp, vpExportArgs);
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
                if (args.ExportDataSummaryTableTypes.ExportSingleRequireBook)
                {
                    result = ExportSingleRequireBook(exportVp, vpExportArgs);
                }
                if (args.ExportDataSummaryTableTypes.ExportVPApplyBook)
                {
                    result = ExportVPApplyBook(exportVp, vpExportArgs);
                    if (!result)
                    {
                        this.ReportInfomation(string.Format("未导出{0}的户主声明书", exportVp.Name));
                    }
                }
                if (args.ExportDataSummaryTableTypes.ExportVPSurvyTable)
                {
                    result = ExportVPSurvyTable(exportVp, vpExportArgs);
                    if (!result)
                    {
                        this.ReportInfomation(string.Format("未导出{0}的承包方调查表", exportVp.Name));
                    }
                }
                if (args.ExportDataSummaryTableTypes.ExportVPDelegateBook)
                {
                    result = ExportVPDelegateBook(exportVp, vpExportArgs);
                    if (!result)
                    {
                        this.ReportInfomation(string.Format("未导出{0}的户主委托书", exportVp.Name));
                    }
                }
            }
            catch (Exception ex)
            {
                if (ex is TaskStopException)
                {
                    result = false;
                    return result;
                }
            }
            return result;

            #endregion 导出个人表格集合
        }

        #region Methods - exportHelper-单个文件导出底层

        /// <summary>
        /// 导出承包合同
        /// </summary>
        private bool ExportContractConcord(VirtualPerson exportVp, ALLValidateArgument vpExportArgs)
        {
            var args = Argument as TaskDataSummaryExportArgument;
            bool result = true;
            var personBusiness = new VirtualPersonBusiness(args.Database);
            personBusiness.VirtualType = eVirtualType.Land;
            List<Dictionary> dictCBFS = args.DictList.FindAll(c => c.GroupCode == DictionaryTypeInfo.CBJYQQDFS);
            if (vpExportArgs.zoneConcords == null || vpExportArgs.zoneConcords.Count <= 0)
            {
                this.ReportWarn(exportVp.Name + "没有对应的承包合同信息");
                result = false;
                return result;
            }

            foreach (var concord in vpExportArgs.zoneConcords)
            {
                try
                {
                    List<ContractLand> lands = vpExportArgs.zoneContractLands.Clone() as List<ContractLand>;
                    VirtualPerson person = exportVp;
                    string templatePath = TemplateHelper.WordTemplate(TemplateFile.ContractConcordWord);
                    string typeString = dictCBFS.Find(c => c.Code == concord.ArableLandType).Name;
                    string fullPath = vpSavePath + @"\" + "承包合同" + "(" + typeString + ")";
                    lands.LandNumberFormat(args.SystemSet);
                    ExportContractConcord exportConcord = new ExportContractConcord(dbContext);

                    #region 通过反射等机制定制化具体的业务处理类

                    var temp = WorksheetConfigHelper.GetInstance(exportConcord);
                    if (temp != null && temp.TemplatePath != null)
                    {
                        if (temp is ExportContractConcord)
                        {
                            exportConcord = (ExportContractConcord)temp;
                        }
                        templatePath = Path.Combine(TheApp.GetApplicationPath(), temp.TemplatePath);
                    }

                    #endregion 通过反射等机制定制化具体的业务处理类

                    exportConcord.CurrentZone = currentZone;
                    exportConcord.AreaType = concordSet.ChooseArea;
                    exportConcord.PersonBusiness = personBusiness;
                    exportConcord.ListLand = lands.FindAll(l => l.ConcordId == concord.ID);
                    exportConcord.VirtualPerson = person;
                    exportConcord.DictList = args.DictList;
                    exportConcord.OpenTemplate(templatePath);  //打开模板
                    exportConcord.SaveAs(concord, fullPath);
                    this.ReportInfomation(string.Format("导出{0}{1}的合同" + "(" + typeString + ")", descZone, concord.ContracterName));
                }
                catch (Exception ex)
                {
                    result = false;
                    if (ex is TaskStopException)
                    {
                        return result;
                    }
                    this.ReportError(ex.Message);
                    YuLinTu.Library.Log.Log.WriteException(this, "ExportConcordData(导出合同)", ex.Message + ex.StackTrace);
                    return result;
                }
            }

            return result;
        }

        /// <summary>
        /// 导出承包权证
        /// </summary>
        private bool ExportContractRegeditBook(VirtualPerson exportVp, ALLValidateArgument vpExportArgs)
        {
            bool result = true;
            var args = Argument as TaskDataSummaryExportArgument;
            string tempPath = TemplateHelper.WordTemplate(TemplateFile.ContractRegeditBookWord);
            if (vpExportArgs.zoneContractRegeditBooks == null || vpExportArgs.zoneContractRegeditBooks.Count <= 0)
            {
                this.ReportWarn("" + exportVp.Name + "承包权证无数据");
                return false;
            }
            try
            {
                List<Dictionary> dictCBFS = args.DictList.FindAll(c => c.GroupCode == DictionaryTypeInfo.CBJYQQDFS);
                var concords = vpExportArgs.zoneConcords;
                var familyConcord = concords.Find(c => c.ArableLandType == ((int)eConstructMode.Family).ToString());
                var familyBook = familyConcord == null ? null : vpExportArgs.zoneContractRegeditBooks.Find(c => c.ID == familyConcord.ID);
                var otherConcord = concords.Find(c => c.ArableLandType != ((int)eConstructMode.Family).ToString());
                var otherBook = otherConcord == null ? null : vpExportArgs.zoneContractRegeditBooks.Find(c => c.ID == otherConcord.ID);
                ContractWarrantPrinter printContract = new ContractWarrantPrinter();

                #region 通过反射等机制定制化具体的业务处理类

                var temp = WorksheetConfigHelper.GetInstance(printContract);
                if (temp != null && temp is ContractWarrantPrinter)
                {
                    printContract = (ContractWarrantPrinter)temp;
                    tempPath = Path.Combine(TheApp.GetApplicationPath(), temp.TemplatePath);
                }

                #endregion 通过反射等机制定制化具体的业务处理类

                printContract.dbContext = args.Database;
                printContract.CurrentZone = currentZone;
                printContract.RegeditBook = familyBook;
                printContract.Concord = familyConcord;
                printContract.OtherBook = otherBook;
                printContract.OtherConcord = otherConcord;
                printContract.LandCollection = vpExportArgs.zoneContractLands;
                printContract.IsDataSummaryExport = true;
                printContract.Contractor = exportVp;
                printContract.Tissue = vpExportArgs.tissue;
                printContract.DictList = args.DictList;
                printContract.TempleFilePath = tempPath;
                printContract.UseExcel = args.ExtendUseExcelDefine.WarrantExtendByExcel;
                printContract.BookPersonNum = args.BookPersonNum;
                printContract.BookLandNum = args.BookLandNum;
                printContract.ParentsToProvince = args.ParentsToProvince;
                printContract.ExportContractLand(vpSavePath);
                //printContract.SaveAs(vpSavePath);
                this.ReportInfomation(string.Format("导出{0}{1}的权证", descZone, exportVp.Name));
            }
            catch (Exception ex)
            {
                result = false;
                if (ex is TaskStopException)
                {
                    return result;
                }
                this.ReportError(ex.Message);
                YuLinTu.Library.Log.Log.WriteException(this, "ExportContractRegeditBook(导出权证)", ex.Message + ex.StackTrace);
                return result;
            }
            return result;

            #endregion Methods - exportHelper-单个文件导出底层
        }

        /// <summary>
        /// 导出登记薄
        /// </summary>
        private bool ExportRegeditBook(VirtualPerson exportVp, ALLValidateArgument vpExportArgs)
        {
            var args = Argument as TaskDataSummaryExportArgument;
            bool result = true;
            List<Dictionary> dictCBFS = args.DictList.FindAll(c => c.GroupCode == DictionaryTypeInfo.CBJYQQDFS);
            if (vpExportArgs.zoneContractRegeditBooks == null || vpExportArgs.zoneContractRegeditBooks.Count <= 0)
            {
                this.ReportWarn("" + exportVp.Name + "承包权证无数据");
                result = false;
            }
            var zonelist = GetParentZone(currentZone, dbContext);
            try
            {
                foreach (var concord in vpExportArgs.zoneConcords)
                {
                    if (!vpExportArgs.zoneContractRegeditBooks.Exists(c => !string.IsNullOrEmpty(c.RegeditNumber) && c.RegeditNumber == concord.ConcordNumber))
                        continue;

                    string typeString = dictCBFS.Find(c => c.Code == concord.ArableLandType).Name;
                    ContractRegeditBookPrinterData data = new ContractRegeditBookPrinterData(concord);
                    data.CurrentZone = currentZone;
                    data.DbContext = dbContext;
                    data.SystemDefine = args.SystemSet;
                    data.DictList = args.DictList;
                    data.InitializeInnerData();
                    string tempPath = TemplateHelper.WordTemplate(TemplateFile.PrivewRegeditBookWord);
                    ContractRegeditBookWork regeditBookWord = new ContractRegeditBookWork();

                    #region 通过反射等机制定制化具体的业务处理类

                    var temp = WorksheetConfigHelper.GetInstance(regeditBookWord);
                    if (temp != null && temp.TemplatePath != null)
                    {
                        if (temp is ContractRegeditBookWork)
                        {
                            regeditBookWord = (ContractRegeditBookWork)temp;
                        }
                        tempPath = Path.Combine(TheApp.GetApplicationPath(), temp.TemplatePath);
                    }

                    #endregion 通过反射等机制定制化具体的业务处理类

                    regeditBookWord.DictList = args.DictList;
                    regeditBookWord.ZoneList = zonelist;
                    regeditBookWord.Tissue = data.Tissue;
                    regeditBookWord.OpenTemplate(tempPath);
                    string filePath = vpSavePath + @"\" + "登记簿" + "(" + typeString + ")";
                    regeditBookWord.SaveAs(data, filePath);
                    this.ReportInfomation(string.Format("导出{0}{1}的登记簿" + "(" + typeString + ")", descZone, concord.ContracterName));
                }
            }
            catch (Exception ex)
            {
                result = false;
                if (ex is TaskStopException)
                {
                    return result;
                }
                this.ReportError(ex.Message);
                YuLinTu.Library.Log.Log.WriteException(this, "ExportRegeditBook(导出登记簿)", ex.Message + ex.StackTrace);
                return result;
            }
            return result;
        }

        /// <summary>
        ///按组导出地块调查表word
        /// </summary>
        private bool ExportLandSurvyTable(VirtualPerson exportVp, ALLValidateArgument vpExportArgs)
        {
            bool result = true;
            try
            {
                var args = Argument as TaskDataSummaryExportArgument;

                //判断是否显示集体信息-勾选常规设置后就设置为不显示
                if (args.SettingDefine.DisplayCollectUsingCBdata)
                {
                    if (exportVp.FamilyExpand.ContractorType != eContractorType.Farmer)
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

                List<ContractLand> landArrays = vpExportArgs.zoneContractLands;
                landArrays.LandNumberFormat(args.SystemSet);

                var listConcords = vpExportArgs.zoneConcords;
                var listBooks = vpExportArgs.zoneContractRegeditBooks;
                var dotStation = dbContext.CreateBoundaryAddressDotWorkStation();
                var coilStation = dbContext.CreateBoundaryAddressCoilWorkStation();
                var listCoil = coilStation.GetByZoneCode(currentZone.FullCode, eLevelOption.Self);
                var listDot = dotStation.GetByZoneCode(currentZone.FullCode, eLevelOption.Self);

                ContractConcord concord = null;
                List<BuildLandBoundaryAddressCoil> listLandCoil = null;
                List<BuildLandBoundaryAddressDot> listLandDot = null;
                List<BuildLandBoundaryAddressDot> listValidLandDot = null;
                int dotCount = 0;

                if (!Directory.Exists(vpSavePath))// + @"\" + "地块调查表"))
                    Directory.CreateDirectory(vpSavePath); // + @"\" + "地块调查表");

                var savePath = vpSavePath;// + @"\" + "地块调查表";

                var senderStation = dbContext.CreateSenderWorkStation();
                if (args.SystemSet.ExportTableSenderDesToVillage && currentZone.Level == eZoneLevel.Group)
                {
                    var Senders = senderStation.GetTissues(currentZone.UpLevelCode, eLevelOption.Self);
                    if (Senders.Count > 0)
                    {
                        vpExportArgs.tissue = Senders[0];
                    }
                }

                ExportLandSurveyWordTable exportSpecial = new ExportLandSurveyWordTable();
                bool isNeedSpecial = IsNeedSpecialHandle(out exportSpecial);
                if (!isNeedSpecial)
                {
                    foreach (var land in landArrays)
                    {
                        concord = (land.ConcordId != null && land.ConcordId.HasValue) ? listConcords.Find(c => c.ID == (Guid)land.ConcordId) : null;
                        listLandCoil = listCoil == null ? new List<BuildLandBoundaryAddressCoil>() : listCoil.FindAll(c => c.LandID == land.ID);
                        listLandDot = listDot == null ? new List<BuildLandBoundaryAddressDot>() : listDot.FindAll(c => c.LandID == land.ID);
                        listValidLandDot = listLandDot == null ? new List<BuildLandBoundaryAddressDot>() : listLandDot.FindAll(c => c.IsValid == true);

                        ExportLandSurveyWordTable export = new ExportLandSurveyWordTable();

                        dotCount = listValidLandDot.Count == 0 ? (listLandDot == null ? 0 : listLandDot.Count) : (listValidLandDot.Count);
                        string tempPath = TemplateHelper.WordTemplate(TemplateFile.ContractAccountLandSurveyWord);  //模板文件
                        if (dotCount > 6 && dotCount <= 21)
                        {
                            tempPath = TemplateHelper.WordTemplate(TemplateFile.ContractAccountLandSurveyWordTwo);  //模板文件2页
                        }
                        else if (dotCount > 21 && dotCount <= 36)
                        {
                            tempPath = TemplateHelper.WordTemplate(TemplateFile.ContractAccountLandSurveyWordThree);  //模板文件3页
                        }
                        else if (dotCount > 36)
                        {
                            tempPath = TemplateHelper.WordTemplate(TemplateFile.ContractAccountLandSurveyWordOther);  //模板文件其它
                        }

                        #region 通过反射等机制定制化具体的业务处理类

                        var temp = WorksheetConfigHelper.GetInstance(export);
                        if (temp != null && temp.TemplatePath != null)
                        {
                            if (temp is ExportLandSurveyWordTable)
                            {
                                export = (ExportLandSurveyWordTable)temp;
                            }
                            tempPath = Path.Combine(TheApp.GetApplicationPath(), temp.TemplatePath);
                        }

                        #endregion 通过反射等机制定制化具体的业务处理类

                        export.Contractor = exportVp;
                        export.DictList = args.DictList == null ? new List<Dictionary>() : args.DictList;
                        export.CurrentZone = currentZone;
                        export.Tissue = vpExportArgs.tissue;
                        export.Concord = concord;
                        export.ListLandCoil = listLandCoil;
                        export.ListLandDot = listLandDot;
                        export.ListLandValidDot = listValidLandDot;
                        export.OpenTemplate(tempPath);
                        string landNumber = land.LandNumber.Length > 5 ? land.LandNumber.Substring(land.LandNumber.Length - 5) : land.LandNumber;
                        string filePath = savePath + @"\" + landNumber + "-" + "地块调查表";
                        export.SaveAs(land, filePath);
                        this.ReportInfomation(string.Format("导出{0}{1}的地块调查表，地块编码为{2}", descZone, exportVp.Name, landNumber));
                    }
                }
                else
                {
                    if (exportSpecial == null)
                        return result;

                    exportSpecial.Contractor = exportVp;
                    exportSpecial.DictList = args.DictList == null ? new List<Dictionary>() : args.DictList;
                    exportSpecial.CurrentZone = currentZone;
                    exportSpecial.Tissue = vpExportArgs.tissue;

                    //exportSpecial.SpecialData = landsOfFamily.ToArray<ContractLand>();

                    int totalDot = 0;
                    foreach (var land in landArrays)
                    {
                        listLandDot = listDot == null ? new List<BuildLandBoundaryAddressDot>() : listDot.FindAll(c => c.LandID == land.ID);
                        listValidLandDot = listLandDot == null ? new List<BuildLandBoundaryAddressDot>() : listLandDot.FindAll(c => c.IsValid == true);
                        totalDot += listValidLandDot.Count + 1;
                    }

                    string dir = Path.GetDirectoryName(exportSpecial.TemplatePath);
                    string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(exportSpecial.TemplatePath);
                    string extension = Path.GetExtension(exportSpecial.TemplatePath);
                    if (totalDot > 25 && totalDot < 56)
                    {
                        exportSpecial.TemplatePath = Path.Combine(dir, fileNameWithoutExtension + "2页" + extension);
                    }

                    string tempPath = Path.Combine(TheApp.GetApplicationPath(), exportSpecial.TemplatePath);
                    exportSpecial.OpenTemplate(tempPath);
                    string filePath = savePath + @"\" + exportVp.Name + "-" + TemplateFile.ContractAccountLandSurveyWord;

                    // 2017/9/21—湖南工单，定制化，传入单个家庭下的所有地块作为数据
                    exportSpecial.SaveAs(landArrays, filePath);
                    string strDesc = string.Format("{0}", descZone + exportVp.Name);
                    this.ReportInfomation(strDesc);
                }
            }
            catch (Exception ex)
            {
                result = false;
                if (ex is TaskStopException)
                {
                    return result;
                }
                this.ReportError(ex.Message);
                YuLinTu.Library.Log.Log.WriteException(this, "ExportDataExcel(导出数据到Excel表)", ex.Message + ex.StackTrace);
                return result;
            }

            return result;
        }

        /// <summary>
        /// 是否需要特殊处理
        /// </summary>
        /// <param name="special"></param>
        /// <returns></returns>
        private bool IsNeedSpecialHandle(out ExportLandSurveyWordTable special)
        {
            bool result = false;
            special = new ExportLandSurveyWordTable();

            #region 通过反射等机制定制化具体的业务处理类

            var temp = WorksheetConfigHelper.GetInstance(special);
            if (temp != null && temp.TemplatePath != null)
            {
                if (temp is ExportLandSurveyWordTable)
                    special = (ExportLandSurveyWordTable)temp;
                if (!(bool)temp.Tag)
                    return result;
                result = true;
                //tempPath = Path.Combine(TheApp.GetApplicationPath(), temp.TemplatePath);
            }

            #endregion 通过反射等机制定制化具体的业务处理类

            return result;
        }

        /// <summary>
        /// 导出地块示意图
        /// </summary>
        private bool ExportLandWordParcel(List<VirtualPerson> exportVps)
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
            List<CollectivityTissue> listTissue = new List<CollectivityTissue>(1000);
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
            string templatePath = TemplateHelper.WordTemplate(TemplateFile.ParcelWord);
            string savePathOfImage = fileName;
            try
            {
                listLand = currentZoneExportArgs.zoneContractLands;
                var VillageZone = zoneStation.Get(currentZone.UpLevelCode);
                var listGeoLands = listLand.FindAll(c => c.Shape != null);
                listGeoLand = InitalizeAgricultureLandSortValue(listGeoLands);
                if (listGeoLand.Count == 0)
                {
                    this.ReportInfomation("当前地域" + currentZone.FullName + "没有地块进行导出，请检查配置选项!");
                }
                listLine = lineStation.GetByZoneCode(currentZone.FullCode);
                dictDKLB = DictList.FindAll(c => !string.IsNullOrEmpty(c.GroupCode) && c.GroupCode == DictionaryTypeInfo.DKLB);

                listDot = dotStation.GetByZoneCode(currentZone.FullCode, eLevelOption.Self);
                listValidDot = listDot.FindAll(c => c.IsValid == true);
                listPoint = PointStation.GetByZoneCode(currentZone.FullCode);
                listPolygon = PolygonStation.GetByZoneCode(currentZone.FullCode);
                if (listTissue.Count == 0)
                {
                    var senderStation = dbContext.CreateSenderWorkStation();
                    var tissue = senderStation.Get(currentZone.ID);

                    if (tissue == null)
                    {
                        listTissue = senderStation.GetTissues(currentZone.FullCode, eLevelOption.Self);
                    }
                    else
                    {
                        listTissue.Add(tissue);
                    }
                }
                Application.Current.Dispatcher.Invoke(new Action(() =>
                {
                    viewOfAllMultiParcel.Paper.Model.Width = 236;
                    viewOfAllMultiParcel.Paper.Model.Height = 217;
                    viewOfAllMultiParcel.Paper.Model.BorderWidth = 0;
                    viewOfAllMultiParcel.Paper.Model.X = 0;
                    viewOfAllMultiParcel.Paper.Model.Y = 0;
                }));

                Application.Current.Dispatcher.Invoke(new Action(() =>
                {
                    viewOfNeighorParcels.Paper.Model.Width = 160;
                    viewOfNeighorParcels.Paper.Model.Height = 160;
                    viewOfNeighorParcels.Paper.Model.BorderWidth = 0;
                    viewOfNeighorParcels.Paper.Model.X = 0;
                    viewOfNeighorParcels.Paper.Model.Y = 0;
                }));

                listGeoLand.RemoveAll(t => !exportVps.Any(v => v.ID == t.OwnerId));

                int indexOfVp = 1;
                int count = 0;   //统计导出宗地图表个数
                double vpPercent = 99 / (double)exportVps.Count;
                foreach (var person in exportVps)
                {
                    // 排除掉没有地块的户
                    if (!listGeoLand.Any(t => t.OwnerId == person.ID))
                    {
                        // 当通过Person的ID到“权属关系”里找的地块仍然不在地块集合里，则此户应排除
                        var belongRelationStation = dbContext.CreateBelongRelationWorkStation();
                        var landList = belongRelationStation.GetLandByPerson(person.ID, person.ZoneCode);
                        bool flag = false;
                        foreach (var land in landList)
                        {
                            if (listGeoLand.Any(c => c.ID.Equals(land.ID)))
                            {
                                flag = true;
                                break;
                            }
                        }
                        if (flag == false)
                            continue;
                    }

                    string savePathOfWord = InitalizeLandImageName(fileName, person);

                    //savePathOfImage = InitalizeLandImageName(fileName, person);
                    savePathOfWord = args.FileName + @"\" + person.FamilyNumber + "-" + person.Name;
                    Directory.CreateDirectory(savePathOfWord);
                    savePathOfWord = Path.Combine(savePathOfWord, Path.GetFileName(InitalizeLandImageName(fileName, person)));
                    var parcelWord = new ExportContractLandParcelWord(dbContext);

                    #region 通过反射等机制定制化具体的业务处理类

                    var temp = WorksheetConfigHelper.GetInstance(parcelWord);
                    if (temp != null && temp.TemplatePath != null)
                    {
                        if (temp is ExportContractLandParcelWord)
                        {
                            parcelWord = (ExportContractLandParcelWord)temp;
                        }
                        templatePath = Path.Combine(TheApp.GetApplicationPath(), temp.TemplatePath);
                    }

                    #endregion 通过反射等机制定制化具体的业务处理类

                    parcelWord.ViewOfAllMultiParcel = viewOfAllMultiParcel;
                    parcelWord.ViewOfNeighorParcels = viewOfNeighorParcels;
                    parcelWord.CurrentZone = currentZone;
                    parcelWord.VillageZone = VillageZone;
                    parcelWord.DictList = DictList;
                    parcelWord.SavePathOfImage = savePathOfImage;
                    parcelWord.SavePathOfWord = savePathOfWord;
                    parcelWord.DictDKLB = dictDKLB;
                    parcelWord.ListGeoLand = listGeoLand;
                    parcelWord.ListLineFeature = listLine;
                    parcelWord.ListPointFeature = listPoint;
                    parcelWord.ListPolygonFeature = listPolygon;
                    parcelWord.ListTissue = listTissue;
                    parcelWord.ListValidDot = listValidDot;
                    parcelWord.ParcelCheckDate = DateTime.Now;
                    parcelWord.ParcelDrawDate = DateTime.Now;
                    parcelWord.OwnedPerson = person;
                    parcelWord.OpenTemplate(templatePath);
                    parcelWord.SaveAsMultiFile(person, savePathOfWord, ParcelWordSettingDefine.SaveParcelPCAsPDF);
                    indexOfVp++;
                    this.ReportProgress((int)(1 + vpPercent * indexOfVp), string.Format("{0}", descZone + person.Name));

                    count++;
                    if (indexOfVp % 10 == 0)
                    {
                        GC.Collect();
                        GC.WaitForPendingFinalizers();
                        GC.Collect();
                    }
                }
                string info = string.Format("{0}导出{1}户承包方,共{2}个宗地图表", descZone, exportVps.Count, count);
                this.ReportInfomation(info);
                result = true;
            }
            catch (Exception exx)
            {
                result = false;
                if (exx is TaskStopException)
                {
                    return result;
                }
                this.ReportWarn(string.Format("导出{0}的地块示意图.发生错误:{1}", descZone, exx.ToString()));
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

        /// <summary>
        /// 按照设置进行地块类别筛选导出
        /// </summary>
        private List<ContractLand> InitalizeAgricultureLandSortValue(List<ContractLand> geoLandCollection)
        {
            if (geoLandCollection.Count == 0) return new List<ContractLand>();
            if (ParcelWordSettingDefine.ExportContractLandType == false)
            {
                geoLandCollection.RemoveAll(go => go.LandCategory == ((int)eLandCategoryType.ContractLand).ToString());
            }
            if (ParcelWordSettingDefine.ExportPrivateLandType == false)
            {
                geoLandCollection.RemoveAll(go => go.LandCategory == ((int)eLandCategoryType.PrivateLand).ToString());
            }
            if (ParcelWordSettingDefine.ExportMotorizeLandType == false)
            {
                geoLandCollection.RemoveAll(go => go.LandCategory == ((int)eLandCategoryType.MotorizeLand).ToString());
            }
            if (ParcelWordSettingDefine.ExportWasteLandType == false)
            {
                geoLandCollection.RemoveAll(go => go.LandCategory == ((int)eLandCategoryType.WasteLand).ToString());
            }
            if (ParcelWordSettingDefine.ExportCollectiveLandType == false)
            {
                geoLandCollection.RemoveAll(go => go.LandCategory == ((int)eLandCategoryType.CollectiveLand).ToString());
            }
            if (ParcelWordSettingDefine.ExportEncollecLandType == false)
            {
                geoLandCollection.RemoveAll(go => go.LandCategory == ((int)eLandCategoryType.EncollecLand).ToString());
            }
            if (ParcelWordSettingDefine.ExportFeedLandType == false)
            {
                geoLandCollection.RemoveAll(go => go.LandCategory == ((int)eLandCategoryType.FeedLand).ToString());
            }
            if (ParcelWordSettingDefine.ExportAbandonedLandType == false)
            {
                geoLandCollection.RemoveAll(go => go.LandCategory == ((int)eLandCategoryType.AbandonedLand).ToString());
            }
            return geoLandCollection;
        }

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
                string tempPath = TemplateHelper.WordTemplate(TemplateFile.PublicityWord);
                ExportPublicityWordTable export = new ExportPublicityWordTable();

                #region 通过反射等机制定制化具体的业务处理类

                var temp = WorksheetConfigHelper.GetInstance(export);
                if (temp != null && temp.TemplatePath != null)
                {
                    if (temp is ExportPublicityWordTable)
                    {
                        export = (ExportPublicityWordTable)temp;
                    }
                    tempPath = Path.Combine(TheApp.GetApplicationPath(), temp.TemplatePath);
                }

                #endregion 通过反射等机制定制化具体的业务处理类

                export.CurrentZone = currentZone;
                export.Contractor = exportVp;
                export.DictList = args.DictList;
                export.LandCollection = landArrays;  //地块集合
                export.Tissue = vpExportArgs.tissue; //发包方
                export.OpenTemplate(tempPath);
                string fileName = vpSavePath + @"\" + "公示结果归户表";
                export.SaveAs(exportVp, fileName);  //合同
                result = true;
                this.ReportInfomation(string.Format("导出{0}{1}的公示结果归户表", descZone, exportVp.Name));
            }
            catch (Exception ex)
            {
                result = false;
                if (ex is TaskStopException)
                {
                    return result;
                }
                this.ReportError(ex.Message);
                YuLinTu.Library.Log.Log.WriteException(this, "ExportDataWord(导出数据到Word表)", ex.Message + ex.StackTrace);
                return result;
            }
            return result;
        }

        /// <summary>
        /// 导出单户申请书
        /// </summary>
        private bool ExportSingleRequireBook(VirtualPerson exportVp, ALLValidateArgument vpExportArgs)
        {
            var args = Argument as TaskDataSummaryExportArgument;
            bool result = true;
            try
            {
                string tempPath = TemplateFile.ContractFamilyRequireBook;
                result = SingleExportRequireBook(currentZone, vpExportArgs, exportVp, tempPath, eConstructMode.Family);
                if (result)
                {
                    this.ReportInfomation(string.Format("导出{0}{1}的单户申请书(家庭承包)", descZone, exportVp.Name));
                }
                tempPath = TemplateFile.ContractFamilyOtherRequireBook;
                result = SingleExportRequireBook(currentZone, vpExportArgs, exportVp, tempPath, eConstructMode.Other);
                if (result)
                {
                    this.ReportInfomation(string.Format("导出{0}{1}的单户申请书(非家庭承包)", descZone, exportVp.Name));
                }
            }
            catch (Exception ex)
            {
                result = false;
                if (ex is TaskStopException)
                {
                    return result;
                }
                this.ReportError(ex.Message);
                YuLinTu.Library.Log.Log.WriteException(this, "ExportDataWord(单户申请书(非家庭承包))", ex.Message + ex.StackTrace);
                return result;
            }
            return result;
        }

        /// <summary>
        /// 导出单户申请书-调用
        /// </summary>
        private bool SingleExportRequireBook(Zone zone, ALLValidateArgument vpExportArgs, VirtualPerson vp, string templemPath, eConstructMode typeMode)
        {
            bool result = true;
            try
            {
                var args = Argument as TaskDataSummaryExportArgument;
                var zoneStation = dbContext.CreateZoneWorkStation();
                string tempPath = TemplateHelper.WordTemplate(templemPath);
                ExportLandRequireBook printBook = new ExportLandRequireBook(vp);

                #region 通过反射等机制定制化具体的业务处理类

                var temp = WorksheetConfigHelper.GetInstance(printBook);
                if (temp != null && temp.TemplatePath != null)
                {
                    if (temp is ExportLandRequireBook)
                    {
                        printBook = (ExportLandRequireBook)temp;
                    }
                    tempPath = Path.Combine(TheApp.GetApplicationPath(), temp.TemplatePath);
                }

                #endregion 通过反射等机制定制化具体的业务处理类

                printBook.DbContext = dbContext;
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
                    filePath = vpSavePath + @"\" + "单户申请书(家庭承包)";
                }
                else
                {
                    filePath = vpSavePath + @"\" + "单户申请书(非家庭承包)";
                }
                printBook.SaveAs(vp, filePath);

                GC.Collect();
                return true;
            }
            catch (Exception ex)
            {
                result = false;
                if (ex is TaskStopException)
                {
                    return result;
                }
                this.ReportError(ex.Message);
                YuLinTu.Library.Log.Log.WriteException(this, "ExportDataWord(导出数据到word表)", ex.Message + ex.StackTrace);
                return result;
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
        /// 导出户主声明书
        /// </summary>
        private bool ExportVPApplyBook(VirtualPerson exportVp, ALLValidateArgument vpExportArgs)
        {
            string templatePath = TemplateHelper.WordTemplate(TemplateFile.VirtualPersonApplyBook);
            string zoneName = currentZone.FullName;
            var args = Argument as TaskDataSummaryExportArgument;
            bool result = true;
            try
            {
                //确定是否导出集体户信息(利用配置文件)
                if (args.FamilyOtherSet.ShowFamilyInfomation && (exportVp.Name.IndexOf("机动地") >= 0 || exportVp.Name.IndexOf("集体") >= 0))
                {
                    return false;
                }
                var zonelist = GetParentZone(currentZone);
                string familyNuber = ToolString.ExceptSpaceString(exportVp.FamilyNumber);
                ExportApplyBook exportFamily = new ExportApplyBook(exportVp);

                #region 通过反射等机制定制化具体的业务处理类

                var temp = WorksheetConfigHelper.GetInstance(exportFamily);
                if (temp != null && temp.TemplatePath != null)
                {
                    if (temp is ExportApplyBook)
                    {
                        exportFamily = (ExportApplyBook)temp;
                    }
                    templatePath = Path.Combine(TheApp.GetApplicationPath(), temp.TemplatePath);
                }

                #endregion 通过反射等机制定制化具体的业务处理类

                exportFamily.Date = args.Date;       //获取日期
                exportFamily.ZoneName = zoneName;
                exportFamily.ZoneList = zonelist;
                exportFamily.CurrentZone = currentZone;
                exportFamily.OpenTemplate(templatePath);
                exportFamily.RightName = InitalizeRightType(args.VirtualType);
                exportFamily.SaveAs(exportVp, vpSavePath + @"\" + TemplateFile.VirtualPersonApplyBook + ".doc");
                this.ReportInfomation(string.Format("导出{0}{1}的户主声明书", descZone, exportVp.Name));
            }
            catch (Exception ex)
            {
                result = false;
                if (ex is TaskStopException)
                {
                    return result;
                }
                this.ReportError(ex.Message);
                YuLinTu.Library.Log.Log.WriteException(this, "ExportDataWord(导出户主声明书)", ex.Message + ex.StackTrace);
                return result;
            }
            return result;
        }

        /// <summary>
        /// 导出承包方调查表word
        /// </summary>
        private bool ExportVPSurvyTable(VirtualPerson exportVp, ALLValidateArgument vpExportArgs)
        {
            bool result = false;
            var args = Argument as TaskDataSummaryExportArgument;
            try
            {
                string tempPath = TemplateHelper.WordTemplate(TemplateFile.VirtualPersonSurveyWord);
                string familyNuber = ToolString.ExceptSpaceString(exportVp.FamilyNumber);
                List<VirtualPerson> exportVps = new List<VirtualPerson>();
                exportVps.Add(exportVp);

                CollectivityTissue tissue = vpExportArgs.tissue;
                string concordnumber = "";
                var vpconcord = vpExportArgs.zoneConcords.Find(cc => cc.ContracterId == exportVp.ID);
                if (vpconcord != null)
                {
                    concordnumber = vpconcord.ConcordNumber;
                }

                ExportContractorTable export = new ExportContractorTable();

                #region 通过反射等机制定制化具体的业务处理类

                var temp = WorksheetConfigHelper.GetInstance(export);
                if (temp != null && temp.TemplatePath != null)
                {
                    if (temp is ExportContractorTable)
                    {
                        export = (ExportContractorTable)temp;
                    }
                    tempPath = Path.Combine(TheApp.GetApplicationPath(), temp.TemplatePath);
                }

                #endregion 通过反射等机制定制化具体的业务处理类

                export.DictList = args.DictList;
                export.MarkDesc = descZone;
                export.Tissue = tissue;
                export.ConcordNumber = concordnumber;
                export.CurrentZone = currentZone;
                export.OpenTemplate(tempPath);
                export.SaveAs(exportVp, vpSavePath + @"\" + "承包方调查表.doc");

                result = true;
            }
            catch (Exception ex)
            {
                if (ex is TaskStopException)
                {
                    result = false;
                    return result;
                }
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

        /// <summary>
        /// 导出户主委托书
        /// </summary>
        private bool ExportVPDelegateBook(VirtualPerson exportVp, ALLValidateArgument vpExportArgs)
        {
            bool result = true;
            try
            {
                string templatePath = TemplateHelper.WordTemplate(TemplateFile.VirtualPersonDelegateBook);
                string zoneName = currentZone.FullName;
                var args = Argument as TaskDataSummaryExportArgument;
                var zoneList = GetParents(currentZone, dbContext);
                //确定是否导出集体户信息(利用配置文件)
                if (args.FamilyOtherSet.ShowFamilyInfomation && (exportVp.Name.IndexOf("机动地") >= 0 || exportVp.Name.IndexOf("集体") >= 0))
                {
                    return false;
                }
                string familyNuber = ToolString.ExceptSpaceString(exportVp.FamilyNumber);
                ExportDelegateBook exportFamily = new ExportDelegateBook(exportVp);
                exportFamily.DateValue = args.Date;
                exportFamily.ZoneList = zoneList == null ? new List<Zone>() : zoneList;
                exportFamily.CurrentZone = currentZone;
                exportFamily.OpenTemplate(templatePath);
                exportFamily.RightName = InitalizeRightType(args.VirtualType);
                exportFamily.SaveAs(exportVp, vpSavePath + @"\" + TemplateFile.VirtualPersonDelegateBook + ".doc");
                this.ReportInfomation(string.Format("导出{0}{1}的户主委托书", descZone, exportVp.Name));
            }
            catch (Exception ex)
            {
                if (ex is TaskStopException)
                {
                    result = false;
                    return result;
                }
                this.ReportError(ex.Message);
                YuLinTu.Library.Log.Log.WriteException(this, "ExportDataExcel(导出户主委托书)", ex.Message + ex.StackTrace);
            }

            return result;
        }

        #endregion Methods - Private



        #region Methods - exportHelper-单个文件组级导出底层

        /// <summary>
        ///按组导出地块调查表excel
        /// </summary>
        private bool ExportZoneLandSurvyTable()
        {
            bool result = true;
            try
            {
                var args = Argument as TaskDataSummaryExportArgument;

                List<VirtualPerson> vps = currentZoneExportArgs.zonevps.Clone() as List<VirtualPerson>;
                //判断是否显示集体信息-勾选常规设置后就设置为不显示
                if (args.SettingDefine.DisplayCollectUsingCBdata)
                {
                    //List<VirtualPerson> vpList = currentZoneExportArgs.zonevps.FindAll(fm => (fm.Name.IndexOf("机动地") >= 0 || fm.Name.IndexOf("集体") >= 0));
                    //foreach (VirtualPerson vpn in vpList)
                    //{
                    //    vps.Remove(vpn);
                    //}
                    //vpList.Clear();
                    vps.RemoveAll(c => c.FamilyExpand.ContractorType != eContractorType.Farmer);
                }
                string fileType = descZone + "承包地块调查表.xls";
                string tempPath = TemplateHelper.ExcelTemplate(TemplateFile.ContractLandSurveyExceltemp);
                string zoneName = GetUinitName(currentZone);
                if (args.SystemSet.CountryTableHead)
                {
                    var zoneStation = dbContext.CreateZoneWorkStation();
                    zoneName = zoneStation.GetVillageName(currentZone);
                }
                var landStation = dbContext.CreateContractLandWorkstation();
                List<ContractLand> landArrays = currentZoneExportArgs.zoneContractLands.Clone() as List<ContractLand>;
                landArrays.LandNumberFormat(args.SystemSet);

                var listConcords = currentZoneExportArgs.zoneConcords.Clone() as List<ContractConcord>;
                var listBooks = currentZoneExportArgs.zoneContractRegeditBooks.Clone() as List<ContractRegeditBook>;
                string filePath = string.Empty;
                ExportContractorSurveyExcel export = new ExportContractorSurveyExcel();

                #region 通过反射等机制定制化具体的业务处理类

                var temp = WorksheetConfigHelper.GetInstance(export);
                if (temp != null && temp.TemplatePath != null)
                {
                    if (temp is ExportContractorSurveyExcel)
                        export = (ExportContractorSurveyExcel)temp;
                    tempPath = Path.Combine(TheApp.GetApplicationPath(), temp.TemplatePath);
                }

                #endregion 通过反射等机制定制化具体的业务处理类

                export.SaveFilePath = args.FileName + @"\" + fileType;
                export.CurrentZone = currentZone;
                export.Familys = vps;
                export.ExcelName = descZone;
                export.UnitName = zoneName;
                export.TableType = 1;//地块调查表
                export.DictionList = args.DictList;
                export.LandArrays = landArrays;
                export.ConcordCollection = listConcords;
                export.BookColletion = listBooks;
                result = export.BeginExcel(null, null, currentZone.FullCode.ToString(), tempPath);
                filePath = export.SaveFilePath;
            }
            catch (Exception ex)
            {
                result = false;
                if (ex is TaskStopException)
                {
                    return result;
                }

                this.ReportError(ex.Message);
                YuLinTu.Library.Log.Log.WriteException(this, "ExportDataExcel(导出数据到Excel表)", ex.Message + ex.StackTrace);
                return result;
            }

            return result;
        }

        #endregion Methods - exportHelper-单个文件组级导出底层

        #region Methods-Helper

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
            string zoneCode = family.ZoneCode;
            if (!string.IsNullOrEmpty(zoneCode) && zoneCode.Length != 14)
                zoneCode = zoneCode.PadRight(14, '0');
            string imagePath = filePath + "\\" + zoneCode;
            if (!Directory.Exists(imagePath))
            {
                Directory.CreateDirectory(imagePath);
            }
            string imageName = imagePath + "\\" + "DKSYT" + zoneCode;
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

        #endregion Methods-Helper

        #endregion Methods
    }

    /// <summary>
    /// 当前地域下导出类需要的所有参数，包括人、地块、权证、合同四类数据
    /// </summary>
    internal class ALLValidateArgument
    {
        public List<VirtualPerson> zonevps { set; get; }
        public List<ContractConcord> zoneConcords { set; get; }
        public List<ContractRegeditBook> zoneContractRegeditBooks { set; get; }
        public List<ContractLand> zoneContractLands { set; get; }
        public CollectivityTissue tissue { set; get; }
    }
}