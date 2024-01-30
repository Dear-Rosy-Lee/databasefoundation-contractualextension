/*
 * (C) 2015  鱼鳞图公司版权所有,保留所有权利 
 */
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using YuLinTu.Data;
using YuLinTu.Library.Business;
using YuLinTu.Library.Entity;
using YuLinTu.Windows;
using YuLinTu.Windows.Wpf.Metro.Components;

namespace YuLinTu.Component.XiZangLZ
{
    /// <summary>
    /// 导出家庭承包方式合同任务类
    /// </summary>
    public class TaskExportFamilyConcordOperation : Task
    {
        #region Ctor

        /// <summary>
        /// 构造函数
        /// </summary>
        public TaskExportFamilyConcordOperation()
        { }

        #endregion

        #region Field

        private string openFilePath;  //打开文件路径

        #endregion

        #region Properties

        /// <summary>
        /// 工作页
        /// </summary>
        public IWorkpage Workpage { get; set; }

        #endregion

        #region Method—Override

        /// <summary>
        /// 开始执行任务
        /// </summary>
        protected override void OnGo()
        {
            TaskExportFamilyConcordArgument argument = Argument as TaskExportFamilyConcordArgument;
            if (argument == null)
                return;
            IDbContext dbContext = argument.DbContext;
            Zone currentZone = argument.CurrentZone;
            string filePath = argument.FileName;
            List<YuLinTu.Library.Entity.VirtualPerson> listPerson = argument.ListPerson;
            List<Dictionary> listDict = argument.ListDict;
            bool canOpen = ExportConcordWord(argument, dbContext, currentZone, filePath, listPerson, listDict);
            if (canOpen)
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

        /// <summary>
        /// 导出西藏农村土地承包合同
        /// </summary>
        private bool ExportConcordWord(TaskExportFamilyConcordArgument argument, IDbContext dbContext,
            Zone currentZone, string filePath, List<YuLinTu.Library.Entity.VirtualPerson> listPerson, List<Dictionary> listDict)
        {
            bool isOpen = true;
            try
            {
                string markDesc = GetMarkDesc(currentZone, dbContext);
                var landStation = dbContext.CreateContractLandWorkstation();
                var concordStation = dbContext.CreateConcordStation();
                var allConcords = concordStation.GetContractsByZoneCode(currentZone.FullCode);  //添加有效性的判断
                var allFamilyConcords = allConcords == null ? null : allConcords.FindAll(c => c.ArableLandType == ((int)eConstructMode.Family).ToString());
                if (allFamilyConcords == null || allFamilyConcords.Count == 0)
                {
                    this.ReportProgress(100, null);
                    this.ReportWarn(string.Format("{0}未获取家庭承包方式合同", markDesc));
                    return false;
                }
                if (allConcords == null || allConcords.Count == 0)
                {
                    this.ReportProgress(100, null);
                    this.ReportWarn(string.Format("{0}未获取有效承包合同", markDesc));
                    return false;
                }
                this.ReportProgress(0, "开始");
                string templateName = "西藏农村土地承包经营权农村土地承包合同书";
                string templatePath = TemplateHelper.WordTemplate(templateName);
                int indexOfPerson = 0; //导出合同索引
                int indexOfExport = 0; //导出合同成功索引
                string folderString = CreateDirectory(GetAllZones(currentZone, dbContext), currentZone);
                string path = filePath + @"\" + folderString;
                openFilePath = path;
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);
                double percent = 99 / (double)listPerson.Count;
                List<ContractLand> lands = landStation.GetCollection(currentZone.FullCode, eLevelOption.Self);
                List<Dictionary> dictCBFS = listDict.FindAll(c => c.GroupCode == DictionaryTypeInfo.CBJYQQDFS);
                foreach (var person in listPerson)
                {
                    var concords = allConcords.FindAll(c => c.ContracterId == person.ID);
                    //var concords = concordStation.GetContractsByFamilyID(person.ID);
                    //concord = concords == null ? null : concords.Find(c => c.ArableLandType == ((int)eConstructMode.Family).ToString());
                    if (concords == null)
                    {
                        this.ReportProgress((int)(1 + percent * indexOfPerson), string.Format("{0}", markDesc + person.Name));
                        indexOfPerson++;
                        continue;
                    }
                    string typeString = string.Empty;
                    foreach (var concord in concords)
                    {
                        //lands = landStation.GetByConcordId(concord.ID);
                        typeString = dictCBFS == null ? "" : dictCBFS.Find(c => c.Code == concord.ArableLandType).Name;
                        var landOfConcord = lands == null ? new List<ContractLand>() : lands.FindAll(c => c.ConcordId == concord.ID);
                        string fullPath = path + @"\" + person.FamilyNumber + "-" + person.Name + "-" + "农村土地承包合同" + "(" + typeString + ")" + ".doc";
                        string dictoryName = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Template\西藏字典.xlsx");
                        var fbfStation = dbContext.CreateSenderWorkStation();
                        ExportAgricultureConcord exportConcord = new ExportAgricultureConcord(dictoryName);
                        exportConcord.dbContext = argument.DbContext;
                        exportConcord.CurrentZone = currentZone;
                        exportConcord.VirtualPerson = person;
                        exportConcord.ListLand = landOfConcord == null ? new List<ContractLand>() : landOfConcord;
                        exportConcord.ListDict = listDict;
                        exportConcord.ZoneNameCounty = argument.ZoneNameCounty;
                        exportConcord.ZoneNameTown = argument.ZoneNameTown;
                        exportConcord.ZoneNameVillage = argument.ZoneNameVillage;
                        exportConcord.ZoneNameGroup = argument.ZoneNameGroup;
                        exportConcord.OpenTemplate(templatePath);
                        exportConcord.SaveAs(concord, fullPath);
                        indexOfExport++;
                    }
                    this.ReportProgress((int)(1 + percent * indexOfPerson), string.Format("{0}", markDesc + person.Name));
                    indexOfPerson++;
                }
                this.ReportProgress(100, null);
                //string strInfo = string.Format("{0}导出{1}个家庭承包方式合同", markDesc, indexOfExport);
                string strInfo = string.Format("{0}导出{1}个承包合同", markDesc, indexOfExport);
                if (indexOfExport == 0)
                {
                    //strInfo = string.Format("{0}未导出家庭承包方式合同", markDesc);
                    strInfo = string.Format("{0}未导出承包合同", markDesc);
                    //TODO 删除路径的作用
                    //if (Directory.Exists(path) && Directory.GetFiles(path).Count() == 0)
                    //    Directory.Delete(path);
                    isOpen = false;
                }
                this.ReportInfomation(strInfo);
            }
            catch (Exception ex)
            {
                isOpen = false;
                this.ReportError("导出家庭承包方式合同失败!");
                YuLinTu.Library.Log.Log.WriteException(this, "TaskExportFamilyConcordOperation(导出家庭承包方式合同任务)", ex.Message + ex.StackTrace);
            }
            return isOpen;
        }

