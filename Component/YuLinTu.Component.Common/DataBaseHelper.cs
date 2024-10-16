/*
 * (C) 2015  鱼鳞图公司版权所有,保留所有权利
 */

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using YuLinTu;
using YuLinTu.Data;
using YuLinTu.Data.Dynamic;
using YuLinTu.Data.SQLite;
using YuLinTu.Library.Entity;
using YuLinTu.Windows;
using YuLinTu.Windows.Wpf;
using YuLinTu.Appwork;
using Ionic.Zip;
using YuLinTu.Library.Business;
using YuLinTu.Spatial;
using CSScriptLibrary;

namespace YuLinTu.Component.Common
{
    /// <summary>
    /// 应用程序上下文
    /// </summary>
    public class DataBaseHelper
    {
        #region Methods

        public bool TryBackupDatabase()
        {
            try
            {
                var action = TheApp.Current["ApplicationStartupProgressReporter"] as Action<double, string, object>;
                if (action != null)
                    action(0, "正在备份数据库...", null);

                //陈泽林 20161025 备份时需要根据用户设置的文件路径和间隔时间去备份，每次备份记录下最新备份的日期
                SettingsProfileCenter center;
                center = TheApp.Current.GetSystemSettingsProfileCenter();
                SystemSetDefine systemset = SystemSetDefine.GetIntence();
                var db = DataSource.Create<IDbContext>(TheBns.Current.GetDataSourceName());
                TryAddTables(db);

                System.Data.SQLite.SQLiteConnectionStringBuilder b =
                    new System.Data.SQLite.SQLiteConnectionStringBuilder(db.DataSource.ConnectionString);

                var source = b.DataSource;
                if (!File.Exists(source))
                    return false;

                //string target = Path.Combine(TheApp.Current.GetDataPath(), "Backup");
                string target = systemset.BackUpPath;
                if (!Directory.Exists(target))
                    Directory.CreateDirectory(target);

                target = Path.Combine(target, string.Format("{0}.zip", DateTime.Now.ToString("yyyy-MM-dd")));
                if (File.Exists(target))
                    return false;
                TimeSpan d = DateTime.Now - systemset.BackUperDate;
                if (d.Days < systemset.BackDay)
                    return false;
                systemset.BackUperDate = DateTime.Now;
                using (var zip = new ZipFile(Encoding.UTF8))
                {
                    zip.AddFile(source, ".");
                    zip.Save(target);
                    center.Save<SystemSetDefine>();
                }
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return false;
            }
        }

        /// <summary>
        /// 创建数据库 
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        static public IDbContext TryCreateDatabase(string fileName, SpatialReference sr)
        {
            try
            {
                IDbContext db = null;
                Exception ex = null;
                try
                {
                    if (!File.Exists(fileName))
                        throw new ArgumentNullException();

                    using (var stream = File.OpenRead(fileName))
                    {
                        if (stream.Length < 1024 * 1024)
                            throw new ArgumentNullException();
                    }

                    db = ProviderDbCSQLite.CreateDataSourceByFileName(fileName) as IDbContext;
                    var dq = new DynamicQuery(db);
                    var els = dq.GetElementProperties(null, ObjectContext.Create(typeof(Zone)).TableName);
                    if (els.Count == 0)
                        throw new ArgumentNullException();
                }
                catch (Exception e)
                {
                    ex = e;
                }

                if (File.Exists(fileName) && ex == null)
                {
                    TryAddTables(db);
                    return db;
                } 

                var action = TheApp.Current["ApplicationStartupProgressReporter"] as Action<double, string, object>;
                if (action != null)
                    action(0, "正在创建数据库...", null);

                db = ProviderDbCSQLite.CreateNewDatabase(fileName) as IDbContext;
                var schema = db.CreateSchema();

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
                schema.Export(typeof(BelongRelation), sr.WKID);
                schema.Export(typeof(StockConcord), sr.WKID);
                schema.Export(typeof(StockWarrant), sr.WKID);

                schema.Export(typeof(SecondTableLand), sr.WKID);
                schema.Export(typeof(ContractLand), sr.WKID);
                schema.Export(typeof(ContractLandMark), sr.WKID);
                schema.Export(typeof(ContractConcord), sr.WKID);
                schema.Export(typeof(ContractRegeditBook), sr.WKID);
                schema.Export(typeof(ContractRequireTable), sr.WKID);

                schema.Export(typeof(Dictionary), sr.WKID);
                schema.Export(typeof(TopologyErrorPoint), sr.WKID);
                schema.Export(typeof(TopologyErrorPolygon), sr.WKID);
                schema.Export(typeof(TopologyErrorPolyline), sr.WKID);

                if (action != null)
                    action(0, "正在创建索引...", null);

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
                    db.BeginTransaction();

                    string cmds = Properties.Resources.DictionarySQL;

                    var sqls = cmds.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);

                    foreach (var sql in sqls)
                    {
                        var cmd = sql.Trim();
                        if (cmd.IsNullOrBlank())
                            continue;

                        var qc = db.CreateQuery();
                        qc.CommandContext.CommandText.Append(cmd);
                        qc.CommandContext.ExecuteArgument = eDbExecuteType.NonQuery;
                        qc.CommandContext.Type = eCommandType.Edit;
                        qc.Execute();
                    }

                    db.CommitTransaction();
                }
                catch
                {
                    db.RollbackTransaction();
                }

