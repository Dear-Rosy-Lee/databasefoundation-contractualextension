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
    /// 家庭关系替换
    /// </summary>
    public class TaskGroupInitialVPRelationShipArgument : TaskArgument
    {
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
        /// 查找的内容
        /// </summary>
        public string ReplaceName { get; set; }
        /// <summary>
        /// 替换为
        /// </summary>
        public string ChooseName { get; set; }

        #endregion

        #region Ctor

        /// <summary>
        /// 构造函数
        /// </summary>
        public TaskGroupInitialVPRelationShipArgument()
        {
        }

        #endregion
    }
}
