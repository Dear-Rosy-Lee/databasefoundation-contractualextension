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
using System.Collections;
using YuLinTu.Data.Dynamic;
using YuLinTu.Data.Shapefile;
using YuLinTu.Library.Office;

namespace YuLinTu.Library.Business
{
    /// <summary>
    /// 导出单户调查表
    /// </summary>
    public class ExportLandSingleSurveyTableOperation : Task
    {
        #region Fields
        private bool returnValue;
        private string openFilePath;  //打开文件路径
        private string fileName;
        private SystemSetDefine SystemSetDefine = SystemSetDefine.GetIntence();
        #endregion

        #region Properties


        /// <summary>
        /// 导出公示调查表日期设置
        /// </summary>
        public DateSetting PublishDateSetting { get; set; }

       


        /// <summary>
        /// 是否批量
        /// </summary>
        public bool IsBatch { get; set; }

        /// <summary>
        /// 当前被选中的承包方
        /// </summary>
        public List<VirtualPerson> SelectContractor { get; set; }

        /// <summary>
        /// 数据库服务上下文
        /// </summary>
        public IDbContext dbContext { get; set; }

        #endregion

        #region Ctor

        /// <summary>
        /// 构造函数
        /// </summary>
        public ExportLandSingleSurveyTableOperation()
        {
        }

        #endregion

        #region Methods - Override

        /// <summary>
        /// 开始操作
        /// </summary>
        protected override void OnGo()
        {
            returnValue = false;
            ExportLandSingleSurveyTableArgument metadata = Argument as ExportLandSingleSurveyTableArgument;
            if (metadata == null)
            {
                return;
            }
            fileName = metadata.FileName;
            bool isClear = metadata.IsClear;
            dbContext = metadata.Database;
            Zone zone = metadata.CurrentZone;
            SelectContractor = metadata.SelectContractor;
            openFilePath = metadata.FileName;
            AccountLandBusiness landBusiness = new AccountLandBusiness(dbContext);
            landBusiness.VirtualType = metadata.VirtualType;
            landBusiness.Alert += ReportInfo;
            landBusiness.ProgressChanged += ReportPercent;
            landBusiness.TableType = metadata.TableType;
            landBusiness.ArgType = metadata.ArgType;
            landBusiness.IsBatch = metadata.IsBatch;
            List<Zone> childrenZone = metadata.SelfAndSubsZones;   //子级地域集合
            Zone parent = landBusiness.GetParent(zone);
            switch (metadata.ArgType)
            {
                case eContractAccountType.ExportSingleFamilySurveyExcel:
                    if (zone.Level >= eZoneLevel.Group && (SelectContractor == null || SelectContractor.Count == 0))
                    {
                        //批量导出承包台账单户确认表
                        ExportSingleFamilyExcelByZone(zone, childrenZone, parent, fileName, landBusiness);
                    }
                    else if (SelectContractor != null && SelectContractor.Count > 0)
                    {
                        //导出当前地域下的承包台账单户调查表
                        ExportSingleFamilySurveyExcel(zone, SelectContractor, fileName);
                    }
                    break;
            }
            if (returnValue)
            {
                CanOpenResult = true;
            }
            GC.Collect();
        }

        /// <summary>
        /// 打开文件
        /// </summary>
        public override void OpenResult()
        {
            if (string.IsNullOrEmpty(fileName) || !Directory.Exists(fileName))
                return;
            System.Diagnostics.Process.Start(fileName);
        }

        protected override void OnStopped()
        {
            base.OnStopped();
            GC.Collect();
        }

        protected override void OnTerminate(Exception ex)
        {
            base.OnTerminate(ex);
            GC.Collect();
        }

        #endregion

        #region 公用之获取全部地域及创建文件目录

