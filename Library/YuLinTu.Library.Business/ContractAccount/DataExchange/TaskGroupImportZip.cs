/*
 * (C) 2025  鱼鳞图公司版权所有,保留所有权利 
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
    /// 批量导入压缩包操作任务类
    /// </summary>
    public class TaskGroupImportZip : TaskGroup
    {
        #region Ctor

        /// <summary>
        /// 构造函数
        /// </summary>
        public TaskGroupImportZip()
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
            string[] fileArray = Directory.GetFiles(fileName);
            if (fileArray.Length == 0)
            {
                this.ReportError("选择目录下不存在文件");
                return;
            }
            foreach (var zone in allZones)
            {
                string filePath = string.Empty;
                for (int i = 0; i < fileArray.Length; i++)
                {
                    string path = fileArray[i];
                    string name = Path.GetFileNameWithoutExtension(path);
                    int index = name.IndexOf("(");
                    if (index > 0)
                        name = name.Substring(0, index);
                    string ext = Path.GetExtension(path);
                    if (zone.FullName.EndsWith(name) && (ext == ".zip"))
                    {
                        filePath = path;
                        break;
                    }
                }
                if (string.IsNullOrEmpty(filePath))
                {
                    continue;
                }
                ArcLandImporArgument meta = new ArcLandImporArgument();
                meta.CurrentZone = zone;
                meta.LandorType = groupMeta.LandorType;
                meta.FileName = filePath;
                meta.Database = dbContext;
                ArcLandImportProgress import = new ArcLandImportProgress();
                import.Name = "批量导入压缩包数据";
                import.Description = string.Format("导入{0}压缩包数据", zone.FullName);
                import.Argument = meta;
                Add(import);   //添加子任务到任务组中
            }
            base.OnGo();
        }

        #endregion
    }
}
