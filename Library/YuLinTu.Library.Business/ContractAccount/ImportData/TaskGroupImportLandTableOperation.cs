/*
 * (C) 2015  鱼鳞图公司版权所有,保留所有权利 
 */
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
    public class TaskGroupImportLandTableOperation : TaskGroup
    {
        #region Ctor

        /// <summary>
        /// 构造函数
        /// </summary>
        public TaskGroupImportLandTableOperation()
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
            TaskGroupImportLandTableArgument groupArgument = Argument as TaskGroupImportLandTableArgument;
            if (groupArgument == null)
            {
                return;
            }
            string fileName = groupArgument.FileName;
            IDbContext dbContext = groupArgument.DbContext;
            Zone currentZone = groupArgument.CurrentZone;
            List<Zone> allZones = new List<Zone>();
            IContractLandWorkStation landStation;
            IZoneWorkStation zoneStation;
            try
            {
                landStation = dbContext.CreateContractLandWorkstation();
                zoneStation = dbContext.CreateZoneWorkStation();
                allZones = zoneStation.GetChildren(currentZone.FullCode, eLevelOption.SelfAndSubs);
            }
            catch (Exception ex)
            {
                YuLinTu.Library.Log.Log.WriteException(this, "GetChildren(获取地域数据失败)", ex.Message + ex.StackTrace);
                ShowBox("批量导入地块调查表", string.Format("获取地域数据失败"));
                return;
            }
            foreach (var zone in allZones)
            {
                TaskImportLandTableArgument argument = new TaskImportLandTableArgument();
                argument.CurrentZone = zone;
                argument.DbContext = dbContext;
                argument.FileName = GetMatchFilePath(fileName, zone, zoneStation);
                argument.VirtualType = groupArgument.VirtualType;
                TaskImportLandTableOperation operation = new TaskImportLandTableOperation();
                operation.Argument = argument;
                operation.Name = "批量导入地块调查表";
                operation.Description = zone.FullName;
                Add(operation);
            }
            base.OnGo();
        }

        /// <summary>
        /// 获取匹配文件的路径
        /// </summary>
        /// 修改于2016/08/27 规定批量导入地块调查表时，表格的命名要符合规范要求：**镇**村**组（一定要和相应地域的村组名称相匹配）
        /// 如果是村级导入，则表格的命名就直接是：**镇**村 即可
        /// 否则一律按照未匹配无法导入处理。
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
            //string findFile = tableFiles.FirstOrDefault(c =>
            //{
            //    var nameWithoutExtension = Path.GetFileNameWithoutExtension(c);
            //    if (string.IsNullOrEmpty(nameWithoutExtension))
            //        return false;
            //    string tableName = nameWithoutExtension.Replace("承包地块调查表", "");
            //    if (string.IsNullOrEmpty(tableName))
            //        return false;
            //    if (tableName.Trim().Equals(zoneName))
            //        return true;
            //    return false;
            //});
            //if (string.IsNullOrEmpty(findFile))
            //    return path;
            //return findFile;
            foreach (var tableFile in tableFiles)
            {
                var nameWithoutExtension = Path.GetFileNameWithoutExtension(tableFile);
                if (nameWithoutExtension != null)
                {
                    string tableName = nameWithoutExtension.Replace("承包地块调查表", "");
                    if (tableName.Trim().Equals(zoneName.Trim())) //导入表名匹配镇村组格式
                    {
                        path = tableFile;
                        break;
                    }
                }
            }
            return path;
        }

        #endregion


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

    }
}
