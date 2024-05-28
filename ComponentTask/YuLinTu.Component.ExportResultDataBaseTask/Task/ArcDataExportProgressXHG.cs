using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YuLinTu.Library.Business;
using YuLinTu.Library.Entity;
using System.Collections;
using YuLinTu.Library.AgriExchange;
using YuLinTu.Component.ExportTask;
using System.IO;
using System.Windows.Forms;
using YuLinTu.Business.TaskBasic;
using YuLinTu.Library.WorkStation;
using YuLinTu.Data;
using YuLinTu.Library.Repository;
using YuLinTu.Spatial;
using System.Threading.Tasks;

namespace YuLinTu.Component.ExportResultDataBaseTask
{
    public class ArcDataExportProgressXHG : Task
    {
        #region Fields

        private int currentIndex = 1;//当前索引号
        //private IDatabase database;//属性数据库
        //private IArcGisDatabase gisInstance;//空间数据库
        //private IFeatureWorkspace workspace;//工作空间
        //private YuLinTu.Library.Entity.Zone currentZone;//当前地域
        private string zoneName;//地域名称
        private List<string> relationList;//家庭关系列表
        private bool canExport;//是否可以导出
        // private ISpatialReference reference;//空间坐标系
        private YuLinTu.Library.Entity.Zone county;//区县地域
        private bool showInformation;//是否显示信息
        private List<Guid> filterLandIDs;//检查后筛选出来最终与界址点线挂钩的地块集合ID

        private IZoneWorkStation zoneStation;
        private IContractLandWorkStation contractLandWorkStation;//承包台账地块业务逻辑层
        private IVirtualPersonWorkStation<LandVirtualPerson> VirtualPersonStation;//承包台账(承包方)Station
        private IConcordWorkStation concordStation;
        private IContractRegeditBookWorkStation contractRegeditBookStation;
        private IBuildLandBoundaryAddressCoilWorkStation jzxStation;
        private IBuildLandBoundaryAddressDotWorkStation jzdStation;
        private ISenderWorkStation senderStation;
        private IDZDWWorkStation dzdwStation;
        private IDCZDWorkStation dczdStation;
        private IXZDWWorkStation xzdwStation;
        private IMZDWWorkStation mzdwStation;
        private IControlPointWorkStation kzdStation;
        private IFarmLandConserveWorkStation jbntbhqStation;
        private IZoneBoundaryWorkStation qyjxStation;

        #endregion

        #region Propertys
        /// <summary>
        /// 目标节点地域
        /// </summary>
        public YuLinTu.Library.Entity.Zone currentZone { get; set; }

        /// <summary>
        /// 数据源上下文
        /// </summary>
        public IDbContext DbContext { get; set; }

        /// <summary>
        /// 目标文件夹
        /// </summary>
        public string Folder { get; set; }

        /// <summary>
        /// 是否包含扫描资料文件夹
        /// </summary>
        public bool ContainMatrical { get; set; }

        /// <summary>
        /// 单位名称
        /// </summary>
        public string UnitName { get; set; }

        /// <summary>
        /// 联系人
        /// </summary>
        public string LinkMan { get; set; }

        /// <summary>
        /// 联系电话
        /// </summary>
        public string Telephone { get; set; }

        /// <summary>
        /// 联系地址
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// 邮政编码
        /// </summary>
        public string PosterNumber { get; set; }

        /// <summary>
        /// 包含界址点
        /// </summary>
        public bool ContainDot { get; set; }

        /// <summary>
        /// 包含界址线
        /// </summary>
        public bool ContainLine { get; set; }

        /// <summary>
        /// 是否检查
        /// </summary>
        public bool CanChecker { get; set; }

        /// <summary>
        /// 是否检查证件号码
        /// </summary>
        public bool CheckCardNumber { get; set; }

        /// <summary>
        /// 任务-导出确权成果库地块与界址点、线匹配设置
        /// </summary>
        public TaskExportLandDotCoilSettingDefine TaskExportLandDotCoilDefine { get; set; }

        #endregion

        #region Ctor

        public ArcDataExportProgressXHG(IDbContext db)
        {
            this.Name = "导出确权登记数据库成果";
            DbContext = db;
            zoneStation = DbContext.CreateZoneWorkStation();
            VirtualPersonStation = DbContext.CreateVirtualPersonStation<LandVirtualPerson>();
            contractLandWorkStation = DbContext.CreateContractLandWorkstation();
            concordStation = DbContext.CreateConcordStation();
            contractRegeditBookStation = DbContext.CreateRegeditBookStation();
            jzxStation = DbContext.CreateBoundaryAddressCoilWorkStation();
            jzdStation = DbContext.CreateBoundaryAddressDotWorkStation();
            senderStation = DbContext.CreateSenderWorkStation();
            dzdwStation = new ContainerFactory(DbContext).CreateWorkstation<IDZDWWorkStation, IDZDWRepository>();
            dczdStation = new ContainerFactory(DbContext).CreateWorkstation<IDCZDWorkStation, IDCZDRepository>();
            xzdwStation = new ContainerFactory(DbContext).CreateWorkstation<IXZDWWorkStation, IXZDWRepository>();
            mzdwStation = new ContainerFactory(DbContext).CreateWorkstation<IMZDWWorkStation, IMZDWRepository>();
            kzdStation = new ContainerFactory(DbContext).CreateWorkstation<IControlPointWorkStation, IControlPointRepository>();
            jbntbhqStation = new ContainerFactory(DbContext).CreateWorkstation<IFarmLandConserveWorkStation, IFarmLandConserveRepository>();
            qyjxStation = new ContainerFactory(DbContext).CreateWorkstation<IZoneBoundaryWorkStation, IZoneBoundaryRepository>();

        }

        #endregion

        #region Override

        public void OnDo()
        {
            this.ReportProgress(0, "开始");
            var metadata = Argument as TaskBuildExportResultDataBaseArgument;
            bool canContinue = InitalizeData(metadata);
            if (!canContinue)
            {
                return;
            }
            try
            {
                if (currentZone.Level > YuLinTu.Library.Entity.eZoneLevel.County)
                {
                    this.ReportError("选择行政区域高于区县级行政区域!");
                    return;
                }
                string dbName = System.Windows.Forms.Application.StartupPath + @"\Template\db.sqlite";
                if (!System.IO.File.Exists(dbName))
                {
                    this.ReportError("系统中缺少数据库文件!");
                    return;
                }
                else
                {
                    System.IO.File.Copy(dbName, System.Windows.Forms.Application.StartupPath + @"\db.sqlite", true);
                }
                ArcDataProgress();
            }
            catch (System.Exception ex)
            {
                this.ReportError("导出数据出现错误，详情请查看错误日志");
                if (county != null)
                {
                    string filePath = Folder + @"\" + county.FullCode + county.Name;
                    if (Directory.Exists(filePath))
                    {
                        System.IO.DirectoryInfo directory = new DirectoryInfo(filePath);
                        directory.Delete(true);
                    }
                }
                else
                {
                    this.ReportError("数据处理时出错!" + ex.Message);
                }
                return;
            }
            this.ReportProgress(100, null);
        }

        #endregion

        #region 数据检查

        /// <summary>
        /// 初始化数据
        /// </summary>
        /// <param name="metadata"></param>
        /// <returns></returns>
        private bool InitalizeData(TaskBuildExportResultDataBaseArgument metadata)
        {
            //if (metadata == null)
            //{
            //    ReportException(null, "选择行政区域无效!");
            //    return false;
            //}
            //database = new YuLinTu.Library.YltDatabase.Database(metadata.PerpertyDatabase);
            //gisInstance = new ArcGisDatabase(metadata.SpaceDatabase);
            //currentZone = database.Zone.Get(metadata.ZoneCode);
            //if (currentZone == null)
            //{
            //    ReportException(null, "选择行政区域无效!");
            //    return false;
            //}
            //currentIndex = 1;
            //relationList = InitalizeAllRelation();
            //canExport = true;
            //bool result = AddConfig.WriteAppConfigByXML();
            //if (!result)
            //{
            //    ReportException(null, "系统文件配置错误!");
            //    return false;
            //}
            //showInformation = currentZone.Level == YuLinTu.Library.Entity.eZoneLevel.Group;
            //if (!showInformation)
            //{
            //    InitalizeDirectory();
            //}
            //return true;

            if (metadata == null)
            {
                this.ReportError("选择行政区域无效!");
                return false;
            }

            if (currentZone == null)
            {
                this.ReportError("选择行政区域无效!");
                return false;
            }
            currentIndex = 1;
            relationList = InitalizeAllRelation();
            canExport = true;
            //bool result = AddConfig.WriteAppConfigByXML();
            //if (!result)
            //{
            //    this.ReportError("系统文件配置错误!");
            //    return false;
            //}
            showInformation = currentZone.Level == YuLinTu.Library.Entity.eZoneLevel.Group;
            if (!showInformation)
            {
                InitalizeDirectory();
            }
            return true;

        }

        /// <summary>
        /// 数据处理
        /// </summary>
        private void ArcDataProgress()
        {
            //YuLinTu.Library.Entity.ZoneCollection zones = database.Zone.GetSubZones(currentZone.FullCode, YuLinTu.Library.Basic.eLevelOption.AllSubLevel);
            List<YuLinTu.Library.Entity.Zone> zones = zoneStation.GetChildren(currentZone.FullCode, eLevelOption.SelfAndSubs);
            //List<YuLinTu.Library.Entity.Zone> noshapezones = zones.FindAll(z => z.Shape == null);
            if (zones == null)
            {
                this.ReportError(string.Format("地域{0}下无地域数据", currentZone.FullName));
                return;
            }
            //if (noshapezones == null || noshapezones.Count != 0)
            //{
            //    foreach (var item in noshapezones)
            //    {
            //        this.ReportError(string.Format("地域{0}下无空间地域数据", item.FullName));
            //    }
            //    return;
            //}
            bool canContinue = CanContinue();
            if (!canContinue)
            {
                return;
            }
            //YuLinTu.Library.Entity.ZoneCollection zoneCollection = InitalizeZoneData();
            // List<YuLinTu.Library.Entity.Zone> zoneCollection = InitalizeZoneData();
            //foreach (var zone in zones)
            //{
            //    zoneCollection.Add(zone);
            //}
            //zones = zoneCollection;
            DataExportProgress exportProgress = new DataExportProgress();
            ArcSpaceDataProgress spaceProgress = new ArcSpaceDataProgress();
            SqliteManager sqliteManager = new SqliteManager();
            canContinue = InitalizeAgricultureDirectory(exportProgress, spaceProgress, sqliteManager);
            if (!canContinue)
            {
                return;
            }
            //ReportProgress(new YuLinTu.Library.Basic.TaskProgressChangedEventArgs(1, string.Format("正在获取{0}数据", ZoneOperator.InitalizeZoneName(database, currentZone))));
            this.ReportProgress(1, string.Format("正在获取{0}数据", currentZone.FullName));
            int count = 0;
            List<DataSummary> summerys = new List<DataSummary>();
            string processInfo = "";
            for (int i = 0; i < zones.Count; i++)
            {
                YuLinTu.Library.Entity.Zone zone = zones[i];
                zoneName = GetZoneName(zones, zone);
                processInfo = string.Format("({0}/{1})", i + 1, zones.Count);
                DataSummary summery = new DataSummary();
                summery.UnitName = zone.FullName;
                summery.UnitCode = InitalizeZoneCode(zone);
                summery.Level = (YuLinTu.Library.AgriExchange.eZoneLevel)((int)(zone.Level));
                summery.ZoneCode = zone.FullCode;
                count = VirtualPersonStation.GetByZoneCode(zone.FullCode).Count();
                if (count <= 0)
                {
                    summerys.Add(summery);
                    continue;
                }
                if (count <= 0 && zone.Level == Library.Entity.eZoneLevel.Group)
                {
                    if (showInformation)
                    {
                        this.ReportAlert(eMessageGrade.Warn, null, zoneName + "下没有数据可供操作!");
                    }
                    else
                    {
                        WriteDataInformation("警告:" + zoneName + "下没有数据可供操作!");
                    }
                    continue;
                }
                this.ReportProgress(1, processInfo + ((CanChecker ? "开始检查" : "开始生成") + zoneName + "数据"));
                List<ComplexRightEntity> entityCollection = DataCheckProgress(zone, processInfo);
                if (!canExport || entityCollection == null || entityCollection.Count == 0)
                {
                    continue;
                }
                try
                {
                    if (ContainDot)
                    {
                        List<JZD> jzds = InitalizeDotData(filterLandIDs, zone.FullCode);//根据地域编码及（地块id集合）取出当前地域下界址点结合
                        sqliteManager.InsertPointIn(jzds);
                        jzds = null;
                        GC.Collect();
                    }
                    if (ContainLine)
                    {
                        List<JZX> jzxs = InitalizeLineData(filterLandIDs, zone.FullCode);//根据地域编码及地块id集合取出当前地域下界址点结合
                        sqliteManager.InsertLineIn(jzxs);
                        jzxs = null;
                        GC.Collect();
                    }
                    List<YuLinTu.Library.Entity.Zone> zonesexport = new List<Library.Entity.Zone>();
                    zonesexport.Add(county);
                    //exportProgress.ExportDataFile(entityCollection, sqliteManager, county.Name, 0, county.FullCode + county.Name, summery);
                    exportProgress.ExportDataFile(entityCollection, sqliteManager, county.Name, county.FullCode, 0, county.FullCode + county.Name, summery);
                    summerys.Add(summery);
                    this.ReportAlert(eMessageGrade.Infomation, null, zoneName + "下确权登记成果数据导出完毕!");
                }
                catch (Exception ex)
                {
                    if (showInformation)
                    {
                        this.ReportAlert(eMessageGrade.Exception, null, ex.Message);
                    }
                    else
                    {
                        WriteDataInformation("错误:" + ex.Message);
                    }
                }
                entityCollection = null;
                GC.Collect();
            }
            if (CheckCardNumber)
            {
                ContractorNubmerProgress();
            }
            if (!CanChecker || canExport)
            {
                string summeryPath = Folder + @"\" + county.FullCode + county.Name + @"\汇总表格";
                try
                {
                    ExportSummaryTable summarytable = new ExportSummaryTable(summerys, summeryPath, county.FullCode + county.Name);
                    summarytable.ExportTable();
                    if (!ContainMatrical)
                    {
                        string fileName = summeryPath + @"\" + county.FullCode + county.Name + "按承包地所有权性质汇总表.xls";
                        if (System.IO.File.Exists(fileName))
                        {
                            System.IO.File.Delete(fileName);
                        }
                        string imagePath = Folder + @"\" + county.FullCode + county.Name + @"\" + "扫描资料";
                        if (Directory.Exists(imagePath))
                        {
                            Directory.Delete(imagePath, true);
                        }
                    }
                    LandDataExportProgress(spaceProgress, sqliteManager, currentZone.FullCode);
                    List<YuLinTu.Library.AgriExchange.Zone> zoneData = TableDataExportProgress(zones);
                    AllDataExportProgress(spaceProgress, zoneData);
                    zoneData = null;
                    summerys = null;
                    zones = null;
                    GC.Collect();
                }
                catch (SystemException ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex.ToString());
                }
                return;
            }
            if (Directory.Exists(exportProgress.ShapeFilePath))
            {
                System.IO.DirectoryInfo directory = Directory.GetParent(exportProgress.ShapeFilePath);
                directory.Delete(true);
            }
            if (!showInformation)
            {
                string dataRecord = Application.StartupPath + @"\Error\DataChecker.txt";
                if (CanChecker)
                {
                    if (!canExport)
                    {
                        this.ReportAlert(eMessageGrade.Error, null, "导出" + currentZone.FullName + "下数据存在问题,请在" + dataRecord + "中查看详细信息...");
                    }
                }
                if (System.IO.File.Exists(dataRecord))
                {
                    System.Diagnostics.Process.Start(dataRecord);
                }
            }
            filterLandIDs = null;
            summerys = null;
            zones = null;
            GC.Collect();
        }

        /// <summary>
        /// 获取地域名称
        /// </summary>
        private string GetZoneName(List<YuLinTu.Library.Entity.Zone> zones, YuLinTu.Library.Entity.Zone zone)
        {
            string name = zone.Name;
            if (((int)zone.Level) >= ((int)YuLinTu.Library.Entity.eZoneLevel.Town))
                return name;
            var czone = zones.Find(t => t.FullCode == zone.UpLevelCode);
            while (czone != null && czone.Level < YuLinTu.Library.Entity.eZoneLevel.County)
            {
                name = czone.Name + name;
                czone = zones.Find(t => t.FullCode == czone.UpLevelCode);
            }
            return name;
        }

