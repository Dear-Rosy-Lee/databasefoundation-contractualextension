// (C) 2015 鱼鳞图公司版权所有，保留所有权利
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YuLinTu.Library.Entity;
using System.Data;
using YuLinTu.Data;
using YuLinTu;

namespace YuLinTu.Library.Repository
{
    /// <summary>
    /// 农村土地承包合同的数据访问类
    /// </summary>
    public class ContractConcordRepository : RepositoryDbContext<ContractConcord>, IContractConcordRepository
    {
        #region Ctor

        private IDataSourceSchema m_DSSchema = null;

        public ContractConcordRepository(IDataSource ds)
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
            //    return m_DSSchema.AnyElement(m_DSSchema.GetElements()[0].Schema, typeof(ContractConcord).GetAttribute<DataTableAttribute>().TableName);
            //}
            //catch (Exception)
            //{
            //    return false;
            //}
            return true;
        }


        //public int DeleteByXXXX(string codeZone, eVirtualPersonStatus state)
        //{
        //    var qcZone = DataSource.CreateQuery<Zone>();
        //    var qcPerson = DataSource.CreateQuery<LandVirtualPerson>();
        //    var qcLand = DataSource.CreateQuery<ContractLand>();
        //    var qcConcord = DataSource.CreateQuery<ContractConcord>();

        //    var qc = from zone in qcZone
        //             join person in qcPerson on zone.FullCode equals person.ZoneCode
        //             where zone.FullCode == codeZone && person.Status == state
        //             select person.ID;

        //    AppendEdit(qcConcord.Where(c => qc.Contains((Guid)c.ContracterId)).Delete());

        //    return 1;
        //}

        //public int DeleteByXXXX(Guid[] ids)
        //{
        //    var qcConcord = DataSource.CreateQuery<ContractConcord>();
        //    AppendEdit(qcConcord.Where(c => ids.Contains((Guid)c.ContracterId)).Delete());

        //    return 1;
        //}

        /// <summary>
        /// 根据承包合同唯一标识删除农村土地承包合同
        /// </summary>
        /// <param name="guid">承包合同唯一标识</param>
        /// <returns>-1（参数错误）/0（失败）/1（成功）</returns>
        public int Delete(Guid guid)
        {
            if (!CheckTableExist())
            {
                throw new ArgumentNullException("数据库不存在表："
                    + this.GetType().ToString().Substring(this.GetType().ToString().LastIndexOf('.') + 1).Replace("Repository", ""));
            }
            if (!CheckRule.CheckGuidNullOrEmpty(guid))
                return 0;

            int count = 0;
            count = Delete(c => c.ID.Equals(guid));

            return count;
        }

        /// <summary>
        /// 更新一个地域下所有的合同为不可用
        /// </summary>
        /// <param name="zoneCode">区域代码</param>
        /// <returns>-1（参数错误）/0（失败）/1（成功）</returns>
        public int UpdateConcordInValid(string zoneCode)
        {
            if (!CheckTableExist())
            {
                throw new ArgumentNullException("数据库不存在表："
                    + this.GetType().ToString().Substring(this.GetType().ToString().LastIndexOf('.') + 1).Replace("Repository", ""));
            }
            if (!CheckRule.CheckStringNullOrEmpty(ref zoneCode))
                return -1;


            var q = from qc in DataSource.CreateQuery<ContractConcord>()
                    where qc.ZoneCode.Equals(zoneCode)
                    select qc;
            foreach (var item in q.ToList())
            {
                item.IsValid = false;
                if (Update(item, c => c.ID.Equals(item.ID)) == 0)
                    return 0;
            }

            return 1;
        }

        /// <summary>
        /// 根据承包方Id更新对象的承包方姓名
        /// </summary>
        ///<param name="houseHolderID">承包方Id</param> 
        ///<param name="houseHolderName">承包方姓名</param>
        ///<returns>-1（参数错误）/0（失败）/1（成功）</returns>
        public int Update(Guid houseHolderID, string houseHolderName)
        {
            if (!CheckTableExist())
            {
                throw new ArgumentNullException("数据库不存在表："
                    + this.GetType().ToString().Substring(this.GetType().ToString().LastIndexOf('.') + 1).Replace("Repository", ""));
            }
            if (!CheckRule.CheckStringNullOrEmpty(ref houseHolderName) || !CheckRule.CheckGuidNullOrEmpty(houseHolderID))
                return -1;

            var q = from qc in DataSource.CreateQuery<ContractConcord>()
                    where qc.ContracterId.Equals(houseHolderID)
                    select qc;
            foreach (var item in q.ToList())
            {
                item.ContracterName = houseHolderName;
                if (Update(item, c => c.ID.Equals(item.ID)) == 0)
                    return 0;
            }

            return 1;
        }

        /// <summary>
        /// 根据当前合同是否可用、当前合同是否可用、区域代码获得农村土地承包合同对象
        /// </summary>
        /// <param name="isValid">当前合同是否可用</param>
        /// <param name="status">业务流程状态</param>
        /// <param name="zoneCode">区域代码</param>
        /// <param name="searchOption">查找类型</param>
        /// <returns>农村土地承包合同</returns>
        public List<ContractConcord> GetCollection(bool isValid, eStatus status, string zoneCode, eLevelOption searchOption)
        {
            if (!CheckTableExist())
            {
                throw new ArgumentNullException("数据库不存在表："
                    + this.GetType().ToString().Substring(this.GetType().ToString().LastIndexOf('.') + 1).Replace("Repository", ""));
            }
            if (!CheckRule.CheckStringNullOrEmpty(ref zoneCode))
                return null;

            object entity = null;
            if (searchOption == eLevelOption.Subs)
            {
                entity = Get(c => c.ZoneCode.StartsWith(zoneCode) && c.IsValid.Equals(isValid) && c.Status.Equals(status));//
            }
            else
            {
                entity = Get(c => c.ZoneCode.Equals(zoneCode) && c.IsValid.Equals(isValid) && c.Status.Equals(status));//
            }

            return entity as List<ContractConcord>;
        }

        /// <summary>
        /// 更新对象
        /// </summary>
        /// <returns>-1（参数错误）/0（失败）/1（成功）</returns>
        public int Update(ContractConcord entity)
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
        /// 根据承包合同唯一标识获取农村土地承包合同
        /// </summary>
        /// <param name="guid">承包合同唯一标识</param>
        public ContractConcord Get(Guid guid)
        {
            if (!CheckTableExist())
            {
                throw new ArgumentNullException("数据库不存在表："
                    + this.GetType().ToString().Substring(this.GetType().ToString().LastIndexOf('.') + 1).Replace("Repository", ""));
            }
            if (!CheckRule.CheckGuidNullOrEmpty(guid))
                return null;
            var data = Get(c => c.ID.Equals(guid)).FirstOrDefault();
            return data;
        }

