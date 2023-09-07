/*
 * (C) 2015  鱼鳞图公司版权所有,保留所有权利 
 */
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using YuLinTu;
using YuLinTu.Data;
using YuLinTu.Library.Business;
using YuLinTu.Library.Entity;

namespace YuLinTu.Component.CombinationDataBaseTask
{
    /// <summary>
    /// 合并SQLite数据库任务
    /// </summary>
    [TaskDescriptor(IsLanguageName = false, Name = "合并数据库数据",
        Gallery = "调查数据库处理",
        UriImage16 = "pack://application:,,,/YuLinTu.Resources;component/Images/16/store.png",
        UriImage24 = "pack://application:,,,/YuLinTu.Resources;component/Images/24/store.png")]
    public class TaskSQLiteCombination : TaskGroup
    {
        #region Ctor

        /// <summary>
        /// 构造函数
        /// </summary>
        public TaskSQLiteCombination()
        {
            Name = "合并数据库数据";
            Description = "合并数据库数据";
            isCoverDataByZoneLevel = false;
            isBatchCombination = false;
        }

        #endregion

        #region Fields

        private Zone currentZone; //当前地域
        private IDbContext dbContextTarget;  //待合并数据源
        private IDbContext dbContextLocal;  //本地数据源
        private bool isCoverDataByZoneLevel;  //是否覆盖数据
        private bool isBatchCombination;  //是否批量合并数据库
        private double averagePercent;  //平均百分比
        private double currentPercent;  //当前百分比
        //private bool isBatch;  //标记是否批量

        #endregion

        #region Methods

        #region Method - Override

        /// <summary>
        /// 开始执行任务
        /// </summary>
        protected override void OnGo()
        {
            this.ReportProgress(0, "开始验证合并数据库参数...");
            this.ReportInfomation("开始验证合并数据库参数...");
            System.Threading.Thread.Sleep(200);
            if (!ValidateArgs())
            {
                this.ReportProgress(100);
                return;
            }
               

            if (!isBatchCombination)
            {
                this.ReportProgress(1, "开始合并...");
                this.ReportInfomation("开始合并...");
                System.Threading.Thread.Sleep(200);
                try
                {
                    if (!BuildDataBaseCombinationPro())
                    {
                        this.ReportError(string.Format("数据库合并出错!"));
                        this.ReportProgress(100);
                        return;
                    }
                }
                catch (Exception ex)
                {
                    YuLinTu.Library.Log.Log.WriteException(this, "OnGo(数据库合并失败!)", ex.Message + ex.StackTrace);
                    this.ReportError(string.Format("数据库合并出错!"));
                    this.ReportProgress(100);
                    return;
                }

                this.ReportProgress(100);
                this.ReportInfomation("合并完成。");
            }
            else
            {
                Clear();
                var args = Argument as TaskSQLiteCombinationArgument;
                if (args.DatabaseFilePath.EndsWith(".sqlite"))
                {
                    this.ReportError("批量合库时请选择数据库所在文件夹路径");
                    this.ReportProgress(100);
                    return;
                }
                string[] arrayDataBase = Directory.GetFiles(args.DatabaseFilePath);
                if (arrayDataBase.Length == 0)
                {
                    this.ReportProgress(100);
                    return;
                }
                foreach (var filePath in arrayDataBase)
                {
                    TaskSQLiteCombination taskGroup = new TaskSQLiteCombination();
                    taskGroup.Name = "批量合并数据库";
                    taskGroup.Description = "批量合并数据库";
                    taskGroup.Argument = new TaskSQLiteCombinationArgument()
                    {
                        DatabaseFilePath = filePath,
                        IsBatchCombination = false,
                        IsCoverDataByZoneLevel = isCoverDataByZoneLevel,
                    };
                    //dbContextTarget = DataBaseSource.GetDataBaseSourceByPath(filePath);
                    this.Add(taskGroup);
                }
                base.OnGo();
            }
        }

        #endregion

        #region Method - Private - Validate

