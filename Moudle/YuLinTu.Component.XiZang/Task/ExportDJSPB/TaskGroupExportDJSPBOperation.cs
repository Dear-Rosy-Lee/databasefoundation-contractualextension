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
using YuLinTu.Library.Business;

namespace YuLinTu.Component.XiZangLZ
{
    public class TaskGroupExportDJSPBOperation : TaskGroup
    {
        /// <summary>
        /// 开始执行组任务
        /// </summary>
        protected override void OnGo()
        {
            Clear();
            TaskGroupExportDJSPBArgument groupMeta = Argument as TaskGroupExportDJSPBArgument;
            if (groupMeta == null)
            {
                return;
            }       
            List<Zone> allZones = groupMeta.AllZones;
            IDbContext dbcontext = groupMeta.Database;
            var vpstation = dbcontext.CreateVirtualPersonStation<LandVirtualPerson>();
            var landstation = dbcontext.CreateContractLandWorkstation();
            var senderstation = dbcontext.CreateSenderWorkStation();
            var concordstation = dbcontext.CreateConcordStation();
            List<YuLinTu.Library.Entity.VirtualPerson> zonevps = new List<YuLinTu.Library.Entity.VirtualPerson>();
            List<ContractLand> zonelands = new List<ContractLand>();           
           int zonesendersCount = 0;
            foreach (var zone in allZones)
            {
                if (zone.Level == eZoneLevel.Village)
                {
                    zonevps = vpstation.GetByZoneCode(zone.FullCode, eLevelOption.SelfAndSubs);
                    zonelands = landstation.GetCollection(zone.FullCode, eLevelOption.SelfAndSubs);
                    zonesendersCount = senderstation.Count(zone.FullCode,eLevelOption.SelfAndSubs);
                   
                    if (zonevps.Count() == 0 && zonelands.Count == 0 && zonesendersCount == 0)
                    {
                        this.ReportInfomation(string.Format("{0}下没有发包方、承包方、地块数据"),zone.FullName);
                        continue;
                    }
                    zonevps.RemoveAll(vp => vp.Name.Contains("集体"));
                    zonelands.RemoveAll(l => l.OwnerName.Contains("集体"));
                }
                else 
                {
                    zonevps = vpstation.GetByZoneCode(zone.FullCode, eLevelOption.SelfAndSubs);
                    zonelands = landstation.GetCollection(zone.FullCode, eLevelOption.SelfAndSubs);
                    zonesendersCount = senderstation.Count(zone.FullCode, eLevelOption.SelfAndSubs);
                   
                    if (zonevps.Count() == 0 && zonelands.Count == 0 && zonesendersCount == 0)
                    {
                        this.ReportInfomation(string.Format("{0}下没有发包方、承包方、地块数据"), zone.FullName);
                        continue;
                    }
                    zonevps.RemoveAll(vp => vp.Name.Contains("集体"));
                    zonelands.RemoveAll(l => l.OwnerName.Contains("集体"));
                }

                TaskExportDJSPBArgument meta = new TaskExportDJSPBArgument();
                meta.CurrentZone = zone;
                meta.FileName = groupMeta.FileName;
                meta.Database = groupMeta.Database;
                meta.VirtualPersons = zonevps;
                meta.ALLLands = zonelands;                      
                meta.SendersCount = zonesendersCount;
                TaskExportDJSPBOperation import = new TaskExportDJSPBOperation();
                import.Argument = meta;
                import.Description = "导出" + zone.FullName+ "登记审批表";
                import.Name = "导出登记审批表";
                Add(import);   //添加子任务到任务组中
            }
           
            base.OnGo();
        }
    }
}
