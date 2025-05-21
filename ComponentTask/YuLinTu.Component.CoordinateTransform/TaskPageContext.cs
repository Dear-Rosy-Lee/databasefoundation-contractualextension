/*
 * (C) 2022 鱼鳞图公司版权所有,保留所有权利 
 */
using YuLinTu.Appwork;
using YuLinTu.Appwork.Task;
using YuLinTu.Windows;

namespace YuLinTu.Component.CoordinateTransformTask
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
                TypeTask = typeof(CoordDefinitionTask),
                TypeArgument = typeof(CoordDefinitionArgument)
            });
            e.Templates.Add(new TaskDescriptor
            {
                TypeTask = typeof(CoordTransTask),
                TypeArgument = typeof(CoordTransArgument)
            });
           
            e.Templates.Add(new TaskDescriptor
            {
                TypeTask = typeof(StripNumberTask),
                TypeArgument = typeof(StripNumberArgument)
            });
            e.Templates.Add(new TaskDescriptor
            {
                TypeTask = typeof(LevelMoveTask),
                TypeArgument = typeof(LevelMoveArgument)
            });
            e.Templates.Add(new TaskDescriptor
            {
                TypeTask = typeof(CoordParamsTask),
                TypeArgument = typeof(CoordParamsArgument)
            });
            e.Templates.Add(new TaskDescriptor
            {
                TypeTask = typeof(CoordShiftingTask),
                TypeArgument = typeof(CoordShiftingArgument)
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
