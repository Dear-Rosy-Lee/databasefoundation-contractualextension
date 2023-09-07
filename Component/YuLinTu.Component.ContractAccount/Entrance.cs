/*
 * (C) 2015  鱼鳞图公司版权所有,保留所有权利 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YuLinTu.Windows;
using YuLinTu;
using YuLinTu.Data;
using YuLinTu.Library.Controls;
using System.Collections.ObjectModel;
using YuLinTu.Library.Business;
using System.Windows;

namespace YuLinTu.Component.ContractAccount
{
    /// <summary>
    /// 承包台账插件入口
    /// </summary>
    public class Entrance : EntranceBase
    {
        #region Method

        /// <summary>
        /// 重写注册工作空间方法
        /// 应用程序上下文，其中注册了一个工作空间上下文
        /// </summary>
        protected override void OnConnect()
        {
            RegisterWorkspaceContext<WorkspaceContext>();
            Application.Current.Resources.MergedDictionaries.Add(
                new ResourceDictionary() { Source = new Uri("pack://application:,,,/YuLinTu.Diagrams;component/Resources/Res.xaml") });
        }

        #endregion
    }
}
