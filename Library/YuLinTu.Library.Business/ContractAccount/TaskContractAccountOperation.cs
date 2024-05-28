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

namespace YuLinTu.Library.Business
{
    /// <summary>
    /// 承包台账数据操作任务类
    /// </summary>
    public class TaskContractAccountOperation : Task
    {
        #region Fields

        //private PublicityConfirmDefine contractLandOutputSurveyDefine;
        //private FamilyOutputDefine familyOutputSet;
        //private FamilyOtherDefine familyOtherSet;
        //private SingleFamilySurveyDefine singleFamilySurveyDefine;
        private object returnValue;

        #endregion Fields

        #region Properties

        /// <summary>
        /// 承包台账常规设置实体
        /// </summary>
        public ContractBusinessSettingDefine SettingDefine = ContractBusinessSettingDefine.GetIntence();

        /// <summary>
        /// 导入界址点图斑设置实体
        /// </summary>

        /// <summary>
        /// 导出公示调查表日期设置
        /// </summary>
        public DateSetting PublishDateSetting { get; set; }

        /// <summary>
        /// 返回值
        /// </summary>
        public object ReturnValue
        {
            get { return returnValue; }
        }

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

        /// <summary>
        /// 选中的承包方集合
        /// </summary>
        public List<VirtualPerson> SelectedPersons { get; set; }

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
            returnValue = null;
            TaskContractAccountArgument metadata = Argument as TaskContractAccountArgument;
            if (metadata == null)
            {
                return;
            }
            string fileName = metadata.FileName;
            bool isClear = metadata.IsClear;
            dbContext = metadata.Database;
            Zone zone = metadata.CurrentZone;
            SelectContractor = metadata.SelectContractor;
            AccountLandBusiness landBusiness = new AccountLandBusiness(dbContext);
            landBusiness.meta = metadata;
            landBusiness.VirtualType = metadata.VirtualType;
            landBusiness.Alert += ReportInfo;
            landBusiness.ProgressChanged += ReportPercent;
            landBusiness.TableType = metadata.TableType;
            landBusiness.ArgType = metadata.ArgType;
            landBusiness.PublishDateSetting = this.PublishDateSetting;
            landBusiness.IsBatch = IsBatch;
            landBusiness.UseContractorInfoImport = metadata.UseContractorInfoImport;
            landBusiness.UseLandCodeBindImport = metadata.UseLandCodeBindImport;
            landBusiness.UseContractorNumberImport = metadata.UseContractorNumberImport;
            landBusiness.UseOldLandCodeBindImport = metadata.UseOldLandCodeBindImport;
            landBusiness.shapeAllcolNameList = metadata.shapeAllcolNameList;
            var zoneStation = dbContext.CreateZoneWorkStation();
            List<Zone> childrenZone = zoneStation.GetChildren(zone.FullCode, eLevelOption.Subs);
            Zone parent = landBusiness.GetParent(zone);
            switch (metadata.ArgType)
            {
                case eContractAccountType.ExportContractAccountExcel:
                    //导出4个表数据
                    ExportFourKindTable(zone, childrenZone, parent, fileName, landBusiness);
                    break;

                case eContractAccountType.ExportSingleFamilySurveyExcel:
                    if (zone.Level > eZoneLevel.Group)
                    {
                        //批量导出承包台账单户确认表
                        ExportSingleFamilyExcelByZone(zone, childrenZone, parent, fileName, landBusiness, eContractAccountType.ExportSingleFamilySurveyExcel);
                    }
                    else
                    {
                        //导出当前地域下的承包台账单户调查表
                        landBusiness.ExportSingleFamilySurveyExcel(zone, SelectContractor, fileName);
                    }
                    break;

                case eContractAccountType.ExportSingleFamilyConfirmExcel:
                    if (zone.Level > eZoneLevel.Group)
                    {
                        //批量导出承包台账单户确认表
                        ExportSingleFamilyExcelByZone(zone, childrenZone, parent, fileName, landBusiness, eContractAccountType.ExportSingleFamilyConfirmExcel);
                    }
                    else
                    {
                        //导出当前地域下的承包方单户确认表信息
                        landBusiness.ExportSingleFamilyConfirmExcel(zone, SelectContractor, fileName);
                    }
                    break;

                case eContractAccountType.ImportData:
                    //导入表
                    //landBusiness.ImportData(zone, fileName, isClear, true, 80, 20);
                    returnValue = zone.FullCode;
                    break;

                case eContractAccountType.VolumnImport:
                    //批量导入表
                    landBusiness.BatchImport(zone, fileName, isClear);
                    returnValue = zone.FullCode;
                    break;

                case eContractAccountType.ImportLandShapeData:
                    //导入承包地块shape图斑数据及相关信息
                    ImportLandDataShape(zone, childrenZone, fileName, landBusiness);
                    break;

                case eContractAccountType.VolumnExportSurveyTable:
                    if (zone.Level > eZoneLevel.Group && childrenZone != null && childrenZone.Count > 0)
                    {
                        //批量导出公示结果归户表
                        VolumnExportPublishWord(zone, childrenZone, parent, fileName, landBusiness);
                    }
                    else
                    {
                        //根据选择的承包方导出公示结果归户表
                        ExportPublishWord(zone, childrenZone, parent, fileName, landBusiness, SelectContractor);
                    }
                    break;

                case eContractAccountType.VolumnExportLandSurveyTable:
                    if (zone.Level > eZoneLevel.Group && childrenZone != null && childrenZone.Count > 0)
                    {
                        //批量导出地块调查表
                        VolumnExportLandSureyWord(zone, childrenZone, parent, fileName, landBusiness);
                    }
                    else
                    {
                        //根据选择的承包方导出地块调查表
                        ExportLandSurveyWord(zone, childrenZone, parent, fileName, landBusiness, SelectContractor);
                    }
                    break;

                case eContractAccountType.VolumnExportContractorTable:
                    if (zone.Level > eZoneLevel.Group && childrenZone != null && childrenZone.Count > 0)
                    {
                        //批量导出承包方调查表
                        VolumnExportContractorWord(zone, childrenZone, parent, fileName, landBusiness);
                    }
                    else
                    {
                        //根据选择的承包方导出承包方调查表
                        ExportContractorWord(zone, childrenZone, parent, fileName, landBusiness, SelectContractor);
                    }
                    break;

                case eContractAccountType.VolumnExportPublishTable:
                    //批量导出调查公示表
                    ExportPublicyResultExcelByZoneZone(zone, childrenZone, parent, fileName, landBusiness);
                    break;

                case eContractAccountType.ExportSendTableWord:
                    //发包方调查表（Word）
                    ExportSendTableWord(zone, childrenZone, parent, fileName, landBusiness);
                    //landBusiness.ExportSenderWord(zone, fileName);
                    break;

                case eContractAccountType.ExportSendTableExcel:
                    //发包方调查表（Excel）
                    landBusiness.ExportSenderExcel(zone, fileName);
                    break;

                case eContractAccountType.VolumnExportVirtualPersonExcel:
                    //批量导出承包方调查表（Excel）
                    ExportVPSurveyExcelByZone(zone, childrenZone, parent, fileName, landBusiness);
                    break;

                case eContractAccountType.ExportSummaryExcel:
                    //批量导出数据汇总表(Excel)
                    ExportSummaryExcelByZone(zone, childrenZone, parent, fileName, landBusiness);
                    break;

                case eContractAccountType.ExportVillageDeclare:
                    //批量导出村组公示公告Word
                    ExportVillagesDeclareWordByZone(zone, childrenZone, parent, fileName, landBusiness);
                    break;

                case eContractAccountType.InitialLandTool:
                    if (zone.Level > eZoneLevel.Group && childrenZone != null && childrenZone.Count > 0)
                    {
                        //此时为批量初始化(最大支持镇级行政区域)
                        InitialDataVolumn(zone, childrenZone, parent, landBusiness, metadata);
                    }
                    else
                    {
                        //此时仅初始化当前选择地域下的所有承包地块属性信息
                        this.ReportProgress(1, "开始");
                        //landBusiness.ContractLandInitialTool(metadata, metadata.CurrentZoneLandList, zone, 99, 1, true);
                    }
                    break;

                case eContractAccountType.InitialAreaTool:
                    if (zone.Level > eZoneLevel.Group && childrenZone != null && childrenZone.Count > 0)
                    {
                        //此时为批量初始化地块面积(最大支持镇级行政区域)
                        InitialDataVolumn(zone, childrenZone, parent, landBusiness, metadata);
                    }
                    else
                    {
                        //此时仅初始化当前选择地域下的所有承包地块(具有空间位置信息)的面积
                        this.ReportProgress(1, "开始");
                        //landBusiness.ContractLandAreaInitialTool(metadata, metadata.CurrentZoneLandList, zone, 99, 1, true);
                    }
                    break;

                case eContractAccountType.InitialAreaNumericFormatTool:
                    if (zone.Level > eZoneLevel.Group && childrenZone != null && childrenZone.Count > 0)
                    {
                        //此时为批量初始化地块面积(最大支持镇级行政区域)
                        InitialDataVolumn(zone, childrenZone, parent, landBusiness, metadata);
                    }
                    else
                    {
                        //此时仅初始化当前选择地域下的所有承包地块(具有空间位置信息)的面积
                        this.ReportProgress(1, "开始");
                        //landBusiness.ContractLandAreaNumericFormatTool(metadata, metadata.CurrentZoneLandList, zone, 99, 1, true);
                    }
                    break;

                case eContractAccountType.InitialIsFarmerTool:
                    if (zone.Level > eZoneLevel.Group && childrenZone != null && childrenZone.Count > 0)
                    {
                        //此时为批量初始化地块是否基本农田(最大支持镇级行政区域)
                        InitialDataVolumn(zone, childrenZone, parent, landBusiness, metadata);
                    }
                    else
                    {
                        //此时仅初始化当前选择地域下的所有承包地块(具有空间位置信息)是否基本农田
                        //landBusiness.ContractLandIsFarmerInitialTool(metadata, metadata.CurrentZoneLandList, zone, 99, 1, true);
                    }
                    break;

                case eContractAccountType.ImportDotShapeData:
                    //导入界址点图斑数据
                    List<Zone> entireZones = GetAllZones(zone, childrenZone, parent, landBusiness);
                    ImportDotDataShape(fileName, zone, entireZones);
                    break;
            }

