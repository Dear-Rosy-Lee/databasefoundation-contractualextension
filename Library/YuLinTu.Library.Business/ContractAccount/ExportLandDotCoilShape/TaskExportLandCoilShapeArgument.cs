/*
 * (C) 2015  鱼鳞图公司版权所有,保留所有权利 
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
    /// 导出地块界址线Shape任务参数
    /// </summary>
    public class TaskExportLandCoilShapeArgument : TaskArgument
    {
        #region Fields

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
        /// 文件路径
        /// </summary>
        public string FileName { get; set; }
       


        /// <summary>
        /// 子级地域集合(包括当前地域)
        /// </summary>
        public List<Zone> AllZones { get; set; }

        /// <summary>
        /// 数据字典
        /// </summary>
        public List<Dictionary> DictList { get; set; }

        #endregion
    }
}