                return db;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return null;
            }
        }

        static public void TryAddTables(IDbContext db)
        {
            var action = TheApp.Current["ApplicationStartupProgressReporter"] as Action<double, string, object>;
            if (action != null)
                action(0, "正在创建索引...", null);

            var schema = db.CreateSchema();
            var sr = schema.GetElementSpatialReference(null, ObjectContext.Create(typeof(Zone)).TableName);
            var srid = sr == null ? 0 : sr.WKID;

            if (!schema.AnyElement(null, ObjectContext.Create(typeof(BuildLandBoundaryAddressCoil)).TableName))
                schema.Export(typeof(BuildLandBoundaryAddressCoil), srid);

            if (!schema.AnyElement(null, ObjectContext.Create(typeof(BuildLandBoundaryAddressDot)).TableName))
                schema.Export(typeof(BuildLandBoundaryAddressDot), srid);

            if (!schema.AnyElement(null, ObjectContext.Create(typeof(TopologyErrorPoint)).TableName))
                schema.Export(typeof(TopologyErrorPoint), srid);

            if (!schema.AnyElement(null, ObjectContext.Create(typeof(TopologyErrorPolyline)).TableName))
                schema.Export(typeof(TopologyErrorPolyline), srid);

            if (!schema.AnyElement(null, ObjectContext.Create(typeof(TopologyErrorPolygon)).TableName))
                schema.Export(typeof(TopologyErrorPolygon), srid);

            schema.CreateIndex(null, ObjectContext.Create(typeof(ContractLand)).TableName, "ID", null, true);
            schema.CreateIndex(null, ObjectContext.Create(typeof(LandVirtualPerson)).TableName, "ID", null, true);
            schema.CreateIndex(null, ObjectContext.Create(typeof(ContractRequireTable)).TableName, "ID", null, true);
            schema.CreateIndex(null, ObjectContext.Create(typeof(ContractRegeditBook)).TableName, "ID", null, true);
            schema.CreateIndex(null, ObjectContext.Create(typeof(ContractConcord)).TableName, "ID", null, true);
            schema.CreateIndex(null, ObjectContext.Create(typeof(BuildLandBoundaryAddressCoil)).TableName, "ID", null, true);
            schema.CreateIndex(null, ObjectContext.Create(typeof(BuildLandBoundaryAddressDot)).TableName, "ID", null, true);

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
        }

        /// <summary>
        /// 设置数据源
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="bForce"></param>
        static public bool TrySetDefaultDatabasePathFirst(string fileName, bool bForce)
        {
            try
            {
                var first = ConfigurationManager.AppSettings.TryGetValue<bool>("FirstRun", true);
                if (!bForce && !first)
                    return false;

                var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                config.AppSettings.Settings["FirstRun"].Value = "false";
                SetDefaultDatabaseName(fileName);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            return false;
        }

        /// <summary>
        /// 升级数据库
        /// </summary>
        /// <param name="dbContextTarget"></param>
        /// <returns></returns>
        static public bool TryUpdateDatabase(IDbContext dbContextTarget)
        {
            UpdateDatabase upDatabase = new UpdateDatabase();
            List<UpgradeDatabase> tableList = UpgradeDatabaseExtent.DeserializeUpgradeDatabaseInfo();
            return upDatabase.UpgradeDatabase(dbContextTarget, tableList);
        }

        static public void SetDefaultDatabaseName(string fileName, bool setanyway = false)
        {
            var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

            var cntString = config.ConnectionStrings.ConnectionStrings[TheBns.Current.GetDataSourceName()];
            if (cntString != null && !setanyway)
                return;

            cntString = config.ConnectionStrings.ConnectionStrings["SourceSQLite"];
            if (cntString == null)
            {
                cntString = new ConnectionStringSettings("SourceSQLite", null, "Common.SQLite");
                config.ConnectionStrings.ConnectionStrings.Add(cntString);
            }
            cntString.ConnectionString = string.Format("Data Source={0}", fileName);

            config.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection("connectionStrings");

            TheBns.Current.SetDataSourceName("SourceSQLite");
            TheBns.Current.TrySave();
        }

        static public void SetDefaulZone(string zonecode, string name)
        {
            var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            config.AppSettings.Settings["DefaultRootZone"].Value = zonecode;
            config.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection("appSettings");
            TheBns.Current.TrySave();

            var center = TheApp.Current.GetSystemSettingsProfileCenter();
            var profile = center.GetProfile<CommonBusinessDefine>();
            var section = profile.GetSection<CommonBusinessDefine>();
            var cbd = section.Settings as CommonBusinessDefine;
            cbd.CurrentZoneFullCode = zonecode;
            cbd.CurrentZoneFullName = name;
            center.Save<CommonBusinessDefine>();
        }

        #endregion
    }
}