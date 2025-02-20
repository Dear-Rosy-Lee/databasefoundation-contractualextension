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
using NPOI.SS.Formula.Functions;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace YuLinTu.Library.Business
{
    /// <summary>
    /// 土地承包经营权单户确认表/村组公示表/签字表/登记公示表/台账调查表
    /// </summary>
    public class TaskGroupAccountFiveTableOperation : TaskGroup
    {

        private string openFilePath;  //打开文件路径

        /// <summary>
        /// 开始执行组任务
        /// </summary>
        protected override void OnGo()
        {
            Clear();
            TaskGroupAccountFiveTableArgument groupMeta = Argument as TaskGroupAccountFiveTableArgument;
            if (groupMeta == null)
            {
                return;
            }
            IDbContext dbContext = groupMeta.Database;
            Zone currentZone = groupMeta.CurrentZone;
            string fileName = groupMeta.FileName;
            List<Zone> selfAndSubsZones = groupMeta.SelfAndSubsZones;

            var nationList = EnumStore<eContractAccountType>.GetListByType();
            foreach (var zone in selfAndSubsZones)
            {
                if (groupMeta.TableType == 5)
                {
                    openFilePath = Path.Combine(fileName, CreateDirectoryHelper.CreateDirectory(groupMeta.AllZones, zone));
                }
                else
                {
                    openFilePath = Path.Combine(fileName, CreateDirectoryHelper.CreateDirectoryByVilliage(groupMeta.AllZones, zone));
                }
                var argtypeName = nationList.Find(t => t.Value == groupMeta.ArgType).DisplayName;
                var meta = new TaskAccountFiveTableArgument();
                meta.IsClear = false;
                meta.FileName = openFilePath;
                meta.ArgType = groupMeta.ArgType;
                meta.Database = dbContext;
                meta.CurrentZone = zone;
                meta.VirtualType = groupMeta.VirtualType;
                meta.UserName = "";
                meta.Date = groupMeta.Date;
                meta.TableType = groupMeta.TableType;
                meta.AllZones = groupMeta.AllZones;
                meta.SelfAndSubsZones = groupMeta.SelfAndSubsZones;
                meta.IsBatch = groupMeta.IsBatch;
                meta.DictList = groupMeta.DictList;
                meta.DelcTime = groupMeta.DelcTime;
                meta.PubTime = groupMeta.PubTime;
                var import = new TaskAccountFiveTableOperation();
                import.Argument = meta;
                import.Description = "导出" + zone.FullName;
                import.Name = $"导出台账报表-{argtypeName}";
                Add(import);   //添加子任务到任务组中
            }
            CanOpenResult = true;
            base.OnGo();
        }

        /// <summary>
        /// 打开文件
        /// </summary>
        public override void OpenResult()
        {
            TaskGroupAccountFiveTableArgument groupMeta = Argument as TaskGroupAccountFiveTableArgument;
            System.Diagnostics.Process.Start(groupMeta.FileName);
            base.OpenResult();
        }

    }
}