        /// <summary>
        /// 获取全部的地域
        /// </summary>
        /// <param name="currentZone">当前地域</param>
        /// <param name="childrenZone">子级地域</param>
        /// <param name="parentZone">父级地域</param>
        /// <param name="fileName">保存路径</param>
        /// <param name="business">合同业务</param>
        public List<Zone> GetAllZones(Zone currentZone, List<Zone> childrenZone, Zone parentZone, AccountLandBusiness business)
        {
            List<Zone> allZones = new List<Zone>();
            allZones.Add(currentZone);
            if (currentZone.Level == eZoneLevel.Group)
            {
                //选择为组
                allZones.Add(parentZone);
                allZones.Add(business.GetParent(parentZone));
            }
            else if (currentZone.Level == eZoneLevel.Village)
            {
                //选择为村
                foreach (var child in childrenZone)
                {
                    allZones.Add(child);
                }
                allZones.Add(parentZone);
            }
            else if (currentZone.Level == eZoneLevel.Town)
            {
                //选择为镇
                foreach (var child in childrenZone)
                {
                    allZones.Add(child);
                    List<Zone> zones = business.GetChildrenZone(child);
                    foreach (var zone in zones)
                    {
                        allZones.Add(zone);
                    }
                }
            }
            return allZones;
        }

        /// <summary>
        /// 创建文件目录(可以创建至组)
        /// </summary>
        /// <param name="allZones">全部地域</param>
        /// <param name="cZone">当前地域</param>
        private string CreateDirectory(List<Zone> allZones, Zone cZone)
        {
            string folderString = cZone.Name;
            Zone z = cZone;
            while (z.Level < eZoneLevel.County)
            {
                z = allZones.Find(t => t.FullCode == z.UpLevelCode);
                if (z != null)
                    folderString = z.Name + @"\" + folderString;
                else
                    break;
            }
            return folderString;
        }

        /// <summary>
        /// 创建文件目录(仅创建至村)
        /// </summary>
        /// <param name="allZones">全部地域</param>
        /// <param name="cZone">当前地域</param>
        private string CreateDirectoryByVilliage(List<Zone> allZones, Zone cZone)
        {
            string folderString = cZone.Level == eZoneLevel.Group ? "" : cZone.Name;
            Zone z = cZone;
            while (z.Level < eZoneLevel.County)
            {
                z = allZones.Find(t => t.FullCode == z.UpLevelCode);
                if (z != null)
                    folderString = z.Name + @"\" + folderString;
                else
                    break;
            }
            return folderString;
        }

        #endregion

        #region Methods - Privates-承包台账导出

        /// <summary>
        /// 批量导出单户调查表-进度处理一致
        /// </summary>
        /// <param name="currentZone">当前地域</param>
        /// <param name="childrenZone">子级地域</param>
        /// <param name="parentZone">父级地域</param>
        /// <param name="savePath">保存文件路径</param>
        /// <param name="business">台账业务</param>  
        /// <param name="exportType">导出类型</param>         
        private void ExportSingleFamilyExcelByZone(Zone currentZone, List<Zone> childrenZone, Zone parentZone, string savePath, AccountLandBusiness business)
        {
            if (currentZone == null)
            {
                this.ReportError("未选择导出数据的地域!");
                return;
            }
            ExportLandSingleSurveyTableArgument metadata = Argument as ExportLandSingleSurveyTableArgument;
            List<Zone> allZones = metadata.AllZones;

            business.ProgressChanged -= ReportPercent;
            this.ReportProgress(0, "开始");
            double percent = 0.0;
            percent = 99.0 / (double)metadata.SelfAndSubsZones.Count;
            double subpercent = 0.0;
            int zoneCount = 0;
            int tableCount = 0;      //统计可导出表的个数      
            business.VirtualType = eVirtualType.Land;

            string desc = ExportZoneListDir(currentZone, metadata.AllZones);// 优化
            List<VirtualPerson> personlists = new List<VirtualPerson>();
            personlists = business.GetByZone(currentZone.FullCode);
            string path = metadata.FileName;
            subpercent = percent * (zoneCount++);
            this.ReportProgress((int)(subpercent), string.Format("{0}", desc));
            double temppercent = percent / (double)(personlists.Count == 0 ? 1 : personlists.Count);
            int index = 1;
            foreach (var exportPerson in personlists)
            {
                business.ExportSingleFamilySurveyExcel(currentZone, exportPerson, path);
                this.ReportProgress((int)(subpercent + temppercent * index), string.Format("导出{0}", desc + exportPerson.Name));
                index++;
                tableCount++;
            }
            string info = personlists.Count == 0 ? string.Format("在{0}下未获取数据", currentZone.FullName) : string.Format("在{0}下成功导出{1}条数据", currentZone.FullName, personlists.Count);

            if (personlists.Count == 0)
            {
                this.ReportWarn(info);
                this.ReportProgress(100, "完成");
                return;
            }
            else
            {
                returnValue = true;
                this.ReportInfomation(info);
            }

            this.ReportProgress(100, "完成");
            this.ReportInfomation(string.Format("共导出{0}条表信息", tableCount));
            allZones = null;
        }

