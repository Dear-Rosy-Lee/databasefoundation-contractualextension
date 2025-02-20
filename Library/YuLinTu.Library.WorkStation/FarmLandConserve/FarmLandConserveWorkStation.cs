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
    /// 基本农田保护区业务逻辑层接口实现
    /// </summary>
    public class FarmLandConserveWorkStation : Workstation<FarmLandConserve>, IFarmLandConserveWorkStation
    {
        #region Properties

        /// <summary>
        /// 数据访问层接口属性
        /// </summary>
        public new IFarmLandConserveRepository DefaultRepository
        {
            get { return base.DefaultRepository as IFarmLandConserveRepository; }
            set { base.DefaultRepository = value; }
        }

        #endregion

        #region Ctor

        /// <summary>
        /// 构造函数
        /// </summary>
        public FarmLandConserveWorkStation(IFarmLandConserveRepository rep)
        {
            DefaultRepository = rep;
        }

        #endregion

        #region Methods

        /// <summary>
        /// 根据唯一标识获取基本农田保护区对象
        /// </summary>
        /// <param name="id">唯一标识</param>
        /// <returns>基本农田保护区对象</returns>
        public FarmLandConserve Get(Guid id)
        {
            FarmLandConserve data = null;
            if (!CheckRule.CheckGuidNullOrEmpty(id))
            {
                return data;
                throw new YltException("获取区域界线唯一标识参数失败!");
            }
            data = DefaultRepository.Get(id);
            return data;
        }

        /// <summary>
        /// 根据唯一标识判断基本农田保护区对象是否存在？
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
        /// 更新基本农田保护区对象
        /// </summary>
        /// <param name="farmLand">基本农田保护区对象</param>
        /// <returns>-1（参数错误）/0（失败）/1（成功）</returns>
        public int Update(FarmLandConserve farmLand)
        {
            int cnt = 0;
            if (farmLand == null || !CheckRule.CheckGuidNullOrEmpty(farmLand.ID))
            {
                return -1;
                throw new YltException("获取区域界线对象参数失败!");
            }
            cnt = DefaultRepository.Update(farmLand);
            return TrySaveChanges(DefaultRepository);
        }

        /// <summary>
        /// 根据唯一标识删除基本农田保护区对象
        /// </summary>
        /// <param name="ID">基本农田保护区对象</param>
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
        /// 根据地域编码获取基本农田保护区对象集合
        /// </summary>
        /// <param name="zoneCode">地域编码</param>
        /// <returns>基本农田保护区对象集合</returns>
        public List<FarmLandConserve> GetByZoneCode(string zoneCode)
        {
            List<FarmLandConserve> listFarmLand = new List<FarmLandConserve>();
            if (!CheckRule.CheckStringNullOrEmpty(ref zoneCode))
            {
                return null;
                throw new YltException("获取地域编码参数失败!");
            }
            listFarmLand = DefaultRepository.GetByZoneCode(zoneCode);
            return listFarmLand;
        }

        /// <summary>
        /// 批量添加基本农田保护区数据
        /// </summary>
        /// <param name="listFarmLandConserve">基本农田保护区对象集合</param>
        /// <returns>-1（参数错误）/int 添加对象数量</returns>
        public int AddRange(List<FarmLandConserve> listFarmLandConserve)
        {
            int addCount = 0;
            if (listFarmLandConserve == null || listFarmLandConserve.Count == 0)
            {
                return addCount;
            }
            foreach (var farmLandConserve in listFarmLandConserve)
            {
                DefaultRepository.Add(farmLandConserve);
            }
            addCount = TrySaveChanges(DefaultRepository);
            return addCount;
        }

        #endregion
    }
}
