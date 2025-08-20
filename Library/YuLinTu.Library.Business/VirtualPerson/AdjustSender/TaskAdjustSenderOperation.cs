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
            var regeditBooks = regeditBookStation.GetByZoneCode(zone.FullCode, eSearchOption.Precision);
            var regeditBookList = new List<ContractRegeditBook>();
            concordList.ForEach(p => { regeditBookList.AddRange(regeditBooks.Where(t => t.RegeditNumber == p.ConcordNumber).ToList()); });

            //开始调整
            var newZone = zones.Where(t => t.FullCode.Contains(metadata.NewSenderCode)).FirstOrDefault();
            if (newZone == null)
            {
                this.ReportWarn(string.Format("找不到地域编码包含{0}的发包方!", metadata.NewSenderCode));
                return;
            }

            // 获取新发包方下的现有承包方，用于计算新的户号和地块编码
            var existingVps = vpStation.GetByZoneCode(newZone.FullCode);
            var existingLands = landStation.GetCollection(newZone.FullCode);

            // 计算新的户号起始值（现有最大值+1）
            int maxHH = 0;
            if (existingVps != null && existingVps.Count > 0)
            {
                foreach (var vp in existingVps)
                {
                    if (!string.IsNullOrEmpty(vp.FamilyNumber))
                    {
                        int hhValue;
                        if (int.TryParse(vp.FamilyNumber, out hhValue))
                        {
                            maxHH = Math.Max(maxHH, hhValue);
                        }
                    }
                }
            }
            int nextHH = maxHH + 1;

            // 计算新的地块编码起始值（现有最大值+1）
            // DKBM = ZLDM + 五位数字，需要提取后五位
            int maxDKBM = 0;
            if (existingLands != null && existingLands.Count > 0)
            {
                foreach (var land in existingLands)
                {
                    if (!string.IsNullOrEmpty(land.LandNumber) && land.LandNumber.Length >= 5)
                    {
                        // 提取DKBM的后五位
                        string last5Digits = land.LandNumber.Substring(land.LandNumber.Length - 5);
                        int dkbmValue;
                        if (int.TryParse(last5Digits, out dkbmValue))
                        {
                            maxDKBM = Math.Max(maxDKBM, dkbmValue);
                        }
                    }
                }
            }
            int nextDKBM = maxDKBM + 1;

            // 创建承包方编码映射字典
            Dictionary<Guid, string> vpCodeMapping = new Dictionary<Guid, string>();

            this.ReportProgress(1 + 5 * percent, string.Format("正在调整{0}承包方到新发包方{1}", zone.Name, newZone.Name));
            foreach (var vp in vps)
            {
                vp.ZoneCode = newZone.FullCode;
                // 设置新的户号
                vp.FamilyNumber = nextHH.ToString().PadLeft(4, '0');
                //// 创建新的承包方编码 = ZoneCode + HH
                string newVpCode = newZone.FullCode + vp.FamilyNumber;
                vpCodeMapping[vp.ID] = nextHH.ToString();
                nextHH++;
            }

            this.ReportProgress(1 + 6 * percent, string.Format("正在调整{0}承包方地块到新发包方{1}", zone.Name, newZone.Name));
            foreach (var land in landList)
            {
                land.ZoneCode = newZone.FullCode;  // ZLDM
                land.ZoneName = newZone.Name;      // ZLMC
                // 设置新的地块编码 DKBM = ZLDM + 五位数字编号
                string dkbmNumber = nextDKBM.ToString().PadLeft(5, '0');
                land.LandNumber = newZone.FullCode + dkbmNumber;  // DKBM
                // 更新地籍编码（如果需要）
                land.CadastralNumber = land.LandNumber;  // DJBM通常与DKBM相同
                nextDKBM++;
            }

            this.ReportProgress(1 + 7 * percent, string.Format("正在调整{0}承包方合同到新发包方{1}", zone.Name, newZone.Name));
            foreach (var concord in concordList)
            {
                concord.ZoneCode = newZone.FullCode;  // DYBM
                concord.SenderId = newZone.ID;        // FBFBS - 发包方ID
                concord.SenderName = newZone.Name;    // FBFMC - 发包方名称

                // 更新合同编码 - 使用承包方编码+'J'
                if (concord.ContracterId.HasValue && vpCodeMapping.ContainsKey(concord.ContracterId.Value))
                {
                    string newVpCode = vpCodeMapping[concord.ContracterId.Value];
                    concord.ConcordNumber = newVpCode + "J";  // CBHTBM
                }
            }

            this.ReportProgress(1 + 8 * percent, string.Format("正在调整{0}承包方权证到新发包方{1}", zone.Name, newZone.Name));
            foreach (var regeditBook in regeditBookList)
            {
                regeditBook.ZoneCode = newZone.FullCode;  // DYDM

                // 根据合同找到对应的承包方编码
                var relatedConcord = concordList.FirstOrDefault(c => c.ConcordNumber == regeditBook.RegeditNumber);
                if (relatedConcord != null && relatedConcord.ContracterId.HasValue && vpCodeMapping.ContainsKey(relatedConcord.ContracterId.Value))
                {
                    string newVpCode = vpCodeMapping[relatedConcord.ContracterId.Value];
                    regeditBook.Number = newVpCode + "J";        // QZBM
                    regeditBook.RegeditNumber = newVpCode + "J"; // CBJYQZBM
                }
            }

            //开始更新
            this.ReportProgress(1 + 9 * percent, string.Format("正在更新{0}承包方到新发包方{1}", zone.Name, newZone.Name));
            vpStation.UpdatePersonList(vps);

            this.ReportProgress(1 + 10 * percent, string.Format("正在更新{0}承包方地块到新发包方{1}", zone.Name, newZone.Name));
            landStation.UpdateRange(landList);

            this.ReportProgress(1 + 11 * percent, string.Format("正在更新{0}承包方合同到新发包方{1}", zone.Name, newZone.Name));
            concordStation.Updatelist(concordList);

            this.ReportProgress(1 + 12 * percent, string.Format("正在更新{0}承包方权证到新发包方{1}", zone.Name, newZone.Name));
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