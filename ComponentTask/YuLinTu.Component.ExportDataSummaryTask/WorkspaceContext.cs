/*
 * (C) 2024  鱼鳞图公司版权所有,保留所有权利 
 */
using YuLinTu.Appwork;
using YuLinTu.Windows;

namespace YuLinTu.Component.ExportDataSummaryTask
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
            Register<DataSummaryExportFramePage, ExportDataSummaryPageContext>();
        }

        #endregion

        #region method             
              
        /// <summary>
        /// 添加项
        /// </summary>
        protected override void OnInstallNew(object sender, InstallNewEventArgs e)
        {
            e.Items.Add(new NewMetadata { Type = typeof(DataSummaryExportFramePage) });
        }
        #endregion
    }
}
