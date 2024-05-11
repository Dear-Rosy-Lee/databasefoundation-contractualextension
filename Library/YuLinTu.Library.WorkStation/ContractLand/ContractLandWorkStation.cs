/*
 * (C) 2015  鱼鳞图公司版权所有,保留所有权利 
 */
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using YuLinTu.Data;
using YuLinTu.Library.Entity;
using YuLinTu.Library.Repository;
using YuLinTu.Spatial;
using YuLinTu.Windows;

namespace YuLinTu.Library.WorkStation
{
    /// <summary>
    /// 承包台账地块类定义
    /// </summary>
    public class ContractLandWorkStation : Workstation<ContractLand>, IContractLandWorkStation
    {
        #region Properties

        /// <summary>
        /// 承包地数据访问层
        /// </summary>
        public new IContractLandRepository DefaultRepository
        {
            get { return base.DefaultRepository as IContractLandRepository; }
            set { base.DefaultRepository = value; }
        }
        /// <summary>
        /// 系统信息常规设置
        /// </summary>
        public SystemSetDefineWork SystemSet
        {
            get
            {
                var center = TheApp.Current.GetSystemSettingsProfileCenter();
                var profile = center.GetProfile<SystemSetDefineWork>();
                var section = profile.GetSection<SystemSetDefineWork>();
                var config = section.Settings as SystemSetDefineWork;
                return config;
            }
        }
        /// <summary>
        /// 行政地域数据访问层
        /// </summary>
        public IZoneRepository ZoneRepository { get; private set; }

        /// <summary>
        /// 行政地域业务逻辑层
        /// </summary>
        public IZoneWorkStation ZoneWorkStation { get; private set; }

        /// <summary>
        /// 发包方数据访问层
        /// </summary>
        public ICollectivityTissueRepository TissueRepository { get; private set; }

        /// <summary>
        /// 承包方数据访问层
        /// </summary>
        public IVirtualPersonRepository<LandVirtualPerson> VirtualPersonRepository { get; private set; }

        /// <summary>
        /// 承包方业务逻辑层
        /// </summary>
        public IVirtualPersonWorkStation<LandVirtualPerson> VirtualPersonWorkStation { get; private set; }

        /// <summary>
        /// 承包合同数据访问层
        /// </summary>
        public IContractConcordRepository ConcordRepository { get; private set; }

        /// <summary>
        /// 承包合同业务逻辑层
        /// </summary>
        public IConcordWorkStation ConcordWorkStation { get; private set; }

        /// <summary>
        /// 承包权证业务逻辑层
        /// </summary>
        public IContractRegeditBookRepository BookRepository { get; private set; }

        /// <summary>
        /// 承包权证业务逻辑层
        /// </summary>
        public IContractRegeditBookWorkStation BookWorkStation { get; private set; }

        /// <summary>
        /// 界址线数据访问
        /// </summary>
        public IBuildLandBoundaryAddressCoilRepository CoilRepository { get; private set; }

        /// <summary>
        /// 界址线业务访问
        /// </summary>
        public IBuildLandBoundaryAddressCoilWorkStation CoilWorkStation { get; private set; }

        /// <summary>
        /// 界址点数据访问
        /// </summary>
        public IBuildLandBoundaryAddressDotRepository DotRepository { get; private set; }

        /// <summary>
        /// 界址点业务访问
        /// </summary>
        public IBuildLandBoundaryAddressDotWorkStation DotWorkStation { get; private set; }

        public IBelongRelationRespository BelongRelationRespository { get; set; }

        #endregion

        #region Ctor
        //public ContractLandWorkStation(IContractLandRepository rep) {
        //    DefaultRepository = rep;
        //}
        /// <summary>
        /// 构造函数
        /// </summary>
        public ContractLandWorkStation(IContractLandRepository rep, IZoneRepository repZone = null, IZoneWorkStation workZone = null,
            ICollectivityTissueRepository repSender = null, IVirtualPersonRepository<LandVirtualPerson> repVirtualPerson = null,
            IVirtualPersonWorkStation<LandVirtualPerson> workVirtualPerson = null, IContractConcordRepository repConcord = null,
           IConcordWorkStation workConcord = null, IContractRegeditBookRepository repBook = null, IContractRegeditBookWorkStation workBook = null,
            IBuildLandBoundaryAddressDotRepository repDot = null, IBuildLandBoundaryAddressDotWorkStation workDot = null,
           IBuildLandBoundaryAddressCoilRepository repCoil = null, IBuildLandBoundaryAddressCoilWorkStation workCoil = null, IBelongRelationRespository belongRep = null)
        {
            DefaultRepository = rep;
            ZoneRepository = repZone;
            ZoneWorkStation = workZone;
            TissueRepository = repSender;
            VirtualPersonRepository = repVirtualPerson;
            VirtualPersonWorkStation = workVirtualPerson;
            ConcordRepository = repConcord;
            ConcordWorkStation = workConcord;
            BookRepository = repBook;
            BookWorkStation = workBook;
            DotRepository = repDot;
            DotWorkStation = workDot;
            CoilRepository = repCoil;
            CoilWorkStation = workCoil;
            BelongRelationRespository = belongRep;
        }

        #endregion

        #region Methods

        /// <summary>
        /// 删除选定承包方所有数据
        /// </summary>
        public void DeleteSelectVirtualPersonAllData(Guid ID)
        {
            DefaultRepository.DeleteSelectVirtualPersonAllData(ID);
            TrySaveChanges(DefaultRepository);
        }

        /// <summary>
        /// 根据地块标识集合删除和更新相关业务数据
        /// </summary>
        /// <param name="ids">地块标识集合</param>
        public int DeleteRelationDataByLand(Guid[] ids)
        {
            DefaultRepository.DeleteRelationDataByLand(ids);
            return TrySaveChanges(DefaultRepository);
        }

        /// <summary>
        /// 根据标识码删除承包台账地块对象
        /// </summary>
        /// <param name="guid">标识码</param>
        /// <returns>-1（参数错误）/0（失败）/1（成功）</returns>
        public int Delete(Guid guid)
        {
            DefaultRepository.Delete(guid);
            return TrySaveChanges(DefaultRepository);
        }

