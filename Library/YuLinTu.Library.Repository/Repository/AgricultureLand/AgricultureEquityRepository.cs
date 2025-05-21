// (C) 2025 鱼鳞图公司版权所有，保留所有权利
using System;
using YuLinTu.Library.Entity;
using System.Data;
using System.Data.SqlClient;
using System.Collections.Generic;
using YuLinTu.Data;
using YuLinTu;

namespace YuLinTu.Library.Repository
{
    /// <summary>
    /// 地块的数据访问类
    /// </summary>
    public class AgricultureEquityRepository : RepositoryDbContext<AgricultureEquity>,  IAgricultureEquityRepository
    {
        #region Ctor
        
        private IDataSourceSchema m_DSSchema = null;

        public AgricultureEquityRepository(IDataSource ds)
            : base(ds) 
        {

            m_DSSchema = ds.CreateSchema();
        }
        #endregion

        #region Methods

        /// <summary>
        /// 检查表是否存在
        /// </summary>
        /// <returns></returns>
        private bool CheckTableExist()
        {
            //try
            //{
            //    return m_DSSchema.AnyElement(m_DSSchema.GetElements()[0].Schema, "AgricultureEquity");
            //}
            //catch (Exception)
            //{
            //    return false;
            //}

            return true;
        }

        /// <summary>
        /// 添加对象
        /// </summary>
        public new string Add(object entity)
        {
            if (!CheckTableExist())
            {
                throw new ArgumentNullException("数据库不存在表："
                    + this.GetType().ToString().Substring(this.GetType().ToString().LastIndexOf('.') + 1).Replace("Repository", ""));
            }
            if (entity == null|| !(entity is AgricultureEquity))
                return "添加实体为null或类型错误";
            int cnt = base.Add(entity);

            if (cnt > 0)
                return entity.ToString();

            return null;
        }

        /// <summary>
        /// 删除对象
        /// </summary>
        /// <returns>-1（参数错误）/0（失败）/1（成功）</returns>
        public int Delete(Guid guid)
        {
            if (!CheckTableExist())
            {
                throw new ArgumentNullException("数据库不存在表："
                    + this.GetType().ToString().Substring(this.GetType().ToString().LastIndexOf('.') + 1).Replace("Repository", ""));
            }
            if (!CheckRule.CheckGuidNullOrEmpty(guid))
                return -1;
            return Delete(c => c.ID.Equals(guid));
        }

        /// <summary>
        /// 更新对象
        /// </summary>
        /// <returns>-1（参数错误）/0（失败）/1（成功）</returns>
        public int Update(AgricultureEquity entity)
        {
            if (!CheckTableExist())
            {
                throw new ArgumentNullException("数据库不存在表："
                    + this.GetType().ToString().Substring(this.GetType().ToString().LastIndexOf('.') + 1).Replace("Repository", ""));
            }
            if (entity == null || !CheckRule.CheckGuidNullOrEmpty(entity.ID))
                return -1;
            entity.ModifiedTime = DateTime.Now;
            return Update(entity, c => c.ID.Equals(entity.ID));
        }

        /// <summary>
        /// 获取对象
        /// </summary>
        public AgricultureEquity Get(Guid guid)
        {
            if (!CheckTableExist())
            {
                throw new ArgumentNullException("数据库不存在表："
                    + this.GetType().ToString().Substring(this.GetType().ToString().LastIndexOf('.') + 1).Replace("Repository", ""));
            }
            if (!CheckRule.CheckGuidNullOrEmpty(guid))
                return null;
            object entity = Get(c => c.ID.Equals(guid));
            return entity as AgricultureEquity;
        }

        /// <summary>
        /// 判断对象是否存在
        /// </summary>
        public bool Exists(Guid guid)
        {
            if (!CheckTableExist())
            {
                throw new ArgumentNullException("数据库不存在表："
                    + this.GetType().ToString().Substring(this.GetType().ToString().LastIndexOf('.') + 1).Replace("Repository", ""));
            }
            if (!CheckRule.CheckGuidNullOrEmpty(guid))
                return false;

            int count = Count(c => c.ID.Equals(guid));
            return count > 0 ? true : false;
        }

        /// <summary>
        /// 判断地籍号是否存在
        /// </summary>
        /// <param name="cadastralNumber"></param>
        /// <returns>true（存在）/false（不存在）</returns>
        public bool ExistByCadastralNumber(string cadastralNumber)
        {
            if (!CheckTableExist())
            {
                throw new ArgumentNullException("数据库不存在表："
                    + this.GetType().ToString().Substring(this.GetType().ToString().LastIndexOf('.') + 1).Replace("Repository", ""));
            }
            if (!CheckRule.CheckStringNullOrEmpty(ref cadastralNumber))
                return false;

            return Count(c => c.CadastralNumber.Equals(cadastralNumber)) > 0;
        }

