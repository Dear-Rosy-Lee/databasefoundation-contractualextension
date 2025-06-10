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
using static Xceed.Wpf.Toolkit.Calculator;

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
            List<ContractLand> currentVillageContractLands = new List<ContractLand>();
            List<Zone> revzones = new List<Zone>(100);
            foreach (var zone in selfAndSubsZones)
            {
                if (revzones.Contains(zone))
                    continue;
                if (zone.Level > eZoneLevel.Village)
                    continue;
                if (zone.Level == eZoneLevel.Village)
                {
                    currentVillageContractLands = ContractLandHeler.GetCurrentVillageContractLand(zone, dbContext, settingDefine.Neighborlandbufferdistence);

                    if (selfAndSubsZones.Any(t => t.UpLevelCode == zone.FullCode))
                    {
                        foreach (var item in selfAndSubsZones.FindAll(t => t.UpLevelCode == zone.FullCode))
                        {
                            var listPersons = selfAndSubsPersons.FindAll(c => !string.IsNullOrEmpty(c.ZoneCode) && c.ZoneCode == item.FullCode);
                            string savePath = CreateDirectoryHelper.CreateDirectory(allZones, item);
                            string fsavePath = fileName + @"\" + savePath;
                            Add(Operation(dbContext, listPersons, item, fsavePath, currentVillageContractLands.FindAll(t => t.ZoneCode == item.FullCode)));
                            revzones.Add(item);
                        }
                    }
                    else
                    {
                        var listPersons = selfAndSubsPersons.FindAll(c => !string.IsNullOrEmpty(c.ZoneCode) && c.ZoneCode == zone.FullCode);
                        string savePath = CreateDirectoryHelper.CreateDirectory(allZones, zone);
                        string fsavePath = fileName + @"\" + savePath;
                        Add(Operation(dbContext, listPersons, zone, fsavePath, currentVillageContractLands.FindAll(t => t.ZoneCode == zone.FullCode)));
                    }
                }
                else if (zone.Level == eZoneLevel.Group)
                {
                    currentVillageContractLands = ContractLandHeler.GetCurrentVillageContractLand(zone, dbContext, settingDefine.Neighborlandbufferdistence);
                    var listPersons = selfAndSubsPersons.FindAll(c => !string.IsNullOrEmpty(c.ZoneCode) && c.ZoneCode == zone.FullCode);
                    string savePath = CreateDirectoryHelper.CreateDirectory(allZones, zone);
                    string fsavePath = fileName + @"\" + savePath;
                    Add(Operation(dbContext, listPersons, zone, fsavePath, currentVillageContractLands));
                }
            }
            base.OnGo();
        }

        private TaskExportMultiParcelWordOperation Operation(IDbContext dbContext, List<VirtualPerson> listPersons, Zone zone, string savePath, List<ContractLand> contractLands)
        {
            TaskExportMultiParcelWordArgument argument = new TaskExportMultiParcelWordArgument();
            argument.SelectedPersons = listPersons == null ? new List<VirtualPerson>(100) : listPersons;
            argument.CurrentZone = zone;
            argument.DbContext = dbContext;
            argument.FileName = savePath;
            TaskExportMultiParcelWordOperation operation = new TaskExportMultiParcelWordOperation();
            operation.IsStockLand = IsStockLand;
            operation.Argument = argument;
            operation.Name = "批量导出地块示意图";
            operation.Description = zone.FullName;
            operation.VillageContractLands = contractLands;
            return operation;
        }

        #endregion

        #region Method—Helper

        #endregion
    }
}
