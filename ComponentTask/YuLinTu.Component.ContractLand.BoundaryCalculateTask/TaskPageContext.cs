/*
 * (C) 2025  鱼鳞图公司版权所有,保留所有权利 
 */
using YuLinTu.Appwork;
using YuLinTu.Appwork.Task;
using YuLinTu.Windows;

namespace YuLinTu.Component.ContractedLand.BoundaryCalculateTask
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
            e.Templates.Add(new TaskDescriptor
            {
                TypeTask = typeof(TaskBoundaryCalculate),
                TypeArgument = typeof(TaskBoundaryCalculateArgument)
            });
        }

        protected override bool NeedHandleMessage()
        {
            return TheApp.Current.GetIsAuthenticated();
        }

        #endregion

        #endregion
    }

}
