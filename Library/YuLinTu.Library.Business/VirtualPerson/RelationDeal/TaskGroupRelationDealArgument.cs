/*
 * (C) 2015  鱼鳞图公司版权所有,保留所有权利 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using YuLinTu.Library.Entity;
using YuLinTu;
using YuLinTu.Data;
using YuLinTu.Library.Business;
using System.Collections.ObjectModel;

namespace YuLinTu.Library.Business
{
    /// <summary>
    /// 家庭关系处理
    /// </summary>
    public class TaskGroupRelationDealArgument : TaskArgument
    {
        #region Fields      

        #endregion

        #region Property

       
        /// <summary>
        /// 数据库
        /// </summary>
        public IDbContext Database { get; set; }

        /// <summary>
        /// 当前地域
        /// </summary>
        public Zone CurrentZone { get; set; }
        public int Type { get; set; }


        #endregion

        #region Ctor

        /// <summary>
        /// 构造函数
        /// </summary>
        public TaskGroupRelationDealArgument()
        {
                        
        }

        #endregion
    }
}