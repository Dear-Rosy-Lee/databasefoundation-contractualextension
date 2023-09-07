// (C) 2015 鱼鳞图公司版权所有，保留所有权利
using System;
using YuLinTu.Library.Entity;
using System.Linq;
using System.Data;
using System.Data.SqlClient;
using System.Collections.Generic;
using YuLinTu.Data;
using YuLinTu;

namespace YuLinTu.Library.Repository
{
    /// <summary>
    /// 农村土地承包地
    /// </summary>
    public class SecondTebleLandRepository : RepositoryDbContext<SecondTebleLand>, ISecondTebleLandRepository
    {
        #region Ctor

        static SecondTebleLandRepository()
        {
            new SecondTebleLand().ToString();
            LanguageAttribute.AddLanguage(Properties.Resources.langChs);
        }


        private IDataSourceSchema m_DSSchema = null;

        public SecondTebleLandRepository(IDataSource ds)
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
            try
            {
                return m_DSSchema.AnyElement(m_DSSchema.GetElements()[0].Schema, "SecondTebleLand");
            }
            catch (Exception)
            {
                return false;
            }
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
            if (entity == null)
                return null;

            int cnt = base.Add(entity);

            if (cnt > 0)
                return entity.ToString();

            return null;
        }

        /// <summary>
        /// 删除对象
        /// </summary>
        public int Delete(Guid guid)
        {
            if (!CheckTableExist())
            {
                throw new ArgumentNullException("数据库不存在表："
                    + this.GetType().ToString().Substring(this.GetType().ToString().LastIndexOf('.') + 1).Replace("Repository", ""));
            }
            if (guid == null)
                return 0;
            return Delete(c => c.ID.Equals(guid));
        }

        /// <summary>
        /// 更新对象
        /// </summary>
        public int Update(SecondTebleLand entity)
        {
            if (!CheckTableExist())
            {
                throw new ArgumentNullException("数据库不存在表："
                    + this.GetType().ToString().Substring(this.GetType().ToString().LastIndexOf('.') + 1).Replace("Repository", ""));
            }
            if (entity == null)
                return 0;
            entity.ModifiedTime = DateTime.Now;
            return Update(entity, c => c.ID.Equals(entity.ID));
        }

        /// <summary>
        /// 获取对象
        /// </summary>
        public SecondTebleLand Get(Guid guid)
        {
            if (!CheckTableExist())
            {
                throw new ArgumentNullException("数据库不存在表："
                    + this.GetType().ToString().Substring(this.GetType().ToString().LastIndexOf('.') + 1).Replace("Repository", ""));
            }
            if (!CheckRule.CheckGuidNullOrEmpty(guid))
                return null;
            object entity = Get(c => c.ID.Equals(guid));
            return entity as SecondTebleLand;
        }

        /// <summary>
        /// 根据标识码判断土地承包地是否存在
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
        #endregion

        #region ExtendMethod

        /// <summary>
        /// 根据地籍号判断土地承包地对象是否存在
        /// </summary>
        /// <param name="cadastralNumber">地籍号</param>
        /// <returns></returns>
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

        /// <summary>
        /// 根据权利人id获取土地承包地
        /// </summary>
        /// <param name="familyID">权利人id</param>
        /// <returns>土地承包地对象</returns>
        public SecondTebleLandCollection GetCollection(Guid familyID)
        {
            if (!CheckTableExist())
            {
                throw new ArgumentNullException("数据库不存在表："
                    + this.GetType().ToString().Substring(this.GetType().ToString().LastIndexOf('.') + 1).Replace("Repository", ""));
            }
            if (!CheckRule.CheckGuidNullOrEmpty(familyID))
                return null;

            object entity = Get(c => c.HouseHolderId.Equals(familyID));
            return entity as SecondTebleLandCollection;
        }

        /// <summary>
        /// 根据土地是否流转获取土地承包地
        /// </summary>
        /// <param name="isTransfer">是否流转</param>
        /// <returns>土地承包地对象</returns>
        public SecondTebleLandCollection GetByIsTransfer(bool isTransfer)
        {
            if (!CheckTableExist())
            {
                throw new ArgumentNullException("数据库不存在表："
                    + this.GetType().ToString().Substring(this.GetType().ToString().LastIndexOf('.') + 1).Replace("Repository", ""));
            }
            object entity = Get(c => c.IsTransfer.Equals(isTransfer));
            return entity as SecondTebleLandCollection;
        }

