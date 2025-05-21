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

namespace YuLinTu.Library.Business
{
    public class TaskGroupExportLandCoilShapeOperation : TaskGroup
    {
        /// <summary>
        /// 开始执行组任务
        /// </summary>
        protected override void OnGo()
        {
            Clear();
            TaskGroupExportLandCoilShapeArgument groupMeta = Argument as TaskGroupExportLandCoilShapeArgument;
            if (groupMeta == null)
            {
                return;
            }
            IDbContext dbContext = groupMeta.Database;
            Zone currentZone = groupMeta.CurrentZone;
            string fileName = groupMeta.FileName;
            List<Zone> allZones = groupMeta.AllZones;
            foreach (var zone in allZones)
            {
                TaskExportLandCoilShapeArgument meta = new TaskExportLandCoilShapeArgument();
                meta.CurrentZone = zone;
                meta.FileName = fileName;
                meta.Database = dbContext;
                meta.DictList = groupMeta.DictList;
                TaskExportLandCoilShapeOperation import = new TaskExportLandCoilShapeOperation();
                import.Argument = meta;
                import.Description = zone.FullName;
                import.Name = "导出界址线Shape图斑";
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
            TaskGroupExportLandCoilShapeArgument groupMeta = Argument as TaskGroupExportLandCoilShapeArgument;
            System.Diagnostics.Process.Start(groupMeta.FileName);
            base.OpenResult();
        }

    }
}
