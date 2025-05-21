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
    /// 提供对地域的数据访问类
    /// </summary>
    public partial class ZoneRepository : RepositoryDbContext<Zone>, IZoneRepository
    {
        #region Fields

        private const string ORDERFIELD_DEFAULT = "FullCode";

        #endregion

        #region Ctor


        static ZoneRepository()
        {
            new Zone().ToString();
            LanguageAttribute.AddLanguage(Properties.Resources.langChs);
        }

        private IDataSourceSchema m_DSSchema = null;

        public ZoneRepository(IDataSource ds)
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
            //    return m_DSSchema.AnyElement(m_DSSchema.GetElements()[0].Schema, typeof(Zone).GetAttribute<DataTableAttribute>().TableName);
            //}
            //catch (Exception)
            //{
            //    return false;
            //}
            return true;
        }

        /// <summary>
        /// 统计指定地域下二级地域各地域等级的地域数量
        /// </summary>
        /// <param name="zoneFullCodd">上级地域代码</param>
        /// <param name="levelOption">匹配等级</param>
        /// <returns>键（地域级别）值（地域数量）对</returns>
        public SortedList<eZoneLevel, int> StatZoneLevelCountByFullCode(string zoneFullCodd, eLevelOption levelOption)
        {
            //if (!CheckTableExist())
            //{
            //    throw new ArgumentNullException("数据库不存在表："
            //        + this.GetType().ToString().Substring(this.GetType().ToString().LastIndexOf('.') + 1).Replace("Repository", ""));
            //}
            if (!CheckRule.CheckStringNullOrEmpty(ref zoneFullCodd))
                return null;

            StringBuilder sqlString = new StringBuilder();

            //zoneFullCodd = levelOption == eLevelOption.AllSubLevel ? zoneFullCodd + "%" : zoneFullCodd;

            SortedList<eZoneLevel, int> statValue = new SortedList<eZoneLevel, int>();
            var qz = DataSource.CreateQuery<Zone>();
            if (levelOption == eLevelOption.Subs)
            {
                if (!statValue.ContainsKey(eZoneLevel.Group))
                    statValue.Add(eZoneLevel.Group, Count(c => c.Level.Equals(1) && c.UpLevelCode.StartsWith(zoneFullCodd)));
                if (!statValue.ContainsKey(eZoneLevel.Village))
                    statValue.Add(eZoneLevel.Village, Count(c => c.Level.Equals(2) && c.UpLevelCode.StartsWith(zoneFullCodd)));
                if (!statValue.ContainsKey(eZoneLevel.Town))
                    statValue.Add(eZoneLevel.Town, Count(c => c.Level.Equals(3) && c.UpLevelCode.StartsWith(zoneFullCodd)));
                if (!statValue.ContainsKey(eZoneLevel.County))
                    statValue.Add(eZoneLevel.County, Count(c => c.Level.Equals(4) && c.UpLevelCode.StartsWith(zoneFullCodd)));
                if (!statValue.ContainsKey(eZoneLevel.City))
                    statValue.Add(eZoneLevel.City, Count(c => c.Level.Equals(5) && c.UpLevelCode.StartsWith(zoneFullCodd)));
                if (!statValue.ContainsKey(eZoneLevel.Province))
                    statValue.Add(eZoneLevel.Province, Count(c => c.Level.Equals(6) && c.UpLevelCode.StartsWith(zoneFullCodd)));
                if (!statValue.ContainsKey(eZoneLevel.State))
                    statValue.Add(eZoneLevel.State, Count(c => c.Level.Equals(7) && c.UpLevelCode.StartsWith(zoneFullCodd)));
            }
            else
            {
                if (!statValue.ContainsKey(eZoneLevel.Group))
                    statValue.Add(eZoneLevel.Group, Count(c => c.Level.Equals(1) && c.UpLevelCode.Equals(zoneFullCodd)));
                if (!statValue.ContainsKey(eZoneLevel.Village))
                    statValue.Add(eZoneLevel.Village, Count(c => c.Level.Equals(2) && c.UpLevelCode.Equals(zoneFullCodd)));
                if (!statValue.ContainsKey(eZoneLevel.Town))
                    statValue.Add(eZoneLevel.Town, Count(c => c.Level.Equals(3) && c.UpLevelCode.Equals(zoneFullCodd)));
                if (!statValue.ContainsKey(eZoneLevel.County))
                    statValue.Add(eZoneLevel.County, Count(c => c.Level.Equals(4) && c.UpLevelCode.Equals(zoneFullCodd)));
                if (!statValue.ContainsKey(eZoneLevel.City))
                    statValue.Add(eZoneLevel.City, Count(c => c.Level.Equals(5) && c.UpLevelCode.Equals(zoneFullCodd)));
                if (!statValue.ContainsKey(eZoneLevel.Province))
                    statValue.Add(eZoneLevel.Province, Count(c => c.Level.Equals(6) && c.UpLevelCode.Equals(zoneFullCodd)));
                if (!statValue.ContainsKey(eZoneLevel.State))
                    statValue.Add(eZoneLevel.State, Count(c => c.Level.Equals(7) && c.UpLevelCode.Equals(zoneFullCodd)));
            }
            return statValue;
        }

        /// <summary>
        /// 统计指定区域指定地域级别的地域数量
        /// </summary>
        /// <param name="codeZone">地域全编码</param>
        /// <param name="levelOption">地域全编码匹配等级</param>
        /// <param name="level">地域等级</param>
        /// <returns>-1（参数错误）/int 地域数量</returns>
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
        /// 根据地域全编码获得地域
        /// </summary>
        /// <param name="codeZone">地域全编码</param>
        /// <returns>地域</returns>
        public Zone Get(string codeZone)
        {
            if (!CheckTableExist())
            {
                throw new ArgumentNullException("数据库不存在表："
                    + this.GetType().ToString().Substring(this.GetType().ToString().LastIndexOf('.') + 1).Replace("Repository", ""));
            }
            if (!CheckRule.CheckStringNullOrEmpty(ref codeZone))
                return null;

            var data = Get(c => c.FullCode.Equals(codeZone)).FirstOrDefault();
            if (data != null)
                (data as Zone).Name = (data as Zone).Name.Trim();
            return (Zone)data;
        }

        /// <summary>
        /// 根据地域id获得地域
        /// </summary>
        /// <param name="idZone">id</param>
        /// <returns>地域</returns>
        public Zone Get(Guid idZone)
        {
            if (!CheckTableExist())
            {
                throw new ArgumentNullException("数据库不存在表："
                    + this.GetType().ToString().Substring(this.GetType().ToString().LastIndexOf('.') + 1).Replace("Repository", ""));
            }
            if (idZone == Guid.Empty || idZone == null)
                return null;

            object data = Get(c => c.ID.Equals(idZone)).FirstOrDefault();

            return (Zone)data;
        }

        /// <summary>
        /// 根据地域级别获得地域对象
        /// </summary>
        /// <param name="level">地域级别</param>
        /// <returns>地域对象</returns>
        public List<Zone> GetByZoneLevel(eZoneLevel level)
        {
            if (!CheckTableExist())
            {
                throw new ArgumentNullException("数据库不存在表："
                    + this.GetType().ToString().Substring(this.GetType().ToString().LastIndexOf('.') + 1).Replace("Repository", ""));
            }
            object data = null;

            data = (from q in DataSource.CreateQuery<Zone>()
                    where q.Level.Equals(level)
                    orderby q.FullCode
                    select q).ToList();
            return (List<Zone>)data;
        }

        /// <summary>
        /// 得到指定区域下所有二级子地域集合
        /// </summary>
        /// <param name="codeZone">上级地域代码</param>
        /// <param name="levelOption">地域代码匹配级别</param>
        /// <returns>地域集合</returns>
        public List<Zone> GetChildren(string codeZone, eLevelOption levelOption)
        {
            if (!CheckTableExist())
            {
                throw new ArgumentNullException("数据库不存在表："
                    + this.GetType().ToString().Substring(this.GetType().ToString().LastIndexOf('.') + 1).Replace("Repository", ""));
            }
            if (!CheckRule.CheckStringNullOrEmpty(ref codeZone))
                return null;

            List<Zone> data = null;

            if (levelOption == eLevelOption.Self)
            {
                data = (from q in DataSource.CreateQuery<Zone>()
                        where q.UpLevelCode.Equals(codeZone)
                        orderby q.FullCode
                        select q).ToList();
            }
            else if (levelOption == eLevelOption.Subs)
            {
                data = (from q in DataSource.CreateQuery<Zone>()
                        where q.UpLevelCode.StartsWith(codeZone)
                        orderby q.FullCode
                        select q).ToList();
            }
            else if (levelOption == eLevelOption.SelfAndSubs)
            {
                data = (from q in DataSource.CreateQuery<Zone>()
                        where q.FullCode.StartsWith(codeZone)
                        orderby q.FullCode
                        select q).ToList();
            }
            List<Zone> collection = new List<Zone>();
            if (data != null && data.Count > 0)
            {
                data.ForEach(t => collection.Add(t));
            }
            return collection;
        }
        /// <summary>
        /// 获取与目标图形相交的最大面积的地域
        /// </summary>
        /// <returns></returns>
        public Zone GetGeoIntersectZoneOFMaxAea(YuLinTu.Spatial.Geometry targetGeo)
        {
            Zone returnZone = null;
            if (targetGeo == null || targetGeo.IsValid() == false)
            {
                return returnZone;
            }

            var dataQuery = DataSource.CreateQuery<Zone>();
            var datas = dataQuery.Where(zz => zz.Shape != null && zz.Shape.Intersects(targetGeo) && zz.Level <= eZoneLevel.Village).ToList();

            var groupDatas = datas.FindAll(dd => dd.Level == eZoneLevel.Group);
            var villageDatas = datas.FindAll(dd => dd.Level == eZoneLevel.Village);
            if (groupDatas != null && groupDatas.Count > 0)
            {
                returnZone = groupDatas.OrderByDescending(g => targetGeo.Intersection(g.Shape).Area()).First();
            }
            else if (villageDatas != null && villageDatas.Count > 0)
            {
                returnZone = villageDatas.OrderByDescending(g => targetGeo.Intersection(g.Shape).Area()).First();
            }

            return returnZone;
        }

        /// <summary>
        /// 统计指定区域下所有二级子地域数量
        /// </summary>
        /// <param name="codeZone">上级地域代码</param>
        /// <param name="levelOption">地域代码匹配级别</param>
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
                cnt = Count(c => c.UpLevelCode.StartsWith(codeZone) && c.FullCode != codeZone);
            else
                cnt = Count(c => c.UpLevelCode.StartsWith(codeZone));
            return cnt;
        }

        /// <summary>
        /// 根据地域编码获得指定地域级别的上级地域
        /// </summary>
        /// <param name="codeZone">地域编码</param>
        /// <param name="level">地域级别</param>
        /// <returns>上级地域</returns>
        public Zone GetUpZone(string codeZone, eZoneLevel level)
        {
            if (!CheckTableExist())
            {
                throw new ArgumentNullException("数据库不存在表："
                    + this.GetType().ToString().Substring(this.GetType().ToString().LastIndexOf('.') + 1).Replace("Repository", ""));
            }
            if (!CheckRule.CheckStringNullOrEmpty(ref codeZone))
                return null;

            Zone zone = Get(codeZone);
            if (zone.Level != level)
                return (GetUpZone(zone.UpLevelCode, level));
            return zone;
        }

        /// <summary>
        /// 根据区域id删除区域对象
        /// </summary>
        /// <param name="zoneID">id</param>
        /// <returns>-1（参数错误）/0（失败）/1（成功）</retuns>
        public int Delete(Guid zoneID)
        {
            if (!CheckTableExist())
            {
                throw new ArgumentNullException("数据库不存在表："
                    + this.GetType().ToString().Substring(this.GetType().ToString().LastIndexOf('.') + 1).Replace("Repository", ""));
            }
            if (zoneID == Guid.Empty || zoneID == null)
                return 0;
            int cnt = 0;
            cnt = Delete(c => c.ID.Equals(zoneID));
            return cnt;
        }

        /// <summary>
        /// 根据地域全编码删除地域
        /// </summary>
        /// <param name="zoneCode">地域全编码</param>
        /// <returns>-1（参数错误）/0（失败）/1（成功）</returns>
        public int DeleteByCode(string zoneCode)
        {
            if (!CheckTableExist())
            {
                throw new ArgumentNullException("数据库不存在表："
                    + this.GetType().ToString().Substring(this.GetType().ToString().LastIndexOf('.') + 1).Replace("Repository", ""));
            }
            if (!CheckRule.CheckStringNullOrEmpty(ref zoneCode))
                return 0;

            int cnt = 0;
            cnt = Delete(c => c.FullCode.Equals(zoneCode));
            return cnt;
        }

        /// <summary>
        /// 根据地域编码删除其二级子地域
        /// </summary>
        /// <param name="zoneCode">地域编码</param>
        /// <param name="levelOption">地域编码匹配等级</param>
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
            if (levelOption == eLevelOption.Self)
                cnt = Delete(c => c.UpLevelCode.StartsWith(zoneCode));
            else
                cnt = Delete(c => c.Equals(zoneCode));

            return cnt;
        }

        /// <summary>
        /// 根据id更新地域对象
        /// </summary>
        /// <param name="zone">实体</param>
        /// <returns>-1（参数错误）/0（失败）/1（成功）</returns>
        public int Update(Zone zone)
        {
            //if (!CheckTableExist())
            //{
            //    throw new ArgumentNullException("数据库不存在表："
            //        + this.GetType().ToString().Substring(this.GetType().ToString().LastIndexOf('.') + 1).Replace("Repository", ""));
            //}
            if (zone == null || zone.ID == Guid.Empty)
                return -1;

            int cnt = 0;
            cnt = Update(zone, c => c.ID == zone.ID);
            return cnt;
        }

        public int UpdateCodeName(Zone zone)
        {
            if (zone == null || zone.ID == Guid.Empty)
                return -1;

            return AppendEdit(DataSource.CreateQuery<Zone>().Where(c => c.ID == zone.ID).
                Update(c => new Zone
                {
                    Name = zone.Name,
                    Code = zone.Code,
                    FullCode = zone.FullCode,
                    FullName = zone.FullName,
                    UpLevelCode = zone.UpLevelCode,
                    UpLevelName = zone.UpLevelName,
                    LastModifyTime = DateTime.Now
                }));
        }

        /// <summary>
        /// 根据地域全编码更新地域对象
        /// </summary>
        /// <param name="zone">地域实体</param>
        /// <returns>-1（参数错误）/0（失败）/1（成功）</returns>
        public int UpdateByFullCode(Zone zone)
        {
            if (!CheckTableExist())
            {
                throw new ArgumentNullException("数据库不存在表："
                    + this.GetType().ToString().Substring(this.GetType().ToString().LastIndexOf('.') + 1).Replace("Repository", ""));
            }
            if (zone == null || zone.ID == Guid.Empty)
                return -1;

            int cnt = 0;
            cnt = Update(zone, c => c.FullCode.Equals(zone.FullCode));
            return cnt;
        }

        /// <summary>
        /// 根据指定的地域编码判断地域是否存在
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

            return Count(c => c.FullCode.Equals(zoneCode)) > 0;
        }

        /// <summary>
        /// 根据区域id确定地域对象是否存在
        /// </summary>
        /// <param name="zoneID">区域id</param>
        /// <returns>true（存在）/false（不存在）</returns>
        public bool Exists(Guid zoneID)
        {
            if (!CheckTableExist())
            {
                throw new ArgumentNullException("数据库不存在表："
                    + this.GetType().ToString().Substring(this.GetType().ToString().LastIndexOf('.') + 1).Replace("Repository", ""));
            }
            if (zoneID == null || zoneID == Guid.Empty)
                return false;
            return Count(c => c.ID.Equals(zoneID)) > 0;
        }

        /// <summary>
        /// 获取指定区域下指定地域等级的地域
        /// </summary>
        /// <param name="codeZone">地域代码</param>
        /// <param name="levelOption">地域等级</param>
        /// <returns>地域</returns>
        public List<Zone> GetByChildLevel(string codeZone, eZoneLevel zoneLevel)
        {
            if (!CheckTableExist())
            {
                throw new ArgumentNullException("数据库不存在表："
                    + this.GetType().ToString().Substring(this.GetType().ToString().LastIndexOf('.') + 1).Replace("Repository", ""));
            }
            if (string.IsNullOrEmpty(codeZone))
                return null;

            List<Zone> data = null;

            if (string.IsNullOrEmpty(codeZone))
                data = (from q in DataSource.CreateQuery<Zone>()
                        where q.Level.Equals(zoneLevel)
                        orderby q.FullCode
                        select q).ToList();
            else
                data = (from q in DataSource.CreateQuery<Zone>()
                        where q.Level.Equals(zoneLevel) && q.UpLevelCode.StartsWith(codeZone)
                        orderby q.FullCode
                        select q).ToList();
            List<Zone> collection = new List<Zone>();
            if (data != null && data.Count > 0)
            {
                data.ForEach(t => collection.Add(t));
            }
            return collection;
        }

        /// <summary>
        /// 得到指定区域所有二级子区域
        /// </summary>
        /// <param name="zoneCode">区域代码</param>
        /// <param name="levelOption">地域代码匹配等级</param>
        /// <returns>区域对象</returns>
        public List<Zone> GetZones(string zoneCode, eLevelOption levelOption)
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
                data = (from q in DataSource.CreateQuery<Zone>()
                        where q.UpLevelCode.Equals(zoneCode)
                        orderby q.FullCode
                        select q).ToList();
            else if (levelOption == eLevelOption.Subs)
                data = (from q in DataSource.CreateQuery<Zone>()
                        where q.UpLevelCode.StartsWith(zoneCode)
                        orderby q.FullCode
                        select q).ToList();

            return (List<Zone>)data;
        }

        /// <summary>
        /// 根据地域编码获取地域
        /// </summary>
        /// <param name="codeZone">地域编码</param>
        /// <param name="levelOption">地域代码匹配等级</param>
        /// <returns>地域对象</returns>
        public List<Zone> GetSubZones(string codeZone, eLevelOption levelOption)
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
                data = (from q in DataSource.CreateQuery<Zone>()
                        where q.FullCode.Equals(codeZone)
                        orderby q.FullCode
                        select q).ToList();
            else if (levelOption == eLevelOption.Subs)
                data = (from q in DataSource.CreateQuery<Zone>()
                        where q.FullCode.StartsWith(codeZone) && q.FullCode != codeZone
                        orderby q.FullCode
                        select q).ToList();
            else
                data = (from q in DataSource.CreateQuery<Zone>()
                        where q.FullCode.StartsWith(codeZone)
                        orderby q.FullCode
                        select q).ToList();

            return (List<Zone>)data;
        }

        /// <summary>
        /// 根据地域名称及其查找类型获取地域对象集合
        /// </summary>
        /// <param name="zoneName">地域名称</param>
        /// <param name="searchType">查找类型</param>
        /// <returns>地域对象集合</returns>
        public List<Zone> SearchByName(string zoneName, eSearchOption searchType)
        {
            if (!CheckTableExist())
            {
                throw new ArgumentNullException("数据库不存在表："
                    + this.GetType().ToString().Substring(this.GetType().ToString().LastIndexOf('.') + 1).Replace("Repository", ""));
            }
            if (!CheckRule.CheckStringNullOrEmpty(ref zoneName))
                return null;

            zoneName = zoneName.Trim();
            if (zoneName == string.Empty)
                return null;

            object data = null;

            if (searchType == eSearchOption.Fuzzy)
                data = Get(c => c.Name.Contains(zoneName));
            else
                data = Get(c => c.Name.Equals(zoneName));

            return (List<Zone>)data;
        }

        /// <summary>
        /// 根据指定的地域名称、地域代码及其查找类型获得区域对象集合
        /// </summary>
        /// <param name="zoneName">区域名称</param>
        /// <param name="zoneCode">地域代码</param>
        /// <param name="searchType">查找类型</param>
        /// <returns>区域对象集合</returns>
        public List<Zone> SearchByName(string zoneName, string zoneCode, eSearchOption searchType)
        {
            if (!CheckTableExist())
            {
                throw new ArgumentNullException("数据库不存在表："
                    + this.GetType().ToString().Substring(this.GetType().ToString().LastIndexOf('.') + 1).Replace("Repository", ""));
            }
            if (!CheckRule.CheckStringNullOrEmpty(ref zoneName) || !CheckRule.CheckStringNullOrEmpty(ref zoneCode))
            {
                return null;
            }
            object data = null;
            if (searchType == eSearchOption.Fuzzy)
                data = Get(q => q.Name.Contains(zoneName) && q.FullCode.Contains(zoneCode));
            else
                data = Get(q => q.Name.Equals(zoneName) && q.FullCode.Contains(zoneCode));
            return (List<Zone>)data;
        }

        /// <summary>
        /// 根据地域全称及其查找类型获得地域对象
        /// </summary>
        /// <param name="zoneName">地域全称</param>
        /// <param name="searchType">查找类型</param>
        /// <returns>地域对象</returns>
        public List<Zone> SearchByFullName(string zoneName, eSearchOption searchType)
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
                data = Get(c => c.FullName.Contains(zoneName));
            else
                data = Get(c => c.FullName.Equals(zoneName));

            return (List<Zone>)data;
        }

        /// <summary>
        /// 根据地域编码及其查找类型获得地域对象
        /// </summary>
        /// <param name="zoneCode">地域编码</param>
        /// <param name="searchType">查找类型</param>
        /// <returns>地域对象</returns>
        public List<Zone> SearchByCode(string zoneCode, eSearchOption searchType)
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
                data = Get(c => c.Code.Contains(zoneCode));
            else
                data = Get(c => c.Code.Equals(zoneCode));

            return (List<Zone>)data;
        }

        /// <summary>
        /// 根据地域全编码及其查找类型获得地域对象
        /// </summary>
        /// <param name="zoneFullCode">地域全编码</param>
        /// <param name="searchType">查找类型</param>
        /// <returns>地域对象</returns>
        public List<Zone> SearchByFullCode(string zoneFullCode, eSearchOption searchType)
        {
            if (!CheckTableExist())
            {
                throw new ArgumentNullException("数据库不存在表："
                    + this.GetType().ToString().Substring(this.GetType().ToString().LastIndexOf('.') + 1).Replace("Repository", ""));
            }
            if (!CheckRule.CheckStringNullOrEmpty(ref zoneFullCode))
                return null;

            zoneFullCode = zoneFullCode.Trim();
            if (zoneFullCode == string.Empty)
                return null;

            object data = null;

            if (searchType == eSearchOption.Fuzzy)
                data = Get(c => c.FullCode.Contains(zoneFullCode));
            else
                data = Get(c => c.FullCode.Equals(zoneFullCode));

            return (List<Zone>)data;
        }

        /// <summary>
        /// 根据指定的上级地域编码判断当前加入的地域名称是否存在
        /// </summary>
        /// <param name="upLevelCode">上级地域编码</param>
        /// <param name="zone">当前添加地域</param>
        public bool ExistName(string upLevelCode, Zone zone)
        {
            if (!CheckTableExist())
            {
                throw new ArgumentNullException("数据库不存在表："
                    + this.GetType().ToString().Substring(this.GetType().ToString().LastIndexOf('.') + 1).Replace("Repository", ""));
            }
            if (!CheckRule.CheckStringNullOrEmpty(ref upLevelCode))
                return false;
            int count = Count(c => c.UpLevelCode.Equals(upLevelCode) && c.Level.Equals(zone.Level) && c.Name.Equals(zone.Name) && c.ID != zone.ID);
            return count > 0 ? true : false;
        }

        /// <summary>
        /// 获取最大行政地域
        /// </summary>
        /// <returns></returns>
        public List<Zone> GetMaxLevelZone()
        {
            var zones = from zone in DataSource.CreateQuery<Zone>() select zone;
            var maxLevel = zones.Max(t => t.Level);
            var maxzone = from z in zones
                          where z.Level.Equals(maxLevel)
                          select z;
            return maxzone.ToList();
        }

        #endregion
    }
}
