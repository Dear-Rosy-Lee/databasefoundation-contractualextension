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
    public class TaskInitialVPRelationShipOperation : Task
    {
        #region Ctor

        /// <summary>
        /// 构造函数
        /// </summary>
        public TaskInitialVPRelationShipOperation()
        { }

        #endregion

        #region Field
        private string ReplaceName;
        private string ChooseName;

        #endregion

        #region Property

        #endregion

        #region Method—Override

        /// <summary>
        /// 开始执行任务
        /// </summary>
        protected override void OnGo()
        {
            TaskInitialVPRelationShipArgument argument = Argument as TaskInitialVPRelationShipArgument;
            if (argument == null)
            {
                return;
            }
            Zone currentZone = argument.CurrentZone;
            IDbContext dbContext = argument.Database;
            ReplaceName = argument.ReplaceName;
            ChooseName = argument.ChooseName;
            //替换家庭关系
            this.ReportProgress(1, "开始获取数据...");
            ContractorDataProgress(currentZone);
            //try
            //{
            //    var zonestation = dbContext.CreateZoneWorkStation();
            //    List<Zone> zones = zonestation.GetChildren(currentZone.FullCode, eLevelOption.SelfAndSubs);//.GetSubZones(CurrentZone.FullCode, eLevelOption.AllSubLevel);
            //    foreach (Zone zone in zones)
            //    {
            //        ContractorDataProgress(zone);
            //    }
            //    zones = null;
            //}
            //catch (System.Exception ex)
            //{
            //    if (!(ex is TaskStopException))
            //    {
            //        this.ReportError(ex.Message + "数据检查时出错!");
            //    }
            //}
            this.ReportProgress(100, null);


            //this.ReportProgress(1);
            //List<VirtualPerson> vps = new List<VirtualPerson>();
            //var vpstation = dbContext.CreateVirtualPersonStation<LandVirtualPerson>();
            //int familyIndex = 1;
            //int oldprogress = 1;
            //int count = argument.VPS.Count;
            //try {
            //    foreach (VirtualPerson vp in argument.VPS)
            //    {
            //        if (oldprogress != (80 * familyIndex / count))
            //        {
            //            oldprogress = (80 * familyIndex / count);
            //            this.ReportProgress(oldprogress, familyIndex.ToString() + "(" + vps.Count.ToString() + ")");
            //        }
            //        familyIndex++;
            //        bool isContinue = InitalizeSharePersonInformation(vp);
            //        if (!isContinue)
            //        {
            //            continue;
            //        }
            //        vps.Add(vp);
            //    }
            //    this.ReportProgress(85, "保存更新");
            //    vpstation.UpdatePersonList(vps);
            //    this.ReportProgress(100);
            //}
            //catch(Exception ex)
            //{
            //    this.ReportError(ex.Message);
            //    this.ReportProgress(100);
            //}
            
        }
        private void ContractorDataProgress(Zone zone)
        {
            TaskInitialVPRelationShipArgument metadata = Argument as TaskInitialVPRelationShipArgument;
            var vpstation = metadata.Database.CreateVirtualPersonStation<LandVirtualPerson>();
            int familyCount = vpstation.Count(c => c.ZoneCode.Equals(zone.FullCode));
            if (familyCount == 0)
            {
                return;
            }
            this.ReportProgress(1, string.Format("开始获取{0}承包方数据...", metadata.CurrentZone.Name));
            List<VirtualPerson> vps = vpstation.GetByZoneCode(zone.FullCode, eLevelOption.Self);
            if (vps == null || vps.Count == 0)
            {
                this.ReportWarn(string.Format("{0}下无承包方数据!", zone.Name));
                return;
            }
            string description = vps.Count > 0 ? ("(" + vps.Count + ")") : "";
            int present = 1;
            string status = string.Format("正在替换{0}下家庭成员关系...", zone.Name);
            this.ReportProgress(2, status);
            this.ReportProgress(3, string.Format("开始替换{0}下家庭成员关系...", zone.Name));
            List<VirtualPerson> updateVps = new List<VirtualPerson>();
            foreach (VirtualPerson vp in vps)
            {
                bool isContinue = InitalizeSharePersonInformation(vp);
                if (!isContinue)
                {
                    continue;
                }
                updateVps.Add(vp);
                present++;
                this.ReportProgress(present, description);
            }
            vpstation.UpdatePersonList(updateVps);
            vps = null;
            GC.Collect();
            this.ReportAlert(null, string.Format("替换{0}下家庭成员关系结束", zone.Name));
            this.ReportProgress(100, "完成");
        }

        /// <summary>
        /// 初始化调查信息
        /// </summary>
        private bool InitalizeSharePersonInformation(VirtualPerson vp)
        {
            bool canUpdate = false;
            List<Person> fsp = vp.SharePersonList;
            foreach (Person person in fsp)
            {                
                string ship = person.Relationship;
                if (ship != ReplaceName)
                {
                    continue;
                }              
                if (person.Name == vp.Name && vp.FamilyExpand != null && vp.FamilyExpand.ContractorType != eContractorType.Farmer)
                {
                    continue;
                }
                person.Relationship = ChooseName;
                canUpdate = true;
            }
            vp.SharePerson = fsp.ToString();
            vp.SharePersonList = fsp;
            fsp = null;
            return canUpdate;
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
