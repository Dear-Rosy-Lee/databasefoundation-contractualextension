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
using YuLinTu.Library.Business;

namespace YuLinTu.Component.XiZangLZ
{
    /// <summary>
    /// 导出家庭承包单户申请书
    /// </summary>
    public class TaskGroupSingleRequireBookOperation : TaskGroup
    {
        #region Override

        /// <summary>
        /// 开始执行组任务
        /// </summary>
        protected override void OnGo()
        {
            Clear();
            TaskGroupSingleRequireBookArgument groupMeta = Argument as TaskGroupSingleRequireBookArgument;
            if (groupMeta == null)
            {
                return;
            }
            IDbContext dbContext = groupMeta.Database;
            Zone currentZone = groupMeta.CurrentZone;
            string fileName = groupMeta.FileName;
            List<Zone> selfAndSubsZones = groupMeta.SelfAndSubsZones;

            var personStation = dbContext.CreateVirtualPersonStation<LandVirtualPerson>();
            var listVp = personStation.GetByZoneCode(currentZone.FullCode, eLevelOption.SelfAndSubs);
            if (listVp == null || listVp.Count == 0)
            {
                this.ReportError("未获取可以导出的数据!");
                return;
            }

            foreach (var zone in selfAndSubsZones)
            {
                var curVps = listVp.FindAll(c => c.ZoneCode == zone.FullCode);
                string savePath = Path.Combine(fileName, CreateDirectoryHelper.CreateDirectory(groupMeta.AllZones, zone));
                TaskSingleRequireBookArgument meta = new TaskSingleRequireBookArgument();
                meta.FileName = savePath;
                meta.Database = dbContext;
                meta.CurrentZone = zone;
                meta.PublishDateSetting = groupMeta.PublishDateSetting;
                meta.SelectContractor = curVps;
                meta.TaskDesc = zone.Name;
                meta.DictList = groupMeta.DictList;
                TaskSingleRequireBookOperation taskConcord = new TaskSingleRequireBookOperation();
                taskConcord.Argument = meta;
                taskConcord.Name = "导出家庭承包单户申请书";
                taskConcord.Description = "导出" + zone.Name;
                Add(taskConcord);
            }
            base.OnGo();
        }

        #endregion
    }
}