        /// <summary>
        /// 根据土地是否流转获取指定区域下的土地承包地
        /// </summary>
        /// <param name="zoneCode">区域代码</param>
        /// <param name="isTransfer">是否流转</param>
        /// <returns>土地承包地；如果区域代码为null，返回所有区域下符合要求的对象</returns>
        public SecondTebleLandCollection GetByIsTransfer(string zoneCode, bool isTransfer)
        {
            if (!CheckTableExist())
            {
                throw new ArgumentNullException("数据库不存在表："
                    + this.GetType().ToString().Substring(this.GetType().ToString().LastIndexOf('.') + 1).Replace("Repository", ""));
            }
            if (!CheckRule.CheckStringNullOrEmpty(ref zoneCode))
                return GetByIsTransfer(isTransfer);

            object entity = Get(c => c.LocationCode.Equals(zoneCode) && c.IsTransfer.Equals(isTransfer));
            return entity as SecondTebleLandCollection;
        }

        /// <summary>
        /// 根据土地是否流转统计指定区域下的土地承包地
        /// </summary>
        /// <param name="zoneCode">区域代码</param>
        /// <param name="isTransfer">是否流转</param>
        /// <returns>-1(参数错误)/土地承包地数量</returns>
        public int CountByIsTransfer(string zoneCode, bool isTransfer)
        {
            if (!CheckTableExist())
            {
                throw new ArgumentNullException("数据库不存在表："
                    + this.GetType().ToString().Substring(this.GetType().ToString().LastIndexOf('.') + 1).Replace("Repository", ""));
            }
            if (!CheckRule.CheckStringNullOrEmpty(ref zoneCode))
                return -1;

            return Count(c => c.LocationCode.Equals(zoneCode) && c.IsTransfer.Equals(isTransfer));
        }

        /// <summary>
        /// 根据流转类型获取承包地对象
        /// </summary>
        /// <param name="transferType">流转类型</param>
        /// <returns>承包地对象</returns>
        public SecondTebleLandCollection GetByTransferType(eTransferType transferType)
        {
            if (!CheckTableExist())
            {
                throw new ArgumentNullException("数据库不存在表："
                    + this.GetType().ToString().Substring(this.GetType().ToString().LastIndexOf('.') + 1).Replace("Repository", ""));
            }
            object entity = Get(c => c.TransferType.Equals(transferType));
            return entity as SecondTebleLandCollection;
        }

        /// <summary>
        /// 根据流转类型获取指定区域下的承包地对象
        /// </summary>
        /// <param name="zoneCode">区域代码</param>
        /// <param name="transferType">流转类型</param>
        /// <returns>承包地对象</returns>
        public SecondTebleLandCollection GetByTransferType(string zoneCode, eTransferType transferType)
        {
            if (!CheckTableExist())
            {
                throw new ArgumentNullException("数据库不存在表："
                    + this.GetType().ToString().Substring(this.GetType().ToString().LastIndexOf('.') + 1).Replace("Repository", ""));
            }
            if (!CheckRule.CheckStringNullOrEmpty(ref zoneCode))
                return GetByTransferType(transferType);

            object entity = Get(c => c.LocationCode.Equals(zoneCode) && c.TransferType.Equals(transferType));
            return entity as SecondTebleLandCollection;
        }

        /// <summary>
        /// 根据户获取地块
        /// </summary>
        /// <param name="familyID">权利人id</param>
        /// <returns>承包地块</returns>
        public SecondTebleLandCollection GetCollection(Guid familyID, eConstructType constructType)
        {
            if (!CheckTableExist())
            {
                throw new ArgumentNullException("数据库不存在表："
                    + this.GetType().ToString().Substring(this.GetType().ToString().LastIndexOf('.') + 1).Replace("Repository", ""));
            }
            if (!CheckRule.CheckGuidNullOrEmpty(familyID))
                return null;

            object entity = Get(c => c.HouseHolderId.Equals(familyID) && c.ContractType.Equals(constructType));
            return entity as SecondTebleLandCollection;
        }

