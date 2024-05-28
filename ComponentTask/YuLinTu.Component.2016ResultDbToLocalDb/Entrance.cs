/*
 * (C) 2015  鱼鳞图公司版权所有,保留所有权利 
 */
using YuLinTu.Windows;

namespace YuLinTu.Component.ResultDbToLocalDb
{
    /// <summary>
    /// 应用程序上下文
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
            //LanguageAttribute.AddLanguage(YuLinTu.Business.TaskBasic.Properties.Resources.FolderChs);
        }

        #endregion
    }
}
