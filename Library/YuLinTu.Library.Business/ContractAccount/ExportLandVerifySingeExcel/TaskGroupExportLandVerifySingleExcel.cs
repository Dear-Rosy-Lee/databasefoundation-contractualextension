using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using YuLinTu.Data;
using YuLinTu.Library.Entity;
using YuLinTu.Windows;
using YuLinTu.Windows.Wpf.Metro.Components;

namespace YuLinTu.Library.Business
{
    public class TaskGroupExportLandVerifySingleExcel : TaskGroup
    {
        #region Fields

        private string fileName; //保存文件路径

        #endregion Fields

        #region Properties

        /// <summary>
        /// 工作页
        /// </summary>
        public IWorkpage Workpage { get; set; }


        #endregion Properties

        #region ctor

        public TaskGroupExportLandVerifySingleExcel()
        {
        }

        #endregion ctor

        #region Method—Override

        /// <summary>
        /// 开始执行任务
        /// </summary>
        protected override void OnGo()
        {
            Clear();
            var groupArgument = Argument as TaskGroupExportArgument;
            if (groupArgument == null)
            {
                return;
            }
            fileName = groupArgument.FileName;
            IDbContext dbContext = groupArgument.Database;
            Zone currentZone = groupArgument.CurrentZone;
            List<Zone> selfAndSubsZones = new List<Zone>();
            List<Zone> allZones = new List<Zone>();
            try
            {
                var zoneStation = dbContext.CreateZoneWorkStation();
                selfAndSubsZones = zoneStation.GetChildren(currentZone.FullCode, eLevelOption.SelfAndSubs);
                var valiages = selfAndSubsZones.Where(x => x.Level == eZoneLevel.Village).ToList();
                selfAndSubsZones = selfAndSubsZones.Where(x => x.Level == eZoneLevel.Group).ToList();
                foreach (var item in valiages)
                {
                    var count = selfAndSubsZones.Count(t => t.FullCode.StartsWith(item.FullCode) && item.FullCode != t.FullCode);
                    if (count == 0)
                        selfAndSubsZones.Add(item);
                }
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
                string savePath = CreateDirectoryHelper.CreateDirectory(allZones, zone);
                var argument = new TaskExportLandVerifySingleExcelArgument();
                argument.CurrentZone = zone;
                argument.DbContext = dbContext;
                argument.FileName = fileName + @"\" + savePath;
                argument.IsShow = false;
                Directory.CreateDirectory(fileName + @"\" + savePath);
                argument.VirtualType = eVirtualType.Land;
                var operation = new TaskExportLandVerifySingleExcelOperation();
                operation.Argument = argument;
                operation.Name = "批量导出单户摸底核实表";
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

        #endregion Method—Override

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

        #endregion Method - 辅助
    }
}