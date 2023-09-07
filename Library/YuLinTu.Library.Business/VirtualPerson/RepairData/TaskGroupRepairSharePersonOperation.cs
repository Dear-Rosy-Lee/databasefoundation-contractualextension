using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YuLinTu;
using YuLinTu.Data;
using YuLinTu.Library.Entity;

namespace YuLinTu.Library.Business
{
    public class TaskGroupRepairSharePersonOperation : TaskGroup
    {
        #region Properties

        #endregion Properties

        #region Fields

        #endregion Fields

        #region Ctor

        public TaskGroupRepairSharePersonOperation()
        {
            Name = "TaskGroupRepairSharePersonOperation";
            Description = "This is TaskGroupRepairSharePersonOperation";
        }

        #endregion Ctor

        #region Methods

        #region Methods - Override

        protected override void OnGo()
        {
            var args = Argument as TaskGroupRepairSharePersonOperationArgument;
            if (args == null)
            {
                this.ReportError("参数不能为空");
                return;
            }

            var groupArgument = Argument as TaskGroupRepairSharePersonOperationArgument;
            if (groupArgument == null)
            {
                return;
            }
            IDbContext dbContext = groupArgument.Database;
            var currentZone = groupArgument.CurrentZone;
            var selfAndSubsZones = new List<Zone>();
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
                var metadata = new TaskRepairSharePersonOperationArgument();
                metadata.CurrentZone = zone;
                metadata.Database = dbContext;
                var task = new TaskRepairSharePersonOperation();
                task.Argument = metadata;
                task.Name = "数据修复";
                task.Description = "家庭成员关联数据修复";
                Add(task);
            }
            CanOpenResult = true;
            base.OnGo();
        }

        #endregion Methods - Override

        #endregion Methods
    }
}