/*
 * (C) 2025  鱼鳞图公司版权所有,保留所有权利 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using YuLinTu.Library.Entity;
using YuLinTu;
using YuLinTu.Data;
using System.Diagnostics;
using System.IO;

namespace YuLinTu.Library.Business
{
    /// <summary>
    /// 承包方数据操作任务类
    /// </summary>
    public class TaskGroupRelationDealOperation : TaskGroup
    {
        #region Fields

        //private int currentIndex = 1;//当前索引号
        //private List<string> shipList;//家庭关系集合
        //private List<string> upList;//上级家庭关系集合
        //private List<string> downList;//下级家庭关系集合

        #endregion

        #region Property


        #endregion

        #region Ctor

        /// <summary>
        /// 构造函数
        /// </summary>
        public TaskGroupRelationDealOperation()
        {
        }

        #endregion

        #region Override

        /// <summary>
        /// 开始操作
        /// </summary
        protected override void OnGo()
        {
            Clear();
            TaskGroupRelationDealArgument groupArgument = Argument as TaskGroupRelationDealArgument;
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
                TaskRelationDealArgument metadata = new TaskRelationDealArgument();
                metadata.CurrentZone = zone;
                metadata.Database = dbContext;
                metadata.Type = 1;
                TaskRelationDealOperation deal = new TaskRelationDealOperation();
                deal.Argument = metadata;
                deal.Name = "数据处理";
                deal.Description = "家庭关系检查";
                Add(deal);
            }
            CanOpenResult = true;
            base.OnGo();
        }

        #endregion

        protected override void OnTerminate(Exception ex)
        {
            base.OnTerminate(ex);
        }

        protected override void OnStopped()
        {
            base.OnStopped();
        }


    }
}
