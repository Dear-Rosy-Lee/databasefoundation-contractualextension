/*
 * (C) 2015  鱼鳞图公司版权所有,保留所有权利 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YuLinTu.Data;
using YuLinTu.Library.Entity;

namespace YuLinTu.Library.Business
{
    /// <summary>
    /// 导入界址点图斑数据操作任务类
    /// </summary>
    public class TaskImportCoilOperation : Task
    {
        #region Ctor

        /// <summary>
        /// 构造函数
        /// </summary>
        public TaskImportCoilOperation()
        {
        }

        #endregion

        #region Field

        private IDbContext dbContext;
        private ImportBoundaryAddressCoilDefine ImportBoundaryAddressCoilDefine = ImportBoundaryAddressCoilDefine.GetIntence();
        private List<BuildLandBoundaryAddressDot> currentZoneDotList;
        #endregion

        #region Methods

        /// <summary>
        /// 开始执行子任务
        /// </summary>
        protected override void OnGo()
        {
            TaskImportCoilArgument metadata = Argument as TaskImportCoilArgument;
            if (metadata == null)
            {
                return;
            }
            string fileName = metadata.FileName;
            dbContext = metadata.Database;
            currentZoneDotList = metadata.CurrentZoneDotList;
            Zone zone = metadata.CurrentZone;
            AccountLandBusiness landBusiness = new AccountLandBusiness(dbContext);
            List<Zone> childrenZone = landBusiness.GetChildrenZone(zone);   //子级地域集合
            Zone parent = landBusiness.GetParent(zone);
            //导入界址点图斑数据
            List<Zone> entireZones = GetAllZones(zone, childrenZone, parent, landBusiness);
            ImportCoilDataShape(fileName, zone, entireZones);
        }

        /// <summary>
        /// 导入承包台账界址线图斑
        /// </summary>
        public void ImportCoilDataShape(string fileName, Zone currentZone, List<Zone> entireZones)
        {
            string markDesc = ExportZoneListDir(currentZone, entireZones);
            var dbContext = DataBaseSource.GetDataBaseSource();
            if (dbContext == null)
            {
                this.ReportError(DataBaseSource.ConnectionError);
                return;
            }
            var landStaion = dbContext.CreateContractLandWorkstation();
            List<ContractLand> listLand = new List<ContractLand>();
            try
            {
                listLand = landStaion.GetCollection(currentZone.FullCode, eLevelOption.Self);
            }
            catch (Exception ex)
            {
                YuLinTu.Library.Log.Log.WriteException(this, "GetCollection(获取当前地域下的承包地块失败)", ex.Message + ex.StackTrace);
                this.ReportError("获取当前地域下的承包地块失败," + ex.Message);
            }
            ImportBoundaryAddressCoil importDot = new ImportBoundaryAddressCoil();
            importDot.ProgressChanged += ReportPercent;
            importDot.Alert += ReportInfo;
            importDot.FileName = fileName;
            importDot.CurrentZone = currentZone;
            importDot.ListLand = listLand;
            importDot.MarkDesc = markDesc;
            importDot.CurrentZoneDotList = currentZoneDotList;
            importDot.CreateImportBoundaryCoilTask();
        }

        #endregion

        #region 公用之获取全部地域

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
            if (zone.Level == eZoneLevel.Village)
            {
                Zone tzone = allZones.Find(t => t.FullCode == zone.UpLevelCode);
                exportzonedir = tzone.Name + zone.Name;
            }
            return exportzonedir;
        }

        #endregion

        #region  提示信息

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
