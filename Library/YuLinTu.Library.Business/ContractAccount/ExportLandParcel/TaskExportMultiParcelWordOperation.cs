/*
 * (C) 2025  鱼鳞图公司版权所有,保留所有权利 
 */
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using YuLinTu.Data;
using YuLinTu.Diagrams;
using YuLinTu.Library.Entity;
using YuLinTu.Library.WorkStation;
using YuLinTu.Windows;

namespace YuLinTu.Library.Business
{
    /// <summary>
    /// 导出地块示意图任务类
    /// </summary>
    public class TaskExportMultiParcelWordOperation : Task
    {
        #region Ctor

        /// <summary>
        /// 构造函数
        /// </summary>
        public TaskExportMultiParcelWordOperation()
        { }

        #endregion

        #region Field

        private string openFilePath;  //打开文件路径
        private ContractBusinessParcelWordSettingDefine ParcelWordSettingDefine = ContractBusinessParcelWordSettingDefine.GetIntence();

        #endregion

        #region Property
        public bool? IsStockLand { get; set; }

        // 村地域的地块集合
        public List<ContractLand> VillageContractLands { get; set; } = new List<ContractLand>();

        #endregion

        #region Method—Override

        /// <summary>
        /// 开始执行任务
        /// </summary>
        protected override void OnGo()
        {
            TaskExportMultiParcelWordArgument argument = Argument as TaskExportMultiParcelWordArgument;
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

                var settingDefine = ContractBusinessParcelWordSettingDefine.GetIntence();
                if (VillageContractLands.Count == 0 && listPerson.Count > 1)//如果未获取地块并且承包方数量大于1（等于1时单独查找，不获取整村数据）
                {
                    VillageContractLands = ContractLandHeler.GetCurrentVillageContractLand(argument.CurrentZone, dbContext, settingDefine.Neighborlandbufferdistence);
                }
                var listLand = VillageContractLands.FindAll(t => t.ZoneCode.StartsWith(zone.FullCode));// landStation.GetCollection(zone.FullCode, eLevelOption.Self);
                if (settingDefine.ContainsOtherZoneLand)
                    listLand = VillageContractLands;
                if (listLand == null || listLand.Count == 0)
                {
                    this.ReportProgress(100, null);
                    this.ReportWarn(string.Format("{0}未获取承包地块数据!", zone.FullName));
                    return;
                }
                // 对于确权确股的，需要把股地也加进来
                if (IsStockLand == null)
                {
                    foreach (var person in listPerson)
                    {
                        var stockLandsvp = dbContext.CreateBelongRelationWorkStation().GetLandByPerson(person.ID, person.ZoneCode);
                        foreach (var stockLand in stockLandsvp)
                        {
                            if (!listLand.Any(t => t.ID.Equals(stockLand.ID)))
                                listLand.Add(stockLand);
                        }
                    }
                }
                bool canOpen = ExportLandMultiParcelWord(argument, listPerson, listLand, listDict);
                if (canOpen)
                    CanOpenResult = true;
            }
            catch (Exception ex)
            {
                YuLinTu.Library.Log.Log.WriteException(this, "TaskExportPublishWordOperation(导出地块示意图任务)", ex.Message + ex.StackTrace);
                this.ReportException(ex, "导出地块示意图出现异常! " + ex.Message);
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
        /// 导出地块示意图业务
        /// </summary>
        public bool ExportLandMultiParcelWord(TaskExportMultiParcelWordArgument argument, List<VirtualPerson> listPerson, List<ContractLand> listLand,
            List<Dictionary> listDict)
        {
            bool result = false;
            if (IsStockLand != null)
                listLand = listLand.FindAll(o => o.IsStockLand == IsStockLand);
            List<XZDW> listLine = new List<XZDW>(1000);
            List<DZDW> listPoint = new List<DZDW>(1000);
            List<MZDW> listPolygon = new List<MZDW>(1000);
            List<ContractLand> listGeoLand = new List<ContractLand>(1000);
            List<ContractConcord> listConcord = new List<ContractConcord>(1000);
            List<ContractRegeditBook> listBook = new List<ContractRegeditBook>(1000);
            List<CollectivityTissue> listTissue = new List<CollectivityTissue>(1000);
            List<Dictionary> dictDKLB = new List<Dictionary>(1000);
            List<BuildLandBoundaryAddressDot> listDot = new List<BuildLandBoundaryAddressDot>(10000);
            List<BuildLandBoundaryAddressCoil> listCoil = new List<BuildLandBoundaryAddressCoil>(10000);
            List<BuildLandBoundaryAddressDot> listValidDot = new List<BuildLandBoundaryAddressDot>(10000);
            DiagramsView viewOfAllMultiParcel = null;
            DiagramsView viewOfNeighorParcels = null;//临宗出图大小
            try
            {
                if (argument.CurrentZone == null)
                {
                    this.ReportError("未选择导出数据的地域!");
                    return result;
                }
                this.ReportProgress(1, "开始");
                var zonelist = GetParentZone(argument.CurrentZone, argument.DbContext);
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
                if (listTissue.Count == 0)
                {
                    var tissue = senderStation.Get(argument.CurrentZone.ID);

                    if (tissue == null)
                    {
                        listTissue = senderStation.GetTissues(argument.CurrentZone.FullCode, eLevelOption.Self);
                    }
                    else
                    {
                        listTissue.Add(tissue);
                    }
                }
                dictDKLB = listDict.FindAll(c => !string.IsNullOrEmpty(c.GroupCode) && c.GroupCode == DictionaryTypeInfo.DKLB);
                listCoil = coilStation.GetByZoneCode(argument.CurrentZone.FullCode, eLevelOption.Self);
                listDot = dotStation.GetByZoneCode(argument.CurrentZone.FullCode, eLevelOption.Self);
                listValidDot = listDot.FindAll(c => c.IsValid == true);
                var listGeoLands = listLand.FindAll(c => c.Shape != null);
                listGeoLand = InitalizeAgricultureLandSortValue(listGeoLands);
                if (listGeoLand.Count == 0)
                {
                    this.ReportInfomation("当前地域" + argument.CurrentZone.FullName + "没有地块进行导出，请检查配置选项!");
                }
                listLine = lineStation.GetByZoneCode(argument.CurrentZone.FullCode);
                listPoint = PointStation.GetByZoneCode(argument.CurrentZone.FullCode);
                listPolygon = PolygonStation.GetByZoneCode(argument.CurrentZone.FullCode);
                listLine.RemoveAll(l => l.Shape == null);
                listPoint.RemoveAll(l => l.Shape == null);
                listPolygon.RemoveAll(l => l.Shape == null);
                Application.Current.Dispatcher.Invoke(new Action(() =>
                {
                    viewOfAllMultiParcel = new DiagramsView();
                    viewOfAllMultiParcel.Paper.Model.Width = 236;//336
                    viewOfAllMultiParcel.Paper.Model.Height = 237;//357
                    viewOfAllMultiParcel.Paper.Model.BorderWidth = 0;
                    viewOfAllMultiParcel.Paper.Model.X = 0;
                    viewOfAllMultiParcel.Paper.Model.Y = 0;

                    viewOfNeighorParcels = new DiagramsView();
                    viewOfNeighorParcels.Paper.Model.Width = 160;
                    viewOfNeighorParcels.Paper.Model.Height = 160;
                    viewOfNeighorParcels.Paper.Model.BorderWidth = 0;
                    viewOfNeighorParcels.Paper.Model.X = 0;
                    viewOfNeighorParcels.Paper.Model.Y = 0;
                }));

                string markDesc = GetMarkDesc(argument.CurrentZone, argument.DbContext);
                int indexOfVp = 1;
                int count = 0;   //统计导出宗地图表个数
                double vpPercent = 99 / (double)listPerson.Count;
                string templatePath = string.Empty;
                if (IsStockLand != null)
                    templatePath = TemplateHelper.WordTemplate(((bool)IsStockLand ? "安徽" : "") + TemplateFile.ParcelWord);
                else
                    templatePath = TemplateHelper.WordTemplate(TemplateFile.ParcelWord);
                string savePathOfImage = System.IO.Path.GetTempPath();
                openFilePath = argument.FileName;
                foreach (var person in listPerson)
                {
                    if (IsStockLand != null)
                    {
                        if ((bool)IsStockLand)
                        {
                            listGeoLand.Clear();
                            var ralations = argument.DbContext.CreateVirtualPersonStation<LandVirtualPerson>().GetRelationsByVpID(person.ID);
                            ralations.ForEach(r => listGeoLand.Add(listLand.Find(g => g.ID == r.LandID)));
                        }
                    }

                    // 是否只包含股地
                    bool isOnlyHaveStock = false;
                    // 排除掉没有地块的户
                    if (!listGeoLand.Any(t => t.OwnerId == person.ID))
                    {
                        // 当通过Person的ID到“权属关系”里找的地块仍然不在地块集合里，则此户应排除
                        var belongRelationStation = argument.DbContext.CreateBelongRelationWorkStation();
                        var landList = belongRelationStation.GetLandByPerson(person.ID, person.ZoneCode);
                        bool flag = false;
                        foreach (var land in landList)
                        {
                            if (listGeoLand.Any(c => c.ID.Equals(land.ID)))
                            {
                                flag = true;
                                isOnlyHaveStock = true;
                                break;
                            }
                        }
                        if (flag == false)
                            continue;
                    }

                    bool isExsitGeo = false;
                    if (isOnlyHaveStock)
                        isExsitGeo = true;
                    else
                        isExsitGeo = listGeoLand.Any(c => c.OwnerId == person.ID) || ((bool)IsStockLand && listGeoLand.Count > 0);
                    string familyNuber = ToolString.ExceptSpaceString(person.FamilyNumber);
                    string savePathOfWord = InitalizeLandImageName(openFilePath, person);  //filePath + @"\" + familyNuber + "-" + person.Name + "-" + TemplateFile.ParcelWord + ".doc";
                    ExportContractLandParcelWord parcelWord = new ExportContractLandParcelWord(argument.DbContext);

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
                    #endregion

                    parcelWord.ViewOfAllMultiParcel = viewOfAllMultiParcel;
                    parcelWord.ViewOfNeighorParcels = viewOfNeighorParcels;
                    parcelWord.CurrentZone = argument.CurrentZone;
                    parcelWord.VillageZone = VillageZone;
                    parcelWord.SavePathOfImage = savePathOfImage;
                    parcelWord.SavePathOfWord = savePathOfWord;
                    parcelWord.DictList = listDict;
                    parcelWord.DictDKLB = dictDKLB;
                    parcelWord.ZoneList = zonelist;
                    parcelWord.IsStockLand = IsStockLand;
                    parcelWord.ListGeoLand = listGeoLand;
                    parcelWord.ListLineFeature = listLine;
                    parcelWord.ListPointFeature = listPoint;
                    parcelWord.ListPolygonFeature = listPolygon;
                    parcelWord.ListConcord = listConcord;
                    parcelWord.ListBook = listBook;
                    parcelWord.ListTissue = listTissue;
                    parcelWord.ListDot = listDot;
                    parcelWord.ListValidDot = listValidDot;
                    parcelWord.ListCoil = listCoil;
                    parcelWord.ParcelCheckDate = DateTime.Now;
                    parcelWord.ParcelDrawDate = DateTime.Now;
                    parcelWord.VillageContractLands = this.VillageContractLands;
                    parcelWord.OwnedPerson = person;
                    parcelWord.OpenTemplate(templatePath);
                    if (isExsitGeo)
                    {
                        parcelWord.SaveAsMultiFile(person, savePathOfWord, ParcelWordSettingDefine.SaveParcelPCAsPDF);
                        count++;
                    }
                    this.ReportProgress((int)(1 + vpPercent * indexOfVp), string.Format("{0}", markDesc + person.Name));
                    indexOfVp++;

                    if (indexOfVp % 10 == 0)
                    {
                        GC.Collect();
                        GC.WaitForFullGCComplete();
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
                this.ReportError("导出地块示意图失败，请检查有无发包方数据!");
                YuLinTu.Library.Log.Log.WriteException(this, "ExportLandMultiParcelWord(批量导出地块示意图)", ex.Message + ex.StackTrace);
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
        /// 初始化地块示意图名称
        /// </summary>
        /// <returns></returns>
        private string InitalizeLandImageName(string filePath, VirtualPerson family)
        {
            if (family == null)
            {
                return "";
            }
            string imagePath = filePath + "\\" + family.ZoneCode.PadRight(14, '0') + "\\" + $"{family.Name}{family.Number}";
            if (!Directory.Exists(imagePath))
            {
                Directory.CreateDirectory(imagePath);
            }

            string imageName = imagePath + "\\" + "DKSYT" + family.ZoneCode.PadRight(14, '0');
            int number = 0;
            Int32.TryParse(family.FamilyNumber, out number);
            imageName += string.Format("{0:D4}", number);
            imageName += "J";
            return imageName;
        }

        #endregion

        #region Method—Private
        /// <summary>
        /// 获取地域集合
        /// </summary>
        public List<Zone> GetParentZone(Zone zone, IDbContext dbContext)
        {
            ModuleMsgArgs arg = new ModuleMsgArgs();
            arg.Datasource = dbContext;
            arg.Parameter = zone;
            arg.Name = ZoneMessage.ZONE_PARENTS_ZONE;
            TheBns.Current.Message.Send(this, arg);
            return (arg.ReturnValue as List<Zone>);
        }
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
