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
    /// 地块的数据访问接口
    /// </summary>
    public interface IAgricultureEquityRepository :   IRepository<AgricultureEquity>
    {
        #region Methods
        
        /// <summary>
        /// 添加对象
        /// </summary>
        new string Add(object entity);

        /// <summary>
        /// 删除对象
        /// </summary>
        /// <returns>-1（参数错误）/0（失败）/1（成功）</returns>
        int Delete(Guid guid);

        /// <summary>
        /// 更新对象
        /// </summary>
        /// <returns>-1（参数错误）/0（失败）/1（成功）</returns>
        int Update(AgricultureEquity entity);

        /// <summary>
        /// 获取对象
        /// </summary>
        AgricultureEquity Get(Guid guid);

        /// <summary>
        /// 判断对象是否存在
        /// </summary>
        bool Exists(Guid guid);

        /// <summary>
        /// 判断地籍号是否存在
        /// </summary>
        /// <param name="cadastralNumber"></param>
        /// <returns>true（存在）/false（不存在）</returns>
        bool ExistByCadastralNumber(string cadastralNumber);

        #endregion

        #region ExtendMethod

        /// <summary>
        /// 获取地域下地块
        /// </summary>
        /// <param name="zoneCode">区域代码</param>
        /// <returns>地域下地块</returns>
        AgricultureEquityCollection GetCollection(string zoneCode);

        /// <summary>
        /// 根据地籍号获取对象
        /// </summary>
        /// <param name="number">地籍号</param>
        /// <returns>地块</returns>
        AgricultureEquity Get(string number);

        /// <summary>
        /// 删除当前地域下所有地块
        /// </summary>
        /// <param name="zoneCode">地域代码</param>
        /// <returns>-1（参数错误）/0（失败）/1（成功）</returns>
        int DeleteByZoneCode(string zoneCode);

        /// <summary>
        /// 按地域统计
        /// </summary>
        /// <param name="zoneCode">区域代码</param>
        /// <param name="searchOption">匹配等级</param>
        /// <returns>-1（参数错误）/int 对象数量</returns>
        int Count(string zoneCode, eLevelOption searchOption);

        /// <summary>
        /// 按地域删除地块
        /// </summary>
        /// <param name="zoneCode">地域代码</param>
        /// <param name="searchOption">匹配等级</param>
        /// <returns>-1（参数错误）/0（失败）/1（成功）</returns>
        int DeleteByZoneCode(string zoneCode, eLevelOption searchOption);

        /// <summary>
        /// 按地域获取地块
        /// </summary>
        /// <param name="zoneCode">地域代码</param>
        /// <param name="searchOption">匹配等级</param>
        /// <returns>地块</returns>
        AgricultureEquityCollection GetCollection(string zoneCode, eLevelOption searchOption);

        #endregion
    }
}