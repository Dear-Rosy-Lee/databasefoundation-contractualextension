/*
 * (C) 2024 鱼鳞图公司版权所有,保留所有权利
*/

using NetTopologySuite.Geometries;
using NetTopologySuite.IO;
using Quality.Business.Entity;
using Quality.Business.TaskBasic;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Windows.Documents;
using YuLinTu.Component.Common;
using YuLinTu.Data;
using YuLinTu.Data.SQLite;
using YuLinTu.Library.Business;
using YuLinTu.Spatial;
using ZoneDto = YuLinTu.Library.Entity.Zone;

namespace YuLinTu.Component.ResultDbToLocalDb
{
    /// <summary>
    /// 获取数据
    /// </summary>
    public partial class DataImportProgress : Task
    {
        #region Fields

        /// <summary>
        /// 无承包方的地块编码集合
        /// </summary>
        private List<string> landCodeSet;

        /// <summary>
        /// 拓展名称
        /// </summary>
        private string extentName;

        /// <summary>
        /// 文件路径
        /// </summary>
        private FilePathInfo currentPath;

        /// <summary>
        /// 数据库
        /// </summary>
        private IDbContext db;

        /// <summary>
        /// 图形SRID
        /// </summary>
        private int srid;

        /// <summary>
        /// 检查模块信息提示
        /// </summary>
        public delegate void QualityModuleAlertDelegate(TaskAlertEventArgs e);

        private bool hasError = false;

        #endregion Fields

        #region Propertys

        /// <summary>
        /// 扩展名称
        /// </summary>
        public string ExtentName
        {
            get { return extentName; }
            set { extentName = value; }
        }

        /// <summary>
        /// 数据总数
        /// </summary>
        public int DataCount { get; set; }

        /// <summary>
        /// 数据库路径
        /// </summary>
        public string FilePath { get; set; }

        /// <summary>
        /// 服务
        /// </summary>
        public IDbContext LocalService { get; set; }

        /// <summary>
        /// 是否检查数据
        /// </summary>
        public bool NeedCheck { get; set; }

        /// <summary>
        /// 容差值
        /// </summary>
        public string VolumeValue { get; set; }

        /// <summary>
        /// 质量模块提示
        /// </summary>
        public QualityModuleAlertDelegate QualityAlert { get; set; }

        /// <summary>
        /// 选择地域
        /// </summary>
        public FileZoneEntity FileZone { get; set; }

        /// <summary>
        /// 是否使用14位标准地域编码
        /// </summary>
        public bool IsStandCode { get; set; }

        public bool GenerateCoilDot { get; set; }

        public bool CreatUnit { get; set; }

        /// <summary>
        /// 自动创建数据库
        /// </summary>
        public bool CreatDataBase { get; set; }

        private Dictionary<string, Library.Entity.Dictionary> dicLandType;

        #endregion Propertys

        #region Ctor

        public DataImportProgress()
        {
            landCodeSet = new List<string>();
        }

        #endregion Ctor

        #region Methods

        /// <summary>
        /// 导入数据
        /// </summary>
        public void ImportData()
        {
            hasError = false;
            var ginfo = new GainInfo(FilePath);
            if (ginfo != null && ExtentName.IsNullOrEmpty())
            {
                extentName = ginfo.ZoneCode + ginfo.Year;
            }
            bool result = true;
            if (NeedCheck)
            {
                double volunm = 0;
                double.TryParse(VolumeValue, out volunm);

                if (!result)
                {
                    QualityAlert(new TaskAlertEventArgs(eMessageGrade.Error, null, "数据检查未通过,不能执行数据导入"));
                    return;
                }
            }
            this.ReportProgress(0, "开始导入数据任务");
            this.ReportInfomation("导入过程将根据数据量的不同，耗时几分钟到几十分钟不等。请在此过程中不要进行任何操作，耐心等待导入的完成。");
            currentPath = FileImportConfig.GetCurrent(FilePath, ginfo.ZoneCode + ginfo.Year);
            if (CreatDataBase)
            {
                SpatialReference sr = GetSridFromDK();
                CreateaDataBase(ginfo.UnitName + ginfo.ZoneCode, sr);
                DataBaseHelper.SetDefaulZone(ginfo.ZoneCode, ginfo.UnitName);
                srid = sr.WKID;
            }
            else
            {
                srid = GetSrid(LocalService);
            }
            dicLandType = new Dictionary<string, Library.Entity.Dictionary>();
            var dics = LocalService.CreateZoneWorkStation().Get<Library.Entity.Dictionary>(c => c.GroupCode == "C26" && c.Code != string.Empty && c.Code != null);
            foreach (var item in dics)
                dicLandType[item.Code] = item;

            result = true;

            result = ProecessZoneData(ginfo, LocalService);
            if (!result)
            {
                return;
            }

            bool indexCreated = false;

            try
            {
                this.ReportProgress(1, "开始删除索引...");
                this.ReportInfomation("开始删除索引...");
                if (!DeleteIndex(LocalService))
                    return;

                this.ReportProgress(1, "开始删除冗余数据...");
                this.ReportInfomation("开始删除冗余数据...");
                if (!DeleteOldData(LocalService, ginfo))
                    return;

                this.ReportProgress(2, "开始优化数据库...");
                this.ReportInfomation("开始优化数据库...");
                if (!CompressDatabase(LocalService))
                    return;

                this.ReportProgress(3, "开始导入数据...");
                this.ReportInfomation("开始导入数据...");

                var shpProcess = new SpaceDataOperator(srid, currentPath.ShapeFileList);
                shpProcess.InsertIndexToDataBase(currentPath.ShapeFileList, GenerateCoilDot);
                bool resultData = ExcuteImportData(ginfo.ZoneCode, LocalService, shpProcess);
                this.ReportProgress(95, "导入其它类型空间数据");
                bool resultSpace = ImportSpaceData(ginfo.ZoneCode, LocalService, shpProcess);//空间

                if (resultData && resultSpace)
                {
                    this.ReportInfomation("导入数据完成!");
                }
                else if (resultData && resultSpace && hasError)
                {
                    this.ReportInfomation("导入数据完成, 但有一些错误!");
                }

                this.ReportProgress(99, "开始创建索引...");
                this.ReportInfomation("开始创建索引...");
                if (!CreateIndex(LocalService))
                    return;

                indexCreated = true;
                this.ReportProgress(100);
                this.ReportInfomation("导入数据结束");
            }
            catch (Exception dd)
            {
                this.ReportException(dd);
            }
            finally
            {
                if (!indexCreated)
                    CreateIndex(LocalService);
            }
        }

        private bool CreateIndex(IDbContext localService)
        {
            try
            {
                var schema = localService.CreateSchema();

                schema.CreateIndex(null, ObjectContext.Create(typeof(Library.Entity.ContractLand)).TableName, "ID", null, true);
                schema.CreateIndex(null, ObjectContext.Create(typeof(Library.Entity.LandVirtualPerson)).TableName, "ID", null, true);
                schema.CreateIndex(null, ObjectContext.Create(typeof(Library.Entity.ContractRequireTable)).TableName, "ID", null, true);
                schema.CreateIndex(null, ObjectContext.Create(typeof(Library.Entity.ContractRegeditBook)).TableName, "ID", null, true);
                schema.CreateIndex(null, ObjectContext.Create(typeof(Library.Entity.ContractConcord)).TableName, "ID", null, true);
                schema.CreateIndex(null, ObjectContext.Create(typeof(Library.Entity.BuildLandBoundaryAddressCoil)).TableName, "ID", null, true);
                schema.CreateIndex(null, ObjectContext.Create(typeof(Library.Entity.BuildLandBoundaryAddressDot)).TableName, "ID", null, true);

                schema.CreateIndex(null, ObjectContext.Create(typeof(Library.Entity.Zone)).TableName, "DYQBM", null);
                schema.CreateIndex(null, ObjectContext.Create(typeof(Library.Entity.ContractLand)).TableName, "DKLB", null);
                schema.CreateIndex(null, ObjectContext.Create(typeof(Library.Entity.ContractLand)).TableName, "ZLDM", null);
                schema.CreateIndex(null, ObjectContext.Create(typeof(Library.Entity.ContractLand)).TableName, "QLRBS", null);
                schema.CreateIndex(null, ObjectContext.Create(typeof(Library.Entity.LandVirtualPerson)).TableName, "DYBM", null);
                schema.CreateIndex(null, ObjectContext.Create(typeof(Library.Entity.BuildLandBoundaryAddressCoil)).TableName, "DKBS", null);
                schema.CreateIndex(null, ObjectContext.Create(typeof(Library.Entity.BuildLandBoundaryAddressCoil)).TableName, "DYDM", null);
                schema.CreateIndex(null, ObjectContext.Create(typeof(Library.Entity.BuildLandBoundaryAddressDot)).TableName, "DKID", null);
                schema.CreateIndex(null, ObjectContext.Create(typeof(Library.Entity.BuildLandBoundaryAddressDot)).TableName, "DYBM", null);

                schema.CreateSpatialIndex(null, ObjectContext.Create(typeof(Library.Entity.Zone)).TableName, "Shape");
                schema.CreateSpatialIndex(null, ObjectContext.Create(typeof(Library.Entity.ContractLand)).TableName, "Shape");
                schema.CreateSpatialIndex(null, ObjectContext.Create(typeof(Library.Entity.SecondTableLand)).TableName, "Shape");
                schema.CreateSpatialIndex(null, ObjectContext.Create(typeof(Library.Entity.ContractLandMark)).TableName, "Shape");
                schema.CreateSpatialIndex(null, ObjectContext.Create(typeof(Library.Entity.BuildLandBoundaryAddressCoil)).TableName, "Shape");
                schema.CreateSpatialIndex(null, ObjectContext.Create(typeof(Library.Entity.BuildLandBoundaryAddressDot)).TableName, "Shape");
                schema.CreateSpatialIndex(null, ObjectContext.Create(typeof(Library.Entity.ControlPoint)).TableName, "Shape");
                schema.CreateSpatialIndex(null, ObjectContext.Create(typeof(Library.Entity.DCZD)).TableName, "Shape");
                schema.CreateSpatialIndex(null, ObjectContext.Create(typeof(Library.Entity.DZDW)).TableName, "Shape");
                schema.CreateSpatialIndex(null, ObjectContext.Create(typeof(Library.Entity.XZDW)).TableName, "Shape");
                schema.CreateSpatialIndex(null, ObjectContext.Create(typeof(Library.Entity.MZDW)).TableName, "Shape");
                schema.CreateSpatialIndex(null, ObjectContext.Create(typeof(Library.Entity.ZoneBoundary)).TableName, "Shape");
                schema.CreateSpatialIndex(null, ObjectContext.Create(typeof(Library.Entity.FarmLandConserve)).TableName, "Shape");

                schema.UpdateSpatialIndex(null, ObjectContext.Create(typeof(Library.Entity.Zone)).TableName, "Shape");
                schema.UpdateSpatialIndex(null, ObjectContext.Create(typeof(Library.Entity.ContractLand)).TableName, "Shape");
                schema.UpdateSpatialIndex(null, ObjectContext.Create(typeof(Library.Entity.SecondTableLand)).TableName, "Shape");
                schema.UpdateSpatialIndex(null, ObjectContext.Create(typeof(Library.Entity.ContractLandMark)).TableName, "Shape");
                schema.UpdateSpatialIndex(null, ObjectContext.Create(typeof(Library.Entity.BuildLandBoundaryAddressCoil)).TableName, "Shape");
                schema.UpdateSpatialIndex(null, ObjectContext.Create(typeof(Library.Entity.BuildLandBoundaryAddressDot)).TableName, "Shape");
                schema.UpdateSpatialIndex(null, ObjectContext.Create(typeof(Library.Entity.ControlPoint)).TableName, "Shape");
                schema.UpdateSpatialIndex(null, ObjectContext.Create(typeof(Library.Entity.DCZD)).TableName, "Shape");
                schema.UpdateSpatialIndex(null, ObjectContext.Create(typeof(Library.Entity.DZDW)).TableName, "Shape");
                schema.UpdateSpatialIndex(null, ObjectContext.Create(typeof(Library.Entity.XZDW)).TableName, "Shape");
                schema.UpdateSpatialIndex(null, ObjectContext.Create(typeof(Library.Entity.MZDW)).TableName, "Shape");
                schema.UpdateSpatialIndex(null, ObjectContext.Create(typeof(Library.Entity.ZoneBoundary)).TableName, "Shape");
                schema.UpdateSpatialIndex(null, ObjectContext.Create(typeof(Library.Entity.FarmLandConserve)).TableName, "Shape");

                this.ReportInfomation("创建索引完成。");

                return true;
            }
            catch (Exception ex)
            {
                this.ReportError(string.Format("创建索引失败，导入将中止。{0}", ex));
                return false;
            }
        }

