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

namespace YuLinTu.Library.Business
{
    public class TaskGroupConcordInitializeOperation : TaskGroup
    {
        /// <summary>
        /// 开始执行组任务
        /// </summary>
        protected override void OnGo()
        {
            Clear();
            TaskGroupConcordInitializeArgument groupMeta = Argument as TaskGroupConcordInitializeArgument;
            if (groupMeta == null)
            {
                return;
            }
            IDbContext dbContext = groupMeta.Database;
            Zone currentZone = groupMeta.CurrentZone;
            List<Zone> allZones = groupMeta.AllZones;
            var senderStation = dbContext.CreateSenderWorkStation();

            //var senderStation = dbContext.CreateSenderWorkStation();
            //var senderList = senderStation.GetTissues(currentZone.FullCode, eLevelOption.SelfAndSubs);
            foreach (var zone in allZones)
            {
                //var sender = senderList.FindAll(c => !string.IsNullOrEmpty(c.ZoneCode) && c.ZoneCode == zone.FullCode).FirstOrDefault();
                //if (sender == null)
                //{
                //    this.ReportWarn(string.Format("{0}未获得发包方信息!", zone.FullName));
                //    continue;
                //}
                var tissue = senderStation.Get(zone.ID);//当前地域发包方               
                if (tissue == null)
                {
                    string code = zone.FullCode;
                    code = code.Length == 16 ? code.Substring(0, 12) + code.Substring(12, 2) : code.PadRight(14, '0');
                    tissue = senderStation.GetByCode(code);
                }
                TaskConcordInitializeArgument argument = new TaskConcordInitializeArgument();
                argument.Database = dbContext;
                argument.CurrentZone = zone;
                argument.ConcordsModified = groupMeta.ConcordsModified == null ?
                    new List<ContractConcord>() : groupMeta.ConcordsModified.FindAll(c => !string.IsNullOrEmpty(c.ZoneCode) && c.ZoneCode == zone.FullCode);
                //argument.ConcordsModified.ForEach(c => { c.SenderId = sender.ID; c.SenderName = sender.Name; });
                argument.LandsOfInitialConcord = groupMeta.LandsOfInitialConcord == null ?
                    new List<ContractLand>() : groupMeta.LandsOfInitialConcord.FindAll(c => !string.IsNullOrEmpty(c.LocationCode) && c.LocationCode == zone.FullCode);
                argument.IsCalculateArea = groupMeta.IsCalculateArea;
                argument.VillageInlitialSet = groupMeta.VillageInlitialSet;
                argument.Sender = tissue;
                TaskConcordInitializeOperation operation = new TaskConcordInitializeOperation();
                operation.Argument = argument;
                operation.Description = "签订" + zone.FullName + "合同";  //任务描述
                operation.Name = "初始合同信息";             //任务名称 
                Add(operation);   //添加子任务到任务组中
            }
            base.OnGo();

        }

    }
}
