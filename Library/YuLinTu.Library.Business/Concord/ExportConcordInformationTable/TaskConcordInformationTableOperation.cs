/*
 * (C) 2015  鱼鳞图公司版权所有,保留所有权利 
 */
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using YuLinTu;
using YuLinTu.Data;
using YuLinTu.Library.Entity;

namespace YuLinTu.Library.Business
{
    /// <summary>
    /// 合同明细汇总表
    /// </summary>
    public class TaskConcordInformationTableOperation : Task
    {
        #region Fields

        private bool returnValue;
        private string openFilePath;  //打开文件路径

        #endregion

        #region Properties

        /// <summary>
        /// 当前选择的承包方
        /// </summary>
        public VirtualPerson Contractor { get; set; }

        /// <summary>
        /// 数据库
        /// </summary>
        public IDbContext DbContext { get; set; }

        /// <summary>
        /// 选中的承包方集合
        /// </summary>
        public List<VirtualPerson> ListPerson { get; set; }


        #endregion

        #region Ctor

        /// <summary>
        /// 构造函数
        /// </summary>
        public TaskConcordInformationTableOperation()
        {
        }

        #endregion

        #region Override

        /// <summary>
        /// 开始操作
        /// </summary>
        protected override void OnGo()
        {
            TaskConcordInformationTableArgument metadata = Argument as TaskConcordInformationTableArgument;
            if (metadata == null)
            {
                return;
            }
            returnValue = false;
            DbContext = metadata.Database;
            string fileName = metadata.FileName;
            openFilePath = metadata.FileName;
            Zone zone = metadata.CurrentZone;
            ConcordBusiness business = new ConcordBusiness(DbContext);
            business.ConcordsModified = metadata.ConcordsModified;
            business.LandsOfInitialConcord = metadata.LandsOfInitialConcord;
            business.Sender = metadata.Sender;
            business.SystemSet = metadata.SystemSet;
            business.ArgType = eContractAccountType.ExportConcordInformationTable;  //传入合同明细表参数
            business.Alert += ReportInfo;
            business.ProgressChanged += ReportPercent;
            business.PublishDateSetting = metadata.PublishDateSetting;
            List<Zone> childrenZone = metadata.SelfAndSubsZones;   //子级地域集合
            Zone parent = business.GetParent(zone);
            ExportConcordInformation(zone, childrenZone, parent, fileName, business);
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
            System.Diagnostics.Process.Start(openFilePath);
            base.OpenResult();
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
        public List<Zone> GetAllZones(Zone currentZone, List<Zone> childrenZone, Zone parentZone, string fileName, ConcordBusiness business)
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

        #region 导出合同明细表

        /// <summary>
        /// 导出合同明细表
        /// </summary>
        /// <param name="currentZone">当前地域</param>
        /// <param name="childrenZone">子级地域</param>
        /// <param name="parentZone">父级地域</param>
        /// <param name="savePath">保存文件路径</param>
        /// <param name="business">合同业务</param>
        private void ExportConcordInformation(Zone currentZone, List<Zone> childrenZone, Zone parentZone, string savePath, ConcordBusiness business)
        {
            TaskConcordInformationTableArgument metadata = Argument as TaskConcordInformationTableArgument;
            if (currentZone == null)
            {
                this.ReportError("未选择导出数据的地域!");
                return;
            }

            business.ProgressChanged -= ReportPercent;
            this.ReportProgress(1, "开始");
            this.ReportProgress(5, "正在获取合同");

            List<Zone> zones = metadata.AllZones;  //存在合同数据的地域集合
            double percent = 95 / (double)metadata.SelfAndSubsZones.Count;
            int indexOfZone = 0;  //地域索引

            string descZone = ExportZoneListDir(currentZone, zones);
            string path = metadata.FileName;
            var concordStation = metadata.Database.CreateConcordStation();
            List<ContractConcord> listConcord = concordStation.GetContractsByZoneCode(currentZone.FullCode, eLevelOption.Self, true);
            this.ReportProgress((int)(5 + percent * indexOfZone), string.Format("{0}", descZone));
            if (listConcord != null && listConcord.Count > 0)
            {
                Directory.CreateDirectory(path);  //有数据则建立文件夹
                ExportConcordInfoTable(currentZone, path, percent, 5 + percent * indexOfZone);
                indexOfZone++;
            }

            if ((currentZone.Level == eZoneLevel.Town || currentZone.Level == eZoneLevel.Village) && listConcord.Count == 0)
            {
                //在镇、村下没有数据(提示信息不显示)
                this.ReportWarn(string.Format("{0}无合同数据", descZone));
                this.ReportProgress(100, "完成");
                return;
            }
            if (currentZone.Level == eZoneLevel.Group && listConcord.Count == 0)
            {
                //地域下无数据
                this.ReportWarn(string.Format("{0}无合同数据", descZone));
                this.ReportProgress(100, "完成");
                return;
            }

            if (listConcord.Count > 0)
            {
                //地域下有数据
                this.ReportInfomation(string.Format("{0}导出{1}条合同明细数据", descZone, listConcord.Count));
            }

            this.ReportProgress(100, "完成");
            this.ReportInfomation(string.Format("共导出{0}个合同明细表", indexOfZone));
        }


        /// <summary>
        /// 导出合同明细表
        /// </summary>
        public void ExportConcordInfoTable(Zone zone, string fileName, double averagePercent = 0.0, double currentPercent = 0.0)
        {
            TaskConcordInformationTableArgument metadata = Argument as TaskConcordInformationTableArgument;
            try
            {
                if (zone == null)
                {
                    this.ReportError("未选择导出数据的地域!");
                    return;
                }
                var vpStation = DbContext.CreateVirtualPersonStation<LandVirtualPerson>();
                var landStation = DbContext.CreateContractLandWorkstation();
                var concordStation = DbContext.CreateConcordStation();
                string excelName = GetMarkDesc(zone);
                List<VirtualPerson> vps = vpStation.GetByZoneCode(zone.FullCode);
                List<ContractConcord> concords = concordStation.GetByZoneCode(zone.FullCode);
                List<VirtualPerson> usevps = new List<VirtualPerson>();
                concords.ForEach(cc =>
                {
                    var usevp = vps.Find(vp => vp.ID == cc.ContracterId);
                    if (usevp != null && usevps.Contains(usevp) == false)
                    {
                        usevps.Add(usevp);
                    }
                });
                List<ContractLand> lands = landStation.GetCollection(zone.FullCode);
                lands.LandNumberFormat(metadata.SystemSet);
                string tempPath = TemplateHelper.ExcelTemplate(TemplateFile.ContractSummaryExcel);

                using (ExportContractorSummaryExcel concordInfoExport = new ExportContractorSummaryExcel(DbContext))
                {
                    string savePath = fileName + @"\" + excelName + "合同明细表" + ".xls";
                    concordInfoExport.SaveFilePath = savePath;
                    concordInfoExport.CurrentZone = zone;
                    concordInfoExport.ListPerson = usevps;
                    concordInfoExport.ListConcord = concords;
                    concordInfoExport.ListLand = lands;
                    concordInfoExport.UnitName = metadata.SystemSet.GetTableHeaderStr(zone);
                    concordInfoExport.StatuDes = excelName + "合同明细表";
                    concordInfoExport.Percent = averagePercent;
                    concordInfoExport.CurrentPercent = currentPercent;
                    concordInfoExport.ZoneDesc = excelName;
                    concordInfoExport.ArgType = eContractAccountType.ExportConcordInformationTable;  //传入合同明细表参数;
                    if (metadata.SummaryDefine.WarrantNumber)
                    {
                        metadata.SummaryDefine.WarrantNumber = false;//合同明细表不导出权证编码
                        metadata.SummaryDefine.ColumnCount = metadata.SummaryDefine.ColumnCount - 1;
                    }
                    concordInfoExport.PostErrorInfoEvent += export_PostErrorInfoEvent;
                    returnValue = concordInfoExport.BeginExcel(tempPath);//开始导表 
                    if (metadata.IsShow)
                        concordInfoExport.PrintView(savePath);
                }
                vps = null;
                usevps = null;
                concords = null;
                lands = null;
                GC.Collect();
            }
            catch (Exception ex)
            {
                this.ReportError(ex.Message);
                YuLinTu.Library.Log.Log.WriteException(this, "ExportConcordInfoTable(导出合同明细表到Excel)", ex.Message + ex.StackTrace);
            }
        }

        #endregion

        #region 辅助功能

        /// <summary>
        /// 判断当前地域下有没有承包方信息
        /// </summary>
        private bool ExitsPerson(Zone zone)
        {
            bool exsit = false;
            AccountLandBusiness business = new AccountLandBusiness(this.DbContext);
            List<VirtualPerson> listPerson = business.GetByZone(zone.FullCode);
            if (listPerson != null && listPerson.Count() > 0)
            {
                exsit = true;
            }
            return exsit;
        }

        /// <summary>
        /// 获取单位名称
        /// </summary>
        public string GetUinitName(Zone zone)
        {
            ModuleMsgArgs arg = new ModuleMsgArgs();
            arg.Datasource = DbContext;
            arg.Parameter = zone;
            arg.Name = VirtualPersonMessage.VIRTUALPERSON_UNITNAME;
            TheBns.Current.Message.Send(this, arg);
            return arg.ReturnValue.ToString();
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
            if (zone.Level == eZoneLevel.Village)
            {
                Zone tzone = allZones.Find(t => t.FullCode == zone.UpLevelCode);
                exportzonedir = tzone.Name + zone.Name;
            }
            return exportzonedir;
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
            arg.Datasource = DbContext;
            arg.Parameter = zone;
            arg.Name = ZoneMessage.ZONE_PARENT_ZONE;
            TheBns.Current.Message.Send(this, arg);
            return (arg.ReturnValue as Zone);
        }

        #endregion

        #region  提示信息

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

        #endregion
    }
}
