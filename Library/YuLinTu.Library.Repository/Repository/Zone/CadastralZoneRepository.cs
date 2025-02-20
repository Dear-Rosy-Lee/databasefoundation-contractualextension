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
    /// 地籍区的数据访问类
    /// </summary>
    public partial class CadastralZoneRepository : RepositoryDbContext<CadastralZone>,  ICadastralZoneRepository
    {
        #region Ctor

        
        private IDataSourceSchema m_DSSchema = null;
        public CadastralZoneRepository(IDataSource ds)
            : base(ds) 
        {
            new CadastralZone().ToString();
            LanguageAttribute.AddLanguage(Properties.Resources.langChs);
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
            //    return m_DSSchema.AnyElement(m_DSSchema.GetElements()[0].Schema, "CadastralZone");
            //}
            //catch (Exception)
            //{
            //    return false;
            //}
            return true;
        }

        /// <summary>
        /// 根据上级地域代码及其匹配等级统计区域等级为2的各类地域的数量
        /// </summary>
        /// <param name="zoneFullCodd">上级地域代码</param>
        /// <param name="levelOption">地域代码匹配等级</param>
        /// <returns>键（地域级别）值（地域数量）对</returns>
        public SortedList<eZoneLevel, int> StatZoneLevelCountByFullCode(string zoneFullCodd, eLevelOption levelOption)
        {
            if (!CheckTableExist())
            {
                throw new ArgumentNullException("数据库不存在表："
                    + this.GetType().ToString().Substring(this.GetType().ToString().LastIndexOf('.') + 1).Replace("Repository", ""));
            }
            if (!CheckRule.CheckStringNullOrEmpty(ref zoneFullCodd))
                return null;

            SortedList<eZoneLevel, int> statValue = new SortedList<eZoneLevel, int>();
            var q=DataSource.CreateQuery<Zone>();
            if (levelOption == eLevelOption.Subs)
            {
                if (!statValue.ContainsKey(eZoneLevel.Village))
                {
                    statValue.Add(eZoneLevel.Village, (from qz in q
                                                       where qz.Level.Equals(2) && qz.UpLevelCode.StartsWith(zoneFullCodd)
                                                       select new { level_CadastralSubRegion = q.Count() }
                                                ).FirstOrDefault().level_CadastralSubRegion);
                }
                if (!statValue.ContainsKey(eZoneLevel.Town))
                {
                    statValue.Add(eZoneLevel.Town, (from qz in q
                                                       where qz.Level.Equals(2) && qz.UpLevelCode.StartsWith(zoneFullCodd)
                                                    select new { level_CadastralRegion = q.Count() }
                                                ).FirstOrDefault().level_CadastralRegion);
                }
                if (!statValue.ContainsKey(eZoneLevel.County))
                {
                    statValue.Add(eZoneLevel.County, (from qz in q
                                                    where qz.Level.Equals(2) && qz.UpLevelCode.StartsWith(zoneFullCodd)
                                                    select new { level_County = q.Count() }
                                                ).FirstOrDefault().level_County);
                }
                if (!statValue.ContainsKey(eZoneLevel.City))
                {
                    statValue.Add(eZoneLevel.City, (from qz in q
                                                    where qz.Level.Equals(2) && qz.UpLevelCode.StartsWith(zoneFullCodd)
                                                    select new { level_City = q.Count() }
                                                ).FirstOrDefault().level_City);
                }
                if (!statValue.ContainsKey(eZoneLevel.Province))
                {
                    statValue.Add(eZoneLevel.Province, (from qz in q
                                                    where qz.Level.Equals(2) && qz.UpLevelCode.StartsWith(zoneFullCodd)
                                                        select new { level_Province = q.Count() }
                                                ).FirstOrDefault().level_Province);
                }
                if (!statValue.ContainsKey(eZoneLevel.State))
                {
                    statValue.Add(eZoneLevel.State, (from qz in q
                                                    where qz.Level.Equals(2) && qz.UpLevelCode.StartsWith(zoneFullCodd)
                                                     select new { level_State = q.Count() }
                                                ).FirstOrDefault().level_State);
                }
            }
            else
            {
                if (!statValue.ContainsKey(eZoneLevel.Village))
                {
                    statValue.Add(eZoneLevel.Village, (from qz in q
                                                       where qz.Level.Equals(2) && qz.UpLevelCode.Equals(zoneFullCodd)
                                                       select new { level_CadastralSubRegion = q.Count() }
                                                ).FirstOrDefault().level_CadastralSubRegion);
                }
                if (!statValue.ContainsKey(eZoneLevel.Town))
                {
                    statValue.Add(eZoneLevel.Town, (from qz in q
                                                    where qz.Level.Equals(2) && qz.UpLevelCode.Equals(zoneFullCodd)
                                                    select new { level_CadastralRegion = q.Count() }
                                                ).FirstOrDefault().level_CadastralRegion);
                }
                if (!statValue.ContainsKey(eZoneLevel.County))
                {
                    statValue.Add(eZoneLevel.County, (from qz in q
                                                    where qz.Level.Equals(2) && qz.UpLevelCode.Equals(zoneFullCodd)
                                                      select new { level_County = q.Count() }
                                                ).FirstOrDefault().level_County);
                }
                if (!statValue.ContainsKey(eZoneLevel.City))
                {
                    statValue.Add(eZoneLevel.City, (from qz in q
                                                    where qz.Level.Equals(2) && qz.UpLevelCode.Equals(zoneFullCodd)
                                                    select new { level_City = q.Count() }
                                                ).FirstOrDefault().level_City);
                }
                if (!statValue.ContainsKey(eZoneLevel.Province))
                {
                    statValue.Add(eZoneLevel.Province, (from qz in q
                                                    where qz.Level.Equals(2) && qz.UpLevelCode.Equals(zoneFullCodd)
                                                        select new { level_Province = q.Count() }
                                                ).FirstOrDefault().level_Province);
                }
                if (!statValue.ContainsKey(eZoneLevel.State))
                {
                    statValue.Add(eZoneLevel.State, (from qz in q
                                                    where qz.Level.Equals(2) && qz.UpLevelCode.Equals(zoneFullCodd)
                                                     select new { level_State = q.Count() }
                                                ).FirstOrDefault().level_State);
                }
            }
            
            return statValue;
        }

        /// <summary>
        /// 获得指定区域类指定地域级别的地域数量
        /// </summary>
        /// <param name="codeZone">地域编码</param>
        /// <param name="levelOption">地域编码匹配级别</param>
        /// <param name="level">地域级别</param>
        /// <returns>-1（参数错误）/int 对象数量</returns>
        public int Count(string codeZone, eLevelOption levelOption, eZoneLevel level)
        {
            if (!CheckTableExist())
            {
                throw new ArgumentNullException("数据库不存在表："
                    + this.GetType().ToString().Substring(this.GetType().ToString().LastIndexOf('.') + 1).Replace("Repository", ""));
            }
            if (!CheckRule.CheckStringNullOrEmpty(ref codeZone))
                return -1;
            int count = 0;
            if (levelOption == eLevelOption.Self)
                count = Count(c => c.FullCode.Equals(codeZone) && c.Level.Equals(level));
            else if (levelOption == eLevelOption.Subs)
                count = Count(c => c.FullCode.StartsWith(codeZone) && c.Level.Equals(level));
            return count;
        }

        /// <summary>
        /// 统计指定区域类地域数量
        /// </summary>
        /// <param name="codeZone">地域编码</param>
        /// <returns>-1（参数错误）/int 对象数量</returns>
        public int Count(string codeZone)
        {
            if (!CheckTableExist())
            {
                throw new ArgumentNullException("数据库不存在表："
                    + this.GetType().ToString().Substring(this.GetType().ToString().LastIndexOf('.') + 1).Replace("Repository", ""));
            }
            if (!CheckRule.CheckStringNullOrEmpty(ref codeZone))
                return -1;
            int count = 0;
            count = Count(c => c.FullCode.Equals(codeZone));
            return count;
        }

        /// <summary>
        /// 根据地域编码获得地籍区对象
        /// </summary>
        /// <param name="codeZone">地域编码</param>
        /// <returns>地籍区对象</returns>
        public CadastralZone Get(string codeZone)
        {
            if (!CheckTableExist())
            {
                throw new ArgumentNullException("数据库不存在表："
                    + this.GetType().ToString().Substring(this.GetType().ToString().LastIndexOf('.') + 1).Replace("Repository", ""));
            }
            if (!CheckRule.CheckStringNullOrEmpty(ref codeZone))
                return null;
            object data = Get(c => c.FullCode.Equals(codeZone));
            if (data != null)
                (data as CadastralZone).Name = (data as CadastralZone).Name.Trim();
            return (CadastralZone)data;
        }

        /// <summary>
        /// 根据地域标识码获得地籍区对象
        /// </summary>
        /// <param name="idZone">地域标识码</param>
        /// <returns>地籍区对象</returns>
        public CadastralZone Get(Guid idZone)
        {
            if (!CheckTableExist())
            {
                throw new ArgumentNullException("数据库不存在表："
                    + this.GetType().ToString().Substring(this.GetType().ToString().LastIndexOf('.') + 1).Replace("Repository", ""));
            }
            if (!CheckRule.CheckGuidNullOrEmpty(idZone))
                return null;
            object data = Get(c => c.ID.Equals(idZone));
            return (CadastralZone)data;
        }

        /// <summary>
        /// 根据地域等级获得地籍区对象
        /// </summary>
        /// <param name="level">地域等级</param>
        /// <returns>地籍区对象</returns>
        public CadastralList<Zone> GetByZoneLevel(eZoneLevel level)
        {
            if (!CheckTableExist())
            {
                throw new ArgumentNullException("数据库不存在表："
                    + this.GetType().ToString().Substring(this.GetType().ToString().LastIndexOf('.') + 1).Replace("Repository", ""));
            }
            object data = null;

            data = Get(c => c.Level.Equals(level));
            return (CadastralList<Zone>)data;
        }

        /// <summary>
        /// 获得指定区域下所有二级子区域
        /// </summary>
        /// <param name="codeZone">区域代码</param>
        /// <param name="levelOption">匹配等级</param>
        /// <returns>地籍区集合对象</returns>
        public CadastralList<Zone> GetChildren(string codeZone, eLevelOption levelOption)
        {
            if (!CheckTableExist())
            {
                throw new ArgumentNullException("数据库不存在表："
                    + this.GetType().ToString().Substring(this.GetType().ToString().LastIndexOf('.') + 1).Replace("Repository", ""));
            }
            if (!CheckRule.CheckStringNullOrEmpty(ref codeZone))
                return null;

            object data = null;

            if (levelOption == eLevelOption.Self)
                data = Get(c => c.UpLevelCode.Equals(codeZone));
            else if (levelOption == eLevelOption.Subs)
                data = Get(c => c.UpLevelCode.StartsWith(codeZone));
            return (CadastralList<Zone>)data;
        }

        /// <summary>
        /// 统计指定区域下所有二级子区域的数量
        /// </summary>
        /// <param name="codeZone">区域代码</param>
        /// <param name="levelOption">匹配等级</param>
        /// <returns>-1（参数错误）/int 对象数量</returns>
        public int Count(string codeZone, eLevelOption levelOption)
        {
            if (!CheckTableExist())
            {
                throw new ArgumentNullException("数据库不存在表："
                    + this.GetType().ToString().Substring(this.GetType().ToString().LastIndexOf('.') + 1).Replace("Repository", ""));
            }
            if (!CheckRule.CheckStringNullOrEmpty(ref codeZone))
                return -1;

            int cnt = 0;

            if (levelOption == eLevelOption.Self)
                cnt = Count(c => c.UpLevelCode.Equals(codeZone));
            else if (levelOption == eLevelOption.Subs)
                cnt = Count(c => c.UpLevelCode.StartsWith(codeZone));

            return cnt;
        }

        /// <summary>
        /// 根据地域ID删除地籍区对象
        /// </summary>
        /// <param name="zoneID">ID</param>
        /// <returns>-1（参数错误）/0（失败）/1（成功）</returns>
        public int Delete(Guid zoneID)
        {
            if (!CheckTableExist())
            {
                throw new ArgumentNullException("数据库不存在表："
                    + this.GetType().ToString().Substring(this.GetType().ToString().LastIndexOf('.') + 1).Replace("Repository", ""));
            }
            if (!CheckRule.CheckGuidNullOrEmpty(zoneID))
                return -1;
            int cnt = 0;
            cnt = Delete(c => c.ID.Equals(zoneID));
            return cnt;
        }

        /// <summary>
        /// 根据地域编码删除地籍区对象
        /// </summary>
        /// <param name="zoneCode">地域编码</param>
        /// <returns>-1（参数错误）/0（失败）/1（成功）</returns>
        public int DeleteByCode(string zoneCode)
        {
            if (!CheckTableExist())
            {
                throw new ArgumentNullException("数据库不存在表："
                    + this.GetType().ToString().Substring(this.GetType().ToString().LastIndexOf('.') + 1).Replace("Repository", ""));
            }
            if (!CheckRule.CheckStringNullOrEmpty(ref zoneCode))
                return -1;
            int cnt = 0;
            cnt = Delete(c => c.FullCode.Equals(zoneCode));
            return cnt;
        }

        /// <summary>
        /// 根据地域编码删除二级子地籍区对象
        /// </summary>
        /// <param name="zoneCode">地域编码</param>
        /// <param name="levelOption">匹配等级</param>
        /// <returns>-1（参数错误）/0（失败）/1（成功）</returns>
        public int DeleteByCode(string zoneCode, eLevelOption levelOption)
        {
            if (!CheckTableExist())
            {
                throw new ArgumentNullException("数据库不存在表："
                    + this.GetType().ToString().Substring(this.GetType().ToString().LastIndexOf('.') + 1).Replace("Repository", ""));
            }
            if (!CheckRule.CheckStringNullOrEmpty(ref zoneCode))
                return -1;

            int cnt = 0;
            if (levelOption == eLevelOption.Subs)
                cnt=Delete(c=>c.UpLevelCode.StartsWith(zoneCode));
            else
                cnt=Delete(c=>c.UpLevelCode.Equals(zoneCode));
            return cnt;
        }

        /// <summary>
        /// 更新地籍区对象
        /// </summary>
        /// <param name="zone">实体</param>
        /// <returns>-1（参数错误）/0（失败）/1（成功）</returns>
        public int Update(CadastralZone zone)
        {
            if (!CheckTableExist())
            {
                throw new ArgumentNullException("数据库不存在表："
                    + this.GetType().ToString().Substring(this.GetType().ToString().LastIndexOf('.') + 1).Replace("Repository", ""));
            }
            if (zone == null || zone.ID == Guid.Empty)
                return -1;

            int cnt = 0;
            cnt = Update(zone, c => c.ID.Equals(zone.ID));
            return cnt;
        }

        /// <summary>
        /// 根据地域编码判断地籍区对象是否存在
        /// </summary>
        /// <param name="zoneCode">地域编码</param>
        /// <returns>true（存在）/false（不存在）</returns>
        public bool Exists(string zoneCode)
        {
            if (!CheckTableExist())
            {
                throw new ArgumentNullException("数据库不存在表："
                    + this.GetType().ToString().Substring(this.GetType().ToString().LastIndexOf('.') + 1).Replace("Repository", ""));
            }
            if (!CheckRule.CheckStringNullOrEmpty(ref zoneCode))
                return false;
            return Count(c=>c.FullCode.Equals(zoneCode)) > 0;
        }

        /// <summary>
        /// 根据地域id判断地籍区对象是否存在
        /// </summary>
        /// <param name="zoneID">id</param>
        /// <returns>true（存在）/false（不存在）</returns>
        public bool Exists(Guid zoneID)
        {
            if (!CheckTableExist())
            {
                throw new ArgumentNullException("数据库不存在表："
                    + this.GetType().ToString().Substring(this.GetType().ToString().LastIndexOf('.') + 1).Replace("Repository", ""));
            }
            if (!CheckRule.CheckGuidNullOrEmpty(zoneID))
                return false;
            return Count(c=>c.ID.Equals(zoneID)) > 0;
        }

        /// <summary>
        /// 获得指定区域下所有指定地域级别的二级地域集合
        /// </summary>
        /// <param name="codeZone">区域代码</param>
        /// <param name="zoneLevel">地域级别</param>
        /// <returns>地域集合</returns>
        public CadastralList<Zone> GetByChildLevel(string codeZone, eZoneLevel zoneLevel)
        {
            if (!CheckTableExist())
            {
                throw new ArgumentNullException("数据库不存在表："
                    + this.GetType().ToString().Substring(this.GetType().ToString().LastIndexOf('.') + 1).Replace("Repository", ""));
            }
            if (!CheckRule.CheckStringNullOrEmpty(ref codeZone))
                return null;
            object data = null;

            if (string.IsNullOrEmpty(codeZone))
            {
                data = Get(c => c.Level.Equals(zoneLevel));
            }
            else
            {
                data = (from q in DataSource.CreateQuery<CadastralZone>()
                        where q.UpLevelCode.StartsWith(codeZone) && q.Level.Equals(zoneLevel)
                        orderby q.FullCode
                        select q).ToList();
            }
            return (CadastralList<Zone>)data;
        }

        /// <summary>
        /// 根据地域编码获得二级子地籍区
        /// </summary>
        /// <param name="zoneCode">地域编码</param>
        /// <param name="levelOption">地域编码匹配级别</param>
        /// <returns>二级子地籍区</returns>
        public CadastralList<Zone> GetZones(string zoneCode, eLevelOption levelOption)
        {
            if (!CheckTableExist())
            {
                throw new ArgumentNullException("数据库不存在表："
                    + this.GetType().ToString().Substring(this.GetType().ToString().LastIndexOf('.') + 1).Replace("Repository", ""));
            }

            if (!CheckRule.CheckStringNullOrEmpty(ref zoneCode))
                return null;

            object data = null;

            if (levelOption == eLevelOption.Self)
                data =(from q in DataSource.CreateQuery<CadastralZone>()
                        where q.UpLevelCode.Equals(zoneCode) 
                        orderby q.FullCode
                        select q).ToList();
            else if (levelOption == eLevelOption.Subs)
                data = (from q in DataSource.CreateQuery<CadastralZone>()
                        where q.UpLevelCode.StartsWith(zoneCode)
                        orderby q.FullCode
                        select q).ToList();
            return (CadastralList<Zone>)data;
        }

        /// <summary>
        /// 得到包含指定区域代码的地域集合
        /// </summary>
        /// <param name="codeZone">区域代码</param>
        /// <param name="levelOption">地域编码匹配级别</param>
        /// <returns>地域集合</returns>
        public CadastralList<Zone> GetSubZones(string codeZone, eLevelOption levelOption)
        {
            if (!CheckTableExist())
            {
                throw new ArgumentNullException("数据库不存在表："
                    + this.GetType().ToString().Substring(this.GetType().ToString().LastIndexOf('.') + 1).Replace("Repository", ""));
            }
            if (!CheckRule.CheckStringNullOrEmpty(ref codeZone))
                return null;
            object data = null;

            if (levelOption == eLevelOption.Self)
                data = Get(c => c.FullCode.Equals(codeZone));
            else if (levelOption == eLevelOption.Subs)
                data = Get(c => c.FullCode.StartsWith(codeZone));
            return (CadastralList<Zone>)data;
        }

        /// <summary>
        /// 根据地域名称获取地域集合
        /// </summary>
        /// <param name="zoneName">地域名称</param>
        /// <param name="searchType">查找类型</param>
        /// <returns>地域集合</returns>
        public CadastralList<Zone> SearchByName(string zoneName, eSearchOption searchType)
        {

            if (!CheckTableExist())
            {
                throw new ArgumentNullException("数据库不存在表："
                    + this.GetType().ToString().Substring(this.GetType().ToString().LastIndexOf('.') + 1).Replace("Repository", ""));
            }
            if (!CheckRule.CheckStringNullOrEmpty(ref zoneName))
                return null;

            object data = null;

            if (searchType == eSearchOption.Fuzzy)
                data = Get(c => c.Name.Contains(zoneName));
            else
                data = Get(c => c.Name.Equals(zoneName));
            return (CadastralList<Zone>)data;
        }

        /// <summary>
        /// 在指定地域下根据地域名称获取地域集合
        /// </summary>
        /// <param name="zoneName">地域名称</param>
        /// <param name="zoneCode">地域编码</param>
        /// <param name="searchType">地域名称查找类型</param>
        /// <returns>地域集合</returns>
        public CadastralList<Zone> SearchByName(string zoneName, string zoneCode, eSearchOption searchType)
        {

            if (!CheckTableExist())
            {
                throw new ArgumentNullException("数据库不存在表："
                    + this.GetType().ToString().Substring(this.GetType().ToString().LastIndexOf('.') + 1).Replace("Repository", ""));
            }
            if (!CheckRule.CheckStringNullOrEmpty(ref zoneName) || !CheckRule.CheckStringNullOrEmpty(ref zoneCode))
                return null;
            object data = null;
            if (searchType == eSearchOption.Fuzzy) 
            {
                data = Get(c => c.Name.Contains(zoneName) && c.FullCode.StartsWith(zoneCode));
            }
            else
            {
                data = Get(c => c.Name.Equals(zoneName) && c.FullCode.StartsWith(zoneCode));
            }

            return (CadastralList<Zone>)data;
        }

        /// <summary>
        /// 根据地域全称获取地域集合
        /// </summary>
        /// <param name="zoneName">地域全称</param>
        /// <param name="searchType">地域全称查找类型</param>
        /// <returns>地域集合</returns>
        public CadastralList<Zone> SearchByFullName(string zoneName, eSearchOption searchType)
        {
            if (!CheckTableExist())
            {
                throw new ArgumentNullException("数据库不存在表："
                    + this.GetType().ToString().Substring(this.GetType().ToString().LastIndexOf('.') + 1).Replace("Repository", ""));
            }
            if (!CheckRule.CheckStringNullOrEmpty(ref zoneName))
                return null;

            object data = null;

            if (searchType == eSearchOption.Fuzzy)
                data =Get(c=>c.FullName.Contains(zoneName));
            else
                data = Get(c => c.FullName.Equals(zoneName));

            return (CadastralList<Zone>)data;
        }

        /// <summary>
        /// 根据区域代码获得地域对象
        /// </summary>
        /// <param name="zoneCode">区域代码</param>
        /// <param name="searchType">区域代码查找类型</param>
        /// <returns>地域对象</returns>
        public CadastralList<Zone> SearchByCode(string zoneCode, eSearchOption searchType)
        {
            if (!CheckTableExist())
            {
                throw new ArgumentNullException("数据库不存在表："
                    + this.GetType().ToString().Substring(this.GetType().ToString().LastIndexOf('.') + 1).Replace("Repository", ""));
            }
            if (!CheckRule.CheckStringNullOrEmpty(ref zoneCode))
                return null;

            object data = null;

            if (searchType == eSearchOption.Fuzzy)
                 data =Get(c=>c.Code.Contains(zoneCode));
            else
                data = Get(c => c.Code.Equals(zoneCode));

            return (CadastralList<Zone>)data;
        }

        /// <summary>
        /// 根据地域全编码获得地域对象
        /// </summary>
        /// <param name="zoneFullCode">地域全编码</param>
        /// <param name="searchType">地域全编码查找类型</param>
        /// <returns>地域对象</returns>
        public CadastralList<Zone> SearchByFullCode(string zoneFullCode, eSearchOption searchType)
        {
            if (!CheckTableExist())
            {
                throw new ArgumentNullException("数据库不存在表："
                    + this.GetType().ToString().Substring(this.GetType().ToString().LastIndexOf('.') + 1).Replace("Repository", ""));
            }
            if (!CheckRule.CheckStringNullOrEmpty(ref zoneFullCode))
                return null;

            object data = null;

            if (searchType == eSearchOption.Fuzzy)
                data = Get(c => c.FullCode.Contains(zoneFullCode));
            else
                data = Get(c => c.FullCode.Equals(zoneFullCode));

            return (CadastralList<Zone>)data;
        }

        #endregion
    }
}
