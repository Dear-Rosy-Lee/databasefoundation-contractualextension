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
    /// 土地利用现状分类表的数据访问接口
    /// </summary>
    public interface ILandTypeRepository :   IRepository<LandType>
    {
        #region Methods

        /// <summary>
        /// 更新土地利用现状分类表对象
        /// </summary>
        /// <returns>-1（参数错误）/0（失败）/1（成功）</returns>
        int Update(LandType landType);

        #endregion

        #region ExtendMethods
        /// <summary>
        /// 根据标识码删除土地利用现状分类表对象
        /// </summary>
        /// <returns>-1（参数错误）/0（失败）/1（成功）</returns>
        int Delete(int id);

        /// <summary>
        /// 根据标识码获取土地利用现状分类表对象
        /// </summary>
        /// <returns>土地利用现状分类表</returns>
        LandType Get(int id);

        /// <summary>
        /// 根据标识码判断土地利用现状分类表对象是否存在
        /// </summary>
        /// <returns>true（存在）/false（不存在）</returns>
        bool Exists(int id);

        /// <summary>
        /// 选择所有二级实体对象
        /// </summary>
        /// <returns>土地利用现状分类表集合</returns>
        LandTypeCollection SelectChildrenType();

        /// <summary>
        /// 选择一级下二级实体对象
        /// </summary>
        /// <param name="code">代码</param>
        /// <returns>土地利用现状分类表集合</returns>
        LandTypeCollection SelectChildrenType(string code);

        /// <summary>
        /// 选择所有类型
        /// </summary>
        /// <returns>土地利用现状分类表集合</returns>
        LandTypeCollection SelectDetailType();

        /// <summary>
        /// 选择主要类型
        /// </summary>
        /// <returns>土地利用现状分类表集合</returns>
        LandTypeCollection SelectMainType();

        /// <summary>
        /// 根据代码选择土地利用现状分类表集合对象
        /// </summary>
        /// <param name="code">代码</param>
        /// <returns>土地利用现状分类表</returns>
        LandType SelectByCode(string code);

        /// <summary>
        /// 根据名称选择对象
        /// </summary>
        /// <param name="name">名称</param>
        /// <returns>土地利用现状分类表</returns>
        LandType SelectByName(string name);
        #endregion
	}
}