        /// <summary>
        /// 数据检查，包括获取当前地域下总的地块开始
        /// </summary>
        private List<ComplexRightEntity> DataCheckProgress(YuLinTu.Library.Entity.Zone zone, string preInfo)
        {
            currentIndex = 1;
            filterLandIDs = new List<Guid>();
            List<ContractConcord> concordCollection = concordStation.GetByZoneCode(zone.FullCode);
            List<ContractRegeditBook> bookCollection = contractRegeditBookStation.GetByZoneCode(zone.FullCode, eSearchOption.Precision);
            List<VirtualPerson> familyCollection = VirtualPersonStation.GetByZoneCode(zone.FullCode);
            List<ContractLand> AllLandCollection = contractLandWorkStation.GetCollection(zone.FullCode, eLevelOption.Self);
            CollectivityTissue tissue = senderStation.Get(zone.FullCode, zone.FullName);
            List<ContractLand> landCollection = FilterLandType(familyCollection, AllLandCollection);//根据设置筛选地块
            if (tissue == null)
            {
                tissue = senderStation.Get(zone.ID);
            }
            if (tissue == null)
            {
                string code = zone.FullCode;
                code = code.Length == 16 ? code.Substring(0, 12) + code.Substring(12, 2) : code.PadRight(14, '0');
                tissue = senderStation.GetByCode(code);
            }
            if (tissue != null)
            {
                tissue.Code = InitalizeSenderCode(tissue, zone);
            }
            if (tissue == null)
                return null;
            List<ContractLand> landSpaceCollection = landCollection.FindAll(l => l.Shape != null); if (CanChecker)
            {
                SenderProgress(tissue);
            }
            bool showPersent = familyCollection.Count >= 100 ? false : true;
            string progressDescription = "(" + familyCollection.Count.ToString() + ")";
            this.ReportProgress(currentIndex, preInfo + zoneName + (CanChecker ? "数据检查" : "数据生成"));
            foreach (VirtualPerson vp in familyCollection)
            {
                if (vp.Name.Contains("集体") || vp.Name.Contains("机动地"))
                {
                    continue;
                }
                if (CanChecker)
                {
                    ContractorProgress(vp);
                }
                else
                {
                    NoCheckerContractorProgress(vp);
                }
                List<ContractLand> lands = landCollection.FindAll(ld => ld.OwnerId != null && ld.OwnerId.HasValue && ld.OwnerId.Value == vp.ID);
                if (lands == null || lands.Count == 0)
                {
                    this.ReportInfomation(vp.Name + "没有地块信息");
                    continue;
                }
              
                string description = string.Format("{0}下承包方:{1}", zoneName, vp.Name);
                foreach (ContractLand land in lands)
                {
                    if (CanChecker)
                    {
                        ContractLandProgress(land);
                    }

                    string landNumber = land.LandNumber;                  
                    if (landNumber==null|| landNumber.Length != 19)
                    {
                        if (showInformation)
                        {
                            this.ReportAlert(eMessageGrade.Error, null, description + "地块编码"+ landNumber + "不符合农业部19位数字要求!");
                        }
                        else
                        {
                            WriteDataInformation("错误:" + description + "地块编码不符合农业部19位数字要求!");
                        }
                        canExport = CanChecker ? false : true;
                    }

                    ContractLand arcLand = landSpaceCollection.Find(ld => ld.ID == land.ID || ld.LandNumber == land.LandNumber);
                    if (arcLand == null)
                    {
                        if (showInformation)
                        {
                            this.ReportInfomation(description + "中地块编码为:" + land.LandNumber + "的地块无空间信息!");
                        }
                        else
                        {
                            WriteDataInformation("错误:" + description + "中地块编码为:" + land.LandNumber + "的地块无空间信息!");
                        }
                        canExport = CanChecker ? false : true;
                    }
                }
                lands = null;
                currentIndex++;
                this.ReportProgress(currentIndex, preInfo + zoneName + (CanChecker ? "数据检查" : "数据生成"));
                bool concordExist = concordCollection.Exists(cd => !string.IsNullOrEmpty(cd.ConcordNumber)
                    && cd.ContracterId != null && cd.ContracterId.HasValue && cd.ContracterId.Value == vp.ID && cd.IsValid);
                if (!concordExist)
                {
                    if (showInformation)
                    {
                        this.ReportWarn(description + "未签订承包合同!");
                        this.ReportWarn(description + "未签订承包权证!");
                        //return null;
                    }
                    else
                    {
                        WriteDataInformation("警告:" + description + "未签订承包合同!");
                        WriteDataInformation("警告:" + description + "未签订承包权证!");
                    }
                    canExport = CanChecker ? false : true;
                    continue;
                }
                List<ContractConcord> concords = concordCollection.FindAll(cd => !string.IsNullOrEmpty(cd.ConcordNumber) && cd.ContracterId != null && cd.ContracterId.HasValue && cd.ContracterId.Value == vp.ID);
                bool warrantExist = false;
                foreach (ContractConcord concord in concords)
                {
                    ContractRegeditBook book = bookCollection.Find(bk => !string.IsNullOrEmpty(concord.ConcordNumber) && bk.ID == concord.ID);
                    if (bookCollection.Count == 0 && !warrantExist)
                    {
                        book = contractRegeditBookStation.Get(concord.ID);
                    }
                    if (book != null && !string.IsNullOrEmpty(book.RegeditNumber))
                    {
                        warrantExist = true;
                        break;
                    }
                }
                concords = null;
                if (!warrantExist)
                {
                    if (showInformation)
                    {
                        this.ReportInfomation(description + "已签订承包合同!");
                        this.ReportWarn(description + "未签订承包权证!");
                        return null;
                    }
                    else
                    {
                        WriteDataInformation("提示:" + description + "已签订承包合同!");
                        WriteDataInformation("警告:" + description + "未签订承包权证!");
                    }
                    canExport = CanChecker ? false : true;
                }
            }
            ReportProgress(new TaskProgressChangedEventArgs(1, preInfo + zoneName + (CanChecker ? "数据检查完毕!" : "数据生成完毕!")));
            List<ComplexRightEntity> entityCollection = new List<ComplexRightEntity>();
            if (canExport)
            {
                this.ReportAlert(eMessageGrade.Infomation, null, string.Format("开始导出{0}下确权登记成果数据!", zoneName));
                currentIndex = 1;
                showPersent = concordCollection.Count >= 100 ? false : true;
                progressDescription = "(" + concordCollection.Count.ToString() + ")";
                foreach (ContractConcord concord in concordCollection)
                {
                    this.ReportProgress(currentIndex, preInfo + zoneName + "数据导出");
                    currentIndex++;
                    if (string.IsNullOrEmpty(concord.ConcordNumber) || !concord.IsValid)
                    {
                        continue;
                    }
                    VirtualPerson vp = familyCollection.Find(fam => fam.ID == concord.ContracterId.Value);
                    if (vp == null)
                    {
                        continue;
                    }
                    try
                    {
                        ComplexRightEntity entity = new ComplexRightEntity();
                        entity.ZoneCode = zone.FullCode;

                        entity.FBF = InitalizeSenderData(tissue);
                        entity.VirtualPersonCode = InitalizeContractorCode(vp, zone);
                        entity.CBF = InitalizeContractorData(vp, entity.VirtualPersonCode);
                        entity.JTCY = InitalizeSharePersonData(vp, entity.VirtualPersonCode);
                        List<ContractLand> lands = landSpaceCollection.FindAll(ld => ld.ConcordId != null && ld.ConcordId.HasValue && ld.ConcordId.Value == concord.ID);

                        List<CBDKXX> cbds = new List<CBDKXX>();
                        foreach (ContractLand land in lands)
                        {
                            filterLandIDs.Add(land.ID);
                            ContractLand arcLand = landSpaceCollection.Find(ld => ld.ID == land.ID || ld.LandNumber == land.LandNumber);
                            CBDKXX cbd = InitalizeAgricultureLandData(arcLand, tissue.Code, entity.VirtualPersonCode);
                            DK dk = InitalizeSpaceLandData(arcLand);
                            cbd.KJDK = dk;
                            cbd.CBHTBM = concord.ConcordNumber;
                            cbd.CBJYQZBM = concord.ConcordNumber;
                            cbds.Add(cbd);
                        }
                        lands = null;
                        entity.DKXX = cbds;
                        entity.HT = InitalizeConcordData(concord);
                        entity.HT.CBDKZS = landCollection.Count(ld => ld.ConcordId != null && ld.ConcordId.HasValue && ld.ConcordId.Value == concord.ID);
                        entity.HT.FBFBM = entity.FBF.FBFBM;
                        entity.HT.CBFBM = entity.CBF.CBFBM;
                        entity.DJB = InitalizeRegeditBook(concord);
                        entity.DJB.DKSYT = @"图件\" + concord.ConcordNumber + @"\DKSYT" + entity.CBF.CBFBM + "J1.jpg";
                        entity.DJB.FBFBM = entity.FBF.FBFBM;
                        entity.DJB.CBFBM = entity.CBF.CBFBM;
                        ContractRegeditBook book = bookCollection.Find(bk => bk.ID == concord.ID);
                        if (bookCollection.Count == 0 && book == null)
                        {
                            book = contractRegeditBookStation.Get(concord.ID);
                        }
                        entity.CBJYQZ = InitalizeWarrantBook(book, vp);
                        if (entity.CBJYQZ == null && string.IsNullOrEmpty(entity.CBJYQZ.CBJYQZBM))
                        {
                            this.ReportInfomation(zone.FullName + vp.Name + "权证号为空!");
                            continue;
                        }
                        if (ContainMatrical)
                        {
                            InitalizeAccessory(book, entity);
                        }
                        entityCollection.Add(entity);
                    }
                    catch (Exception sfdsa)
                    {                        
                        var mds = sfdsa.Message;
                    }
                }
            }
            landSpaceCollection = null;
            tissue = null;
            bookCollection = null;
            concordCollection = null;
            landCollection = null;
            familyCollection = null;
            GC.Collect();
            return entityCollection;
        }

        /// <summary>
        /// 发包方处理
        /// </summary>
        private void SenderProgress(CollectivityTissue tissue)
        {
            if (tissue == null)
            {
                return;
            }
            string description = string.Format("{0}下发包方:{1}中", zoneName, tissue.Name);
            if (string.IsNullOrEmpty(YuLinTu.Library.WorkStation.ToolString.ExceptSpaceString(tissue.LawyerName)))
            {
                if (showInformation)
                {
                    this.ReportAlert(eMessageGrade.Error, null, description + "发包方负责人姓名未填写!");
                }
                else
                {
                    WriteDataInformation("错误:" + description + "发包方负责人姓名未填写!");
                }
                canExport = false;
            }
            if (string.IsNullOrEmpty(YuLinTu.Library.WorkStation.ToolString.ExceptSpaceString(tissue.LawyerCartNumber)))
            {
                if (showInformation)
                {
                    this.ReportAlert(eMessageGrade.Error, null, description + "发包方负责人证件号码未填写!");
                }
                else
                {
                    WriteDataInformation("错误:" + description + "发包方负责人证件号码未填写!");
                }
                canExport = false;
            }
            if (tissue.LawyerCredentType == eCredentialsType.IdentifyCard && !string.IsNullOrEmpty(tissue.LawyerCartNumber))
            {
                bool isRight = YuLinTu.Library.WorkStation.ToolICN.Check(tissue.LawyerCartNumber);
                if (!isRight)
                {
                    if (showInformation)
                    {
                        this.ReportAlert(eMessageGrade.Infomation, null, description + "发包方负责人证件号码" + tissue.LawyerCartNumber + "不符合身份证算法验证规范!");
                    }
                    else
                    {
                        WriteDataInformation("警告:" + description + "发包方负责人证件号码" + tissue.LawyerCartNumber + "不符合身份证算法验证规范!");
                    }
                }
            }
            if (string.IsNullOrEmpty(YuLinTu.Library.WorkStation.ToolString.ExceptSpaceString(tissue.LawyerTelephone)))
            {
                if (showInformation)
                {
                    this.ReportAlert(eMessageGrade.Infomation, null, description + "发包方负责人联系电话未填写!");
                }
                else
                {
                    WriteDataInformation("提示:" + description + "发包方负责人联系电话未填写!");
                }
            }
            else
            {
                bool isRight = YuLinTu.Library.Business.ToolMath.MatchAllNumber(tissue.LawyerTelephone.Replace("+", "").Replace("-", ""));
                if (!isRight)
                {
                    if (showInformation)
                    {
                        this.ReportAlert(eMessageGrade.Error, null, description + "发包方负责人联系电话" + tissue.LawyerTelephone + "不符合数字要求!");
                    }
                    else
                    {
                        WriteDataInformation("错误:" + description + "发包方负责人联系电话" + tissue.LawyerTelephone + "不符合数字要求!");
                    }
                    canExport = false;
                }
            }
            if (string.IsNullOrEmpty(YuLinTu.Library.WorkStation.ToolString.ExceptSpaceString(tissue.LawyerAddress)))
            {
                if (showInformation)
                {
                    this.ReportAlert(eMessageGrade.Error, null, description + "发包方负责人地址未填写!");
                }
                else
                {
                    WriteDataInformation("错误:" + description + "发包方负责人地址未填写!");
                }
                canExport = false;
            }
            if (string.IsNullOrEmpty(YuLinTu.Library.WorkStation.ToolString.ExceptSpaceString(tissue.LawyerPosterNumber)))
            {
                if (showInformation)
                {
                    this.ReportAlert(eMessageGrade.Error, null, description + "邮政编码未填写!");
                }
                else
                {
                    WriteDataInformation("错误:" + description + "邮政编码未填写!");
                }
                canExport = false;
            }
            else
            {
                bool isRight = tissue.LawyerPosterNumber.Length == 6 && YuLinTu.Library.Business.ToolMath.MatchAllNumber(tissue.LawyerPosterNumber);
                if (!isRight)
                {
                    if (showInformation)
                    {
                        this.ReportAlert(eMessageGrade.Error, null, description + "发包方负责人邮政编码" + tissue.LawyerTelephone + "不符合数字要求!");
                    }
                    else
                    {
                        WriteDataInformation("错误:" + description + "发包方负责人邮政编码" + tissue.LawyerTelephone + "不符合数字要求!");
                    }
                    canExport = false;
                }
            }
            if (string.IsNullOrEmpty(YuLinTu.Library.WorkStation.ToolString.ExceptSpaceString(tissue.SurveyPerson)))
            {
                if (showInformation)
                {
                    this.ReportAlert(eMessageGrade.Error, null, description + "发包方调查员未填写!");
                }
                else
                {
                    WriteDataInformation("错误:" + description + "发包方调查员未填写!");
                }
                canExport = false;
            }
            if (tissue.SurveyDate == null || !tissue.SurveyDate.HasValue)
            {
                if (showInformation)
                {
                    this.ReportAlert(eMessageGrade.Error, null, description + "发包方调查日期未填写!");
                }
                else
                {
                    WriteDataInformation("错误:" + description + "发包方调查日期未填写!");
                }
                canExport = false;
            }
            if (string.IsNullOrEmpty(YuLinTu.Library.WorkStation.ToolString.ExceptSpaceString(tissue.SurveyChronicle)))
            {
                if (showInformation)
                {
                    this.ReportAlert(eMessageGrade.Infomation, null, description + "发包方调查记事未填写!");
                }
                else
                {
                    WriteDataInformation("提示:" + description + "发包方调查记事未填写!");
                }
            }
        }

