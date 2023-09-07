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
    /// 初始化界址点线数据操作任务类
    /// </summary>
    public class TaskInitializeLandCoilOperation : Task
    {
        #region Ctor

        /// <summary>
        /// 构造函数
        /// </summary>
        public TaskInitializeLandCoilOperation()
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
            TaskInitializeLandCoilArgument metadata = Argument as TaskInitializeLandCoilArgument;
            if (metadata == null)
            {
                return;
            }
            InitializeLandCoil importDot = new InitializeLandCoil();
            importDot.ProgressChanged += ReportPercent;
            importDot.Alert += ReportInfo;
            importDot.Argument = metadata;
            importDot.ContractLandInitialTool();
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
