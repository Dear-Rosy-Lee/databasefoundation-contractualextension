// (C) 2015 鱼鳞图公司版权所有，保留所有权利
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YuLinTu.Data;
using YuLinTu.Library.Entity;
using YuLinTu;

namespace YuLinTu.Library.Repository
{
    /// <summary>
    /// 农村土地承包经营权登记薄的数据访问接口
    /// </summary>
    public interface IContractRegeditBookRepository : IRepository<ContractRegeditBook>
    {
        #region Methods

        /// <summary>
        /// 删除农村土地承包经营权登记薄对象
        /// </summary>
        /// <returns>-1（参数错误）/0（失败）/1（成功）</returns>
        int Delete(Guid guid);

        /// <summary>
        /// 删除农村土地承包经营权登记薄对象
        /// </summary>
        /// <param name="number">权证编号</param>
        /// <returns>-1（参数错误）/0（失败）/1（成功）</returns>
        int DeleteByRegeditNumber(string number);

        /// <summary>
        /// 更新农村土地承包经营权登记薄对象
        /// </summary>
        /// <returns>-1（参数错误）/0（失败）/1（成功）</returns>
        int Update(ContractRegeditBook entity, bool onlycode = false);

        /// <summary>
        /// 根据ID获取农村土地承包经营权登记薄对象
        /// </summary>
        ContractRegeditBook Get(Guid guid);

        /// <summary>
        /// 根据合同id集合获取权证集合
        /// </summary>
        List<ContractRegeditBook> GetByConcordIds(Guid[] ids);

        /// <summary>
        /// 根据ID判断农村土地承包经营权登记薄对象是否存在
        /// </summary>
        /// <param name="guid">ID</param>
        /// <returns>true（存在）/false（不存在）</returns>
        bool Exists(Guid guid);

        /// <summary>
        /// 通过登记薄编号查看是否存在有权证号相同但Guid不同的存在。
        /// </summary>
        /// <param name="concordNumber">登记薄编号</param>
        /// <returns>true（存在）/false（不存在）</returns>
        bool Exists(string regeditNumber);

        /// <summary>
        /// 通过登记薄编号获取农村土地承包经营权登记薄对象
        /// </summary>
        /// <param name="regeditNumber">登记薄编号</param>
        ContractRegeditBook Get(string regeditNumber);

        /// <summary>
        /// 根据权证流水号及其查找类型获取农村土地承包经营权登记薄对象
        /// </summary>
        /// <param name="number">权证流水号</param>
        /// <param name="searchOption">查找类型</param>
        /// <returns>农村土地承包经营权登记薄对象</returns>
        ContractRegeditBook GetByNumber(string number, eSearchOption searchOption);

        /// <summary>
        /// 根据地域代码及其查找类型获取权证
        /// </summary>
        /// <param name="zoneCode">地域代码</param>
        /// <param name="searchOption">查找类型</param>
        /// <returns>权证</returns>
        List<ContractRegeditBook> GetByZoneCode(string zoneCode, eSearchOption searchOption);

        /// <summary>
        /// 根据权证号获取及其查找类型获取权证
        /// </summary>
        /// <param name="number">权证号</param>
        /// <param name="searchOption">查找类型</param>
        /// <returns>权证</returns>
        List<ContractRegeditBook> GetCollection(string number, eSearchOption searchOption);

        /// <summary>
        /// 根据不同匹配等级获取该地域下的权证
        /// </summary>
        /// <param name="zoneCode">区域代码</param>
        /// <param name="searchOption">匹配级别</param>
        /// <returns>农村土地承包权证</returns>
        List<ContractRegeditBook> GetContractsByZoneCode(string zoneCode, eLevelOption searchOption);

        /// <summary>
        /// 根据地域代码及其查找类型删除权证
        /// </summary>
        /// <param name="zoneCode">地域代码</param>
        /// <param name="searchOption">查找类型</param>
        /// <returns>-1（参数错误）/0（失败）/1（成功）</returns>
        int DeleteByZoneCode(string zoneCode, eSearchOption searchOption);

        /// <summary>
        /// 删除当前地域下所有指定承包方状态的权证
        /// </summary>
        /// <param name="zoneCode">地域代码</param>
        /// <param name="virtualStatue">承包方状态</param>
        /// <param name="levelOption">地域匹配等级</param>
        /// <returns>-1（参数错误）/0（失败）/1（成功）</returns>
        int DeleteByZoneCode(string zoneCode, eLevelOption levelOption, eVirtualPersonStatus virtualStatus);

        /// <summary>
        /// 删除当前地域下所有指定承包方状态的权证
        /// </summary>
        /// <param name="zoneCode">地域代码</param>
        /// <param name="levelOption">地域匹配等级</param>
        /// <returns>-1（参数错误）/0（失败）/1（成功）</returns>
        int DeleteByZoneCode(string zoneCode, eLevelOption levelOption);

        /// <summary>
        /// 按地域及其匹配类型统计权证数量
        /// </summary>
        /// <param name="zoneCode">地域代码</param>
        /// <param name="searchOption">匹配类型</param>
        /// <returns>-1（参数错误）/int 权证数量</returns>
        int Count(string zoneCode, eLevelOption searchOption);

        /// <summary>
        /// 存在权证数据的地域集合
        /// </summary>
        List<Zone> ExistZones(List<Zone> zoneList);

        /// <summary>
        /// 获取最新的权证流水号
        /// </summary>
        string GetNewSerialNumber(int minNumber,int maxNumber);

        /// <summary>
        /// 获取最大流水号
        /// </summary>
        /// <returns></returns>
        int GetMaxSerialNumber();

        /// <summary>
        /// 在整库中获取最大流水号
        /// </summary>
        int GetMaxSerialNumber1();

        #endregion
    }
}