        private bool CompressDatabase(IDbContext localService)
        {
            try
            {
                (localService.DataSource as YuLinTu.Data.SQLite.IProviderDbCSQLite).Compress();
                this.ReportInfomation("优化数据库完成。");

                return true;
            }
            catch (Exception ex)
            {
                this.ReportError(string.Format("删除索引失败，导入将中止。{0}", ex));
                return false;
            }
        }

        private bool DeleteOldData(IDbContext localService, GainInfo ginfo)
        {
            try
            {
                var codeZone = ginfo.ZoneCode;

                localService.Queries.Add(
                    localService.CreateQuery<YuLinTu.Library.Entity.ContractRequireTable>().
                    Where(c => c.ZoneCode.StartsWith(codeZone)).Delete());

                localService.Queries.Add(
                    localService.CreateQuery<YuLinTu.Library.Entity.ContractRegeditBook>().
                    Where(c => c.ZoneCode.StartsWith(codeZone)).Delete());

                localService.Queries.Add(
                    localService.CreateQuery<YuLinTu.Library.Entity.ContractConcord>().
                    Where(c => c.ZoneCode.StartsWith(codeZone)).Delete());

                localService.Queries.Add(
                    localService.CreateQuery<YuLinTu.Library.Entity.ContractLand>().
                    Where(c => c.ZoneCode.StartsWith(codeZone)).Delete());

                localService.Queries.Add(
                    localService.CreateQuery<YuLinTu.Library.Entity.BelongRelation>().
                    Where(c => c.ZoneCode.StartsWith(codeZone)).Delete());

                localService.Queries.Add(
                    localService.CreateQuery<YuLinTu.Library.Entity.SecondTableLand>().
                    Where(c => c.SenderCode.StartsWith(codeZone)).Delete());

                localService.Queries.Add(
                    localService.CreateQuery<YuLinTu.Library.Entity.BuildLandBoundaryAddressCoil>().
                    Where(c => c.ZoneCode.StartsWith(codeZone)).Delete());

                localService.Queries.Add(
                    localService.CreateQuery<YuLinTu.Library.Entity.BuildLandBoundaryAddressDot>().
                    Where(c => c.ZoneCode.StartsWith(codeZone)).Delete());

                localService.Queries.Add(
                    localService.CreateQuery<YuLinTu.Library.Entity.LandVirtualPerson>().
                    Where(c => c.ZoneCode.StartsWith(codeZone)).Delete());

                localService.Queries.Add(
                    localService.CreateQuery<YuLinTu.Library.Entity.CollectivityTissue>().
                    Where(c => c.ZoneCode.StartsWith(codeZone)).Delete());

                localService.Queries.Add(
                    localService.CreateQuery<YuLinTu.Library.Entity.ZoneBoundary>().
                    Where(c => c.ZoneCode.StartsWith(codeZone)).Delete());

                localService.Queries.Add(
                    localService.CreateQuery<YuLinTu.Library.Entity.DZDW>().
                    Where(c => c.ZoneCode.StartsWith(codeZone)).Delete());

                localService.Queries.Add(
                    localService.CreateQuery<YuLinTu.Library.Entity.XZDW>().
                    Where(c => c.ZoneCode.StartsWith(codeZone)).Delete());

                localService.Queries.Add(
                    localService.CreateQuery<YuLinTu.Library.Entity.MZDW>().
                    Where(c => c.ZoneCode.StartsWith(codeZone)).Delete());

                localService.Queries.Add(
                    localService.CreateQuery<YuLinTu.Library.Entity.FarmLandConserve>().
                    Where(c => c.ZoneCode.StartsWith(codeZone)).Delete());

                localService.Queries.Add(
                    localService.CreateQuery<YuLinTu.Library.Entity.ContractLandMark>().
                    Where(c => c.SenderCode.StartsWith(codeZone)).Delete());

                localService.Queries.Add(
                    localService.CreateQuery<YuLinTu.Library.Entity.ControlPoint>().
                    Where(c => c.ZoneCode.StartsWith(codeZone)).Delete());

                localService.Queries.Add(
                    localService.CreateQuery<YuLinTu.Library.Entity.ContractLandMark>().
                    Where(c => c.SenderCode.StartsWith(codeZone)).Delete());

                localService.Queries.Save();

                this.ReportInfomation(string.Format("删除冗余数据完成。"));

                return true;
            }
            catch (Exception ex)
            {
                this.ReportError(string.Format("删除数据失败，导入将中止。{0}", ex));
                return false;
            }
        }

        private bool DeleteIndex(IDbContext localService)
        {
            try
            {
                var schema = localService.CreateSchema();

                schema.DeleteIndex(null, ObjectContext.Create(typeof(Library.Entity.ContractLand)).TableName, "ID", null);
                schema.DeleteIndex(null, ObjectContext.Create(typeof(Library.Entity.LandVirtualPerson)).TableName, "ID", null);
                schema.DeleteIndex(null, ObjectContext.Create(typeof(Library.Entity.ContractRequireTable)).TableName, "ID", null);
                schema.DeleteIndex(null, ObjectContext.Create(typeof(Library.Entity.ContractRegeditBook)).TableName, "ID", null);
                schema.DeleteIndex(null, ObjectContext.Create(typeof(Library.Entity.ContractConcord)).TableName, "ID", null);
                schema.DeleteIndex(null, ObjectContext.Create(typeof(Library.Entity.BuildLandBoundaryAddressCoil)).TableName, "ID", null);
                schema.DeleteIndex(null, ObjectContext.Create(typeof(Library.Entity.BuildLandBoundaryAddressDot)).TableName, "ID", null);

                schema.DeleteIndex(null, ObjectContext.Create(typeof(Library.Entity.Zone)).TableName, "DYQBM", null);
                schema.DeleteIndex(null, ObjectContext.Create(typeof(Library.Entity.ContractLand)).TableName, "DKLB", null);
                schema.DeleteIndex(null, ObjectContext.Create(typeof(Library.Entity.ContractLand)).TableName, "ZLDM", null);
                schema.DeleteIndex(null, ObjectContext.Create(typeof(Library.Entity.ContractLand)).TableName, "QLRBS", null);
                schema.DeleteIndex(null, ObjectContext.Create(typeof(Library.Entity.LandVirtualPerson)).TableName, "DYBM", null);
                schema.DeleteIndex(null, ObjectContext.Create(typeof(Library.Entity.BuildLandBoundaryAddressCoil)).TableName, "DKBS", null);
                schema.DeleteIndex(null, ObjectContext.Create(typeof(Library.Entity.BuildLandBoundaryAddressCoil)).TableName, "DYDM", null);
                schema.DeleteIndex(null, ObjectContext.Create(typeof(Library.Entity.BuildLandBoundaryAddressDot)).TableName, "DKID", null);
                schema.DeleteIndex(null, ObjectContext.Create(typeof(Library.Entity.BuildLandBoundaryAddressDot)).TableName, "DYBM", null);

                schema.DeleteSpatialIndex(null, ObjectContext.Create(typeof(Library.Entity.Zone)).TableName, "Shape");
                schema.DeleteSpatialIndex(null, ObjectContext.Create(typeof(Library.Entity.ContractLand)).TableName, "Shape");
                schema.DeleteSpatialIndex(null, ObjectContext.Create(typeof(Library.Entity.SecondTableLand)).TableName, "Shape");
                schema.DeleteSpatialIndex(null, ObjectContext.Create(typeof(Library.Entity.ContractLandMark)).TableName, "Shape");
                schema.DeleteSpatialIndex(null, ObjectContext.Create(typeof(Library.Entity.BuildLandBoundaryAddressCoil)).TableName, "Shape");
                schema.DeleteSpatialIndex(null, ObjectContext.Create(typeof(Library.Entity.BuildLandBoundaryAddressDot)).TableName, "Shape");
                schema.DeleteSpatialIndex(null, ObjectContext.Create(typeof(Library.Entity.ControlPoint)).TableName, "Shape");
                schema.DeleteSpatialIndex(null, ObjectContext.Create(typeof(Library.Entity.DCZD)).TableName, "Shape");
                schema.DeleteSpatialIndex(null, ObjectContext.Create(typeof(Library.Entity.DZDW)).TableName, "Shape");
                schema.DeleteSpatialIndex(null, ObjectContext.Create(typeof(Library.Entity.XZDW)).TableName, "Shape");
                schema.DeleteSpatialIndex(null, ObjectContext.Create(typeof(Library.Entity.MZDW)).TableName, "Shape");
                schema.DeleteSpatialIndex(null, ObjectContext.Create(typeof(Library.Entity.ZoneBoundary)).TableName, "Shape");
                schema.DeleteSpatialIndex(null, ObjectContext.Create(typeof(Library.Entity.FarmLandConserve)).TableName, "Shape");

                this.ReportInfomation("删除索引完成。");

                return true;
            }
            catch (Exception ex)
            {
                this.ReportError(string.Format("删除索引失败，导入将中止。{0}", ex));
                return false;
            }
        }

