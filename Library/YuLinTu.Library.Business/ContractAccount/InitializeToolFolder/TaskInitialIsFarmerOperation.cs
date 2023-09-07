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
    /// 初始化是否基本农田任务类
    /// </summary>
    public class TaskInitialIsFarmerOperation : Task
    {
        #region Ctor

        /// <summary>
        /// 构造函数
        /// </summary>
        public TaskInitialIsFarmerOperation()
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
            TaskInitialIsFarmerArgument argument = Argument as TaskInitialIsFarmerArgument;
            if (argument == null)
            {
                return;
            }
            Zone currentZone = argument.CurrentZone;
            IDbContext dbContext = argument.DbContext;
            List<ContractLand> listGeoLand = argument.ListGeoLand;   //当前地域下的空间地块
            AccountLandBusiness landBusiness = new AccountLandBusiness(dbContext);
            landBusiness.Alert += this.ReportInfo;
            landBusiness.ProgressChanged += this.ReportPercent;
            //初始化是否基本农田
            landBusiness.ContractLandIsFarmerInitialTool(argument, listGeoLand, currentZone);
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
