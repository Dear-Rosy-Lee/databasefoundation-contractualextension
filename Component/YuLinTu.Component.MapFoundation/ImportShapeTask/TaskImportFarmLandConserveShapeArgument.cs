/*
 * (C) 2025  鱼鳞图公司版权所有,保留所有权利 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YuLinTu.Data;
using YuLinTu.Library.Entity;

namespace YuLinTu.Component.MapFoundation
{
    /// <summary>
    /// 导入基本农田保护区图斑参数
    /// </summary>
    public class TaskImportFarmLandConserveShapeArgument : TaskArgument
    {
        #region Ctor

        /// <summary>
        /// 构造函数
        /// </summary>
        public TaskImportFarmLandConserveShapeArgument()
        {
        }

        #endregion

        #region Properties

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

        /// <summary>
        /// 导入基本农田保护区图斑配置实体
        /// </summary>
        public ImportFarmLandConserveDefine ImportConserveDefine { get; set; }

        /// <summary>
        /// 当前选择图层名称
        /// </summary>
        public string SelectLayerName { get; set; }

        #endregion
    }
}
