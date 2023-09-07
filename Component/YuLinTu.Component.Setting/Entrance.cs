/*
 * (C) 2015  鱼鳞图公司版权所有,保留所有权利 
 */
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using YuLinTu;
using YuLinTu.Data;
using YuLinTu.Windows;

namespace YuLinTu.Component.Setting
{
    /// <summary>
    /// 配置插件应用程序上下文
    /// </summary>
    public class Entrance:EntranceBase
    {
        #region Methods

        /// <summary>
        /// 重写注册工作空间方法
        /// </summary>
        protected override void OnConnect()
        {
            LanguageAttribute.AddLanguage(Properties.Resources.langChs);
            RegisterWorkspaceContext<WorkspaceContext>();
        }

        #endregion
    }
}
