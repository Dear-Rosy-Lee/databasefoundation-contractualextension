/*
 * (C) 2025  鱼鳞图公司版权所有,保留所有权利 
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
    public class TaskGroupExportConcordDataOperation : TaskGroup
    {

        private string openFilePath;  //打开文件路径

        /// <summary>
        /// 开始执行组任务
        /// </summary>
        protected override void OnGo()
        {
            Clear();
            TaskGroupExportConcordDataArgument groupMeta = Argument as TaskGroupExportConcordDataArgument;
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
                openFilePath = Path.Combine(fileName, CreateDirectoryHelper.CreateDirectory(groupMeta.AllZones, zone));
                TaskExportConcordDataArgument meta = new TaskExportConcordDataArgument();
                var selfSubsZones = zoneStation.GetChildren(zone.FullCode, eLevelOption.SelfAndSubs);
                meta.FileName = openFilePath;
                meta.ArgType = eContractConcordArgType.ExportConcordData;
                meta.Database = dbContext;
                meta.CurrentZone = zone;
                meta.PublishDateSetting = groupMeta.PublishDateSetting;
                meta.SummaryDefine = groupMeta.SummaryDefine;
                meta.SelfAndSubsZones = selfSubsZones;
                meta.AllZones = groupMeta.AllZones;
                meta.AreaType = groupMeta.AreaType;
                meta.SystemSet = groupMeta.SystemSet;
                TaskExportConcordDataOperation taskConcord = new TaskExportConcordDataOperation();
                taskConcord.Argument = meta;               
                taskConcord.Name = "导出合同";
                taskConcord.Description = "导出" + zone.FullName;
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
            TaskGroupExportConcordDataArgument groupMeta = Argument as TaskGroupExportConcordDataArgument;
            System.Diagnostics.Process.Start(groupMeta.FileName);
            base.OpenResult();
        }
    }
}
