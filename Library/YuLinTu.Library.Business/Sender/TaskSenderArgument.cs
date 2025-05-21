/*
 * (C) 2025  鱼鳞图公司版权所有,保留所有权利 
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

namespace YuLinTu.Library.Business
{
    /// <summary>
    /// 发包方数据参数
    /// </summary>
    public class TaskSenderArgument : TaskArgument
    {
        #region Property

        /// <summary>
        /// 文件路径
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// 清空数据
        /// </summary>
        public bool IsClear { get; set; }

        /// <summary>
        /// 数据库
        /// </summary>
        public IDbContext Database { get; set; }

        /// <summary>
        /// 当前地域
        /// </summary>
        public Zone CurrentZone { get; set; }

        /// <summary>
        /// 参数类型
        /// </summary>
        public eSenderArgType ArgType { get; set; }

        /// <summary>
        /// 子集地域
        /// </summary>
        public List<Zone> ChildrenZone { get; set; }

        #endregion
    }
}