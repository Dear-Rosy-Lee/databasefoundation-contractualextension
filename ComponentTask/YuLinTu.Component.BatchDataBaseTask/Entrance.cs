/*
 * (C) 2025  鱼鳞图公司版权所有,保留所有权利 
 */
using Quality.Business.TaskBasic.GDAL;
using YuLinTu.Windows;

namespace YuLinTu.Component.BatchDataBaseTask
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
            LanguageAttribute.AddLanguage(Quality.Business.TaskBasic.Properties.Resources.FolderChs);
            GDALShapeFileWriter<string>.Registerdll();
        }

        #endregion
    }
}
