/*
 * (C) 2015  鱼鳞图公司版权所有,保留所有权利 
 */
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using YuLinTu.Data;
using YuLinTu.Library.Entity;
using YuLinTu.Library.WorkStation;
using YuLinTu.Windows;

namespace YuLinTu.Library.Business
{
    /// <summary>
    /// 导出承包地块Word调查表任务类
    /// </summary>
    public class TaskExportLandWordOperation : Task
    {
        #region Ctor

        /// <summary>
        /// 构造函数
        /// </summary>
        public TaskExportLandWordOperation()
        { }

        #endregion

        #region Field

        private string openFilePath;  //打开文件路径
        private SystemSetDefine SystemSetDefine = SystemSetDefine.GetIntence();

        #endregion

        #region Property

        #endregion

        #region Method—Override

        /// <summary>
        /// 开始执行任务
        /// </summary>
        protected override void OnGo()
        {
            TaskExportLandWordArgument argument = Argument as TaskExportLandWordArgument;
            if (argument == null)
            {
                return;
            }
            IDbContext dbContext = argument.DbContext;
            var zone = argument.CurrentZone;
            List<Guid> ownerIds = new List<Guid>();
            try
            {
                var listPerson = argument.SelectedPersons;
                var senderStation = dbContext.CreateSenderWorkStation();
                var landStation = dbContext.CreateContractLandWorkstation();
                var dictStation = dbContext.CreateDictWorkStation();
                var listDict = dictStation.Get();
                var tissue = senderStation.Get(zone.ID);
                if (SystemSetDefine.ExportTableSenderDesToVillage && zone.Level == eZoneLevel.Group)
                {
                    var Senders = senderStation.GetTissues(zone.UpLevelCode, eLevelOption.Self);
                    if (Senders.Count > 0)
                    {
                        tissue = Senders[0];
                    }
                }
                if (listPerson == null || listPerson.Count == 0)
                {
                    this.ReportProgress(100, null);
                    this.ReportWarn(string.Format("{0}未获取承包方数据!", zone.FullName));
                    return;
                }
                listPerson.ForEach(c => ownerIds.Add(c.ID));
                var listLand = landStation.GetCollection(ownerIds);
                if (listLand == null || listLand.Count == 0)
                {
                    this.ReportProgress(100, null);
                    this.ReportWarn(string.Format("{0}未获取承包地块数据!", zone.FullName));
                    return;
                }
                listLand.LandNumberFormat(SystemSetDefine);
                bool canOpen = ExportLandWord(argument, listPerson, listLand, listDict, tissue);
                if (canOpen)
                    CanOpenResult = true;
            }
            catch (Exception ex)
            {
                YuLinTu.Library.Log.Log.WriteException(this, "TaskExportLandWordOperation(导出承包地块调查表任务)", ex.Message + ex.StackTrace);
                this.ReportException(ex, "导出承包地块调查表出现异常!");
            }
            finally
            {
                ownerIds.Clear();
                ownerIds = null;
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
        /// 导出数据到承包地块Word调查表
        /// </summary>
        public bool ExportLandWord(TaskExportLandWordArgument argument, List<VirtualPerson> listPerson, List<ContractLand> listLand,
            List<Dictionary> listDict, CollectivityTissue tissue)
        {
            int dotCount = 0;
            bool result = false;
            try
            {
                if (argument.CurrentZone == null)
                {
                    this.ReportError("未选择导出数据的地域!");
                    return result;
                }
                this.ReportProgress(1, "开始");

                var dotStation = argument.DbContext.CreateBoundaryAddressDotWorkStation();
                var coilStation = argument.DbContext.CreateBoundaryAddressCoilWorkStation();
                var concordStation = argument.DbContext.CreateConcordStation();
                var zoneStation = argument.DbContext.CreateZoneWorkStation();
                var listConcord = concordStation.GetByZoneCode(argument.CurrentZone.FullCode);
                var listCoil = coilStation.GetByZoneCode(argument.CurrentZone.FullCode, eLevelOption.Self);
                var listDot = dotStation.GetByZoneCode(argument.CurrentZone.FullCode, eLevelOption.Self);

                string markDesc = GetMarkDesc(argument.CurrentZone, argument.DbContext);
                int indexOfLand = 1;
                double landPercent = 99 / (double)listLand.Count;
                openFilePath = argument.FileName;
                List<Zone> allZones = zoneStation.GetAllZones(argument.CurrentZone);

                string savePath = CreateDirectoryHelper.CreateDirectory(allZones, argument.CurrentZone);
                if (argument.CurrentZone.Level == eZoneLevel.Group)
                    openFilePath = openFilePath + @"\" + savePath;//+ @"\" + argument.CurrentZone.Name;

                ContractConcord concord = null;
                List<BuildLandBoundaryAddressCoil> listLandCoil = null;
                List<BuildLandBoundaryAddressDot> listLandDot = null;
                List<BuildLandBoundaryAddressDot> listValidLandDot = null;

                foreach (VirtualPerson family in listPerson)
                {
                    var landsOfFamily = listLand.FindAll(c => c.OwnerId == family.ID);
                    if (landsOfFamily == null || landsOfFamily.Count == 0)
                    {
                        this.ReportWarn(string.Format("{0}未获取承包地块!", markDesc + family.Name));
                        continue;
                    }

                    ExportLandSurveyWordTable exportSpecial = new ExportLandSurveyWordTable();
                    bool isNeedSpecial = IsNeedSpecialHandle(out exportSpecial);
                    if (!isNeedSpecial)
                    {
                        foreach (var land in landsOfFamily)
                        {
                            concord = (land.ConcordId != null && land.ConcordId.HasValue) ? listConcord.Find(c => c.ID == (Guid)land.ConcordId) : null;
                            listLandCoil = listCoil == null ? new List<BuildLandBoundaryAddressCoil>() : listCoil.FindAll(c => c.LandID == land.ID);
                            listLandDot = listDot == null ? new List<BuildLandBoundaryAddressDot>() : listDot.FindAll(c => c.LandID == land.ID);
                            listValidLandDot = listLandDot == null ? new List<BuildLandBoundaryAddressDot>() : listLandDot.FindAll(c => c.IsValid == true);

                            ExportLandSurveyWordTable export = new ExportLandSurveyWordTable();
                            //dotCount = export.InitalizeDotCount(land);
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
                            else { }

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
                            #endregion

                            export.Contractor = family;
                            export.DictList = listDict == null ? new List<Dictionary>() : listDict;
                            export.CurrentZone = argument.CurrentZone;
                            export.Tissue = tissue;
                            export.Concord = concord;
                            export.ListLandCoil = listLandCoil;
                            export.ListLandDot = listLandDot;
                            export.ListLandValidDot = listValidLandDot;
                            export.OpenTemplate(tempPath);
                            string landNumber = land.LandNumber.Length > 5 ? land.LandNumber.Substring(land.LandNumber.Length - 5) : land.LandNumber;
                            //string filePath = openFilePath + @"\" + family.Name + @"\" + landNumber + "-" + TemplateFile.ContractAccountLandSurveyWord;
                            string filePath = openFilePath + @"\" + family.FamilyNumber?.TrimStart('0') + family.Name + @"\" + landNumber + "-" + TemplateFile.ContractAccountLandSurveyWord;//青海用
                            export.SaveAs(land, filePath);
                            string strDesc = string.Format("{0}({1})", markDesc + family.Name, land.Name);
                            this.ReportProgress((int)(1 + landPercent * indexOfLand), strDesc);
                            indexOfLand++;
                        }
                    }
                    else
                    {
                        if (exportSpecial == null)
                            return result;

                        exportSpecial.DbContext = argument.DbContext;
                        exportSpecial.Contractor = family;
                        exportSpecial.DictList = listDict == null ? new List<Dictionary>() : listDict;
                        exportSpecial.CurrentZone = argument.CurrentZone;
                        exportSpecial.Tissue = tissue;

                        int totalDot = 0;
                        foreach (var land in landsOfFamily)
                        {
                            listLandDot = listDot == null ? new List<BuildLandBoundaryAddressDot>() : listDot.FindAll(c => c.LandID == land.ID);
                            listValidLandDot = listLandDot == null ? new List<BuildLandBoundaryAddressDot>() : listLandDot.FindAll(c => c.IsValid == true);
                            totalDot += listValidLandDot.Count + 1;
                        }

                        string dir = Path.GetDirectoryName(exportSpecial.TemplatePath);
                        string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(exportSpecial.TemplatePath);
                        string extension = Path.GetExtension(exportSpecial.TemplatePath);
                        if (landsOfFamily.Count <= 14)
                        {
                            if (totalDot > 25 && totalDot < 56)
                            {
                                exportSpecial.TemplatePath = Path.Combine(dir, fileNameWithoutExtension + "2页" + extension);
                            }
                            else if (totalDot > 56 && totalDot <= 87)
                            {
                                exportSpecial.TemplatePath = Path.Combine(dir, fileNameWithoutExtension + "3页" + extension);
                            }
                            else if (totalDot > 87 && totalDot <= 118)
                            {
                                exportSpecial.TemplatePath = Path.Combine(dir, fileNameWithoutExtension + "4页" + extension);
                            }
                            else if (totalDot > 118)
                            {
                                exportSpecial.TemplatePath = Path.Combine(dir, fileNameWithoutExtension + "5页" + extension);
                            }
                        }
                        else
                        {
                            if (totalDot > 25 && totalDot < 56)
                            {
                                exportSpecial.TemplatePath = Path.Combine(dir, fileNameWithoutExtension + "2(2)页" + extension);
                            }
                            else if (totalDot > 56 && totalDot <= 87)
                            {
                                exportSpecial.TemplatePath = Path.Combine(dir, fileNameWithoutExtension + "2(3)页" + extension);
                            }
                            else if (totalDot > 87 && totalDot <= 118)
                            {
                                exportSpecial.TemplatePath = Path.Combine(dir, fileNameWithoutExtension + "2(4)页" + extension);
                            }
                            else if (totalDot > 118)
                            {
                                exportSpecial.TemplatePath = Path.Combine(dir, fileNameWithoutExtension + "2(5)页" + extension);
                            }
                        }

                        string tempPath = Path.Combine(TheApp.GetApplicationPath(), exportSpecial.TemplatePath);
                        exportSpecial.OpenTemplate(tempPath);
                        string filePath = openFilePath + @"\" + family.Name + "-" + TemplateFile.ContractAccountLandSurveyWord;

                        // 2017/9/21—湖南工单，定制化，传入单个家庭下的所有地块作为数据
                        exportSpecial.SaveAs(landsOfFamily, filePath);
                        string strDesc = string.Format("{0}", markDesc + family.Name);
                        this.ReportProgress((int)(1 + landPercent * indexOfLand), strDesc);
                    }
                }
                result = true;
                string info = string.Format("{0}导出{1}户承包方,共{2}个承包地块Word调查表", markDesc, listPerson.Count, indexOfLand - 1);
                this.ReportInfomation(info);
                this.ReportProgress(100, "完成");

                listConcord.Clear();
                listConcord = null;
                listDot.Clear();
                listDot = null;
                listCoil.Clear();
                listCoil = null;
            }
            catch (Exception ex)
            {
                result = false;
                this.ReportError("导出承包地块Word调查表失败");
                YuLinTu.Library.Log.Log.WriteException(this, "ExportLandWord(导出承包地块数据到Word调查表)", ex.Message + ex.StackTrace);
            }
            return result;
        }

        #endregion

        #region Method—Private


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
                {
                    special = (ExportLandSurveyWordTable)temp;
                }
                if (!(bool)temp.Tag)
                    return result;
                result = true;

                //tempPath = Path.Combine(TheApp.GetApplicationPath(), temp.TemplatePath);
            }
            #endregion
            return result;
        }

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

        #endregion
    }
}
