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
using YuLinTu.Library.Entity;
using YuLinTu.Library.WorkStation;
using YuLinTu.Windows;
using YuLinTu.Windows.Wpf.Metro.Components;

namespace YuLinTu.Library.Business
{
    /// <summary>
    /// 批量导入地块调查表任务类
    /// </summary>
    public class TaskGroupImportVirtualPersonTableOperation : TaskGroup
    {
        #region Ctor

        /// <summary>
        /// 构造函数
        /// </summary>
        public TaskGroupImportVirtualPersonTableOperation()
        { }

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
            TaskGroupImportVirtualPersonTableArgument groupArgument = Argument as TaskGroupImportVirtualPersonTableArgument;
            if (groupArgument == null)
            {
                return;
            }
            string fileName = groupArgument.FileName;
            IDbContext dbContext = groupArgument.DbContext;
            Zone currentZone = groupArgument.CurrentZone;
            List<Zone> allZones = new List<Zone>();
            IZoneWorkStation zoneStation;
            try
            {
                zoneStation = dbContext.CreateZoneWorkStation();
                allZones = zoneStation.GetChildren(currentZone.FullCode, eLevelOption.SelfAndSubs);
            }
            catch (Exception ex)
            {
                YuLinTu.Library.Log.Log.WriteException(this, "GetChildren(获取地域数据失败)", ex.Message + ex.StackTrace);
                ShowBox("批量导入承包方调查表", string.Format("获取地域数据失败"));
                return;
            }
            foreach (var zone in allZones)
            {
                TaskImportVirtualPersonTableArgument argument = new TaskImportVirtualPersonTableArgument();
                argument.CurrentZone = zone;
                argument.DbContext = dbContext;
                argument.FileName = GetMatchFilePath(fileName, zone, zoneStation);
                argument.VirtualType = groupArgument.VirtualType;
                argument.FamilyImportSet = groupArgument.FamilyImportSet;
                argument.FamilyOtherSet = groupArgument.FamilyOtherSet;
                TaskImportVirtualPersonTableOperation operation = new TaskImportVirtualPersonTableOperation();
                operation.Argument = argument;
                operation.Name = "批量导入承包方调查表";
                operation.Description = zone.FullName;
                Add(operation);
            }
            base.OnGo();
        }

        /// <summary>
        /// 获取匹配文件的路径
        /// </summary>
        private string GetMatchFilePath(string fileName, Zone zone, IZoneWorkStation zoneWorkStation)
        {
            string path = string.Empty;
            if (string.IsNullOrEmpty(fileName) || zone == null)
                return path;
            var tableFiles = Directory.GetFiles(fileName);   //获得文件夹下的所有表格文件
            if (tableFiles.Count() == 0)
            {
                return path;
            }
            var zoneName = zoneWorkStation.GetZoneNameByLevel(zone.FullCode, eZoneLevel.Town) +
                           zoneWorkStation.GetZoneNameByLevel(zone.FullCode, eZoneLevel.Village) +
                           zoneWorkStation.GetZoneNameByLevel(zone.FullCode, eZoneLevel.Group);
            foreach (var tableFile in tableFiles)
            {
                string tableName = Path.GetFileNameWithoutExtension(tableFile);    //.Replace("承包地块调查表", "");
                if (tableName.Trim().Equals(zoneName.Trim())) //导入表名匹配镇村组格式
                {
                    path = tableFile;
                    break;
                }
            }
            return path;
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
