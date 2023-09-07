/*
 * (C) 2015  鱼鳞图公司版权所有,保留所有权利 
 */
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using YuLinTu;
using YuLinTu.Data;
using YuLinTu.Library.Entity;
using YuLinTu.Library.Office;
using YuLinTu.Library.Repository;
using YuLinTu.Library.WorkStation;
using YuLinTu.Windows;

namespace YuLinTu.Library.Business
{
    /// <summary>
    /// 承包权证业务处理
    /// </summary>
    public class ContractRegeditBookBusiness : Task
    {
        #region Fields

        //private bool isErrorRecord;

        /// <summary>
        /// 承包合同业务逻辑层
        /// </summary>
        private IContractRegeditBookWorkStation regeditBookStation;

        /// <summary>
        /// 合同业务
        /// </summary>
        private ConcordBusiness concordBusiness;

        /// <summary>
        /// 承包台账(承包方)Station
        /// </summary>
        private IVirtualPersonWorkStation<LandVirtualPerson> personStation;

        /// <summary>
        /// 行政地域业务
        /// </summary>
        private ZoneDataBusiness zoneBusiness;

        #endregion

        #region Properties

        /// <summary>
        /// 承包方类型
        /// </summary>
        public eVirtualType VirtualType { get; set; }

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
        /// 数据库
        /// </summary>
        public IDbContext dbContext { get; set; }

        /// <summary>
        /// 数据字典处理业务
        /// </summary>
        public DictionaryBusiness DictBusiness { get; set; }

        /// <summary>
        /// 土地业务
        /// </summary>
        public AccountLandBusiness AccountLandBusiness { get; set; }

        /// <summary>
        /// 承包方业务
        /// </summary>
        public VirtualPersonBusiness PersonBusiness { get; set; }

        /// <summary>
        /// 权证汇总表设置
        /// </summary>
        public DataSummaryDefine SummaryDefine { get; set; }

        /// <summary>
        /// 权证数据汇总表参数
        /// </summary>
        public eContractAccountType ArgType { get; set; }

        /// <summary>
        /// 当前地域下合同集合
        /// </summary>
        public List<ContractConcord> ConcordsOfInitialWarrant { get; set; }

        /// <summary>
        /// 权证集合(待初始化)
        /// </summary>
        public List<ContractRegeditBook> WarrantsModified { get; set; }


        /// <summary>
        /// 承包权证导出选择扩展模板格式设置
        /// </summary>
        public ContractRegeditBookSettingDefine ExtendUseExcelDefine
        { get; set; }

        /// <summary>
        /// 系统信息常规设置
        /// </summary>
        public SystemSetDefine SystemSet { get; set; }

        #endregion

        #region Ctor

        /// <summary>
        /// 构造函数
        /// </summary>
        public ContractRegeditBookBusiness(IDbContext db)
        {
            dbContext = db;
            regeditBookStation = db.CreateRegeditBookStation();
            concordBusiness = new ConcordBusiness(dbContext);
            DictBusiness = new DictionaryBusiness(dbContext);
            PersonBusiness = new VirtualPersonBusiness(dbContext);
            PersonBusiness.VirtualType = eVirtualType.Land;
            AccountLandBusiness = new AccountLandBusiness(dbContext);
            personStation = db.CreateVirtualPersonStation<LandVirtualPerson>();
            zoneBusiness = new ZoneDataBusiness();
            zoneBusiness.Station = db.CreateZoneWorkStation();
        }

        #endregion

        #region Methods

        #region 业务

        /// <summary>
        /// 开启事物
        /// </summary>
        public void BeganTranscation()
        {
            if (dbContext != null)
                dbContext.BeginTransaction();
        }

        /// <summary>
        /// 提交数据
        /// </summary>
        public void Commit()
        {
            if (dbContext != null)
                dbContext.CommitTransaction();
        }

        /// <summary>
        /// 回滚
        /// </summary>
        public void RollBack()
        {
            if (dbContext != null)
                dbContext.RollbackTransaction();
        }

        #endregion

        #region 数据处理

        #region 初始化权证业务

