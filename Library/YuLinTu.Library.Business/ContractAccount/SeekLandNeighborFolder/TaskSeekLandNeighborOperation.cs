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
    /// 查找四至操作任务类
    /// </summary>
    public class TaskSeekLandNeighborOperation : Task
    {
        #region Ctor

        /// <summary>
        /// 构造函数
        /// </summary>
        public TaskSeekLandNeighborOperation()
        {
        }
        #endregion

        #region Field

        #endregion

        #region Methods

        /// <summary>
        /// 开始执行子任务
        /// </summary>
        protected override void OnGo()
        {
            TaskSeekLandNeighborArgument metadata = Argument as TaskSeekLandNeighborArgument;
            if (metadata == null)
            {
                return;
            }
            SeekLandNeighborBus seeklnbb = new SeekLandNeighborBus();
            seeklnbb.ProgressChanged += ReportPercent;
            seeklnbb.Alert += ReportInfo;
            seeklnbb.Argument = metadata;
            seeklnbb.ContractLandInitialTool();
        }

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
