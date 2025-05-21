// (C) 2025 鱼鳞图公司版权所有，保留所有权利
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YuLinTu.Data;
using YuLinTu.Library.Entity;
using YuLinTu;
using System.Linq.Expressions;

namespace YuLinTu.Library.Repository
{
    /// <summary>
    /// 农村土地承包合同的数据访问接口
    /// </summary>
    public interface IContractConcordRepository : IRepository<ContractConcord>
    {
        #region Methods

        /// <summary>
        /// 根据承包合同唯一标识删除农村土地承包合同
        /// </summary>
        /// <param name="guid">承包合同唯一标识</param>
        /// <returns>-1（参数错误）/0（失败）/1（成功）</returns>
        int Delete(Guid guid);

        /// <summary>
        /// 更新一个地域下所有的合同为不可用
        /// </summary>
        /// <param name="zoneCode">区域代码</param>
        /// <returns>-1（参数错误）/0（失败）/1（成功）</returns>
        int UpdateConcordInValid(string zoneCode);

        /// <summary>
        /// 根据承包方Id更新对象的承包方姓名
        /// </summary>
        ///<param name="houseHolderID">承包方Id</param>
        ///<param name="houseHolderName">承包方姓名</param>
        ///<returns>-1（参数错误）/0（失败）/1（成功）</returns>
        int Update(Guid houseHolderID, string houseHolderName);

        /// <summary>
        /// 更新对象
        /// </summary>
        /// <returns>-1（参数错误）/0（失败）/1（成功）</returns>
        int Update(ContractConcord entity, bool onlycode = false);

        /// <summary>
        /// 批量更新实体集合
        /// </summary>
        /// <param name="entities"></param>
        /// <returns></returns>
        int UpdateRange(Expression<Func<ContractConcord, bool>> predicate, KeyValueList<string, object> values);

        /// <summary>
        /// 根据承包合同唯一标识获取农村土地承包合同
        /// </summary>
        /// <param name="guid">承包合同唯一标识</param>
        ContractConcord Get(Guid guid);

        /// <summary>
        /// 根据农村土地承包合同编号获取合同对象
        /// </summary>
        ContractConcord Get(string concordNumber);

        /// <summary>
        /// 根据承包合同唯一标识判断农村土地承包合同对象是否存在
        /// </summary>
        bool Exists(Guid guid);

        /// <summary>
        /// 存在合同数据的地域集合
        /// </summary>
        List<Zone> ExistZones(List<Zone> zoneList);

        /// <summary>
        /// 通过合同编号查看合同是否存在 如果存在又是否为指定的承包方姓名
        /// </summary>
        /// <param name="concordNumber">合同编号</param>
        /// <param name="virtualPersonName">指定的承包方姓名</param>
        /// <returns>true（存在）/false（不存在</returns>
        bool ExistsNotZoneCodeToConcordNumber(string concordNumber, string virtualPersonName);

        /// <summary>
        /// 根据农村土地承包经营权申请书ID获取农村土地承包合同
        /// </summary>
        /// <param name="requireBookId">农村土地承包经营权申请书ID</param>
        /// <returns>农村土地承包合同</returns>
        ContractConcord GetByRequireBookId(Guid requireBookId);

        /// <summary>
        /// 获取该地域下的合同
        /// </summary>
        /// <param name="zoneCode">区域代码</param>
        /// <returns>农村土地承包合同</returns>
        List<ContractConcord> GetContractsByZoneCode(string zoneCode);

        /// <summary>
        /// 根据区域代码、合同可用性获取农村土地承包合同集合
        /// </summary>
        /// <param name="zoneCode">区域代码</param>
        /// <param name="isValid">合同可用性</param>
        /// <returns>农村土地承包合同集合</returns>
        List<ContractConcord> GetContractsByZoneCode(string zoneCode, bool isVaild);

        /// <summary>
        /// 根据区域代码、合同可用性不同匹配性获取农村土地承包合同集合
        /// </summary>
        /// <param name="zoneCode">区域代码</param>
        /// <param name="searchOption">匹配级别</param>
        /// <param name="isValid">合同可用性</param>
        /// <returns>农村土地承包合同集合</returns>
        List<ContractConcord> GetContractsByZoneCode(string zoneCode, eLevelOption searchOption, bool isVaild);

        /// <summary>
        /// 根据当前合同是否可用、当前合同是否可用、区域代码获得农村土地承包合同对象
        /// </summary>
        /// <param name="isValid">当前合同是否可用</param>
        /// <param name="status">业务流程状态</param>
        /// <param name="zoneCode">区域代码</param>
        /// <param name="searchOption">查找类型</param>
        /// <returns>农村土地承包合同</returns>
        List<ContractConcord> GetCollection(bool isValid, eStatus status, string zoneCode, eLevelOption searchOption);