        /// <summary>
        /// 权证数据处理之签订权证(初始化权证)
        /// </summary>
        /// <param name="currentZone"></param>
        public void InitialWarrantData(Zone currentZone)
        {
            int index = 1;   //合同索引
            double percent = 0.0;  //百分比
            try
            {
                if (currentZone == null)
                {
                    this.ReportError("未选择初始化数据的地域!");
                    return;
                }
                var metaargument = Argument as TaskContractRegeditBookArgument;
                var rbStation = dbContext.CreateRegeditBookStation();                
                List<ContractRegeditBook> listWarrant = rbStation.GetByZoneCode(currentZone.FullCode, eSearchOption.Precision);//metaargument.ListWarrants;
                List<VirtualPerson> listPerson = metaargument.listPerson;
                if (metaargument.Concords == null || metaargument.Concords.Count == 0)
                {
                    this.ReportWarn(string.Format("在{0}下未获得待签订的合同集合!", currentZone.FullName));
                    this.ReportProgress(100, "完成");
                    return;
                }
                this.ReportProgress(1, "开始初始化承包权证");
                if (WarrantsModified == null || WarrantsModified.Count == 0)
                {
                    this.ReportWarn(string.Format("在{0}下未获得待签订的权证集合!", currentZone.FullName));
                    this.ReportProgress(100, "完成");
                    return;
                }            
                percent = 99 / (double)WarrantsModified.Count;

                int maxSerialNumber = 0;
                var allBooks = regeditBookStation.Get();
                var otherDefine = ContractRegeditBookSettingDefine.GetIntence();

                if (allBooks != null && allBooks.Count > 0)
                {
                    maxSerialNumber = allBooks.Max(c =>
                    {
                        int ret = 0;
                        string erri1 = "";
                        string erri2 = "";
                        try
                        {
                            if (c.SerialNumber.IsNullOrEmpty())
                                return 0;
                            erri1 = c.Number + c.ContractRegeditBookPerson;
                            erri2 = c.SerialNumber;
                            ret = Convert.ToInt32(c.SerialNumber);
                           
                        }
                        catch (Exception)
                        {
                            this.ReportInfomation($"发现错误权证数据，权证编码为{erri1},流水号为{erri2}");                            
                        }
                        return ret;
                    });
                }

                foreach (var warrant in WarrantsModified)
                {
                    warrant.SerialNumber = InitialBookSerialNumber(ref maxSerialNumber, warrant, otherDefine);

                    var getconcord = metaargument.Concords.Find(c => c.ID == warrant.ID);
                    var person = listPerson.Find(c => c.ID == getconcord.ContracterId);
                    bool isExsit = listWarrant.Any(c => c.ID == warrant.ID);
                    if (isExsit)
                    {
                        //更新
                        Update(warrant);
                    }
                    else
                    {
                        //添加
                        AddRegeditBook(warrant);
                    }                    

                    this.ReportProgress((int)(1 + percent * index), string.Format("初始化{0}的权证", person.Name));
                    index++;
                }

                allBooks = null;

                this.ReportProgress(100, "初始化完成.");
                this.ReportInfomation(string.Format("在{0}下共初始化{1}条权证信息", currentZone.FullName, WarrantsModified.Count));

                WarrantsModified = null;
                listWarrant = null;
                listPerson = null;
                ConcordsOfInitialWarrant = null;
            }
            catch (Exception ex)
            {
                this.ReportError(ex.Message);
                YuLinTu.Library.Log.Log.WriteError(this, "InitialConcordData(提交初始化权证数据)", ex.Message + ex.StackTrace);
            }
        }

        /// <summary>
        /// 生成流水号
        /// </summary>
        /// <returns></returns>
        private string InitialBookSerialNumber(ref int maxNumber, ContractRegeditBook book, ContractRegeditBookSettingDefine otherDefine)
        {
            if (!string.IsNullOrWhiteSpace(book.SerialNumber))
                return book.SerialNumber;
            maxNumber++;
            if (maxNumber < otherDefine.MinNumber)
            {
                maxNumber = otherDefine.MinNumber;
                return otherDefine.MinNumber.ToString();
            }
            if (maxNumber > otherDefine.MaxNumber)
            {
                return string.Empty;
            }
            return maxNumber.ToString();
        }

        /* 见缝插针的方式生成流水号
        /// <summary>
        /// 生成流水号
        /// </summary>
        /// <returns></returns>
        private string InitalizeBookSerialNumber(ContractRegeditBook book, List<ContractRegeditBook> allBooks, ContractRegeditBookSettingDefine otherDefine)
        {
            if (_globleSerialIndex > otherDefine.MaxNumber)
                return string.Empty;
            ContractRegeditBook regeditBook = null;
            ContractRegeditBook existRegbook = null;
            if (allBooks != null)
            {
                regeditBook = allBooks.Find(o => o.SerialNumber == _globleSerialIndex.ToString());
                existRegbook = allBooks.Find(o => o.ID == book.ID);
            }
            if (existRegbook != null && existRegbook.SerialNumber != null)
                return existRegbook.SerialNumber;
            if (regeditBook != null)
            {
                _globleSerialIndex++;
                return InitalizeBookSerialNumber(book, allBooks, otherDefine);
            }
            var index = _globleSerialIndex;
            _globleSerialIndex++;
            return index.ToString();
        }
    */

        #endregion

        /// <summary>
        /// 添加承包权证信息
        /// </summary>
        /// <param name="book">权证对象</param>
        /// <returns>-1(参数错误)/0(失败)/1(成功)</returns>
        public int AddRegeditBook(ContractRegeditBook book)
        {
            int addCount = 0;
            if (!CanContinue())
            {
                return addCount;
            }
            if (book == null)
            {
                addCount = -1;
                return addCount;
            }
            try
            {
                addCount = regeditBookStation.AddRegeditBook(book);
            }
            catch (Exception ex)
            {
                YuLinTu.Library.Log.Log.WriteException(this, "AddRegeditBook(添加承包权证失败)", ex.Message + ex.StackTrace);
                this.ReportError("添加承包权证失败" + ex.Message);
            }
            return addCount;
        }

        /// <summary>
        /// 根据权证ID删除农村土地承包经营权登记薄对象
        /// </summary>
        /// <param name="guid">权证ID</param>
        /// <returns>-1（参数错误）/0（失败）/1（成功）</returns>
        public int Delete(Guid guid)
        {
            int delCount = 0;
            if (!CanContinue())
            {
                return delCount;
            }
            if (guid == null)
            {
                delCount = -1;
                return delCount;
            }
            try
            {
                delCount = regeditBookStation.Delete(guid);
            }
            catch (Exception ex)
            {
                YuLinTu.Library.Log.Log.WriteException(this, "Delete(删除承包权证失败)", ex.Message + ex.StackTrace);
                this.ReportError("删除承包权证失败" + ex.Message);
            }
            return delCount;
        }

        /// <summary>
        /// 删除农村土地承包经营权登记薄对象
        /// </summary>
        /// <param name="number">权证编号</param>
        /// <returns>-1（参数错误）/0（失败）/1（成功）</returns>
        public int DeleteByRegeditNumber(string number)
        {
            int delCount = 0;
            if (!CanContinue())
            {
                return delCount;
            }
            if (number == null)
            {
                delCount = -1;
                return delCount;
            }
            try
            {
                delCount = regeditBookStation.DeleteByRegeditNumber(number);
            }
            catch (Exception ex)
            {
                YuLinTu.Library.Log.Log.WriteException(this, "Delete(删除承包权证失败)", ex.Message + ex.StackTrace);
                this.ReportError("删除承包权证失败" + ex.Message);
            }
            return delCount;
        }

