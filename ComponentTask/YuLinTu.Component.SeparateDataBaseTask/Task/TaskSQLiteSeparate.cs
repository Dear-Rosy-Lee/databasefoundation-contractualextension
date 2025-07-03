/*
 * (C) 2025  鱼鳞图公司版权所有,保留所有权利
 */

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using YuLinTu;
using YuLinTu.Data;
using YuLinTu.Data.SQLite;
using YuLinTu.Library.Business;
using YuLinTu.Library.Entity;
using YuLinTu.Library.Log;
using YuLinTu.Spatial;

namespace YuLinTu.Component.SeparateDataBaseTask
{
    /// <summary>
    /// 分离SQLite数据库任务
    /// </summary>
    [TaskDescriptor(IsLanguageName = false, Name = "分离数据库数据",
        Gallery = "调查数据库处理",
        UriImage16 = "pack://application:,,,/YuLinTu.Resources;component/Images/16/store.png",
        UriImage24 = "pack://application:,,,/YuLinTu.Resources;component/Images/24/store.png")]
    public class TaskSQLiteSeparate : TaskGroup
    {
        #region Ctor

        /// <summary>
        /// 构造函数
        /// </summary>
        public TaskSQLiteSeparate()
        {
            Name = "分离数据库数据";
            Description = "分离数据库数据";
        }

        #endregion Ctor

        #region Fields

        private Zone currentZone; //当前地域
        private IDbContext dbContextTarget;  //待合并数据源
        private IDbContext dbContextLocal;  //本地数据源
        private double averagePercent;  //平均百分比
        private double currentPercent;  //当前百分比
        private string newDatabaseSavePath;  //分离数据库保存路径

        #endregion Fields

        #region Methods

        #region Method - Override

        /// <summary>
        /// 开始执行任务
        /// </summary>
        protected override void OnGo()
        {
            this.ReportProgress(0, "开始验证分离数据库参数...");
            this.ReportInfomation("开始验证分离数据库参数...");
            System.Threading.Thread.Sleep(200);
            if (!ValidateArgs())
                return;
            //this.ReportProgress(1, "开始分离...");
            //this.ReportInfomation("开始分离...");
            System.Threading.Thread.Sleep(200);
            try
            {
                if (!BuildDataBaseSeparatePro())
                {
                    //this.ReportError(string.Format("数据库分离出错!"));
                    return;
                }
            }
            catch (Exception ex)
            {
                YuLinTu.Library.Log.Log.WriteException(this, "OnGo(数据库分离失败!)", ex.Message + ex.StackTrace);
                this.ReportError(string.Format("数据库分离出错!"));
                return;
            }
            this.ReportProgress(100);
            this.ReportInfomation("分离完成。");
        }

        #endregion Method - Override

        #region Method - Private - Validate

        /// <summary>
        /// 参数合法性检查
        /// </summary>
        private bool ValidateArgs()
        {
            var args = Argument as TaskSQLiteSeparateArgument;
            if (args == null)
            {
                this.ReportError(string.Format("参数错误!"));
                return false;
            }
            if (args.DatabaseFilePath.IsNullOrBlank())
            {
                this.ReportError(string.Format("请选择待分离数据库。"));
                return false;
            }
            if (args.ZoneNameAndCode.IsNullOrBlank())
            {
                this.ReportError(string.Format("地域不能为空,请选择待分离地域信息。"));
                return false;
            }
            if (args.DatabaseSavePath.IsNullOrBlank())
            {
                this.ReportError(string.Format("请选择分离数据库保存路径。"));
                return false;
            }
            try
            {
                newDatabaseSavePath = args.DatabaseSavePath;
                dbContextLocal = DataBaseSource.GetDataBaseSourceByPath(args.DatabaseFilePath);
                if (dbContextLocal == null)
                {
                    this.ReportError(DataBaseSource.ConnectionError);
                    return false;
                }
                var zoneStation = dbContextLocal.CreateZoneWorkStation();
                string fullCode = (args.ZoneNameAndCode.Split('#'))[1];
                currentZone = zoneStation.Get(c => c.FullCode == fullCode).FirstOrDefault();
            }
            catch (Exception ex)
            {
                YuLinTu.Library.Log.Log.WriteException(this, "ValidateArgs(参数合法性检查失败!)", ex.Message + ex.StackTrace);
                return false;
            }
            this.ReportInfomation(string.Format("分离数据库数据参数正确。"));
            return true;
        }

        #endregion Method - Private - Validate

