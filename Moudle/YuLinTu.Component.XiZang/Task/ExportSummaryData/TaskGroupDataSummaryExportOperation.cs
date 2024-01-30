/*
 * (C) 2015  鱼鳞图公司版权所有,保留所有权利 
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
using YuLinTu.Library.Business;

namespace YuLinTu.Component.XiZangLZ
{
    /// <summary>
    /// 批量导出汇总数据操作
    /// </summary>
   public class TaskGroupDataSummaryExportOperation: TaskGroup
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
            var zoneStation = dbContext.CreateZoneWorkStation();
            Zone currentZone = groupMeta.CurrentZone;
            string fileName = groupMeta.FileName;
            List<Zone> selfAndSubsZones = groupMeta.SelfAndSubsZones;
            //选择到镇或村时，导出登记审批表
            var villageOrTown = groupMeta.SelfAndSubsZones.Where(s => s.Level == eZoneLevel.Village || s.Level == eZoneLevel.Town);
            if (villageOrTown!=null&& groupMeta.ExportDataSummaryTableTypes.ExportDataDJBZSPTableTable)
            {
                foreach (var zone in villageOrTown)
                {
                    this.ReportProgress(0, "开始导出登记审批表...");
                    this.ReportAlert(eMessageGrade.Infomation, null, "开始导出登记审批表...");
                    var result = ExportDJBZSPtable(zone, groupMeta);
                    if (!result)
                    {
                        this.ReportInfomation(string.Format("未导出{0}的登记审批表", zone.Name));
                    }
                }
            }
            foreach (var zone in selfAndSubsZones)
            {
                //如果是镇级地域，不导出除登记审批表之外的其他表
                if (zone.Level == eZoneLevel.Town)
                {
                    continue;
                }
                //如果是村级地域且含有子级地域，不导出除登记审批表之外的其他表
                if (zone.Level == eZoneLevel.Village)
                {
                    var groupZones = zoneStation.GetChildren(zone.FullCode, eLevelOption.Subs);
                    if (groupZones != null && groupZones.Count != 0)
                    {
                        continue;
                    }
                }
                //其他地域情况导出除登记审批表之外的其他表，包括不含子级地域的村级地域
                this.ReportProgress(0, "开始导出其他表...");
                this.ReportAlert(eMessageGrade.Infomation, null, "开始导出其他表...");
                openFilePath = Path.Combine(fileName, CreateDirectoryHelper.CreateDirectory(groupMeta.AllZones, zone));
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
        /// 导出登记颁证审批表
        /// </summary>       
        private bool ExportDJBZSPtable(Zone villageOrTown, TaskGroupDataSummaryExportArgument vpExportArgs)
        {
            bool result = true;
            try
            {
                var vpstation = vpExportArgs.Database.CreateVirtualPersonStation<LandVirtualPerson>();
                var landstation = vpExportArgs.Database.CreateContractLandWorkstation();
                var senderstation = vpExportArgs.Database.CreateSenderWorkStation();
                var concordstation = vpExportArgs.Database.CreateConcordStation();
                List<YuLinTu.Library.Entity.VirtualPerson> vps = vpstation.GetByZoneCode(villageOrTown.FullCode, eLevelOption.SelfAndSubs);
                List<ContractLand> alllands = landstation.GetCollection(villageOrTown.FullCode, eLevelOption.SelfAndSubs);
                int allsendersCount = senderstation.Count(villageOrTown.FullCode, eLevelOption.SelfAndSubs);
                List<ContractConcord> allConcords = concordstation.GetContractsByZoneCode(villageOrTown.FullCode, eLevelOption.SelfAndSubs);
                if (vps.Count() == 0 && alllands.Count == 0 && allsendersCount == 0)
                {
                    this.ReportInfomation("提示", "本地域下没有发包方、承包方、地块数据");
                    return false;
                }
                vps.RemoveAll(vp => vp.Name.Contains("集体"));
                alllands.RemoveAll(l => l.OwnerName!=null&& l.OwnerName.Contains("集体"));
                string tempPath = TemplateHelper.WordTemplate("农村土地承包经营权登记审批表");
                ExportDJSPBTable export = new ExportDJSPBTable();
                export.CurrentZone = vpExportArgs.CurrentZone;
                export.ConstructType = ((int)eConstructMode.Family).ToString();
                export.SendersCount = allsendersCount;
                export.VirtualPersons = vps;
                export.CurrentZone = villageOrTown;
                export.DictList = vpExportArgs.DictList;
                export.LandCollection = alllands;  //地块集合
                export.OpenTemplate(tempPath);
                string pathTemp = Path.Combine(vpExportArgs.FileName, CreateDirectoryHelper.CreateDirectoryByVilliage(vpExportArgs.AllZones, villageOrTown));
                string savePath = pathTemp + @"\"+ "确权登记审批表";
                export.SaveAs(villageOrTown, savePath);
                result = true;
                this.ReportInfomation(string.Format("导出{0}的确权登记审批表", villageOrTown.Name));
            }
            catch (Exception ex)
            {
                this.ReportError(ex.Message);
                YuLinTu.Library.Log.Log.WriteException(this, "ExportDataWord(导出数据到Word表)", ex.Message + ex.StackTrace);
            }

            return result;

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