        /// <summary>
        /// 更新承包台账地块对象
        /// </summary>
        /// <returns>-1（参数错误）/0（失败）/1（成功）</returns>
        public int Update(ContractLand entity)
        {
            DefaultRepository.Update(entity);
            return TrySaveChanges(DefaultRepository);
        }

        /// <summary>
        /// 跟新地块Shape数据
        /// </summary>
        /// <param name="entity">地块实体对象</param>
        /// <returns>-1（参数错误）/0（失败）/1（成功）</returns>
        public int UpdateShape(ContractLand entity)
        {
            DefaultRepository.UpdateShape(entity);
            return TrySaveChanges(DefaultRepository);
        }

        /// <summary>
        /// 批量更新承包地块
        /// </summary>
        /// <param name="entities">合同集合</param>
        /// <returns>0：更新个数为零;-1：更新失败;大于0:成功</returns>
        public int UpdateRange(List<ContractLand> entities)
        {
            int updateCount = 0;
            if (entities == null || entities.Count == 0)
            {
                return updateCount;
            }
            try
            {
                foreach (var entity in entities)
                {
                    DefaultRepository.Update(entity);
                }
                updateCount = TrySaveChanges(DefaultRepository);
            }
            catch (Exception ex)
            {
                updateCount = -1;
                YuLinTu.Library.Log.Log.WriteError(this, "UpdateRange(提交批量更新合同数据)", ex.Message + ex.StackTrace);
            }
            return updateCount;
        }

        /// <summary>
        ///  根据承包方id更新承包方名称
        /// </summary>
        /// <param name="ownerId">承包方id</param>
        /// <param name="ownerName">承包方名称</param>
        /// <returns>-1（参数错误）/0（失败）/1（成功）</returns>
        public int Update(Guid ownerId, string ownerName)
        {
            DefaultRepository.Update(ownerId, ownerName);
            return TrySaveChanges(DefaultRepository);
        }

        /// <summary>
        /// 获取对象
        /// </summary>
        public ContractLand Get(Guid guid)
        {
            return DefaultRepository.Get(guid);
        }

        /// <summary>
        /// 根据标识码判断承包台账地块对象是否存在
        /// </summary>
        public bool Exists(Guid guid)
        {
            return DefaultRepository.Exists(guid);
        }

        /// <summary>
        /// 批量添加承包地块数据
        /// </summary>
        /// <param name="listLand">承包地块对象集合</param>
        /// <returns>-1（参数错误）/int 添加对象数量</returns>
        public int AddRange(List<ContractLand> listLand)
        {
            int addCount = 0;
            if (listLand == null || listLand.Count == 0)
            {
                return addCount;
            }
            foreach (var land in listLand)
            {
                DefaultRepository.Add(land);
            }
            addCount = TrySaveChanges(DefaultRepository);
            return addCount;
        }


        /// <summary>
        /// 批量删除确股地块同时删除联动关系
        /// </summary>
        /// <returns></returns>
        public int DeleteStockLandList(List<ContractLand> listLand)
        {
            listLand.ForEach(o =>
            {
                Delete(o.ID);
                BelongRelationRespository.Delete(r => r.LandID == o.ID);
            });
            return TrySaveChanges(DefaultRepository);
        }

        #endregion

        #region ExtendMethod

        /// <summary>
        /// 将数据库中承包地承包方式设置为家庭承包
        /// </summary>
        public void SetDataBaseCBDFamilyCBFS()
        {
            DefaultRepository.SetDataBaseCBDFamilyCBFS();
        }

        /// <summary>
        /// 根据地籍号精确判断农村土地承包地对象是否存在
        /// </summary>
        /// <param name="cadastralNumber">地籍号</param>
        /// <returns>true（存在）/false（不存在）</returns>
        public bool ExistByCadastralNumberP(string cadastralNumber)
        {
            return DefaultRepository.ExistByCadastralNumberP(cadastralNumber);
        }

        /// <summary>
        /// 根据承包方id获取承包台账地块信息
        /// </summary>
        /// <param name="ownerId">承包方id</param>
        /// <returns>承包台账地块集合</returns>
        public List<ContractLand> GetCollection(Guid ownerId)
        {
            return DefaultRepository.GetCollection(ownerId);
        }

        /// <summary>
        /// 根据土地是否流转获取承包台账地块信息
        /// </summary>
        /// <param name="isTransfer">土地是否流转</param>
        /// <returns>承包台账地块</returns>
        public List<ContractLand> GetByIsTransfer(bool isTransfer)
        {
            return DefaultRepository.GetByIsTransfer(isTransfer);
        }

        /// <summary>
        /// 根据土地是否流转获取指定区域下的承包台账地块集合
        /// </summary>
        /// <param name="zoneCode">区域代码</param>
        /// <param name="isTransfer">土地是否流转</param>
        /// <returns>承包台账地块集合；指定区域为null，返回所有区域符合土地是否流转参数的土地对象</returns>
        public List<ContractLand> GetByIsTransfer(string zoneCode, bool isTransfer)
        {
            return DefaultRepository.GetByIsTransfer(zoneCode, isTransfer);
        }

        /// <summary>
        /// 根据土地是否流转统计指定区域下的承包台账地块数量
        /// </summary>
        /// <param name="zoneCode">区域代码</param>
        /// <param name="isTransfer">土地是否流转</param>
        /// <returns>-1（参数错误）/int 对象数量</returns>
        public int CountByIsTransfer(string zoneCode, bool isTransfer)
        {
            return DefaultRepository.CountByIsTransfer(zoneCode, isTransfer);
        }

        /// <summary>
        /// 根据流转类型获取承包台账地块集合
        /// </summary>
        /// <param name="transferType">流转类型</param>
        /// <returns>承包台账地块集合</returns>
        public List<ContractLand> GetByTransferType(string transferType)
        {
            return DefaultRepository.GetByTransferType(transferType);
        }

        /// <summary>
        /// 根据流转类型获取指定区域下的承包台账地块集合
        /// </summary>
        /// <param name="zoneCode">区域代码</param>
        /// <param name="transferType">流转类型</param>
        /// <returns>承包台账地块信集合；指定区域为null，返回所有区域符合流转类型参数的土地对象</returns>
        public List<ContractLand> GetByTransferType(string zoneCode, string transferType)
        {
            return DefaultRepository.GetByTransferType(zoneCode, transferType);
        }

