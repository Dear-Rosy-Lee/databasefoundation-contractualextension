using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YuLinTu.Data;
using YuLinTu.Library.Business;
using YuLinTu.Library.Entity;
using YuLinTu.Library.WorkStation;
using YuLinTu.Windows;

namespace YuLinTu.Component.XiZangLZ
{
    public class TaskExportWarrentOperation : Task
    {
        #region Ctor

        /// <summary>
        /// 构造函数
        /// </summary>
        public TaskExportWarrentOperation()
        { }

        #endregion Ctor

        #region Field

        private string openFilePath;  //打开文件路径

        #endregion Field

        #region Method—Override

        /// <summary>
        /// 开始执行任务
        /// </summary>
        protected override void OnGo()
        {
            TaskExportWarrentArgument argument = Argument as TaskExportWarrentArgument;
            if (argument == null)
            {
                return;
            }
            IDbContext dbContext = argument.DbContext;
            var zone = argument.CurrentZone;
            try
            {
                openFilePath = argument.FileName;
                var listPerson = argument.SelectedPersons;
                var concordStation = dbContext.CreateConcordStation();
                var bookStation = dbContext.CreateRegeditBookStation();
                var landStation = dbContext.CreateContractLandWorkstation();
                var dictStation = dbContext.CreateDictWorkStation();
                var senderStation = dbContext.CreateSenderWorkStation();
                var zoneStaion = dbContext.CreateZoneWorkStation();
                var listDict = dictStation.Get();
                var listConcord = concordStation.GetContractsByZoneCode(zone.FullCode, eLevelOption.Self);
                var listBook = bookStation.GetContractsByZoneCode(zone.FullCode, eLevelOption.Self);
                var listLand = landStation.GetCollection(zone.FullCode, eLevelOption.Self);
                var tissue = senderStation.Get(zone.ID);
                if (tissue == null)
                    tissue = senderStation.GetTissues(zone.FullCode, eLevelOption.Self).FirstOrDefault();
                var parentsToProvince = zoneStaion.GetParentsToProvince(zone);
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
                listLand.LandNumberFormat(argument.SystemDefine);
                bool canOpen = ExportWarrent(argument, listPerson, listLand, listDict, listConcord, listBook, tissue, parentsToProvince);
                if (canOpen)
                    CanOpenResult = true;
            }
            catch (Exception ex)
            {
                YuLinTu.Library.Log.Log.WriteException(this, "TaskExportWarrentOperation(导出证书任务)", ex.Message + ex.StackTrace);
                this.ReportException(ex, "导出证书出现异常!");
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
        /// 导出证书
        /// </summary>
        public bool ExportWarrent(TaskExportWarrentArgument argument, List<VirtualPerson> listPerson, List<ContractLand> listLand,
            List<Dictionary> listDict, List<ContractConcord> listConcord, List<Library.Entity.ContractRegeditBook> listBook, CollectivityTissue tissue, List<Zone> parentsToProvince)
        {
            bool result = false;
            try
            {
                string excelName = GetMarkDesc(argument.CurrentZone, argument.DbContext);
                this.ReportProgress(1, "开始");
                string tempPath = TemplateHelper.WordTemplate("西藏农村土地承包经营权证");
                int percentOfPerson = (100 / listPerson.Count);
                int percentIndex = 1;

                var zonelist = GetParentZone(argument.CurrentZone, argument.DbContext);
                zonelist.Add(argument.CurrentZone);
                foreach (var person in listPerson)
                {
                    var concords = listConcord.FindAll(c => c.ContracterId == person.ID);
                    if (concords == null || concords.Count == 0)
                        continue;
                    var familyConcord = concords.Find(c => c.ArableLandType == ((int)eConstructMode.Family).ToString());
                    var familyBook = familyConcord == null ? null : listBook.Find(c => c.ID == familyConcord.ID);
                    var otherConcord = concords.Find(c => c.ArableLandType != ((int)eConstructMode.Family).ToString());
                    var otherBook = otherConcord == null ? null : listBook.Find(c => c.ID == otherConcord.ID);
                    var contractLands = listLand.FindAll(c => (familyConcord != null && c.ConcordId == familyConcord.ID) ||
                                        (otherConcord != null && c.ConcordId == otherConcord.ID));
                    ContractWarrantPrinter printContract = new ContractWarrantPrinter();

                    #region 通过反射等机制定制化具体的业务处理类

                    var temp = WorksheetConfigHelper.GetInstance(printContract);
                    if (temp != null && temp is ContractWarrantPrinter)
                    {
                        printContract = (ContractWarrantPrinter)temp;
                        tempPath = Path.Combine(TheApp.GetApplicationPath(), temp.TemplatePath);
                    }

                    #endregion 通过反射等机制定制化具体的业务处理类

                    printContract.ZoneList = zonelist;
                    printContract.ParentsToProvince = parentsToProvince;
                    printContract.dbContext = argument.DbContext;
                    printContract.CurrentZone = argument.CurrentZone;
                    printContract.RegeditBook = familyBook;
                    printContract.Book = familyBook;
                    printContract.Concord = familyConcord;
                    printContract.OtherBook = otherBook;
                    printContract.OtherConcord = otherConcord;
                    printContract.LandCollection = contractLands;
                    printContract.BatchExport = true;
                    printContract.Contractor = person;
                    printContract.Tissue = tissue;
                    printContract.DictList = listDict;
                    printContract.TempleFilePath = tempPath;
                    printContract.UseExcel = argument.ExtendUseExcelDefine.WarrantExtendByExcel;
                    printContract.BookPersonNum = argument.BookPersonNum;
                    printContract.BookLandNum = argument.BookLandNum;
                    printContract.BookNumSetting = argument.BookNumSetting;
                    string fileName = argument.FileName;
                    printContract.ExportContractLand(fileName);
                    string strDesc = string.Format("{0}", excelName + person.Name);
                    this.ReportProgress(1 + percentOfPerson * percentIndex, strDesc);
                    percentIndex++;
                }

                result = true;
                string info = string.Format("{0}导出{1}户承包方,共{2}张权证证书", excelName, listPerson.Count, percentIndex - 1);
                this.ReportInfomation(info);
                this.ReportProgress(100, "完成");

                listDict = null;
            }
            catch (Exception ex)
            {
                result = false;
                this.ReportError("导出证书失败!");
                YuLinTu.Library.Log.Log.WriteException(this, "ExportWarrent(导出承包权证数据到Word文档)", ex.Message + ex.StackTrace);
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

        #endregion Method—Private

        #region Method—Helper

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

        #endregion Method—Helper
    }
}