        /// <summary>
        /// 根据区域代码获取以合同编号排序的农村土地承包合同集合
        /// </summary>
        /// <param name="zoneCode">区域代码</param>
        /// <returns>农村土地承包合同集合</returns>
        List<ContractConcord> GetByZoneCode(string zoneCode);

        /// <summary>
        /// 根据区域与承包方姓名获取所有农村土地承包合同集合
        /// </summary>
        /// <param name="zoneCode">区域代码</param>
        /// <param name="name">承包方姓名</param>
        /// <returns>农村土地承包合同集合</returns>
        List<ContractConcord> GetByZoneCodeAndFamilyName(string zoneCode, string familyName);

        /// <summary>
        /// 根据不同匹配等级获取该地域下的合同
        /// </summary>
        /// <param name="zoneCode">区域代码</param>
        /// <param name="searchOption">匹配级别</param>
        /// <returns>农村土地承包合同</returns>
        List<ContractConcord> GetContractsByZoneCode(string zoneCode, eLevelOption searchOption);

        /// <summary>
        /// 根据发包方Id获取以承包方姓名排序的农村土地承包合同
        /// </summary>
        /// <param name="guid">发包方Id</param>
        /// <returns>农村土地承包合同</returns>
        List<ContractConcord> GetContractsByTissueID(Guid guid);

        /// <summary>
        /// 根据发包方Id统计农村土地承包合同的数量
        /// </summary>
        /// <param name="tissueID">发包方Id</param>
        /// <returns>-1（参数错误）/int 农村土地承包合同的数量</returns>
        int CountByTissueID(Guid tissueID);

        /// <summary>
        /// 统计指定地域下合同数量。
        /// </summary>
        /// <param name="zoneCode">地域编码</param>
        /// <returns>合同的数量。</returns>
        int CountCords(string zoneCode, eLevelOption searchOption);

        /// <summary>
        /// 根据承包方Id获取农村土地承包合同
        /// </summary>
        /// <param name="guid">承包方Id</param>
        /// <returns>农村土地承包合同</returns>
        List<ContractConcord> GetContractsByFamilyID(Guid guid);

        /// <summary>
        /// 根据承包方Id集合获取农村土地承包合同集合
        /// </summary>
        List<ContractConcord> GetContractsByFamilyIDs(Guid[] guids);

        /// <summary>
        /// 根据承包方Id获取农村土地承包合同
        /// </summary>
        /// <param name="guid">承包方Id</param>
        /// <returns>农村土地承包合同</returns>
        List<ContractConcord> GetAllConcordByFamilyID(Guid guid);

        /// <summary>
        /// 根据合同号和地域查找合同
        /// </summary>
        /// <param name="number">农村土地承包合同编号</param>
        /// <param name="searchOption">合同编号查找类型</param>
        /// <param name="zoneCode">区域代码</param>
        /// <param name="levelOption">区域代码匹配级别</param>
        /// <returns>农村土地承包合同集合</returns>
        List<ContractConcord> SearchContractsByNumber(string number, eSearchOption searchOption, string zoneCode, eLevelOption levelOption);

        /// <summary>
        /// 根据承包方和地域查找合同
        /// </summary>
        /// <param name="name">承包方姓名</param>
        /// <param name="searchOption">承包方姓名查找类型</param>
        /// <param name="zoneCode">区域代码</param>
        /// <param name="searchOption">区域代码匹配级别</param>
        /// <returns>农村土地承包合同集合</returns>
        List<ContractConcord> SearchContractsByContracterName(string number, eSearchOption searchOption, string zoneCode, eLevelOption levelOption);

        /// <summary>
        /// 根据合同编号查找合同
        /// </summary>
        /// <param name="concordNumber">合同编号</param>
        /// <param name="searchOption">查找类型</param>
        /// <returns>农村土地承包合同集合</returns>
        List<ContractConcord> SearchContractsByConcordNumber(string concordNumber, eSearchOption searchOption);

        /// <summary>
        /// 根据户主名称查找合同
        /// </summary>
        /// <param name="name">承包方姓名</param>
        /// <param name="searchOption">查找类型</param>
        /// <returns>农村土地承包合同集合</returns>
        List<ContractConcord> GetContractsByFamilyName(string name, eSearchOption searchOption);

        /// <summary>
        /// 根据地域与户主名称查找合同可用的农村土地承包合同
        /// </summary>
        /// <param name="zoneCode">区域代码</param>
        /// <param name="Name">承包方姓名</param>
        /// <returns>农村土地承包合同集合</returns>
        List<ContractConcord> GetContractsByFamilyName(string zoneCode, string name);

