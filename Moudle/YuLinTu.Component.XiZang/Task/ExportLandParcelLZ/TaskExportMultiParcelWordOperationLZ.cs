/*
 * (C) 2015  鱼鳞图公司版权所有,保留所有权利 
 */
using System;
using System.Collections.Generic;
using System.IO;
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
    /// 导出标准地块示意图任务类
    /// </summary>
    public class TaskExportMultiParcelWordOperationLZ : Task
    {
        #region Ctor

        /// <summary>
        /// 构造函数
        /// </summary>
        public TaskExportMultiParcelWordOperationLZ()
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
            TaskExportMultiParcelWordArgumentLZ argument = Argument as TaskExportMultiParcelWordArgumentLZ;
            if (argument == null)
            {
                return;
            }
            IDbContext dbContext = argument.DbContext;
            var zone = argument.CurrentZone;
            try
            {
                var listPerson = argument.SelectedPersons;
                var landStation = dbContext.CreateContractLandWorkstation();
                var dictStation = dbContext.CreateDictWorkStation();
                var listDict = dictStation.Get();
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
                bool canOpen = ExportLandMultiParcelWord(argument, listPerson, listLand, listDict);
                if (canOpen)
                    CanOpenResult = true;
            }
            catch (Exception ex)
            {
                YuLinTu.Library.Log.Log.WriteException(this, "TaskExportPublishWordOperation(导出标准地块示意图任务)", ex.Message + ex.StackTrace);
                this.ReportException(ex, "导出标准地块示意图出现异常!");
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
        /// 导出单户多宗示意图业务
        /// </summary>
        public bool ExportLandMultiParcelWord(TaskExportMultiParcelWordArgumentLZ argument, List<VirtualPerson> listPerson, List<ContractLand> listLand,
            List<Dictionary> listDict)
        {
            bool result = false;

            List<XZDW> listLine = new List<XZDW>(1000);
            List<ContractLand> listGeoLand = new List<ContractLand>(1000);
            List<ContractConcord> listConcord = new List<ContractConcord>(1000);
            List<YuLinTu.Library.Entity.ContractRegeditBook> listBook = new List<YuLinTu.Library.Entity.ContractRegeditBook>(1000);
            List<CollectivityTissue> listTissue = new List<CollectivityTissue>(1000);
            List<Dictionary> dictDKLB = new List<Dictionary>(1000);
            List<BuildLandBoundaryAddressDot> listDot = new List<BuildLandBoundaryAddressDot>(10000);
            List<BuildLandBoundaryAddressCoil> listCoil = new List<BuildLandBoundaryAddressCoil>(10000);
            List<BuildLandBoundaryAddressDot> listValidDot = new List<BuildLandBoundaryAddressDot>(10000);
            DiagramsView viewOfAllMultiParcel = null;
            DiagramsView viewOfNeighorParcels = null;
            try
            {
                if (argument.CurrentZone == null)
                {
                    this.ReportError("未选择导出数据的地域!");
                    return result;
                }
                this.ReportProgress(1, "开始");

                var concordStation = argument.DbContext.CreateConcordStation();
                var bookStation = argument.DbContext.CreateRegeditBookStation();
                var senderStation = argument.DbContext.CreateSenderWorkStation();
                var landStation = argument.DbContext.CreateContractLandWorkstation();
                var dotStation = argument.DbContext.CreateBoundaryAddressDotWorkStation();
                var coilStation = argument.DbContext.CreateBoundaryAddressCoilWorkStation();
                var lineStation = argument.DbContext.CreateXZDWWorkStation();
                listConcord = concordStation.GetByZoneCode(argument.CurrentZone.FullCode);
                listBook = bookStation.GetByZoneCode(argument.CurrentZone.FullCode, eSearchOption.Precision);
                listTissue = landStation.GetTissuesByConcord(argument.CurrentZone);
                dictDKLB = listDict.FindAll(c => !string.IsNullOrEmpty(c.GroupCode) && c.GroupCode == DictionaryTypeInfo.DKLB);
                listCoil = coilStation.GetByZoneCode(argument.CurrentZone.FullCode, eSearchOption.Precision, eLandPropertyRightType.AgricultureLand);
                listDot = dotStation.GetByZoneCode(argument.CurrentZone.FullCode, eSearchOption.Precision, ((int)eLandPropertyRightType.AgricultureLand).ToString());
                listValidDot = listDot.FindAll(c => c.IsValid == true);
                listGeoLand = listLand.FindAll(c => c.Shape != null);
                listLine = lineStation.GetByZoneCode(argument.CurrentZone.FullCode);

                Application.Current.Dispatcher.Invoke(new Action(() =>
                {
                    viewOfAllMultiParcel = new DiagramsView();
                    viewOfAllMultiParcel.Paper.Model.Width = 326;
                    viewOfAllMultiParcel.Paper.Model.Height = 398;
                    viewOfAllMultiParcel.Paper.Model.BorderWidth = 0;
                    viewOfAllMultiParcel.Paper.Model.X = 0;
                    viewOfAllMultiParcel.Paper.Model.Y = 0;

                    viewOfNeighorParcels = new DiagramsView();
                    viewOfNeighorParcels.Paper.Model.Width = 150;
                    viewOfNeighorParcels.Paper.Model.Height = 150;
                    viewOfNeighorParcels.Paper.Model.BorderWidth = 0;
                    viewOfNeighorParcels.Paper.Model.X = 0;
                    viewOfNeighorParcels.Paper.Model.Y = 0;
                }));

                string markDesc = GetMarkDesc(argument.CurrentZone, argument.DbContext);
                int indexOfVp = 1;
                int count = 0;   //统计导出宗地图表个数
                double vpPercent = 99 / (double)listPerson.Count;
                string templatePath = TemplateHelper.WordTemplate("农村土地承包经营权标准地块示意图");
                string savePathOfImage = System.IO.Path.GetTempPath();
                openFilePath = argument.FileName;
                foreach (var person in listPerson)
                {
                    bool isExsitGeo = listGeoLand.Any(c => c.OwnerId == person.ID);
                    string familyNuber = ToolString.ExceptSpaceString(person.FamilyNumber);
                    string savePathOfWord = InitalizeLandImageName(openFilePath, person);  //filePath + @"\" + familyNuber + "-" + person.Name + "-" + TemplateFile.ParcelWord + ".doc";
                    ExportContractLandParcelWordLZ parcelWord = new ExportContractLandParcelWordLZ(argument.DbContext);
                    parcelWord.ViewOfAllMultiParcel = viewOfAllMultiParcel;
                    parcelWord.ViewOfNeighorParcels = viewOfNeighorParcels;
                    parcelWord.CurrentZone = argument.CurrentZone;
                    parcelWord.SavePathOfImage = savePathOfImage;
                    parcelWord.SavePathOfWord = savePathOfWord;
                    parcelWord.DictList = listDict;
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
                    if (isExsitGeo)
                    {
                        var savePath = savePathOfWord.Replace("\\\\", "\\");
                        parcelWord.SaveAsMultiFile(person, savePathOfWord, ContractBusinessParcelWordSettingDefine.GetIntence().SaveParcelPCAsPDF);
                        count++;
                    }
                    this.ReportProgress((int)(1 + vpPercent * indexOfVp), string.Format("{0}", markDesc + person.Name));
                    indexOfVp++;

                    if (indexOfVp % 10 == 0)
                    {
                        GC.Collect();
                        GC.WaitForPendingFinalizers();
                        GC.Collect();
                    }

                }

                result = true;
                string info = string.Format("{0}导出{1}户承包方,共{2}个宗地图表", markDesc, listPerson.Count, count);
                this.ReportInfomation(info);
                this.ReportProgress(100, "完成");
            }
            catch (Exception ex)
            {
                result = false;
                this.ReportError("导出标准地块示意图失败!");
                YuLinTu.Library.Log.Log.WriteException(this, "ExportLandMultiParcelWord(批量导出标准地块示意图)", ex.Message + ex.StackTrace);
            }
            finally
            {
                listLand.Clear();
                listLand = null;
                listGeoLand.Clear();
                listGeoLand = null;
                listLine.Clear();
                listLine = null;
                listConcord.Clear();
                listConcord = null;
                listBook.Clear();
                listBook = null;
                listTissue.Clear();
                listTissue = null;
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
                    if (viewOfAllMultiParcel != null)
                    {
                        viewOfAllMultiParcel.Dispose();
                        viewOfAllMultiParcel = null;
                    }
                    if (viewOfNeighorParcels != null)
                    {
                        viewOfNeighorParcels.Dispose();
                        viewOfNeighorParcels = null;
                    }
                }));

                GC.Collect();
                GC.WaitForPendingFinalizers();
                GC.Collect();
            }
            return result;
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
            string imagePath = filePath + "\\" + family.Name;   //取承包方名作为上级文件夹名
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

        #endregion

        #region Method—Private

        /// <summary>
        ///  获取上级地域
        /// </summary>
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
