/*
 * (C) 2025  鱼鳞图公司版权所有,保留所有权利 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YuLinTu.Library.Business;
using YuLinTu.Windows;

namespace YuLinTu.Component.Zone
{
    /// <summary>
    /// 插件入口
    /// </summary>
    public class Entrance : EntranceBase
    {
        #region Methods

        /// <summary>
        /// 重写注册工作空间方法
        /// </summary>
        protected override void OnConnect()
        {
            LanguageAttribute.AddLanguage(Properties.Resources.langChs);
            RegisterWorkspaceContext<WorkspaceContext>();
            RegisterWorkstationContext<ZoneMessageContext>();
        }

        #endregion
    }
}
