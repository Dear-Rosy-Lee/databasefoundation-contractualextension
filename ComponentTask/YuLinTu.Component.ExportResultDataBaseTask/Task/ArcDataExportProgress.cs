/*
 * (C) 2021 - 2025 鱼鳞图公司版权所有,保留所有权利
*/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Quality.Business.Entity;
using Quality.Business.TaskBasic;
using YuLinTu.Data;
using YuLinTu.Library.Aux;
using YuLinTu.Library.Business;
using YuLinTu.Library.Entity;
using YuLinTu.Library.Repository;
using YuLinTu.Library.Result;
using YuLinTu.Library.WorkStation;
using YuLinTu.Spatial;
using YuLinTu.Windows;

namespace YuLinTu.Component.ExportResultDataBaseTask
{
    /// <summary>
    /// 导出行政区划成果数据库
    /// </summary>
    public class ArcDataExportProgress : Task
    {
        #region Fields

        protected int currentIndex = 1;//当前索引号
        protected string zoneName;//地域名称
        protected List<string> relationList;//家庭关系列表
        protected YuLinTu.Library.Entity.Zone county;//区县地域
        protected bool showInformation;//是否显示信息
        protected List<Guid> filterLandIDs;//检查后筛选出来最终与界址点线挂钩的地块集合ID
        protected string serNumberTemp;
        //private int currentZoneJZDs;//当前地域下界址点线数量
        // private int currentZoneJZXs;
        //private int dkbsm = QuantityValue.Land;
        //private int jzdbsm = QuantityValue.Point;
        //private int jzxbsm = QuantityValue.Line;
        protected int currentZoneLandCount = 0;//当前地域下地块个数

        protected IZoneWorkStation zoneStation;
        protected IContractLandWorkStation contractLandWorkStation;//承包台账地块业务逻辑层
        protected IVirtualPersonWorkStation<LandVirtualPerson> VirtualPersonStation;//承包台账(承包方)Station
        protected IConcordWorkStation concordStation;
        protected IContractRegeditBookWorkStation contractRegeditBookStation;
        protected IBuildLandBoundaryAddressCoilWorkStation jzxStation;
        protected IBuildLandBoundaryAddressDotWorkStation jzdStation;
        protected ISenderWorkStation senderStation;
        protected IDZDWWorkStation dzdwStation;
        protected IDCZDWorkStation dczdStation;
        protected IXZDWWorkStation xzdwStation;
        protected IMZDWWorkStation mzdwStation;
        protected IControlPointWorkStation kzdStation;
        protected IFarmLandConserveWorkStation jbntbhqStation;
        protected IZoneBoundaryWorkStation qyjxStation;

        protected IStockConcordWorkStation stockconcordStation;
        protected IStockWarrantWorkStation stockwarrantStation;
        protected DataExportProgress dataProgress;

        #endregion Fields

        #region Propertys

        /// <summary>
        /// 目标节点地域
        /// </summary>
        public YuLinTu.Library.Entity.Zone currentZone { get; set; }

        /// <summary>
        /// 数据源上下文
        /// </summary>
        public IDbContext DbContext
        {
            get { return dbContext; }
            set
            {
                dbContext = value;
                InitallStation();
            }
        }
        private IDbContext dbContext;


        /// <summary>
        /// 目标文件夹
        /// </summary>
        public string Folder { get; set; }

        /// <summary>
        /// 导出权属来源附件
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
        /// 包含界址点、界址线
        /// </summary>
        public bool ContainDotLine { get; set; }

        /// <summary>
        /// 导出地域编码，地域名称
        /// </summary>
        public bool ExportLandCode { get; set; }

        /// <summary>
        /// 重复点容差
        /// </summary>
        public double RepeatValue { get; set; }

        /// <summary>
        /// 是否检查
        /// </summary>
        public bool CanChecker { get; set; }

        /// <summary>
        /// 是否检查证件号码
        /// </summary>
        public bool CheckCardNumber { get; set; }

        /// <summary>
        /// 是否导出没签合同空户
        /// </summary>
        public bool IsReportNoConcordNoLandsFamily { get; set; }

        /// <summary>
        /// 是否导出没签合同地块
        /// </summary>
        public bool IsReportNoConcordLands { get; set; }

        /// <summary>
        /// 登记簿中地块示意图为PDF路径
        /// </summary>
        public bool IsSaveParcelPathAsPDF { get; set; }

        ///<summary>
        ///导出CBDKXX确权(合同)面积设置
        /// <summary>
        public CbdkxxAwareAreaExportEnum CBDKXXAwareAreaExportSet { get; set; }

        /// <summary>
        /// 数据字典
        /// </summary>
        public List<Dictionary> DictList
        {
            get;
            set;
        }

        /// <summary>
        /// 任务-导出确权成果库地块与界址点、线匹配设置
        /// </summary>
        public TaskExportLandDotCoilSettingDefine TaskExportLandDotCoilDefine { get; set; }

        /// <summary>
        /// 是否去除重名标识
        /// </summary>
        public bool KeepRepeatFlag { get; set; }

        /// <summary>
        /// 只导出关键界址点
        /// </summary>
        public bool OnlyKey { get; set; }

        /// <summary>
        /// 只导出地块、界址线、界址点矢量数据
        /// </summary>
        public bool OnlyExportLandResult { get; set; }

        /// <summary>
        /// 使用统编号导出-和台账的初始化处理一致，重复点统编号一致
        /// </summary>
        public bool UseUniteNumberExport { get; set; }

        protected bool qghttable;
        protected bool qgqztable;

        #endregion Propertys

        #region Ctor

        public ArcDataExportProgress()
        {
        }

        public ArcDataExportProgress(IDbContext db)
        {
            this.Name = "导出确权登记数据库成果";
            DbContext = db;
        }

        private void InitallStation()
        {
            zoneStation = DbContext.CreateZoneWorkStation();
            VirtualPersonStation = DbContext.CreateVirtualPersonStation<LandVirtualPerson>();
            contractLandWorkStation = DbContext.CreateContractLandWorkstation();
            concordStation = DbContext.CreateConcordStation();
            contractRegeditBookStation = DbContext.CreateRegeditBookStation();
            jzxStation = DbContext.CreateBoundaryAddressCoilWorkStation();
            jzdStation = DbContext.CreateBoundaryAddressDotWorkStation();
            senderStation = DbContext.CreateSenderWorkStation();
            stockconcordStation = DbContext.CreateStockConcordWorkStation();
            stockwarrantStation = DbContext.CreateStockWarrantWorkStation();

            dzdwStation = new ContainerFactory(DbContext).CreateWorkstation<IDZDWWorkStation, IDZDWRepository>();
            dczdStation = new ContainerFactory(DbContext).CreateWorkstation<IDCZDWorkStation, IDCZDRepository>();
            xzdwStation = new ContainerFactory(DbContext).CreateWorkstation<IXZDWWorkStation, IXZDWRepository>();
            mzdwStation = new ContainerFactory(DbContext).CreateWorkstation<IMZDWWorkStation, IMZDWRepository>();
            kzdStation = new ContainerFactory(DbContext).CreateWorkstation<IControlPointWorkStation, IControlPointRepository>();
            jbntbhqStation = new ContainerFactory(DbContext).CreateWorkstation<IFarmLandConserveWorkStation, IFarmLandConserveRepository>();
            qyjxStation = new ContainerFactory(DbContext).CreateWorkstation<IZoneBoundaryWorkStation, IZoneBoundaryRepository>();
            RepeatValue = 0.05;

            qghttable = DbContext.DataSource.CreateSchema().AnyElement(null, "QGCBJYQ_HT");
            qgqztable = DbContext.DataSource.CreateSchema().AnyElement(null, "QGCBJYQ_QZ");
            dataProgress = new DataExportProgress();
        }

        #endregion Ctor

        #region Override

