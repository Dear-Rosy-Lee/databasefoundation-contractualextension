/*
 * (C) 2015  鱼鳞图公司版权所有,保留所有权利 
 */
using YuLinTu.Windows;
using YuLinTu.Appwork;
using YuLinTu.Component.ContractAccount;
using YuLinTu.Component.Concord;
using YuLinTu.Component.ContractRegeditBook;

namespace YuLinTu.Component.XiZangLZ
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
            Register<ContractAccountFramePage, WorkspacePageContext>();
            Register<ConcordFramePage, WorkspacePageContext>();
            Register<ContractRegeditBookFramePage, WorkspacePageContext>();
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
