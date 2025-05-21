using YuLinTu.Appwork;
using YuLinTu.Appwork.Task;
using YuLinTu.Windows;

namespace YuLinTu.Component.AssociateLandCode
{
    public class TaskPageContext : WorkpageContextTask
    {
        public TaskPageContext(IWorkpage workpage)
            : base(workpage)
        {
        }

        #region Methods

        #region Methods - Override

        [MessageHandler(ID = EdTask.LangInstallTaskTemplates)]
        private void OnInstallTaskTemplates(object sender, InstallTaskTemplatesEventArgs e)
        {
            e.Templates.Add(new TaskDescriptor
            {
                TypeTask = typeof(AssociateLandCodeTask),
                TypeArgument = typeof(AssociateLandCodeArgument)
            });
            e.Templates.Add(new TaskDescriptor
            {
                TypeTask = typeof(AssociatePersonAndLandTask),
                TypeArgument = typeof(AssociatePersonAndLandArgument)
            });
        }

        protected override bool NeedHandleMessage()
        {
            return TheApp.Current.GetIsAuthenticated();
        }

        #endregion Methods - Override

        #region Methods - Private

        [MessageHandler(ID = EdTask.langViewTaskAlertDetails)]
        private void ViewTaskAlterDetails(object sender, ViewTaskAlertDetailsEventArgs e)
        {

        }

        #endregion Methods - Private

        #endregion Methods
    }
}
