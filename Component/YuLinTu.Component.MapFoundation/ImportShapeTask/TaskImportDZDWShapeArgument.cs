/*
 * (C) 2015  鱼鳞图公司版权所有,保留所有权利 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YuLinTu.Data;
using YuLinTu.Library.Entity;

namespace YuLinTu.Component.MapFoundation
{
   public class TaskImportDZDWShapeArgument: TaskArgument
    {
        /// <summary>
        /// 数据库
        /// </summary>
        public IDbContext Database { get; set; }

        /// <summary>
        /// 当前地域
        /// </summary>
        public Zone CurrentZone { get; set; }

        /// <summary>
        /// 导入文件名称
        /// </summary>
        public string FileName { get; set; }
        public string Type { get; set; }

        public ImportDZDWDefine importDZDWDefine { get; set; }


        /// <summary>
        /// 当前选择图层名称
        /// </summary>
        public string SelectLayerName { get; set; }

        public TaskImportDZDWShapeArgument()
        {
                
        }
    }
}
