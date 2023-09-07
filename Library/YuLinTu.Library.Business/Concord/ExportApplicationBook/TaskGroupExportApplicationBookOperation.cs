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

namespace YuLinTu.Library.Business
{
    /// <summary>
    /// 批量导出集体申请书
    /// </summary>
    public class TaskGroupExportApplicationBookOperation : TaskGroup
    {

        private string openFilePath;  //打开文件路径

        /// <summary>
        /// 开始执行组任务
        /// </summary>
        protected override void OnGo()
        {
            Clear();
            TaskGroupExportApplicationBookArgument groupMeta = Argument as TaskGroupExportApplicationBookArgument;
            if (groupMeta == null)
            {
                return;
            }
            IDbContext dbContext = groupMeta.Database;
            Zone currentZone = groupMeta.CurrentZone;
            string fileName = groupMeta.FileName;
            List<Zone> selfAndSubsZones = groupMeta.SelfAndSubsZones;
            var zoneStation = dbContext.CreateZoneWorkStation();
            foreach (var zone in selfAndSubsZones)
            {
                var SelfSubsZones = zoneStation.GetChildren(zone.FullCode, eLevelOption.SelfAndSubs);  //当前地域下的
                openFilePath = Path.Combine(fileName, CreateDirectoryHelper.CreateDirectory(groupMeta.AllZones, zone));
                TaskExportApplicationBookArgument meta = new TaskExportApplicationBookArgument();
                meta.FileName = openFilePath;
                meta.ArgType = eContractConcordArgType.ExportConcordData;
                meta.Database = dbContext;
                meta.CurrentZone = zone;             
                meta.SelfAndSubsZones = SelfSubsZones;
                meta.AllZones = groupMeta.AllZones;
                meta.SystemSet = groupMeta.SystemSet;
                meta.PublishDateSetting = groupMeta.PublishDateSetting;
                TaskExportApplicationBookOperation taskConcord = new TaskExportApplicationBookOperation();
                taskConcord.Argument = meta;
                taskConcord.Name = "导出集体申请书";
                taskConcord.Description = "导出" + zone.Name;
                Add(taskConcord);   //添加子任务到任务组中
            }
            CanOpenResult = true;
            base.OnGo();
        }

        /// <summary>
        /// 打开文件
        /// </summary>
        public override void OpenResult()
        {
            TaskGroupExportApplicationBookArgument groupMeta = Argument as TaskGroupExportApplicationBookArgument;
            System.Diagnostics.Process.Start(groupMeta.FileName);
            base.OpenResult();
        }
    }
}