        #endregion

        #region ExtendMethod

        /// <summary>
        /// 获取地域下地块
        /// </summary>
        /// <param name="zoneCode">区域代码</param>
        /// <returns>地域下地块</returns>
        public AgricultureEquityCollection GetCollection(string zoneCode)
        {
            if (!CheckTableExist())
            {
                throw new ArgumentNullException("数据库不存在表："
                    + this.GetType().ToString().Substring(this.GetType().ToString().LastIndexOf('.') + 1).Replace("Repository", ""));
            }

            if (!CheckRule.CheckStringNullOrEmpty(ref zoneCode))
                return null;
            object entity = Get(c => c.LocationCode.Contains(zoneCode));
            return entity as AgricultureEquityCollection;
        }

        /// <summary>
        /// 根据地籍号获取对象
        /// </summary>
        /// <param name="number">地籍号</param>
        /// <returns>地块</returns>
        public AgricultureEquity Get(string number)
        {
            if (!CheckTableExist())
            {
                throw new ArgumentNullException("数据库不存在表："
                    + this.GetType().ToString().Substring(this.GetType().ToString().LastIndexOf('.') + 1).Replace("Repository", ""));
            }
            if (!CheckRule.CheckStringNullOrEmpty(ref number))
                return null;

            object entity = Get(c => c.CadastralNumber.Equals(number));

            return entity as AgricultureEquity;
        }

        /// <summary>
        /// 删除当前地域下所有地块
        /// </summary>
        /// <param name="zoneCode">地域代码</param>
        /// <returns>-1（参数错误）/0（失败）/1（成功）</returns>
        public int DeleteByZoneCode(string zoneCode)
        {
            if (!CheckTableExist())
            {
                throw new ArgumentNullException("数据库不存在表："
                    + this.GetType().ToString().Substring(this.GetType().ToString().LastIndexOf('.') + 1).Replace("Repository", ""));
            }
            if (!CheckRule.CheckStringNullOrEmpty(ref zoneCode))
                return -1;
            return Delete(c => c.LocationCode.Equals(zoneCode));
        }

        /// <summary>
        /// 按地域统计
        /// </summary>
        /// <param name="zoneCode">区域代码</param>
        /// <param name="searchOption">匹配等级</param>
        /// <returns>-1（参数错误）/int 对象数量</returns>
        public int Count(string zoneCode, eLevelOption searchOption)
        {
            if (!CheckTableExist())
            {
                throw new ArgumentNullException("数据库不存在表："
                    + this.GetType().ToString().Substring(this.GetType().ToString().LastIndexOf('.') + 1).Replace("Repository", ""));
            }
            if (!CheckRule.CheckStringNullOrEmpty(ref zoneCode))
                return -1;

            if (searchOption == eLevelOption.Self)
                return Count(c => c.LocationCode.Equals(zoneCode));
            else
                return Count(c => c.LocationCode.Contains(zoneCode));
        }

        /// <summary>
        /// 按地域删除地块
        /// </summary>
        /// <param name="zoneCode">地域代码</param>
        /// <param name="searchOption">匹配等级</param>
        /// <returns>-1（参数错误）/0（失败）/1（成功）</returns>
        public int DeleteByZoneCode(string zoneCode, eLevelOption searchOption)
        {
            if (!CheckTableExist())
            {
                throw new ArgumentNullException("数据库不存在表："
                    + this.GetType().ToString().Substring(this.GetType().ToString().LastIndexOf('.') + 1).Replace("Repository", ""));
            }
            if (!CheckRule.CheckStringNullOrEmpty(ref zoneCode))
                return -1;

            int cnt = 0;

            if (searchOption == eLevelOption.Self)
                cnt = Delete(c => c.LocationCode.Equals(zoneCode));
            else if (searchOption == eLevelOption.Subs)
                cnt = Delete(c => c.LocationCode.Contains(zoneCode));

            return cnt;
        }

        /// <summary>
        /// 按地域获取地块
        /// </summary>
        /// <param name="zoneCode">地域代码</param>
        /// <param name="searchOption">匹配等级</param>
        /// <returns>地块</returns>
        public AgricultureEquityCollection GetCollection(string zoneCode, eLevelOption searchOption)
        {
            if (!CheckTableExist())
            {
                throw new ArgumentNullException("数据库不存在表："
                    + this.GetType().ToString().Substring(this.GetType().ToString().LastIndexOf('.') + 1).Replace("Repository", ""));
            }
            if (!CheckRule.CheckStringNullOrEmpty(ref zoneCode))
                return null;

            object entity = null;

            if (searchOption == eLevelOption.Self)
                entity = Get(c => c.LocationCode.Equals(zoneCode));
            else
                entity = Get(c => c.LocationCode.Contains(zoneCode));

            return entity as AgricultureEquityCollection;
        }

        #endregion
    }
}