        #region Method - Private - Pro

        /// <summary>
        /// 分离数据库业务
        /// </summary>
        private bool BuildDataBaseSeparatePro()
        {
            try
            {
                if (!UpgradeDatabase(dbContextLocal))
                {
                    this.ReportError("待分离数据库未升级到最新版,请检查!");
                    return false;
                }

                this.ReportProgress(1, "开始创建新数据库...");
                this.ReportInfomation("开始创建新数据库...");
                var localSpatialReference = dbContextLocal.CreateSchema().GetElementSpatialReference(
                    ObjectContext.Create(typeof(Zone)).Schema,
                    ObjectContext.Create(typeof(Zone)).TableName);
                if (!CreateNewDatabase(newDatabaseSavePath, localSpatialReference))
                {
                    this.ReportError("创建新数据库失败,无法进行分库操作!");
                    return false;
                }
                else
                {
                    if (!UpgradeDatabase(dbContextTarget))
                    {
                        this.ReportError("待分离数据库未升级到最新版,请检查!");
                        return false;
                    }
                }
                this.ReportProgress(5, "开始分离新数据库...");
                this.ReportInfomation("开始分离新数据库...");

                CreateWorkStation createWsLocal = new CreateWorkStation();
                createWsLocal.Create(dbContextLocal);
                CreateWorkStation createWsTarget = new CreateWorkStation();
                createWsTarget.Create(dbContextTarget);

                var zoneStationLocal = createWsLocal.ZoneStation;
                var zoneStationTarget = createWsTarget.ZoneStation;
                List<Zone> allZones = currentZone.Level == eZoneLevel.State ? zoneStationLocal.GetAll() : zoneStationLocal.GetAllZonesToProvince(currentZone);
                List<Zone> zones = zoneStationTarget.GetAll();
                if (allZones == null || allZones.Count == 0)
                {
                    this.ReportError("未获取地域信息,无法进行分库操作!");
                    return false;
                }
                allZones.OrderByDescending(c => c.Level);
                if (zones == null)
                    zones = new List<Zone>();

                DataProcess dataProcess = new DataProcess();
                dataProcess.ReportInfo += ReportInfoDelegate;
                if (!dataProcess.ZoneDataProcess(allZones, zones, dbContextTarget, createWsTarget))
                {
                    this.ReportError("分离行政地域数据失败!");
                    return false;
                }

                if (!dataProcess.SurveyLandDataProcess(createWsLocal, createWsTarget, true, false))
                {
                    this.ReportWarn("分离调查宗地数据失败!");
                }

                int removeCount = allZones.RemoveAll(c => c.Level > currentZone.Level || c.Level > eZoneLevel.Town);
                int index = 0;  //索引
                averagePercent = 95.0 / (double)allZones.Count;
                foreach (Zone zone in allZones)
                {
                    if (!dataProcess.OtherDataProcess(zone, createWsLocal, createWsTarget, true, false))
                    {
                        this.ReportAlert(eMessageGrade.Error, null, string.Format("分离{0}业务数据失败!", zone.FullName));
                        index++;
                        continue;
                    }
                    currentPercent = 5.0 + averagePercent * (index++);
                    string info = string.Format("{0}数据分离完毕。", zone.FullName);
                    this.ReportProgress((int)currentPercent, zone.FullName);
                    this.ReportInfomation(info);
                }

                allZones = null;
            }
            catch (System.Exception ex)
            {
                YuLinTu.Library.Log.Log.WriteException(this, "BuildDataBaseSeparatePro(处理分离数据库业务失败!)", ex.Message + ex.StackTrace);
                this.ReportError("处理分离数据库业务失败! 详细错误请查看日志信息");
                return false;
            }
            finally
            {
                GC.Collect();
            }
            return true;
        }


        /// <summary>
        /// 报告信息
        /// </summary>
        public void ReportInfoDelegate(eMessageGrade errortype, string msg)
        {
            if (errortype == eMessageGrade.Warn)
                this.ReportWarn(msg);
            else if (errortype == eMessageGrade.Infomation)
                this.ReportInfomation(msg);
            else if (errortype == eMessageGrade.Error)
                this.ReportError(msg);
        }

        /// <summary>
        /// 默认升级数据库
        /// </summary>
        private bool UpgradeDatabase(IDbContext dbContext)
        {
            UpdateDatabase upDatabase = new UpdateDatabase();
            List<UpgradeDatabase> tableList = UpgradeDatabaseExtent.DeserializeUpgradeDatabaseInfo();
            return upDatabase.UpgradeDatabase(dbContext, tableList);
        }

