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
    /// 批量导入界址点图斑数据操作任务类
    /// </summary>
    public class TaskGroupImportDotOperation : TaskGroup
    {
        #region Ctor

        /// <summary>
        /// 构造函数
        /// </summary>
        public TaskGroupImportDotOperation()
        {
        }

        #endregion

        #region Method

        /// <summary>
        /// 开始执行组任务
        /// </summary>
        protected override void OnGo()
        {
            Clear();
            TaskGroupImportDotArgument groupMeta = Argument as TaskGroupImportDotArgument;
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
                TaskImportDotArgument meta = new TaskImportDotArgument();
                meta.CurrentZone = zone;
                meta.FileName = fileName;
                meta.Database = dbContext;
                meta.VirtualType = groupMeta.VirtualType;
                TaskImportDotOperation import = new TaskImportDotOperation();
                import.Argument = meta;
                import.Description = zone.FullName;
                import.Name = "导入界址点图斑";
                Add(import);   //添加子任务到任务组中
            }
            base.OnGo();
        }

        #endregion
    }
}
