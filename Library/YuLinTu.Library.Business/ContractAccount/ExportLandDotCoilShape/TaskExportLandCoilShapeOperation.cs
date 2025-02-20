/*
 * (C) 2025  鱼鳞图公司版权所有,保留所有权利 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using YuLinTu.Library.Entity;
using YuLinTu;
using YuLinTu.Data;
using System.Diagnostics;

namespace YuLinTu.Library.Business
{
    public class TaskExportLandCoilShapeOperation : Task
    {
        #region Fields

        private object returnValue;

        #endregion

        #region Property

        /// <summary>
        /// 返回值
        /// </summary>
        public object ReturnValue
        {
            get { return returnValue; }
        }

        private string openFilePath;  //打开文件路径

        #endregion

        #region Ctor

        public TaskExportLandCoilShapeOperation()
        {
        }

        #endregion

        #region Override

        /// <summary>
        /// 开始操作
        /// </summary>
        protected override void OnGo()
        {
            TaskExportLandCoilShapeArgument metadata = Argument as TaskExportLandCoilShapeArgument;
            if (metadata == null)
            {
                return;
            }
            returnValue = null;
            IDbContext dbContext = metadata.Database;
            string fileName = metadata.FileName;
            openFilePath = fileName;
            Zone zone = metadata.CurrentZone;
            ExportLandCoilShapeBusiness business = new ExportLandCoilShapeBusiness();
            business.Argument = metadata;
            business.DbContext = dbContext;
            business.Station = dbContext.CreateZoneWorkStation();
            business.Alert += ReportInfo;
            business.ProgressChanged += ReportPercent;
            returnValue = business.ExportZoneShape(zone, fileName);
            if ((bool)returnValue)
                CanOpenResult = true;
        }

        /// <summary>
        /// 打开文件
        /// </summary>
        public override void OpenResult()
        {
            System.Diagnostics.Process.Start(openFilePath);
            base.OpenResult();
        }

        #endregion

        #region 辅助方法

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
