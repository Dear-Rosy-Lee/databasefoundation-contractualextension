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
using System.Collections;
using YuLinTu.Data.Dynamic;
using YuLinTu.Data.Shapefile;
using YuLinTu.Library.Office;
using YuLinTu.Library.WorkStation;
using YuLinTu.Windows;
using YuLinTu.Library.Business;

namespace YuLinTu.Component.FuSui
{
    /// <summary>
    /// 土地承包经营权单户确认表/村组公示表/签字表/登记公示表/台账调查表
    /// </summary>
    public class TaskContractAccountOperation : Task
    {
        #region Fields

        private bool returnValue;
        private string openFilePath;
        private SystemSetDefine SystemSetDefine = SystemSetDefine.GetIntence();//打开文件路径

        #endregion Fields

        #region Properties

        ///// <summary>
        ///// 承包台账导出配置
        ///// </summary>
        //public PublicityConfirmDefine ContractLandOutputSurveyDefine
        //{
        //    get { return PublicityConfirmDefine.GetIntence(); }
        //}

        ///// <summary>
        ///// 承包台账单户调查表设置
        ///// </summary>
        //public SingleFamilySurveyDefine SingleFamilySurveySetting
        //{
        //    get { return SingleFamilySurveyDefine.GetIntence(); }
        //}

        //private ContractBusinessSettingDefine _contractBusinessSettingDefine;

        /// <summary>
        /// 承包台账常规设置实体
        /// </summary>
        public ContractBusinessSettingDefine SettingDefine = ContractBusinessSettingDefine.GetIntence();

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

        public SystemSetDefine SystemSet = SystemSetDefine.GetIntence();

        #endregion Properties

        #region Ctor

        /// <summary>
        /// 构造函数
        /// </summary>
        public TaskContractAccountOperation()
        {
        }

        #endregion Ctor

        #region Methods - Override

