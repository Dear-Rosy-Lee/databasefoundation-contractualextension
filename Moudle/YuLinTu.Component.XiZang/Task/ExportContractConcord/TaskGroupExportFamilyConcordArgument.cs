/*
 * (C) 2015  鱼鳞图公司版权所有,保留所有权利 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YuLinTu.Data;
using YuLinTu.Library.Business;
using YuLinTu.Library.Entity;

namespace YuLinTu.Component.XiZangLZ
{
    /// <summary>
    /// 批量导出合同任务参数
    /// </summary>
    public class TaskGroupExportFamilyConcordArgument : TaskArgument
    {
        #region Properties

        /// <summary>
        /// 数据库连接
        /// </summary>
        public IDbContext DbContext { get; set; }

        /// <summary>
        /// 当前地域
        /// </summary>
        public Zone CurrentZone { get; set; }

        /// <summary>
        /// 保存文件路径
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// 县级地域名称
        /// </summary>
        public string ZoneNameCounty { get; set; }

        /// <summary>
        /// 镇级地域名称
        /// </summary>
        public string ZoneNameTown { get; set; }

        /// <summary>
        /// 村级地域名称
        /// </summary>
        public string ZoneNameVillage { get; set; }

        /// <summary>
        /// 组级地域名称
        /// </summary>
        public string ZoneNameGroup { get; set; }


        /// <summary>
        /// 系统设置
        /// </summary>
        public SystemSetDefine systemset { get; set; }
        #endregion

        #region Ctor

        /// <summary>
        /// 构造函数
        /// </summary>
        public TaskGroupExportFamilyConcordArgument()
        {
        }

        #endregion
    }
}
