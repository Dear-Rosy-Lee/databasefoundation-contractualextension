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


namespace YuLinTu.Component.XiZangLZ
{
    public class TaskExportDJSPBOperation : Task
    {
        /// <summary>
        /// 开始执行组任务
        /// </summary>
        protected override void OnGo()
        {
            TaskExportDJSPBArgument metadata = Argument as TaskExportDJSPBArgument;
            if (metadata == null)
            {
                return;
            }                  
            ExportDJSPBBusiness business = new ExportDJSPBBusiness();        
            business.Argument = metadata;       
            business.Alert += ReportInfo;          
            business.ProgressChanged += ReportPercent;          
            business.SingleExportRequireBook(metadata.CurrentZone, metadata.ALLLands);
            this.ReportInfomation(string.Format("导出{0}登记审批表", metadata.CurrentZone.Name));
             
            this.ReportProgress(100);

        }


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
