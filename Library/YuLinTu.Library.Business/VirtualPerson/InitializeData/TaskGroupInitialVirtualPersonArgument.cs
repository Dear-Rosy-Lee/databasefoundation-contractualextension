/*
 * (C) 2025  鱼鳞图公司版权所有,保留所有权利 
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
    /// 批量初始化承包方基本信息任务参数
    /// </summary>
    public class TaskGroupInitialVirtualPersonArgument : TaskInitialVirtualPersonArgument
    {
        #region Properties         

        /// <summary>
        /// 承包方其它设置
        /// </summary>
        public FamilyOtherDefine FamilyOtherSet { get; set; }

        #endregion

        #region Ctor

        /// <summary>
        /// 构造函数
        /// </summary>
        public TaskGroupInitialVirtualPersonArgument()
        {
            VirtualType = eVirtualType.Land;
        }

        #endregion
    }
}
