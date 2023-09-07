/*
 * (C) 2015  鱼鳞图公司版权所有,保留所有权利 
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
    /// <summary>
    /// 地域数据操作任务类
    /// </summary>
    public class TaskZoneOperation : Task
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

        #endregion

        #region Ctor

        public TaskZoneOperation()
        {
        }

        #endregion

        #region Override

        /// <summary>
        /// 开始操作
        /// </summary>
        protected override void OnGo()
        {
            TaskZoneArgument metadata = Argument as TaskZoneArgument;
            if (metadata == null)
            {
                return;
            }
            returnValue = null;
            IDbContext dbContext = metadata.Database;
            string fileName = metadata.FileName;
            bool isClear = metadata.IsClear;
            Zone zone = metadata.CurrentZone;
            ZoneDataBusiness business = new ZoneDataBusiness();
            business.DbContext = dbContext;
            business.Station = dbContext.CreateZoneWorkStation();
            business.Define = metadata.Define;
            business.Alert += ReportInfo;
            business.ProgressChanged += ReportPercent;
            switch (metadata.ArgType)
            {
                case eZoneArgType.ExportData:
                    returnValue = business.ExportZoneTable(zone, fileName);
                    break;
                case eZoneArgType.ExportShape:
                    returnValue = business.ExportZoneShape(zone, fileName);
                    break;
                case eZoneArgType.ExportPackage:
                    returnValue = business.ExportZonePackage(zone, fileName);
                    break;
                case eZoneArgType.ImportData:
                    returnValue = business.ImportZoneData(fileName, isClear);
                    break;
                case eZoneArgType.ImportShape:
                    returnValue = business.ImportZoneShape(fileName, isClear);
                    break;
                case eZoneArgType.UpServiceData:
                    ServiceSetDefine define = metadata.ServiceDefine;
                    if (define != null)
                    {
                        returnValue = business.UpdateToService(metadata.CurrentZone, define.UseSafeConfirm,
                            define.BusinessDataAddress, metadata.UserName, metadata.SessionCode);
                    }
                    break;
            }
            if (metadata.FilePath != null)
            {
                CanOpenResult = true;
            }
        }

        /// <summary>
        /// 打开文件
        /// </summary>
        public override void OpenResult()
        {
            TaskZoneArgument metadata = Argument as TaskZoneArgument;            
            System.Diagnostics.Process.Start(metadata.FilePath);
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
