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
    /// 承包权证数据操作任务类
    /// </summary>
    public class TaskContractRegeditBookOperation : Task
    {
        #region Fields

        private object returnValue;   //返回值

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
        /// 权证集合
        /// </summary>
        public List<ContractRegeditBook> ListWarrant { get; set; }

        /// <summary>
        /// 权证汇总表设置
        /// </summary>
        public DataSummaryDefine SummaryDefine { get; set; }

        /// <summary>
        /// 系统信息设置
        /// </summary>
        public SystemSetDefine SystemSet { get; set; }

        /// <summary>
        /// 是否批量
        /// </summary>
        public bool IsBatch { get; set; }

        /// <summary>
        /// 证书共有人数设置-证书数据处理分页设置
        /// </summary>
        public int? BookPersonNum { get; set; }

        /// <summary>
        /// 证书地块数设置-证书数据处理分页设置
        /// </summary>
        public int? BookLandNum { get; set; }

        /// <summary>
        /// 证书编码设置-证书编码样式设置
        /// </summary>
        public string BookNumSetting { get; set; }

        /// <summary>
        /// 数据字典集合
        /// </summary>
        public List<Dictionary> DictList { get; set; }

        /// <summary>
        /// 使用Excel文件
        /// </summary>
        public bool UseExcel { get; set; }

        /// <summary>
        /// 承包方业务
        /// </summary>
        public VirtualPersonBusiness PersonBusiness { get; set; }

        /// <summary>
        /// 数据字典处理业务
        /// </summary>
        public DictionaryBusiness DictBusiness { get; set; }

        /// <summary>
        /// 土地业务
        /// </summary>
        public AccountLandBusiness AccountLandBusiness { get; set; }

        /// <summary>
        /// 合同业务类
        /// </summary>       
        public ConcordBusiness ConcordBusiness { get; set; }

        /// <summary>
        /// 权证业务类
        /// </summary>  
        public ContractRegeditBookBusiness ContractRegeditBookBusiness { get; set; }

        /// <summary>
        /// 选中承包方
        /// </summary>
        public List<VirtualPerson> SelectedPersons { get; set; }

        /// <summary>
        /// 承包权证导出选择扩展模板格式设置
        /// </summary>
        public ContractRegeditBookSettingDefine ExtendUseExcelDefine
        { get; set; }
       

        #endregion

        #region Ctor

        /// <summary>
        /// 构造函数
        /// </summary>
        public TaskContractRegeditBookOperation()
        {

        }

        #endregion

        #region Override

        /// <summary>
        /// 开始操作
        /// </summary>
        protected override void OnGo()
        {
            TaskContractRegeditBookArgument metadata = Argument as TaskContractRegeditBookArgument;
            if (metadata == null)
            {
                return;
            }
            returnValue = null;
            DbContext = metadata.Database;
            PersonBusiness = new VirtualPersonBusiness(DbContext);
            PersonBusiness.VirtualType = eVirtualType.Land;   //指定承包方类型
            DictBusiness = new DictionaryBusiness(DbContext);
            AccountLandBusiness = new AccountLandBusiness(DbContext);
            ConcordBusiness = new ConcordBusiness(DbContext);
            ContractRegeditBookBusiness = new ContractRegeditBookBusiness(DbContext);
            string fileName = metadata.FileName;
            Zone zone = metadata.CurrentZone;
            ContractRegeditBookBusiness business = new ContractRegeditBookBusiness(DbContext);
            business.Argument = metadata;
            business.BookLandNum = BookLandNum;
            business.BookPersonNum = BookPersonNum;
            business.BookNumSetting = BookNumSetting;
            business.SystemSet = metadata.SystemSet;
            business.ArgType = eContractAccountType.ExportWarrentSummaryTable;  //传入权证汇总表参数
            business.SummaryDefine = this.SummaryDefine;
            business.Alert += ReportInfo;
            business.ProgressChanged += ReportPercent;
            business.WarrantsModified = metadata.WarrantsModified;
            business.ConcordsOfInitialWarrant = metadata.Concords;
            business.ExtendUseExcelDefine = ExtendUseExcelDefine;
            List<Zone> childrenZone = business.GetChildrenZone(zone);   //子级地域集合
            Zone parent = business.GetParent(zone);
            switch (metadata.ArgType)
            {
                case eContractRegeditBookType.ExportWarrant:
                    //导出权证
                    if (zone.Level > eZoneLevel.Group && childrenZone != null && childrenZone.Count > 0)
                    {
                        ExportContractWarrant(zone, childrenZone, parent, fileName, business);
                    }
                    else
                    {
                        business.ExportWarrantWord(zone, ListPerson, fileName);
                    }
                    break;
                case eContractRegeditBookType.ExportWarrentSummaryData:
                    //权证数据汇总表
                    //(批量)保存
                    ExportWarrentSummary(zone, childrenZone, parent, fileName, business);
                    break;
                case eContractRegeditBookType.ExportAwareInventoryData:
                    //颁证清册
                    //(批量)保存
                    ExportAwareInventory(zone, childrenZone, parent, fileName, business);
                    break;
                case eContractRegeditBookType.ExportSingleFamilyConfirmData:
                    //单户确认表
                    if (zone.Level > eZoneLevel.Group && childrenZone != null && childrenZone.Count > 0)
                    {
                        //批量保存(此时遍历不同地域进行统一保存)
                        ExportFamilyConfirm(zone, childrenZone, parent, fileName, business);
                    }
                    else
                    {
                        //批量保存(此时按照选中承包方进行保存)
                        List<Zone> allZones = GetAllZones(zone, childrenZone, parent, fileName, business);
                        string folderString = CreateDirectory(allZones, zone);
                        string path = fileName + @"\" + folderString;
                        var dir = Directory.CreateDirectory(path);
                        business.ExportFamilyConfirmTable(zone, SelectedPersons, path, false);
                    }
                    break;
                case eContractRegeditBookType.BatchExportRegeditBookData:
                    if (zone.Level > eZoneLevel.Group && childrenZone != null && childrenZone.Count > 0)
                    {
                        BatchExportRegeditBookWord(zone, childrenZone, parent, fileName, business);
                    }
                    else
                    {
                        ExportRegeditBookWord(zone, childrenZone, parent, fileName, business, SelectedPersons);
                    }
                    break;
                case eContractRegeditBookType.InitialWarrantData:
                    //权证登记
                    business.InitialWarrantData(zone);
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
        public List<Zone> GetAllZones(Zone currentZone, List<Zone> childrenZone, Zone parentZone, string fileName, ContractRegeditBookBusiness business)
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

        #region 批量导出权证

        /// <summary>
        /// 导出权证
        /// </summary>
        /// <param name="allZones">全部地域</param>
        /// <param name="currentZone">当前地域</param>
        /// <param name="folderString">文件目录</param>
        private void ExportContractWarrant(Zone currentZone, List<Zone> childrenZone, Zone parentZone, string savePath, ContractRegeditBookBusiness business)
        {
            if (currentZone == null)
            {
                this.ReportError("未选择导出数据的地域!");
                return;
            }
            List<Zone> allZones = GetAllZones(currentZone, childrenZone, parentZone, savePath, business);
            List<Zone> tempAllZones = new List<Zone>();
            allZones.ForEach(c => tempAllZones.Add(c));
            allZones.ForEach(c =>
            {
                //将大于当前选中地域的地域(集合)排除
                if (c.Level > currentZone.Level)
                    allZones.Remove(c);
            });
            business.ProgressChanged -= ReportPercent;
            this.ReportProgress(0, "开始获取权证");
            double percent = 0.0;
            percent = 99.0 / (double)allZones.Count;
            double subpercent = 0.0;
            int zoneCount = 0;
            int warrantsCount = 0;      //统计可导出权证的个数         
            foreach (var zone in allZones)
            {
                string desc = ExportZoneListDir(zone, tempAllZones);// 优化

                List<ContractRegeditBook> getWarrants = ContractRegeditBookBusiness.GetByZoneCode(zone.FullCode, eSearchOption.Precision);
                List<ContractConcord> listConcord = ConcordBusiness.GetContractsByZoneCode(zone.FullCode, eLevelOption.SelfAndSubs);

                string folderString = CreateDirectory(allZones, zone);
                string path = savePath + @"\" + folderString;

                subpercent = percent * (zoneCount++);
                this.ReportProgress((int)(subpercent), string.Format("{0}", desc));
                double temppercent = percent / (double)(getWarrants.Count == 0 ? 1 : getWarrants.Count);
                int index = 1;
                foreach (var exportWarrant in getWarrants)
                {
                    if (exportWarrant.RegeditNumber == "")
                    {
                        if (exportWarrant.Number != "")
                        {
                            YuLinTu.Library.Log.Log.WriteException(this, "导出权证", string.Format("权证号:{0}权证编号为空", exportWarrant.Number));
                            continue;
                        }
                        else
                        {
                            YuLinTu.Library.Log.Log.WriteException(this, "导出权证", string.Format("权证ID:{0}权证编号及权证编码为空，请检查数据", exportWarrant.ID.ToString()));
                            continue;
                        }
                    }
                    ContractConcord concord = listConcord.Find(t => t.ID == exportWarrant.ID);
                    if (concord == null)
                    {
                        this.ReportInfomation(string.Format("权证编码:{0}无对应合同,请检查地块及合同数据", exportWarrant.Number));  
                        continue;
                    }
                    business.ExportWarrantData(zone, exportWarrant, path);
                    this.ReportProgress((int)(subpercent + temppercent * index), string.Format("导出{0}", desc + concord.ContracterName));
                    index++;
                    warrantsCount++;
                }
                if ((zone.Level == eZoneLevel.Village && (getWarrants == null || getWarrants.Count == 0)) || zone.Level == eZoneLevel.Town)
                    continue;
                string info = getWarrants.Count == 0 ? string.Format("在{0}下未获取权证数据", zone.FullName) : string.Format("在{0}下成功导出{1}条权证", zone.FullName, index-1);
                this.ReportInfomation(info);
            }
            this.ReportProgress(100, "完成");
            this.ReportInfomation(string.Format("共导出{0}条权证信息", warrantsCount));
            allZones = null;
            tempAllZones = null;
        }

        //private

        #endregion

        #region 批量导出权证数据汇总表

        /// <summary>
        /// 导出权证数据汇总表
        /// </summary>
        /// <param name="currentZone">当前地域</param>
        /// <param name="childrenZone">子级地域</param>
        /// <param name="parentZone">父级地域</param>
        /// <param name="savePath">保存文件路径</param>
        /// <param name="bookBusiness">承包权证业务</param>
        public void ExportWarrentSummary(Zone currentZone, List<Zone> childrenZone, Zone parentZone, string savePath, ContractRegeditBookBusiness bookBusiness)
        {
            if (currentZone == null)
            {
                this.ReportError("未选择导出数据的地域!");
                return;
            }
            List<Zone> allZones = GetAllZones(currentZone, childrenZone, parentZone, savePath, bookBusiness);
            this.ReportProgress(1, "开始");
            this.ReportProgress(5, "正在获取权证");
            List<Zone> tempAllZones = new List<Zone>();
            allZones.ForEach(c => tempAllZones.Add(c));
            allZones.ForEach(c =>
            {
                //将大于当前选中地域的地域(集合)排除
                if (c.Level > currentZone.Level)
                    allZones.Remove(c);
            });
            List<Zone> zones = bookBusiness.ExistZones(allZones);  //存在权证数据的地域集合 
            double percent = 95 / (double)allZones.Count;
            int indexOfZone = 0;  //地域索引
            foreach (var zone in allZones)
            {
                string descZone = ExportZoneListDir(zone, tempAllZones);
                string folderString = CreateDirectoryByVilliage(tempAllZones, zone);
                string path = savePath + @"\" + folderString;
                if (zones.Exists(c => c.FullCode == zone.FullCode))
                    Directory.CreateDirectory(path);   //有数据则建立文件夹    
                List<ContractRegeditBook> books = bookBusiness.GetByZoneCode(zone.FullCode, eSearchOption.Precision);
                this.ReportProgress((int)(5 + percent * indexOfZone), string.Format("{0}", descZone));
                if (books != null && books.Count > 0)
                    bookBusiness.ExportWarrentSummaryTable(zone, path, percent, 5 + percent * indexOfZone);
                indexOfZone++;

                //提示信息
                if ((zone.Level == eZoneLevel.Town || zone.Level == eZoneLevel.Village) && books.Count == 0)
                {
                    //在镇、村下没有数据(提示信息不显示)
                    continue;
                }
                if (books.Count > 0)
                {
                    //地域下有数据
                    this.ReportInfomation(string.Format("{0}导出{1}条权证汇总数据", descZone, books.Count));
                }
                else
                {
                    //地域下无数据
                    this.ReportInfomation(string.Format("{0}无权证数据", descZone));
                }
            }
            this.ReportProgress(100, "完成");
            this.ReportInfomation(string.Format("共导出{0}个权证汇总表", zones.Count));
        }

        #endregion

        #region 批量导出颁证清册

        /// <summary>
        /// 导出颁证清册
        /// </summary>
        /// <param name="currentZone">当前地域</param>
        /// <param name="childrenZone">子级地域</param>
        /// <param name="parentZone">父级地域</param>
        /// <param name="savePath">保存文件路径</param>
        /// <param name="bookBusiness">承包权证业务</param>
        public void ExportAwareInventory(Zone currentZone, List<Zone> childrenZone, Zone parentZone, string savePath, ContractRegeditBookBusiness bookBusiness)
        {
            if (currentZone == null)
            {
                this.ReportError("未选择导出数据的地域!");
                return;
            }
            List<Zone> allZones = GetAllZones(currentZone, childrenZone, parentZone, savePath, bookBusiness);
            this.ReportProgress(1, "开始");
            this.ReportProgress(5, "正在获取合同(权证)");
            List<Zone> tempAllZones = new List<Zone>();
            allZones.ForEach(c => tempAllZones.Add(c));
            allZones.ForEach(c =>
            {
                //将大于当前选中地域的地域(集合)排除
                if (c.Level > currentZone.Level)
                    allZones.Remove(c);
            });
            List<Zone> zones = bookBusiness.ExistZones(allZones);  //存在权证数据的地域集合 
            double percent = 95 / (double)allZones.Count;
            int indexOfZone = 0;
            foreach (var zone in allZones)
            {
                string descZone = ExportZoneListDir(zone, tempAllZones);
                string folderString = CreateDirectoryByVilliage(tempAllZones, zone);
                string path = savePath + @"\" + folderString;
                if (zones.Exists(c => c.FullCode == zone.FullCode))
                    Directory.CreateDirectory(path);       //有合同(权证)数据的建立文件夹
                List<ContractRegeditBook> books = bookBusiness.GetByZoneCode(zone.FullCode, eSearchOption.Precision);
                this.ReportProgress((int)(5 + percent * indexOfZone), string.Format("{0}", descZone));
                if (books != null && books.Count > 0)
                    bookBusiness.ExportAwareInventoryTable(zone, path, percent, 5 + percent * indexOfZone);
                indexOfZone++;

                //提示信息
                if ((zone.Level == eZoneLevel.Town || zone.Level == eZoneLevel.Village) && books.Count == 0)
                {
                    //在镇、村下没有数据(提示信息不显示)
                    continue;
                }
                if (books.Count > 0)
                {
                    //地域下有数据
                    this.ReportInfomation(string.Format("{0}导出{1}条权证信息", descZone, books.Count));
                }
                else
                {
                    //地域下无数据
                    this.ReportInfomation(string.Format("{0}无权证数据", descZone));
                }
            }
            this.ReportProgress(100, "完成");
            this.ReportInfomation(string.Format("共导出{0}个颁证清册表", zones.Count));
        }

        #endregion

        #region 批量导出单户确认表

        /// <summary>
        /// 批量导出单户确认表
        /// </summary>
        /// <param name="currentZone">当前地域</param>
        /// <param name="childrenZone">子级地域</param>
        /// <param name="parentZone">父级地域</param>
        /// <param name="savePath">保存文件路径</param>
        /// <param name="bookBusiness">权证业务</param>
        public void ExportFamilyConfirm(Zone currentZone, List<Zone> childrenZone, Zone parentZone, string savePath, ContractRegeditBookBusiness bookBusiness)
        {
            if (currentZone == null)
            {
                this.ReportError("未选择导出数据的地域!");
                return;
            }
            List<Zone> allZones = GetAllZones(currentZone, childrenZone, parentZone, savePath, bookBusiness);
            this.ReportProgress(1, "开始");
            List<Zone> tempAllZones = new List<Zone>();
            allZones.ForEach(c => tempAllZones.Add(c));
            allZones.ForEach(c =>
            {
                //将大于当前选中地域的地域(集合)排除
                if (c.Level > currentZone.Level)
                    allZones.Remove(c);
            });
            List<Zone> zones = bookBusiness.ExistZones(allZones);  //存在权证数据的地域集合 
            double percent = 99 / (double)allZones.Count;
            int indexOfZone = 0;  //地域索引
            int count = 0;  //统计导出表个数
            foreach (var zone in allZones)
            {
                string descZone = ExportZoneListDir(zone, tempAllZones);
                string folderString = CreateDirectory(tempAllZones, zone);
                string path = savePath + @"\" + folderString;
                if (zones.Exists(c => c.FullCode == zone.FullCode))
                    Directory.CreateDirectory(path);  //有权证数据则建立文件夹
                List<VirtualPerson> listPerson = PersonBusiness.GetByZone(zone.FullCode);
                List<ContractRegeditBook> books = bookBusiness.GetByZoneCode(zone.FullCode, eSearchOption.Precision);
                this.ReportProgress((int)(1 + percent * indexOfZone), string.Format("{0}", descZone));
                if (books != null && books.Count > 0)
                    bookBusiness.ExportFamilyConfirmVolumn(zone, listPerson, path, false, percent, (1 + percent * indexOfZone));
                indexOfZone++;

                //提示信息
                if ((zone.Level == eZoneLevel.Town || zone.Level == eZoneLevel.Village) && books.Count == 0)
                {
                    //在镇、村下没有数据(提示信息不显示)
                    continue;
                }
                if (books.Count > 0)
                {
                    //地域下有数据
                    this.ReportInfomation(string.Format("{0}导出{1}条单户确认信息", descZone, listPerson.Count));
                    count += listPerson.Count;
                }
                else
                {
                    //地域下无数据
                    this.ReportInfomation(string.Format("{0}无权证数据", descZone));
                }
            }
            this.ReportProgress(100, "完成");
            this.ReportInfomation(string.Format("共导出{0}个单户确认表", count));
        }

        #endregion

        #region 批量导出登记薄

        /// <summary>
        /// 批量导出登记簿
        /// </summary>
        public void BatchExportRegeditBookWord(Zone currentZone, List<Zone> childrenZone, Zone parentZone, string savePath, ContractRegeditBookBusiness bookBusiness)
        {
            if (currentZone == null)
            {
                this.ReportError("未选择导出数据的地域!");
                return;
            }
            List<Zone> allZones = GetAllZones(currentZone, childrenZone, parentZone, savePath, bookBusiness);
            bookBusiness.ProgressChanged -= ReportPercent;
            int count = 0;      //统计可导出表格的个数         
            this.ReportProgress(0, "开始获取登记簿");
            double percent = 99.0 / (double)allZones.Count;
            double subprecent = 0.0;
            int zoneCount = 0;
            foreach (var zone in allZones)
            {
                string desc = ExportZoneListDir(zone, allZones);// 优化
                string folderString = CreateDirectory(allZones, zone);
                string path = savePath + @"\" + folderString;
                List<ContractRegeditBook> books = bookBusiness.GetByZoneCode(zone.FullCode, eSearchOption.Precision);
                if ((books == null || books.Count == 0))
                {
                    books = new List<ContractRegeditBook>();
                }
                subprecent = percent * (zoneCount++);
                this.ReportProgress((int)(subprecent), string.Format("{0}", desc));
                double temppercent = percent / (double)(books.Count == 0 ? 1 : books.Count);
                int index = 1;
                foreach (var regeditBookItem in books)
                {
                    if (regeditBookItem.RegeditNumber == "") 
                    {
                        if (regeditBookItem.Number != "")
                        {
                            YuLinTu.Library.Log.Log.WriteException(this, "导出登记簿", string.Format("权证号:{0}登记簿编号为空", regeditBookItem.Number));
                            continue;
                        }
                        else 
                        {                          
                            YuLinTu.Library.Log.Log.WriteException(this, "导出登记簿", string.Format("权证ID:{0}登记簿编号及权证编码为空，请检查数据", regeditBookItem.ID.ToString()));                           
                            continue;
                        }
                    }
                    ContractConcord concord = ConcordBusiness.Get(regeditBookItem.RegeditNumber);
                    if (concord == null) 
                    {
                        this.ReportInfomation(string.Format("登记簿号:{0}无对应合同", regeditBookItem.RegeditNumber));        
                        continue;
                    }
                    string filePath = path + @"\" + concord.ContracterName;
                    bookBusiness.PrivewRegeditBookWord(zone, concord, filePath, false);
                    this.ReportProgress((int)(subprecent + temppercent * index), string.Format("{0}", desc + concord.ContracterName));
                    index++;
                    count++;
                }
                if ((zone.Level == eZoneLevel.Village && (books == null || books.Count == 0)) || zone.Level == eZoneLevel.Town)
                    continue;
                string info = books.Count == 0 ? string.Format("在{0}下无登记簿数据", zone.FullName) : string.Format("在{0}下成功导出{1}条登记簿", zone.FullName, index-1);
                this.ReportInfomation(info);
            }
            this.ReportProgress(100, "完成");
            this.ReportInfomation(string.Format("共导出{0}个登记簿", count));
        }

        /// <summary>
        /// 根据选择的承包方批量导出登记簿
        /// </summary>
        /// <param name="currentZone"></param>
        /// <param name="childrenZone"></param>
        /// <param name="parentZone"></param>
        /// <param name="savePath"></param>
        /// <param name="bookBusiness"></param>
        /// <param name="listPerson"></param>
        public void ExportRegeditBookWord(Zone currentZone, List<Zone> childrenZone, Zone parentZone, string savePath, ContractRegeditBookBusiness bookBusiness, List<VirtualPerson> listPerson)
        {
            if (currentZone == null)
            {
                this.ReportError("未选择导出数据的地域!");
                return;
            }
            if (listPerson == null || listPerson.Count == 0)
            {
                this.ReportError("选择导出数据的地域无承包方!");
                return;
            }
            List<Zone> allZones = GetAllZones(currentZone, childrenZone, parentZone, savePath, bookBusiness);
            string folderString = CreateDirectory(allZones, currentZone);
            string path = savePath + @"\" + folderString;
            var dir = Directory.CreateDirectory(path);
            List<ContractRegeditBook> books = bookBusiness.GetByZoneCode(currentZone.FullCode, eSearchOption.Precision);
            List<ContractConcord> concordList = ConcordBusiness.GetCollection(currentZone.FullCode);
            bookBusiness.ProgressChanged -= ReportPercent;
            this.ReportProgress(1, "开始");
            int count = 0;   //统计可导出表格的个数
            this.ReportProgress(5, "获取登记簿");
            double percent = 90.0 / (double)listPerson.Count;
            int index = 1; 
            foreach (var person in listPerson)
            {
                List<ContractConcord> vpConcordList = concordList.FindAll(c => c.ContracterId == person.ID);
                foreach (var cocord in vpConcordList)
                {
                    if (books.Exists(c => c.RegeditNumber == cocord.ConcordNumber))
                    {
                        string filePath = path + @"\" + cocord.ContracterName;
                        bookBusiness.PrivewRegeditBookWord(currentZone, cocord, filePath, false);
                        count++;
                    }
                    this.ReportInfomation(string.Format("成功导出承包方{0}的登记簿", cocord.ContracterName));
                }
                string desc = bookBusiness.GetMarkDesc(currentZone);
                this.ReportProgress((int)(5 + percent * index), string.Format("{0}", desc+person.Name));
                index++;
                vpConcordList = null;
            }
            this.ReportProgress(100, "完成");
            this.ReportInfomation(string.Format("共导出{0}个登记簿", count));
            books = null;
            concordList = null;
            allZones = null;
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
