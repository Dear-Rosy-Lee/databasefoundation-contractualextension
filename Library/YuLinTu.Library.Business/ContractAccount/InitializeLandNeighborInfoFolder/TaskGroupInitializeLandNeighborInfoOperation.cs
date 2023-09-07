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
    /// 批量初始化地块数据操作任务类
    /// </summary>
    public class TaskGroupInitializeLandNeighborInfoOperation : TaskGroup
    {
        #region Ctor

        /// <summary>
        /// 构造函数
        /// </summary>
        public TaskGroupInitializeLandNeighborInfoOperation()
        {
        }

        #endregion

        #region Method

        /// <summary>
        /// 开始执行组任务
        /// </summary>
        protected override void OnGo()
        {
            TaskGroupInitializeLandNeighborInfoArgument groupMeta = Argument as TaskGroupInitializeLandNeighborInfoArgument;
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

            allZones.RemoveAll(a => a.Level != eZoneLevel.Town);

            foreach (var zone in allZones)
            {

                var argument = new TaskInitializeLandNeighborInfoArgument();
                argument.CurrentZone = zone;
                argument.Database = dbContext;
                string zoneCode = zone.FullCode;                
                argument.TownZoneCode = zoneCode;

                TaskInitializeLandNeighborInfoOperation operation = new TaskInitializeLandNeighborInfoOperation();
                operation.Argument = argument;
                operation.Description = zone.Name;   //任务描述
                operation.Name = "初始化查找地块周边地块信息";         //任务名称
                
                Add(operation);   //添加子任务到任务组中
            }
            base.OnGo();
        }

        #endregion
    }
}
