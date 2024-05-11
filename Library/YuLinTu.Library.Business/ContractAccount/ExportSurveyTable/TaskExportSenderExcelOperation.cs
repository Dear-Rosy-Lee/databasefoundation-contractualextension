/*
 * (C) 2015  鱼鳞图公司版权所有,保留所有权利 
 */
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using YuLinTu.Data;
using YuLinTu.Library.Entity;

namespace YuLinTu.Library.Business
{
    /// <summary>
    /// 导出发包方excel调查表任务类
    /// </summary>
    public class TaskExportSenderExcelOperation : Task
    {
        #region Ctor

        /// <summary>
        /// 构造函数
        /// </summary>
        public TaskExportSenderExcelOperation()
        { }

        #endregion

        #region Field

        private string openFilePath;  //打开文件路径

        #endregion

        #region Property

        #endregion

        #region Method—Override

        /// <summary>
        /// 开始执行任务
        /// </summary>
        protected override void OnGo()
        {
            TaskExportSenderExcelArgument argument = Argument as TaskExportSenderExcelArgument;
            if (argument == null)
            {
                return;
            }
            openFilePath = argument.FileName;
            try
            {
                bool canOpen = ExportSenderExcel(argument);
                if (canOpen)
                    CanOpenResult = true;
            }
            catch (Exception ex)
            {
                YuLinTu.Library.Log.Log.WriteException(this, "TaskExportSenderExcelOperation(导出发包方调查表任务)", ex.Message + ex.StackTrace);
                this.ReportException(ex, "导出发包方调查表出现异常!");
            }
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

        #region Method—ExportBusiness

        /// <summary>
        /// 导出发包方调查表（Excel）
        /// </summary>
        public bool ExportSenderExcel(TaskExportSenderExcelArgument argument)
        {
            bool result = false;
            try
            {
                if (argument.CurrentZone == null)
                {
                    this.ReportError("未选择导出数据的地域!");
                    return result;
                }
                this.ReportProgress(1, "正在获取发包方数据......");
                string messageName = SenderMessage.SENDER_GETCHILDRENDATA;
                ModuleMsgArgs args = new ModuleMsgArgs();
                args.Name = messageName;
                args.Parameter = argument.CurrentZone.FullCode;
                args.Datasource = DataBaseSource.GetDataBaseSource();
                TheBns.Current.Message.Send(this, args);
                string excelName = GetMarkDesc(argument.CurrentZone, argument.DbContext);
                List<CollectivityTissue> list = args.ReturnValue as List<CollectivityTissue>;
                this.ReportProgress(10, "发包方数据获取完成");
                if (list.Count > 0)
                {
                    string tempName = TemplateHelper.ExcelTemplate(TemplateFile.SenderSurveyExcel);
                    this.ReportProgress(15, string.Format("导出{0}", excelName));
                    var diretory = argument.FileName;
                    if (!Directory.Exists(diretory))
                    {
                        Directory.CreateDirectory(diretory);
                    }
                    using (ExportSenderSurveyTable export = new ExportSenderSurveyTable())
                    {
                        export.TissueCollection = list;
                        export.ShowValue = false;
                        export.SaveFileName = Path.Combine(diretory, excelName + "发包方调查表");
                        export.BeginToZone(tempName);
                    }
                    result = true;
                    this.ReportProgress(100);
                    this.ReportInfomation(string.Format("{0}成功导出{1}条发包方数据", excelName, list.Count));
                }
                else
                {
                    this.ReportWarn("未获取到发包方数据!");
                }
            }
            catch (Exception ex)
            {
                result = false;
                this.ReportError("导出发包方调查表失败!");
                YuLinTu.Library.Log.Log.WriteException(this, "ExportSenderExcel(导出发包方调查表)", ex.Message + ex.StackTrace);
            }
            return result;
        }

        #endregion

        #region Method—Private

        /// <summary>
        ///  获取上级地域
        /// </summary>
        /// <param name="zone"></param>
        /// <param name="dbContext"></param>
        /// <returns></returns>
        private Zone GetParent(Zone zone, IDbContext dbContext)
        {
            Zone parentZone = null;
            if (zone == null || dbContext == null)
                return parentZone;
            try
            {
                var zoneStation = dbContext.CreateZoneWorkStation();
                parentZone = zoneStation.Get(zone.UpLevelCode);
            }
            catch (Exception ex)
            {
                YuLinTu.Library.Log.Log.WriteException(this, "GetParent(获取父级地域失败!)", ex.Message + ex.StackTrace);
            }
            return parentZone;
        }

        /// <summary>
        /// 根据当前地域获得任务描述信息
        /// </summary>
        private string GetMarkDesc(Zone zone, IDbContext dbContext)
        {
            string excelName = string.Empty;
            if (zone == null || dbContext == null)
                return excelName;
            Zone parent = GetParent(zone, dbContext);  //获取上级地域
            string parentName = parent == null ? "" : parent.Name;
            if (zone.Level == eZoneLevel.County)
            {
                excelName = zone.Name;
            }
            else if (zone.Level == eZoneLevel.Town)
            {
                excelName = zone.Name;
            }
            else if (zone.Level == eZoneLevel.Village)
            {
                excelName = parentName + zone.Name;
            }
            else if (zone.Level == eZoneLevel.Group)
            {
                Zone parentTowm = GetParent(parent, dbContext);
                string parentTownName = parentTowm == null ? "" : parentTowm.Name;
                excelName = parentTownName + parentName + zone.Name;
            }
            return excelName;
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
        private void ReportPercent(object sender, TaskProgressChangedEventArgs e)
        {
            if (e != null)
            {
                this.ReportProgress(e.Percent, e.UserState);
            }
        }

        /// <summary>
        ///  进度报告
        /// </summary>
        private void export_PostProgressEvent(int progress, object userState)
        {
            this.ReportProgress(progress, userState);
        }

        /// <summary>
        /// 信息报告
        /// </summary>
        private void export_PostErrorEvent(string error)
        {
            this.ReportError(error);
        }

        #endregion
    }
}
