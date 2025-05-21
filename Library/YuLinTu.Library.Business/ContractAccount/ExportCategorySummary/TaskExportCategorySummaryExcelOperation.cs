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
using System.Collections;
using YuLinTu.Data.Dynamic;
using YuLinTu.Data.Shapefile;
using YuLinTu.Library.Office;

namespace YuLinTu.Library.Business
{
    /// <summary>
    /// 数据汇总表
    /// </summary>
    public class TaskExportCategorySummaryExcelOperation : Task
    {
        #region Fields
        //private PublicityConfirmDefine contractLandOutputSurveyDefine;
        //private FamilyOutputDefine familyOutputSet;
        //private FamilyOtherDefine familyOtherSet;
        //private SingleFamilySurveyDefine singleFamilySurveyDefine;
        //private bool returnValue;
        public string openFilePath;  //打开文件路径
        private SystemSetDefine SystemSetDefine = SystemSetDefine.GetIntence();
        #endregion

        #region Properties

        //public bool IsGroup

        /// <summary>
        /// 导出公示调查表日期设置
        /// </summary>
        public DateSetting PublishDateSetting { get; set; }

        /// <summary>
        /// 是否批量
        /// </summary>
        public bool IsBatch { get; set; }

        /// <summary>
        /// 当前被选中的承包方
        /// </summary>
        public List<VirtualPerson> SelectContractor { get; set; }

        /// <summary>
        /// 数据库服务上下文
        /// </summary>
        public IDbContext dbContext { get; set; }

        #endregion

        #region Ctor

        /// <summary>
        /// 构造函数
        /// </summary>
        public TaskExportCategorySummaryExcelOperation()
        {
        }

        #endregion

        #region Methods - Override

        /// <summary>
        /// 开始操作
        /// </summary>
        protected override void OnGo()
        {
            //returnValue = false;
            TaskExportCategorySummaryExcelArgument metadata = Argument as TaskExportCategorySummaryExcelArgument;
            if (metadata == null)
            {
                return;
            }
            if (metadata.Database == null)
            {
                this.ReportError("数据库获取失败");
                this.ReportProgress(100);
                return;
            }
            string Filename = metadata.FileName + @"\" + metadata.CurrentZone.Name + "-地块类别汇总表.xls";
            ExportCategorySummaryExcel export = new ExportCategorySummaryExcel();
            export.CurrentZone = metadata.CurrentZone;
            export.FilePath = Filename;
            export.DB = metadata.Database;
            export.UnitName = metadata.UnitName;
            export.PostProgressEvent += export_PostProgressEvent;
            export.PostErrorInfoEvent += export_PostErrorInfoEvent;
            export.BeginExcel(TemplateHelper.ExcelTemplate("地块类别汇总表"));
            export.PrintView(Filename);

            this.ReportProgress(100);
            openFilePath = Filename;
            CanOpenResult = true;

            GC.Collect();
        }


        /// <summary>
        /// 打开文件
        /// </summary>
        public override void OpenResult()
        {
            System.Diagnostics.Process.Start(openFilePath);
            base.OpenResult();
        }

        protected override void OnStopped()
        {
            base.OnStopped();
            GC.Collect();
        }

        protected override void OnTerminate(Exception ex)
        {
            base.OnTerminate(ex);
            GC.Collect();
        }

        #endregion


        #region  提示信息

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

        /// <summary>
        ///  错误信息报告
        /// </summary>
        private void export_PostErrorInfoEvent(string message)
        {
            if (!string.IsNullOrEmpty(message))
            {
                this.ReportError(message);
            }
        }

        /// <summary>
        /// 报告进度
        /// </summary>
        private void export_PostProgressEvent(int progress, object userState)
        {
            this.ReportProgress(progress, userState);
        }

        #endregion

    }
}