        /// <summary>
        /// 更新农村土地承包经营权登记薄对象
        /// </summary>
        /// <param name="book">权证对象</param>
        /// <returns>-1（参数错误）/0（失败）/1（成功）</returns>
        public int Update(ContractRegeditBook book)
        {
            int updateCount = 0;
            if (!CanContinue())
            {
                return updateCount;
            }
            if (book == null)
            {
                updateCount = -1;
                return updateCount;
            }
            try
            {
                updateCount = regeditBookStation.Update(book);
            }
            catch (Exception ex)
            {
                YuLinTu.Library.Log.Log.WriteException(this, "Update(更新承包权证失败)", ex.Message + ex.StackTrace);
                this.ReportError("更新承包权证失败," + ex.Message);
            }
            return updateCount;
        }

        /// <summary>
        /// 根据ID获取农村土地承包经营权登记薄对象
        /// </summary>
        /// <param name="id">权证ID</param>
        /// <returns>权证对象</returns>
        public ContractRegeditBook Get(Guid guid)
        {
            ContractRegeditBook book = null;
            if (!CanContinue() || guid == null)
            {
                return book;
            }
            try
            {
                book = regeditBookStation.Get(guid);
            }
            catch (Exception ex)
            {
                YuLinTu.Library.Log.Log.WriteException(this, "Get(获取承包权证失败)", ex.Message + ex.StackTrace);
                this.ReportError("获取承包权证失败," + ex.Message);
            }
            return book;
        }

        /// <summary>
        /// 根据权证ID判断农村土地承包经营权登记薄对象是否存在
        /// </summary>
        /// <param name="guid">权证ID</param>
        /// <returns>true（存在）/false（不存在）</returns>
        public bool Exists(Guid guid)
        {
            bool isExisting = false;
            if (!CanContinue() || guid == null)
            {
                return isExisting;
            }
            try
            {
                isExisting = regeditBookStation.Exists(guid);
            }
            catch (Exception ex)
            {
                YuLinTu.Library.Log.Log.WriteException(this, "Exists(判断承包权证是否存在失败)", ex.Message + ex.StackTrace);
                this.ReportError("判断承包权证是否存在失败" + ex.Message);
            }
            return isExisting;
        }

        /// <summary>
        /// 根据登记簿编号查看农村土地承包经营权登记薄对象是否存在
        /// </summary>
        /// <param name="regeditNumber">登记薄编号</param>
        /// <returns>true（存在）/false（不存在）</returns>
        public bool Exists(string regeditNumber)
        {
            bool isExisting = false;
            if (!CanContinue() || string.IsNullOrEmpty(regeditNumber))
            {
                return isExisting;
            }
            try
            {
                isExisting = regeditBookStation.Exists(regeditNumber);
            }
            catch (Exception ex)
            {
                YuLinTu.Library.Log.Log.WriteException(this, "Exists(判断承包权证是否存在失败)", ex.Message + ex.StackTrace);
                this.ReportError("判断承包权证是否存在失败" + ex.Message);
            }
            return isExisting;
        }

        /// <summary>
        /// 通过登记薄编号获取农村土地承包经营权登记薄对象
        /// </summary>
        /// <param name="regeditNumber">登记薄编号</param>
        /// <returns>权证对象</returns>
        public ContractRegeditBook Get(string regeditNumber)
        {
            ContractRegeditBook book = null;
            if (!CanContinue() || string.IsNullOrEmpty(regeditNumber))
            {
                return book;
            }
            try
            {
                book = regeditBookStation.Get(regeditNumber);
            }
            catch (Exception ex)
            {
                YuLinTu.Library.Log.Log.WriteException(this, "Get(获取承包权证失败)", ex.Message + ex.StackTrace);
                this.ReportError("获取承包权证失败" + ex.Message);
            }
            return book;
        }

        /// <summary>
        /// 根据权证流水号及其查找类型获取农村土地承包经营权登记薄对象
        /// </summary>
        /// <param name="number">权证流水号</param>
        /// <param name="searchOption">查找类型</param>
        /// <returns>权证对象</returns>
        public ContractRegeditBook GetByNumber(string number, eSearchOption searchOption)
        {
            ContractRegeditBook book = null;
            if (!CanContinue() || string.IsNullOrEmpty(number))
            {
                return book;
            }
            try
            {
                book = regeditBookStation.GetByNumber(number, searchOption);
            }
            catch (Exception ex)
            {
                YuLinTu.Library.Log.Log.WriteException(this, "GetByNumber(获取承包权证失败)", ex.Message + ex.StackTrace);
                this.ReportError("获取承包权证失败" + ex.Message);
            }
            return book;
        }
        /// <summary>
        /// 获取最大流水号
        /// </summary>
        /// <returns></returns>
        public int GetMaxSerialNumber()
        {
            return regeditBookStation.GetMaxSerialNumber();
        }
        /// <summary>
        /// 根据地域代码及其查找类型获取权证集合
        /// </summary>
        /// <param name="zoneCode">地域编码</param>
        /// <param name="searchOption">查找类型</param>
        /// <returns>权证对象集合</returns>
        public List<ContractRegeditBook> GetByZoneCode(string zoneCode, eSearchOption searchOption)
        {
            List<ContractRegeditBook> listBook = null;
            if (!CanContinue() || string.IsNullOrEmpty(zoneCode))
            {
                return listBook;
            }
            try
            {
                listBook = regeditBookStation.GetByZoneCode(zoneCode, searchOption);
            }
            catch (Exception ex)
            {
                YuLinTu.Library.Log.Log.WriteException(this, "GetByZoneCode(获取承包权证集合失败)", ex.Message + ex.StackTrace);
                this.ReportError("获取承包权证集合失败" + ex.Message);
            }
            return listBook;
        }