        /// <summary>
        /// 根据承包方id获取地块
        /// </summary>
        /// <param name="ownerId">承包方id</param>
        /// <param name="constructType">承包经营权类型</param>
        /// <returns>地块</returns>
        public List<ContractLand> GetCollection(Guid ownerId, string constructType)
        {
            return DefaultRepository.GetCollection(ownerId, constructType);
        }

        /// <summary>
        /// 根据承包方式获取指定承包方id下的地块集合
        /// </summary>
        /// <param name="ownerId">承包方id</param>
        /// <param name="constructMode">承包方式</param>
        /// <returns>地块集合</returns>
        public List<ContractLand> GetCollectionByConstructMode(Guid ownerId, string constructMode)
        {
            return DefaultRepository.GetCollectionByConstructMode(ownerId, constructMode);
        }

        /// <summary>
        /// 获取地域下承包台账地块集合
        /// </summary>
        /// <param name="zoneCode">区域代码</param>
        /// <returns>承包台账地块集合</returns>
        public List<ContractLand> GetCollection(string zoneCode)
        {
            return DefaultRepository.GetCollection(zoneCode);
        }

        /// <summary>
        /// 根据地籍号获取对象
        /// </summary>
        /// <param name="cadastralNumber">地籍号</param>
        /// <returns>承包台账地块对象</returns>
        public ContractLand Get(string cadastralNumber)
        {
            return DefaultRepository.Get(cadastralNumber);
        }


        public ContractLand GetByLandNumber(string landNumber)
        {
            return DefaultRepository.GetByLandNumber(landNumber);
        }

        /// <summary>
        /// 根据地籍号模糊判断对象是否存在
        /// </summary>
        /// <param name="cadastralNumber">地籍号</param>
        /// <returns>true（存在）/false（不存在）</returns>
        public bool ExistByCadastralNumberF(string cadastralNumber)
        {
            return DefaultRepository.ExistByCadastralNumberF(cadastralNumber);
        }

        /// <summary>
        /// 根据地籍号获取承包台账地块对象
        /// </summary>
        /// <param name="cadastralNumber">地籍号</param>
        /// <returns>承包台账地块对象</returns>
        public ContractLand GetByCadastralNumber(string cadastralNumber)
        {
            return DefaultRepository.GetByCadastralNumber(cadastralNumber);
        }

        /// <summary>
        /// 获取指定区域下指定承包方姓名的承包台账地块对象集合
        /// </summary>
        /// <param name="ownerName">承包方名称</param>
        /// <param name="zoneCode">区域代码</param>
        /// <returns>承包台账地块集合</returns>
        public List<ContractLand> GetByOwnerName(string zoneCode, string ownerName)
        {
            return DefaultRepository.GetByOwnerName(zoneCode, ownerName);
        }

        /// <summary>
        /// 根据合同Id获取地块信息
        /// </summary>
        /// <param name="concordId">合同Id</param>
        /// <returns>承包台账地块集合</returns>
        public List<ContractLand> GetByConcordId(Guid concordId)
        {
            return DefaultRepository.GetByConcordId(concordId);
        }

        /// <summary>
        /// 根据合同Id集合获取地块信息集合
        /// </summary>
        public List<ContractLand> GetByConcordIds(Guid[] ids)
        {
            return DefaultRepository.GetByConcordIds(ids);
        }

        /// <summary>
        /// 删除当前地域下所有地块
        /// </summary>
        /// <param name="zoneCode">地域代码</param>
        /// <returns>-1（参数错误）/0（失败）/1（成功）</returns>
        public int DeleteByZoneCode(string zoneCode)
        {
            DefaultRepository.DeleteByZoneCode(zoneCode);
            return TrySaveChanges(DefaultRepository);
        }

        /// <summary>
        /// 根据承包方ID删除下属地块信息
        /// </summary>
        /// <param name="guid">承包方标识码</param>
        /// <returns>-1（参数错误）/0（失败）/1（成功）</returns>
        public int DeleteLandByPersonID(Guid guid)
        {
            DefaultRepository.DeleteLandByPersonID(guid);
            return TrySaveChanges(DefaultRepository);
        }

        /// <summary>
        /// 删除当前地域下指定承包经营权类型的所有承包台账地块
        /// </summary>
        /// <param name="zoneCode">地域代码</param>
        /// <param name="constructType">承包经营权类型</param>
        /// <returns>-1（参数错误）/0（失败）/1（成功）</returns>
        public int DeleteByZoneCode(string zoneCode, string constructType)
        {
            DefaultRepository.DeleteByZoneCode(zoneCode, constructType);
            return TrySaveChanges(DefaultRepository);
        }

        /// <summary>
        /// 删除当前地域下指定的承包方状态的所有地块
        /// </summary>
        /// <param name="zoneCode">地域代码</param>
        /// <param name="virtualStatus">承包方状态</param>
        /// <param name="levelOption">地域匹配等级</param>
        /// <returns>-1（参数错误）/0（失败）/1（成功）</returns>
        public int DeleteByZoneCode(string zoneCode, eVirtualPersonStatus virtualStatus, eLevelOption levelOption)
        {
            DefaultRepository.DeleteByZoneCode(zoneCode, virtualStatus, levelOption);
            return TrySaveChanges(DefaultRepository);
        }

        /// <summary>
        /// 删除当前地域下指定的承包方状态的所有地块
        /// </summary>
        /// <param name="zoneCode">地域代码</param>
        /// <param name="virtualStatus">承包方状态</param>
        /// <param name="levelOption">地域匹配等级</param>
        /// <returns>-1（参数错误）/0（失败）/1（成功）</returns>
        public int DeleteOtherByZoneCode(string zoneCode, eLevelOption levelOption)
        {
            DefaultRepository.DeleteOtherByZoneCode(zoneCode, levelOption);
            return TrySaveChanges(DefaultRepository);
        }

        /// <summary>
        /// 根据目标地块，返回相交地块集合，不包括传入的地块
        /// </summary>
        /// <param name="tagetLand"></param>
        /// <returns></returns>
        public List<ContractLand> GetIntersectLands(ContractLand tagetLand, Geometry tagetLandShape)
        {
            return DefaultRepository.GetIntersectLands(tagetLand, tagetLandShape);
        }

