/*
 * (C) 2015  鱼鳞图公司版权所有,保留所有权利 
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
    /// 集体建设用地调查宗地的业务逻辑层接口
    /// </summary>
    public interface IDCZDWorkStation : IWorkstation<DCZD>
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

        ///// <summary>
        ///// 添加调查宗地
        ///// </summary>  
        //int Add(DCZD dot);

        /// <summary>
        /// 批量添加调查宗地数据
        /// </summary>
        /// <param name="listDot">调查宗地对象集合</param>
        /// <returns>-1（参数错误）/int 添加对象数量</returns>
        int AddRange(List<DCZD> listDot);

        /// <summary>
        /// 批量更新调查宗地数据
        /// </summary>
        /// <param name="listDot">调查宗地对象集合</param>
        /// <returns>-1（参数错误）/int 更新对象数量</returns>
        int UpdateRange(List<DCZD> listDot);

        #endregion
    }
}