        /// <summary>
        /// 根据权证号获取及其查找类型获取权证
        /// </summary>
        /// <param name="number">权证号</param>
        /// <param name="searchOption">查找类型</param>
        /// <returns>权证对象</returns>
        public List<ContractRegeditBook> GetCollection(string number, eSearchOption searchOption)
        {
            List<ContractRegeditBook> listBook = null;
            if (!CanContinue() || string.IsNullOrEmpty(number))
            {
                return listBook;
            }
            try
            {
                listBook = regeditBookStation.GetCollection(number, searchOption);
            }
            catch (Exception ex)
            {
                YuLinTu.Library.Log.Log.WriteException(this, "GetCollection(获取承包权证集合失败)", ex.Message + ex.StackTrace);
                this.ReportError("获取承包权证集合失败" + ex.Message);
            }
            return listBook;
        }

        /// <summary>
        /// 根据地域代码及其查找类型删除权证
        /// </summary>
        /// <param name="zoneCode">地域编码</param>
        /// <param name="searchOption">查找类型</param>
        /// <returns>-1（参数错误）/0（失败）/1（成功）</returns>
        public int DeleteByZoneCode(string zoneCode, eSearchOption searchOption)
        {
            int delByCount = 0;
            if (!CanContinue())
            {
                return delByCount;
            }
            if (string.IsNullOrEmpty(zoneCode))
            {
                delByCount = -1;
                return delByCount;
            }
            try
            {
                delByCount = regeditBookStation.DeleteByZoneCode(zoneCode, searchOption);
            }
            catch (Exception ex)
            {
                YuLinTu.Library.Log.Log.WriteException(this, "DeleteByZoneCode(删除承包权证失败)", ex.Message + ex.StackTrace);
                this.ReportError("删除承包权证失败" + ex.Message);
            }
            return delByCount;
        }

        /// <summary>
        /// 按地域及其匹配类型统计权证数量
        /// </summary>
        /// <param name="zoneCode">地域编码</param>
        /// <param name="levelOption">匹配类型</param>
        /// <returns>-1（参数错误）/int 权证数量</returns>
        public int Count(string zoneCode, eLevelOption levelOption)
        {
            int count = -1;
            if (!CanContinue() || string.IsNullOrEmpty(zoneCode))
            {
                return count;
            }
            try
            {
                count = regeditBookStation.Count(zoneCode, levelOption);
            }
            catch (Exception ex)
            {
                YuLinTu.Library.Log.Log.WriteException(this, "Count(统计承包权证失败)", ex.Message + ex.StackTrace);
                this.ReportError("统计承包权证失败" + ex.Message);
            }
            return count;
        }

        /// <summary>
        /// 统计权证记录个数
        /// </summary>
        /// <returns></returns>
        public int CountAll()
        {
            int count = -1;
            if (!CanContinue())
            {
                return count;
            }
            try
            {
                count = regeditBookStation.Count();
            }
            catch (Exception ex)
            {
                YuLinTu.Library.Log.Log.WriteException(this, "Count(统计承包权证失败)", ex.Message + ex.StackTrace);
                this.ReportError("统计承包权证失败" + ex.Message);
            }
            return count;
        }

        /// <summary>
        /// 存在权证的地域集合
        /// </summary>
        /// <param name="listZone">地域集合</param>
        public List<Zone> ExistZones(List<Zone> listZone)
        {
            List<Zone> zones = null;
            if (!CanContinue())
            {
                return zones;
            }
            try
            {
                zones = regeditBookStation.ExistZones(listZone);
            }
            catch (Exception ex)
            {
                YuLinTu.Library.Log.Log.WriteException(this, "ExistZones(统计存在承包权证的地域集合)", ex.Message + ex.StackTrace);
                this.ReportError("统计存在承包权证的地域集合失败" + ex.Message);
            }
            return zones;
        }

        #endregion

        #region 导出证书

