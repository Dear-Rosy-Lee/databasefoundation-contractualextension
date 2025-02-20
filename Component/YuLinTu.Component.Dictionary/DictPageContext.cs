/*
 * (C) 2025  鱼鳞图公司版权所有,保留所有权利 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YuLinTu.Appwork;
using YuLinTu.Windows;
using YuLinTu.Library.Business;
using YuLinTu.Windows.Wpf.Metro.Components;

namespace YuLinTu.Component.Dictionary
{
    /// <summary>
    /// 数据字典工作页上下文
    /// </summary>
    public class DictPageContext : TheWorkpageContext
    {
        #region Ctro

        /// <summary>
        /// 初始化工作页
        /// </summary>
        public DictPageContext(ITheWorkpage workpage)
            : base(workpage)
        {
        }

        #endregion

        #region Methods 接收消息


        /// <summary>
        /// 删除属性组消息提示
        /// </summary>

        [MessageHandler(ID = 111111)]
        public void OnDelDictGroupSuccess(object sender, ModuleMsgArgs e)
        {
            Workpage.Page.ShowDialog(new TabMessageBoxDialog
            {
                Header = "数据字典",
                Message = e.Count > 0 ? "共删除属性信息" + e.Count.ToString() + "条" : "该属性组无属性",
                MessageGrade = eMessageGrade.Error,
                CancelButtonText = "取消",
            });
        }

        #endregion

        #region Methods-override

        /// <summary>
        /// 账户配置数据读取
        /// </summary>
        protected override void OnInstallAccountData(object sender, AccountEventArgs e)
        {
            var profile = Workpage.Workspace.GetUserProfile();
            var section = profile.GetSection<DictLeftSidebarStateConfig>();
            var config = (section.Settings as DictLeftSidebarStateConfig);
            (PageContent as WorkpageFrame).LeftSidebarLength = config.Width;
            (PageContent as WorkpageFrame).IsLeftSidebarExpanded = config.IsExpanded;
        }

        /// <summary>
        /// 存储账户配置数据
        /// </summary>
        protected override void OnUninstallAccountData(object sender, AccountEventArgs e)
        {
            var profile = Workpage.Workspace.GetUserProfile();
            var section = profile.GetSection<DictLeftSidebarStateConfig>();
            var config = (section.Settings as DictLeftSidebarStateConfig);
            config.IsExpanded = (PageContent as WorkpageFrame).IsLeftSidebarExpanded;
            config.Width = (PageContent as WorkpageFrame).LeftSidebarLength;
        }

        #endregion
    }
}