        /// <summary>
        /// 处理地域数据
        /// </summary>
        /// <returns></returns>
        private bool ProecessZoneData(GainInfo ginfo, IDbContext localService)
        {
            currentPath.YearCode = ginfo.Year;
            currentPath.ZoneCode = ginfo.ZoneCode;
            if (!FileZone.ImportFile.VictorZone.IsExport)
            {
                return true;
            }
            List<Zone> zoneCollection = ZoneDataIntegration.IntegrateZoneData(currentPath);
            if (zoneCollection != null && zoneCollection.Count > 0 && CheckZoneInfo(zoneCollection))
            {
                ImportZoneData(zoneCollection, localService);
            }
            ZoneDto zone = localService.CreateZoneWorkStation().Get(ginfo.ZoneCode);
            if (zone == null)
            {
                this.ReportError("系统中不存在地域编码为" + ginfo.ZoneCode + "的地域,不能继续执行数据导入!");
                hasError = true;
            }
            return zone == null ? false : true;
        }

        /// <summary>
        /// 创建数据库
        /// </summary>
        private void CreateaDataBase(string name, SpatialReference sr)
        {
            string databasepath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, name + ".sqlite");
            bool filecanread = true;
            if (File.Exists(databasepath))
            {
                using (var stream = File.OpenRead(databasepath))
                {
                    if (stream.Length < 1024 * 1024)
                        filecanread = false;
                }
            }
            if ((!File.Exists(databasepath) || !filecanread) && sr != null)
            {
                var ds = DataBaseHelper.TryCreateDatabase(databasepath, sr);
                if (ds != null)
                {
                    DataBaseHelper.TryUpdateDatabase(ds);
                    DataBaseHelper.SetDefaultDatabaseName(databasepath, true);
                    LocalService = ds;
                }
            }
            else
            {
                LocalService = ProviderDbCSQLite.CreateDataSourceByFileName(databasepath) as IDbContext;
            }
        }

        /// <summary>
        /// 导入地域数据
        /// </summary>
        private bool ImportZoneData(List<Zone> zoneCollection, IDbContext localService)
        {
            bool result = true;
            this.ReportProgress(1, "正在导入地域数据");
            var zones = new List<ZoneDto>();

            var zgzoncount = zoneCollection.Count(zz => zz.Level == eZoneLevel.Town);
            var czoncount = zoneCollection.Count(zz => zz.Level == eZoneLevel.Village);
            var zzoncount = zoneCollection.Count(zz => zz.Level == eZoneLevel.Group);

            if (zgzoncount == 0)
            {
                this.ReportWarn("在成果库中没有获取到镇级地域数据!");
            }
            if (czoncount == 0)
            {
                this.ReportWarn("在成果库中没有获取到村级地域数据!");
            }
            if (zzoncount == 0)
            {
                this.ReportWarn("在成果库中没有获取到组级地域数据!");
            }

            var level = zoneCollection.Max(t => t.Level);
            var maxzone = zoneCollection.Find(t => t.Level == level);
            var upZone = localService.CreateZoneWorkStation().Get(maxzone.UpLevelCode);
            if (upZone != null)
            {
                maxzone.Name = maxzone.Name == null ? "" : maxzone.Name.Replace(upZone.FullName, "");
                maxzone.UpLevelName = upZone.FullName;
            }
            try
            {
                var pzndic = GetZoneNameProperties();
                var zoneSet = new HashSet<string>();
                var zoneList = new List<ZoneDto>();
                int importcount = 0;
                foreach (var zone in zoneCollection)
                {
                    if ((zone.FullCode.Length == 2 || zone.FullCode.Length == 4) && pzndic.ContainsKey(zone.FullCode))
                        zone.Name = pzndic[zone.FullCode];
                    GeoAPI.Geometries.IGeometry geo = zone.Shape as GeoAPI.Geometries.IGeometry;
                    if (geo != null)
                        geo.SRID = srid;

                    var z = new ZoneDto()
                    {
                        Code = zone.Code,
                        Level = (YuLinTu.Library.Entity.eZoneLevel)((short)zone.Level),
                        Name = zone.Name,
                        UpLevelCode = zone.UpLevelCode,
                        UpLevelName = zone.UpLevelName,
                        FullName = zone.FullName,
                        FullCode = zone.FullCode,
                        ID = Guid.NewGuid(),
                        Shape = zone.Shape == null ? null : Spatial.Geometry.FromBytes(Spatial.Geometry.FromInstance(zone.Shape as GeoAPI.Geometries.IGeometry).AsBinary(), (zone.Shape as GeoAPI.Geometries.IGeometry).SRID)
                    };
                    if (!zoneSet.Contains(z.FullCode))
                        zones.Add(z);
                    if (zones.Count == 100)
                    {
                        localService.CreateZoneWorkStation().Add(zones, true, (s, index, cnt) => { });
                        importcount += zones.Count;
                        zones.Clear();
                        this.ReportProgress(1, $"({importcount}/{zoneCollection.Count})正在导入地域数据");
                    }
                }
                if (zones.Count > 0)
                {
                    localService.CreateZoneWorkStation().Add(zones, true, (z, index, cnt) => { });
                    importcount += zones.Count;
                }
                this.ReportInfomation("成功导入地域" + importcount + "条");
                zoneSet.Clear();
            }
            catch (Exception ex)
            {
                result = false;
                string errorInfo = "导入地域数据失败!";
                this.ReportError(errorInfo + ex.ToString());
                hasError = true;
                throw new Exception(errorInfo);
            }
            finally
            {
                zones.Clear();
                GC.Collect();
            }
            return result;
        }

        /// <summary>
        /// 执行导入数据
        /// </summary>
        private bool ExcuteImportData(string zoneCode, IDbContext localService, SpaceDataOperator shpProcess)
        {
            if (currentPath == null)
            {
                return false;
            }
            if (!FileZone.IsImportBusinessData)
            {
                return true;
            }
            db = DataBase.CreateDbContext(currentPath.DataBasePath);
            this.ReportProgress(3, "正在获取交换数据...");
            var dbProcess = new DbDataProcess(db);
            dbProcess.UseZoneCode = ConfigurationManager.AppSettings.TryGetValue<bool>("UseZoneCode", false);
            dbProcess.ReportInfo += (info) => { this.ReportInfomation(info); };
            dbProcess.ReportWarningInfo += (warninfo) => { this.ReportWarn(warninfo); };
            var currentZoneList = dbProcess.CurrentZoneCode();//得到当前数据地域编码
            if (currentZoneList.Count == 0)
            {
                return false;
            }
            var zoneStation = LocalService.CreateZoneWorkStation();
            var zones = zoneStation.GetByChildLevel(zoneCode, Library.Entity.eZoneLevel.Town);
            var townList = currentZoneList.GroupBy(t => t.Substring(0, 9)).Select(s => s.Key).ToList();//按乡镇分组地域
            foreach (var tcode in townList)
            {
                if (!zones.Any(z => z.FullCode == tcode))
                {
                    currentZoneList.RemoveAll(z => z.StartsWith(tcode));
                }
            }
            //按地域导入数据
            bool sucess = ImportDataByZoneCode(currentZoneList, dbProcess, shpProcess);
            currentZoneList.Clear();
            GC.Collect();
            return sucess;
        }

        /// <summary>
        /// 按地域导入数据
        /// </summary>
        private bool ImportDataByZoneCode(List<string> codeList, DbDataProcess dbProcess, SpaceDataOperator shpProcess)
        {
            int index = 0;
            bool sucess = true;

            double percent = 90 / (double)codeList.Count;
            var townList = codeList.GroupBy(t => t.Substring(0, 9)).ToList();//按乡镇分组地域
            var dicCodeName = dbProcess.GetPresonCodeName();
            foreach (var town in townList)
            {
                var townCodeList = town.ToList();
                var groupList = townCodeList.GroupBy(t => t.Substring(0, 12)).ToList();
                var fnps = currentPath.ShapeFileList.FindAll(t => t.FullName.ToLower().StartsWith(DK.TableName.ToLower() + extentName));
                var towndataCollection = dbProcess.GetCollection(town.Key);//按乡镇获取属性数据
                foreach (var group in groupList)
                {
                    var keyCode = group.Key;
                    var dataCollection = dbProcess.FileterDataCollection(towndataCollection, keyCode);
                    if (dataCollection.CBFJH == null || dataCollection.CBFJH.Count == 0)
                    {
                        dataCollection = dbProcess.GetCollection(keyCode);//按村获取属性数据
                    }
                    //var valligelandList = shpProcess.InitiallLandList(fnps, keyCode);//按村组获取空间地块
                    var valligelandList = shpProcess.GetLandFromDatabase(keyCode, GenerateCoilDot);

                    foreach (var z in group)//按组处理导入
                    {
                        index++;

                        var landList = valligelandList.FindAll(l => l.DKBM.StartsWith(z));
                        var dkDic = new Dictionary<string, DKEX>();
                        landList.ForEach(t =>
                        {
                            if (!dkDic.ContainsKey(t.DKBM))
                                dkDic.Add(t.DKBM, t);
                        });
                        var rest = ExecuteImport(dkDic, z, codeList.Count, dicCodeName,
                            dataCollection, dbProcess, index, percent);
                        sucess = (!rest) ? rest : sucess;
                    }
                }
            }
            shpProcess.ClearLandDictionary();
            GC.Collect();
            return sucess;
        }

