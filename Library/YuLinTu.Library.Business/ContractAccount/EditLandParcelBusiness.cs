using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using YuLinTu.Library.Entity;
using YuLinTu.Windows;
using YuLinTu.Unity;
using YuLinTu;
using YuLinTu.Data;
using YuLinTu.Library.Repository;
using System.ComponentModel;
using System.Threading;
using System.Diagnostics;
using YuLinTu.Library.WorkStation;
using YuLinTu.Library.Office;
using System.IO;
using System.Collections.ObjectModel;
using YuLinTu.Spatial;
using YuLinTu.Data.Shapefile;
using YuLinTu.Data.Dynamic;
using System.Collections;
using System.Reflection;
using YuLinTu.Diagrams;
using System.Threading.Tasks;

namespace YuLinTu.Library.Business
{
    /// <summary>
    /// 编辑地块示意图中地块示意图导出
    /// </summary>
    public class EditLandParcelBusiness : Task
    {
        #region Fields

        private IDbContext dbContext;
        //private bool isErrorRecord;

        private eVirtualType virtualType;
        private IContractLandWorkStation landStation;//承包台账地块业务逻辑层
        private IVirtualPersonWorkStation<LandVirtualPerson> tableStation;  //承包台账(承包方)Station
        //private double projectionUnit = 0.0015; //空间参考系单位换算亩系数

        private IVirtualPersonWorkStation<LandVirtualPerson> landVirtualPersonStation;
        private bool _isCheckLandNumberRepeat = true;
        private ContractBusinessParcelWordSettingDefine ParcelWordSettingDefine = ContractBusinessParcelWordSettingDefine.GetIntence();

        #endregion

        #region Properties

        /// <summary>
        /// 表格类型
        /// </summary>
        public int TableType { get; set; }

        /// <summary>
        /// 是否验证地块编码重复 不要删除此属性  安徽插件使用
        /// </summary>
        public bool IsCheckLandNumberRepeat
        {
            get { return _isCheckLandNumberRepeat; }
            set { _isCheckLandNumberRepeat = value; }
        }

        /// <summary>
        /// 表格类型
        /// </summary>
        public eContractAccountType ArgType { get; set; }

        /// <summary>
        /// 日期
        /// </summary>
        public DateTime? Date { get; set; }

        /// <summary>
        /// 承包方类型
        /// </summary>
        public eVirtualType VirtualType
        {
            get { return virtualType; }
            set { virtualType = value; }
        }

        /// <summary>
        /// 任务
        /// </summary>
        public TaskContractAccountArgument meta
        { get; set; }

        /// <summary>
        /// 承包台账常规设置实体
        /// </summary>
        public ContractBusinessSettingDefine SettingDefine = ContractBusinessSettingDefine.GetIntence();

        /// <summary>
        /// 导出公示调查表日期设置
        /// </summary>
        public DateSetting PublishDateSetting { get; set; }

        /// <summary>
        /// 系统信息常规设置
        /// </summary>
        public SystemSetDefine SystemSet = SystemSetDefine.GetIntence();

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
        /// <summary>
        /// 是否批量
        /// </summary>
        public bool IsBatch { get; set; }

        /// <summary>
        /// 当前百分比
        /// </summary>
        public double percent { get; set; }

        #region Properties - 导入地块图斑
        /// <summary>
        /// 按照地块编码绑定导入-导入地块图斑设置
        /// </summary>
        public bool UseLandCodeBindImport { get; set; }

        /// <summary>
        /// 按照承包方信息绑定导入-导入地块图斑设置
        /// </summary>
        public bool UseContractorInfoImport { get; set; }

        /// <summary>
        /// 按照承包方户号绑定导入-导入地块图斑设置
        /// </summary>
        public bool UseContractorNumberImport { get; set; }

        /// <summary>
        /// 地块图斑导入设置实体
        /// </summary>
        public ImportAccountLandShapeSettingDefine ImportLandShapeInfoDefine =
            ImportAccountLandShapeSettingDefine.GetIntence();


        /// <summary>
        /// 读取的shp所有字段名称
        /// </summary>
        public List<KeyValue<int, string>> shapeAllcolNameList { get; set; }

        /// <summary>
        /// 空间参考系
        /// </summary>
        public YuLinTu.Spatial.SpatialReference shpRef { get; set; }
        #endregion

        #endregion

        #region Ctor

        public EditLandParcelBusiness(IDbContext db)
        {
            dbContext = db;
            landStation = db == null ? null : db.CreateContractLandWorkstation();
            landVirtualPersonStation = db == null ? null : db.CreateVirtualPersonStation<LandVirtualPerson>();
            tableStation = db == null ? null : db.CreateVirtualPersonStation<LandVirtualPerson>();
        }

