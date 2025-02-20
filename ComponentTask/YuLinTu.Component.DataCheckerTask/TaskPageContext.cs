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
using Xceed.Wpf.Toolkit;
using YuLinTu.Windows;
using YuLinTu.Windows.Wpf;
using YuLinTu.Windows.Wpf.Metro;
using YuLinTu.Windows.Wpf.Metro.Components;
using System.Windows;
using YuLinTu.Appwork;
using YuLinTu.Appwork.Task;
using YuLinTu.Library.Entity;

namespace YuLinTu.Component.DataCheckerTask
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
                TypeTask = typeof(TaskDataChecker),
                TypeArgument = typeof(TaskDataCheckerArgument)
            });
        }

        protected override bool NeedHandleMessage()
        {
            return TheApp.Current.GetIsAuthenticated();
        }

        #endregion

        #region Methods - Private

        [MessageHandler(ID = EdTask.langViewTaskAlertDetails)]
        private void ViewTaskAlterDetails(object sender, ViewTaskAlertDetailsEventArgs e)
        {
            var meta = e.Alert.Metadata;
            if (meta == null)
                return;

            var senderView = meta as TaskAlterEditView<CollectivityTissue>;
            if (senderView != null)
            {
                e.Handled = true;
                senderView.Workpage = Workpage;
                senderView.AlterArgs = e.Alert;
                senderView.EditSenderAlterDetails();
            }

            var virtualPersonView = meta as TaskAlterEditView<VirtualPerson>;
            if (virtualPersonView != null)
            {
                e.Handled = true;
                virtualPersonView.Workpage = Workpage;
                virtualPersonView.AlterArgs = e.Alert;
                virtualPersonView.EditPersonAlterDetails();
            }

            var landView = meta as TaskAlterEditView<ContractLand>;
            if (landView != null)
            {
                e.Handled = true;
                landView.Workpage = Workpage;
                landView.AlterArgs = e.Alert;
                landView.EditContractLandAlterDetails();
            }

            var concordView = meta as TaskAlterEditView<ContractConcord>;
            if (concordView != null)
            {
                e.Handled = true;
                concordView.Workpage = Workpage;
                concordView.AlterArgs = e.Alert;
                concordView.EditContractConcordAlterDetails();
            }

            var regeditBookView = meta as TaskAlterEditView<ContractRegeditBook>;
            if (regeditBookView != null)
            {
                e.Handled = true;
                regeditBookView.Workpage = Workpage;
                regeditBookView.AlterArgs = e.Alert;
                regeditBookView.EditContractRegeditBookAlterDetails();
            }
        }

        #endregion

        #endregion
    }
}
