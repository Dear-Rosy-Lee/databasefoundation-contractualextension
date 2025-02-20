// (C) 2025 鱼鳞图公司版权所有，保留所有权利
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YuLinTu.Library.Entity;
using System.Data.SqlClient;
using YuLinTu.Data;
using YuLinTu;

namespace YuLinTu.Library.Repository
{
    /// <summary>
    /// 
    /// </summary>
    public partial class AgricultureAffairRepository : RepositoryDbContext<AgricultureAffair>, IAgricultureAffairRepository
    {
        #region Ctor

        private IDataSourceSchema m_DSSchema = null;

        public AgricultureAffairRepository(IDataSource ds)
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
            //    return m_DSSchema.AnyElement(m_DSSchema.GetElements()[0].Schema, "AgricultureAffair");

            //}
            //catch (Exception)
            //{
            //    return false;
            //}
            return true;
        }

        /// <summary>
        /// 统计数据
        /// </summary>
        /// <param name="zoneCode">区域代码</param>
        /// <param name="levelOption">匹配等级</param>
        /// <returns>对象数量</returns>
        public int Count(string zoneCode, eLevelOption levelOption)
        {
            if (!CheckTableExist())
            {
                throw new ArgumentNullException("数据库不存在表："
                    + this.GetType().ToString().Substring(this.GetType().ToString().LastIndexOf('.') + 1).Replace("Repository", ""));
            }

            if (!CheckRule.CheckStringNullOrEmpty(ref zoneCode))
                return 0;

            int count = 0;

            if (levelOption == eLevelOption.Self)
            {
                var q = from qA in DataSource.CreateQuery<AgricultureAffair>()
                        where qA.ZoneCode.Equals(zoneCode)
                        select qA;
                count = q.Count();
            }
            else if (levelOption == eLevelOption.Subs)
            {
                var q = from qA in DataSource.CreateQuery<AgricultureAffair>()
                        where qA.ZoneCode.StartsWith(zoneCode)
                        select qA;
                count = q.Count();
            }
            return count;
        }

