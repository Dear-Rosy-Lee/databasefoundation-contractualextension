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

namespace YuLinTu.Library.Business
{
    /// <summary>
    /// 承包合同数据操作任务类
    /// </summary>
    public class TaskContractConcordOperation : Task
    {
        #region Fields

        private object returnValue;

        #endregion

        #region Properties

        /// <summary>
        /// 返回值
        /// </summary>
        public object ReturnValue
        {
            get { return returnValue; }
        }

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
        /// 选中的承包方集合
        /// </summary>
        public List<VirtualPerson> ListPerson { get; set; }

        /// <summary>
        /// 合同明细表设置
        /// </summary>
        public DataSummaryDefine SummaryDefine { get; set; }

        #endregion

        #region Ctor

        /// <summary>
        /// 构造函数
        /// </summary>
        public TaskContractConcordOperation()
        {
        }

        #endregion

        #region Override

        /// <summary>
        /// 开始操作
        /// </summary>
        protected override void OnGo()
        {
            TaskContractConcordArgument metadata = Argument as TaskContractConcordArgument;
            if (metadata == null)
            {
                return;
            }
            returnValue = null;
            DbContext = metadata.Database;
            string fileName = metadata.FileName;
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
            List<Zone> childrenZone = business.GetChildrenZone(zone);   //子级地域集合
            Zone parent = business.GetParent(zone);
            switch (metadata.ArgType)
            {
                case eContractConcordArgType.SingleExportRequireBook:
                    //导出单户申请书-单个或多个
                    BatchSingleExportRequireBook(zone, ListPerson, parent, fileName, business);
                    break;

                case eContractConcordArgType.BatchSingleExportRequireBook:
                    //批量导出单户申请书
                    BatchSingleExportRequireBook(zone, childrenZone, parent, fileName, business);
                    break;
                case eContractConcordArgType.BatchExportApplicationByOther:
                    //批量导出单户申请书(非承包方式)
                    BatchExportApplicationByOther(zone, childrenZone, parent, fileName, business);
                    break;

                case eContractConcordArgType.ExportConcordData:
                    //导出合同
                    if (zone.Level > eZoneLevel.Group && childrenZone != null && childrenZone.Count > 0)
                    {
                        //批量保存(此时遍历不同地域进行统一保存)
                        ExportContractConcord(zone, childrenZone, parent, fileName, business, false);
                    }
                    else
                    {
                        //批量保存(此时按照选中承包方进行保存)
                        ExportContractConcord(zone, childrenZone, parent, fileName, business, true);
                    }
                    break;

                case eContractConcordArgType.BatchExportApplicationBook:
                    BatchExportApplicationBookWord(zone, childrenZone, parent, fileName, business);
                    break;

                case eContractConcordArgType.InitialConcordData:
                    business.InitialConcordData(zone, metadata.Sender);
                    break;

                case eContractConcordArgType.ExportConcordInformationTable:
                    //(批量)保存                   
                    ExportConcordInformation(zone, childrenZone, parent, fileName, business);

                    break;

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
            if (currentZone == null)
            {
                this.ReportError("未选择导出数据的地域!");
                return;
            }
            List<Zone> allZones = GetAllZones(currentZone, childrenZone, parentZone, savePath, business);
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
                string folderString = CreateDirectory(allZones, currentZone);
                string path = savePath + @"\" + folderString;
                var dir = Directory.CreateDirectory(path);
                List<Guid> vpIds = new List<Guid>(ListPerson.Count);
                ListPerson.ForEach(c => vpIds.Add(c.ID));
                concords = business.GetByVpIds(vpIds.ToArray());
                if (concords.Count == 0)
                {
                    this.ReportWarn(string.Format("当前地域{0}下未获取到合同", currentZone.FullName));
                    this.ReportProgress(100);
                    return;
                }
                percent = 95 / (double)concords.Count;
                string descZone = ExportZoneListDir(currentZone, allZones);
                foreach (var concord in concords)
                {
                    /* 修改于2012/08/27 优化速度 */
                    var person = ListPerson.Find(c => c.ID == concord.ContracterId);
                    var lands = allLands.FindAll(c => c.ConcordId == concord.ID);
                    business.ExportConcordData(currentZone, person, lands, concord, path,business.AreaType);
                    this.ReportProgress((int)(5 + percent * index), string.Format("{0}", descZone + concord.ContracterName));
                    index++;
                }

                this.ReportInfomation(string.Format("{0}导出{1}条合同", descZone, concords.Count));
            }
            else
            {
                //批量保存(此时遍历不同地域进行统一保存)
                this.ReportProgress(5, "正在获取合同");
                List<Zone> tempAllZones = new List<Zone>();
                allZones.ForEach(c => tempAllZones.Add(c));
                allZones.ForEach(c =>
                {
                    //将大于当前选中地域的地域(集合)排除
                    if (c.Level > currentZone.Level)
                        allZones.Remove(c);
                });
                List<Zone> zones = business.ExsitZones(allZones);  //存在合同数据的地域集合
                percent = 95 / (double)allZones.Count;
                int indexOfZone = 0;  //地域索引
                foreach (var zone in allZones)
                {
                    string descZone = ExportZoneListDir(zone, tempAllZones);
                    int endPercent = (int)(5 + percent * indexOfZone);
                    this.ReportProgress(endPercent, string.Format("{0}", descZone));
                    string folderString = CreateDirectory(allZones, zone);
                    string path = savePath + @"\" + folderString;
                    if (zones.Exists(c => c.FullCode == zone.FullCode))
                        Directory.CreateDirectory(path);  //有数据则建立文件夹
                    List<ContractConcord> listConcord = business.GetCollection(zone.FullCode);
                    double percentOfCord = percent / (double)listConcord.Count;
                    int indexOfCord = 1;
                    foreach (var concord in listConcord)
                    {
                        /* 修改于2012/08/27 优化速度 */
                        var lands = allLands.FindAll(c => c.ConcordId == concord.ID);
                        var person = allPersons.Find(c => c.ID == concord.ContracterId);
                        business.ExportConcordData(zone, person, lands, concord, path,business.AreaType);
                        this.ReportProgress((int)(endPercent + percentOfCord * indexOfCord), string.Format("{0}", descZone + concord.ContracterName));
                        indexOfCord++;
                    }
                    indexOfZone++;

                    //提示信息
                    if ((zone.Level == eZoneLevel.Town || zone.Level == eZoneLevel.Village) && listConcord.Count == 0)
                    {
                        //在镇、村下没有数据(提示信息不显示)
                        continue;
                    }
                    if (listConcord.Count > 0)
                    {
                        //地域下有数据
                        this.ReportInfomation(string.Format("{0}导出{1}条合同", descZone, listConcord.Count));
                    }
                    else
                    {
                        //地域下无数据
                        this.ReportInfomation(string.Format("{0}无合同信息", descZone));
                    }
                }
            }
            this.ReportProgress(100, "完成");
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
            if (currentZone == null)
            {
                this.ReportError("未选择导出数据的地域!");
                return;
            }
            List<Zone> allZones = GetAllZones(currentZone, childrenZone, parentZone, savePath, business);
            business.ProgressChanged -= ReportPercent;
            this.ReportProgress(1, "开始");
            this.ReportProgress(5, "正在获取合同");
            List<Zone> tempAllZones = new List<Zone>();
            allZones.ForEach(c => tempAllZones.Add(c));
            allZones.ForEach(c =>
            {
                //将大于当前选中地域的地域(集合)排除
                if (c.Level > currentZone.Level)
                    allZones.Remove(c);
            });
            List<Zone> zones = business.ExsitZones(allZones);  //存在合同数据的地域集合
            double percent = 95 / (double)allZones.Count;
            int indexOfZone = 0;  //地域索引
            foreach (var zone in allZones)
            {
                string descZone = ExportZoneListDir(zone, tempAllZones);
                string folderString = CreateDirectoryByVilliage(allZones, zone);
                string path = savePath + @"\" + folderString;
                if (zones.Exists(c => c.FullCode == zone.FullCode))
                    Directory.CreateDirectory(path);  //有数据则建立文件夹
                List<ContractConcord> listConcord = business.GetCollection(zone.FullCode);
                this.ReportProgress((int)(5 + percent * indexOfZone), string.Format("{0}", descZone));
                if (listConcord != null && listConcord.Count > 0)
                    business.ExportConcordInfoTable(zone, path, percent, 5 + percent * indexOfZone);
                indexOfZone++;

                //提示信息
                if ((zone.Level == eZoneLevel.Town || zone.Level == eZoneLevel.Village) && listConcord.Count == 0)
                {
                    //在镇、村下没有数据(提示信息不显示)
                    continue;
                }
                if (listConcord.Count > 0)
                {
                    //地域下有数据
                    this.ReportInfomation(string.Format("{0}导出{1}条合同明细数据", descZone, listConcord.Count));
                }
                else
                {
                    //地域下无数据
                    this.ReportInfomation(string.Format("{0}无合同信息", descZone));
                }
            }
            this.ReportProgress(100, "完成");
            this.ReportInfomation(string.Format("共导出{0}个合同明细表", zones.Count));
        }

