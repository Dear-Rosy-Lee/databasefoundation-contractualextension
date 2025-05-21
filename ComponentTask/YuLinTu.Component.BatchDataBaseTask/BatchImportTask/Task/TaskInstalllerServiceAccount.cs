/*
 * (C) 2024  鱼鳞图公司版权所有,保留所有权利
 * http://www.yulintu.com
*/
using System;
using System.Configuration;
using Quality.Business.TaskBasic;

namespace YuLinTu.Component.BatchDataBaseTask
{
    /// <summary>
    /// 农业部数据交换任务导入
    /// </summary>
    [TaskDescriptor(Name = "批量导入地块Shape数据", Gallery = "矢量数据处理",
        UriImage16 = "pack://application:,,,/YuLinTu.Resources;component/Images/16/table-import.png",
        UriImage24 = "pack://application:,,,/YuLinTu.Resources;component/Images/24/table-import.png")]
    public class TaskInstalllerServiceAccount : YuLinTu.Task
    {
        #region Ctor

        static TaskInstalllerServiceAccount()
        {
        }

        public TaskInstalllerServiceAccount()
        {
            Name = "导入数据任务";
            Description = "批量导入数据";
        }

        #endregion

        #region Methods

        #region Methods - Property Changed

        #endregion

        #region Methods - Override

        /// <summary>
        /// 执行任务
        /// </summary>
        protected override void OnGo()
        {
            TaskInstalllerServiceAccountArgument argument = this.Argument as TaskInstalllerServiceAccountArgument;
            try
            {
                string volumeValue = ConfigurationManager.AppSettings.TryGetValue("VolumeValue", "2.0").ToString();
                string IsStandCode = ConfigurationManager.AppSettings.TryGetValue("IsStandCode", true).ToString();
                DataImportProgress dep = new DataImportProgress();
                dep.Alert += Report_Alert;
                dep.ProgressChanged += Report_ProgressChanged;
                dep.FilePath = argument.ImportFilePath;
                dep.ID = this.ID;
                dep.ImportData();
            }
            catch (Exception ex)
            {
                this.ReportError("导入数据发生错误:" + ex.Message);
                LogWrite.WriteErrorLog("导入数据发生错误:" + ex.Message + ex.StackTrace);
                return;
            }
            finally
            {
                GC.Collect();
            }
        }

        protected override void OnValidate()
        {
            this.ReportProgress(0);
            TaskInstalllerServiceAccountArgument argument = this.Argument as TaskInstalllerServiceAccountArgument;
        }

        #endregion

        #region Methods - Register

        #endregion

        #region Methods - Validate

        /// <summary>
        /// 进度提示
        /// </summary>
        private void Report_ProgressChanged(object sender, TaskProgressChangedEventArgs e)
        {
            this.ReportProgress(e.Percent, e.UserState);
        }

        /// <summary>
        /// 消息提示
        /// </summary>
        private void Report_Alert(object sender, TaskAlertEventArgs e)
        {
            if (e.Grade == eMessageGrade.Error)
            {
                this.ReportError(e.Description, "导入数据信息");
            }
            if (e.Grade == eMessageGrade.Infomation)
            {
                this.ReportInfomation(e.Description, "导入数据信息");
            }
            if (e.Grade == eMessageGrade.Warn)
            {
                this.ReportWarn(e.Description, "导入数据信息");
            }
            if (e.Grade == eMessageGrade.Exception)
            {
                this.ReportAlert(e);
            }
        }

        /// <summary>
        /// 消息提示
        /// </summary>
        private void QualityInofAlert(TaskAlertEventArgs e)
        {
            this.ReportAlert(e);
        }

        #endregion

        #endregion
    }
}
