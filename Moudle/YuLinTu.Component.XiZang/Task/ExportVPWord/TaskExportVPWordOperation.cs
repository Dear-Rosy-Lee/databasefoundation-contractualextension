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

namespace YuLinTu.Component.XiZangLZ
{
    /// <summary>
    /// 导出承包方Word调查表任务类
    /// </summary>
    public class TaskExportVPWordOperation : Task
    {
        #region Ctor

        /// <summary>
        /// 构造函数
        /// </summary>
        public TaskExportVPWordOperation()
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
            TaskExportVPWordArgument argument = Argument as TaskExportVPWordArgument;
            if (argument == null)
            {
                return;
            }
            IDbContext dbContext = argument.DbContext;
            var zone = argument.CurrentZone;
            try
            {
                var senderStation = dbContext.CreateSenderWorkStation();
                var tissue = senderStation.Get(zone.ID);
                if (tissue == null)
                {
                    var tissues = senderStation.GetTissues(zone.FullCode, eLevelOption.Self);
                    if (tissues != null && tissues.Count > 0)
                    {
                        tissue = tissues[0];
                    }
                }
                if (SystemSetDefine.ExportTableSenderDesToVillage && zone.Level == eZoneLevel.Group)
                {
                    var Sender = senderStation.GetTissues(zone.UpLevelCode, eLevelOption.Self);
                    if (Sender.Count > 0)
                    {
                        tissue = Sender[0];
                    }
                }
                var listPerson = argument.SelectedPersons;
                if (listPerson == null || listPerson.Count == 0)
                {
                    this.ReportProgress(100, null);
                    this.ReportWarn(string.Format("{0}未获取承包方数据!", zone.FullName));
                    return;
                }

                bool canOpen = ExportVPWord(argument, listPerson, tissue);
                if (canOpen)
                    CanOpenResult = true;
            }
            catch (Exception ex)
            {
                YuLinTu.Library.Log.Log.WriteException(this, "TaskExportVPWordOperation(导出承包方调查表任务)", ex.Message + ex.StackTrace);
                this.ReportException(ex, "导出承包方Word调查表出现异常!");
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
        /// 导出数据到承包方Word调查表
        /// </summary>
        public bool ExportVPWord(TaskExportVPWordArgument argument, List<VirtualPerson> listPerson, CollectivityTissue tissue)
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
                string tempPath = TemplateHelper.WordTemplate("西藏农村土地承包经营权承包方调查表");  //模板文件
                int index = 1;
                var concordStation = argument.DbContext.CreateConcordStation();
                var bookStation = argument.DbContext.CreateRegeditBookStation();
                var zoneStation = argument.DbContext.CreateZoneWorkStation();

                double vpPercent = 99 / (double)listPerson.Count;
                openFilePath = argument.FileName;

                var personIds = new List<Guid>(listPerson.Count);
                listPerson.ForEach(c => personIds.Add(c.ID));
                var concordsOfPersons = concordStation.GetContractsByFamilyIDs(personIds.ToArray());
                var booksOfConcords = bookStation.GetByZoneCode(argument.CurrentZone.FullCode, eSearchOption.Precision);
                List<Zone> allZones = zoneStation.GetAllZones(argument.CurrentZone);
                string savePath = CreateDirectoryHelper.CreateDirectory(allZones, argument.CurrentZone);

                openFilePath = openFilePath + @"\" + savePath;
                //openFilePath= openFilePath + @"\" + savePath+@"\"+argument.CurrentZone.Name;
                foreach (VirtualPerson family in listPerson)
                {
                    var concords = concordsOfPersons.FindAll(c => c.ContracterId != null && c.ContracterId.Value == family.ID);
                    string concordnumber = "";
                    string bookNumber = string.Empty;
                    YuLinTu.Library.Entity.ContractRegeditBook book = null;
                    if (concords != null && concords.Count > 0)
                    {
                        concordnumber = concords[0].ConcordNumber;
                        book = booksOfConcords.Find(c => c.ID == concords[0].ID);
                        if (book != null)
                        {
                            bookNumber = book.Number;
                        }
                    }
                    VirtualPerson vp = family.Clone() as VirtualPerson;
                    if (SystemSetDefine.PersonTable)
                    {
                        List<Person> person = vp.SharePersonList;
                        person = person.FindAll(c => c.IsSharedLand.Equals("是"));
                        vp.SharePersonList = person;
                    }
                    string dictoryName = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Template\西藏字典.xlsx");
                    ExportContractorTable export = new ExportContractorTable(dictoryName);
                    export.Contractor = vp;
                    export.DictList = argument.DictList;
                    export.Book = book;
                    //export.ExportVPTableCountContainsDiedPerson = SystemSetDefine.ExportVPTableCountContainsDiedPerson;
                    //export.IsShare = SystemSetDefine.PersonTable;
                    //export.ConcordNumber = concordnumber;
                    export.Tissue = tissue == null ? new CollectivityTissue { Name = "" } : tissue;
                    //export.WarrentNumber = bookNumber;
                    export.OpenTemplate(tempPath);
                    string familyNuber = ToolString.ExceptSpaceString(family.FamilyNumber);
                    string filePath = openFilePath + @"\" + familyNuber + "-" + family.Name + "-" + TemplateFile.ContractSurveyWord;
                    export.SaveAs(vp, filePath);
                    this.ReportProgress((int)(1 + vpPercent * index), markDesc + family.Name);
                    index++;
                }
                result = true;
                string info = string.Format("{0}导出{1}张承包方Word调查表", markDesc, index - 1);
                this.ReportInfomation(info);
                this.ReportProgress(100, "完成");
            }
            catch (Exception ex)
            {
                result = false;
                this.ReportError("导出承包方Word调查表失败");
                YuLinTu.Library.Log.Log.WriteException(this, "ExportVPWord(导出承包方数据到Word调查表)", ex.Message + ex.StackTrace);
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
