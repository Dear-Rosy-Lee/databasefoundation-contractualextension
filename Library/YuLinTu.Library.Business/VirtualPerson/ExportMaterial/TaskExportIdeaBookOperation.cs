/*
 * (C) 2025  鱼鳞图公司版权所有,保留所有权利 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YuLinTu.Data;
using YuLinTu.Library.Entity;
using YuLinTu.Library.WorkStation;

namespace YuLinTu.Library.Business
{
    /// <summary>
    /// 导出无异议声明书任务类
    /// </summary>
    public class TaskExportIdeaBookOperation : Task
    {
        #region Ctor

        /// <summary>
        /// 构造函数
        /// </summary>
        public TaskExportIdeaBookOperation()
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
            TaskExportIdeaBookArgument argument = Argument as TaskExportIdeaBookArgument;
            if (argument == null)
            {
                return;
            }
            IDbContext dbContext = argument.DbContext;
            var zone = argument.CurrentZone;
            try
            {
                var listPerson = argument.SelectedPersons;
                if (listPerson == null || listPerson.Count == 0)
                {
                    this.ReportProgress(100, null);
                    this.ReportWarn(string.Format("{0}未获取承包方数据!", zone.FullName));
                    return;
                }
                bool canOpen = ExportIdeaBookWord(argument, listPerson);
                if (canOpen)
                    CanOpenResult = true;
            }
            catch (Exception ex)
            {
                YuLinTu.Library.Log.Log.WriteException(this, "TaskExportIdeaBookOperation(导出无异议声明书任务)", ex.Message + ex.StackTrace);
                this.ReportException(ex, "导出无异议声明书出现异常!");
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
        /// 导出数据到无异议声明书
        /// </summary>
        public bool ExportIdeaBookWord(TaskExportIdeaBookArgument argument, List<VirtualPerson> listPerson)
        {
            bool result = false;
            try
            {
                if (argument.CurrentZone == null)
                {
                    this.ReportError("未选择导出数据的地域!");
                    return result;
                }
                this.ReportProgress(1, "开始");
                string markDesc = GetMarkDesc(argument.CurrentZone, argument.DbContext);
                //string info = string.Format("{0}导出{1}张户主声明书", markDesc, listPerson.Count);
                string templatePath = TemplateHelper.WordTemplate(TemplateFile.VirtualPersonIdeaBook);
                var zoneStation = argument.DbContext.CreateZoneWorkStation();
                string zoneName = zoneStation.GetZoneName(argument.CurrentZone);
                int index = 1;
                double vpPercent = 99 / (double)listPerson.Count;
                openFilePath = argument.FileName;
                //var zoneList = GetParents(argument.CurrentZone, argument.DbContext);
                foreach (VirtualPerson family in listPerson)
                {
                    //确定是否导出集体户信息(利用配置文件)
                    if (argument.FamilyOtherSet.ShowFamilyInfomation && (family.Name.IndexOf("机动地") >= 0 || family.Name.IndexOf("集体") >= 0))
                    {
                        continue;
                    }
                    string familyNuber = ToolString.ExceptSpaceString(family.FamilyNumber);
                    ExportIdeaBook exportDelegate = new ExportIdeaBook(family);
                    exportDelegate.Date = argument.Time == null ? DateTime.Now : argument.Time;
                    exportDelegate.PubDate = argument.PubTime == null ? DateTime.Now : argument.PubTime;
                    exportDelegate.ZoneName = zoneName;
                    exportDelegate.RightName = InitalizeRightType(argument.VirtualType);
                    exportDelegate.LandAliseName = InitalizeLandRightName(argument.VirtualType);
                    exportDelegate.OpenTemplate(templatePath);
                    exportDelegate.SaveAs(family, openFilePath + @"\" + familyNuber + "-" + family.Name + "-" + TemplateFile.VirtualPersonIdeaBook + ".doc");
                    this.ReportProgress((int)(1 + vpPercent * index), markDesc + family.Name);
                    index++;
                }
                result = true;
                string info = string.Format("{0}导出{1}张无异议声明书", markDesc, index - 1);
                this.ReportInfomation(info);
                this.ReportProgress(100, "完成");
            }
            catch (Exception ex)
            {
                result = false;
                this.ReportError("导出无异议声明书失败");
                YuLinTu.Library.Log.Log.WriteException(this, "ExportIdeaBookWord(无异议声明书处理)", ex.Message + ex.StackTrace);
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
        /// 获取父级地域集合(县级)
        /// </summary>
        private List<Zone> GetParents(Zone zone, IDbContext dbContext)
        {
            List<Zone> zoneList = new List<Zone>();
            if (zone == null || (zone != null && zone.Level >= eZoneLevel.County) || dbContext == null)
            {
                return zoneList;
            }
            try
            {
                var zoneStation = dbContext.CreateZoneWorkStation();
                Zone temZone = zoneStation.Get(zone.UpLevelCode);
                zoneList.Add(temZone);
                while (temZone.Level < eZoneLevel.County)
                {
                    temZone = zoneStation.Get(temZone.UpLevelCode);
                    zoneList.Add(temZone);
                }
            }
            catch (Exception ex)
            {
                YuLinTu.Library.Log.Log.WriteException(this, "GetParents(获取父级地域集合)", ex.Message + ex.StackTrace);
            }
            return zoneList;
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

        /// <summary>
        /// 初始化权属类型
        /// </summary>
        private string InitalizeRightType(eVirtualType virtualType)
        {
            string templateName = "农村土地承包经营权";
            switch (virtualType)
            {
                case eVirtualType.Land:
                    templateName = "农村土地承包经营权";
                    break;
                case eVirtualType.Yard:
                    templateName = "集体建设用地使用权";
                    break;
                case eVirtualType.House:
                    templateName = "房屋所有权";
                    break;
                case eVirtualType.Wood:
                    templateName = "林权";
                    break;
                default:
                    break;
            }
            return templateName;
        }

        /// <summary>
        /// 初始化权属地块类型
        /// </summary>
        private string InitalizeLandRightName(eVirtualType virtualType)
        {
            string templateName = "承包地田块";
            switch (virtualType)
            {
                case eVirtualType.Land:
                    templateName = "承包地田块";
                    break;
                case eVirtualType.Yard:
                    templateName = "建设用地";
                    break;
                case eVirtualType.House:
                    templateName = "房屋";
                    break;
                case eVirtualType.Wood:
                    templateName = "林地";
                    break;
                default:
                    break;
            }
            return templateName;
        }

        #endregion

        #region Method—Helper

        #endregion
    }
}
