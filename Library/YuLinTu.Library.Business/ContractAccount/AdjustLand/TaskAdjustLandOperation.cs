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
    public class TaskAdjustLandOperation : Task
    {
        #region Ctor

        /// <summary>
        /// 构造函数
        /// </summary>
        public TaskAdjustLandOperation()
        {
        }

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
            TaskAdjustLandArgument argument = Argument as TaskAdjustLandArgument;
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

            TaskAdjustLandArgument metadata = Argument as TaskAdjustLandArgument;
            var zone = metadata.CurrentZone;
            var db = metadata.Database;
            //var vpStation = db.CreateVirtualPersonStation<LandVirtualPerson>();
            var landStation = db.CreateContractLandWorkstation();
            var percent = 20;
            //获取数据
            this.ReportProgress(1, string.Format("开始获取{0}地块数据...", zone.Name));
            List<ContractLand> lands = metadata.Lands;
            if (lands == null || lands.Count == 0)
            {
                this.ReportWarn(string.Format("{0}下无地块数据!", zone.Name));
                return;
            }

            this.ReportProgress(1 + 2 * percent, string.Format("开始获取{0}承包方数据...", zone.Name));
            //var vps = vpStation.GetByZoneCode(zone.FullCode);


            //开始调整
            var vpNumber = metadata.NewVPName.Split(new[] { "(", ")" }, StringSplitOptions.RemoveEmptyEntries)[1];
            //var newCBF = vps.Where(t => t.FamilyNumber.Contains(vpNumber)).FirstOrDefault();
            //this.ReportProgress(1 + 3 * percent, string.Format("正在调整地块到新承包方{0}", newCBF.Name));
            //lands.ForEach(land =>
            //{
            //    land.OwnerId = newCBF.ID;
            //    land.OwnerName = newCBF.Name;
            //});

            //开始更新
            this.ReportProgress(1 + 4 * percent, $"正在更新地块到新承包方{metadata.NewVPName}");
            landStation.UpdateRange(lands);
            var lstr = string.Join(",", lands.Select(t => t.LandNumber.Substring(14)).ToList());
            this.ReportAlert(null, $"完成调整{zone.Name}下地块编码为{lstr}的地块到新承包方{metadata.NewVPName}");
            this.ReportProgress(100, "完成");
            lands = null;
            GC.Collect();
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
