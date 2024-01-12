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

namespace YuLinTu.Component.FuSui
{
    public class TaskGroupExportContractInformationOperationFusui : TaskGroup
    {
        public TaskGroupExportContractInformationOperationFusui()
        {
        }

        #region Properties

        /// <summary>
        /// 工作页
        /// </summary>
        public IWorkpage Workpage { get; set; }

        #endregion Properties

        #region Method-Override

        /// <summary>
        /// 开始执行任务
        /// </summary>
        protected override void OnGo()
        {
            Clear();
            TaskAccountFiveTableArgument groupArgument = Argument as TaskAccountFiveTableArgument;
            if (groupArgument == null)
            {
                return;
            }
            string fileName = groupArgument.FileName;
            IDbContext dbContext = groupArgument.Database;
            Zone currentZone = groupArgument.CurrentZone;
            List<Zone> selfAndSubsZones = new List<Zone>();
            List<Zone> allZones = new List<Zone>();
            List<VirtualPerson> selfAndSubsPersons = new List<VirtualPerson>();
            try
            {
                var zoneStation = dbContext.CreateZoneWorkStation();
                var personStation = dbContext.CreateVirtualPersonStation<LandVirtualPerson>();
                selfAndSubsPersons = personStation.GetByZoneCode(currentZone.FullCode, eLevelOption.SelfAndSubs);
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
                var listPersons = selfAndSubsPersons.FindAll(c => !string.IsNullOrEmpty(c.ZoneCode) && c.ZoneCode == zone.FullCode);
                string savePath = CreateDirectoryHelper.CreateDirectory(allZones, zone);
                TaskAccountFiveTableArgument argument = new TaskAccountFiveTableArgument();
                argument.SelfAndSubsZones = selfAndSubsZones;
                argument.CurrentZone = zone;
                argument.Database = dbContext;
                argument.FileName = fileName + @"\" + savePath;
                argument.VirtualType = groupArgument.VirtualType;
                TaskExportContractInformationOperationFusui operation = new TaskExportContractInformationOperationFusui();
                operation.Argument = argument;
                operation.Name = "批量导出合同信息表";
                operation.Description = zone.FullName;
                Add(operation);
            }
            base.OnGo();
        }

        #endregion Method-Override

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