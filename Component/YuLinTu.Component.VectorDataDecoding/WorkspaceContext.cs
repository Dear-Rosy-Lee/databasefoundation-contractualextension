using YuLinTu.Appwork;
using YuLinTu.Windows;

namespace YuLinTu.Component.VectorDataDecoding
{
    [Workspace(null)]
    public class WorkspaceContext : TheWorkspaceContext
    {
        #region Fields

        #endregion Fields

        #region Ctor

        public WorkspaceContext(IWorkspace workspace)
            : base(workspace)
        {
   
            Register<VectorDataDecodePage, WorkpageContext>();
        }

        #endregion Ctor2

        #region method


        ///// <summary>
        ///// 系统管理选项卡
        ///// </summary>
        //[MessageHandler(ID = EdCore.langInstallOptionsEditor)]
        //private void langInstallOptionsEditor(object sender, InstallOptionsEditorEventArgs e)
        //{
        //    Workspace.Window.Dispatcher.Invoke(new Action(() =>
        //    {
        //        e.Editors.Add(new OptionsEditorMetadata()
        //        {
        //            Name = "文件系统",
        //            //Editor = new VirtualFileOptionsEditor(Workspace),
        //        });
        //    }));
        //}

        #endregion method
    }
}