        /// <summary>
        /// 开始操作
        /// </summary>
        protected override void OnGo()
        {
            returnValue = false;
            TaskAccountFiveTableArgument metadata = Argument as TaskAccountFiveTableArgument;
            if (metadata == null)
            {
                return;
            }
            string fileName = metadata.FileName;
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
                case eContractAccountType.ExportContractAccountExcel:
                    //导出4个表数据
                    ExportFourKindTable(zone, childrenZone, parent, fileName, landBusiness);
                    break;

                case eContractAccountType.ExportSingleFamilyConfirmExcel:
                    if (zone.Level >= eZoneLevel.Group && (SelectContractor == null || SelectContractor.Count == 0))
                    {
                        //批量导出承包台账单户确认表
                        ExportSingleFamilyExcelByZone(zone, childrenZone, parent, fileName, landBusiness);
                    }
                    else if (SelectContractor != null && SelectContractor.Count > 0)
                    {
                        //导出当前地域下的承包方单户确认表信息
                        ExportSingleFamilyConfirmExcel(zone, SelectContractor, fileName);
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
            System.Diagnostics.Process.Start(openFilePath);
            base.OpenResult();
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

        #endregion Methods - Override

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

        #endregion 公用之获取全部地域及创建文件目录

        #region Methods - Privates-承包台账导出

        /// <summary>
        /// 导出台账报表4个同底层表
        /// </summary>
        /// <param name="currentZone">当前地域</param>
        /// <param name="childrenZone">子级地域</param>
        /// <param name="parentZone">父级地域</param>
        /// <param name="savePath">保存文件路径</param>
        /// <param name="business">台账业务</param>
        private void ExportFourKindTable(Zone currentZone, List<Zone> childrenZone, Zone parentZone, string savePath, AccountLandBusiness business)
        {
            if (currentZone == null)
            {
                this.ReportError("未选择导出数据的地域!");
                return;
            }
            TaskAccountFiveTableArgument metadata = Argument as TaskAccountFiveTableArgument;
            //List<Zone> allZones = GetAllZones(currentZone, childrenZone, parentZone, business);
            List<Zone> allZones = metadata.AllZones;
            this.ReportProgress(1, "开始");
            this.ReportProgress(5, "正在获取表");

            double percent = 95 / (double)metadata.SelfAndSubsZones.Count;
            int indexOfZone = 0;  //地域索引
            int dataCount = 0;//有数据表个数

            string descZone = ExportZoneListDir(currentZone, metadata.AllZones);
            string path = metadata.FileName;
            var vpStation = metadata.Database.CreateVirtualPersonStation<LandVirtualPerson>();
            bool hasperson = false;
            if (vpStation.GetByZoneCode(currentZone.FullCode, eLevelOption.Self).Count > 0)
            {
                hasperson = true;
            }
            this.ReportProgress((int)(5 + percent * indexOfZone), string.Format("{0}", descZone));
            if (hasperson)//有数据则建立文件夹
            {
                Directory.CreateDirectory(path);
                ExportDataExcel(currentZone, path, percent, 5 + percent * (indexOfZone));
                this.ReportInfomation(string.Format("{0}导出{1}个调查表数据", descZone, 1));
                dataCount++;
            }
            indexOfZone++;
            if ((currentZone.Level == eZoneLevel.Town || currentZone.Level == eZoneLevel.Village) && !hasperson)
            {
                //在镇、村下没有数据(提示信息不显示)
                this.ReportWarn(string.Format("{0}无数据", descZone));
                this.ReportProgress(100, "完成");
            }
            if (currentZone.Level == eZoneLevel.Group && !hasperson)
            {
                //地域下无数据
                this.ReportWarn(string.Format("{0}无数据", descZone));
                this.ReportProgress(100, "完成");
            }

            this.ReportProgress(100, "完成");
            this.ReportInfomation(string.Format("共导出{0}个调查表", dataCount));
        }

        /// <summary>
        /// 批量导出单户确认表/单户调查表-进度处理一致
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
            TaskAccountFiveTableArgument metadata = Argument as TaskAccountFiveTableArgument;
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
                business.ExportSingleFamilyConfirmExcel(currentZone, exportPerson, path);
                this.ReportProgress((int)(subpercent + temppercent * index), string.Format("导出{0}", desc + exportPerson.Name));
                index++;
                tableCount++;
            }
            string info = personlists.Count == 0 ? string.Format("在{0}下未获取数据", currentZone.FullName) : string.Format("在{0}下成功导出{1}条数据", currentZone.FullName, personlists.Count);

            if (personlists.Count == 0)
            {
                this.ReportWarn(info);
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

        #endregion Methods - Privates-承包台账导出

        /// <summary>
        /// 导出数据到Excel表-承包台账五个报表导出
        /// </summary>
        public void ExportDataExcel(Zone zone, string fileName, double averagePercent = 0.0, double currentPercent = 0.0)
        {
            try
            {
                TaskAccountFiveTableArgument metadata = Argument as TaskAccountFiveTableArgument;
                if (zone == null)
                {
                    this.ReportError("未选择导出数据的地域!");
                    return;
                }
                string excelName = GetMarkDesc(zone);
                var vpStation = metadata.Database.CreateVirtualPersonStation<LandVirtualPerson>();
                List<VirtualPerson> vps = vpStation.GetByZoneCode(zone.FullCode);

                //判断是否显示集体信息-勾选常规设置后就设置为不显示
                if (SettingDefine.DisplayCollectUsingCBdata)
                {
                    //List<VirtualPerson> vpList = vps.FindAll(fm => (fm.Name.IndexOf("机动地") >= 0 || fm.Name.IndexOf("集体") >= 0));
                    //foreach (VirtualPerson vpn in vpList)
                    //{
                    //    vps.Remove(vpn);
                    //}
                    //vpList.Clear();
                    vps.RemoveAll(c => c.FamilyExpand.ContractorType != eContractorType.Farmer);
                }

                string fileType = "承包地块延包调查表.xls";

                string tempPath = TemplateHelper.ExcelTemplate(TemplateFile.ContractLandDelaySurveyExceltempFuSui);
                string zoneName = GetUinitName(zone);
                if (SystemSetDefine.CountryTableHead)
                {
                    var zoneStation = dbContext.CreateZoneWorkStation();
                    zoneName = zoneStation.GetVillageName(zone);
                }
                var landStation = dbContext.CreateContractLandWorkstation();
                List<ContractLand> landArrays = landStation.GetCollection(zone.FullCode);
                landArrays.LandNumberFormat(SystemSetDefine);
                var concordStation = dbContext.CreateConcordStation();
                var bookStation = dbContext.CreateRegeditBookStation();
                var listConcords = concordStation.GetContractsByZoneCode(zone.FullCode);
                var listBooks = bookStation.GetByZoneCode(zone.FullCode, eSearchOption.Precision);
                string filePath = string.Empty;
                ExportContractorSurveyExcel export = new ExportContractorSurveyExcel();

                #region 通过反射等机制定制化具体的业务处理类

                var temp = WorksheetConfigHelper.GetInstance(export);
                if (temp != null && temp.TemplatePath != null)
                {
                    if (temp is ExportContractorSurveyExcel)
                    {
                        export = (ExportContractorSurveyExcel)temp;
                    }
                    tempPath = Path.Combine(TheApp.GetApplicationPath(), temp.TemplatePath);
                }

                #endregion 通过反射等机制定制化具体的业务处理类

                string savePath = fileName + @"\" + excelName + fileType;
                export.SaveFilePath = savePath;
                export.CurrentZone = zone;
                export.Familys = vps;
                export.ExcelName = SystemSet.GetTBDWStr(zone);
                export.UnitName = SystemSet.GetTableHeaderStr(zone);
                export.TableType = metadata.TableType;
                export.DictionList = metadata.DictList;
                export.LandArrays = landArrays;
                export.Percent = averagePercent;
                export.CurrentPercent = currentPercent;
                export.ConcordCollection = listConcords;
                export.BookColletion = listBooks;
                export.PostProgressEvent += export_PostProgressEvent;
                export.PostErrorInfoEvent += export_PostErrorInfoEvent;
                returnValue = export.BeginExcel(zone.FullCode.ToString(), tempPath);
                if (metadata.IsShow)
                    export.PrintView(savePath);
            }
            catch (Exception ex)
            {
                this.ReportError(ex.Message);
                YuLinTu.Library.Log.Log.WriteException(this, "ExportDataExcel(导出数据到Excel表)", ex.Message + ex.StackTrace);
            }
        }

        /// <summary>
        /// 导出数据到Excel表-承包台账单户确认报表单个导出
        /// </summary>
        public void ExportSingleFamilyConfirmExcel(Zone zone, List<VirtualPerson> selectVirtualPerson, string fileName)
        {
            try
            {
                TaskAccountFiveTableArgument metadata = Argument as TaskAccountFiveTableArgument;

                if (selectVirtualPerson == null)
                {
                    this.ReportError("未选择导出数据的承包方!");
                    return;
                }
                string excelName = GetMarkDesc(zone);
                List<VirtualPerson> vps = new List<VirtualPerson>();
                this.ReportProgress(0, "开始");
                string reInfo = string.Format("从{0}下成功导出{1}条承包方数据!", excelName, selectVirtualPerson.Count.ToString());
                string tempPath = TemplateHelper.ExcelTemplate(TemplateFile.ContractLandSurveyExceltemp);
                //string zoneName = GetUinitName(zone);
                //if (SystemSetDefine.CountryTableHead)
                //{
                //    var zoneStation = dbContext.CreateZoneWorkStation();
                //    zoneName = zoneStation.GetVillageName(zone);
                //}
                int percent = (100 / selectVirtualPerson.Count);
                this.ReportProgress(10, "正在获取数据");
                int percentIndex = 1;
                var zoneStation = dbContext.CreateZoneWorkStation();
                List<Zone> allZones = zoneStation.GetAllZones(zone);
                string savePath = CreateDirectoryHelper.CreateDirectory(allZones, zone);
                fileName = fileName + @"\" + savePath;
                foreach (var item in selectVirtualPerson)
                {
                    var landStation = dbContext.CreateContractLandWorkstation();
                    List<ContractLand> landArrays = landStation.GetCollection(item.ID);
                    landArrays.LandNumberFormat(SystemSetDefine);
                    var concordStation = dbContext.CreateConcordStation();
                    var bookStation = dbContext.CreateRegeditBookStation();
                    var listConcords = concordStation.GetContractsByZoneCode(zone.FullCode);
                    var listBooks = bookStation.GetByZoneCode(zone.FullCode, eSearchOption.Precision);
                    string filePath = string.Empty;
                    int familyNumber = 0;
                    Int32.TryParse(item.FamilyNumber, out familyNumber);
                    this.ReportProgress(10 + percent * percentIndex, item.Name);
                    ++percentIndex;
                    using (ExportContractorSurveyExcel export = new ExportContractorSurveyExcel())
                    {
                        export.SaveFilePath = fileName + @"\" + familyNumber + "-" + item.Name + "-" + "单户确认表" + ".xls";
                        export.CurrentZone = zone;
                        export.Familys = vps;
                        //export.UnitName = zoneName;
                        export.UnitName = SystemSet.GetTableHeaderStr(zone);
                        export.ExcelName = SystemSet.GetTBDWStr(zone);
                        export.TableType = metadata.TableType;
                        export.DictionList = metadata.DictList;
                        export.LandArrays = landArrays;
                        export.ConcordCollection = listConcords;
                        export.BookColletion = listBooks;
                        export.Contractor = item;
                        export.PostErrorInfoEvent += export_PostErrorInfoEvent;
                        returnValue = export.BeginExcel(zone.FullCode.ToString(), tempPath);
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

        #endregion 辅助功能

        #region 提示信息

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

        #endregion 提示信息
    }
}