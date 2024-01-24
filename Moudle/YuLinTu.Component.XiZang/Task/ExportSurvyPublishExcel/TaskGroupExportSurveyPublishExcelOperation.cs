/*
 * (C) 2015  鱼鳞图公司版权所有,保留所有权利 
 */
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using YuLinTu.Data;
using YuLinTu.Library.Business;
using YuLinTu.Library.Entity;
using YuLinTu.Windows;
using YuLinTu.Windows.Wpf.Metro.Components;

namespace YuLinTu.Component.XiZangLZ
{
    /// <summary>
    /// 批量导出调查信息公示表任务类
    /// </summary>
    public class TaskGroupExportSurveyPublishExcelOperation : TaskGroup
    {
        #region Ctor

        /// <summary>
        /// 构造函数
        /// </summary>
        public TaskGroupExportSurveyPublishExcelOperation()
        { }

        #endregion

        #region Fields

        private string fileName; //保存文件路径

        #endregion

        #region Properties

        /// <summary>
        /// 工作页
        /// </summary>
        public IWorkpage Workpage { get; set; }

        #endregion

        #region Method—Override

        /// <summary>
        /// 开始执行任务
        /// </summary>
        protected override void OnGo()
        {
            Clear();
            TaskGroupExportSurveyPublishExcelArgument groupArgument = Argument as TaskGroupExportSurveyPublishExcelArgument;
            if (groupArgument == null)
            {
                return;
            }
            fileName = groupArgument.FileName;
            IDbContext dbContext = groupArgument.DbContext;
            Zone currentZone = groupArgument.CurrentZone;
            List<Zone> selfAndSubsZones = new List<Zone>();
            List<Zone> allZones = new List<Zone>();
            try
            {
                var zoneStation = dbContext.CreateZoneWorkStation();
                selfAndSubsZones = zoneStation.GetChildren(currentZone.FullCode, eLevelOption.SelfAndSubs);
                allZones = zoneStation.GetAllZones(currentZone);
            }
            catch (Exception ex)
            {
                YuLinTu.Library.Log.Log.WriteException(this, "GetChildren(获取地域数据失败)", ex.Message + ex.StackTrace);
                //ShowBox("批量导入承包方调查表", string.Format("获取地域数据失败"));
                this.ReportError("获取子级地域数据失败!");
                return;
            }
            foreach (var zone in selfAndSubsZones)
            {
                string savePath = CreateDirectoryHelper.CreateDirectoryByVilliage(allZones, zone);
                TaskExportSurveyPublishExcelArgument argument = new TaskExportSurveyPublishExcelArgument();
                argument.CurrentZone = zone;
                argument.DbContext = dbContext;
                argument.FileName = fileName + @"\" + savePath;
                argument.VirtualType = groupArgument.VirtualType;
                argument.SettingDefine = groupArgument.SettingDefine;
                argument.SystemSet = groupArgument.SystemSet;
                argument.PublishDateSetting = groupArgument.PublishDateSetting;
                TaskExportSurveyPublishExcelOperation operation = new TaskExportSurveyPublishExcelOperation();
                operation.Argument = argument;
                operation.Name = "批量导出调查信息公示表";
                operation.Description = zone.FullName;
                Add(operation);
            }
            CanOpenResult = true;
            base.OnGo();
        }

        /// <summary>
        /// 打开
        /// </summary>
        public override void OpenResult()
        {
            System.Diagnostics.Process.Start(fileName);
        }

        protected override void OnStopped()
        {
            base.OnStopped();
        }

        protected override void OnTerminate(Exception ex)
        {
            base.OnTerminate(ex);
        }

        #endregion

        #region Method - 辅助

        /// <summary>
        /// 消息显示框
        /// </summary>
        private void ShowBox(string header, string msg, eMessageGrade type = eMessageGrade.Error)
        {
            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                Workpage.Page.ShowMessageBox(new TabMessageBoxDialog
                {
                    Header = header,
                    Message = msg,
                    MessageGrade = type,
                    CancelButtonText = "取消",
                });
            })); ;
        }

        #endregion
    }
}
