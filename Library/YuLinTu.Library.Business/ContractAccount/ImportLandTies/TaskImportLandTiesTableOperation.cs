using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YuLinTu.Data;
using YuLinTu.Library.Entity;

namespace YuLinTu.Library.Business
{
    public class TaskImportLandTiesTableOperation : Task
    {
        #region Ctor

        public TaskImportLandTiesTableOperation()
        {
        }

        #endregion Ctor

        #region Method—Override

        /// <summary>
        /// 开始执行任务
        /// </summary>
        protected override void OnGo()
        {
            TaskImportLandTiesTableArgument argument = Argument as TaskImportLandTiesTableArgument;
            if (argument == null)
            {
                return;
            }
            string fileName = argument.FileName;
            if (string.IsNullOrEmpty(fileName))
            {
                this.ReportWarn("没有找到" + argument.CurrentZone.Name + "对应的调查表");
                this.ReportProgress(100);
                return;
            }
            Zone currentZone = argument.CurrentZone;
            IDbContext dbContext = argument.DbContext;
            AccountLandBusiness landBusiness = new AccountLandBusiness(dbContext);
            landBusiness.Alert += this.ReportInfo;
            landBusiness.ProgressChanged += this.ReportPercent;
            //导入地块调查表
            bool flag = landBusiness.ImportLandTies(currentZone, fileName);
            this.ReportProgress(100, null);
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