        /// <summary>
        /// 获取合同id下的承包台账地块面积
        /// </summary>
        /// <param name="concordId">合同id</param>
        /// <returns>地块面积</returns>
        public double GetLandAreaByConcordID(Guid concordId)
        {
            return DefaultRepository.GetLandAreaByConcordID(concordId);
        }

        /// <summary>
        /// 根据区域代码获得所有该区域下的并且以地籍号排序的承包台账土地对象集合【CadastralNumber.Contains(zoneCode)】
        /// </summary>
        /// <param name="zoneCode">区域代码</param>
        /// <returns>以地籍号排序的承包台账地块</returns>
        public List<ContractLand> GetLandsByZoneCodeInCadastralNumber(string zoneCode)
        {
            return DefaultRepository.GetLandsByZoneCodeInCadastralNumber(zoneCode);
        }

        /// <summary>
        /// 按地域统计
        /// </summary>
        /// <param name="zoneCode">区域代码</param>
        /// <param name="searchOption">匹配等级</param>
        /// <returns>地块数量</returns>
        public int Count(string zoneCode, eLevelOption searchOption)
        {
            return DefaultRepository.Count(zoneCode, searchOption);
        }

        public int CountByConcordId(Guid concordId)
        {
            return DefaultRepository.CountByConcordId(concordId);
        }

        /// <summary>
        /// 按地域统计面积
        /// </summary>
        /// <param name="zoneCode">地域代码</param>
        /// <param name="searchOption">匹配等级</param>
        /// <returns>面积</returns>
        public double CountArea(string zoneCode, eLevelOption searchOption)
        {
            return DefaultRepository.CountArea(zoneCode, searchOption);
        }

        /// <summary>
        /// 统计地域下实测面积
        /// </summary>
        /// <param name="zoneCode">地域代码</param>
        /// <param name="searchOption">匹配等级</param>
        /// <returns>实测面积</returns>
        public double CountActualArea(string zoneCode, eLevelOption searchOption)
        {
            return DefaultRepository.CountActualArea(zoneCode, searchOption);
        }

        /// <summary>
        /// 统计区域下没有合同的地块
        /// </summary>
        /// <param name="zoneCode">地域代码</param>
        /// <param name="searchOption">匹配等级</param>
        /// <returns>地块数量</returns>
        public int CountNoConcord(string zoneCode, eLevelOption searchOption)
        {
            return DefaultRepository.CountNoConcord(zoneCode, searchOption);
        }

        /// <summary>
        /// 按地域删除地块
        /// </summary>
        /// <param name="zoneCode">地域代码</param>
        /// <param name="searchOption">匹配等级</param>
        /// <returns>-1（参数错误）/0（失败）/1（成功）</returns>
        public int DeleteByZoneCode(string zoneCode, eLevelOption searchOption)
        {
            DefaultRepository.DeleteByZoneCode(zoneCode, searchOption);
            return TrySaveChanges(DefaultRepository);
        }

        /// <summary>
        /// 按地域获取承包台账地块集合
        /// </summary>
        /// <param name="zoneCode">地域代码</param>
        /// <param na承包台账me="searchOption">匹配等级</param>
        /// <returns>承包台账地块集合</returns>
        public List<ContractLand> GetCollection(string zoneCode, eLevelOption searchOption)
        {
            return DefaultRepository.GetCollection(zoneCode, searchOption);
        }

        /// <summary>
        /// 获取指定地域下所有空间地块集合
        /// </summary>
        /// <param name="zoneCode">指定地域</param>
        /// <param name="levelOption">指定查找地域级别</param>
        /// <returns>空间地块集合</returns>
        public List<ContractLand> GetShapeCollection(string zoneCode, eLevelOption levelOption)
        {
            return DefaultRepository.GetShapeCollection(zoneCode, levelOption);
        }

        /// <summary>
        /// 按地域匹配等级获取指定承包方状态的地块集合
        /// </summary>
        /// <param name="zoneCode">地域代码</param>
        /// <param name="virtualStatus">承包方状态</param>
        /// <param name="levelOption">匹配等级</param>
        /// <returns>地块集合</returns>
        public List<ContractLand> GetCollection(string zoneCode, eVirtualPersonStatus virtualStatus, eLevelOption levelOption)
        {
            var notStockLandlist = DefaultRepository.GetCollection(zoneCode, virtualStatus, levelOption);

            //此逻辑通过权属关系获取指定承包方状态的确股地
            //屏蔽通过承包方ID找不到确股地的影响，2017.8.16 liuyichuan
            var allRelation = BelongRelationRespository.Get(s => s.ZoneCode == zoneCode);
            if (allRelation != null && allRelation.Count > 0)
            {
                List<Guid> vpIds = new List<Guid>();
                allRelation.ForEach(s =>
                {
                    if (!vpIds.Contains(s.VirtualPersonID))
                    {
                        vpIds.Add(s.VirtualPersonID);
                    }
                });
                if (vpIds.Count == 0)
                {
                    return notStockLandlist;
                }
                var vpList = new List<VirtualPerson>();
                var otherLandList = new List<ContractLand>();
                vpIds.ForEach(vpid =>
                {
                    var getvp = VirtualPersonWorkStation.Get(vpid);
                    if (getvp == null) return;
                    vpList.Add(getvp);
                }
                );
                vpList.RemoveAll(vp => vp.Status != virtualStatus);

                if (vpList.Count == 0)
                {
                    return notStockLandlist;
                }
                vpIds.RemoveAll(s => vpList.Find(vp => vp.ID == s) == null);

                var otherLandIds = allRelation.Where(re => vpIds.Contains(re.VirtualPersonID))?.Select(s => s.LandID)?.Distinct().ToList();
                otherLandIds?.ForEach(landId => otherLandList.Add(DefaultRepository.Get(landId)));
                notStockLandlist.AddRange(otherLandList);
            }

            return notStockLandlist;
        }

