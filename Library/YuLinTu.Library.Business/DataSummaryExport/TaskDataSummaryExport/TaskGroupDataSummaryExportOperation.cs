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
    /// 批量导出汇总数据操作
    /// </summary>
    public class TaskGroupDataSummaryExportOperation : TaskGroup
    {

        private string openFilePath;  //打开文件路径

        /// <summary>
        /// 开始执行组任务
        /// </summary>
        protected override void OnGo()
        {
            Clear();
            TaskGroupDataSummaryExportArgument groupMeta = Argument as TaskGroupDataSummaryExportArgument;
            if (groupMeta == null)
            {
                return;
            }
            IDbContext dbContext = groupMeta.Database;
            Zone currentZone = groupMeta.CurrentZone;
            string fileName = groupMeta.FileName;
            List<Zone> selfAndSubsZones = groupMeta.SelfAndSubsZones;

            var personStaion = dbContext.CreateVirtualPersonStation<LandVirtualPerson>();
            var selfAndSubsVps = personStaion.GetByZoneCode(currentZone.FullCode, eLevelOption.SelfAndSubs);
            if (selfAndSubsVps == null || selfAndSubsVps.Count == 0)
            {
                this.ReportError(string.Format("{0}未获取承包方数据", currentZone.FullName));
                return;
            }

            foreach (var zone in selfAndSubsZones)
            {
                var curVps = selfAndSubsVps.FindAll(c => c.ZoneCode == zone.FullCode);
                if (zone.Level == eZoneLevel.Group)
                {
                    openFilePath = Path.Combine(fileName, CreateDirectoryHelper.CreateDirectory(groupMeta.AllZones, zone));
                }
                else
                {
                    openFilePath = Path.Combine(fileName, CreateDirectoryHelper.CreateDirectoryByVilliage(groupMeta.AllZones, zone));
                }
                TaskDataSummaryExportArgument meta = new TaskDataSummaryExportArgument();
                meta.IsClear = false;
                meta.FileName = openFilePath;
                meta.ArgType = groupMeta.ArgType;
                meta.Database = dbContext;
                meta.CurrentZone = zone;
                meta.VirtualType = groupMeta.VirtualType;
                meta.UserName = "";
                meta.Date = groupMeta.Date;
                meta.TableType = groupMeta.TableType;
                meta.SystemSet = groupMeta.SystemSet;
                meta.AllZones = groupMeta.AllZones;
                meta.SelfAndSubsZones = groupMeta.SelfAndSubsZones;
                meta.ContractLandOutputSurveyDefine = groupMeta.ContractLandOutputSurveyDefine;
                meta.SettingDefine = groupMeta.SettingDefine;
                meta.SingleFamilySurveySetting = groupMeta.SingleFamilySurveySetting;
                meta.FamilyOtherSet = groupMeta.FamilyOtherSet;
                meta.FamilyOutputSet = groupMeta.FamilyOutputSet;
                meta.PublishDateSetting = groupMeta.PublishDateSetting;
                meta.PubDateValue = groupMeta.PubDateValue;
                meta.IsBatch = groupMeta.IsBatch;
                meta.DictList = groupMeta.DictList;
                meta.ExportDataSummaryTableTypes = groupMeta.ExportDataSummaryTableTypes;
                meta.ConcordSettingDefine = groupMeta.ConcordSettingDefine;
                meta.ExtendUseExcelDefine = groupMeta.ExtendUseExcelDefine;
                meta.BookLandNum = groupMeta.BookLandNum;
                meta.BookPersonNum = groupMeta.BookPersonNum;
                meta.ParentsToProvince = groupMeta.ParentsToProvince;
                meta.SelectContractors = curVps == null ? new List<VirtualPerson>() : curVps;

                TaskDataSummaryExportOperation import = new TaskDataSummaryExportOperation();
                import.Argument = meta;
                import.Description = "导出" + zone.FullName;
                import.Name = "导出汇总数据";
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
            TaskGroupDataSummaryExportArgument groupMeta = Argument as TaskGroupDataSummaryExportArgument;
            System.Diagnostics.Process.Start(groupMeta.FileName);
            base.OpenResult();
        }
    }
}
