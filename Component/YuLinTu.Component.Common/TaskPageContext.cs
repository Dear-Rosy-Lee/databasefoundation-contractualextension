/*
 * (C) 2015  鱼鳞图公司版权所有,保留所有权利 
 */
using System;
using YuLinTu.Appwork;
using YuLinTu.Appwork.Task;
using YuLinTu.Windows;


namespace YuLinTu.Component.Common
{
    public class TaskPageContext : WorkpageContextTask
    {
        #region Fields
       
        #endregion

        #region Ctor

        public TaskPageContext(IWorkpage workpage)
            : base(workpage)
        {

        }

        #endregion

        #region Methods

        #region Methods - Override

        [MessageHandler(ID = EdTask.LangInstallTaskTemplates)]
        private void OnInstallTaskTemplates(object sender, InstallTaskTemplatesEventArgs e)
        {
            //e.Templates.Add(new TaskDescriptor
            //{
            //    TypeTask = typeof(TaskBuildExportResultDataBase),
            //    TypeArgument = typeof(TaskBuildExportResultDataBaseArgument)
            //});
        }
        
        protected override bool NeedHandleMessage()
        {
            return TheApp.Current.GetIsAuthenticated();
        }


        #endregion

        #region Methods - Events

        #endregion

        #region Methods - Private

        /// <summary>
        /// 系统配置
        /// </summary>
        [MessageHandler(ID = EdCore.langInstallWorkpageOptionsEditor)]
        private void langInstallWorkpageOptionsEditor(object sender, InstallWorkpageOptionsEditorEventArgs e)
        {
            Workpage.Workspace.Window.Dispatcher.Invoke(new Action(() =>
            {
                e.Editors.Add(new WorkpageOptionsEditorMetadata()
                {
                    Name = "导出设置",
                    Editor = new ExportLandDotCoilSetControl(Workpage),
                });
            }));
        }

        #endregion

        #endregion
    }

}
