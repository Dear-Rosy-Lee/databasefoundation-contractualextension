/*
 * (C) 2025  鱼鳞图公司版权所有,保留所有权利 
 */
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using YuLinTu.Data;
using YuLinTu.Diagrams;
using YuLinTu.Library.Entity;

namespace YuLinTu.Library.Business
{
    /// <summary>
    /// 批量导出地块示意图任务类
    /// </summary>
    public class TaskGroupExportMultiParcelWordOperation : ParallelTaskGroup
    {
        #region Ctor

        /// <summary>
        /// 构造函数
        /// </summary>
        public TaskGroupExportMultiParcelWordOperation()
        {
            MaxThreadCount = 3;
        }

        #endregion

        #region Field

        //private string openFilePath;  //打开文件路径

        #endregion

        #region Property
        public bool? IsStockLand { get; set; }

        #endregion

        #region Method—Override

        /// <summary>
        /// 开始执行任务
        /// </summary>
        protected override void OnGo()
        {
            Clear();
            TaskGroupExportMultiParcelWordArgument groupArgument = Argument as TaskGroupExportMultiParcelWordArgument;
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

            //var currentGroupZone = selfAndSubsZones.FirstOrDefault(c => c.Level == eZoneLevel.Group);
            //if (currentGroupZone == null)
            //    currentGroupZone = selfAndSubsZones.FirstOrDefault(c => c.Level == eZoneLevel.Village);
            var settingDefine = ContractBusinessParcelWordSettingDefine.GetIntence();
            double buffer = settingDefine.Neighborlandbufferdistence;
            //var currentVillageContractLands = ContractLandHeler.GetCurrentVillageContractLand(currentGroupZone, dbContext, buffer);
            foreach (var zone in selfAndSubsZones)
            {
                var listPersons = selfAndSubsPersons.FindAll(c => !string.IsNullOrEmpty(c.ZoneCode) && c.ZoneCode == zone.FullCode);
                string savePath = CreateDirectoryHelper.CreateDirectory(allZones, zone);
                TaskExportMultiParcelWordArgument argument = new TaskExportMultiParcelWordArgument();
                argument.SelectedPersons = listPersons == null ? new List<VirtualPerson>(100) : listPersons;
                argument.CurrentZone = zone;
                argument.DbContext = dbContext;
                argument.FileName = fileName + @"\" + savePath;
                TaskExportMultiParcelWordOperation operation = new TaskExportMultiParcelWordOperation();
                operation.IsStockLand = IsStockLand;
                operation.Argument = argument;
                operation.Name = "批量导出地块示意图";
                operation.Description = zone.FullName;

                //if (currentGroupZone != null && (zone.Level == eZoneLevel.Group || zone.Level == eZoneLevel.Village))
                //{
                //    bool isVillageChanged = false;  // 村地域是否发生改变
                //    if (zone.Level == eZoneLevel.Group && !zone.UpLevelCode.Equals(currentGroupZone.UpLevelCode))
                //        isVillageChanged = true;
                //    else if (zone.Level == eZoneLevel.Village && !zone.FullCode.Equals(currentGroupZone.UpLevelCode))
                //        isVillageChanged = true;

                //    // 当村地域未发生改变时，用同一村的地块集合
                //    if (!isVillageChanged)
                //        operation.VillageContractLands = currentVillageContractLands;
                //    else
                //    {
                //        // 当村地域发生改变时，使用新村的地块集合
                //        if (zone.Level == eZoneLevel.Village)
                //            currentGroupZone = selfAndSubsZones.FirstOrDefault(c => c.Level == eZoneLevel.Group && c.UpLevelCode.Equals(zone.FullCode));
                //        else
                //            currentGroupZone = zone;
                //        currentVillageContractLands = ContractLandHeler.GetCurrentVillageContractLand(currentGroupZone, dbContext, buffer);
                //        operation.VillageContractLands = currentVillageContractLands;
                //    }
                //}
                
                operation.VillageContractLands = ContractLandHeler.GetCurrentVillageContractLand(zone, dbContext, settingDefine.Neighborlandbufferdistence);
                Add(operation);
            }
            base.OnGo();
        }

        #endregion

        #region Method—Helper

        #endregion
    }
}
