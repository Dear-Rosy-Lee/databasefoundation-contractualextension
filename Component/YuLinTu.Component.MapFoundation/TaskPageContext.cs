/*
 * (C) 2025  鱼鳞图公司版权所有,保留所有权利 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Controls;
using YuLinTu.Windows;
using YuLinTu.Windows.Wpf;
using YuLinTu.Windows.Wpf.Metro;
using YuLinTu.Windows.Wpf.Metro.Components;
using System.Windows;
using YuLinTu.Appwork;
using YuLinTu.Appwork.Task;

namespace YuLinTu.Component.MapFoundation
{
    /// <summary>
    /// 任务上下文
    /// </summary>
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
           
        }

        protected override bool NeedHandleMessage()
        {
            return TheApp.Current.GetIsAuthenticated();
        }

        #endregion

        #region Methods - Events

        #endregion

        #region Methods - Private

        #endregion

        #endregion
    }
}