        /// <summary>
        /// 获取地域下以坐落单位代码排序的地块
        /// </summary>
        /// <param name="zoneCode">地域代码</param>
        /// <returns>以坐落单位代码排序的地块</returns>
        public SecondTebleLandCollection GetCollection(string zoneCode)
        {
            if (!CheckTableExist())
            {
                throw new ArgumentNullException("数据库不存在表："
                    + this.GetType().ToString().Substring(this.GetType().ToString().LastIndexOf('.') + 1).Replace("Repository", ""));
            }
            if (!CheckRule.CheckStringNullOrEmpty(ref zoneCode))
                return null;
            object entity = Get(c => c.LocationCode.StartsWith(zoneCode)).OrderBy(c => c.LocationCode).ToList();
            return entity as SecondTebleLandCollection;
        }

        /// <summary>
        /// 根据地籍号获取对象
        /// </summary>
        /// <param name="number"></param>
        /// <returns>土地承包地对象</returns>
        public SecondTebleLand Get(string number)
        {
            if (!CheckTableExist())
            {
                throw new ArgumentNullException("数据库不存在表："
                    + this.GetType().ToString().Substring(this.GetType().ToString().LastIndexOf('.') + 1).Replace("Repository", ""));
            }
            if (!CheckRule.CheckStringNullOrEmpty(ref number))
                return null;

            object entity = Get(c => c.CadastralNumber.Equals(number));
            return entity as SecondTebleLand;
        }

        /// <summary>
        /// 根据地籍号判断土地承包地对象是否存在
        /// </summary>
        /// <param name="cadastralNumber">地籍号</param>
        /// <returns>true（存在）/false（不存在）</returns>
        public bool ExistByNumber(string cadastralNumber)
        {
            if (!CheckTableExist())
            {
                throw new ArgumentNullException("数据库不存在表："
                    + this.GetType().ToString().Substring(this.GetType().ToString().LastIndexOf('.') + 1).Replace("Repository", ""));
            }
            if (!CheckRule.CheckStringNullOrEmpty(ref cadastralNumber))
                return false;

            return Count(c => c.CadastralNumber.EndsWith(cadastralNumber)) > 0;
        }

        /// <summary>
        /// 根据地籍号获取土地承包地对象
        /// </summary>
        /// <param name="number">地籍号</param>
        /// <returns>土地承包地对象</returns>
        public SecondTebleLand GetByNumber(string number)
        {
            if (!CheckTableExist())
            {
                throw new ArgumentNullException("数据库不存在表："
                    + this.GetType().ToString().Substring(this.GetType().ToString().LastIndexOf('.') + 1).Replace("Repository", ""));
            }
            if (!CheckRule.CheckStringNullOrEmpty(ref number))
                return null;

            object entity = Get(c => c.CadastralNumber.EndsWith(number));
            return entity as SecondTebleLand;
        }

        /// <summary>
        /// 根据户主名称获取地块
        /// </summary>
        /// <param name="householderName">权利人名称</param>
        /// <param name="zoneCode">区域代码</param>
        /// <returns>地块对象</returns>
        public SecondTebleLandCollection GetByHouseholderName(string zoneCode, string householderName)
        {
            if (!CheckTableExist())
            {
                throw new ArgumentNullException("数据库不存在表："
                    + this.GetType().ToString().Substring(this.GetType().ToString().LastIndexOf('.') + 1).Replace("Repository", ""));
            }
            if (!CheckRule.CheckStringNullOrEmpty(ref zoneCode) && !CheckRule.CheckStringNullOrEmpty(ref householderName))
                return null;
            object entity = Get(c => c.LocationCode.Equals(zoneCode) && c.HouseHolderName.Equals(householderName)).OrderBy(c => c.CadastralNumber).ToList();

            return entity as SecondTebleLandCollection;
        }