        /// <summary>
        /// 根据地域及地域级别查询指定承包方状态下没有界址信息地块
        /// </summary>
        /// <param name="zoneCode">地域信息</param>
        /// <param name="option">地域级别</param>
        /// <returns>查询结果</returns>
        public List<ContractLand> GetLandsWithoutSiteInfoByZone(string zoneCode, eVirtualPersonStatus virtualStatus, eLevelOption option = eLevelOption.Self)
        {
            return DefaultRepository.GetLandsWithoutSiteInfoByZone(zoneCode, virtualStatus, option);
        }

        /// <summary>
        /// 根据承包方标识集合获取地块集合
        /// </summary>
        /// <param name="obligeeIds">承包方标识集合</param>
        /// <returns>查询地块集合</returns>
        public List<ContractLand> GetLandsByObligeeIds(Guid[] obligeeIds)
        {
            return DefaultRepository.GetLandsByObligeeIds(obligeeIds);
        }

        /// <summary>
        /// 按承包方ID统计地块
        /// </summary>
        /// <param name="ownerId">承包方ID</param>
        /// <returns>地块数量</returns>
        public int Count(Guid ownerId)
        {
            return DefaultRepository.Count(ownerId);
        }

        /// <summary>
        /// 根据地籍号获取以承包方名称排序的承包台账地块集合
        /// </summary>
        /// <param name="cadastralNumber">地籍号</param>
        /// <param name="searchType">查找类型</param>
        /// <returns>以承包方名称排序的承包台账地块集合</returns>
        public List<ContractLand> SearchByCadastralNumber(string cadastralNumber, eSearchOption searchType)
        {
            return DefaultRepository.SearchByCadastralNumber(cadastralNumber, searchType);
        }

        /// <summary>
        /// 根据承包方名称获取以承包方名称排序的承包台账地块集合
        /// </summary>
        /// <param name="ownerName">承包方名称</param>
        /// <param name="searchType">查找类型</param>
        /// <returns>以承包方名称排序的承包台账地块集合</returns>
        public List<ContractLand> SearchByVirtualPersonName(string ownerName, eSearchOption searchType)
        {
            return DefaultRepository.SearchByVirtualPersonName(ownerName, searchType);
        }

        /// <summary>
        /// 根据地籍号获取指定区域下以承包方名称排序的承包台账地块集合
        /// </summary>
        /// <param name="cadastralNumber">地籍号</param>
        /// <param name="searchType">查找类型</param>
        /// <param name="zoneCode">区域代码</param>
        /// <returns>以承包方名称排序的承包台账地块集合;区域代码为空时，返回所有满足指定地籍号的并以承包方名称排序的地块集合</returns>
        public List<ContractLand> SearchByCadastralNumber(string cadastralNumber, eSearchOption searchType, string zoneCode)
        {
            return DefaultRepository.SearchByCadastralNumber(cadastralNumber, searchType, zoneCode);
        }

        /// <summary>
        /// 根据承包方名称获取指定区域下以承包方名称排序的承包台账地块集合
        /// </summary>
        /// <param name="ownerName">承包方名称</param>
        /// <param name="searchType">查找类型</param>
        /// <param name="zoneCode">区域代码</param>
        /// <returns>以承包方名称排序的承包台账地块集合;区域代码为空时，返回所有满足指定承包方名称的并以承包方名称排序的地块集合</returns>
        public List<ContractLand> SearchByVirtualPersonName(string ownerName, eSearchOption searchType, string zoneCode)
        {
            return DefaultRepository.SearchByVirtualPersonName(ownerName, searchType, zoneCode);
        }

        /// <summary>
        /// 按承包经营权类型搜索指定区域的承包台账地块集合
        /// </summary>
        /// <param name="zoneCode">区域代码</param>
        /// <param name="constructType">承包经营权类型</param>
        /// <returns>承包台账地块集合；如果区域代码为null，返回空的承包台账地块对象集合</returns>
        public List<ContractLand> SearchByLandType(string zoneCode, string constructType)
        {
            return DefaultRepository.SearchByLandType(zoneCode, constructType);
        }

        /// <summary>
        /// 按承包经营权类型搜索指定承包地地籍区编号的承包台账地块集合
        /// </summary>
        /// <param name="cadastralZoneCode">地籍区编号</param>
        /// <param name="constructType">承包经营权类型</param>
        /// <returns>承包台账地块集合；如果区域代码为null，返回空的承包地集合对象</returns>
        public List<ContractLand> SearchByLandTypeAndCadaZone(string cadastralZoneCode, string constructType)
        {
            return DefaultRepository.SearchByLandTypeAndCadaZone(cadastralZoneCode, constructType);
        }
        public List<ContractLand> GetLandsBYGraph(Geometry graph)
        {
            return DefaultRepository.GetLandsBYGraph(graph);
        }

        /// <summary>
        /// 按区域搜索指定地类名称的承包台账地块信息
        /// </summary>
        /// <param name="landName">地类名称</param>
        /// <returns>承包台账地块对象</returns>
        public ContractLand SearchByLandNumberAndZoneCode(string landName, string zoneCode)
        {
            return DefaultRepository.SearchByLandNameAndZoneCode(landName, zoneCode);
        }

