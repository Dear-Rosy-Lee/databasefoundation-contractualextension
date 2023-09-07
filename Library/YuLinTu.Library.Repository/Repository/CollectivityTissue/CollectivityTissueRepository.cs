// (C) 2015 鱼鳞图公司版权所有，保留所有权利
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YuLinTu.Library.Entity;
using System.Data;
using System.Data.SqlClient;
using YuLinTu.Data;
using YuLinTu;

namespace YuLinTu.Library.Repository
{
    /// <summary>
    /// 集体经济组织的数据访问类
    /// </summary>
    public class CollectivityTissueRepository : RepositoryDbContext<CollectivityTissue>, ICollectivityTissueRepository
    {
        #region Ctor

        private IDataSourceSchema m_DSSchema = null;

        public CollectivityTissueRepository(IDataSource ds)
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
            //    return m_DSSchema.AnyElement(m_DSSchema.GetElements()[0].Schema, typeof(CollectivityTissue).GetAttribute<DataTableAttribute>().TableName);
            //}
            //catch (Exception)
            //{
            //    return false;
            //}

            return true;
        }

        /// <summary>
        /// 获取与目标集体经济组织名称相同的集体经济组织对象
        /// </summary>
        /// <param name="name">集体经济组织名称</param>
        /// <returns>集体经济组织对象</returns>
        public CollectivityTissue Get(string name)
        {
            if (!CheckTableExist())
            {
                throw new ArgumentNullException("数据库不存在表："
                    + this.GetType().ToString().Substring(this.GetType().ToString().LastIndexOf('.') + 1).Replace("Repository", ""));
            }
            if (!CheckRule.CheckStringNullOrEmpty(ref name))
                return null;

            object data = Get(c => c.Name.Equals(name));

            return (CollectivityTissue)data;
        }

        /// <summary>
        /// 获取与集体经济组织编码相同的集体经济组织对象
        /// </summary>
        /// <param name="guid">集体经济组织编码</param>
        /// <returns>集体经济组织对象</returns>
        public CollectivityTissue Get(Guid guid)
        {
            if (!CheckTableExist())
            {
                throw new ArgumentNullException("数据库不存在表："
                    + this.GetType().ToString().Substring(this.GetType().ToString().LastIndexOf('.') + 1).Replace("Repository", ""));
            }
            if (!CheckRule.CheckGuidNullOrEmpty(guid))
                return null;

            CollectivityTissue ret;
            try
            {
                ret = Get(c => c.ID.Equals(guid)).FirstOrDefault();
            }
            catch (Exception)
            {
                ret = null;                
            }
            return ret;
        }

        /// <summary>
        /// 是否存在与组织编码相同、与集体经济组织编码不同的集体经济组织对象
        /// </summary>
        /// <param name="code">组织编码</param>
        /// <param name="tissueId">集体经济组织编码</param>
        /// <returns>true（存在）/false（不存在）</returns>
        public bool CodeExists(string code, Guid tissueId)
        {
            if (!CheckTableExist())
            {
                throw new ArgumentNullException("数据库不存在表："
                    + this.GetType().ToString().Substring(this.GetType().ToString().LastIndexOf('.') + 1).Replace("Repository", ""));
            }
            if (!CheckRule.CheckStringNullOrEmpty(ref code) || !CheckRule.CheckGuidNullOrEmpty(tissueId))
                return false;

            object entity = Get(c => c.Code.Equals(code) && (!c.ID.Equals(tissueId)));

            if (entity == null)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// 根据区域代码获得最大的组织编码
        /// </summary>
        /// <param name="codeZone">区域代码</param>
        /// <returns>最大的组织编码</returns>
        public int GetValidCodeByZone(string codeZone)
        {
            if (!CheckTableExist())
            {
                throw new ArgumentNullException("数据库不存在表："
                    + this.GetType().ToString().Substring(this.GetType().ToString().LastIndexOf('.') + 1).Replace("Repository", ""));
            }
            return Get(c => c.Code.Equals(codeZone)).Max(c => int.Parse(c.Code));
            //object data = Provider.ExecuteNonQueryProcedure("Proc_GetMaxTissueNumber",
            //                                                new string[] { "Code", "OutCode" },
            //                                                new object[] { codeZone, 0 },
            //                                                new object[] { DbType.String, DbType.Int32 },
            //                                                new ParameterDirection[] { ParameterDirection.Input, ParameterDirection.Output });

            //return (int)(data as object[])[1];
        }

        /// <summary>
        /// 根据目标集体经济组织编码判断集体经济组织对象是否存在
        /// </summary>
        /// <param name="guid">目标集体经济组织编码</param>
        /// <returns>true（存在）/false（不存在）</returns>
        public bool Exists(Guid guid)
        {
            if (!CheckTableExist())
            {
                throw new ArgumentNullException("数据库不存在表："
                    + this.GetType().ToString().Substring(this.GetType().ToString().LastIndexOf('.') + 1).Replace("Repository", ""));
            }
            if (!CheckRule.CheckGuidNullOrEmpty(guid))
                return false;
            return Count(c => c.ID.Equals(guid)) > 0;
        }

        /// <summary>
        /// 根据地域编码判断集体经济组织对象是否存在
        /// </summary>
        /// <param name="guid">地域编码</param>
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
            return Count(c => c.ZoneCode.Equals(zoneCode)) > 0;
        }