            GC.Collect();
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

        #region Methods - Privates-承包台账地块图斑导入

        /// <summary>
        /// 导入承包方shape图斑数据
        /// </summary>
        private void ImportLandDataShape(Zone currentZone, List<Zone> childrenZone, string fileName, AccountLandBusiness accountBusiness)
        {
            //获取当前路径下shape数据
            IList landShapeList = null;
            string filepath = string.Empty;
            string filename = string.Empty;
            IDbContext ds = null;
            try
            {
                filepath = System.IO.Path.GetDirectoryName(fileName);
                filename = System.IO.Path.GetFileNameWithoutExtension(fileName);
                ds = ProviderShapefile.CreateDataSource(filepath, false) as IDbContext;
                var dq = new DynamicQuery(ds);
                var landdata = dq.Get(null, filename).Result as IList;
                landShapeList = landdata;
            }
            catch (Exception ex)
            {
                this.ReportError("当前打开Shape文件有误，请检查文件是否正确 " + ex.Message);
                return;
            }

            var importShpType = ds.DataSource as IProviderShapefile;
            if (importShpType.GetGeometryType(filename) != Spatial.eGeometryType.Polygon)
            {
                this.ReportError("当前Shape文件不为面文件，请重新选择面文件导入");
                return;
            }
            if (landShapeList.Count == 0)
            {
                this.ReportInfomation("当前导入文件没有数据");
                return;
            }
            var dps = ds.DataSource as ProviderShapefile;
            YuLinTu.Spatial.SpatialReference shpRef = dps.GetSpatialReference(filename);
            accountBusiness.shpRef = shpRef;

            if (landShapeList.Count == 0 || landShapeList == null)
            {
                return;
            }
            double currentPercent = 0.0;
            if (currentZone.Level == eZoneLevel.Village)
            {
                if (childrenZone == null)
                {
                    var zoneStation = dbContext.CreateZoneWorkStation();
                    childrenZone = zoneStation.GetChildren(currentZone.FullCode, eLevelOption.SelfAndSubs);
                }
                else
                {
                    childrenZone.Add(currentZone);
                }
                int importCount = 0;//总数
                int importZoneCount = 0;//各组个数
                this.ReportProgress(0, "开始");
                double indexCount = (double)childrenZone.Count / 95;//均分进度
                int zoneCurrentIndex = 0;//进度索引

                foreach (var itemZone in childrenZone)
                {
                    currentPercent = 5 + indexCount * zoneCurrentIndex;
                    importZoneCount = accountBusiness.ImportLandShapeDataInfo(itemZone, landShapeList, currentPercent, indexCount);
                    this.ReportProgress((int)currentPercent, string.Format("导入{0}下地块", itemZone.FullName));
                    zoneCurrentIndex++;
                    importCount = importCount + importZoneCount;
                }
                if (importCount != 0)
                {
                    this.ReportProgress(100, "完成");
                    this.ReportInfomation(string.Format("{0}共导入{1}条信息", currentZone.Name, importCount));
                }
                currentPercent = 0;
            }
            else if (currentZone.Level == eZoneLevel.Group)
            {
                int importCount = 0;
                this.ReportProgress(0, "开始");
                importCount = accountBusiness.ImportLandShapeDataInfo(currentZone, landShapeList, currentPercent);
                if (importCount != 0)
                {
                    this.ReportProgress(100, "完成");
                    this.ReportInfomation(string.Format("{0}共导入{1}条信息", currentZone.Name, importCount));
                }
                currentPercent = 0;
            }

            landShapeList = null;
        }

