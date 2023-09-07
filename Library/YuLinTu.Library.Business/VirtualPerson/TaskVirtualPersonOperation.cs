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
    /// 承包方数据操作任务类
    /// </summary>
    public class TaskVirtualPersonOperation : Task
    {
        #region Fields

        private FamilyImportDefine familyImportSet;
        private FamilyOutputDefine familyOutputSet;
        private FamilyOtherDefine familyOtherSet;
        private object returnValue;

        #endregion

        #region Property

        /// <summary>
        /// 返回值
        /// </summary>
        public object ReturnValue
        {
            get { return returnValue; }
        }

        /// <summary>
        /// 承包方导入配置
        /// </summary>
        public FamilyImportDefine FamilyImportSet
        {
            get { return familyImportSet; }
            set { familyImportSet = value; }
        }

        /// <summary>
        /// 承包方导出配置
        /// </summary>
        public FamilyOutputDefine FamilyOutputSet
        {
            get { return familyOutputSet; }
            set { familyOutputSet = value; }
        }

        /// <summary>
        /// 承包方其它配置
        /// </summary>
        public FamilyOtherDefine FamilyOtherSet
        {
            get { return familyOtherSet; }
            set { familyOtherSet = value; }
        }

        /// <summary>
        /// 选中承包方(导表用)
        /// </summary>
        public List<VirtualPerson> SelectedPersons { get; set; }

        /// <summary>
        /// 承包方集合
        /// </summary>
        public List<VirtualPerson> ListPerson { get; set; }

        /// <summary>
        /// 是否批量
        /// </summary>
        public bool IsBatch { get; set; }

        #endregion

        #region Ctor

        /// <summary>
        /// 构造函数
        /// </summary>
        public TaskVirtualPersonOperation()
        {
        }

        #endregion

        #region Override

        /// <summary>
        /// 开始操作
        /// </summary
        protected override void OnGo()
        {
            TaskVirtualPersonArgument metadata = Argument as TaskVirtualPersonArgument;
            if (metadata == null)
            {
                return;
            }
            returnValue = null;
            IDbContext dbContext = metadata.Database;
            string fileName = metadata.FileName;
            bool isClear = metadata.IsClear;
            Zone zone = metadata.CurrentZone;
            VirtualPersonBusiness business = new VirtualPersonBusiness(dbContext);
            business.Alert += ReportInfo;
            business.VirtualType = metadata.virtualType;
            business.ProgressChanged += ReportPercent;
            business.FamilyImportSet = FamilyImportSet;
            business.FamilyOutputSet = FamilyOutputSet;
            business.FamilyOtherSet = FamilyOtherSet;
            List<Zone> listZone = business.GetChildZone(zone);  //获取子级地域
            Zone parent = business.GetParent(zone);
            if (metadata == null)
            {
                return;
            }
            switch (metadata.ArgType)
            {
                case ePersonArgType.ImportData:
                    //business.ImportData(zone, fileName, isClear, true, 80, 20);
                    returnValue = zone.FullCode;
                    break;

                case ePersonArgType.VolumnImport:
                    business.VolumnImport(zone, fileName, isClear);
                    returnValue = zone.FullCode;
                    break;

                case ePersonArgType.ExportExcel:
                    //(批量)导出承包方Excel调查表
                    ExportVPSurveyExcel(zone, listZone, parent, fileName, business);
                    break;

                case ePersonArgType.ExportWord:
                    //(批量)导出承包方Word调查表
                    if (zone.Level > eZoneLevel.Group && listZone != null && listZone.Count > 0)
                    {
                        //批量保存(此时遍历不同地域进行统一保存)  
                        ExportFamilyInfoByType(zone, listZone, parent, fileName, business, metadata.ArgType);
                    }
                    else
                    {
                        //批量保存(此时按照选中承包方进行保存)
                        List<Zone> allZones = GetAllZones(zone, listZone, parent, business);
                        string folderString = CreateDirectory(allZones, zone);
                        string path = fileName + @"\" + folderString;
                        var dir = Directory.CreateDirectory(path);
                        //this.ReportProgress(1, "开始");
                        //business.ExportDataWord(zone, SelectedPersons, path, 99, 1, true);
                    }
                    break;

                case ePersonArgType.ExportApply:
                    //(批量)导出户主声明书
                    if (zone.Level > eZoneLevel.Group && listZone != null && listZone.Count > 0)
                    {
                        //批量保存(此时遍历不同地域进行统一保存)  
                        ExportFamilyInfoByType(zone, listZone, parent, fileName, business, metadata.ArgType, metadata.DateValue);
                    }
                    else
                    {
                        //批量保存(此时按照选中承包方进行保存)
                        List<Zone> allZones = GetAllZones(zone, listZone, parent, business);
                        string folderString = CreateDirectory(allZones, zone);
                        string path = fileName + @"\" + folderString;
                        var dir = Directory.CreateDirectory(path);
                        this.ReportProgress(1, "开始");
                        business.ExportApplyBook(zone, SelectedPersons, path, metadata.DateValue, 99, 1, true);
                    }
                    break;

                case ePersonArgType.ExportDelegate:
                    //(批量)导出委托声明书
                    if (zone.Level > eZoneLevel.Group && listZone != null && listZone.Count > 0)
                    {
                        //批量保存(此时遍历不同地域进行统一保存)  
                        ExportFamilyInfoByType(zone, listZone, parent, fileName, business, metadata.ArgType, metadata.DateValue);
                    }
                    else
                    {
                        //批量保存(此时按照选中承包方进行保存)
                        List<Zone> allZones = GetAllZones(zone, listZone, parent, business);
                        string folderString = CreateDirectory(allZones, zone);
                        string path = fileName + @"\" + folderString;
                        var dir = Directory.CreateDirectory(path);
                        this.ReportProgress(1, "开始");
                        business.ExportDelegateBook(zone, SelectedPersons, path, metadata.DateValue, 99, 1, true);
                    }
                    break;

                case ePersonArgType.ExportIdea:
                    //(批量)导出无异议声明书
                    if (zone.Level > eZoneLevel.Group && listZone != null && listZone.Count > 0)
                    {
                        //批量保存(此时遍历不同地域进行统一保存)  
                        ExportFamilyInfoByType(zone, listZone, parent, fileName, business, metadata.ArgType, metadata.DateValue, metadata.PubDateValue);
                    }
                    else
                    {
                        //批量保存(此时按照选中承包方进行保存)
                        List<Zone> allZones = GetAllZones(zone, listZone, parent, business);
                        string folderString = CreateDirectory(allZones, zone);
                        string path = fileName + @"\" + folderString;
                        var dir = Directory.CreateDirectory(path);
                        this.ReportProgress(1, "开始");
                        business.ExportIdeaBook(zone, SelectedPersons, path, metadata.DateValue, metadata.PubDateValue, 99, 1, true);
                    }
                    break;

                case ePersonArgType.ExportSurvey:
                    //(批量)导出测绘申请书
                    if (zone.Level > eZoneLevel.Group && listZone != null && listZone.Count > 0)
                    {
                        //批量保存(此时遍历不同地域进行统一保存)  
                        ExportFamilyInfoByType(zone, listZone, parent, fileName, business, metadata.ArgType, metadata.DateValue);
                    }
                    else
                    {
                        //批量保存(此时按照选中承包方进行保存)
                        List<Zone> allZones = GetAllZones(zone, listZone, parent, business);
                        string folderString = CreateDirectory(allZones, zone);
                        string path = fileName + @"\" + folderString;
                        var dir = Directory.CreateDirectory(path);
                        this.ReportProgress(1, "开始");
                        business.ExportSurveyBook(zone, SelectedPersons, path, metadata.DateValue, 99, 1, true);
                    }
                    break;

                case ePersonArgType.InitialVirtualPerson:
                    if (zone.Level > eZoneLevel.Group && listZone != null && listZone.Count > 0)
                    {
                        //此时为批量初始化承包方信息(最大支持镇级行政区域)    
                        InitialVpInfoVolumn(zone, listZone, parent, business, metadata);
                    }
                    else
                    {
                        //此时仅初始化当前选择地域下的承包方信息
                        this.ReportProgress(1, "开始");
                        //business.InitialVirtualPersonInfo(metadata, ListPerson, zone, 99, 1, true);
                    }
                    break;
            }
        }

        #endregion

        #region 批量导出承包方Excel调查表

        /// <summary>
        /// 导出承包方Excel调查表
        /// </summary>
        /// <param name="currentZone">当前地域</param>
        /// <param name="childrenZone">子级地域</param>
        /// <param name="parentZone">父级地域</param>
        /// <param name="savePath">保存文件路径</param>
        /// <param name="personBusiness">承包方业务</param>
        public void ExportVPSurveyExcel(Zone currentZone, List<Zone> childrenZone, Zone parentZone, string savePath, VirtualPersonBusiness personBusiness)
        {
            if (currentZone == null)
            {
                this.ReportError("未选择导出数据的地域!");
                return;
            }
            List<Zone> allZones = GetAllZones(currentZone, childrenZone, parentZone, personBusiness);
            this.ReportProgress(1, "开始");
            this.ReportProgress(5, "正在获取承包方");
            List<Zone> tempAllZones = new List<Zone>();
            allZones.ForEach(c => tempAllZones.Add(c));
            allZones.ForEach(c =>
            {
                //将大于当前选中地域的地域(集合)排除
                if (c.Level > currentZone.Level)
                    allZones.Remove(c);
            });
            List<Zone> zones = personBusiness.GetExsitZones(allZones);  //存在承包方数据的地域集合 
            double percent = 95 / (double)allZones.Count;
            int indexOfZone = 0;  //地域索引
            foreach (var zone in allZones)
            {
                string descZone = ExportZoneListDir(zone, tempAllZones);
                string folderString = CreateDirectoryByVilliage(tempAllZones, zone);
                string path = savePath + @"\" + folderString;
                if (zones.Exists(c => c.FullCode == zone.FullCode))
                    Directory.CreateDirectory(path);   //有数据则建立文件夹 
                List<VirtualPerson> persons = personBusiness.GetByZone(zone.FullCode);
                this.ReportProgress((int)(5 + percent * indexOfZone), string.Format("{0}", descZone));
                if (persons != null && persons.Count > 0)
                    personBusiness.ExportDataExcel(zone, path, percent, 5 + percent * indexOfZone);
                indexOfZone++;

                //提示信息
                if ((zone.Level == eZoneLevel.Town || zone.Level == eZoneLevel.Village) && persons.Count == 0)
                {
                    //在镇、村下没有承包方数据(提示信息不显示)
                    continue;
                }
                if (persons.Count > 0)
                {
                    //地域下有承包方数据
                    this.ReportInfomation(string.Format("{0}导出{1}条承包方数据", descZone, persons.Count));
                }
                else
                {
                    //地域下无数据
                    this.ReportInfomation(string.Format("{0}无承包方数据", descZone));
                }
            }
            this.ReportProgress(100, "完成");
            this.ReportInfomation(string.Format("共导出{0}个承包方Excel调查表", zones.Count));
        }

        #endregion

        #region 批量导出承包方信息(承包方Word调查表、户主声明书、委托声明书、无异议声明书、测绘申请书)

        /// <summary>
        /// 根据业务类型导出不同承包方信息表(一个承包方一张表)
        /// </summary>
        /// <param name="currentZone">当前地域</param>
        /// <param name="childrenZone">子级地域</param>
        /// <param name="parentZone">父级地域</param>
        /// <param name="savePath">保存文件路径</param>
        /// <param name="personBusiness">承包方业务</param>
        /// <param name="type">导出表格类型</param>
        /// <param name="time">设置(声明)时间</param>
        /// <param name="pubTime">公示时间</param>
        public void ExportFamilyInfoByType(Zone currentZone, List<Zone> childrenZone, Zone parentZone,
            string savePath, VirtualPersonBusiness personBusiness, ePersonArgType type, DateTime? time = null, DateTime? pubTime = null)
        {
            if (currentZone == null)
            {
                this.ReportError("未选择导出数据的地域!");
                return;
            }
            List<Zone> allZones = GetAllZones(currentZone, childrenZone, parentZone, personBusiness);
            this.ReportProgress(1, "开始");
            List<Zone> tempAllZones = new List<Zone>();
            allZones.ForEach(c => tempAllZones.Add(c));
            allZones.ForEach(c =>
            {
                //将大于当前选中地域的地域(集合)排除
                if (c.Level > currentZone.Level)
                    allZones.Remove(c);
            });
            List<Zone> zones = personBusiness.GetExsitZones(allZones);  //存在承包方数据的地域集合 
            double percent = 99 / (double)allZones.Count;
            int indexOfZone = 0;  //地域索引
            int count = 0;  //统计导出word个数
            foreach (var zone in allZones)
            {
                string descZone = ExportZoneListDir(zone, tempAllZones);
                string folderString = CreateDirectory(tempAllZones, zone);
                string path = savePath + @"\" + folderString;
                if (zones.Exists(c => c.FullCode == zone.FullCode))
                    Directory.CreateDirectory(path);  //有承包方数据则建立文件夹
                List<VirtualPerson> listPerson = personBusiness.GetByZone(zone.FullCode);
                this.ReportProgress((int)(1 + percent * indexOfZone), string.Format("{0}", descZone));
                if (listPerson != null && listPerson.Count > 0)
                {
                    switch (type)
                    {
                        case ePersonArgType.ExportWord:
                           // personBusiness.ExportDataWord(zone, listPerson, path, percent, (1 + percent * indexOfZone));
                            break;
                        case ePersonArgType.ExportApply:
                            personBusiness.ExportApplyBook(zone, listPerson, path, time, percent, (1 + percent * indexOfZone));
                            break;
                        case ePersonArgType.ExportDelegate:
                            personBusiness.ExportDelegateBook(zone, listPerson, path, time, percent, (1 + percent * indexOfZone));
                            break;
                        case ePersonArgType.ExportIdea:
                            personBusiness.ExportIdeaBook(zone, listPerson, path, time, pubTime, percent, (1 + percent * indexOfZone));
                            break;
                        case ePersonArgType.ExportSurvey:
                            personBusiness.ExportSurveyBook(zone, listPerson, path, time, percent, (1 + percent * indexOfZone));
                            break;
                    }
                }
                indexOfZone++;

                //提示信息
                if ((zone.Level == eZoneLevel.Town || zone.Level == eZoneLevel.Village) && listPerson.Count == 0)
                {
                    //在镇、村下没有承包方数据(提示信息不显示)
                    continue;
                }
                if (listPerson.Count > 0)
                {
                    //地域下有承包方数据
                    switch (type)
                    {
                        case ePersonArgType.ExportWord:
                            this.ReportInfomation(string.Format("{0}导出{1}张承包方调查表", descZone, listPerson.Count));
                            break;
                        case ePersonArgType.ExportApply:
                            this.ReportInfomation(string.Format("{0}导出{1}张户主声明书", descZone, listPerson.Count));
                            break;
                        case ePersonArgType.ExportDelegate:
                            this.ReportInfomation(string.Format("{0}导出{1}张委托声明书", descZone, listPerson.Count));
                            break;
                        case ePersonArgType.ExportIdea:
                            this.ReportInfomation(string.Format("{0}导出{1}张无异议声明书", descZone, listPerson.Count));
                            break;
                        case ePersonArgType.ExportSurvey:
                            this.ReportInfomation(string.Format("{0}导出{1}张测绘申请书", descZone, listPerson.Count));
                            break;
                    }
                    count += listPerson.Count;
                }
                else
                {
                    //地域下无数据
                    this.ReportInfomation(string.Format("{0}无承包方数据", descZone));
                }
            }
            this.ReportProgress(100, "完成");
            switch (type)
            {
                case ePersonArgType.ExportWord:
                    this.ReportInfomation(string.Format("共导出{0}张承包方Word调查表", count));
                    break;
                case ePersonArgType.ExportApply:
                    this.ReportInfomation(string.Format("共导出{0}张户主声明书", count));
                    break;
                case ePersonArgType.ExportDelegate:
                    this.ReportInfomation(string.Format("共导出{0}张委托声明书", count));
                    break;
                case ePersonArgType.ExportIdea:
                    this.ReportInfomation(string.Format("共导出{0}张无异议声明书", count));
                    break;
                case ePersonArgType.ExportSurvey:
                    this.ReportInfomation(string.Format("共导出{0}张测绘申请书", count));
                    break;
            }
        }

        #endregion

        #region 工具之批量初始化承包方信息

        /// <summary>
        /// 批量初始化承包方信息
        /// </summary>
        /// <param name="currentZone">当前地域</param>
        /// <param name="childrenZone">子级地域集合</param>
        /// <param name="parentZone">父级地域</param>
        /// <param name="personBusiness">承包方业务</param>
        /// <param name="metadata">初始化任务参数</param>
        public void InitialVpInfoVolumn(Zone currentZone, List<Zone> childrenZone, Zone parentZone,
            VirtualPersonBusiness personBusiness, TaskVirtualPersonArgument metadata)
        {
            if (currentZone == null)
            {
                this.ReportError("未选择导出数据的地域!");
                return;
            }
            List<Zone> allZones = GetAllZones(currentZone, childrenZone, parentZone, personBusiness);
            this.ReportProgress(1, "开始");
            List<Zone> tempAllZones = new List<Zone>();
            allZones.ForEach(c => tempAllZones.Add(c));
            tempAllZones.ForEach(c =>
            {
                //将大于当前选中地域的地域(集合)排除
                if (c.Level > currentZone.Level)
                    allZones.Remove(c);
            });
            double percent = 99 / (double)allZones.Count;
            int indexOfZone = 0;  //地域索引
            int personCount = 0;   //统计承包方个数
            foreach (var zone in allZones)
            {
                string descZone = ExportZoneListDir(zone, tempAllZones);
                List<VirtualPerson> persons = personBusiness.GetByZone(zone.FullCode);
                this.ReportProgress((int)(1 + percent * indexOfZone), string.Format("{0}", descZone));
                if (persons != null && persons.Count > 0)
                {
                    //personBusiness.InitialVirtualPersonInfo(metadata, persons, zone, percent, 1 + percent * indexOfZone, false);
                }
                indexOfZone++;

                //提示信息
                if ((zone.Level == eZoneLevel.Town || zone.Level == eZoneLevel.Village) && persons.Count == 0)
                {
                    //在镇、村下没有数据(提示信息不显示)
                    continue;
                }
                if (persons.Count > 0)
                {
                    //地域下有数据
                    this.ReportInfomation(string.Format("{0}初始化{1}个承包方", descZone, persons.Count));
                    personCount += persons.Count;
                }
                else
                {
                    //地域下无数据
                    this.ReportInfomation(string.Format("{0}无承包方信息", descZone));
                }
            }
            this.ReportProgress(100, "完成");
            this.ReportInfomation(string.Format("共初始化{0}个承包方", personCount));
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
        public List<Zone> GetAllZones(Zone currentZone, List<Zone> childrenZone, Zone parentZone, VirtualPersonBusiness business)
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
                    List<Zone> zones = business.GetChildZone(child);
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

        #region 辅助方法

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
