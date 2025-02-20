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
using System.IO;

namespace YuLinTu.Library.Business
{
    /// <summary>
    /// 二轮台账数据操作任务类
    /// </summary>
    public class TaskSecondTableOperation : Task
    {
        #region Fields

        private object returnValue; 
        private SecondTableExportDefine secondTableDefine;

        #endregion

        #region Property

        /// <summary>
        /// 二轮台账配置实体属性
        /// </summary>
        public SecondTableExportDefine SecondTableDefine
        {
            get { return secondTableDefine; }
            set { secondTableDefine = value; }
        }

        /// <summary>
        /// 返回值
        /// </summary>
        public object ReturnValue
        {
            get { return returnValue; }
        }

        #endregion

        #region Ctor

        public TaskSecondTableOperation()
        {
        }

        #endregion

        #region Override

        /// <summary>
        /// 开始操作
        /// </summary>
        protected override void OnGo()
        {
            TaskSecondTableArgument metadata = Argument as TaskSecondTableArgument;
            if (metadata == null)
            {
                return;
            }
            returnValue = null;
            IDbContext dbContext = metadata.Database;
            string fileName = metadata.FileName;
            bool isClear = metadata.IsClear;
            Zone zone = metadata.CurrentZone;
            SecondTableLandBusiness business = new SecondTableLandBusiness(dbContext);      
            business.SecondTableDefine = SecondTableDefine;
            business.Alert += ReportInfo;
            business.ProgressChanged += ReportPercent;
            if (metadata == null)
            {
                return;
            }
            switch (metadata.ArgType)
            {
                case eSecondTableArgType.ImportData:
                    business.ImportData(zone, fileName, isClear);
                    returnValue = zone.FullCode;
                    break;
                case eSecondTableArgType.RealQueryExcel:
                    business.ExportSecondSurveyTable(zone, fileName);
                    break;
                case eSecondTableArgType.PublicityExcel:
                    business.ExportSurveyPublicTable(zone, fileName);
                    break;

                case eSecondTableArgType.IdentifyExcel:
                    business.ExportSecondLandSignTable(zone, fileName);
                    break;
                case eSecondTableArgType.UserIdentify:
                    business.ExportSecondUserSignTable(zone, fileName, metadata.DateValue);
                    break;
                case eSecondTableArgType.ExportBoundarySettleExcel:
                    business.ExportBoundarySettleTable(zone, fileName);   //导出勘界调查表
                    break;
                case eSecondTableArgType.ExportSingleFamilyExcel:
                    business.ExportSingleFamilyTable(zone, fileName);
                    break;
            }
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
