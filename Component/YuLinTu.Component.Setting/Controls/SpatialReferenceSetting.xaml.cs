/*
 * (C) 2015  鱼鳞图公司版权所有,保留所有权利
 */

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Windows;
using YuLinTu.Windows.Wpf.Metro.Components;
using YuLinTu.Appwork;
using YuLinTu.Windows;
using YuLinTu.Library.Business;
using YuLinTu.Library.Entity;
using YuLinTu.Data;
using YuLinTu.Data.SQLite;
using YuLinTu.Library.Log;
using YuLinTu.Spatial;
using DotSpatial.Projections.Transforms;
using DotSpatial.Projections;
using System.ComponentModel;

namespace YuLinTu.Component.Setting
{
    /// <summary>
    /// 新建数据库
    /// </summary>
    public partial class SpatialReferenceSetting : OptionsEditor, INotifyPropertyChanged
    {
        #region Fields

        private SettingsProfileCenter center = TheApp.Current.GetSystemSettingsProfileCenter();

        private IDbContext dbContext;

        private string defaultDataSourceName = "SourceSQLite";
        private IProviderDbCSQLite provider;
        private SystemSetDefine systemSet = SystemSetDefine.GetIntence();
        //private SystemSetDefine SystemSettingDefine = SystemSetDefine.GetIntence();

        #endregion Fields

        #region Properties

        public SystemSetDefine SystemSettingDefine
        {
            get { return _SystemSettingDefine; }
            set { _SystemSettingDefine = value; NotifyPropertyChanged(() => SystemSettingDefine); }
        }

        private SystemSetDefine _SystemSettingDefine = SystemSetDefine.GetIntence();

        //public SystemSetDefine SystemSettingDefine
        //{
        //    get { return (SystemSetDefine)GetValue(SystemSettingDefineProperty); }
        //    set { SetValue(SystemSettingDefineProperty, value); }
        //}

        //public static readonly DependencyProperty SystemSettingDefineProperty =
        //    DependencyProperty.Register("SystemSettingDefine", typeof(SystemSetDefine), typeof(SpatialReferenceSetting));

        #endregion Properties

        #region Events

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion Events

        #region Ctor

        public SpatialReferenceSetting(IWorkspace workspace)
            : base(workspace)
        {
            InitializeComponent();
            DataContext = this;
        }

        #endregion Ctor

        #region Methods - Protected

        protected void NotifyPropertyChanged(string propertyName)
        {
            var evt = PropertyChanged;
            if (evt == null)
                return;

            evt(this, new PropertyChangedEventArgs(propertyName));
        }

        protected void NotifyPropertyChanged<T>(System.Linq.Expressions.Expression<Func<T>> lambda)
        {
            LambdaPropertyNotifier.NotifyPropertyChanged(
                lambda, name => NotifyPropertyChanged(name));
        }

        #endregion Methods - Protected

        #region Methods - Override

        protected override void OnInstall()
        {
            //dbContext = DataBaseSource.GetDataBaseSource();
        }

        protected override void OnUninstall()
        {
        }

        protected override void OnLoad()
        {
            dbContext = DataBaseSource.GetDataBaseSource();
            if (dbContext == null)
            {
                Dispatcher.Invoke(new Action(() =>
                {
                    mtbNowDBSR.Text = "当前数据源连接异常";
                    SystemSettingDefine = systemSet.Clone() as SystemSetDefine;
                }));
                return;
            }
            else
            {
                provider = dbContext.DataSource as IProviderDbCSQLite;
                var ver = GetBBH();
                var targetSpatialReference = dbContext.CreateSchema().GetElementSpatialReference(
                    ObjectContext.Create(typeof(Zone)).Schema,
                    ObjectContext.Create(typeof(Zone)).TableName);
                if (targetSpatialReference == null) return;
                Dispatcher.Invoke(new Action(() =>
                {
                    VersionNumber.Text = ver;
                    mtbNowDBSR.Text = FormatSpatialReference(targetSpatialReference);
                    SystemSettingDefine = systemSet.Clone() as SystemSetDefine;
                }));
            }
        }

        protected override void OnSave()
        {
            Dispatcher.Invoke(new Action(() =>
            {
                systemSet.BackDay = SystemSettingDefine.BackDay;
                systemSet.BackUpPath = SystemSettingDefine.BackUpPath;
                center.Save<SystemSetDefine>();
            }));
        }

        #endregion Methods - Override

