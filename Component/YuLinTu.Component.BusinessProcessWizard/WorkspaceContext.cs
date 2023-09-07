/*
 * (C) 2015  鱼鳞图公司版权所有,保留所有权利 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using YuLinTu.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using YuLinTu.Appwork;
using YuLinTu.Appwork.New;

namespace YuLinTu.Component.BusinessProcessWizard
{
   public class WorkspaceContext : TheWorkspaceContext
    {
        #region Ctor

        /// <summary>
        /// 构造函数
        /// </summary>
        public WorkspaceContext(IWorkspace workspace)
            : base(workspace)
        {
            //Register<TaskPage, TaskPageContext>();
            Register<BusinessProcessWizardFramePage, BusinessProcessWizardPageContext>();
        }

        #endregion

        #region method

        /// <summary>
        /// 添加项
        /// </summary>
        protected override void OnInstallNew(object sender, InstallNewEventArgs e)
        {
            e.Items.Add(new NewMetadata { Type = typeof(BusinessProcessWizardFramePage) });
        }
        #endregion
    }
}