        /// <summary>
        /// 更新合同
        /// </summary>
        public void SignConcord(ContractLand land, List<string> fList, List<string> ofList)
        {
            //先解除合同
            var person = VirtualPersonWorkStation.Get(c => c.ID == (Guid)land.OwnerId).FirstOrDefault();
            var listConcords = ConcordWorkStation.GetContractsByFamilyID((Guid)land.OwnerId);
            if (listConcords == null)
                listConcords = new List<ContractConcord>();

            // ConcordRepository.Delete(c => c.ContracterId != null && c.ContracterId == land.OwnerId);

            //再签订合同
            var listLand = GetCollection((Guid)land.OwnerId);
            string familyMode = ((int)eConstructMode.Family).ToString();
            ContractConcord concordFamily = listConcords.Find(c => !string.IsNullOrEmpty(c.ArableLandType) && c.ArableLandType == familyMode);
            ContractConcord concordOther = listConcords.Find(c => !string.IsNullOrEmpty(c.ArableLandType) && c.ArableLandType != familyMode);
            InitiallizeArea(concordFamily);
            InitiallizeArea(concordOther);
            //List<string> fList = ConcordExtend.DeserializeContractInfo(true);
            //List<string> ofList = ConcordExtend.DeserializeContractInfo(false);
            if (listLand != null && listLand.Count > 0)
            {
                bool isFamilyUpdate = false;
                bool isOtherUpdate = false;
                //承包方下有地块信息才考虑签订合同
                foreach (var ld in listLand)
                {
                    bool isExsit = fList.Any(c => c.Equals(ld.LandCategory));
                    bool oisExsit = ofList.Any(c => c.Equals(ld.LandCategory));
                    bool isFamilyMode = ld.ConstructMode == familyMode ? true : false;
                    if (concordFamily != null && isExsit && isFamilyMode)
                    {
                        //家庭承包方式合同
                        concordFamily.CountActualArea += ld.ActualArea;
                        concordFamily.CountAwareArea += ld.AwareArea;
                        concordFamily.TotalTableArea += ld.TableArea;
                        ld.ConcordId = concordFamily.ID;
                        isFamilyUpdate = true;
                    }
                    if (concordOther != null && oisExsit && !isFamilyMode)
                    {
                        //其他承包方式合同
                        concordOther.CountActualArea += ld.ActualArea;
                        concordOther.CountAwareArea += ld.AwareArea;
                        concordOther.TotalTableArea += ld.TableArea;
                        ld.ConcordId = concordOther.ID;
                        isOtherUpdate = true;
                    }
                    UpdateContractLand(ld, DefaultRepository);
                }
                if (isFamilyUpdate)
                {
                    concordFamily.ContracterId = person.ID;
                    concordFamily.ContracterName = person.Name;
                    UpdateConcord(concordFamily);
                }
                if (isOtherUpdate)
                {
                    concordOther.ContracterId = person.ID;
                    concordOther.ContracterName = person.Name;
                    UpdateConcord(concordOther);
                }
            }

            TrySaveChanges(DefaultRepository);
        }


        /// <summary>
        /// 根据删除的地块，更新合同和权证
        /// </summary>
        /// <param name="landIDs">删除的地块集合ID</param>
        /// <param name="landHTIDs">删除的地块集合有关的合同集合</param>
        public void UpdateConcordByDelLand(List<Guid> landIDs, List<ContractConcord> landHTs)
        {
            if (landIDs.Count == 0) return;
            DefaultRepository.DeleteCoilDotByLandIDs(landIDs);

            foreach (var item in landIDs)
            {
                var landitem = Get(item);
                if (landitem == null || landitem.ConcordId == null || landitem.ConcordId.HasValue == false) continue;
                var landht = landHTs.Find(cc => cc.ID == landitem.ConcordId);
                if (landht == null) continue;
                landht.CountActualArea -= landitem.ActualArea;
                landht.CountAwareArea -= landitem.AwareArea;
                landht.TotalTableArea -= landitem.TableArea;
                UpdateConcord(landht);
            }
            TrySaveChanges(DefaultRepository);
        }

        /// <summary>
        /// 根据承包方合户更新地块及合同、权证
        /// </summary>
        /// <param name="a">目标人</param>
        /// <param name="b">追加人</param>
        public void UpdateConcordAndBookByCombineContractor(VirtualPerson a, VirtualPerson b)
        {
            var targetVp = a;//人
            var addVp = b;
            var targetVpID = targetVp.ID;//目标人ID
            var addVpID = addVp.ID;//追加人ID
            List<ContractConcord> targetVpConcords = ConcordWorkStation.GetContractsByFamilyID(targetVpID);
            List<ContractConcord> addVpConcords = ConcordWorkStation.GetContractsByFamilyID(addVpID);
            if (addVpConcords.Count == 0)
                return;

            List<ContractLand> targetVpLands = DefaultRepository.GetCollection(targetVpID); //地块、合同和权证  
            List<ContractLand> addVpLands = DefaultRepository.GetCollection(addVpID);
            List<ContractRegeditBook> targetVpCTBs = new List<ContractRegeditBook>();
            foreach (var targetVPCitem in targetVpConcords)
            {
                var targetVpCTB = BookWorkStation.Get(targetVPCitem.ID);
                targetVpCTBs.Add(targetVpCTB);
            }
            List<ContractRegeditBook> addVpCTBs = new List<ContractRegeditBook>();
            foreach (var addVpCitem in addVpConcords)
            {
                var addVpCTB = BookWorkStation.Get(addVpCitem.ID);
                addVpCTBs.Add(addVpCTB);
            }
            //更新合同
            //如果目标合同为0
            if (targetVpConcords.Count == 0)
            {
                foreach (var addVpCitem in addVpConcords)
                {
                    addVpCitem.ContracerType = ((int)targetVp.FamilyExpand.ContractorType).ToString();
                    addVpCitem.ContracterId = targetVpID;
                    addVpCitem.ContracterName = targetVp.Name;
                    addVpCitem.ContracterIdentifyNumber = targetVp.Number;
                    ConcordWorkStation.Update(addVpCitem);
                    var lands = targetVpLands.FindAll(tvl => tvl.ConcordId == addVpCitem.ID);
                    if (lands != null && lands.Count != 0)
                    {
                        foreach (var landitem in lands)
                        {
                            landitem.ConcordId = addVpCitem.ID;
                        }
                        UpdateRange(lands);
                    }
                }
            }
            else
            {
                foreach (var targetVpCitem in targetVpConcords)
                {
                    var addVpConcord = addVpConcords.Find(avc => avc.ArableLandType == targetVpCitem.ArableLandType);
                    if (addVpConcord == null) continue;
                    targetVpCitem.CountActualArea += addVpConcord.CountActualArea;
                    targetVpCitem.CountAwareArea += addVpConcord.CountAwareArea;   //界面显示的合同面积
                    targetVpCitem.TotalTableArea += addVpConcord.TotalTableArea;
                    targetVpCitem.CountMotorizeLandArea += addVpConcord.CountMotorizeLandArea;

                    var updateaddVpLands = addVpLands.FindAll(tvl => tvl.ConcordId == addVpConcord.ID);
                    if (updateaddVpLands != null && updateaddVpLands.Count != 0)
                    {
                        foreach (var landitem in updateaddVpLands)
                        {
                            landitem.ConcordId = targetVpCitem.ID;

                        }
                        UpdateRange(updateaddVpLands);
                    }
                    ConcordWorkStation.Update(targetVpCitem);

                    var targetbook = targetVpCTBs.Find(t => t.ID == targetVpCitem.ID);
                    var addbook = addVpCTBs.Find(t => t.ID == addVpConcord.ID);
                    if (targetbook == null && addbook != null)
                    {
                        addbook.ID = targetVpCitem.ID;
                    }
                    if (targetbook != null && addbook != null)
                    {
                        BookWorkStation.Delete(addbook.ID);
                    }
                    BookWorkStation.Delete(addVpConcord.ID);
                }
            }
        }