        private string FormatSpatialReference(SpatialReference spatialReference)
        {
            if (spatialReference == null || spatialReference.WKID <= 0)
            {
                return "Unknown WKID is Erro";
            }
            ProjectionInfo info = null;
            try
            {
                info = spatialReference.CreateProjectionInfo();
            }
            catch { }
            if (info == null)
            {
                return "Unknown Info is Null";
            }

            StringBuilder sb = new StringBuilder();
            /*
            投影坐标系: CGCS2000_3_Degree_GK_CM_105E
            投影: Gauss_Kruger
            False_Easting: 500000.00000000
            False_Northing: 0.00000000
            Central_Meridian: 105.00000000
            Scale_Factor: 1.00000000
            Latitude_Of_Origin: 0.00000000
            线性单位: Meter

            地理坐标系: GCS_China_Geodetic_Coordinate_System_2000
            基准面:  D_China_2000
            本初子午线:  Greenwich
            角度单位:  Degree
            EPSG:2380";*/

            sb.Append("标识：");
            sb.Append(spatialReference.WKID);
            sb.AppendLine();

            ITransform tf = info.Transform;
            GeographicInfo geoInfo = info.GeographicInfo;
            bool proj = !info.IsLatLon;
            if (proj)
            {
                sb.Append("投影坐标系：");
                sb.Append(info.Name);
                sb.AppendLine();

                if (tf != null)
                {
                    sb.Append("投影：");
                    sb.Append(tf.Name);
                    sb.AppendLine();
                }
                sb.Append("东伪偏移：");
                sb.Append(info.FalseEasting);
                sb.AppendLine();

                sb.Append("北伪偏移：");
                sb.Append(info.FalseNorthing);
                sb.AppendLine();

                sb.Append("中央经线：");
                sb.Append(info.CentralMeridian);
                sb.AppendLine();

                sb.Append("比例因子：");
                sb.Append(info.ScaleFactor);
                sb.AppendLine();

                sb.Append("起始原点：");
                sb.Append(info.LatitudeOfOrigin);
                sb.AppendLine();

                sb.Append("线性单位：");
                sb.Append(info.Unit.Name);
                sb.Append('（');
                sb.Append(info.Unit.Meters);
                sb.Append('）');
                sb.AppendLine();
                sb.AppendLine();
            }
            if (geoInfo != null)
            {
                sb.Append("地理坐标系：");
                sb.Append(geoInfo.Name);
                sb.AppendLine();

                sb.Append("基准面：");
                sb.Append(geoInfo.Datum.Name);
                sb.AppendLine();

                sb.Append("本初子午线：");
                sb.Append(geoInfo.Meridian.Name);
                sb.Append('（');
                sb.Append(geoInfo.Meridian.Longitude);
                sb.Append('）');
                sb.AppendLine();

                sb.Append("角度单位：");
                sb.Append(geoInfo.Unit.Name);
                sb.Append('（');
                sb.Append(geoInfo.Unit.Radians);
                sb.Append('）');
                sb.AppendLine();
            }

            return sb.ToString();
        }

        //新建代码-新建数据库
        public void mtbCreatNewSqlLitedb_Click(object sender, RoutedEventArgs e)
        {
            SpatialReferenceSetDialogArgument pdbssrb = new SpatialReferenceSetDialogArgument();
            PropertyGridDialog exportPage = new PropertyGridDialog();
            exportPage.Object = pdbssrb;
            exportPage.Header = "建库配置";
            exportPage.PropertyGrid.Properties["workSpace"] = Workspace;
            IDbContext ds = null;
            Workspace.Window.ShowDialog(exportPage);
            exportPage.Confirm += (s, a) =>
            {
                string sfn = string.Empty;
                Spatial.SpatialReference sf = null;
                bool GoOn = false;
                Dispatcher.Invoke(new Action(() =>
                {
                    var srsa = exportPage.Object as SpatialReferenceSetDialogArgument;
                    sfn = srsa.SaveFileName;
                    sf = srsa.SelectSRName;
                    if (sfn == "" || sfn == null || sf == null)
                    {
                        TabMessageBoxDialog messagebox = new TabMessageBoxDialog();
                        messagebox.Message = "未选择新库路径或坐标系";
                        messagebox.Header = "提示";
                        messagebox.MessageGrade = eMessageGrade.Error;
                        messagebox.CancelButtonText = "取消";
                        Workspace.Window.ShowDialog(messagebox);
                        a.Parameter = false;
                    }
                    if (sfn != "" && sf != null && sfn != null) GoOn = true;
                }));
                if (GoOn)
                {
                    ds = ProviderDbCSQLite.CreateNewDatabase(sfn) as IDbContext;
                    bool creatsucess = false;
                    if (sfn != "" && sf != null)
                    {
                        creatsucess = TryCreateDatabase(ds, sfn, sf);
                        UpgradeDatabaseExtent.SerializeUpgradeDatabaseInfo();
                    }
                    if (creatsucess)
                    {
                        TrySetDefaultDatabasePath(sfn, true);

                        Dispatcher.Invoke(new Action(() =>
                        {
                            TheBns.Current.SetDataSourceName(defaultDataSourceName);
                            TheBns.Current.Save();

                            Workspace.Message.Send(this, new SettingsProfileChangedEventArgs()
                            {
                                Profile = new SettingsProfile() { Name = TheBns.stringDataSourceNameChangedMessageKey }
                            });
                            //设置版本号
                            provider = ds.DataSource as IProviderDbCSQLite;
                            SetBBH();
                            RaiseRequestRefresh();//刷新整个配置界面
                        }));
                        a.Parameter = true;
                    }
                    else
                    {
                        a.Parameter = false;//为false就不会关闭
                    }
                }
                else
                {
                    return;
                }
            };
        }

