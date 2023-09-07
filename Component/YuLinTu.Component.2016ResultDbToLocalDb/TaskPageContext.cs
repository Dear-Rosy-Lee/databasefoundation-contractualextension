/*
 * (C) 2015  鱼鳞图公司版权所有,保留所有权利 
 */
using YuLinTu.Windows;
using YuLinTu.Appwork;
using YuLinTu.Appwork.Task;

namespace YuLinTu.Component.ResultDbof2016ToLocalDb
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
                TypeTask = typeof(ResultDbof2016ToLocalDb.TaskInstalllerServiceAccount),
                TypeArgument = typeof(ResultDbof2016ToLocalDb.TaskInstalllerServiceAccountArgument)
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
