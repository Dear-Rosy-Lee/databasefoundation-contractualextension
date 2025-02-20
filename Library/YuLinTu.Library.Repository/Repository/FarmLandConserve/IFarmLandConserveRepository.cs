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
    /// 基本农田保护区数据访问接口
    /// </summary>
    public interface IFarmLandConserveRepository : IRepository<FarmLandConserve>
    {
        #region Methods

        /// <summary>
        /// 根据唯一标识获取基本农田保护区对象
        /// </summary>
        /// <param name="id">唯一标识</param>
        /// <returns>基本农田保护区对象</returns>
        FarmLandConserve Get(Guid id);

        /// <summary>
        /// 根据唯一标识判断基本农田保护区对象是否存在？
        /// </summary>
        /// <param name="id">唯一标识</param>
        bool Exist(Guid id);

        /// <summary>
        /// 更新基本农田保护区对象
        /// </summary>
        /// <param name="farmLand">基本农田保护区对象</param>
        /// <returns>-1（参数错误）/0（失败）/1（成功）</returns>
        int Update(FarmLandConserve farmLand);

        /// <summary>
        /// 根据唯一标识删除基本农田保护区对象
        /// </summary>
        /// <param name="ID">基本农田保护区对象</param>
        /// <returns>-1（参数错误）/0（失败）/1（成功）</returns>
        int Delete(Guid ID);

        /// <summary>
        /// 根据地域编码获取基本农田保护区对象集合
        /// </summary>
        /// <param name="zoneCode">地域编码</param>
        /// <returns>基本农田保护区对象集合</returns>
        List<FarmLandConserve> GetByZoneCode(string zoneCode);

        #endregion
    }
}
