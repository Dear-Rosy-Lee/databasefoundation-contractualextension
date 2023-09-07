/*
 * (C) 2015  鱼鳞图公司版权所有,保留所有权利 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YuLinTu.Data;
using YuLinTu.Library.Entity;

namespace YuLinTu.Library.Business
{
    /// <summary>
    /// 签订合同任务处理类
    /// </summary>
    public class TaskConcordInitializeOperation : Task
    {
        #region Properties

        /// <summary>
        /// 数据库
        /// </summary>
        public IDbContext DbContext { get; set; }

        #endregion

        #region Override

        /// <summary>
        /// 开始操作
        /// </summary>
        protected override void OnGo()
        {
            TaskConcordInitializeArgument metadata = Argument as TaskConcordInitializeArgument;
            if (metadata == null)
            {
                return;
            }
            DbContext = metadata.Database;
            Zone zone = metadata.CurrentZone;
            ConcordBusiness business = new ConcordBusiness(DbContext);
            business.ConcordsModified = metadata.ConcordsModified;
            business.LandsOfInitialConcord = metadata.LandsOfInitialConcord;          
            business.Alert += ReportInfo;
            business.ProgressChanged += ReportPercent;
            business.InitialConcordData(zone, metadata.Sender, metadata.IsCalculateArea);
        }

        #endregion

        #region  提示信息

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

        #endregion
    }
}
