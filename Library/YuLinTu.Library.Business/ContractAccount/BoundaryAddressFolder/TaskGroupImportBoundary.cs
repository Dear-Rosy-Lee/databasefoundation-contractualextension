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
    /// 批量导入界址调查表操作任务类
    /// </summary>
    public class TaskGroupImportBoundary : TaskGroup
    {
        #region Ctor

        /// <summary>
        /// 构造函数
        /// </summary>
        public TaskGroupImportBoundary()
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
            TaskImportBoundarytArgument groupMeta = Argument as TaskImportBoundarytArgument;
            if (groupMeta == null)
            {
                return;
            }
            IDbContext dbContext = groupMeta.Dbcontext;
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
                    string ext = Path.GetExtension(path);
                    if (zone.FullName.EndsWith(name) && (ext == ".xls" || ext == ".xlsx"))
                    {
                        filePath = path;
                        break;
                    }
                }
                if (string.IsNullOrEmpty(filePath))
                {
                    continue;
                }
                TaskImportBoundarytArgument meta = new TaskImportBoundarytArgument();
                meta.CurrentZone = zone;
                meta.LandorType = groupMeta.LandorType;
                meta.FileName = filePath;
                meta.Dbcontext = dbContext;
                meta.DicList = groupMeta.DicList;
                AgricultureLandDotSurvey import = new AgricultureLandDotSurvey();
                import.Argument = meta;
                import.Description = string.Format("导入{0}界址调查表", zone.FullName);
                import.Name = "导入界址调查表";
                Add(import);   //添加子任务到任务组中
            }
            base.OnGo();
        }

        #endregion
    }
}
