/*
 * (C) 2015  鱼鳞图公司版权所有,保留所有权利 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YuLinTu.Data;
using YuLinTu.Library.Entity;

namespace YuLinTu.Library.Repository
{
    /// <summary>
    /// 基本农田保护区数据访问接口实现
    /// </summary>
    public class FarmLandConserveRepository : RepositoryDbContext<FarmLandConserve>, IFarmLandConserveRepository
    {
        #region Ctor

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="ds"></param>
        public FarmLandConserveRepository(IDataSource ds)
            : base(ds)
        {
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
            data = base.Get(c => c.ID == id).FirstOrDefault();
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
            isExsit = base.Count(c => c.ID == id) > 0 ? true : false;
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
            cnt = base.Update(farmLand, c => c.ID == farmLand.ID);
            return cnt;
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
            cnt = base.Delete(c => c.ID == ID);
            return cnt;
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
            listFarmLand = (from q in DataSource.CreateQuery<FarmLandConserve>()
                            where q.ZoneCode.Equals(zoneCode)
                            orderby q.Code
                            select q).ToList();
            return listFarmLand;
        }

        #endregion
    }
}