        /// <summary>
        /// 导出多个选中承包方权证-证书数据处理
        /// </summary>
        public void ExportWarrantWord(Zone zone, List<VirtualPerson> selectVirtualPerson, string fileName)
        {
            try
            {
                if (selectVirtualPerson == null)
                {
                    this.ReportError("未选择导出数据的承包方!");
                    return;
                }
                string excelName = GetMarkDesc(zone);
                List<VirtualPerson> vps = new List<VirtualPerson>();
                this.ReportProgress(0, "开始");
                int warrantNumAll = 0;//计算当前多选人下总共有多少证书
                string tempPath = TemplateHelper.WordTemplate(TemplateFile.ContractRegeditBookWord);
                List<Dictionary> DictList = DictBusiness.GetAll();

                int percent = (100 / selectVirtualPerson.Count);
                this.ReportProgress(10, "正在获取数据");
                int percentIndex = 1;

                foreach (var item in selectVirtualPerson)
                {
                    VirtualPerson vpn = item;
                    List<ContractConcord> currentConcord = concordBusiness.GetCollection(vpn.ID);
                    if (currentConcord == null || currentConcord.Count == 0) return;
                    List<ContractRegeditBook> currentWarrant = new List<ContractRegeditBook>();
                    this.ReportProgress(10 + percent * percentIndex, string.Format("导出{0}", vpn.Name));
                    ++percentIndex;
                    foreach (var concord in currentConcord)
                    {
                        currentWarrant.Add(Get(concord.ID));
                    }
                    warrantNumAll = warrantNumAll + currentWarrant.Count;

                    if (currentConcord == null || currentConcord.Count == 0)
                    {

                        return;
                    }

                    foreach (var warrant in currentWarrant)
                    {
                        ContractConcord exportConcord = concordBusiness.Get(warrant.ID);
                        List<ContractLand> contractLands = AccountLandBusiness.GetByConcordId(exportConcord.ID);
                        CollectivityTissue Tissue = concordBusiness.GetSenderById(exportConcord.SenderId);
                        ContractWarrantPrinter printContract = new ContractWarrantPrinter();
                        try
                        {
                            #region 通过反射等机制定制化具体的业务处理类
                            var temp = WorksheetConfigHelper.GetInstance(printContract);
                            if (temp != null && temp is ContractWarrantPrinter)
                            {
                                printContract = (ContractWarrantPrinter)temp;
                                tempPath = Path.Combine(TheApp.GetApplicationPath(), temp.TemplatePath);
                            }
                            #endregion

                            printContract.dbContext = dbContext;
                            printContract.CurrentZone = zone;
                            printContract.RegeditBook = warrant;
                            printContract.Concord = exportConcord;
                            printContract.LandCollection = contractLands;
                            printContract.BatchExport = IsBatch;
                            printContract.Contractor = vpn;
                            printContract.Tissue = Tissue;
                            printContract.DictList = DictList;
                            printContract.TempleFilePath = tempPath;                           
                            printContract.UseExcel = ExtendUseExcelDefine.WarrantExtendByExcel;
                            printContract.BookPersonNum = BookPersonNum;
                            printContract.BookLandNum = BookLandNum;
                            printContract.BookNumSetting = BookNumSetting;
                            printContract.ExportContractLand(fileName);
                        }
                        catch (System.Exception ex)
                        {
                            System.Diagnostics.Debug.WriteLine(ex.ToString());
                        }
                    }
                }
                string reInfo = string.Format("从{0}下成功导出{1}条权证数据!", excelName, warrantNumAll.ToString());

                this.ReportProgress(100, "完成");
                this.ReportInfomation(reInfo);
                vps = null;
                DictList = null;
            }
            catch (Exception ex)
            {
                this.ReportError(ex.Message);
                YuLinTu.Library.Log.Log.WriteException(this, "ExportDataWord(导出数据到Word文档)", ex.Message + ex.StackTrace);
            }
        }

        /// <summary>
        /// 导出权证-证书数据处理，批量用
        /// </summary>
        public void ExportWarrantData(Zone currentZone, ContractRegeditBook warrant, string savePath)
        {
            ContractConcord currentConcord = concordBusiness.Get(warrant.ID);
            List<ContractLand> contractLands = AccountLandBusiness.GetByConcordId(currentConcord.ID);
            CollectivityTissue Tissue = concordBusiness.GetSenderById(currentConcord.SenderId);
            string tempPath = TemplateHelper.WordTemplate(TemplateFile.ContractRegeditBookWord);
            List<Dictionary> DictList = DictBusiness.GetAll();
            VirtualPerson vpn = new VirtualPerson();
            if (currentConcord.ContracterId.HasValue)
            {
                vpn = personStation.Get(currentConcord.ContracterId.Value);
            }
            ContractWarrantPrinter printContract = new ContractWarrantPrinter();
            try
            {
                #region 通过反射等机制定制化具体的业务处理类
                var temp = WorksheetConfigHelper.GetInstance(printContract);
                if (temp != null && temp is ContractWarrantPrinter)
                {
                    printContract = (ContractWarrantPrinter)temp;
                    tempPath = Path.Combine(TheApp.GetApplicationPath(), temp.TemplatePath);
                }
                #endregion

                printContract.dbContext = dbContext;
                printContract.CurrentZone = currentZone;
                printContract.RegeditBook = warrant;
                printContract.Concord = currentConcord;
                printContract.LandCollection = contractLands;
                printContract.BatchExport = IsBatch;
                printContract.Contractor = vpn;
                printContract.Tissue = Tissue;
                printContract.DictList = DictList;
                printContract.TempleFilePath = tempPath;             
                printContract.UseExcel = ExtendUseExcelDefine.WarrantExtendByExcel;              
                printContract.BookPersonNum = BookPersonNum;
                printContract.BookLandNum = BookLandNum;
                printContract.BookNumSetting = BookNumSetting;
                printContract.ExportContractLand(savePath);
            }
            catch (System.Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());
            }
        }

        #endregion

        #region 登记簿

