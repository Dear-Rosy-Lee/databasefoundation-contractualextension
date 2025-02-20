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
    /// 导入承包方调查表任务类
    /// </summary>
    public class TaskImportVirtualPersonTableOperation : Task
    {
        #region Ctor

        /// <summary>
        /// 构造函数
        /// </summary>
        public TaskImportVirtualPersonTableOperation()
        { }

        #endregion

        #region Field

        #endregion

        #region Property

        /// <summary>
        /// 是否是确权确股插件导入数据，供确权确股插件使用，不要删除该属性
        /// </summary>
        public bool IsStockRight { get; set; }

        #endregion

        #region Method—Override

        /// <summary>
        /// 开始执行任务
        /// </summary>
        protected override void OnGo()
        {
            TaskImportVirtualPersonTableArgument argument = Argument as TaskImportVirtualPersonTableArgument;
            if (argument == null)
            {
                return;
            }
            string fileName = argument.FileName;
            Zone currentZone = argument.CurrentZone;
            IDbContext dbContext = argument.DbContext;
            VirtualPersonBusiness business = new VirtualPersonBusiness(dbContext);
            business.Alert += this.ReportInfo;
            business.ProgressChanged += this.ReportPercent;
            business.VirtualType = argument.VirtualType;
            business.FamilyImportSet = argument.FamilyImportSet;
            business.FamilyOtherSet = argument.FamilyOtherSet;
            //导入承包方调查表
            business.ImportData(currentZone, fileName, IsStockRight);
            this.ReportPercent(100, null);
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
