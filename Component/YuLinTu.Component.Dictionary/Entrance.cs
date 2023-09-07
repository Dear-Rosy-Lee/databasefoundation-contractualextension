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
using YuLinTu.Library.Business;
using YuLinTu.Library.Controls;
using YuLinTu.Library.Entity;
using YuLinTu.Library.Repository;
using YuLinTu.Library.WorkStation;
using YuLinTu.Windows;

namespace YuLinTu.Component.Dictionary
{
    /// <summary>
    /// 数据字典插件应用程序上下文
    /// </summary>
    public class Entrance : EntranceBase
    {
        #region Methods

        /// <summary>
        /// 重写注册工作空间方法
        /// 应用程序上下文，其中注册了一个工作空间上下文
        /// </summary>
        protected override void OnConnect()
        {
            LanguageAttribute.AddLanguage(Properties.Resources.langChs);
            RegisterWorkspaceContext<WorkspaceContext>();
            YuLinTu.Windows.TheApp.SettingsTypes.Add(typeof(DictLeftSidebarStateConfig));
            RegisterWorkstationContext<DictionaryMessageContext>();  //注册接收消息类
        }

        #endregion
    }
}
