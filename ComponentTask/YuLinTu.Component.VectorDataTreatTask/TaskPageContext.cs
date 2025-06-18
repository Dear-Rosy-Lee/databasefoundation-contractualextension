using YuLinTu.Windows;
using YuLinTu.Appwork;
using YuLinTu.Appwork.Task;

namespace YuLinTu.Component.VectorDataTreatTask
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
            //e.Templates.Add(new TaskDescriptor
            //{
            //    TypeTask = typeof(VectorDataUpdateTask),
            //    TypeArgument = typeof(VectorDataUpdateArgument)
            //});

            //e.Templates.Add(new TaskDescriptor
            //{
            //    TypeTask = typeof(SpatialDataMatchingTask),
            //    TypeArgument = typeof(SpatialDataMatchingArgument)
            //});
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