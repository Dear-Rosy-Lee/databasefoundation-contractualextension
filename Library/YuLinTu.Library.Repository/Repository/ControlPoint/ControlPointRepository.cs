/*
 * (C) 2025  鱼鳞图公司版权所有,保留所有权利 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YuLinTu.Data;
using YuLinTu.Library.Entity;
using YuLinTu;
using System.Linq.Expressions;

namespace YuLinTu.Library.Repository
{
    /// <summary>
    /// 控制点数据访问接口实现
    /// </summary>
    public class ControlPointRepository : RepositoryDbContext<ControlPoint>, IControlPointRepository
    {
        #region Ctor

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="ds"></param>
        public ControlPointRepository(IDataSource ds)
            : base(ds)
        {
        }

        #endregion

        #region Methods

        /// <summary>
        /// 根据唯一标识获取控制点对象
        /// </summary>
        /// <param name="id">唯一标识</param>
        /// <returns>控制点对象</returns>
        public ControlPoint Get(Guid id)
        {
            ControlPoint data = null;
            if (!CheckRule.CheckGuidNullOrEmpty(id))
            {
                return data;
                throw new YltException("获取控制点唯一标识参数失败!");
            }
            data = base.Get(c => c.ID == id).FirstOrDefault();
            return data;
        }

        /// <summary>
        /// 根据唯一标识判断控制点对象是否存在？
        /// </summary>
        /// <param name="id">唯一标识</param>
        public bool Exist(Guid id)
        {
            bool isExsit = false;
            if (!CheckRule.CheckGuidNullOrEmpty(id))
            {
                return isExsit;
                throw new YltException("获取控制点唯一标识参数失败!");
            }
            isExsit = base.Count(c => c.ID == id) > 0 ? true : false;
            return isExsit;
        }

        /// <summary>
        /// 更新控制点对象
        /// </summary>
        /// <param name="controlPoint">控制点对象</param>
        /// <returns>-1（参数错误）/0（失败）/1（成功）</returns>
        public int Update(ControlPoint controlPoint)
        {
            int cnt = 0;
            if (controlPoint == null || !CheckRule.CheckGuidNullOrEmpty(controlPoint.ID))
            {
                return -1;
                throw new YltException("获取控制点对象参数失败!");
            }
            cnt = base.Update(controlPoint, c => c.ID == controlPoint.ID);
            return cnt;
        }

        /// <summary>
        /// 根据唯一标识删除控制点对象
        /// </summary>
        /// <param name="ID">唯一标识</param>
        /// <returns>-1（参数错误）/0（失败）/1（成功）</returns>
        public int Delete(Guid ID)
        {
            int cnt = 0;
            if (!CheckRule.CheckGuidNullOrEmpty(ID))
            {
                return -1;
                throw new YltException("获取控制点唯一标识参数失败!");
            }
            cnt = base.Delete(c => c.ID == ID);
            return cnt;
        }

        /// <summary>
        /// 根据地域编码获取控制点对象集合
        /// </summary>
        /// <param name="zoneCode">地域编码</param>
        /// <returns>控制点对象集合</returns>
        public List<ControlPoint> GetByZoneCode(string zoneCode)
        {
            List<ControlPoint> listControlPoint = new List<ControlPoint>();
            if (!CheckRule.CheckStringNullOrEmpty(ref zoneCode))
            {
                return null;
                throw new YltException("获取地域编码参数失败!");
            }
            listControlPoint = (from q in DataSource.CreateQuery<ControlPoint>()
                                where q.ZoneCode.Equals(zoneCode)
                                orderby q.Code
                                select q).ToList();
            return listControlPoint;
        }

        #endregion
    }
}
