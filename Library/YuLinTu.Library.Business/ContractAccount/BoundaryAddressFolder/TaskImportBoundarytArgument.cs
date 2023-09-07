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
    /// 导入界址任务参数
    /// </summary>
    public class TaskImportBoundarytArgument : TaskArgument
    {
        #region Fields

        #endregion

        #region Properties

        /// <summary>
        /// 当前地域
        /// </summary>
        public Zone CurrentZone { get; set; }

        /// <summary>
        /// 地域集合
        /// </summary>
        public List<Zone> ZoneList { get; set; }

        /// <summary>
        /// 地块类型
        /// </summary>
        public LanderType LandorType { get; set; }

        /// <summary>
        /// 数据库
        /// </summary>
        public IDbContext Dbcontext { get; set; }

        /// <summary>
        /// 文件名称
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// 数据字典
        /// </summary>
        public List<Dictionary> DicList { get; set; }

        /// <summary>
        /// 是否批量
        /// </summary>
        public bool isBatch { get; set; }

        #endregion

        #region Ctor

        /// <summary>
        /// 构造函数
        /// </summary>
        public TaskImportBoundarytArgument()
        {
            LandorType = LanderType.AgricultureLand;
        }

        #endregion
    }
}
