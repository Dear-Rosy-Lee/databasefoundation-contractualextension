/*
 * (C) 2025  鱼鳞图公司版权所有,保留所有权利 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YuLinTu.Library.Entity;
using System.Windows.Forms;
using System.IO;
using System.Collections;
using System.Security.Cryptography;
using System.Web;
using System.Net;
using YuLinTu.PropertyRight.ContractLand;
using YuLinTu.Data;
using YuLinTu.Business.ContractLand.Exchange2;
using YuLinTu.PropertyRight.Services.Client;
using YuLinTu.Windows;
using YuLinTu.Library.Business;

namespace YuLinTu.Library.Business
{
    /// <summary>
    /// 导出压缩包
    /// </summary>
    public partial class ArcLandExportProgress : Task
    {
        #region Fields

        private IDbContext db;//属性数据库
        private int currentIndex = 1;//当前进度值
        private int landIndex = 1;//地块索引值
        private bool isFamilyCoding;//是否按照户进行编码
        private ExLandContractorCollection agriLandCollection;//产权产籍承包地集合
        private bool checkFamilyName = false;//是否检查户主名称
        private AgriLandEntity agriEntity;//农用地实体
        //private BuildLandPropertyCollection yardCollection;//宅基地集合
        private bool showInformation;//显示提示信息
        private LanderType landorType;
        private bool isStandCode;
        private ServiceSetDefine ServiceSetDefine = ServiceSetDefine.GetIntence();
        private UploadSettingDefine UploadSettingDefine = UploadSettingDefine.GetIntence();
        #endregion

        #region Propertys

        /// <summary>
        /// 当前地域
        /// </summary>
        public Zone CurrentZone { get; set; }

        /// <summary>
        /// 操作类型,,导入压缩包 1，还是上传 0
        /// </summary>
        public int OperateType { get; set; }

        /// <summary>
        /// 宗地操作对象
        /// </summary>
        //public LandOperatorEventArgs LandOperator { get; set; }

        /// <summary>
        /// 承包方集合
        /// </summary>
        //public List<VirtualPerson> VirPersonCollection { get; set; }

        /// <summary>
        /// 是否是农业部要求压缩包
        /// </summary>
        //public bool AgricultureType { get; set; }

        /// <summary>
        /// 台账常规设置
        /// </summary>
        public ContractBusinessSettingDefine ContractBusinessSettingDefine = ContractBusinessSettingDefine.GetIntence();


        /// <summary>
        /// 行政地域常规设置
        /// </summary>
        public ZoneDefine ZoneDefine = ZoneDefine.GetIntence();


        #endregion

        #region Ctor

        public ArcLandExportProgress()
        {

        }

        #endregion

        #region Methods

        /// <summary>
        /// 开始处理
        /// </summary>
        /// <param name="arg"></param>
        protected override void OnGo()
        {
            ArcLandImporArgument arg = Argument as ArcLandImporArgument;
            bool canContinue = InitalizeData(arg);
            if (!canContinue)
            {
                return;
            }
            try
            {
                db = arg.Database;
                CurrentZone = arg.CurrentZone.Clone() as Zone;
                OperateType = arg.OpratorName == "UpLoad" ? 0 : 1;
                landorType = arg.LandorType;
                AgriLandProgress(arg.FileName);
                db = null;
                this.ReportProgress(100, "完成");
                CanOpenResult = false;
            }
            catch (System.Exception ex)
            {
                this.ReportException(ex, ex.Message);
            }
        }

        public override void OpenResult()
        {
            ArcLandImporArgument arg = Argument as ArcLandImporArgument;
            System.Diagnostics.Process.Start(arg.FileName);
            base.OpenResult();
        }

        /// <summary>
        /// 初始化数据
        /// </summary>
        /// <param name="metadata"></param>
        /// <returns></returns>
        private bool InitalizeData(ArcLandImporArgument metadata)
        {
            if (metadata == null)
            {
                this.ReportException(null, "参数错误!");
                return false;
            }
            if (metadata.Database == null)
            {
                this.ReportException(null, "数据库参数错误!");
                return false;
            }
            if (metadata.CurrentZone == null)
            {
                this.ReportException(null, "地域为空!");
                return false;
            }

            //currentIndex = 1;
            //string value = ToolConfiguration.GetSpecialAppSettingValue("ExportPackageCheckFamilyName", "false");
            //Boolean.TryParse(value, out checkFamilyName);
            //homeLandCollection = new ExHomeSteadLandEntityCollection();
            //entityCollection = new ExLandEntityCollection();
            //consLandCollection = new ExConstructionLandEntityCollection();
            //houseCollection = new ExHouseLandEntityCollection();//房屋数据集合
            //AgricultureType = YuLinTu.Library.Business.AgricultureSetting.IsAgricultureLandPackage;
            return true;
        }

        #endregion

        #region 农村土地承包经营权基础数据处理

        /// <summary>
        /// 地块处理
        /// </summary>
        private void AgriLandProgress(string filePath)
        {
            string fileName = filePath + @"\";
            List<Zone> zoneCollection = db.CreateZoneWorkStation().GetChildren(CurrentZone.FullCode, eLevelOption.SelfAndSubs);
            var personStation = db.CreateVirtualPersonStation<LandVirtualPerson>();
            showInformation = zoneCollection.Count <= 1;
            var zoneDefine = GetSystemSetting();
            isStandCode = zoneDefine == null ? true : zoneDefine.UseStandCode;
            if (!isStandCode && CurrentZone.Level == eZoneLevel.Group)
                CurrentZone.FullCode = CurrentZone.FullCode.Substring(0, 12) + "00" + CurrentZone.FullCode.Substring(12, 2);
            agriEntity = new AgriLandEntity() { CurrentZone = CurrentZone };
            agriEntity.Tissues = new List<CollectivityTissue>();
            InitalizeDirectory();
            this.ReportProgress(1, "正在获取数据...");
            AgricultureLandDataProgress(zoneCollection, true, "", filePath, fileName, isStandCode);
            agriEntity = null;
            zoneCollection = null;
            db.CloseConnection();
            GC.Collect();
        }

        /// <summary>
        /// 获取系统配置
        /// </summary>
        private ZoneDefine GetSystemSetting()
        {
            SettingsProfileCenter systemCenter = TheApp.Current.GetSystemSettingsProfileCenter();  //系统配置
            var profile = systemCenter.GetProfile<ZoneDefine>();
            var section = profile.GetSection<ZoneDefine>();
            return section.Settings;
        }

        /// <summary>
        /// 初始化错误记录文件目录
        /// </summary>
        private void InitalizeDirectory()
        {
            if (!Directory.Exists(System.Windows.Forms.Application.StartupPath + @"\Error"))
            {
                Directory.CreateDirectory(System.Windows.Forms.Application.StartupPath + @"\Error");
            }
            string fileName = System.Windows.Forms.Application.StartupPath + @"\Error\DataRecord.txt";
            using (System.IO.StreamWriter writer = new System.IO.StreamWriter(fileName))
            {
                writer.WriteLine(System.DateTime.Now.ToString());
            }
        }

        /// <summary>
        /// 农用地数据处理
        /// </summary>
        private void AgricultureLandDataProgress(List<Zone> zoneCollection, bool showPersent, string desctiption, string filePath, string fileName, bool isStandCode)
        {
            bool canContinue = true;
            bool needWarning = false;
            landIndex = 1;
            foreach (Zone zone in zoneCollection)
            {
                List<VirtualPerson> vps = db.CreateVirtualPersonStation<LandVirtualPerson>().GetByZoneCode(zone.FullCode, eLevelOption.Self);
                if (vps == null || vps.Count == 0)
                {

                    this.ReportInfomation(zone.FullName + "下没有承包方等相关数据!");
                    continue;
                }
                //string zoneName = SpliteZoneCode(zone);
                //string zipName = fileName + zoneName + ".xml";
                this.ReportAlert(eMessageGrade.Infomation, null, "开始生成" + zone.FullName + "数据...");
                canContinue = AgricultureLandPackageProgress(zone, vps, showPersent, desctiption, filePath, fileName, isStandCode);
                if (!needWarning)
                {
                    needWarning = canContinue;
                }
                vps = null;
            }
            if (needWarning)
            {
                string dataRecord = AppDomain.CurrentDomain.BaseDirectory + @"\Error\DataRecord.txt";
                this.ReportAlert(eMessageGrade.Warn, null, "上传" + CurrentZone.FullName + "下数据存在问题,请在" + dataRecord + "中查看详细信息...");
                if (System.IO.File.Exists(dataRecord))
                {
                    System.Diagnostics.Process.Start(dataRecord);
                }
            }
        }


        /// <summary>
        /// 数据压缩包处理
        /// </summary>
        /// <returns></returns>
        private bool AgricultureLandPackageProgress(Zone zone, List<VirtualPerson> vps, bool showPersent, string desctiption, string filePath, string fileName, bool isStandCode)
        {
            ContractLandRegistrationServiceClient landService = null;
            ArcLandImporArgument arg = Argument as ArcLandImporArgument;
            bool canContinue = false;
            ArrayList fileList = new ArrayList();
            List<EX_CBJYQ_DJB> bookCollection = new List<EX_CBJYQ_DJB>();
            List<EX_CBJYQ_DJB> DCbookCollection = new List<EX_CBJYQ_DJB>();
            List<EX_CBJYQ_DJB> DJbookCollection = new List<EX_CBJYQ_DJB>();
            if (OperateType == 1)
            {
                bookCollection = AgriLandPackageProgress(zone, vps, showPersent, desctiption, landService);
            }
            if (OperateType != 1 && UploadSettingDefine.UploadInvestigationData)
            {
                DCbookCollection = GetdcAgriLandPackageProgress(zone, vps, showPersent, desctiption, landService);
            }
            if (OperateType != 1 && UploadSettingDefine.InitalizeRegistrationData)
            {
                DJbookCollection = GetdjAgriLandPackageProgress(zone, vps, showPersent, desctiption, landService);
            }
            if (OperateType != 1 && UploadSettingDefine.UploadInvestigationData && (DCbookCollection == null || DCbookCollection.Count == 0))
            {
                this.ReportAlert(eMessageGrade.Warn, null, string.Format("未获取到调查交换数据，在{0}!", zone.FullName));
                //return false;
            }
            if (OperateType != 1 && UploadSettingDefine.InitalizeRegistrationData && (DJbookCollection == null || DJbookCollection.Count == 0))
            {
                this.ReportAlert(eMessageGrade.Warn, null, string.Format("未获取到登记交换数据，在{0}!", zone.FullName));
                //return false;
            }
            if (OperateType == 1 && (bookCollection == null || bookCollection.Count == 0))
            {
                this.ReportAlert(eMessageGrade.Warn, null, string.Format("未获取到交换数据，在{0}!", zone.FullName));
                return false;
            }
            if (zone.Level == eZoneLevel.Group && !isStandCode && zone.FullCode.Length == 14 && !ZoneDefine.UseStandCode)
            {

                if (OperateType != 1)
                {
                    SubStringCode(DCbookCollection, zone, false);
                    SubStringCode(DJbookCollection, zone, false);
                }
                else
                {
                    SubStringCode(bookCollection, zone, false);
                }
            }
            else if (ZoneDefine.UseStandCode)
            {
                if (OperateType != 1)
                {
                    SubStringCode(DCbookCollection, zone, true);
                    SubStringCode(DJbookCollection, zone, true);
                }
                else
                {
                    SubStringCode(bookCollection, zone, true);
                }
            }
            this.ReportAlert(eMessageGrade.Infomation, null, "生成" + zone.FullName + "数据完毕...");
            if (OperateType == 1)
            {
                string zipName = fileName + eTaskType.InitializationRegister.ToString() + ".xml";
                ToolSerialization.SerializeBinary(zipName, bookCollection);
                fileList.Add(zipName);
                ArcFileProgresss(filePath, fileList, zone, "土地承包经营权");
            }
            else
            {
                landService = ServiceHelper.InitazlieServerData(arg.UserName, arg.SessionCode.ToString(), arg.UseSafeTrans, ServiceSetDefine.BusinessDataAddress);
                string fullcode = zone.FullCode;
                if (!ZoneDefine.UseStandCode && zone.FullCode.Length == 14)
                {
                    fullcode = zone.FullCode.Substring(0, 12) + "00" + zone.FullCode.Substring(12, 2);
                }
                var value = landService.GetZone(fullcode);
                if (value == null)
                {
                    this.ReportAlert(eMessageGrade.Warn, null, string.Format("服务器上不存在{0}行政区域数据!", zone.FullName));
                    return false;
                }
                canContinue = AgriLandPackageUpLoadProgress(showPersent, desctiption, zone, DCbookCollection, DJbookCollection, landService, vps);
            }
            fileList = null;
            agriLandCollection = null;
            DCbookCollection = null;
            GC.Collect();
            return canContinue;
        }

        /// <summary>
        /// 截取集合的地块编码
        /// </summary>
        private void SubStringCode(List<EX_CBJYQ_DJB> bookCollection, Zone zone, bool use14ZoneCode = false)
        {
            string newZoneCode = string.Empty;
            if (zone.FullCode.Length == 14 && use14ZoneCode == false)
            {
                newZoneCode = zone.FullCode.Substring(0, 12) + "00" + zone.FullCode.Substring(12, 2);
            }
            else if (zone.FullCode.Length == 16 && use14ZoneCode)
            {
                newZoneCode = zone.FullCode.Substring(0, 12) + zone.FullCode.Substring(14, 2);
            }
            else if (zone.FullCode.Length == 14 && use14ZoneCode)
            {
                newZoneCode = zone.FullCode;
            }
            foreach (var bookitem in bookCollection)
            {
                bookitem.SZDY = newZoneCode;
                if (bookitem.DJCBD != null)
                {
                    foreach (var DJCBDitem in bookitem.DJCBD)
                    {
                        DJCBDitem.SZDY = newZoneCode;
                    }
                }
                if (bookitem.DJZL != null)
                {
                    foreach (var DJZLitem in bookitem.DJZL)
                    {
                        DJZLitem.SZDY = newZoneCode;
                    }
                }
                if (bookitem.FBF != null)
                {
                    bookitem.FBF.SZDY = newZoneCode;
                }
                if (bookitem.LYXZ != null)
                {
                    foreach (var LYXZitem in bookitem.LYXZ)
                    {
                        LYXZitem.SZDY = newZoneCode;
                    }
                }
            }
        }


        /// <summary>
        /// 更新空间数据
        /// </summary>
        private List<EX_CBJYQ_DJB> AgriLandPackageProgress(Zone zone, List<VirtualPerson> vps, bool showPersent, string description, ContractLandRegistrationServiceClient landService)
        {
            try
            {
                YuLinTu.Business.ContractLand.Exchange2.ExZone exZone = AgriLandZoneMapping(zone);
                agriEntity.InitalizeZoneData(db, zone.FullCode);
                isFamilyCoding = agriEntity.Lands.Any(land => agriEntity.Lands.Any((ld => (ld.CadastralNumber != null && land.CadastralNumber != null && ld.CadastralNumber.Right(4) == land.CadastralNumber.Right(4) && ld.ID != land.ID))));

                AgricultureInformationMapping landMapping = new AgricultureInformationMapping();
                landMapping.DataInstance = db;
                landMapping.OperateType = OperateType;
                landMapping.IsOnLine = landService != null;
                landMapping.LandService = landService;
                landMapping.IsFamilyCoding = isFamilyCoding;
                landMapping.ArgicultureEntity = agriEntity;
                landMapping.IsStandCode = isStandCode;
                landMapping.ContractBusinessSettingDefine = ContractBusinessSettingDefine;
                landMapping.InformationReportged -= landMapping_InformationReportged;
                landMapping.InformationReportged += landMapping_InformationReportged;
                landMapping.ErrorReportged -= landMapping_ErrorReportged;
                landMapping.ErrorReportged += landMapping_ErrorReportged;
                List<EX_CBJYQ_DJB> bookCollection = new List<EX_CBJYQ_DJB>();
                Dictionary<VirtualPerson, List<ContractConcord>> dic = new Dictionary<VirtualPerson, List<ContractConcord>>();

                foreach (VirtualPerson family in vps)
                {
                    List<ContractConcord> concords = agriEntity.Concords.FindAll(cd => (cd.ContracterId != null && cd.ContracterId.HasValue && cd.ContracterId.Value == family.ID));
                    EX_CBJYQ_DJB book = landMapping.InitalizeUnRegisterBook(zone, family, concords);
                    if (book != null)
                    {
                        bookCollection.Add(book);
                        this.ReportProgress(landIndex, description);
                        landIndex++;
                    }
                    if (concords == null || concords.Count == 0)
                    {
                        concords = agriEntity.Concords.FindAll(cd => cd.ContracterName == family.Name);
                    }
                    if (concords != null && concords.Count > 0)
                    {
                        dic.Add(family, concords);
                    }
                }
                foreach (KeyValuePair<VirtualPerson, List<ContractConcord>> pair in dic)
                {
                    foreach (ContractConcord concord in pair.Value)
                    {
                        if (string.IsNullOrEmpty(concord.ConcordNumber))
                        {
                            continue;
                        }
                        EX_CBJYQ_DJB book = landMapping.InitalizeRegisterBook(zone, pair.Key, concord);
                        bookCollection.Add(book);
                    }
                }
                if (landMapping.HasError)
                {
                    bookCollection.Clear();
                }
                landMapping.Clear();
                dic.Clear();
                agriEntity.Disponse();
                GC.Collect();
                return bookCollection;
            }
            catch (SystemException ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());
                this.ReportAlert(eMessageGrade.Error, null, ex.Message);
            }
            return null;
        }

        /// <summary>
        /// 更新空间数据-获取调查交换实体
        /// </summary>
        private List<EX_CBJYQ_DJB> GetdcAgriLandPackageProgress(Zone zone, List<VirtualPerson> vps, bool showPersent, string description, ContractLandRegistrationServiceClient landService)
        {
            try
            {
                YuLinTu.Business.ContractLand.Exchange2.ExZone exZone = AgriLandZoneMapping(zone);
                agriEntity.InitalizeZoneData(db, zone.FullCode);
                isFamilyCoding = agriEntity.Lands.Any(land => agriEntity.Lands.Any((ld => (ld.CadastralNumber != null && land.CadastralNumber != null && ld.CadastralNumber.Right(4) == land.CadastralNumber.Right(4) && ld.ID != land.ID))));

                AgricultureInformationMapping landMapping = new AgricultureInformationMapping();
                landMapping.DataInstance = db;
                landMapping.OperateType = OperateType;
                landMapping.IsOnLine = landService != null;
                landMapping.LandService = landService;
                landMapping.IsFamilyCoding = isFamilyCoding;
                landMapping.ArgicultureEntity = agriEntity;
                landMapping.IsStandCode = isStandCode;
                landMapping.ContractBusinessSettingDefine = ContractBusinessSettingDefine;
                landMapping.InformationReportged -= landMapping_InformationReportged;
                landMapping.InformationReportged += landMapping_InformationReportged;
                landMapping.ErrorReportged -= landMapping_ErrorReportged;
                landMapping.ErrorReportged += landMapping_ErrorReportged;
                List<EX_CBJYQ_DJB> bookCollection = new List<EX_CBJYQ_DJB>();
                Dictionary<VirtualPerson, List<ContractConcord>> dic = new Dictionary<VirtualPerson, List<ContractConcord>>();

                foreach (VirtualPerson family in vps)
                {
                    List<ContractConcord> concords = agriEntity.Concords.FindAll(cd => (cd.ContracterId != null && cd.ContracterId.HasValue && cd.ContracterId.Value == family.ID));

                    EX_CBJYQ_DJB book = landMapping.InitalizeUnRegisterBook(zone, family, concords);
                    bookCollection.Add(book);

                    //landIndex++;
                }
                this.ReportProgress(80, "获取数据完成，开始上传");
                if (landMapping.HasError)
                {
                    bookCollection.Clear();
                }
                landMapping.Clear();
                dic.Clear();
                agriEntity.Disponse();
                GC.Collect();
                return bookCollection;
            }
            catch (SystemException ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());
                this.ReportAlert(eMessageGrade.Error, null, ex.Message);
            }
            return null;
        }

        /// <summary>
        /// 更新空间数据-获取登记交换实体
        /// </summary>
        private List<EX_CBJYQ_DJB> GetdjAgriLandPackageProgress(Zone zone, List<VirtualPerson> vps, bool showPersent, string description, ContractLandRegistrationServiceClient landService)
        {
            try
            {
                YuLinTu.Business.ContractLand.Exchange2.ExZone exZone = AgriLandZoneMapping(zone);
                agriEntity.InitalizeZoneData(db, zone.FullCode);
                isFamilyCoding = agriEntity.Lands.Any(land => agriEntity.Lands.Any((ld => (ld.CadastralNumber != null && land.CadastralNumber != null && ld.CadastralNumber.Right(4) == land.CadastralNumber.Right(4) && ld.ID != land.ID))));

                AgricultureInformationMapping landMapping = new AgricultureInformationMapping();
                landMapping.DataInstance = db;
                landMapping.OperateType = OperateType;
                landMapping.IsOnLine = landService != null;
                landMapping.LandService = landService;
                landMapping.IsFamilyCoding = isFamilyCoding;
                landMapping.ArgicultureEntity = agriEntity;
                landMapping.IsStandCode = isStandCode;
                landMapping.ContractBusinessSettingDefine = ContractBusinessSettingDefine;
                landMapping.InformationReportged -= landMapping_InformationReportged;
                landMapping.InformationReportged += landMapping_InformationReportged;
                landMapping.ErrorReportged -= landMapping_ErrorReportged;
                landMapping.ErrorReportged += landMapping_ErrorReportged;
                List<EX_CBJYQ_DJB> bookCollection = new List<EX_CBJYQ_DJB>();
                Dictionary<VirtualPerson, List<ContractConcord>> dic = new Dictionary<VirtualPerson, List<ContractConcord>>();

                foreach (VirtualPerson family in vps)
                {
                    List<ContractConcord> concords = agriEntity.Concords.FindAll(cd => (cd.ContracterId != null && cd.ContracterId.HasValue && cd.ContracterId.Value == family.ID));

                    if (concords != null && concords.Count > 0)
                    {
                        dic.Add(family, concords);
                    }
                }
                foreach (KeyValuePair<VirtualPerson, List<ContractConcord>> pair in dic)
                {
                    foreach (ContractConcord concord in pair.Value)
                    {
                        if (string.IsNullOrEmpty(concord.ConcordNumber))
                        {
                            continue;
                        }
                        EX_CBJYQ_DJB book = landMapping.InitalizeRegisterBook(zone, pair.Key, concord);
                        bookCollection.Add(book);
                    }
                }
                this.ReportProgress(80, "获取数据完成，开始上传");
                if (landMapping.HasError)
                {
                    bookCollection.Clear();
                }
                landMapping.Clear();
                dic.Clear();
                agriEntity.Disponse();
                GC.Collect();
                return bookCollection;
            }
            catch (SystemException ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());
                this.ReportAlert(eMessageGrade.Error, null, ex.Message);
            }
            return null;
        }

        /// <summary>
        /// 上传数据到b/s
        /// </summary>
        /// <param name="showPersent"></param>
        /// <param name="description"></param>
        private bool AgriLandPackageUpLoadProgress(bool showPersent, string description, Zone zone, List<EX_CBJYQ_DJB> dcbookCollection, List<EX_CBJYQ_DJB> djbookCollection, ContractLandRegistrationServiceClient landService, List<VirtualPerson> vps)
        {
            ArcLandImporArgument arg = Argument as ArcLandImporArgument;
            this.ReportAlert(eMessageGrade.Infomation, null, "开始上传" + zone.FullName + "数据...");
            bool needWarning = false;
            try
            {
                ExhangeCount surveyEntity = new ExhangeCount();
                ExhangeCount registerEntity = new ExhangeCount();
                int count = db.CreateVirtualPersonStation<LandVirtualPerson>().Count(t => t.ZoneCode == zone.FullCode);
                IEnumerable<YuLinTu.PropertyRight.Registration.DataOperationResult> result = null;
                List<EX_CBJYQ_DJB> regeterBooks = new List<EX_CBJYQ_DJB>();
                List<EX_CBJYQ_DJB> dcbooks = new List<EX_CBJYQ_DJB>();

                if (UploadSettingDefine.UploadInvestigationData)
                {
                    dcbooks = dcbookCollection;
                    if (dcbooks != null && dcbooks.Count > 0)
                    {
                        landService.UploadInvestigationDatas(dcbooks);
                    }
                    surveyEntity.FamilyCount = dcbooks.Count;
                    surveyEntity.PersonCount = dcbooks.Sum(bk => bk.DJGYR != null ? bk.DJGYR.Count : 0);
                    surveyEntity.LandCount = dcbooks.Sum(bk => bk.DJCBD != null ? bk.DJCBD.Count : 0);

                }
                if (UploadSettingDefine.InitalizeRegistrationData)
                {
                    regeterBooks = djbookCollection;
                    if (regeterBooks != null && regeterBooks.Count > 0)
                    {
                        result = landService.InitalizeRegistration(regeterBooks);
                    }
                    registerEntity.FamilyCount = regeterBooks.Count;
                    registerEntity.PersonCount = regeterBooks.Sum(bk => bk.DJGYR != null ? bk.DJGYR.Count : 0);
                    registerEntity.LandCount = regeterBooks.Sum(bk => bk.DJCBD != null ? bk.DJCBD.Count : 0);
                }
                YuLinTu.PropertyRight.Registration.DataOperationResult res = new PropertyRight.Registration.DataOperationResult();
                res.AddErrors(new string[] { zone.FullName + "上传数据时相关信息:" });
                WriteDataRecordInformation(res);
                List<Guid> listResult = ReportProgressResult(result);
                needWarning = listResult.Count > 0;
                landService.Close();
                ExhangeCount regeterBookerrorEntry = InitalzieErrorEntity(regeterBooks, listResult);
                //ExhangeCount dcBookerrorEntry = InitalzieErrorEntity(dcbooks, listResult);
                //ExhangeCount allerrorEntry = new ExhangeCount()
                //{
                //    FamilyCount = regeterBookerrorEntry.FamilyCount + dcBookerrorEntry.FamilyCount,
                //    LandCount = regeterBookerrorEntry.LandCount + dcBookerrorEntry.LandCount,
                //    PersonCount = regeterBookerrorEntry.PersonCount + dcBookerrorEntry.PersonCount,
                //};
                ReportExchangeInformation(surveyEntity, registerEntity, regeterBookerrorEntry);
                this.ReportAlert(eMessageGrade.Infomation, null, "上传" + zone.FullName + "数据完毕...");
                regeterBooks = null;
                dcbooks = null;
                listResult = null;
                return needWarning;
            }
            catch (SystemException ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());
                this.ReportAlert(eMessageGrade.Error, null, ex.Message);
                this.ReportAlert(eMessageGrade.Warn, null, zone.FullName + "下数据上传失败!");
            }
            catch (ApplicationException ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());
                this.ReportAlert(eMessageGrade.Error, null, ex.Message);
                this.ReportAlert(eMessageGrade.Warn, null, zone.FullName + "下数据上传失败!");
            }
            return needWarning;
        }

        /// <summary>
        /// 上传数据
        /// </summary>
        private bool AgriLandUpLoadProgress(bool showPersent, string description, Zone zone, List<VirtualPerson> vps)
        {
            this.ReportAlert(eMessageGrade.Infomation, null, "开始上传" + zone.FullName + "数据...");
            bool needWarning = false;
            try
            {
                ContractLandRegistrationServiceClient landService = AgricultureExchangeEntity.InitazlieServerData();
                int count = db.CreateVirtualPersonStation<LandVirtualPerson>().Count(t => t.ZoneCode == zone.FullCode);
                if (vps == null || vps.Count == 0)
                {
                    YuLinTu.PropertyRight.Registration.DataOperationResult[] results = landService.Upload(agriLandCollection);
                    YuLinTu.PropertyRight.Registration.DataOperationResult res = new PropertyRight.Registration.DataOperationResult();
                    res.AddErrors(new string[] { zone.FullName + "上传数据时相关信息:" });
                    WriteDataRecordInformation(res);
                    foreach (var result in results)
                    {
                        WriteDataRecordInformation(result);
                        if (!needWarning)
                        {
                            needWarning = result.HasError;
                        }
                    }
                }
                else
                {
                    foreach (YuLinTu.Business.ContractLand.Exchange2.ExLandContractor landContractor in agriLandCollection)
                    {
                        try
                        {
                            YuLinTu.PropertyRight.Registration.DataOperationResult result = landService.Upload(landContractor);
                            if (!result.HasError)
                            {
                                continue;
                            }
                            foreach (var value in result.Errors)
                            {
                                this.ReportAlert(eMessageGrade.Warn, null, value);
                            }
                        }
                        catch (SystemException ex)
                        {
                            System.Diagnostics.Debug.WriteLine(ex.ToString());
                        }
                        // this.ReportProgress(currentIndex, description);
                    }
                }
                landService.Close();
                this.ReportAlert(eMessageGrade.Infomation, null, "上传" + zone.FullName + "数据完毕...");
                return needWarning;
            }
            catch (SystemException ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());
                this.ReportAlert(eMessageGrade.Error, null, "服务地址错误,请确认服务器连接是否正确!");
                this.ReportAlert(eMessageGrade.Warn, null, zone.FullName + "下数据上传失败!");
            }
            return needWarning;
        }

        /// <summary>
        /// 报告交换信息
        /// </summary>
        private void ReportExchangeInformation(ExhangeCount surveyEntity, ExhangeCount regesterEntity, ExhangeCount errorEntity)
        {
            string record = string.Format("上传调查数据总户数:{0}、总人数:{1}、总地块数:{2}", surveyEntity.FamilyCount, surveyEntity.PersonCount, surveyEntity.LandCount);
            //if (surveyEntity.FamilyCount > 0)
            //{
            //    record += ",全部数据上传成功!";
            //}
            YuLinTu.PropertyRight.Registration.DataOperationResult res = new PropertyRight.Registration.DataOperationResult();
            res.AddErrors(new string[] { record });
            WriteDataRecordInformation(res);
            this.ReportAlert(eMessageGrade.Infomation, null, record);
            record = string.Format("上传登记数据总户数:{0}、总人数:{1}、总地块数:{2}", regesterEntity.FamilyCount, regesterEntity.PersonCount, regesterEntity.LandCount);
            //if (errorEntity.FamilyCount == 0 && regesterEntity.FamilyCount > 0)
            //{
            //    record += ",全部数据上传成功!";
            //}
            res.ClearErrors();
            res.AddErrors(new string[] { record });
            WriteDataRecordInformation(res);
            this.ReportAlert(eMessageGrade.Infomation, null, record);
            if (errorEntity.FamilyCount == 0)
            {
                return;
            }
            record = string.Format("上传登记数据成功户数:{0}、人数:{1}、地块数:{2}", regesterEntity.FamilyCount - errorEntity.FamilyCount, regesterEntity.PersonCount - errorEntity.PersonCount, regesterEntity.LandCount - errorEntity.LandCount);
            res.ClearErrors();
            res.AddErrors(new string[] { record });
            WriteDataRecordInformation(res);
            this.ReportAlert(eMessageGrade.Warn, null, record);
            record = string.Format("上传登记数据失败户数:{0}、人数:{1}、地块数:{2}", errorEntity.FamilyCount, errorEntity.PersonCount, errorEntity.LandCount);
            res.ClearErrors();
            res.AddErrors(new string[] { record });
            WriteDataRecordInformation(res);
            this.ReportAlert(eMessageGrade.Warn, null, record);
        }

        /// <summary>
        /// 初始化错误交换实体
        /// </summary>
        /// <param name="regeterBook"></param>
        /// <param name="errorList"></param>
        /// <returns></returns>
        private ExhangeCount InitalzieErrorEntity(List<EX_CBJYQ_DJB> regeterBook, List<Guid> errorList)
        {
            ExhangeCount entry = new ExhangeCount();
            foreach (Guid gd in errorList)
            {
                EX_CBJYQ_DJB djb = regeterBook.Find(id => id.ID == gd);
                if (djb == null)
                {
                    continue;
                }
                entry.FamilyCount++;
                entry.PersonCount += djb.DJGYR.Count;
                entry.LandCount += djb.DJCBD.Count;
            }
            return entry;
        }

        /// <summary>
        /// 报告处理结果
        /// </summary>
        /// <param name="result"></param>
        private List<Guid> ReportProgressResult(IEnumerable result)
        {
            List<Guid> idList = new List<Guid>();
            if (result == null)
            {
                return idList;
            }
            IEnumerator enumerator = result.GetEnumerator();
            if (enumerator == null)
            {
                return idList;
            }
            enumerator.Reset();
            while (enumerator.MoveNext())
            {
                YuLinTu.PropertyRight.Registration.DataOperationResult dataResult = enumerator.Current as YuLinTu.PropertyRight.Registration.DataOperationResult;
                WriteDataRecordInformation(dataResult);
                if (dataResult.HasError)
                {
                    idList.Add(dataResult.ID);
                }
                dataResult = null;
            }
            return idList;
        }

        /// <summary>
        /// 报告结果
        /// </summary>
        /// <param name="result"></param>
        private int ReportDataOperationResult(YuLinTu.PropertyRight.Registration.DataOperationResult result)
        {
            int resultCount = 0;
            if (result == null)
            {
                return resultCount;
            }
            if (!result.HasError)
            {
                return resultCount;
            }
            foreach (var value in result.Errors)
            {
                this.ReportAlert(eMessageGrade.Error, null, value);
            }
            resultCount += result.Errors.Length;
            return resultCount;
        }

        /// <summary>
        /// 进度报告
        /// </summary>
        /// <param name="information"></param>
        private void landMapping_InformationReportged(string information)
        {
            this.ReportAlert(eMessageGrade.Infomation, null, information);
        }

        /// <summary>
        /// 错误信息报告
        /// </summary>
        /// <param name="information"></param>
        private void landMapping_ErrorReportged(string information)
        {
            this.ReportAlert(eMessageGrade.Error, null, information);
        }

        /// <summary>
        /// 更新空间数据
        /// </summary>
        private void AgriLandExchangeProgress(Zone zone, List<VirtualPerson> vps, bool showPersent, string description)
        {
            try
            {
                ExZone exZone = AgriLandZoneMapping(zone);
                agriEntity.Lands = db.CreateContractLandWorkstation().GetCollection(zone.FullCode, eLevelOption.Self);
                agriEntity.Concords = db.CreateConcordStation().GetByZoneCode(zone.FullCode);
                landIndex = 1;
                isFamilyCoding = agriEntity.Lands.Any(land => agriEntity.Lands.Any((ld => (ld.CadastralNumber.Right(4) == land.CadastralNumber.Right(4) && ld.ID != land.ID))));//判断是否是按户进行编码
                Dictionary<VirtualPerson, List<ContractConcord>> dic = new Dictionary<VirtualPerson, List<ContractConcord>>();
                double percent = 98 / (double)vps.Count;
                foreach (VirtualPerson person in vps)
                {
                    List<ContractConcord> concords = agriEntity.Concords.FindAll(cd => (cd.ContracterId != null && cd.ContracterId.HasValue && cd.ContracterId.Value == person.ID));
                    if (concords == null || concords.Count == 0)
                    {
                        concords = agriEntity.Concords.FindAll(cd => cd.ContracterName == person.Name);
                    }
                    if (concords != null && concords.Count > 1)
                    {
                        dic.Add(person, concords);
                        continue;
                    }
                    ContractConcord concord = (concords != null && concords.Count == 1) ? concords[0] : null;
                    InitalizeExchangeData(zone, exZone, person, concord, showPersent, description, concord != null);
                    if (concord == null)
                    {
                        continue;
                    }
                    this.ReportProgress((int)(currentIndex * percent), person.Name);
                    currentIndex++;
                    List<ContractLand> landArray = agriEntity.Lands.FindAll(cd => cd.OwnerId != null && cd.OwnerId.HasValue && cd.OwnerId.Value == person.ID
                                                   && (cd.ConcordId == null || !cd.ConcordId.HasValue || cd.ConcordId.Value == Guid.Empty));//找地通过户id去找
                    if (checkFamilyName && (landArray == null || landArray.Count == 0))
                    {
                        landArray = agriEntity.Lands.FindAll(cd => cd.OwnerName == person.Name && (cd.ConcordId == null || !cd.ConcordId.HasValue || cd.ConcordId.Value == Guid.Empty));
                    }
                    if (landArray != null && landArray.Count > 0)
                    {
                        InitalizeExchangeData(zone, exZone, person, null, showPersent, description, false);
                    }
                    concord = null;
                    concords = null;
                }
                foreach (KeyValuePair<VirtualPerson, List<ContractConcord>> pair in dic)
                {
                    foreach (ContractConcord concord in pair.Value)
                    {
                        InitalizeExchangeData(zone, exZone, pair.Key, concord, showPersent, description, true);
                    }
                }
                agriEntity.Disponse();
            }
            catch (SystemException ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());
            }
        }
        /// <summary>
        /// 初始化交换数据
        /// </summary>
        private void InitalizeExchangeData(Zone zone, YuLinTu.Business.ContractLand.Exchange2.ExZone exZone, VirtualPerson person,
                                           ContractConcord concord, bool showPersent, string description, bool useConcord)
        {
            YuLinTu.Business.ContractLand.Exchange2.ExLandContractor landContractor = new YuLinTu.Business.ContractLand.Exchange2.ExLandContractor();
            landContractor.Zone = exZone;
            AgriContractorMapping(zone, person, landContractor);
            landContractor.Contractor.LandLocated = exZone.FullName;
            AgriSharePersonMapping(person, landContractor);
            AgriLandMapping(zone, person, useConcord ? concord : null, landContractor);
            AgriOtherMapping(zone, person, landContractor, concord);
            agriLandCollection.Add(landContractor);
            if (exZone.Shape != null)
            {
                exZone.Shape = null;
            }
        }

        /// <summary>
        /// 承包方信息映射
        /// </summary>
        /// <param name="exZone"></param>
        /// <param name="zone"></param>
        private void AgriContractorMapping(Zone zone, VirtualPerson person, YuLinTu.Business.ContractLand.Exchange2.ExLandContractor landContractor)
        {
            YuLinTu.Business.ContractLand.Exchange2.ExContractor contractor = new YuLinTu.Business.ContractLand.Exchange2.ExContractor();
            contractor.ID = person.ID;
            contractor.Comment = person.Comment;
            contractor.CreationTime = person.CreationTime;
            contractor.Founder = person.Founder;
            contractor.ModifiedTime = person.ModifiedTime;
            contractor.Modifier = person.Modifier;
            contractor.Name = person.Name;
            contractor.Number = person.Number;
            contractor.VirtualType = (int)person.VirtualType;
            contractor.Telephone = person.Telephone;
            contractor.FamilyNumber = InitalizeFamilyNumber(zone, person.FamilyNumber);
            contractor.PostNumber = person.PostalNumber;
            if (zone.FullCode == person.ZoneCode)
            {
                contractor.ZoneId = zone != null ? zone.ID : Guid.NewGuid();
            }
            else
            {
                Zone curZone = db.CreateZoneWorkStation().Get(person.ZoneCode);
                contractor.ZoneId = curZone != null ? curZone.ID : Guid.NewGuid();
                curZone = null;
            }
            landContractor.Contractor = contractor;
        }

        /// <summary>
        /// 初始化户号
        /// </summary>
        /// <param name="familyNumber"></param>
        /// <returns></returns>
        private string InitalizeFamilyNumber(Zone zone, string familyNumber)
        {
            int number = 0;
            Int32.TryParse(familyNumber, out number);
            string famNumber = familyNumber;
            string zoneCode = zone.FullCode;
            switch (zone.Level)
            {
                case eZoneLevel.Village:
                    zoneCode = zone.FullCode + "00";
                    break;
                case eZoneLevel.Group:
                    zoneCode = zone.FullCode.Substring(0, 12) + zone.FullCode.Substring(14);
                    break;
                default:
                    break;
            }
            famNumber = zoneCode + string.Format("{0:D4}", number);
            return famNumber;
        }

        /// <summary>
        /// 共有人信息映射
        /// </summary>
        private void AgriSharePersonMapping(VirtualPerson constractor, YuLinTu.Business.ContractLand.Exchange2.ExLandContractor landContractor)
        {
            ExSharePersonCollection personCollection = new ExSharePersonCollection();
            foreach (Person person in constractor.SharePersonList)
            {
                person.FamilyID = constractor.ID;
                person.ZoneCode = constractor.ZoneCode;
                YuLinTu.Business.ContractLand.Exchange2.ExSharePerson sharePerson = new YuLinTu.Business.ContractLand.Exchange2.ExSharePerson();
                sharePerson.ID = person.ID;
                sharePerson.Age = person.GetAge();
                sharePerson.BirthDate = (person.Birthday != null && person.Birthday.HasValue && person.Birthday.Value.Year == 0001) ? null : person.Birthday;
                sharePerson.Comment = person.Comment;
                sharePerson.ContractorId = person.FamilyID.Value;
                sharePerson.CreationTime = person.CreateTime;
                sharePerson.CredentialNumber = person.ICN;
                sharePerson.CredentialType = (int)eCredentialsType.IdentifyCard;
                sharePerson.Founder = person.CreateUser;
                sharePerson.ModifiedTime = person.LastModifyTime;
                sharePerson.Modifier = person.LastModifyUser;
                sharePerson.Name = person.Name;
                sharePerson.Nationality = EnumNameAttribute.GetDescription(person.Nation);
                sharePerson.Relation = person.Relationship;
                sharePerson.Sex = (int)person.Gender;
                personCollection.Add(sharePerson);
            }
            landContractor.SharePersons = personCollection;
        }

        /// <summary>
        /// 地块信息映射
        /// </summary>
        /// <param name="exZone"></param>
        /// <param name="zone"></param>
        private void AgriLandMapping(Zone zone, VirtualPerson constractor, ContractConcord concord, YuLinTu.Business.ContractLand.Exchange2.ExLandContractor landContractor)
        {
            List<ContractLand> landArray = InitalizeLandArray(constractor, concord);
            YuLinTu.Business.ContractLand.Exchange2.ExContractLandCollection lands = new YuLinTu.Business.ContractLand.Exchange2.ExContractLandCollection();
            foreach (ContractLand land in landArray)
            {
                if (string.IsNullOrEmpty(land.LandNumber))
                {
                    land.LandNumber = ContractLand.GetLandNumber(land.CadastralNumber);
                }
                YuLinTu.Business.ContractLand.Exchange2.ExContractLand exLand = SetAgriLandInformation(zone, land);
                if (showInformation && land.Shape == null)
                {
                    string information = string.Format("承包方:{0}下地块编码为{1}的地块没有空间信息!", land.OwnerName, ContractLand.GetLandNumber(land.CadastralNumber));
                    this.ReportAlert(eMessageGrade.Infomation, null, information);
                }
                lands.Add(exLand);
                landIndex++;
            }
            landContractor.ContractLands = lands;
            landArray = null;
        }

        /// <summary>
        /// 初始化地块集合
        /// </summary>
        /// <returns></returns>
        private List<ContractLand> InitalizeLandArray(VirtualPerson constractor, ContractConcord concord)
        {
            List<ContractLand> landArray = null;
            if (concord == null)
            {
                landArray = agriEntity.Lands.FindAll(cd => cd.OwnerId != null && cd.OwnerId.HasValue && cd.OwnerId.Value == constractor.ID
                            && (cd.ConcordId == null || !cd.ConcordId.HasValue || cd.ConcordId.Value == Guid.Empty));//找地通过户id去找
                if (checkFamilyName && (landArray == null || landArray.Count == 0))
                {
                    landArray = agriEntity.Lands.FindAll(cd => cd.OwnerName == constractor.Name && (cd.ConcordId == null || !cd.ConcordId.HasValue || cd.ConcordId.Value == Guid.Empty));
                }
            }
            else
            {
                landArray = agriEntity.Lands.FindAll(cd => cd.ConcordId != null && cd.ConcordId.HasValue && cd.ConcordId.Value == concord.ID);//找地通过合同id去找
            }
            return landArray;
        }

        /// <summary>
        /// 地域信息映射
        /// </summary>
        /// <param name="exZone"></param>
        /// <param name="zone"></param>
        private void AgriOtherMapping(Zone zone, VirtualPerson constractor, YuLinTu.Business.ContractLand.Exchange2.ExLandContractor landContractor, ContractConcord concord)
        {
            try
            {
                if (concord == null)
                {
                    return;
                }
                YuLinTu.Library.Entity.ContractRegeditBook book = db.CreateRegeditBookStation().Get(concord.ID);
                YuLinTu.Business.ContractLand.Exchange2.ExContractConcord exConcord = new YuLinTu.Business.ContractLand.Exchange2.ExContractConcord();
                YuLinTu.Business.ContractLand.Exchange2.ExContractRegeditBook exBook = new YuLinTu.Business.ContractLand.Exchange2.ExContractRegeditBook();
                exConcord.ID = concord.ID;
                exConcord.ConcordNumber = concord.ConcordNumber;
                exConcord.AgriConcordNumber = InitalizeAgricultureConcordNumber(zone, constractor, concord.ConcordNumber);
                if (exConcord.ConcordNumber == exConcord.AgriConcordNumber)
                {
                    exConcord.ConcordNumber = "";
                }
                exConcord.AgentCrdentialNumber = concord.AgentCrdentialNumber;
                exConcord.AgentCrdentialType = (int)Enum.Parse(typeof(eCredentialsType), concord.AgentCrdentialType);
                exConcord.AgentName = concord.AgentName;
                exConcord.AgentTelphone = concord.AgentTelphone;
                exConcord.ArableLandEndTime = concord.ArableLandEndTime;
                exConcord.ArableLandStartTime = concord.ArableLandStartTime;
                exConcord.ArableLandType = (int)Enum.Parse(typeof(eConstructMode), concord.ArableLandType);
                exConcord.BadlandEndTime = concord.BadlandEndTime;
                exConcord.BadlandPurpose = (int)Enum.Parse(typeof(eLandPurposeType), concord.BadlandPurpose);
                exConcord.BadlandStartTime = concord.BadlandStartTime;
                exConcord.BadlandType = (int)Enum.Parse(typeof(eConstructMode), concord.BadlandType);
                exConcord.CheckAgencyDate = concord.CheckAgencyDate;
                exConcord.Comment = concord.Comment;
                exConcord.ContracerType = (int)Enum.Parse(typeof(eContractorType), concord.ContracerType);
                exConcord.ContractCredentialNumber = concord.ContractCredentialNumber;
                exConcord.ContractDate = concord.ContractDate;
                exConcord.ContractorId = (concord.ContracterId != null && concord.ContracterId.HasValue) ? concord.ContracterId.Value : Guid.Empty;
                exConcord.ContracterIdentifyNumber = concord.ContracterIdentifyNumber;
                exConcord.ContracterName = concord.ContracterName;
                exConcord.ContracterRepresentName = concord.ContracterRepresentName;
                exConcord.ContracterRepresentNumber = concord.ContracterRepresentNumber;
                exConcord.ContracterRepresentTelphone = concord.ContracterRepresentTelphone;
                exConcord.ContracterRepresentType = (int)Enum.Parse(typeof(eCredentialsType), concord.ContracterRepresentType);
                exConcord.ContractMoney = (concord.ContractMoney != null && concord.ContractMoney.HasValue) ? concord.ContractMoney.Value : 0.0;
                exConcord.CountActualArea = concord.CountActualArea;
                exConcord.CountAwareArea = concord.CountAwareArea;
                exConcord.CountMotorizeLandArea = concord.CountMotorizeLandArea;
                exConcord.CreationTime = concord.CreationTime;
                exConcord.Flag = concord.Flag;
                exConcord.Founder = concord.Founder;
                exConcord.ID = concord.ID;
                exConcord.IsValid = false;
                exConcord.LandPurpose = (int)Enum.Parse(typeof(eLandPurposeType), concord.LandPurpose);
                exConcord.ManagementTime = concord.ManagementTime;
                exConcord.ModifiedTime = concord.ModifiedTime;
                exConcord.Modifier = concord.Modifier;
                exConcord.PersonAvgArea = concord.PersonAvgArea;
                exConcord.PrivateArea = concord.PrivateArea.HasValue ? concord.PrivateArea.Value : 0.0;
                exConcord.RequireBookId = (concord.RequireBookId != null && concord.RequireBookId.HasValue) ? concord.RequireBookId.Value : Guid.Empty;
                exConcord.SecondContracterLocated = concord.SecondContracterLocated;
                exConcord.SecondContracterName = concord.SecondContracterName;
                exConcord.SenderDate = concord.SenderDate;
                exConcord.SenderName = concord.SenderName;
                exConcord.EmployerId = concord.SenderId;
                exConcord.Status = (int)concord.Status;
                exConcord.TotalTableArea = concord.TotalTableArea.HasValue ? concord.TotalTableArea.Value : 0.0;
                if (book != null)
                {
                    exBook.ID = book.ID;
                    exBook.Comment = book.Comment;
                    exBook.Count = book.Count;
                    exBook.CreationTime = book.CreationTime;
                    exBook.Founder = book.Founder;
                    exBook.ModifiedTime = book.ModifiedTime;
                    exBook.Modifier = book.Modifier;
                    exBook.Number = book.RegeditNumber;
                    exBook.PrintDate = book.PrintDate;
                    exBook.RegeditNumber = InitalizeAgricultureConcordNumber(zone, constractor, concord.ConcordNumber);
                    if (exBook.Number == exBook.RegeditNumber)
                    {
                        exBook.Number = "";
                    }
                    exBook.SendDate = book.SendDate;
                    exBook.SendOrganization = book.SendOrganization;
                    exBook.WriteDate = book.WriteDate;
                    exBook.WriteOrganization = book.WriteOrganization;
                    exBook.Year = book.Year;
                }
                landContractor.ContractConcord = exConcord;
                landContractor.ContractRegeditBook = book == null ? null : exBook;
                AgriRegeditApplicationMapping(concord, landContractor);
                AgriSenderMapping(zone, concord, landContractor);
            }
            catch (SystemException ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());
            }
        }

        /// <summary>
        /// 撰写数据记录信息
        /// </summary>
        private void WriteDataRecordInformation(YuLinTu.PropertyRight.Registration.DataOperationResult result)
        {
            string fileName = System.Windows.Forms.Application.StartupPath + @"\Error\DataRecord.txt";
            if (!System.IO.File.Exists(fileName))
            {
                return;
            }
            using (System.IO.StreamWriter writer = new System.IO.StreamWriter(fileName, true))
            {
                if (result.HasError)
                {
                    foreach (var value in result.Errors)
                    {
                        writer.WriteLine(value);
                    }
                }
            }
        }

        /// <summary>
        /// 申请书
        /// </summary>
        private void AgriRegeditApplicationMapping(ContractConcord concord, YuLinTu.Business.ContractLand.Exchange2.ExLandContractor landContractor)
        {
            if (concord == null || concord.RequireBookId == null || !concord.RequireBookId.HasValue)
            {
                return;
            }
            Guid requireId = concord.RequireBookId.Value;
            if (concord.RequireBookId != null && concord.RequireBookId.HasValue)
            {
                return;
            }
            ContractRequireTable requireTable = db.CreateRequireTableWorkStation().Get(concord.RequireBookId.Value);
            if (requireTable == null)
            {
                return;
            }
            YuLinTu.Business.ContractLand.Exchange2.ExRegisterApplication regApplication = new YuLinTu.Business.ContractLand.Exchange2.ExRegisterApplication();
            regApplication.Applicant = concord.SenderName;
            regApplication.Number = requireTable.Number;
            regApplication.Year = requireTable.Year;
            landContractor.RegisterApplication = regApplication;
            requireTable = null;
        }

        /// <summary>
        /// 文件处理(生成压缩包)
        /// </summary>
        /// <returns></returns>
        private string ArcFileProgresss(string filePath, ArrayList fileList, Zone zone, string name)
        {
            if (string.IsNullOrEmpty(filePath))
            {
                return string.Empty;
            }
            Environment.CurrentDirectory = filePath;
            string zoneName = SpliteZoneName(zone);
            ArrayList listFile = new ArrayList();
            foreach (string fileName in fileList)
            {
                listFile.Add(System.IO.Path.GetFileName(fileName));
            }
            try
            {
                if (landorType == LanderType.AgricultureLand)
                {
                    ZipOperation.Compress(filePath, zoneName + "(" + name + ")", listFile, "");
                }
                else
                {
                    ZipOperation.Compress(filePath, zoneName + "(" + name + ")", listFile, "");
                }
            }
            catch
            {
                //if (LandType == LanderType.AgricultureLand)
                //{
                //    SharpZipOperation.CompressContractLand(filePath, zoneName + "(" + name + ")", fileList, "");
                //}
                //else
                //{
                SharpZipOperation.Compress(filePath, zoneName + "(" + name + ")", fileList, "");
                //}
            }
            finally
            {
                foreach (string fileName in fileList)
                {
                    System.IO.File.SetAttributes(fileName, System.IO.FileAttributes.Normal);
                    File.Delete(fileName);
                }
            }
            listFile.Clear();
            listFile = null;
            return filePath + @"\" + zoneName + "(" + name + ").zip";
        }

        /// <summary>
        /// 截取地域代码
        /// </summary>
        /// <returns></returns>
        private string SpliteZoneCode(Zone zone)
        {
            if (zone == null)
            {
                return string.Empty;
            }
            if (zone.Level > eZoneLevel.County)
            {
                return zone.FullCode;
            }
            Zone city = db.CreateZoneWorkStation().Get(CurrentZone.FullCode.Substring(0, Zone.ZONE_CITY_LENGTH));
            string zoneName = city != null ? zone.FullCode.Replace(city.FullCode, "") : zone.FullCode;
            city = null;
            return zoneName;
        }

        /// <summary>
        /// 截取地域名称
        /// </summary>
        /// <returns></returns>
        private string SpliteZoneName(Zone zone)
        {
            if (zone == null)
            {
                return string.Empty;
            }
            if (zone.Level > eZoneLevel.County)
            {
                return zone.FullName;
            }
            Zone city = db.CreateZoneWorkStation().Get(CurrentZone.FullCode.Substring(0, Zone.ZONE_CITY_LENGTH));
            string zoneName = city != null ? zone.FullName.Replace(city.FullName, "") : zone.FullName;
            city = null;
            return zoneName;
        }

        /// <summary>
        /// 发包方映射
        /// </summary>
        private void AgriSenderMapping(Zone zone, ContractConcord concord, YuLinTu.Business.ContractLand.Exchange2.ExLandContractor landContractor)
        {
            if (concord == null || concord.SenderId == null || string.IsNullOrEmpty(concord.SenderName))
            {
                return;
            }
            CollectivityTissue tissue = null;//集体经济组织
            tissue = agriEntity.Tissues.Find(ts => ts.ID == concord.SenderId);
            if (tissue == null)
            {
                tissue = db.CreateSenderWorkStation().Get(concord.SenderId);
                if (tissue != null)
                {
                    agriEntity.Tissues.Add(tissue);
                }
            }
            if (tissue == null)
            {
                return;
            }
            string zoneCode = tissue.ZoneCode;
            switch (zoneCode.Length)
            {
                case Zone.ZONE_TOWN_LENGTH:
                    zoneCode += "00000";
                    break;
                case Zone.ZONE_VILLAGE_LENGTH:
                    zoneCode += "00";
                    break;
                case Zone.ZONE_GROUP_LENGTH:
                    zoneCode = zoneCode.Substring(0, Zone.ZONE_VILLAGE_LENGTH) + zoneCode.Substring(Zone.ZONE_VILLAGE_LENGTH + 2);
                    break;
            }
            ExCollectiveTissue exTissue = new ExCollectiveTissue();
            exTissue.ID = tissue.ID;
            exTissue.Code = tissue.Code;
            exTissue.AgricultureCode = zoneCode;
            exTissue.LawyerCartNumber = tissue.LawyerCartNumber;
            exTissue.LawyerName = tissue.LawyerName;
            exTissue.Name = tissue.Name;
            exTissue.Type = (int)tissue.Type;
            exTissue.ZoneCode = tissue.ZoneCode;
            landContractor.Contractee = exTissue;
            tissue = null;
        }

        /// <summary>
        /// 地域信息映射
        /// </summary>
        /// <param name="exZone"></param>
        /// <param name="zone"></param>
        private ExZone AgriLandZoneMapping(Zone zone)
        {
            ExZone exZone = new ExZone();
            exZone.ID = zone.ID;
            exZone.Code = zone.Code;
            exZone.Comment = zone.Comment;
            exZone.CreateTime = DateTime.Now;
            exZone.CreateUser = zone.CreateUser;
            exZone.FullCode = zone.FullCode;
            exZone.FullName = zone.FullName;
            exZone.Level = (int)zone.Level;
            exZone.ModifyTime = DateTime.Now;
            exZone.ModifyUser = zone.LastModifyUser;
            exZone.Name = zone.Name;
            exZone.ParentCode = zone.UpLevelCode;
            exZone.ParentName = zone.UpLevelName;
            YuLinTu.Business.ContractLand.Exchange2.ExGeometry exGeometry = new YuLinTu.Business.ContractLand.Exchange2.ExGeometry();
            if (zone.Shape != null)
            {
                exGeometry.WellKnownBinary = zone.Shape.ToBytes();
                exGeometry.Srid = zone.Shape.Srid;
                exZone.Shape = exGeometry;
            }
            else
            {
                this.ReportInfomation(string.Format("{0}行政地域空间数据为空", zone.FullName));
                exZone.Shape = null;
            }

            // exZone.Shape = (arcZone != null && arcZone.Shape != null) ? arcZone.Shape.ToGeneralGeometry() : null;
            return exZone;
        }

        /// <summary>
        /// 初始化农业部编码
        /// </summary>
        /// <returns></returns>
        private string InitalizeAgricultureConcordNumber(Zone zone, VirtualPerson contractor, string mode)
        {
            if (zone == null && contractor == null)
            {
                return string.Empty;
            }
            string zoneCode = zone.FullCode.PadRight(14, '0');
            string familyNumber = contractor.FamilyNumber.PadLeft(4, '0');
            string familyMode = "J";
            int index = mode.LastIndexOf("J");
            if (index == 0)
            {
                familyMode = "J";
            }
            index = mode.LastIndexOf("Q");
            if (index == 0)
            {
                familyMode = "Q";
            }
            return zoneCode + familyNumber + familyMode;
        }

        /// <summary>
        /// 设置地块信息
        /// </summary>
        private ExContractLand SetAgriLandInformation(Zone zone, ContractLand land)
        {
            YuLinTu.Business.ContractLand.Exchange2.ExContractLand exLand = new YuLinTu.Business.ContractLand.Exchange2.ExContractLand();
            exLand.ActualArea = land.ActualArea;
            exLand.AwareArea = land.AwareArea;
            exLand.CadastralNumber = land.CadastralNumber;
            exLand.AgricultureNumber = string.IsNullOrEmpty(land.LandExpand.AgricultureNumber) ? InitalizeAgricultureNumber(zone, ContractLand.GetLandNumber(land.CadastralNumber)) : land.LandExpand.AgricultureNumber;
            if (ContractLand.GetLandNumber(exLand.CadastralNumber) == exLand.AgricultureNumber)
            {
                exLand.CadastralNumber = "";
            }
            exLand.Comment = land.Comment;
            if (land.ConcordId.HasValue)
            {
                exLand.ConcordId = land.ConcordId.Value;
            }
            exLand.ContractType = (int)Enum.Parse(typeof(eConstructMode), land.ConstructMode);
            exLand.CreationTime = land.CreationTime;
            exLand.FormerPerson = land.OwnerName;
            exLand.Founder = land.Founder;
            exLand.HouseHolderName = land.OwnerName;
            exLand.ID = land.ID;
            exLand.IsFarmerLand = land.IsFarmerLand;
            exLand.IsFlyLand = land.IsFlyLand;
            exLand.IsTransfer = land.IsTransfer;
            exLand.LandCode = land.LandCode;
            exLand.LandLevel = (int)Enum.Parse(typeof(eLandLevel), land.LandLevel);
            exLand.LandName = land.LandName;
            exLand.LandNeighbor = land.NeighborEast + "#" + land.NeighborSouth + "#" + land.NeighborWest + "#" + land.NeighborNorth;
            exLand.LandNumber = land.LandNumber;
            eLandSlopeLevel spopeLevel = eLandSlopeLevel.UnKnown;
            Enum.TryParse<eLandSlopeLevel>(land.LandScopeLevel, out spopeLevel);
            exLand.LandScopeLevel = (int)spopeLevel;
            exLand.LineArea = (land.LineArea == null || !land.LineArea.HasValue) ? 0.0 : land.LineArea.Value;
            if (zone.FullCode == land.ZoneCode)
            {
                exLand.LocationCode = zone != null ? zone.ID : Guid.Empty;
            }
            else
            {
                Zone curZone = db.CreateZoneWorkStation().Get(land.ZoneCode);
                exLand.LocationCode = curZone != null ? curZone.ID : Guid.Empty;
            }
            exLand.ManagementType = (int)Enum.Parse(typeof(eManageType), land.ManagementType);
            exLand.ModifiedTime = land.ModifiedTime;
            exLand.Modifier = land.Modifier;
            exLand.Name = land.Name;
            exLand.OwnerRightName = land.SenderName;
            exLand.OwnerRightType = (int)Enum.Parse(typeof(eLandPropertyType), land.OwnRightType);
            exLand.OwnRightCode = zone != null ? zone.ID : Guid.Empty;
            exLand.ParcelNumber = land.ExtendB;
            exLand.PertainToArea = land.PertainToArea;
            exLand.PlantType = (int)Enum.Parse(typeof(ePlantProtectType), land.PlantType);
            exLand.PlatType = (int)Enum.Parse(typeof(YuLinTu.Library.Entity.ePlantingType), land.PlatType);
            exLand.PlotNumber = land.PlotNumber;
            exLand.Purpose = (int)Enum.Parse(typeof(eLandPurposeType), land.Purpose);
            exLand.Soiltype = land.Soiltype;
            if (land.TableArea.HasValue)
            {
                exLand.TableArea = land.TableArea.Value;
            }
            if (land.MotorizeLandArea.HasValue)
            {
                exLand.MotorizeLandArea = land.MotorizeLandArea.Value;
            }
            exLand.TransferTime = land.TransferTime;
            exLand.TransferType = (int)Enum.Parse(typeof(eTransferType), land.TransferType);
            exLand.TransferWhere = land.TransferWhere;
            exLand.WidthHeight = land.WidthHeight;
            exLand.IsTransfer = land.IsTransfer;
            AgricultureLandExpand landExpand = land.LandExpand;//扩展信息

            exLand.Extend7 = landExpand.UseSituation;
            exLand.Extend8 = landExpand.Yield;
            exLand.Extend9 = landExpand.OutputValue;
            exLand.Extend10 = landExpand.IncomeSituation;
            if (exLand.LandExpand == null)
            {
                exLand.LandExpand = new Dictionary<string, object>();
            }
            exLand.LandExpand.Add("Elevation", landExpand.Elevation);
            return exLand;
        }

        /// <summary>
        /// 初始化农业部编码
        /// </summary>
        /// <returns></returns>
        private string InitalizeAgricultureNumber(Zone zone, string landNumber)
        {
            string zoneCode = zone.FullCode;
            switch (zone.Level)
            {
                case eZoneLevel.Village:
                    zoneCode = zone.FullCode + "00";
                    break;
                case eZoneLevel.Group:
                    zoneCode = zone.FullCode.Substring(0, 12) + zone.FullCode.Substring(14);
                    break;
                default:
                    break;
            }
            if (isFamilyCoding)
            {
                return zoneCode + string.Format("{0:D5}", landIndex);
            }
            if (string.IsNullOrEmpty(landNumber))
            {
                return "";
            }
            string parcelNumber = landNumber;
            int index = landNumber.IndexOf("J");
            if (index > 0)
            {
                parcelNumber = landNumber.Substring(index + 1);
            }
            index = landNumber.IndexOf("Q");
            if (index > 0)
            {
                parcelNumber = landNumber.Substring(index + 1);
            }
            if (string.IsNullOrEmpty(parcelNumber))
            {
                return parcelNumber;
            }
            if (parcelNumber.Length > 14)
            {
                parcelNumber = parcelNumber.Substring(14);
            }
            return zoneCode + parcelNumber.PadLeft(5, '0');
        }

        #endregion
    }
}