        /// <summary>
        /// 按最小地域进行数据导入
        /// </summary>
        /// <param name="landList">全部地块集合</param>
        /// <param name="searchCode">查询数据编码，与地域编码可能不同</param>
        /// <param name="zoneList">地域集合</param>
        /// <param name="localService">数据服务</param>
        /// <param name="townCollection">一个村的所有数据</param>
        /// <param name="dicCodeName">编码名称字典</param>
        /// <param name="dbProcess">数据库查询类实体</param>
        /// <param name="index">处理序号</param>
        /// <param name="percent">处理百分比</param>
        /// <returns></returns>
        private bool ExecuteImport(Dictionary<string, DKEX> dkDic, string searchCode, int zoneListCount, Dictionary<string, string> dicCodeName,
            DataCollectionDb townCollection, DbDataProcess dbProcess, int index, double percent)
        {
            bool sucess = true;

            var standCode = searchCode;
            var zoneStation = LocalService.CreateZoneWorkStation();
            var zone = zoneStation.Get(searchCode);
            if (zone == null)
            {
                this.ReportError(string.Format("系统中不存在地域编码为{0}的行政地域,无法导入该地域相关数据!", searchCode));
                return false;
            }
            var zoneName = zone.FullName + "(" + zone.FullCode + ")";

            string zoneCode = zone.FullCode;
            //this.ReportInfomation(string.Format("正在导入{0}下的数据", zoneName));
            this.ReportProgress(3 + (int)(index * percent), string.Format("({0}/{1})导入{2}数据", index, zoneListCount, zone.FullName));

            //执行数据导入
            var creList = new List<ComplexRightEntity>();//空户
            landCodeSet.Clear();
            var entityList = dbProcess.GetRightCollectionByZone(searchCode, zoneCode, dkDic, townCollection,
                dicCodeName, creList, landCodeSet);
            var entityfbf = townCollection.FBFJH.Find(fbf => fbf.FBFBM == standCode.PadRight(14, '0'));
            var localfbf = LocalComplexRightEntity.InitalizeSenderData(entityfbf, zone.FullCode);
            if (localfbf != null)
            {
                var senderStation = LocalService.CreateSenderWorkStation();
                var nowzone = zoneStation.FirstOrDefault<Library.Entity.Zone>(c => c.FullCode == localfbf.ZoneCode);
                if (nowzone != null)
                    localfbf.ID = nowzone.ID;

                var fbfcn = senderStation.Count(localfbf.ZoneCode, eLevelOption.Self);
                if (fbfcn == 0)
                {
                    senderStation.Add(localfbf);
                }
            }
            else
            {
                this.ReportInfomation("当前地域" + zone.FullName + "下发包方为空");
            }
            try
            {
                if (entityList.Count > 0)
                {
                    ImportContractLandPropertyRightDatas(LocalService, entityList, zone.FullName, zone.FullCode);
                }
                if (creList.Count > 0)
                {
                    ImportNoContractLandVPDatas(LocalService, creList, zone.FullName, zone.FullCode);
                }
                //提示信息，用总的
                entityList.AddRange(creList);
                var noContractLands = new List<DKEX>();//无承包方编码的地-未签合同的地
                foreach (var item in landCodeSet)
                {
                    if (dkDic.ContainsKey(item))
                    {
                        noContractLands.Add(dkDic[item]);
                    }
                }
                int nccount = ImportContractLandDatas(LocalService, noContractLands, zone.FullName, zone.FullCode,
                    8000 + creList.Count + 1);
                ReportImportInfo(entityList, zoneName, nccount);
            }
            catch (Exception ex)
            {
                sucess = false;
                string errorMsg = string.Format("导入{0}数据失败", zone.FullName);
                this.ReportError(errorMsg + ex.Message);
                LogWrite.WriteErrorLog(errorMsg + ex.ToString());
            }
            entityList.Clear();
            entityList = null;
            creList.Clear();
            creList = null;
            GC.Collect();
            return sucess;
        }

        /// <summary>
        /// 提示信息
        /// </summary>
        private void ReportImportInfo(List<ComplexRightEntity> entityList, string name, int nccount)
        {
            if (entityList == null)
                return;
            int dkNumber = 0;
            int cyNumber = 0;
            int lzNumber = 0;
            int bfNumber = 0;
            int hfNumber = 0;
            int djbNumber = 0;
            int htNumber = 0;
            int cbfNumber = 0;
            int zxNumber = 0;
            int fjNumber = 0;
            int qzNumner = 0;
            foreach (var entity in entityList)
            {
                djbNumber += (entity.DJB == null ? 0 : entity.CBJYQZ.Count);
                qzNumner += (entity.CBJYQZ == null ? 0 : entity.CBJYQZ.Count);
                htNumber += (entity.HT == null ? 0 : entity.HT.Count);
                cbfNumber += (entity.CBF == null ? 0 : 1);
                zxNumber += (entity.QZZX == null ? 0 : entity.QZZX.Count);
                fjNumber += (entity.FJ == null ? 0 : entity.FJ.Count);
                dkNumber += entity.DKXX == null ? 0 : entity.DKXX.Count;
                cyNumber += entity.JTCY == null ? 0 : entity.JTCY.Count;
                lzNumber += entity.LZHT == null ? 0 : entity.LZHT.Count;
                bfNumber += entity.QZBF == null ? 0 : entity.QZBF.Count;
                hfNumber += entity.QZHF == null ? 0 : entity.QZHF.Count;
            }
            this.ReportInfomation("成功导入" + name + "下发包方1个,承包方" + cbfNumber + "个,家庭成员" + cyNumber + "条,合同" +
                htNumber + "条,登记簿" + djbNumber + "条,权证" + qzNumner + "条,地块" + dkNumber + "条,权证注销数据" + zxNumber +
                "条,权证补发数据" + bfNumber + "条,权证换发数据" + hfNumber + "条,流转合同数据" + lzNumber + "条,附件数据" + fjNumber + "条,非承包地" + nccount + "块");
            entityList = null;
        }

        private void ImportContractLandPropertyRightDatas(IDbContext localService, List<ComplexRightEntity> entityList, string zoneName = null, string zoneCode = null, bool isnormalexport = true)
        {
            var db = localService.CreateZoneWorkStation();
            List<ResultDbToLocalDb.LocalComplexRightEntity> list = new List<ResultDbToLocalDb.LocalComplexRightEntity>();
            foreach (var item in entityList)
            {
                ResultDbToLocalDb.LocalComplexRightEntity obj =
                    ResultDbToLocalDb.LocalComplexRightEntity.From(item,
                        new Action<string>(t => this.ReportError(t)));
                list.Add(obj);
            }
            Dictionary<string, Library.Entity.VirtualPerson> families = new Dictionary<string, Library.Entity.VirtualPerson>();
            foreach (var item in list)
                families[string.Format("{0}#{1}", item.CBF.FamilyNumber, item.CBF.ZoneCode)] = item.CBF;

            Dictionary<string, Library.Entity.CollectivityTissue> tissues = new Dictionary<string, Library.Entity.CollectivityTissue>();
            foreach (var item in list)
                tissues[item.ZoneCode] = item.FBF;

            foreach (var item in tissues)
            {
                var zone = db.FirstOrDefault<Library.Entity.Zone>(c => c.FullCode == item.Value.ZoneCode);
                if (zone != null)
                    item.Value.ID = zone.ID;
            }

            List<Library.Entity.ContractConcord> listCC = new List<Library.Entity.ContractConcord>();
            List<Library.Entity.ContractRegeditBook> listCRB = new List<Library.Entity.ContractRegeditBook>();
            List<Library.Entity.ContractLand> listCL = new List<Library.Entity.ContractLand>();

            //确股地块数据
            List<Library.Entity.BelongRelation> listCLBR = new List<Library.Entity.BelongRelation>();
            var listSC = new List<Library.Entity.StockConcord>();
            var listSW = new List<Library.Entity.StockWarrant>();

            //当前地域下的所有界址点线
            List<Library.Entity.BuildLandBoundaryAddressDot> zonelistDots = new List<Library.Entity.BuildLandBoundaryAddressDot>();
            List<Library.Entity.BuildLandBoundaryAddressCoil> zonelistCoils = new List<Library.Entity.BuildLandBoundaryAddressCoil>();
            //要保存的
            List<Library.Entity.BuildLandBoundaryAddressDot> zonelistSaveDots = new List<Library.Entity.BuildLandBoundaryAddressDot>();
            List<Library.Entity.BuildLandBoundaryAddressCoil> zonelistSaveCoils = new List<Library.Entity.BuildLandBoundaryAddressCoil>();

            if (GenerateCoilDot)
            {
                if (isnormalexport)
                {
                    this.ReportInfomation("开始导入" + zoneName + "下界址数据");
                }
                zonelistDots = GetDataListFromShapes<Library.Entity.BuildLandBoundaryAddressDot>((currentPath.ShapeFileList.FindAll(t => t.FullName.StartsWith(JZD.TableName + extentName))), "LandNumber", zoneCode, true);
                zonelistCoils = GetDataListFromShapes<Library.Entity.BuildLandBoundaryAddressCoil>((currentPath.ShapeFileList.FindAll(t => t.FullName.StartsWith(JZX.TableName + extentName))), "LandNumber", zoneCode, true);
            }
            foreach (var obj in list)
            {
                SetExchangeEntity(zonelistSaveDots, zonelistSaveCoils, zonelistDots, zonelistCoils, listCC, listCRB, listCL, listCLBR, listSC, listSW, obj, zoneCode, zoneName, families, tissues);
            }

            listCC.RemoveAll(ccid => ccid.ID == Guid.Empty);
            listCRB.RemoveAll(ccid => ccid.ID == Guid.Empty);

            localService.Queries.Add(
                localService.CreateQuery<Library.Entity.LandVirtualPerson>().
                AddRange(families.Select(c => c.Value).ToArray()));

            localService.Queries.Add(
                localService.CreateQuery<Library.Entity.ContractConcord>().
                AddRange(listCC.ToArray()));

            localService.Queries.Add(
                localService.CreateQuery<Library.Entity.ContractRegeditBook>().
                AddRange(listCRB.ToArray()));

            localService.Queries.Add(
                localService.CreateQuery<Library.Entity.ContractLand>().
                AddRange(listCL.ToArray()));

            localService.Queries.Add(
               localService.CreateQuery<Library.Entity.BelongRelation>().
               AddRange(listCLBR.ToArray()));

            localService.Queries.Add(
               localService.CreateQuery<Library.Entity.StockConcord>().
               AddRange(listSC.ToArray()));

            localService.Queries.Add(
               localService.CreateQuery<Library.Entity.StockWarrant>().
               AddRange(listSW.ToArray()));

            if (isnormalexport && listCLBR.Count > 0)
            {
                this.ReportInfomation("成功导入" + zoneName + "下确股数据" + listCLBR.Count.ToString() + "条");
            }
            if (GenerateCoilDot)
            {
                localService.Queries.Add(
                    localService.CreateQuery<Library.Entity.BuildLandBoundaryAddressDot>().
                    AddRange(zonelistSaveDots.ToArray()));

                localService.Queries.Add(
                    localService.CreateQuery<Library.Entity.BuildLandBoundaryAddressCoil>().
                    AddRange(zonelistSaveCoils.ToArray()));
                if (isnormalexport)
                {
                    this.ReportInfomation("成功导入" + zoneName + "下界址点数据" + zonelistSaveDots.Count.ToString() + "条,界址线数据" + zonelistSaveCoils.Count.ToString() + "条");
                }
            }
            localService.Queries.Save();
        }