        /// <summary>
        /// 获取对象
        /// </summary>
        /// <param name="idZone">id</param>
        /// <returns>对象</returns>
        public AgricultureAffair Get(Guid id)
        {
            if (!CheckTableExist())
            {
                throw new ArgumentNullException("数据库不存在表："
                            + this.GetType().ToString().Substring(this.GetType().ToString().LastIndexOf('.') + 1).Replace("Repository", ""));
            }
            if (!CheckTableExist())
            {
                return null;
            }

            if (!CheckRule.CheckGuidNullOrEmpty(id))
                return null;

            var data = from qA in DataSource.CreateQuery<AgricultureAffair>()
                       where qA.ID == id
                       select qA;
            return (AgricultureAffair)data;
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="zoneID">区域id</param>
        /// <returns>-1(参数错误)/0（失败）/1(成功)</returns>
        public int Delete(Guid id)
        {
            if (!CheckTableExist())
            {
                throw new ArgumentNullException("数据库不存在表："
                    + this.GetType().ToString().Substring(this.GetType().ToString().LastIndexOf('.') + 1).Replace("Repository", ""));
            }
            if (!CheckTableExist())
            {
                return -1;
            }
            if (!CheckRule.CheckGuidNullOrEmpty(id))
                return -1;
            return Delete(c => c.ID == id);
        }

        /// <summary>
        /// 根据地域删除数据
        /// </summary>
        /// <param name="zoneCode">区域代码</param>
        /// <param name="levelOption">匹配等级</param>
        /// <returns>0（失败）/1(成功)</returns>
        public int DeleteByZoneCode(string zoneCode, eLevelOption levelOption)
        {
            if (!CheckTableExist())
            {
                throw new ArgumentNullException("数据库不存在表："
                    + this.GetType().ToString().Substring(this.GetType().ToString().LastIndexOf('.') + 1).Replace("Repository", ""));
            }
            if (!CheckRule.CheckStringNullOrEmpty(ref zoneCode) || !CheckTableExist())
            {
                return -1;
            }
            int cnt = 0;
            if (levelOption == eLevelOption.Subs)
            {
                cnt = Delete(c => c.ZoneCode.StartsWith(zoneCode));
            }
            else
            {
                cnt = Delete(c => c.ZoneCode.Equals(zoneCode));
            }
            return cnt;
        }

        /// <summary>
        /// 更新数据
        /// </summary>
        /// <param name="affair">用来更新的对象</param>
        /// <returns>0（失败）/1(成功)</returns>
        public int Update(AgricultureAffair affair)
        {
            if (!CheckTableExist())
            {
                throw new ArgumentNullException("数据库不存在表："
                    + this.GetType().ToString().Substring(this.GetType().ToString().LastIndexOf('.') + 1).Replace("Repository", ""));
            }
            if (affair == null || affair.ID == Guid.Empty || !CheckTableExist())
            {
                return -1;
            } 
            int cnt = 0;
            cnt = Update(affair, c => c.ID == affair.ID);
            return cnt;
        }

        /// <summary>
        /// 判断是否存在
        /// </summary>
        /// <param name="zoneID"></param>
        /// <returns></returns>
        public bool Exists(Guid id)
        {
            if (!CheckTableExist())
            {
                throw new ArgumentNullException("数据库不存在表："
                    + this.GetType().ToString().Substring(this.GetType().ToString().LastIndexOf('.') + 1).Replace("Repository", ""));
            }
            if (!CheckTableExist())
            {
                return false;
            }

            if (!CheckRule.CheckGuidNullOrEmpty(id))
                return false;

            return Any(c => c.ID == id);
        }

        /// <summary>
        /// 根据名称搜索
        /// </summary>
        /// <param name="name">名称</param>
        /// <param name="searchOption">查找类型</param>
        /// <returns>对象集合</returns>
        public AgricultureAffairCollection SearchByName(string name, eSearchOption searchOption)
        {
            if (!CheckTableExist())
            {
                throw new ArgumentNullException("数据库不存在表："
                    + this.GetType().ToString().Substring(this.GetType().ToString().LastIndexOf('.') + 1).Replace("Repository", ""));
            }
            if (!CheckRule.CheckStringNullOrEmpty(ref name) || !CheckTableExist())
            {
                return null;
            }
            object data = null;

            if (searchOption == eSearchOption.Fuzzy)
                data = Get(c => c.Name.Contains(name));
            else
                data = Get(c => c.Name.Equals(name));


            return (AgricultureAffairCollection)data;
        }

        /// <summary>
        /// 根据名称与地域获取对象
        /// </summary>
        /// <param name="name">名称</param>
        /// <param name="zoneCode">区域代码</param>
        /// <param name="searchOption">查找类型</param>
        /// <returns>对象集合</returns>
        public AgricultureAffairCollection SearchByName(string name, string zoneCode, eSearchOption searchOption)
        {
            if (!CheckTableExist())
            {
                throw new ArgumentNullException("数据库不存在表："
                     + this.GetType().ToString().Substring(this.GetType().ToString().LastIndexOf('.') + 1).Replace("Repository", ""));
            }
            if (!CheckRule.CheckStringNullOrEmpty(ref name) || !CheckRule.CheckStringNullOrEmpty(ref zoneCode) || !CheckTableExist())
            {
                return null;
            }
            object data = null;
            if (searchOption == eSearchOption.Fuzzy)
                data = Get(c => c.Name.Contains(name) && c.ZoneCode.StartsWith(zoneCode));
            else
                data = Get(c => c.Name.Equals(name) && c.ZoneCode.StartsWith(zoneCode));
            return (AgricultureAffairCollection)data;
        }

        /// <summary>
        /// 根据地域编码获取对象
        /// </summary>
        /// <param name="zoneCode">区域代码</param>
        /// <param name="searchOption">查找类型</param>
        /// <returns>对象集合</returns>
        public AgricultureAffairCollection SearchByZoneCode(string zoneCode, eSearchOption searchOption)
        {
            if (!CheckTableExist())
            {
                throw new ArgumentNullException("数据库不存在表："
                    + this.GetType().ToString().Substring(this.GetType().ToString().LastIndexOf('.') + 1).Replace("Repository", ""));
            }
            if (!CheckRule.CheckStringNullOrEmpty(ref zoneCode) || !CheckTableExist())
            {
                return null;
            }
            object data = null;

            if (searchOption == eSearchOption.Fuzzy)
            {
                data = Get(c => c.ZoneCode.Contains(zoneCode));
            }
            else
            {
                data = Get(c => c.ZoneCode.Equals(zoneCode));
            }
            return (AgricultureAffairCollection)data;
        }

        #endregion
    }
}
