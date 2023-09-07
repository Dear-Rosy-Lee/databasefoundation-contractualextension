/*
 * (C) 2017 鱼鳞图公司版权所有,保留所有权利 
 */
using System; 
using YuLinTu.Windows; 

namespace YuLinTu.Component.ExportResultDataBaseTask
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
            LanguageAttribute.AddLanguage(YuLinTuQuality.Business.TaskBasic.Properties.Resources.FolderChs);
            YuLinTuQuality.Business.TaskBasic.GDALShapeFileWriter<string>.Registerdll();
        }

        #endregion
    }
}
