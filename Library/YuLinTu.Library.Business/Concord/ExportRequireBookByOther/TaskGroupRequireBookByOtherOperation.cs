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
    /// 导出非家庭承包单户申请书
    /// </summary>
    public class TaskGroupRequireBookByOtherOperation : TaskGroup
    {

        private string openFilePath;  //打开文件路径

        /// <summary>
        /// 开始执行组任务
        /// </summary>
        protected override void OnGo()
        {
            Clear();
            TaskGroupRequireBookByOtherArgument groupMeta = Argument as TaskGroupRequireBookByOtherArgument;
            if (groupMeta == null)
            {
                return;
            }
            IDbContext dbContext = groupMeta.Database;
            Zone currentZone = groupMeta.CurrentZone;
            string fileName = groupMeta.FileName;
            List<Zone> selfAndSubsZones = groupMeta.SelfAndSubsZones;

            foreach (var zone in selfAndSubsZones)
            {
                openFilePath = Path.Combine(fileName, CreateDirectoryHelper.CreateDirectory(groupMeta.AllZones, zone));
                TaskRequireBookByOtherArgument meta = new TaskRequireBookByOtherArgument();
                meta.FileName = openFilePath;
                meta.ArgType = eContractConcordArgType.ExportApplicationByOther;
                meta.Database = dbContext;
                meta.CurrentZone = zone;
                meta.SelfAndSubsZones = selfAndSubsZones;
                meta.AllZones = groupMeta.AllZones;
                meta.SystemSet = groupMeta.SystemSet;
                meta.PublishDateSetting = groupMeta.PublishDateSetting;
                meta.SummaryDefine = groupMeta.SummaryDefine;
                TaskRequireBookByOtherOperation taskConcord = new TaskRequireBookByOtherOperation();
                taskConcord.Argument = meta;
                taskConcord.Name = "导出非家庭承包单户申请书";
                taskConcord.Description = "导出" + zone.Name;
                Add(taskConcord);   //添加子任务到任务组中
            }
            CanOpenResult = true;
            base.OnGo();
        }

        /// <summary>
        /// 打开文件
        /// </summary>
        public override void OpenResult()
        {
            TaskGroupRequireBookByOtherArgument groupMeta = Argument as TaskGroupRequireBookByOtherArgument;
            System.Diagnostics.Process.Start(groupMeta.FileName);
            base.OpenResult();
        }
    }
}
