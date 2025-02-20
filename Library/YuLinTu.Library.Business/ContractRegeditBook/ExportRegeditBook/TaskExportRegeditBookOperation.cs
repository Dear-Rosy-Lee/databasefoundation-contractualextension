/*
 * (C) 2025  鱼鳞图公司版权所有,保留所有权利 
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
    /// 导出登记薄任务类
    /// </summary>
    public class TaskExportRegeditBookOperation : Task
    {
        #region Ctor

        /// <summary>
        /// 构造函数
        /// </summary>
        public TaskExportRegeditBookOperation()
        { }

        #endregion

        #region Field

        private string openFilePath;  //打开文件路径

        IDbContext dbContext;

        #endregion

        #region Property

        #endregion

        #region Method—Override

        /// <summary>
        /// 开始执行任务
        /// </summary>
        protected override void OnGo()
        {
            TaskExportRegeditBookArgument argument = Argument as TaskExportRegeditBookArgument;
            if (argument == null)
            {
                return;
            }
            dbContext = argument.DbContext;
            var zone = argument.CurrentZone;
            try
            {
                var listPerson = argument.SelectedPersons;
                var concordStation = dbContext.CreateConcordStation();
                var bookStation = dbContext.CreateRegeditBookStation();
                var dictStation = dbContext.CreateDictWorkStation();
                var listDict = dictStation.Get();
                var listConcord = concordStation.GetContractsByZoneCode(zone.FullCode, eLevelOption.Self);
                var listBook = bookStation.GetContractsByZoneCode(zone.FullCode, eLevelOption.Self);
                if (listPerson == null || listPerson.Count == 0)
                {
                    this.ReportProgress(100, null);
                    this.ReportWarn(string.Format("{0}未获取承包方数据!", zone.FullName));
                    return;
                }
                if (listConcord == null || listConcord.Count == 0)
                {
                    this.ReportProgress(100, null);
                    this.ReportWarn(string.Format("{0}未获取承包合同数据!", zone.FullName));
                    return;
                }
                if (listBook == null || listBook.Count == 0)
                {
                    this.ReportProgress(100, null);
                    this.ReportWarn(string.Format("{0}未获取承包权证数据!", zone.FullName));
                    return;
                }
                bool canOpen = ExportRegeditBook(argument, listPerson, listDict, listConcord, listBook);
                if (canOpen)
                    CanOpenResult = true;
            }
            catch (Exception ex)
            {
                YuLinTu.Library.Log.Log.WriteException(this, "TaskExportRegeditBookOperation(导出登记簿任务)", ex.Message + ex.StackTrace);
                this.ReportException(ex, "导出登记簿出现异常!");
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
        /// 导出登记簿
        /// </summary>
        public bool ExportRegeditBook(TaskExportRegeditBookArgument argument, List<VirtualPerson> listPerson,
            List<Dictionary> listDict, List<ContractConcord> listConcord, List<ContractRegeditBook> listBook)
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
                 var zonelist=  GetParentZone(argument.CurrentZone, argument.DbContext);
                int indexOfBook = 1;
                double bookPercent = 99 / (double)listBook.Count;
                openFilePath = argument.FileName;
                foreach (VirtualPerson family in listPerson)
                {
                    var concords = listConcord.FindAll(c => c.ContracterId != null && c.ContracterId == family.ID);
                    if (concords == null || concords.Count == 0)
                    {
                        this.ReportWarn(string.Format("{0}未获取承包合同!", markDesc + family.Name));
                        continue;
                    }
                    foreach (var concord in concords)
                    {
                        if (!listBook.Exists(c => !string.IsNullOrEmpty(c.RegeditNumber) && c.RegeditNumber == concord.ConcordNumber))
                            continue;
                        ContractRegeditBookPrinterData data = new ContractRegeditBookPrinterData(concord);
                        data.CurrentZone = argument.CurrentZone;
                        data.DbContext = argument.DbContext;
                        data.SystemDefine = argument.SystemDefine;
                        data.DictList = listDict == null ? new List<Dictionary>() : listDict;
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
                        #endregion

                        regeditBookWord.DbContext = dbContext;
                        regeditBookWord.ZoneList = zonelist;
                        regeditBookWord.DictList = listDict == null ? new List<Dictionary>() : listDict;
                        regeditBookWord.Tissue = data.Tissue;
                        regeditBookWord.OpenTemplate(tempPath);
                        string filePath = openFilePath + @"\" + family.Name + @"\" + family.FamilyNumber + "-" + concord.ContracterName + "-" + concord.ConcordNumber + "-" + TemplateFile.PrivewRegeditBookWord;
                        regeditBookWord.SaveAs(data, filePath);

                        string strDesc = string.Format("{0}({1})", markDesc + family.Name, concord.ConcordNumber);
                        this.ReportProgress((int)(1 + bookPercent * indexOfBook), strDesc);
                        indexOfBook++;

                        data = null;
                    }
                }
                result = true;
                string info = string.Format("{0}导出{1}户承包方,共{2}张登记簿", markDesc, listPerson.Count, indexOfBook - 1);
                this.ReportInfomation(info);
                this.ReportProgress(100, "完成");
            }
            catch (Exception ex)
            {
                result = false;
                this.ReportError("导出登记簿失败");
                YuLinTu.Library.Log.Log.WriteException(this, "ExportRegeditBook(导出承包权证数据到登记簿)", ex.Message + ex.StackTrace);
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
        #endregion

        #region Method—Helper

        #endregion
    }
}
