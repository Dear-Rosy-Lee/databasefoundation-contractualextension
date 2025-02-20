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
    /// 导出单户确认表任务类
    /// </summary>
    public class TaskExportFamilyConfirmOperation : Task
    {
        #region Ctor

        /// <summary>
        /// 构造函数
        /// </summary>
        public TaskExportFamilyConfirmOperation()
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
            TaskExportFamilyConfirmArgument argument = Argument as TaskExportFamilyConfirmArgument;
            if (argument == null)
            {
                return;
            }
            IDbContext dbContext = argument.DbContext;
            var zone = argument.CurrentZone;
            try
            {
                var listPerson = argument.SelectedPersons;
                var concordStation = dbContext.CreateConcordStation();
                var bookStation = dbContext.CreateRegeditBookStation();
                var landStation = dbContext.CreateContractLandWorkstation();
                var listConcord = concordStation.GetContractsByZoneCode(zone.FullCode, eLevelOption.Self);
                var listBook = bookStation.GetContractsByZoneCode(zone.FullCode, eLevelOption.Self);
                if (listPerson == null || listPerson.Count == 0)
                {
                    this.ReportProgress(100, null);
                    this.ReportWarn(string.Format("{0}未获取承包方数据!", zone.FullName));
                    return;
                }
                List<Guid> listId = new List<Guid>();
                listPerson.ForEach(c => listId.Add(c.ID));
                var listLand = landStation.GetCollection(listId);
                if (listConcord == null || listConcord.Count == 0)
                {
                    this.ReportProgress(100, null);
                    this.ReportWarn(string.Format("{0}未获取承包合同数据!", zone.FullName));
                    return;
                }
                if (listBook == null || listBook.Count == 0)
                {
                    this.ReportProgress(100, null);
                    this.ReportWarn(string.Format("{0}未获取承包权证数据!", zone.FullName));
                    return;
                }
                listLand.LandNumberFormat(argument.SystemDefine);
                bool canOpen = ExportFamilyConfirm(argument, listPerson, listLand, listConcord, listBook);
                if (canOpen)
                    CanOpenResult = true;
            }
            catch (Exception ex)
            {
                YuLinTu.Library.Log.Log.WriteException(this, "TaskExportFamilyConfirmOperation(导出单户确认表任务)", ex.Message + ex.StackTrace);
                this.ReportException(ex, "导出单户确认表出现异常!");
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
        /// 导出单户确认表
        /// </summary>
        public bool ExportFamilyConfirm(TaskExportFamilyConfirmArgument argument, List<VirtualPerson> listPerson, List<ContractLand> listLand,
            List<ContractConcord> listConcord, List<ContractRegeditBook> listBook)
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
                var zoneStation = argument.DbContext.CreateZoneWorkStation();
                //string unitName = zoneStation.GetZoneName(argument.CurrentZone);
                string villageName = zoneStation.GetVillageName(argument.CurrentZone);
                string unitName = GetZoneName(argument.CurrentZone, zoneStation);
                string titleName = GetTitle(argument.CurrentZone, zoneStation);
                string excelName = GetMarkDesc(argument.CurrentZone, argument.DbContext);
                string templatePath = TemplateHelper.ExcelTemplate(TemplateFile.SingleFamilyConfirmTable);  //获取模板  
                double tempPercent = 99 / (double)listPerson.Count;  //计算百分比
                int index = 1;  //导出标记 
                openFilePath = argument.FileName;
                foreach (var person in listPerson)
                {
                    ExportSingleFamilyConfirmTable confirmExport = new ExportSingleFamilyConfirmTable(argument.DbContext);
                    confirmExport.CurrentZone = argument.CurrentZone;
                    confirmExport.ListLand = listLand == null ? new List<ContractLand>() : listLand;
                    confirmExport.ListConcord = listConcord == null ? new List<ContractConcord>() : listConcord;
                    confirmExport.ListRegeditBook = listBook == null ? new List<ContractRegeditBook>() : listBook;
                    //confirmExport.UnitName = argument.SystemDefine.CountryTableHead ? villageName : unitName;
                    //confirmExport.TitleName = titleName;
                    confirmExport.UnitName = argument.SystemDefine.GetTableHeaderStr(argument.CurrentZone);
                    confirmExport.TitleName = argument.SystemDefine.GetTBDWStr(argument.CurrentZone);
                    confirmExport.StatuDes = excelName + "单户确认表";
                    confirmExport.SaveFilePath = openFilePath + @"\" + person.FamilyNumber + "-" + person.Name + "-" + "单户确认表" + ".xls";
                    //confirmExport.PostProgressEvent += export_PostProgressEvent;
                    //confirmExport.PostErrorInfoEvent += export_PostErrorEvent;
                    confirmExport.BeginToVirtualPerson(person, templatePath);   //开始导表
                    this.ReportProgress((int)(1 + tempPercent * index), string.Format("{0}", excelName + person.Name));
                    index++;
                }
                string str = string.Format("{0}共导出{1}张单户确认表", excelName, index - 1);
                this.ReportProgress(100, null);
                this.ReportInfomation(str);
                result = true;

                listLand.Clear();
                listLand = null;
                listConcord.Clear();
                listConcord = null;
                listBook.Clear();
                listBook = null;
                GC.Collect();
            }
            catch (Exception ex)
            {
                result = false;
                this.ReportError("导出单户确认表失败!");
                YuLinTu.Library.Log.Log.WriteException(this, "ExportFamilyConfirmVolumn(批量导出单户确认表到Excel)", ex.Message + ex.StackTrace);
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

        /// <summary>
        /// 获取标题
        /// </summary>
        private string GetTitle(Zone currentZone, IZoneWorkStation zoneStation)
        {
            if (currentZone != null && currentZone.FullCode.Length > 0)
            {
                Zone county = zoneStation.Get(currentZone.FullCode.Substring(0, Zone.ZONE_COUNTY_LENGTH));
                Zone city = zoneStation.Get(currentZone.FullCode.Substring(0, Zone.ZONE_CITY_LENGTH));
                if (city != null && county != null)
                {
                    string zoneName = county.FullName.Replace(city.FullName, "");
                    return city.Name + zoneName.Substring(0, zoneName.Length - 1);
                }
                return currentZone.Name;
            }
            return "";
        }

        /// <summary>
        /// 获取单位名称
        /// </summary>   
        private string GetZoneName(Zone zone, IZoneWorkStation zoneStation)
        {
            Zone county = zoneStation.Get(zone.FullCode.Substring(0, Zone.ZONE_COUNTY_LENGTH));
            if (county == null)
            {
                return zone.FullName;
            }
            return zone.FullName.Replace(county.FullName, "");
        }

        #endregion

        #region Method—Helper

        /// <summary>
        /// 错误信息报告
        /// </summary>
        /// <param name="message"></param>
        private void ReportInfo(object sender, TaskAlertEventArgs e)
        {
            if (e != null)
            {
                this.ReportAlert(e.Grade, e.UserState, e.Description);
            }
        }

        /// <summary>
        /// 进度报告
        /// </summary>
        private void ReportPercent(object sender, TaskProgressChangedEventArgs e)
        {
            if (e != null)
            {
                this.ReportProgress(e.Percent, e.UserState);
            }
        }

        /// <summary>
        ///  进度报告
        /// </summary>
        private void export_PostProgressEvent(int progress, object userState)
        {
            this.ReportProgress(progress, userState);
        }

        /// <summary>
        /// 信息报告
        /// </summary>
        private void export_PostErrorEvent(string error)
        {
            this.ReportError(error);
        }

        #endregion
    }
}
