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
    public class TaskExportApplicationBookOperation : Task
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
        public TaskExportApplicationBookOperation()
        {
        }

        #endregion

        #region Override

        /// <summary>
        /// 开始操作
        /// </summary>
        protected override void OnGo()
        {
            TaskExportApplicationBookArgument metadata = Argument as TaskExportApplicationBookArgument;
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
            business.Alert += ReportInfo;
            business.ProgressChanged += ReportPercent;
            business.PublishDateSetting = metadata.PublishDateSetting;
            List<Zone> childrenZone = metadata.SelfAndSubsZones;   //子级地域集合
            Zone parent = business.GetParent(zone);
            BatchExportApplicationBookWord(zone, childrenZone, parent, fileName, business);
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

        #region 批量导出集体申请书

        /// <summary>
        /// 导出集体申请书
        /// </summary>
        private void BatchExportApplicationBookWord(Zone currentZone, List<Zone> childrenZone, Zone parentZone, string savePath, ConcordBusiness business)
        {
            TaskExportApplicationBookArgument metadata = Argument as TaskExportApplicationBookArgument;
            if (currentZone == null)
            {
                this.ReportError("未选择导出数据的地域!");
                return;
            }
            List<Zone> allZones = metadata.AllZones;
            business.ProgressChanged -= ReportPercent;
            double percent = 99.0 / (double)metadata.SelfAndSubsZones.Count;
            int currentIndex = 1;
            int index = 1;
            this.ReportProgress(1, "开始");
            bool flag = false;
            string descZone = ExportZoneListDir(currentZone, allZones);
            
            var tissuecount = business.GetTissuesByConcord(currentZone).Count;
            if ((currentZone.Level == eZoneLevel.Town || currentZone.Level == eZoneLevel.Village) && tissuecount == 0)
            {
                //在镇、村下没有数据(提示信息不显示)
                this.ReportWarn(string.Format("{0}无数据", descZone));
                this.ReportProgress(100, "完成");
                return;
            }
            if (currentZone.Level == eZoneLevel.Group && tissuecount == 0)
            {
                //地域下无数据
                this.ReportWarn(string.Format("{0}无数据", descZone));
                this.ReportProgress(100, "完成");
                return;
            }
            string path = metadata.FileName;
            if (!Directory.Exists(path))
            {
                var dir = Directory.CreateDirectory(path);
            }
            flag = business.BatchExportApplicationBookWord(currentZone, path);
            index++;
            this.ReportProgress((int)(1 + percent * index), string.Format("正导出{0}", descZone));

            if (flag)
            {
                this.ReportInfomation(string.Format("{0}成功导出集体申请书", currentZone.FullName));
               
                returnValue = flag;
            }

            this.ReportProgress(100, "完成");
            this.ReportInfomation(string.Format("共导出{0}条集体申请书信息", currentIndex));
            currentIndex++;
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