        /// <summary>
        /// 承包方处理
        /// </summary>
        private void ContractorProgress(VirtualPerson vp)
        {
            VirtualPersonExpand expand = vp.FamilyExpand;
            string description = string.Format("{0}下承包方:{1}中", zoneName, vp.Name);
            if (expand != null)
            {
                int number = 0;
                Int32.TryParse(vp.FamilyNumber, out number);
                string familyType = EnumNameAttribute.GetDescription(expand.ContractorType);
                switch (expand.ContractorType)
                {
                    case eContractorType.Farmer:
                        if (number < 0 || number > 8000)
                        {
                            if (showInformation)
                            {
                                this.ReportAlert(eMessageGrade.Error, null, description + "承包方编码:" + string.Format("{0:D4}", number) + "与承包方类型：" + familyType + "不匹配,其值应大于0小于等于8000!");
                            }
                            else
                            {
                                WriteDataInformation("错误:" + description + "承包方编码:" + string.Format("{0:D4}", number) + "与承包方类型：" + familyType + "不匹配,其值应大于0小于等于8000!");
                            }
                        }
                        break;
                    case eContractorType.Personal:
                        if (number <= 8000 || number > 9000)
                        {
                            if (showInformation)
                            {
                                this.ReportAlert(eMessageGrade.Error, null, description + "承包方编码:" + string.Format("{0:D4}", number) + "与承包方类型：" + familyType + "不匹配,其值应在8001-9000间!");
                            }
                            else
                            {
                                WriteDataInformation("错误:" + description + "承包方编码:" + string.Format("{0:D4}", number) + "与承包方类型：" + familyType + "不匹配,其值应在8001-9000间!");
                            }
                        }
                        break;
                    case eContractorType.Unit:
                        if (number <= 9000 || number > 9999)
                        {
                            if (showInformation)
                            {
                                this.ReportAlert(eMessageGrade.Error, null, description + "承包方编码:" + string.Format("{0:D4}", number) + "与承包方类型：" + familyType + "不匹配,其值应在9001-9999间!");
                            }
                            else
                            {
                                WriteDataInformation("错误:" + description + "承包方编码:" + string.Format("{0:D4}", number) + "与承包方类型：" + familyType + "不匹配,其值应在9001-9999间!");
                            }
                        }
                        break;
                    default:
                        break;
                }
            }
            if (string.IsNullOrEmpty(YuLinTu.Library.WorkStation.ToolString.ExceptSpaceString(vp.Number)))
            {
                if (showInformation)
                {
                    this.ReportAlert(eMessageGrade.Error, null, description + "承包方证件号码未填写!");
                }
                else
                {
                    WriteDataInformation("错误:" + description + "承包方证件号码未填写!");
                }
                canExport = false;
            }
            if (vp.CardType == eCredentialsType.IdentifyCard && !string.IsNullOrEmpty(vp.Number))
            {
                bool isRight = YuLinTu.Library.WorkStation.ToolICN.Check(vp.Number);
                if (!isRight)
                {
                    if (showInformation)
                    {
                        this.ReportAlert(eMessageGrade.Infomation, null, description + "承包方证件号码" + vp.Number + "不符合身份证算法验证规范!");
                    }
                    else
                    {
                        WriteDataInformation("提示:" + description + "承包方证件号码" + vp.Number + "不符合身份证算法验证规范!");
                    }
                }
            }
            if (string.IsNullOrEmpty(YuLinTu.Library.WorkStation.ToolString.ExceptSpaceString(vp.Address)))
            {
                if (showInformation)
                {
                    this.ReportAlert(eMessageGrade.Error, null, description + "承包方地址未填写!");
                }
                else
                {
                    WriteDataInformation("错误:" + description + "承包方地址未填写!");
                }
                canExport = false;
            }
            if (string.IsNullOrEmpty(YuLinTu.Library.WorkStation.ToolString.ExceptSpaceString(vp.PostalNumber)))
            {
                if (showInformation)
                {
                    this.ReportAlert(eMessageGrade.Error, null, description + "承包方邮政编码未填写!");
                }
                else
                {
                    WriteDataInformation("错误:" + description + "承包方邮政编码未填写!");
                }
                canExport = false;
            }
            else
            {
                bool isRight = vp.PostalNumber.Length == 6 && YuLinTu.Library.Business.ToolMath.MatchAllNumber(vp.PostalNumber);
                if (!isRight)
                {
                    if (showInformation)
                    {
                        this.ReportAlert(eMessageGrade.Error, null, description + "承包方邮政编码" + vp.PostalNumber + "不符合数字要求!");
                    }
                    else
                    {
                        WriteDataInformation("错误:" + description + "承包方邮政编码" + vp.PostalNumber + "不符合数字要求!");
                    }
                    canExport = false;
                }
            }
            if (string.IsNullOrEmpty(YuLinTu.Library.WorkStation.ToolString.ExceptSpaceString(vp.Telephone)))
            {
                if (showInformation)
                {
                    this.ReportAlert(eMessageGrade.Infomation, null, description + "承包方联系电话未填写!");
                }
                else
                {
                    WriteDataInformation("提示:" + description + "承包方联系电话未填写!");
                }
            }
            else
            {
                bool isRight = YuLinTu.Library.Business.ToolMath.MatchAllNumber(vp.Telephone.Replace("+", "").Replace("-", ""));
                if (!isRight)
                {
                    if (showInformation)
                    {
                        this.ReportAlert(eMessageGrade.Error, null, description + "承包方联系电话" + vp.Telephone + "不符合数字要求!");
                    }
                    else
                    {
                        WriteDataInformation("错误:" + description + "承包方联系电话" + vp.Telephone + "不符合数字要求!");
                    }
                    canExport = false;
                }
            }
            if (expand != null)
            {
                if (string.IsNullOrEmpty(YuLinTu.Library.WorkStation.ToolString.ExceptSpaceString(expand.SurveyPerson)))
                {
                    if (showInformation)
                    {
                        this.ReportAlert(eMessageGrade.Error, null, description + "承包方调查员未填写!");
                    }
                    else
                    {
                        WriteDataInformation("错误:" + description + "承包方调查员未填写!");
                    }
                    canExport = false;
                }
                if (expand.SurveyDate == null || !expand.SurveyDate.HasValue)
                {
                    if (showInformation)
                    {
                        this.ReportAlert(eMessageGrade.Error, null, description + "承包方调查日期未填写!");
                    }
                    else
                    {
                        WriteDataInformation("错误:" + description + "承包方调查日期未填写!");
                    }
                    canExport = false;
                }
                if (string.IsNullOrEmpty(YuLinTu.Library.WorkStation.ToolString.ExceptSpaceString(expand.SurveyChronicle)))
                {
                    if (showInformation)
                    {
                        this.ReportAlert(eMessageGrade.Infomation, null, description + "承包方调查记事未填写!");
                    }
                    else
                    {
                        WriteDataInformation("提示:" + description + "承包方调查记事未填写!");
                    }
                }
                if (string.IsNullOrEmpty(YuLinTu.Library.WorkStation.ToolString.ExceptSpaceString(expand.PublicityChroniclePerson)))
                {
                    if (showInformation)
                    {
                        //this.ReportAlert(eMessageGrade.Error, null, description + "承包方公示记事人未填写!");
                        this.ReportInfomation(description + "承包方公示记事人未填写!");
                    }
                    else
                    {
                        WriteDataInformation("错误:" + description + "承包方公示记事人未填写!");
                    }
                    canExport = false;
                }
                if (string.IsNullOrEmpty(YuLinTu.Library.WorkStation.ToolString.ExceptSpaceString(expand.PublicityChronicle)))
                {
                    if (showInformation)
                    {
                        //this.ReportAlert(eMessageGrade.Infomation, null, description + "承包方公示记事未填写!");
                        this.ReportInfomation(description + "承包方公示记事未填写!");
                    }
                    else
                    {
                        WriteDataInformation("提示:" + description + "承包方公示记事未填写!");
                    }
                }
                if (string.IsNullOrEmpty(YuLinTu.Library.WorkStation.ToolString.ExceptSpaceString(expand.PublicityCheckPerson)))
                {
                    if (showInformation)
                    {
                        //this.ReportAlert(eMessageGrade.Error, null, description + "承包方公示审核人未填写!");
                        this.ReportInfomation(description + "承包方公示审核人未填写!");
                    }
                    else
                    {
                        WriteDataInformation("错误:" + description + "承包方公示审核人未填写!");
                    }
                    canExport = false;
                }
                if (expand.PublicityDate == null || !expand.PublicityDate.HasValue)
                {
                    if (showInformation)
                    {
                        //this.ReportAlert(eMessageGrade.Error, null, description + "承包方公示审核日期未填写!");
                        this.ReportInfomation(description + "承包方公示审核日期未填写!");
                    }
                    else
                    {
                        WriteDataInformation("错误:" + description + "承包方公示审核日期未填写!");
                    }
                    canExport = false;
                }
            }
            else
            {
                if (showInformation)
                {
                    //this.ReportAlert(eMessageGrade.Error, null, description + "承包方调查员未填写!");
                    //this.ReportAlert(eMessageGrade.Error, null, description + "承包方调查日期未填写!");
                    //this.ReportAlert(eMessageGrade.Infomation, null, description + "承包方调查记事未填写!");
                    //this.ReportAlert(eMessageGrade.Error, null, description + "承包方公示记事人未填写!");
                    //this.ReportAlert(eMessageGrade.Infomation, null, description + "承包方公示记事未填写!");
                    //this.ReportAlert(eMessageGrade.Error, null, description + "承包方公示审核人未填写!");
                    //this.ReportAlert(eMessageGrade.Error, null, description + "承包方公示审核日期未填写!");

                    this.ReportInfomation( description + "承包方调查员未填写!");
                    this.ReportInfomation( description + "承包方调查日期未填写!");
                    this.ReportInfomation( description + "承包方调查记事未填写!");
                    this.ReportInfomation(description + "承包方公示记事人未填写!");
                    this.ReportInfomation( description + "承包方公示记事未填写!");
                    this.ReportInfomation( description + "承包方公示审核人未填写!");
                    this.ReportInfomation( description + "承包方公示审核日期未填写!");
                }
                else
                {
                    WriteDataInformation("错误:" + description + "承包方调查员未填写!");
                    WriteDataInformation("错误:" + description + "承包方调查日期未填写!");
                    WriteDataInformation("提示:" + description + "承包方调查记事未填写!");
                    WriteDataInformation("错误:" + description + "承包方公示记事人未填写!");
                    WriteDataInformation("提示:" + description + "承包方公示记事未填写!");
                    WriteDataInformation("错误:" + description + "承包方公示审核人未填写!");
                    WriteDataInformation("错误:" + description + "承包方公示审核日期未填写!");
                }
                canExport = false;
            }
            PersonCollection persons = SortSharePerson(vp);
            foreach (Person person in persons)
            {
                if (person.Name == vp.Name && person.ICN == vp.Number && string.IsNullOrEmpty(YuLinTu.Library.WorkStation.ToolString.ExceptSpaceString(person.Relationship)))
                {
                    person.Relationship = "户主";
                }
                if (person.Gender != eGender.Male && person.Gender != eGender.Female)
                {
                    if (showInformation)
                    {
                        this.ReportAlert(eMessageGrade.Error, null, description + "家庭成员:" + person.Name + "性别未知!");
                    }
                    else
                    {
                        WriteDataInformation("错误:" + description + "家庭成员:" + person.Name + "性别未知!");
                    }
                    canExport = false;
                }
                if (string.IsNullOrEmpty(YuLinTu.Library.WorkStation.ToolString.ExceptSpaceString(person.ICN)))
                {
                    if (showInformation)
                    {
                        this.ReportAlert(eMessageGrade.Error, null, description + "家庭成员:" + person.Name + "证件号码未填写!");
                    }
                    else
                    {
                        WriteDataInformation("错误:" + description + "家庭成员:" + person.Name + "证件号码未填写!");
                    }
                    canExport = false;
                }
                if (person.CardType == eCredentialsType.IdentifyCard && !string.IsNullOrEmpty(person.ICN))
                {
                    bool isRight = YuLinTu.Library.WorkStation.ToolICN.Check(person.ICN);
                    if (!isRight)
                    {
                        if (showInformation)
                        {
                            this.ReportAlert(eMessageGrade.Infomation, null, description + "家庭成员:" + person.Name + "证件号码" + person.ICN + "不符合身份证算法验证规范!");
                        }
                        else
                        {
                            WriteDataInformation("错误:" + description + "家庭成员:" + person.Name + "证件号码" + person.ICN + "不符合身份证算法验证规范!");
                        }
                    }
                }
                if (person.Name != vp.Name)
                {
                    if (string.IsNullOrEmpty(YuLinTu.Library.WorkStation.ToolString.ExceptSpaceString(person.Relationship)))
                    {
                        if (showInformation)
                        {
                            this.ReportAlert(eMessageGrade.Error, null, description + "家庭成员:" + person.Name + "家庭关系未填写!");
                        }
                        else
                        {
                            WriteDataInformation("错误:" + description + "家庭成员:" + person.Name + "家庭关系未填写!");
                        }
                        canExport = false;
                    }
                    else
                    {
                        if (!relationList.Contains(person.Relationship))
                        {
                            if (showInformation)
                            {
                                this.ReportAlert(eMessageGrade.Error, null, description + "家庭成员:" + person.Name + "家庭关系" + person.Relationship + "不符合农业部家庭关系填写要求!");
                            }
                            else
                            {
                                WriteDataInformation("错误:" + description + "家庭成员:" + person.Name + "家庭关系" + person.Relationship + "不符合农业部家庭关系填写要求!");
                            }
                            canExport = false;
                        }
                    }
                }
            }
            persons = null;
        }

        /// <summary>
        /// 承包方处理-未点击检查下默认检查提示
        /// </summary>
        private void NoCheckerContractorProgress(VirtualPerson vp)
        {
            VirtualPersonExpand expand = vp.FamilyExpand;
            string description = string.Format("{0}下承包方:{1}中", zoneName, vp.Name);
            if (expand != null)
            {
                if (string.IsNullOrEmpty(YuLinTu.Library.WorkStation.ToolString.ExceptSpaceString(expand.SurveyPerson)))
                {
                    if (showInformation)
                    {
                        this.ReportAlert(eMessageGrade.Error, null, description + "承包方调查员未填写!");
                        //this.ReportInfomation(description + "承包方调查员未填写!");
                    }
                    else
                    {
                        WriteDataInformation("错误:" + description + "承包方调查员未填写!");
                    }
                    canExport = false;
                }
                if (expand.SurveyDate == null || !expand.SurveyDate.HasValue)
                {
                    if (showInformation)
                    {
                        this.ReportAlert(eMessageGrade.Error, null, description + "承包方调查日期未填写!");
                        //this.ReportInfomation(description + "承包方调查日期未填写!");
                    }
                    else
                    {
                        WriteDataInformation("错误:" + description + "承包方调查日期未填写!");
                    }
                    canExport = false;
                }
                if (string.IsNullOrEmpty(YuLinTu.Library.WorkStation.ToolString.ExceptSpaceString(expand.SurveyChronicle)))
                {
                    if (showInformation)
                    {
                        this.ReportAlert(eMessageGrade.Infomation, null, description + "承包方调查记事未填写!");
                        //this.ReportInfomation(description + "承包方调查记事未填写!");
                    }
                    else
                    {
                        WriteDataInformation("提示:" + description + "承包方调查记事未填写!");
                    }
                }
                if (string.IsNullOrEmpty(YuLinTu.Library.WorkStation.ToolString.ExceptSpaceString(expand.PublicityChroniclePerson)))
                {
                    if (showInformation)
                    {
                        this.ReportAlert(eMessageGrade.Error, null, description + "承包方公示记事人未填写!");
                        //this.ReportInfomation(description + "承包方公示记事人未填写!");
                    }
                    else
                    {
                        WriteDataInformation("错误:" + description + "承包方公示记事人未填写!");
                    }
                    canExport = false;
                }
                if (string.IsNullOrEmpty(YuLinTu.Library.WorkStation.ToolString.ExceptSpaceString(expand.PublicityChronicle)))
                {
                    if (showInformation)
                    {
                        this.ReportAlert(eMessageGrade.Infomation, null, description + "承包方公示记事未填写!");
                        //this.ReportInfomation(description + "承包方公示记事未填写!");
                    }
                    else
                    {
                        WriteDataInformation("提示:" + description + "承包方公示记事未填写!");
                    }
                }
                if (string.IsNullOrEmpty(YuLinTu.Library.WorkStation.ToolString.ExceptSpaceString(expand.PublicityCheckPerson)))
                {
                    if (showInformation)
                    {
                        this.ReportAlert(eMessageGrade.Error, null, description + "承包方公示审核人未填写!");
                        //this.ReportInfomation(description + "承包方公示审核人未填写!");
                    }
                    else
                    {
                        WriteDataInformation("错误:" + description + "承包方公示审核人未填写!");
                    }
                    canExport = false;
                }
                if (expand.PublicityDate == null || !expand.PublicityDate.HasValue)
                {
                    if (showInformation)
                    {
                        this.ReportAlert(eMessageGrade.Error, null, description + "承包方公示审核日期未填写!");
                        //this.ReportInfomation(description + "承包方公示审核日期未填写!");
                    }
                    else
                    {
                        WriteDataInformation("错误:" + description + "承包方公示审核日期未填写!");
                    }
                    canExport = false;
                }
            }
            else
            {
                if (showInformation)
                {
                    this.ReportAlert(eMessageGrade.Error, null, description + "承包方调查员未填写!");
                    this.ReportAlert(eMessageGrade.Error, null, description + "承包方调查日期未填写!");
                    this.ReportAlert(eMessageGrade.Infomation, null, description + "承包方调查记事未填写!");
                    this.ReportAlert(eMessageGrade.Error, null, description + "承包方公示记事人未填写!");
                    this.ReportAlert(eMessageGrade.Infomation, null, description + "承包方公示记事未填写!");
                    this.ReportAlert(eMessageGrade.Error, null, description + "承包方公示审核人未填写!");
                    this.ReportAlert(eMessageGrade.Error, null, description + "承包方公示审核日期未填写!");
                }
                else
                {
                    WriteDataInformation("错误:" + description + "承包方调查员未填写!");
                    WriteDataInformation("错误:" + description + "承包方调查日期未填写!");
                    WriteDataInformation("提示:" + description + "承包方调查记事未填写!");
                    WriteDataInformation("错误:" + description + "承包方公示记事人未填写!");
                    WriteDataInformation("提示:" + description + "承包方公示记事未填写!");
                    WriteDataInformation("错误:" + description + "承包方公示审核人未填写!");
                    WriteDataInformation("错误:" + description + "承包方公示审核日期未填写!");
                }
                canExport = false;
            }

        }


        /// <summary>
        /// 初始化家庭关系
        /// </summary>
        private List<string> InitalizeAllRelation()
        {
            List<string> list = new List<string>();
            list.Add("本人");
            list.Add("户主");
            list.Add("配偶");
            list.Add("夫");
            list.Add("妻");
            list.Add("父亲");
            list.Add("母亲");
            list.Add("子");
            list.Add("女");
            list.Add("独生子");
            list.Add("长子");
            list.Add("次子");
            list.Add("三子");
            list.Add("四子");
            list.Add("五子");
            list.Add("养子或继子");
            list.Add("其他儿子");
            list.Add("女婿");
            list.Add("独生女");
            list.Add("长女");
            list.Add("次女");
            list.Add("三女");
            list.Add("四女");
            list.Add("五女");
            list.Add("养女或继女");
            list.Add("儿媳");
            list.Add("其他女儿");
            list.Add("孙子");
            list.Add("孙女");
            list.Add("外孙子");
            list.Add("外孙女");
            list.Add("孙媳妇或外孙媳妇");
            list.Add("孙女婿或外孙女婿");
            list.Add("曾孙子或外曾孙子");
            list.Add("曾孙女或外曾孙女");
            list.Add("其他孙子、孙女或外孙子、外孙女");
            list.Add("公公");
            list.Add("婆婆");
            list.Add("岳父");
            list.Add("岳母");
            list.Add("继父或养父");
            list.Add("继母或养母");
            list.Add("其他父母关系");
            list.Add("祖父");
            list.Add("祖母");
            list.Add("外祖父");
            list.Add("外祖母");
            list.Add("配偶的祖父母或外祖父母");
            list.Add("曾祖父");
            list.Add("曾祖母");
            list.Add("配偶的曾祖父母或外曾祖父母");
            list.Add("其他祖父母或外祖父母关系");
            list.Add("兄");
            list.Add("嫂");
            list.Add("弟");
            list.Add("弟媳");
            list.Add("姐姐");
            list.Add("姐夫");
            list.Add("妹妹");
            list.Add("妹夫");
            list.Add("其他兄弟姐妹");
            list.Add("伯父");
            list.Add("伯母");
            list.Add("叔父");
            list.Add("婶母");
            list.Add("舅父");
            list.Add("舅母");
            list.Add("姨父");
            list.Add("姨母");
            list.Add("姑父");
            list.Add("姑母");
            list.Add("堂兄弟、堂姐妹");
            list.Add("表兄弟、表姐妹");
            list.Add("侄子");
            list.Add("侄女");
            list.Add("外甥");
            list.Add("外甥女");
            list.Add("其他亲属");
            list.Add("非亲属");
            return list;
        }