        /// <summary>
        /// 查找这个地域编码对应地域下所有的不可用的合同
        /// </summary>
        /// <param name="zoneCode">地域编码</param>
        /// <returns>不可用的合同</returns>
        List<ContractConcord> GetByZoneSearchBadConcord(string zoneCode);

        /// <summary>
        /// 通过合同编号查看是否存在有此合同
        /// </summary>
        /// <param name="concordNumber">合同编号</param>
        /// <returns></returns>
        bool Exists(string concordNumber);

        /// <summary>
        /// 删除当前地域下所有合同
        /// </summary>
        /// <param name="zoneCode">地域代码</param>
        /// <returns>-1（参数错误）/0（失败）/1（成功）</returns>
        int DeleteByZoneCode(string zoneCode);

        /// <summary>
        /// 删除当前地域下所有合同
        /// </summary>
        /// <param name="zoneCode">地域代码</param>
        /// <returns>-1（参数错误）/0（失败）/1（成功）</returns>
        int DeleteByZoneCode(string zoneCode, eLevelOption searchOption);

        /// <summary>
        /// 删除当前地域下所有指定承包方状态的合同
        /// </summary>
        /// <param name="zoneCode">地域代码</param>
        /// <param name="virtualStatue">承包方状态</param>
        /// <param name="levelOption">地域匹配等级</param>
        /// <returns>-1（参数错误）/0（失败）/1（成功）</returns>
        int DeleteByZoneCode(string zoneCode, eVirtualPersonStatus virtualStatue, eLevelOption levelOption);

        /// <summary>
        /// 删除当前地域下所有指定承包方状态的合同(包括相关联数据)
        /// </summary>
        int DeleteRelationDataByZoneCode(string zoneCode, eVirtualPersonStatus virtualStatue, eLevelOption levelOption);

        /// <summary>
        /// 根据承包方唯一标识删除农村土地承包合同
        /// </summary>
        /// <param name="guid">承包方唯一标识</param>
        /// <returns>-1（参数错误）/0（失败）/1（成功）</returns>
        int DeleteByOwnerId(Guid ownerId);

        /// <summary>
        /// 删除当前地域下所有(指定承包方状态)的合同
        /// </summary>
        /// <param name="zoneCode">地域代码</param>
        /// <param name="levelOption">地域匹配等级</param>
        /// <returns>-1（参数错误）/0（失败）/1（成功）</returns>
        int DeleteOtherByZoneCode(string zoneCode, eLevelOption levelOption);

        /// <summary>
        /// 统计指定地域下已颁证的数量。
        /// </summary>
        /// <param name="zoneCode">地域编码</param>
        /// <returns>已颁证的数量。</returns>
        /// <returns>-1（参数错误）/0（失败）/1（成功）</returns>
        int CountAwareRegeditBooks(string zoneCode);

        /// <summary>
        /// 根据农村土地承包合同编号及其不同的查找类型来获得以承包方姓名排序的合同
        /// </summary>
        /// <param name="concordNumber">农村土地承包合同编号</param>
        /// <param name="searchOption">查找类型</param>
        /// <returns>合同</returns>
        List<ContractConcord> SearchByConcordNumber(string number, eSearchOption searchOption);

        /// <summary>
        /// 根据承包方姓名及其不同的查找类型来获得以目标区域代码开始的以承包方姓名排序的合同
        /// </summary>
        /// <param name="name">承包方姓名</param>
        /// <param name="searchOption">查找类型</param>
        /// <param name="zoneCode">目标区域代码</param>
        /// <returns>合同</returns>
        List<ContractConcord> SearchByVirtualPersonName(string name, eSearchOption searchOption);

        /// <summary>
        /// 根据农村土地承包合同编号及其不同的查找类型来获得以目标区域代码开始的以承包方姓名排序的合同
        /// </summary>
        /// <param name="concordNumber">农村土地承包合同编号</param>
        /// <param name="searchOption">查找类型</param>
        /// <param name="zoneCode">目标区域代码</param>
        /// <returns>合同</returns>
        List<ContractConcord> SearchByConcordNumber(string number, eSearchOption searchOption, string zoneCode);

        /// <summary>
        /// 根据承包方姓名及其不同的查找类型来获得以目标区域代码开始的以承包方姓名排序的合同
        /// </summary>
        /// <param name="name">承包方姓名</param>
        /// <param name="searchOption">查找类型</param>
        /// <param name="zoneCode">目标区域代码</param>
        /// <returns>合同</returns>
        List<ContractConcord> SearchByVirtualPersonName(string name, eSearchOption searchOption, string zoneCode);

        #endregion Methods
    }
}