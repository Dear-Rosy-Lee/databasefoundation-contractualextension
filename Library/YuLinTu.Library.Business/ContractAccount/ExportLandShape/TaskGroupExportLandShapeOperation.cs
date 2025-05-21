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
    public class TaskGroupExportLandShapeOperation : TaskGroup
    {

        private string openFilePath;  //打开文件路径

        /// <summary>
        /// 开始执行组任务
        /// </summary>
        protected override void OnGo()
        {
            Clear();
            TaskGroupExportLandShapeArgument groupMeta = Argument as TaskGroupExportLandShapeArgument;
            if (groupMeta == null)
            {
                return;
            }
            IDbContext dbContext = groupMeta.Database;
            Zone currentZone = groupMeta.CurrentZone;
            string fileName = groupMeta.FileName;
            List<Zone> selfAndSubsZones = groupMeta.AllZones;
            var zoneStation = dbContext.CreateZoneWorkStation();
            var allZones = zoneStation.GetAllZones(currentZone);

            foreach (var zone in selfAndSubsZones)
            {
                openFilePath = Path.Combine(fileName, CreateDirectoryHelper.CreateDirectoryByVilliage(allZones, zone));
                TaskExportLandShapeArgument meta = new TaskExportLandShapeArgument();
                meta.CurrentZone = zone;
                meta.FileName = openFilePath;
                meta.Database = dbContext;
                meta.DictList = groupMeta.DictList;
                meta.vps = groupMeta.vps;
                TaskExportLandShapeOperation import = new TaskExportLandShapeOperation();
                import.Argument = meta;
                import.Description = zone.FullName;
                import.Name = "导出Shape图斑";
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
            TaskGroupExportLandShapeArgument groupMeta = Argument as TaskGroupExportLandShapeArgument;
            System.Diagnostics.Process.Start(groupMeta.FileName);
            base.OpenResult();
        }
    }
}
