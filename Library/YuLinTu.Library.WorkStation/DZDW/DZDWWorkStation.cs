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
    /// 集体建设用地点状地物的业务逻辑层实现
    /// </summary>
    public class DZDWWorkStation : Workstation<DZDW>, IDZDWWorkStation
    {
        #region Properties

        /// <summary>
        /// 数据访问层接口
        /// </summary>
        public new IDZDWRepository DefaultRepository
        {
            get { return base.DefaultRepository as IDZDWRepository; }
            set { base.DefaultRepository = value; }
        }

        #endregion

        #region Ctor

        /// <summary>
        /// 构造函数
        /// </summary>
        public DZDWWorkStation(IDZDWRepository rep)
        {
            DefaultRepository = rep;
        }

        /// <summary>
        /// 根据区域代码获取以标识码排序的点状地物集合
        /// </summary>
        /// <param name="zoneCode">区域代码</param>
        /// <returns>点状地物集合</returns>
        public List<DZDW> GetByZoneCode(string zoneCode)
        {
            return DefaultRepository.GetByZoneCode(zoneCode);
        }

        /// <summary>
        /// 批量添加点状地物数据
        /// </summary>
        /// <param name="listPoint">点状地物对象集合</param>
        /// <returns>-1（参数错误）/int 添加对象数量</returns>
        public int AddRange(List<DZDW> listPoint)
        {
            int addCount = 0;
            if (listPoint == null || listPoint.Count == 0)
            {
                return addCount;
            }
            foreach (var point in listPoint)
            {
                DefaultRepository.Add(point);
            }
            addCount = TrySaveChanges(DefaultRepository);
            return addCount;
        }

        #endregion
    }
}