        /// <summary>
        /// 共有人排序
        /// </summary>
        /// <param name="vp"></param>
        /// <returns></returns>
        private PersonCollection SortSharePerson(VirtualPerson vp)
        {
            List<Person> fsp = vp.SharePersonList;
            if (fsp == null || fsp.Count == 0)
            {
                return new PersonCollection();
            }
            PersonCollection sharePersonCollection = new PersonCollection();
            foreach (Person person in fsp)
            {
                if (person.Name == vp.Name)
                {
                    sharePersonCollection.Add(person);
                    break;
                }
            }
            foreach (Person person in fsp)
            {
                if (person.Name != vp.Name)
                {
                    sharePersonCollection.Add(person);
                }
            }
            return sharePersonCollection;
        }

        /// <summary>
        /// 承包地块处理
        /// </summary>
        private void ContractLandProgress(ContractLand land)
        {
            string description = string.Format("{0}下承包方:{1}下地块编码为:{2}的地块", zoneName, land.OwnerName, land.LandNumber);
            if (string.IsNullOrEmpty(YuLinTu.Library.WorkStation.ToolString.ExceptSpaceString(land.Name)))
            {
                if (showInformation)
                {
                    this.ReportInfomation(description + "地块名称未填写!");
                }
                else
                {
                    WriteDataInformation("错误:" + description + "地块名称未填写!");
                }
                canExport = false;
            }
            string landNumber = land.LandNumber;
            bool numberError = false;
            if (landNumber.Length != 19)
            {
                numberError = true;
            }
            else
            {
                string medianString = landNumber.Substring(landNumber.Length - 5);
                int median = 0;
                Int32.TryParse(medianString, out median);
                if (median == 0)
                {
                    numberError = true;
                }
            }
            if (numberError)
            {
                if (showInformation)
                {
                    this.ReportInfomation(description + "地块编码不符合农业部19位数字要求!");
                }
                else
                {
                    WriteDataInformation("警告:" + description + "地块编码不符合农业部19位数字要求!");
                }
                canExport = false;
            }
            if (!string.IsNullOrEmpty(land.LandCode) && land.LandCode.Length != 3)
            {
                if (land.LandCode.IndexOf("XX") >= 0)
                {
                    if (showInformation)
                    {
                        this.ReportAlert(eMessageGrade.Error, null, description + "土地利用类型未知不符合农业部二级类型要求!");
                    }
                    else
                    {
                        WriteDataInformation("错误:" + description + "土地利用类型未知不符合农业部二级类型要求!");
                    }
                }
                else
                {
                    if (showInformation)
                    {
                        this.ReportAlert(eMessageGrade.Error, null, description + "土地利用类型填写不符合农业部二级类型要求!");
                    }
                    else
                    {
                        WriteDataInformation("错误:" + description + "土地利用类型填写不符合农业部二级类型要求!");
                    }
                }
                canExport = false;
            }
            if (land.LandLevel == "" || land.LandLevel == null)
            {
                if (showInformation)
                {
                    this.ReportAlert(eMessageGrade.Error, null, description + "地力等级未填写!");
                }
                else
                {
                    WriteDataInformation("错误:" + description + "地力等级未填写!");
                }
                canExport = false;
            }
            if (land.IsFarmerLand == null || !land.IsFarmerLand.HasValue)
            {
                if (showInformation)
                {
                    this.ReportAlert(eMessageGrade.Error, null, description + "是否基本农田未填写!");
                }
                else
                {
                    WriteDataInformation("错误:" + description + "是否基本农田未填写!");
                }
                canExport = false;
            }
            if (land.ActualArea <= 0.00)
            {
                if (showInformation)
                {
                    this.ReportAlert(eMessageGrade.Error, null, description + "实测面积未填写!");
                }
                else
                {
                    WriteDataInformation("错误:" + description + "实测面积未填写!");
                }
                canExport = false;
            }
            string[] neighbors = new string[] { land.NeighborEast, land.NeighborSouth, land.NeighborWest, land.NeighborNorth };
            if (neighbors != null && neighbors.Length > 0 && string.IsNullOrEmpty(YuLinTu.Library.WorkStation.ToolString.ExceptSpaceString(neighbors[0])))
            {
                if (showInformation)
                {
                    this.ReportAlert(eMessageGrade.Error, null, description + "四至东未填写!");
                }
                else
                {
                    WriteDataInformation("错误:" + description + "四至东未填写!");
                }
                canExport = false;
            }
            if (neighbors != null && neighbors.Length > 1 && string.IsNullOrEmpty(YuLinTu.Library.WorkStation.ToolString.ExceptSpaceString(neighbors[1])))
            {
                if (showInformation)
                {
                    this.ReportAlert(eMessageGrade.Error, null, description + "四至南未填写!");
                }
                else
                {
                    WriteDataInformation("错误:" + description + "四至南未填写!");
                }
                canExport = false;
            }
            if (neighbors != null && neighbors.Length > 2 && string.IsNullOrEmpty(YuLinTu.Library.WorkStation.ToolString.ExceptSpaceString(neighbors[2])))
            {
                if (showInformation)
                {
                    this.ReportAlert(eMessageGrade.Error, null, description + "四至西未填写!");
                }
                else
                {
                    WriteDataInformation("错误:" + description + "四至西未填写!");
                }
                canExport = false;
            }
            if (neighbors != null && neighbors.Length > 3 && string.IsNullOrEmpty(YuLinTu.Library.WorkStation.ToolString.ExceptSpaceString(neighbors[3])))
            {
                if (showInformation)
                {
                    this.ReportAlert(eMessageGrade.Error, null, description + "四至北未填写!");
                }
                else
                {
                    WriteDataInformation("错误:" + description + "四至北未填写!");
                }
                canExport = false;
            }
            if (land == null || string.IsNullOrEmpty(YuLinTu.Library.WorkStation.ToolString.ExceptSpaceString(land.LandExpand.ReferPerson)))
            {
                if (showInformation)
                {
                    this.ReportAlert(eMessageGrade.Error, null, description + "指界人未填写!");
                }
                else
                {
                    WriteDataInformation("错误:" + description + "指界人未填写!");
                }
            }
        }

        /// <summary>
        /// 承包方证件号码检查
        /// </summary>
        private void ContractorNubmerProgress()
        {
            //VirtualPersonCollection familyCollection = database.LandVirtualPerson.GetByZoneCode(currentZone.FullCode, YuLinTu.Library.Basic.eLevelOption.AllSubLevel);
            //var rePNums = from f in familyCollection
            //              let m = new FamilySharePerson(f.SharePerson)
            //              from p in m.SharePersons.Select(t => new { Address = f.LandLocated, Person = t })
            //              group p by null == p.Person.ICN ? String.Empty : p.Person.ICN.Trim() into g
            //              where g.Count() > 1
            //              select new
            //              {
            //                  Number = g.Key,
            //                  People = g.ToList()
            //              };
            List<VirtualPerson> familyCollection = VirtualPersonStation.GetByZoneCode(currentZone.FullCode, eLevelOption.SelfAndSubs);
            var rePNums = from f in familyCollection
                          let m = f.SharePersonList
                          from p in m.Select(t => new { Address = f.Address, Person = t })
                          group p by null == p.Person.ICN ? String.Empty : p.Person.ICN.Trim() into g
                          where g.Count() > 1
                          select new
                          {
                              Number = g.Key,
                              People = g.ToList()
                          };
            foreach (var person in rePNums)
            {
                if (string.IsNullOrEmpty(person.Number))
                {
                    continue;
                }
                string information = String.Join("、", person.People.Select(p => p.Address).Distinct().ToArray());
                string description = string.Format("证件号码:{0}重复,存在于{1}!", person.Number, information);
                if (showInformation)
                {
                    this.ReportAlert(eMessageGrade.Infomation, null, description);
                }
                else
                {
                    WriteDataInformation("提示:" + description);
                }
            }
            rePNums = null;
            familyCollection = null;
            GC.Collect();
        }

        /// <summary>
        /// 获取系统坐标系
        /// </summary>
        /// <returns></returns>
        //private ISpatialReference InitalizeSystemReference()
        //{
        //    if (gisInstance == null)
        //    {
        //        return null;
        //    }
        //    try
        //    {
        //        IProviderArcGis provider = gisInstance.Metadata.Provider as IProviderArcGis;
        //        if (provider == null || provider.Workspace == null)
        //        {
        //            return null;
        //        }
        //        workspace = provider.WorkspaceFeature;
        //        IEnumDataset enumDateset = provider.Workspace.get_Datasets(esriDatasetType.esriDTFeatureClass);
        //        if (enumDateset == null)
        //        {
        //            return null;
        //        }
        //        IDataset dataset = enumDateset.Next() as IDataset;
        //        IGeoDataset geoDataset = dataset as IGeoDataset;
        //        if (geoDataset == null)
        //        {
        //            return null;
        //        }
        //        return geoDataset.SpatialReference;
        //    }
        //    catch (SystemException ex)
        //    {
        //        System.Diagnostics.Debug.WriteLine(ex.ToString());
        //    }
        //    return null;
        //}

        /// <summary>
        /// 是否继续
        /// </summary>
        /// <returns></returns>
        private bool CanContinue()
        {
            bool canContinue = true;
            //int familyCount = database.LandVirtualPerson.SL_Count("ZoneCode", currentZone.FullCode, Library.Data.ConditionOption.Like_LeftFixed);          
            List<YuLinTu.Library.Entity.VirtualPerson> spots = VirtualPersonStation.GetByZoneCode(currentZone.FullCode, eLevelOption.SelfAndSubs);
            int familyCount = spots.Count();
            if (familyCount <= 0)
            {
                this.ReportAlert(eMessageGrade.Warn, null, "选择地域下不存在承包方与地块数据,无法执行导出操作!");
                canContinue = false;
            }
            string fileName = Application.StartupPath + @"\ShapeTemplate";
            if (!Directory.Exists(fileName))
            {
                this.ReportAlert(eMessageGrade.Error, null, "矢量数据文件模板不存在!");
                canContinue = false;
            }
            fileName = Application.StartupPath + @"\Template\DataBase.mdb";
            if (!File.Exists(fileName))
            {
                this.ReportAlert(eMessageGrade.Error, null, "权属数据文件模板不存在!");
                canContinue = false;
            }
            fileName = Application.StartupPath + @"\Template\db.sqlite";
            if (!File.Exists(fileName))
            {
                this.ReportAlert(eMessageGrade.Error, null, "临时数据库文件模板不存在!");
                canContinue = false;
            }
            return canContinue;
        }

        /// <summary>
        /// 根据设置筛选地块
        /// </summary>
        /// <param name="allLands">所有地块</param>     
        public List<YuLinTu.Library.Entity.ContractLand> FilterLandType(List<YuLinTu.Library.Entity.VirtualPerson> allVirtualPersons, List<YuLinTu.Library.Entity.ContractLand> allLands)
        {
            List<YuLinTu.Library.Entity.ContractLand> filterLands = new List<ContractLand>();
            //添加的方法
            //Parallel.ForEach(allLands, new Action<ContractLand>(item =>
            //{
            //    lock (allLands)
            //    {
            //        var vp = allVirtualPersons.Find(v => v.ID == item.OwnerId);
            //        if (TaskExportLandDotCoilDefine.Farmer && vp.FamilyExpand.ContractorType == eContractorType.Farmer)
            //        {
            //            filterLands.Add(item);
            //        }
            //        if (TaskExportLandDotCoilDefine.Personal && vp.FamilyExpand.ContractorType == eContractorType.Personal)
            //        {
            //            filterLands.Add(item);
            //        }
            //        if (TaskExportLandDotCoilDefine.Unit && vp.FamilyExpand.ContractorType == eContractorType.Unit)
            //        {
            //            filterLands.Add(item);
            //        }

            //        if (TaskExportLandDotCoilDefine.ContractLand && item.LandCategory == ((int)eConstructType.ContractLand).ToString())
            //        {
            //            filterLands.Add(item);
            //        }
            //        if (TaskExportLandDotCoilDefine.AbandonedLand && item.LandCategory == ((int)eConstructType.AbandonedLand).ToString())
            //        {
            //            filterLands.Add(item);
            //        }
            //        if (TaskExportLandDotCoilDefine.CollectiveLand && item.LandCategory == ((int)eConstructType.CollectiveLand).ToString())
            //        {
            //            filterLands.Add(item);
            //        }
            //        if (TaskExportLandDotCoilDefine.EncollecLand && item.LandCategory == ((int)eConstructType.EncollecLand).ToString())
            //        {
            //            filterLands.Add(item);
            //        }
            //        if (TaskExportLandDotCoilDefine.FeedLand && item.LandCategory == ((int)eConstructType.FeedLand).ToString())
            //        {
            //            filterLands.Add(item);
            //        }
            //        if (TaskExportLandDotCoilDefine.MotorizeLand && item.LandCategory == ((int)eConstructType.MotorizeLand).ToString())
            //        {
            //            filterLands.Add(item);
            //        }
            //        if (TaskExportLandDotCoilDefine.PrivateLand && item.LandCategory == ((int)eConstructType.PrivateLand).ToString())
            //        {
            //            filterLands.Add(item);
            //        }
            //        if (TaskExportLandDotCoilDefine.WasteLand && item.LandCategory == ((int)eConstructType.WasteLand).ToString())
            //        {
            //            filterLands.Add(item);
            //        }
            //    }
            //}
            //));
            if (!TaskExportLandDotCoilDefine.Farmer)
            {
                allLands.RemoveAll((item) =>
                {
                    bool rev = false;
                    var vp = allVirtualPersons.Find(v => v.ID == item.OwnerId);
                    if (vp == null || vp.FamilyExpand == null)
                    {
                        return rev;
                    }
                    if (vp.FamilyExpand.ContractorType == eContractorType.Farmer) { rev = true; }
                    return rev;
                });
            }
            if (!TaskExportLandDotCoilDefine.Personal)
            {
                allLands.RemoveAll((item) =>
                {
                    bool rev = false;
                    var vp = allVirtualPersons.Find(v => v.ID == item.OwnerId);
                    if (vp == null || vp.FamilyExpand == null)
                    {
                        return rev;
                    }
                    if (vp.FamilyExpand.ContractorType == eContractorType.Personal) { rev = true; }
                    return rev;
                });
            }
            if (!TaskExportLandDotCoilDefine.Unit)
            {
                allLands.RemoveAll((item) =>
                {
                    bool rev = false;
                    var vp = allVirtualPersons.Find(v => v.ID == item.OwnerId);
                    if (vp == null || vp.FamilyExpand == null)
                    {
                        return rev;
                    }
                    if (vp.FamilyExpand.ContractorType == eContractorType.Unit) { rev = true; }
                    return rev;
                });
            }

            if (!TaskExportLandDotCoilDefine.ContractLand)
            {
                allLands.RemoveAll((item) =>
                {
                    bool rev = false;
                    if (item.LandCategory == ((int)eLandCategoryType.ContractLand).ToString()) { rev = true; }
                    return rev;
                });
            }

            if (!TaskExportLandDotCoilDefine.AbandonedLand)
            {
                allLands.RemoveAll((item) =>
                {
                    bool rev = false;
                    if (item.LandCategory == ((int)eLandCategoryType.AbandonedLand).ToString()) { rev = true; }
                    return rev;
                });
            }

            if (!TaskExportLandDotCoilDefine.CollectiveLand)
            {
                allLands.RemoveAll((item) =>
                {
                    bool rev = false;
                    if (item.LandCategory == ((int)eLandCategoryType.CollectiveLand).ToString()) { rev = true; }
                    return rev;
                });
            }
            if (!TaskExportLandDotCoilDefine.EncollecLand)
            {
                allLands.RemoveAll((item) =>
                {
                    bool rev = false;
                    if (item.LandCategory == ((int)eLandCategoryType.EncollecLand).ToString()) { rev = true; }
                    return rev;
                });
            }
            if (!TaskExportLandDotCoilDefine.FeedLand)
            {
                allLands.RemoveAll((item) =>
                {
                    bool rev = false;
                    if (item.LandCategory == ((int)eLandCategoryType.FeedLand).ToString()) { rev = true; }
                    return rev;
                });
            }
            if (!TaskExportLandDotCoilDefine.MotorizeLand)
            {
                allLands.RemoveAll((item) =>
                {
                    bool rev = false;
                    if (item.LandCategory == ((int)eLandCategoryType.MotorizeLand).ToString()) { rev = true; }
                    return rev;
                });
            }
            if (!TaskExportLandDotCoilDefine.PrivateLand)
            {
                allLands.RemoveAll((item) =>
                {
                    bool rev = false;
                    if (item.LandCategory == ((int)eLandCategoryType.PrivateLand).ToString()) { rev = true; }
                    return rev;
                });
            }
            if (!TaskExportLandDotCoilDefine.WasteLand)
            {
                allLands.RemoveAll((item) =>
                {
                    bool rev = false;
                    if (item.LandCategory == ((int)eLandCategoryType.WasteLand).ToString()) { rev = true; }
                    return rev;
                });
            }
            filterLands = allLands;
            return filterLands;
        }

