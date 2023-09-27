/*
 * (C) 2015  鱼鳞图公司版权所有,保留所有权利
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YuLinTu.Data;
using YuLinTu.Library.Entity;
using YuLinTu.Library.Repository;

namespace YuLinTu.Library.WorkStation
{
    /// <summary>
    /// 承包合同接口实现
    /// </summary>
    public class ConcordWorkStation : Workstation<ContractConcord>, IConcordWorkStation
    {
        #region Properties

        public new IContractConcordRepository DefaultRepository
        {
            get { return base.DefaultRepository as IContractConcordRepository; }
            set { base.DefaultRepository = value; }
        }

        #endregion Properties

        #region Ctor

        public ConcordWorkStation(IContractConcordRepository rep)
        {
            DefaultRepository = rep;
        }

        #endregion Ctor

        #region Methods

        /// <summary>
        /// 根据id删除承包合同
        /// </summary>
        public int Delete(Guid guid)
        {
            DefaultRepository.Delete(guid);
            return TrySaveChanges(DefaultRepository);
        }

        /// <summary>
        /// 根据承包承包方标识集合删除农村土地承包合同
        /// </summary>
        /// <param name="ownerIds">承包方标识集合</param>
        /// <returns></returns>
        public int DeleteByOwnerIds(List<Guid> ownerIds)
        {
            foreach (var id in ownerIds)
            {
                DefaultRepository.DeleteByOwnerId(id);
            }
            return TrySaveChanges(DefaultRepository);
        }

        /// <summary>
        /// 更新一个地域下所有的合同为不可用
        /// </summary>
        /// <param name="zoneCode">区域代码</param>
        /// <returns>-1（参数错误）/0（失败）/1（成功）</returns>
        public int UpdateConcordInValid(string zoneCode)
        {
            DefaultRepository.UpdateConcordInValid(zoneCode);
            return TrySaveChanges(DefaultRepository);
        }

        /// <summary>
        /// 根据承包方Id更新对象的承包方姓名
        /// </summary>
        ///<param name="houseHolderID">承包方Id</param>
        ///<param name="houseHolderName">承包方姓名</param>
        ///<returns>-1（参数错误）/0（失败）/1（成功）</returns>
        public int Update(Guid houseHolderID, string houseHolderName)
        {
            DefaultRepository.Update(houseHolderID, houseHolderName);
            return TrySaveChanges(DefaultRepository);
        }

        /// <summary>
        /// 更新合同
        /// </summary>
        public int Update(ContractConcord entity)
        {
            DefaultRepository.Update(entity);
            return TrySaveChanges(DefaultRepository);
        }

        /// <summary>
        /// 根据id获取合同
        /// </summary>
        public ContractConcord Get(Guid guid)
        {
            return DefaultRepository.Get(guid);
        }

        /// <summary>
        /// 根据合同编码获取合同
        /// </summary>
        public ContractConcord Get(string concordNumber)
        {
            return DefaultRepository.Get(concordNumber);
        }

        /// <summary>
        /// 根据承包合同唯一标识判断农村土地承包合同对象是否存在
        /// </summary>
        public bool Exists(Guid guid)
        {
            return DefaultRepository.Exists(guid);
        }

        /// <summary>
        /// 通过合同编号查看合同是否存在 如果存在又是否为指定的承包方姓名
        /// </summary>
        /// <param name="concordNumber">合同编号</param>
        /// <param name="virtualPersonName">指定的承包方姓名</param>
        /// <returns>true（存在）/false（不存在</returns>
        public bool ExistsNotZoneCodeToConcordNumber(string concordNumber, string virtualPersonName)
        {
            return DefaultRepository.ExistsNotZoneCodeToConcordNumber(concordNumber, virtualPersonName);
        }

        /// <summary>
        /// 据农村土地承包经营权申请书ID获取农村土地承包合同
        /// </summary>
        /// <param name="requireBookId">农村土地承包经营权申请书ID</param>
        /// <returns>农村土地承包合同</returns>
        public ContractConcord GetByRequireBookId(Guid requireBookId)
        {
            return DefaultRepository.GetByRequireBookId(requireBookId);
        }

        /// <summary>
        /// 获取该地域下的合同
        /// </summary>
        /// <param name="zoneCode">区域代码</param>
        /// <returns>农村土地承包合同</returns>
        public List<ContractConcord> GetContractsByZoneCode(string zoneCode)
        {
            return DefaultRepository.GetContractsByZoneCode(zoneCode);
        }

        /// <summary>
        /// 根据区域代码、合同可用性获取农村土地承包合同集合
        /// </summary>
        /// <param name="zoneCode">区域代码</param>
        /// <param name="isValid">合同可用性</param>
        /// <returns>农村土地承包合同集合</returns>
        public List<ContractConcord> GetContractsByZoneCode(string zoneCode, bool isVaild)
        {
            return DefaultRepository.GetContractsByZoneCode(zoneCode, isVaild);
        }

        /// <summary>
        /// 根据区域代码、合同可用性获取农村土地承包合同集合
        /// </summary>
        /// <param name="zoneCode">区域代码</param>
        /// <param name="isValid">合同可用性</param>
        /// <returns>农村土地承包合同集合</returns>
        public List<ContractConcord> GetContractsByZoneCode(string zoneCode, eLevelOption searchOption, bool isVaild)
        {
            return DefaultRepository.GetContractsByZoneCode(zoneCode, searchOption, isVaild);
        }

        /// <summary>
        /// 根据当前合同是否可用、当前合同是否可用、区域代码获得农村土地承包合同对象
        /// </summary>
        /// <param name="isValid">当前合同是否可用</param>
        /// <param name="status">业务流程状态</param>
        /// <param name="zoneCode">区域代码</param>
        /// <param name="searchOption">查找类型</param>
        /// <returns>农村土地承包合同</returns>
        public List<ContractConcord> GetCollection(bool isValid, eStatus status, string zoneCode, eLevelOption searchOption)
        {
            return DefaultRepository.GetCollection(isValid, status, zoneCode, searchOption);
        }

        /// <summary>
        /// 根据区域代码获取以合同编号排序的农村土地承包合同集合
        /// </summary>
        /// <param name="zoneCode">地域编码</param>
        /// <returns></returns>
        public List<ContractConcord> GetByZoneCode(string zoneCode)
        {
            return DefaultRepository.GetByZoneCode(zoneCode);
        }

        /// <summary>
        /// 根据区域与承包方姓名获取所有农村土地承包合同集合
        /// </summary>
        /// <param name="zoneCode">区域代码</param>
        /// <param name="name">承包方姓名</param>
        /// <returns>农村土地承包合同集合</returns>
        public List<ContractConcord> GetByZoneCodeAndFamilyName(string zoneCode, string familyName)
        {
            return DefaultRepository.GetByZoneCodeAndFamilyName(zoneCode, familyName);
        }

        /// <summary>
        /// 根据区域代码、合同可用性获取农村土地承包合同集合
        /// </summary>
        /// <param name="zoneCode">区域代码</param>
        /// <param name="isValid">合同可用性</param>
        /// <returns>农村土地承包合同集合</returns>
        public List<ContractConcord> GetContractsByZoneCode(string zoneCode, eLevelOption searchOption)
        {
            return DefaultRepository.GetContractsByZoneCode(zoneCode, searchOption);
        }

        /// <summary>
        /// 根据发包方Id获取以承包方姓名排序的农村土地承包合同
        /// </summary>
        /// <param name="guid">发包方Id</param>
        /// <returns>农村土地承包合同</returns>
        public List<ContractConcord> GetContractsByTissueID(Guid guid)
        {
            return DefaultRepository.GetContractsByTissueID(guid);
        }

        public int CountByTissueID(Guid tissueID)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 根据发包方Id统计农村土地承包合同的数量
        /// </summary>
        /// <param name="tissueID">发包方Id</param>
        /// <returns>-1（参数错误）/int 农村土地承包合同的数量</returns>
        public List<ContractConcord> GetContractsByFamilyID(Guid guid)
        {
            return DefaultRepository.GetContractsByFamilyID(guid);
        }

        /// <summary>
        /// 根据承包方Id集合获取农村土地承包合同集合
        /// </summary>
        public List<ContractConcord> GetContractsByFamilyIDs(Guid[] guids)
        {
            return DefaultRepository.GetContractsByFamilyIDs(guids);
        }

        /// <summary>
        /// 根据承包方Id获取农村土地承包合同
        /// </summary>
        /// <param name="guid">承包方Id</param>
        /// <returns>农村土地承包合同</returns>
        public List<ContractConcord> GetAllConcordByFamilyID(Guid guid)
        {
            return DefaultRepository.GetAllConcordByFamilyID(guid);
        }

        /// <summary>
        /// 根据合同号和地域查找合同
        /// </summary>
        /// <param name="number">农村土地承包合同编号</param>
        /// <param name="searchOption">合同编号查找类型</param>
        /// <param name="zoneCode">区域代码</param>
        /// <param name="searchOption">区域代码匹配级别</param>
        /// <returns>农村土地承包合同集合</returns>
        public List<ContractConcord> SearchContractsByNumber(string number, eSearchOption searchOption, string zoneCode, eLevelOption levelOption)
        {
            return DefaultRepository.SearchContractsByNumber(number, searchOption, zoneCode, levelOption);
        }

        /// <summary>
        /// 根据承包方和地域查找合同
        /// </summary>
        /// <param name="name">承包方姓名</param>
        /// <param name="searchOption">承包方姓名查找类型</param>
        /// <param name="zoneCode">区域代码</param>
        /// <param name="searchOption">区域代码匹配级别</param>
        /// <returns>农村土地承包合同集合</returns>
        public List<ContractConcord> SearchContractsByContracterName(string number, eSearchOption searchOption, string zoneCode, eLevelOption levelOption)
        {
            return DefaultRepository.SearchContractsByContracterName(number, searchOption, zoneCode, levelOption);
        }

        /// <summary>
        /// 根据合同编号查找合同
        /// </summary>
        /// <param name="concordNumber">合同编号</param>
        /// <param name="searchOption">查找类型</param>
        /// <returns>农村土地承包合同集合</returns>
        public List<ContractConcord> SearchContractsByConcordNumber(string concordNumber, eSearchOption searchOption)
        {
            return DefaultRepository.SearchContractsByConcordNumber(concordNumber, searchOption);
        }

        /// <summary>
        /// 根据户主名称查找合同
        /// </summary>
        /// <param name="name">承包方姓名</param>
        /// <param name="searchOption">查找类型</param>
        /// <returns>农村土地承包合同集合</returns>
        public List<ContractConcord> GetContractsByFamilyName(string name, eSearchOption searchOption)
        {
            return DefaultRepository.GetContractsByFamilyName(name, searchOption);
        }

        /// <summary>
        /// 根据地域与户主名称查找合同可用的农村土地承包合同
        /// </summary>
        /// <param name="zoneCode">区域代码</param>
        /// <param name="Name">承包方姓名</param>
        /// <returns>农村土地承包合同集合</returns>
        public List<ContractConcord> GetContractsByFamilyName(string zoneCode, string name)
        {
            return DefaultRepository.GetContractsByFamilyName(zoneCode, name);
        }

        /// <summary>
        /// 查找这个地域编码对应地域下所有的不可用的合同
        /// </summary>
        /// <param name="zoneCode">地域编码</param>
        /// <returns>不可用的合同</returns>
        public List<ContractConcord> GetByZoneSearchBadConcord(string zoneCode)
        {
            return DefaultRepository.GetByZoneSearchBadConcord(zoneCode);
        }

        /// <summary>
        /// 验证合同编号是否存在
        /// </summary>
        public bool Exists(string concordNumber)
        {
            return DefaultRepository.Exists(concordNumber);
        }

        /// <summary>
        /// 删除当前地域下所有合同
        /// </summary>
        /// <param name="zoneCode">地域代码</param>
        /// <returns>-1（参数错误）/0（失败）/1（成功）</returns>
        public int DeleteByZoneCode(string zoneCode)
        {
            DefaultRepository.DeleteByZoneCode(zoneCode);
            return TrySaveChanges(DefaultRepository);
        }

        /// <summary>
        /// 根据匹配级别删除地域下的合同
        /// </summary>
        /// <param name="zoneCode">地域编码</param>
        /// <param name="searchOption">匹配级别</param>
        /// <returns>-1（参数错误）/0（失败）/1（成功）</returns>
        public int DeleteByZoneCode(string zoneCode, eLevelOption searchOption)
        {
            DefaultRepository.DeleteByZoneCode(zoneCode, searchOption);
            return TrySaveChanges(DefaultRepository);
        }

        /// <summary>
        /// 删除当前地域下所有指定承包方状态的合同
        /// </summary>
        /// <param name="zoneCode">地域代码</param>
        /// <param name="virtualStatue">承包方状态</param>
        /// <param name="levelOption">地域匹配等级</param>
        /// <returns>-1（参数错误）/0（失败）/1（成功）</returns>
        public int DeleteByZoneCode(string zoneCode, eVirtualPersonStatus virtualStatue, eLevelOption levelOption)
        {
            DefaultRepository.DeleteByZoneCode(zoneCode, virtualStatue, levelOption);
            return TrySaveChanges(DefaultRepository);
        }

        /// <summary>
        /// 删除当前地域下所有指定承包方状态的合同(包括相关联数据)
        /// </summary>
        public int DeleteRelationDataByZoneCode(string zoneCode, eVirtualPersonStatus virtualStatue, eLevelOption levelOption)
        {
            DefaultRepository.DeleteRelationDataByZoneCode(zoneCode, virtualStatue, levelOption);
            return TrySaveChanges(DefaultRepository);
        }

        /// <summary>
        /// 删除当前地域下所有(指定承包方状态)的合同
        /// </summary>
        /// <param name="zoneCode">地域代码</param>
        /// <param name="levelOption">地域匹配等级</param>
        /// <returns>-1（参数错误）/0（失败）/1（成功）</returns>
        public int DeleteOtherByZoneCode(string zoneCode, eLevelOption levelOption)
        {
            DefaultRepository.DeleteOtherByZoneCode(zoneCode, levelOption);
            return TrySaveChanges(DefaultRepository);
        }

        /// <summary>
        /// 统计指定地域下已颁证的数量。
        /// </summary>
        /// <param name="zoneCode">地域编码</param>
        /// <returns>已颁证的数量。</returns>
        public int CountAwareRegeditBooks(string zoneCode)
        {
            return DefaultRepository.CountAwareRegeditBooks(zoneCode);
        }

        /// <summary>
        /// 根据农村土地承包合同编号及其不同的查找类型来获得以承包方姓名排序的合同
        /// </summary>
        /// <param name="concordNumber">农村土地承包合同编号</param>
        /// <param name="searchOption">查找类型</param>
        /// <returns>合同</returns>
        public List<ContractConcord> SearchByConcordNumber(string number, eSearchOption searchOption)
        {
            return DefaultRepository.SearchContractsByConcordNumber(number, searchOption);
        }

        /// <summary>
        /// 根据承包方姓名及其不同的查找类型来获得以承包方姓名排序的合同
        /// </summary>
        /// <param name="name">承包方姓名</param>
        /// <param name="searchOption">查找类型</param>
        /// <returns>合同</returns>
        public List<ContractConcord> SearchByVirtualPersonName(string name, eSearchOption searchOption)
        {
            return DefaultRepository.SearchByVirtualPersonName(name, searchOption);
        }

        /// <summary>
        /// 根据农村土地承包合同编号及其不同的查找类型来获得以目标区域代码开始的以承包方姓名排序的合同
        /// </summary>
        /// <param name="concordNumber">农村土地承包合同编号</param>
        /// <param name="searchOption">查找类型</param>
        /// <param name="zoneCode">目标区域代码</param>
        /// <returns>合同</returns>
        public List<ContractConcord> SearchByConcordNumber(string number, eSearchOption searchOption, string zoneCode)
        {
            return DefaultRepository.SearchByConcordNumber(number, searchOption, zoneCode);
        }

        /// <summary>
        /// 根据承包方姓名及其不同的查找类型来获得以目标区域代码开始的以承包方姓名排序的合同
        /// </summary>
        /// <param name="name">承包方姓名</param>
        /// <param name="searchOption">查找类型</param>
        /// <param name="zoneCode">目标区域代码</param>
        /// <returns>合同</returns>
        public List<ContractConcord> SearchByVirtualPersonName(string name, eSearchOption searchOption, string zoneCode)
        {
            return DefaultRepository.SearchByConcordNumber(name, searchOption, zoneCode);
        }

        /// <summary>
        /// 统计指定地域下合同数量。
        /// </summary>
        /// <param name="zoneCode">地域编码</param>
        /// <returns>合同的数量。</returns>
        public int CountCords(string zoneCode, eLevelOption searchOption)
        {
            return DefaultRepository.CountCords(zoneCode, searchOption);
        }

        /// <summary>
        /// 存在合同的地域集合
        /// </summary>
        public List<Zone> ExistZones(List<Zone> zoneList)
        {
            return DefaultRepository.ExistZones(zoneList);
        }

        /// <summary>
        /// 批量添加承包合同数据
        /// </summary>
        /// <param name="listPerson">承包合同对象集合</param>
        /// <returns>-1（参数错误）/int 添加对象数量</returns>
        public int AddRange(List<ContractConcord> listConcord)
        {
            int addCount = 0;
            if (listConcord == null || listConcord.Count == 0)
            {
                return addCount;
            }
            foreach (var concord in listConcord)
            {
                DefaultRepository.Add(concord);
            }
            addCount = TrySaveChanges(DefaultRepository);
            return addCount;
        }

        #endregion Methods
    }
}