        /// <summary>
        /// 设置交换实体
        /// </summary>
        private void SetExchangeEntity(
            List<Library.Entity.BuildLandBoundaryAddressDot> zonelistSaveDots,
            List<Library.Entity.BuildLandBoundaryAddressCoil> zonelistSaveCoils,
            List<Library.Entity.BuildLandBoundaryAddressDot> zonelistDots,
            List<Library.Entity.BuildLandBoundaryAddressCoil> zonelistCoils,
            List<Library.Entity.ContractConcord> listCC,
            List<Library.Entity.ContractRegeditBook> listCRB,
            List<Library.Entity.ContractLand> listCL,
            List<Library.Entity.BelongRelation> listCLBR,
            List<Library.Entity.StockConcord> listSC,
            List<Library.Entity.StockWarrant> listSW,
            ResultDbToLocalDb.LocalComplexRightEntity obj,
            string zoneCode,
            string zoneName,
            Dictionary<string, Library.Entity.VirtualPerson> families,
            Dictionary<string, Library.Entity.CollectivityTissue> tissues)
        {
            var family = families[string.Format("{0}#{1}", obj.CBF.FamilyNumber, obj.CBF.ZoneCode)];
            var tissue = tissues[obj.FBF.ZoneCode];

            if (obj.HT != null)
            {
                obj.HT.ForEach(ht =>
                {
                    ht.ContracterId = family.ID;
                    ht.ContracterName = family.Name;
                    ht.SenderId = tissue.ID;
                    ht.SenderName = tissue.Name;
                    //需要重新计算实测和确权面积
                    ht.CountActualArea = 0;
                    ht.CountAwareArea = 0;

                    if (listCC.Any(dd => dd.ConcordNumber == ht.ConcordNumber) == false)
                    {
                        listCC.Add(ht);
                    }

                    if (obj.CBJYQZ != null && obj.DJB != null)
                    {
                        var localqz = obj.CBJYQZ.Find(qz => qz.RegeditNumber == ht.ConcordNumber);
                        var localdjb = obj.DJB.Find(qz => qz.Number == ht.ConcordNumber);

                        if (localqz != null && localdjb != null)
                        {
                            localqz.ID = ht.ID;
                            localqz.Year = currentPath.YearCode;
                            localqz.Number = localdjb.Number;
                            localqz.RegeditNumber = localdjb.RegeditNumber;
                            localqz.SerialNumber = localdjb.SerialNumber.IsNullOrEmpty() ? "" : (localdjb.SerialNumber.Length <= 6 ? localdjb.SerialNumber : localdjb.SerialNumber.Substring(localdjb.SerialNumber.Length - 7, 6));

                            localqz.ContractRegeditBookExcursus = localdjb.ContractRegeditBookExcursus;
                            localqz.ContractRegeditBookPerson = localdjb.ContractRegeditBookPerson;
                            localqz.ContractRegeditBookTime = localdjb.ContractRegeditBookTime;
                        }

                        if (localqz != null)
                            if (listCRB.Any(dd => dd.Number == localqz.Number) == false)
                            {
                                listCRB.Add(localqz);
                            }
                    }
                });
            }

            Library.Entity.ContractConcord landcc = null;
            Library.Entity.ContractRegeditBook landcrb = null;
            Library.Entity.ContractConcord qglandcc = null;
            Library.Entity.ContractRegeditBook qglandcrb = null;
            int qglandCount = 0;
            if (obj.DKXXS != null)
            {
                foreach (var land in obj.DKXXS)
                {
                    landcc = listCC.Find(cc => cc.ConcordNumber == obj.LandguidHTBMList[land.ID]);
                    landcrb = listCRB.Find(cc => cc.Number == obj.LandguidHTBMList[land.ID]);
                    if (land.IsStockLand == true)
                    {
                        family.IsStockFarmer = true;

                        qglandCount++;

                        qglandcc = landcc;
                        qglandcrb = landcrb;

                        var brland = obj.BRDKS.Find(brid => brid.LandID == land.ID);
                        var existedLand = listCL.FirstOrDefault(x => x.LandNumber.Equals(land.LandNumber));
                        if (existedLand != null)
                        {
                            brland.LandID = existedLand.ID;
                            land.ID = existedLand.ID;
                        }
                        if (brland != null)
                        {
                            brland.VirtualPersonID = family.ID;
                            listCLBR.Add(brland);
                        }
                    }
                    else
                    {
                        if (landcc != null)
                        {
                            landcc.CountAwareArea = landcc.CountAwareArea + land.AwareArea;
                            landcc.CountActualArea = landcc.CountActualArea + land.ActualArea;
                        }

                        land.OwnerId = family.ID;
                        if (landcc != null)
                        {
                            land.ConcordId = landcc.ID;
                        }
                        //else
                        //{
                        //    this.ReportWarn(string.Format("承包地块{0}的合同不存在!", land.LandNumber));
                        //}
                    }
                    land.OwnerName = family.Name;
                    land.ZoneCode = obj.ZoneCode;
                    land.ZoneName = zoneName;

                    land.LandName = land.LandCode.IsNullOrEmpty() ? string.Empty : (dicLandType.ContainsKey(land.LandCode) ? dicLandType[land.LandCode].Name : string.Empty);
                    land.OwnRightType = YuLinTu.Library.Entity.DictionaryTypeInfo.JTTDSYQ;
                    if (land.IsStockLand && listCL.Any(x => x.LandNumber.Equals(land.LandNumber)))
                    {
                    }
                    else
                    {
                        listCL.Add(land);

                        if (GenerateCoilDot)
                        {
                            SetLineAndPoint(zonelistSaveDots, zonelistSaveCoils, zonelistDots, zonelistCoils, land, obj, zoneCode);
                        }
                    }
                }

                if (qglandcc != null && qglandCount == obj.DKXXS.Count)
                {
                    listSC.Add(qglandcc.ConvertTo<Library.Entity.StockConcord>(true));
                    listCC.Remove(qglandcc);
                    if (qglandcrb != null)
                    {
                        listSW.Add(qglandcrb.ConvertTo<Library.Entity.StockWarrant>(true));
                        listCRB.Remove(qglandcrb);
                    }
                }
            }
        }

        /// <summary>
        /// 处理生成界址信息
        /// </summary>
        private void SetLineAndPoint(List<Library.Entity.BuildLandBoundaryAddressDot> zonelistSaveDots, List<Library.Entity.BuildLandBoundaryAddressCoil> zonelistSaveCoils,
            List<Library.Entity.BuildLandBoundaryAddressDot> zonelistDots, List<Library.Entity.BuildLandBoundaryAddressCoil> zonelistCoils,
            Library.Entity.ContractLand land, ResultDbToLocalDb.LocalComplexRightEntity obj, string zoneCode)
        {
            var landDots = new List<Library.Entity.BuildLandBoundaryAddressDot>();
            var landCoils = new List<Library.Entity.BuildLandBoundaryAddressCoil>();

            var landnumber = land.LandNumber;
            if (landnumber.IsNullOrEmpty()) return;
            if (zonelistDots == null || zonelistCoils == null) return;
            if (zonelistDots.Count == 0 || zonelistCoils.Count == 0) return;

            var landlistDots = zonelistDots.FindAll(dd => dd.LandNumber == landnumber);
            var landlistCoils = zonelistCoils.FindAll(dc => dc.LandNumber == landnumber);
            if (landlistDots == null || landlistCoils == null) return;
            if (landlistDots.Count == 0 || landlistCoils.Count == 0) return;

            var landKJZBvalue = obj.LandguidkjzbList[land.ID];
            if (landKJZBvalue.IsNullOrEmpty()) return;
            //shape中地块统编界址点号
            List<string> tbjzdh = new List<string>();
            if (landKJZBvalue.Contains("/"))
            {
                tbjzdh = landKJZBvalue.Split('/').ToList();
                if (tbjzdh.Count == 0) return;
            }

            //找出空间坐标对应的界址点
            int index = 1;
            foreach (var tbjzdhitem in tbjzdh)
            {
                var item = landlistDots.Find(d => d.DotNumber == tbjzdhitem);
                if (item == null) continue;
                var landdotitem = new Library.Entity.BuildLandBoundaryAddressDot();
                landdotitem.CopyPropertiesFrom(item);
                landdotitem.ID = Guid.NewGuid();
                landdotitem.UniteDotNumber = tbjzdhitem;
                landdotitem.IsValid = false;
                landdotitem.DotNumber = GetPrefix(item) + index;
                landdotitem.LandID = land.ID;
                landdotitem.ZoneCode = zoneCode;
                if (landDots.Any(ld => ld.ID == landdotitem.ID) == false)
                {
                    landDots.Add(landdotitem);
                    index++;
                }
            }
            //处理界址线
            short coilindex = 1;
            foreach (var item in landlistCoils)
            {
                var landcoilitem = new Library.Entity.BuildLandBoundaryAddressCoil();
                landcoilitem.CopyPropertiesFrom(item);
                landcoilitem.ID = Guid.NewGuid();
                landcoilitem.LandID = land.ID;
                landcoilitem.ZoneCode = zoneCode;
                if (landcoilitem.Position != null && landcoilitem.Position.Length == 2)
                {
                    landcoilitem.Position = landcoilitem.Position.Substring(1);
                }
                var qjzd = landDots.Find(ld => ld.UniteDotNumber == landcoilitem.StartNumber);
                if (qjzd != null)
                {
                    qjzd.IsValid = true;
                    landcoilitem.StartPointID = qjzd.ID;
                    landcoilitem.StartNumber = qjzd.DotNumber;
                }

                var zjzd = landDots.Find(ld => ld.UniteDotNumber == landcoilitem.EndNumber);
                if (zjzd != null)
                {
                    zjzd.IsValid = true;
                    landcoilitem.EndPointID = zjzd.ID;
                    landcoilitem.EndNumber = zjzd.DotNumber;
                }

                if (landCoils.Any(ld => ld.ID == landcoilitem.ID) == false)
                {
                    landcoilitem.CoilLength = YuLinTu.Library.Business.ToolMath.CutNumericFormat(landcoilitem.Shape.Length(), 2);
                    landcoilitem.OrderID = coilindex;
                    landCoils.Add(landcoilitem);
                    coilindex++;
                }
            }
            zonelistSaveDots.AddRange(landDots);
            zonelistSaveCoils.AddRange(landCoils);
        }

