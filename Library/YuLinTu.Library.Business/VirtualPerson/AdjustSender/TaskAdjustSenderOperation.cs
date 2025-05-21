/*
 * (C) 2025  鱼鳞图公司版权所有,保留所有权利 
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
    /// 初始化承包方基本信息任务类
    /// </summary>
    public class TaskAdjustSenderOperation : Task
    {
        #region Ctor

        /// <summary>
        /// 构造函数
        /// </summary>
        public TaskAdjustSenderOperation()
        { }
    
        #endregion

        #region Field

        #endregion

        #region Property

        #endregion

        #region Method—Override

        /// <summary>
        /// 开始执行任务
        /// </summary>
        protected override void OnGo()
        {
            TaskAdjustSenderArgument argument = Argument as TaskAdjustSenderArgument;
            if (argument == null)
            {
                return;
            }
           
            //开始调整
            this.ReportProgress(1, "开始获取数据...");
            AdjustSender();
            this.ReportProgress(100, null);
        }
        private void AdjustSender()
        {
            
            TaskAdjustSenderArgument metadata = Argument as TaskAdjustSenderArgument;
            var zone = metadata.CurrentZone;
            var db = metadata.Database;
            var zoneStation = db.CreateZoneWorkStation();
            var vpStation = db.CreateVirtualPersonStation<LandVirtualPerson>();
            var zones = zoneStation.Get();
            var percent = 8;
            //获取数据
            this.ReportProgress(1, string.Format("开始获取{0}承包方数据...", zone.Name));
            List<VirtualPerson> vps = metadata.VirtualPeoples;
            if (vps == null || vps.Count == 0)
            {
                this.ReportWarn(string.Format("{0}下无承包方数据!", zone.Name));
                return;
            }
            
            this.ReportProgress(1 + 2 * percent, string.Format("开始获取{0}承包方地块数据...", zone.Name));
            var landStation = db.CreateContractLandWorkstation();
            var lands = landStation.GetCollection(zone.FullCode);
            var landList = new List<ContractLand>();
            vps.ForEach(p => { landList.AddRange(lands.Where(t => t.OwnerId == p.ID).ToList()); });
            this.ReportProgress(1 + 3 * percent, string.Format("开始获取{0}承包方合同数据...", zone.Name));
            var concordStation = db.CreateConcordStation();
            var concords = concordStation.GetByZoneCode(zone.FullCode);
            var concordList = new List<ContractConcord>();
            vps.ForEach(p => { concordList.AddRange(concords.Where(t => t.ContracterId == p.ID).ToList()); });
            this.ReportProgress(1 + 4 * percent, string.Format("开始获取{0}承包方权证数据...", zone.Name));
            var regeditBookStation = db.CreateRegeditBookStation();
            var regeditBooks = regeditBookStation.GetByZoneCode(zone.FullCode,eSearchOption.Precision);
            var regeditBookList = new List<ContractRegeditBook>();
            concordList.ForEach(p => { regeditBookList.AddRange(regeditBooks.Where(t => t.RegeditNumber == p.ConcordNumber).ToList()); });
            //开始调整
            var newZone = zones.Where(t => t.FullName.Contains(metadata.NewSenderName)).FirstOrDefault();
            this.ReportProgress(1 + 5 * percent, string.Format("正在调整{0}承包方到新发包方{1}", zone.Name, newZone.Name));
            vps.ForEach(vp => vp.ZoneCode = newZone.FullCode);
            this.ReportProgress(1 + 6 * percent, string.Format("正在调整{0}承包方地块到新发包方{1}", zone.Name, newZone.Name));
            landList.ForEach(land => land.ZoneCode = newZone.FullCode);
            this.ReportProgress(1 + 7 * percent, string.Format("正在调整{0}承包方合同到新发包方{1}", zone.Name, newZone.Name));
            concordList.ForEach(concord => concord.ZoneCode = newZone.FullCode);
            this.ReportProgress(1 + 8 * percent, string.Format("正在调整{0}承包方权证到新发包方{1}", zone.Name, newZone.Name));
            regeditBookList.ForEach(regeditBook => regeditBook.ZoneCode = newZone.FullCode);
            //开始更新
            this.ReportProgress(1 + 9 * percent, string.Format("正在更新{0}承包方到新发包方{1}", zone.Name, newZone.Name));
            vpStation.UpdatePersonList(vps);
            this.ReportProgress(1 + 10 * percent, string.Format("正在调整{0}承包方地块到新发包方{1}", zone.Name, newZone.Name));
            landStation.UpdateRange(landList);
            this.ReportProgress(1 + 11 * percent, string.Format("正在调整{0}承包方合同到新发包方{1}", zone.Name, newZone.Name));
            concordStation.Updatelist(concordList);
            this.ReportProgress(1 + 12 * percent, string.Format("正在调整{0}承包方权证到新发包方{1}", zone.Name, newZone.Name));
            regeditBookStation.UpdataList(regeditBookList);
            vps = null;
            GC.Collect();
            this.ReportAlert(null, string.Format("完成调整承包方到新发包方{0}", newZone.Name));
            this.ReportProgress(100, "完成");
        }

        

        #endregion

        #region Method—Helper

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
