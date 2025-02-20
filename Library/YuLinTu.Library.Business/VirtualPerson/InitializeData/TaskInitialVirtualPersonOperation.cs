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
    /// 初始化承包方基本信息任务类
    /// </summary>
    public class TaskInitialVirtualPersonOperation : Task
    {
        #region Ctor

        /// <summary>
        /// 构造函数
        /// </summary>
        public TaskInitialVirtualPersonOperation()
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
            TaskInitialVirtualPersonArgument argument = Argument as TaskInitialVirtualPersonArgument;
            if (argument == null)
            {
                return;
            }
            Zone currentZone = argument.CurrentZone;
            IDbContext dbContext = argument.Database;
            List<VirtualPerson> listVps = argument.ListPerson;   //当前地域下的承包方集合
            if (listVps.Count > 0 && !string.IsNullOrEmpty(listVps[0].OldVirtualCode))
            {
                listVps = listVps.OrderBy(o => o.OldVirtualCode).ToList();
            }
            VirtualPersonBusiness personBusiness = new VirtualPersonBusiness(dbContext);
            personBusiness.VirtualType = argument.VirtualType;
            personBusiness.Alert += this.ReportInfo;
            personBusiness.ProgressChanged += this.ReportPercent;
            //初始化承包方基本信息
            personBusiness.InitialVirtualPersonInfo(argument, listVps, currentZone);
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
