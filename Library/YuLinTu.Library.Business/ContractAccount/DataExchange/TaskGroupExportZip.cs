/*
 * (C) 2015  鱼鳞图公司版权所有,保留所有权利 
 */
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using YuLinTu.Data;
using YuLinTu.Library.Entity;

namespace YuLinTu.Library.Business
{
    /// <summary>
    /// 批量导出压缩包操作任务类
    /// </summary>
    public class TaskGroupExportZip : TaskGroup
    {
        #region Ctor

        /// <summary>
        /// 构造函数
        /// </summary>
        public TaskGroupExportZip()
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
            ArcLandImporArgument groupMeta = Argument as ArcLandImporArgument;
            if (groupMeta == null)
            {
                return;
            }
            IDbContext dbContext = groupMeta.Database;
            string fileName = groupMeta.FileName;
            List<Zone> allZones = groupMeta.ZoneList;
            //string[] fileArray = Directory.GetFiles(fileName);
            //if (fileArray.Length == 0)
            //{
            //    this.ReportError("选择目录下不存在文件");
            //    return;
            //}
            foreach (var zone in allZones)
            {
                ArcLandImporArgument zoneMeta = new ArcLandImporArgument();
                zoneMeta.Database = dbContext;
                zoneMeta.OpratorName = "Package";
                zoneMeta.CurrentZone = zone;
                zoneMeta.FileName = fileName;
                ArcLandExportProgress dataProgress = new ArcLandExportProgress();
                dataProgress.Name = "导出压缩包";
                dataProgress.Argument = zoneMeta;
                dataProgress.Description = string.Format("导出{0}下的数据压缩包", zone.FullName);
                Add(dataProgress);   //添加子任务到任务组中
            }
            base.OnGo();
        }

        #endregion
    }
}