        /// <summary>
        /// 处理空户
        /// </summary>
        private void ImportNoContractLandVPDatas(IDbContext localService, List<ComplexRightEntity> entityList, string zoneName = null, string zoneCode = null)
        {
            var db = localService.CreateZoneWorkStation();
            List<ResultDbToLocalDb.LocalComplexRightEntity> list = new List<ResultDbToLocalDb.LocalComplexRightEntity>();

            foreach (var item in entityList)
            {
                ResultDbToLocalDb.LocalComplexRightEntity obj = ResultDbToLocalDb.LocalComplexRightEntity.From(item);
                list.Add(obj);
            }

            Dictionary<string, Library.Entity.VirtualPerson> families = new Dictionary<string, Library.Entity.VirtualPerson>();
            var lands = new List<Library.Entity.ContractLand>();
            foreach (var item in list)
            {
                if (item.DKXXS != null && item.DKXXS.Count > 0)
                {
                    foreach (var land in item.DKXXS)
                    {
                        land.OwnerId = item.CBF.ID;
                        land.OwnerName = item.CBF.Name;
                        land.ZoneCode = zoneCode;
                        land.ZoneName = zoneName;
                        land.LandName = land.LandCode.IsNullOrEmpty() ? string.Empty : (dicLandType.ContainsKey(land.LandCode) ? dicLandType[land.LandCode].Name : string.Empty);
                        land.OwnRightType = YuLinTu.Library.Entity.DictionaryTypeInfo.JTTDSYQ;
                        lands.Add(land);
                    }
                }

                families[string.Format("{0}#{1}", item.CBF.FamilyNumber, item.CBF.ZoneCode)] = item.CBF;
            }

            var vplist = families.Select(c => c.Value).ToArray();
            if (vplist.Count() == 0) return;
            localService.Queries.Add(
                localService.CreateQuery<Library.Entity.LandVirtualPerson>().
                AddRange(vplist));
            localService.Queries.Add(
                localService.CreateQuery<Library.Entity.ContractLand>().
                AddRange(lands.ToArray()));
            this.ReportInfomation("成功导入" + zoneName + "下承包方(空户)数据" + vplist.Count().ToString() + "条。");

            localService.Queries.Save();
        }

        /// <summary>
        /// 处理未签合同的地
        /// </summary>
        private int ImportContractLandDatas(IDbContext localService, List<DKEX> dkexList,
            string zoneName, string zoneCode, int fnum)
        {
            if (dkexList == null || dkexList.Count == 0)
                return 0;
            Library.Entity.LandVirtualPerson vp = null;
            if (CreatUnit)
            {
                vp = new Library.Entity.LandVirtualPerson()
                {
                    ID = Guid.NewGuid(),
                    Name = "集体",
                    FamilyNumber = fnum.ToString(),
                    IsStockFarmer = false,
                    SharePersonList = new List<Library.Entity.Person>(),
                    ZoneCode = zoneCode,
                    VirtualType = Library.Entity.eVirtualPersonType.CollectivityTissue,
                    FamilyExpand = new Library.Entity.VirtualPersonExpand() { ContractorType = Library.Entity.eContractorType.Unit }
                };
                localService.Queries.Add(localService.CreateQuery<Library.Entity.LandVirtualPerson>().Add(vp));
            }
            List<Library.Entity.ContractLand> listCL = new List<Library.Entity.ContractLand>();
            //导出成果库里面，地块矢量数据里面只有实测面积，没有确权及台账面积，导回来就会空
            Dictionary<Guid, string> LandguidkjzbList = new Dictionary<Guid, string>();
            List<Library.Entity.ContractLand> listhandleCL = new List<Library.Entity.ContractLand>();
            Library.Entity.ContractLand cbd = null;
            foreach (var dkexitem in dkexList)
            {
                cbd = LocalComplexRightEntity.InitalizeSpaceLandData(dkexitem);
                LandguidkjzbList.Add(cbd.ID, dkexitem.KJZB);
                listhandleCL.Add(cbd);
            }

            //当前地域下的所有界址点线
            var zonelistDots = new List<Library.Entity.BuildLandBoundaryAddressDot>();
            var zonelistCoils = new List<Library.Entity.BuildLandBoundaryAddressCoil>();
            //要保存的
            var zonelistSaveDots = new List<Library.Entity.BuildLandBoundaryAddressDot>();
            var zonelistSaveCoils = new List<Library.Entity.BuildLandBoundaryAddressCoil>();

            if (GenerateCoilDot)
            {
                this.ReportInfomation("开始导入" + zoneName + "下未签合同地块对应界址数据");
                zonelistDots = GetDataListFromShapes<Library.Entity.BuildLandBoundaryAddressDot>((currentPath.ShapeFileList.FindAll(t => t.Name == JZD.TableName)), "LandNumber", zoneCode, true);
                zonelistCoils = GetDataListFromShapes<Library.Entity.BuildLandBoundaryAddressCoil>((currentPath.ShapeFileList.FindAll(t => t.Name == JZX.TableName)), "LandNumber", zoneCode, true);
            }

            //var prefix = GetPrefix(zonelistDots.FirstOrDefault());
            List<Library.Entity.BuildLandBoundaryAddressDot> landDots = null;
            List<Library.Entity.BuildLandBoundaryAddressCoil> landCoils = null;

            foreach (var land in listhandleCL)
            {
                land.ZoneCode = zoneCode;
                land.ZoneName = zoneName;
                land.LandName = land.LandCode.IsNullOrEmpty() ? string.Empty : (dicLandType.ContainsKey(land.LandCode) ? dicLandType[land.LandCode].Name : string.Empty);
                land.OwnRightType = YuLinTu.Library.Entity.DictionaryTypeInfo.JTTDSYQ;
                if (vp != null)
                {
                    land.OwnerId = vp.ID;
                    land.OwnerName = vp.Name;
                }
                listCL.Add(land);

                #region 处理生成界址信息

                if (GenerateCoilDot)
                {
                    landDots = new List<Library.Entity.BuildLandBoundaryAddressDot>();
                    landCoils = new List<Library.Entity.BuildLandBoundaryAddressCoil>();

                    var landnumber = land.LandNumber;
                    if (landnumber.IsNullOrEmpty()) continue;
                    if (zonelistDots == null || zonelistCoils == null) continue;
                    if (zonelistDots.Count == 0 || zonelistCoils.Count == 0) continue;

                    var landlistDots = zonelistDots.FindAll(dd => dd.LandNumber == landnumber);
                    var landlistCoils = zonelistCoils.FindAll(dc => dc.LandNumber == landnumber);
                    if (landlistDots == null || landlistCoils == null) continue;
                    if (landlistDots.Count == 0 || landlistCoils.Count == 0) continue;

                    var landKJZBvalue = LandguidkjzbList.Where(t => t.Key == land.ID).Select(v => v.Value).FirstOrDefault();
                    if (landKJZBvalue.IsNullOrEmpty()) continue;
                    //shape中地块统编界址点号
                    List<string> tbjzdh = new List<string>();
                    if (landKJZBvalue.Contains("/"))
                    {
                        tbjzdh = landKJZBvalue.Split('/').ToList();
                        if (tbjzdh.Count == 0) continue;
                    }

                    //找出空间坐标对应的界址点
                    int index = 1;
                    foreach (var tbjzdhitem in tbjzdh)
                    {
                        var item = landlistDots.Find(d => d.DotNumber == tbjzdhitem);
                        if (item == null) continue;
                        var landdotitem = new Library.Entity.BuildLandBoundaryAddressDot();
                        landdotitem.CopyPropertiesFrom(item);
                        landdotitem.ID = Guid.NewGuid();
                        landdotitem.UniteDotNumber = tbjzdhitem;
                        landdotitem.IsValid = false;
                        landdotitem.DotNumber = GetPrefix(item) + index;
                        landdotitem.LandID = land.ID;
                        landdotitem.ZoneCode = zoneCode;
                        if (landDots.Any(ld => ld.ID == landdotitem.ID) == false)
                        {
                            landDots.Add(landdotitem);
                            index++;
                        }
                    }
                    //处理界址线
                    short coilindex = 1;
                    foreach (var item in landlistCoils)
                    {
                        var landcoilitem = new Library.Entity.BuildLandBoundaryAddressCoil();
                        landcoilitem.CopyPropertiesFrom(item);
                        landcoilitem.ID = Guid.NewGuid();
                        landcoilitem.LandID = land.ID;
                        landcoilitem.ZoneCode = zoneCode;
                        if (landcoilitem.Position.StartsWith("0") == false)
                        {
                            landcoilitem.Position = "0" + landcoilitem.Position;
                        }
                        var qjzd = landDots.Find(ld => ld.UniteDotNumber == landcoilitem.StartNumber);
                        if (qjzd != null)
                        {
                            qjzd.IsValid = true;
                            landcoilitem.StartPointID = qjzd.ID;
                            landcoilitem.StartNumber = qjzd.DotNumber;
                        }

                        var zjzd = landDots.Find(ld => ld.UniteDotNumber == landcoilitem.EndNumber);
                        if (zjzd != null)
                        {
                            zjzd.IsValid = true;
                            landcoilitem.EndPointID = zjzd.ID;
                            landcoilitem.EndNumber = zjzd.DotNumber;
                        }

                        if (landCoils.Any(ld => ld.ID == landcoilitem.ID) == false)
                        {
                            landcoilitem.CoilLength = YuLinTu.Library.Business.ToolMath.CutNumericFormat(landcoilitem.Shape.Length(), 2);
                            landcoilitem.OrderID = coilindex;
                            landCoils.Add(landcoilitem);
                            coilindex++;
                        }
                    }
                    zonelistSaveDots.AddRange(landDots);
                    zonelistSaveCoils.AddRange(landCoils);
                }

                #endregion 处理生成界址信息
            }

            localService.Queries.Add(
                localService.CreateQuery<Library.Entity.ContractLand>().
                AddRange(listCL.ToArray()));
            string jzdxInfo = "";
            if (GenerateCoilDot)
            {
                localService.Queries.Add(
                    localService.CreateQuery<Library.Entity.BuildLandBoundaryAddressDot>().
                    AddRange(zonelistSaveDots.ToArray()));

                localService.Queries.Add(
                    localService.CreateQuery<Library.Entity.BuildLandBoundaryAddressCoil>().
                    AddRange(zonelistSaveCoils.ToArray()));
                jzdxInfo = ",界址点数据" + zonelistSaveDots.Count.ToString() + "条,界址线数据" + zonelistSaveCoils.Count.ToString() + "条";
            }

            if (listCL.Count > 0)
            {
                //this.ReportInfomation("成功导入" + zoneName + "下未签合同地块" + listCL.Count.ToString() + "条" + jzdxInfo);
                localService.Queries.Save();
            }
            return listCL.Count;
        }