        /// <summary>
        /// 根据合同Id获取地块信息
        /// </summary>
        /// <param name="concordId">合同Id</param>
        /// <returns>地块信息</returns>
        public SecondTebleLandCollection GetByConcordId(Guid concordId)
        {
            if (!CheckTableExist())
            {
                throw new ArgumentNullException("数据库不存在表："
                    + this.GetType().ToString().Substring(this.GetType().ToString().LastIndexOf('.') + 1).Replace("Repository", ""));
            }
            if (!CheckRule.CheckGuidNullOrEmpty(concordId))
                return null;

            object entity = Get(c => c.ConcordId.Equals(concordId)).OrderBy(c => c.CadastralNumber).ToList();

            return entity as SecondTebleLandCollection;
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
        /// 删除当前地域下指定承包经营权类型所有地块
        /// </summary>
        /// <param name="zoneCode">地域代码</param>
        /// <param name="constructType">承包经营权类型</param>
        /// <returns>-1（参数错误）/0（失败）/1（成功）</returns>
        public int DeleteByZoneCode(string zoneCode, eConstructType constructType)
        {
            if (!CheckTableExist())
            {
                throw new ArgumentNullException("数据库不存在表："
                    + this.GetType().ToString().Substring(this.GetType().ToString().LastIndexOf('.') + 1).Replace("Repository", ""));
            }
            if (string.IsNullOrEmpty(zoneCode))
                return -1;

            return Delete(c => c.LocationCode.Equals(zoneCode) && c.ContractType.Equals(constructType));

        }

        /// <summary>
        /// 获取合同下地块面积
        /// </summary>
        /// <param name="guid">合同ID</param>
        /// <returns>地块面积</returns>
        public double GetLandAreaByConcordID(Guid guid)
        {
            if (!CheckTableExist())
            {
                throw new ArgumentNullException("数据库不存在表："
                    + this.GetType().ToString().Substring(this.GetType().ToString().LastIndexOf('.') + 1).Replace("Repository", ""));
            }
            if (!CheckRule.CheckGuidNullOrEmpty(guid))
                return 0D;
            object entity = Get(c => c.ConcordId.Equals(guid)).Sum(c => c.AwareArea);

            return (double)entity;
        }

        /// <summary>
        /// 根据区域代码匹配地籍号来获取土地承包地对象
        /// </summary>
        /// <param name="zoneCode">区域代码</param>
        /// <returns>土地承包地对象</returns>
        public SecondTebleLandCollection GetLandsByZoneCodeInCadastralNumber(string zoneCode)
        {
            if (!CheckTableExist())
            {
                throw new ArgumentNullException("数据库不存在表："
                    + this.GetType().ToString().Substring(this.GetType().ToString().LastIndexOf('.') + 1).Replace("Repository", ""));
            }
            if (!CheckRule.CheckStringNullOrEmpty(ref zoneCode))
                return null;
            object entity = Get(c => c.CadastralNumber.StartsWith(zoneCode)).OrderBy(c => c.CadastralNumber);
            return entity as SecondTebleLandCollection;
        }

        /// <summary>
        /// 按地域统计
        /// </summary>
        /// <param name="zoneCode">区域代码</param>
        /// <param name="searchOption">匹配等级</param>
        /// <returns>-1(参数错误)/地块数量</returns>
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
                return Count(c => c.LocationCode.StartsWith(zoneCode));
        }

        /// <summary>
        /// 按地域统计面积
        /// </summary>
        /// <param name="zoneCode">区域代码</param>
        /// <param name="searchOption">匹配等级</param>
        /// <returns>统计面积</returns>
        public double CountArea(string zoneCode, eLevelOption searchOption)
        {
            if (!CheckTableExist())
            {
                throw new ArgumentNullException("数据库不存在表："
                    + this.GetType().ToString().Substring(this.GetType().ToString().LastIndexOf('.') + 1).Replace("Repository", ""));
            }
            if (!CheckRule.CheckStringNullOrEmpty(ref zoneCode))
                return 0D;

            object obj;
            if (searchOption == eLevelOption.Self)
                obj = Get(c => c.LocationCode.Equals(zoneCode)).Sum(c => c.AwareArea);
            else
                obj = Get(c => c.LocationCode.StartsWith(zoneCode)).Sum(c => c.AwareArea);
            if (obj == DBNull.Value)
                return 0D;

            return double.Parse(obj.ToString());
        }

        /// <summary>
        /// 统计地域下实测面积
        /// </summary>
        /// <param name="zoneCode">区域代码</param>
        /// <param name="searchOption">匹配等级</param>
        /// <returns>实测面积</returns>
        public double CountActualArea(string zoneCode, eLevelOption searchOption)
        {
            if (!CheckTableExist())
            {
                throw new ArgumentNullException("数据库不存在表："
                    + this.GetType().ToString().Substring(this.GetType().ToString().LastIndexOf('.') + 1).Replace("Repository", ""));
            }
            if (!CheckRule.CheckStringNullOrEmpty(ref zoneCode))
            {
                return 0D;
            }
            object obj;
            if (searchOption == eLevelOption.Self)
                obj = Get(c => c.LocationCode.Equals(zoneCode)).Sum(c => c.ActualArea);
            else
                obj = Get(c => c.LocationCode.StartsWith(zoneCode)).Sum(c => c.ActualArea);
            if (obj == DBNull.Value)
                return 0D;
            return double.Parse(obj.ToString());
        }