        #endregion

        #region 批量导出集体申请书

        /// <summary>
        /// 导出集体申请书
        /// </summary>
        private void BatchExportApplicationBookWord(Zone currentZone, List<Zone> childrenZone, Zone parentZone, string savePath, ConcordBusiness business)
        {
            if (currentZone == null)
            {
                this.ReportError("未选择导出数据的地域!");
                return;
            }
            List<Zone> allZones = GetAllZones(currentZone, childrenZone, parentZone, savePath, business);
            business.ProgressChanged -= ReportPercent;
            double percent = 99.0 / (double)allZones.Count;
            int currentIndex = 1;
            int index = 1;
            this.ReportProgress(1, "开始");
            bool flag = false;
            foreach (var zone in allZones)
            {
                string desc = ExportZoneListDir(zone, allZones);
                string folderString = CreateDirectoryByVilliage(allZones, zone);
                string path = savePath + @"\" + folderString;
                if (!Directory.Exists(path))
                {
                    var dir = Directory.CreateDirectory(path);
                }
                flag = business.BatchExportApplicationBookWord(zone, path);
                index++;
                if (flag)
                {
                    this.ReportInfomation(string.Format("{0}成功导出集体申请书", zone.FullName));
                    currentIndex++;
                }
                if ((zone.Level == eZoneLevel.Village && business.GetTissuesByConcord(zone).Count == 0) || zone.Level == eZoneLevel.Town)
                    continue;
                this.ReportProgress((int)(1 + percent * index), string.Format("正导出{0}", desc));
            }
            this.ReportProgress(100, "完成");
            this.ReportInfomation(string.Format("共导出{0}条集体申请书信息", currentIndex));
        }