        #endregion

        #region Methods


        #region 数据处理

        /// <summary>
        /// 根据地域编码和匹配等级获取承包台账地块集合
        /// </summary>
        /// <param name="zoneCode">地域编码</param>
        /// <param name="option">匹配等级</param>
        /// <returns>地块集合</returns>
        public List<ContractLand> GetCollection(string zoneCode, eLevelOption option)
        {
            List<ContractLand> list = null;
            if (!CanContinue() || string.IsNullOrEmpty(zoneCode))
            {
                return list;
            }
            try
            {
                list = ContractLandHeler.GetParcelLands(zoneCode, dbContext, false);
                //list = landStation.GetCollection(zoneCode, option);
            }
            catch (Exception ex)
            {
                YuLinTu.Library.Log.Log.WriteException(this, "GetCollection(获取承包台账地块集合)", ex.Message + ex.StackTrace);
                this.ReportError("获取承包台账地块集合失败," + ex.Message);
            }
            return list;
        }



        #endregion

        #region 地籍数据处理

        /// <summary>
        /// 导出单户的多地块示意图
        /// </summary>
        /// <param name="currentZone">当前地域</param>
        /// <param name="person">当前户</param>
        /// <param name="filePath">保存文件路径</param>
        /// <param name="save">是否保存</param>
        public void ExportLandParcelWord(Zone currentZone, VirtualPerson person, string filePath, List<DiagramsView> LstViewOfNeighorParcels, bool save = false, string imageSavePath = "", bool? isStockLand = false)
        {
            List<ContractLand> listLand = new List<ContractLand>();
            List<ContractLand> listGeoLand = new List<ContractLand>(1000);
            List<XZDW> listLine = new List<XZDW>(1000);
            List<DZDW> listPoint = new List<DZDW>(1000);
            List<MZDW> listPolygon = new List<MZDW>(1000);
            List<ContractConcord> listConcord = new List<ContractConcord>(1000);
            List<ContractRegeditBook> listBook = new List<ContractRegeditBook>(1000);
            List<CollectivityTissue> listTissue = new List<CollectivityTissue>(1000);
            List<Dictionary> dictDKLB = new List<Dictionary>(500);
            List<BuildLandBoundaryAddressDot> listDot = new List<BuildLandBoundaryAddressDot>(1000);
            List<BuildLandBoundaryAddressCoil> listCoil = new List<BuildLandBoundaryAddressCoil>(1000);
            List<BuildLandBoundaryAddressDot> listValidDot = new List<BuildLandBoundaryAddressDot>(1000);
            DiagramsView viewOfAllMultiParcel = null;
            DiagramsView viewOfNeighorParcels = null;
            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                viewOfAllMultiParcel = new DiagramsView();
                viewOfNeighorParcels = new DiagramsView();
            }));
            try
            {
                var dotStation = dbContext.CreateBoundaryAddressDotWorkStation();
                var lineStation = dbContext.CreateXZDWWorkStation();
                var PointStation = dbContext.CreateDZDWWorkStation();
                var PolygonStation = dbContext.CreateMZDWWorkStation();
                string templatePath = string.Empty;
                if (isStockLand != null)
                    templatePath = TemplateHelper.WordTemplate(((bool)isStockLand ? "安徽" : "") + TemplateFile.ParcelWord);
                else
                    templatePath = TemplateHelper.WordTemplate(TemplateFile.ParcelWord);
                string savePathOfImage = string.IsNullOrEmpty(imageSavePath) ? filePath : imageSavePath;
                string savePathOfWord = InitalizeLandImageName(filePath, person); // filePath + @"\" + familyNuber + "-" + person.Name + "-" + TemplateFile.ParcelWord + ".doc";
                listLand = ContractLandHeler.GetParcelLands(currentZone.FullCode, dbContext, ParcelWordSettingDefine.ContainsOtherZoneLand);
                //listLand = GetCollection(currentZone.FullCode, eLevelOption.Self);
                var VillageZone = GetParent(currentZone);
                if (isStockLand != null)
                {
                    if ((bool)isStockLand)
                    {
                        var stockLandsvp = dbContext.CreateBelongRelationWorkStation().GetLandByPerson(person.ID, currentZone.FullCode);
                        listLand = stockLandsvp;
                    }
                    else
                    {
                        // 导出确权地块示意图，要排除掉股地
                        listLand = listLand.FindAll(c => !c.IsStockLand);
                    }
                }
                listGeoLand = listLand.FindAll(c => c.Shape != null);
                listGeoLand = InitalizeAgricultureLandSortValue(listGeoLand);
                listLine = lineStation.GetByZoneCode(currentZone.FullCode);
                dictDKLB = DictList.FindAll(c => !string.IsNullOrEmpty(c.GroupCode) && c.GroupCode == DictionaryTypeInfo.DKLB);
                listTissue = landStation.GetTissuesByConcord(currentZone);
                listDot = dotStation.GetByZoneCode(currentZone.FullCode, eLevelOption.Self);
                listValidDot = listDot.FindAll(c => c.IsValid == true);
                listPoint = PointStation.GetByZoneCode(currentZone.FullCode);
                listPolygon = PolygonStation.GetByZoneCode(currentZone.FullCode);
                listLine.RemoveAll(l => l.Shape == null);
                listPoint.RemoveAll(l => l.Shape == null);
                listPolygon.RemoveAll(l => l.Shape == null);
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
                #endregion
                parcelWord.LstViewOfNeighorParcels = LstViewOfNeighorParcels;
                parcelWord.IsStockLand = isStockLand;
                parcelWord.ViewOfAllMultiParcel = viewOfAllMultiParcel;
                parcelWord.ViewOfNeighorParcels = viewOfNeighorParcels;
                parcelWord.CurrentZone = currentZone;
                parcelWord.VillageZone = VillageZone;
                parcelWord.DictList = DictList;
                parcelWord.SavePathOfImage = savePathOfImage;
                parcelWord.SavePathOfWord = savePathOfWord;
                parcelWord.DictDKLB = dictDKLB;
                parcelWord.ListTissue = listTissue;
                parcelWord.ListGeoLand = listGeoLand;
                parcelWord.ListLineFeature = listLine;
                parcelWord.ListPointFeature = listPoint;
                parcelWord.ListPolygonFeature = listPolygon;
                parcelWord.ListValidDot = listValidDot;
                parcelWord.ParcelCheckDate = DateTime.Now;
                parcelWord.ParcelDrawDate = DateTime.Now;
                parcelWord.IsSave = save;
                parcelWord.OwnedPerson = person;
                parcelWord.OpenTemplate(templatePath);
                if (!save)
                    parcelWord.PrintPreview(person, savePathOfWord);
                else
                {
                    parcelWord.SaveAsMultiFile(person, savePathOfWord, ParcelWordSettingDefine.SaveParcelPCAsPDF);
                }
            }
            catch (Exception ex)
            {
                this.ReportError(ex.Message + "，请检查有无发包方信息等");
                throw ex;
            }
            finally
            {
                listLand.Clear();
                listGeoLand.Clear();
                listLine.Clear();
                listConcord.Clear();
                listBook.Clear();
                listTissue.Clear();
                dictDKLB.Clear();
                listDot.Clear();
                listCoil.Clear();
                listValidDot.Clear();

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


        #endregion

        #region 工具之初始数据
        /// <summary>
        /// 初始化承包地块编码时，如果只初始化承包地块时，避免与非承包地块编码重复
        /// </summary>
        /// <param name="unitCode"></param>
        /// <param name="index"></param>
        /// <param name="landsOfStatus"></param>
        /// <returns></returns>
        private int CheckLandNumber(string unitCode, int index, List<ContractLand> landsOfStatus)
        {
            string landnumber = unitCode + index.ToString().PadLeft(5, '0');
            var findLands = landsOfStatus.FindAll(c => c.LandNumber == landnumber);
            if (findLands != null && findLands.Count > 0)
                return CheckLandNumber(unitCode, index + 1, landsOfStatus);
            else
                return index;
        }



        #endregion

        #region 辅助方法

        /// <summary>
        /// 获取上级地域
        /// </summary>
        /// <param name="zone"></param>
        /// <returns></returns>
        public Zone GetParent(Zone zone)
        {
            ModuleMsgArgs arg = new ModuleMsgArgs();
            arg.Datasource = dbContext;
            arg.Parameter = zone;
            arg.Name = ZoneMessage.ZONE_PARENT_ZONE;
            TheBns.Current.Message.Send(this, arg);
            return (arg.ReturnValue as Zone);
        }

        /// <summary>
        /// 辅助判断方法
        /// </summary>
        public bool CanContinue()
        {
            if (landStation == null)
            {
                this.ReportError("尚未初始化数据字典的访问接口");
                return false;
            }
            return true;
        }


        #endregion

        #endregion
    }
}