        #endregion

        #region Method—Helper

        #region 任务描述信息

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
            if (zone.Level == eZoneLevel.Town)
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

        #endregion

        #region 处理消息

        /// <summary>
        /// 获取上级地域
        /// </summary>
        /// <param name="zone"></param>
        /// <returns></returns>
        public Zone GetParent(Zone zone, IDbContext dbContext)
        {
            ModuleMsgArgs arg = new ModuleMsgArgs();
            arg.Datasource = dbContext;
            arg.Parameter = zone;
            arg.Name = ZoneMessage.ZONE_PARENT_ZONE;
            TheBns.Current.Message.Send(this, arg);
            return (arg.ReturnValue as Zone);
        }

        /// <summary>
        /// 获取上级地域集合
        /// </summary>
        /// <param name="zone"></param>
        /// <returns></returns>
        private List<Zone> GetParents(Zone zone, IDbContext dbContext)
        {
            ModuleMsgArgs arg = new ModuleMsgArgs();
            arg.Datasource = dbContext;
            arg.Parameter = zone;
            arg.Name = ZoneMessage.ZONE_PARENTSTOTOWN_ZONE;
            TheBns.Current.Message.Send(this, arg);
            return (arg.ReturnValue as List<Zone>);
        }

        /// <summary>
        /// 获取地域下所有子级地域集合
        /// </summary>
        /// <param name="zone"></param>
        /// <returns></returns>
        private List<Zone> GetChildrenZone(Zone zone, IDbContext dbContext)
        {
            ModuleMsgArgs arg = new ModuleMsgArgs();
            arg.Datasource = dbContext;
            arg.Parameter = zone;
            arg.Name = ZoneMessage.ZONE_ALLCHILDREN_ZONE;
            TheBns.Current.Message.Send(this, arg);
            return (arg.ReturnValue as List<Zone>);
        }

        #endregion

        #region 获取全部地域及创建文件目录

        /// <summary>
        /// 获取全部的地域
        /// </summary>
        /// <param name="currentZone">当前地域</param>
        /// <param name="childrenZone">子级地域</param>
        /// <param name="parentZone">父级地域</param>
        /// <param name="fileName">保存路径</param>
        /// <param name="business">合同业务</param>
        private List<Zone> GetAllZones(Zone currentZone, IDbContext dbContext)
        {
            List<Zone> allZones = new List<Zone>();
            if (currentZone == null || dbContext == null)
                return allZones;
            List<Zone> childrenZones = GetChildrenZone(currentZone, dbContext);
            List<Zone> parentZone = GetParents(currentZone, dbContext);
            if (currentZone.Level == eZoneLevel.Group)
            {
                //选择为组
                allZones.Add(currentZone);
                if (parentZone != null)
                    allZones.AddRange(parentZone);
            }
            else if (currentZone.Level == eZoneLevel.Village)
            {
                //选择为村
                if (parentZone != null)
                    allZones.AddRange(parentZone);
                if (childrenZones != null)
                    allZones.AddRange(childrenZones);
            }
            else if (currentZone.Level == eZoneLevel.Town)
            {
                //选择为镇
                if (childrenZones != null)
                    allZones.AddRange(childrenZones);
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
            if (allZones == null || cZone == null)
                return "";
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
        /// <param name="allZones">三级地域集合(镇、村、组)</param>
        /// <param name="cZone">当前地域</param>
        private string CreateDirectoryByVilliage(List<Zone> allZones, Zone cZone)
        {
            if (allZones == null || cZone == null)
                return "";
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

        #region 其他

        /// <summary>
        /// 消息显示框
        /// </summary>
        private void ShowBox(string header, string msg, eMessageGrade type = eMessageGrade.Error)
        {
            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                Workpage.Page.ShowMessageBox(new TabMessageBoxDialog
                {
                    Header = header,
                    Message = msg,
                    MessageGrade = type,
                    CancelButtonText = "取消",
                });
            })); ;
        }

        #endregion

        #endregion
    }
}
