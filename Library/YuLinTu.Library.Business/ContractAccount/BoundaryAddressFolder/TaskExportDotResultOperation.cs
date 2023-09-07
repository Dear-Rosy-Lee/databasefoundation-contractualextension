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
    /// 导出界址点成果表操作任务类
    /// </summary>
    public class TaskExportDotResultOperation : Task
    {
        #region Ctor

        /// <summary>
        /// 构造函数
        /// </summary>
        public TaskExportDotResultOperation()
        {
        }

        #endregion

        #region Field

        private IDbContext dbContext;
        string fileName = string.Empty;

        #endregion

        #region Methods

        /// <summary>
        /// 开始执行子任务
        /// </summary>
        protected override void OnGo()
        {
            TaskExportDotResultArgument metadata = Argument as TaskExportDotResultArgument;
            if (metadata == null)
            {
                return;
            }
             this.ReportProgress(0, string.Format("导出{0}界址点成果表...", metadata.CurrentPerson.Name));
            fileName = metadata.FileName;
            dbContext = metadata.Database;
            Zone zone = metadata.CurrentZone;
            List<ContractLand> listGeoLand = metadata.ListGeoLand;
            VirtualPerson currentPerson = metadata.CurrentPerson;
            AccountLandBusiness landBusiness = new AccountLandBusiness(dbContext);
            landBusiness.ProgressChanged += ReportPercent;
            landBusiness.Alert += ReportInfo;
            List<Zone> childrenZone = landBusiness.GetChildrenZone(zone);   //子级地域集合
            Zone parent = landBusiness.GetParent(zone);
            //导出界址点成果表
            List<Zone> entireZones = GetAllZones(zone, childrenZone, parent, landBusiness);
            bool canOpen = ExportDotResultExcel(fileName, listGeoLand, currentPerson, landBusiness, zone, entireZones);
            if (canOpen)
            {
                CanOpenResult = true;
            }
        }

        /// <summary>
        /// 打开文件
        /// </summary>
        public override void OpenResult()
        {
            System.Diagnostics.Process.Start(fileName);
            base.OpenResult();
        }

        /// <summary>
        /// 导出界址点成果表
        /// </summary>
        /// <param name="fileName">保存问价路径</param>
        /// <param name="listGeoLand">空间地块集合(当前承包方下)</param>
        /// <param name="currentPerson">当前承包方</param>
        /// <param name="landBusiness">地块业务</param>
        /// <param name="currentZone">当前地域</param>
        /// <param name="entireZones">全部地域</param>
        private bool ExportDotResultExcel(string fileName, List<ContractLand> listGeoLand, VirtualPerson currentPerson,
            AccountLandBusiness landBusiness, Zone currentZone, List<Zone> entireZones)
        {
            bool canOpen = true;
            string markDesc = ExportZoneListDir(currentZone, entireZones);
            //this.ReportProgress(1, "开始");
            int index = 1;
            int count = 0;
            if (listGeoLand.Count == 0)
            {
                //当前承包方下没有空间地块
                this.ReportProgress(100, null);
                this.ReportWarn(string.Format("{0}未获取空间地块信息", markDesc + currentPerson.Name));
                return false;
            }
            double percent = 100 / (double)listGeoLand.Count;
            foreach (var geoLand in listGeoLand)
            {
                if (landBusiness.ExportDotResultExcel(currentZone, geoLand, fileName, 3))
                    count++;
                this.ReportProgress((int)(index * percent), string.Format("{0}", markDesc + geoLand.OwnerName));
                index++;
            }
            this.ReportProgress(100, null);
            if (count == 0)
            {
                this.ReportInfomation(string.Format("{0}未导出界址点成果表", markDesc + currentPerson.Name));
                canOpen = false;
            }
            else
            {
                this.ReportInfomation(string.Format("{0}导出{1}个界址点成果表", markDesc + currentPerson.Name, count));
            }
            return canOpen;
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