        /// <summary>
        /// 建库
        /// </summary>
        private bool TryCreateDatabase(IDbContext ds, string fileName, Spatial.SpatialReference sr)
        {
            bool creatsucess = true;
            var schema = ds.CreateSchema();

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
                ds.BeginTransaction();
                string cmds = YuLinTu.Component.Common.Properties.Resources.DictionarySQL;

                var sqls = cmds.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);

                foreach (var sql in sqls)
                {
                    var cmd = sql.Trim();
                    if (cmd.IsNullOrBlank())
                        continue;

                    var qc = ds.CreateQuery();
                    qc.CommandContext.CommandText.Append(cmd);
                    qc.CommandContext.ExecuteArgument = eDbExecuteType.NonQuery;
                    qc.CommandContext.Type = eCommandType.Edit;
                    qc.Execute();
                }
                ds.CommitTransaction();
                return creatsucess;
            }
            catch (Exception ex)
            {
                ds.RollbackTransaction();
                Dispatcher.Invoke(new Action(() =>
                  {
                      TabMessageBoxDialog messagebox = new TabMessageBoxDialog();
                      messagebox.Message = ex.Message + ex.StackTrace;
                      messagebox.Header = "提示";
                      messagebox.MessageGrade = eMessageGrade.Error;
                      messagebox.CancelButtonText = "取消";
                      Workspace.Window.ShowDialog(messagebox);
                      Log.WriteException(this, "创建新数据库", ex.Message + ex.StackTrace);
                  }));
                return false;
            }
        }

        /// <summary>
        /// 更改配置显示
        /// </summary>
        private void TrySetDefaultDatabasePath(string fileName, bool bForce)
        {
            try
            {
                var first = ConfigurationManager.AppSettings.TryGetValue<bool>("FirstRun", true);
                if (!first && !bForce)
                    return;

                var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                config.AppSettings.Settings["FirstRun"].Value = "false";

                var cntString = config.ConnectionStrings.ConnectionStrings[defaultDataSourceName];
                if (cntString == null)
                {
                    ConnectionStringSettings nowconss = new ConnectionStringSettings(defaultDataSourceName, string.Format("Data Source={0}", fileName), "Common.SQLite");
                    config.ConnectionStrings.ConnectionStrings.Add(nowconss);
                }
                else
                {
                    cntString.ConnectionString = string.Format("Data Source={0}", fileName);
                }

                config.Save(ConfigurationSaveMode.Modified);
                ConfigurationManager.RefreshSection("appSettings");
                ConfigurationManager.RefreshSection("connectionStrings");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        /// <summary>
        /// 数据库升级
        /// </summary>
        private void updateDatabase_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var dbContext = DataSource.Create<IDbContext>(Workspace.Properties.TryGetValue("CurrentDataSourceName", TheBns.Current.GetDataSourceName()));
                if (dbContext == null)
                {
                    var errorPage = new TabMessageBoxDialog
                    {
                        Header = "更新数据库",
                        Message = "未获取数据源!",
                        MessageGrade = eMessageGrade.Error,
                        CancelButtonText = "确定",
                        ConfirmButtonVisibility = Visibility.Collapsed,
                    };
                    Workspace.Window.ShowDialog(errorPage);
                    return;
                }
                bool serialSuccess = true;
                string path = AppDomain.CurrentDomain.BaseDirectory + "Config";

                //if (System.IO.Directory.Exists(path))
                //    System.IO.Directory.Delete(path, true);

                List<UpgradeDatabase> tableList = UpgradeDatabaseExtent.DeserializeUpgradeDatabaseInfo();
                if (tableList.Count == 0)
                {
                    serialSuccess = UpgradeDatabaseExtent.SerializeUpgradeDatabaseInfo();
                    tableList = UpgradeDatabaseExtent.DeserializeUpgradeDatabaseInfo();
                }
                foreach (var item in tableList)
                {
                    var table = dbContext.CreateSchema().GetElementProperties(null, item.TableName);
                    item.FieldList.RemoveAll(r => table.Any(t => t.ColumnName == r.FieldName));
                }
                tableList.RemoveAll(t => t.FieldList.Count == 0);
                //if (serialSuccess)
                //tableList = 
                if (tableList == null || tableList.Count == 0)
                {
                    var msgPage = new TabMessageBoxDialog
                    {
                        Header = "更新数据库",
                        Message = "数据库已更新为最新!",
                        MessageGrade = eMessageGrade.Infomation,
                        CancelButtonText = "确定",
                        ConfirmButtonVisibility = Visibility.Collapsed,
                    };
                    Workspace.Window.ShowDialog(msgPage);
                    return;
                }
                UpdateDatabase upDatabase = new UpdateDatabase();
                var result = upDatabase.UpgradeDatabase(dbContext, tableList, dbContext);
                if (result)
                {
                    var successPage = new TabMessageBoxDialog
                    {
                        Header = "更新数据库",
                        Message = "更新数据库成功",
                        MessageGrade = eMessageGrade.Infomation,
                        CancelButtonText = "确定",
                        ConfirmButtonVisibility = Visibility.Collapsed,
                    };
                    Workspace.Window.ShowDialog(successPage);
                    SetBBH();
                    VersionNumber.Text = GetBBH();
                }
                else
                {
                    var msgPage = new TabMessageBoxDialog
                    {
                        Header = "更新数据库",
                        Message = "数据库更新失败，请检查升级配置文件是否最新!",
                        MessageGrade = eMessageGrade.Warn,
                        CancelButtonText = "确定",
                        ConfirmButtonVisibility = Visibility.Collapsed,
                    };
                    Workspace.Window.ShowDialog(msgPage);
                    return;
                }
            }
            catch (Exception ex)
            {
                YuLinTu.Library.Log.Log.WriteException(this, "updateDatabase_Click(更新数据库)", ex.Message + ex.StackTrace);
                var errorPage = new TabMessageBoxDialog
                {
                    Header = "更新数据库",
                    Message = "更新数据库失败!",
                    MessageGrade = eMessageGrade.Error,
                    CancelButtonText = "确定",
                    ConfirmButtonVisibility = Visibility.Collapsed,
                };
                Workspace.Window.ShowDialog(errorPage);
            }
        }

        /// <summary>
        /// 设置数据库承包地承包方式为家庭承包
        /// </summary>
        private void SetDataBaseCBFS_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var dbContext = DataBaseSource.GetDataBaseSource();
                if (dbContext == null)
                {
                    var errorPage = new TabMessageBoxDialog
                    {
                        Header = "更新数据库",
                        Message = "未获取数据源!",
                        MessageGrade = eMessageGrade.Error,
                        CancelButtonText = "取消",
                        ConfirmButtonVisibility = Visibility.Collapsed,
                    };
                    Workspace.Window.ShowDialog(errorPage);
                    return;
                }
                var cbdStation = dbContext.CreateContractLandWorkstation();
                cbdStation.SetDataBaseCBDFamilyCBFS();

                var successPage = new TabMessageBoxDialog
                {
                    Header = "更新数据库",
                    Message = "更新数据库成功",
                    MessageGrade = eMessageGrade.Infomation,
                    CancelButtonText = "取消",
                    ConfirmButtonVisibility = Visibility.Collapsed,
                };
                Workspace.Window.ShowDialog(successPage);
            }
            catch (Exception ex)
            {
                YuLinTu.Library.Log.Log.WriteException(this, "updateDatabase_Click(更新数据库)", ex.Message + ex.StackTrace);
                var errorPage = new TabMessageBoxDialog
                {
                    Header = "更新数据库",
                    Message = "更新数据库失败!",
                    MessageGrade = eMessageGrade.Error,
                    CancelButtonText = "取消",
                    ConfirmButtonVisibility = Visibility.Collapsed,
                };
                Workspace.Window.ShowDialog(errorPage);
            }
        }

        #region 版本号

        //陈泽林 20161024  版本号控制
        private string GetBBH()
        {
            try
            {
                int number = provider.GetUserVersion();
                string text = "版本号：V";
                if (number == 0)
                    return "版本号：无";
                string num = number.ToString();
                for (int i = 0; i < num.Length; i++)
                {
                    text += num[i];
                    if (num.Length - i <= 3)
                        text += ".";
                }
                text = text.TrimEnd('.');
                return text;
            }
            catch
            {
                return "版本号：无";
            }
        }

        private bool SetBBH()
        {
            try
            {
                SystemSetDefine system = SystemSetDefine.GetIntence();
                provider.SetUserVersion(system.VersionNumber);
                return true;
            }
            catch
            {
                return false;
            }
        }

        #endregion 版本号
    }
}