        /// <summary>
        /// 创建新数据库
        /// </summary>
        /// <returns></returns>
        private bool CreateNewDatabase(string fileName, SpatialReference sr)
        {
            bool creatsucess = true;

            dbContextTarget = ProviderDbCSQLite.CreateNewDatabase(fileName) as IDbContext;
            if (dbContextTarget == null)
                return false;
            var schema = dbContextTarget.CreateSchema();

            schema.Export(typeof(Zone), sr.WKID);
            schema.Export(typeof(CollectivityTissue), sr.WKID);

            schema.Export(typeof(BuildLandBoundaryAddressDot), sr.WKID);
            schema.Export(typeof(BuildLandBoundaryAddressCoil), sr.WKID);

            schema.Export(typeof(CollectivityTissue), sr.WKID);
            schema.Export(typeof(LandVirtualPerson), sr.WKID);
            schema.Export(typeof(CollectiveLandVirtualPerson), sr.WKID);
            schema.Export(typeof(HouseVirtualPerson), sr.WKID);
            schema.Export(typeof(TableVirtualPerson), sr.WKID);
            schema.Export(typeof(WoodVirtualPerson), sr.WKID);
            schema.Export(typeof(YardVirtualPerson), sr.WKID);

            schema.Export(typeof(ZoneBoundary), sr.WKID);
            schema.Export(typeof(XZDW), sr.WKID);
            schema.Export(typeof(MZDW), sr.WKID);
            schema.Export(typeof(FarmLandConserve), sr.WKID);
            schema.Export(typeof(DZDW), sr.WKID);
            schema.Export(typeof(DCZD), sr.WKID);
            schema.Export(typeof(ControlPoint), sr.WKID);

            schema.Export(typeof(SecondTableLand), sr.WKID);
            schema.Export(typeof(ContractLand), sr.WKID);
            schema.Export(typeof(ContractLandMark), sr.WKID);
            schema.Export(typeof(ContractConcord), sr.WKID);
            schema.Export(typeof(ContractRegeditBook), sr.WKID);
            schema.Export(typeof(ContractRequireTable), sr.WKID);
            schema.Export(typeof(BelongRelation), sr.WKID);
            schema.Export(typeof(StockConcord), sr.WKID);
            schema.Export(typeof(StockWarrant), sr.WKID);

            schema.Export(typeof(Dictionary), sr.WKID);
            schema.Export(typeof(TopologyErrorPoint), sr.WKID);
            schema.Export(typeof(TopologyErrorPolygon), sr.WKID);
            schema.Export(typeof(TopologyErrorPolyline), sr.WKID);

            schema.CreateIndex(null, ObjectContext.Create(typeof(ContractLand)).TableName, "ID", null, true);
            schema.CreateIndex(null, ObjectContext.Create(typeof(LandVirtualPerson)).TableName, "ID", null, true);
            schema.CreateIndex(null, ObjectContext.Create(typeof(ContractRequireTable)).TableName, "ID", null, true);
            schema.CreateIndex(null, ObjectContext.Create(typeof(ContractRegeditBook)).TableName, "ID", null, true);
            schema.CreateIndex(null, ObjectContext.Create(typeof(ContractConcord)).TableName, "ID", null, true);
            schema.CreateIndex(null, ObjectContext.Create(typeof(BuildLandBoundaryAddressCoil)).TableName, "ID", null, true);
            schema.CreateIndex(null, ObjectContext.Create(typeof(BuildLandBoundaryAddressDot)).TableName, "ID", null, true);
            schema.CreateIndex(null, ObjectContext.Create(typeof(StockConcord)).TableName, "ID", null, true);
            schema.CreateIndex(null, ObjectContext.Create(typeof(StockWarrant)).TableName, "ID", null, true);

            schema.CreateIndex(null, ObjectContext.Create(typeof(Zone)).TableName, "DYQBM", null);
            schema.CreateIndex(null, ObjectContext.Create(typeof(ContractLand)).TableName, "DKLB", null);
            schema.CreateIndex(null, ObjectContext.Create(typeof(ContractLand)).TableName, "ZLDM", null);
            schema.CreateIndex(null, ObjectContext.Create(typeof(ContractLand)).TableName, "QLRBS", null);
            schema.CreateIndex(null, ObjectContext.Create(typeof(LandVirtualPerson)).TableName, "DYBM", null);
            schema.CreateIndex(null, ObjectContext.Create(typeof(BuildLandBoundaryAddressCoil)).TableName, "DKBS", null);
            schema.CreateIndex(null, ObjectContext.Create(typeof(BuildLandBoundaryAddressCoil)).TableName, "DYDM", null);
            schema.CreateIndex(null, ObjectContext.Create(typeof(BuildLandBoundaryAddressDot)).TableName, "DKID", null);
            schema.CreateIndex(null, ObjectContext.Create(typeof(BuildLandBoundaryAddressDot)).TableName, "DYBM", null);

            schema.CreateSpatialIndex(null, ObjectContext.Create(typeof(Zone)).TableName, "Shape");
            schema.CreateSpatialIndex(null, ObjectContext.Create(typeof(ContractLand)).TableName, "Shape");
            schema.CreateSpatialIndex(null, ObjectContext.Create(typeof(SecondTableLand)).TableName, "Shape");
            schema.CreateSpatialIndex(null, ObjectContext.Create(typeof(ContractLandMark)).TableName, "Shape");
            schema.CreateSpatialIndex(null, ObjectContext.Create(typeof(BuildLandBoundaryAddressCoil)).TableName, "Shape");
            schema.CreateSpatialIndex(null, ObjectContext.Create(typeof(BuildLandBoundaryAddressDot)).TableName, "Shape");
            schema.CreateSpatialIndex(null, ObjectContext.Create(typeof(ControlPoint)).TableName, "Shape");
            schema.CreateSpatialIndex(null, ObjectContext.Create(typeof(DCZD)).TableName, "Shape");
            schema.CreateSpatialIndex(null, ObjectContext.Create(typeof(DZDW)).TableName, "Shape");
            schema.CreateSpatialIndex(null, ObjectContext.Create(typeof(XZDW)).TableName, "Shape");
            schema.CreateSpatialIndex(null, ObjectContext.Create(typeof(MZDW)).TableName, "Shape");
            schema.CreateSpatialIndex(null, ObjectContext.Create(typeof(ZoneBoundary)).TableName, "Shape");
            schema.CreateSpatialIndex(null, ObjectContext.Create(typeof(FarmLandConserve)).TableName, "Shape");

            try
            {
                dbContextTarget.BeginTransaction();
                string cmds = YuLinTu.Library.Business.Properties.Resources.DictionarySQL;

                var sqls = cmds.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);

                foreach (var sql in sqls)
                {
                    var cmd = sql.Trim();
                    if (cmd.IsNullOrBlank())
                        continue;

                    var qc = dbContextTarget.CreateQuery();
                    qc.CommandContext.CommandText.Append(cmd);
                    qc.CommandContext.ExecuteArgument = eDbExecuteType.NonQuery;
                    qc.CommandContext.Type = eCommandType.Edit;
                    qc.Execute();
                }
                dbContextTarget.CommitTransaction();
                //UpgradeDatabase();
                return creatsucess;
            }
            catch (Exception ex)
            {
                dbContextTarget.RollbackTransaction();
                Log.WriteException(this, "创建新数据库", ex.Message + ex.StackTrace);
                return false;
            }
        }

        /// <summary>
        /// 默认升级数据库
        /// </summary>
        private bool UpgradeDatabase()
        {
            UpdateDatabase upDatabase = new UpdateDatabase();
            List<UpgradeDatabase> tableList = UpgradeDatabaseExtent.DeserializeUpgradeDatabaseInfo();
            return upDatabase.UpgradeDatabase(dbContextTarget, tableList);
        }

        /// <summary>
        /// 对比数据源坐标系
        /// </summary>
        private bool CompareDatabaseCoordinate()
        {
            try
            {
                var targetSpatialReference = dbContextTarget.CreateSchema().GetElementSpatialReference(
                    ObjectContext.Create(typeof(Zone)).Schema,
                    ObjectContext.Create(typeof(Zone)).TableName);  //待合并数据库坐标系
                var localSpatialReference = dbContextLocal.CreateSchema().GetElementSpatialReference(
                    ObjectContext.Create(typeof(Zone)).Schema,
                    ObjectContext.Create(typeof(Zone)).TableName);  //系统数据库坐标系
                if (targetSpatialReference.WKID == localSpatialReference.WKID)
                    return true;
                else
                    return false;
            }
            catch
            {
                return false;
            }
        }

        #endregion Method - Private - Pro

        #endregion Methods
    }
}