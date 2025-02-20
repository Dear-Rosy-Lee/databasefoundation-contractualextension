/*
 * (C) 2025  鱼鳞图公司版权所有,保留所有权利 
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
    /// 查找四至
    /// </summary>
  public  class TaskGroupSeekLandNeighborOperation : TaskGroup
    {
        #region Ctor

        /// <summary>
        /// 构造函数
        /// </summary>
        public TaskGroupSeekLandNeighborOperation()
        {
        }

        #endregion

        #region Method

        /// <summary>
        /// 开始执行组任务
        /// </summary>
        protected override void OnGo()
        {
            TaskGroupSeekLandNeighborArgument groupMeta = Argument as TaskGroupSeekLandNeighborArgument;
            if (groupMeta == null)
            {
                return;
            }
            IDbContext dbContext = groupMeta.Database;
            Zone currentZone = groupMeta.CurrentZone;
            List<Zone> allZones = new List<Zone>();           
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
            var landStation = dbContext.CreateContractLandWorkstation();
            foreach (var zone in allZones)
            {
                List<ContractLand> listGeoLand = landStation.GetCollection(zone.FullCode, eLevelOption.Self).FindAll(c=> c.Shape != null);
                //执行单个任务
                TaskSeekLandNeighborArgument meta = new TaskSeekLandNeighborArgument();
                meta.CurrentZone = zone;
                meta.Database = dbContext;
                meta.CurrentZoneLandList = listGeoLand;
                meta.DicList = groupMeta.DictList;
                meta.seekLandNeighborSet = groupMeta.seekLandNeighborSet;
                TaskSeekLandNeighborOperation Seek = new TaskSeekLandNeighborOperation();
                Seek.Argument = meta;
                Seek.Description = zone.FullName;
                Seek.Name = "查找四至";
                Add(Seek);   //添加子任务到任务组中
            }
            base.OnGo();
        }

        #endregion

    }
}