        /// <summary>
        /// 参数合法性检查
        /// </summary>
        private bool ValidateArgs()
        {
            var args = Argument as TaskSQLiteCombinationArgument;

            if (args == null)
            {
                this.ReportError(string.Format("参数错误!"));
                return false;
            }
            isCoverDataByZoneLevel = args.IsCoverDataByZoneLevel;
            isBatchCombination = args.IsBatchCombination;
            if (!isBatchCombination && args.DatabaseFilePath.IsNullOrBlank())
            {
                this.ReportError(string.Format("请选择待合并数据库。"));
                return false;
            }
            if (isBatchCombination && args.DatabaseFilePath.IsNullOrBlank())
            {
                this.ReportError(string.Format("请选择待批量合并数据库文件夹。"));
                return false;
            }
            //if (!isBatchCombination && !isBatch && args.ZoneNameAndCode.IsNullOrBlank())
            //{
            //    this.ReportError(string.Format("地域不能为空，请选择地域信息。"));
            //    return false;
            //}
            try
            {
                dbContextLocal = DataBaseSource.GetDataBaseSource();
                if (!isBatchCombination)
                {
                    dbContextTarget = DataBaseSource.GetDataBaseSourceByPath(args.DatabaseFilePath);
                    if (dbContextTarget == null || dbContextLocal == null)
                    {
                        this.ReportError(DataBaseSource.ConnectionError);
                        return false;
                    }
                    var zoneStation = dbContextTarget.CreateZoneWorkStation();
                    if (args.ZoneNameAndCode.IsNullOrBlank())
                    {
                        currentZone = zoneStation.Get(c => c.Level == eZoneLevel.State).FirstOrDefault();
                    }
                    else
                    {
                        string fullCode = (args.ZoneNameAndCode.Split('#'))[1];
                        currentZone = zoneStation.Get(c => c.FullCode == fullCode).FirstOrDefault();
                    }
                    if (currentZone == null)
                    {
                        this.ReportError(string.Format("地域参数错误!"));
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                YuLinTu.Library.Log.Log.WriteException(this, "ValidateArgs(参数合法性检查失败!)", ex.Message + ex.StackTrace);
                return false;
            }
            this.ReportInfomation(string.Format("合并数据库数据参数正确。"));
            return true;
        }

        #endregion

        #region Method - Private - Pro

        /// <summary>
        /// 合并数据库业务
        /// </summary>
        private bool BuildDataBaseCombinationPro()
        {
            try
            {
                if (!UpgradeDatabase())   //默认升级数据库
                {
                    this.ReportError("待合并数据库未升级到最新版,请检查!");
                    return false;
                }
                if (!CompareDatabaseCoordinate())
                {
                    this.ReportError("待合并数据库坐标系与系统数据库不一致,请检查!");
                    return false;
                }

                var createWsTarget = new CreateWorkStation();
                createWsTarget.Create(dbContextTarget);
                var createWsLocal = new CreateWorkStation();
                createWsLocal.Create(dbContextLocal);

                List<Zone> zones = currentZone.Level == eZoneLevel.State ? createWsTarget.ZoneStation.GetAll() : createWsTarget.ZoneStation.GetAllZonesToProvince(currentZone);
                List<Zone> selfAndSubsZones = currentZone.Level == eZoneLevel.State ? createWsTarget.ZoneStation.GetAll() : createWsTarget.ZoneStation.GetChildren(currentZone.FullCode, eLevelOption.SelfAndSubs);
                int removeCount = selfAndSubsZones.RemoveAll(c => c.Level > eZoneLevel.Town);
                if (zones == null || zones.Count == 0)
                {
                    return false;
                }
                var allZonesLocal = createWsLocal.ZoneStation.GetAll();
                if (allZonesLocal == null)
                    allZonesLocal = new List<Zone>();

                DataProcess dataProcess = new DataProcess();
                dataProcess.ReportInfo += ReportInfoDelegate;
                if (!dataProcess.ZoneDataProcess(zones, allZonesLocal, dbContextLocal, createWsLocal))
                {
                    this.ReportError("合并行政地域数据失败!");
                    return false;
                }

                int index = 0;  //索引
                selfAndSubsZones.RemoveAll(r => r.Level >= eZoneLevel.Town);
                averagePercent = 99.0 / (double)selfAndSubsZones.Count;

                Dictionary<string, Zone> zonesDict = new Dictionary<string, Zone>();
                selfAndSubsZones.ForEach(c => zonesDict.Add(c.FullCode, c));

                foreach (Zone zone in selfAndSubsZones)
                {
                    if (zone.Level == eZoneLevel.Village)
                    {
                        if (zonesDict.Any(c => c.Key != zone.FullCode && c.Key.StartsWith(zone.FullCode)))
                        {
                            CombinVillageLand(dataProcess, zone, createWsTarget, createWsLocal, isCoverDataByZoneLevel);
                            index++;
                            continue;
                        }
                    }
                    if (!dataProcess.OtherDataProcess(zone, createWsTarget, createWsLocal, isCoverDataByZoneLevel))
                    {
                        this.ReportAlert(eMessageGrade.Error, null, string.Format("合并{0}业务数据失败!", zone.FullName));
                        index++;
                        continue;
                    }
                    currentPercent = 1.0 + averagePercent * (index++);
                    string info = string.Format("{0}数据合并完毕。", zone.FullName);
                    this.ReportProgress((int)currentPercent, zone.FullName);
                    this.ReportInfomation(info);
                }

                if (!dataProcess.SurveyLandDataProcess(createWsTarget, createWsLocal, isCoverDataByZoneLevel))
                {
                    this.ReportWarn(string.Format("调查宗地数据合并失败"));
                }

                zones = null;
                GC.Collect();
            }
            catch (System.Exception ex)
            {
                YuLinTu.Library.Log.Log.WriteException(this, "BuildDataBaseCombinationPro(处理合并数据库业务失败!)", ex.Message + ex.StackTrace);
                this.ReportError("处理合并数据库业务失败!");
                return false;
            }
            finally
            {
                GC.Collect();
            }
            return true;
        }

        /// <summary>
        /// 合并村级地块
        /// </summary>
        private void CombinVillageLand(DataProcess dataProcess, Zone zone, CreateWorkStation createWsTarget,
            CreateWorkStation createWsLocal, bool isCoverDataByZoneLevel, bool isCombination = true)
        {
            dataProcess.BatchImportData<CollectivityTissue>("发包方", zone.FullCode, zone.FullName, isCoverDataByZoneLevel, isCombination,
                   (zoneCode) => { return createWsTarget.SenderStation.GetTissues(zoneCode, eLevelOption.Self); },
                   (zoneCode) => { return createWsLocal.SenderStation.GetTissues(zoneCode, eLevelOption.Self); },
                   (zoneCode) => { return createWsLocal.SenderStation.DeleteByZoneCode(zoneCode, eLevelOption.Self); },
                   (list) => { return createWsLocal.SenderStation.AddRange(list); });

            dataProcess.BatchImportData<ContractLand>("承包地块", zone.FullCode, zone.FullName, isCoverDataByZoneLevel, isCombination,
                    (zoneCode) => { return createWsTarget.ContractLandStation.GetCollection(zoneCode, eLevelOption.Self); },
                    (zoneCode) => { return createWsLocal.ContractLandStation.GetCollection(zoneCode, eLevelOption.Self); },
                    (zoneCode) => { return createWsLocal.ContractLandStation.DeleteByZoneCode(zoneCode, eLevelOption.Self); },
                    (list) => { return createWsLocal.ContractLandStation.AddRange(list); });

            dataProcess.BatchImportData<BuildLandBoundaryAddressDot>("界址点", zone.FullCode, zone.FullName, isCoverDataByZoneLevel, isCombination,
                   (zoneCode) => { return createWsTarget.DotStation.GetByZoneCode(zoneCode, eLevelOption.Self); },
                   (zoneCode) => { return createWsLocal.DotStation.GetByZoneCode(zoneCode, eLevelOption.Self); },
                   (zoneCode) => { return createWsLocal.DotStation.DeleteByZoneCode(zoneCode, eLevelOption.Self); },
                   (list) => { return createWsLocal.DotStation.AddRange(list); }, false);

            dataProcess.BatchImportData<BuildLandBoundaryAddressCoil>("界址线", zone.FullCode, zone.FullName, isCoverDataByZoneLevel, isCombination,
                (zoneCode) => { return createWsTarget.CoilStation.GetByZoneCode(zoneCode, eLevelOption.Self); },
                (zoneCode) => { return createWsLocal.CoilStation.GetByZoneCode(zoneCode, eLevelOption.Self); },
                (zoneCode) => { return createWsLocal.CoilStation.DeleteByZoneCode(zoneCode, eLevelOption.Self); },
                (list) => { return createWsLocal.CoilStation.AddRange(list); }, false);
        }

        /// <summary>
        /// 获取区县数据
        /// </summary>
        /// <returns></returns>
        private List<Zone> GetCountys(CreateWorkStation createWsTarget)
        {
            List<Zone> countys = new List<Zone>();
            string zoneCode = currentZone.FullCode.Length >= 6 ? currentZone.FullCode.Substring(0, 6) : "";
            if (!string.IsNullOrEmpty(zoneCode))
            {
                Zone zoneRoot = createWsTarget.ZoneStation.Get(zoneCode);
                if (zoneRoot != null)
                {
                    countys.Add(zoneRoot);
                }
            }
            if (currentZone.FullCode == "86")
            {
                countys = createWsTarget.ZoneStation.GetByZoneLevel(eZoneLevel.County);
            }
            return countys;
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

        #endregion

        #endregion
    }
}
