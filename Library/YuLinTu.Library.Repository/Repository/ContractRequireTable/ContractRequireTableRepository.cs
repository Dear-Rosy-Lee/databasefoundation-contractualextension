// (C) 2025 鱼鳞图公司版权所有，保留所有权利
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YuLinTu.Data;
using YuLinTu.Library.Entity;
using YuLinTu;

namespace YuLinTu.Library.Repository
{
    /// <summary>
    /// 农村土地承包经营申请登记表的数据访问类
    /// </summary>
    public class ContractRequireTableRepository : RepositoryDbContext<ContractRequireTable>,  IContractRequireTableRepository
    {
        #region Ctor
        
        private IDataSourceSchema m_DSSchema = null;

        public ContractRequireTableRepository(IDataSource ds)
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
            //    return m_DSSchema.AnyElement(m_DSSchema.GetElements()[0].Schema, typeof(ContractRegeditBook).GetAttribute<DataTableAttribute>().TableName);
            //}
            //catch (Exception)
            //{
            //    return false;
            //}
            return true;
        }

        /// <summary>
        /// 根据ID删除农村土地承包经营申请登记表对象
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
        /// 更新农村土地承包经营申请登记表对象
        /// </summary>
        /// <returns>-1（参数错误）/0（失败）/1（成功）</returns>
        public int Update(ContractRequireTable entity)
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
        /// 根据ID获取农村土地承包经营申请登记表对象
        /// </summary>
        public ContractRequireTable Get(Guid guid)
        {
            if (!CheckTableExist())
            {
                throw new ArgumentNullException("数据库不存在表："
                    + this.GetType().ToString().Substring(this.GetType().ToString().LastIndexOf('.') + 1).Replace("Repository", ""));
            }
            if (!CheckRule.CheckGuidNullOrEmpty(guid))
                return null;
            return Get(c => c.ID.Equals(guid)).FirstOrDefault();
        }

        /// <summary>
        /// 根据ID判断农村土地承包经营申请登记表对象是否存在
        /// </summary>
        public bool Exists(Guid guid)
        {
            if (!CheckTableExist())
            {
                throw new ArgumentNullException("数据库不存在表："
                    + this.GetType().ToString().Substring(this.GetType().ToString().LastIndexOf('.') + 1).Replace("Repository", ""));
            }
            if (guid == null || guid == Guid.Empty)
                return false;
            return Count(c=>c.ID.Equals(guid)) > 0 ? true : false;
        }

        /// <summary>
        /// 根据申请书编号获取农村土地承包经营申请登记表对象
        /// </summary>
        /// <param name="tabNumber">申请书编号</param>
        /// <returns>农村土地承包经营申请登记表</returns>
        public ContractRequireTable Get(string tabNumber)
        {
            if (!CheckTableExist())
            {
                throw new ArgumentNullException("数据库不存在表："
                    + this.GetType().ToString().Substring(this.GetType().ToString().LastIndexOf('.') + 1).Replace("Repository", ""));
            }
            if (!CheckRule.CheckStringNullOrEmpty(ref tabNumber))
                return null;

            object entity = Get(c => c.Number.Equals(tabNumber));
            return entity as ContractRequireTable;
        }
        public int GetMaxNumber()
        {
            if (!CheckTableExist())
            {
                throw new ArgumentNullException("数据库不存在表："
                    + this.GetType().ToString().Substring(this.GetType().ToString().LastIndexOf('.') + 1).Replace("Repository", ""));
            }
            var table = Get().Where(c=>c.Number!=null && c.Number!="").ToList();
            if (table.Count <= 0)
                return 0;
            else
                return table.Max(c => int.Parse(c.Number));
        }
        /// <summary>
        /// 根据组织编码获得以申请书编号排序农村土地承包经营申请登记表
        /// </summary>
        /// <param name="tissueCode">组织编码</param>
        /// <returns>申请登记表</returns>
        public List<ContractRequireTable> GetTissueRequireTable(string tissueCode)
        {
            if (!CheckTableExist())
            {
                throw new ArgumentNullException("数据库不存在表："
                    + this.GetType().ToString().Substring(this.GetType().ToString().LastIndexOf('.') + 1).Replace("Repository", ""));
            }
            if (!CheckRule.CheckStringNullOrEmpty(ref tissueCode))
                return null;

            object entity =(from q in DataSource.CreateQuery<ContractRequireTable>()
                            where q.Path.StartsWith(tissueCode)
                            orderby q.Number
                            select q).ToList();
            return entity as List<ContractRequireTable>;
        }


        /// <summary>
        /// 根据地域代码获取农村土地承包经营申请登记表
        /// </summary>
        /// <param name="zoneCode">地域代码</param>
        /// <param name="searchOption">查找类型</param>
        /// <returns>农村土地承包经营申请登记表</returns>
        public List<ContractRequireTable> GetByZoneCode(string zoneCode, eSearchOption searchOption)
        {
            if (!CheckTableExist())
            {
                throw new ArgumentNullException("数据库不存在表："
                    + this.GetType().ToString().Substring(this.GetType().ToString().LastIndexOf('.') + 1).Replace("Repository", ""));
            }
            if (!CheckRule.CheckStringNullOrEmpty(ref zoneCode))
                return null;
            object entity = null;
            if (searchOption == eSearchOption.Fuzzy)
                entity = Get(c => c.ZoneCode.Contains(zoneCode));
            else
                entity = Get(c => c.ZoneCode.Equals(zoneCode));
            return entity as List<ContractRequireTable>;
        }

        /// <summary>
        /// 根据地域代码删除农村土地承包经营申请登记表
        /// </summary>
        /// <param name="zoneCode">地域代码</param>
        /// <param name="searchOption">查找类型</param>
        /// <returns>-1（参数错误）/0（失败）/1（成功）</returns>
        public int DeleteByZoneCode(string zoneCode, eSearchOption searchOption)
        {
            if (!CheckTableExist())
            {
                throw new ArgumentNullException("数据库不存在表："
                    + this.GetType().ToString().Substring(this.GetType().ToString().LastIndexOf('.') + 1).Replace("Repository", ""));
            }
            if (!CheckRule.CheckStringNullOrEmpty(ref zoneCode))
            {
                return -1;
            }
            int cnt = 0;
            if (searchOption == eSearchOption.Fuzzy)
                cnt = Delete(c => c.ZoneCode.Contains(zoneCode));
            else
                cnt = Delete(c => c.ZoneCode.Equals(zoneCode));
            return cnt;
        }

        /// <summary>
        /// 按地域统计农村土地承包经营申请登记表的数量
        /// </summary>
        /// <param name="zoneCode">地域代码</param>
        /// <param name="searchOption">匹配级别</param>
        /// <returns>-1（参数错误）/int 对象数量</returns>
        public int Count(string zoneCode, eLevelOption searchOption)
        {
            if (!CheckTableExist())
            {
                throw new ArgumentNullException("数据库不存在表："
                    + this.GetType().ToString().Substring(this.GetType().ToString().LastIndexOf('.') + 1).Replace("Repository", ""));
            }
            if (!CheckRule.CheckStringNullOrEmpty(ref zoneCode))
            {
                return -1;
            }
            if (searchOption == eLevelOption.Self)
            {
                return Count(c => c.ZoneCode.Equals(zoneCode));
            }
            return Count(c => c.ZoneCode.StartsWith(zoneCode));
            
        }

        #endregion
	}
}