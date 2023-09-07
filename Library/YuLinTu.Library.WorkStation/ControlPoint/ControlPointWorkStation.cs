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
    /// 控制点业务逻辑层接口实现
    /// </summary>
    public class ControlPointWorkStation : Workstation<ControlPoint>, IControlPointWorkStation
    {
        #region Properties

        /// <summary>
        /// 数据访问层接口属性
        /// </summary>
        public new IControlPointRepository DefaultRepository
        {
            get { return base.DefaultRepository as IControlPointRepository; }
            set { base.DefaultRepository = value; }
        }

        #endregion

        #region Ctor

        /// <summary>
        /// 构造函数
        /// </summary>
        public ControlPointWorkStation(IControlPointRepository rep)
        {
            DefaultRepository = rep;
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
            data = DefaultRepository.Get(id);
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
            isExsit = DefaultRepository.Exist(id);
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
            cnt = DefaultRepository.Update(controlPoint);
            return TrySaveChanges(DefaultRepository);
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
            cnt = DefaultRepository.Delete(ID);
            return TrySaveChanges(DefaultRepository);
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
            listControlPoint = DefaultRepository.GetByZoneCode(zoneCode);
            return listControlPoint;
        }

        /// <summary>
        /// 批量添加控制点数据
        /// </summary>
        /// <param name="listControlPoint">控制点对象集合</param>
        /// <returns>-1（参数错误）/int 添加对象数量</returns>
        public int AddRange(List<ControlPoint> listControlPoint)
        {
            int addCount = 0;
            if (listControlPoint == null || listControlPoint.Count == 0)
            {
                return addCount;
            }
            foreach (var controlPoint in listControlPoint)
            {
                DefaultRepository.Add(controlPoint);
            }
            addCount = TrySaveChanges(DefaultRepository);
            return addCount;
        }

        #endregion
    }
}
