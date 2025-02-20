/*
 * (C) 2025  鱼鳞图公司版权所有,保留所有权利 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using YuLinTu.Library.Entity;
using YuLinTu.Windows;
using YuLinTu;
using YuLinTu.Data;
using YuLinTu.Library.WorkStation;
using System.IO;

namespace YuLinTu.Library.Business
{
    /// <summary>
    /// 承包合同业务处理
    /// </summary>
    public class ConcordBusiness : Task
    {
        #region Fields

        private IDbContext dbContext;
        //private bool isErrorRecord;
        private DictionaryBusiness dictBusiness;
        private VirtualPersonBusiness personBusiness;
        private List<Dictionary> dictList = new List<Dictionary>();  //数据字典集合
        private AccountLandBusiness landBusiness;

        /// <summary>
        /// 承包合同业务逻辑层
        /// </summary>
        private IConcordWorkStation concordStation;

        #endregion

        #region Properties

        /// <summary>
        ///  审核申请日期设置
        /// </summary>
        public DateSetting PublishDateSetting { get; set; }

        /// <summary>
        /// 承包方类型
        /// </summary>
        public eVirtualType VirtualType { get; set; }

        /// <summary>
        /// 导出表类型
        /// </summary>
        public eContractAccountType ArgType { get; set; }

        /// <summary>
        /// 汇总表设置实体
        /// </summary>
        public DataSummaryDefine SummaryDefine { get; set; }
        /// <summary>
        /// 导出合同面积类型
        /// </summary>
        public int AreaType { get; set; }

        #region Properties - 合同数据处理

        /// <summary>
        /// 合同集合(待初始化)
        /// </summary>
        public List<ContractConcord> ConcordsModified { get; set; }

        /// <summary>

        /// 农村土地承包经营申请登记表业务逻辑层
        /// </summary>
        private IContractRequireTableWorkStation requireTableWorkStation;
        private IContractLandWorkStation landWorkStation;

        /// <summary>
        /// 地块集合(待更改面积)
        /// </summary>
        public List<ContractLand> LandsOfInitialConcord { get; set; }

        /// <summary>
        /// 发包方
        /// </summary>
        public CollectivityTissue Sender { get; set; }

        /// <summary>
        /// 记录警告信息
        /// </summary>
        public string MessageWarn { get; set; }

        #endregion

        /// <summary>
        /// 承包台账(承包方)Station
        /// </summary>
        private IVirtualPersonWorkStation<LandVirtualPerson> personStation;

        /// <summary>
        /// 系统信息常规设置
        /// </summary>
        public SystemSetDefine SystemSet { get; set; }

        public List<Dictionary> DictList
        {
            get { return dictList; }
            set { dictList = value; }
        }

        #endregion

        #region Ctor

        /// <summary>
        /// 构造函数
        /// </summary>
        public ConcordBusiness(IDbContext db)
        {
            dbContext = db;
            concordStation = db.CreateConcordStation();
            requireTableWorkStation = db.CreateRequireTableWorkStation();
            personStation = db.CreateVirtualPersonStation<LandVirtualPerson>();
            personBusiness = new VirtualPersonBusiness(db);
            personBusiness.VirtualType = eVirtualType.Land;
            dictBusiness = new DictionaryBusiness(dbContext);   //数据字典
            landWorkStation = db.CreateAgriculturalLandWorkstation();
            dictList = dictBusiness.GetAll();
            if (SystemSet == null)
                SystemSet = SystemSetDefine.GetIntence();
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

        /// <summary>
        /// 根据不同匹配等级获取该地域下的合同
        /// </summary>
        /// <param name="zoneCode">区域代码</param>
        /// <param name="searchOption">匹配级别</param>
        /// <returns>农村土地承包合同</returns>
        public List<ContractConcord> GetContractsByZoneCode(string zoneCode, eLevelOption searchOption)
        {
            List<ContractConcord> list = null;
            if (!CanContinue() || string.IsNullOrEmpty(zoneCode))
            {
                return list;
            }
            try
            {
                list = concordStation.GetContractsByZoneCode(zoneCode, searchOption);
            }
            catch (Exception ex)
            {
                YuLinTu.Library.Log.Log.WriteException(this, "GetContractsByZoneCode(获取承包合同集合)", ex.Message + ex.StackTrace);
                this.ReportError("获取承包合同集合失败," + ex.Message);
            }
            return list;
        }

        /// <summary>
        /// 根据id获取承包合同
        /// </summary>
        /// <param name="id">id</param>
        public ContractConcord Get(Guid id)
        {
            ContractConcord concord = null;
            if (!CanContinue() || id == null)
            {
                return concord;
            }
            try
            {
                concord = concordStation.Get(id);
            }
            catch (Exception ex)
            {
                YuLinTu.Library.Log.Log.WriteException(this, "Get(获取承包合同失败)", ex.Message + ex.StackTrace);
                this.ReportError("获取承包合同失败," + ex.Message);
            }
            return concord;
        }


        /// <summary>
        /// 根据农村土地承包合同编号获取合同对象
        /// </summary>
        public ContractConcord Get(string concordNumber)
        {
            ContractConcord concord = null;
            if (!CanContinue() || concordNumber == null)
            {
                return concord;
            }
            try
            {
                concord = concordStation.Get(concordNumber);
            }
            catch (Exception ex)
            {
                YuLinTu.Library.Log.Log.WriteException(this, "Get(获取承包合同失败)", ex.Message + ex.StackTrace);
                this.ReportError("获取承包合同失败," + ex.Message);
            }
            return concord;
        }

        /// <summary>
        /// 判断合同编码是否存在
        /// </summary>
        /// <param name="concordNumber">合同编码</param>
        /// <param name="id">合同ID</param>
        /// <returns></returns>
        public bool IsConcordNumberReapet(string concordNumber, Guid id)
        {
            if (!CanContinue())
            {
                return false;
            }
            if (string.IsNullOrEmpty(concordNumber))
            {
                return false;
            }
            bool result = false;
            try
            {
                if (id == null)
                {
                    result = concordStation.Any(t => t.ConcordNumber == concordNumber);
                }
                else
                {
                    result = concordStation.Any(t => t.ConcordNumber == concordNumber && t.ID != id);
                }
            }
            catch (Exception ex)
            {
                YuLinTu.Library.Log.Log.WriteException(this, "IsConcordNumberReapet(判断合同编码是否存在)", ex.Message + ex.StackTrace);
                this.ReportError("判断合同编码是否存在失败," + ex.Message);
            }
            return result;
        }

        /// <summary>
        /// 根据id删除承包合同
        /// </summary>
        /// <param name="id">id</param>
        public int Delete(Guid id)
        {
            int land = 0;
            if (!CanContinue() || id == null)
            {
                return land;
            }
            try
            {
                land = concordStation.Delete(id);
            }
            catch (Exception ex)
            {
                YuLinTu.Library.Log.Log.WriteException(this, "Delete(删除承包合同失败)", ex.Message + ex.StackTrace);
                this.ReportError("删除承包合同失败," + ex.Message);
            }
            return land;
        }

        /// <summary>
        /// 根据承包方id获取承包合同集合
        /// </summary>
        /// <param name="ownerId">承包方id</param>
        /// <returns>合同集合</returns>
        public List<ContractConcord> GetCollection(Guid ownerId)
        {
            List<ContractConcord> list = null;
            if (!CanContinue() || ownerId == null)
            {
                return list;
            }
            try
            {
                list = concordStation.GetContractsByFamilyID(ownerId);
            }
            catch (Exception ex)
            {
                YuLinTu.Library.Log.Log.WriteException(this, "GetCollection(获取承包合同集合)", ex.Message + ex.StackTrace);
                this.ReportError("获取承包合同集合失败," + ex.Message);
            }
            return list;
        }

        /// <summary>
        /// 更新发包方编码
        /// </summary>
        /// <param name="oldsenderCode"></param>
        /// <param name="newsenderCode"></param>
        /// <param name="zonecode"></param>
        public void UpdataSenderCode(string oldsenderCode, CollectivityTissue sender)
        {
            var concords = concordStation.GetByZoneCode(oldsenderCode);
            if (concords.Count == 0)
                return;
            foreach (var cc in concords)
            {
                cc.ZoneCode = sender.ZoneCode;
                cc.SenderName = sender.Name;
                cc.SenderId = sender.ID;
            }
            concordStation.Updatelist(concords);
        }


        /// <summary>
        /// 根据承包方id集合获取承包方集合
        /// </summary>
        public List<ContractConcord> GetByVpIds(Guid[] ownerIds)
        {
            List<ContractConcord> list = null;
            if (!CanContinue() || ownerIds == null)
            {
                return list;
            }
            try
            {
                list = concordStation.GetContractsByFamilyIDs(ownerIds);
            }
            catch (Exception ex)
            {
                YuLinTu.Library.Log.Log.WriteException(this, "GetByVpIds(获取承包合同集合)", ex.Message + ex.StackTrace);
                this.ReportError("获取承包合同集合失败," + ex.Message);
            }
            return list;
        }

        /// <summary>
        /// 根据发包方id获取承包合同集合
        /// </summary>
        /// <param name="ownerId">承包方id</param>
        /// <returns>合同集合</returns>
        public List<ContractConcord> GetContractsByTissueID(Guid tissueId)
        {
            List<ContractConcord> list = null;
            if (!CanContinue() || tissueId == null)
            {
                return list;
            }
            try
            {
                list = concordStation.GetContractsByTissueID(tissueId);
            }
            catch (Exception ex)
            {
                YuLinTu.Library.Log.Log.WriteException(this, "GetCollection(获取承包合同集合)", ex.Message + ex.StackTrace);
                this.ReportError("获取承包合同集合失败," + ex.Message);
            }
            return list;
        }


        /// <summary>
        ///  根据地域编码获取承包合同集合
        /// </summary>
        /// <param name="zoneCode">地域编码</param>
        /// <returns>合同集合</returns>
        public List<ContractConcord> GetCollection(string zoneCode)
        {
            List<ContractConcord> list = null;
            if (!CanContinue() || string.IsNullOrEmpty(zoneCode))
            {
                return list;
            }
            try
            {
                list = concordStation.GetContractsByZoneCode(zoneCode);
            }
            catch (Exception ex)
            {
                YuLinTu.Library.Log.Log.WriteException(this, "GetCollection(获取承包合同集合)", ex.Message + ex.StackTrace);
                this.ReportError("获取承包合同集合失败," + ex.Message);
            }
            return list;
        }


        /// <summary>
        /// 统计地域下的合同数量
        /// </summary>
        /// <param name="zoneCode">地域编码</param>
        /// <param name="searchOption">条件</param>  
        public int CountConcords(string zoneCode, eLevelOption searchOption)
        {
            if (!CanContinue() || string.IsNullOrEmpty(zoneCode))
            {
                return 0;
            }
            try
            {
                int count = concordStation.CountCords(zoneCode, searchOption);
                return count;
            }
            catch (Exception ex)
            {
                YuLinTu.Library.Log.Log.WriteException(this, "CountConcords(获取地域下的合同数量)", ex.Message + ex.StackTrace);
                this.ReportError("获取地域下的合同数量失败," + ex.Message);
            }
            return 0;
        }

        /// <summary>
        /// 获取存在合同的地域集合 
        /// </summary>
        /// <param name="zoneList">地域集合</param>
        public List<Zone> ExsitZones(List<Zone> zoneList)
        {
            if (!CanContinue() || zoneList == null || zoneList.Count == 0)
            {
                return null;
            }
            try
            {
                return concordStation.ExistZones(zoneList);
            }
            catch (Exception ex)
            {
                YuLinTu.Library.Log.Log.WriteException(this, "ExsitZones(获取承包合同集合)", ex.Message + ex.StackTrace);
                this.ReportError("获取承包合同集合失败," + ex.Message);
            }
            return null;
        }

        /// <summary>
        /// 逐条添加承包合同信息
        /// </summary>
        /// <param name="concord">承包合同对象</param>
        public int Add(ContractConcord concord)
        {
            int addCount = 0;
            if (!CanContinue() || concord == null)
            {
                return addCount;
            }
            try
            {
                addCount = concordStation.Add(concord);
            }
            catch (Exception ex)
            {
                YuLinTu.Library.Log.Log.WriteException(this, "Add(添加合同数据)", ex.Message + ex.StackTrace);
                this.ReportError("添加合同数据失败," + ex.Message);
                throw ex;
            }
            return addCount;
        }

        /// <summary>
        /// 根据承包方ID删除下属合同信息
        /// </summary>
        /// <param name="guid">承包方标识码</param>
        /// <returns>-1（参数错误）/0（失败）/1（成功）</returns>
        public int DeleteByPersonID(Guid guid)
        {
            int deltCount = 0;
            if (!CanContinue() || guid == null)
            {
                return deltCount;
            }
            try
            {
                deltCount = concordStation.Delete(c => c.ContracterId == guid);
            }
            catch (Exception ex)
            {
                YuLinTu.Library.Log.Log.WriteException(this, "DeleteLandByPersonID(删除承包方下的合同)", ex.Message + ex.StackTrace);
                this.ReportError("删除合同数据失败," + ex.Message);
            }
            return deltCount;
        }

        /// <summary>
        /// 更新承包合同信息
        /// </summary>
        /// <param name="concord">承包合同</param>
        public int Update(ContractConcord concord)
        {
            int modifyCount = 0;
            if (!CanContinue() || concord == null)
            {
                return modifyCount;
            }
            try
            {
                modifyCount = concordStation.Update(concord);
            }
            catch (Exception ex)
            {
                YuLinTu.Library.Log.Log.WriteException(this, "Update(编辑合同数据)", ex.Message + ex.StackTrace);
                this.ReportError("编辑合同数据失败," + ex.Message);
                throw ex;
            }
            return modifyCount;
        }

        /// <summary>
        /// 删除承包合同信息
        /// </summary>
        /// <param name="concord">承包合同</param>
        public int Delete(ContractConcord concord)
        {
            int DelCount = 0;
            if (!CanContinue() || concord == null)
            {
                return DelCount;
            }
            try
            {
                DelCount = concordStation.Delete(c => c.ID == concord.ID);
            }
            catch (Exception ex)
            {
                YuLinTu.Library.Log.Log.WriteException(this, "Delete(删除合同数据)", ex.Message + ex.StackTrace);
                this.ReportError("删除合同数据失败," + ex.Message);
            }
            return DelCount;
        }

        /// <summary>
        /// 根据行政地域编码删除该地域下的所有合同
        /// </summary>
        /// <param name="zoneCode">行政地域编码</param>
        public int DeleteByZoneCode(string zoneCode)
        {
            int delAllCount = 0;
            if (!CanContinue())
            {
                return delAllCount;
            }
            try
            {
                if (!string.IsNullOrEmpty(zoneCode))
                {
                    delAllCount = concordStation.DeleteByZoneCode(zoneCode);
                }
            }
            catch (Exception ex)
            {
                YuLinTu.Library.Log.Log.WriteException(this, "DeleteLandByZoneCode(删除合同数据)", ex.Message + ex.StackTrace);
                this.ReportError("删除合同数据失败," + ex.Message);
            }
            return delAllCount;
        }

        /// <summary>
        /// 根据组织编码获得以申请书编号排序农村土地承包经营申请登记表
        /// </summary>
        /// <param name="tissueCode">组织编码</param>
        /// <returns>申请登记表</returns>
        public List<ContractRequireTable> GetTissueRequireTable(string tissueCode)
        {
            List<ContractRequireTable> requierTableCollection = null;
            if (!CanContinue() || string.IsNullOrEmpty(tissueCode))
            {
                return requierTableCollection;
            }
            try
            {
                requierTableCollection = requireTableWorkStation.GetTissueRequireTable(tissueCode);
            }
            catch (Exception ex)
            {
                YuLinTu.Library.Log.Log.WriteException(this, "GetTissueRequireTable(获得以申请书编号排序农村土地承包经营申请登记表)", ex.Message + ex.StackTrace);
                this.ReportError("获得以申请书编号排序农村土地承包经营申请登记表失败," + ex.Message);
            }
            return requierTableCollection;
        }
        public int GetMaxNumber()
        {
            return requireTableWorkStation.GetMaxNumber();
        }
        public List<ContractLand> GetLandByConcordId(Guid id)
        {
            return landWorkStation.GetByConcordId(id);
        }
        /// <summary>
        /// 根据申请书编号获取农村土地承包经营申请登记表对象
        /// </summary>
        public ContractRequireTable GetRequireTable(string number)
        {
            ContractRequireTable requireTable = null;
            if (!CanContinue() || string.IsNullOrEmpty(number))
            {
                return requireTable;
            }
            try
            {
                requireTable = requireTableWorkStation.Get(number);
            }
            catch (Exception ex)
            {
                YuLinTu.Library.Log.Log.WriteException(this, "GetTissueRequireTable(农村土地承包经营申请登记表)", ex.Message + ex.StackTrace);
                this.ReportError("农村土地承包经营申请登记表失败," + ex.Message);
            }
            return requireTable;
        }

        /// <summary>
        /// 添加申请表数据
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public int AddRequireTable(ContractRequireTable entity)
        {
            int addCount = 0;
            if (!CanContinue() || entity == null)
            {
                return addCount;
            }
            try
            {
                addCount = requireTableWorkStation.Add(entity);
            }
            catch (Exception ex)
            {
                YuLinTu.Library.Log.Log.WriteException(this, "Add(登记申请书数据)", ex.Message + ex.StackTrace);
                this.ReportError("登记申请书数据失败," + ex.Message);
            }
            return addCount;
        }

        /// <summary>
        /// 删除申请表记录信息
        /// </summary>
        /// <returns></returns>
        public int DeleteRequireTable(Guid guid)
        {
            int DelCount = 0;
            if (!CanContinue() || guid == null)
            {
                return DelCount;
            }
            try
            {
                DelCount = requireTableWorkStation.Delete(guid);
            }
            catch (Exception ex)
            {
                YuLinTu.Library.Log.Log.WriteException(this, "Delete(删除申请书数据)", ex.Message + ex.StackTrace);
                this.ReportError("删除申请书数据失败," + ex.Message);
            }
            return DelCount;
        }

        /// <summary>
        /// 根据根据地域删除申请书记录信息
        /// </summary>
        /// <param name="zoneCode"></param>
        /// <param name="searchOption"></param>
        /// <returns></returns>
        public int DeleteRequireTableByZoneCode(string zoneCode, eSearchOption searchOption)
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
                delByCount = requireTableWorkStation.DeleteByZoneCode(zoneCode, searchOption);
            }
            catch (Exception ex)
            {
                YuLinTu.Library.Log.Log.WriteException(this, "DeleteByZoneCode(删除承包权证失败)", ex.Message + ex.StackTrace);
                this.ReportError("删除承包权证失败" + ex.Message);
            }
            return delByCount;
        }

        /// <summary>
        /// 通过合同编号查看是否存在有此合同
        /// </summary>
        /// <param name="concordNumber"></param>
        /// <returns></returns>
        public bool Exists(string concordNumber)
        {
            bool flag = false;
            if (!CanContinue() || concordNumber == null)
            {
                return flag;
            }
            try
            {
                flag = concordStation.Exists(concordNumber);

            }
            catch (Exception ex)
            {
                YuLinTu.Library.Log.Log.WriteException(this, "ExsitZones(获取承包合同集合)", ex.Message + ex.StackTrace);
                this.ReportError("获取承包合同集合失败," + ex.Message);
            }
            return flag;
        }

        #endregion

        #region 家庭承包方式

        /// <summary>
        /// 预览申请书
        /// </summary>
        private bool PrintRequreBook(Zone zone, VirtualPerson vp, string templemPath, string warnNoConcordInfo, string warnNoLandInfo, eConstructMode typeMode)
        {
            try
            {
                if (vp == null || zone == null)
                {
                    MessageWarn = "地域为空!";
                    return false;
                }
                var zoneStation = dbContext.CreateZoneWorkStation();
                var zonelist = GetParentZone(zone);
                zonelist.Add(zone);
                string tempPath = TemplateHelper.WordTemplate(templemPath);
                ExportLandRequireBook printBook = new ExportLandRequireBook(vp);

                #region 通过反射等机制定制化具体的业务处理类
                var temp = WorksheetConfigHelper.GetInstance(printBook);
                if (temp != null && temp.TemplatePath != null)
                {
                    if (temp is ExportLandRequireBook)
                    {
                        printBook = (ExportLandRequireBook)temp;
                    }
                    tempPath = Path.Combine(TheApp.GetApplicationPath(), temp.TemplatePath);
                }
                #endregion
                printBook.DbContext = dbContext;
                printBook.CurrentZone = zone;
                printBook.ConstructMode = typeMode;
                printBook.ZoneList = zonelist;
                printBook.RequireDate = this.PublishDateSetting.PublishStartDate;
                printBook.CheckDate = this.PublishDateSetting.PublishEndDate;
                printBook.DictList = this.dictList;
                printBook.Concords = GetCollection(vp.ID);
                if (printBook.Concords == null || printBook.Concords.Count == 0)
                {
                    this.ReportWarn(string.Format("{0}下未发现对应签订的合同", vp.Name));
                    return false;
                }
                ContractConcord nowconrd = new ContractConcord();
                var concord = printBook.InitalizeConcord(nowconrd);
                if (concord == null)
                {
                    MessageWarn = warnNoConcordInfo;
                    this.ReportWarn(warnNoConcordInfo);
                    return false;
                }
                printBook.ConcordLands = GetLandsByConcordId(concord);

                if (printBook.ConcordLands == null || printBook.ConcordLands.Count == 0)
                {
                    MessageWarn = string.Format("未获取到合同对应地块集合,合同ID为{0}", concord.ID.ToString());
                    this.ReportWarn(warnNoLandInfo);
                    return false;
                }
                printBook.Lands = GetLandCollenction(vp);
                if (printBook.Lands == null || printBook.Lands.Count == 0)
                {
                    MessageWarn = string.Format("未获取到承包方对应地块集合,名称:{0}", vp.Name.ToString());
                    this.ReportWarn(warnNoLandInfo);
                    return false;
                }
                List<ContractLand> landuse = new List<ContractLand>();
                if (typeMode != eConstructMode.Family)
                {
                    landuse = printBook.Lands.FindAll(l => l.ConstructMode != ((int)eConstructMode.Family).ToString());
                }
                else
                {
                    landuse = printBook.Lands.FindAll(l => l.ConstructMode == ((int)eConstructMode.Family).ToString());
                }

                printBook.Lands = landuse;
                if (vp.IsStockFarmer)
                {
                    var stockLandsvp = dbContext.CreateBelongRelationWorkStation().GetLandByPerson(vp.ID, zone.FullCode);
                    if (stockLandsvp.Count > 0)
                    {
                        printBook.ConcordLands.AddRange(stockLandsvp);
                        printBook.Lands.AddRange(stockLandsvp);
                    }
                }
                if (landuse.Count == 0)
                {
                    MessageWarn = warnNoLandInfo;
                    this.ReportWarn(warnNoLandInfo);
                    return false;
                }
                //printBook.CurrentZoneCounty = zoneStation.Get(vp.SenderCode.Substring(0, Zone.ZONE_COUNTY_LENGTH));
                //printBook.CurrentZoneProvince = zoneStation.Get(vp.SenderCode.Substring(0, Zone.ZONE_PROVICE_LENGTH));
                //printBook.CurrentZoneTown = zoneStation.Get(vp.SenderCode.Substring(0, Zone.ZONE_TOWN_LENGTH));
                //printBook.CurrentZoneCity = zoneStation.Get(vp.SenderCode.Substring(0, Zone.ZONE_CITY_LENGTH));
                //printBook.CurrentZoneVillage = zoneStation.Get(vp.SenderCode.Substring(0, Zone.ZONE_VILLAGE_LENGTH));
                //printBook.CurrentZoneGroup = zoneStation.Get(vp.SenderCode);
                printBook.OpenTemplate(tempPath);
                string filepath = SystemSet.DefaultPath + "\\" + vp.Name;
                printBook.PrintPreview(vp, filepath);
                vp = null;
                GC.Collect();
                return true;
            }
            catch (Exception ex)
            {
                this.ReportError(ex.Message);
                YuLinTu.Library.Log.Log.WriteException(this, "ExportDataWord(导出数据到word表)", ex.Message + ex.StackTrace);
                return false;
            }
        }


        /// <summary>
        /// 导出单户申请书
        /// </summary>
        private bool SingleExportRequireBook(Zone zone, string fileName, VirtualPerson vp, string templemPath, eConstructMode typeMode)
        {
            try
            {
                if (zone == null)
                {
                    MessageWarn = "地域为空!";
                    return false;
                }
                var zoneStation = dbContext.CreateZoneWorkStation();
                var zonelist = GetParentZone(zone);
                zonelist.Add(zone);
                string tempPath = TemplateHelper.WordTemplate(templemPath);
                ExportLandRequireBook printBook = new ExportLandRequireBook(vp);

                #region 通过反射等机制定制化具体的业务处理类
                var temp = WorksheetConfigHelper.GetInstance(printBook);
                if (temp != null && temp.TemplatePath != null)
                {
                    if (temp is ExportLandRequireBook)
                    {
                        printBook = (ExportLandRequireBook)temp;
                    }
                    tempPath = Path.Combine(TheApp.GetApplicationPath(), temp.TemplatePath);
                }
                #endregion
                printBook.DbContext = dbContext;
                printBook.CurrentZone = zone;
                printBook.ZoneList = zonelist;
                printBook.ConstructMode = typeMode;
                printBook.RequireDate = this.PublishDateSetting.PublishStartDate;
                printBook.CheckDate = this.PublishDateSetting.PublishEndDate;
                printBook.DictList = this.dictList;
                printBook.Concords = GetCollection(vp.ID);
                ContractConcord concord = null;
                var useconcord = printBook.InitalizeConcord(concord);
                if (useconcord == null)
                {
                    string warnNoConcordInfo = typeMode == eConstructMode.Family ? string.Format("当前承包方{0}没有签订家庭承包方式合同!", vp.Name) : string.Format("当前承包方{0}没有签订非家庭承包方式合同!", vp.Name);
                    MessageWarn = warnNoConcordInfo;
                    return false;
                }
                printBook.ConcordLands = GetLandsByConcordId(useconcord);
                if (vp.IsStockFarmer)
                {
                    var stockLandsvp = dbContext.CreateBelongRelationWorkStation().GetLandByPerson(vp.ID, zone.FullCode);
                    if (stockLandsvp.Count > 0)
                    {
                        printBook.LandCollection?.AddRange(stockLandsvp);
                        printBook.ConcordLands?.AddRange(stockLandsvp);
                    }
                }
                printBook.Lands = GetLandCollenction(vp);
                List<ContractLand> landuse = new List<ContractLand>();
                if (typeMode != eConstructMode.Family)
                {
                    landuse = printBook.Lands.FindAll(l => l.ConstructMode != ((int)eConstructMode.Family).ToString());
                }
                else
                {
                    landuse = printBook.Lands.FindAll(l => l.ConstructMode == ((int)eConstructMode.Family).ToString());
                }
                if (landuse.Count == 0)
                {
                    string warnNoLandInfo = typeMode == eConstructMode.Family ? string.Format("当前承包方{0}没有家庭承包方式的地块!", vp.Name) : string.Format("当前承包方{0}没有非家庭承包方式的地块!", vp.Name);
                    MessageWarn = warnNoLandInfo;
                    return false;
                }
                //printBook.CurrentZoneCounty = zoneStation.Get(vp.SenderCode.Substring(0, Zone.ZONE_COUNTY_LENGTH));
                //printBook.CurrentZoneProvince = zoneStation.Get(vp.SenderCode.Substring(0, Zone.ZONE_PROVICE_LENGTH));
                //printBook.CurrentZoneTown = zoneStation.Get(vp.SenderCode.Substring(0, Zone.ZONE_TOWN_LENGTH));
                //printBook.CurrentZoneCity = zoneStation.Get(vp.SenderCode.Substring(0, Zone.ZONE_CITY_LENGTH));
                //printBook.CurrentZoneVillage = zoneStation.Get(vp.SenderCode.Substring(0, Zone.ZONE_VILLAGE_LENGTH));
                //printBook.CurrentZoneGroup = zoneStation.Get(vp.SenderCode);
                printBook.OpenTemplate(tempPath);
                string filePath = fileName + @"\" + vp.FamilyNumber + "-" + vp.Name + templemPath;
                printBook.SaveAs(vp, filePath);
                MessageWarn = string.Format("成功导出承包方{0}单户申请书", vp.Name);
                GC.Collect();
                return true;
            }
            catch (Exception ex)
            {
                this.ReportError(ex.Message);
                YuLinTu.Library.Log.Log.WriteException(this, "ExportDataWord(导出数据到word表)", ex.Message + ex.StackTrace);
                return false;
            }
        }

        /// <summary>
        /// 批量导出单户申请书
        /// </summary>
        private void BatchSingleExportRequireBook(Zone zone, string fileName, string templemPath, eConstructMode typeMode)
        {
            try
            {
                if (zone == null)
                {
                    this.ReportError("未选择导出数据的地域!");
                    return;
                }
                List<ContractAccountLandFamily> accountFamilyCollection = new List<ContractAccountLandFamily>();
                //得到承包方的集合
                List<VirtualPerson> vps = GetByZone(zone.FullCode);
                if (vps == null || vps.Count == 0)
                {
                    this.ReportInfomation(zone.FullName + " 下没有承包方数据!");
                    return;
                }
                string markDesc = GetMarkDesc(zone);
                string tempPath = TemplateHelper.WordTemplate(templemPath);
                int percent = (100 / vps.Count);
                this.ReportProgress(10, null);
                int percentIndex = 1;
                var zoneStation = dbContext.CreateZoneWorkStation();
                foreach (var vp in vps)
                {
                    this.ReportProgress(10 + percent * percentIndex, null);
                    ExportLandRequireBook printBook = new ExportLandRequireBook(vp);

                    #region 通过反射等机制定制化具体的业务处理类
                    var temp = WorksheetConfigHelper.GetInstance(printBook);
                    if (temp != null && temp.TemplatePath != null)
                    {
                        if (temp is ExportLandRequireBook)
                        {
                            printBook = (ExportLandRequireBook)temp;
                        }
                        tempPath = Path.Combine(TheApp.GetApplicationPath(), temp.TemplatePath);
                    }
                    #endregion

                    printBook.CurrentZone = zone;
                    printBook.ConstructMode = typeMode;
                    printBook.RequireDate = this.PublishDateSetting.PublishStartDate;
                    printBook.CheckDate = this.PublishDateSetting.PublishEndDate;
                    printBook.DictList = this.dictList;
                    printBook.Concords = GetCollection(vp.ID);
                    ContractConcord concord = null;
                    var useconcord = printBook.InitalizeConcord(concord);
                    if (useconcord == null)
                    {
                        string warnConcordInfo = typeMode == eConstructMode.Family ? string.Format("承包方{0}没有签订家庭承包方式合同!", vp.Name) : string.Format("承包方{0}没有签订非家庭承包方式合同!", vp.Name);
                        this.ReportWarn(warnConcordInfo);
                        continue;
                    }
                    printBook.ConcordLands = GetLandsByConcordId(useconcord);
                    printBook.Lands = GetLandCollenction(vp);

                    List<ContractLand> landuse = new List<ContractLand>();
                    if (typeMode != eConstructMode.Family)
                    {
                        landuse = printBook.Lands.FindAll(l => l.ConstructMode != ((int)eConstructMode.Family).ToString());
                    }
                    else
                    {
                        landuse = printBook.Lands.FindAll(l => l.ConstructMode == ((int)eConstructMode.Family).ToString());
                    }
                    if (landuse.Count == 0)
                    {
                        string warnLandInfo = typeMode == eConstructMode.Family ? string.Format("承包方{0}没有家庭承包方式的地块!", vp.Name) : string.Format("承包方{0}没有非家庭承包方式的地块!", vp.Name);
                        this.ReportWarn(warnLandInfo);
                        continue;
                    }
                    ++percentIndex;
                    printBook.CurrentZoneCounty = zoneStation.Get(vp.ZoneCode.Substring(0, Zone.ZONE_COUNTY_LENGTH));
                    printBook.CurrentZoneProvince = zoneStation.Get(vp.ZoneCode.Substring(0, Zone.ZONE_PROVICE_LENGTH));
                    printBook.CurrentZoneTown = zoneStation.Get(vp.ZoneCode.Substring(0, Zone.ZONE_TOWN_LENGTH));
                    printBook.CurrentZoneCity = zoneStation.Get(vp.ZoneCode.Substring(0, Zone.ZONE_CITY_LENGTH));
                    printBook.CurrentZoneVillage = zoneStation.Get(vp.ZoneCode.Substring(0, Zone.ZONE_VILLAGE_LENGTH));
                    printBook.CurrentZoneGroup = zoneStation.Get(vp.ZoneCode);

                    printBook.OpenTemplate(tempPath);
                    string filePath = fileName + @"\" + vp.FamilyNumber + "-" + vp.Name + templemPath;
                    printBook.SaveAs(vp, filePath);
                }
                this.ReportProgress(100, "完成");
                this.ReportInfomation(string.Format("从{0}下成功导出{1}个" + templemPath, markDesc, percentIndex - 1));
                vps = null;
            }
            catch (Exception ex)
            {
                this.ReportError(ex.Message);
                YuLinTu.Library.Log.Log.WriteException(this, "ExportDataWord(导出数据到word表)", ex.Message + ex.StackTrace);
            }
        }

        /// <summary>
        /// 预览单户申请书
        /// </summary>
        public bool PrintRequireBookWord(Zone zone, VirtualPerson vp, out string message)
        {
            if (vp == null || zone == null)
            {
                message = "地域为空!";
                return false;
            }
            bool flag = false;
            string tempPath = TemplateFile.ContractFamilyRequireBook;
            string concordWarnInfo = string.Format("当前承包方{0}没有签订家庭承包方式合同!", vp.Name);
            string landWarnInfo = string.Format("当前承包方{0}没有家庭承包方式的地块!", vp.Name);
            flag = PrintRequreBook(zone, vp, tempPath, concordWarnInfo, landWarnInfo, eConstructMode.Family);
            message = MessageWarn;
            return flag;
        }

        /// <summary>
        /// 导出单户申请书
        /// </summary>
        public bool SingleExportRequireBookWord(Zone zone, string fileName, VirtualPerson vp, out string message)
        {
            if (zone == null)
            {
                message = "地域为空!";
                return false;
            }
            string tempPath = TemplateFile.ContractFamilyRequireBook;
            bool flag = SingleExportRequireBook(zone, fileName, vp, tempPath, eConstructMode.Family);
            message = MessageWarn;
            return flag;
        }

        /// <summary>
        /// 预览集体申请书
        /// </summary>
        /// <param name="zone"></param>
        public void PrintViewApplicationBookWord(Zone zone)
        {
            if (zone == null)
            {
                return;
            }
            List<CollectivityTissue> tissues = GetTissuesByConcord(zone);
            foreach (CollectivityTissue tissue in tissues)
            {
                List<ContractRequireTable> tabs = GetTissueRequireTable(tissue.Code);
                bool first = tabs == null || tabs.Count == 0;
                if (tabs != null && tabs.Count > 0)
                {
                    first = tabs.Find(tb => tb.ZoneCode == zone.FullCode) == null;
                }
                if (first)
                {
                    PrintApplicationFirst(tissue, zone, true);
                }
                else
                {
                    PrintApplicationOld(tissue, zone, true);
                }
            }
        }

        /// <summary>
        /// 打印老申请书
        /// </summary>
        public bool PrintApplicationOld(CollectivityTissue tissue, Zone currentZone, bool printView, string fileName = "")
        {
            if (tissue == null || currentZone == null)
            {
                return false;
            }
            List<ContractRequireTable> tabs = GetTissueRequireTable(tissue.Code);
            ContractRequireTable tab = tabs[tabs.Count - 1];
            ConcordApplicationPrinterData data = new ConcordApplicationPrinterData(dbContext, tissue, currentZone, tab.Year, tab.Number);
            var zoneStation = dbContext.CreateZoneWorkStation();
            Zone zoneTemp;
            if (tissue.ZoneCode.Length >= Zone.ZONE_PROVICE_LENGTH)
            {
                zoneTemp = zoneStation.Get(tissue.ZoneCode.Substring(0, Zone.ZONE_PROVICE_LENGTH));
                data.NameProvice = zoneTemp != null ? zoneTemp.Name : "";
            }
            if (tissue.ZoneCode.Length >= Zone.ZONE_CITY_LENGTH)
            {
                zoneTemp = zoneStation.Get(tissue.ZoneCode.Substring(0, Zone.ZONE_CITY_LENGTH));
                data.NameCity = zoneTemp != null ? zoneTemp.Name : "";
            }
            if (tissue.ZoneCode.Length >= Zone.ZONE_TOWN_LENGTH)
            {
                zoneTemp = zoneStation.Get(tissue.ZoneCode.Substring(0, Zone.ZONE_TOWN_LENGTH));
                data.NameTown = zoneTemp != null ? zoneTemp.Name : "";
            }
            if (tissue.ZoneCode.Length >= Zone.ZONE_COUNTY_LENGTH)
            {
                zoneTemp = zoneStation.Get(tissue.ZoneCode.Substring(0, Zone.ZONE_COUNTY_LENGTH));
                data.NameCounty = zoneTemp != null ? zoneTemp.Name : "";
            }
            if (tissue.ZoneCode.Length >= Zone.ZONE_VILLAGE_LENGTH)
            {
                zoneTemp = zoneStation.Get(tissue.ZoneCode.Substring(0, Zone.ZONE_VILLAGE_LENGTH));
                data.NameVillage = zoneTemp != null ? zoneTemp.Name : "";
            }
            if (tissue.ZoneCode.Length >= Zone.ZONE_GROUP_LENGTH)
            {
                zoneTemp = zoneStation.Get(tissue.ZoneCode.Substring(0, Zone.ZONE_GROUP_LENGTH));
                data.NameGroup = zoneTemp != null ? zoneTemp.Name : "";
            }

            data.RequireDate = tab.Date;
            data.LandCollection = GetLandsByZoneCode(currentZone.FullCode);
            data.ConcordCollection = GetContractsByTissueID(tissue.ID);
            data.DictList = this.dictList;
            data.InitializeInnerData();
            string templatePath = TemplateHelper.WordTemplate(TemplateFile.ContractCollectApplicationBook);
            ConcordApplicationPrinter concordApplication = new ConcordApplicationPrinter();

            #region 支持反射，以根据具体业务实例化不同子类
            var temp = WorksheetConfigHelper.GetInstance(concordApplication);
            if (temp != null && temp.TemplatePath != null)
            {
                if (temp is ConcordApplicationPrinter)
                {
                    concordApplication = (ConcordApplicationPrinter)temp;
                }
                templatePath = Path.Combine(TheApp.GetApplicationPath(), temp.TemplatePath);
            }
            #endregion

            concordApplication.OpenTemplate(templatePath);
            if (printView)
            {
                string filePath = SystemSet.DefaultPath + @"\" + tissue.Name + TemplateFile.ContractCollectApplicationBook;
                concordApplication.PrintPreview(data, filePath);
            }
            else
            {
                string filePath = fileName + @"\" + tissue.Name + TemplateFile.ContractCollectApplicationBook;
                concordApplication.SaveAs(data, filePath);
            }
            data = null;
            tabs = null;
            tab = null;
            GC.Collect();
            return true;
        }

        /// <summary>
        /// 打印第一个申请书
        /// </summary>
        public bool PrintApplicationFirst(CollectivityTissue tissue, Zone currentZone, bool printView, string fileName = "", ConcordApplicationSet applicationSet = null)
        {
            if (tissue == null || currentZone == null)
            {
                return false;
            }
            //string requireNumber = applicationSet == null ? GenerateNumber(tissue) : applicationSet.ApplicationBookNumber;
            string requireNumber = applicationSet == null ? (GetMaxNumber() + 1).ToString() : applicationSet.ApplicationBookNumber;
            string year = applicationSet == null ? DateTime.Now.Year.ToString() : applicationSet.YearNumber;
            string templatePath = TemplateHelper.WordTemplate(TemplateFile.ContractCollectApplicationBook);
            ConcordApplicationPrinterData data = new ConcordApplicationPrinterData(dbContext, tissue, currentZone, year, number: requireNumber);
            Zone zoneTemp;
            var zoneStation = dbContext.CreateZoneWorkStation();
            if (tissue.ZoneCode.Length >= Zone.ZONE_PROVICE_LENGTH)
            {
                zoneTemp = zoneStation.Get(tissue.ZoneCode.Substring(0, Zone.ZONE_PROVICE_LENGTH));
                data.NameProvice = zoneTemp != null ? zoneTemp.Name : "";
            }
            if (tissue.ZoneCode.Length >= Zone.ZONE_CITY_LENGTH)
            {
                zoneTemp = zoneStation.Get(tissue.ZoneCode.Substring(0, Zone.ZONE_CITY_LENGTH));
                data.NameCity = zoneTemp != null ? zoneTemp.Name : "";
            }
            if (tissue.ZoneCode.Length >= Zone.ZONE_TOWN_LENGTH)
            {
                zoneTemp = zoneStation.Get(tissue.ZoneCode.Substring(0, Zone.ZONE_TOWN_LENGTH));
                data.NameTown = zoneTemp != null ? zoneTemp.Name : "";
            }
            if (tissue.ZoneCode.Length >= Zone.ZONE_COUNTY_LENGTH)
            {
                zoneTemp = zoneStation.Get(tissue.ZoneCode.Substring(0, Zone.ZONE_COUNTY_LENGTH));
                data.NameCounty = zoneTemp != null ? zoneTemp.Name : "";
            }
            if (tissue.ZoneCode.Length >= Zone.ZONE_VILLAGE_LENGTH)
            {
                zoneTemp = zoneStation.Get(tissue.ZoneCode.Substring(0, Zone.ZONE_VILLAGE_LENGTH));
                data.NameVillage = zoneTemp != null ? zoneTemp.Name : "";
            }
            if (tissue.ZoneCode.Length >= Zone.ZONE_GROUP_LENGTH)
            {
                zoneTemp = zoneStation.Get(tissue.ZoneCode.Substring(0, Zone.ZONE_GROUP_LENGTH));
                data.NameGroup = zoneTemp != null ? zoneTemp.Name : "";
            }
            data.LandCollection = GetLandsByZoneCode(currentZone.FullCode);
            data.ConcordCollection = GetContractsByTissueID(tissue.ID);
            data.RequireDate = applicationSet == null ? DateTime.Now : applicationSet.CheckDate;
            data.DictList = this.dictList;
            data.InitializeInnerData();
            data.RequireTable = GetRequireTable(requireNumber);
            ConcordApplicationPrinter concordApplication = new ConcordApplicationPrinter();

            #region 支持反射，以根据具体业务实例化不同子类
            var temp = WorksheetConfigHelper.GetInstance(concordApplication);
            if (temp != null && temp.TemplatePath != null)
            {
                if (temp is ConcordApplicationPrinter)
                {
                    concordApplication = (ConcordApplicationPrinter)temp;
                }
                templatePath = Path.Combine(TheApp.GetApplicationPath(), temp.TemplatePath);
            }
            #endregion

            concordApplication.OpenTemplate(templatePath);
            if (printView)
            {
                string filePath = SystemSet.DefaultPath + @"\" + tissue.Name + TemplateFile.ContractCollectApplicationBook;
                concordApplication.PrintPreview(data, filePath);
            }
            else
            {
                string filePath = fileName + @"\" + tissue.Name + TemplateFile.ContractCollectApplicationBook;
                concordApplication.SaveAs(data, filePath);
            }
            ContractRequireTable table = data.Record();
            AddRequireTable(table);
            UpdateConcordInformation(table.ID, currentZone);
            data = null;
            GC.Collect();
            return true;
        }

        /// <summary>
        /// 导出集体申请书
        /// </summary>
        public bool ExportApplicationBookWord(Zone zone, string fileName)
        {
            if (zone == null)
            {
                return false;
            }
            bool flag = false;
            List<CollectivityTissue> tissues = GetTissuesByConcord(zone, eLevelOption.Self);
            if (tissues == null || tissues.Count == 0)
            {
                if (zone.Level != eZoneLevel.Town && zone.Level != eZoneLevel.Village)
                {
                    this.ReportInfomation(string.Format("{0}下无数据！", zone.FullName));
                }
                return false;
            }
            foreach (CollectivityTissue tissue in tissues)
            {
                List<ContractRequireTable> tabs = GetTissueRequireTable(tissue.Code);
                bool first = tabs == null || tabs.Count == 0;
                if (tabs != null && tabs.Count > 0)
                {
                    first = !tabs.Any(tb => tb.ZoneCode == zone.FullCode);
                }
                if (first)
                {
                    flag = PrintApplicationFirst(tissue, zone, false, fileName);
                }
                else
                {
                    flag = PrintApplicationOld(tissue, zone, false, fileName);
                }
            }
            return flag;
        }

        /// <summary>
        /// 批量导出集体申请书
        /// </summary>
        public bool BatchExportApplicationBookWord(Zone zone, string fileName)
        {
            if (zone == null)
            {
                return false;
            }
            bool flag = ExportApplicationBookWord(zone, fileName);
            return flag;
        }

        #endregion

        #region 其他承包方式

        /// <summary>
        /// 其他承包方式预览申请书
        /// </summary>
        /// <param name="zone"></param>
        /// <param name="vp"></param>
        public bool PrintViewOtherApplicationBook(Zone zone, VirtualPerson vp, out string message)
        {
            if (vp == null || zone == null)
            {
                message = "地域为空!";
                return false;
            }
            string tempPath = TemplateFile.ContractFamilyOtherRequireBook;
            string concordWarnInfo = string.Format("当前承包方{0}没有签订非家庭承包方式合同!", vp.Name);
            string landWarnInfo = string.Format("当前承包方{0}没有非家庭承包方式的地块!", vp.Name);
            bool flag = PrintRequreBook(zone, vp, tempPath, concordWarnInfo, landWarnInfo, eConstructMode.Other);
            message = MessageWarn;
            return flag;
        }

        /// <summary>
        /// 其他承包方式导出单户申请书
        /// </summary>
        public bool ExportApplicationByOtherBookWord(Zone zone, string fileName, VirtualPerson vp, out string message)
        {
            if (zone == null)
            {
                message = "地域为空!";
                return false;
            }
            string tempPath = TemplateFile.ContractFamilyOtherRequireBook;
            bool flag = SingleExportRequireBook(zone, fileName, vp, tempPath, eConstructMode.Other);
            message = MessageWarn;
            return flag;
        }

        #endregion

        #region 合同数据处理

        /// <summary>
        /// 合同数据处理之签订合同(初始化合同)
        /// </summary>
        /// <param name="currentZone"></param>
        public void InitialConcordData(Zone currentZone, CollectivityTissue sender, bool isCalculateArea = false)
        {
            int index = 1;   //合同索引
            double percent = 0.0;  //百分比
            var db = DataBaseSource.GetDataBaseSource();
            try
            {
                if (db == null)
                {
                    this.ReportError(DataBaseSource.ConnectionError);
                    return;
                }
                if (currentZone == null)
                {
                    this.ReportError("未选择初始化数据的地域!");
                    return;
                }
                //if (currentZone.Level>eZoneLevel.Village && con)
                string zoneDesc = GetMarkDesc(currentZone);
                var cdStation = db.CreateConcordStation();
                var pnStation = db.CreateVirtualPersonStation<LandVirtualPerson>();
                List<ContractConcord> listConcord = cdStation.GetByZoneCode(currentZone.FullCode);
                List<VirtualPerson> listPerson = pnStation.GetByZoneCode(currentZone.FullCode);
                //List<VirtualPerson> listPerson = GetByZone(currentZone.FullCode);
                if (ConcordsModified == null || ConcordsModified.Count == 0)
                {
                    this.ReportWarn(string.Format("在{0}下未获得待签订的承包方!", currentZone.FullName));
                    this.ReportProgress(100, null);
                    return;
                }
                this.ReportProgress(1, "开始初始化承包合同");
                percent = 99 / (double)ConcordsModified.Count;

                db.BeginTransaction();
                //string concordNumber = string.Empty;
                foreach (var concord in ConcordsModified)
                {
                    //concordNumber = concord.ConcordNumber.Length > 14 ? concord.ConcordNumber.Substring(14)
                    //                : concord.ConcordNumber.PadLeft(14, '0').Substring(14);
                    //if (isVillageInlitialSet)
                    //{
                    //    if (currentZone.Level == eZoneLevel.Group)
                    //        concordNumber = currentZone.UpLevelCode.PadLeft(14, '0') + concordNumber;
                    //    else
                    //        concordNumber = currentZone.FullCode.PadLeft(14, '0') + concordNumber;
                    //}
                    //else
                    //{
                    //    concordNumber = currentZone.FullCode.PadRight(14, '0') + concordNumber;
                    //}
                    //concord.ConcordNumber = concordNumber;

                    var person = listPerson.Find(c => c.ID == concord.ContracterId);
                    bool isExsit = listConcord.Any(c => c.ID == concord.ID);
                    //if (sender != null)
                    //{
                    //    concord.SenderId = sender.ID;//发包方Id
                    //    concord.SenderName = sender.Name;//发包方名称
                    //}
                    if (isExsit)
                    {
                        //更新
                        cdStation.Update(concord);
                        //Update(concord);
                    }
                    else
                    {
                        //添加
                        cdStation.Add(concord);
                        //Add(concord);
                    }
                    //VirtualPersonExpand vpexpand = null;
                    //if (person.FamilyExpand != null)
                    //    vpexpand = person.FamilyExpand;
                    //else
                    //    vpexpand = new VirtualPersonExpand();
                    //vpexpand.ConcordNumber = concord.ConcordNumber;
                    //vpexpand.ConcordStartTime = concord.ArableLandStartTime;
                    //vpexpand.ConcordEndTime = concord.ArableLandEndTime;
                    //person.FamilyExpand = vpexpand;
                    //pnStation.Update(person);
                    this.ReportProgress((int)(1 + percent * index), string.Format("{0}", zoneDesc + person.Name));
                    index++;
                }
                if (LandsOfInitialConcord != null && LandsOfInitialConcord.Count > 0)
                {
                    this.ReportProgress(99, "正在修改地块信息");
                    var landStation = db.CreateContractLandWorkstation();
                    int updateCount = landStation.UpdateRange(LandsOfInitialConcord);
                }

                this.ReportProgress(100, null);
                this.ReportInfomation(string.Format("在{0}下共初始化{1}条合同信息", currentZone.FullName, ConcordsModified.Count));

                db.CommitTransaction();
            }
            catch (Exception ex)
            {
                db.RollbackTransaction();
                this.ReportError("签订合同失败!");
                YuLinTu.Library.Log.Log.WriteError(this, "InitialConcordData(提交初始化合同数据)", ex.Message + ex.StackTrace);
            }
        }

        /// <summary>
        /// 预览合同
        /// </summary>
        public void PreviewConcordData(Zone currentZone, ContractConcord concord, VirtualPerson curVp)
        {
            try
            {
                List<ContractLand> lands = GetLandsByConcordId(concord);
                string templatePath = TemplateHelper.WordTemplate(TemplateFile.ContractConcordWord);
                var exportConcord = new ExportContractConcord(dbContext);

                #region 通过反射等机制定制化具体的业务处理类
                var temp = WorksheetConfigHelper.GetInstance(exportConcord);
                if (temp != null && temp.TemplatePath != null)
                {
                    if (temp is ExportContractConcord)
                    {
                        exportConcord = (ExportContractConcord)temp;
                    }
                    templatePath = Path.Combine(TheApp.GetApplicationPath(), temp.TemplatePath);
                }
                #endregion

                exportConcord.CurrentZone = currentZone;
                exportConcord.PersonBusiness = personBusiness;
                exportConcord.AreaType = AreaType;
                if (curVp.IsStockFarmer)
                {
                    var stockLands = dbContext.CreateBelongRelationWorkStation().GetLandByPerson(curVp.ID, currentZone.FullCode);
                    if (stockLands.Count > 0)
                        lands.AddRange(stockLands);
                }
                lands.LandNumberFormat(SystemSet);
                exportConcord.ListLand = lands;
                exportConcord.DictList = dictList;
                exportConcord.VirtualPerson = curVp;
                exportConcord.OpenTemplate(templatePath);  //打开模板
                string filePath = SystemSet.DefaultPath + "\\" + curVp.Name;
                exportConcord.PrintPreview(concord, filePath);  //打开预览
            }
            catch (Exception ex)
            {
                this.ReportError(ex.Message);
                YuLinTu.Library.Log.Log.WriteException(this, "PreviewConcordData(预览合同)", ex.Message + ex.StackTrace);
            }
        }


        /// <summary>
        /// 导出合同
        /// </summary>
        public bool ExportConcordData(Zone currentZone, VirtualPerson curVp, List<ContractLand> lands, ContractConcord concord, string savePath, int areatype)
        {
            bool result = true;
            try
            {
                //List<VirtualPerson> persons = GetByZone(currentZone.FullCode);
                //if (persons.Count <= 0)
                //    return false;
                //List<ContractLand> lands = GetLandsByConcordId(concord);
                //VirtualPerson person = persons.Find(c => c.ID == concord.ContracterId);
                string templatePath = TemplateHelper.WordTemplate(TemplateFile.ContractConcordWord);
                List<Dictionary> dictCBFS = dictList.FindAll(c => c.GroupCode == DictionaryTypeInfo.CBJYQQDFS);
                var mode = dictCBFS.Find(c => c.Code == concord.ArableLandType);
                string typeString = "其他方式";
                if (mode != null)
                    typeString = mode.Name;
                string fullPath = savePath + @"\" + curVp.FamilyNumber + "-" + curVp.Name + "-" + TemplateFile.ContractConcordWord /*+ "(" + typeString + ")"*/;

                var exportConcord = new ExportContractConcord(dbContext);

                #region 通过反射等机制定制化具体的业务处理类
                var temp = WorksheetConfigHelper.GetInstance(exportConcord);
                if (temp != null && temp.TemplatePath != null)
                {
                    if (temp is ExportContractConcord)
                    {
                        exportConcord = (ExportContractConcord)temp;
                    }
                    templatePath = Path.Combine(TheApp.GetApplicationPath(), temp.TemplatePath);
                }
                #endregion

                exportConcord.CurrentZone = currentZone;
                exportConcord.PersonBusiness = personBusiness;
                exportConcord.AreaType = areatype;

                if (curVp.IsStockFarmer)
                {
                    var stockLands = dbContext.CreateBelongRelationWorkStation().GetLandByPerson(curVp.ID, currentZone.FullCode);
                    if (stockLands.Count > 0)
                        lands.AddRange(stockLands);
                }
                lands.LandNumberFormat(SystemSet);
                exportConcord.ListLand = lands;
                exportConcord.DictList = dictList;
                exportConcord.VirtualPerson = curVp;
                exportConcord.OpenTemplate(templatePath);  //打开模板
                exportConcord.SaveAs(concord, fullPath);
            }
            catch (Exception ex)
            {
                this.ReportError(ex.Message);
                YuLinTu.Library.Log.Log.WriteException(this, "ExportConcordData(导出合同)", ex.Message + ex.StackTrace);
                result = false;
            }
            return result;
        }

        #endregion

        #region 合同明细表

        /// <summary>
        /// 导出合同明细表
        /// </summary>
        public void ExportConcordInfoTable(Zone zone, string fileName, double averagePercent = 0.0, double currentPercent = 0.0)
        {
            try
            {
                if (zone == null)
                {
                    this.ReportError("未选择导出数据的地域!");
                    return;
                }
                string excelName = GetMarkDesc(zone);
                List<VirtualPerson> vps = GetByZone(zone.FullCode);
                List<ContractConcord> concords = GetCollection(zone.FullCode);
                List<ContractLand> lands = GetLandsByZoneCode(zone.FullCode);
                string tempPath = TemplateHelper.ExcelTemplate(TemplateFile.ContractSummaryExcel);
                string uinitName = GetUinitName(zone);   //单位名称
                using (ExportContractorSummaryExcel concordInfoExport = new ExportContractorSummaryExcel(dbContext))
                {
                    concordInfoExport.SaveFilePath = fileName + @"\" + excelName + "合同明细表" + ".xls";
                    concordInfoExport.CurrentZone = zone;
                    concordInfoExport.ListPerson = vps;
                    concordInfoExport.ListConcord = concords;
                    concordInfoExport.ListLand = lands;
                    concordInfoExport.UnitName = uinitName;
                    concordInfoExport.StatuDes = excelName + "合同明细表";
                    concordInfoExport.Percent = averagePercent;
                    concordInfoExport.CurrentPercent = currentPercent;
                    concordInfoExport.ZoneDesc = excelName;
                    concordInfoExport.ArgType = this.ArgType;
                    if (SummaryDefine.WarrantNumber)
                    {
                        SummaryDefine.WarrantNumber = false;//合同明细表不导出权证编码
                        SummaryDefine.ColumnCount = SummaryDefine.ColumnCount - 1;
                    }
                    concordInfoExport.PostErrorInfoEvent += export_PostErrorInfoEvent;
                    bool result = concordInfoExport.BeginExcel(tempPath);   //开始导表  
                }
                vps = null;
                concords = null;
                lands = null;
                GC.Collect();
            }
            catch (Exception ex)
            {
                this.ReportError(ex.Message);
                YuLinTu.Library.Log.Log.WriteException(this, "ExportConcordInfoTable(导出合同明细表到Excel)", ex.Message + ex.StackTrace);
            }
        }

        #endregion

        #region Messages

        /// <summary>
        /// 获取申请书编号
        /// </summary>
        private string GenerateNumber(CollectivityTissue tissue)
        {
            List<ContractRequireTable> tabs = GetTissueRequireTable(tissue.Code);
            int num = 1;
            string numString = string.Empty;
            if (tabs == null || tabs.Count == 0)
            { }
            else
            {
                do
                {
                    numString = num.ToString();
                    bool found = false;
                    for (int i = 0; i < tabs.Count; i++)
                        if (tabs[i].Number == numString)
                        {
                            found = true;
                            break;
                        }

                    if (!found)
                        break;

                    num++;
                } while (num < 1000);
            }
            string Number = numString;
            while (GetRequireTable(num.ToString()) != null)
            {
                num++;
            }
            Number = num.ToString();
            return Number;
        }

        /// <summary>
        /// 更新承包合同信息
        /// </summary>
        private void UpdateConcordInformation(Guid tableId, Zone zone)
        {
            List<ContractConcord> concords = GetCollection(zone.FullCode);
            string code = this.dictList.FindAll(c => c.GroupCode == DictionaryTypeInfo.CBJYQQDFS).Find(c => c.Name == "家庭承包").Code;
            //foreach (ContractConcord concord in concords)
            //{
            //    if (concord.ArableLandType != code)
            //    {
            //        continue;
            //    }
            //    if (concord.RequireBookId == tableId)
            //    {
            //        continue;
            //    }
            //    concord.RequireBookId = tableId;
            //    Update(concord);
            //}

            List<ContractConcord> updateConcords = concords.FindAll(c => c.ArableLandType != code && c.RequireBookId == tableId);
            foreach (var item in updateConcords)
            {
                item.RequireBookId = tableId;
                Update(item);
            }
        }

        /// <summary>
        /// 获取发包方
        /// </summary>
        public List<CollectivityTissue> GetTissuesByConcord(Zone zone)
        {
            List<CollectivityTissue> tissues = new List<CollectivityTissue>();
            List<ContractConcord> concords = GetCollection(zone.FullCode);
            foreach (ContractConcord concord in concords)
            {
                if (concord.ContracterName.IndexOf("机动地") >= 0 || concord.ContracterName.IndexOf("集体") >= 0)
                {
                    continue;
                }
                CollectivityTissue tissue = tissues.Find(tu => tu.ID == concord.SenderId);
                if (tissue != null)
                {
                    continue;
                }
                tissue = GetSenderById(concord.SenderId);
                if (tissue != null)
                {
                    tissues.Add(tissue);
                }
            }
            concords.Clear();
            return tissues;
        }
        /// <summary>
        /// 获取发包方
        /// </summary>
        public List<CollectivityTissue> GetTissuesByConcord(Zone zone, eLevelOption level)
        {
            List<CollectivityTissue> tissues = new List<CollectivityTissue>();
            List<ContractConcord> concords = concordStation.GetContractsByZoneCode(zone.FullCode, level, true);
            foreach (ContractConcord concord in concords)
            {
                if (concord.ContracterName.IndexOf("机动地") >= 0 || concord.ContracterName.IndexOf("集体") >= 0)
                {
                    continue;
                }
                CollectivityTissue tissue = tissues.Find(tu => tu.ID == concord.SenderId);
                if (tissue != null)
                {
                    continue;
                }
                tissue = GetSenderById(concord.SenderId);
                if (tissue != null)
                {
                    tissues.Add(tissue);
                }
            }
            concords.Clear();
            return tissues;
        }

        /// <summary>
        /// 通过合同ID获取地块
        /// </summary>
        public List<ContractLand> GetLandsByConcordId(ContractConcord concord)
        {
            List<ContractLand> contractLands = null;
            try
            {
                landBusiness = new AccountLandBusiness(dbContext);
                contractLands = landBusiness.GetByConcordId(concord.ID);
            }
            catch (Exception ex)
            {
                YuLinTu.Library.Log.Log.WriteException(this, "GetLandsByConcordId(获取地块集合)", ex.Message + ex.StackTrace);
                this.ReportError("获取地块集合失败," + ex.Message);
            }
            return contractLands;
        }

        /// <summary>
        /// 得到地块集合
        /// </summary>
        public List<ContractLand> GetLandCollenction(VirtualPerson vp)
        {
            List<ContractLand> contractLands = null;
            try
            {
                AccountLandBusiness landBusiness = new AccountLandBusiness(dbContext);
                contractLands = landBusiness.GetPersonCollection(vp.ID);
            }
            catch (Exception ex)
            {
                YuLinTu.Library.Log.Log.WriteException(this, "GetLandCollenction(获取地块集合)", ex.Message + ex.StackTrace);
                this.ReportError("获取地块集合失败," + ex.Message);
            }
            return contractLands;
        }

        /// <summary>
        /// 得到地块集合
        /// </summary>
        public List<ContractLand> GetLandsByZoneCode(string zoneCode)
        {
            List<ContractLand> contractLands = null;
            try
            {
                AccountLandBusiness landBusiness = new AccountLandBusiness(dbContext);
                contractLands = landBusiness.GetLandCollection(zoneCode);
            }
            catch (Exception ex)
            {
                YuLinTu.Library.Log.Log.WriteException(this, "GetLandsByZoneCode(获取地块集合)", ex.Message + ex.StackTrace);
                this.ReportError("获取地块集合失败," + ex.Message);
            }
            return contractLands;
        }

        /// <summary>
        /// 根据地域编码和地域等级获取地域名称
        /// </summary>
        /// <param name="zoneCode">地域编码</param>
        /// <param name="level">地域等级</param>
        /// <returns>地域名称</returns>
        private Zone GetZoneNameByLevel(Zone zone, eZoneLevel level)
        {
            if (zone == null)
                return null;
            if (zone.Level == level)
                return zone;
            else
            {
                Zone parent = GetZone(zone.Code);  //获取上级地域
                return GetZoneNameByLevel(parent, level);
            }
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
        /// 获取上级地域
        /// </summary>
        /// <param name="zone"></param>
        /// <returns></returns>
        public Zone GetZone(string zoneCode)
        {
            ModuleMsgArgs arg = new ModuleMsgArgs();
            arg.Datasource = dbContext;
            arg.Parameter = zoneCode;
            arg.Name = ZoneMessage.VIRTUALPERSON_ZONE;
            TheBns.Current.Message.Send(this, arg);
            return (arg.ReturnValue as Zone);
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
        /// 获取发包方
        /// </summary>
        /// <param name="id">合同ID</param>
        /// <returns></returns>
        public CollectivityTissue GetSenderById(Guid id)
        {
            if (id == null)
            {
                return null;
            }
            ModuleMsgArgs arg = new ModuleMsgArgs();
            arg.Datasource = this.dbContext;
            arg.Parameter = id;
            arg.Name = SenderMessage.SENDER_GET_ID;
            TheBns.Current.Message.Send(this, arg);
            CollectivityTissue tissue = arg.ReturnValue as CollectivityTissue;
            return tissue;
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

        #endregion

        #region 辅助方法

        /// <summary>
        /// 辅助判断方法
        /// </summary>
        public bool CanContinue()
        {
            if (concordStation == null)
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


        #endregion

        #endregion
    }
}