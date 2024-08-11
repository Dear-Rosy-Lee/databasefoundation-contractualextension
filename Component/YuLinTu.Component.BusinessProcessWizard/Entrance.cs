/*
 * (C) 2024  鱼鳞图公司版权所有,保留所有权利 
 */
using YuLinTu.Windows;

namespace YuLinTu.Component.BusinessProcessWizard
{
    public class Entrance : EntranceBase
    {
        #region Methods

        /// <summary>
        /// 重写注册工作空间方法
        /// </summary>
        protected override void OnConnect()
        {      
            RegisterWorkspaceContext<WorkspaceContext>();          
        }

        #endregion
    }
}
