/*
 * (C) 2015  鱼鳞图公司版权所有,保留所有权利 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YuLinTu.Library.Business
{
    /// <summary>
    /// 地域插件的所有事件定义
    /// </summary>
    public class ZoneEventDefine : Ed
    {
        #region Fields

        /// <summary>
        /// 向工具栏中添加按钮的事件ID
        /// </summary>
        public const int AddInstallToolbarButton= 200001;

        #endregion

        #region Ctor

        static ZoneEventDefine()
        {
        }

        #endregion
    }
}
