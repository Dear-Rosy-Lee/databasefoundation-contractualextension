/*
 * (C) 2025  鱼鳞图公司版权所有,保留所有权利 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YuLinTu.Data;
using YuLinTu.Library.Entity;

namespace YuLinTu.Library.Repository
{
    /// <summary>
    /// 区域界线数据访问接口
    /// </summary>
    public interface IZoneBoundaryRepository : IRepository<ZoneBoundary>
    {
        #region Methods

        /// <summary>
        /// 根据唯一标识获取区域界线对象
        /// </summary>
        /// <param name="id">唯一标识</param>
        /// <returns>区域界线对象</returns>
        ZoneBoundary Get(Guid id);

        /// <summary>
        /// 根据唯一标识判断区域界线对象是否存在？
        /// </summary>
        /// <param name="id">唯一标识</param>
        bool Exist(Guid id);

        /// <summary>
        /// 更新区域界线对象
        /// </summary>
        /// <param name="zoneBoundary">区域界线对象</param>
        /// <returns>-1（参数错误）/0（失败）/1（成功）</returns>
        int Update(ZoneBoundary zoneBoundary);

        /// <summary>
        /// 根据唯一标识删除区域界线对象
        /// </summary>
        /// <param name="ID">区域界线对象</param>
        /// <returns>-1（参数错误）/0（失败）/1（成功）</returns>
        int Delete(Guid ID);

        /// <summary>
        /// 根据地域编码获取区域界线对象集合
        /// </summary>
        /// <param name="zoneCode">地域编码</param>
        /// <returns>区域界线对象集合</returns>
        List<ZoneBoundary> GetByZoneCode(string zoneCode);

        #endregion    
    }
}