        #endregion

        /// <summary>
        /// 导出数据到Excel表-承包台账单户调查报表单个导出
        /// </summary>
        public void ExportSingleFamilySurveyExcel(Zone zone, List<VirtualPerson> selectVirtualPerson, string fileName)
        {
            try
            {
                ExportLandSingleSurveyTableArgument metadata = Argument as ExportLandSingleSurveyTableArgument;

                if (selectVirtualPerson == null)
                {
                    this.ReportError("未选择导出数据的承包方!");
                    return;
                }
                string excelName = GetMarkDesc(zone);
                var tablePersonStation = dbContext.CreateVirtualPersonStation<TableVirtualPerson>();
                List<VirtualPerson> tableVps = tablePersonStation.GetByZoneCode(zone.FullCode);
                this.ReportProgress(0, "开始");
                string reInfo = string.Format("从{0}下成功导出{1}条承包方数据!", excelName, selectVirtualPerson.Count.ToString());
                string tempPath = TemplateHelper.ExcelTemplate(TemplateFile.ContractLandSingleSurveyExceltemp);
                string zoneName = GetUinitName(zone);
                var zoneStation = dbContext.CreateZoneWorkStation();
                if (SystemSetDefine.CountryTableHead)
                {                   
                    zoneName = zoneStation.GetVillageName(zone);
                }
                int percent = (100 / selectVirtualPerson.Count);
                this.ReportProgress(10, "正在获取数据");
                int percentIndex = 1;
                SecondTableLandBusiness secondlandbus = new SecondTableLandBusiness(dbContext);
                List<Zone> allZones = zoneStation.GetAllZones(zone);
                string savePath = CreateDirectoryHelper.CreateDirectory(allZones, zone);
                fileName = fileName + @"\" + savePath;
                foreach (var item in selectVirtualPerson)
                {
                    var landStation = dbContext.CreateContractLandWorkstation();
                    List<ContractLand> landArrays = landStation.GetCollection(item.ID);
                    landArrays.LandNumberFormat(SystemSetDefine);
                    List<SecondTableLand> tableLandArrays = secondlandbus.GetCollection(item.ID);

                    string filePath = string.Empty;
                    int familyNumber = 0;
                    Int32.TryParse(item.FamilyNumber, out familyNumber);
                    this.ReportProgress(10 + percent * percentIndex, item.Name);
                    ++percentIndex;
                    using (ExportContractorLandSingleSurveyTable export = new ExportContractorLandSingleSurveyTable())
                    {
                        export.SaveFilePath = fileName + @"\" + familyNumber + "-" + item.Name + "-" + "单户调查表" + ".xls";
                        export.CurrentZone = zone;
                        export.TableFamilys = tableVps;
                        export.ShowValue = false;
                        if (selectVirtualPerson.Count == 1)
                        {
                            export.ShowValue = true;
                        }
                        export.UnitName = zoneName;
                        export.TableLandArrays = tableLandArrays;
                        export.DictionList = metadata.DictList;
                        export.LandArrays = landArrays;
                        export.Contractor = item;
                        export.PostErrorInfoEvent += export_PostErrorInfoEvent;
                        returnValue = export.BeginToVirtualPerson(item, tempPath);
                        openFilePath = export.SaveFilePath;
                    }
                }
                this.ReportProgress(100, "完成");
                this.ReportInfomation(reInfo);
            }
            catch (Exception ex)
            {
                this.ReportError(ex.Message);
                YuLinTu.Library.Log.Log.WriteException(this, "ExportDataExcel(导出数据到Excel表)", ex.Message + ex.StackTrace);
            }
        }

        #region 辅助功能

