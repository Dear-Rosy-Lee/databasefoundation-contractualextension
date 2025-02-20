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
    /// 区域界线业务逻辑层接口实现
    /// </summary>
    public class ZoneBoundaryWorkStation : Workstation<ZoneBoundary>, IZoneBoundaryWorkStation
    {
        #region Properties

        /// <summary>
        /// 数据访问层接口属性
        /// </summary>
        public new IZoneBoundaryRepository DefaultRepository
        {
            get { return base.DefaultRepository as IZoneBoundaryRepository; }
            set { base.DefaultRepository = value; }
        }

        #endregion

        #region Ctor

        /// <summary>
        /// 构造函数
        /// </summary>
        public ZoneBoundaryWorkStation(IZoneBoundaryRepository rep)
        {
            DefaultRepository = rep;
        }

        #endregion

        #region Methods

        /// <summary>
        /// 根据唯一标识获取区域界线对象
        /// </summary>
        /// <param name="id">唯一标识</param>
        /// <returns>区域界线对象</returns>
        public ZoneBoundary Get(Guid id)
        {
            ZoneBoundary data = null;
            if (!CheckRule.CheckGuidNullOrEmpty(id))
            {
                return data;
                throw new YltException("获取区域界线唯一标识参数失败!");
            }
            data = DefaultRepository.Get(id);
            return data;
        }

        /// <summary>
        /// 根据唯一标识判断区域界线对象是否存在？
        /// </summary>
        /// <param name="id">唯一标识</param>
        public bool Exist(Guid id)
        {
            bool isExsit = false;
            if (!CheckRule.CheckGuidNullOrEmpty(id))
            {
                return isExsit;
                throw new YltException("获取区域界线唯一标识参数失败!");
            }
            isExsit = DefaultRepository.Exist(id);
            return isExsit;
        }

        /// <summary>
        /// 更新区域界线对象
        /// </summary>
        /// <param name="zoneBoundary">区域界线对象</param>
        /// <returns>-1（参数错误）/0（失败）/1（成功）</returns>
        public int Update(ZoneBoundary zoneBoundary)
        {
            int cnt = 0;
            if (zoneBoundary == null || !CheckRule.CheckGuidNullOrEmpty(zoneBoundary.ID))
            {
                return -1;
                throw new YltException("获取区域界线对象参数失败!");
            }
            cnt = DefaultRepository.Update(zoneBoundary);
            return TrySaveChanges(DefaultRepository);
        }

        /// <summary>
        /// 根据唯一标识删除区域界线对象
        /// </summary>
        /// <param name="ID">区域界线对象</param>
        /// <returns>-1（参数错误）/0（失败）/1（成功）</returns>
        public int Delete(Guid ID)
        {
            int cnt = 0;
            if (!CheckRule.CheckGuidNullOrEmpty(ID))
            {
                return -1;
                throw new YltException("获取区域界线唯一标识参数失败!");
            }
            cnt = DefaultRepository.Delete(ID);
            return TrySaveChanges(DefaultRepository);
        }

        /// <summary>
        /// 根据地域编码获取区域界线对象集合
        /// </summary>
        /// <param name="zoneCode">地域编码</param>
        /// <returns>区域界线对象集合</returns>
        public List<ZoneBoundary> GetByZoneCode(string zoneCode)
        {
            List<ZoneBoundary> listZoneBoundary = new List<ZoneBoundary>();
            if (!CheckRule.CheckStringNullOrEmpty(ref zoneCode))
            {
                return null;
                throw new YltException("获取地域编码参数失败!");
            }
            listZoneBoundary = DefaultRepository.GetByZoneCode(zoneCode);
            return listZoneBoundary;
        }

        /// <summary>
        /// 批量添加区域界线数据
        /// </summary>
        /// <param name="listZoneBoundary">区域界线对象集合</param>
        /// <returns>-1（参数错误）/int 添加对象数量</returns>
        public int AddRange(List<ZoneBoundary> listZoneBoundary)
        {
            int addCount = 0;
            if (listZoneBoundary == null || listZoneBoundary.Count == 0)
            {
                return addCount;
            }
            foreach (var zoneBoundary in listZoneBoundary)
            {
                DefaultRepository.Add(zoneBoundary);
            }
            addCount = TrySaveChanges(DefaultRepository);
            return addCount;
        }

        #endregion
    }
}
