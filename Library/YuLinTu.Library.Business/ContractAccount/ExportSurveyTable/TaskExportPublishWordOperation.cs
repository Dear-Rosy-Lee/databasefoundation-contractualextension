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
    /// 导出公示结果归户表任务类
    /// </summary>
    public class TaskExportPublishWordOperation : Task
    {

        #region Ctor

        /// <summary>
        /// 构造函数
        /// </summary>
        public TaskExportPublishWordOperation()
        { }

        #endregion

        #region Field

        private IDbContext dbContext;
        private Zone zone;
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
            TaskExportPublishWordArgument argument = Argument as TaskExportPublishWordArgument;
            if (argument == null)
            {
                return;
            }
            dbContext = argument.DbContext;
            zone = argument.CurrentZone;
            try
            {
                var listPerson = argument.SelectedPersons;
                var senderStation = dbContext.CreateSenderWorkStation();
                var landStation = dbContext.CreateContractLandWorkstation();
                var dictStation = dbContext.CreateDictWorkStation();
                var listDict = dictStation.Get();
                var sender = senderStation.Get(zone.ID); //发包方
                if (sender == null)
                {                  
                    var tissues = senderStation.GetTissues(zone.FullCode, eLevelOption.Self);
                    if (tissues != null && tissues.Count > 0)
                        sender = tissues[0];
                }
               
                if (listPerson == null || listPerson.Count == 0)
                {
                    this.ReportProgress(100, null);
                    this.ReportWarn(string.Format("{0}未获取承包方数据!", zone.FullName));
                    return;
                }
                var listLand = landStation.GetCollection(zone.FullCode, eLevelOption.Self);
               
                if (listLand == null || listLand.Count == 0)
                {
                    this.ReportProgress(100, null);
                    this.ReportWarn(string.Format("{0}未获取承包地块数据!", zone.FullName));
                    return;
                }
                listLand.LandNumberFormat(SystemSetDefine);
                bool canOpen = ExportPublishWord(argument, listPerson, listLand, listDict, sender);
                if (canOpen)
                    CanOpenResult = true;
            }
            catch (Exception ex)
            {
                YuLinTu.Library.Log.Log.WriteException(this, "TaskExportPublishWordOperation(导出公示结果归户表任务)", ex.Message + ex.StackTrace);
                this.ReportException(ex, "导出公示结果归户表出现异常!");
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
        /// 导出数据到公示结果归户表
        /// </summary>
        public bool ExportPublishWord(TaskExportPublishWordArgument argument, List<VirtualPerson> listPerson, List<ContractLand> listLand,
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
                string markDesc = GetMarkDesc(argument.CurrentZone, argument.DbContext);
                int indexOfVp = 1;
                double vpPercent = 99 / (double)listPerson.Count;
                var zoneStation = argument.DbContext.CreateZoneWorkStation();
                openFilePath = argument.FileName;

                List<Zone> allZones = zoneStation.GetAllZones(argument.CurrentZone);
                string savePath = CreateDirectoryHelper.CreateDirectory(allZones, argument.CurrentZone);               
                 openFilePath = openFilePath + @"\" + savePath;                
                var concordStation = argument.DbContext.CreateConcordStation();
                
                //var landStocks = new AccountLandBusiness(dbContext).GetStockRightLand(zone);
                foreach (VirtualPerson family in listPerson)
                {
                    var concords = concordStation.GetAllConcordByFamilyID(family.ID);
                    var landsOfFamily = listLand.FindAll(c => c.OwnerId == family.ID);
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
                    #endregion

                    export.CurrentZone = argument.CurrentZone;
                    export.Contractor = family;
                    export.DictList = listDict == null ? new List<Dictionary>() : listDict;
                    export.LandCollection = landsOfFamily == null ? new List<ContractLand>() : landsOfFamily;  //地块集合
                    //if(family.IsStockFarmer)
                    //    export.LandCollection.AddRange(landStocks);
                    if (SystemSetDefine.ExportTableSenderDesToVillage) 
                    {
                        tissue.Name = GetVillageLevelDesc(argument.CurrentZone, argument.DbContext);
                    }
                    if (concords != null && concords.Count > 0)
                    {
                        export.Concord = concords.Find(d => d.ArableLandType == "110");
                    }
                    export.Tissue = tissue; //发包方
                    export.OpenTemplate(tempPath);
                    string fileName = openFilePath + @"\" +family.FamilyNumber+"-"+ family.Name + "-" + TemplateFile.PublicityWord;
                    export.SaveAs(family, fileName);
                    string strDesc = string.Format("{0}", markDesc + family.Name);
                    this.ReportProgress((int)(1 + vpPercent * indexOfVp), strDesc);
                    indexOfVp++;
                }
                result = true;
                string info = string.Format("{0}导出{1}户承包方,共{2}个公示结果归户表", markDesc, listPerson.Count, indexOfVp - 1);
                this.ReportInfomation(info);
                this.ReportProgress(100, "完成");
            }
            catch (Exception ex)
            {
                result = false;
                this.ReportError("导出公示结果归户表失败");
                YuLinTu.Library.Log.Log.WriteException(this, "ExportLandWord(导出公示结果归户表表)", ex.Message + ex.StackTrace);
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



        #endregion

        #region Method—Helper

        #endregion
    }
}
