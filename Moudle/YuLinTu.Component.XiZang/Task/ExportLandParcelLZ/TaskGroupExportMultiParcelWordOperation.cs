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

namespace YuLinTu.Component.XiZangLZ
{
    /// <summary>
    /// 批量导出标准地块示意图任务类
    /// </summary>
    public class TaskGroupExportMultiParcelWordOperationLZ : TaskGroup
    {
        #region Ctor

        /// <summary>
        /// 构造函数
        /// </summary>
        public TaskGroupExportMultiParcelWordOperationLZ()
        { }

        #endregion

        #region Field

        #endregion

        #region Property

        #endregion

        #region Method—Override

        /// <summary>
        /// 开始执行任务
        /// </summary>
        protected override void OnGo()
        {
            Clear();
            TaskGroupExportMultiParcelWordArgumentLZ groupArgument = Argument as TaskGroupExportMultiParcelWordArgumentLZ;
            if (groupArgument == null)
            {
                return;
            }
            string fileName = groupArgument.FileName;
            IDbContext dbContext = groupArgument.DbContext;
            Zone currentZone = groupArgument.CurrentZone;
            List<Zone> selfAndSubsZones = new List<Zone>(100);
            List<Zone> allZones = new List<Zone>(100);
            List<VirtualPerson> selfAndSubsPersons = new List<VirtualPerson>(500);
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
                this.ReportError("获取子级地域数据失败!");
                return;
            }
            foreach (var zone in selfAndSubsZones)
            {
                var listPersons = selfAndSubsPersons.FindAll(c => !string.IsNullOrEmpty(c.ZoneCode) && c.ZoneCode == zone.FullCode);
                string savePath = CreateDirectoryHelper.CreateDirectory(allZones, zone);
                TaskExportMultiParcelWordArgumentLZ argument = new TaskExportMultiParcelWordArgumentLZ();
                argument.SelectedPersons = listPersons == null ? new List<VirtualPerson>(100) : listPersons;
                argument.CurrentZone = zone;
                argument.DbContext = dbContext;
                argument.FileName = fileName + @"\" + savePath;
                argument.SettingDefine = groupArgument.SettingDefine;
                TaskExportMultiParcelWordOperationLZ operation = new TaskExportMultiParcelWordOperationLZ();
                operation.Argument = argument;
                operation.Name = "批量导出标准地块示意图";
                operation.Description = zone.FullName;
                Add(operation);
            }
            base.OnGo();
        }

        #endregion

        #region Method—Helper

        #endregion
    }
}