        /// <summary>
        /// 登记簿预览
        /// </summary>
        /// <param name="zone">当前地域</param>
        /// <param name="concord">合同</param>
        public void PrivewRegeditBookWord(Zone zone, ContractConcord concord, string fileName = "", bool isPrint = true)
        {
            try
            {
                if (zone == null)
                {
                    return;
                }
                var zonelist = GetParentZone(zone, dbContext);
                var data = new ContractRegeditBookPrinterData(concord);
                data.CurrentZone = zone;
                data.DbContext = dbContext;
                data.SystemDefine = SystemSet;
                data.AccountLandBusiness = AccountLandBusiness;
                data.ConcordBusiness = concordBusiness;
                data.DictBusiness = DictBusiness;
                data.PersonBusiness = PersonBusiness;
                data.SystemDefine = SystemSet;
                data.InitializeInnerData();
                string tempPath = TemplateHelper.WordTemplate(TemplateFile.PrivewRegeditBookWord);
                var regeditBookWord = new ContractRegeditBookWork();

                #region 通过反射等机制定制化具体的业务处理类
                var temp = WorksheetConfigHelper.GetInstance(regeditBookWord);
                if (temp != null && temp.TemplatePath != null)
                {
                    if (temp is ContractRegeditBookWork)
                    {
                        regeditBookWord = (ContractRegeditBookWork)temp;
                    }
                    tempPath = Path.Combine(TheApp.GetApplicationPath(), temp.TemplatePath);
                }
                #endregion

                regeditBookWord.DbContext = dbContext;
                regeditBookWord.ZoneList = zonelist;
                regeditBookWord.DictList = DictBusiness.GetAll();
                regeditBookWord.Tissue = data.Tissue;
                regeditBookWord.OpenTemplate(tempPath);
                if (isPrint)
                {
                    VirtualPerson vp = personStation.Get(concord.ContracterId.Value);
                    regeditBookWord.PrintPreview(data, SystemSet.DefaultPath + @"\" + vp.FamilyNumber + "-" + concord.ContracterName + "-" + concord.ConcordNumber + "-" + TemplateFile.PrivewRegeditBookWord);
                }
                else
                {
                    VirtualPerson vp = personStation.Get(concord.ContracterId.Value);
                    string filePath = fileName + @"\" + vp.FamilyNumber + "-" + concord.ContracterName + "-" + concord.ConcordNumber + "-" + TemplateFile.PrivewRegeditBookWord;
                    regeditBookWord.SaveAs(data, filePath);
                }
                data = null;
            }
            catch (Exception ex)
            {
                this.ReportError(ex.Message);
                YuLinTu.Library.Log.Log.WriteException(this, "(导出数据到Word文档)", ex.Message + ex.StackTrace);
            }
        }

        /// <summary>
        /// 导出登记簿(单)
        /// </summary>
        /// <param name="zone"></param>
        /// <param name="concord"></param>
        /// <param name="fileName"></param>
        /// <param name="message"></param>
        public bool SingleExportRegeditBookWord(Zone zone, ContractConcord concord, string fileName, out string message)
        {
            if (zone == null)
            {
                message = "地域为空!";
                return false;
            }
            PrivewRegeditBookWord(zone, concord, fileName, false); //保存
            message = string.Format("成功导出承包方{0}登记簿", concord.ContracterName);
            return true;
        }

        /// <summary>
        /// 批量导出权证登记簿
        /// </summary>
        /// <param name="zone"></param>
        /// <param name="fileName"></param>
        /// <param name="isPrint"></param>
        public void BatchExportRegeditBookTable(Zone zone, string fileName, bool isPrint = false)
        {

        }

        #endregion

        #region 工具(数据汇总表、单户确认表、颁证清册)

        /// <summary>
        /// 导出权证数据汇总表
        /// </summary>
        public void ExportWarrentSummaryTable(Zone zone, string fileName, double averagePercent = 0.0, double currentPercent = 0.0)
        {
            try
            {
                if (zone == null)
                {
                    this.ReportError("未选择导出数据的地域!");
                    return;
                }
                string excelName = GetMarkDesc(zone);
                List<VirtualPerson> vps = concordBusiness.GetByZone(zone.FullCode);
                List<ContractConcord> concords = concordBusiness.GetCollection(zone.FullCode);
                List<ContractLand> lands = concordBusiness.GetLandsByZoneCode(zone.FullCode);
                List<ContractRegeditBook> books = GetByZoneCode(zone.FullCode, eSearchOption.Precision);
                string tempPath = TemplateHelper.ExcelTemplate(TemplateFile.ContractSummaryExcel);  //获取模板
                string uinitName = concordBusiness.GetUinitName(zone);   //单位名称
                using (ExportContractorSummaryExcel warrentSummaryExport = new ExportContractorSummaryExcel(dbContext))
                {
                    warrentSummaryExport.SaveFilePath = fileName + @"\" + excelName + "权证数据汇总表" + ".xls";
                    warrentSummaryExport.CurrentZone = zone;
                    warrentSummaryExport.ListPerson = vps;
                    warrentSummaryExport.ListConcord = concords;
                    warrentSummaryExport.ListLand = lands;
                    warrentSummaryExport.ListBook = books;
                    warrentSummaryExport.UnitName = uinitName;
                    warrentSummaryExport.StatuDes = excelName + "权证数据汇总表";
                    warrentSummaryExport.Percent = averagePercent;
                    warrentSummaryExport.CurrentPercent = currentPercent;
                    warrentSummaryExport.ZoneDesc = excelName;
                    warrentSummaryExport.ArgType = this.ArgType;
                    warrentSummaryExport.PostProgressEvent += export_PostProgressEvent;
                    warrentSummaryExport.PostErrorInfoEvent += export_PostErrorInfoEvent;
                    bool result = warrentSummaryExport.BeginExcel(tempPath);   //开始导表                 
                }
                vps = null;
                concords = null;
                lands = null;
                books = null;
                GC.Collect();
            }
            catch (Exception ex)
            {
                this.ReportError(ex.Message);
                YuLinTu.Library.Log.Log.WriteException(this, "ExportWarrentSummaryTable(导出权证数据汇总表到Excel)", ex.Message + ex.StackTrace);
            }
        }

        /// <summary>
        /// 导出颁证清册
        /// </summary>
        public void ExportAwareInventoryTable(Zone zone, string fileName, double averagePercent = 0.0, double currentPercent = 0.0)
        {
            try
            {
                if (zone == null)
                {
                    this.ReportError("未选择导出数据的地域!");
                    return;
                }
                string excelName = GetMarkDesc(zone);
                List<VirtualPerson> vps = concordBusiness.GetByZone(zone.FullCode);
                List<ContractConcord> concords = concordBusiness.GetCollection(zone.FullCode);
                string tempPath = TemplateHelper.ExcelTemplate(TemplateFile.AwareInventoryTable);  //获取模板  
                using (ExportAwareInventoryTable awareExport = new ExportAwareInventoryTable(dbContext))
                {
                    awareExport.SaveFilePath = fileName + @"\" + excelName + "颁证清册" + ".xls";
                    awareExport.CurrentZone = zone;
                    awareExport.ListPerson = vps;
                    awareExport.ListConcord = concords;
                    awareExport.StatuDes = excelName + "颁证清册";
                    awareExport.Percent = averagePercent;
                    awareExport.CurrentPercent = currentPercent;
                    awareExport.ZoneDesc = excelName;
                    awareExport.PostProgressEvent += export_PostProgressEvent;
                    awareExport.PostErrorInfoEvent += export_PostErrorInfoEvent;
                    awareExport.BeginToZone(tempPath);   //开始导表
                }
                vps = null;
                concords = null;
                GC.Collect();
            }
            catch (Exception ex)
            {
                this.ReportError(ex.Message);
                YuLinTu.Library.Log.Log.WriteException(this, "ExportAwareInventoryTable(导出颁证清册到Excel)", ex.Message + ex.StackTrace);
            }
        }

        /// <summary>
        /// 导出(预览)单户确认表
        /// </summary>
        /// <param name="zone">当前地域</param>
        /// <param name="listPerson">承包方集合</param>
        /// <param name="fileName">保存文件路径</param>
        /// <param name="isShow">是否预览</param>
        public void ExportFamilyConfirmTable(Zone zone, List<VirtualPerson> listPerson, string fileName = "", bool isShow = true)
        {
            try
            {
                if (zone == null)
                {
                    this.ReportError("未选择导出数据的地域!");
                    return;
                }
                if (!isShow)
                    this.ReportProgress(1, "开始");
                List<ContractLand> listLand = AccountLandBusiness.GetCollection(zone.FullCode, eLevelOption.Self);
                listLand.LandNumberFormat(SystemSet);
                List<ContractConcord> listConcord = concordBusiness.GetCollection(zone.FullCode);
                List<ContractRegeditBook> listRegeditBook = GetByZoneCode(zone.FullCode, eSearchOption.Precision);
                string excelName = GetMarkDesc(zone);
                string reInfo = string.Format("在{0}下成功导出{1}条单户确认信息", excelName, listPerson.Count);
                string templatePath = TemplateHelper.ExcelTemplate(TemplateFile.SingleFamilyConfirmTable);  //获取模板  
                double percent = 99 / (double)listPerson.Count;  //计算百分比
                int index = 1;  //导出标记 

                var zoneStation = dbContext.CreateZoneWorkStation();
                string villageName = zoneStation.GetVillageName(zone);
                string unitName = GetZoneName(zone, zoneStation);
                string titleName = GetTitle(zone, zoneStation);

                foreach (var person in listPerson)
                {
                    ExportSingleFamilyConfirmTable confirmExport = new ExportSingleFamilyConfirmTable(dbContext);
                    confirmExport.CurrentZone = zone;
                    confirmExport.ListLand = listLand == null ? new List<ContractLand>() : listLand;
                    confirmExport.ListConcord = listConcord;
                    confirmExport.ListRegeditBook = listRegeditBook;
                    //confirmExport.TitleName = titleName;
                    //confirmExport.UnitName = SystemSet.CountryTableHead ? villageName : unitName;
                    confirmExport.UnitName = SystemSet.GetTableHeaderStr(zone);
                    confirmExport.TitleName = SystemSet.GetTBDWStr(zone);
                    confirmExport.ZoneBusiness = this.zoneBusiness;
                    confirmExport.StatuDes = excelName + "单户确认表";
                    confirmExport.SaveFilePath = fileName + @"\" + person.FamilyNumber + "-" + person.Name + "-" + "单户确认表" + ".xls";
                    if (isShow)
                    {
                        confirmExport.PostProgressEvent +=export_PostProgressEvent;
                    }
                    confirmExport.PostErrorInfoEvent += export_PostErrorInfoEvent;
                    confirmExport.BeginToVirtualPerson(person, templatePath);   //开始导表
                    if (isShow)
                    {
                        //批量处理时不显示
                        System.Diagnostics.Process.Start(confirmExport.SaveFilePath);
                    }
                    else
                    {
                        this.ReportProgress((int)(1 + percent * index), string.Format("{0}", excelName + person.Name));
                    }
                    index++;
                }
                if (!isShow)
                    this.ReportProgress(100, "完成");
                this.ReportInfomation(reInfo);
                listLand = null;
                listConcord = null;
                listRegeditBook = null;
                GC.Collect();
            }
            catch (Exception ex)
            {
                this.ReportError(ex.Message);
                YuLinTu.Library.Log.Log.WriteException(this, "ExportSingleFamilyConfirmTable(导出单户确认表到Excel)", ex.Message + ex.StackTrace);
            }
        }

        /// <summary>
        /// 批量导出单户确认表(用于选择地域大于组级时)
        /// </summary>
        /// <param name="zone">当前地域</param>
        /// <param name="fileName">保存文件路径</param>
        /// <param name="isShow">是否预览</param>
        /// <param name="averagePercent">平均百分比</param>
        /// <param name="percent">当前百分比</param>
        public void ExportFamilyConfirmVolumn(Zone zone, List<VirtualPerson> listPerson,
            string fileName = "", bool isShow = true, double averagePercent = 0.0, double percent = 0.0)
        {
            try
            {
                if (zone == null)
                {
                    this.ReportError("未选择导出数据的地域!");
                    return;
                }

                List<ContractLand> listLand = AccountLandBusiness.GetCollection(zone.FullCode, eLevelOption.Self);
                List<ContractConcord> listConcord = concordBusiness.GetCollection(zone.FullCode);
                List<ContractRegeditBook> listRegeditBook = GetByZoneCode(zone.FullCode, eSearchOption.Precision);
                string excelName = GetMarkDesc(zone);
                string templatePath = TemplateHelper.ExcelTemplate(TemplateFile.SingleFamilyConfirmTable);  //获取模板  
                double tempPercent = averagePercent / (double)listPerson.Count;  //计算百分比
                int index = 1;  //导出标记 
                foreach (var person in listPerson)
                {
                    ExportSingleFamilyConfirmTable confirmExport = new ExportSingleFamilyConfirmTable(dbContext);
                    confirmExport.CurrentZone = zone;
                    confirmExport.ListLand = listLand;
                    confirmExport.ListConcord = listConcord;
                    confirmExport.ListRegeditBook = listRegeditBook;
                    confirmExport.ZoneBusiness = this.zoneBusiness;
                    confirmExport.StatuDes = excelName + "单户确认表";
                    confirmExport.SaveFilePath = fileName + @"\" + person.FamilyNumber + "-" + person.Name + "-" + "单户确认表" + ".xls";
                    if (isShow)
                    {
                        confirmExport.PostProgressEvent +=export_PostProgressEvent;
                    }
                    confirmExport.PostErrorInfoEvent += export_PostErrorInfoEvent;
                    confirmExport.BeginToVirtualPerson(person, templatePath);   //开始导表
                    this.ReportProgress((int)(percent + tempPercent * index), string.Format("{0}", excelName + person.Name));
                    index++;
                }
                listLand = null;
                listConcord = null;
                listRegeditBook = null;
                GC.Collect();
            }
            catch (Exception ex)
            {
                this.ReportError(ex.Message);
                YuLinTu.Library.Log.Log.WriteException(this, "ExportFamilyConfirmVolumn(批量导出单户确认表到Excel)", ex.Message + ex.StackTrace);
            }
        }

        #endregion

        #region 辅助方法

        /// <summary>
        /// 获取标题
        /// </summary>
        private string GetTitle(Zone currentZone, IZoneWorkStation zoneStation)
        {
            if (currentZone != null && currentZone.FullCode.Length > 0)
            {
                Zone county = zoneStation.Get(currentZone.FullCode.Substring(0, Zone.ZONE_COUNTY_LENGTH));
                Zone city = zoneStation.Get(currentZone.FullCode.Substring(0, Zone.ZONE_CITY_LENGTH));
                if (city != null && county != null)
                {
                    string zoneName = county.FullName.Replace(city.FullName, "");
                    return city.Name + zoneName.Substring(0, zoneName.Length - 1);
                }
                return currentZone.Name;
            }
            return "";
        }

        /// <summary>
        /// 获取单位名称
        /// </summary>   
        private string GetZoneName(Zone zone, IZoneWorkStation zoneStation)
        {
            Zone county = zoneStation.Get(zone.FullCode.Substring(0, Zone.ZONE_COUNTY_LENGTH));
            if (county == null)
            {
                return zone.FullName;
            }
            return zone.FullName.Replace(county.FullName, "");
        }

        /// <summary>
        /// 根据当前地域获得任务描述信息
        /// </summary>
        public string GetMarkDesc(Zone zone)
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
        /// 获取地域集合
        /// </summary>
        public List<Zone> GetParentZone(Zone zone)
        {
            ModuleMsgArgs arg = new ModuleMsgArgs();
            arg.Datasource = dbContext;
            arg.Parameter = zone;
            arg.Name = ZoneMessage.ZONE_PARENTS_ZONE;
            TheBns.Current.Message.Send(this, arg);
            return (arg.ReturnValue as List<Zone>);
        }
        /// <summary>    
        /// 获取地域集合
        /// </summary>
        public List<Zone> GetParentZone(Zone zone, IDbContext dbContext)
        {
            ModuleMsgArgs arg = new ModuleMsgArgs();
            arg.Datasource = dbContext;
            arg.Parameter = zone;
            arg.Name = ZoneMessage.ZONE_PARENTS_TOPROVINCEZONE;
            TheBns.Current.Message.Send(this, arg);
            return (arg.ReturnValue as List<Zone>);
        }
        /// <summary>
        /// 获取地域下所有子级地域集合
        /// </summary>
        /// <param name="zone"></param>
        /// <returns></returns>
        public List<Zone> GetChildrenZone(Zone zone)
        {
            ModuleMsgArgs arg = new ModuleMsgArgs();
            arg.Datasource = dbContext;
            arg.Parameter = zone;
            arg.Name = ZoneMessage.ZONE_CHILDREN_ZONE;
            TheBns.Current.Message.Send(this, arg);
            return (arg.ReturnValue as List<Zone>);
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

        /// <summary>
        /// 根据地域编码获取承包方
        /// </summary>
        public List<VirtualPerson> GetByZone(string zoneCode)
        {
            List<VirtualPerson> list = null;
            if (!CanContinue() || string.IsNullOrEmpty(zoneCode))
            {
                return list;
            }
            try
            {
                list = personStation.GetByZoneCode(zoneCode);
            }
            catch (Exception ex)
            {
                YuLinTu.Library.Log.Log.WriteException(this, "ZonesByCode(获取承包方数据集合)", ex.Message + ex.StackTrace);
                this.ReportError("获取承包方数据出错," + ex.Message);
            }
            return list;
        }

        /// <summary>
        /// 辅助判断方法
        /// </summary>
        public bool CanContinue()
        {
            if (regeditBookStation == null)
            {
                this.ReportError("尚未初始化承包合同的访问接口");
                return false;
            }
            return true;
        }

        /// <summary>
        /// 进度报告
        /// </summary>
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

        ///// <summary>
        ///// 错误信息报告
        ///// </summary>
        //private void ReportInfo(object sender, TaskAlertEventArgs e)
        //{
        //    if (e != null)
        //    {
        //        this.ReportAlert(e.Grade, e.UserState, e.Description);
        //        if (e.Grade == eMessageGrade.Error)
        //            isErrorRecord = true;
        //    }
        //}

        /// <summary>
        /// 报告进度
        /// </summary>
        private void export_PostProgressEvent(int progress, object userState)
        {
            this.ReportProgress(progress, userState);
        }

        #endregion

        #endregion
    }
}
