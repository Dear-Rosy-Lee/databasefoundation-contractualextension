/*
 * (C) 2020  鱼鳞图公司版权所有,保留所有权利 
 */
using YuLinTu.Windows;
using YuLinTu.Appwork;
using YuLinTu.Appwork.Task;

namespace YuLinTu.Component.CombineWordTask
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
                TypeTask = typeof(TaskCombineWordGroup),
                TypeArgument = typeof(TaskCombineWordGroupArgument)
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