        #endregion

        #region 数据转换

        /// <summary>
        /// 初始化地域代码
        /// </summary>
        /// <param name="zone"></param>
        /// <returns></returns>
        private string InitalizeZoneCode(YuLinTu.Library.Entity.Zone zone)
        {
            //string zoneCode = ZoneOperator.InitalzieAgricultureGroupCode(zone.FullCode);
            string zoneCode = zone.FullCode;
            switch (zone.Level)
            {
                case Library.Entity.eZoneLevel.County:
                    zoneCode += "00000000";
                    break;
                case Library.Entity.eZoneLevel.Town:
                    zoneCode += "00000";
                    break;
                case Library.Entity.eZoneLevel.Village:
                    zoneCode += "00";
                    break;
                default:
                    break;
            }
            return zoneCode;
        }

        /// <summary>
        /// 获得承包方编码
        /// </summary>
        /// <param name="contractor"></param>
        /// <returns></returns>
        private string InitalizeContractorCode(VirtualPerson contractor, YuLinTu.Library.Entity.Zone zone)
        {
            if (contractor == null || zone == null)
            {
                return "";
            }
            int number = 0;
            Int32.TryParse(contractor.FamilyNumber, out number);
            string familyNumber = null;
            switch (zone.Level)
            {
                case YuLinTu.Library.Entity.eZoneLevel.Village:
                    familyNumber = zone.FullCode + "00" + string.Format("{0:D4}", number);
                    break;
                case YuLinTu.Library.Entity.eZoneLevel.Group:
                    //familyNumber = zone.FullCode.Substring(0, 12) + zone.FullCode.Substring(14, 2) + string.Format("{0:D4}", number);
                    familyNumber = zone.FullCode + string.Format("{0:D4}", number);
                    break;
                default:
                    break;
            }
            return familyNumber;
        }

        /// <summary>
        /// 获得发包方编码
        /// </summary>
        /// <param name="tissue">发包方</param>
        /// <returns>发包方编码</returns>
        private string InitalizeSenderCode(CollectivityTissue tissue, YuLinTu.Library.Entity.Zone zone)
        {
            string code = "";
            if (tissue == null)
            {
                code = zone != null ? zone.FullCode : "";
            }
            else
            {
                code = tissue.Code;
            }
            if (code.Length == 12)
            {
                code = code + "00";
            }
            if (code.Length > 14)
            {
                code = code.Substring(0, 12) + code.Substring(14, 2);
            }
            return code;
        }

        /// <summary>
        /// 初始化发包方数据
        /// </summary>
        private FBF InitalizeSenderData(CollectivityTissue tissue)
        {
            if (tissue == null)
            {
                return null;
            }
            FBF fbf = new FBF();
            fbf.FBFBM = tissue.Code;
            fbf.FBFMC = tissue.Name;
            fbf.FBFFZRXM = tissue.LawyerName;
            fbf.FZRZJLX = ((int)(tissue.LawyerCredentType)).ToString();
            fbf.FZRZJHM = tissue.LawyerCartNumber;
            fbf.LXDH = tissue.LawyerTelephone;
            fbf.FBFDZ = tissue.LawyerAddress;
            fbf.YZBM = tissue.LawyerPosterNumber;
            fbf.FBFDCY = tissue.SurveyPerson;
            if (tissue.SurveyDate != null && tissue.SurveyDate.HasValue)
            {
                fbf.FBFDCRQ = tissue.SurveyDate.Value;
            }
            fbf.FBFDCJS = tissue.LawyerTelephone;
            return fbf;
        }

        /// <summary>
        /// 获得承包方实体
        /// </summary>
        /// <returns></returns>
        public CBF InitalizeContractorData(VirtualPerson vp, string familyNumber)
        {
            if (vp == null)
            {
                return null;
            }
            CBF cbf = new CBF();
            cbf.CBFBM = familyNumber;
            cbf.CBFLX = vp.FamilyExpand != null ? ((int)(vp.FamilyExpand.ContractorType)).ToString() : "1";
            cbf.CBFMC = vp.Name;
            cbf.CBFZJLX = ((int)(vp.CardType)).ToString();
            cbf.CBFZJHM = vp.Number;
            cbf.CBFDZ = vp.Address;
            cbf.YZBM = vp.PostalNumber;
            cbf.LXDH = vp.Telephone;
            List<Person> fsp = vp.SharePersonList;
            if (fsp != null)
            {
                cbf.CBFCYSL = fsp.Count;
            }
            if (vp.FamilyExpand != null)
            {
                if (vp.FamilyExpand.SurveyDate != null && vp.FamilyExpand.SurveyDate.HasValue)
                {
                    cbf.CBFDCRQ = vp.FamilyExpand.SurveyDate;
                }
                cbf.CBFDCY = vp.FamilyExpand.SurveyPerson;
                cbf.CBFDCJS = vp.FamilyExpand.SurveyChronicle;
                cbf.GSJS = vp.FamilyExpand.PublicityChronicle;
                cbf.GSJSR = vp.FamilyExpand.PublicityChroniclePerson;
                if (vp.FamilyExpand.PublicityDate != null && vp.FamilyExpand.PublicityDate.HasValue)
                {
                    cbf.GSSHRQ = vp.FamilyExpand.PublicityDate.Value;
                }
                cbf.GSSHR = vp.FamilyExpand.PublicityCheckPerson;
            }
            return cbf;
        }

        /// <summary>
        /// 初始化共有人信息
        /// </summary>
        /// <returns></returns>
        private List<CBF_JTCY> InitalizeSharePersonData(VirtualPerson vp, string familyNumber)
        {
            List<CBF_JTCY> persons = new List<CBF_JTCY>();
            if (vp == null)
            {
                return persons;
            }
            List<Person> fsp = vp.SharePersonList;
            foreach (Person person in fsp)
            {
                if (person.Name == vp.Name && person.Relationship != "户主" && person.Relationship != "本人")
                {
                    person.Relationship = "户主";
                }
                CBF_JTCY jtcy = new CBF_JTCY();
                jtcy.CBFBM = familyNumber;
                jtcy.CYXM = person.Name;
                jtcy.CYXB = person.Gender == eGender.Male ? "1" : "2";
                jtcy.CYZJLX = ((int)(person.CardType)).ToString();
                jtcy.CYZJHM = person.ICN;
                jtcy.YHZGX = RelationShipMapping.NameMapping(person.Relationship);
                jtcy.CYBZ = InitalizeSharePersonComment(person.Comment);
                if (person.Name == vp.Name && person.ICN == vp.Number)
                {
                    jtcy.SFGYR = "1";
                }
                else
                {
                    jtcy.SFGYR = person.IsSharedLand == "是" ? "1" : "2";
                }
                persons.Add(jtcy);
            }
            return persons;
        }

        /// <summary>
        /// 获得 成员备注
        /// </summary>
        /// <param name="comment">备注</param>
        /// <returns>成员备注</returns>
        private string InitalizeSharePersonComment(string comment)
        {
            string value = "";
            switch (comment)
            {
                case "外嫁女":
                    value = "1";
                    break;
                case "入赘男":
                    value = "2";
                    break;
                case "在校大学生":
                    value = "3";
                    break;
                case "国家公职人员":
                    value = "4";
                    break;
                case "军人（军官、士兵）":
                case "军人":
                case "军官":
                case "士兵":
                    value = "5";
                    break;
                case "新生儿":
                    value = "6";
                    break;
                case "去世":
                    value = "7";
                    break;
                case "其他备注":
                    value = "9";
                    break;
                default:
                    value = "9";
                    break;
            }
            return value;
        }

        /// <summary>
        /// 获得承包地块实体
        /// </summary>
        /// <returns></returns>
        private CBDKXX InitalizeAgricultureLandData(ContractLand land, string senderCode, string familyNumber)
        {
            if (land == null)
            {
                return null;
            }
            CBDKXX cbdkxx = new CBDKXX();
            string landCode = land.LandNumber;
            cbdkxx.DKBM = landCode;
            cbdkxx.FBFBM = senderCode;
            cbdkxx.CBFBM = familyNumber;
            cbdkxx.HTMJ = Math.Round(land.ActualArea / 0.0015, 2);
            cbdkxx.CBJYQQDFS = land.ConstructMode;
            return cbdkxx;
        }

        /// <summary>
        /// 初始化空间地块
        /// </summary>
        /// <returns></returns>
        private DK InitalizeSpaceLandData(ContractLand land)
        {
            try
            {

                if (land == null)
                {
                    return null;
                }
                DK dk = new DK();
                dk.YSDM = "211011";
                //dk.DKBM = ContractLand.GetLandNumber(land.CadastralNumber);
                dk.DKBM = land.LandNumber;
                dk.DKMC = land.Name;
                dk.SYQXZ = "30";
                dk.DKLB = land.LandCategory;
                dk.TDLYLX = land.LandCode;
                dk.DLDJ = land.LandLevel;
                dk.TDYT = land.Purpose;
                if (land.IsFarmerLand != null && land.IsFarmerLand.HasValue)
                {
                    dk.SFJBNT = land.IsFarmerLand.Value ? "1" : "2";
                }
                dk.SCMJ = Math.Round(land.ActualArea / 0.0015, 2);
                dk.DKDZ = land.NeighborEast;
                dk.DKNZ = land.NeighborSouth;
                dk.DKXZ = land.NeighborWest;
                dk.DKBZ = land.NeighborNorth;
                dk.DKBZXX = land.Comment;
                if (land.LandExpand != null && land.LandExpand.ReferPerson != null)
                {
                    if (land.LandExpand.ReferPerson != "")
                    {
                        dk.ZJRXM = land.LandExpand.ReferPerson;
                    }
                    else
                    {
                        dk.ZJRXM = land.OwnerName;
                    }
                }
                else
                {
                    dk.ZJRXM = land.OwnerName;
                }
                dk.Shape = land.Shape == null ? null : land.Shape.Instance;
                return dk;
            }
            catch (Exception fds)
            {
                var dsafdsa = fds.Message;
                throw;
            }
        }

        /// <summary>
        /// 获得地块类别
        /// </summary>
        /// <param name="constructType"></param>
        /// <returns></returns>
        private string InitalizeLandCatalog(eLandCategoryType constructType)
        {
            string value = "";
            switch (constructType)
            {
                case eLandCategoryType.ContractLand:
                    value = "10";
                    break;
                case eLandCategoryType.MotorizeLand:
                    value = "22";
                    break;
                case eLandCategoryType.PrivateLand:
                    value = "21";
                    break;
                case eLandCategoryType.WasteLand:
                    value = "23";
                    break;
                default:
                    value = "99";
                    break;
            }
            return value;
        }

        /// <summary>
        /// 获得地力等级
        /// </summary>
        /// <param name="contractLandLevel">eContractLandLevel</param>
        /// <returns>地力等级</returns>
        private string InitalizeLandLevel(eContractLandLevel contractLandLevel)
        {
            string value = "";
            switch (contractLandLevel)
            {
                case eContractLandLevel.EightLevel:
                    value = "08";
                    break;
                case eContractLandLevel.FiveLevel:
                    value = "05";
                    break;
                case eContractLandLevel.FourLevel:
                    value = "04";
                    break;
                case eContractLandLevel.NineLevel:
                    value = "09";
                    break;
                case eContractLandLevel.OneLevel:
                    value = "01";
                    break;
                case eContractLandLevel.SevenLevel:
                    value = "07";
                    break;
                case eContractLandLevel.SixLevel:
                    value = "06";
                    break;
                case eContractLandLevel.TenLevel:
                    value = "10";
                    break;
                case eContractLandLevel.ThreeLevel:
                    value = "03";
                    break;
                case eContractLandLevel.TwoLevel:
                    value = "02";
                    break;
                default:
                    value = "";
                    break;
            }
            return value;
        }

        /// <summary>
        /// 获得 土地用途
        /// </summary>
        /// <param name="landPurposeType">eLandPurposeType</param>
        /// <returns> 土地用途</returns>
        private string InitalizeLandPurpose(eLandPurposeType landPurposeType)
        {
            string value = "";
            switch (landPurposeType)
            {
                case eLandPurposeType.Cultred:
                    value = "3";
                    break;
                case eLandPurposeType.Fish:
                    value = "4";
                    break;
                case eLandPurposeType.Other:
                    value = "5";
                    break;
                case eLandPurposeType.Planting:
                    value = "1";
                    break;
                case eLandPurposeType.WoodPlant:
                    value = "2";
                    break;
                default:
                    value = "5";
                    break;
            }
            return value;
        }

        /// <summary>
        /// 获得 承包经营权取得方式
        /// </summary>
        /// <param name="constructMode">eConstructMode</param>
        /// <returns>承包经营权取得方式代码</returns>
        private string InitalizeContractMode(eConstructMode constructMode)
        {
            string value = "";
            switch (constructMode)
            {
                case eConstructMode.Consensus:
                    value = "123";
                    break;
                case eConstructMode.Exchange:
                    value = "300";
                    break;
                case eConstructMode.Family:
                    value = "110";
                    break;
                case eConstructMode.Other:
                    value = "900";
                    break;
                case eConstructMode.Tenderee:
                    value = "121";
                    break;
                case eConstructMode.Transfer:
                    value = "200";
                    break;
                case eConstructMode.Vendue:
                    value = "122";
                    break;
                default:
                    value = "900";
                    break;
            }
            return value;
        }

        /// <summary>
        /// 初始化界址点数据
        /// </summary>
        /// <returns></returns>
        private List<JZD> InitalizeDotData(ContractLand land, List<BuildLandBoundaryAddressDot> dots)
        {
            List<JZD> jzds = new List<JZD>();
            if (land == null || dots == null)
            {
                return jzds;
            }
            List<BuildLandBoundaryAddressDot> dotCollection = dots.FindAll(dt => dt.LandID == land.ID);
            foreach (BuildLandBoundaryAddressDot dot in dotCollection)
            {
                JZD jzd = new JZD();
                jzd.YSDM = FeatureCode.JZD;
                jzd.JZDH = dot.DotNumber;
                jzd.JZDLX = dot.DotType;
                jzd.JBLX = dot.LandMarkType.ToString();
                //if (dot.XCoordinate != null && dot.XCoordinate.HasValue &&
                //    dot.YCoordinate != null && dot.YCoordinate.HasValue)
                //{
                //    IPoint point = new PointClass();
                //    point.PutCoords(dot.XCoordinate.Value, dot.YCoordinate.Value);
                //    if (reference != null)
                //    {
                //        point.Project(reference);
                //    }
                //    point.SnapToSpatialReference();
                jzd.Shape = dot.Shape == null ? null : dot.Shape.Instance;
                //}
                jzds.Add(jzd);
            }
            dotCollection = null;
            return jzds;
        }

        /// <summary>
        /// 初始化界址点数据
        /// </summary>
        /// <returns></returns>
        private List<JZX> InitalizeLineData(ContractLand land, List<BuildLandBoundaryAddressCoil> lines, List<BuildLandBoundaryAddressDot> dots)
        {
            List<JZX> jzxs = new List<JZX>();
            if (land == null || lines == null || dots == null)
            {
                return jzxs;
            }
            List<BuildLandBoundaryAddressCoil> lineCollection = lines.FindAll(le => le.LandID == land.ID);
            foreach (BuildLandBoundaryAddressCoil line in lineCollection)
            {
                JZX jzx = new JZX();
                jzx.YSDM = FeatureCode.JZX;
                jzx.JXXZ = line.LineType.ToString();
                jzx.JZXLB = line.CoilType.ToString();
                jzx.JZXWZ = line.Position.ToString() == "0" ? "1" : line.Position.ToString();
                jzx.JZXSM = line.Description;
                jzx.PLDWQLR = line.NeighborPerson;
                jzx.PLDWZJR = line.NeighborFefer;
                var startDot = jzdStation.Get(line.StartPointID);
                var endDot = jzdStation.Get(line.EndPointID);
                if (startDot != null && endDot != null)
                {
                    //if (startDot.XCoordinate != null && startDot.XCoordinate.HasValue
                    //    && startDot.YCoordinate != null && startDot.YCoordinate.HasValue
                    //    && endDot.XCoordinate != null && endDot.XCoordinate.HasValue
                    //    && endDot.YCoordinate != null && endDot.YCoordinate.HasValue)
                    //{
                    //    IPoint startPoint = new PointClass();
                    //    IPoint endPoint = new PointClass();
                    //    startPoint.PutCoords(startDot.XCoordinate.Value, startDot.YCoordinate.Value);
                    //    endPoint.PutCoords(endDot.XCoordinate.Value, endDot.YCoordinate.Value);
                    //    IPointCollection pointCollection = new PolylineClass();
                    //    pointCollection.AddPoint(startPoint);
                    //    pointCollection.AddPoint(endPoint);
                    //    IPolyline polyline = pointCollection as IPolyline;
                    //    polyline.Project(reference);
                    //    polyline.SnapToSpatialReference();
                    //    ITopologicalOperator topoOperation = polyline as ITopologicalOperator;
                    //    topoOperation.Simplify();
                    jzx.Shape = line.Shape == null ? null : line.Shape.Instance;
                    //}
                }
                jzxs.Add(jzx);
            }
            lineCollection = null;
            return jzxs;
        }

