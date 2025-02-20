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
    /// 控制点数据访问接口
    /// </summary>
    public interface IControlPointRepository : IRepository<ControlPoint>
    {
        #region Methods

        /// <summary>
        /// 根据唯一标识获取控制点对象
        /// </summary>
        /// <param name="id">唯一标识</param>
        /// <returns>控制点对象</returns>
        ControlPoint Get(Guid id);

        /// <summary>
        /// 根据唯一标识判断控制点对象是否存在？
        /// </summary>
        /// <param name="id">唯一标识</param>
        bool Exist(Guid id);

        /// <summary>
        /// 更新控制点对象
        /// </summary>
        /// <param name="controlPoint">控制点对象</param>
        /// <returns>-1（参数错误）/0（失败）/1（成功）</returns>
        int Update(ControlPoint controlPoint);

        /// <summary>
        /// 根据唯一标识删除控制点对象
        /// </summary>
        /// <param name="ID">唯一标识</param>
        /// <returns>-1（参数错误）/0（失败）/1（成功）</returns>
        int Delete(Guid ID);

        /// <summary>
        /// 根据地域编码获取控制点对象集合
        /// </summary>
        /// <param name="zoneCode">地域编码</param>
        /// <returns>控制点对象集合</returns>
        List<ControlPoint> GetByZoneCode(string zoneCode);

        #endregion
    }
}
