/*
 * (C) 2014  鱼鳞图公司版权所有,保留所有权利
 * http://www.yulintu.com
*/
using Quality.Business.TaskBasic;
using System;
using System.Configuration;
using System.IO;

namespace YuLinTu.Component.ResultDbof2016ToLocalDb
{
    /// <summary>
    /// 2016年新规范农业部数据交换任务导入
    /// </summary>
    [TaskDescriptor(Name = "导入2016数据任务", Gallery = "导入数据库成果",
        UriImage16 = "pack://application:,,,/YuLinTu.Component.ResultDbof2016ToLocalDb;component/Resources/import16.png",
        UriImage24 = "pack://application:,,,/YuLinTu.Component.ResultDbof2016ToLocalDb;component/Resources/import24.png")]
    public class TaskInstalllerServiceAccount : YuLinTu.Task
    {
        #region Ctor

        static TaskInstalllerServiceAccount()
        {
        }

        public TaskInstalllerServiceAccount()
        {
            Name = "导入2016数据任务";
            Description = "导入符合农业部要求的2016年格式的数据";
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
            var argument = this.Argument as TaskInstalllerServiceAccountArgument;
            bool checkResult = CheckArgument(argument);
            if (!checkResult)
            {
                return;
            }
            try
            {
                string volumeValue = ConfigurationManager.AppSettings.TryGetValue("VolumeValue", "2.0").ToString();
                string IsStandCode = ConfigurationManager.AppSettings.TryGetValue("IsStandCode", true).ToString();
                var dep = new DataImportProgress();
                dep.FileZone = argument.FileZone;
                dep.Alert += Report_Alert;
                dep.GenerateCoilDot = argument.GenerateCoilDot;
                dep.ProgressChanged += Report_ProgressChanged;
                dep.QualityAlert += QualityInofAlert;
                dep.FilePath = argument.ImportFilePath;
                dep.LocalService = Library.Business.DataBaseSource.GetDataBaseSource();
                dep.ID = this.ID;
                dep.NeedCheck = false;
                dep.VolumeValue = volumeValue;
                dep.IsStandCode = IsStandCode == "True" ? true : false;
                dep.CreatUnit = argument.CreatUnit;
                dep.ImportData();
            }
            catch (Exception ex)
            {
                LogWrite.WriteErrorLog("请检查导入的数据是否为新库,导入数据发生错误:" + ex.ToString());
                this.ReportError("导入数据发生错误:" + ex.ToString());
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
            CheckArgument(argument);
        }

        #endregion

        #region Methods - Register

        #endregion

        #region Methods - Validate

        /// <summary>
        /// 检查参数
        /// </summary>
        /// <param name="argument"></param>
        private bool CheckArgument(TaskInstalllerServiceAccountArgument argument)
        {
            bool checkResult = true;
            if (!Directory.Exists(argument.ImportFilePath) || !Directory.Exists(argument.ImportFilePath))
            {
                this.ReportError("导入数据的文件路径不正确!");
                checkResult = false;
            }

            return checkResult;
        }

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
