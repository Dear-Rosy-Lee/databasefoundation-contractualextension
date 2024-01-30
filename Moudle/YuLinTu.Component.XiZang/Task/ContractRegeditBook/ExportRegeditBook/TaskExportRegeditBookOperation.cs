/*
 * (C) 2015  鱼鳞图公司版权所有,保留所有权利 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using YuLinTu.Data;
using YuLinTu.Diagrams;
using YuLinTu.Library.Business;
using YuLinTu.Library.Entity;

namespace YuLinTu.Component.XiZangLZ
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
            IDbContext dbContext = argument.DbContext;
            var zone = argument.CurrentZone;
            try
            {
                var listPerson = argument.SelectedPersons;
                var concordStation = dbContext.CreateConcordStation();
                var bookStation = dbContext.CreateRegeditBookStation();
                var dictStation = dbContext.CreateDictWorkStation();
                var landStation = dbContext.CreateContractLandWorkstation();
                var listDict = dictStation.Get();
                var listConcord = concordStation.GetContractsByZoneCode(zone.FullCode, eLevelOption.Self);
                var listBook = bookStation.GetContractsByZoneCode(zone.FullCode, eLevelOption.Self);
                var listLand = landStation.GetCollection(zone.FullCode, eLevelOption.Self);
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
                bool canOpen = ExportRegeditBook(argument, listPerson, listDict, listConcord, listLand, listBook);
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
            List<Dictionary> listDict, List<ContractConcord> listConcord, List<ContractLand> listLand, List<YuLinTu.Library.Entity.ContractRegeditBook> listBook)
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
                    foreach (var concord in concords)
                    {
                        var concordStation = argument.DbContext.CreateConcordStation();
                        var bookStation = argument.DbContext.CreateRegeditBookStation();
                        var senderStation = argument.DbContext.CreateSenderWorkStation();
                        var landStation = argument.DbContext.CreateContractLandWorkstation();
                        var dotStation = argument.DbContext.CreateBoundaryAddressDotWorkStation();
                        var coilStation = argument.DbContext.CreateBoundaryAddressCoilWorkStation();
                        var lineStation = argument.DbContext.CreateXZDWWorkStation();
                        var PointStation = argument.DbContext.CreateDZDWWorkStation();
                        var PolygonStation = argument.DbContext.CreateMZDWWorkStation();
                        var VillageZone = GetParent(argument.CurrentZone, argument.DbContext);
                        listConcord = concordStation.GetByZoneCode(argument.CurrentZone.FullCode);
                        listBook = bookStation.GetByZoneCode(argument.CurrentZone.FullCode, eSearchOption.Precision);
                        listTissue = landStation.GetTissuesByConcord(argument.CurrentZone);
                        dictDKLB = listDict.FindAll(c => !string.IsNullOrEmpty(c.GroupCode) && c.GroupCode == DictionaryTypeInfo.DKLB);
                        listCoil = coilStation.GetByZoneCode(argument.CurrentZone.FullCode, eLevelOption.Self);
                        listDot = dotStation.GetByZoneCode(argument.CurrentZone.FullCode, eLevelOption.Self);
                        listValidDot = listDot.FindAll(c => c.IsValid == true);
                        listGeoLand = listLand.FindAll(c => c.Shape != null);
                        listLine = lineStation.GetByZoneCode(argument.CurrentZone.FullCode);
                        listPoint = PointStation.GetByZoneCode(argument.CurrentZone.FullCode);
                        listPolygon = PolygonStation.GetByZoneCode(argument.CurrentZone.FullCode);
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
                        if (!listBook.Exists(c => !string.IsNullOrEmpty(c.RegeditNumber) && c.RegeditNumber == concord.ConcordNumber))
                            continue;
                        ContractRegeditBookPrinterData data = new ContractRegeditBookPrinterData(concord);
                        data.CurrentZone = argument.CurrentZone;
                        data.DbContext = argument.DbContext;
                        data.SystemDefine = argument.SystemDefine;
                        data.DictList = listDict == null ? new List<Dictionary>() : listDict;
                        data.InitializeInnerData();
                        string tempPath = TemplateHelper.WordTemplate("农村土地承包经营权登记簿");
                        ContractRegeditBookWork regeditBookWord = new ContractRegeditBookWork(argument.DbContext);
                        regeditBookWord.ViewOfAllMultiParcel = viewOfAllMultiParcel;
                        regeditBookWord.CurrentZone = argument.CurrentZone;
                        regeditBookWord.VillageZone = VillageZone;
                        regeditBookWord.DictList = listDict;
                        regeditBookWord.DictDKLB = dictDKLB;
                        regeditBookWord.DbContext = argument.DbContext;
                        regeditBookWord.ListGeoLand = listGeoLand;
                        regeditBookWord.ListLineFeature = listLine;
                        regeditBookWord.ListPointFeature = listPoint;
                        regeditBookWord.ListPolygonFeature = listPolygon;
                        regeditBookWord.DictList = listDict == null ? new List<Dictionary>() : listDict;
                        regeditBookWord.OpenTemplate(tempPath);
                        regeditBookWord.SavePathOfImage = System.IO.Path.GetTempPath();
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

        #endregion

        #region Method—Helper

        #endregion
    }
}
