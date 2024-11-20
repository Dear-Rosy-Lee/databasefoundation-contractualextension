using YuLinTu.Data;
using YuLinTu.Library.Entity;

namespace YuLinTu.Library.Business
{
    public class TaskImportRelationOperation : Task
    {
        #region Ctor

        public TaskImportRelationOperation()
        {
        }

        #endregion Ctor

        #region Method—Override

        /// <summary>
        /// 开始执行任务
        /// </summary>
        protected override void OnGo()
        {
            var argument = Argument as TaskImportLandTiesTableArgument;
            if (argument == null)
            {
                return;
            }
            string fileName = argument.FileName;
            if (string.IsNullOrEmpty(fileName))
            {
                this.ReportWarn("选择文件不存在！");
                this.ReportProgress(100);
                return;
            }
            Zone currentZone = argument.CurrentZone;
            IDbContext dbContext = argument.DbContext;
            var landBusiness = new ContractRegeditBookBusiness(dbContext);
            landBusiness.Alert += this.ReportInfo;
            landBusiness.ProgressChanged += this.ReportPercent;
            bool flag = landBusiness.ImportLandTies(currentZone, fileName, argument.ImportType);
            var error = landBusiness.ErrorInformation;
            if (!flag)
            {
                this.ReportError(string.Join(",", error));
            }
            else
            {
                this.ReportProgress(100, null);
            }
        }

        #endregion Method—Override

        #region Method—Helper

        /// <summary>
        /// 错误信息报告
        /// </summary>
        /// <param name="message"></param>
        private void ReportInfo(object sender, TaskAlertEventArgs e)
        {
            if (e != null)
            {
                this.ReportAlert(e.Grade, e.UserState, e.Description);
            }
        }

        /// <summary>
        /// 进度报告
        /// </summary>
        /// <param name="progress"></param>
        private void ReportPercent(object sender, TaskProgressChangedEventArgs e)
        {
            if (e != null)
            {
                this.ReportProgress(e.Percent, e.UserState);
            }
        }

        #endregion Method—Helper
    }
}