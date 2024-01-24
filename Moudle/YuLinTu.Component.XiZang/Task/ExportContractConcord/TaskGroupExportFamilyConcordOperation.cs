/*
 * (C) 2015  鱼鳞图公司版权所有,保留所有权利 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using YuLinTu.Data;
using YuLinTu.Library.Entity;
using YuLinTu.Windows;
using YuLinTu.Windows.Wpf.Metro.Components;
using YuLinTu.Library.Business;
using YuLinTu.Library.WorkStation;

namespace YuLinTu.Component.XiZangLZ
{
    /// <summary>
    /// 批量导出合同任务类
    /// </summary>
    public class TaskGroupExportFamilyConcordOperation : TaskGroup
    {
        #region Ctor

        /// <summary>
        /// 构造函数
        /// </summary>
        public TaskGroupExportFamilyConcordOperation()
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
            TaskGroupExportFamilyConcordArgument groupArgument = Argument as TaskGroupExportFamilyConcordArgument;
            if (groupArgument == null)
            {
                return;
            }
            IDbContext dbContext = groupArgument.DbContext;
            Zone currentZone = groupArgument.CurrentZone;
            string filePath = groupArgument.FileName;
            List<Dictionary> listDict = new List<Dictionary>();
            List<Zone> allZones = new List<Zone>();
            try
            {
                var personStation = dbContext.CreateVirtualPersonStation<LandVirtualPerson>();
                var landStation = dbContext.CreateContractLandWorkstation();
                var zoneStation = dbContext.CreateZoneWorkStation();
                var dictStation = dbContext.CreateDictWorkStation();
                listDict = dictStation.Get();
                allZones = zoneStation.GetChildren(currentZone.FullCode, eLevelOption.SelfAndSubs);
                List<YuLinTu.Library.Entity.VirtualPerson> listPerson = new List<YuLinTu.Library.Entity.VirtualPerson>();
                List<YuLinTu.Library.Entity.VirtualPerson> allPersons = new List<YuLinTu.Library.Entity.VirtualPerson>();
                allPersons = personStation.GetByZoneCode(currentZone.FullCode, eLevelOption.SelfAndSubs);
                string zoneNameCounty = string.Empty;
                string zoneNameTown = string.Empty;
                string zoneNameVillage = string.Empty;
                string zoneNameGroup = string.Empty;
                foreach (var zone in allZones)
                {
                    zoneNameCounty = GetZoneNameByLevel(zone.FullCode, eZoneLevel.County, zoneStation);
                    zoneNameTown = GetZoneNameByLevel(zone.FullCode, eZoneLevel.Town, zoneStation);
                    zoneNameVillage = GetZoneNameByLevel(zone.FullCode, eZoneLevel.Village, zoneStation);
                    zoneNameGroup = GetZoneNameByLevel(zone.FullCode, eZoneLevel.Group, zoneStation);
                    listPerson = allPersons.FindAll(c => c.ZoneCode == zone.FullCode);
                    TaskExportFamilyConcordArgument meta = new TaskExportFamilyConcordArgument();
                    meta.FileName = filePath;
                    meta.DbContext = dbContext;
                    meta.CurrentZone = zone;
                    meta.ListPerson = listPerson;
                    meta.ListDict = listDict;
                    meta.systemset = groupArgument.systemset;
                    meta.ZoneNameCounty = zoneNameCounty;
                    meta.ZoneNameTown = zoneNameTown;
                    meta.ZoneNameVillage = zoneNameVillage;
                    meta.ZoneNameGroup = zoneNameGroup;
                    TaskExportFamilyConcordOperation taskConcord = new TaskExportFamilyConcordOperation();
                    taskConcord.Workpage = Workpage;
                    taskConcord.Argument = meta;
                    taskConcord.Description = zone.FullName;
                    taskConcord.Name = "批量导出承包合同";
                    Add(taskConcord);
                }
            }
            catch (Exception ex)
            {
                YuLinTu.Library.Log.Log.WriteException(this, "GetChildren(获取承包地块数据失败)", ex.Message + ex.StackTrace);
                ShowBox("批量导出承包合同", string.Format("导出{0}下的承包合同数据失败", currentZone.FullName));
                return;
            }
            base.OnGo();
        }

        #endregion

        #region Method - 辅助

        /// <summary>
        /// 根据地域编码与级别获取名称
        /// </summary> 
        private string GetZoneNameByLevel(string zoneCode, eZoneLevel level, IZoneWorkStation zoneStation)
        {
            Zone temp = zoneStation.Get(c => c.FullCode == zoneCode).FirstOrDefault();
            if (temp == null)
                return " ";
            if (temp.Level == level)
                return temp.Name;
            else
                return GetZoneNameByLevel(temp.UpLevelCode, level, zoneStation);
        }

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