        public bool Exists(string zoneCode, string tissueName)
        {
            if (!CheckTableExist())
            {
                throw new ArgumentNullException("数据库不存在表："
                    + this.GetType().ToString().Substring(this.GetType().ToString().LastIndexOf('.') + 1).Replace("Repository", ""));
            }
            return Count(c => c.Name.Equals(tissueName) && c.ZoneCode.Equals(zoneCode)) > 0;
        }

        /// <summary>
        /// 获取指定区域下指定集体经济组织名称的集体经济组织对象
        /// </summary>
        /// <param name="zoneCode">区域代码</param>
        /// <param name="name">集体经济组织名称</param>
        /// <returns>集体经济组织对象</returns>
        public CollectivityTissue Get(string zoneCode, string name)
        {
            if (!CheckTableExist())
            {
                throw new ArgumentNullException("数据库不存在表："
                    + this.GetType().ToString().Substring(this.GetType().ToString().LastIndexOf('.') + 1).Replace("Repository", ""));
            }
            if (!CheckRule.CheckStringNullOrEmpty(ref zoneCode) || !CheckRule.CheckStringNullOrEmpty(ref name))
                return null;
            return Get(c => c.Name.Equals(name) && c.ZoneCode.Equals(zoneCode)).FirstOrDefault();
        }

        /// <summary>
        /// 是否存在与组织编码相同的集体经济组织对象
        /// </summary>
        /// <param name="code">组织编码</param>
        /// <returns>true（存在）/false（不存在）</returns>
        public bool CodeExists(string code)
        {
            if (!CheckTableExist())
            {
                throw new ArgumentNullException("数据库不存在表："
                    + this.GetType().ToString().Substring(this.GetType().ToString().LastIndexOf('.') + 1).Replace("Repository", ""));
            }
            if (!CheckRule.CheckStringNullOrEmpty(ref code))
                return false;
            return Count(c => c.Code.Equals(code)) > 0;
        }

        /// <summary>
        /// 是否存在与集体经济组织名称相同的集体经济组织对象
        /// </summary>
        /// <param name="name">集体经济组织名称</param>
        /// <returns>true（存在）/false（不存在）</returns>
        public bool NameExists(string name)
        {
            if (!CheckTableExist())
            {
                throw new ArgumentNullException("数据库不存在表："
                    + this.GetType().ToString().Substring(this.GetType().ToString().LastIndexOf('.') + 1).Replace("Repository", ""));
            }
            if (!CheckRule.CheckStringNullOrEmpty(ref name))
                return false;
            return Count(c => c.Name.Equals(name)) > 0;
        }

        /// <summary>
        /// 是否存在与集体经济组织名称相同、与集体经济组织编码不同的集体经济组织对象
        /// </summary>
        /// <param name="name">集体经济组织名称</param>
        /// <param name="tissueId">集体经济组织编码</param>
        /// <returns>true（存在）/false（不存在）</returns>
        public bool NameExists(string name, Guid tissueId)
        {
            if (!CheckTableExist())
            {
                throw new ArgumentNullException("数据库不存在表："
                    + this.GetType().ToString().Substring(this.GetType().ToString().LastIndexOf('.') + 1).Replace("Repository", ""));
            }
            if (!CheckRule.CheckStringNullOrEmpty(ref name) || !CheckRule.CheckGuidNullOrEmpty(tissueId))
                return false;

            return Count(c => c.Name.Equals(name) && c.ID.Equals(tissueId)) > 0;
        }

        /// <summary>
        /// 统计所属地域下的集体经济组织对象
        /// </summary>
        /// <param name="zoneCode">所属地域</param>
        /// <returns>-1（参数错误）/int 对象数量</returns>
        public int Count(string zoneCode)
        {
            if (!CheckTableExist())
            {
                throw new ArgumentNullException("数据库不存在表："
                    + this.GetType().ToString().Substring(this.GetType().ToString().LastIndexOf('.') + 1).Replace("Repository", ""));
            }
            if (!CheckRule.CheckStringNullOrEmpty(ref zoneCode))
                return -1;

            return Count(c => c.ZoneCode.Equals(zoneCode));
        }

