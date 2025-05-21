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
    /// <summary>
    /// 导出家庭承包单户申请书
    /// </summary>
    public class TaskRequireBookByFamilyOperation : Task
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
        public List<VirtualPerson> SelectContractor { get; set; }


        #endregion

        #region Ctor

        /// <summary>
        /// 构造函数
        /// </summary>
        public TaskRequireBookByFamilyOperation()
        {
        }

        #endregion

        #region Override

        /// <summary>
        /// 开始操作
        /// </summary>
        protected override void OnGo()
        {
            TaskRequireBookByFamilyArgument metadata = Argument as TaskRequireBookByFamilyArgument;
            if (metadata == null)
            {
                return;
            }
            returnValue = false;
            DbContext = metadata.Database;
            string fileName = metadata.FileName;
            openFilePath = metadata.FileName;
            SelectContractor = metadata.SelectContractor;
            Zone zone = metadata.CurrentZone;
            ConcordBusiness business = new ConcordBusiness(DbContext);
            business.SystemSet = metadata.SystemSet;
            business.Alert += ReportInfo;
            business.ProgressChanged += ReportPercent;
            business.PublishDateSetting = metadata.PublishDateSetting;
            List<Zone> childrenZone = metadata.SelfAndSubsZones;   //子级地域集合
            Zone parent = business.GetParent(zone);
            if (zone.Level >= eZoneLevel.Group && (SelectContractor == null || SelectContractor.Count == 0))
            {
                //批量导出单户申请书
                BatchSingleExportRequireBook(zone, childrenZone, parent, fileName, business);
            }
            else if (SelectContractor != null && SelectContractor.Count > 0)
            {
                //导出单户申请书-单个或多个
                BatchSingleExportRequireBook(zone, SelectContractor, parent, fileName, business);
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

        #region 承包方式下单户申请书

        /// <summary>
        /// 批量导出单户申请书
        /// </summary>
        private void BatchSingleExportRequireBook(Zone currentZone, List<Zone> childrenZone, Zone parentZone, string savePath, ConcordBusiness business)
        {
            TaskRequireBookByFamilyArgument metadata = Argument as TaskRequireBookByFamilyArgument;

            if (currentZone == null)
            {
                this.ReportError("未选择导出数据的地域!");
                return;
            }
            business.ProgressChanged -= ReportPercent;
            int count = 0;      //统计可导出表格的个数         
            this.ReportProgress(0, "开始获取单户申请书");
            double percent = 99.0 / (double)metadata.SelfAndSubsZones.Count;
            double subprecent = 0.0;
            int zoneCount = 0;

            string desc = ExportZoneListDir(currentZone, metadata.AllZones);// 优化
            string path = metadata.FileName;

            //得到承包方的集合
            List<VirtualPerson> vps = business.GetByZone(currentZone.FullCode);
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
                bool flag = business.SingleExportRequireBookWord(currentZone, path, vp, out message);
                if (flag == true)
                {
                    returnValue = true;
                    this.ReportProgress((int)(subprecent + temppercent * index), string.Format("{0}", desc + vp.Name));
                    index++;
                    count++;
                }
            }
            string info = index == 1 ? string.Format("{0}无家庭承包方式合同书数据", currentZone.FullName) : string.Format("{0}下成功导出{1}个家庭承包方式单户申请书", currentZone.FullName, index - 1);
            if (vps.Count == 0)
            {
                this.ReportWarn(info);
                this.ReportProgress(100, "完成");
                return;
            }
            else
            {               
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
            TaskRequireBookByFamilyArgument metadata = Argument as TaskRequireBookByFamilyArgument;

            if (currentZone == null)
            {
                this.ReportError("未选择导出数据的地域!");
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
                    returnValue = true;
                    this.ReportProgress((int)(subprecent + temppercent * index), string.Format("{0}", vp.Name));
                    index++;
                }
            }
            string info = index == 1 ? string.Format("{0}无家庭承包方式合同书数据", currentZone.FullName) : string.Format("{0}下成功导出{1}个家庭承包方式单户申请书", currentZone.FullName, index - 1);
            if (vps.Count == 0)
            {
                this.ReportWarn(info);
                this.ReportProgress(100, "完成");
                return;
            }
            else
            {
                
                this.ReportInfomation(info);
            }

            this.ReportProgress(100, "完成");
            this.ReportInfomation(string.Format("共导出{0}家庭承包方式单户申请书", index - 1));
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
