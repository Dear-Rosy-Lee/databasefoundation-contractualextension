/*
 * (C) 2025  鱼鳞图公司版权所有,保留所有权利 
 */
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using YuLinTu.Data;
using YuLinTu.Library.Entity;
using YuLinTu.Library.WorkStation;
using YuLinTu.Windows;

namespace YuLinTu.Library.Business
{
    /// <summary>
    /// 导出承包方Word调查表任务类
    /// </summary>
    public class TaskExportVirtualPersonWordOperation : Task
    {
        #region Ctor

        /// <summary>
        /// 构造函数
        /// </summary>
        public TaskExportVirtualPersonWordOperation()
        { }

        #endregion

        #region Field

        private string openFilePath;  //打开文件路径

        #endregion

        #region Property

        #endregion

        #region Method—Override

        /// <summary>
        /// 开始执行任务
        /// </summary>
        protected override void OnGo()
        {
            TaskExportVirtualPersonWordArgument argument = Argument as TaskExportVirtualPersonWordArgument;
            if (argument == null)
            {
                return;
            }
            IDbContext dbContext = argument.DbContext;
            var zone = argument.CurrentZone;
            openFilePath = argument.FileName;
            try
            {
                var listPerson = argument.SelectedPersons;
                if (listPerson == null || listPerson.Count == 0)
                {
                    this.ReportProgress(100, null);
                    this.ReportWarn(string.Format("{0}未获取承包方数据!", zone.FullName));
                    return;
                }
                bool canOpen = ExportVirtualPersonWord(argument, listPerson);
                if (canOpen)
                    CanOpenResult = true;
            }
            catch (Exception ex)
            {
                YuLinTu.Library.Log.Log.WriteException(this, "TaskExportVirtualPersonWordOperation(导出承包方数据任务)", ex.Message + ex.StackTrace);
                this.ReportException(ex, "导出承包方Word调查表出现异常!");
            }
        }

        /// <summary>
        /// 打开文件
        /// </summary>
        public override void OpenResult()
        {
            System.Diagnostics.Process.Start(openFilePath);
            base.OpenResult();
        }

        #endregion

        #region Method—ExportBusiness

        /// <summary>
        /// 导出数据到Word文档
        /// </summary>
        public bool ExportVirtualPersonWord(TaskExportVirtualPersonWordArgument argument, List<VirtualPerson> listPerson)
        {
            bool result = false;
            try
            {
                if (argument.CurrentZone == null)
                {
                    this.ReportError("未选择导出数据的地域!");
                    return result;
                }
                var senderStation = argument.DbContext.CreateSenderWorkStation();
                CollectivityTissue tissue = senderStation.Get(argument.CurrentZone.ID);
                if (tissue is null)
                {
                    tissue = senderStation.GetTissues(argument.CurrentZone.FullCode, eLevelOption.Self).FirstOrDefault();
                }
                if (tissue == null)
                {
                    //界面上没有选择项
                    this.ReportError(argument.CurrentZone.FullName + "下没有发包方数据");
                    return result;
                }

                this.ReportProgress(1, "开始");
                var zonelist = argument.DbContext.CreateZoneWorkStation().GetParentsToProvince(argument.CurrentZone);
                string markDesc = GetMarkDesc(argument.CurrentZone, argument.DbContext);
                string tempPath = TemplateHelper.WordTemplate(TemplateFile.VirtualPersonSurveyWord);
                int index = 1;
                var concordStation = argument.DbContext.CreateConcordStation();
                double vpPercent = 99 / (double)listPerson.Count;
                foreach (VirtualPerson family in listPerson)
                {
                    var concords = concordStation.GetAllConcordByFamilyID(family.ID);
                    string concordnumber = "";
                    if (concords != null && concords.Count > 0)
                    {
                        concordnumber = concords[0].ConcordNumber;
                    }
                    //是否显示集体户信息
                    if (argument.FamilyOtherSet.ShowFamilyInfomation && (family.Name.IndexOf("机动地") >= 0 || family.Name.IndexOf("集体") >= 0))
                    {
                        continue;
                    }
                    //只导出共有人为是的家庭成员
                    if (argument.SystemSet.PersonTable)
                        family.SharePersonList.Remove(family.SharePersonList.Find(c => c.IsSharedLand == "否"));
                    ExportContractorTable export = new ExportContractorTable();

                    #region 通过反射等机制定制化具体的业务处理类
                    var temp = WorksheetConfigHelper.GetInstance(export);
                    if (temp != null && temp.TemplatePath != null)
                    {
                        if (temp is ExportContractorTable)
                            export = (ExportContractorTable)temp;
                        tempPath = Path.Combine(TheApp.GetApplicationPath(), temp.TemplatePath);
                    }
                    #endregion

                    export.MarkDesc = markDesc;
                    export.CurrentZone = argument.CurrentZone;
                    export.Tissue = tissue;
                    export.DictList = argument.DictList;
                    export.ConcordNumber = concordnumber;
                    export.ZoneList = zonelist;
                    export.OpenTemplate(tempPath);
                    string familyNuber = ToolString.ExceptSpaceString(family.FamilyNumber);
                    export.SaveAs(family, openFilePath + @"\" + familyNuber + "-" + family.Name + "-农村土地承包经营权承包方调查表.doc");
                    this.ReportProgress((int)(1 + vpPercent * index), markDesc + family.Name);
                    index++;
                }
                result = true;
                string info = string.Format("{0}导出{1}张承包方Word调查表", markDesc, index - 1);
                this.ReportInfomation(info);
                this.ReportProgress(100, "完成");
            }
            catch (Exception ex)
            {
                result = false;
                this.ReportError("导出承包方Word调查表失败");
                YuLinTu.Library.Log.Log.WriteException(this, "ExportVirtualPersonWord(导出承包方数据到Word表)", ex.Message + ex.StackTrace);
            }
            return result;
        }

        #endregion

        #region Method—Private

        /// <summary>
        ///  获取上级地域
        /// </summary>
        /// <param name="zone"></param>
        /// <param name="dbContext"></param>
        /// <returns></returns>
        private Zone GetParent(Zone zone, IDbContext dbContext)
        {
            Zone parentZone = null;
            if (zone == null || dbContext == null)
                return parentZone;
            try
            {
                var zoneStation = dbContext.CreateZoneWorkStation();
                parentZone = zoneStation.Get(zone.UpLevelCode);
            }
            catch (Exception ex)
            {
                YuLinTu.Library.Log.Log.WriteException(this, "GetParent(获取父级地域失败!)", ex.Message + ex.StackTrace);
            }
            return parentZone;
        }

        /// <summary>
        /// 根据当前地域获得任务描述信息
        /// </summary>
        private string GetMarkDesc(Zone zone, IDbContext dbContext)
        {
            string excelName = string.Empty;
            if (zone == null || dbContext == null)
                return excelName;
            Zone parent = GetParent(zone, dbContext);  //获取上级地域
            string parentName = parent == null ? "" : parent.Name;
            if (zone.Level == eZoneLevel.County)
            {
                excelName = zone.Name;
            }
            else if (zone.Level == eZoneLevel.Town)
            {
                excelName = zone.Name;
            }
            else if (zone.Level == eZoneLevel.Village)
            {
                excelName = parentName + zone.Name;
            }
            else if (zone.Level == eZoneLevel.Group)
            {
                Zone parentTowm = GetParent(parent, dbContext);
                string parentTownName = parentTowm == null ? "" : parentTowm.Name;
                excelName = parentTownName + parentName + zone.Name;
            }
            return excelName;
        }

        #endregion

        #region Method—Helper

        #endregion
    }
}