        /// <summary>
        /// 导出数据成果
        /// </summary>
        public virtual void Export()
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
                string dbName = System.Windows.Forms.Application.StartupPath + @"\Template\Data.sqlite";
                if (!System.IO.File.Exists(dbName))
                {
                    this.ReportError("系统中缺少数据库文件!");
                    return;
                }
                else
                {
                    System.IO.File.Copy(dbName, System.Windows.Forms.Application.StartupPath + @"\Data.sqlite", true);
                }
                ArcDataProgress();
                ExporOther();
            }
            catch (System.Exception ex)
            {
                if (ex.GetType().Name == "TaskStopException")
                    return;
                LogWrite.WriteErrorLog(ex.ToString());

                this.ReportError("导出成果库出错:" + ex.Message);
                this.ReportProgress(100, "完成");
                return;
            }
            this.ReportProgress(100, "完成");
        }

        public virtual void ExporOther()
        {
        }

        /// <summary>
        /// 导出界址点、界址线和地块
        /// 颜学铭 2016年9月27日添加
        /// </summary>
        /// <param name="dataSouce">数据源</param>
        /// <param name="shapeFileOutputPath">输出路径</param>
        /// <param name="currentZoneCode">当前地域代码</param>
        /// <param name="zoneYearCode">地域年份代码</param>
        /// <param name="prjString">坐标文件</param>
        /// <param name="excludeDkbm">编码集合</param>
        [Obsolete]
        public virtual void testExportJzd(IDbContext dataSouce, string shapeFileOutputPath,
         string currentZoneCode, string zoneYearCode, string prjString,
         HashSet<string> excludeDkbm, int numLand, int numPoint, int numLine)
        {
            using (var db = new NetAux.DBSpatialite())
            {
                var dbFile = dataSouce.DataSource.ConnectionString;
                dbFile = dbFile.Substring(dbFile.IndexOf('=') + 1);

                db.Open(dbFile);
                var prms = new ExportJzdxParam(0.05);
                var settings = System.Configuration.ConfigurationManager.AppSettings;

                int dknumber = 0;
                int jzdnumber = 0;
                int jzxnumber = 0;

                if (settings.AllKeys.Contains("SplitNumber"))
                {
                    var spliteNumber = settings["SplitNumber"].ToString();
                    var ss = spliteNumber.Split('|');

                    if (ss.Length == 3)
                    {
                        int.TryParse(ss[0], out dknumber);
                        int.TryParse(ss[1], out jzdnumber);
                        int.TryParse(ss[2], out jzxnumber);
                    }
                }
                //var numLand = manager.GetDataCount<SqliteDK>();
                //var numPoint = manager.GetDataCount<SqliteJZD>();
                //var numLine = manager.GetDataCount<SqliteJZX>();
                prms.sDkSYQXZ = "";
                prms.exportOnlyKey = OnlyKey;
                prms.UseUniteNumberExport = UseUniteNumberExport;
                prms.JzdMaxRecords = numPoint > jzdnumber ? jzdnumber : 0;//每个界址点文件最多 条
                prms.JzxMaxRecords = numLine > jzxnumber ? jzxnumber : 0;//每个界址线文件最多 条
                prms.DkMaxRecords = numLand > dknumber ? dknumber : 0;////每个界址线文件最多 条

                #region 删除文件

                var dkPath = string.Format(@"{0}\{1}{2}.shp", shapeFileOutputPath, "DK", zoneYearCode);
                var pointPath = string.Format(@"{0}\{1}{2}.shp", shapeFileOutputPath, "JZX", zoneYearCode);
                var linePath = string.Format(@"{0}\{1}{2}.shp", shapeFileOutputPath, "JZD", zoneYearCode);
                DeleteFile(prms.DkMaxRecords > 0, dkPath);
                DeleteFile(prms.JzdMaxRecords > 0, pointPath);
                DeleteFile(prms.JzxMaxRecords > 0, linePath);

                #endregion 删除文件

                #region 输出.prj文件需要

                var srid = db.QuerySRID(Zd_cbdFields.TABLE_NAME);
                if (srid >= 0)
                {
                    prms.sESRIPrjStr = prjString; //SpatialReferenceUtil.FindEsriSpatialReferenceString(spatialReferencePath, srid);
                }

                #endregion 输出.prj文件需要

                var exp = new ExportJzdx(db, prms, shapeFileOutputPath + @"\", currentZoneCode, zoneYearCode);
                exp.ReportProgress += (msg, i) =>
                {//进度
                    this.ReportProgress(i, msg);
                };
                exp.ReportInfomation += msg =>
                {//提示信息
                    this.ReportInfomation(msg);
                };
                exp.ReportWarn += msg =>
                {//提示警告信息
                    this.ReportWarn(msg);
                };
                exp.OnPresaveDk += en =>
                {//这里可以根据需要修改地块实体的内容
                    if (en.SYQXZ != "10" && en.SYQXZ != "30" && en.SYQXZ != "31" &&
                     en.SYQXZ != "32" && en.SYQXZ != "33" && en.SYQXZ != "34")
                        en.SYQXZ = "30";
                };

                exp.DoExport(excludeDkbm);
            }
        }

        /// <summary>
        /// 导出地块
        /// </summary>
        public virtual void ExportLand(IDbContext dataSouce, string shapeFileOutputPath,
         string currentZoneCode, string zoneYearCode, string prjString,
         HashSet<string> excludeDkbm, int numLand)
        {
            using (var db = new NetAux.DBSpatialite())
            {
                var dbFile = dataSouce.DataSource.ConnectionString;
                dbFile = dbFile.Substring(dbFile.IndexOf('=') + 1);

                db.Open(dbFile);
                var prms = new ExportJzdxParam(0.05);
                var settings = System.Configuration.ConfigurationManager.AppSettings;

                int dknumber = 0;

                if (settings.AllKeys.Contains("SplitNumber"))
                {
                    var spliteNumber = settings["SplitNumber"].ToString();
                    var ss = spliteNumber.Split('|');

                    int.TryParse(ss[0], out dknumber);
                }
                prms.sDkSYQXZ = "";
                prms.exportOnlyKey = OnlyKey;
                prms.UseUniteNumberExport = UseUniteNumberExport;
                prms.DkMaxRecords = numLand > dknumber ? dknumber : 0;////每个界址线文件最多 条

                #region 删除文件

                var dkPath = string.Format(@"{0}\{1}{2}.shp", shapeFileOutputPath, "DK", zoneYearCode);
                DeleteFile(prms.DkMaxRecords > 0, dkPath);

                #endregion 删除文件

                #region 输出.prj文件需要

                var srid = db.QuerySRID(Zd_cbdFields.TABLE_NAME);
                if (srid >= 0)
                {
                    prms.sESRIPrjStr = prjString; //SpatialReferenceUtil.FindEsriSpatialReferenceString(spatialReferencePath, srid);
                }

                #endregion 输出.prj文件需要

                var exp = new ExportJzdx(db, prms, shapeFileOutputPath + @"\", currentZoneCode, zoneYearCode);
                exp.ReportProgress += (msg, i) =>
                {//进度
                    this.ReportProgress(i, msg);
                };
                exp.ReportInfomation += msg =>
                {//提示信息
                    this.ReportInfomation(msg);
                };
                exp.ReportWarn += msg =>
                {//提示警告信息
                    this.ReportWarn(msg);
                };
                exp.OnPresaveDk += en =>
                {//这里可以根据需要修改地块实体的内容
                    if (en.SYQXZ != "10" && en.SYQXZ != "30" && en.SYQXZ != "31" &&
                     en.SYQXZ != "32" && en.SYQXZ != "33" && en.SYQXZ != "34")
                        en.SYQXZ = "30";
                };

                exp.DoExportLandOnly(excludeDkbm);
            }
        }

        public virtual void DeleteFile(bool del, string path)
        {
            if (!del)
                return;
            try
            {
                File.Delete(path);
                var prj = Path.ChangeExtension(path, ".prj");
                if (File.Exists(prj))
                    File.Delete(prj);
                var dbf = Path.ChangeExtension(path, ".dbf");
                if (File.Exists(dbf))
                    File.Delete(dbf);
                var shx = Path.ChangeExtension(path, ".shx");
                if (File.Exists(shx))
                    File.Delete(shx);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion Override

        #region 数据检查

        /// <summary>
        /// 初始化数据
        /// </summary>
        public virtual bool InitalizeData(TaskBuildExportResultDataBaseArgument metadata)
        {
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
            showInformation = currentZone.Level == YuLinTu.Library.Entity.eZoneLevel.Group;
            if (!showInformation)
            {
                InitalizeDirectory();
                this.ReportInfomation("非组级地域请在安装目录下查看导出及检查信息!");
            }

            return true;
        }

        /// <summary>
        /// 数据处理
        /// </summary>
        public virtual void ArcDataProgress()
        {
            var zones = zoneStation.GetChildren(currentZone.FullCode, eLevelOption.SelfAndSubs);
            if (zones == null)
            {
                this.ReportError(string.Format("地域{0}下无地域数据", currentZone.FullName));
                return;
            }
            bool canContinue = CanContinue();
            if (!canContinue)
            {
                return;
            }
            //var dataProgress = new DataExportProgress();
            var spaceProgress = new ArcSpaceDataProgress();
            var extendSet = new HashSet<string>();
            spaceProgress.Alert += (s, e) => { this.ReportAlert(e.Grade, e.UserState, e.Description); };

            var efe = new ExportFileEntity();

            dataProgress.ContainDotLine = ContainDotLine;
            canContinue = InitalizeAgricultureDirectory(dataProgress, spaceProgress, efe);
            if (!canContinue)
            {
                return;
            }


            this.ReportProgress(1, string.Format("正在获取{0}数据", currentZone.FullName));
            var summerys = new List<DataSummary>();
            var dbReference = ReferenceHelper.GetDbReference<YuLinTu.Library.Entity.Zone>(DbContext, "JCSJ_XZQY", "Shape");
            currentZoneLandCount = contractLandWorkStation.Count(currentZone.FullCode, eLevelOption.SelfAndSubs);

            serNumberTemp = GetFullSerialNumberTemp(currentZone);
            var zonCount = zones.Count();
            var cZone = zones.Find(t => t.FullCode == currentZone.FullCode);
            if (CheckCardNumber)
            {
                ContractorNubmerProgress();
            }
            if (cZone.Level > Library.Entity.eZoneLevel.Village)
            {
                ExportXZLevel(cZone, zones, summerys, dataProgress, extendSet, dbReference);
            }
            else
            {
                ExportCZLevel(zones, summerys, zonCount, dataProgress, extendSet, dbReference);
            }
            if (!CanChecker)
            {
                ExportShapeExcel(summerys, spaceProgress, /*sqliteManager,*/ zones);
                if (OnlyExportLandResult)
                {
                    ExportOnlyLandResult(efe);
                }
                else
                {
                    if (ContainDotLine)
                    {
                        ExportLandResultFile(spaceProgress, extendSet);
                    }
                    ExportLandOnly(spaceProgress, extendSet);
                }
            }
            else
                ShowChecker(dataProgress);
            filterLandIDs = null;
            summerys = null;
            zones = null;
            GC.Collect();
        }

        /// <summary>
        /// 导出村/组的数据
        /// </summary>
        public virtual void ExportCZLevel(List<Library.Entity.Zone> zones, List<DataSummary> summerys, int zonCount,
            DataExportProgress dataProgress, HashSet<string> extendSet, SpatialReference dbReference)
        {
            int processIndex = 1;
            bool hasDx = false;
            foreach (var zone in zones)
            {
                zoneName = GetZoneName(zones, zone);
                var summery = new DataSummary();
                summery.UnitName = zone.FullName;
                summery.UnitCode = InitalizeZoneCode(zone);
                summery.Level = (Quality.Business.Entity.eZoneLevel)((int)(zone.Level));
                summery.ZoneCode = zone.FullCode;
                var count = VirtualPersonStation.GetByZoneCode(zone.FullCode).Count();
                summerys.Add(summery);
                processIndex++;
                if (count <= 0 && zone.Level == Library.Entity.eZoneLevel.Group)
                {
                    this.ReportAlert(eMessageGrade.Warn, null, zoneName + "下没有数据可供操作!");
                    continue;
                }
                var sqllandList = new List<SqliteDK>();
                //数据检查
                dataProgress.Srid = dbReference.WKID;
                var qgcbfs = VirtualPersonStation.GetRelationByZone(zone.FullCode, eLevelOption.Self);
                var qghts = stockconcordStation.Get(sv => sv.ZoneCode.Equals(zone.FullCode));
                var qgqzs = stockwarrantStation.GetByZoneCode(zone.FullCode, eLevelOption.Self);
                var entityCollection = DataCheckProgress(zone, ref hasDx, extendSet, sqllandList, qgcbfs, qghts, qgqzs);
                if ((entityCollection == null || entityCollection.Count == 0) && sqllandList.Count == 0)
                {
                    continue;
                }
                try
                {
                    var info = dataProgress.ExportDataFile(entityCollection, /*sqliteManager,*/
                        county.Name, county.FullCode, 0, county.FullCode + county.Name, summery,
                        /*ContainDotLine,*/ CBDKXXAwareAreaExportSet, sqllandList);
                    this.ReportAlert(eMessageGrade.Infomation, null, $"{zoneName}下属性成果数据导出完成:{info}");
                }
                catch (Exception ex)
                {
                    this.ReportAlert(eMessageGrade.Error, null, ex.Message + ",详细信息请查看日志");
                }
                entityCollection.Clear();
            }
        }

        /// <summary>
        /// 导出县/镇的数据
        /// </summary>
        public virtual void ExportXZLevel(Library.Entity.Zone cZone, List<Library.Entity.Zone> zones, List<DataSummary> summerys,
            DataExportProgress dataProgress, HashSet<string> extendSet, SpatialReference dbReference)
        {
            int processIndex = 1;
            var childzones = zones.FindAll(t => t.UpLevelCode == cZone.FullCode);
            var valligeCount = zones.Count(t => t.Level == Library.Entity.eZoneLevel.Village);
            double percent = 90.00 / valligeCount;
            dataProgress.Srid = dbReference.WKID;
            var dbs = new DataBaseCollection();
            dbs = GetDatasByZone(cZone, dbs);
            CreateUpSummary(cZone, zones, summerys);
            var type = CanChecker ? "检查" : "导出";
            foreach (var zone in childzones)
            {
                CreateSummary(zone, summerys);
                var dataBaseCollection = GetDatasByZone(zone, dbs);
                if (zone.Level == Library.Entity.eZoneLevel.Town)
                {
                    var czones = zones.FindAll(t => t.UpLevelCode == zone.FullCode);
                    foreach (var z in czones)//村
                    {
                        CreateSummary(z, summerys);
                        var dataCollection = GetDatasByZone(z, dataBaseCollection);
                        this.ReportProgress(5 + (int)(percent * (processIndex - 1)), $"({processIndex}/{valligeCount}){type}{GetZoneName(zones, z)}下的数据");
                        ExportDataByVallige(dataCollection, z, dataProgress, summerys, zones, extendSet);
                        processIndex++;
                    }
                }
                else
                {
                    this.ReportProgress(5 + (int)(percent * (processIndex - 1)), $"({processIndex}/{valligeCount}){type}{GetZoneName(zones, zone)}下的数据");
                    ExportDataByVallige(dataBaseCollection, zone, dataProgress, summerys, zones, extendSet);
                    processIndex++;
                }
            }
        }

        /// <summary>
        /// 创建上级统计
        /// </summary>
        public virtual void CreateUpSummary(Library.Entity.Zone cZone, List<Library.Entity.Zone> zones, List<DataSummary> summerys, bool insert = false)
        {
            CreateSummary(cZone, summerys, insert);
            if (cZone.Level < Library.Entity.eZoneLevel.County)
            {
                var z = zones.Find(f => f.FullCode == cZone.UpLevelCode);
                if (z == null)
                    z = zoneStation.Get(t => t.FullCode == cZone.UpLevelCode).FirstOrDefault();
                if (z != null)
                    CreateUpSummary(z, zones, summerys, true);
            }
        }


        /// <summary>
        /// 创建统计
        /// </summary>
        public virtual void CreateSummary(Library.Entity.Zone zone, List<DataSummary> summerys, bool insert = false)
        {
            var summery = new DataSummary();
            summery.UnitName = zone.FullName;
            summery.UnitCode = InitalizeZoneCode(zone);
            summery.Level = (Quality.Business.Entity.eZoneLevel)((int)zone.Level);
            summery.ZoneCode = zone.FullCode;
            if (insert)
            {
                summerys.Insert(0, summery);
            }
            else
            {
                summerys.Add(summery);
            }
        }

        /// <summary>
        /// 按村导出数据成果
        /// </summary>
        /// <param name="datas"></param>
        /// <param name="zone"></param>
        public virtual void ExportDataByVallige(DataBaseCollection datas, Library.Entity.Zone zone, DataExportProgress dataProgress,
        List<DataSummary> summerys, List<Library.Entity.Zone> zones, HashSet<string> extendSet)
        {
            bool hasDx = false;
            var qgcbfs = VirtualPersonStation.GetRelationByZone(zone.FullCode, eLevelOption.Subs);
            var qghts = stockconcordStation.Get(sv => sv.ZoneCode.StartsWith(zone.FullCode));
            var qgqzs = stockwarrantStation.GetByZoneCode(zone.FullCode, eLevelOption.SelfAndSubs);

            DataCollection collection = new DataCollection();
            var sqllandList = new List<SqliteDK>();
            var czones = zones.FindAll(t => t.UpLevelCode == zone.FullCode);
            if (czones.Count == 0)
            {
                var summary = new DataSummary();
                summary.UnitName = zone.FullName;
                summary.UnitCode = InitalizeZoneCode(zone);
                summary.Level = (Quality.Business.Entity.eZoneLevel)((int)(zone.Level));
                summary.ZoneCode = zone.FullCode;
                summerys.Add(summary);
                var entityCollection = DataCheckProgress(zone, ref hasDx, extendSet, sqllandList, qgcbfs, qghts, qgqzs, datas);
                var pdata = dataProgress.SetDataToProgress(entityCollection);
                ExportSummaryTable.SummaryData(pdata, summary, CBDKXXAwareAreaExportSet, sqllandList);
                collection.Add(pdata);
            }
            else
            {
                foreach (var cz in czones)
                {
                    var summary = new DataSummary();
                    summary.UnitName = cz.FullName;
                    summary.UnitCode = InitalizeZoneCode(cz);
                    summary.Level = (Quality.Business.Entity.eZoneLevel)((int)(cz.Level));
                    summary.ZoneCode = cz.FullCode;
                    summerys.Add(summary);
                    var dcount = datas.FamilyCollection.Count(t => t.ZoneCode == cz.FullCode);
                    if (dcount <= 0 && cz.Level == Library.Entity.eZoneLevel.Group)
                    {
                        this.ReportAlert(eMessageGrade.Warn, null, zoneName + "下没有数据可供操作!");
                        continue;
                    }
                    var entityCollection = DataCheckProgress(cz, ref hasDx, extendSet, sqllandList, qgcbfs, qghts, qgqzs, datas);
                    if ((entityCollection == null || entityCollection.Count == 0) && sqllandList.Count == 0)
                    {
                        continue;
                    }
                    var pdata = dataProgress.SetDataToProgress(entityCollection);
                    ExportSummaryTable.SummaryData(pdata, summary, CBDKXXAwareAreaExportSet, sqllandList);
                    collection.Add(pdata);
                    entityCollection.Clear();
                }
            }
            try
            {
                var info = dataProgress.ExportDataFile(collection, "", 1);
                //var info = dataProgress.ExportDataFile(entityCollection, county.Name, county.FullCode, 0, county.FullCode + county.Name,
                //    summery, CBDKXXAwareAreaExportSet, sqllandList);
                this.ReportAlert(eMessageGrade.Infomation, null, $"导出{zone.FullName}下数据:{info}");
            }
            catch (Exception ex)
            {
                this.ReportAlert(eMessageGrade.Error, null, ex.Message + ",详细信息请查看日志");
            }
        }


        /// <summary>
        /// 获取数据
        /// </summary> 
        public virtual DataBaseCollection GetDatasByZone(Library.Entity.Zone zone, DataBaseCollection dbs)
        {
            var zoneCode = zone.FullCode;
            DataBaseCollection dataBaseCollection = new DataBaseCollection();
            dataBaseCollection.ZoneCode = zoneCode;
            if (zone.Level == Library.Entity.eZoneLevel.County)
                return dataBaseCollection;
            if (zone.Level == Library.Entity.eZoneLevel.Town)
            {
                dataBaseCollection.ConcordCollection = concordStation.GetContractsByZoneCode(zoneCode, eLevelOption.SelfAndSubs);
                dataBaseCollection.BookCollection = contractRegeditBookStation.GetContractsByZoneCode(zoneCode, eLevelOption.SelfAndSubs);
                List<VirtualPerson> familyCollection = VirtualPersonStation.GetByZoneCode(zoneCode, eLevelOption.SelfAndSubs);
                if (familyCollection.Any(x => x.Name == "集体"))
                {
                    familyCollection.RemoveAt(familyCollection.Count - 1);
                }
                dataBaseCollection.FamilyCollection = familyCollection;
                dataBaseCollection.TissueCollection = senderStation.GetTissues(zoneCode, eLevelOption.SelfAndSubs);
                //List<ContractLand> AllLandCollection = contractLandWorkStation.GetCollection(zoneCode, eLevelOption.Self);
            }
            else
            {
                dataBaseCollection.BookCollection = dbs.BookCollection.FindAll(t => t.ZoneCode.StartsWith(zoneCode));
                dataBaseCollection.FamilyCollection = dbs.FamilyCollection.FindAll(t => t.ZoneCode.StartsWith(zoneCode));
                dataBaseCollection.ConcordCollection = dbs.ConcordCollection.FindAll(t => t.ZoneCode.StartsWith(zoneCode));
                dataBaseCollection.TissueCollection = dbs.TissueCollection.FindAll(t => t.ZoneCode.StartsWith(zoneCode));
                dataBaseCollection.LandCollection = contractLandWorkStation.GetCollection(zoneCode, eLevelOption.SelfAndSubs);
            }
            return dataBaseCollection;
        }


        /// <summary>
        /// 导出地块、界址点、界址线
        /// </summary>
        public virtual void ExportLandResultFile(ArcSpaceDataProgress spaceProgress, HashSet<string> extendSet)
        {
            var pointquery = DbContext.CreateQuery<BuildLandBoundaryAddressDot>();
            var linequery = DbContext.CreateQuery<BuildLandBoundaryAddressCoil>();
            var landquery = DbContext.CreateQuery<ContractLand>();

            //var landCount = landquery.Where(t => t.SenderCode.StartsWith(currentZone.FullCode)).Select(s => s.ID).Count();
            //var jzdcount = pointquery.Where(t => t.SenderCode.StartsWith(currentZone.FullCode)).Select(s => s.ID).Count();
            //var jzxcount = linequery.Where(t => t.SenderCode.StartsWith(currentZone.FullCode)).Select(s => s.ID).Count();

            var landCount = landquery.Count(t => t.ZoneCode.StartsWith(currentZone.FullCode));
            var jzdcount = jzdStation.Count(currentZone.FullCode, eLevelOption.SelfAndSubs);
            var jzxcount = jzxStation.Count(currentZone.FullCode, eLevelOption.SelfAndSubs);

            if (jzdcount == 0 && jzxcount == 0)
            {
                ContainDotLine = false;
                this.ReportWarn(string.Format("{0}({1})下无界址点、界址线数据，请初始化后再导出数据！", currentZone.FullName, currentZone.FullCode));
            }
            if (ContainDotLine)
            {
                if (jzdcount > 6000000)
                {
                    ExportBigData(landCount, jzdcount, jzxcount, spaceProgress.ShapeFilePath, spaceProgress.SpatialText);
                }
                else
                {
                    testExportJzd(DbContext, spaceProgress.ShapeFilePath, currentZone.FullCode,
                    county.FullCode + DateTime.Now.Year, spaceProgress.SpatialText, extendSet,
                    landCount, jzdcount, jzxcount);
                }
            }
        }

        /// <summary>
        /// 导出地块
        /// </summary>
        public virtual void ExportLandOnly(ArcSpaceDataProgress spaceProgress, HashSet<string> extendSet)
        {
            var landquery = DbContext.CreateQuery<ContractLand>();

            var landCount = landquery.Count(t => t.SenderCode.StartsWith(currentZone.FullCode));
            var lands = landquery.Where(t => t.SenderCode.StartsWith(currentZone.FullCode));
            ExportLand(DbContext, spaceProgress.ShapeFilePath, currentZone.FullCode,
                    county.FullCode + DateTime.Now.Year, spaceProgress.SpatialText, extendSet,
                    landCount);
        }

        /// <summary>
        /// 导出大数据
        /// </summary>
        public virtual void ExportBigData(int landCount, int jzdcount, int jzxcount, string outpath, string prj)
        {
            var dbFile = DbContext.DataSource.ConnectionString;
            dbFile = dbFile.Substring(dbFile.IndexOf('=') + 1);
            var args = new ExportShapeTaskArgument()
            {
                DbFile = dbFile,
                LandNumber = landCount,
                LineNumber = jzxcount,
                PointNumber = jzdcount,
                ShapeOutPath = outpath,
                ZoneYearCode = county.FullCode + DateTime.Now.Year,
                ZoneCode = currentZone.FullCode,
                ESRIPrjStr = prj,
                OnlyKey = OnlyKey,
                UseUniteNumberExport = UseUniteNumberExport
            };
            args.ESRIPrjStr = "";
            var task = new ExportShapeTask();
            task.Argument = args;
            task.Name = "数据成果导出";
            task.Description = "导出界址点、线、地块";
            var exepath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"ManualSlaveProcessTask\ManualSlaveProcessTask.exe");
            var childTask = new PrimaryHostTask(task) { SlaveProcessExeFileName = exepath };
            childTask.Alert += (s, e) => { this.ReportAlert(e); };
            childTask.ProgressChanged += (s, e) => { this.ReportProgress(e.Percent); };
            childTask.Start();
        }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="efe"></param>
        public void ExportOnlyLandResult(ExportFileEntity efe)
        {
            efe.IsAllExport = false;
            var properties = typeof(ExportFileEntity).GetProperties();
            foreach (var p in properties)
            {
                FileEntity fe = p.GetValue(efe) as FileEntity;
                if (fe == null)
                    continue;
                if (p.Name == "VictorDK" ||
                    p.Name == "VictorJZD" ||
                    p.Name == "VictorJZX")
                {
                    fe.IsExport = true;
                }
                else
                {
                    fe.IsExport = false;
                }
            }
        }

        /// <summary>
        /// 导出shape及Excel文件
        /// </summary>
        public virtual void ExportShapeExcel(List<DataSummary> summerys, ArcSpaceDataProgress spaceProgress,
           /*SqliteManager manager,*/ List<YuLinTu.Library.Entity.Zone> zones)
        {
            if (!ContainMatrical)
            {
                string imagePath = Folder + @"\" + county.FullCode + county.Name + @"\" + "扫描资料";
                if (Directory.Exists(imagePath))
                    Directory.Delete(imagePath, true);
            }

            this.ReportProgress(92, string.Format("导出汇总表格数据..."));
            ExportMataFile(spaceProgress);
            string summeryPath = Folder + @"\" + county.FullCode + county.Name + @"\汇总表格";
            ExportSummaryTable summarytable = new ExportSummaryTable(summerys, summeryPath, county.FullCode + county.Name);
            var excelResult = summarytable.ExportTable();
            if (excelResult)
                this.ReportProgress(93, string.Format("汇总表格文件导出成功"));
            try
            {
                this.ReportProgress(93, string.Format("正在处理矢量数据"));
                //if (!ContainDotLine)
                //{
                //    spaceProgress.ExportSpaceDataBase(manager, currentZone.FullCode, true);
                //}
                ZoneSpaceDataExport(spaceProgress, zones);
            }
            catch (Exception ex)
            {
                this.ReportProgress(100, "完成");
                this.ReportAlert(eMessageGrade.Exception, null, ex.Message + "详细信息请看日志");
            }
            GC.Collect();
        }

        /// <summary>
        /// 显示检查信息
        /// </summary>
        public virtual void ShowChecker(DataExportProgress progress)
        {
            if (Directory.Exists(progress.ShapeFilePath))
            {
                System.IO.DirectoryInfo directory = Directory.GetParent(progress.ShapeFilePath);
                directory.Delete(true);
            }
            if (!showInformation)
            {
                string dataRecord = Application.StartupPath + @"\Error\DataChecker.txt";
                if (CanChecker)
                {
                    this.ReportAlert(eMessageGrade.Error, null, "导出" + currentZone.FullName + "下数据存在问题,请在" + dataRecord + "中查看详细信息...");
                }
                if (System.IO.File.Exists(dataRecord))
                {
                    System.Diagnostics.Process.Start(dataRecord);
                }
            }
            this.ReportProgress(100, "完成");
        }

        /// <summary>
        /// 数据检查，包括获取当前地域下总的地块开始
        /// </summary>
        public virtual List<ExchangeRightEntity> DataCheckProgress(YuLinTu.Library.Entity.Zone zone, ref bool hasDx, HashSet<string> extendset,
         List<SqliteDK> sqliteLand, List<BelongRelation> realationList, List<StockConcord> qghts, List<StockWarrant> qgqzs, DataBaseCollection datas = null)
        {
            currentIndex = 1;
            filterLandIDs = new List<Guid>();
            // zoneName = zone.FullName;
            if (datas == null)
                datas = new DataBaseCollection();
            List<ContractConcord> concordCollection = datas.ConcordCollection.FindAll(t => t.ZoneCode == zone.FullCode);
            List<ContractRegeditBook> bookCollection = datas.BookCollection.FindAll(t => t.ZoneCode == zone.FullCode);
            List<VirtualPerson> familyCollection = datas.FamilyCollection.FindAll(t => t.ZoneCode == zone.FullCode);
            List<ContractLand> AllLandCollection = datas.LandCollection.FindAll(t => t.SenderCode == zone.FullCode);
            CollectivityTissue tissue = datas.TissueCollection.Find(t => t.Code == zone.FullCode.PadRight(14, '0'));
            if (concordCollection == null || concordCollection.Count == 0)
            {
                concordCollection = concordStation.GetByZoneCode(zone.FullCode);
            }
            if (bookCollection == null || bookCollection.Count == 0)
            {
                bookCollection = contractRegeditBookStation.GetByZoneCode(zone.FullCode, eSearchOption.Precision);
            }
            if (familyCollection == null || familyCollection.Count == 0)
            {
                familyCollection = VirtualPersonStation.GetByZoneCode(zone.FullCode);
                if (familyCollection.Any(x => x.Name == "集体"))
                {
                    familyCollection.RemoveAt(familyCollection.Count - 1);
                }
            }
            if (AllLandCollection == null || AllLandCollection.Count == 0)
            {
                AllLandCollection = contractLandWorkStation.GetCollection(zone.FullCode, eLevelOption.Self);
            }
            if (tissue == null)
            {
                if (zone.Level == YuLinTu.Library.Entity.eZoneLevel.Group)
                {
                    tissue = senderStation.GetByCode(zone.FullCode.Substring(0, 14));
                }
                else
                {
                    tissue = senderStation.GetByCode(zone.FullCode.PadRight(14, '0'));
                }
            }
            if (tissue == null)
            {
                if (familyCollection.Count > 0)
                    this.ReportError($"未找到{zone.FullName}下的编码为{zone.FullCode.PadRight(14, '0')}的发包方！");
                return null;
            }
            List<ContractLand> landCollection = FilterLandType(familyCollection, AllLandCollection);//根据设置筛选地块
            List<ContractLand> landSpaceCollection = landCollection.FindAll(l => l.Shape != null);

            bool showPersent = familyCollection.Count >= 100 ? false : true;
            string progressDescription = "(" + familyCollection.Count.ToString() + ")";
            double vppercent = 90 / (double)familyCollection.Count;

            //if (canExport == false) return null;//如果发包方检查调查日期为空，直接返回
            bool canExport = Checkdata(familyCollection, landCollection, landSpaceCollection, concordCollection, bookCollection, realationList, qghts, qgqzs, tissue, zone);
            var entityCollection = new List<ExchangeRightEntity>();
            if (canExport)
            {
                entityCollection = CreateExchangeEntity(familyCollection, concordCollection,
                    landCollection, tissue, zone, bookCollection, ref hasDx, realationList, qghts);
                var extendColl = ExtentLandCode(AllLandCollection, entityCollection);
                foreach (var item in extendColl)
                {
                    if (!IsReportNoConcordLands)
                    {
                        extendset.Add(item);
                    }
                    else
                    {
                        var land = AllLandCollection.Find(t => t.LandNumber == item);
                        if (land != null)
                        {
                            var dkex = InitalizeSpaceLandData(land);
                            var sldk = dkex.ConvertTo<SqliteDK>();
                            sldk.Shape = dkex.Shape as YuLinTu.Spatial.Geometry;
                            sqliteLand.Add(sldk);
                        }
                    }
                }
            }
            if (entityCollection.Count == 0 && tissue != null && sqliteLand.Count > 0)
            {
                entityCollection.Add(new ExchangeRightEntity() { FBF = InitalizeSenderData(tissue) });
            }
            landSpaceCollection = null;
            tissue = null;
            bookCollection = null;
            concordCollection = null;
            landCollection = null;
            familyCollection = null;
            return entityCollection;
        }

        /// <summary>
        /// 数据检查
        /// </summary>
        public virtual bool Checkdata(List<VirtualPerson> familyCollection, List<ContractLand> landCollection,
          List<ContractLand> landSpaceCollection, List<ContractConcord> concordCollection, List<ContractRegeditBook> bookCollection,
          List<BelongRelation> realationList, List<StockConcord> qghts, List<StockWarrant> qgqzs, CollectivityTissue tissue, Library.Entity.Zone zone)
        {
            bool canExport = true;//是否可以导出
            foreach (VirtualPerson vp in familyCollection)
            {
                if (vp.Name.Contains("集体") || vp.Name.Contains("机动地"))
                {
                    continue;
                }
                List<ContractLand> lands = landCollection.FindAll(ld => ld.OwnerId != null && ld.OwnerId.HasValue && ld.OwnerId.Value == vp.ID);
                var brqglandcount = realationList.Count(t => t.VirtualPersonID == vp.ID);//确股的
                if ((lands == null || lands.Count == 0) && brqglandcount == 0)
                {
                    continue;
                }
                if (CanChecker)
                {
                    if (!ContractorProgress(vp))
                        canExport = false;
                }
                string description = string.Format("{0}下承包方:{1}", zoneName, vp.Name);
                foreach (var land in lands)
                {
                    if (CanChecker)
                    {
                        if (!ContractLandProgress(land))
                            canExport = false;
                    }
                    var arcLand = landSpaceCollection.Find(ld => ld.ID == land.ID || ld.LandNumber == land.LandNumber);
                    if (arcLand == null)
                    {
                        if (showInformation)
                        {
                            this.ReportWarn(description + "中地块编码为:" + land.LandNumber + "的地块无空间信息!");
                        }
                        else
                        {
                            WriteDataInformation("警告:" + description + "中地块编码为:" + land.LandNumber + "的地块无空间信息!");
                        }
                        canExport = CanChecker ? false : true;
                    }
                    if (land.LandNumber.IsNullOrEmpty() || land.LandNumber.Length < 19)
                    {
                        if (showInformation)
                        {
                            this.ReportWarn(description + "中地块编码为:" + land.LandNumber + "的不符合农业部规范!");
                        }
                        else
                        {
                            WriteDataInformation("警告:" + description + "中地块编码为:" + land.LandNumber + "的不符合农业部规范!");
                        }
                    }
                }
                lands = null;
                currentIndex++;
                bool concordExist = concordCollection.Exists(cd => !string.IsNullOrEmpty(cd.ConcordNumber) && cd.ContracterId != null && cd.ContracterId.HasValue && cd.ContracterId.Value == vp.ID);
                var qgconcord = qghts.FindAll(t => t.ContracterId == vp.ID);
                if (qgconcord.Count > 0)
                    concordExist = true;
                if (!concordExist)
                {
                    if (showInformation)
                    {
                        this.ReportWarn(description + "未签订承包合同!");
                    }
                    else
                    {
                        WriteDataInformation("警告:" + description + "未签订承包合同!");
                    }
                    canExport = CanChecker ? false : true;
                }
                List<ContractConcord> concords = concordCollection.FindAll(cd => !string.IsNullOrEmpty(cd.ConcordNumber) && cd.ContracterId != null && cd.ContracterId.HasValue && cd.ContracterId.Value == vp.ID);
                if (concords.Count > 2)
                {
                    if (showInformation)
                    {
                        this.ReportWarn(description + "承包合同数据超过2个，请检查!");
                    }
                    else
                    {
                        WriteDataInformation("警告:" + "承包合同数据超过2个，请检查!");
                    }
                }
                bool warrantExist = false;
                foreach (ContractConcord concord in concords)
                {
                    if (concord.ArableLandStartTime == null)
                    {
                        WriteDataInformation(string.Format("警告:" + "承包合同{0}的承包起始时间为空!", concord.ConcordNumber));
                    }
                    //if (!string.IsNullOrEmpty(concord.ConcordNumber))
                    //{
                    //    tissue = senderStation.GetByCode(concord.ConcordNumber.Substring(0, 14));
                    //}

                    ContractRegeditBook book = bookCollection.Find(bk => !string.IsNullOrEmpty(concord.ConcordNumber) && bk.ID == concord.ID);
                    if (bookCollection.Count == 0 && !warrantExist)
                    {
                        book = contractRegeditBookStation.Get(concord.ID);
                    }
                    if (book != null && !string.IsNullOrEmpty(book.RegeditNumber))
                    {
                        warrantExist = true;
                        // break;
                    }
                    else if (book == null)
                    {
                        warrantExist = false;
                    }
                }
                concords = null;
                foreach (var qgconcorditem in qgconcord)
                {
                    var books = qgqzs.FindAll(bk => bk.ID == qgconcorditem.ID);
                    if (books.Count == 0)
                    {
                        warrantExist = false;
                    }
                    else
                    {
                        warrantExist = true;
                    }
                }

                if (!warrantExist)
                {
                    if (showInformation)
                    {
                        this.ReportWarn(description + "有承包权证未签订!");
                    }
                    else
                    {
                        WriteDataInformation("警告:" + description + "有承包权证未签订!");
                    }
                    canExport = CanChecker ? false : true;
                }
            }
            if (CanChecker)
            {
                if (!SenderProgress(tissue))
                    canExport = false;
            }
            if (tissue == null)
            {
                this.ReportWarn(zone.FullName + "(" + zone.FullCode + ")" + "未获取到合同对应的发包方，请检查是否签订合同或发包方数据!");
            }
            return canExport;
        }

        /// <summary>
        /// 获取排除掉的地块编码
        /// </summary>
        /// <returns></returns>
        public virtual HashSet<string> ExtentLandCode(List<ContractLand> allLandCollection,
            List<ExchangeRightEntity> ereList)
        {
            if (ereList == null)
                return new HashSet<string>();
            HashSet<string> exportCode = new HashSet<string>();
            HashSet<string> extendCode = new HashSet<string>();
            foreach (var item in ereList)
            {
                var dkList = item.KJDK;
                if (dkList == null)
                    continue;
                foreach (var dk in dkList)
                {
                    if (!string.IsNullOrEmpty(dk.DKBM))
                        exportCode.Add(dk.DKBM);
                }
            }
            foreach (var dk in allLandCollection)
            {
                if (!string.IsNullOrEmpty(dk.LandNumber) && !exportCode.Contains(dk.LandNumber))
                    extendCode.Add(dk.LandNumber);
            }
            return extendCode;
        }

        /// <summary>
        /// 构建交换实体
        /// </summary>
        public virtual List<ExchangeRightEntity> CreateExchangeEntity(List<VirtualPerson> familyCollection, List<ContractConcord> concordCollection,
            List<ContractLand> landCollection, CollectivityTissue tissue, YuLinTu.Library.Entity.Zone zone, List<ContractRegeditBook> bookCollection,
            ref bool hasDx, List<BelongRelation> qglands, List<StockConcord> qghts)
        {
            var entityCollection = new List<ExchangeRightEntity>();
            var landArray = GetLandMapNumberSet();
            currentIndex = 1;
            var vppercent = 90 / (double)familyCollection.Count;
            var showPersent = concordCollection.Count >= 100 ? false : true;
            var progressDescription = "(" + concordCollection.Count.ToString() + ")";
            HashSet<string> spacecdbDKBMs = new HashSet<string>();

            //var qghttable = DbContext.DataSource.CreateSchema().AnyElement(null, "QGCBJYQ_HT");
            //var qgqztable = DbContext.DataSource.CreateSchema().AnyElement(null, "QGCBJYQ_QZ");
            //var qglands = VirtualPersonStation.GetRelationByZone(zone.FullCode);//确股的地块
            //var qghts = stockconcordStation.Get(sv => sv.SenderCode == zone.FullCode);//确股的合同
            //var serNumberTemp = GetFullSerialNumberTemp(zone);

            //ParallelThread.ForEach(familyCollection, (i, vp) =>
            foreach (var vp in familyCollection)
            {
                //处理空户
                var vplands = landCollection.FindAll(ld => ld.OwnerId == vp.ID);
                var brqglands = qglands.FindAll(t => t.VirtualPersonID == vp.ID);//确股的

                if (vplands != null && vplands.Count == 0 && brqglands.Count == 0 && IsReportNoConcordNoLandsFamily == false)
                {
                    continue;
                }

                var concords = concordCollection.FindAll(fam => fam.ContracterId == vp.ID);
                var entity = new ExchangeRightEntity();
                entity.ZoneCode = zone.FullCode;
                entity.FBF = InitalizeSenderData(tissue);
                entity.VirtualPersonCode = InitalizeContractorCode(vp, zone);
                entity.CBF = InitalizeContractorData(vp, entity.VirtualPersonCode);
                entity.JTCY = InitalizeSharePersonData(vp, entity.VirtualPersonCode, false);
                var vphts = new List<ICBHT>();
                List<CBJYQZ> vpcbjyqzs = new List<CBJYQZ>();
                List<CBJYQZDJB> vpcbjyqzdjbs = new List<CBJYQZDJB>();
                List<CBDKXX> cbds = new List<CBDKXX>();
                List<DKEX> spacecdbs = new List<DKEX>();
                int concordlandsCount = 0;

                //处理未签合同的地
                if (IsReportNoConcordLands)
                {
                    var noconcordlands = new List<ContractLand>();//未签合同的地
                    if ((concords != null && concords.Count > 0) && vplands.Count > 0)//签了合同，另外没签合同的地
                    {
                        foreach (var ld in vplands)
                        {
                            if (concords.Any(cc => cc.ID == ld.ConcordId) == false)
                                noconcordlands.Add(ld);
                        }
                    }
                    else if ((concords != null && concords.Count == 0) && vplands.Count > 0)//没签合同有地
                    {
                        noconcordlands.AddRange(vplands);
                    }
                    if (noconcordlands != null && noconcordlands.Count > 0)
                    {
                        noconcordlands.RemoveAll(nn => nn.IsStockLand == true);//移除确股的，确股的会单独处理
                        foreach (var noconcordland in noconcordlands)
                        {
                            DKEX dk = InitalizeSpaceLandData(noconcordland);//, pointList, lineList);
                            spacecdbs.Add(dk);
                            spacecdbDKBMs.Add(dk.DKBM);
                        }
                    }
                }

                //签订了合同的-确权的
                foreach (var concorditem in concords)
                {
                    ProcessConcordData(entity, concorditem, vplands, cbds, spacecdbs, spacecdbDKBMs, brqglands,
                        vp, landCollection, vphts, vpcbjyqzdjbs, bookCollection, vpcbjyqzs, landArray, serNumberTemp);

                }
                if (qghttable && qgqztable)
                {
                    ///如果只有确股地块，就按照确股的流程来处理
                    var qgconcord = qghts.FindAll(sv => sv.ContracterId == vp.ID);

                    //签订了合同的-确股的
                    foreach (var concorditem in qgconcord)
                    {
                        var qgrelationLands = VirtualPersonStation.GetRelationsByVpID(vp.ID);
                        concordlandsCount = qgrelationLands.Count;
                        var ht = InitalizeConcordData(concorditem, concordlandsCount);
                        if (ht != null)
                        {
                            ht.FBFBM = tissue.Code;
                            ht.CBFBM = entity.VirtualPersonCode;
                            ht.HTZMJ = 0;
                            ht.HTZMJM = 0;
                            ht.YCBHTBM = vp.FamilyExpand.ConcordNumber;
                            vphts.Add(ht);
                        }

                        foreach (BelongRelation land in qgrelationLands)
                        {
                            var qgland = landCollection.Find(ll => ll.ID == land.LandID);
                            if (qgland != null)
                            {
                                if (filterLandIDs.Contains(qgland.ID) == false)
                                {
                                    filterLandIDs.Add(qgland.ID);
                                }
                                CBDKXX cbd = InitalizeQgAgricultureLandData(qgland, land, tissue.Code, entity.VirtualPersonCode);
                                DKEX dk = InitalizeSpaceLandData(qgland);//, pointList, lineList);
                                cbd.CBHTBM = concorditem.ConcordNumber;
                                cbd.CBJYQZBM = concorditem.ConcordNumber;
                                cbds.Add(cbd);
                                if (spacecdbDKBMs.Contains(dk.DKBM) == false)
                                {
                                    spacecdbs.Add(dk);
                                    spacecdbDKBMs.Add(dk.DKBM);
                                }
                            }
                            ht.HTZMJ = ht.HTZMJ + Math.Round(land.QuanficationArea / 0.0015, 2);
                            ht.HTZMJM = ht.HTZMJM + Math.Round(land.QuanficationArea, 2);
                        }

                        var books = stockwarrantStation.Get(bk => bk.ID == concorditem.ID);
                        if (books != null && books.Count > 0)
                        {
                            var book = books[0];
                            entity.BookCode = book != null ? book.Number : "";
                            var cbjyqz = InitalizeWarrantBook(book, vp);
                            if (cbjyqz != null)
                                vpcbjyqzs.Add(cbjyqz);
                            var djb = InitalizeRegeditBook(concorditem, book);
                            if (djb != null)
                            {
                                djb.YCBJYQZBH = vp.FamilyExpand.WarrantNumber;
                                djb.CBFBM = entity.VirtualPersonCode;
                                djb.FBFBM = tissue.Code;
                                if (IsSaveParcelPathAsPDF)
                                {
                                    djb.DKSYT = "图件" + "\\" + djb.FBFBM + "\\" + "DKSYT" + djb.CBJYQZBM + ".pdf";
                                }
                                else
                                {
                                    djb.DKSYT = BuildLandMapString(djb.FBFBM, djb.CBJYQZBM, cbds, landArray);
                                }
                                djb.CBJYQZLSH = SetFullSerialNumberName(serNumberTemp, book.Year, book.SerialNumber);

                                vpcbjyqzdjbs.Add(djb);
                            }
                            if (ContainMatrical)
                            {
                                InitalizeAccessory(book, entity);
                            }
                        }
                    }
                }

                entity.DKXX = cbds;
                entity.HT = vphts;
                entity.CBJYQZ = vpcbjyqzs;
                entity.DJB = vpcbjyqzdjbs;
                entity.KJDK = spacecdbs;
                entityCollection.Add(entity);
            }
            return entityCollection;
        }

        /// <summary>
        /// 处理合同数据
        /// </summary> 
        public virtual void ProcessConcordData(ExchangeRightEntity entity, ContractConcord concorditem, List<ContractLand> vplands, List<CBDKXX> cbds,
            List<DKEX> spacecdbs, HashSet<string> spacecdbDKBMs, List<BelongRelation> brqglands, VirtualPerson vp, List<ContractLand> landCollection,
            List<ICBHT> vphts, List<CBJYQZDJB> vpcbjyqzdjbs, List<ContractRegeditBook> bookCollection, List<CBJYQZ> vpcbjyqzs, int[] landArray, string serNumberTemp)
        {
            List<ContractLand> lands = vplands.FindAll(ld => ld.ConcordId != null && ld.ConcordId.HasValue && ld.ConcordId.Value == concorditem.ID);
            var concordlandsCount = lands.Count;
            foreach (ContractLand land in lands)
            {
                filterLandIDs.Add(land.ID);
                ContractLand arcLand = vplands.Find(ld => ld.ID == land.ID || ld.LandNumber == land.LandNumber);
                CBDKXX cbd = InitalizeAgricultureLandData(arcLand, entity.FBF.FBFBM, entity.VirtualPersonCode);
                DKEX dk = InitalizeSpaceLandData(arcLand);//, pointList, lineList);
                cbd.CBHTBM = concorditem.ConcordNumber;
                cbd.CBJYQZBM = concorditem.ConcordNumber;
                cbds.Add(cbd);
                spacecdbs.Add(dk);
                spacecdbDKBMs.Add(dk.DKBM);
            }

            var ht = InitalizeConcordData(concorditem, concordlandsCount);
            if (ht != null)
            {
                ht.YCBHTBM = vp.FamilyExpand.ConcordNumber;
                ht.FBFBM = entity.FBF.FBFBM;
                ht.CBFBM = entity.VirtualPersonCode;
                vphts.Add(ht);
            }
            if (ht.CBFS == "110")
            {
                //处理这个人下的确股地块
                ht.CBDKZS += brqglands.Count;
                foreach (var britem in brqglands)
                {
                    var qgland = landCollection.Find(ll => ll.ID == britem.LandID);
                    if (qgland != null)
                    {
                        if (filterLandIDs.Contains(qgland.ID) == false)
                        {
                            filterLandIDs.Add(qgland.ID);
                        }
                        var relationland = VirtualPersonStation.GetRelationByID(vp.ID, qgland.ID);
                        if (relationland != null)
                        {
                            CBDKXX cbd = InitalizeQgAgricultureLandData(qgland, relationland, entity.FBF.FBFBM, entity.VirtualPersonCode);
                            DKEX dk = InitalizeSpaceLandData(qgland);//, pointList, lineList);
                            cbd.CBHTBM = concorditem.ConcordNumber;
                            cbd.CBJYQZBM = concorditem.ConcordNumber;
                            cbds.Add(cbd);
                            if (spacecdbDKBMs.Contains(dk.DKBM) == false)
                            {
                                spacecdbs.Add(dk);
                                spacecdbDKBMs.Add(dk.DKBM);
                            }
                            ht.HTZMJ = ht.HTZMJ + Math.Round(relationland.QuanficationArea / 0.0015, 2);
                            ht.HTZMJM = ht.HTZMJM + Math.Round(relationland.QuanficationArea, 2);
                        }
                    }
                }
            }
            ContractRegeditBook book = bookCollection.Find(bk => bk.ID == concorditem.ID);
            if (bookCollection.Count == 0 && book == null)
            {
                book = contractRegeditBookStation.Get(concorditem.ID);
            }
            entity.BookCode = book != null ? book.Number : "";
            var cbjyqz = InitalizeWarrantBook(book, vp);
            if (cbjyqz != null)
                vpcbjyqzs.Add(cbjyqz);
            var djb = InitalizeRegeditBook(concorditem, book);
            if (djb != null)
            {
                djb.YCBJYQZBH = vp.FamilyExpand.WarrantNumber;
                djb.CBFBM = entity.VirtualPersonCode;
                djb.FBFBM = entity.FBF.FBFBM;
                if (IsSaveParcelPathAsPDF)
                {
                    djb.DKSYT = "图件" + "\\" + djb.FBFBM + "\\" + "DKSYT" + djb.CBJYQZBM + ".pdf";
                }
                else
                {
                    djb.DKSYT = BuildLandMapString(djb.FBFBM, djb.CBJYQZBM, cbds, landArray);
                }
                djb.CBJYQZLSH = SetFullSerialNumberName(serNumberTemp, book.Year, book.SerialNumber);

                vpcbjyqzdjbs.Add(djb);
            }
            if (ContainMatrical)
            {
                InitalizeAccessory(book, entity);
            }
        }

        /// <summary>
        /// 构建地块示意图
        /// </summary>
        public virtual string BuildLandMapString(string fbfbm, string qzbm, List<CBDKXX> cbds, int[] array)
        {
            var syt = "";
            var dksty = "图件\\{0}\\DKSYT{1}{2}.jpg";
            var lands = new HashSet<string>();
            foreach (var dk in cbds)
            {
                if (!lands.Contains(dk.DKBM))
                    lands.Add(dk.DKBM);
            }
            int count = lands.Count;
            for (int i = 0; i < array.Length; i++)
            {
                count -= array[i];
                if (count > 0)
                {
                    syt += string.Format(dksty, fbfbm, qzbm, i + 1) + "/";
                }
                else
                {
                    syt += string.Format(dksty, fbfbm, qzbm, i + 1) + "";
                    break;
                }
            }
            lands.Clear();
            return syt.TrimEnd('/');
        }

        /// <summary>
        /// 设置地块示意图数量
        /// </summary>
        /// <returns></returns>
        public virtual int[] GetLandMapNumberSet()
        {
            var settings = System.Configuration.ConfigurationManager.AppSettings;
            var numSet = new int[10] { 8, 8, 8, 8, 8, 8, 8, 8, 8, 8 };
            if (settings.AllKeys.Contains("LandMapNumber"))
            {
                try
                {
                    var settingvalue = settings.Get("LandMapNumber");
                    var array = settingvalue.Split(',');
                    var arrayInt = new int[10];
                    for (int i = 0; i < array.Length; i++)
                    {
                        if (i > 9)
                            break;
                        arrayInt[i] = Convert.ToInt32(array[i]);
                    }
                    return arrayInt;
                }
                catch
                {
                    return numSet;
                }
            }
            else
            {
                return numSet;
            }
        }

        /// <summary>
        /// 发包方处理-检查处理
        /// </summary>
        public virtual bool SenderProgress(CollectivityTissue tissue)
        {
            bool canExport = true;
            if (tissue == null)
            {
                return false;
            }
            string description = string.Format("{0}下发包方:{1}中", zoneName, tissue.Name);
            if (string.IsNullOrEmpty(YuLinTu.Library.Business.ToolString.ExceptSpaceString(tissue.LawyerName)))
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
            if (string.IsNullOrEmpty(YuLinTu.Library.Business.ToolString.ExceptSpaceString(tissue.LawyerCartNumber)))
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
                bool isRight = YuLinTu.Library.Business.ToolICN.Check(tissue.LawyerCartNumber);
                if (!isRight)
                {
                    if (showInformation)
                    {
                        this.ReportAlert(eMessageGrade.Warn, null, description + "发包方负责人证件号码" + tissue.LawyerCartNumber + "不符合身份证算法验证规范!");
                    }
                    else
                    {
                        WriteDataInformation("警告:" + description + "发包方负责人证件号码" + tissue.LawyerCartNumber + "不符合身份证算法验证规范!");
                    }
                }
            }
            if (string.IsNullOrEmpty(YuLinTu.Library.Business.ToolString.ExceptSpaceString(tissue.LawyerTelephone)))
            {
                if (showInformation)
                {
                    this.ReportAlert(eMessageGrade.Warn, null, description + "发包方负责人联系电话未填写!");
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
            if (string.IsNullOrEmpty(YuLinTu.Library.Business.ToolString.ExceptSpaceString(tissue.LawyerAddress)))
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
            if (string.IsNullOrEmpty(YuLinTu.Library.Business.ToolString.ExceptSpaceString(tissue.LawyerPosterNumber)))
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
            if (string.IsNullOrEmpty(YuLinTu.Library.Business.ToolString.ExceptSpaceString(tissue.SurveyPerson)))
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
            if (string.IsNullOrEmpty(YuLinTu.Library.Business.ToolString.ExceptSpaceString(tissue.SurveyChronicle)))
            {
                if (showInformation)
                {
                    this.ReportAlert(eMessageGrade.Warn, null, description + "发包方调查记事未填写!");
                }
                else
                {
                    WriteDataInformation("提示:" + description + "发包方调查记事未填写!");
                }
            }
            return canExport;
        }

        /// <summary>
        /// 承包方处理
        /// </summary>
        public virtual bool ContractorProgress(VirtualPerson vp)
        {
            bool canExport = true;
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
            if (string.IsNullOrEmpty(YuLinTu.Library.Business.ToolString.ExceptSpaceString(vp.Number)))
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
                bool isRight = YuLinTu.Library.Business.ToolICN.Check(vp.Number);
                if (!isRight)
                {
                    if (showInformation)
                    {
                        this.ReportAlert(eMessageGrade.Warn, null, description + "承包方证件号码" + vp.Number + "不符合身份证算法验证规范!");
                    }
                    else
                    {
                        WriteDataInformation("提示:" + description + "承包方证件号码" + vp.Number + "不符合身份证算法验证规范!");
                    }
                }
            }
            if (string.IsNullOrEmpty(YuLinTu.Library.Business.ToolString.ExceptSpaceString(vp.Address)))
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
            if (string.IsNullOrEmpty(YuLinTu.Library.Business.ToolString.ExceptSpaceString(vp.PostalNumber)))
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
            if (string.IsNullOrEmpty(YuLinTu.Library.Business.ToolString.ExceptSpaceString(vp.Telephone)))
            {
                if (showInformation)
                {
                    this.ReportAlert(eMessageGrade.Warn, null, description + "承包方联系电话未填写!");
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
                if (string.IsNullOrEmpty(YuLinTu.Library.Business.ToolString.ExceptSpaceString(expand.SurveyPerson)))
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
                if (string.IsNullOrEmpty(YuLinTu.Library.Business.ToolString.ExceptSpaceString(expand.SurveyChronicle)))
                {
                    if (showInformation)
                    {
                        this.ReportAlert(eMessageGrade.Warn, null, description + "承包方调查记事未填写!");
                    }
                    else
                    {
                        WriteDataInformation("提示:" + description + "承包方调查记事未填写!");
                    }
                }
                if (string.IsNullOrEmpty(YuLinTu.Library.Business.ToolString.ExceptSpaceString(expand.PublicityChroniclePerson)))
                {
                    if (showInformation)
                    {
                        this.ReportAlert(eMessageGrade.Error, null, description + "承包方公示记事人未填写!");
                    }
                    else
                    {
                        WriteDataInformation("错误:" + description + "承包方公示记事人未填写!");
                    }
                    canExport = false;
                }
                if (string.IsNullOrEmpty(YuLinTu.Library.Business.ToolString.ExceptSpaceString(expand.PublicityChronicle)))
                {
                    if (showInformation)
                    {
                        this.ReportAlert(eMessageGrade.Warn, null, description + "承包方公示记事未填写!");
                    }
                    else
                    {
                        WriteDataInformation("提示:" + description + "承包方公示记事未填写!");
                    }
                }
                if (string.IsNullOrEmpty(YuLinTu.Library.Business.ToolString.ExceptSpaceString(expand.PublicityCheckPerson)))
                {
                    if (showInformation)
                    {
                        this.ReportAlert(eMessageGrade.Error, null, description + "承包方公示审核人未填写!");
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
                    this.ReportAlert(eMessageGrade.Warn, null, description + "承包方调查记事未填写!");
                    this.ReportAlert(eMessageGrade.Error, null, description + "承包方公示记事人未填写!");
                    this.ReportAlert(eMessageGrade.Warn, null, description + "承包方公示记事未填写!");
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
            var persons = SortSharePerson(vp);
            foreach (var person in persons)
            {
                if (person.Name == vp.Name && person.ICN == vp.Number && string.IsNullOrEmpty(YuLinTu.Library.Business.ToolString.ExceptSpaceString(person.Relationship)))
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
                if (string.IsNullOrEmpty(YuLinTu.Library.Business.ToolString.ExceptSpaceString(person.ICN)))
                {
                    if (showInformation)
                    {
                        this.ReportAlert(eMessageGrade.Error, null, description + "家庭成员:" + person.Name.TrimEnd(' ') + "证件号码未填写!");
                    }
                    else
                    {
                        WriteDataInformation("错误:" + description + "家庭成员:" + person.Name.TrimEnd(' ') + "证件号码未填写!");
                    }
                    canExport = false;
                }
                if (person.CardType == eCredentialsType.IdentifyCard && !string.IsNullOrEmpty(person.ICN))
                {
                    bool isRight = YuLinTu.Library.Business.ToolICN.Check(person.ICN);
                    if (!isRight)
                    {
                        if (showInformation)
                        {
                            this.ReportAlert(eMessageGrade.Warn, null, description + "家庭成员:" + person.Name.TrimEnd(' ') + "证件号码:" + person.ICN + "不符合身份证算法验证规范!");
                        }
                        else
                        {
                            WriteDataInformation("错误:" + description + "家庭成员:" + person.Name.TrimEnd(' ') + "证件号码:" + person.ICN + "不符合身份证算法验证规范!");
                        }
                    }
                }
                if (person.Name != vp.Name)
                {
                    if (string.IsNullOrEmpty(YuLinTu.Library.Business.ToolString.ExceptSpaceString(person.Relationship)))
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
            return canExport;
        }

        /// <summary>
        /// 初始化家庭关系
        /// </summary>
        public virtual List<string> InitalizeAllRelation()
        {
            var list = FamilyRelationShip.AllRelation();
            return list;
        }

        /// <summary>
        /// 共有人排序
        /// </summary>
        public virtual PersonCollection SortSharePerson(VirtualPerson vp)
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
        public virtual bool ContractLandProgress(ContractLand land)
        {
            var canExport = true;
            string description = string.Format("{0}下承包方:{1}下地块编码为:{2}的地块", zoneName, land.OwnerName, land.LandNumber);
            if (string.IsNullOrEmpty(YuLinTu.Library.Business.ToolString.ExceptSpaceString(land.Name)))
            {
                if (showInformation)
                {
                    this.ReportWarn(description + "地块名称未填写!");
                }
                else
                {
                    WriteDataInformation("错误:" + description + "地块名称未填写!");
                }
                canExport = false;
            }
            string landNumber = string.IsNullOrEmpty(land.LandNumber) ? "" : land.LandNumber;
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
                    this.ReportWarn(description + "地块编码不符合农业部19位数字要求!");
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
            if (neighbors != null && neighbors.Length > 0 && string.IsNullOrEmpty(YuLinTu.Library.Business.ToolString.ExceptSpaceString(neighbors[0])))
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
            if (neighbors != null && neighbors.Length > 1 && string.IsNullOrEmpty(YuLinTu.Library.Business.ToolString.ExceptSpaceString(neighbors[1])))
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
            if (neighbors != null && neighbors.Length > 2 && string.IsNullOrEmpty(YuLinTu.Library.Business.ToolString.ExceptSpaceString(neighbors[2])))
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
            if (neighbors != null && neighbors.Length > 3 && string.IsNullOrEmpty(YuLinTu.Library.Business.ToolString.ExceptSpaceString(neighbors[3])))
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
            if (land == null || string.IsNullOrEmpty(YuLinTu.Library.Business.ToolString.ExceptSpaceString(land.LandExpand.ReferPerson)))
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
            return canExport;
        }

        /// <summary>
        /// 承包方证件号码检查
        /// </summary>
        public virtual void ContractorNubmerProgress()
        {
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
                    this.ReportAlert(eMessageGrade.Warn, null, description);
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
        /// 获取地域名称
        /// </summary>
        public virtual string GetZoneName(List<YuLinTu.Library.Entity.Zone> zones, YuLinTu.Library.Entity.Zone cZone)
        {
            if (cZone.Level >= YuLinTu.Library.Entity.eZoneLevel.Town)
                return cZone.Name;
            string name = cZone.Name;
            var pZone = zones.Find(t => t.FullCode == cZone.UpLevelCode);
            if (pZone == null)
            {
                if (zones.Count == 1)
                    return cZone.FullName;
                else
                    return name;
            }
            while (pZone != null && pZone.Level <= YuLinTu.Library.Entity.eZoneLevel.County)
            {
                name = pZone.Name + name;
                pZone = zones.Find(t => t.FullCode == pZone.UpLevelCode);
            }
            return name;
        }

        /// <summary>
        /// 是否继续
        /// </summary>
        /// <returns></returns>
        private bool CanContinue()
        {
            bool canContinue = true;
            int spotscount = VirtualPersonStation.CountByZone(currentZone.FullCode);
            if (spotscount <= 0)
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
            fileName = Application.StartupPath + @"\Template\Data.sqlite";
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
        public List<YuLinTu.Library.Entity.ContractLand> FilterLandType(List<YuLinTu.Library.Entity.VirtualPerson> allVirtualPersons,
            List<YuLinTu.Library.Entity.ContractLand> allContractLands)
        {
            List<YuLinTu.Library.Entity.ContractLand> allLands = new List<ContractLand>();
            allContractLands.ForEach(t => allLands.Add(t));
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
            return allLands;
        }

        #endregion 数据检查

        #region 数据转换

        /// <summary>
        /// 初始化地域代码
        /// </summary>
        public string InitalizeZoneCode(YuLinTu.Library.Entity.Zone zone)
        {
            string zoneCode = zone.FullCode;
            return zoneCode.PadRight(14, '0');
        }

        /// <summary>
        /// 获得承包方编码
        /// </summary>
        public string InitalizeContractorCode(VirtualPerson contractor, YuLinTu.Library.Entity.Zone zone)
        {
            if (contractor == null || zone == null)
            {
                return "";
            }
            int number = 0;
            Int32.TryParse(contractor.FamilyNumber, out number);
            string familyNumber = zone.FullCode.PadRight(14, '0') + contractor.FamilyNumber.PadLeft(4, '0');
            return familyNumber;
        }

        /// <summary>
        /// 获得发包方编码
        /// </summary>
        public string InitalizeSenderCode(CollectivityTissue tissue, YuLinTu.Library.Entity.Zone zone)
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
        public FBF InitalizeSenderData(CollectivityTissue tissue)
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
            fbf.FZRZJHM = tissue.LawyerCartNumber.IsNullOrEmpty() ? " " : tissue.LawyerCartNumber;
            fbf.LXDH = tissue.LawyerTelephone;
            fbf.FBFDZ = tissue.LawyerAddress;
            fbf.YZBM = tissue.LawyerPosterNumber;
            fbf.FBFDCY = tissue.SurveyPerson.IsNullOrEmpty() ? " " : tissue.SurveyPerson;
            if (tissue.SurveyDate != null && tissue.SurveyDate.HasValue)
            {
                fbf.FBFDCRQ = tissue.SurveyDate.Value;
            }
            else
            {
                fbf.FBFDCRQ = DateTime.MinValue;
            }
            fbf.FBFDCJS = tissue.SurveyChronicle.IsNullOrEmpty() ? " " : tissue.SurveyChronicle;
            return fbf;
        }

        /// <summary>
        /// 获得承包方实体
        /// </summary>
        public CBF InitalizeContractorData(VirtualPerson vp, string familyNumber)
        {
            if (vp == null)
            {
                return null;
            }
            string cbfzjlx = "9";
            if (DictList != null)
            {
                var dic = DictList.Find(d => d.GroupCode == DictionaryTypeInfo.ZJLX && d.Code == ((int)(vp.CardType)).ToString());
                cbfzjlx = dic != null ? dic.Code : "9";
            }
            CBF cbf = new CBF();
            cbf.CBFBM = familyNumber;
            cbf.CBFLX = vp.FamilyExpand != null ? ((int)(vp.FamilyExpand.ContractorType)).ToString() : "1";
            cbf.CBFMC = InitalizeFamilyName(vp.Name);
            cbf.CBFZJLX = cbfzjlx;
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
                var expand = vp.FamilyExpand;
                if (expand.SurveyDate != null && expand.SurveyDate.HasValue)
                {
                    cbf.CBFDCRQ = expand.SurveyDate;
                }
                else
                {
                    cbf.CBFDCRQ = DateTime.MinValue;
                }
                cbf.CBFDCY = expand.SurveyPerson.IsNullOrEmpty() ? " " : expand.SurveyPerson;
                cbf.CBFDCJS = expand.SurveyChronicle.IsNullOrEmpty() ? " " : expand.SurveyChronicle;
                cbf.GSJS = expand.PublicityChronicle.IsNullOrEmpty() ? " " : expand.PublicityChronicle;
                cbf.GSJSR = expand.PublicityChroniclePerson.IsNullOrEmpty() ? " " : expand.PublicityChroniclePerson;
                if (vp.FamilyExpand.CheckDate != null && expand.CheckDate.HasValue)
                {
                    cbf.GSSHRQ = expand.CheckDate.Value;
                }
                else
                {
                    cbf.GSSHRQ = DateTime.MinValue;
                }
                if (cbf.GSSHRQ == null && cbf.CBFDCRQ != null)
                    cbf.GSSHRQ = cbf.CBFDCRQ;
                cbf.GSSHR = expand.PublicityCheckPerson.IsNullOrEmpty() ? " " : expand.PublicityCheckPerson;
            }
            return cbf;
        }

        /// <summary>
        /// 初始化共有人信息
        /// </summary>
        /// <param name="vp"></param>
        /// <param name="familyNumber"></param>
        /// <param name="exportAll">导出非农户的家庭成员</param>
        public List<CBF_JTCY> InitalizeSharePersonData(VirtualPerson vp, string familyNumber, bool exportAll = true)
        {
            var persons = new List<CBF_JTCY>();
            if (vp == null)
            {
                return persons;
            }
            if (!exportAll && vp.FamilyExpand.ContractorType != eContractorType.Farmer)
            {
                return persons;
            }
            List<Person> fsp = vp.SharePersonList;
            foreach (Person person in fsp)
            {
                if (person.Name == vp.Name)
                {
                    person.Name = InitalizeFamilyName(person.Name);
                    if (person.Relationship != "户主" && person.Relationship != "本人")
                        person.Relationship = "户主";
                }
                CBF_JTCY jtcy = new CBF_JTCY();
                jtcy.CBFBM = familyNumber;
                jtcy.CYXM = person.Name;
                jtcy.CYXB = person.Gender == eGender.Male ? "1" : "2";
                jtcy.CYZJLX = ((int)(person.CardType)).ToString();
                jtcy.CYZJHM = person.ICN;
                jtcy.CYBZSM = person.Comment;
                jtcy.YHZGX = RelationShipMapping.NameMapping(person.Relationship);
                jtcy.CYBZ = person.Comment.IsNullOrEmpty() ? "9" : InitalizeSharePersonComment(person.Comment);
                if (jtcy.CYBZ == " ")
                {
                    jtcy.CYBZ = "9";
                }
                if (person.Name == vp.Name && person.ICN == vp.Number)
                {
                    jtcy.SFGYR = "1";
                }
                else
                {
                    jtcy.SFGYR = person.IsSharedLand == "是" ? "1" : "2";
                }
                //if (jtcy.CYBZ == "9" && person.Comment.IsNullOrEmpty())
                //{
                //    jtcy.CYBZSM = "其他备注";
                //}/*去除掉赋值*/
                persons.Add(jtcy);
            }
            return persons;
        }

        /// <summary>
        /// 获得 成员备注
        /// </summary>
        public string InitalizeSharePersonComment(string comment)
        {
            string value = "9";
            if (comment.Contains("外嫁女") || comment.Contains("外嫁"))
            {
                value = "1";
            }
            if (comment.Contains("入赘男") || comment.Contains("入赘"))
            {
                value = "2";
            }
            if (comment.Contains("在校大学生") || comment.Contains("在校学生"))
            {
                value = "3";
            }
            if (comment.Contains("国家公职人员") || comment.Contains("国家公职"))
            {
                value = "4";
            }
            if (comment.Contains("军人（军官、士兵）") || comment.Contains("军人") ||
                comment.Contains("军官") || comment.Contains("士兵"))
            {
                value = "5";
            }
            if (comment.Contains("新生儿"))
            {
                value = "6";
            }
            if (comment.Contains("去世") || comment.Contains("去世"))
            {
                value = "7";
            }

            return value;
        }

        /// <summary>
        /// 获得承包地块实体-确权
        /// </summary>
        public CBDKXX InitalizeAgricultureLandData(ContractLand land, string senderCode, string familyNumber)
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
            if (CBDKXXAwareAreaExportSet == CbdkxxAwareAreaExportEnum.实测面积)
            {
                cbdkxx.HTMJM = Math.Round(land.ActualArea, 2);
                if (land.LandExpand.MeasureArea > 0)
                {
                    cbdkxx.HTMJ = land.LandExpand.MeasureArea;
                }
                else
                {
                    cbdkxx.HTMJ = Math.Round(cbdkxx.HTMJM.Value / 0.0015, 2);
                }
            }
            else if (CBDKXXAwareAreaExportSet == CbdkxxAwareAreaExportEnum.确权面积)
            {
                cbdkxx.HTMJM = Math.Round(land.AwareArea, 2);
                if (land.LandExpand.MeasureArea > 0)
                {
                    cbdkxx.HTMJ = land.LandExpand.MeasureArea;
                }
                else
                {
                    cbdkxx.HTMJ = Math.Round(cbdkxx.HTMJM.Value / 0.0015, 2);
                }
            }
            cbdkxx.YHTMJM = Math.Round((land.TableArea != null ? land.TableArea.Value : 0), 2);
            cbdkxx.YHTMJ = Math.Round(cbdkxx.YHTMJM.Value / 0.0015, 2);
            cbdkxx.CBJYQQDFS = land.ConstructMode == null ? "110" : land.ConstructMode;
            cbdkxx.SFQQQG = "2";
            return cbdkxx;
        }

        /// <summary>
        /// 获得承包地块实体-确股
        /// </summary>
        public CBDKXX InitalizeQgAgricultureLandData(ContractLand land, BelongRelation qgLand, string senderCode, string familyNumber)
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
            cbdkxx.HTMJ = Math.Round(qgLand.QuanficationArea / 0.0015, 2);
            cbdkxx.HTMJM = Math.Round(qgLand.QuanficationArea, 2);
            cbdkxx.YHTMJM = Math.Round(qgLand.TableArea, 2);
            cbdkxx.YHTMJ = Math.Round(qgLand.TableArea / 0.0015, 2);
            cbdkxx.CBJYQQDFS = land.ConstructMode == null ? "110" : land.ConstructMode;
            cbdkxx.SFQQQG = "1";
            return cbdkxx;
        }

        /// <summary>
        /// 初始化空间地块-DKEX
        /// </summary>
        public DKEX InitalizeSpaceLandData(ContractLand land)//, List<YuLinTu.Library.Entity.BuildLandBoundaryAddressDot> pointList, List<YuLinTu.Library.Entity.BuildLandBoundaryAddressCoil> lineList)
        {
            if (land == null)
            {
                return null;
            }
            DKEX dk = new DKEX();
            dk.YSDM = eFeatureType.DK;
            //dk.DKBM = ContractLand.GetLandNumber(land.CadastralNumber);
            dk.DKBM = land.LandNumber;
            dk.DKMC = land.Name;
            dk.SYQXZ = land.OwnRightType;
            dk.DKLB = land.LandCategory;
            dk.TDLYLX = land.LandCode;
            dk.DLDJ = land.LandLevel;
            if (dk.DLDJ != null && dk.DLDJ.Length > 2)
                dk.DLDJ = dk.DLDJ.Substring(0, 2);

            dk.TDYT = land.Purpose;
            if (land.IsFarmerLand != null && land.IsFarmerLand.HasValue)
            {
                dk.SFJBNT = land.IsFarmerLand.Value ? "1" : "2";
            }
            else
            {
                dk.SFJBNT = "0";
            }
            dk.SCMJ = Math.Round(land.ActualArea / 0.0015, 2);
            dk.SCMJM = Math.Round(land.ActualArea, 2);
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
            dk.Shape = land.Shape == null ? null : land.Shape;
            dk.KJZB = "";

            return dk;
        }

        /// <summary>
        /// 承包合同实体
        /// </summary>
        public CBHT InitalizeConcordData(ContractConcord concord, int concordlandsCount)
        {
            if (concord == null)
            {
                return null;
            }
            CBHT cbht = new CBHT();
            cbht.CBDKZS = concordlandsCount;
            cbht.CBHTBM = concord.ConcordNumber;
            cbht.CBFS = concord.ArableLandType;
            if (cbht.CBFS == "家庭承包")
            {
                cbht.CBFS = "110";
            }
            cbht.CBQXQ = concord.ArableLandStartTime.Value.Date;
            if (concord.ManagementTime == "长久")
            {
                cbht.CBQXZ = new DateTime(9999, 01, 01);
            }
            else
            {
                cbht.CBQXZ = concord.ArableLandEndTime.Value.Date;
            }
            if (CBDKXXAwareAreaExportSet == CbdkxxAwareAreaExportEnum.实测面积)
            {
                cbht.HTZMJ = Math.Round(concord.CountActualArea / 0.0015, 2);
                cbht.HTZMJM = Math.Round(concord.CountActualArea, 2);
            }
            else if (CBDKXXAwareAreaExportSet == CbdkxxAwareAreaExportEnum.确权面积)
            {
                cbht.HTZMJ = Math.Round(concord.CountAwareArea / 0.0015, 2);
                cbht.HTZMJM = Math.Round(concord.CountAwareArea, 2);
            }
            cbht.YHTZMJM = concord.TotalTableArea != null ? Math.Round(concord.TotalTableArea.Value, 2) : concord.TotalTableArea;
            cbht.YHTZMJ = cbht.YHTZMJM != null ? Math.Round(cbht.YHTZMJM.Value / 0.0015, 2) : 0;

            cbht.QDSJ = concord.ContractDate != null ? concord.ContractDate.Value.Date : DateTime.Now;
            return cbht;
        }

        /// <summary>
        /// 初始化登记簿实体
        /// </summary>
        public CBJYQZDJB InitalizeRegeditBook(ContractConcord concord, ContractRegeditBook book)
        {
            if (concord == null || book == null)
            {
                return null;
            }
            CBJYQZDJB djb = new CBJYQZDJB();
            djb.CBJYQZBM = concord.ConcordNumber;
            djb.CBFS = concord.ArableLandType;
            if (djb.CBFS == "家庭承包")
            {
                djb.CBFS = "110";
            }
            djb.CBQXQ = concord.ArableLandStartTime.Value.Date;
            djb.CBQX = concord.ManagementTime;
            if (concord.ManagementTime == "长久")
            {
                djb.CBQXZ = new DateTime(9999, 01, 01);
            }
            else
            {
                djb.CBQXZ = concord.ArableLandEndTime.Value.Date;
            }

            djb.DJBFJ = book.ContractRegeditBookExcursus.IsNullOrEmpty() ? " " : book.ContractRegeditBookExcursus;
            djb.DBR = book.ContractRegeditBookPerson.IsNullOrEmpty() ? " " : book.ContractRegeditBookPerson;
            djb.DJSJ = book.ContractRegeditBookTime == null ? DateTime.MinValue : book.ContractRegeditBookTime.Value.Date;
            return djb;
        }

        /// <summary>
        /// 初始化权证实体
        /// </summary>
        public CBJYQZ InitalizeWarrantBook(ContractRegeditBook regeditBook, VirtualPerson vp)
        {
            if (regeditBook == null)
            {
                return null;
            }
            CBJYQZ qz = new CBJYQZ();
            qz.CBJYQZBM = regeditBook.RegeditNumber;
            qz.FZJG = regeditBook.SendOrganization;
            qz.FZRQ = regeditBook.SendDate.Date;
            qz.QZSFLQ = regeditBook.RegeditBookGetted == null ? "1" : regeditBook.RegeditBookGetted;//国标为1，领取  2 未领取
            qz.QZLQRQ = (regeditBook.PrintDate.Date >= regeditBook.SendDate.Date) ? regeditBook.PrintDate.Date : regeditBook.SendDate.Date;
            qz.QZLQRXM = vp.Name;
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
        public void InitalizeAccessory(ContractRegeditBook regeditBook, ComplexRightEntity entity)
        {
            if (!ContainMatrical || regeditBook == null || string.IsNullOrEmpty(regeditBook.RegeditNumber))
            {
                return;
            }
            QSLYZLFJ attch = new QSLYZLFJ();
            attch.CBJYQZBM = regeditBook.RegeditNumber;
            attch.ZLFJMC = entity.CBF.CBFMC;
            attch.ZLFJBH = entity.CBF.CBFBM;
            attch.ZLFJRQ = regeditBook.PrintDate;
            attch.FJ = @"其他资料\权属来源附件\" + regeditBook.RegeditNumber + "CBHT2_1.jpg";
            if (entity.FJ == null)
            {
                entity.FJ = new List<QSLYZLFJ>();
            }
            entity.FJ.Add(attch);
        }

        /// <summary>
        /// 初始化县级行政区
        /// </summary>
        public List<XJXZQ> InitalizeCountyData(List<Quality.Business.Entity.Zone> zones)
        {
            List<XJXZQ> countys = new List<XJXZQ>();
            foreach (var zone in zones)
            {
                if (zone.Level != Quality.Business.Entity.eZoneLevel.County)
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
        public List<XJQY> InitalizeTownData(List<Quality.Business.Entity.Zone> zones)
        {
            List<XJQY> towns = new List<XJQY>();
            foreach (var zone in zones)
            {
                if (zone.Level != Quality.Business.Entity.eZoneLevel.Town)
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
        public List<CJQY> InitalizeVillageData(List<Quality.Business.Entity.Zone> zones)
        {
            List<CJQY> villages = new List<CJQY>();
            foreach (var zone in zones)
            {
                if (zone.Level != Quality.Business.Entity.eZoneLevel.Village)
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
        public List<ZJQY> InitalizeGroupData(List<Quality.Business.Entity.Zone> zones)
        {
            List<ZJQY> groups = new List<ZJQY>();
            foreach (var zone in zones)
            {
                if (zone.Level != Quality.Business.Entity.eZoneLevel.Group)
                {
                    continue;
                }
                ZJQY group = new ZJQY();
                group.YSDM = FeatureCode.ZJQY;
                group.ZJQYDM = zone.FullCode;
                group.ZJQYMC = zone.Name;
                group.Shape = zone.Shape;
                groups.Add(group);
            }
            return groups;
        }

        /// <summary>
        /// 初始化点状地物
        /// </summary>
        public List<Quality.Business.Entity.DZDW> InitalizePointData()
        {
            List<YuLinTu.Library.Entity.Zone> zones = zoneStation.GetChildren(currentZone.FullCode, eLevelOption.SelfAndSubs);
            List<YuLinTu.Library.Entity.DZDW> points = new List<YuLinTu.Library.Entity.DZDW>();
            zones.ForEach(z => points.AddRange(dzdwStation.GetByZoneCode(z.FullCode)));

            List<Quality.Business.Entity.DZDW> dzdws = new List<Quality.Business.Entity.DZDW>();
            int startIndex = QuantityValue.PointThing;
            foreach (var point in points)
            {
                Quality.Business.Entity.DZDW dzdw = new Quality.Business.Entity.DZDW();
                dzdw.BSM = startIndex;
                //dzdw.YSDM = point.YSDM;
                dzdw.YSDM = "196011";
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
        public List<Quality.Business.Entity.XZDW> InitalizePolylineData()
        {
            List<YuLinTu.Library.Entity.Zone> zones = zoneStation.GetChildren(currentZone.FullCode, eLevelOption.SelfAndSubs);
            List<YuLinTu.Library.Entity.XZDW> roads = new List<YuLinTu.Library.Entity.XZDW>();
            zones.ForEach(z => roads.AddRange(xzdwStation.GetByZoneCode(z.FullCode)));

            int startIndex = QuantityValue.LineThing;
            List<Quality.Business.Entity.XZDW> xzdws = new List<Quality.Business.Entity.XZDW>();
            foreach (var road in roads)
            {
                Quality.Business.Entity.XZDW xzdw = new Quality.Business.Entity.XZDW();
                xzdw.BSM = startIndex;
                //xzdw.YSDM = road.YSDM;
                xzdw.YSDM = "196021";
                xzdw.BZ = road.Comment;
                xzdw.DWMC = road.DWMC;
                xzdw.CD = road.CD;
                xzdw.KD = road.KD;
                xzdw.Shape = road.Shape == null ? null : road.Shape.Instance;
                xzdws.Add(xzdw);
                startIndex++;
            }

            roads = null;
            //waters = null;
            return xzdws;
        }

        /// <summary>
        /// 初始化面状地物
        /// </summary>
        public List<Quality.Business.Entity.MZDW> InitalizePolygonData()
        {
            List<YuLinTu.Library.Entity.Zone> zones = zoneStation.GetChildren(currentZone.FullCode, eLevelOption.SelfAndSubs);
            List<YuLinTu.Library.Entity.MZDW> spots = new List<YuLinTu.Library.Entity.MZDW>();
            zones.ForEach(z => spots.AddRange(mzdwStation.GetByZoneCode(z.FullCode)));

            List<Quality.Business.Entity.MZDW> mzdws = new List<Quality.Business.Entity.MZDW>();
            int startIndex = QuantityValue.AreaThing;
            foreach (var spot in spots)
            {
                Quality.Business.Entity.MZDW mzdw = new Quality.Business.Entity.MZDW();
                mzdw.BSM = startIndex;
                //mzdw.YSDM = spot.YSDM;
                mzdw.YSDM = "196031";
                mzdw.BZ = spot.Comment;
                mzdw.DWMC = spot.DWMC;
                mzdw.MJ = spot.Area;
                mzdw.MJM = Math.Round(spot.Area * 0.0015, 2);
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
        public List<KZD> InitalizeControlPoint()
        {
            List<YuLinTu.Library.Entity.Zone> zones = zoneStation.GetChildren(currentZone.FullCode, eLevelOption.SelfAndSubs);
            List<YuLinTu.Library.Entity.ControlPoint> points = new List<YuLinTu.Library.Entity.ControlPoint>();
            zones.ForEach(z => points.AddRange(kzdStation.GetByZoneCode(z.FullCode)));
            int startIndex = QuantityValue.ControlPoint;
            List<KZD> kzds = new List<KZD>();
            foreach (var point in points)
            {
                Quality.Business.Entity.KZD kzd = new Quality.Business.Entity.KZD();
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
            return kzds;
        }

        /// <summary>
        /// 初始化区域界线数据
        /// </summary>
        public List<QYJX> InitalizeRegionData()
        {
            List<YuLinTu.Library.Entity.Zone> zones = zoneStation.GetChildren(currentZone.FullCode, eLevelOption.SelfAndSubs);
            List<YuLinTu.Library.Entity.ZoneBoundary> points = new List<YuLinTu.Library.Entity.ZoneBoundary>();
            zones.ForEach(z => points.AddRange(qyjxStation.GetByZoneCode(z.FullCode)));
            int startIndex = QuantityValue.AreaLine;
            List<QYJX> regions = new List<QYJX>();
            foreach (var point in points)
            {
                Quality.Business.Entity.QYJX qyjx = new Quality.Business.Entity.QYJX();
                qyjx.BSM = startIndex;
                qyjx.YSDM = point.FeatureCode;
                qyjx.JXLX = point.BoundaryLineType;
                qyjx.JXXZ = point.BoundaryLineNature;

                qyjx.Shape = point.Shape == null ? null : point.Shape.Instance;
                regions.Add(qyjx);
                startIndex++;
            }
            points = null;
            return regions;
        }

        /// <summary>
        /// 初始化基本农田保护区数据
        /// </summary>
        public List<JBNTBHQ> InitalizeFarmerRegionData()
        {
            List<YuLinTu.Library.Entity.Zone> zones = zoneStation.GetChildren(currentZone.FullCode, eLevelOption.SelfAndSubs);
            List<YuLinTu.Library.Entity.FarmLandConserve> points = new List<YuLinTu.Library.Entity.FarmLandConserve>();
            zones.ForEach(z => points.AddRange(jbntbhqStation.GetByZoneCode(z.FullCode)));
            int startIndex = QuantityValue.ProtectArea;
            List<JBNTBHQ> farmers = new List<JBNTBHQ>();
            foreach (var point in points)
            {
                Quality.Business.Entity.JBNTBHQ jbnt = new Quality.Business.Entity.JBNTBHQ();
                jbnt.BSM = startIndex;
                jbnt.YSDM = point.FeatureCode;
                jbnt.BHQBH = point.ConserveNumber;
                jbnt.JBNTMJ = point.FarmLandArea;
                jbnt.JBNTMJM = point.FarmLandArea != null ? Math.Round(point.FarmLandArea.Value * 0.0015, 2) : point.FarmLandArea;
                jbnt.Shape = point.Shape == null ? null : point.Shape.Instance;
                farmers.Add(jbnt);
                startIndex++;
            }
            points = null;
            return farmers;
        }

        /// <summary>
        /// 点
        /// </summary>
        public class Point
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
        public class Line
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

        #endregion 数据转换

        #region 数据导出

        /// <summary>
        /// 初始化农业部目录
        /// </summary>
        public virtual bool InitalizeAgricultureDirectory(DataExportProgress exportProgress, ArcSpaceDataProgress spaceProgress, ExportFileEntity efe = null)
        {
            exportProgress.IsExportScan = false;//2016-09-23 扫描资料不导出 ContainMatrical;
            county = zoneStation.Get(currentZone.FullCode.Substring(0, 6));
            if (county == null)
            {
                this.ReportAlert(eMessageGrade.Error, null, "行政区域级别不能大于区县级!");
                return false;
            }
            if (efe == null)
                efe = new ExportFileEntity();
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
                InitalizeSpatialCoordianteSystem(spaceProgress);
            }
            catch (SystemException ex)
            {
                this.ReportAlert(eMessageGrade.Error, null, ex.ToString());
                return false;
            }
            return true;
        }

        /// <summary>
        /// 初始化空间坐标系
        /// </summary>
        public void InitalizeSpatialCoordianteSystem(ArcSpaceDataProgress spaceProgress)
        {
            if (spaceProgress == null)
            {
                return;
            }
            var targetSpatialReference = DbContext.CreateSchema().GetElementSpatialReference(
                ObjectContext.Create(typeof(YuLinTu.Library.Entity.Zone)).Schema,
                ObjectContext.Create(typeof(YuLinTu.Library.Entity.Zone)).TableName);

            var info = targetSpatialReference.ToEsriString();
            spaceProgress.SpatialText = info;
        }

        /// <summary>
        /// 导出元数据文件
        /// </summary>
        public void ExportMataFile(ArcSpaceDataProgress spaceProgress)
        {
            VictorData data = new VictorData();
            data.InitiallClass();
            data.Data_Info.Title = county.Name + "矢量元数据";
            data.Data_Info.GeoID = county.FullCode;
            data.Data_Info.Ending = DateTime.Now.ToString("yyyyMMdd");
            data.Data_Info.RpOrgName = UnitName;
            data.Data_Info.RpCnt = LinkMan;
            data.Data_Info.VoiceNum = Telephone;
            data.Data_Info.CntAddress = Address;
            data.Data_Info.CntCode = PosterNumber;
            data.Data_Info.IdAbs = "";
            data.Cont_Info.CatFetTyps = "数据集要素类型";
            data.Cont_Info.AttrTypList = "与数据集要素类名称对应的主要属性列表";
            spaceProgress.ExportMeataData(spaceProgress.ShapeFilePath, "SL" + county.FullCode + DateTime.Now.Year.ToString(), data);
        }

        /// <summary>
        /// 地域数据处理
        /// </summary>
        public void ZoneSpaceDataExport(ArcSpaceDataProgress spaceProgress, List<YuLinTu.Library.Entity.Zone> zoneList)
        {
            var zones = ZoneMapping(zoneList);
            TableDataExportProgress(zones);
            spaceProgress.XAJXZQS = InitalizeCountyData(zones.FindAll(ze => ze.Level == Quality.Business.Entity.eZoneLevel.County));
            spaceProgress.XGJXZQS = InitalizeTownData(zones.FindAll(ze => ze.Level == Quality.Business.Entity.eZoneLevel.Town));
            spaceProgress.CJXZQS = InitalizeVillageData(zones.FindAll(ze => ze.Level == Quality.Business.Entity.eZoneLevel.Village));
            spaceProgress.ZJXZQS = InitalizeGroupData(zones.FindAll(ze => ze.Level == Quality.Business.Entity.eZoneLevel.Group));
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
        /// 导出权属单位代码表
        /// </summary>
        public bool TableDataExportProgress(List<Quality.Business.Entity.Zone> zones)
        {
            try
            {
                var list = new List<Quality.Business.Entity.Zone>();
                var tissues = new List<CollectivityTissue>();

                if (currentZone.Level < YuLinTu.Library.Entity.eZoneLevel.County)
                {
                    var partents = zoneStation.GetParentsToProvince(currentZone);
                    var listzone = zoneStation.GetAllZonesToProvince(currentZone);
                    partents.RemoveAll(t => t.Level > Library.Entity.eZoneLevel.County);
                    var entityParentsZones = partents.OrderByDescending(t => t.Level).ToList();
                    var entityZones = listzone.OrderByDescending(t => t.Level).ToList();
                    foreach (var item in entityZones)
                    {
                        var tissueStation = DbContext.CreateCollectivityTissueWorkStation();
                        var res = tissueStation.GetTissues(item.FullCode);
                        res.ForEach(x => { tissues.Add(x); });
                    }

                    foreach (var z in entityParentsZones)
                    {
                        Quality.Business.Entity.Zone exZone = new Quality.Business.Entity.Zone();
                        exZone.ID = z.ID;
                        exZone.Code = z.Code;
                        exZone.Comment = z.Comment;
                        exZone.CreateTime = z.CreateTime;
                        exZone.CreateUser = z.CreateUser;
                        exZone.FullCode = z.FullCode;
                        exZone.FullName = z.FullName;
                        exZone.Level = (Quality.Business.Entity.eZoneLevel)((int)(z.Level));
                        exZone.Name = z.Name;
                        exZone.UpLevelCode = z.UpLevelCode;
                        exZone.UpLevelName = z.UpLevelName;
                        exZone.Shape = z.Shape == null ? null : z.Shape.Instance;
                        list.Add(exZone);
                    }
                    list.AddRange(zones);
                }
                else
                    list = zones;
                var table = new ExportUnitTable();
                table.ZoneList = list;
                table.Tissues = tissues;
                table.Argument = Argument;
                table.PreviewName = county.FullCode + DateTime.Now.Year.ToString();
                table.FilePath = Folder + @"\" + county.FullCode + county.Name + @"\权属数据";
                table.ExportData();
                return true;
            }
            catch (Exception ex)
            {
                LogWrite.WriteErrorLog("导出权属单位代码表:", ex);
                return false;
            }
        }

        /// <summary>
        /// 地域数据映射
        /// </summary>
        public List<Quality.Business.Entity.Zone> ZoneMapping(List<YuLinTu.Library.Entity.Zone> zones)
        {
            List<Quality.Business.Entity.Zone> zoneCollection = new List<Quality.Business.Entity.Zone>();
            // ArcGisZoneCollection arcZones = gisInstance.ArcGisZone.GetByZoneCode(county.FullCode, Library.Data.ConditionOption.Like_LeftFixed);
            List<YuLinTu.Library.Entity.Zone> arcZones = zoneStation.GetChildren(currentZone.FullCode, eLevelOption.SelfAndSubs);

            foreach (var zone in zones)
            {
                Quality.Business.Entity.Zone exZone = new Quality.Business.Entity.Zone();
                exZone.ID = zone.ID;
                exZone.Code = zone.Code;
                exZone.Comment = zone.Comment;
                exZone.CreateTime = zone.CreateTime;
                exZone.CreateUser = zone.CreateUser;
                exZone.FullCode = zone.FullCode;
                exZone.FullName = zone.FullName;
                exZone.Level = (Quality.Business.Entity.eZoneLevel)((int)(zone.Level));
                exZone.Name = zone.Name;
                exZone.UpLevelCode = zone.UpLevelCode;
                exZone.UpLevelName = zone.UpLevelName;
                exZone.Shape = zone.Shape == null ? null : zone.Shape.Instance;
                zoneCollection.Add(exZone);
            }
            arcZones = null;
            GC.Collect();
            return zoneCollection;
        }

        #endregion 数据导出

        #region Helper

        /// <summary>
        /// 初始化错误记录文件目录
        /// </summary>
        public void InitalizeDirectory()
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
        public void WriteDataInformation(string message)
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

        /// <summary>
        /// 获取完整的流水编号，如  川(2016)通川区农村土地承包经营权证第000001号
        /// </summary>
        /// <returns></returns>
        public string GetFullSerialNumberName(YuLinTu.Library.Entity.Zone CurrentZone, string qznh, string serialNumber)
        {
            string fullSerialNumberName = "";

            var sqzoneName = InitalizeZoneName(CurrentZone, YuLinTu.Library.Entity.Zone.ZONE_COUNTY_LENGTH);
            var zonename = InitalizeZoneName(CurrentZone, YuLinTu.Library.Entity.Zone.ZONE_PROVICE_LENGTH);
            var simpleProvinceNamesDics = InitalizeSimpleProvice();
            var simplenamedic = simpleProvinceNamesDics.Where(s => s.Key == zonename).FirstOrDefault();
            var simplename = simplenamedic.Value != null ? simplenamedic.Value : "";
            string useserialNumber = serialNumber != null ? serialNumber : "      ";
            if (serialNumber != null && serialNumber.Length != 6)
            {
                useserialNumber = serialNumber.PadLeft(6, '0');
            }
            sqzoneName = sqzoneName.IsNullOrEmpty() ? " " : sqzoneName;
            qznh = qznh.IsNullOrEmpty() ? "    " : qznh;
            simplename = simplename.IsNullOrEmpty() ? " " : simplename;

            fullSerialNumberName = simplename + "(" + qznh + ")" + sqzoneName + "农村土地承包经营权证第" + useserialNumber + "号";

            return fullSerialNumberName;
        }

        /// <summary>
        /// 设置流水号
        /// </summary>
        /// <returns></returns>
        public string SetFullSerialNumberName(string serNumberTemp, string qznh, string serialNumber)
        {
            string useserialNumber = serialNumber != null ? serialNumber : "      ";
            if (serialNumber != null && serialNumber.Length != 6)
            {
                useserialNumber = serialNumber.PadLeft(6, '0');
            }
            qznh = qznh.IsNullOrEmpty() ? "    " : qznh;
            var fullSerialNumberName = string.Format(serNumberTemp, qznh, useserialNumber);
            return fullSerialNumberName;
        }

        /// <summary>
        /// 获取完整的流水编号模板，如  川(2016)通川区农村土地承包经营权证第000001号
        /// </summary>
        /// <returns></returns>
        public string GetFullSerialNumberTemp(YuLinTu.Library.Entity.Zone CurrentZone)
        {
            string fullSerialNumberName = "";

            var sqzoneName = InitalizeZoneName(CurrentZone, YuLinTu.Library.Entity.Zone.ZONE_COUNTY_LENGTH);
            var zonename = InitalizeZoneName(CurrentZone, YuLinTu.Library.Entity.Zone.ZONE_PROVICE_LENGTH);
            var simpleProvinceNamesDics = InitalizeSimpleProvice();
            var simplenamedic = simpleProvinceNamesDics.Where(s => s.Key == zonename).FirstOrDefault();
            var simplename = simplenamedic.Value != null ? simplenamedic.Value : "";
            sqzoneName = sqzoneName.IsNullOrEmpty() ? " " : sqzoneName;
            simplename = simplename.IsNullOrEmpty() ? " " : simplename;

            fullSerialNumberName = simplename + "({0})" + sqzoneName + "农村土地承包经营权证第{1}号";

            return fullSerialNumberName;
        }


        /// <summary>
        /// 初始化省市简写
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, string> InitalizeSimpleProvice()
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic.Add("北京市", "京");
            dic.Add("天津市", "津");
            dic.Add("河北省", "冀");
            dic.Add("山西省", "晋");
            dic.Add("内蒙古自治区", "内蒙古");
            dic.Add("辽宁省", "辽");
            dic.Add("吉林省", "吉");
            dic.Add("黑龙江省", "黑");
            dic.Add("上海市", "沪");
            dic.Add("江苏省", "苏");
            dic.Add("浙江省", "浙");
            dic.Add("安徽省", "皖");
            dic.Add("福建省", "闽");
            dic.Add("江西省", "赣");
            dic.Add("山东省", "鲁");
            dic.Add("河南省", "豫");
            dic.Add("湖北省", "鄂");
            dic.Add("湖南省", "湘");
            dic.Add("广东省", "粤");
            dic.Add("广西壮族自治区", "桂");
            dic.Add("海南省", "琼");
            dic.Add("重庆市", "渝");
            dic.Add("四川省", "川");
            dic.Add("贵州省", "贵");
            dic.Add("云南省", "云");
            dic.Add("西藏自治区", "藏");
            dic.Add("陕西省", "陕");
            dic.Add("甘肃省", "甘");
            dic.Add("青海省", "青");
            dic.Add("宁夏回族自治区", "宁");
            dic.Add("新疆维吾尔自治区", "新");
            dic.Add("香港特别行政区", "港");
            dic.Add("澳门特别行政区", "澳");
            dic.Add("台湾省", "台");
            return dic;
        }

        /// <summary>
        /// 初始化地域名称
        /// </summary>
        public string InitalizeZoneName(YuLinTu.Library.Entity.Zone CurrentZone, int length)
        {
            var ZoneList = GetParentZone(CurrentZone, DbContext);
            string zoneName = string.Empty;
            if (ZoneList == null || ZoneList.Count == 0 || CurrentZone.FullCode.Length < length)
            {
                return zoneName;
            }
            string code = CurrentZone.FullCode.Substring(0, length);
            YuLinTu.Library.Entity.Zone zone = ZoneList.Find(t => t.FullCode == code);
            if (zone != null)
            {
                zoneName = zone.Name;
            }
            return zoneName;
        }

        /// <summary>
        /// 获取地域集合
        /// </summary>
        public List<YuLinTu.Library.Entity.Zone> GetParentZone(YuLinTu.Library.Entity.Zone zone, IDbContext dbContext)
        {
            ModuleMsgArgs arg = new ModuleMsgArgs();
            arg.Datasource = dbContext;
            arg.Parameter = zone;
            arg.Name = ZoneMessage.ZONE_PARENTS_TOPROVINCEZONE;
            TheBns.Current.Message.Send(this, arg);
            return (arg.ReturnValue as List<YuLinTu.Library.Entity.Zone>);
        }

        /// <summary>
        /// 初始化户主名称
        /// </summary>
        /// <param name="familyName"></param>
        /// <returns></returns>
        public string InitalizeFamilyName(string familyName)
        {
            if (string.IsNullOrEmpty(familyName))
            {
                return "/";
            }
            if (!KeepRepeatFlag)
            {
                return familyName;
            }
            string number = Library.Business.ToolString.GetAllNumberWithInString(familyName);
            if (!string.IsNullOrEmpty(number))
            {
                return familyName.Replace(number, "");
            }
            int index = familyName.IndexOf("(");
            if (index > 0)
            {
                return familyName.Substring(0, index);
            }
            index = familyName.IndexOf("（");
            if (index > 0)
            {
                return familyName.Substring(0, index);
            }
            return familyName;
        }

        #endregion Helper
    }
}