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

namespace YuLinTu.Library.Business
{
    public class TaskGroupInitalizeWarrentOperation : TaskGroup
    {
        /// <summary>
        /// 开始执行组任务
        /// </summary>
        protected override void OnGo()
        {
            Clear();
            TaskGroupInitalizeWarrentArgument groupMeta = Argument as TaskGroupInitalizeWarrentArgument;
            if (groupMeta == null)
            {
                return;
            }
            IDbContext dbContext = groupMeta.Database;
            Zone currentZone = groupMeta.CurrentZone;
            string fileName = groupMeta.FileName;
            List<Zone> allZones = groupMeta.AllZones;
            foreach (var zone in allZones)
            {
                var argument = new TaskContractRegeditBookArgument();
                argument.Database = dbContext;
                argument.CurrentZone = zone;
                argument.Concords = groupMeta.Concords.FindAll(c=>c.ZoneCode==zone.FullCode);
                argument.ListWarrants = groupMeta.ListWarrants.FindAll(c => c.ZoneCode == zone.FullCode);
                argument.WarrantsModified = groupMeta.WarrantsModified.FindAll(c => c.ZoneCode == zone.FullCode);                            
                argument.listPerson = groupMeta.listPerson.FindAll(c => c.ZoneCode == zone.FullCode);
                argument.ArgType = eContractRegeditBookType.InitialWarrantData;
                var operation = new TaskContractRegeditBookOperation();
                operation.Argument = argument;
                operation.Description = "初始化" + zone.Name+ "权证信息";  //任务描述
                operation.Name = "初始化权证信息";         //任务名称
                Add(operation);   //添加子任务到任务组中
            }
            base.OnGo();

        }
    }
}
