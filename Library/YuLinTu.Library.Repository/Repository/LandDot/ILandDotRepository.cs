// (C) 2015 鱼鳞图公司版权所有，保留所有权利
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YuLinTu.Data;
using YuLinTu.Library.Entity;

namespace YuLinTu.Library.Repository
{
    /// <summary>
    /// 界址点表的数据访接口
    /// </summary>
    public interface ILandDotRepository :   IRepository<LandDot>
    {
        #region Methods

        /// <summary>
        /// 根据标识码获取界址点表数据
        /// </summary>
        /// <param name="id">标识码</param>
        /// <returns>界址点表数据</returns>
        LandDot Get(Guid id);

        /// <summary>
        /// 根据x、y坐标与区域代码获取界址点表数据
        /// </summary>
        /// <param name="x">x坐标</param>
        /// <param name="y">y坐标</param>
        /// <param name="zoneCode">区域代码</param>
        /// <returns>界址点表数据</returns>
        LandDot Get(double x, double y, string zoneCode);

        /// <summary>
        /// 根据区域代码获取界址点表数据
        /// </summary>
        /// <param name="zoneCode">区域代码</param>
        /// <returns>界址点表数据集合</returns>
        LandDotCollection Get(string zoneCode);

        /// <summary>
        /// 根据x、y坐标获取界址点表数据
        /// </summary>
        /// <param name="x">x坐标</param>
        /// <param name="y">y坐标</param>
        /// <returns>界址点表数据</returns>
        LandDot Get(double x, double y);

        /// <summary>
        /// 更新界址点表数据
        /// </summary>
        /// <param name="landDot">实体</param>
        /// <returns>-1（参数错误）/0（失败）/1（成功）</returns>
        int Update(LandDot landDot);

        /// <summary>
        /// 删除界址点表数据
        /// </summary>
        /// <param name="landDotID">界址点表标识码</param>
        /// <returns>-1（参数错误）/0（失败）/1（成功）</returns>
        int Delete(Guid landDotID);

        /// <summary>
        /// 根据区域代码删除界址点表数据
        /// </summary>
        /// <param name="zoneCode">区域代码</param>
        /// <returns>-1（参数错误）/0（失败）/1（成功）</returns>
        int DeleteByZone(string zoneCode);

        #endregion
    }
}