        private List<CollectivityTissue> GetTissueCollection(Zone zone)
        {
            string messageName = SenderMessage.SENDER_GETDATA;
            ModuleMsgArgs args = new ModuleMsgArgs();
            args.Name = messageName;
            args.Parameter = zone.FullCode;
            args.Datasource = DataBaseSource.GetDataBaseSource();
            TheBns.Current.Message.Send(this, args);
            List<CollectivityTissue> list = args.ReturnValue as List<CollectivityTissue>;
            return list;
        }

        /// <summary>
        /// 进度提示用，导出时获取当前地域的上级地域名称路径到镇级
        /// </summary>       
        private string ExportZoneListDir(Zone zone, List<Zone> allZones)
        {
            string exportzonedir = string.Empty;
            if (zone.Level == eZoneLevel.Group)
            {
                Zone vzone = allZones.Find(t => t.FullCode == zone.UpLevelCode);
                Zone tzone = allZones.Find(t => t.FullCode == vzone.UpLevelCode);
                exportzonedir = tzone.Name + vzone.Name + zone.Name;
            }
            else if (zone.Level == eZoneLevel.Village)
            {
                Zone tzone = allZones.Find(t => t.FullCode == zone.UpLevelCode);
                exportzonedir = tzone.Name + zone.Name;
            }
            else if (zone.Level == eZoneLevel.Town)
            {
                exportzonedir = zone.Name;
            }
            return exportzonedir;
        }

        /// <summary>
        /// 获取单位名称
        /// </summary>
        public string GetUinitName(Zone zone)
        {
            ModuleMsgArgs arg = new ModuleMsgArgs();
            arg.Datasource = dbContext;
            arg.Parameter = zone;
            arg.Name = VirtualPersonMessage.VIRTUALPERSON_UNITNAME;
            TheBns.Current.Message.Send(this, arg);
            return arg.ReturnValue.ToString();
        }

        /// <summary>
        /// 根据当前地域获得任务描述信息
        /// </summary>
        private string GetMarkDesc(Zone zone)
        {
            Zone parent = GetParent(zone);  //获取上级地域
            string excelName = string.Empty;
            if (zone.Level == eZoneLevel.Town)
            {
                excelName = zone.Name;
            }
            else if (zone.Level == eZoneLevel.Village)
            {
                excelName = parent.Name + zone.Name;
            }
            else if (zone.Level == eZoneLevel.Group)
            {
                Zone parentTowm = GetParent(parent);
                excelName = parentTowm.Name + parent.Name + zone.Name;
            }
            return excelName;
        }


        /// <summary>
        /// 获取上级地域
        /// </summary>
        /// <param name="zone"></param>
        /// <returns></returns>
        public Zone GetParent(Zone zone)
        {
            ModuleMsgArgs arg = new ModuleMsgArgs();
            arg.Datasource = dbContext;
            arg.Parameter = zone;
            arg.Name = ZoneMessage.ZONE_PARENT_ZONE;
            TheBns.Current.Message.Send(this, arg);
            return (arg.ReturnValue as Zone);
        }
        #endregion

        #region  提示信息

        /// <summary>
        /// 判断当前地域下有没有承包方信息
        /// </summary>
        private bool ExitsPerson(Zone zone)
        {
            bool exsit = false;
            AccountLandBusiness business = new AccountLandBusiness(dbContext);
            business.VirtualType = eVirtualType.Land;
            List<VirtualPerson> listPerson = business.GetByZone(zone.FullCode);
            if (listPerson != null && listPerson.Count() > 0)
            {
                exsit = true;
            }
            return exsit;
        }


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
        /// <param name="progress"></param>
        private void ReportPercent(object sender, TaskProgressChangedEventArgs e)
        {
            if (e != null)
            {
                this.ReportProgress(e.Percent, e.UserState);
            }
        }

        /// <summary>
        ///  错误信息报告
        /// </summary>
        private void export_PostErrorInfoEvent(string message)
        {
            if (!string.IsNullOrEmpty(message))
            {
                this.ReportError(message);
            }
        }

        /// <summary>
        /// 报告进度
        /// </summary>
        private void export_PostProgressEvent(int progress, object userState)
        {
            this.ReportProgress(progress, userState);
        }

        #endregion
    }
}
