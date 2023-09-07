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
    /// 初始化承包方基本信息任务类
    /// </summary>
    public class TaskGroupInitialVPRelationShipOperation : TaskGroup
    {
        #region Ctor

        /// <summary>
        /// 构造函数
        /// </summary>
        public TaskGroupInitialVPRelationShipOperation()
        { }

        #endregion

        #region Field
        //private string ReplaceName;
        //private string ChooseName;

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
            TaskGroupInitialVPRelationShipArgument groupArgument = Argument as TaskGroupInitialVPRelationShipArgument;
            if (groupArgument == null)
            {
                return;
            }
            IDbContext dbContext = groupArgument.Database;
            Zone currentZone = groupArgument.CurrentZone;
            List<Zone> selfAndSubsZones = new List<Zone>();
            try
            {
                var zoneStation = dbContext.CreateZoneWorkStation();
                selfAndSubsZones = zoneStation.GetChildren(currentZone.FullCode, eLevelOption.SelfAndSubs);
            }
            catch (Exception ex)
            {
                YuLinTu.Library.Log.Log.WriteException(this, "GetChildren(获取地域数据失败)", ex.Message + ex.StackTrace);
                this.ReportError("获取子级地域数据失败!");
                return;
            }
            foreach (var zone in selfAndSubsZones)
            {
                TaskInitialVPRelationShipArgument argument = new TaskInitialVPRelationShipArgument();              
                argument.Database = dbContext;
                argument.CurrentZone = zone;
                argument.ReplaceName = groupArgument.ReplaceName;
                argument.ChooseName = groupArgument.ChooseName;
                TaskInitialVPRelationShipOperation task = new TaskInitialVPRelationShipOperation();
                task.Argument = argument;
                task.Name = "家庭关系替换";
                task.Description = zone.FullName;
                Add(task);                
            }
            CanOpenResult = true;
            base.OnGo();

        }

        protected override void OnTerminate(Exception ex)
        {
            base.OnTerminate(ex);
        }

        protected override void OnStopped()
        {
            base.OnStopped();
        }


        #endregion
        
    }
}
