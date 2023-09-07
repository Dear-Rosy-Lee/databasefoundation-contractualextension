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
    /// 批量初始化界址点线任务参数
    /// </summary>
    public class TaskGroupInitializeLandNeighborInfoArgument : TaskArgument
    {
        #region Fields

        #endregion

        /// <summary>
        /// 数据库
        /// </summary>
        public IDbContext Database { get; set; }

        /// <summary>
        /// 数据库
        /// </summary>
        public IDbContext TempDatabase { get; set; }

        /// <summary>
        /// 当前地域
        /// </summary>
        public Zone CurrentZone { get; set; }

        public string TownZoneCode { get; set; }


        public double Tolerance { get; set; }

        #region Ctor

        /// <summary>
        /// 构造函数
        /// </summary>
        public TaskGroupInitializeLandNeighborInfoArgument()
        {

        }
        #endregion


    }
}
