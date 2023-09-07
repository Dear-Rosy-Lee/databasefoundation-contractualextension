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
using YuLinTu.Library.WorkStation;
using YuLinTu.Windows;

namespace YuLinTu.Library.Business
{
    /// <summary>
    /// 导出发包方Word任务类
    /// </summary>
    public class TaskExportSenderWordOperation : Task
    {
        #region Ctor

        /// <summary>
        /// 构造函数
        /// </summary>
        public TaskExportSenderWordOperation()
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
            TaskExportSenderWordArgument argument = Argument as TaskExportSenderWordArgument;
            if (argument == null)
            {
                return;
            }
            IDbContext dbContext = argument.DbContext;
            var zone = argument.CurrentZone;
            try
            {
                var senderStation = dbContext.CreateSenderWorkStation();
                var listTissue = senderStation.GetTissues(zone.FullCode);
                if (listTissue == null || listTissue.Count == 0)
                {
                    this.ReportProgress(100, null);
                    this.ReportWarn(string.Format("{0}未获取发包方!", zone.FullName));
                    return;
                }
                bool canOpen = ExportSenderWord(argument, listTissue);
                if (canOpen)
                    CanOpenResult = true;
            }
            catch (Exception ex)
            {
                YuLinTu.Library.Log.Log.WriteException(this, "TaskExportSenderWordOperation(导出发包方数据任务)", ex.Message + ex.StackTrace);
                this.ReportException(ex, "导出发包方Word表出现异常!");
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
        /// 导出数据到发包方Word
        /// </summary>
        public bool ExportSenderWord(TaskExportSenderWordArgument argument, List<CollectivityTissue> listTissue)
        {
            bool result = false;
            try
            {
                if (argument.CurrentZone == null)
                {
                    this.ReportError("未选择导出数据的地域!");
                    return result;
                }
                this.ReportProgress(1, "开始");
                string markDesc = GetMarkDesc(argument.CurrentZone, argument.DbContext);
                string tempPath = TemplateHelper.WordTemplate(TemplateFile.SenderSurveyWord);
                int index = 1;
                double vpPercent = 99 / (double)listTissue.Count;
                openFilePath = argument.FileName;
                foreach (CollectivityTissue tissue in listTissue)
                {
                    ExportSenderWord senderTable = new ExportSenderWord();

                    #region 通过反射等机制定制化具体的业务处理类
                    var temp = WorksheetConfigHelper.GetInstance(senderTable);
                    if (temp != null && temp.TemplatePath != null)
                    {
                        if (temp is ExportSenderWord)
                        {
                            senderTable = (ExportSenderWord)temp;
                        }
                        tempPath = Path.Combine(TheApp.GetApplicationPath(), temp.TemplatePath);
                    }
                    #endregion

                    senderTable.OpenTemplate(tempPath);
                    senderTable.SaveAs(tissue, openFilePath + @"\" + tissue.Name + "(" + tissue.Code + ")" + TemplateFile.SenderSurveyWord);
                    this.ReportProgress((int)(1 + vpPercent * index), tissue.Name);
                    index++;
                }
                result = true;
                string info = string.Format("{0}导出{1}张发包方Word表", markDesc, index - 1);
                this.ReportInfomation(info);
                this.ReportProgress(100, "完成");
            }
            catch (Exception ex)
            {
                result = false;
                this.ReportError("导出发包方Word失败");
                YuLinTu.Library.Log.Log.WriteException(this, "ExportSenderWord(导出发包方数据到Word表)", ex.Message + ex.StackTrace);
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

        #endregion
    }
}
