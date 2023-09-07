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

namespace YuLinTu.Library.Business
{
    /// <summary>
    /// 二轮台账数据参数
    /// </summary>
    public class TaskSecondTableArgument : TaskArgument
    {
        #region Field

        /// <summary>
        /// 承包方类型
        /// </summary>
        public eVirtualType virtualType;

        #endregion

        #region Property

        /// <summary>
        /// 文件路径
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// 文件目录
        /// </summary>
        public string FolderDir{ get; set; }

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
        public eSecondTableArgType ArgType { get; set; }

        /// <summary>
        /// 承包方类型
        /// </summary>
        public eVirtualType VirtualType
        {
            get { return virtualType; }
            set
            {
                virtualType = value;
            }
        }

        /// <summary>
        /// 时间
        /// </summary>
        public DateTime? DateValue { get; set; }

        /// <summary>
        /// 公示时间
        /// </summary>
        public DateTime? PubDateValue { get; set; }

        /// <summary>
        /// 使用者
        /// </summary>
        public string UserName { get; set; }

        #endregion

        #region Ctor

        public TaskSecondTableArgument()
        {
            virtualType = eVirtualType.SecondTable;
        }

        #endregion
    }
}

