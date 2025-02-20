// (C) 2025 鱼鳞图公司版权所有，保留所有权利
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
    /// 集体建设用地使用权的数据访问类
    /// </summary>
    public class BuildLandPropertyRepository : RepositoryDbContext<BuildLandProperty>, IBuildLandPropertyRepository
    {
        #region Ctor
        
        private IDataSourceSchema m_DSSchema = null;

        public BuildLandPropertyRepository(IDataSource ds)
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
            //    return m_DSSchema.AnyElement(m_DSSchema.GetElements()[0].Schema, "BuildLandProperty");
            //}
            //catch (Exception)
            //{
            //    return false;
            //}
            return true;
        }

        /// <summary>
        /// 根据权属单位代码获取集体建设用地使用权对象数据
        /// </summary>
        /// <param name="codeZone">权属单位代码</param>
        /// <returns>根据承包方名称排序的集体建设用地使用权对象</returns>
        public BuildLandPropertyCollection GetByOwnUnitCode(string ownUnitCode)
        {
            if (!CheckTableExist())
            {
                throw new ArgumentNullException("数据库不存在表："
                    + this.GetType().ToString().Substring(this.GetType().ToString().LastIndexOf('.') + 1).Replace("Repository", ""));
            }
            if (!CheckRule.CheckStringNullOrEmpty(ref ownUnitCode))
                return null;

            var q = (from qa in DataSource.CreateQuery<BuildLandProperty>()
                    where qa.ZoneCode.Equals(ownUnitCode)
                    orderby qa.HouseHolderName
                    select qa).ToList<BuildLandProperty>();
            var data = q;
            return data as BuildLandPropertyCollection;
        }

        /// <summary>
        /// 根据权属单位代码、是否可用获取集体建设用地使用权对象数据
        /// </summary>
        /// <param name="ownUnitCode">根据权属单位代码</param>
        /// <param name="levelOption">匹配级别</param>
        /// <param name="isValid">是否可用</param>
        /// <returns>根据承包方名称排序的目标对象集合</returns>
        public BuildLandPropertyCollection GetByOwnUnitCode(string ownUnitCode, eLevelOption levelOption, bool isValid)
        {
            if (!CheckTableExist())
            {
                throw new ArgumentNullException("数据库不存在表："
                    + this.GetType().ToString().Substring(this.GetType().ToString().LastIndexOf('.') + 1).Replace("Repository", ""));
            }
            if (!CheckRule.CheckStringNullOrEmpty(ref ownUnitCode))
                return null;


            object data = null;
            if (levelOption == eLevelOption.Subs)
                data =(from q in DataSource.CreateQuery<BuildLandProperty>()
                        where q.ZoneCode.StartsWith(ownUnitCode) && q.IsValid.Equals(isValid)
                        orderby q.HouseHolderName
                        select q).ToList();
            else
                data =(from q in DataSource.CreateQuery<BuildLandProperty>()
                        where q.ZoneCode.Equals(ownUnitCode) && q.IsValid.Equals(isValid)
                        orderby q.HouseHolderName
                        select q).ToList();

            return data as BuildLandPropertyCollection;
        }

        /// <summary>
        /// 根据地籍号获取集体建设用地使用权对象数据
        /// </summary>
        /// <param name="cadastralNumber">地籍号</param>
        /// <returns>目标对象集合</returns>
        public BuildLandPropertyCollection GetByCadastralNumber(string cadastralNumber)
        {
            if (!CheckTableExist())
            {
                throw new ArgumentNullException("数据库不存在表："
                    + this.GetType().ToString().Substring(this.GetType().ToString().LastIndexOf('.') + 1).Replace("Repository", ""));
            }
            if (!CheckRule.CheckStringNullOrEmpty(ref cadastralNumber))
                return null;
            object data = Get(c => c.CadastralNumber.Equals(cadastralNumber));
            return data as BuildLandPropertyCollection;
        }
        
        /// <summary>
        /// 根据承包方ID获取集体建设用地使用权对象数据
        /// </summary>
        /// <param name="familyID">承包方ID</param>
        /// <returns>目标对象集合</returns>
        public BuildLandPropertyCollection GetByFamilyID(Guid familyID)
        {
            if (!CheckTableExist())
            {
                throw new ArgumentNullException("数据库不存在表："
                    + this.GetType().ToString().Substring(this.GetType().ToString().LastIndexOf('.') + 1).Replace("Repository", ""));
            }
            if (!CheckRule.CheckGuidNullOrEmpty(familyID))
                return null;
            object data = Get(c => c.HouseHolderID.Equals(familyID));
            return data as BuildLandPropertyCollection;
        }

        /// <summary>
        /// 根据权属单位代码、户主名称获取地块
        /// </summary>
        ///<param name="zoneCode">根据权属单位代码</param>
        /// <param name="householderName">承包方名称</param>
        /// <returns>根据地籍号排序的目标对象集合</returns>
        public BuildLandPropertyCollection GetByHouseholderName(string zoneCode, string householderName)
        {
            if (!CheckTableExist())
            {
                throw new ArgumentNullException("数据库不存在表："
                    + this.GetType().ToString().Substring(this.GetType().ToString().LastIndexOf('.') + 1).Replace("Repository", ""));
            }
            if (!CheckRule.CheckStringNullOrEmpty(ref zoneCode) || !CheckRule.CheckStringNullOrEmpty(ref householderName))
                return null;
            object entity =(from q in DataSource.CreateQuery<BuildLandProperty>()
                            where q.ZoneCode.Equals(zoneCode) && q.HouseHolderName.Equals(householderName)
                            orderby q.CadastralNumber
                            select q).ToList();
            return entity as BuildLandPropertyCollection;
        }

        /// <summary>
        /// 根据权属单位代码统计集体建设用地使用权对象数据
        /// </summary>
        /// <param name="zoneCode">权属单位代码</param>
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
            return Count(c => c.ZoneCode.Equals(zoneCode));
        }

        /// <summary>
        /// 集体建设用地使用权对象数据
        /// </summary>
        /// <param name="fullCode">权属单位代码</param>
        /// <returns>目标对象集合</returns>
        public BuildLandPropertyCollection Get(string fullCode)
        {
            if (!CheckTableExist())
            {
                throw new ArgumentNullException("数据库不存在表："
                    + this.GetType().ToString().Substring(this.GetType().ToString().LastIndexOf('.') + 1).Replace("Repository", ""));
            }
            if (!CheckRule.CheckStringNullOrEmpty(ref fullCode))
                return null;
            object data = Get(c => c.ZoneCode.Equals(fullCode));
            return data as BuildLandPropertyCollection;
        }

        /// <summary>
        /// 获取集体建设用地使用权
        /// </summary>
        /// <param name="buildLandPropertyID">标识码</param>
        /// <returns>目标对象</returns>
        public BuildLandProperty Get(Guid buildLandPropertyID)
        {
            if (!CheckTableExist())
            {
                throw new ArgumentNullException("数据库不存在表："
                    + this.GetType().ToString().Substring(this.GetType().ToString().LastIndexOf('.') + 1).Replace("Repository", ""));
            }
            if (!CheckRule.CheckGuidNullOrEmpty(buildLandPropertyID))
                return null;
            object data = Get(c => c.ID.Equals(buildLandPropertyID));
            return data as BuildLandProperty;
        }

        /// <summary>
        /// 判断宅基地是否存在
        /// </summary>
        /// <param name="guid">标识码</param>
        /// <returns>true（存在）/false（不存在）</returns>
        public bool Exists(Guid guid)
        {
            if (!CheckTableExist())
            {
                throw new ArgumentNullException("数据库不存在表："
                    + this.GetType().ToString().Substring(this.GetType().ToString().LastIndexOf('.') + 1).Replace("Repository", ""));
            }
            if (!CheckRule.CheckGuidNullOrEmpty(guid))
                return false ;
            int count = Count(c => c.ID.Equals(guid));
            return count > 0 ? true : false;
        }

        /// <summary>
        /// 删除实体
        /// </summary>
        /// <param name="guid">标识码</param>
        /// <returns>-1（参数错误）/0（失败）/1（成功）</returns>
        public int Delete(Guid guid)
        {
            if (!CheckTableExist())
            {
                throw new ArgumentNullException("数据库不存在表："
                    + this.GetType().ToString().Substring(this.GetType().ToString().LastIndexOf('.') + 1).Replace("Repository", ""));
            }
            if (guid == null || guid == Guid.Empty)
                return 0;

            return Delete(c => c.ID.Equals(guid));
        }

        /// <summary>
        /// 根据权属单位代码更新集体建设用地使用权对象数据
        /// </summary>
        /// <param name="zoneCode">权属单位代码</param>
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

            var data = Get(c => c.ZoneCode.Equals(zoneCode));
            foreach (var item in data)
            {
                item.IsValid=false;
                if(Update(item, c => c.ID.Equals(item.ID)) == 0)
                    return 0;
            }
            return 1;
        }

        /// <summary>
        /// 更新集体建设用地使用权对象数据
        /// </summary>
        /// <param name="yard">集体建设用地使用权对象</param>
        /// <returns>-1（参数错误）/0（失败）/1（成功）</returns>
        public int Update(BuildLandProperty yard)
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
            if (houseHolderID == null)
                return -1;
            if (!CheckRule.CheckStringNullOrEmpty(ref houseHolderName))
                return -1;

            var data = Get(c => c.HouseHolderID.Equals(houseHolderID));
            foreach (var item in data)
            {
                item.HouseHolderName = houseHolderName;
                if (Update(item, c => c.HouseHolderID.Equals(item.HouseHolderID)) == 0)
                    return 0;
            }
            return 1;
        }

        /// <summary>
        /// 删除权属单位代码下所有集体建设用地使用权对象
        /// </summary>
        /// <param name="fullCode">权属单位代码</param>
        /// <returns>-1（参数错误）/0（失败）/1（成功）</returns>
        public int DeleteByZoneCode(string fullCode)
        {
            if (!CheckTableExist())
            {
                throw new ArgumentNullException("数据库不存在表："
                    + this.GetType().ToString().Substring(this.GetType().ToString().LastIndexOf('.') + 1).Replace("Repository", ""));
            }
            if (!CheckRule.CheckStringNullOrEmpty(ref fullCode))
            {
                return -1;
            }

            return Delete(c => c.ZoneCode.Equals(fullCode));
        }

        /// <summary>
        /// 是否存在以末尾包含目标地籍号的集体建设用地使用权对象
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
        /// 根据地籍号获取集体建设用地使用权对象
        /// </summary>
        /// <param name="number">地籍号</param>
        /// <returns>目标对象</returns>
        public BuildLandProperty GetByNumber(string number)
        {
            if (!CheckTableExist())
            {
                throw new ArgumentNullException("数据库不存在表："
                    + this.GetType().ToString().Substring(this.GetType().ToString().LastIndexOf('.') + 1).Replace("Repository", ""));
            }
            if (!CheckRule.CheckStringNullOrEmpty(ref number))
                return null;

            object entity = Get(c => c.CadastralNumber.EndsWith(number));
            return entity as BuildLandProperty;
        }

        /// <summary>
        /// 根据标识码与宗地号判断是否存在集体建设用地使用权对象
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
            if (!CheckRule.CheckGuidNullOrEmpty(id) || !CheckRule.CheckStringNullOrEmpty(ref landNumber))
                return false;


            if (Count(c => c.LandNumber.Equals(landNumber)) > 0)
                return Count(c => (!c.ID.Equals(id)) && c.LandNumber.Equals(landNumber)) != 0;
            else
                return false;
        }      

        /// <summary>
        /// 通过地籍号查找集体建设用地使用权对象
        /// </summary>
        /// <param name="number">地籍号</param>
        /// <param name="searchType">查找类型</param>
        /// <returns>根据承包方名称排序的目标对象集合</returns>
        public BuildLandPropertyCollection SearchByNumber(string number, eSearchOption searchType)
        {
            if (!CheckTableExist())
            {
                throw new ArgumentNullException("数据库不存在表："
                    + this.GetType().ToString().Substring(this.GetType().ToString().LastIndexOf('.') + 1).Replace("Repository", ""));
            }
            if ( !CheckRule.CheckStringNullOrEmpty(ref number))
                return null;

            object data = null;
            var qb = DataSource.CreateQuery<BuildLandProperty>();
            if (searchType == eSearchOption.Precision)
            {
                var q = from qa in qb
                        where qa.CadastralNumber.EndsWith(number)
                        orderby qa.HouseHolderName
                        select qa;
                data = q.ToList();
            }

            else if (searchType == eSearchOption.Fuzzy)
            {
                var q = from qa in qb
                        where qa.CadastralNumber.Contains(number)
                        orderby qa.HouseHolderName
                        select qa;
                data = q.ToList();
            }

            return (BuildLandPropertyCollection)data;
        }

        /// <summary>
        /// 通过承包方名称查找集体建设用地使用权对象
        /// </summary>
        /// <param name="name">承包方名称</param>
        /// <param name="searchType">查找类型</param>
        /// <returns>根据承包方名称排序的目标对象集合</returns>
        public BuildLandPropertyCollection SearchByVirtualPersonName(string name, eSearchOption searchType)
        {
            if (!CheckTableExist())
            {
                throw new ArgumentNullException("数据库不存在表："
                    + this.GetType().ToString().Substring(this.GetType().ToString().LastIndexOf('.') + 1).Replace("Repository", ""));
            }
            if (!CheckRule.CheckStringNullOrEmpty(ref name))
                return null;


            object data = null;
            var qb = DataSource.CreateQuery<BuildLandProperty>();
            if (searchType == eSearchOption.Precision)
            {
                var q = from qa in qb
                        where qa.HouseHolderName.Equals(name)
                        orderby qa.HouseHolderName
                        select qa;
                data = q.ToList();
            }
            else if (searchType == eSearchOption.Fuzzy)
            {
                var q = from qa in qb
                        where qa.HouseHolderName.Contains(name)
                        orderby qa.HouseHolderName
                        select qa;
                data = q.ToList();
            }
            return (BuildLandPropertyCollection)data;
        }

        /// <summary>
        /// 通过地籍号、权属单位代码查找集体建设用地使用权对象
        /// </summary>
        /// <param name="number">地籍号</param>
        /// <param name="searchType">查找类型（精确：地籍号右相似、权属单位代码左相似；模糊：地籍号包含、权属单位代码左相似）</param>
        /// <param name="zoneCode">权属单位代码</param>
        /// <returns>根据承包方名称排序的目标对象集合</returns>
        public BuildLandPropertyCollection SearchByNumber(string number, eSearchOption searchType, string zoneCode)
        {
            if (!CheckTableExist())
            {
                throw new ArgumentNullException("数据库不存在表："
                    + this.GetType().ToString().Substring(this.GetType().ToString().LastIndexOf('.') + 1).Replace("Repository", ""));
            }
            if (string.IsNullOrEmpty(number))
                throw new ArgumentNullException("number");

            if (string.IsNullOrEmpty(zoneCode))
                return SearchByNumber(number, searchType);

            object data = null;
            var qb = DataSource.CreateQuery<BuildLandProperty>();
            if (searchType == eSearchOption.Precision)
            {
                var q = from qa in qb
                        where qa.CadastralNumber.EndsWith(number) && qa.ZoneCode.StartsWith(zoneCode)
                        orderby qa.HouseHolderName
                        select qa;
                data = q.ToList();
            }
            else if (searchType == eSearchOption.Fuzzy)
            {
                var q = from qa in qb
                        where qa.CadastralNumber.Contains(number) && qa.ZoneCode.StartsWith(zoneCode)
                        orderby qa.HouseHolderName
                        select qa;
                data = q.ToList();

            }
            return (BuildLandPropertyCollection)data;
        }

        /// <summary>
        /// 通过承包方名称、权属单位代码查找集体建设用地使用权对象
        /// </summary>
        /// <param name="name">承包方名称</param>
        /// <param name="searchType">查找类型（精确：承包方名称相同、权属单位代码左相似；模糊：承包方名称包含、权属单位代码左相似）</param>
        /// <param name="zoneCode">权属单位代码</param>
        /// <returns>根据承包方名称排序的目标对象集合</returns>
        public BuildLandPropertyCollection SearchByVirtualPersonName(string name, eSearchOption searchType, string zoneCode)
        {
            if (!CheckTableExist())
            {
                throw new ArgumentNullException("数据库不存在表："
                    + this.GetType().ToString().Substring(this.GetType().ToString().LastIndexOf('.') + 1).Replace("Repository", ""));
            }
            if (!CheckRule.CheckStringNullOrEmpty(ref name) || !CheckRule.CheckStringNullOrEmpty(ref zoneCode))
                return SearchByVirtualPersonName(name, searchType);

            object data = null;
            var qb = DataSource.CreateQuery<BuildLandProperty>();
            if (searchType == eSearchOption.Precision)
            {
                var q = from qa in qb
                        where qa.HouseHolderName.Equals(name) && qa.ZoneCode.StartsWith(zoneCode)
                        orderby qa.HouseHolderName
                        select qa;
                data = q.ToList();
            }
            else if (searchType == eSearchOption.Fuzzy)
            {
                var q = from qa in qb
                        where qa.HouseHolderName.Contains(name) && qa.ZoneCode.StartsWith(zoneCode)
                        orderby qa.HouseHolderName
                        select qa;
                data = q.ToList();
            }   
            return (BuildLandPropertyCollection)data;
        }

        /// <summary>
        /// 获取集体建设用地使用权确权审批表编号的最大值
        /// </summary>
        /// <returns>编号的最大值</returns>
        public int GetMaxNumber()
        {
            if (!CheckTableExist())
            {
                throw new ArgumentNullException("数据库不存在表："
                    + this.GetType().ToString().Substring(this.GetType().ToString().LastIndexOf('.') + 1).Replace("Repository", ""));
            }
            var qb=DataSource.CreateQuery<BuildLandProperty>();
            var q = (from qa in qb
                     where !string.IsNullOrEmpty(qa.ExtendB)
                     orderby int.Parse(qa.ExtendB) descending
                     select qa).FirstOrDefault();

            return int.Parse(q.ExtendB);
        }

        #endregion
    }
}