        /// <summary>
        /// 承包合同实体
        /// </summary>
        /// <returns></returns>
        private CBHT InitalizeConcordData(ContractConcord concord)
        {
            if (concord == null)
            {
                return null;
            }
            CBHT cbht = new CBHT();
            cbht.CBHTBM = concord.ConcordNumber;
            cbht.YCBHTBM = concord.ConcordNumber;
            cbht.CBFS = concord.ArableLandType;
            cbht.CBQXQ = concord.ArableLandStartTime.Value;
            cbht.CBQXZ = concord.ArableLandEndTime.Value;
            cbht.HTZMJ = Math.Round(concord.CountActualArea / 0.0015, 2);
            cbht.QDSJ = concord.ContractDate.Value;
            return cbht;
        }

        /// <summary>
        /// 初始化登记簿实体
        /// </summary>
        /// <returns></returns>
        private CBJYQZDJB InitalizeRegeditBook(ContractConcord concord)
        {
            if (concord == null)
            {
                return null;
            }
            CBJYQZDJB djb = new CBJYQZDJB();
            djb.CBJYQZBM = concord.ConcordNumber;
            //djb.CBFS = concord.ArableLandType == eConstructMode.Family ? "110" : "120";
            djb.CBFS = concord.ArableLandType;
            djb.CBQXQ = concord.ArableLandStartTime.Value;
            djb.CBQXZ = concord.ArableLandEndTime.Value;
            djb.CBQX = concord.ManagementTime;
            return djb;
        }

        /// <summary>
        /// 初始化权证实体
        /// </summary>
        /// <returns></returns>
        private CBJYQZ InitalizeWarrantBook(ContractRegeditBook regeditBook, VirtualPerson vp)
        {
            if (regeditBook == null)
            {
                return null;
            }
            CBJYQZ qz = new CBJYQZ();
            qz.CBJYQZBM = regeditBook.RegeditNumber;
            qz.FZJG = regeditBook.SendOrganization;
            qz.FZRQ = regeditBook.PrintDate;
            qz.QZSFLQ = "1";
            qz.QZLQRQ = regeditBook.PrintDate;
            qz.QZLQRXM = string.IsNullOrEmpty(vp.Name) ? " " : vp.Name.Length > 10 ? vp.Name.Substring(0, 10) : vp.Name;
            qz.QZLQRZJHM = vp.Number;
            qz.QZLQRZJLX = ((int)(vp.CardType)).ToString();
            if (qz.QZLQRZJLX == "0")
            {
                qz.QZLQRZJLX = "1";
            }
            return qz;
        }

        /// <summary>
        /// 初始化权属来源资料附件
        /// </summary>
        /// <returns></returns>
        private void InitalizeAccessory(ContractRegeditBook regeditBook, ComplexRightEntity entity)
        {
            if (!ContainMatrical || regeditBook == null || string.IsNullOrEmpty(regeditBook.RegeditNumber))
            {
                return;
            }
            QSLYZLFJ attch = new QSLYZLFJ();
            attch.CBJYQZBM = regeditBook.RegeditNumber;
            attch.CLFJMC = entity.CBF.CBFMC;
            attch.CLFJBH = entity.CBF.CBFBM;
            attch.ZLFJRQ = regeditBook.PrintDate;
            attch.FJ = @"扫描资料\权属来源证明资料附件\" + regeditBook.RegeditNumber + @"\P" + regeditBook.RegeditNumber + "CBHT2_1.jpg";
            if (entity.FJ == null)
            {
                entity.FJ = new List<QSLYZLFJ>();
            }
            entity.FJ.Add(attch);
        }

        /// <summary>
        /// 初始化县级行政区
        /// </summary>
        /// <param name="zones"></param>
        /// <returns></returns>
        private List<XJXZQ> InitalizeCountyData(List<YuLinTu.Library.AgriExchange.Zone> zones)
        {
            List<XJXZQ> countys = new List<XJXZQ>();
            foreach (var zone in zones)
            {
                if (zone.Level != Library.AgriExchange.eZoneLevel.County)
                {
                    continue;
                }
                XJXZQ county = new XJXZQ();
                county.YSDM = FeatureCode.XJXZQ;
                county.XZQDM = zone.FullCode;
                county.XZQMC = zone.Name;
                county.Shape = zone.Shape;
                countys.Add(county);
            }
            return countys;
        }

        /// <summary>
        /// 初始化乡镇级行政区
        /// </summary>
        /// <param name="zones"></param>
        /// <returns></returns>
        private List<XJQY> InitalizeTownData(List<YuLinTu.Library.AgriExchange.Zone> zones)
        {
            List<XJQY> towns = new List<XJQY>();
            foreach (var zone in zones)
            {
                if (zone.Level != Library.AgriExchange.eZoneLevel.Town)
                {
                    continue;
                }
                XJQY town = new XJQY();
                town.YSDM = FeatureCode.XJQY;
                town.XJQYDM = zone.FullCode;
                town.XJQYMC = zone.Name;
                town.Shape = zone.Shape;
                towns.Add(town);
            }
            return towns;
        }

        /// <summary>
        /// 初始化村级行政区
        /// </summary>
        /// <param name="zones"></param>
        /// <returns></returns>
        private List<CJQY> InitalizeVillageData(List<YuLinTu.Library.AgriExchange.Zone> zones)
        {
            List<CJQY> villages = new List<CJQY>();
            foreach (var zone in zones)
            {
                if (zone.Level != Library.AgriExchange.eZoneLevel.Village)
                {
                    continue;
                }
                CJQY village = new CJQY();
                village.YSDM = FeatureCode.CJQY;
                village.CJQYDM = zone.FullCode;
                village.CJQYMC = zone.Name;
                village.Shape = zone.Shape;
                villages.Add(village);
            }
            return villages;
        }

        /// <summary>
        /// 初始化组级行政区
        /// </summary>
        /// <param name="zones"></param>
        /// <returns></returns>
        private List<ZJQY> InitalizeGroupData(List<YuLinTu.Library.AgriExchange.Zone> zones)
        {
            List<ZJQY> groups = new List<ZJQY>();
            foreach (var zone in zones)
            {
                if (zone.Level != Library.AgriExchange.eZoneLevel.Group)
                {
                    continue;
                }
                ZJQY group = new ZJQY();
                group.YSDM = FeatureCode.ZJQY;
                group.ZJQYDM = zone.FullCode;
                group.ZJQYMC = zone.Name;
                //group.ZJQYMC = zone.Name;
                group.Shape = zone.Shape;
                groups.Add(group);
            }
            return groups;
        }

        /// <summary>
        /// 初始化点状地物
        /// </summary>
        /// <returns></returns>
        private List<YuLinTu.Library.AgriExchange.DZDW> InitalizePointData()
        {
            // List<YuLinTu.Library.Entity.Zone> zones = zoneStation.GetChildren(currentZone.FullCode, eLevelOption.SelfAndSubs);
            List<YuLinTu.Library.Entity.DZDW> points = dzdwStation.Get(t => t.ZoneCode.StartsWith(currentZone.FullCode));//.GetByZoneCode(z.FullCode) new List<YuLinTu.Library.Entity.DZDW>();
            //zones.ForEach(z => points.AddRange(dzdwStation.GetByZoneCode(z.FullCode)));

            List<YuLinTu.Library.AgriExchange.DZDW> dzdws = new List<YuLinTu.Library.AgriExchange.DZDW>();
            int startIndex = QuantityValue.PointThing;
            foreach (var point in points)
            {
                YuLinTu.Library.AgriExchange.DZDW dzdw = new YuLinTu.Library.AgriExchange.DZDW();
                dzdw.BSM = startIndex;
                dzdw.YSDM = point.YSDM;
                dzdw.BZ = point.Comment;
                dzdw.DWMC = point.DWMC;
                dzdw.Shape = point.Shape == null ? null : point.Shape.Instance;
                dzdws.Add(dzdw);
                startIndex++;
            }
            points = null;
            return dzdws;
        }

        /// <summary>
        /// 初始化线状地物
        /// </summary>
        /// <returns></returns>
        private List<YuLinTu.Library.AgriExchange.XZDW> InitalizePolylineData()
        {
            // List<YuLinTu.Library.Entity.XZDW> roads = xzdwStation.GetByZoneCode(currentZone.FullCode);
            // ArcGisWaterCollection waters = gisInstance.ArcGisWater.GetByZoneCode(currentZone.FullCode, Library.Data.ConditionOption.Like_LeftFixed);
            // int startIndex = QuantityValue.LineThing;

            // List<YuLinTu.Library.Entity.Zone> zones = zoneStation.GetChildren(currentZone.FullCode, eLevelOption.SelfAndSubs);
            List<YuLinTu.Library.Entity.XZDW> roads = xzdwStation.Get(t => t.ZoneCode.StartsWith(currentZone.FullCode));// new List<YuLinTu.Library.Entity.XZDW>();
            //zones.ForEach(z => roads.AddRange(xzdwStation.GetByZoneCode(z.FullCode)));

            int startIndex = QuantityValue.LineThing;
            List<YuLinTu.Library.AgriExchange.XZDW> xzdws = new List<YuLinTu.Library.AgriExchange.XZDW>();
            foreach (var road in roads)
            {
                YuLinTu.Library.AgriExchange.XZDW xzdw = new YuLinTu.Library.AgriExchange.XZDW();
                xzdw.BSM = startIndex;
                xzdw.YSDM = road.YSDM;
                xzdw.BZ = road.Comment;
                xzdw.DWMC = road.DWMC;
                xzdw.CD = road.CD;
                xzdw.KD = 0.1;
                xzdw.Shape = road.Shape == null ? null : road.Shape.Instance;
                xzdws.Add(xzdw);
                startIndex++;
            }
            //foreach (var water in waters)
            //{
            //    XZDW xzdw = new XZDW();
            //    xzdw.BSM = startIndex;
            //    xzdw.YSDM = FeatureCode.XZDW;
            //    xzdw.BZ = water.Comment;
            //    xzdw.DWMC = water.Name;
            //    xzdw.CD = water.Shape.Length;
            //    xzdw.KD = 0.1;
            //    xzdw.Shape = water.Shape != null ? water.Shape.ToNormalGeometry() : null;
            //    xzdws.Add(xzdw);
            //    startIndex++;
            //}
            roads = null;
            //waters = null;
            return xzdws;
        }

        /// <summary>
        /// 初始化面状地物
        /// </summary>
        /// <returns></returns>
        private List<YuLinTu.Library.AgriExchange.MZDW> InitalizePolygonData()
        {
            //List<YuLinTu.Library.Entity.MZDW> spots = mzdwStation.GetByZoneCode(currentZone.FullCode);
            //List<YuLinTu.Library.Entity.Zone> zones = zoneStation.GetChildren(currentZone.FullCode, eLevelOption.SelfAndSubs);
            List<YuLinTu.Library.Entity.MZDW> spots = mzdwStation.Get(t => t.ZoneCode.StartsWith(currentZone.FullCode));// new List<YuLinTu.Library.Entity.MZDW>();
            //zones.ForEach(z => spots.AddRange(mzdwStation.GetByZoneCode(z.FullCode)));

            List<YuLinTu.Library.AgriExchange.MZDW> mzdws = new List<YuLinTu.Library.AgriExchange.MZDW>();
            int startIndex = QuantityValue.AreaThing;
            foreach (var spot in spots)
            {
                YuLinTu.Library.AgriExchange.MZDW mzdw = new YuLinTu.Library.AgriExchange.MZDW();
                mzdw.BSM = startIndex;
                mzdw.YSDM = spot.YSDM;
                mzdw.BZ = spot.Comment;
                mzdw.DWMC = spot.DWMC;
                if (spot.Shape != null)
                {
                    mzdw.MJ = spot.Shape.Area();
                }
                mzdw.Shape = spot.Shape == null ? null : spot.Shape.Instance;
                mzdws.Add(mzdw);
                startIndex++;
            }
            spots = null;
            return mzdws;
        }

        /// <summary>
        /// 初始化控制点数据
        /// </summary>
        /// <returns></returns>
        private List<KZD> InitalizeControlPoint()
        {
            //List<YuLinTu.Library.Entity.Zone> zones = zoneStation.GetChildren(currentZone.FullCode, eLevelOption.SelfAndSubs);
            List<YuLinTu.Library.Entity.ControlPoint> points = kzdStation.Get(t => t.ZoneCode.StartsWith(currentZone.FullCode));// new List<YuLinTu.Library.Entity.ControlPoint>();
            //zones.ForEach(z => points.AddRange(kzdStation.GetByZoneCode(z.FullCode)));
            int startIndex = QuantityValue.ControlPoint;
            List<KZD> kzds = new List<KZD>();
            foreach (var point in points)
            {
                YuLinTu.Library.AgriExchange.KZD kzd = new YuLinTu.Library.AgriExchange.KZD();
                kzd.BSM = startIndex;
                kzd.YSDM = point.FeatureCode;
                kzd.KZDMC = point.PointName;
                kzd.KZDDH = point.PointNumber;
                kzd.KZDLX = point.PointType;
                kzd.KZDDJ = point.PointRank;
                kzd.BSLX = point.BsType;
                kzd.BZLX = point.BzType;
                kzd.KZDZT = point.PointState;
                kzd.DZJ = point.Dzj;
                kzd.X2000 = point.X2000.Value;
                kzd.Y2000 = point.Y2000.Value;
                kzd.X80 = point.X80;
                kzd.Y80 = point.Y80;
                kzd.Shape = point.Shape == null ? null : point.Shape.Instance;
                kzds.Add(kzd);
                startIndex++;
            }
            points = null;

            //if (workspace == null)
            //{
            //    return kzds;
            //}
            //IWorkspace2 workSpace = workspace as IWorkspace2;
            //bool canContinue = workSpace != null ? workSpace.get_NameExists(esriDatasetType.esriDTFeatureClass, "ControlPoint") : false;
            //if (!canContinue)
            //{
            //    return kzds;
            //}
            //IFeatureClass featureClass = workspace.OpenFeatureClass("ControlPoint");
            //if (featureClass == null)
            //{
            //    return kzds;
            //}
            //List<IFeature> features = FeatureUtility.GetFeatureCollection(featureClass);
            //int index = -1;
            //System.Object val = null;
            //double value = 0.0;
            //int startIndex = QuantityValue.ControlPoint;
            //foreach (IFeature feature in features)
            //{
            //    KZD kzd = new KZD();
            //    kzd.BSM = startIndex;
            //    kzd.YSDM = "111000";
            //    index = feature.Fields.FindField("KZDMC");
            //    if (index >= 0)
            //    {
            //        val = feature.get_Value(index);
            //        kzd.KZDMC = val != null ? val.ToString() : "";
            //    }
            //    index = feature.Fields.FindField("KZDDH");
            //    if (index >= 0)
            //    {
            //        val = feature.get_Value(index);
            //        kzd.KZDDH = val != null ? val.ToString() : "";
            //    }
            //    index = feature.Fields.FindField("KZDLX");
            //    if (index >= 0)
            //    {
            //        val = feature.get_Value(index);
            //        kzd.KZDLX = val != null ? val.ToString() : "";
            //    }
            //    index = feature.Fields.FindField("KZDDJ");
            //    if (index >= 0)
            //    {
            //        val = feature.get_Value(index);
            //        kzd.KZDDJ = val != null ? val.ToString() : "";
            //    }
            //    index = feature.Fields.FindField("BSLX");
            //    if (index >= 0)
            //    {
            //        val = feature.get_Value(index);
            //        kzd.BSLX = val != null ? val.ToString() : "";
            //    }
            //    index = feature.Fields.FindField("BZLX");
            //    if (index >= 0)
            //    {
            //        val = feature.get_Value(index);
            //        kzd.BZLX = val != null ? val.ToString() : "";
            //    }
            //    index = feature.Fields.FindField("KZDZT");
            //    if (index >= 0)
            //    {
            //        val = feature.get_Value(index);
            //        kzd.KZDZT = val != null ? val.ToString() : "";
            //    }
            //    index = feature.Fields.FindField("DZJ");
            //    if (index >= 0)
            //    {
            //        val = feature.get_Value(index);
            //        kzd.DZJ = val != null ? val.ToString() : "";
            //    }
            //    index = feature.Fields.FindField("X2000");
            //    if (index >= 0)
            //    {
            //        val = feature.get_Value(index);
            //        if (val != null)
            //        {
            //            double.TryParse(val.ToString(), out value);
            //        }
            //        else
            //        {
            //            value = 0.0;
            //        }
            //        kzd.X2000 = value;
            //    }
            //    index = feature.Fields.FindField("Y2000");
            //    if (index >= 0)
            //    {
            //        val = feature.get_Value(index);
            //        if (val != null)
            //        {
            //            double.TryParse(val.ToString(), out value);
            //        }
            //        else
            //        {
            //            value = 0.0;
            //        }
            //        kzd.Y2000 = value;
            //    }
            //    index = feature.Fields.FindField("X80");
            //    if (index >= 0)
            //    {
            //        val = feature.get_Value(index);
            //        if (val != null)
            //        {
            //            double.TryParse(val.ToString(), out value);
            //        }
            //        else
            //        {
            //            value = 0.0;
            //        }
            //        kzd.X80 = value;
            //    }
            //    index = feature.Fields.FindField("Y80");
            //    if (index >= 0)
            //    {
            //        val = feature.get_Value(index);
            //        if (val != null)
            //        {
            //            double.TryParse(val.ToString(), out value);
            //        }
            //        else
            //        {
            //            value = 0.0;
            //        }
            //        kzd.Y80 = value;
            //    }
            //    if (UseArcShape)
            //    {
            //        kzd.Shape = feature.Shape;
            //    }
            //    else
            //    {
            //        kzd.Shape = feature.Shape.ToNormalGeometry();
            //    }
            //    kzds.Add(kzd);
            //    startIndex++;
            //}
            //features = null;
            //GC.Collect();
            return kzds;
        }