        /// <summary>
        /// 初始化承包方后更新所有相关的数据(合同、权证)
        /// 初始化承包方后承包方的户号发生变化，导致合同、权证的编号发生变化
        /// </summary>
        /// <param name="virtualPerson">更新承包方对象</param>
        public int UpdateDataForInitialVirtualPerson(VirtualPerson virtualPerson)
        {
            if (virtualPerson == null)
            {
                return 0;
                throw new YltException("获取承包方数据失败!");
            }
            var listConcords = ConcordWorkStation.GetContractsByFamilyID(virtualPerson.ID);
            if (listConcords == null)
                listConcords = new List<ContractConcord>();
            string familyMode = ((int)eConstructMode.Family).ToString();
            ContractConcord concordFamily = listConcords.Find(c => !string.IsNullOrEmpty(c.ArableLandType) && c.ArableLandType == familyMode);
            ContractConcord concordOther = listConcords.Find(c => !string.IsNullOrEmpty(c.ArableLandType) && c.ArableLandType != familyMode);
            if (concordFamily != null)
            {
                concordFamily.ConcordNumber = virtualPerson.ZoneCode.PadRight(14, '0') + virtualPerson.FamilyNumber.PadLeft(4, '0') + "J";
                concordFamily.ContractCredentialNumber = concordFamily.ConcordNumber;
                ConcordRepository.Update(concordFamily);
                var bookFamily = BookWorkStation.Get(concordFamily.ID);
                if (bookFamily != null)
                {
                    bookFamily.Number = concordFamily.ConcordNumber;
                    bookFamily.RegeditNumber = concordFamily.ConcordNumber;
                    BookRepository.Update(bookFamily);
                }
            }
            if (concordOther != null)
            {
                concordOther.ConcordNumber = virtualPerson.ZoneCode.PadRight(14, '0') + virtualPerson.FamilyNumber.PadLeft(4, '0') + "Q";
                concordOther.ContractCredentialNumber = concordOther.ConcordNumber;
                ConcordRepository.Update(concordOther);
                var bookOther = BookWorkStation.Get(concordOther.ID);
                if (bookOther != null)
                {
                    bookOther.Number = concordOther.ConcordNumber;
                    bookOther.RegeditNumber = concordOther.ConcordNumber;
                    BookRepository.Update(bookOther);
                }
            }
            VirtualPersonRepository.Update(virtualPerson);
            return TrySaveChanges(DefaultRepository);
        }

        /// <summary>
        /// 根据承包方id集合获取承包地块集合
        /// </summary>
        public List<ContractLand> GetCollection(List<Guid> ownerIds)
        {
            List<ContractLand> listLand = new List<ContractLand>();
            if (ownerIds == null || ownerIds.Count == 0)
            {
                return listLand;
                throw new YltException("获取承包方数据失败!");
            }
            foreach (var ownerId in ownerIds)
            {
                var lands = DefaultRepository.GetCollection(ownerId);
                if (lands == null || lands.Count == 0)
                    continue;
                listLand.AddRange(lands);
            }
            return listLand;
        }

        /// <summary>
        /// 获取发包方
        /// </summary>
        public List<CollectivityTissue> GetTissuesByConcord(Zone zone)
        {
            List<CollectivityTissue> tissues = new List<CollectivityTissue>();
            List<ContractConcord> concords = ConcordWorkStation.GetByZoneCode(zone.FullCode);     //GetCollection(zone.FullCode);
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
                tissue = TissueRepository.Get(concord.SenderId); //GetSenderById(concord.SenderId);                            
                if (tissue != null)
                {
                    tissues.Add(tissue);
                }
            }
            concords.Clear();
            return tissues;
        }

        #endregion

        #region Private

        /// <summary>
        /// 清空合同中的面积
        /// </summary>
        private void InitiallizeArea(ContractConcord concordFamily)
        {
            if (concordFamily == null)
                return;
            concordFamily.CountActualArea = 0;
            concordFamily.CountAwareArea = 0;
            concordFamily.TotalTableArea = 0;
        }

        /// <summary>
        /// 更新合同
        /// </summary> 
        private void UpdateConcord(ContractConcord concordFamily)
        {
            if (concordFamily == null)
                return;
            ConcordRepository.Update(concordFamily);
        }

        /// <summary>
        /// 更新地块
        /// </summary>
        private void UpdateContractLand(ContractLand land, IContractLandRepository landRep)
        {
            if (land == null)
                return;
            landRep.Update(land);
        }

        #endregion

        #region 数据导入导出

        /// <summary>
        /// 导出发包方Excel调查表
        /// </summary>
        /// <param name="zoneCode">行政地域编码</param>
        /// <returns>参数错误：-1；未获取发包方：0；成功导出：1</returns>
        public int ExportSenderSurveyExcelNew(string zoneCode, string DefaultPath, string unitname)
        {
            if (zoneCode.IsNullOrBlank())
                return -1;
            string tempName = TemplateHelperWork.ExcelTemplate(TemplateFileWork.SenderSurveyExcel);
            var listTissue = TissueRepository.GetTissues(zoneCode, eLevelOption.SelfAndSubs);
            if (listTissue == null || listTissue.Count == 0)
                return 0;

            using (ExportSenderSurveyTable export = new ExportSenderSurveyTable())
            {
                export.TissueCollection = listTissue;
                export.ShowValue = true;
                export.UnitName = unitname;
                export.SaveFileName = DefaultPath + "\\" + zoneCode + "-发包方调查表.xls";
                export.BeginToZone(tempName);
            }
            return 1;
        }

