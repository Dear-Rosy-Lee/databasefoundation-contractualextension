/*
 * (C) 2025  鱼鳞图公司版权所有,保留所有权利 
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
    /// 初始化地块基本信息任务类
    /// </summary>
    public class TaskInitialLandInfoOperation : Task
    {
        #region Ctor

        /// <summary>
        /// 构造函数
        /// </summary>
        public TaskInitialLandInfoOperation()
        { }

        #endregion

        #region Field

        #endregion

        #region Property

        #endregion

        #region Method—Override

        /// <summary>
        /// 开始执行任务
        /// </summary>
        protected override void OnGo()
        {
            var argument = Argument as TaskInitialLandInfoArgument;
            if (argument == null)
            {
                return;
            }
            Zone currentZone = argument.CurrentZone;
            IDbContext dbContext = argument.Database;
            List<ContractLand> listLand = argument.CurrentZoneLandList;   //当前地域下的地块
            if(listLand==null || listLand.Count<=0)
            {
                this.ReportWarn(argument.CurrentZone.FullName+"下无承包地块");
                this.ReportProgress(100);
                return;
            }
            AccountLandBusiness landBusiness = new AccountLandBusiness(dbContext);
            landBusiness.Alert += this.ReportInfo;
            landBusiness.ProgressChanged += this.ReportPercent;
            //初始化地块基本信息
            landBusiness.ContractLandInitialTool(argument, listLand, currentZone);
        }

        #endregion

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

        #endregion
    }
}