        #endregion Methods - Privates-承包台账地块图斑导入

        #region Methods -承包台账界址点图斑导入

        /// <summary>
        /// 导入承包台账界址点图斑
        /// </summary>
        public void ImportDotDataShape(string fileName, Zone currentZone, List<Zone> entireZones)
        {
            string markDesc = ExportZoneListDir(currentZone, entireZones);
            var dbContext = DataBaseSource.GetDataBaseSource();
            if (dbContext == null)
            {
                this.ReportError(DataBaseSource.ConnectionError);
                return;
            }
            var landStaion = dbContext.CreateContractLandWorkstation();
            List<ContractLand> listLand = new List<ContractLand>();
            try
            {
                listLand = landStaion.GetCollection(currentZone.FullCode, eLevelOption.Self);
            }
            catch (Exception ex)
            {
                YuLinTu.Library.Log.Log.WriteException(this, "GetCollection(获取当前地域下的承包地块失败)", ex.Message + ex.StackTrace);
                this.ReportError("获取当前地域下的承包地块失败," + ex.Message);
            }
            if (listLand.Count == 0)
            {
                this.ReportError(string.Format("{0}地域无承包地块", markDesc));
                return;
            }
            ImportBoundaryAddressDot importDot = new ImportBoundaryAddressDot();
            importDot.ProgressChanged += ReportPercent;
            importDot.Alert += ReportInfo;
            importDot.FileName = fileName;
            importDot.CurrentZone = currentZone;
            importDot.ListLand = listLand;
            importDot.MarkDesc = markDesc;
            importDot.CreateImportBoundaryDotTask();
        }

        #endregion Methods -承包台账界址点图斑导入

        #region Methods - 导出调查表

        /// <summary>
        /// 导出发包方调查表
        /// </summary>
        private void ExportSendTableWord(Zone currentZone, List<Zone> childrenZone, Zone parent, string fileName, AccountLandBusiness accountBusiness)
        {
            if (currentZone == null)
            {
                this.ReportError("未选择导出数据的地域!");
                return;
            }
            List<Zone> allZones = GetAllZones(currentZone, childrenZone, parent, accountBusiness);
            List<Zone> tempAllZones = new List<Zone>();
            allZones.ForEach(c => tempAllZones.Add(c));
            foreach (var item in tempAllZones)
            {
                if (item.Level > currentZone.Level)
                {
                    allZones.Remove(item);
                }
            }
            accountBusiness.ProgressChanged -= ReportPercent;
            int count = 0;      //统计可导出表格的个数
            this.ReportProgress(0, "开始导出发包方调查表");
            double percent = 99.0 / (double)allZones.Count;
            double subprecent = 0.0;
            int zoneCount = 0;
            foreach (var zone in allZones)
            {
                string desc = ExportZoneListDir(zone, tempAllZones);
                string folderString = CreateDirectory(tempAllZones, zone);
                string path = fileName + @"\" + folderString;
                List<CollectivityTissue> list = GetTissueCollection(zone);
                int index = 1;
                double temppercent = percent / (double)(list.Count == 0 ? 1 : list.Count);
                subprecent = percent * (zoneCount++);
                this.ReportProgress((int)(subprecent), string.Format("{0}", desc));
                foreach (CollectivityTissue tissue in list)
                {
                    accountBusiness.ExportSenderWord(zone, tissue, path);
                    this.ReportProgress(((int)(subprecent + temppercent * index)), "导出" + tissue.Name);
                    index++;
                    count++;
                }
                string info = string.Format("{0}成功导出{1}发包方调查表", zone.FullName, index - 1);
                this.ReportInfomation(info);
                list = null;
            }
            this.ReportProgress(100, "完成");
            this.ReportInfomation(string.Format("{0}成功导出{1}个发包方调查表", currentZone.FullName, count));
        }

        /// <summary>
        /// 批量导出导出公示结果归户表(Word)
        /// </summary>
        private void VolumnExportPublishWord(Zone currentZone, List<Zone> childrenZone, Zone parent, string fileName, AccountLandBusiness accountBusiness)
        {
            if (currentZone == null)
            {
                this.ReportError("未选择导出数据的地域!");
                return;
            }
            List<Zone> allZones = GetAllZones(currentZone, childrenZone, parent, accountBusiness);
            accountBusiness.ProgressChanged -= ReportPercent;
            int count = 0;      //统计可导出表格的个数
            this.ReportProgress(0, "开始导出公示结果归户表");
            double percent = 99.0 / (double)allZones.Count;
            double subprecent = 0.0;
            int zoneCount = 0;
            foreach (var zone in allZones)
            {
                string desc = ExportZoneListDir(zone, allZones);
                string folderString = CreateDirectory(allZones, zone);
                string path = fileName + @"\" + folderString;
                List<VirtualPerson> vps = accountBusiness.GetByZone(zone.FullCode);
                if (vps == null || vps.Count == 0)
                {
                    vps = new List<VirtualPerson>();
                }
                subprecent = percent * (zoneCount++);
                this.ReportProgress((int)(subprecent), string.Format("{0}", desc));
                double temppercent = percent / (double)(vps.Count == 0 ? 1 : vps.Count);
                int index = 1;
                List<ContractLand> lands = new List<ContractLand>();
                // var stockLands = accountBusiness.GetStockRightLand(currentZone);
                foreach (var vp in vps)
                {
                    lands = accountBusiness.GetPersonCollection(vp.ID);
                    //if(vp.IsStockFarmer)
                    //lands.AddRange(stockLands);
                    if (lands == null || lands.Count == 0)
                    {
                        continue;
                    }
                    accountBusiness.ExportPublishWord(zone, vp, lands, path);
                    this.ReportProgress((int)(subprecent + temppercent * index), string.Format("{0}", desc + vp.Name));
                    index++;
                    count++;
                }
                if ((zone.Level == eZoneLevel.Village && (vps == null || vps.Count == 0)) || zone.Level == eZoneLevel.Town)
                    continue;
                string info = index == 1 ? string.Format("{0}无承包方数据", zone.FullName) : string.Format("{0}成功导出{1}公示结果归户表", zone.FullName, index - 1);
                this.ReportInfomation(info);
            }
            this.ReportProgress(100, "完成");
            this.ReportInfomation(string.Format("{0}成功导出{1}个", currentZone.FullName, count));
            allZones = null;
        }