        /// <summary>
        /// 根据承包合同唯一标识判断农村土地承包合同对象是否存在
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
            return Count(c => c.ID.Equals(guid)) > 0 ? true : false;
        }

        #endregion

        #region ExtendMethod

        /// <summary>
        /// 根据区域代码获取以合同编号排序的农村土地承包合同集合
        /// </summary>
        /// <param name="zoneCode">区域代码</param>
        /// <returns>农村土地承包合同集合</returns>
        public List<ContractConcord> GetByZoneCode(string zoneCode)
        {
            if (!CheckTableExist())
            {
                throw new ArgumentNullException("数据库不存在表："
                    + this.GetType().ToString().Substring(this.GetType().ToString().LastIndexOf('.') + 1).Replace("Repository", ""));
            }
            if (!CheckRule.CheckStringNullOrEmpty(ref zoneCode))
                return null;


            object entity = (from q in DataSource.CreateQuery<ContractConcord>()
                             where q.ZoneCode.Equals(zoneCode)
                             orderby q.ConcordNumber
                             select q).ToList();
            return entity as List<ContractConcord>;
        }

        /// <summary>
        /// 根据区域代码、合同可用性获取农村土地承包合同集合
        /// </summary>
        /// <param name="zoneCode">区域代码</param>
        /// <param name="isValid">合同可用性</param>
        /// <returns>农村土地承包合同集合</returns>
        public List<ContractConcord> GetContractsByZoneCode(string zoneCode, bool isValid)
        {
            if (!CheckTableExist())
            {
                throw new ArgumentNullException("数据库不存在表："
                    + this.GetType().ToString().Substring(this.GetType().ToString().LastIndexOf('.') + 1).Replace("Repository", ""));
            }
            if (!CheckRule.CheckStringNullOrEmpty(ref zoneCode))
                return null;


            object entity = Get(c => c.ZoneCode.Equals(zoneCode) && c.IsValid.Equals(isValid));
            return entity as List<ContractConcord>;
        }

        /// <summary>
        /// 根据区域代码、合同可用性不同匹配性获取农村土地承包合同集合
        /// </summary>
        /// <param name="zoneCode">区域代码</param>
        /// <param name="searchOption">匹配级别</param>
        /// <param name="isValid">合同可用性</param>
        /// <returns>农村土地承包合同集合</returns>
        public List<ContractConcord> GetContractsByZoneCode(string zoneCode, eLevelOption searchOption, bool isValid)
        {
            if (!CheckTableExist())
            {
                throw new ArgumentNullException("数据库不存在表："
                    + this.GetType().ToString().Substring(this.GetType().ToString().LastIndexOf('.') + 1).Replace("Repository", ""));
            }
            if (!CheckRule.CheckStringNullOrEmpty(ref zoneCode))
                return null;

            object entity = 0;

            if (searchOption == eLevelOption.Self)
                entity = Get(c => c.ZoneCode.Equals(zoneCode) && c.IsValid.Equals(isValid));
            else if (searchOption == eLevelOption.Subs)
                entity = Get(c => c.ZoneCode.StartsWith(zoneCode) && c.IsValid.Equals(isValid));
            return entity as List<ContractConcord>;
        }

        /// <summary>
        /// 根据农村土地承包经营权申请书ID获取农村土地承包合同
        /// </summary>
        /// <param name="requireBookId">农村土地承包经营权申请书ID</param>
        /// <returns>农村土地承包合同</returns>
        public ContractConcord GetByRequireBookId(Guid requireBookId)
        {
            if (!CheckTableExist())
            {
                throw new ArgumentNullException("数据库不存在表："
                    + this.GetType().ToString().Substring(this.GetType().ToString().LastIndexOf('.') + 1).Replace("Repository", ""));
            }
            if (requireBookId == null)
            {
                throw new ArgumentNullException("申请书标识不符合要求!");
            }
            if (requireBookId == Guid.Empty)
            {
                throw new ArgumentNullException("申请书标识不符合要求!");
            }
            object contractConcord = Get(c => c.RequireBookId.Equals(requireBookId));
            return contractConcord as ContractConcord;
        }



        public List<ContractConcord> GetContractsByZoneCode(string zoneCode)
        {
            if (!CheckTableExist())
            {
                throw new ArgumentNullException("数据库不存在表："
                    + this.GetType().ToString().Substring(this.GetType().ToString().LastIndexOf('.') + 1).Replace("Repository", ""));
            }
            if (!CheckRule.CheckStringNullOrEmpty(ref zoneCode))
                return null;
            object entity = Get(c => c.ZoneCode.StartsWith(zoneCode) && c.IsValid.Equals(true));//
            return entity as List<ContractConcord>;
        }

        /// <summary>
        ///// 获取该地域下的合同
        ///// </summary>
        ///// <param name="zoneCode">区域代码</param>
        ///// <returns>农村土地承包合同</returns>
        //public List<ContractConcord> GetContractsByZoneCode(string zoneCode,eLevelOption eLevelOption)
        //{
        //    if (!CheckTableExist())
        //    {
        //        throw new ArgumentNullException("数据库不存在表："
        //            + this.GetType().ToString().Substring(this.GetType().ToString().LastIndexOf('.') + 1).Replace("Repository", ""));
        //    }
        //    if (!CheckRule.CheckStringNullOrEmpty(ref zoneCode))
        //        return null;
        //    object entity = null;
        //    if (eLevelOption == eLevelOption.Self)
        //    {
        //        entity = Get(c => c.ZoneCode.Equals(zoneCode) && c.IsValid.Equals(true));//
        //    }
        //    else if (eLevelOption==eLevelOption.Subs)
        //    {
        //        entity= Get(c => (c.ZoneCode.Equals(zoneCode)&&c.ZoneCode.StartsWith(zoneCode)) && c.IsValid.Equals(true));//
        //    }
        //    else if(eLevelOption == eLevelOption.SelfAndSubs)
        //    {
        //        entity = Get(c => c.ZoneCode.StartsWith(zoneCode) && c.IsValid.Equals(true));//
        //    }
        //    return entity as List<ContractConcord>;
        //}

        /// <summary>
        /// 根据不同匹配等级获取该地域下的合同
        /// </summary>
        /// <param name="zoneCode">区域代码</param>
        /// <param name="searchOption">匹配级别</param>
        /// <returns>农村土地承包合同</returns>
        public List<ContractConcord> GetContractsByZoneCode(string zoneCode, eLevelOption searchOption)
        {
            if (!CheckTableExist())
            {
                throw new ArgumentNullException("数据库不存在表："
                    + this.GetType().ToString().Substring(this.GetType().ToString().LastIndexOf('.') + 1).Replace("Repository", ""));
            }
            if (zoneCode == null)
                return null;

            zoneCode = zoneCode.Trim();
            if (zoneCode == string.Empty)
                return null;

            object entity = 0;

            if (searchOption == eLevelOption.Self)
            {
                entity = Get(c => c.ZoneCode.Equals(zoneCode) && c.IsValid.Equals(true));
            }
            else if (searchOption == eLevelOption.Subs)
            {
                entity = Get(c => c.ZoneCode != zoneCode && c.ZoneCode.StartsWith(zoneCode) && c.IsValid.Equals(true));
            }
            else if (searchOption == eLevelOption.SelfAndSubs)
            {
                entity = Get(c => c.ZoneCode.StartsWith(zoneCode) && c.IsValid.Equals(true));
            }
            return entity as List<ContractConcord>;
        }

        /// <summary>
        /// 根据发包方Id获取以承包方姓名排序的农村土地承包合同
        /// </summary>
        /// <param name="guid">发包方Id</param>
        /// <returns>农村土地承包合同</returns>
        public List<ContractConcord> GetContractsByTissueID(Guid guid)
        {
            if (!CheckTableExist())
            {
                throw new ArgumentNullException("数据库不存在表："
                    + this.GetType().ToString().Substring(this.GetType().ToString().LastIndexOf('.') + 1).Replace("Repository", ""));
            }
            if (guid == null || guid == Guid.Empty)
                return null;

            object entity = (from q in DataSource.CreateQuery<ContractConcord>()
                             where q.SenderId.Equals(guid) && q.IsValid.Equals(true)
                             orderby q.ContracterName
                             select q).ToList();
            return entity as List<ContractConcord>;
        }

        /// <summary>
        /// 根据发包方Id统计农村土地承包合同的数量
        /// </summary>
        /// <param name="tissueID">发包方Id</param>
        /// <returns>-1（参数错误）/int 农村土地承包合同的数量</returns>
        public int CountByTissueID(Guid tissueID)
        {
            if (!CheckTableExist())
            {
                throw new ArgumentNullException("数据库不存在表："
                    + this.GetType().ToString().Substring(this.GetType().ToString().LastIndexOf('.') + 1).Replace("Repository", ""));
            }
            if (tissueID == null || tissueID == Guid.Empty)
                return -1;

            return Count(c => c.SenderId.Equals(tissueID));
        }

        /// <summary>
        /// 根据承包方Id获取农村土地承包合同
        /// </summary>
        /// <param name="guid">承包方Id</param>
        /// <returns>农村土地承包合同</returns>
        public List<ContractConcord> GetContractsByFamilyID(Guid guid)
        {
            if (!CheckTableExist())
            {
                throw new ArgumentNullException("数据库不存在表："
                    + this.GetType().ToString().Substring(this.GetType().ToString().LastIndexOf('.') + 1).Replace("Repository", ""));
            }
            if (guid == null || guid == Guid.Empty)
                return null;

            object entity = Get(c => c.ContracterId.Equals(guid));
            return entity as List<ContractConcord>;
        }

        /// <summary>
        /// 根据承包方Id集合获取农村土地承包合同集合
        /// </summary>
        public List<ContractConcord> GetContractsByFamilyIDs(Guid[] guids)
        {
            if (guids == null)
                return null;
            //var query = from concord in DataSource.CreateQuery<ContractConcord>()
            //            select concord;
            //return query.Where(c => guids.Contains((Guid)c.ContracterId)).ToList();
            return DataSource.CreateQuery<ContractConcord>().Where(c => guids.Contains((Guid)c.ContracterId)).ToList();
        }

        /// <summary>
        /// 根据承包方Id获取农村土地承包合同
        /// </summary>
        /// <param name="guid">承包方Id</param>
        /// <returns>农村土地承包合同</returns>
        public List<ContractConcord> GetAllConcordByFamilyID(Guid guid)
        {
            if (!CheckTableExist())
            {
                throw new ArgumentNullException("数据库不存在表："
                    + this.GetType().ToString().Substring(this.GetType().ToString().LastIndexOf('.') + 1).Replace("Repository", ""));
            }
            if (guid == null || guid == Guid.Empty)
                return null;

            object entity = Get(c => c.ContracterId.Equals(guid));
            return entity as List<ContractConcord>;
        }

        /// <summary>
        /// 根据区域与承包方姓名获取所有农村土地承包合同集合
        /// </summary>
        /// <param name="zoneCode">区域代码</param>
        /// <param name="name">承包方姓名</param>
        /// <returns>农村土地承包合同集合</returns>
        public List<ContractConcord> GetByZoneCodeAndFamilyName(string zoneCode, string name)
        {
            if (!CheckTableExist())
            {
                throw new ArgumentNullException("数据库不存在表："
                    + this.GetType().ToString().Substring(this.GetType().ToString().LastIndexOf('.') + 1).Replace("Repository", ""));
            }
            if (!CheckRule.CheckStringNullOrEmpty(ref zoneCode) || !CheckRule.CheckStringNullOrEmpty(ref name))
                return null;

            object entity = Get(c => c.ZoneCode.Equals(zoneCode) && c.ContracterName.Equals(name));
            return entity as List<ContractConcord>;
        }

        /// <summary>
        /// 根据地域与户主名称查找合同可用的农村土地承包合同
        /// </summary>
        /// <param name="zoneCode">区域代码</param>
        /// <param name="Name">承包方姓名</param>
        /// <returns>农村土地承包合同集合</returns>
        public List<ContractConcord> GetContractsByFamilyName(string zoneCode, string name)
        {
            if (!CheckTableExist())
            {
                throw new ArgumentNullException("数据库不存在表："
                    + this.GetType().ToString().Substring(this.GetType().ToString().LastIndexOf('.') + 1).Replace("Repository", ""));
            }
            if (!CheckRule.CheckStringNullOrEmpty(ref zoneCode) || !CheckRule.CheckStringNullOrEmpty(ref name))
                return null;

            object entity = Get(c => c.ZoneCode.Equals(zoneCode) && c.ContracterName.Equals(name) && c.IsValid.Equals(true));
            return entity as List<ContractConcord>;
        }

        /// <summary>
        /// 根据户主名称查找合同
        /// </summary>
        /// <param name="name">承包方姓名</param>
        /// <param name="searchOption">查找类型</param>
        /// <returns>农村土地承包合同集合</returns>
        public List<ContractConcord> GetContractsByFamilyName(string name, eSearchOption searchOption)
        {
            if (!CheckTableExist())
            {
                throw new ArgumentNullException("数据库不存在表："
                    + this.GetType().ToString().Substring(this.GetType().ToString().LastIndexOf('.') + 1).Replace("Repository", ""));
            }
            if (!CheckRule.CheckStringNullOrEmpty(ref name))
                return null;

            object entity = null;

            if (searchOption == eSearchOption.Precision)
            {
                entity = Get(c => c.ContracterName.Equals(name) && c.IsValid.Equals(true));
            }
            else if (searchOption == eSearchOption.Fuzzy)
            {
                entity = Get(c => c.ContracterName.Contains(name) && c.IsValid.Equals(true));
            }
            return entity as List<ContractConcord>;
        }

        /// <summary>
        /// 根据合同号和地域查找合同
        /// </summary>
        /// <param name="number">农村土地承包合同编号</param>
        /// <param name="searchOption">合同编号查找类型</param>
        /// <param name="zoneCode">区域代码</param>
        /// <param name="searchOption">区域代码匹配级别</param>
        /// <returns>农村土地承包合同集合</returns>
        public List<ContractConcord> SearchContractsByNumber(string number, eSearchOption searchType, string zoneCode, eLevelOption searchOption)
        {
            if (!CheckTableExist())
            {
                throw new ArgumentNullException("数据库不存在表："
                    + this.GetType().ToString().Substring(this.GetType().ToString().LastIndexOf('.') + 1).Replace("Repository", ""));
            }
            if (!CheckRule.CheckStringNullOrEmpty(ref number) || !CheckRule.CheckStringNullOrEmpty(ref zoneCode))
            {
                return null;
            }
            object entities = null;
            if (searchType == eSearchOption.Fuzzy)
            {
                if (searchOption == eLevelOption.Self)
                {
                    entities = Get(c => c.ConcordNumber.Contains(number) && c.ZoneCode.Equals(zoneCode) && c.IsValid.Equals(true));
                }
                else
                {
                    entities = Get(c => c.ConcordNumber.Contains(number) && c.ZoneCode.StartsWith(zoneCode) && c.IsValid.Equals(true));
                }
            }
            else
            {
                if (searchOption == eLevelOption.Self)
                {
                    entities = Get(c => c.ContracterName.Equals(Name) && c.ZoneCode.Equals(zoneCode) && c.IsValid.Equals(true));
                }
                else
                {
                    entities = Get(c => c.ContracterName.Equals(Name) && c.ZoneCode.StartsWith(zoneCode) && c.IsValid.Equals(true));
                }
            }
            return entities as List<ContractConcord>;
        }

        /// <summary>
        /// 根据承包方和地域查找合同
        /// </summary>
        /// <param name="name">承包方姓名</param>
        /// <param name="searchOption">承包方姓名查找类型</param>
        /// <param name="zoneCode">区域代码</param>
        /// <param name="searchOption">区域代码匹配级别</param>
        /// <returns>农村土地承包合同集合</returns>
        public List<ContractConcord> SearchContractsByContracterName(string name, eSearchOption searchType, string zoneCode, eLevelOption searchOption)
        {
            if (!CheckTableExist())
            {
                throw new ArgumentNullException("数据库不存在表："
                    + this.GetType().ToString().Substring(this.GetType().ToString().LastIndexOf('.') + 1).Replace("Repository", ""));
            }
            if (!CheckRule.CheckStringNullOrEmpty(ref name) || !CheckRule.CheckStringNullOrEmpty(ref zoneCode))
            {
                return null;
            }

            object entities = null;
            if (searchType == eSearchOption.Fuzzy)
            {
                if (searchOption == eLevelOption.Self)
                {
                    entities = Get(c => c.ContracterName.Contains(name) && c.ZoneCode.Equals(zoneCode) && c.IsValid.Equals(true));
                }
                else
                {
                    entities = Get(c => c.ContracterName.Contains(name) && c.ZoneCode.StartsWith(zoneCode) && c.IsValid.Equals(true));
                }
            }
            else
            {
                if (searchOption == eLevelOption.Self)
                {
                    entities = Get(c => c.ContracterName.Equals(name) && c.ZoneCode.Equals(zoneCode) && c.IsValid.Equals(true));
                }
                else
                {
                    entities = Get(c => c.ContracterName.Equals(name) && c.ZoneCode.StartsWith(zoneCode) && c.IsValid.Equals(true));
                }
            }
            return entities as List<ContractConcord>;
        }

        /// <summary>
        /// 查找这个地域编码对应地域下所有的不可用的合同
        /// </summary>
        /// <param name="zoneCode">地域编码</param>
        /// <returns>不可用的合同</returns>
        public List<ContractConcord> GetByZoneSearchBadConcord(string zoneCode)
        {
            if (!CheckTableExist())
            {
                throw new ArgumentNullException("数据库不存在表："
                    + this.GetType().ToString().Substring(this.GetType().ToString().LastIndexOf('.') + 1).Replace("Repository", ""));
            }
            if (!CheckRule.CheckStringNullOrEmpty(ref zoneCode))
            {
                return null;
            }
            object entities = Get(c => c.ZoneCode.Equals(zoneCode) && c.IsValid.Equals(false));
            return entities as List<ContractConcord>;
        }

        /// <summary>
        /// 根据合同编号查找合同
        /// </summary>
        /// <param name="concordNumber">合同编号</param>
        /// <param name="searchOption">查找类型</param>
        /// <returns>农村土地承包合同集合</returns>
        public List<ContractConcord> SearchContractsByConcordNumber(string concordNumber, eSearchOption searchOption)
        {
            if (!CheckTableExist())
            {
                throw new ArgumentNullException("数据库不存在表："
                    + this.GetType().ToString().Substring(this.GetType().ToString().LastIndexOf('.') + 1).Replace("Repository", ""));
            }
            if (!CheckRule.CheckStringNullOrEmpty(ref concordNumber))
            {
                return null;
            }
            object entity = null;

            if (searchOption == eSearchOption.Precision)
                entity = Get(c => c.ConcordNumber.Equals(concordNumber) && c.IsValid.Equals(true));
            else if (searchOption == eSearchOption.Fuzzy)
                entity = Get(c => c.ConcordNumber.Contains(concordNumber) && c.IsValid.Equals(true));
            return entity as List<ContractConcord>;
        }


        public bool Exists(string concordNumber)
        {
            if (!CheckTableExist())
            {
                throw new ArgumentNullException("数据库不存在表："
                    + this.GetType().ToString().Substring(this.GetType().ToString().LastIndexOf('.') + 1).Replace("Repository", ""));
            }
            if (!CheckRule.CheckStringNullOrEmpty(ref concordNumber))
            {
                return false;
            }
            return Count(c => c.ContracterName.Equals(concordNumber)) > 0;
        }

        /// <summary>
        /// 通过合同编号查看合同是否存在 如果存在又是否为指定的承包方姓名
        /// </summary>
        /// <param name="concordNumber">合同编号</param>
        /// <param name="virtualPersonName">指定的承包方姓名</param>
        /// <returns>true（存在）/false（不存在</returns>
        public bool ExistsNotZoneCodeToConcordNumber(string concordNumber, string virtualPersonName)
        {
            if (!CheckTableExist())
            {
                throw new ArgumentNullException("数据库不存在表："
                    + this.GetType().ToString().Substring(this.GetType().ToString().LastIndexOf('.') + 1).Replace("Repository", ""));
            }
            if (!CheckRule.CheckStringNullOrEmpty(ref concordNumber) || !CheckRule.CheckStringNullOrEmpty(ref virtualPersonName))
                return false;

            //先寻找这个编号有几个
            int count = Count(c => c.ConcordNumber.EndsWith(concordNumber));

            //大于一个则直接返回已存在
            if (count > 1)
                return true;
            else if (count == 0)
                return false;
            else if (count == 1)//如果有且只有一个 则判断是否同名
                count = Count(c => c.ConcordNumber.StartsWith(concordNumber) && c.ContracterName.Equals(virtualPersonName));
            //如果同名则返回不存在 不同名则返回存在
            return count > 0 ? false : true;
        }

        /// <summary>
        /// 根据农村土地承包合同编号获取合同对象
        /// </summary>
        public ContractConcord Get(string concordNumber)
        {
            if (!CheckTableExist())
            {
                throw new ArgumentNullException("数据库不存在表："
                    + this.GetType().ToString().Substring(this.GetType().ToString().LastIndexOf('.') + 1).Replace("Repository", ""));
            }
            if (!CheckRule.CheckStringNullOrEmpty(ref concordNumber))
                return null;

            object data = Get(c => c.ConcordNumber.Equals(concordNumber)).FirstOrDefault();

            return data as ContractConcord;
        }

        /// <summary>
        /// 删除当前地域下所有合同
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
            return Delete(c => c.ZoneCode.Equals(zoneCode));
        }

        /// <summary>
        /// 统计指定地域下已颁证的数量。
        /// </summary>
        /// <param name="zoneCode">地域编码</param>
        /// <returns>已颁证的数量。</returns>
        public int CountAwareRegeditBooks(string zoneCode)
        {
            if (!CheckTableExist())
            {
                throw new ArgumentNullException("数据库不存在表："
                    + this.GetType().ToString().Substring(this.GetType().ToString().LastIndexOf('.') + 1).Replace("Repository", ""));
            }
            if (!CheckRule.CheckStringNullOrEmpty(ref zoneCode))
                return -1;
            return Count(c => c.ZoneCode.StartsWith(zoneCode) && c.Status.Equals(eStatus.Checked));// 
        }

        /// <summary>
        /// 统计指定地域下合同数量。
        /// </summary>
        /// <param name="zoneCode">地域编码</param>
        /// <returns>合同的数量。</returns>
        public int CountCords(string zoneCode, eLevelOption searchOption)
        {
            if (!CheckTableExist())
            {
                throw new ArgumentNullException("数据库不存在表："
                    + this.GetType().ToString().Substring(this.GetType().ToString().LastIndexOf('.') + 1).Replace("Repository", ""));
            }
            if (!CheckRule.CheckStringNullOrEmpty(ref zoneCode))
                return -1;
            int count = 0;
            if (searchOption == eLevelOption.Self)
                count = Count(c => c.ZoneCode.Equals(zoneCode));
            else if (searchOption == eLevelOption.Subs)
                count = Count(c => c.ZoneCode != zoneCode && c.ZoneCode.StartsWith(zoneCode));
            else if (searchOption == eLevelOption.SelfAndSubs)
                count = Count(c => c.ZoneCode.StartsWith(zoneCode));
            return count;
        }

        /// <summary>
        /// 根据匹配级别删除地域下的合同
        /// </summary>
        /// <param name="zoneCode">地域编码</param>
        /// <param name="searchOption">匹配级别</param>
        /// <returns>-1（参数错误）/0（失败）/1（成功）</returns>
        public int DeleteByZoneCode(string zoneCode, eLevelOption searchOption)
        {
            if (!CheckRule.CheckStringNullOrEmpty(ref zoneCode))
                return -1;

            int cnt = 0;

            if (searchOption == eLevelOption.Self)
                cnt = Delete(c => c.ZoneCode.Equals(zoneCode));
            else if (searchOption == eLevelOption.SelfAndSubs)
                cnt = Delete(c => c.ZoneCode.StartsWith(zoneCode));
            else
                cnt = Delete(c => c.ZoneCode.StartsWith(zoneCode) && c.ZoneCode != zoneCode);

            return cnt;
        }

        /// <summary>
        /// 删除当前地域下所有指定承包方状态的合同(包括相关联数据)
        /// </summary>
        public int DeleteRelationDataByZoneCode(string zoneCode, eVirtualPersonStatus virtualStatue, eLevelOption levelOption)
        {
            if (!CheckRule.CheckStringNullOrEmpty(ref zoneCode))
                return -1;

            int delAppend = 0;
            Guid[] vpIds = null;
            if (levelOption == eLevelOption.Self)
            {
                int count = DataSource.CreateQuery<LandVirtualPerson>().Where(c => c.ZoneCode == zoneCode && c.Status == virtualStatue).Count();
                if (count == 0)
                    return delAppend;
                vpIds = DataSource.CreateQuery<LandVirtualPerson>().Where(c => c.ZoneCode == zoneCode && c.Status == virtualStatue).Select(s => new { s.ID }).ToList().Select(c => c.ID).ToArray();
            }
            else if (levelOption == eLevelOption.SelfAndSubs)
            {
                int count = DataSource.CreateQuery<LandVirtualPerson>().Where(c => c.ZoneCode.StartsWith(zoneCode) && c.Status == virtualStatue).Count();
                if (count == 0)
                    return delAppend;
                vpIds = DataSource.CreateQuery<LandVirtualPerson>().Where(c => c.ZoneCode.StartsWith(zoneCode) && c.Status == virtualStatue).Select(s => new { s.ID }).ToList().Select(c => c.ID).ToArray();
            }
            else
            {
                int count = DataSource.CreateQuery<LandVirtualPerson>().Where(c => c.ZoneCode.StartsWith(zoneCode) && c.ZoneCode != zoneCode && c.Status == virtualStatue).Count();
                if (count == 0)
                    return delAppend;
                vpIds = DataSource.CreateQuery<LandVirtualPerson>().Where(c => c.ZoneCode.StartsWith(zoneCode) && c.ZoneCode != zoneCode && c.Status == virtualStatue).Select(s => new { s.ID }).ToList().Select(c => c.ID).ToArray();
            }
            if (vpIds == null || vpIds.Length == 0)
                return delAppend;

            Guid[] concordIds = null;
            concordIds = DataSource.CreateQuery<ContractConcord>().Where(c => vpIds.Contains((Guid)c.ContracterId)).Select(t => t.ID).ToArray();
            if (concordIds == null || concordIds.Length == 0)
                return delAppend;

            //删除承包合同
            delAppend = AppendEdit(DataSource.CreateQuery<ContractConcord>().Where(c => c.ContracterId != null && vpIds.Contains((Guid)c.ContracterId)).Delete());
            //删除权证
            delAppend = AppendEdit(DataSource.CreateQuery<ContractRegeditBook>().Where(c => concordIds.Contains(c.ID)).Delete());
            //删除申请书
            if (levelOption == eLevelOption.Self)
                delAppend = AppendEdit(DataSource.CreateQuery<ContractRequireTable>().Where(c => c.ZoneCode.Equals(zoneCode)).Delete());
            else if (levelOption == eLevelOption.SelfAndSubs)
                delAppend = AppendEdit(DataSource.CreateQuery<ContractRequireTable>().Where(c => c.ZoneCode.StartsWith(zoneCode)).Delete());
            else
                delAppend = AppendEdit(DataSource.CreateQuery<ContractRequireTable>().Where(c => c.ZoneCode != zoneCode && c.ZoneCode.StartsWith(zoneCode)).Delete());

            //更新承包地块
            delAppend = AppendEdit(DataSource.CreateQuery<ContractLand>().Where(c => concordIds.Contains((Guid)c.ConcordId)).Update(c => new ContractLand { ConcordId = null }));

            return delAppend;
        }

        /// <summary>
        /// 删除当前地域下所有指定承包方状态的合同
        /// </summary>
        /// <param name="zoneCode">地域代码</param>
        /// <param name="virtualStatue">承包方状态</param>
        /// <param name="levelOption">地域匹配等级</param>
        /// <returns>-1（参数错误）/0（失败）/1（成功）</returns>
        public int DeleteByZoneCode(string zoneCode, eVirtualPersonStatus virtualStatue, eLevelOption levelOption)
        {
            if (!CheckRule.CheckStringNullOrEmpty(ref zoneCode))
                return -1;

            int cnt = 0;
            int compare = 50;

            if (levelOption == eLevelOption.Self)
            {
                int count = DataSource.CreateQuery<LandVirtualPerson>().Where(c => c.ZoneCode == zoneCode && c.Status == virtualStatue).Count();//.Select(c => c.ID)
                if (count == 0)
                    return 0;
                var idsSelf = DataSource.CreateQuery<LandVirtualPerson>().Where(c => c.ZoneCode == zoneCode && c.Status == virtualStatue).Select(s => new { s.ID }).ToList().Select(c => c.ID).ToList();
                cnt = DeleteByVitualPersonStatue(compare, idsSelf);
            }
            else if (levelOption == eLevelOption.SelfAndSubs)
            {
                int count = DataSource.CreateQuery<LandVirtualPerson>().Where(c => c.ZoneCode.StartsWith(zoneCode) && c.Status == virtualStatue).Count();//.Select(c => c.ID)
                if (count == 0)
                    return 0;
                var idsSelfAndSubs = DataSource.CreateQuery<LandVirtualPerson>().Where(c => c.ZoneCode.StartsWith(zoneCode) && c.Status == virtualStatue).Select(s => new { s.ID }).ToList().Select(c => c.ID).ToList();
                cnt = DeleteByVitualPersonStatue(compare, idsSelfAndSubs);
            }
            else
            {
                int count = DataSource.CreateQuery<LandVirtualPerson>().Where(c => c.ZoneCode.StartsWith(zoneCode) && c.ZoneCode != zoneCode && c.Status == virtualStatue).Count();//.Select(c => c.ID)
                if (count == 0)
                    return 0;
                var idsSubs = DataSource.CreateQuery<LandVirtualPerson>().Where(c => c.ZoneCode.StartsWith(zoneCode) && c.ZoneCode != zoneCode && c.Status == virtualStatue).Select(s => new { s.ID }).ToList().Select(c => c.ID).ToList();
                cnt = DeleteByVitualPersonStatue(compare, idsSubs);
            }
            var res = DataSource.CreateQuery<ContractConcord>().Where(c => c.ZoneCode.StartsWith(zoneCode) && (c.ConcordNumber == null || c.ConcordNumber == string.Empty)).Delete().Save();

            return cnt;
        }

        /// <summary>
        /// 根据承包方状态删除数据
        /// </summary>
        /// <param name="compare">拼接Whrer语句最大个数(此数字过大会导致程序异常)</param>
        /// <param name="ids">查找的承包方id集合</param>
        private int DeleteByVitualPersonStatue(int compare, List<Guid> ids)
        {
            int cnt = 0;
            StringBuilder b = new StringBuilder();
            if (ids.Count > compare)
            {
                int count = ids.Count / compare;
                List<object> listObj = new List<object>();
                int index = 0;
                for (int i = 0; i < count; i++)
                {
                    int j = 0;
                    while (j < compare)
                    {
                        if (j == compare - 1)
                            b.AppendFormat("ContracterId == @{0}", j);
                        else
                            b.AppendFormat("ContracterId == @{0}" + " || ", j);
                        listObj.Add(ids[index]);
                        j++;
                        index++;
                    }
                    cnt = AppendEdit(DataSource.CreateQuery<ContractConcord>().Where(b.ToString(), listObj.ToArray()).Delete());
                    b.Clear();
                    listObj.Clear();
                }
                int other = ids.Count - (compare * count);
                if (other == 0)
                    return cnt;
                for (int i = 0; i < other; i++)
                {
                    listObj.Add(ids[index + i]);
                    if (i == other - 1)
                        b.AppendFormat("ContracterId == @{0}", i);
                    else
                        b.AppendFormat("ContracterId == @{0}" + " || ", i);
                }
                cnt = AppendEdit(DataSource.CreateQuery<ContractConcord>().Where(b.ToString(), listObj.ToArray()).Delete());
            }
            else
            {
                for (int i = 0; i < ids.Count; i++)
                {
                    if (i == ids.Count - 1)
                        b.AppendFormat("ContracterId == @{0}", i);
                    else
                        b.AppendFormat("ContracterId == @{0}" + " || ", i);
                }
                cnt = AppendEdit(DataSource.CreateQuery<ContractConcord>().Where(b.ToString(), ids.Cast<object>().ToArray()).Delete());
            }

            return cnt;
        }


        /// <summary>
        /// 根据承包方删除合同数据
        /// </summary>
        /// <param name="compare">拼接Whrer语句最大个数(此数字过大会导致程序异常)</param>
        /// <param name="ids">查找的承包方id集合</param>
        private int DeleteByVitualPerson(int compare, List<Guid> ids)
        {
            int cnt = 0;
            StringBuilder b = new StringBuilder();
            if (ids.Count > compare)
            {
                int count = ids.Count / compare;
                List<object> listObj = new List<object>();
                int index = 0;
                for (int i = 0; i < count; i++)
                {
                    int j = 0;
                    while (j < compare)
                    {
                        if (j == compare - 1)
                            b.AppendFormat("ContracterId != @{0}", j);
                        else
                            b.AppendFormat("ContracterId != @{0}" + " || ", j);
                        listObj.Add(ids[index]);
                        j++;
                        index++;
                    }
                    cnt = AppendEdit(DataSource.CreateQuery<ContractConcord>().Where(b.ToString(), listObj.ToArray()).Delete());
                    b.Clear();
                    listObj.Clear();
                }
                int other = ids.Count - (compare * count);
                if (other == 0)
                    return cnt;
                for (int i = 0; i < other; i++)
                {
                    listObj.Add(ids[index + i]);
                    if (i == other - 1)
                        b.AppendFormat("ContracterId != @{0}", i);
                    else
                        b.AppendFormat("ContracterId != @{0}" + " || ", i);
                }
                cnt = AppendEdit(DataSource.CreateQuery<ContractConcord>().Where(b.ToString(), listObj.ToArray()).Delete());
            }
            else
            {
                for (int i = 0; i < ids.Count; i++)
                {
                    if (i == ids.Count - 1)
                        b.AppendFormat("ContracterId != @{0}", i);
                    else
                        b.AppendFormat("ContracterId != @{0}" + " || ", i);
                }
                cnt = AppendEdit(DataSource.CreateQuery<ContractConcord>().Where(b.ToString(), ids.Cast<object>().ToArray()).Delete());
            }

            return cnt;
        }

        /// <summary>
        /// 删除当前地域下所有(指定承包方状态)的合同
        /// </summary>
        /// <param name="zoneCode">地域代码</param>
        /// <param name="levelOption">地域匹配等级</param>
        /// <returns>-1（参数错误）/0（失败）/1（成功）</returns>
        public int DeleteOtherByZoneCode(string zoneCode, eLevelOption levelOption)
        {
            if (!CheckRule.CheckStringNullOrEmpty(ref zoneCode))
                return -1;

            int cnt = 0;
            int compare = 50;

            if (levelOption == eLevelOption.Self)
            {
                int count = DataSource.CreateQuery<LandVirtualPerson>().Where(c => c.ZoneCode == zoneCode).Count();//.Select(c => c.ID)
                if (count == 0)
                    return Delete(c => c.ZoneCode == zoneCode);
                var idsSelf = DataSource.CreateQuery<LandVirtualPerson>().Where(c => c.ZoneCode == zoneCode).Select(s => new { s.ID }).ToList().Select(c => c.ID).ToList();
                cnt = DeleteByVitualPerson(compare, idsSelf);
            }
            else if (levelOption == eLevelOption.SelfAndSubs)
            {
                int count = DataSource.CreateQuery<LandVirtualPerson>().Where(c => c.ZoneCode.StartsWith(zoneCode)).Count();//.Select(c => c.ID)
                if (count == 0)
                    return Delete(c => c.ZoneCode.StartsWith(zoneCode));
                var idsSelfAndSubs = DataSource.CreateQuery<LandVirtualPerson>().Where(c => c.ZoneCode.StartsWith(zoneCode)).Select(s => new { s.ID }).ToList().Select(c => c.ID).ToList();
                cnt = DeleteByVitualPerson(compare, idsSelfAndSubs);
            }
            else
            {
                int count = DataSource.CreateQuery<LandVirtualPerson>().Where(c => c.ZoneCode.StartsWith(zoneCode) && c.ZoneCode != zoneCode).Count();//.Select(c => c.ID)
                if (count == 0)
                    return Delete(c => c.ZoneCode.StartsWith(zoneCode) && c.ZoneCode != zoneCode);
                var idsSubs = DataSource.CreateQuery<LandVirtualPerson>().Where(c => c.ZoneCode.StartsWith(zoneCode) && c.ZoneCode != zoneCode).Select(s => new { s.ID }).ToList().Select(c => c.ID).ToList();
                cnt = DeleteByVitualPerson(compare, idsSubs);
            }

            return cnt;
        }

        /// <summary>
        /// 根据承包方唯一标识删除农村土地承包合同
        /// </summary>
        /// <param name="guid">承包方唯一标识</param>
        /// <returns>-1（参数错误）/0（失败）/1（成功）</returns>
        public int DeleteByOwnerId(Guid ownerId)
        {
            if (!CheckRule.CheckGuidNullOrEmpty(ownerId))
                return 0;
            int count = 0;
            count = Delete(c => c.ContracterId.Equals(ownerId));
            return count;
        }

        /// <summary>
        /// 根据农村土地承包合同编号及其不同的查找类型来获得以承包方姓名排序的合同
        /// </summary>
        /// <param name="concordNumber">农村土地承包合同编号</param>
        /// <param name="searchOption">查找类型</param>
        /// <returns>合同</returns>
        public List<ContractConcord> SearchByConcordNumber(string concordNumber, eSearchOption searchOption)
        {
            if (!CheckTableExist())
            {
                throw new ArgumentNullException("数据库不存在表："
                    + this.GetType().ToString().Substring(this.GetType().ToString().LastIndexOf('.') + 1).Replace("Repository", ""));
            }
            if (!CheckRule.CheckStringNullOrEmpty(ref concordNumber))
                return null;

            object data = null;

            if (searchOption == eSearchOption.Precision)
                data = (from q in DataSource.CreateQuery<ContractConcord>()
                        where q.ConcordNumber.Equals(concordNumber)
                        orderby q.ContracterName
                        select q).ToList();
            else if (searchOption == eSearchOption.Fuzzy)
                data = (from q in DataSource.CreateQuery<ContractConcord>()
                        where q.ConcordNumber.Contains(concordNumber)
                        orderby q.ContracterName
                        select q).ToList();
            return (List<ContractConcord>)data;
        }

        /// <summary>
        /// 根据承包方姓名及其不同的查找类型来获得以承包方姓名排序的合同
        /// </summary>
        /// <param name="name">承包方姓名</param>
        /// <param name="searchOption">查找类型</param>
        /// <returns>合同</returns>
        public List<ContractConcord> SearchByVirtualPersonName(string name, eSearchOption searchOption)
        {
            if (!CheckTableExist())
            {
                throw new ArgumentNullException("数据库不存在表："
                    + this.GetType().ToString().Substring(this.GetType().ToString().LastIndexOf('.') + 1).Replace("Repository", ""));
            }
            if (!CheckRule.CheckStringNullOrEmpty(ref name))
                return null;
            object data = null;

            if (searchOption == eSearchOption.Precision)
                data = (from q in DataSource.CreateQuery<ContractConcord>()
                        where q.ConcordNumber.Equals(name)
                        orderby q.ContracterName
                        select q).ToList();
            else if (searchOption == eSearchOption.Fuzzy)
                data = (from q in DataSource.CreateQuery<ContractConcord>()
                        where q.ContracterName.Contains(name)
                        orderby q.ContracterName
                        select q).ToList();
            return (List<ContractConcord>)data;
        }



        /// <summary>
        /// 根据农村土地承包合同编号及其不同的查找类型来获得以目标区域代码开始的以承包方姓名排序的合同
        /// </summary>
        /// <param name="concordNumber">农村土地承包合同编号</param>
        /// <param name="searchOption">查找类型</param>
        /// <param name="zoneCode">目标区域代码</param>
        /// <returns>合同</returns>
        public List<ContractConcord> SearchByConcordNumber(string concordNumber, eSearchOption searchOption, string zoneCode)
        {
            if (!CheckTableExist())
            {
                throw new ArgumentNullException("数据库不存在表："
                    + this.GetType().ToString().Substring(this.GetType().ToString().LastIndexOf('.') + 1).Replace("Repository", ""));
            }
            if (!CheckRule.CheckStringNullOrEmpty(ref concordNumber) || !CheckRule.CheckStringNullOrEmpty(ref zoneCode))
                return null;

            object data = null;

            if (searchOption == eSearchOption.Precision)
                data = (from q in DataSource.CreateQuery<ContractConcord>()
                        where q.ConcordNumber.Equals(concordNumber) && q.ZoneCode.StartsWith(zoneCode)
                        orderby q.ContracterName
                        select q).ToList();
            else if (searchOption == eSearchOption.Fuzzy)
                data = (from q in DataSource.CreateQuery<ContractConcord>()
                        where q.ConcordNumber.Contains(concordNumber) && q.ZoneCode.StartsWith(zoneCode)
                        orderby q.ContracterName
                        select q).ToList(); Get(c => c.ConcordNumber.Contains(concordNumber) && c.ZoneCode.StartsWith(zoneCode));
            return (List<ContractConcord>)data;
        }


        /// <summary>
        /// 根据承包方姓名及其不同的查找类型来获得以目标区域代码开始的以承包方姓名排序的合同
        /// </summary>
        /// <param name="name">承包方姓名</param>
        /// <param name="searchOption">查找类型</param>
        /// <param name="zoneCode">目标区域代码</param>
        /// <returns>合同</returns>
        public List<ContractConcord> SearchByVirtualPersonName(string name, eSearchOption searchOption, string zoneCode)
        {
            if (!CheckTableExist())
            {
                throw new ArgumentNullException("数据库不存在表："
                    + this.GetType().ToString().Substring(this.GetType().ToString().LastIndexOf('.') + 1).Replace("Repository", ""));
            }
            if (string.IsNullOrEmpty(zoneCode))
                return SearchByVirtualPersonName(name, searchOption);

            object data = null;

            if (searchOption == eSearchOption.Precision)
                data = (from q in DataSource.CreateQuery<ContractConcord>()
                        where q.ContracterName.Equals(name) && q.ZoneCode.StartsWith(zoneCode)
                        orderby q.ContracterName
                        select q).ToList();
            else if (searchOption == eSearchOption.Fuzzy)
                data = (from q in DataSource.CreateQuery<ContractConcord>()
                        where q.ContracterName.Contains(name) && q.ZoneCode.StartsWith(zoneCode)
                        orderby q.ContracterName
                        select q).ToList();
            return (List<ContractConcord>)data;
        }

        /// <summary>
        /// 存在合同数据的地域集合
        /// </summary>
        /// <param name="zoneCode">地域集合</param>  
        public List<Zone> ExistZones(List<Zone> zoneList)
        {
            if (!CheckTableExist())
            {
                throw new ArgumentNullException("数据库不存在表："
                    + this.GetType().ToString().Substring(this.GetType().ToString().LastIndexOf('.') + 1).Replace("Repository", ""));
            }
            if (zoneList == null || zoneList.Count == 0)
            {
                return null;
            }
            var qc = DataSource.CreateQuery<ContractConcord>();
            var q1 = (from concord in qc
                      group concord by concord.ZoneCode into gp
                      select new { ZoneCode = gp.Key, Count = gp.Count() });
            var q2 = (from z in zoneList
                      join gp in q1 on z.FullCode equals gp.ZoneCode
                      where gp.Count > 0
                      select z).ToList();
            return q2;
        }

        #endregion



    }
}