        private string GetPrefix(Library.Entity.BuildLandBoundaryAddressDot dot)
        {
            if (dot == null)
                return "";
            var dotNumber = dot.DotNumber;
            var prefix = dotNumber.IsNullOrEmpty() ? "" : dotNumber.Substring(0, 1);
            var regex = new System.Text.RegularExpressions.Regex("[a-zA-Z]");
            if (regex.IsMatch(prefix))
            {
                return prefix;
            }
            return "";
        }

        #region 获取Shape数据

        /// <summary>
        /// 导入空间数据
        /// </summary>
        private bool ImportSpaceData(string zoneCode, IDbContext localService, SpaceDataOperator shpProcess)
        {
            bool result = true;
            int maxRecord = 1000;
            string info = string.Empty;
            string operatName = "控制点、农田保护区、点(线、面)状地物、区域界线数据";

            int kzdcount = 0;
            int jbntbhqcount = 0;
            int qyjxcount = 0;
            int dzdwcount = 0;
            int xzdwcount = 0;
            int mzdwcount = 0;

            try
            {
                if (FileZone.ImportFile.VictorKZD.IsExport)
                    UploadDataByGroup<KZD>(shpProcess, maxRecord, localService, (en, data) => { en.KZD = data; en.ZoneCode = zoneCode; kzdcount += data.Count; });

                if (FileZone.ImportFile.VictorJBNTBHQ.IsExport)
                    UploadDataByGroup<JBNTBHQ>(shpProcess, maxRecord, localService, (en, data) => { en.JBNTBHQ = data; en.ZoneCode = zoneCode; jbntbhqcount += data.Count; });

                if (FileZone.ImportFile.VictorQYJX.IsExport)
                    UploadDataByGroup<QYJX>(shpProcess, maxRecord, localService, (en, data) => { en.QYJX = data; en.ZoneCode = zoneCode; qyjxcount += data.Count; });

                if (FileZone.ImportFile.VictorDZDW.IsExport)
                    UploadDataByGroup<DZDW>(shpProcess, maxRecord, localService, (en, data) => { en.DZDW = data; en.ZoneCode = zoneCode; dzdwcount += data.Count; });

                if (FileZone.ImportFile.VictorXZDW.IsExport)
                    UploadDataByGroup<XZDW>(shpProcess, maxRecord, localService, (en, data) => { en.XZDW = data; en.ZoneCode = zoneCode; xzdwcount += data.Count; });

                if (FileZone.ImportFile.VictorMZDW.IsExport)
                    UploadDataByGroup<MZDW>(shpProcess, maxRecord, localService, (en, data) => { en.MZDW = data; en.ZoneCode = zoneCode; mzdwcount += data.Count; });

                this.ReportInfomation("成功导入" + "控制点" + kzdcount + "条，" + "农田保护区" + jbntbhqcount + "条，" + "区域界线" + qyjxcount + "条，" + "点状地物" + dzdwcount + "条，" + "线状地物" + xzdwcount + "条，" + "面状地物" + mzdwcount + "条，");
            }
            catch (Exception ex)
            {
                result = false;
                string errorInfo = "导入" + operatName + "发生错误:" + ex;
                this.ReportError(errorInfo);
                LogWrite.WriteErrorLog(errorInfo, ex);
            }
            return result;
        }

        /// <summary>
        /// 分组上传数据
        /// </summary>
        private void UploadDataByGroup<T>(SpaceDataOperator shpProcess, int number, IDbContext localService, Action<ComplexSpaceEntity, List<T>> SetData) where T : class, new()
        {
            string msg = "";
            var dataList = shpProcess.GetShapeData<T>(ref msg);
            if (dataList == null || dataList.Count == 0)
                return;
            var enumList = dataList.TakeGroup(number);

            foreach (var item in enumList)
            {
                var spaceEntity = new ComplexSpaceEntity();
                SetData(spaceEntity, item);
                UploadExtraSpatialData(localService, spaceEntity);
            }
        }

        /// <summary>
        /// 获取数据
        /// </summary>
        /// <param name="fnp"></param>
        /// <returns></returns>
        private List<T> GetDataListFromShapes<T>(List<FileCondition> fnps, string mainField = "", string zoneCode = "", bool isSelectDotCoil = false) where T : class, new()
        {
            var list = new List<T>();
            if (fnps == null || fnps.Count == 0)
            {
                return list;
            }
            foreach (var item in fnps)
            {
                var ts = GetDataFromShape<T>(item, mainField, zoneCode, isSelectDotCoil);
                list.AddRange(ts);
            }
            return list;
        }

        /// <summary>
        /// 获取数据
        /// </summary>
        /// <param name="fnp"></param>
        /// <returns></returns>
        private List<T> GetDataFromShape<T>(FileCondition fnp, string mainField = "", string zoneCode = "", bool isSelectDotCoil = false) where T : class, new()
        {
            var list = new List<T>();
            if (fnp == null || string.IsNullOrEmpty(fnp.FilePath))
            {
                return list;
            }
            using (var dataReader = new ShapefileDataReader(fnp.FilePath, GeometryFactory.Default))
            {
                foreach (var dk in ForEnumRecord<T>(dataReader, fnp.FilePath, mainField, zoneCode, isSelectDotCoil))
                {
                    list.Add(dk);
                }
            }
            return list;
        }

        private void UploadExtraSpatialData(IDbContext localService, ComplexSpaceEntity spaceEntity)
        {
            try
            {
                localService.BeginTransaction();

                var obj = ResultDbToLocalDb.LocalComplexSpaceEntity.From(spaceEntity);
                var db = localService.CreateZoneWorkStation();

                foreach (var item in obj.MZDW)
                {
                    item.ZoneCode = obj.ZoneCode;
                    db.Add<Library.Entity.MZDW>(item);
                }
                foreach (var item in obj.XZDW)
                {
                    item.ZoneCode = obj.ZoneCode;
                    db.Add<Library.Entity.XZDW>(item);
                }
                foreach (var item in obj.DZDW)
                {
                    item.ZoneCode = obj.ZoneCode;
                    db.Add<Library.Entity.DZDW>(item);
                }
                foreach (var item in obj.QYJX)
                {
                    item.ZoneCode = obj.ZoneCode;
                    db.Add<Library.Entity.ZoneBoundary>(item);
                }
                foreach (var item in obj.KZD)
                {
                    item.ZoneCode = obj.ZoneCode;
                    db.Add<Library.Entity.ControlPoint>(item);
                }
                foreach (var item in obj.JBNTBHQ)
                {
                    item.ZoneCode = obj.ZoneCode;
                    db.Add<Library.Entity.FarmLandConserve>(item);
                }

                localService.CommitTransaction();
            }
            catch (Exception ex)
            {
                localService.RollbackTransaction();
                throw ex;
            }
            finally
            {
            }
        }

        #endregion 获取Shape数据

        #region 辅助方法

        /// <summary>
        /// 报告地域错误
        /// </summary>
        /// <param name="countZone"></param>
        private bool CheckZoneInfo(List<Zone> zoneCollection)
        {
            bool result = true;
            bool exitVillige = false;
            bool exitGroup = false;
            foreach (var z in zoneCollection)
            {
                z.FullCode = z.FullCode == null ? "" : z.FullCode;
                if ((string.IsNullOrEmpty(z.UpLevelCode) || string.IsNullOrEmpty(z.UpLevelName))
                    && z.FullCode.Length > 2)
                {
                    hasError = true;
                    this.ReportError(string.Format("{0}({1})的上级地域在数据成果和系统中不存在,不能执行导入!", z.Name, z.FullCode));
                    result = false;
                }
                if (z.Level == eZoneLevel.Village)
                {
                    exitVillige = true;
                }
                if (z.Level == eZoneLevel.Group)
                {
                    exitGroup = true;
                }
            }
            if (!exitVillige)
            {
            }
            if (!exitGroup)
            {
            }
            return result;
        }

        /// <summary>
        /// 获取Srid
        /// </summary>
        private int GetSrid(IDbContext localService)
        {
            var targetSpatialReference = localService.CreateSchema().GetElementSpatialReference(
                    ObjectContext.Create(typeof(Library.Entity.ContractLand)).Schema,
                    ObjectContext.Create(typeof(Library.Entity.ContractLand)).TableName);
            if (targetSpatialReference == null)
                throw new Exception("无法获取到本地是数据库表ZD_CBD的坐标信息");
            return targetSpatialReference.WKID;
        }