        /// <summary>
        /// 根据选择的承包方选择批量导出公示结果归户表
        /// </summary>
        public void ExportPublishWord(Zone currentZone, List<Zone> childrenZone, Zone parent, string savePath, AccountLandBusiness accountBusiness, List<VirtualPerson> listPerson)
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
            List<Zone> allZones = GetAllZones(currentZone, childrenZone, parent, accountBusiness);
            string folderString = CreateDirectory(allZones, currentZone);
            string path = savePath + @"\" + folderString;
            var dir = Directory.CreateDirectory(path);
            accountBusiness.ProgressChanged -= ReportPercent;
            string desc = ExportZoneListDir(currentZone, allZones);
            this.ReportProgress(0, "开始导出公示结果归户表");
            double percent = 99.0 / (double)listPerson.Count;
            int index = 1;
            List<ContractLand> lands = new List<ContractLand>();
            foreach (var vp in listPerson)
            {
                lands = accountBusiness.GetPersonCollection(vp.ID);
                string filepath = path + @"\" + vp.Name;
                accountBusiness.ExportPublishWord(currentZone, vp, lands, filepath);
                this.ReportProgress((int)(percent * index), string.Format("{0}", desc + vp.Name));
                index++;
            }
            this.ReportProgress(100, "完成");
            this.ReportInfomation(string.Format("从{0}下成功导出{1}个公示结果归户表", currentZone.FullName, index - 1));
            lands = null;
        }

        /// <summary>
        /// 批量导出地块调查表（Word）
        /// </summary>
        private void VolumnExportLandSureyWord(Zone currentZone, List<Zone> childrenZone, Zone parent, string fileName, AccountLandBusiness accountBusiness)
        {
            if (currentZone == null)
            {
                this.ReportError("未选择导出数据的地域!");
                return;
            }
            List<Zone> allZones = GetAllZones(currentZone, childrenZone, parent, accountBusiness);
            accountBusiness.ProgressChanged -= ReportPercent;
            int count = 0;      //统计可导出表格的个数
            this.ReportProgress(0, "开始导出地块调查表");
            double percent = 99.0 / (double)allZones.Count;
            double subprecent = 0.0;
            int zoneCount = 0;
            foreach (var zone in allZones)
            {
                string desc = ExportZoneListDir(zone, allZones);
                string folderString = CreateDirectory(allZones, zone);
                string path = fileName + @"\" + folderString;
                List<VirtualPerson> vps = accountBusiness.GetByZone(zone.FullCode);
                if (vps == null || vps.Count == 0)
                {
                    vps = new List<VirtualPerson>();
                }
                subprecent = percent * (zoneCount++);
                this.ReportProgress((int)(subprecent), string.Format("{0}", desc));
                double temppercent = percent / (double)(vps.Count == 0 ? 1 : vps.Count);
                int index = 1;
                int countlands = 0;
                List<ContractLand> lands = new List<ContractLand>();
                foreach (var vp in vps)
                {
                    lands = accountBusiness.GetPersonCollection(vp.ID);
                    if (lands == null || lands.Count == 0)
                    {
                        continue;
                    }
                    foreach (var item in lands)
                    {
                        string filepath = path + @"\" + vp.Name;
                        accountBusiness.ExportLandWord(zone, item, vp, filepath);
                        this.ReportProgress((int)(subprecent + temppercent * index), string.Format("{0}", desc + vp.Name));
                        count++;
                    }
                    countlands += lands.Count;
                    index++;
                }
                if ((zone.Level == eZoneLevel.Village && (vps == null || vps.Count == 0)) || zone.Level == eZoneLevel.Town)
                    continue;
                string info = index == 1 ? string.Format("在{0}无承包方数据", zone.FullName) : string.Format("在{0}下成功导出{1}个承包方总共{2}地块调查表", zone.FullName, index - 1, countlands);
                this.ReportInfomation(info);
            }
            this.ReportProgress(100, "完成");
            this.ReportInfomation(string.Format("从{0}下成功导出{1}个地块调查表", currentZone.FullName, count));
            allZones = null;
        }

