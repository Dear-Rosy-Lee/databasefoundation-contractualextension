/*
 * (C) 2025  鱼鳞图公司版权所有,保留所有权利 
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
    public class MZDWWorkStation : Workstation<MZDW>, IMZDWWorkStation
    {
        #region Properties

        /// <summary>
        /// 数据访问层接口
        /// </summary>
        public new IMZDWRepository DefaultRepository
        {
            get { return base.DefaultRepository as IMZDWRepository; }
            set { base.DefaultRepository = value; }
        }

        #endregion

        #region Ctor

        /// <summary>
        /// 构造函数
        /// </summary>
        public MZDWWorkStation(IMZDWRepository rep)
        {
            DefaultRepository = rep;
        }

        /// <summary>
        /// 根据区域代码获取以标识码排序的面状地物集合
        /// </summary>
        /// <param name="zoneCode">区域代码</param>
        /// <returns>面状地物集合</returns>
        public List<MZDW> GetByZoneCode(string zoneCode)
        {
            return DefaultRepository.GetByZoneCode(zoneCode);
        }

        /// <summary>
        /// 批量添加面状地物数据
        /// </summary>
        /// <param name="listPolygon">面状地物对象集合</param>
        /// <returns>-1（参数错误）/int 添加对象数量</returns>
        public int AddRange(List<MZDW> listPolygon)
        {
            int addCount = 0;
            if (listPolygon == null || listPolygon.Count == 0)
            {
                return addCount;
            }
            foreach (var polygon in listPolygon)
            {
                DefaultRepository.Add(polygon);
            }
            addCount = TrySaveChanges(DefaultRepository);
            return addCount;
        }

        #endregion
    }
}
