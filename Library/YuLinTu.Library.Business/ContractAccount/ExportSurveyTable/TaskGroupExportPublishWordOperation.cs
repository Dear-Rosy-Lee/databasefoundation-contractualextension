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
using YuLinTu.Windows;
using YuLinTu.Windows.Wpf.Metro.Components;

namespace YuLinTu.Library.Business
{
    /// <summary>
    /// 批量导出公示结果归户表任务类
    /// </summary>
    public class TaskGroupExportPublishWordOperation : TaskGroup
    {
        #region Ctor

        /// <summary>
        /// 构造函数
        /// </summary>
        public TaskGroupExportPublishWordOperation()
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
            TaskGroupExportPublishWordArgument groupArgument = Argument as TaskGroupExportPublishWordArgument;
            if (groupArgument == null)
            {
                return;
            }
            string fileName = groupArgument.FileName;
            IDbContext dbContext = groupArgument.DbContext;
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

                var argument = new TaskExportPublishWordArgument();
                argument.SelectedPersons = listPersons == null ? new List<VirtualPerson>() : listPersons;
                argument.CurrentZone = zone;
                argument.DbContext = dbContext;
                argument.FileName = fileName;
                argument.ContractSettingDefine = groupArgument.ContractSettingDefine;
                var operation = new TaskExportPublishWordOperation();
                operation.Argument = argument;
                operation.Name = "批量导出公示结果归户表";
                operation.Description = zone.FullName;
                Add(operation);
            }
            base.OnGo();
        }

        #endregion
    }
}