        /// <summary>
        /// 导出发包方Excel调查表
        /// </summary>
        public void ExportSenderSurveyExcel(string zoneCode)
        {
            if (zoneCode.IsNullOrBlank())
                return;
            string tempName = TemplateHelperWork.ExcelTemplate(TemplateFileWork.SenderSurveyExcel);
            var listTissue = TissueRepository.GetTissues(zoneCode);
            using (ExportSenderSurveyTable export = new ExportSenderSurveyTable())
            {
                export.TissueCollection = listTissue;
                export.ShowValue = true;
                export.BeginToZone(tempName);
            }
        }

        /// <summary>
        /// 导出发包方Word调查表
        /// </summary>
        public void ExportSenderSurveyWord(CollectivityTissue tissue, string DefaultPath)
        {
            if (tissue == null)
                return;
            string tempPath = TemplateHelperWork.WordTemplate(TemplateFileWork.SenderSurveyWord);
            ExportSenderWord senderTable = new ExportSenderWord();

            #region 通过反射等机制定制化具体的业务处理类
            var temp = WorksheetConfigHelper.GetInstance(senderTable);
            if (temp != null && temp.TemplatePath != null)
            {
                if (temp is ExportSenderWord)
                    senderTable = (ExportSenderWord)temp;
                tempPath = Path.Combine(TheApp.GetApplicationPath(), temp.TemplatePath);
            }
            #endregion

            senderTable.OpenTemplate(tempPath);
            senderTable.PrintPreview(tissue, DefaultPath + "\\" + tissue.Name + "-发包方调查表");
        }

        /// <summary>
        /// 导出承包方Word调查表
        /// </summary>
        public void ExportObligeeWord(Zone zone, VirtualPerson vp, string MarkDesc,
            string ConcordNumber, CollectivityTissue sender, List<Dictionary> diclist,
            string WarrentNumber, ContractRegeditBook book, string DefaultPath, bool ExportVPTableCountContainsDiedPerson, bool KeepRepeatFlag, Func<string> GetReplace = null)
        {
            if (vp == null || zone == null)
                return;

            string tempPath = TemplateHelperWork.WordTemplate(TemplateFileWork.ContractSurveyWord);  //模板文件
            var export = new ExportContractorWordTable();
            if (GetReplace != null)
                export.EmptyReplacement = GetReplace();

            #region 通过反射等机制定制化具体的业务处理类
            var temp = WorksheetConfigHelper.GetInstance(export);
            if (temp != null && temp.TemplatePath != null)
            {
                if (temp is ExportContractorWordTable)
                    export = (ExportContractorWordTable)temp;
                tempPath = Path.Combine(TheApp.GetApplicationPath(), temp.TemplatePath);
            }
            #endregion
            export.SystemSet.KeepRepeatFlag = KeepRepeatFlag;
            export.Contractor = vp;
            export.MarkDesc = MarkDesc;
            export.ExportVPTableCountContainsDiedPerson = ExportVPTableCountContainsDiedPerson;
            export.DictList = diclist;
            export.Book = book;
            export.ConcordNumber = ConcordNumber;
            export.Tissue = sender;  //发包方
            export.WarrentNumber = WarrentNumber;
            export.OpenTemplate(tempPath);
            export.PrintPreview(vp, DefaultPath + "\\" + vp.Name + "-承包方调查表");
        }

        /// <summary>
        /// 导出地块调查表(Word)
        /// </summary>
        public bool ExportLandWord(Zone zone, ContractLand land, VirtualPerson vp,
            ContractBusinessSettingDefineWork settingDefine, List<Dictionary> lstDict, string DefaultPath)
        {
            bool flag = true;
            int dotCount = 0;
            try
            {
                if (land == null)
                {
                    return flag;
                }
                var concord = (land.ConcordId != null && land.ConcordId.HasValue) ? ConcordRepository.Get(land.ConcordId.Value) : null;
                var tissue = concord != null ? TissueRepository.Get(concord.SenderId) : null;
                if (tissue == null && zone != null)
                {
                    tissue = TissueRepository.Get(zone.ID);
                }
                var listLandCoil = CoilRepository.GetByLandID(land.ID);
                var listLandDot = DotRepository.GetByLandID(land.ID);
                var listValidLandDot = listLandDot == null ? new List<BuildLandBoundaryAddressDot>() : listLandDot.FindAll(c => c.IsValid == true);
                ExportLandSurveyWordTable export = new ExportLandSurveyWordTable();
                export.SettingDefine = settingDefine;
                dotCount = listValidLandDot.Count == 0 ? (listLandDot == null ? 0 : listLandDot.Count) : (listValidLandDot.Count);
                string tempPath = TemplateHelperWork.WordTemplate(TemplateFileWork.ContractAccountLandSurveyWord);  //模板文件
                if (dotCount > 6 && dotCount <= 21)
                {
                    tempPath = TemplateHelperWork.WordTemplate(TemplateFileWork.ContractAccountLandSurveyWordTwo);  //模板文件2页
                }
                else if (dotCount > 21 && dotCount <= 36)
                {
                    tempPath = TemplateHelperWork.WordTemplate(TemplateFileWork.ContractAccountLandSurveyWordThree);  //模板文件3页
                }
                else if (dotCount > 36)
                {
                    tempPath = TemplateHelperWork.WordTemplate(TemplateFileWork.ContractAccountLandSurveyWordOther);  //模板文件其它
                }
                else { }
                export.Contractor = vp;
                export.DictList = lstDict;
                export.CurrentZone = zone;
                export.Concord = concord;
                export.Tissue = tissue; //GetTissue(zone.ID); //发包方
                export.ListLandCoil = listLandCoil == null ? new List<BuildLandBoundaryAddressCoil>() : listLandCoil;
                export.ListLandDot = listLandDot == null ? new List<BuildLandBoundaryAddressDot>() : listLandDot;
                export.ListLandValidDot = listValidLandDot;
                export.OpenTemplate(tempPath);
                export.PrintPreview(land, DefaultPath + "\\" + land.LandNumber + "-地块调查表");

            }
            catch (Exception ex)
            {
                flag = false;
                YuLinTu.Library.Log.Log.WriteException(this, "ExportLandWord(导出地块数据到Word表)", ex.Message + ex.StackTrace);
            }
            finally
            {
                GC.Collect();
            }
            return flag;
        }

        #endregion

        #region 配置获取

        #endregion
    }
}
