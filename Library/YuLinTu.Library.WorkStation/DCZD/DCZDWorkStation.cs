/*
 * (C) 2015  鱼鳞图公司版权所有,保留所有权利 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YuLinTu.Data;
using YuLinTu.Library.Entity;
using YuLinTu.Library.Repository;

namespace YuLinTu.Library.WorkStation
{
    /// <summary>
    /// 集体建设用地调查宗地的业务逻辑层实现
    /// </summary>
    public class DCZDWorkStation : Workstation<DCZD>, IDCZDWorkStation
    {
        #region Properties

        /// <summary>
        /// 数据访问层接口
        /// </summary>
        public new IDCZDRepository DefaultRepository
        {
            get { return base.DefaultRepository as IDCZDRepository; }
            set { base.DefaultRepository = value; }
        }

        #endregion

        #region Ctor

        /// <summary>
        /// 构造函数
        /// </summary>
        public DCZDWorkStation(IDCZDRepository rep)
        {
            DefaultRepository = rep;
        }

        #endregion

        #region Methods

        /// <summary>
        /// 根据标识码获取集体建设用地使用权调查宗地对象
        /// </summary>
        /// <param name="id">标识码</param>
        /// <returns>集体建设用地使用权调查宗地对象</returns>
        public DCZD Get(Guid id)
        {
            return DefaultRepository.Get(id);
        }

        /// <summary>
        /// 判断标识码对象是否存在？
        /// </summary>
        /// <param name="id">标识码</param>
        public bool Exist(Guid id)
        {
            return DefaultRepository.Exist(id);
        }



        /// <summary>
        /// 更新集体建设用地使用权调查宗地对象
        /// </summary>
        /// <param name="dot">集体建设用地使用权调查宗地对象</param>
        /// <returns>-1（参数错误）/0（失败）/1（成功）</returns>
        public int Update(DCZD dot)
        {
            DefaultRepository.Update(dot);
            return TrySaveChanges(DefaultRepository);
        }

        /// <summary>
        /// 根据标识码删除集体建设用地使用权调查宗地对象数量
        /// </summary>
        /// <param name="ID">标识码</param>
        /// <returns>-1（参数错误）/0（失败）/1（成功）</returns>
        public int Delete(Guid ID)
        {
            DefaultRepository.Delete(ID);
            return TrySaveChanges(DefaultRepository);
        }



        /// <summary>
        /// 按地域、土地权属类型统计集体建设用地使用权调查宗地对象
        /// </summary>
        /// <param name="zoneCode">地域代码</param>
        /// <param name="searchOption">查找类型</param>
        /// <returns>-1（参数错误）/int 对象数量</returns>
        public int Count(string zoneCode, eLevelOption searchOption, string landType)
        {
            return DefaultRepository.Count(zoneCode, searchOption, landType);
        }

        ///// <summary>
        ///// 添加调查宗地
        ///// </summary>
        //public int Add(DCZD dot)
        //{
        //    DefaultRepository.Add(dot);
        //    return TrySaveChanges(DefaultRepository);
        //}

        /// <summary>
        /// 批量添加调查宗地数据
        /// </summary>
        /// <param name="listDot">调查宗地对象集合</param>
        /// <returns>-1（参数错误）/int 添加对象数量</returns>
        public int AddRange(List<DCZD> listDot)
        {
            foreach (var dot in listDot)
            {
                DefaultRepository.Add(dot);
            }
            return TrySaveChanges(DefaultRepository);
        }

        /// <summary>
        /// 批量更新调查宗地数据
        /// </summary>
        /// <param name="listDot">调查宗地对象集合</param>
        /// <returns>-1（参数错误）/int 更新对象数量</returns>
        public int UpdateRange(List<DCZD> listDot)
        {
            foreach (var dot in listDot)
            {
                DefaultRepository.Update(dot);
            }
            return TrySaveChanges(DefaultRepository);
        }

        #endregion
    }
}
