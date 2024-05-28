/*
 * (C) 2015  鱼鳞图公司版权所有,保留所有权利 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using YuLinTu.Library.Entity;
using YuLinTu;
using YuLinTu.Data;
using System.Diagnostics;
using System.IO;
using System.Collections;
using YuLinTu.Data.Dynamic;
using YuLinTu.Data.Shapefile;
using YuLinTu.Library.Business;

namespace YuLinTu.Component.MapFoundation
{
    /// <summary>
    /// 鱼鳞图模块导入承包地空间数据-参数文件与底层导入类用公共的，本任务类单独使用-地域节点与初始导航选择一致
    /// </summary>
    public class TaskImportMapCBDShapeOperation : Task
    {
        #region Fields      

        private object returnValue;
        #endregion

        #region Properties

        /// <summary>
        /// 承包台账常规设置实体
        /// </summary>
        public ContractBusinessSettingDefine SettingDefine = ContractBusinessSettingDefine.GetIntence();

        /// <summary>
        /// 导入界址点图斑设置实体
        /// </summary>

        /// <summary>
        /// 导出公示调查表日期设置
        /// </summary>
        public DateSetting PublishDateSetting { get; set; }

        /// <summary>
        /// 返回值
        /// </summary>
        public object ReturnValue
        {
            get { return returnValue; }
        }

        /// <summary>
        /// 是否批量
        /// </summary>
        public bool IsBatch { get; set; }

        /// <summary>
        /// 当前被选中的承包方
        /// </summary>
        public List<VirtualPerson> SelectContractor { get; set; }

        /// <summary>
        /// 数据库服务上下文
        /// </summary>
        public IDbContext dbContext { get; set; }

        /// <summary>
        /// 选中的承包方集合
        /// </summary>
        public List<VirtualPerson> SelectedPersons { get; set; }

        #endregion

        #region Ctor

        /// <summary>
        /// 构造函数
        /// </summary>
        public TaskImportMapCBDShapeOperation()
        {
        }

        #endregion

        #region Methods - Override

        /// <summary>
        /// 开始操作
        /// </summary>
        protected override void OnGo()
        {
            returnValue = null;
            TaskContractAccountArgument metadata = Argument as TaskContractAccountArgument;
            if (metadata == null)
            {
                return;
            }
            string fileName = metadata.FileName;
            bool isClear = metadata.IsClear;
            dbContext = metadata.Database;
            Zone zone = metadata.CurrentZone;
            SelectContractor = metadata.SelectContractor;
            AccountLandBusiness landBusiness = new AccountLandBusiness(dbContext);
            landBusiness.meta = metadata;
            landBusiness.VirtualType = metadata.VirtualType;
            landBusiness.Alert += ReportInfo;
            landBusiness.ProgressChanged += ReportPercent;
            landBusiness.TableType = metadata.TableType;
            landBusiness.ArgType = metadata.ArgType;
            landBusiness.PublishDateSetting = this.PublishDateSetting;
            landBusiness.IsBatch = IsBatch;
            landBusiness.UseContractorInfoImport = metadata.UseContractorInfoImport;
            landBusiness.UseLandCodeBindImport = metadata.UseLandCodeBindImport;
            landBusiness.UseOldLandCodeBindImport = metadata.UseOldLandCodeBindImport;
            landBusiness.shapeAllcolNameList = metadata.shapeAllcolNameList;
            var zoneStation = dbContext.CreateZoneWorkStation();
            List<Zone> childrenZone = zoneStation.GetChildren(zone.FullCode, eLevelOption.Subs);
            Zone parent = landBusiness.GetParent(zone);
            ImportLandDataShape(zone, childrenZone, fileName, landBusiness);
            this.ReportProgress(100);
            GC.Collect();
        }

        protected override void OnStopped()
        {
            base.OnStopped();
            GC.Collect();
        }

        protected override void OnTerminate(Exception ex)
        {
            base.OnTerminate(ex);
            GC.Collect();
        }

        #endregion

        #region 公用之获取全部地域及创建文件目录

        /// <summary>
        /// 获取全部的地域
        /// </summary>
        /// <param name="currentZone">当前地域</param>
        /// <param name="childrenZone">子级地域</param>
        /// <param name="parentZone">父级地域</param>
        /// <param name="fileName">保存路径</param>
        /// <param name="business">合同业务</param>
        public List<Zone> GetAllZones(Zone currentZone, List<Zone> childrenZone, Zone parentZone, AccountLandBusiness business)
        {
            List<Zone> allZones = new List<Zone>();
            allZones.Add(currentZone);
            if (currentZone.Level == eZoneLevel.Group)
            {
                //选择为组
                allZones.Add(parentZone);
                allZones.Add(business.GetParent(parentZone));
            }
            else if (currentZone.Level == eZoneLevel.Village)
            {
                //选择为村
                foreach (var child in childrenZone)
                {
                    allZones.Add(child);
                }
                allZones.Add(parentZone);
            }
            else if (currentZone.Level == eZoneLevel.Town)
            {
                //选择为镇
                foreach (var child in childrenZone)
                {
                    allZones.Add(child);
                    List<Zone> zones = business.GetChildrenZone(child);
                    foreach (var zone in zones)
                    {
                        allZones.Add(zone);
                    }
                }
            }
            return allZones;
        }

        /// <summary>
        /// 创建文件目录(可以创建至组)
        /// </summary>
        /// <param name="allZones">全部地域</param>
        /// <param name="cZone">当前地域</param>
        private string CreateDirectory(List<Zone> allZones, Zone cZone)
        {
            string folderString = cZone.Name;
            Zone z = cZone;
            while (z.Level < eZoneLevel.County)
            {
                z = allZones.Find(t => t.FullCode == z.UpLevelCode);
                if (z != null)
                    folderString = z.Name + @"\" + folderString;
                else
                    break;
            }
            return folderString;
        }

        /// <summary>
        /// 创建文件目录(仅创建至村)
        /// </summary>
        /// <param name="allZones">全部地域</param>
        /// <param name="cZone">当前地域</param>
        private string CreateDirectoryByVilliage(List<Zone> allZones, Zone cZone)
        {
            string folderString = cZone.Level == eZoneLevel.Group ? "" : cZone.Name;
            Zone z = cZone;
            while (z.Level < eZoneLevel.County)
            {
                z = allZones.Find(t => t.FullCode == z.UpLevelCode);
                if (z != null)
                    folderString = z.Name + @"\" + folderString;
                else
                    break;
            }
            return folderString;
        }

        #endregion

        #region Methods - Privates-承包台账地块图斑导入

        /// <summary>
        /// 导入承包方shape图斑数据
        /// </summary>
        private void ImportLandDataShape(Zone currentZone, List<Zone> childrenZone, string fileName, AccountLandBusiness accountBusiness)
        {
            //获取当前路径下shape数据
            IList landShapeList = null;
            string filepath = string.Empty;
            string filename = string.Empty;
            IDbContext ds = null;
            try
            {
                filepath = System.IO.Path.GetDirectoryName(fileName);
                filename = System.IO.Path.GetFileNameWithoutExtension(fileName);
                ds = ProviderShapefile.CreateDataSource(filepath, false) as IDbContext;
                var dq = new DynamicQuery(ds);
                var landdata = dq.Get(null, filename).Result as IList;
                landShapeList = landdata;
            }
            catch
            {
                this.ReportError("当前打开Shape文件有误，请检查文件是否正确");
                return;
            }

            var importShpType = ds.DataSource as IProviderShapefile;
            if (importShpType.GetGeometryType(filename) != Spatial.eGeometryType.Polygon)
            {
                this.ReportError("当前Shape文件不为面文件，请重新选择面文件导入");
                return;
            }
            if (landShapeList.Count == 0)
            {
                this.ReportInfomation("当前导入文件没有数据");
                return;
            }
            var dps = ds.DataSource as ProviderShapefile;
            YuLinTu.Spatial.SpatialReference shpRef = dps.GetSpatialReference(filename);
            accountBusiness.shpRef = shpRef;

            if (landShapeList.Count == 0 || landShapeList == null)
            {
                return;
            }
            double currentPercent = 0.0;
            if(currentZone.Level>eZoneLevel.Village)
            {
                this.ReportWarn("请选择镇以下的行政区域");
                return;
            }
            if (currentZone.Level == eZoneLevel.Village)
            {
                if (childrenZone == null)
                {
                    var zoneStation = dbContext.CreateZoneWorkStation();
                    childrenZone = zoneStation.GetChildren(currentZone.FullCode, eLevelOption.SelfAndSubs);
                }
                else
                {
                    childrenZone.Add(currentZone);
                }
                int importCount = 0;//总数
                int importZoneCount = 0;//各组个数
                this.ReportProgress(0, "开始");
                double indexCount = (double)childrenZone.Count / 95;//均分进度
                int zoneCurrentIndex = 0;//进度索引

                foreach (var itemZone in childrenZone)
                {
                    currentPercent = 5 + indexCount * zoneCurrentIndex;
                    importZoneCount = accountBusiness.ImportLandShapeDataInfo(itemZone, landShapeList, currentPercent, indexCount);
                    this.ReportProgress((int)currentPercent, string.Format("导入{0}下地块", itemZone.FullName));
                    zoneCurrentIndex++;
                    importCount = importCount + importZoneCount;
                }
                if (importCount != 0)
                {
                    this.ReportProgress(100, "完成");
                    this.ReportInfomation(string.Format("{0}共导入{1}条信息", currentZone.Name, importCount));
                }
                currentPercent = 0;
            }
            else if (currentZone.Level == eZoneLevel.Group)
            {
                int importCount = 0;
                this.ReportProgress(0, "开始");
                importCount = accountBusiness.ImportLandShapeDataInfo(currentZone, landShapeList, currentPercent);
                if (importCount != 0)
                {
                    this.ReportProgress(100, "完成");
                    this.ReportInfomation(string.Format("{0}共导入{1}条信息", currentZone.Name, importCount));
                }
                currentPercent = 0;
            }

            landShapeList = null;
        }

        #endregion


        #region 辅助功能

        private List<CollectivityTissue> GetTissueCollection(Zone zone)
        {
            string messageName = SenderMessage.SENDER_GETDATA;
            ModuleMsgArgs args = new ModuleMsgArgs();
            args.Name = messageName;
            args.Parameter = zone.FullCode;
            args.Datasource = DataBaseSource.GetDataBaseSource();
            TheBns.Current.Message.Send(this, args);
            List<CollectivityTissue> list = args.ReturnValue as List<CollectivityTissue>;
            return list;
        }

        /// <summary>
        /// 进度提示用，导出时获取当前地域的上级地域名称路径到镇级
        /// </summary>       
        private string ExportZoneListDir(Zone zone, List<Zone> allZones)
        {
            string exportzonedir = string.Empty;
            if (zone.Level == eZoneLevel.Group)
            {
                Zone vzone = allZones.Find(t => t.FullCode == zone.UpLevelCode);
                Zone tzone = allZones.Find(t => t.FullCode == vzone.UpLevelCode);
                exportzonedir = tzone.Name + vzone.Name + zone.Name;
            }
            else if (zone.Level == eZoneLevel.Village)
            {
                Zone tzone = allZones.Find(t => t.FullCode == zone.UpLevelCode);
                exportzonedir = tzone.Name + zone.Name;
            }
            else if (zone.Level == eZoneLevel.Town)
            {
                exportzonedir = zone.Name;
            }
            return exportzonedir;
        }

        #endregion

        #region  提示信息

        /// <summary>
        /// 判断当前地域下有没有承包方信息
        /// </summary>
        private bool ExitsPerson(Zone zone)
        {
            bool exsit = false;
            AccountLandBusiness business = new AccountLandBusiness(dbContext);
            business.VirtualType = eVirtualType.Land;
            List<VirtualPerson> listPerson = business.GetByZone(zone.FullCode);
            if (listPerson != null && listPerson.Count() > 0)
            {
                exsit = true;
            }
            return exsit;
        }


        /// <summary>
        /// 错误信息报告
        /// </summary>
        /// <param name="message"></param>
        private void ReportInfo(object sender, TaskAlertEventArgs e)
        {
            if (e != null)
            {
                this.ReportAlert(e.Grade, e.UserState, e.Description);
            }
        }

        /// <summary>
        /// 进度报告
        /// </summary>
        /// <param name="progress"></param>
        private void ReportPercent(object sender, TaskProgressChangedEventArgs e)
        {
            if (e != null)
            {
                this.ReportProgress(e.Percent, e.UserState);
            }
        }

        #endregion


    }
}
