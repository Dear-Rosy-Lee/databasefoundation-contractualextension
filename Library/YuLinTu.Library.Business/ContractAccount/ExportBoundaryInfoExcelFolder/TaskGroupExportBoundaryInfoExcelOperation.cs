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
    /// <summary>
    /// 批量导出界址信息调查表任务类
    /// </summary>
    public class TaskGroupExportBoundaryInfoExcelOperation : TaskGroup
    {
        #region Field

        private string openFilePath;  //打开文件路径

        #endregion

        #region Method - Override

        /// <summary>
        /// 开始执行组任务
        /// </summary>
        protected override void OnGo()
        {
            Clear();
            TaskGroupExportBoundaryInfoExcelArgument groupMeta = Argument as TaskGroupExportBoundaryInfoExcelArgument;
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
                TaskExportBoundaryInfoExcelArgument meta = new TaskExportBoundaryInfoExcelArgument();
                meta.FileName = openFilePath;
                meta.ArgType = groupMeta.ArgType;
                meta.Database = dbContext;
                meta.CurrentZone = zone;
                meta.DictList = groupMeta.DictList;
                TaskExportBoundaryInfoExcelOperation import = new TaskExportBoundaryInfoExcelOperation();
                import.Argument = meta;
                import.Description = "导出" + zone.FullName;
                import.Name = "导出界址信息表";
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
            TaskGroupExportBoundaryInfoExcelArgument groupMeta = Argument as TaskGroupExportBoundaryInfoExcelArgument;
            System.Diagnostics.Process.Start(groupMeta.FileName);
            base.OpenResult();
        }

        #endregion
    }
}
