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
    /// 界址线四至的数据访问接口
    /// </summary>
    public interface IPropertyLineNeighborRepository :   IRepository<PropertyLineNeighbor>
    {
        #region Methods

        /// <summary>
        /// 根据id获得界址线四至数据
        /// </summary>
        /// <param name="id">id</param>
        /// <returns>界址线四至数据</returns>
        PropertyLineNeighbor Get(Guid id);

        /// <summary>
        /// 根据id判断界址线四至数据是否存在
        /// </summary>
        /// <param name="id">id</param>
        /// <returns>true（存在）/false（不存在）</returns>
        int Exist(Guid id);

        /// <summary>
        /// 根据集体建设用地使用权ID获得界址线四至数据
        /// </summary>
        /// <param name="landID">集体建设用地使用权ID</param>
        /// <returns>界址线四至数据</returns>
        PropertyLineNeighborCollection GetByLandID(Guid landID);

        /// <summary>
        /// 根据id删除界址线四至数据
        /// </summary>
        /// <param name="id">id</param>
        /// <returns>-1（参数错误）/0（失败）/1（成功）</returns>
        int Delete(Guid id);
        /// <summary>
        /// 根据集体建设用地使用权ID删除界址线四至数据
        /// </summary>
        /// <param name="landID">集体建设用地使用权ID</param>
        /// <returns>-1（参数错误）/0（失败）/1（成功）</returns>
        int DeleteByLand(Guid landID);

        /// <summary>
        /// 更新界址线四至数据
        /// </summary>
        /// <param name="neighbor">实体</param>
        /// <returns>-1（参数错误）/0（失败）/1（成功）</returns>
        int Update(PropertyLineNeighbor neighbor);

        #endregion
	}
}