        /// <summary>
        /// 批量导出选中的承包方地块调查表
        /// </summary>
        private void ExportLandSurveyWord(Zone currentZone, List<Zone> childrenZone, Zone parent, string savePath, AccountLandBusiness accountBusiness, List<VirtualPerson> listPerson)
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
            List<Zone> allZones = GetAllZones(currentZone, childrenZone, parent, accountBusiness);
            string folderString = CreateDirectory(allZones, currentZone);
            string path = savePath + @"\" + folderString;
            var dir = Directory.CreateDirectory(path);
            accountBusiness.ProgressChanged -= ReportPercent;
            string desc = ExportZoneListDir(currentZone, allZones);
            int count = 0;      //统计可导出表格的个数
            this.ReportProgress(0, "开始导出地块调查表");
            double percent = 99.0 / (double)listPerson.Count;
            int index = 1;
            List<ContractLand> lands = new List<ContractLand>();
            foreach (var vp in listPerson)
            {
                lands = accountBusiness.GetPersonCollection(vp.ID);
                if (lands == null || lands.Count == 0)
                {
                    continue;
                }
                foreach (var item in lands)
                {
                    string filepath = path + @"\" + vp.Name;
                    accountBusiness.ExportLandWord(currentZone, item, vp, filepath);
                    this.ReportProgress((int)(percent * index), string.Format("{0}", desc + vp.Name));
                    count++;
                }
                index++;
                this.ReportInfomation(string.Format("从{0}下导出{1}个地块调查表", currentZone.FullName + vp.Name, lands.Count));
            }
            this.ReportProgress(100, "完成");
            this.ReportInfomation(string.Format("从{0}下成功导出{1}个承包方导出{2}个地块调查表", currentZone.FullName, index - 1, count));
            lands = null;
        }

        /// <summary>
        /// 批量导出调查公示表
        /// </summary>
        public void ExportPublicyResultExcelByZoneZone(Zone currentZone, List<Zone> childrenZone, Zone parentZone, string savePath, AccountLandBusiness accountBusiness)
        {
            VirtualPersonBusiness business = new VirtualPersonBusiness(dbContext);
            business.VirtualType = eVirtualType.Land;
            if (currentZone == null)
            {
                this.ReportError("未选择导出数据的地域!");
                return;
            }
            List<Zone> allZones = GetAllZones(currentZone, childrenZone, parentZone, accountBusiness);
            this.ReportProgress(1, "开始");
            this.ReportProgress(5, "正在获取承包方");
            List<Zone> tempAllZones = new List<Zone>();
            allZones.ForEach(c => tempAllZones.Add(c));
            //allZones.ForEach(c =>
            //{
            //    //将大于当前选中地域的地域(集合)排除
            //    if (c.Level > currentZone.Level)
            //        allZones.Remove(c);
            //});
            allZones.RemoveAll(c => c.Level > currentZone.Level);
            List<Zone> zones = business.GetExsitZones(allZones);  //存在承包方数据的地域集合
            double percent = 95 / (double)allZones.Count;
            int indexOfZone = 0;  //地域索引
            foreach (var zone in allZones)
            {
                string descZone = ExportZoneListDir(zone, tempAllZones);
                string folderString = CreateDirectoryByVilliage(tempAllZones, zone);
                string path = savePath + @"\" + folderString;
                if (zones.Exists(c => c.FullCode == zone.FullCode))
                    Directory.CreateDirectory(path);   //有数据则建立文件夹
                List<VirtualPerson> persons = accountBusiness.GetByZone(zone.FullCode);
                this.ReportProgress((int)(5 + percent * indexOfZone), string.Format("{0}", descZone));
                if (persons != null && persons.Count > 0)
                    accountBusiness.ExportPublishExcel(zone, path, percent, 5 + percent * indexOfZone);
                indexOfZone++;

                //提示信息
                if ((zone.Level == eZoneLevel.Town || zone.Level == eZoneLevel.Village) && persons.Count == 0)
                {
                    //在镇、村下没有承包方数据(提示信息不显示)
                    continue;
                }
                if (persons.Count == 0)
                {
                    //地域下无数据
                    this.ReportInfomation(string.Format("{0}无承包方数据", descZone));
                }
            }
            this.ReportProgress(100, "完成");
            this.ReportInfomation(string.Format("共导出{0}个调查信息公示调查表", zones.Count));
        }

        /// <summary>
        /// 批量导出承包方调查表
        /// </summary>
        public void VolumnExportContractorWord(Zone currentZone, List<Zone> childrenZone, Zone parent, string fileName, AccountLandBusiness accountBusiness)
        {
            if (currentZone == null)
            {
                this.ReportError("未选择导出数据的地域!");
                return;
            }
            List<Zone> allZones = GetAllZones(currentZone, childrenZone, parent, accountBusiness);
            accountBusiness.ProgressChanged -= ReportPercent;
            int count = 0;      //统计可导出表格的个数
            this.ReportProgress(0, "开始导出承包方调查表");
            double percent = 99.0 / (double)allZones.Count;
            double subprecent = 0.0;
            int zoneCount = 0;
            foreach (var zone in allZones)
            {
                string desc = ExportZoneListDir(zone, allZones);
                string folderString = CreateDirectory(allZones, zone);
                string path = fileName + @"\" + folderString;
                List<VirtualPerson> vps = accountBusiness.GetByZone(zone.FullCode);
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
                    //是否显示集体户信息
                    if (vp.Name.IndexOf("机动地") >= 0 || vp.Name.IndexOf("集体") >= 0)
                    {
                        continue;
                    }
                    bool flag = accountBusiness.ExportVPWord(zone, vp, path, desc);
                    if (flag == true)
                    {
                        this.ReportProgress((int)(subprecent + temppercent * index), string.Format("{0}", desc + vp.Name));
                        index++;
                        count++;
                    }
                }
                if ((zone.Level == eZoneLevel.Village && (vps == null || vps.Count == 0)) || zone.Level == eZoneLevel.Town)
                    continue;
                string info = index == 1 ? string.Format("在{0}无承包方数据", zone.FullName) : string.Format("在{0}下成功导出{1}个承包方调查表", zone.FullName, index - 1);
                this.ReportInfomation(info);
            }
            this.ReportProgress(100, "完成");
            this.ReportInfomation(string.Format("共导出{0}个承包方调查表", count));
        }

        /// <summary>
        /// 根据选择的承包方导出承包方调查表
        /// </summary>>
        public void ExportContractorWord(Zone currentZone, List<Zone> childrenZone, Zone parent, string fileName, AccountLandBusiness accountBusiness, List<VirtualPerson> listPerson)
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
            List<Zone> allZones = GetAllZones(currentZone, childrenZone, parent, accountBusiness);
            string folderString = CreateDirectory(allZones, currentZone);
            string path = fileName + @"\" + folderString;
            var dir = Directory.CreateDirectory(path);
            accountBusiness.ProgressChanged -= ReportPercent;
            string desc = ExportZoneListDir(currentZone, allZones);
            this.ReportProgress(0, "开始导出承包方调查表");
            double percent = 99.0 / (double)listPerson.Count;
            int index = 1;
            foreach (var vp in listPerson)
            {
                //是否显示集体户信息
                if (vp.Name.IndexOf("机动地") >= 0 || vp.Name.IndexOf("集体") >= 0)
                {
                    continue;
                }
                accountBusiness.ExportVPWord(currentZone, vp, path, desc);
                this.ReportProgress((int)(percent * index), string.Format("{0}", desc + vp.Name));
                index++;
            }
            this.ReportProgress(100, "完成");
            this.ReportInfomation(string.Format("从{0}下成功导出{1}个承包方调查表", currentZone.FullName, index - 1));
        }

        /// <summary>
        /// 批量导出承包方Excel调查表
        /// </summary>
        /// <param name="currentZone">当前地域</param>
        /// <param name="childrenZone">子级地域</param>
        /// <param name="parentZone">父级地域</param>
        /// <param name="savePath">保存文件路径</param>
        /// <param name="personBusiness">承包方业务</param>
        public void ExportVPSurveyExcelByZone(Zone currentZone, List<Zone> childrenZone, Zone parentZone, string savePath, AccountLandBusiness accountBusiness)
        {
            VirtualPersonBusiness business = new VirtualPersonBusiness(dbContext);
            business.VirtualType = eVirtualType.Land;
            if (currentZone == null)
            {
                this.ReportError("未选择导出数据的地域!");
                return;
            }
            List<Zone> allZones = GetAllZones(currentZone, childrenZone, parentZone, accountBusiness);
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
            List<Zone> zones = business.GetExsitZones(allZones);  //存在承包方数据的地域集合
            double percent = 95 / (double)allZones.Count;
            int indexOfZone = 0;  //地域索引
            foreach (var zone in allZones)
            {
                string descZone = ExportZoneListDir(zone, tempAllZones);
                string folderString = CreateDirectoryByVilliage(tempAllZones, zone);
                string path = savePath + @"\" + folderString;
                if (zones.Exists(c => c.FullCode == zone.FullCode))
                    Directory.CreateDirectory(path);   //有数据则建立文件夹
                List<VirtualPerson> persons = accountBusiness.GetByZone(zone.FullCode);
                this.ReportProgress((int)(5 + percent * indexOfZone), string.Format("{0}", descZone));
                if (persons != null && persons.Count > 0)
                    accountBusiness.ExportVirtualPersonExcel(zone, path, percent, 5 + percent * indexOfZone);
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

        #endregion Methods - 导出调查表

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
            List<Zone> allZones = GetAllZones(currentZone, childrenZone, parentZone, business);
            //business.ProgressChanged -= ReportPercent;
            this.ReportProgress(1, "开始");
            this.ReportProgress(5, "正在获取表");
            List<Zone> tempAllZones = new List<Zone>();
            allZones.ForEach(c => tempAllZones.Add(c));
            tempAllZones.ForEach(c =>
            {
                //将大于当前选中地域的地域(集合)排除
                if (c.Level > currentZone.Level)
                    allZones.Remove(c);
            });
            double percent = 95 / (double)allZones.Count;
            int indexOfZone = 0;  //地域索引
            int dataCount = 0;//有数据表个数
            foreach (var zone in allZones)
            {
                string descZone = ExportZoneListDir(zone, tempAllZones);
                string folderString = CreateDirectoryByVilliage(tempAllZones, zone);
                string path = savePath + @"\" + folderString;
                bool hasperson = ExitsPerson(zone);
                this.ReportProgress((int)(5 + percent * indexOfZone), string.Format("{0}", descZone));
                if (hasperson)//有数据则建立文件夹
                {
                    Directory.CreateDirectory(path);
                    business.ExportDataExcel(zone, path, percent, 5 + percent * (indexOfZone));
                    this.ReportInfomation(string.Format("{0}导出{1}个调查表数据", descZone, 1));
                    dataCount++;
                }
                indexOfZone++;
                if ((zone.Level == eZoneLevel.Town || zone.Level == eZoneLevel.Village) && !hasperson)
                {
                    //在镇、村下没有数据(提示信息不显示)
                    continue;
                }
                if (zone.Level == eZoneLevel.Group && !hasperson)
                {
                    //地域下无数据
                    this.ReportInfomation(string.Format("{0}无数据", descZone));
                }
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
        private void ExportSingleFamilyExcelByZone(Zone currentZone, List<Zone> childrenZone, Zone parentZone, string savePath, AccountLandBusiness business, eContractAccountType exportType)
        {
            if (currentZone == null)
            {
                this.ReportError("未选择导出数据的地域!");
                return;
            }
            List<Zone> allZones = GetAllZones(currentZone, childrenZone, parentZone, business);
            List<Zone> tempAllZones = new List<Zone>();
            allZones.ForEach(c => tempAllZones.Add(c));
            allZones.ForEach(c =>
            {
                //将大于当前选中地域的地域(集合)排除
                if (c.Level > currentZone.Level)
                    allZones.Remove(c);
            });
            business.ProgressChanged -= ReportPercent;
            this.ReportProgress(0, "开始");
            double percent = 0.0;
            percent = 99.0 / (double)allZones.Count;
            double subpercent = 0.0;
            int zoneCount = 0;
            int tableCount = 0;      //统计可导出表的个数
            business.VirtualType = eVirtualType.Land;
            foreach (var zone in allZones)
            {
                string desc = ExportZoneListDir(zone, tempAllZones);// 优化
                List<VirtualPerson> personlists = new List<VirtualPerson>();
                personlists = business.GetByZone(zone.FullCode);
                string folderString = CreateDirectory(allZones, zone);
                string path = savePath + @"\" + folderString;
                subpercent = percent * (zoneCount++);
                this.ReportProgress((int)(subpercent), string.Format("{0}", desc));
                double temppercent = percent / (double)(personlists.Count == 0 ? 1 : personlists.Count);
                int index = 1;
                foreach (var exportPerson in personlists)
                {
                    switch (exportType)
                    {
                        //导出当前人的单户调查表
                        case eContractAccountType.ExportSingleFamilySurveyExcel:
                            business.ExportSingleFamilySurveyExcel(zone, exportPerson, path);
                            break;
                        //导出当前人的单户确认表
                        case eContractAccountType.ExportSingleFamilyConfirmExcel:
                            business.ExportSingleFamilyConfirmExcel(zone, exportPerson, path);
                            break;
                    }
                    this.ReportProgress((int)(subpercent + temppercent * index), string.Format("导出{0}", desc + exportPerson.Name));
                    index++;
                    tableCount++;
                }
                if ((zone.Level == eZoneLevel.Village && (personlists == null || personlists.Count == 0)) || zone.Level == eZoneLevel.Town)
                    continue;
                string info = personlists.Count == 0 ? string.Format("在{0}下未获取数据", zone.FullName) : string.Format("在{0}下成功导出{1}条数据", zone.FullName, personlists.Count);
                this.ReportInfomation(info);
            }
            this.ReportProgress(100, "完成");
            this.ReportInfomation(string.Format("共导出{0}条表信息", tableCount));
            allZones = null;
            tempAllZones = null;
        }

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
            if (currentZone == null)
            {
                this.ReportError("未选择导出数据的地域!");
                return;
            }
            List<Zone> allZones = GetAllZones(currentZone, childrenZone, parentZone, business);
            business.ProgressChanged -= ReportPercent;
            this.ReportProgress(1, "开始");
            this.ReportProgress(5, "正在获取表");
            List<Zone> tempAllZones = new List<Zone>();
            allZones.ForEach(c => tempAllZones.Add(c));
            allZones.ForEach(c =>
            {
                //将大于当前选中地域的地域(集合)排除
                if (c.Level > currentZone.Level)
                    allZones.Remove(c);
            });
            double percent = 95 / (double)allZones.Count;
            int indexOfZone = 0;  //地域索引
            int dataCount = 0;//有数据表个数
            foreach (var zone in allZones)
            {
                string descZone = ExportZoneListDir(zone, tempAllZones);
                string folderString = CreateDirectoryByVilliage(tempAllZones, zone);
                string path = savePath + @"\" + folderString;
                bool hasperson = ExitsPerson(zone);
                this.ReportProgress((int)(5 + percent * indexOfZone), string.Format("{0}", descZone));
                if (hasperson)//有数据则建立文件夹
                {
                    Directory.CreateDirectory(path);
                    business.VolumnExportVillagesDeclareWord(zone, path, PublishDateSetting);
                    this.ReportInfomation(string.Format("{0}导出{1}个公告表数据", descZone, 1));
                    dataCount++;
                }
                indexOfZone++;
                if ((zone.Level == eZoneLevel.Town || zone.Level == eZoneLevel.Village) && !hasperson)
                {
                    //在镇、村下没有数据(提示信息不显示)
                    continue;
                }
                if (zone.Level == eZoneLevel.Group && !hasperson)
                {
                    //地域下无数据
                    this.ReportInfomation(string.Format("{0}无数据", descZone));
                }
            }
            this.ReportProgress(100, "完成");
            this.ReportInfomation(string.Format("共导出{0}个公告表", dataCount));
        }

        /// <summary>
        /// 批量导出数据汇总表
        /// </summary>
        /// <param name="currentZone">当前地域</param>
        /// <param name="childrenZone">子级地域</param>
        /// <param name="parentZone">父级地域</param>
        /// <param name="savePath">保存文件路径</param>
        /// <param name="business">台账业务</param>
        private void ExportSummaryExcelByZone(Zone currentZone, List<Zone> childrenZone, Zone parentZone, string savePath, AccountLandBusiness business)
        {
            if (currentZone == null)
            {
                this.ReportError("未选择导出数据的地域!");
                return;
            }
            List<Zone> allZones = GetAllZones(currentZone, childrenZone, parentZone, business);
            //business.ProgressChanged -= ReportPercent;
            this.ReportProgress(1, "开始");
            this.ReportProgress(5, "正在获取表");
            List<Zone> tempAllZones = new List<Zone>();
            allZones.ForEach(c => tempAllZones.Add(c));
            allZones.ForEach(c =>
            {
                //将大于当前选中地域的地域(集合)排除
                if (c.Level > currentZone.Level)
                    allZones.Remove(c);
            });
            double percent = 95 / (double)allZones.Count;
            int indexOfZone = 0;  //地域索引
            int dataCount = 0;//有数据表个数
            foreach (var zone in allZones)
            {
                string descZone = ExportZoneListDir(zone, tempAllZones);
                string folderString = CreateDirectoryByVilliage(tempAllZones, zone);
                string path = savePath + @"\" + folderString;
                bool hasperson = ExitsPerson(zone);
                this.ReportProgress((int)(5 + percent * indexOfZone), string.Format("{0}", descZone));
                if (hasperson)//有数据则建立文件夹
                {
                    Directory.CreateDirectory(path);
                    business.ExportSummaryExcel(zone, path, percent, 5 + percent * (indexOfZone));
                    this.ReportInfomation(string.Format("{0}导出{1}个汇总表数据", descZone, 1));
                    dataCount++;
                }
                indexOfZone++;
                if ((zone.Level == eZoneLevel.Town || zone.Level == eZoneLevel.Village) && !hasperson)
                {
                    //在镇、村下没有数据(提示信息不显示)
                    continue;
                }
                if (zone.Level == eZoneLevel.Group && !hasperson)
                {
                    //地域下无数据
                    this.ReportInfomation(string.Format("{0}无数据", descZone));
                }
            }
            this.ReportProgress(100, "完成");
            this.ReportInfomation(string.Format("共导出{0}个汇总表", dataCount));
        }

        /// <summary>
        /// 批量初始化
        /// </summary>
        /// <param name="currentZone">当前初始化地域</param>
        /// <param name="listZone">当前初始化地域之子级地域集合</param>
        /// <param name="landBusiness">承包台账数据业务</param>
        /// <param name="metadata">任务参数</param>
        private void InitialDataVolumn(Zone currentZone, List<Zone> listZone, Zone parentZone, AccountLandBusiness landBusiness, TaskContractAccountArgument metadata)
        {
            if (currentZone == null)
            {
                this.ReportError("未选择导出数据的地域!");
                return;
            }
            List<Zone> allZones = GetAllZones(currentZone, listZone, parentZone, landBusiness);
            this.ReportProgress(1, "开始");
            List<Zone> tempAllZones = new List<Zone>();
            allZones.ForEach(c => tempAllZones.Add(c));
            allZones.ForEach(c =>
            {
                //将大于当前选中地域的地域(集合)排除
                if (c.Level > currentZone.Level)
                    allZones.Remove(c);
            });
            double percent = 99 / (double)allZones.Count;
            int indexOfZone = 0;  //地域索引
            int landCount = 0;   //统计总地块(空间)个数
            foreach (var zone in allZones)
            {
                string descZone = ExportZoneListDir(zone, tempAllZones);
                List<ContractLand> listLand = landBusiness.GetCollection(zone.FullCode, eLevelOption.Self);
                List<ContractLand> listGeoLand = listLand.FindAll(c => c.Shape != null);
                this.ReportProgress((int)(1 + percent * indexOfZone), string.Format("{0}", descZone));
                if (listLand != null && listLand.Count > 0 && metadata.ArgType == eContractAccountType.InitialLandTool)
                {
                    InitialDataByType(metadata, zone, listLand, landBusiness, percent, 1 + percent * indexOfZone);
                }
                if (listLand != null && listLand.Count > 0 && metadata.ArgType == eContractAccountType.InitialAreaNumericFormatTool)
                {
                    InitialDataByType(metadata, zone, listLand, landBusiness, percent, 1 + percent * indexOfZone);
                }
                else if (listGeoLand != null && listGeoLand.Count > 0 && metadata.ArgType != eContractAccountType.InitialLandTool)
                {
                    InitialDataByType(metadata, zone, listGeoLand, landBusiness, percent, 1 + percent * indexOfZone);
                }
                indexOfZone++;

                //提示信息
                if (metadata.ArgType == eContractAccountType.InitialLandTool)     //初始化承包地块属性信息
                {
                    if ((zone.Level == eZoneLevel.Town || zone.Level == eZoneLevel.Village) && listLand.Count == 0)
                    {
                        //在镇、村下没有数据(提示信息不显示)
                        continue;
                    }
                    if (listLand.Count > 0)
                    {
                        //地域下有数据
                        this.ReportInfomation(string.Format("{0}初始化{1}个地块", descZone, listLand.Count));
                        landCount += listLand.Count;
                    }
                    else
                    {
                        //地域下无数据
                        this.ReportInfomation(string.Format("{0}无地块数据", descZone));
                    }
                }
                else if (metadata.ArgType == eContractAccountType.InitialAreaTool)    //初始化承包地块面积
                {
                    if ((zone.Level == eZoneLevel.Town || zone.Level == eZoneLevel.Village) && listGeoLand.Count == 0)
                    {
                        //在镇、村下没有空间数据(提示信息不显示)
                        continue;
                    }
                    if (listGeoLand.Count > 0)
                    {
                        //地域下有空间数据
                        this.ReportInfomation(string.Format("{0}初始化{1}个具有空间属性的地块面积", descZone, listGeoLand.Count));
                        landCount += listGeoLand.Count;
                    }
                    else
                    {
                        //地域下无空间数据
                        this.ReportInfomation(string.Format("{0}无空间地块数据", descZone));
                    }
                }
                else if (metadata.ArgType == eContractAccountType.InitialAreaNumericFormatTool)    //截取承包地块面积小数位
                {
                    if ((zone.Level == eZoneLevel.Town || zone.Level == eZoneLevel.Village) && listLand.Count == 0)
                    {
                        //在镇、村下没有数据(提示信息不显示)
                        continue;
                    }
                    if (listLand.Count > 0)
                    {
                        //地域下有数据
                        this.ReportInfomation(string.Format("{0}初始化{1}个地块", descZone, listLand.Count));
                        landCount += listLand.Count;
                    }
                    else
                    {
                        //地域下无数据
                        this.ReportInfomation(string.Format("{0}无地块数据", descZone));
                    }
                }
                else      //初始化承包地块是否基本农田
                {
                    if ((zone.Level == eZoneLevel.Town || zone.Level == eZoneLevel.Village) && listGeoLand.Count == 0)
                    {
                        //在镇、村下没有空间数据(提示信息不显示)
                        continue;
                    }
                    if (listGeoLand.Count > 0)
                    {
                        //地域下有空间数据
                        this.ReportInfomation(string.Format("{0}共有{1}个地块参与拓扑(包含)判断", descZone, listGeoLand.Count));
                        landCount += listGeoLand.Count;
                    }
                    else
                    {
                        //地域下无空间数据
                        this.ReportInfomation(string.Format("{0}无空间地块数据", descZone));
                    }
                }
            }
            this.ReportProgress(100, "完成");
            switch (metadata.ArgType)
            {
                case eContractAccountType.InitialLandTool:
                    this.ReportInfomation(string.Format("共初始化{0}个承包地块", landCount));
                    break;

                case eContractAccountType.InitialAreaTool:
                    this.ReportInfomation(string.Format("共初始化{0}个具有空间属性的地块面积", landCount));
                    break;

                case eContractAccountType.InitialAreaNumericFormatTool:
                    this.ReportInfomation(string.Format("共截取{0}个地块面积", landCount));
                    break;

                case eContractAccountType.InitialIsFarmerTool:
                    this.ReportInfomation(string.Format("共{0}个地块参与拓扑(包含)判断", landCount));
                    break;
            }
        }

        /// <summary>
        /// 根据类型进行初始化操作
        /// </summary>
        /// <param name="metadata">任务参数(针对不同的初始化类型有不同的任务参数)</param>
        /// <param name="currentZone">当前初始化地域</param>
        /// <param name="listLand">当前初始化地块集合</param>
        /// <param name="landBusiness">承包地块业务</param>
        /// <param name="averagePercent">每个地域占的百分比(平均百分比)</param>
        /// <param name="percent">当前进度百分比</param>
        private void InitialDataByType(TaskContractAccountArgument metadata, Zone currentZone, List<ContractLand> listLand, AccountLandBusiness landBusiness,
            double averagePercent = 0.0, double percent = 0.0)
        {
            switch (metadata.ArgType)
            {
                case eContractAccountType.InitialLandTool:
                    //landBusiness.ContractLandInitialTool(metadata, listLand, currentZone, averagePercent, percent);
                    break;

                case eContractAccountType.InitialAreaTool:
                    //landBusiness.ContractLandAreaInitialTool(metadata, listLand, currentZone, averagePercent, percent);
                    break;

                case eContractAccountType.InitialAreaNumericFormatTool:
                    //landBusiness.ContractLandAreaNumericFormatTool(metadata, listLand, currentZone, averagePercent, percent);
                    break;

                case eContractAccountType.InitialIsFarmerTool:
                    //landBusiness.ContractLandIsFarmerInitialTool(metadata, listLand, currentZone, averagePercent, percent);
                    break;
            }
        }

        #endregion Methods - Privates-承包台账导出

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

        #endregion 提示信息
    }
}