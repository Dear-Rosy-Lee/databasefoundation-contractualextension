/*
 * (C) 2015  鱼鳞图公司版权所有,保留所有权利 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YuLinTu.Data;
using YuLinTu.Library.Business;
using YuLinTu.Library.Entity;
using YuLinTu.Library.WorkStation;

namespace YuLinTu.Component.XiZangLZ
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
                //listLand.LandNumberFormat(SystemSetDefine);
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
                openFilePath = openFilePath + @"\" + savePath ;//+ @"\" + argument.CurrentZone.Name;

                ContractConcord concord = null;
                foreach (VirtualPerson family in listPerson)
                {
                    var landsOfFamily = listLand.FindAll(c => c.OwnerId == family.ID);
                    if (landsOfFamily == null || landsOfFamily.Count == 0)
                    {
                        this.ReportWarn(string.Format("{0}未获取承包地块!", markDesc + family.Name));
                        continue;
                    }
                    ExportLandSurveyWordTable export = new ExportLandSurveyWordTable();
                    string tempPath = TemplateHelper.WordTemplate("西藏" + TemplateFile.ContractAccountLandSurveyWord);  //模板文件
                    export.Contractor = family;
                    export.DictList = listDict == null ? new List<Dictionary>() : listDict;
                    export.CurrentZone = argument.CurrentZone;
                    export.Tissue = tissue;
                    export.LandCollection = landsOfFamily;
                    export.Concord = concord;
                    export.OpenTemplate(tempPath);
                    string filePath = openFilePath + @"\" + family.FamilyNumber.PadLeft(4, '0')+"-" + family.Name + "-" + TemplateFile.ContractAccountLandSurveyWord;
                    export.SaveAs(family, filePath);
                    string strDesc = markDesc + family.Name;
                    this.ReportProgress((int)(1 + landPercent * indexOfLand), strDesc);
                    indexOfLand++;
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