        #endregion

        #region 承包方式下单户申请书

        /// <summary>
        /// 批量导出单户申请书
        /// </summary>
        private void BatchSingleExportRequireBook(Zone currentZone, List<Zone> childrenZone, Zone parentZone, string savePath, ConcordBusiness business)
        {
            if (currentZone == null)
            {
                this.ReportError("未选择导出数据的地域!");
                return;
            }
            List<Zone> allZones = GetAllZones(currentZone, childrenZone, parentZone, savePath, business);
            business.ProgressChanged -= ReportPercent;
            int count = 0;      //统计可导出表格的个数         
            this.ReportProgress(0, "开始获取单户申请书");
            double percent = 99.0 / (double)allZones.Count;
            double subprecent = 0.0;
            int zoneCount = 0;
            foreach (var zone in allZones)
            {
                string desc = ExportZoneListDir(zone, allZones);// 优化
                string folderString = CreateDirectory(allZones, zone);
                string path = savePath + @"\" + folderString;
                //得到承包方的集合
                List<VirtualPerson> vps = business.GetByZone(zone.FullCode);
                if (vps == null || vps.Count == 0)
                {
                    vps = new List<VirtualPerson>();
                }
                subprecent = percent * (zoneCount++);
                this.ReportProgress((int)(subprecent), string.Format("{0}", desc));
                double temppercent = percent / (double)(vps.Count == 0 ? 1 : vps.Count);
                int index = 1;
                foreach (var vp in vps)
                {
                    string message = null;
                    bool flag = business.SingleExportRequireBookWord(zone, path, vp, out message);
                    if (flag == true)
                    {
                        this.ReportProgress((int)(subprecent + temppercent * index), string.Format("{0}", desc + vp.Name));
                        index++;
                        count++;
                    }
                }
                if ((zone.Level == eZoneLevel.Village && (vps == null || vps.Count == 0)) || zone.Level == eZoneLevel.Town)
                    continue;
                string info = index == 1 ? string.Format("{0}无家庭承包方式合同书数据", zone.FullName) : string.Format("{0}下成功导出{1}个家庭承包方式单户申请书", zone.FullName, index - 1);
                this.ReportInfomation(info);
            }
            this.ReportProgress(100, "完成");
            this.ReportInfomation(string.Format("共导出{0}家庭承包方式单户申请书", count));
        }