        private SpatialReference GetSridFromDK()
        {
            var landfile = currentPath.ShapeFileList.Find(t => t.Name == "DK");
            var landprj = Path.ChangeExtension(landfile.FilePath, ".prj");
            if (File.Exists(landprj))
            {
                var filebytes = File.ReadAllBytes(landprj);
                var gstr = Encoding.Default.GetString(filebytes);
                if (gstr.StartsWith("PROJCS["))
                {
                    string epsgstr = GetContentFromString(gstr, "AUTHORITY", "\"]");
                    epsgstr = epsgstr.Length > 10 ? epsgstr.Substring(6, 4) : "";
                    int epsgint = 0;
                    int.TryParse(epsgstr, out epsgint);
                    if (epsgint > 0)
                    {
                        return new SpatialReference(epsgint);
                    }
                    else
                    {
                        throw new Exception("地块矢量数据的坐标文件EPSG值不正确：" + gstr);
                    }
                }
                else
                {
                    throw new Exception("地块矢量数据的坐标文件内容无法解析：" + gstr);
                }
            }
            else
            {
                throw new Exception("未找到地块矢量数据的坐标文件：" + landprj);
            }
            return null;
        }

        protected string GetContentFromString(string complateString, string compareString, string splitString)
        {
            if (string.IsNullOrEmpty(complateString))
            {
                return string.Empty;
            }
            int indexstart = complateString.LastIndexOf(compareString);
            if (indexstart == -1)
            {
                return string.Empty;
            }
            string shootStr = string.Empty;
            if (complateString.Length > 10)
            {
                shootStr = complateString.Substring(indexstart + compareString.Length + 2);
                int indexend = shootStr.IndexOf(splitString);
                if (indexend != -1)
                {
                    shootStr = shootStr.Substring(0, indexend);
                }
            }
            return shootStr;
        }


        /// <summary>
        /// 循环获取空间记录
        /// </summary>
        private IEnumerable<T> ForEnumRecord<T>(ShapefileDataReader dataReader, string fileName, string mainField = "", string zoneCode = "", bool isSelectDotCoil = false) where T : class, new()
        {
            DbaseFieldDescriptor[] fileds = dataReader.DbaseHeader.Fields;//shape的字段
            PropertyInfo[] infoArray = typeof(T).GetProperties();
            bool isSelect = (mainField != "" && zoneCode != "") ? true : false;
            List<T> entitys = new List<T>();
            while (dataReader.Read())
            {
                if (dataReader.Geometry == null)
                {
                    continue;
                }
                if (isSelect && isSelectDotCoil == false)
                {
                    var pi = infoArray.Where(t => t.Name == mainField).FirstOrDefault();
                    if (pi == null)
                        continue;
                    int index = GetIndexFromArray(fileds, pi.Name);
                    if (index < 0)
                    {
                        continue;
                    }
                    if (!FieldValue(index, dataReader, pi).ToString().StartsWith(zoneCode))
                    {
                        continue;
                    }
                }
                if (isSelectDotCoil)
                {
                    List<string> landdkbms = new List<string>();
                    var pi = infoArray.Where(t => t.Name == mainField).FirstOrDefault();
                    if (pi == null)
                        continue;
                    int index = GetIndexFromArray(fileds, pi.GetAttribute<YuLinTu.Data.DataColumnAttribute>().ColumnName);
                    if (index < 0)
                    {
                        continue;
                    }
                    var landdkbm = FieldValue(index, dataReader, pi).ToString();
                    if (landdkbm.Contains("/"))
                    {
                        landdkbms = landdkbm.Split('/').ToList();
                        foreach (var dkbmitem in landdkbms)
                        {
                            if (!dkbmitem.StartsWith(zoneCode)) continue;
                            T entitydotcoil = new T();
                            for (int i = 0; i < infoArray.Length; i++)
                            {
                                PropertyInfo info = infoArray[i];
                                if (info.Name != "Shape")
                                {
                                    int indexdotcoil = GetIndexFromArray(fileds, info.GetAttribute<YuLinTu.Data.DataColumnAttribute>().ColumnName);
                                    if (indexdotcoil < 0)
                                    {
                                        continue;
                                    }

                                    info.SetValue(entitydotcoil, FieldValue(indexdotcoil, dataReader, info, true, true, dkbmitem), null);
                                }
                            }
                            ObjectExtension.SetPropertyValue(entitydotcoil, "Shape", SetGeometry(dataReader.Geometry, srid));
                            entitys.Add(entitydotcoil);
                        }
                    }
                    else if (landdkbm.StartsWith(zoneCode))
                    {
                        T entitydotcoil = new T();
                        for (int i = 0; i < infoArray.Length; i++)
                        {
                            PropertyInfo info = infoArray[i];
                            if (info.Name != "Shape")
                            {
                                int indexdotcoil = GetIndexFromArray(fileds, info.GetAttribute<YuLinTu.Data.DataColumnAttribute>().ColumnName);
                                if (indexdotcoil < 0)
                                {
                                    continue;
                                }
                                info.SetValue(entitydotcoil, FieldValue(indexdotcoil, dataReader, info, true), null);
                            }
                        }
                        ObjectExtension.SetPropertyValue(entitydotcoil, "Shape", SetGeometry(dataReader.Geometry, srid));
                        entitys.Add(entitydotcoil);
                    }
                }
                else
                {
                    T entity = new T();
                    for (int i = 0; i < infoArray.Length; i++)
                    {
                        PropertyInfo info = infoArray[i];
                        int index = GetIndexFromArray(fileds, info.Name);
                        if (index < 0)
                        {
                            continue;
                        }
                        info.SetValue(entity, FieldValue(index, dataReader, info), null);
                    }
                    ObjectExtension.SetPropertyValue(entity, "Shape", SetGeometry(dataReader.Geometry, srid));
                    //yield return entity;
                    entitys.Add(entity);
                }
            }
            return entitys;
        }

        /// <summary>
        /// 字段值获取
        /// </summary>
        private object FieldValue(int index, ShapefileDataReader dataReader, PropertyInfo info, bool isSelectDotCoil = false, bool islongDKBM = false, string dotcoildkbm = "")
        {
            object value = null;
            string dotcoilinfo = "";
            if (isSelectDotCoil)
            {
                dotcoilinfo = info.GetAttribute<YuLinTu.Data.DataColumnAttribute>().ColumnName;
            }
            if (info.Name == "BSM")
            {
                int bsm = 0;
                int.TryParse(dataReader.GetValue(index).ToString(), out bsm);
                value = bsm;
            }
            else if (info.Name.Contains("MJ") || info.Name == "CD" || info.Name == "KD" ||
                 info.Name == "X80" || info.Name == "Y80" || info.Name == "Y2000" || info.Name == "X2000")
            {
                double scmj = 0;
                double.TryParse(dataReader.GetValue(index).ToString(), out scmj);
                value = scmj;
            }
            else if (isSelectDotCoil && (dotcoilinfo == "DKID" || dotcoilinfo == "DKBS" || dotcoilinfo == "JZXQD" || dotcoilinfo == "JZXZD"))
            {
                value = Guid.NewGuid();
            }
            else if (isSelectDotCoil && islongDKBM && dotcoilinfo == "DKBM")
            {
                value = dotcoildkbm;
            }
            else
            {
                var obj = dataReader.GetValue(index);
                //var bytes = ObjectToByte(obj);
                value = obj.ToString();// ncoding.UTF8.GetString(bytes);// dataReader.GetValue(index).ToString();
            }
            return value;
        }

        /// <summary>
        /// objectTobyte
        /// </summary>
        /// <returns></returns>
        private byte[] ObjectToByte(object obj)
        {
            byte[] buff;
            using (MemoryStream stream = new MemoryStream())
            {
                IFormatter fromat = new BinaryFormatter();
                fromat.Serialize(stream, obj);
                buff = stream.GetBuffer();
            }
            return buff;
        }

        /// <summary>
        /// 获取图形SRID
        /// </summary>
        private int GetGeometryID(string fileName)
        {
            int srid = 0;
            string prjfileName = Path.ChangeExtension(fileName, ".prj");
            StreamReader sr = new StreamReader(prjfileName, System.Text.Encoding.GetEncoding("GB2312"));
            string sridString = sr.ReadToEnd();
            sridString = sridString.Substring(sridString.Length - 6);
            sridString = sridString.Replace("]]", "");
            if (!string.IsNullOrEmpty(sridString))
                int.TryParse(sridString, out srid);
            return srid;
        }

        /// <summary>
        /// 设置图形信息
        /// </summary>
        private object SetGeometry(GeoAPI.Geometries.IGeometry value, int srid)
        {
            value.SRID = srid;
            YuLinTu.Spatial.Geometry geo = YuLinTu.Spatial.Geometry.FromInstance(value);
            return geo;
        }

        /// <summary>
        /// 获取字段index
        /// </summary>
        private int GetIndexFromArray(DbaseFieldDescriptor[] fileds, string name)
        {
            for (int i = 0; i < fileds.Length; i++)
            {
                DbaseFieldDescriptor filed = fileds[i];
                if (filed.Name == name)
                    return i;
                if (GenerateCoilDot)
                {
                    if (filed.Name == "QJZDH" && name == "JZXQDH"
                        || filed.Name == "ZJZDH" && name == "JZXZDH")
                    {
                        return i;
                    }
                }
            }
            return -1;
        }

        /// <summary>
        /// 进度报告
        /// </summary>
        private void infoCheck_ProgressChanged(object sender, TaskProgressChangedEventArgs e)
        {
            this.ReportProgress(e);
        }

        /// <summary>
        /// 消息报告
        /// </summary>
        private void infoCheck_Alert(object sender, TaskAlertEventArgs e)
        {
            if (QualityAlert != null)
                QualityAlert(e);
        }


        private Dictionary<string, string> GetZoneNameProperties()
        {
            Dictionary<string, string> dictionary = new Dictionary<string, string>();
            var divisionCodeDic = new Library.Entity.DivisionCodeDictionary();
            try
            {
                divisionCodeDic.Deserialize();
                foreach (var item in divisionCodeDic.Items)
                {
                    dictionary.Add(item.Code, item.Name);
                }
                divisionCodeDic.Serialize();
            }
            catch
            {
            }
            return dictionary;
        }
        #endregion 辅助方法

        #endregion Methods
    }
}