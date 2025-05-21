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
    /// 导出公示表
    /// </summary>
    public class TaskExportVillagesDeclareOperation : Task
    {
        #region Fields
        private bool returnValue;
        private string openFilePath;  //打开文件路径
        private SystemSetDefine SystemSetDefine = SystemSetDefine.GetIntence();
        #endregion

        #region Properties



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
        public TaskExportVillagesDeclareOperation()
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
            TaskExportVillagesDeclareArgument metadata = Argument as TaskExportVillagesDeclareArgument;
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

            //批量导出村组公示公告Word                       
            ExportVillagesDeclareWordByZone(zone, childrenZone, parent, fileName, landBusiness);
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
        /// 批量导出村组公告表
        /// </summary>
        /// <param name="currentZone">当前地域</param>
        /// <param name="childrenZone">子级地域</param>
        /// <param name="parentZone">父级地域</param>
        /// <param name="savePath">保存文件路径</param>
        /// <param name="business">台账业务</param>
        private void ExportVillagesDeclareWordByZone(Zone currentZone, List<Zone> childrenZone, Zone parentZone, string savePath, AccountLandBusiness business)
        {
            TaskExportVillagesDeclareArgument metadata = Argument as TaskExportVillagesDeclareArgument;

            if (currentZone == null)
            {
                this.ReportError("未选择导出数据的地域!");
                return;
            }
            business.ProgressChanged -= ReportPercent;
            this.ReportProgress(1, "开始");
            this.ReportProgress(5, "正在获取表");

            double percent = 95 / (double)metadata.SelfAndSubsZones.Count;
            int indexOfZone = 0;  //地域索引
            int dataCount = 0;//有数据表个数
            var vpStation = metadata.Database.CreateVirtualPersonStation<LandVirtualPerson>();
            string descZone = ExportZoneListDir(currentZone, metadata.AllZones);
            string path = metadata.FileName;
            bool hasperson = false;
            if (vpStation.GetByZoneCode(currentZone.FullCode, eLevelOption.Self).Count > 0)
            {
                hasperson = true;
            }
            this.ReportProgress((int)(5 + percent * indexOfZone), string.Format("{0}", descZone));
            if (hasperson)//有数据则建立文件夹
            {
                Directory.CreateDirectory(path);
                VillagesDeclareWord(currentZone, path, metadata.PublishDateSetting);
                this.ReportInfomation(string.Format("{0}导出{1}个公告表数据", descZone, 1));
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
            this.ReportInfomation(string.Format("共导出{0}个公告表", dataCount));
        }


        /// <summary>
        /// 导出村组公示公告表Word
        /// </summary>    
        public void VillagesDeclareWord(Zone zone, string filePath, DateSetting dateSetting = null)
        {
            TaskExportVillagesDeclareArgument metadata = Argument as TaskExportVillagesDeclareArgument;

            try
            {
                if (zone == null)
                {
                    this.ReportError("信息提示", "未选择导出数据的地域!");
                    return;
                }
                string excelName = GetMarkDesc(zone);
                var landStation = dbContext.CreateContractLandWorkstation();
                var vpStation = metadata.Database.CreateVirtualPersonStation<LandVirtualPerson>();
                List<VirtualPerson> listVp = vpStation.GetByZoneCode(zone.FullCode);
                List<ContractLand> listLand = landStation.GetCollection(zone.FullCode, eLevelOption.Self);
                listLand.LandNumberFormat(SystemSetDefine);
                if (listVp == null || listVp.Count == 0)
                {
                    this.ReportError("信息提示", "当前地域下无承包方数据!");
                    return;
                }
                string statueDes = excelName + "公示公告";
                var senderStation = metadata.Database.CreateSenderWorkStation();
                string savePath = filePath + @"\" + excelName + TemplateFile.AnnouncementWord + ".doc";   //保存文件全路径
                string templatePath = TemplateHelper.WordTemplate(TemplateFile.AnnouncementWord);  //模板路径               
                ExportAnnouncementWord exportWord = new ExportAnnouncementWord(metadata.Database);
                exportWord.CurrentZone = zone;
                exportWord.DateSettingForAnnoucementWord = metadata.PublishDateSetting;
                exportWord.ListPerson = listVp;
                exportWord.ListLand = listLand;
                exportWord.Tissue = senderStation.GetByCode(zone.FullCode.PadRight(14, '0'));
                if (exportWord.OpenTemplate(templatePath))   //打开模板
                {
                    if (metadata.IsShow)
                        exportWord.PrintPreview(zone, filePath + @"\" + excelName + TemplateFile.AnnouncementWord);
                    else
                        exportWord.SaveAs(zone, savePath);
                }

                returnValue = true;
                listVp = null;
                listLand = null;
            }
            catch (Exception ex)
            {
                YuLinTu.Library.Log.Log.WriteException(this, "ExportVillagesDeclareWord(导出村组公示公告Word)", ex.Message + ex.StackTrace);
                throw ex;
            }
        }

        #endregion

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
