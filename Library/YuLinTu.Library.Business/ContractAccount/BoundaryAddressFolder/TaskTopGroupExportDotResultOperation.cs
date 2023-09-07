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
    /// 批量导出界址点成果表数据操作任务类
    /// </summary>
    public class TaskTopGroupExportDotResultOperation : TaskGroup
    {
        #region Ctor

        /// <summary>
        /// 构造函数
        /// </summary>
        public TaskTopGroupExportDotResultOperation()
        { }

        #endregion

        #region Method—Override

        /// <summary>
        /// 开始执行任务
        /// </summary>
        protected override void OnGo()
        {
            Clear();
            TaskTopGropExportDotResultArgument topArgument = Argument as TaskTopGropExportDotResultArgument;
            if (topArgument == null)
            {
                return;
            }
            IDbContext dbContext = topArgument.Database;
            List<Zone> allZones = topArgument.AllZones;
            List<VirtualPerson> listPerson = topArgument.ListPerson;
            List<ContractLand> listGeoLand = topArgument.ListGeoLand;
            string fileName = topArgument.FileName;
            foreach (var zone in allZones)
            {
                List<VirtualPerson> currentPersons = listPerson.FindAll(c => c.ZoneCode.Equals(zone.FullCode));
                TaskGroupExportDotResultArgument groupArgument = new TaskGroupExportDotResultArgument();
                groupArgument.ListPerson = currentPersons;
                groupArgument.ListGeoLand = listGeoLand;
                groupArgument.CurrentZone = zone;
                groupArgument.Database = dbContext;
                groupArgument.FileName = fileName;
                groupArgument.VirtualType = topArgument.VirtualType;
                TaskGroupExportDotResultOperation groupOperation = new TaskGroupExportDotResultOperation();
                groupOperation.Argument = groupArgument;
                groupOperation.Name = "导出界址点成果表";
                groupOperation.Description = zone.FullName;
                Add(groupOperation);
            }
            base.OnGo();
        }

        #endregion

        #region Method—Helper

        #endregion
    }
}
