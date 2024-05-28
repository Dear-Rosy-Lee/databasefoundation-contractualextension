/*
 * (C) 2015  鱼鳞图公司版权所有,保留所有权利 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YuLinTu.Data;
using YuLinTu.Library.Entity;

namespace YuLinTu.Library.Business
{
    /// <summary>
    /// 批量初始化界址点线数据操作任务类
    /// </summary>
    public class TaskGroupInitializeLandCoilOperation : TaskGroup
    {
        #region Ctor

        /// <summary>
        /// 构造函数
        /// </summary>
        public TaskGroupInitializeLandCoilOperation()
        {
        }

        #endregion

        #region Method

        /// <summary>
        /// 开始执行组任务
        /// </summary>
        protected override void OnGo()
        {
            TaskGroupInitializeLandCoilArgument groupMeta = Argument as TaskGroupInitializeLandCoilArgument;
            if (groupMeta == null)
            {
                return;
            }
            IDbContext dbContext = groupMeta.Database;
            Zone currentZone = groupMeta.CurrentZone;
            List<Zone> allZones = new List<Zone>();
            var landStation = dbContext.CreateContractLandWorkstation();
            List<ContractLand> allLands = new List<ContractLand>();
            try
            {
                var zoneStation = dbContext.CreateZoneWorkStation();               
                allZones = zoneStation.GetChildren(currentZone.FullCode, eLevelOption.SelfAndSubs);
               
            }
            catch (Exception ex)
            {
                YuLinTu.Library.Log.Log.WriteException(this, "GetChildren(获取地域数据失败)", ex.Message + ex.StackTrace);
                this.ReportError("获取子级地域数据失败!");
                return;
            }
            foreach (var zone in allZones)
            {
                allLands = landStation.GetCollection(zone.FullCode, eLevelOption.Self);
                List<ContractLand> listGeoLand = allLands.FindAll(c => c.ZoneCode == zone.FullCode && c.Shape != null);
                TaskInitializeLandCoilArgument meta = new TaskInitializeLandCoilArgument();
                meta.CurrentZoneLandList = listGeoLand == null ? new List<ContractLand>() : listGeoLand;
                meta.CurrentZone = zone;
                meta.Database = dbContext;
                meta.AddressDotMarkType = groupMeta.AddressDotMarkType;
                meta.AddressDotType = groupMeta.AddressDotType;
                meta.AddressLineCatalog = groupMeta.AddressLineCatalog;
                meta.AddressLinedbiDistance = groupMeta.AddressLinedbiDistance;
                meta.AddressLinePosition = groupMeta.AddressLinePosition;
                meta.AddressLineType = groupMeta.AddressLineType;
                meta.IsNeighborExportVillageLevel = groupMeta.IsNeighborExportVillageLevel;
                meta.AddressPointPrefix = groupMeta.AddressPointPrefix;
                meta.IsLineDescription = groupMeta.IsLineDescription;
                meta.IsNeighbor = groupMeta.IsNeighbor;
                meta.IsPostion = groupMeta.IsPostion;
                meta.IsUnit = groupMeta.IsUnit;
                //meta.IsVirtualPersonFilter = groupMeta.IsVirtualPersonFilter;
                //meta.IsAngleFilter = groupMeta.IsAngleFilter;
                meta.IsFilterDot = groupMeta.IsFilterDot;
                meta.MinAngleFileter = groupMeta.MinAngleFileter;
                meta.MaxAngleFilter = groupMeta.MaxAngleFilter;
                meta.IsAllLands = groupMeta.IsAllLands;
                meta.IsLandsWithoutInfo = groupMeta.IsLandsWithoutInfo;
                meta.IsSelectedLands = groupMeta.IsSelectedLands;
                meta.SelectedObligees = groupMeta.SelectedObligees;
                TaskInitializeLandCoilOperation import = new TaskInitializeLandCoilOperation();
                import.Argument = meta;
                import.Description = zone.FullName;
                import.Name = "初始化地块界址数据";
                Add(import);   //添加子任务到任务组中
            }
            base.OnGo();
        }

        #endregion
    }
}