        /// <summary>
        /// 按照匹配等级统计所属地域下的集体经济组织对象
        /// </summary>
        /// <param name="zoneCode">所属地域</param>
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

            int cnt = 0;
            if (searchOption == eLevelOption.Self)
                cnt = Count(c => c.ZoneCode.Equals(zoneCode));
            else if (searchOption == eLevelOption.SelfAndSubs)
                cnt = Count(c => c.ZoneCode.StartsWith(zoneCode));
            else
                cnt = Count(c => c.ZoneCode.StartsWith(zoneCode) && c.ZoneCode != zoneCode);
            return cnt;
        }

        /// <summary>
        /// 获取所属地域下所有的集体经济组织对象
        /// </summary>
        /// <param name="zoneCode">所属地域编码</param>
        /// <returns>集体经济组织对象集合</returns>
        public List<CollectivityTissue> GetTissues(string zoneCode)
        {
            if (!CheckTableExist())
            {
                throw new ArgumentNullException("数据库不存在表："
                    + this.GetType().ToString().Substring(this.GetType().ToString().LastIndexOf('.') + 1).Replace("Repository", ""));
            }
            if (!CheckRule.CheckStringNullOrEmpty(ref zoneCode))
                return null;

            List<CollectivityTissue> data = Get(c => c.ZoneCode.Equals(zoneCode));
            return data;
        }

        /// <summary>
        /// 根据匹配等级获取所属地域下所有的集体经济组织排序对象
        /// </summary>
        /// <param name="zoneCode">所属地域编码</param>
        /// <param name="searchOption">匹配等级</param>
        /// <returns>集体经济组织名称排序的集体经济组织对象集合</returns>
        public List<CollectivityTissue> GetTissues(string zoneCode, eLevelOption searchOption)
        {
            if (!CheckTableExist())
            {
                throw new ArgumentNullException("数据库不存在表："
                    + this.GetType().ToString().Substring(this.GetType().ToString().LastIndexOf('.') + 1).Replace("Repository", ""));
            }
            if (!CheckRule.CheckStringNullOrEmpty(ref zoneCode))
                return null;

            List<CollectivityTissue> entity = null;

            if (searchOption == eLevelOption.Self)
            {
                entity = (from q in DataSource.CreateQuery<CollectivityTissue>()
                          where q.ZoneCode.Equals(zoneCode)
                          orderby q.Name
                          select q).ToList();
            }
            if (searchOption == eLevelOption.Subs)
            {
                entity = (from q in DataSource.CreateQuery<CollectivityTissue>()
                          where q.ZoneCode.StartsWith(zoneCode) && q.ZoneCode != zoneCode
                          orderby q.Name
                          select q).ToList();
            }
            if (searchOption == eLevelOption.SelfAndSubs)
            {
                entity = (from q in DataSource.CreateQuery<CollectivityTissue>()
                          where q.ZoneCode.StartsWith(zoneCode)
                          orderby q.Name
                          select q).ToList();
            }
            return entity;
        }

        /// <summary>
        /// 根据集体经济组织名称获取集体经济组织对象
        /// </summary>
        /// <param name="name">集体经济组织名称</param>
        /// <param name="searchType">查找类型</param>
        /// <returns>集体经济组织名称集合</returns>
        public List<CollectivityTissue> SearchByName(string name, eSearchOption searchType)
        {
            if (!CheckTableExist())
            {
                throw new ArgumentNullException("数据库不存在表："
                    + this.GetType().ToString().Substring(this.GetType().ToString().LastIndexOf('.') + 1).Replace("Repository", ""));
            }
            if (!CheckRule.CheckStringNullOrEmpty(ref name))
                return null;
            object dr = null;

            if (searchType == eSearchOption.Precision)
                dr = Get(c => c.Name.Equals(name));
            else if (searchType == eSearchOption.Fuzzy)
                dr = Get(c => c.Name.Equals(name));
            return (List<CollectivityTissue>)dr;
        }

        /// <summary>
        /// 根据法人证明书编号获取集体经济组织对象
        /// </summary>
        /// <param name="cartNumber">法人证明书编号</param>
        /// <param name="searchType">查找类型</param>
        /// <returns>集体经济组织名称集合</returns>
        public List<CollectivityTissue> SearchByLawyerCartNumber(string cartNumber, eSearchOption searchType)
        {
            if (!CheckTableExist())
            {
                throw new ArgumentNullException("数据库不存在表："
                    + this.GetType().ToString().Substring(this.GetType().ToString().LastIndexOf('.') + 1).Replace("Repository", ""));
            }
            if (!CheckRule.CheckStringNullOrEmpty(ref cartNumber))
                return null;
            object entity = null;

            if (searchType == eSearchOption.Precision)
                entity = Get(c => c.LawyerCartNumber.Equals(cartNumber));
            else if (searchType == eSearchOption.Fuzzy)
                entity = Get(c => c.LawyerCartNumber.Contains(cartNumber));

            return (List<CollectivityTissue>)entity;
        }

        /// <summary>
        /// 更新集体经济组织名称
        /// </summary>
        /// <param name="tissue"></param>
        /// <returns>-1（参数错误）/0（失败）/1（成功）</returns>
        public int Update(CollectivityTissue tissue)
        {
            if (!CheckTableExist())
            {
                throw new ArgumentNullException("数据库不存在表："
                    + this.GetType().ToString().Substring(this.GetType().ToString().LastIndexOf('.') + 1).Replace("Repository", ""));
            }
            if (tissue == null || tissue.ID == Guid.Empty)
                return -1;
            tissue.ModifiedTime = DateTime.Now;
            int cnt = 0;
            cnt = Update(tissue, c => c.ID.Equals(tissue.ID));
            return cnt;
        }

        /// <summary>
        /// 删除集体经济组织
        /// </summary>
        /// <param name="zoneID">集体经济组织编码</param>
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

            return Delete(c => c.ID.Equals(zoneID));
        }

        /// <summary>
        /// 根据集体经济组织对象删除
        /// </summary>
        /// <param name="tissue">集体经济组织</param>
        /// <returns>-1（参数错误）/0（失败）/1（成功）</returns>
        public int Delete(CollectivityTissue tissue)
        {
            if (!CheckTableExist())
            {
                throw new ArgumentNullException("数据库不存在表："
                    + this.GetType().ToString().Substring(this.GetType().ToString().LastIndexOf('.') + 1).Replace("Repository", ""));
            }
            if (tissue == null || tissue.ID == null)
                return -1;

            return Delete(c => c.ID.Equals(tissue.ID));
        }

        /// <summary>
        /// 删除所属区域下的所有集体经济组织对象
        /// </summary>
        /// <param name="zoneCode">所属区域代码</param>
        /// <param name="searchOption">查找类型</param>
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
                cnt = Delete(c => c.ZoneCode.Equals(zoneCode));
            else if (searchOption == eLevelOption.Subs)
                cnt = Delete(c => c.ZoneCode.StartsWith(zoneCode));

            return cnt;
        }

        /// <summary>
        /// 添加集体经济组织对象集合
        /// </summary>
        /// <param name="listTissue">集体经济组织对象集合</param>
        /// <param name="overwrite">是否覆盖</param>
        /// <param name="action"></param>
        /// <returns></returns>
        public int Add(List<CollectivityTissue> listTissue, bool overwrite, Action<CollectivityTissue, int, int> action)
        {
            if (!CheckTableExist())
            {
                throw new ArgumentNullException("数据库不存在表："
                    + this.GetType().ToString().Substring(this.GetType().ToString().LastIndexOf('.') + 1).Replace("Repository", ""));
            }
            int count = 0;

            for (int i = 0; i < listTissue.Count; i++)
            {
                CollectivityTissue tissue = listTissue[i];
                action(tissue, i, listTissue.Count);

                if (this.Exists(tissue.ID))
                {
                    if (!overwrite)
                        continue;

                    CollectivityTissue old = this.Get(tissue.ID);
                    if (old == null) continue;
                    tissue.CreationTime = old.CreationTime;
                    tissue.ModifiedTime = DateTime.Now;

                    count += Update(tissue);
                }
                else
                {
                    tissue.CreationTime = DateTime.Now;
                    tissue.ModifiedTime = DateTime.Now;
                    count += Add(tissue);
                }
            }

            return count;
        }

        /// <summary>
        /// 根据指定区域数组获得其下的所有集体经济组织对象集合
        /// </summary>
        /// <param name="codeZones">区域代码数组</param>
        /// <param name="action"></param>
        /// <returns>集体经济对象集合</returns>
        public List<CollectivityTissue> GetTissues(string[] codeZones, Action<string, int, int> action)
        {
            if (!CheckTableExist())
            {
                throw new ArgumentNullException("数据库不存在表："
                    + this.GetType().ToString().Substring(this.GetType().ToString().LastIndexOf('.') + 1).Replace("Repository", ""));
            }
            List<CollectivityTissue> list = new List<CollectivityTissue>();

            for (int i = 0; i < codeZones.Length; i++)
            {
                string code = codeZones[i];

                if (action != null)
                    action(code, i, codeZones.Length);

                list.AddRange(GetTissues(code));
            }

            return list;
        }

        /// <summary>
        /// 清空发包方
        /// </summary>
        public void ClearTissue()
        {
            if (!CheckTableExist())
            {
                throw new ArgumentNullException("数据库不存在表："
                    + this.GetType().ToString().Substring(this.GetType().ToString().LastIndexOf('.') + 1).Replace("Repository", ""));
            }
            Delete();
        }
        #endregion
    }
}
