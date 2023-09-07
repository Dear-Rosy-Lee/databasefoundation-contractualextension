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
    /// 导出村组公示表
    /// </summary>
    public class TaskGroupExportVillagesDeclareOperation : TaskGroup
    {
        private string openFilePath;  //打开文件路径

        /// <summary>
        /// 开始执行组任务
        /// </summary>
        protected override void OnGo()
        {
            Clear();
            TaskGroupExportVillagesDeclareArgument groupMeta = Argument as TaskGroupExportVillagesDeclareArgument;
            if (groupMeta == null)
            {
                return;
            }
            IDbContext dbContext = groupMeta.Database;
            Zone currentZone = groupMeta.CurrentZone;
            string fileName = groupMeta.FileName;
            List<Zone> selfAndSubsZones = groupMeta.SelfAndSubsZones;

            foreach (var zone in selfAndSubsZones)
            {
                openFilePath = Path.Combine(fileName, CreateDirectoryHelper.CreateDirectoryByVilliage(groupMeta.AllZones, zone));
                TaskExportVillagesDeclareArgument meta = new TaskExportVillagesDeclareArgument();
                meta.IsClear = false;
                meta.FileName = openFilePath;
                meta.ArgType = groupMeta.ArgType;
                meta.Database = dbContext;
                meta.CurrentZone = zone;
                meta.VirtualType = groupMeta.VirtualType;
                meta.UserName = "";
                meta.Date = groupMeta.Date;
                meta.AllZones = groupMeta.AllZones;
                meta.SelfAndSubsZones = groupMeta.SelfAndSubsZones;
                meta.PublishDateSetting = groupMeta.PublishDateSetting;
                meta.DictList = groupMeta.DictList;
                TaskExportVillagesDeclareOperation import = new TaskExportVillagesDeclareOperation();
                import.Argument = meta;
                import.Description = "导出" + zone.FullName;
                import.Name = "导出村组公告表";
                Add(import);   //添加子任务到任务组中
            }
            CanOpenResult = true;
            base.OnGo();
        }

        /// <summary>
        /// 打开文件
        /// </summary>
        public override void OpenResult()
        {
            TaskGroupExportVillagesDeclareArgument groupMeta = Argument as TaskGroupExportVillagesDeclareArgument;
            System.Diagnostics.Process.Start(groupMeta.FileName);
            base.OpenResult();
        }
    }
}
