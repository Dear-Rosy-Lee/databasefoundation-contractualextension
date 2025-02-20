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
    public class TaskExportConcordDataOperation : Task
    {
        #region Fields

        private bool returnValue = false;
        private string openFilePath;  //打开文件路径
        #endregion

        #region Properties

        /// <summary>
        ///  审核申请日期设置
        /// </summary>
        public DateSetting PublishDateSetting { get; set; }

        /// <summary>
        /// 当前选择的承包方
        /// </summary>
        public VirtualPerson Contractor { get; set; }

        /// <summary>
        /// 数据库
        /// </summary>
        public IDbContext DbContext { get; set; }

        /// <summary>
        /// 合同明细表设置
        /// </summary>
        public DataSummaryDefine SummaryDefine { get; set; }

        #endregion

        #region Ctor

        /// <summary>
        /// 构造函数
        /// </summary>
        public TaskExportConcordDataOperation()
        {
        }

        #endregion

        #region Override

        /// <summary>
        /// 开始操作
        /// </summary>
        protected override void OnGo()
        {
            TaskExportConcordDataArgument metadata = Argument as TaskExportConcordDataArgument;
            if (metadata == null)
            {
                return;
            }
            returnValue = false;
            DbContext = metadata.Database;
            openFilePath = metadata.FileName;
            Zone zone = metadata.CurrentZone;
            ConcordBusiness business = new ConcordBusiness(DbContext);
            business.ConcordsModified = metadata.ConcordsModified;
            business.LandsOfInitialConcord = metadata.LandsOfInitialConcord;
            business.Sender = metadata.Sender;
            business.ArgType = eContractAccountType.ExportConcordInformationTable;  //传入合同明细表参数
            business.SummaryDefine = this.SummaryDefine;
            business.Alert += ReportInfo;
            business.ProgressChanged += ReportPercent;
            business.PublishDateSetting = this.PublishDateSetting;
            business.SystemSet = metadata.SystemSet;
            business.AreaType = metadata.AreaType;

            List<Zone> childrenZone = business.GetChildrenZone(zone);   //子级地域集合
            Zone parent = business.GetParent(zone);

            //导出合同
            if (metadata.ListPerson == null || metadata.ListPerson.Count == 0)
            {
                //批量保存(此时遍历不同地域进行统一保存)
                ExportContractConcord(zone, childrenZone, parent, openFilePath, business, false);
            }
            else
            {
                //批量保存(此时按照选中承包方进行保存)
                ExportContractConcord(zone, childrenZone, parent, openFilePath, business, true);
            }
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

        #region 导出合同

        /// <summary>
        /// 导出合同
        /// </summary>
        /// <param name="allZones">全部地域</param>
        /// <param name="currentZone">当前地域</param>
        /// <param name="folderString">文件目录</param>
        private void ExportContractConcord(Zone currentZone, List<Zone> childrenZone, Zone parentZone, string savePath,
       ConcordBusiness business, bool isGroup = true)
        {
            TaskExportConcordDataArgument metadata = Argument as TaskExportConcordDataArgument;
            if (currentZone == null)
            {
                this.ReportError("未选择导出数据的地域!");
                return;
            }
            List<Zone> allZones = metadata.AllZones;//当前地域下所有地域，包括本地域
            business.ProgressChanged -= ReportPercent;
            List<ContractConcord> concords = new List<ContractConcord>();
            double percent = 0.0;
            int index = 1;
            this.ReportProgress(1, "开始");

            /* 修改与2016/08/27 优化导出速度 */
            var personStation = DbContext.CreateVirtualPersonStation<LandVirtualPerson>();
            var landStation = DbContext.CreateContractLandWorkstation();
            List<ContractLand> allLands = null;
            List<VirtualPerson> allPersons = null;
            try
            {
                allLands = landStation.GetCollection(currentZone.FullCode, eLevelOption.SelfAndSubs);
                allPersons = personStation.GetByZoneCode(currentZone.FullCode, eLevelOption.SelfAndSubs);
            }
            catch (Exception)
            {
                allLands = new List<ContractLand>(1);
                allPersons = new List<VirtualPerson>(1);
            }

            if (isGroup)
            {
                //批量保存(此时按照选中承包方进行保存)
                this.ReportProgress(5, "正在获取合同");
                if (!Directory.Exists(Path.GetDirectoryName(savePath)))
                    Directory.CreateDirectory(Path.GetDirectoryName(savePath));

                List<Guid> vpIds = new List<Guid>(metadata.ListPerson.Count);
                metadata.ListPerson.ForEach(c => vpIds.Add(c.ID));
                concords = business.GetByVpIds(vpIds.ToArray());
                if (concords.Count == 0)
                {
                    this.ReportWarn(string.Format("当前地域{0}下未获取到合同"), currentZone.FullName);
                    this.ReportProgress(100);
                    return;
                }
                percent = 95 / (double)concords.Count;
                string descZone = ExportZoneListDir(currentZone, allZones);
                foreach (var concord in concords)
                {
                    /* 修改于2012/08/27 优化速度 */
                    var person = metadata.ListPerson.Find(c => c.ID == concord.ContracterId);
                    var lands = allLands.FindAll(c => c.ConcordId == concord.ID);

                    returnValue = business.ExportConcordData(currentZone, person, lands, concord, savePath,metadata.AreaType);
                    this.ReportProgress((int)(5 + percent * index), string.Format("{0}", descZone + concord.ContracterName));
                    index++;
                }
                this.ReportInfomation(string.Format("{0}导出{1}条合同", descZone, concords.Count));
            }
            else
            {
                //批量保存(此时遍历不同地域进行统一保存)
                this.ReportProgress(5, "正在获取合同");

                List<Zone> zones = business.ExsitZones(metadata.SelfAndSubsZones);  //存在合同数据的地域集合
                percent = 95 / (double)zones.Count;
                int indexOfZone = 0;  //地域索引

                string descZone = ExportZoneListDir(currentZone, metadata.AllZones);
                int endPercent = (int)(5 + percent * indexOfZone);
                this.ReportProgress(endPercent, string.Format("{0}", descZone));

                //有数据则建立文件夹
                if (!Directory.Exists(Path.GetDirectoryName(savePath)) && zones.Exists(c => c.FullCode == currentZone.FullCode))
                    Directory.CreateDirectory(Path.GetDirectoryName(savePath));

                //List<ContractConcord> listConcord = business.GetCollection(currentZone.FullCode, eLevelOption.Self);
                var concordStation = DbContext.CreateConcordStation();
                var listConcord = concordStation.GetContractsByZoneCode(currentZone.FullCode, eLevelOption.Self, true);
                //List<ContractConcord> cocords = business.GetCollection(currentZone.FullCode);
                if (listConcord == null || listConcord.Count <= 0)
                {
                    if (zones.Count <= 1)
                    {
                        this.ReportWarn(currentZone.Name + "下没有合同信息");
                        this.ReportProgress(100);
                        return;
                    }
                    return;
                }
                double percentOfCord = percent / (double)listConcord.Count;
                int indexOfCord = 1;
                var landStocks = allLands.Where(o => o.IsStockLand).ToList();//确股地块
                string landStockAreaAll = "0.00";
                if (landStocks != null && landStocks.Count != 0)
                {
                    landStockAreaAll = landStocks.Sum(o => o.ActualArea).ToString("N2");
                    landStocks.ForEach(o => o.Comment = "共有宗地总面积" + landStockAreaAll);
                }
                foreach (var concord in listConcord)
                {
                    /* 修改于2012/08/27 优化速度 */
                    var lands = allLands.FindAll(c => c.ConcordId == concord.ID);
                    lands.AddRange(landStocks);
                    var person = allPersons.Find(c => c.ID == concord.ContracterId);

                    returnValue = business.ExportConcordData(currentZone, person, lands, concord, savePath,metadata.AreaType);
                    this.ReportProgress((int)(endPercent + percentOfCord * indexOfCord), string.Format("{0}", descZone + concord.ContracterName));
                    indexOfCord++;
                }
                indexOfZone++;

                if (listConcord.Count > 0)
                {
                    //地域下有数据
                    this.ReportInfomation(string.Format("{0}导出{1}条合同", descZone, listConcord.Count));
                }
                else
                {
                    //地域下无数据
                    this.ReportWarn(string.Format("{0}无合同信息", descZone));
                    this.ReportProgress(100);
                }

            }
            this.ReportProgress(100, "完成");
            if (returnValue)
            {
                CanOpenResult = true;
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

        #region 辅助功能

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


        #endregion

    }
}
