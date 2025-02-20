/*
 * (C) 2025  鱼鳞图公司版权所有,保留所有权利 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YuLinTu.Data;
using YuLinTu.Library.Entity;


namespace YuLinTu.Library.WorkStation
{
    /// <summary>
    /// 农村土地承包经营申请登记表业务逻辑层接口定义
    /// </summary>
    public interface IContractRequireTableWorkStation : IWorkstation<ContractRequireTable>
    {
        #region Methods


        /// <summary>
        /// 根据ID删除农村土地承包经营申请登记表对象
        /// </summary>
        /// <returns>-1（参数错误）/0（失败）/1（成功）</returns>
        int Delete(Guid guid);

        /// <summary>
        /// 更新农村土地承包经营申请登记表对象
        /// </summary>
        /// <returns>-1（参数错误）/0（失败）/1（成功）</returns>
        int Update(ContractRequireTable entity);

        /// <summary>
        /// 根据ID获取农村土地承包经营申请登记表对象
        /// </summary>
        ContractRequireTable Get(Guid guid);

        /// <summary>
        /// 根据ID判断农村土地承包经营申请登记表对象是否存在
        /// </summary>
        bool Exists(Guid guid);

        /// <summary>
        /// 根据申请书编号获取农村土地承包经营申请登记表对象
        /// </summary>
        /// <param name="tabNumber">申请书编号</param>
        /// <returns>农村土地承包经营申请登记表</returns>
        ContractRequireTable Get(string tabNumber);

        /// <summary>
        /// 根据组织编码获得以申请书编号排序农村土地承包经营申请登记表
        /// </summary>
        /// <param name="tissueCode">组织编码</param>
        /// <returns>申请登记表</returns>
        List<ContractRequireTable> GetTissueRequireTable(string tissueCode);


        /// <summary>
        /// 根据地域代码获取农村土地承包经营申请登记表
        /// </summary>
        /// <param name="zoneCode">地域代码</param>
        /// <param name="searchOption">查找类型</param>
        /// <returns>农村土地承包经营申请登记表</returns>
        List<ContractRequireTable> GetByZoneCode(string zoneCode, eSearchOption searchOption);

        /// <summary>
        /// 根据地域代码删除农村土地承包经营申请登记表
        /// </summary>
        /// <param name="zoneCode">地域代码</param>
        /// <param name="searchOption">查找类型</param>
        /// <returns>-1（参数错误）/0（失败）/1（成功）</returns>
        int DeleteByZoneCode(string zoneCode, eSearchOption searchOption);

        /// <summary>
        /// 按地域统计农村土地承包经营申请登记表的数量
        /// </summary>
        /// <param name="zoneCode">地域代码</param>
        /// <param name="searchOption">匹配级别</param>
        /// <returns>-1（参数错误）/int 对象数量</returns>
        int Count(string zoneCode, eLevelOption searchOption);

        /// <summary>
        /// 批量添加农村土地承包经营申请登记表数据
        /// </summary>
        /// <param name="listRequireTable">农村土地承包经营申请登记表对象集合</param>
        /// <returns>-1（参数错误）/int 添加对象数量</returns>
        int AddRange(List<ContractRequireTable> listRequireTable);
        /// <summary>
        /// 获取最大编号
        /// </summary>
        /// <returns></returns>
        int GetMaxNumber();

        #endregion
    }
}
