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
    /// 集体建设用地调查宗地的数据访问接口
    /// </summary>
    public interface IDCZDRepository : IRepository<DCZD>
    {
        #region Methods

        /// <summary>
        /// 根据标识码获取集体建设用地使用权调查宗地对象
        /// </summary>
        /// <param name="id">标识码</param>
        /// <returns>集体建设用地使用权调查宗地对象</returns>
        DCZD Get(Guid id);

        /// <summary>
        /// 判断标识码对象是否存在？
        /// </summary>
        /// <param name="id">标识码</param>
        bool Exist(Guid id);

        /// <summary>
        /// 更新集体建设用地使用权调查宗地对象
        /// </summary>
        /// <param name="dot">集体建设用地使用权调查宗地对象</param>
        /// <returns>-1（参数错误）/0（失败）/1（成功）</returns>
        int Update(DCZD dot);

        /// <summary>
        /// 根据标识码删除集体建设用地使用权调查宗地对象数量
        /// </summary>
        /// <param name="ID">标识码</param>
        /// <returns>-1（参数错误）/0（失败）/1（成功）</returns>
        int Delete(Guid ID);

        #endregion
    }
}