        /// <summary>
        /// 初始化区域界线数据
        /// </summary>
        /// <returns></returns>
        private List<QYJX> InitalizeRegionData()
        {
            //List<YuLinTu.Library.Entity.Zone> zones = zoneStation.GetChildren(currentZone.FullCode, eLevelOption.SelfAndSubs);
            List<YuLinTu.Library.Entity.ZoneBoundary> points = qyjxStation.Get(t => t.ZoneCode.StartsWith(currentZone.FullCode));// new List<YuLinTu.Library.Entity.ZoneBoundary>();
            //zones.ForEach(z => points.AddRange(qyjxStation.GetByZoneCode(z.FullCode)));
            int startIndex = QuantityValue.AreaLine;
            List<QYJX> regions = new List<QYJX>();
            foreach (var point in points)
            {
                YuLinTu.Library.AgriExchange.QYJX qyjx = new YuLinTu.Library.AgriExchange.QYJX();
                qyjx.BSM = startIndex;
                qyjx.YSDM = point.FeatureCode;
                qyjx.JXLX = point.BoundaryLineType;
                qyjx.JXXZ = point.BoundaryLineNature;

                qyjx.Shape = point.Shape == null ? null : point.Shape.Instance;
                regions.Add(qyjx);
                startIndex++;
            }
            points = null;


            //if (workspace == null)
            //{
            //    return regions;
            //}
            //IWorkspace2 workSpace = workspace as IWorkspace2;
            //bool canContinue = workSpace != null ? workSpace.get_NameExists(esriDatasetType.esriDTFeatureClass, "QYJX") : false;
            //if (!canContinue)
            //{
            //    return regions;
            //}
            //IFeatureClass featureClass = workspace.OpenFeatureClass("QYJX");
            //if (featureClass == null)
            //{
            //    return regions;
            //}
            //List<IFeature> features = FeatureUtility.GetFeatureCollection(featureClass);
            //int index = -1;
            //System.Object val = null;
            //int startIndex = QuantityValue.AreaLine;
            //foreach (IFeature feature in features)
            //{
            //    QYJX qyjx = new QYJX();
            //    qyjx.BSM = startIndex;
            //    qyjx.YSDM = "161051";
            //    index = feature.Fields.FindField("JXLX");
            //    if (index >= 0)
            //    {
            //        val = feature.get_Value(index);
            //        qyjx.JXLX = val != null ? val.ToString() : "";
            //    }
            //    index = feature.Fields.FindField("JXXZ");
            //    if (index >= 0)
            //    {
            //        val = feature.get_Value(index);
            //        qyjx.JXXZ = val != null ? val.ToString() : "";
            //    }
            //    if (UseArcShape)
            //    {
            //        qyjx.Shape = feature.Shape;
            //    }
            //    else
            //    {
            //        qyjx.Shape = feature.Shape.ToNormalGeometry();
            //    }
            //    regions.Add(qyjx);
            //    startIndex++;
            //}
            //features = null;
            //GC.Collect();
            return regions;
        }

        /// <summary>
        /// 初始化基本农田保护区数据
        /// </summary>
        /// <returns></returns>
        private List<JBNTBHQ> InitalizeFarmerRegionData()
        {
            //List<YuLinTu.Library.Entity.Zone> zones = zoneStation.GetChildren(currentZone.FullCode, eLevelOption.SelfAndSubs);
            List<YuLinTu.Library.Entity.FarmLandConserve> points = jbntbhqStation.Get(t => t.ZoneCode.StartsWith(currentZone.FullCode));// new List<YuLinTu.Library.Entity.FarmLandConserve>();
            // zones.ForEach(z => points.AddRange(jbntbhqStation.GetByZoneCode(z.FullCode)));
            int startIndex = QuantityValue.ProtectArea;
            List<JBNTBHQ> farmers = new List<JBNTBHQ>();
            foreach (var point in points)
            {
                YuLinTu.Library.AgriExchange.JBNTBHQ jbnt = new YuLinTu.Library.AgriExchange.JBNTBHQ();
                jbnt.BSM = startIndex;
                jbnt.YSDM = point.FeatureCode;
                jbnt.BHQBH = point.ConserveNumber;
                jbnt.JBNTMJ = point.FarmLandArea.Value;

                jbnt.Shape = point.Shape == null ? null : point.Shape.Instance;
                farmers.Add(jbnt);
                startIndex++;
            }
            points = null;




            //if (workspace == null)
            //{
            //    return farmers;
            //}
            //IWorkspace2 workSpace = workspace as IWorkspace2;
            //bool canContinue = workSpace != null ? workSpace.get_NameExists(esriDatasetType.esriDTFeatureClass, "JBNTBHQ") : false;
            //if (!canContinue)
            //{
            //    return farmers;
            //}
            //IFeatureClass featureClass = workspace.OpenFeatureClass("JBNTBHQ");
            //if (featureClass == null)
            //{
            //    return farmers;
            //}
            //List<IFeature> features = FeatureUtility.GetFeatureCollection(featureClass);
            //int index = -1;
            //System.Object val = null;
            //double value = 0.0;
            //int startIndex = QuantityValue.ProtectArea;
            //foreach (IFeature feature in features)
            //{
            //    JBNTBHQ farmer = new JBNTBHQ();
            //    farmer.BSM = startIndex;
            //    farmer.YSDM = "251100";
            //    index = feature.Fields.FindField("BHQBH");
            //    if (index >= 0)
            //    {
            //        val = feature.get_Value(index);
            //        farmer.BHQBH = val != null ? val.ToString() : "";
            //    }
            //    index = feature.Fields.FindField("JBNTMJ");
            //    if (index >= 0)
            //    {
            //        val = feature.get_Value(index);
            //        if (val == null)
            //        {
            //            value = 0.0;
            //        }
            //        else
            //        {
            //            double.TryParse(val.ToString(), out value);
            //        }
            //        farmer.JBNTMJ = value;
            //    }
            //    if (UseArcShape)
            //    {
            //        farmer.Shape = feature.Shape;
            //    }
            //    else
            //    {
            //        farmer.Shape = feature.Shape.ToNormalGeometry();
            //    }
            //    farmers.Add(farmer);
            //    startIndex++;
            //}
            //features = null;
            //GC.Collect();

            return farmers;
        }

        /// <summary>
        /// 两点距离
        /// </summary>
        private bool IsRepitePoint(YuLinTu.Spatial.Coordinate t, YuLinTu.Spatial.Coordinate item)
        {
            return ((Math.Pow(t.X - item.X, 2) + Math.Pow(t.Y - item.Y, 2)) < 0.025);
        }


        /// <summary>
        /// 初始化界址点数据
        /// </summary>
        /// <returns></returns>
        private List<JZD> InitalizeDotData(List<Guid> landIDs, string zoneCode)
        {
            List<JZD> jzds = new List<JZD>();
            //List<YuLinTu.Library.Entity.Zone> zones = zoneStation.GetChildren(currentZone.FullCode, eLevelOption.SelfAndSubs);
            //List<YuLinTu.Library.Entity.BuildLandBoundaryAddressDot> allrepeatDots = new List<BuildLandBoundaryAddressDot>();
            //zones.ForEach(z => allrepeatDots.AddRange(jzdStation.GetByZoneCode(z.FullCode, eSearchOption.Precision, ((int)eLandPropertyRightType.AgricultureLand).ToString())));
            string landtype = ((int)eLandPropertyRightType.AgricultureLand).ToString();
            // var jzdQuery = jzdStation.Get(t => t.ZoneCode == zoneCode && t.LandType == landtype);
            //  var allrepeatDots = jzdQuery.ToList();
            var allrepeatDots = jzdStation.GetByZoneCode(zoneCode, eSearchOption.Precision, landtype);
            List<YuLinTu.Library.Entity.BuildLandBoundaryAddressDot> repeatDots = new List<BuildLandBoundaryAddressDot>();
            Parallel.ForEach(landIDs, new Action<Guid>((item) =>
            {
                lock (landIDs)
                {
                    List<BuildLandBoundaryAddressDot> finddots = allrepeatDots.FindAll(d => d.LandID == item);
                    repeatDots.AddRange(finddots);
                }
            }));

            if (repeatDots == null || repeatDots.Count == 0)
            {
                jzds = InitalizeDotValue(landIDs, repeatDots);
                return jzds;
            }
            List<YuLinTu.Library.Entity.BuildLandBoundaryAddressDot> features = new List<BuildLandBoundaryAddressDot>();
            //if (CanChecker)
            //{
            this.ReportInfomation(string.Format("正在去除{0}下界址重复点，共{1}条数据", currentZone.Name, repeatDots.Count()));
            Parallel.ForEach(repeatDots, new Action<BuildLandBoundaryAddressDot>((item) =>
            {
                if (item.Shape != null)
                {
                    var itempoint = item.Shape.ToCoordinates()[0];
                    lock (features)
                    {
                        if (!features.Any(f => IsRepitePoint(f.Shape.ToCoordinates()[0], itempoint)))
                        {
                            features.Add(item);
                        }
                    }
                }
            }));
            repeatDots.Clear();
            repeatDots = null;
            this.ReportInfomation(string.Format("去除{0}下界址重复点，完成", currentZone.Name));

            //}
            //else
            //{
            //    features = repeatDots;
            //}
            foreach (YuLinTu.Library.Entity.BuildLandBoundaryAddressDot feature in features)
            {
                JZD jzd = new JZD();
                jzd.YSDM = FeatureCode.JZD;
                jzd.JZDH = feature.DotNumber;
                jzd.JZDLX = feature.DotType;
                jzd.JBLX = feature.LandMarkType;
                jzd.Shape = feature.Shape == null ? null : feature.Shape.Instance;
                jzds.Add(jzd);
            }
            allrepeatDots = null;
            repeatDots = null;
            features = null;
            GC.Collect();
            return jzds;

        }

        /// <summary>
        /// 初始化界址值
        /// </summary>
        /// <returns></returns>
        private List<JZD> InitalizeDotValue(List<Guid> landIDs, List<YuLinTu.Library.Entity.BuildLandBoundaryAddressDot> dots)
        {
            //BuildLandBoundaryAddressDotCollection dots = database.BuildLandBoundaryAddressDot.GetByZoneCode(currentZone.FullCode, eSearchType.Fuzzy, eLandPropertyRightType.AgricultureLand);
            if (dots == null)
            {
                return new List<JZD>();
            }
            foreach (var landID in landIDs)
            {
                dots.RemoveAll(dt => dt.LandID == landID);
            }
            //var query = (from p in dots
            //             group p by new Point(p.Shape.ToCoordinates()[0].X, p.Shape.ToCoordinates()[0].Y) into g
            //             select g.First()).ToList();
            //dots = null;
            GC.Collect();
            bool useDotNumber = dots.Exists(dt => string.IsNullOrEmpty(dt.UniteDotNumber));
            int dotNumber = 1;
            List<JZD> jzds = new List<JZD>();
            foreach (var dot in dots)
            {
                JZD jzd = new JZD();
                jzd.YSDM = FeatureCode.JZD;
                jzd.JZDH = useDotNumber ? ("J" + dotNumber.ToString()) : dot.UniteDotNumber;
                jzd.JZDLX = dot.DotType.ToString();
                jzd.JBLX = dot.LandMarkType.ToString();
                jzd.Shape = dot.Shape == null ? null : dot.Shape.Instance;
                //IPoint point = new PointClass();
                //point.PutCoords(dot.XCoordinate.Value, dot.YCoordinate.Value);
                //if (reference != null)
                //{
                //    point.Project(reference);
                //}
                //point.SnapToSpatialReference();
                //if (UseArcShape)
                //{
                //    jzd.Shape = point;
                //}
                //else
                //{
                //    jzd.Shape = point.ToNormalGeometry();
                //}
                jzds.Add(jzd);
                dotNumber++;
            }
            return jzds;
        }

        /// <summary>
        /// 初始化界址线数据
        /// </summary>
        /// <returns></returns>
        private List<JZX> InitalizeLineData(List<Guid> landIDs, string zoneCode)
        {
            List<JZX> jzxs = new List<JZX>();
            //if (workspace == null)
            //{
            //    return jzxs;
            //}
            //IWorkspace2 workSpace = workspace as IWorkspace2;
            //bool canContinue = false;//workSpace != null ? workSpace.get_NameExists(esriDatasetType.esriDTFeatureClass, "JZX") : false;
            //if (!canContinue)
            //{
            //    jzxs = InitalizeLineValue(lands);
            //    return jzxs;
            //}
            //IFeatureClass featureClass = workspace.OpenFeatureClass("JZX");
            //if (featureClass == null)
            //{
            //    return jzxs;
            //}
            //IQueryFilter filter = YuLinTu.ArcGIS.Common.ConstructQueryFilter.GetQueryFilter(string.Format("XZQYDM LIKE '{0}%'", currentZone.FullCode), "XZQYDM");
            //List<IFeature> features = FeatureUtility.GetFeatureCollection(featureClass, filter);
            //List<YuLinTu.Library.Entity.BuildLandBoundaryAddressCoil> features = jzxStation.GetByZoneCode(zoneCode, eSearchOption.Precision, eLandPropertyRightType.AgricultureLand);
            //List<YuLinTu.Library.Entity.Zone> zones = zoneStation.GetChildren(currentZone.FullCode, eLevelOption.SelfAndSubs);
            //List<YuLinTu.Library.Entity.BuildLandBoundaryAddressCoil> allreapeatlinefeatures = new List<BuildLandBoundaryAddressCoil>();
            //zones.ForEach(z => allreapeatlinefeatures.AddRange(jzxStation.GetByZoneCode(z.FullCode, eSearchOption.Precision, eLandPropertyRightType.AgricultureLand)));

            string landtype = ((int)eLandPropertyRightType.AgricultureLand).ToString();
            var jzxQuery = jzxStation.Get(t => t.ZoneCode == zoneCode && t.LandType == landtype);
            var allreapeatlinefeatures = jzxQuery.ToList();

            List<YuLinTu.Library.Entity.BuildLandBoundaryAddressCoil> reapeatlinefeatures = new List<BuildLandBoundaryAddressCoil>();
            Parallel.ForEach(landIDs, new Action<Guid>((item) =>
            {
                lock (landIDs)
                {
                    List<BuildLandBoundaryAddressCoil> findcoils = allreapeatlinefeatures.FindAll(d => d.LandID == item);
                    reapeatlinefeatures.AddRange(findcoils);
                }
            }));

            //List<YuLinTu.Library.Entity.BuildLandBoundaryAddressDot> dotfeatures = new List<BuildLandBoundaryAddressDot>();
            //zones.ForEach(z => dotfeatures.AddRange(jzdStation.GetByZoneCode(z.FullCode, eSearchOption.Precision, ((int)eLandPropertyRightType.AgricultureLand).ToString())));

            var jzdQuery = jzdStation.Get(t => t.ZoneCode == zoneCode && t.LandType == landtype);
            var dotfeatures = jzdQuery.ToList();

            if (reapeatlinefeatures == null || reapeatlinefeatures.Count == 0)
            {
                jzxs = InitalizeLineValue(landIDs, dotfeatures, reapeatlinefeatures);
                return jzxs;
            }

            List<YuLinTu.Library.Entity.BuildLandBoundaryAddressCoil> linefeatures = new List<BuildLandBoundaryAddressCoil>();

            //if (CanChecker)
            //{
            this.ReportInfomation(string.Format("正在去除{0}下界址重复线，共{1}条数据", currentZone.Name, reapeatlinefeatures.Count()));

            Parallel.ForEach(reapeatlinefeatures, new Action<BuildLandBoundaryAddressCoil>((item) =>
            {
                var itemdotStartPoint = item.Shape.ToCoordinates()[0];
                var itemdotEndPoint = item.Shape.ToCoordinates()[1];
                lock (linefeatures)
                {

                    if (!linefeatures.Any(l =>
                    {

                        var ldotStartPoint = l.Shape.ToCoordinates()[0];
                        var ldotEndPoint = l.Shape.ToCoordinates()[1];

                        if ((((IsRepitePoint(ldotStartPoint, itemdotStartPoint) && IsRepitePoint(ldotEndPoint, itemdotEndPoint))
                            || (IsRepitePoint(ldotStartPoint, itemdotEndPoint) && IsRepitePoint(ldotEndPoint, itemdotStartPoint)))))
                        {
                            return true;//是重复线
                        }
                        return false;
                    }))
                    {
                        linefeatures.Add(item);
                    }
                }
            }));
            reapeatlinefeatures.Clear();
            reapeatlinefeatures = null;
            this.ReportInfomation(string.Format("去除{0}下界址重复线，完成", currentZone.Name));
            //}
            //else
            //{
            //    linefeatures = reapeatlinefeatures;
            //}


            //int index = -1;
            //System.Object val = null;
            //List<IPolyline> lines = new List<IPolyline>();
            foreach (YuLinTu.Library.Entity.BuildLandBoundaryAddressCoil feature in linefeatures)
            {
                JZX jzx = new JZX();
                jzx.YSDM = FeatureCode.JZX;
                jzx.JXXZ = feature.LineType;
                jzx.JZXLB = feature.CoilType;
                jzx.JZXWZ = feature.Position == "0" ? "1" : feature.Position;
                jzx.JZXSM = feature.Description;
                jzx.PLDWQLR = feature.NeighborPerson;
                jzx.PLDWZJR = feature.NeighborFefer;
                jzx.Shape = feature.Shape == null ? null : feature.Shape.Instance;

                if (jzx.Shape == null)
                {
                    continue;
                }
                jzxs.Add(jzx);
            }
            allreapeatlinefeatures = null;
            reapeatlinefeatures = null;
            linefeatures = null;
            //lines = null;
            GC.Collect();
            return jzxs;
        }

