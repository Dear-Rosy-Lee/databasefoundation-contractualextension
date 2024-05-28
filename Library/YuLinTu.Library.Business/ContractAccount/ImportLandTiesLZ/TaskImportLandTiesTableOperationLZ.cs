using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YuLinTu.Data;
using YuLinTu.Library.Entity;

namespace YuLinTu.Library.Business
{
    public class TaskImportLandTiesTableOperationLZ : Task
    {
        #region Ctor

        public TaskImportLandTiesTableOperationLZ()
        {
        }

        #endregion Ctor

        #region Method—Override

        /// <summary>
        /// 开始执行任务
        /// </summary>
        protected override void OnGo()
        {
            TaskImportLandTiesTableArgumentLZ argument = Argument as TaskImportLandTiesTableArgumentLZ;
            if (argument == null)
            {
                return;
            }
            string fileName = argument.FileName;
            if (string.IsNullOrEmpty(fileName))
            {
                this.ReportWarn("没有找到" + argument.CurrentZone.Name + "对应的文件夹");
                this.ReportProgress(100);
                return;
            }
            var files = Directory.GetFiles(fileName, "*.xls", SearchOption.AllDirectories).Where(x => x.EndsWith("农户与宗地台账变更后.xls", StringComparison.OrdinalIgnoreCase)).ToList();

            Zone currentZone = argument.CurrentZone;
            IDbContext dbContext = argument.DbContext;
            AccountLandBusiness landBusiness = new AccountLandBusiness(dbContext);
            landBusiness.Alert += this.ReportInfo;
            landBusiness.ProgressChanged += this.ReportPercent;
            //导入地块调查表
            bool flag = landBusiness.ImportLandTiesLZ(currentZone, files);
            var error = landBusiness.ErrorInformation;
            string errInformation = "";
            if (error != null)
            {
                errInformation = string.Join(",", error);
            }
            if (flag == false)
            {
                this.ReportProgress(0, "失败");
                this.ReportError(errInformation);
            }
            else
            {
                this.ReportProgress(100, "完成");
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