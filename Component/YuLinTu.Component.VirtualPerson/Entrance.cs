/*
 * (C) 2015  鱼鳞图公司版权所有,保留所有权利 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YuLinTu.Data;
using YuLinTu.Library.Business;
using YuLinTu.Library.Controls;
using YuLinTu.Library.Entity;
using YuLinTu.Windows;
using YuLinTu.Windows.Wpf.Metro.Components;

namespace YuLinTu.Component.VirtualPerson
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
            RegisterWorkspaceContext<WorkspaceContext>();
            RegisterWorkstationContext<VirtualPersonMessageContext>();
            TheApp.SettingsTypes.Add(typeof(YuLinTu.Library.Business.FamilyImportDefine));
            TheApp.SettingsTypes.Add(typeof(YuLinTu.Library.Business.FamilyOutputDefine));
            TheApp.SettingsTypes.Add(typeof(YuLinTu.Library.Business.FamilyDefine));
            RegisterWorkstationContext<AccountLandMessageContext>(); 
        }

        #endregion
    }
}
