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
    /// 集体建设用地使用权表的数据访问类
    /// </summary>
    public class ConstructionLandRepository : RepositoryDbContext<ConstructionLand>,  IConstructionLandRepository
    {
        #region Ctor
        
        private IDataSourceSchema m_DSSchema = null;

        public ConstructionLandRepository(IDataSource ds)
            : base(ds) 
        {
            m_DSSchema = ds.CreateSchema();
        }


        #endregion

        #region Fields

        private const string ORDERFIELD_DEFAULT = "HouseHolderName";

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
            //    return m_DSSchema.AnyElement(m_DSSchema.GetElements()[0].Schema, "ConstructionLand");
            //}
            //catch (Exception)
            //{
            //    return false;
            //}
            return true;
        }

        /// <summary>
        /// 根据权属单位代码获取数据
        /// </summary>
        /// <param name="codeZone">权属单位代码</param>
        /// <returns>目标对象集合</returns>
        public List<ContractLand> GetByOwnUnitCode(string ownUnitCode)
        {
            if (!CheckTableExist())
            {
                throw new ArgumentNullException("数据库不存在表："
                    + this.GetType().ToString().Substring(this.GetType().ToString().LastIndexOf('.') + 1).Replace("Repository", ""));
            }
            if (!CheckRule.CheckStringNullOrEmpty(ref ownUnitCode))
                return null;

            var q = from qc in DataSource.CreateQuery<ConstructionLand>()
                    where qc.SenderCode.Equals(ownUnitCode)
                    orderby qc.OwnerName
                    select qc;
            object data = q.ToList();
            return data as List<ContractLand>;
        }

        /// <summary>
        /// 根据权属单位代码、获取数据
        /// </summary>
        /// <param name="ownUnitCode">权属单位代码</param>
        /// <param name="levelOption">匹配等级</param>
        /// <param name="isValid">是否可用</param>
        /// <returns>目标对象集合</returns>
        public List<ContractLand> GetByOwnUnitCode(string ownUnitCode, eLevelOption levelOption, bool isValid)
        {

            if (!CheckTableExist())
            {
                throw new ArgumentNullException("数据库不存在表："
                    + this.GetType().ToString().Substring(this.GetType().ToString().LastIndexOf('.') + 1).Replace("Repository", ""));
            }
            if (!CheckRule.CheckStringNullOrEmpty(ref ownUnitCode))
                return null;
            
            object data = null;
            if (levelOption == eLevelOption.Self)
            {
                var q = from qc in DataSource.CreateQuery<ConstructionLand>()
                        where qc.SenderCode.StartsWith(ownUnitCode) && qc.IsValid.Equals(isValid)
                        orderby qc.OwnerName
                        select qc;
                data = q.ToList();
            }
            else
            {
                var q = from qc in DataSource.CreateQuery<ConstructionLand>()
                        where qc.SenderCode.Equals(ownUnitCode) && qc.IsValid.Equals(isValid)
                        orderby qc.OwnerName
                        select qc;
                data = q.ToList();
            }
            
            return data as List<ContractLand>;
        }

        /// <summary>
        /// 根据地籍号获取数据
        /// </summary>
        /// <param name="cadastralNumber">地籍号</param>
        /// <returns>目标对象集合</returns>
        public List<ContractLand> GetByCadastralNumber(string cadastralNumber)
        {

            if (!CheckTableExist())
            {
                throw new ArgumentNullException("数据库不存在表："
                    + this.GetType().ToString().Substring(this.GetType().ToString().LastIndexOf('.') + 1).Replace("Repository", ""));
            }
            if (!CheckRule.CheckStringNullOrEmpty(ref cadastralNumber))
                return null;

            object data = Get(c => c.CadastralNumber.Equals(cadastralNumber));
            return data as List<ContractLand>;
        }

        /// <summary>
        /// 根据承包方ID获取集体建设用地使用权集合
        /// </summary>
        /// <param name="familyID">承包方ID</param>
        /// <returns>集体建设用地使用权集合</returns>
        public List<ContractLand> GetByFamilyID(Guid familyID)
        {

            if (!CheckTableExist())
            {
                throw new ArgumentNullException("数据库不存在表："
                    + this.GetType().ToString().Substring(this.GetType().ToString().LastIndexOf('.') + 1).Replace("Repository", ""));
            }
            if (!CheckRule.CheckGuidNullOrEmpty(familyID))
                return null;

            object data = Get(c => c.OwnerId.Equals(familyID));

            return data as List<ContractLand>;
        }

        /// <summary>
        /// 统计集体建设用地使用权对象数量
        /// </summary>
        /// <param name="zoneCode">区域代码</param>
        /// <returns>-1（参数错误）/int 对象数量</returns>
        public int CountByZoneCode(string zoneCode)
        {
            if (!CheckTableExist())
            {
                throw new ArgumentNullException("数据库不存在表："
                    + this.GetType().ToString().Substring(this.GetType().ToString().LastIndexOf('.') + 1).Replace("Repository", ""));
            }
            if (!CheckRule.CheckStringNullOrEmpty(ref zoneCode))
                return -1;
            return Count(c => c.SenderCode.Equals(zoneCode));
        }

        /// <summary>
        /// 获取数据
        /// </summary>
        /// <param name="fullCode">权属单位代码</param>
        /// <returns>集体建设用地使用权集合</returns>
        public List<ContractLand> Get(string fullCode)
        {
            if (!CheckTableExist())
            {
                throw new ArgumentNullException("数据库不存在表："
                    + this.GetType().ToString().Substring(this.GetType().ToString().LastIndexOf('.') + 1).Replace("Repository", ""));
            }
            if (!CheckRule.CheckStringNullOrEmpty(ref fullCode))
                return null;

            object data = Get(c => c.SenderCode.Equals(fullCode));
            return data as List<ContractLand>;
        }

        /// <summary>
        /// 根据集体建设用地使用权ID获取数据
        /// </summary>
        /// <param name="buildLandPropertyID"></param>
        /// <returns>集体建设用地使用权集合</returns>
        public ConstructionLand Get(Guid buildLandPropertyID)
        {
            if (!CheckTableExist())
            {
                throw new ArgumentNullException("数据库不存在表："
                    + this.GetType().ToString().Substring(this.GetType().ToString().LastIndexOf('.') + 1).Replace("Repository", ""));
            }
            if (!CheckRule.CheckGuidNullOrEmpty(buildLandPropertyID))
                return null;

            object data = Get(c => c.ID.Equals(buildLandPropertyID));
            return data as ConstructionLand;
        }

        /// <summary>
        /// 判断宅基地是否存在
        /// </summary>
        /// <param name="guid">唯一标识</param>
        /// <returns>true（存在）/false（不存在）</returns>
        public bool Exists(Guid guid)
        {
            if (!CheckTableExist())
            {
                throw new ArgumentNullException("数据库不存在表："
                    + this.GetType().ToString().Substring(this.GetType().ToString().LastIndexOf('.') + 1).Replace("Repository", ""));
            }
            if (guid == null || guid == Guid.Empty)
                return false;
            return Count(c => c.ID.Equals(guid)) > 0;
        }

        /// <summary>
        /// 删除实体
        /// </summary>
        /// <param name="guid">标识</param>
        /// <returns>-1（参数错误）/0（失败）/1（成功）</returns>
        public int Delete(Guid guid)
        {
            if (!CheckTableExist())
            {
                throw new ArgumentNullException("数据库不存在表："
                    + this.GetType().ToString().Substring(this.GetType().ToString().LastIndexOf('.') + 1).Replace("Repository", ""));
            }
            if (guid == null || guid == Guid.Empty)
                return -1;

            return Delete(c => c.ID.Equals(guid));
        }

        /// <summary>
        /// 通过区域代码更新其可利用性为不可用
        /// </summary>
        /// <param name="zoneCode">区域代码</param>
        /// <returns>-1（参数错误）/0（失败）/1（成功）</returns>
        public int UpdateDataIsNotValidByZoneCode(string zoneCode)
        {
            if (!CheckTableExist())
            {
                throw new ArgumentNullException("数据库不存在表："
                    + this.GetType().ToString().Substring(this.GetType().ToString().LastIndexOf('.') + 1).Replace("Repository", ""));
            }
            if (!CheckRule.CheckStringNullOrEmpty(ref zoneCode))
                return -1;

            var q = from qc in DataSource.CreateQuery<ConstructionLand>()
                    where qc.SenderCode.Equals(zoneCode)
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
        /// 更新实体
        /// </summary>
        /// <param name="entity"></param>
        /// <returns>-1（参数错误）/0（失败）/1（成功）</returns>
        public int Update(ConstructionLand yard)
        {
            if (!CheckTableExist())
            {
                throw new ArgumentNullException("数据库不存在表："
                    + this.GetType().ToString().Substring(this.GetType().ToString().LastIndexOf('.') + 1).Replace("Repository", ""));
            }
            if (yard == null || yard.ID == Guid.Empty)
            {
                return -1;
            }
            yard.ModifiedTime = DateTime.Now;
            return Update(yard, c => c.ID.Equals(yard.ID));
        }

        /// <summary>
        /// 更新户主名称
        /// </summary>
        /// <param name="houseHolderID">承包方ID</param>
        /// <param name="houseHolderName">承包方名称</param>
        /// <returns>-1（参数错误）/0（失败）/1（成功）</returns>
        public int Update(Guid houseHolderID, string houseHolderName)
        {
            if (!CheckTableExist())
            {
                throw new ArgumentNullException("数据库不存在表："
                    + this.GetType().ToString().Substring(this.GetType().ToString().LastIndexOf('.') + 1).Replace("Repository", ""));
            }
            if (!CheckRule.CheckStringNullOrEmpty(ref houseHolderName) || !CheckRule.CheckGuidNullOrEmpty(houseHolderID))
                return -1;

            var q = from qc in DataSource.CreateQuery<ConstructionLand>()
                    where qc.OwnerId.Equals(houseHolderID)
                    select qc;
            foreach (var item in q.ToList())
            {
                item.OwnerName = houseHolderName;
                if (Update(item, c => c.ID.Equals(item.ID)) == 0)
                    return 0;
            }
            return 1;
        }

        /// <summary>
        /// 删除地域代码下所有宅基地
        /// </summary>
        /// <param name="fullCode">地域代码</param>
        /// <returns>-1（参数错误）/0（失败）/1（成功）</returns>
        public int DeleteByZoneCode(string fullCode)
        {
            if (!CheckTableExist())
            {
                throw new ArgumentNullException("数据库不存在表："
                    + this.GetType().ToString().Substring(this.GetType().ToString().LastIndexOf('.') + 1).Replace("Repository", ""));
            }
            if (!CheckRule.CheckStringNullOrEmpty(ref fullCode))
                return -1;
            return Delete(c => c.SenderCode.Equals(fullCode));
        }

        /// <summary>
        /// 根据地籍号判断集体建设用地使用权是否存在
        /// </summary>
        /// <param name="number">地籍号</param>
        /// <returns>true（存在）/false（不存在）</returns>
        public bool ExistsByNumber(string number)
        {
            if (!CheckTableExist())
            {
                throw new ArgumentNullException("数据库不存在表："
                    + this.GetType().ToString().Substring(this.GetType().ToString().LastIndexOf('.') + 1).Replace("Repository", ""));
            }
            if (!CheckRule.CheckStringNullOrEmpty(ref number))
                return false;

            return Count(c => c.CadastralNumber.EndsWith(number)) > 0;
        }

        /// <summary>
        /// 根据地籍号获得集体建设用地使用权
        /// </summary>
        /// <param name="number">地籍号</param>
        /// <returns>集体建设用地使用权</returns>
        public ConstructionLand GetByNumber(string number)
        {
            if (!CheckTableExist())
            {
                throw new ArgumentNullException("数据库不存在表："
                    + this.GetType().ToString().Substring(this.GetType().ToString().LastIndexOf('.') + 1).Replace("Repository", ""));
            }
            if (!CheckRule.CheckStringNullOrEmpty(ref number)) 
                return null;

            object entity = Get(c => c.CadastralNumber.EndsWith(number));
            return entity as ConstructionLand;
        }

        /// <summary>
        /// 根据地籍号判断集体建设用地使用权是否存在
        /// </summary>
        /// <param name="id">标识码</param>
        /// <param name="landNumber">宗地号</param>
        /// <returns>true（存在）/false（不存在）</returns>
        public bool ExistsToLandNumber(Guid id, string landNumber)
        {
            if (!CheckTableExist())
            {
                throw new ArgumentNullException("数据库不存在表："
                    + this.GetType().ToString().Substring(this.GetType().ToString().LastIndexOf('.') + 1).Replace("Repository", ""));
            }
            if (!CheckRule.CheckStringNullOrEmpty(ref landNumber) || !CheckRule.CheckGuidNullOrEmpty(id)) 
                return false;

            if (Count(c=>c.LandNumber.Equals(landNumber))>0)
                if (Count(c=>(!c.ID.Equals(id))&&c.LandNumber.Equals(landNumber)) != 0)
                    return true;

            return false;
        }

       
        /// <summary>
        /// 根据地籍号获得集体建设用地使用权集合
        /// </summary>
        /// <param name="number">地籍号</param>
        /// <param name="searchOption">查找类型</param>
        /// <returns>集体建设用地使用权集合</returns>
        public List<ContractLand> SearchByNumber(string number, eSearchOption searchOption)
        {
            if (!CheckTableExist())
            {
                throw new ArgumentNullException("数据库不存在表："
                    + this.GetType().ToString().Substring(this.GetType().ToString().LastIndexOf('.') + 1).Replace("Repository", ""));
            }
            if (!CheckRule.CheckStringNullOrEmpty(ref number))
                return null;

            object data = null;

            if (searchOption == eSearchOption.Precision)
            {
                var q=from qc in DataSource.CreateQuery<ConstructionLand>()
                      where qc.CadastralNumber.EndsWith(number)
                      orderby qc.OwnerName
                      select qc;
                data = q.ToList();
            }
            else if (searchOption == eSearchOption.Fuzzy)
            {
                
                var q=from qc in DataSource.CreateQuery<ConstructionLand>()
                      where qc.CadastralNumber.Contains(number)
                      orderby qc.OwnerName
                      select qc;
                data = q.ToList();
            }

            return (List<ContractLand>)data;
        }

        /// <summary>
        /// 通过承包方名称获得集体建设用地使用权集合
        /// </summary>
        /// <param name="name">承包方名称</param>
        /// <param name="searchOption">查找类型</param>
        /// <returns>集体建设用地使用权集合</returns>
        public List<ContractLand> SearchByVirtualPersonName(string name, eSearchOption searchOption)
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
            {
                var q=from qc in DataSource.CreateQuery<ConstructionLand>()
                      where qc.OwnerName.Equals(name)
                      orderby qc.OwnerName
                      select qc;
                data = q.ToList();
            }
            else if (searchOption == eSearchOption.Fuzzy) 
            {                
                var q=from qc in DataSource.CreateQuery<ConstructionLand>()
                      where qc.OwnerName.Contains(name)
                      orderby qc.OwnerName
                      select qc;
                data = q.ToList();
            }
            return (List<ContractLand>)data;
        }

        /// <summary>
        /// 通过地籍号、区域代码获得集体建设用地使用权集合
        /// </summary>
        /// <param name="number">地籍号</param>
        /// <param name="searchOption">查找类型</param>
        /// <param name="zoneCode">区域代码</param>
        /// <returns>集体建设用地使用权集合</returns>
        public List<ContractLand> SearchByNumber(string number, eSearchOption searchOption, string zoneCode)
        {

            if (!CheckTableExist())
            {
                throw new ArgumentNullException("数据库不存在表："
                    + this.GetType().ToString().Substring(this.GetType().ToString().LastIndexOf('.') + 1).Replace("Repository", ""));
            }
            if (!CheckRule.CheckStringNullOrEmpty(ref number))
                return null;

            if (string.IsNullOrEmpty(zoneCode))
                return SearchByNumber(number, searchOption);

            object data = null;

            if (searchOption == eSearchOption.Precision)
            {
                var q=from qc in DataSource.CreateQuery<ConstructionLand>()
                      where qc.CadastralNumber.EndsWith(number) && qc.SenderCode.StartsWith(zoneCode)
                      orderby qc.OwnerName
                      select qc;
                data = q.ToList();
            }
            else if (searchOption == eSearchOption.Fuzzy)
            {                
                var q=from qc in DataSource.CreateQuery<ConstructionLand>()
                      where qc.CadastralNumber.Contains(number) && qc.SenderCode.StartsWith(zoneCode)
                      orderby qc.OwnerName
                      select qc;
                data = q.ToList();
            }
            return (List<ContractLand>)data;
        }

        /// <summary>
        /// 通过承包方名称、区域代码获得集体建设用地使用权集合
        /// </summary>
        /// <param name="name">承包方名称</param>
        /// <param name="searchOption">查找类型</param>
        /// <param name="zoneCode">区域代码</param>
        /// <returns>集体建设用地使用权集合</returns>
        public List<ContractLand> SearchByVirtualPersonName(string name, eSearchOption searchOption, string zoneCode)
        {
            if (!CheckTableExist())
            {
                throw new ArgumentNullException("数据库不存在表："
                    + this.GetType().ToString().Substring(this.GetType().ToString().LastIndexOf('.') + 1).Replace("Repository", ""));
            }

            if (!CheckRule.CheckStringNullOrEmpty(ref name))
                return null;

            if (string.IsNullOrEmpty(zoneCode))
                return SearchByVirtualPersonName(name, searchOption);

            object data = null;

            if (searchOption == eSearchOption.Precision)
            {
                 var q=from qc in DataSource.CreateQuery<ConstructionLand>()
                      where qc.OwnerName.Equals(name) && qc.SenderCode.StartsWith(zoneCode)
                      orderby qc.OwnerName
                      select qc;
                data = q.ToList();
            }
            else if (searchOption == eSearchOption.Fuzzy) 
            {                
                var q=from qc in DataSource.CreateQuery<ConstructionLand>()
                      where qc.OwnerName.Contains(name) && qc.SenderCode.StartsWith(zoneCode)
                      orderby qc.OwnerName
                      select qc;
                data = q.ToList();
            }
            return (List<ContractLand>)data;
        }

        /// <summary>
        /// 获取集体建设用地使用权确权审批表编号最大值
        /// </summary>
        /// <returns>-1（不存在）/int 表中集体建设用地使用权确权审批表编号最大值</returns>
        public int GetMaxNumber()
        {
            if (!CheckTableExist())
            {
                throw new ArgumentNullException("数据库不存在表："
                    + this.GetType().ToString().Substring(this.GetType().ToString().LastIndexOf('.') + 1).Replace("Repository", ""));
            }
            var qc = DataSource.CreateQuery<ConstructionLand>();
            var q = (from qa in qc
                     where !string.IsNullOrEmpty(qa.ExtendB)
                     orderby int.Parse(qa.ExtendB) descending
                     select qa).FirstOrDefault();
            if (q == null)
                return -1;
            else
                return int.Parse(q.ExtendB);

        }

        #endregion
    }
}