        /// <summary>
        /// 初始化界址线值
        /// </summary>
        /// <returns></returns>
        private List<JZX> InitalizeLineValue(List<Guid> landIDs, List<YuLinTu.Library.Entity.BuildLandBoundaryAddressDot> dots, List<YuLinTu.Library.Entity.BuildLandBoundaryAddressCoil> lines)
        {
            //BuildLandBoundaryAddressDotCollection dots = database.BuildLandBoundaryAddressDot.GetByZoneCode(currentZone.FullCode, eSearchType.Fuzzy, eLandPropertyRightType.AgricultureLand);
            //BuildLandBoundaryAddressCoilCollection lines = database.BuildLandBoundaryAddressCoil.GetByZoneCode(currentZone.FullCode, eSearchType.Fuzzy, eLandPropertyRightType.AgricultureLand);
            if (dots == null || lines == null)
            {
                return new List<JZX>();
            }
            foreach (var line in lines)
            {
                BuildLandBoundaryAddressDot sd = dots.Find(dt => dt.ID == line.StartPointID);
                BuildLandBoundaryAddressDot ed = dots.Find(dt => dt.ID == line.EndPointID);
                if (sd == null || ed == null)
                {
                    continue;
                }
                line.StartNumber = sd.DotNumber;
                line.StartPointID = sd.ID;
                line.EndNumber = ed.DotNumber;
                line.EndPointID = ed.ID;
                sd = null;
                ed = null;
            }
            foreach (var landID in landIDs)
            {
                lines.RemoveAll(dt => dt.LandID == landID);
            }
            var query = (from p in lines
                         group p by new Line(new Point(dots.Find(d => d.ID == p.StartPointID).Shape.ToCoordinates()[0].X, dots.Find(d => d.ID == p.StartPointID).Shape.ToCoordinates()[0].Y), new Point(dots.Find(d => d.ID == p.EndPointID).Shape.ToCoordinates()[1].X, dots.Find(d => d.ID == p.EndPointID).Shape.ToCoordinates()[1].Y)) into g
                         select g.First()).ToList();
            dots = null;
            lines = null;
            GC.Collect();
            List<JZX> jzxs = new List<JZX>();
            foreach (var line in query)
            {
                JZX jzx = new JZX();
                jzx.YSDM = FeatureCode.JZX;
                jzx.JXXZ = line.LineType.ToString();
                jzx.JZXLB = line.CoilType.ToString();
                jzx.JZXWZ = line.Position.ToString() == "0" ? "1" : line.Position.ToString();
                jzx.JZXSM = line.Description;
                jzx.PLDWQLR = line.NeighborPerson;
                jzx.PLDWZJR = line.NeighborFefer;
                jzx.Shape = line.Shape == null ? null : line.Shape.Instance;
                //IPoint startPoint = new PointClass();
                //startPoint.PutCoords(line.StartX, line.StartY);
                //IPoint endPoint = new PointClass();
                //endPoint.PutCoords(line.EndX, line.EndY);
                //IPointCollection pointCollection = new PolylineClass();
                //pointCollection.AddPoint(startPoint);
                //pointCollection.AddPoint(endPoint);
                //IPolyline polyline = pointCollection as IPolyline;
                //polyline.Project(reference);
                //polyline.SnapToSpatialReference();
                //ITopologicalOperator topoOperation = polyline as ITopologicalOperator;
                //topoOperation.Simplify();
                //if (UseArcShape)
                //{
                //    jzx.Shape = polyline;
                //}
                //else
                //{
                //    jzx.Shape = polyline.ToNormalGeometry();
                //}
                jzxs.Add(jzx);
            }
            return jzxs;
        }

        /// <summary>
        /// 点
        /// </summary>
        private class Point
        {
            public double X { get; set; }
            public double Y { get; set; }
            public double Dist
            {
                get
                {
                    return Math.Sqrt(X * X + Y * Y);
                }
            }
            public Point(double x, double y)
            {
                this.X = Math.Round(x, 4);
                this.Y = Math.Round(y, 4);
            }

            public override bool Equals(object obj)
            {
                Point y = obj as Point;
                if (null == y)
                    return false;

                if (X == y.X && Y == y.Y)
                    return true;

                return Dist == y.Dist;
            }

            public override int GetHashCode()
            {
                return Dist.GetHashCode();
            }
        }

        /// <summary>
        /// 线
        /// </summary>
        private class Line
        {
            public Point Start { get; set; }
            public Point End { get; set; }
            public Line()
            {

            }
            public Line(Point start, Point end)
            {
                this.Start = start;
                this.End = end;
            }

            public override bool Equals(object obj)
            {
                var y = obj as Line;
                if (null == y)
                    return false;

                if (Start.Equals(y.Start) && End.Equals(y.End))
                    return true;

                return Start.Equals(y.End) && End.Equals(y.Start);
            }

            public override int GetHashCode()
            {
                return Start.GetHashCode() ^ End.GetHashCode();
            }
        }

        #endregion

        #region 数据导出

        /// <summary>
        /// 初始化农业部目录
        /// </summary>
        /// <returns></returns>
        private bool InitalizeAgricultureDirectory(DataExportProgress exportProgress, ArcSpaceDataProgress spaceProgress, SqliteManager sqliteManager)
        {
            exportProgress.IsExportScan = ContainMatrical;
            //county = database.Zone.Get(currentZone.FullCode.Substring(0, 6));
            county = zoneStation.Get(currentZone.FullCode.Substring(0, 6));
            if (county == null)
            {
                this.ReportAlert(eMessageGrade.Error, null, "行政区域级别不能大于区县级!");
                return false;
            }
            //reference = InitalizeSystemReference();
            ExportFileEntity efe = new ExportFileEntity();
            efe.VictorJZD.IsExport = ContainDot;
            efe.VictorJZX.IsExport = ContainLine;
            efe.VictorZJ.IsExport = false;
            bool canContinue = exportProgress.CreatFolderFile(Folder, county.FullCode, DateTime.Now.Year.ToString(), county.Name, efe);
            if (!canContinue)
            {
                this.ReportAlert(eMessageGrade.Error, null, "创建确权登记成果数据库失败!");
                return false;
            }
            try
            {
                spaceProgress.ExtentName = county.FullCode + DateTime.Now.Year.ToString();
                spaceProgress.ShapeFilePath = exportProgress.ShapeFilePath;
                sqliteManager.CopyNewDatabase();
                InitalizeSpatialCoordianteSystem(spaceProgress);
            }
            catch (SystemException ex)
            {
                this.ReportAlert(eMessageGrade.Error, null, ex.Message);
                return false;
            }
            return true;
        }

        /// <summary>
        /// 初始化空间坐标系
        /// </summary>
        /// <param name="spaceProgress"></param>
        private void InitalizeSpatialCoordianteSystem(ArcSpaceDataProgress spaceProgress)
        {
            if (spaceProgress == null)
            {
                return;
            }
            //string name = "CGCS2000_3_Degree_GK_CM_105E" + ".prj";
            //name = name.Replace("_", " ");
            //string coordSystem = System.Windows.Forms.Application.StartupPath + @"\Data\SpatialReferences\Projected Coordinate Systems\Gauss Kruger\CGCS2000";
            //string fileName = coordSystem + @"\" + name;
            //if (Directory.Exists(coordSystem) && Directory.GetFiles(coordSystem).Contains(fileName))
            //{
            //    TextReader reader = new StreamReader(fileName);
            //    spaceProgress.SpatialText = reader.ReadToEnd();
            //    return;
            //}
            //coordSystem = System.Windows.Forms.Application.StartupPath + @"\Data\SpatialReferences\Projected Coordinate Systems\Gauss Kruger\Xian 1980";
            //fileName = coordSystem + @"\" + name;
            //if (Directory.Exists(coordSystem) && Directory.GetFiles(coordSystem).Contains(fileName))
            //{
            //    TextReader reader = new StreamReader(fileName);
            //    spaceProgress.SpatialText = reader.ReadToEnd();
            //    return;
            //}
            var targetSpatialReference = DbContext.CreateSchema().GetElementSpatialReference(
                ObjectContext.Create(typeof(YuLinTu.Library.Entity.Zone)).Schema,
                ObjectContext.Create(typeof(YuLinTu.Library.Entity.Zone)).TableName);

            var info = targetSpatialReference.ToEsriString();

            spaceProgress.SpatialText = info;
            //StringBuilder stringBuilder = new StringBuilder();
            //string coordinateString = ArcGisGeometry.InitalizeCoordinateSytem(reference);
            //stringBuilder.Append(coordinateString);
            //if (reference.FactoryCode != 0 && !coordinateString.Contains("AUTHORITY"))
            //{
            //    stringBuilder.Append(",AUTHORITY[\"EPSG\",");
            //    stringBuilder.Append(reference.FactoryCode.ToString());
            //    stringBuilder.Append("]]");
            //}
            //spaceProgress.SpatialText = stringBuilder.ToString();
        }

        /// <summary>
        /// 数据导出
        /// </summary>
        /// <param name="?"></param>
        private void LandDataExportProgress(ArcSpaceDataProgress spaceProgress, SqliteManager sqliteManager, string cZoneCode)
        {
            spaceProgress.ExportSpaceDataBase(sqliteManager, cZoneCode, true);
            VictorData data = new VictorData();
            data.InitiallClass();
            data.Data_Info.Title = county.Name + "矢量元数据";
            data.Data_Info.GeoID = county.FullCode;
            data.Data_Info.Ending = DateTime.Now.ToString();
            data.Data_Info.RpOrgName = UnitName;
            data.Data_Info.RpCnt = LinkMan;
            data.Data_Info.VoiceNum = Telephone;
            data.Data_Info.CntAddress = Address;
            data.Data_Info.CntCode = PosterNumber;
            data.Cont_Info.CatFetTyps = "数据集要素类型";
            data.Cont_Info.AttrTypList = "与数据集要素类名称对应的主要属性列表";
            spaceProgress.ExportMeataData(spaceProgress.ShapeFilePath, "SL" + county.FullCode + DateTime.Now.Year.ToString(), data);
        }

        /// <summary>
        /// 地域数据处理
        /// </summary>
        /// <param name="spaceProgress"></param>
        private void AllDataExportProgress(ArcSpaceDataProgress spaceProgress, List<YuLinTu.Library.AgriExchange.Zone> zones)
        {
            spaceProgress.XAJXZQS = InitalizeCountyData(zones.FindAll(ze => ze.Level == Library.AgriExchange.eZoneLevel.County));
            spaceProgress.XGJXZQS = InitalizeTownData(zones.FindAll(ze => ze.Level == Library.AgriExchange.eZoneLevel.Town));
            spaceProgress.CJXZQS = InitalizeVillageData(zones.FindAll(ze => ze.Level == Library.AgriExchange.eZoneLevel.Village));
            spaceProgress.ZJXZQS = InitalizeGroupData(zones.FindAll(ze => ze.Level == Library.AgriExchange.eZoneLevel.Group));
            spaceProgress.ExportSpaceZone();//导出空间地域数据
            ComplexSpaceEntity spaceEntity = new ComplexSpaceEntity();
            spaceEntity.DZDW = InitalizePointData();
            spaceEntity.XZDW = InitalizePolylineData();
            spaceEntity.MZDW = InitalizePolygonData();
            spaceEntity.KZD = InitalizeControlPoint();
            spaceEntity.QYJX = InitalizeRegionData();
            spaceEntity.JBNTBHQ = InitalizeFarmerRegionData();
            spaceProgress.ExportSpaceData(spaceEntity);//导出基础数据
        }

        /// <summary>
        /// 权属单位代码表处理
        /// </summary>
        private List<YuLinTu.Library.AgriExchange.Zone> TableDataExportProgress(List<YuLinTu.Library.Entity.Zone> zones)
        {
            List<YuLinTu.Library.AgriExchange.Zone> zoneData = ZoneMapping(zones);
            ExportUnitTable table = new ExportUnitTable();
            table.ZoneList = zoneData;
            table.PreviewName = county.FullCode + DateTime.Now.Year.ToString();
            table.FilePath = Folder + @"\" + county.FullCode + county.Name + @"\权属数据";
            table.ExportData();
            return zoneData;
        }

        /// <summary>
        /// 初始化地域数据
        /// </summary>
        /// <returns></returns>
        private List<YuLinTu.Library.Entity.Zone> InitalizeZoneData()
        {
            List<YuLinTu.Library.Entity.Zone> zones = new List<YuLinTu.Library.Entity.Zone>();
            YuLinTu.Library.Entity.Zone zone = currentZone;
            while (zone.FullCode.Length > YuLinTu.Library.Entity.Zone.ZONE_COUNTY_LENGTH)
            {
                YuLinTu.Library.Entity.Zone curZone = zoneStation.Get(zone.UpLevelCode);
                zones.Add(curZone);
                zone = curZone.Clone() as YuLinTu.Library.Entity.Zone;
            }
            zones.Reverse();
            return zones;
        }

        /// <summary>
        /// 地域数据映射
        /// </summary>
        /// <param name="zones"></param>
        /// <returns></returns>
        private List<YuLinTu.Library.AgriExchange.Zone> ZoneMapping(List<YuLinTu.Library.Entity.Zone> zones)
        {
            List<YuLinTu.Library.AgriExchange.Zone> zoneCollection = new List<Library.AgriExchange.Zone>();
            // ArcGisZoneCollection arcZones = gisInstance.ArcGisZone.GetByZoneCode(county.FullCode, Library.Data.ConditionOption.Like_LeftFixed);
            List<YuLinTu.Library.Entity.Zone> arcZones = zoneStation.GetChildren(currentZone.FullCode, eLevelOption.SelfAndSubs);

            foreach (var zone in zones)
            {
                YuLinTu.Library.AgriExchange.Zone exZone = new Library.AgriExchange.Zone();
                exZone.ID = zone.ID;
                exZone.Code = zone.Code;
                exZone.Comment = zone.Comment;
                exZone.CreateTime = zone.CreateTime;
                exZone.CreateUser = zone.CreateUser;
                exZone.FullCode = zone.FullCode;
                exZone.FullName = zone.FullName;
                exZone.Level = (YuLinTu.Library.AgriExchange.eZoneLevel)((int)(zone.Level));
                exZone.Name = zone.Name;
                exZone.UpLevelCode = zone.UpLevelCode;
                exZone.UpLevelName = zone.UpLevelName;
                //ArcGisZone arcZone = arcZones.Find(ze => ze.FullCode == zone.FullCode);
                //if (UseArcShape)
                //{
                //    exZone.Shape = arcZone != null ? arcZone.Shape : null;
                //}
                //else
                //{
                //    exZone.Shape = arcZone != null ? arcZone.Shape.ToNormalGeometry() : null;
                //}
                exZone.Shape = zone.Shape == null ? null : zone.Shape.Instance;
                // arcZone = null;
                zoneCollection.Add(exZone);
            }
            arcZones = null;
            GC.Collect();
            return zoneCollection;
        }

        #endregion

        #region Helper

        /// <summary>
        /// 初始化错误记录文件目录
        /// </summary>
        private void InitalizeDirectory()
        {
            if (!Directory.Exists(System.Windows.Forms.Application.StartupPath + @"\Error"))
            {
                Directory.CreateDirectory(System.Windows.Forms.Application.StartupPath + @"\Error");
            }
            string fileName = System.Windows.Forms.Application.StartupPath + @"\Error\DataChecker.txt";
            using (System.IO.StreamWriter writer = new System.IO.StreamWriter(fileName))
            {
                writer.WriteLine(System.DateTime.Now.ToString());
            }
        }

        /// <summary>
        /// 撰写数据记录信息
        /// </summary>
        private void WriteDataInformation(string message)
        {
            string fileName = System.Windows.Forms.Application.StartupPath + @"\Error\DataChecker.txt";
            if (!System.IO.File.Exists(fileName))
            {
                return;
            }
            using (System.IO.StreamWriter writer = new System.IO.StreamWriter(fileName, true))
            {
                writer.WriteLine(message);
            }
        }

        #endregion

        #region 矢量数据

        /// <summary>
        /// 矢量数据处理
        /// </summary>
        private void ArcShapeDataProgress(ComplexSpaceEntity spaceEntity)
        {
        }

        /// <summary>
        /// 地块数据处理
        /// </summary>
        /// <param name="sqliteManager"></param>
        private void ArcLandShapeProgress(SqliteManager sqliteManager)
        {

        }

        private void ArcRegionShapeProgress(ArcSpaceDataProgress spaceProgress)
        {
        }

        #endregion
    }
}