        /// <summary>
        /// 统计区域下没有合同的地块
        /// </summary>
        /// <param name="zoneCode">区域代码</param>
        /// <param name="searchOption">匹配等级</param>
        /// <returns>-1(参数错误)/没有合同的地块数量</returns>
        public int CountNoConcord(string zoneCode, eLevelOption searchOption)
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
            int obj;
            if (searchOption == eLevelOption.Self)
                obj = Count(c => c.LocationCode.Equals(zoneCode) && c.ConcordId.Equals(null));
            else
                obj = Count(c => c.LocationCode.StartsWith(zoneCode) && c.ConcordId.Equals(null));
            return obj;
        }

        /// <summary>
        /// 按地域删除地块
        /// </summary>
        /// <param name="zoneCode">区域代码</param>
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
                cnt = Delete(c => c.LocationCode.StartsWith(zoneCode));

            return cnt;
        }

        /// <summary>
        /// 按地域获取地块
        /// </summary>
        /// <param name="zoneCode">区域代码</param>
        /// <param name="searchOption"><匹配等级/param>
        /// <returns>地块</returns>
        public SecondTebleLandCollection GetCollection(string zoneCode, eLevelOption searchOption)
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
                entity = Get(c => c.LocationCode.StartsWith(zoneCode));

            return entity as SecondTebleLandCollection;
        }

        /// <summary>
        /// 按户统计地块
        /// </summary>
        /// <param name="familyID">权利人id</param>
        /// <returns></returns>
        public int Count(Guid familyID)
        {
            if (!CheckTableExist())
            {
                throw new ArgumentNullException("数据库不存在表："
                    + this.GetType().ToString().Substring(this.GetType().ToString().LastIndexOf('.') + 1).Replace("Repository", ""));
            }
            if (!CheckRule.CheckGuidNullOrEmpty(familyID))
                return -1;
            return Count(c => c.HouseHolderId.Equals(familyID));
        }


        /// <summary>
        /// 根据地籍号获得以权利人名称排序的承包地对象
        /// </summary>
        /// <param name="cadastralNumber">地籍号</param>
        /// <param name="searchType">查找类型</param>
        /// <returns>承包地对象</returns>
        public SecondTebleLandCollection SearchByCadastralNumber(string cadastralNumber, eSearchOption searchType)
        {
            if (!CheckTableExist())
            {
                throw new ArgumentNullException("数据库不存在表："
                    + this.GetType().ToString().Substring(this.GetType().ToString().LastIndexOf('.') + 1).Replace("Repository", ""));
            }
            if (!CheckRule.CheckStringNullOrEmpty(ref cadastralNumber))
                return null;

            object data = null;

            if (searchType == eSearchOption.Precision)
                data = Get(c => c.CadastralNumber.EndsWith(cadastralNumber)).OrderBy(c => c.HouseHolderName).ToList();
            else if (searchType == eSearchOption.Fuzzy)
                data = Get(c => c.CadastralNumber.Contains(cadastralNumber)).OrderBy(c => c.HouseHolderName).ToList();

            return (SecondTebleLandCollection)data;
        }

        /// <summary>
        /// 根据权利人名称获得以权利人名称排序的承包地对象
        /// </summary>
        /// <param name="name">权利人名称</param>
        /// <param name="searchType">查找类型</param>
        /// <returns>承包地对象</returns>
        public SecondTebleLandCollection SearchByVirtualPersonName(string name, eSearchOption searchType)
        {
            if (!CheckTableExist())
            {
                throw new ArgumentNullException("数据库不存在表："
                    + this.GetType().ToString().Substring(this.GetType().ToString().LastIndexOf('.') + 1).Replace("Repository", ""));
            }
            if (!CheckRule.CheckStringNullOrEmpty(ref name))
                return null;

            object data = null;

            if (searchType == eSearchOption.Precision)
                data = Get(c => c.HouseHolderName.Equals(name)).OrderBy(c => c.HouseHolderName);
            else if (searchType == eSearchOption.Fuzzy)
                data = Get(c => c.HouseHolderName.Contains(name)).OrderBy(c => c.HouseHolderName);

            return (SecondTebleLandCollection)data;
        }

        /// <summary>
        /// 根据地籍号与权属单位代码获取以权利人姓名排序的土地承包地对象
        /// </summary>
        /// <param name="cadastralNumber">地籍号</param>
        /// <param name="searchType">地籍号查找类型</param>
        /// <param name="zoneCode">区域代码</param>
        /// <returns>以权利人姓名排序的土地承包地对象</returns>
        public SecondTebleLandCollection SearchByCadastralNumber(string cadastralNumber, eSearchOption searchType, string zoneCode)
        {
            if (!CheckTableExist())
            {
                throw new ArgumentNullException("数据库不存在表："
                    + this.GetType().ToString().Substring(this.GetType().ToString().LastIndexOf('.') + 1).Replace("Repository", ""));
            }
            if (!CheckRule.CheckStringNullOrEmpty(ref zoneCode))
                return null;

            if (!CheckRule.CheckStringNullOrEmpty(ref cadastralNumber))
                return SearchByCadastralNumber(cadastralNumber, searchType);

            object data = null;

            if (searchType == eSearchOption.Precision)
                data = Get(c => c.CadastralNumber.EndsWith(cadastralNumber) && c.OwnRightCode.StartsWith(zoneCode)).OrderBy(c => c.HouseHolderName);
            else if (searchType == eSearchOption.Fuzzy)
                data = Get(c => c.CadastralNumber.Contains(cadastralNumber) && c.OwnRightCode.StartsWith(zoneCode)).OrderBy(c => c.HouseHolderName);

            return (SecondTebleLandCollection)data;
        }

        /// <summary>
        /// 根据地籍号与权属单位代码获取以权利人姓名排序的土地承包地对象
        /// </summary>
        /// <param name="name">权利人名称</param>
        /// <param name="searchType">权利人名称匹配类型</param>
        /// <param name="zoneCode">权属单位代码</param>
        /// <returns>土地承包对象</returns>
        public SecondTebleLandCollection SearchByVirtualPersonName(string name, eSearchOption searchType, string zoneCode)
        {
            if (!CheckTableExist())
            {
                throw new ArgumentNullException("数据库不存在表："
                    + this.GetType().ToString().Substring(this.GetType().ToString().LastIndexOf('.') + 1).Replace("Repository", ""));
            }
            if (!CheckRule.CheckStringNullOrEmpty(ref zoneCode))
                return SearchByVirtualPersonName(name, searchType);

            object data = null;

            if (searchType == eSearchOption.Precision)
                data = Get(c => c.HouseHolderName.Equals(name) && c.OwnRightCode.StartsWith(zoneCode)).OrderBy(c => c.HouseHolderName).ToList();
            else if (searchType == eSearchOption.Fuzzy)
                data = Get(c => c.HouseHolderName.Contains(name) && c.OwnRightCode.StartsWith(zoneCode)).OrderBy(c => c.HouseHolderName).ToList();

            return (SecondTebleLandCollection)data;
        }

        /// <summary>
        /// 按土地类型搜索
        /// </summary>
        /// <returns></returns>
        public SecondTebleLandCollection SearchByLandType(string zoneCode, eConstructType constructType)
        {
            if (!CheckTableExist())
            {
                throw new ArgumentNullException("数据库不存在表："
                    + this.GetType().ToString().Substring(this.GetType().ToString().LastIndexOf('.') + 1).Replace("Repository", ""));
            }
            if (!CheckRule.CheckStringNullOrEmpty(ref zoneCode))
                return new SecondTebleLandCollection();

            object entity = Get(c => c.LocationCode.Equals(zoneCode) && c.ContractType.Equals(constructType));

            return entity as SecondTebleLandCollection;
        }

        /// <summary>
        /// 按承包地类型搜索指定区域下的承包地对象
        /// </summary>
        /// <param name="zoneCode">区域代码</param>
        /// <param name="constructType">经营权类型</param>
        /// <returns>承包地对象</returns>
        public SecondTebleLandCollection SearchByLandTypeAndCadaZone(string zoneCode, eConstructType constructType)
        {
            if (!CheckTableExist())
            {
                throw new ArgumentNullException("数据库不存在表："
                    + this.GetType().ToString().Substring(this.GetType().ToString().LastIndexOf('.') + 1).Replace("Repository", ""));
            }
            if (!CheckRule.CheckStringNullOrEmpty(ref zoneCode))
                return new SecondTebleLandCollection();

            object entity = Get(c => c.CadastralZoneCode.Equals(zoneCode) && c.ContractType.Equals(constructType));
            return entity as SecondTebleLandCollection;
        }

        #endregion
    }
}