        /// <summary>
        /// 多个导出单户申请书
        /// </summary>
        private void BatchSingleExportRequireBook(Zone currentZone, List<VirtualPerson> vps, Zone parentZone, string savePath, ConcordBusiness business)
        {
            if (currentZone == null)
            {
                this.ReportError("未选择导出数据的地域!");
                return;
            }
            if (vps == null || vps.Count == 0)
            {
                this.ReportError("未选择导出承包方!");
                return;
            }
            business.ProgressChanged -= ReportPercent;

            this.ReportProgress(0, "开始获取单户申请书");
            double percent = 99.0 / (double)vps.Count;
            double subprecent = 0.0;
            int zoneCount = 0;
            subprecent = percent * (zoneCount++);
            double temppercent = percent / (double)(vps.Count == 0 ? 1 : vps.Count);
            int index = 1;
            foreach (var vp in vps)
            {
                string message = null;
                bool flag = business.SingleExportRequireBookWord(currentZone, savePath, vp, out message);
                if (flag == true)
                {
                    this.ReportProgress((int)(subprecent + temppercent * index), string.Format("{0}", vp.Name));
                    index++;
                }
            }
            string info = index == 1 ? string.Format("{0}无家庭承包方式合同书数据", currentZone.FullName) : string.Format("{0}下成功导出{1}个家庭承包方式单户申请书", currentZone.FullName, index - 1);
            this.ReportInfomation(info);

            this.ReportProgress(100, "完成");
            this.ReportInfomation(string.Format("共导出{0}家庭承包方式单户申请书", index - 1));
        }


        #endregion

        #region 非家庭承包方式下单户申请书

        /// <summary>
        /// 批量导出非家庭承包导出单户申请书
        /// </summary>
        private void BatchExportApplicationByOther(Zone currentZone, List<Zone> childrenZone, Zone parentZone, string savePath, ConcordBusiness business)
        {
            if (currentZone == null)
            {
                this.ReportError("未选择导出数据的地域!");
                return;
            }
            List<Zone> allZones = GetAllZones(currentZone, childrenZone, parentZone, savePath, business);
            business.ProgressChanged -= ReportPercent;
            int count = 0;      //统计可导出表格的个数         
            this.ReportProgress(0, "开始获取单户申请书");
            double percent = 99.0 / (double)allZones.Count;
            double subprecent = 0.0;
            int zoneCount = 0;
            foreach (var zone in allZones)
            {
                string desc = ExportZoneListDir(zone, allZones);// 优化
                string folderString = CreateDirectory(allZones, zone);
                string path = savePath + @"\" + folderString;
                //得到承包方的集合
                List<VirtualPerson> vps = business.GetByZone(zone.FullCode);
                if (vps == null || vps.Count == 0)
                {
                    vps = new List<VirtualPerson>();
                }
                subprecent = percent * (zoneCount++);
                this.ReportProgress((int)(subprecent), string.Format("{0}", desc));
                double temppercent = percent / (double)(vps.Count == 0 ? 1 : vps.Count);
                int index = 1;
                foreach (var vp in vps)
                {
                    string message = null;
                    bool flag = business.ExportApplicationByOtherBookWord(zone, path, vp, out message);
                    if (flag == true)
                    {
                        this.ReportProgress((int)(subprecent + temppercent * index), string.Format("{0}", desc + vp.Name));
                        index++;
                        count++;
                    }
                }
                if ((zone.Level == eZoneLevel.Village && (vps == null || vps.Count == 0)) || zone.Level == eZoneLevel.Town)
                    continue;
                string info = index == 1 ? string.Format("{0}无其他承包方式合同数据", zone.FullName) : string.Format("{0}下成功导出{1}其他承包方式单户申请书", zone.FullName, index - 1);
                this.ReportInfomation(info);
            }
            this.ReportProgress(100, "完成");
            this.ReportInfomation(string.Format("共导出{0}其他承包方式单户申请书", count));
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
