/*
 * (C) 2015  鱼鳞图公司版权所有,保留所有权利 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YuLinTu.Data;
using YuLinTu.Library.Entity;
using YuLinTu.Spatial;

namespace YuLinTu.Library.Repository
{
    /// <summary>
    /// 用于(ContractLand、SeondTableLand)地块的数据访问类
    /// </summary>
    public class AgricultureLandRepository<T> : RepositoryDbContext<T>, IAgricultureLandRepository<T> where T : ContractLand
    {
        #region Ctor

        private IDataSourceSchema m_DSSchema = null;

        public AgricultureLandRepository(IDataSource ds)
            : base(ds)
        {
            m_DSSchema = ds.CreateSchema();
        }

        #endregion

        #region Fields

        private const string ORDERFIELD_DEFAULT = "Name";

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
            //    return m_DSSchema.AnyElement(m_DSSchema.GetElements()[0].Schema, typeof(T).GetAttribute<DataTableAttribute>().TableName);
            //}
            //catch (Exception)
            //{
            //    return false;
            //}
            return true;
        }

        /// <summary>
        /// 添加农村土地承包地对象
        /// </summary>
        public new string Add(T entity)
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
        /// 根据标识码删除农村土地承包地对象
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
            if (!CheckRule.CheckGuidNullOrEmpty(guid))
                return 0;

            return base.Delete(c => c.ID.Equals(guid));
        }

        /// <summary>
        /// 根据承包方ID删除下属地块信息
        /// </summary>
        /// <param name="guid">承包方标识码</param>
        /// <returns>-1（参数错误）/0（失败）/1（成功）</returns>
        public int DeleteLandByPersonID(Guid guid)
        {
            if (!CheckTableExist())
            {
                throw new ArgumentNullException("数据库不存在表："
                    + this.GetType().ToString().Substring(this.GetType().ToString().LastIndexOf('.') + 1).Replace("Repository", ""));
            }
            if (!CheckRule.CheckGuidNullOrEmpty(guid))
                return 0;

            return base.Delete(c => c.OwnerId.Equals(guid));
        }

        /// <summary>
        /// 更新对象
        /// </summary>
        /// <returns>-1（参数错误）/0（失败）/1（成功）</returns>
        public int Update(T entity)
        {
            //if (!CheckTableExist())
            //{
            //    throw new ArgumentNullException("数据库不存在表："
            //        + this.GetType().ToString().Substring(this.GetType().ToString().LastIndexOf('.') + 1).Replace("Repository", ""));
            //}
            if (entity == null)
                return 0;
            entity.ModifiedTime = DateTime.Now;
            return base.Update(entity, c => c.ID.Equals(entity.ID));
        }

        /// <summary>
        /// 根据承包方id更新承包方名称
        /// </summary>
        /// <param name="ownerId">承包方id</param>
        /// <param name="ownerName">承包方名称</param>
        /// <returns>-1（参数错误）/0（失败）/1（成功）</returns>
        public int Update(Guid ownerId, string ownerName)
        {
            if (!CheckTableExist())
            {
                throw new ArgumentNullException("数据库不存在表："
                    + this.GetType().ToString().Substring(this.GetType().ToString().LastIndexOf('.') + 1).Replace("Repository", ""));
            }
            if (!CheckRule.CheckGuidNullOrEmpty(ownerId) || !CheckRule.CheckStringNullOrEmpty(ref ownerName))
                return -1;

            var q = Get(c => c.OwnerId.Equals(ownerId));
            if (q == null || q.Count == 0)
                return -1;
            int cnt = 0;
            foreach (var item in q)
            {
                item.OwnerName = ownerName;
                cnt += Update(item);
            }
            if (cnt < q.Count)
                return 0;
            else
                return 1;
        }

        /// <summary>
        /// 根据标识码获取对象
        /// </summary>
        public T Get(Guid guid)
        {
            if (!CheckTableExist())
            {
                throw new ArgumentNullException("数据库不存在表："
                    + this.GetType().ToString().Substring(this.GetType().ToString().LastIndexOf('.') + 1).Replace("Repository", ""));
            }
            if (!CheckRule.CheckGuidNullOrEmpty(guid))
                return null;
            T entity = Get(c => c.ID.Equals(guid)).FirstOrDefault();
            return entity;
        }

        /// <summary>
        /// 根据标识码判断对象是否存在
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
        /// 根据地籍号精确判断农村土地承包地对象是否存在
        /// </summary>
        /// <param name="cadastralNumber">地籍号</param>
        /// <returns>true（存在）/false（不存在）</returns>
        public bool ExistByCadastralNumberP(string cadastralNumber)
        {
            if (!CheckTableExist())
            {
                throw new ArgumentNullException("数据库不存在表："
                    + this.GetType().ToString().Substring(this.GetType().ToString().LastIndexOf('.') + 1).Replace("Repository", ""));
            }
            if (!CheckRule.CheckStringNullOrEmpty(ref cadastralNumber))
                return false;
            int count = Count(c => c.CadastralNumber.Equals(cadastralNumber));
            return count > 0 ? true : false;
        }

        /// <summary>
        /// 根据承包方id获取地块
        /// </summary>
        /// <param name="ownerId">承包方id</param>
        /// <returns>地块</returns>
        public List<T> GetCollection(Guid ownerId)
        {
            if (!CheckTableExist())
            {
                throw new ArgumentNullException("数据库不存在表："
                    + this.GetType().ToString().Substring(this.GetType().ToString().LastIndexOf('.') + 1).Replace("Repository", ""));
            }
            if (!CheckRule.CheckGuidNullOrEmpty(ownerId))
                return null;

            var entity = Get(c => c.OwnerId.Equals(ownerId));
            return entity;
        }

        /// <summary>
        /// 根据土地是否流转获取农村土地承包地对象集合
        /// </summary>
        /// <param name="isTransfer">土地是否流转</param>
        /// <returns>农村土地承包地对象集合</returns>
        public List<T> GetByIsTransfer(bool isTransfer)
        {
            if (!CheckTableExist())
            {
                throw new ArgumentNullException("数据库不存在表："
                    + this.GetType().ToString().Substring(this.GetType().ToString().LastIndexOf('.') + 1).Replace("Repository", ""));
            }
            var entity = Get(c => c.IsTransfer.Equals(isTransfer));
            return entity;
        }

        /// <summary>
        /// 根据土地是否流转获取指定区域下的农村土地承包地对象集合
        /// </summary>
        /// <param name="zoneCode">区域代码</param>
        /// <param name="isTransfer">土地是否流转</param>
        /// <returns>农村土地承包地对象集合；指定区域为null，返回所有区域符合土地是否流转参数的土地对象</returns>
        public List<T> GetByIsTransfer(string zoneCode, bool isTransfer)
        {
            if (!CheckTableExist())
            {
                throw new ArgumentNullException("数据库不存在表："
                    + this.GetType().ToString().Substring(this.GetType().ToString().LastIndexOf('.') + 1).Replace("Repository", ""));
            }
            if (!CheckRule.CheckStringNullOrEmpty(ref zoneCode))
                return GetByIsTransfer(isTransfer);

            var entity = Get(c => c.ZoneCode.Equals(zoneCode) && c.IsTransfer.Equals(isTransfer));
            return entity;
        }

        /// <summary>
        /// 根据土地是否流转统计指定区域下的农村土地承包地对象
        /// </summary>
        /// <param name="zoneCode">区域代码</param>
        /// <param name="isTransfer">土地是否流转</param>
        /// <returns>-1（参数错误）/int 对象数量</returns>
        public int CountByIsTransfer(string zoneCode, bool isTransfer)
        {
            if (!CheckTableExist())
            {
                throw new ArgumentNullException("数据库不存在表："
                    + this.GetType().ToString().Substring(this.GetType().ToString().LastIndexOf('.') + 1).Replace("Repository", ""));
            }
            if (!CheckRule.CheckStringNullOrEmpty(ref zoneCode))
                return -1;

            return Count(c => c.ZoneCode.Equals(zoneCode) && c.IsTransfer.Equals(isTransfer));
        }

        /// <summary>
        /// 根据流转类型获取土地承包地对象集合
        /// </summary>
        /// <param name="transferType">流转类型</param>
        /// <returns>农村土地承包地对象集合</returns>
        public List<T> GetByTransferType(string transferType)
        {
            if (!CheckTableExist())
            {
                throw new ArgumentNullException("数据库不存在表："
                    + this.GetType().ToString().Substring(this.GetType().ToString().LastIndexOf('.') + 1).Replace("Repository", ""));
            }
            var entity = Get(c => c.TransferType.Equals(transferType));

            return entity;
        }

        /// <summary>
        /// 根据流转类型获取指定区域下的土地承包地对象集合
        /// </summary>
        /// <param name="zoneCode">区域代码</param>
        /// <param name="transferType">流转类型</param>
        /// <returns>土地承包地对象集合；指定区域为null，返回所有区域符合流转类型参数的土地对象</returns>
        public List<T> GetByTransferType(string zoneCode, string transferType)
        {
            if (!CheckTableExist())
            {
                throw new ArgumentNullException("数据库不存在表："
                    + this.GetType().ToString().Substring(this.GetType().ToString().LastIndexOf('.') + 1).Replace("Repository", ""));
            }
            if (!CheckRule.CheckStringNullOrEmpty(ref zoneCode))
                return GetByTransferType(transferType);

            var entity = Get(c => c.ZoneCode.Equals(zoneCode) && c.TransferType.Equals(transferType));
            return entity;
        }

        /// <summary>
        /// 根据承包经营权类型获取指定承包方id下的地块集合
        /// </summary>
        /// <param name="ownerId">承包方id</param>
        /// <param name="constructType">地块类别</param>
        /// <returns>地块集合</returns>
        public List<T> GetCollection(Guid ownerId, string constructType)
        {
            if (!CheckTableExist())
            {
                throw new ArgumentNullException("数据库不存在表："
                    + this.GetType().ToString().Substring(this.GetType().ToString().LastIndexOf('.') + 1).Replace("Repository", ""));
            }
            if (!CheckRule.CheckGuidNullOrEmpty(ownerId))
                return null;

            var entity = Get(c => c.OwnerId.Equals(ownerId) && c.LandCategory.Equals(constructType));

            return entity;
        }

        /// <summary>
        /// 根据承包方式获取指定承包方id下的地块集合
        /// </summary>
        /// <param name="ownerId">承包方id</param>
        /// <param name="constructMode">承包方式</param>
        /// <returns>地块集合</returns>
        public List<T> GetCollectionByConstructMode(Guid ownerId, string constructMode)
        {
            if (!CheckTableExist())
            {
                throw new ArgumentNullException("数据库不存在表："
                    + this.GetType().ToString().Substring(this.GetType().ToString().LastIndexOf('.') + 1).Replace("Repository", ""));
            }
            if (!CheckRule.CheckGuidNullOrEmpty(ownerId))
                return null;

            var entity = Get(c => c.OwnerId.Equals(ownerId) && c.ConstructMode.Equals(constructMode));

            return entity;
        }

        /// <summary>
        /// 获取地域下地块
        /// </summary>
        /// <param name="zoneCode">区域代码</param>
        /// <returns>土地对象</returns>
        public List<T> GetCollection(string zoneCode)
        {
            if (!CheckTableExist())
            {
                throw new ArgumentNullException("数据库不存在表："
                    + this.GetType().ToString().Substring(this.GetType().ToString().LastIndexOf('.') + 1).Replace("Repository", ""));
            }
            if (!CheckRule.CheckStringNullOrEmpty(ref zoneCode))
                return null;
            var entity = Get(c => c.ZoneCode.Contains(zoneCode));
            return entity;
        }

        /// <summary>
        /// 根据地籍号获取对象
        /// </summary>
        /// <param name="cadastralNumber">地籍号</param>
        /// <returns>土地承包地对象</returns>
        public T Get(string cadastralNumber)
        {
            if (!CheckTableExist())
            {
                throw new ArgumentNullException("数据库不存在表："
                    + this.GetType().ToString().Substring(this.GetType().ToString().LastIndexOf('.') + 1).Replace("Repository", ""));
            }
            if (!CheckRule.CheckStringNullOrEmpty(ref cadastralNumber))
                return null;

            T entity = Get(c => c.CadastralNumber.Equals(cadastralNumber)).FirstOrDefault();
            return entity;
        }

        /// <summary>
        /// 根据地籍号模糊判断对象是否存在
        /// </summary>
        /// <param name="cadastralNumber">地籍号</param>
        /// <returns>true（存在）/false（不存在）</returns>
        public bool ExistByCadastralNumberF(string cadastralNumber)
        {
            if (!CheckTableExist())
            {
                throw new ArgumentNullException("数据库不存在表："
                    + this.GetType().ToString().Substring(this.GetType().ToString().LastIndexOf('.') + 1).Replace("Repository", ""));
            }
            if (!CheckRule.CheckStringNullOrEmpty(ref cadastralNumber))
                return false;
            int count = Count(c => c.CadastralNumber.EndsWith(cadastralNumber));
            return count > 0 ? true : false;
        }

        /// <summary>
        /// 根据地籍号获取土地承包地对象
        /// </summary>
        /// <param name="cadastralNumber">地籍号</param>
        /// <returns>土地承包地对象</returns>
        public T GetByCadastralNumber(string cadastralNumber)
        {
            if (!CheckTableExist())
            {
                throw new ArgumentNullException("数据库不存在表："
                    + this.GetType().ToString().Substring(this.GetType().ToString().LastIndexOf('.') + 1).Replace("Repository", ""));
            }
            if (!CheckRule.CheckStringNullOrEmpty(ref cadastralNumber))
                return null;

            T entity = Get(c => c.CadastralNumber.EndsWith(cadastralNumber)).FirstOrDefault();
            return entity;
        }

        /// <summary>
        /// 获取指定区域下指定承包方姓名的地块集合
        /// </summary>
        /// <param name="ownerName">承包方名称</param>
        /// <param name="zoneCode">区域代码</param>
        /// <returns>地块集合</returns>
        public List<T> GetByOwnerName(string zoneCode, string ownerName)
        {
            if (!CheckTableExist())
            {
                throw new ArgumentNullException("数据库不存在表："
                    + this.GetType().ToString().Substring(this.GetType().ToString().LastIndexOf('.') + 1).Replace("Repository", ""));
            }
            if (!CheckRule.CheckStringNullOrEmpty(ref zoneCode) || !CheckRule.CheckStringNullOrEmpty(ref ownerName))
                return null;
            var entity = Get(c => c.ZoneCode.Equals(zoneCode) && c.OwnerName.Equals(ownerName));
            return entity;
        }

        /// <summary>
        /// 根据合同Id获取地块信息
        /// </summary>
        /// <param name="concordId">合同Id</param>
        /// <returns>地块集合</returns>
        public List<T> GetByConcordId(Guid concordId)
        {
            if (!CheckTableExist())
            {
                throw new ArgumentNullException("数据库不存在表："
                    + this.GetType().ToString().Substring(this.GetType().ToString().LastIndexOf('.') + 1).Replace("Repository", ""));
            }
            if (!CheckRule.CheckGuidNullOrEmpty(concordId))
                return null;
            var entity = Get(c => c.ConcordId.Equals(concordId));
            return entity;
        }

        /// <summary>
        /// 根据合同Id获取地块个数
        /// </summary>
        /// <param name="concordId">合同Id</param>
        /// <returns>地块集合</returns>
        public int CountByConcordId(Guid concordId)
        {
            if (!CheckTableExist())
            {
                throw new ArgumentNullException("数据库不存在表："
                    + this.GetType().ToString().Substring(this.GetType().ToString().LastIndexOf('.') + 1).Replace("Repository", ""));
            }
            if (!CheckRule.CheckGuidNullOrEmpty(concordId))
                return 0;
            var entity = Count(c => c.ConcordId.Equals(concordId));
            return entity;
        }


        /// <summary>
        /// 根据合同Id集合获取地块信息集合
        /// </summary>
        public List<T> GetByConcordIds(Guid[] ids)
        {
            if (ids == null || ids.Length == 0)
                return null;

            var data = DataSource.CreateQuery<T>().Where(c => c.ConcordId != null && ids.Contains((Guid)c.ConcordId)).ToList();
            return data;
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
                return 0;
            return Delete(c => c.ZoneCode.Equals(zoneCode));
        }

        /// <summary>
        /// 删除选定承包方所有数据
        /// </summary>
        public void DeleteSelectVirtualPersonAllData(Guid ID)
        {
            var coilArray = (from p in DataSource.CreateQuery<LandVirtualPerson>()
                             join l in DataSource.CreateQuery<ContractLand>() on p.ID equals l.OwnerId
                             join c in DataSource.CreateQuery<BuildLandBoundaryAddressCoil>() on l.ID equals c.LandID
                             where p.ID == ID
                             select new { CoilID = c.ID }).Select(i => i.CoilID).ToArray();

            var dotArray = (from p in DataSource.CreateQuery<LandVirtualPerson>()
                            join l in DataSource.CreateQuery<ContractLand>() on p.ID equals l.OwnerId
                            join c in DataSource.CreateQuery<BuildLandBoundaryAddressDot>() on l.ID equals c.LandID
                            where p.ID == ID
                            select new { DotID = c.ID }).Select(i => i.DotID).ToArray();

            var regeditdArray = (from p in DataSource.CreateQuery<LandVirtualPerson>()
                                 join l in DataSource.CreateQuery<ContractConcord>() on p.ID equals l.ContracterId
                                 where p.ID == ID
                                 select new { RegeditID = l.ID }).Select(i => i.RegeditID).ToArray();

            var queryvpdata = DataSource.CreateQuery<LandVirtualPerson>();
            AppendEdit(queryvpdata.Where(c => c.ID == ID).Delete());

            var querylanddata = DataSource.CreateQuery<ContractLand>();
            AppendEdit(querylanddata.Where(c => c.OwnerId == ID).Delete());

            var queryconcorddata = DataSource.CreateQuery<ContractConcord>();
            AppendEdit(queryconcorddata.Where(c => c.ContracterId == ID).Delete());

            AppendEdit(DataSource.CreateQuery<BuildLandBoundaryAddressCoil>().Where(c => coilArray.Contains(c.ID)).Delete());

            AppendEdit(DataSource.CreateQuery<BuildLandBoundaryAddressDot>().Where(c => dotArray.Contains(c.ID)).Delete());

            AppendEdit(DataSource.CreateQuery<ContractRegeditBook>().Where(c => regeditdArray.Contains(c.ID)).Delete());
        }

        /// <summary>
        /// 根据地块标识集合删除和更新相关业务数据
        /// </summary>
        /// <param name="ids">地块标识集合</param>
        public int DeleteRelationDataByLand(Guid[] ids)
        {
            if (ids == null || ids.Count() == 0)
                return -1;

            int delAppend = 0;

            //删除地块
            delAppend = AppendEdit(DataSource.CreateQuery<ContractLand>().Where(c => ids.Contains(c.ID)).Delete());
            //删除界址点
            delAppend = AppendEdit(DataSource.CreateQuery<BuildLandBoundaryAddressDot>().Where(c => ids.Contains(c.LandID)).Delete());
            //删除界址线
            delAppend = AppendEdit(DataSource.CreateQuery<BuildLandBoundaryAddressCoil>().Where(c => ids.Contains(c.LandID)).Delete());

            //对合同和权证进行处理
            var qc = from land in DataSource.CreateQuery<ContractLand>()
                     where ids.Contains(land.ID) && land.ConcordId != null
                     select land.ConcordId;

            var qc1 = DataSource.CreateQuery<ContractLand>().Where(c => qc.Contains(c.ConcordId));

            var qc2 = from land in qc1
                      group land by land.ConcordId into gp
                      select new
                      {
                          gp.Key,
                          ActualArea = gp.Sum(c => ids.Contains(c.ID) ? 0.0 : c.ActualArea),
                          AwareArea = gp.Sum(c => ids.Contains(c.ID) ? 0.0 : c.AwareArea),
                          TableArea = gp.Sum(c => ids.Contains(c.ID) ? 0.0 : c.TableArea)
                      };

            var qc3 = qc2.ToList();

            foreach (var q in qc3)
            {
                if (q.ActualArea == 0.0 && q.AwareArea == 0.0 && q.TableArea == 0.0)
                {
                    //删除合同和权证
                    delAppend = AppendEdit(DataSource.CreateQuery<ContractConcord>().Where(c => c.ID == q.Key).Delete());
                    delAppend = AppendEdit(DataSource.CreateQuery<ContractRegeditBook>().Where(c => c.ID == q.Key).Delete());
                }
                else
                {
                    //更新合同和权证
                    delAppend = AppendEdit(DataSource.CreateQuery<ContractConcord>().Where(c => c.ID == q.Key).Update(c => new ContractConcord { CountActualArea = q.ActualArea, CountAwareArea = q.AwareArea, TotalTableArea = q.TableArea }));
                }
            }
            return delAppend;
        }

        /// <summary>
        /// 删除当前地域下指定承包经营权类型的所有地块数量
        /// </summary>
        /// <param name="zoneCode">地域代码</param>
        /// <param name="constructType">承包经营权类型</param>
        /// <returns>-1（参数错误）/0（失败）/1（成功）</returns>
        public int DeleteByZoneCode(string zoneCode, string constructType)
        {
            if (!CheckTableExist())
            {
                throw new ArgumentNullException("数据库不存在表："
                    + this.GetType().ToString().Substring(this.GetType().ToString().LastIndexOf('.') + 1).Replace("Repository", ""));
            }
            if (!CheckRule.CheckStringNullOrEmpty(ref zoneCode))
                return -1;

            return Delete(c => c.ZoneCode.Equals(zoneCode) && c.LandCategory.Equals(constructType));

        }

        /// <summary>
        /// 删除当前地域下指定的承包方状态的所有地块
        /// </summary>
        /// <param name="zoneCode">地域代码</param>
        /// <param name="virtualStatus">承包方状态</param>
        /// <param name="levelOption">地域匹配等级</param>
        /// <returns>-1（参数错误）/0（失败）/1（成功）</returns>
        public int DeleteByZoneCode(string zoneCode, eVirtualPersonStatus virtualStatus, eLevelOption levelOption)
        {
            if (!CheckRule.CheckStringNullOrEmpty(ref zoneCode))
                return -1;

            int cnt = 0;
            int compare = 50;

            if (levelOption == eLevelOption.Self)
            {
                int count = DataSource.CreateQuery<LandVirtualPerson>().Where(c => c.ZoneCode == zoneCode && c.Status == virtualStatus).Count();//.Select(c => c.ID)
                if (count == 0)
                    return 0;
                var idsSelf = DataSource.CreateQuery<LandVirtualPerson>().Where(c => c.ZoneCode == zoneCode && c.Status == virtualStatus).Select(s => new { s.ID }).ToList().Select(c => c.ID).ToList();
                cnt = DeleteByVitualPersonStatue(compare, idsSelf);
            }
            else if (levelOption == eLevelOption.SelfAndSubs)
            {
                int count = DataSource.CreateQuery<LandVirtualPerson>().Where(c => c.ZoneCode.StartsWith(zoneCode) && c.Status == virtualStatus).Count();//.Select(c => c.ID)
                if (count == 0)
                    return 0;
                var idsSelfAndSubs = DataSource.CreateQuery<LandVirtualPerson>().Where(c => c.ZoneCode.StartsWith(zoneCode) && c.Status == virtualStatus).Select(s => new { s.ID }).ToList().Select(c => c.ID).ToList();
                cnt = DeleteByVitualPersonStatue(compare, idsSelfAndSubs);
            }
            else
            {
                int count = DataSource.CreateQuery<LandVirtualPerson>().Where(c => c.ZoneCode.StartsWith(zoneCode) && c.ZoneCode != zoneCode && c.Status == virtualStatus).Count();//.Select(c => c.ID)
                if (count == 0)
                    return 0;
                var idsSubs = DataSource.CreateQuery<LandVirtualPerson>().Where(c => c.ZoneCode.StartsWith(zoneCode) && c.ZoneCode != zoneCode && c.Status == virtualStatus).Select(s => new { s.ID }).ToList().Select(c => c.ID).ToList();
                cnt = DeleteByVitualPersonStatue(compare, idsSubs);
            }

            return cnt;
        }

        /// <summary>
        /// 根据承包方状态删除数据
        /// </summary>
        /// <param name="compare">拼接Whrer语句最大个数(此数字过大会导致程序异常)</param>
        /// <param name="ids">查找的地块id集合</param>
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
                    b.Append("(");
                    int j = 0;
                    while (j < compare)
                    {
                        if (j == compare - 1)
                        {
                            b.AppendFormat("OwnerId == @{0}", j);
                        }
                        else
                        {
                            b.AppendFormat("OwnerId == @{0}" + " || ", j);
                        }
                        listObj.Add(ids[index]);
                        j++;
                        index++;
                    }
                    b.Append(")&&IsStockLand==false");
                    cnt = AppendEdit(DataSource.CreateQuery<T>().Where(b.ToString(), listObj.Cast<object>().ToArray()).Delete());
                    b.Clear();
                    listObj.Clear();
                }
                int other = ids.Count - (compare * count);
                if (other == 0)
                    return cnt;
                b.Append("(");
                for (int i = 0; i < other; i++)
                {
                    listObj.Add(ids[index + i]);
                    if (i == other - 1)
                    {
                        b.AppendFormat("OwnerId == @{0}", i);
                    }
                    else
                    {
                        b.AppendFormat("OwnerId == @{0}" + " || ", i);
                    }
                }
                b.Append(")&&IsStockLand==false");
                cnt = AppendEdit(DataSource.CreateQuery<T>().Where(b.ToString(), listObj.Cast<object>().ToArray()).Delete());
            }
            else
            {
                b.Append("(");
                for (int i = 0; i < ids.Count; i++)
                {
                    if (i == ids.Count - 1)
                    {
                        b.AppendFormat("OwnerId == @{0}", i);
                    }
                    else
                    {
                        b.AppendFormat("OwnerId == @{0}" + " || ", i);
                    }
                }
                b.Append(")&&IsStockLand==false");
                cnt = AppendEdit(DataSource.CreateQuery<T>().Where(b.ToString(), ids.Cast<object>().ToArray()).Delete());
            }
            return cnt;
        }


        /// <summary>
        /// 根据地块ID来删除地块对应的界址点界址线
        /// </summary>
        /// <param name="ID"></param>
        public void DeleteCoilDotByLandIDs(List<Guid> landIDs)
        {
            //var coilArray = (from l in DataSource.CreateQuery<ContractLand>()
            //                join d in DataSource.CreateQuery<BuildLandBoundaryAddressCoil>() on l.ID equals d.LandID
            //                where landIDs.Contains(l.ID)
            //                select new { DotID = d.ID }).Select(m => m.DotID).ToArray();

            //var dotArray = (from l in DataSource.CreateQuery<ContractLand>()
            //                join d in DataSource.CreateQuery<BuildLandBoundaryAddressDot>() on l.ID equals d.LandID
            //                where landIDs.Contains(l.ID)
            //                select new { DotID = d.ID }).Select(m => m.DotID).ToArray();

            //AppendEdit(DataSource.CreateQuery<BuildLandBoundaryAddressDot>().Where(c => dotArray.Contains(c.ID)).Delete());
            //AppendEdit(DataSource.CreateQuery<BuildLandBoundaryAddressCoil>().Where(c => coilArray.Contains(c.ID)).Delete());
            var landArrayIDs = landIDs.ToArray();
            AppendEdit(DataSource.CreateQuery<BuildLandBoundaryAddressDot>().Where(c => landArrayIDs.Contains(c.LandID)).Delete());
            AppendEdit(DataSource.CreateQuery<BuildLandBoundaryAddressCoil>().Where(c => landArrayIDs.Contains(c.LandID)).Delete());
        }

        /// <summary>
        /// 将数据库中承包地承包方式设置为家庭承包
        /// </summary>
        public void SetDataBaseCBDFamilyCBFS()
        {
            var querydata = DataSource.CreateQuery<ContractLand>();
            var cbdcount = querydata.Select(s => s.ID).ToList().Count;
            if (cbdcount == 0) return;

            var query = DataSource.CreateQuery();
            string commandStrFieldupdate = string.Empty;
            commandStrFieldupdate = string.Format("UPDATE ZD_CBD SET CBFS = '110'");
            query.CommandContext.CommandText.Append(commandStrFieldupdate);
            query.Execute();
            query.CommandContext.CommandText.Clear();
        }

        /// <summary>
        /// 根据承包方状态删除数据
        /// </summary>
        /// <param name="compare">拼接Whrer语句最大个数(此数字过大会导致程序异常)</param>
        /// <param name="ids">查找的地块id集合</param>
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
                            b.AppendFormat("OwnerId != @{0}", j);
                        else
                            b.AppendFormat("OwnerId != @{0}" + " || ", j);
                        b.Append("IsStockLand==false");

                        listObj.Add(ids[index]);
                        j++;
                        index++;
                    }
                    cnt = AppendEdit(DataSource.CreateQuery<T>().Where(b.ToString(), listObj.ToArray()).Delete());
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
                        b.AppendFormat("OwnerId != @{0}", i);
                    else
                        b.AppendFormat("OwnerId != @{0}" + " || ", i);
                    b.Append("IsStockLand==false");
                }
                cnt = AppendEdit(DataSource.CreateQuery<T>().Where(b.ToString(), listObj.ToArray()).Delete());
            }
            else
            {
                for (int i = 0; i < ids.Count; i++)
                {
                    if (i == ids.Count - 1)
                        b.AppendFormat("OwnerId != @{0}", i);
                    else
                        b.AppendFormat("OwnerId != @{0}" + " || ", i);
                }
                b.Append("IsStockLand==false");
                cnt = AppendEdit(DataSource.CreateQuery<T>().Where(b.ToString(), ids.Cast<object>().ToArray()).Delete());
            }

            return cnt;
        }

        /// <summary>
        /// 删除当前地域下指定的承包方状态的所有地块
        /// </summary>
        /// <param name="zoneCode">地域代码</param>
        /// <param name="virtualStatus">承包方状态</param>
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
                    return Delete(c => c.ZoneCode == zoneCode && c.IsStockLand == false);//by 江宇 2016.11.10 如果没有承包方，只删除确权地
                var idsSelf = DataSource.CreateQuery<LandVirtualPerson>().Where(c => c.ZoneCode == zoneCode).ToList().Select(c => c.ID).ToList();
                cnt = DeleteByVitualPerson(compare, idsSelf);
            }
            else if (levelOption == eLevelOption.SelfAndSubs)
            {
                int count = DataSource.CreateQuery<LandVirtualPerson>().Where(c => c.ZoneCode.StartsWith(zoneCode)).Count();//.Select(c => c.ID)
                if (count == 0)
                    return Delete(c => c.ZoneCode.StartsWith(zoneCode));//by 江宇 2016.11.10 如果没有承包方，只删除确权地
                var idsSelfAndSubs = DataSource.CreateQuery<LandVirtualPerson>().Where(c => c.ZoneCode.StartsWith(zoneCode)).ToList().Select(c => c.ID).ToList();
                cnt = DeleteByVitualPerson(compare, idsSelfAndSubs);
            }
            else//只管子集
            {
                int count = DataSource.CreateQuery<LandVirtualPerson>().Where(c => c.ZoneCode.StartsWith(zoneCode) && c.ZoneCode != zoneCode).Count();//.Select(c => c.ID)
                if (count == 0)
                    return Delete(c => c.ZoneCode.StartsWith(zoneCode) && c.SenderCode != zoneCode);//by 江宇 2016.11.10 如果没有承包方，只删除确权地
                var idsSubs = DataSource.CreateQuery<LandVirtualPerson>().Where(c => c.ZoneCode.StartsWith(zoneCode) && c.ZoneCode != zoneCode).ToList().Select(c => c.ID).ToList();
                cnt = DeleteByVitualPerson(compare, idsSubs);
            }

            return cnt;
        }

        /// <summary>
        /// 根据目标地块，返回相交地块集合，不包括传入的地块
        /// </summary>
        /// <param name="tagetLand"></param>
        /// <returns></returns>
        public List<ContractLand> GetIntersectLands(ContractLand tagetLand, Geometry tagetLandShape)
        {
            List<ContractLand> intersectLands = new List<ContractLand>();
            if (tagetLand.Shape == null || tagetLandShape == null || tagetLandShape.IsValid() == false)
            {
                return intersectLands;
            }
            var results = DataSource.CreateQuery<ContractLand>().Where(c => c.Shape.Intersects(tagetLandShape) && c.ID != tagetLand.ID).ToList();
            if (results != null) intersectLands = results;
            return intersectLands;
        }


        /// <summary>
        /// 获取指定合同Id下的地块面积
        /// </summary>
        /// <param name="concordId">合同id</param>
        /// <returns>地块面积</returns>
        public double GetLandAreaByConcordID(Guid concordId)
        {
            if (!CheckTableExist())
            {
                throw new ArgumentNullException("数据库不存在表："
                    + this.GetType().ToString().Substring(this.GetType().ToString().LastIndexOf('.') + 1).Replace("Repository", ""));
            }
            if (!CheckRule.CheckGuidNullOrEmpty(concordId))
                return 0;
            return (double)Get(c => c.ConcordId.Equals(concordId)).Sum(c => c.AwareArea);
        }

        /// <summary>
        /// 根据区域代码获得所有该区域下的并且以地籍号排序的土地对象集合【CadastralNumber.Contains(zoneCode)】
        /// </summary>
        /// <param name="zoneCode">区域代码</param>
        /// <returns>以地籍号排序的承包地对象集合</returns>
        public List<T> GetLandsByZoneCodeInCadastralNumber(string zoneCode)
        {
            if (!CheckTableExist())
            {
                throw new ArgumentNullException("数据库不存在表："
                    + this.GetType().ToString().Substring(this.GetType().ToString().LastIndexOf('.') + 1).Replace("Repository", ""));
            }
            if (!CheckRule.CheckStringNullOrEmpty(ref zoneCode))
                return null;
            var entity = Get(c => c.CadastralNumber.Contains(zoneCode)).OrderBy(c => c.CadastralNumber).ToList();
            return entity;
        }

        /// <summary>
        /// 按地域统计
        /// </summary>
        /// <param name="zoneCode">区域代码</param>
        /// <param name="searchOption">匹配等级</param>
        /// <returns>地块数量</returns>
        public int Count(string zoneCode, eLevelOption searchOption)
        {
            if (!CheckTableExist())
            {
                throw new ArgumentNullException("数据库不存在表："
                    + this.GetType().ToString().Substring(this.GetType().ToString().LastIndexOf('.') + 1).Replace("Repository", ""));
            }
            if (zoneCode == null)
                return 0;

            zoneCode = zoneCode.Trim();
            if (String.IsNullOrEmpty(zoneCode))
                return 0;

            if (searchOption == eLevelOption.Self)
                return Count(c => c.ZoneCode.Equals(zoneCode));
            else if (searchOption == eLevelOption.SelfAndSubs)
                return Count(c => c.ZoneCode.StartsWith(zoneCode));
            else
                return Count(c => c.ZoneCode.StartsWith(zoneCode) && c.ZoneCode != zoneCode);
        }

        /// <summary>
        /// 按地域统计面积
        /// </summary>
        /// <param name="zoneCode">地域代码</param>
        /// <param name="searchOption">匹配等级</param>
        /// <returns>面积</returns>
        public double CountArea(string zoneCode, eLevelOption searchOption)
        {
            if (!CheckTableExist())
            {
                throw new ArgumentNullException("数据库不存在表："
                    + this.GetType().ToString().Substring(this.GetType().ToString().LastIndexOf('.') + 1).Replace("Repository", ""));
            }
            if (!CheckRule.CheckStringNullOrEmpty(ref zoneCode))
                return 0D;

            object obj = null;
            if (searchOption == eLevelOption.SelfAndSubs)
                obj = Get(c => c.ZoneCode.StartsWith(zoneCode)).Sum(c => c.AwareArea);
            else if (searchOption == eLevelOption.Self)
                obj = Get(c => c.ZoneCode.Equals(zoneCode)).Sum(c => c.AwareArea);
            else
                obj = Get(c => c.ZoneCode.StartsWith(zoneCode) && c.ZoneCode != zoneCode).Sum(c => c.AwareArea);

            if (obj == DBNull.Value)
                return 0D;

            return double.Parse(obj.ToString());
        }

        /// <summary>
        /// 统计地域下实测面积
        /// </summary>
        /// <param name="zoneCode">地域代码</param>
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
                return 0D;
            if (searchOption == eLevelOption.SelfAndSubs)
                return Get(c => c.ZoneCode.StartsWith(zoneCode)).Sum(c => c.ActualArea);
            else if (searchOption == eLevelOption.Self)
                return Get(c => c.ZoneCode.Equals(zoneCode)).Sum(c => c.ActualArea);
            else
                return Get(c => c.ZoneCode.StartsWith(zoneCode) && c.ZoneCode != zoneCode).Sum(c => c.ActualArea);
        }

        /// <summary>
        /// 统计区域下没有合同的地块
        /// </summary>
        /// <param name="zoneCode">地域代码</param>
        /// <param name="searchOption">匹配等级</param>
        /// <returns>地块数量</returns>
        public int CountNoConcord(string zoneCode, eLevelOption searchOption)
        {
            if (!CheckTableExist())
            {
                throw new ArgumentNullException("数据库不存在表："
                    + this.GetType().ToString().Substring(this.GetType().ToString().LastIndexOf('.') + 1).Replace("Repository", ""));
            }
            if (!CheckRule.CheckStringNullOrEmpty(ref zoneCode))
                return 0;
            if (searchOption == eLevelOption.SelfAndSubs)
                return Count(c => c.ZoneCode.StartsWith(zoneCode) && c.ConcordId.Equals(null));
            else if (searchOption == eLevelOption.Self)
                return Count(c => c.ZoneCode.Equals(zoneCode) && c.ConcordId.Equals(null));
            else
                return Count(c => c.ZoneCode.StartsWith(zoneCode) && c.ZoneCode != zoneCode && c.ConcordId.Equals(null));
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
                cnt = Delete(c => c.ZoneCode.Equals(zoneCode));
            else if (searchOption == eLevelOption.SelfAndSubs)
                cnt = Delete(c => c.ZoneCode.StartsWith(zoneCode));
            else
                cnt = Delete(c => c.ZoneCode.StartsWith(zoneCode) && c.ZoneCode != zoneCode);

            return cnt;
        } 

        /// <summary>
        /// 按地域获取地块集合
        /// </summary>
        /// <param name="zoneCode">地域代码</param>
        /// <param name="searchOption">匹配等级</param>
        /// <returns>地块集合</returns>
        public List<T> GetCollection(string zoneCode, eLevelOption searchOption)
        {
            if (!CheckTableExist())
            {
                throw new ArgumentNullException("数据库不存在表："
                    + this.GetType().ToString().Substring(this.GetType().ToString().LastIndexOf('.') + 1).Replace("Repository", ""));
            }
            if (!CheckRule.CheckStringNullOrEmpty(ref zoneCode))
                return null;

            List<T> entity = null;

            if (searchOption == eLevelOption.Self)
                entity = Get(c => c.ZoneCode.Equals(zoneCode));
            else if (searchOption == eLevelOption.SelfAndSubs)
                entity = Get(c => c.ZoneCode.StartsWith(zoneCode));
            else if (searchOption == eLevelOption.Subs)
                entity = Get(c => c.ZoneCode.StartsWith(zoneCode) && c.ZoneCode != zoneCode);
            return entity;
        }

        /// <summary>
        /// 获取指定地域下的所有空间地块集合
        /// </summary>
        /// <param name="zoneCode">指定地域编码</param>
        /// <param name="levelOption">查找地域级别</param>
        /// <returns>空间地块集合</returns>
        public List<T> GetShapeCollection(string zoneCode, eLevelOption levelOption)
        {
            if (!CheckRule.CheckStringNullOrEmpty(ref zoneCode))
                return null;
            List<T> entity = null;
            if (levelOption == eLevelOption.Self)
            {
                entity = (from q in DataSource.CreateQuery<T>()
                          where q.ZoneCode.Equals(zoneCode) && q.Shape != null
                          select q).ToList();
            }
            else if (levelOption == eLevelOption.SelfAndSubs)
            {
                entity = (from q in DataSource.CreateQuery<T>()
                          where q.ZoneCode.StartsWith(zoneCode) && q.Shape != null
                          select q).ToList();
            }
            else
            {
                entity = (from q in DataSource.CreateQuery<T>()
                          where q.ZoneCode.StartsWith(zoneCode) && q.ZoneCode != zoneCode && q.Shape != null
                          select q).ToList();
            }
            return entity;
        }

        /// <summary>
        /// 按地域匹配等级获取指定承包方状态的地块集合
        /// </summary>
        /// <param name="zoneCode">地域代码</param>
        /// <param name="virtualStatus">承包方状态</param>
        /// <param name="levelOption">匹配等级</param>
        /// <returns>地块集合</returns>
        public List<T> GetCollection(string zoneCode, eVirtualPersonStatus virtualStatus, eLevelOption levelOption)
        {
            if (!CheckRule.CheckStringNullOrEmpty(ref zoneCode))
                return null;

            List<T> entity = null;
            int compare = 50;

            if (levelOption == eLevelOption.Self)
            {
                int count = DataSource.CreateQuery<LandVirtualPerson>().Where(c => c.ZoneCode == zoneCode && c.Status == virtualStatus).Count();//.Select(c => c.ID)
                if (count == 0)
                    return entity;
                var idsSelf = DataSource.CreateQuery<LandVirtualPerson>().Where(c => c.ZoneCode == zoneCode && c.Status == virtualStatus).Select(s => new { s.ID }).ToList().Select(c => c.ID).ToList();
                entity = GetByVirtualPerson(compare, idsSelf);
            }
            else if (levelOption == eLevelOption.SelfAndSubs)
            {
                int count = DataSource.CreateQuery<LandVirtualPerson>().Where(c => c.ZoneCode.StartsWith(zoneCode) && c.Status == virtualStatus).Count();//.Select(c => c.ID)
                if (count == 0)
                    return entity;
                var idsSelfAndSubs = DataSource.CreateQuery<LandVirtualPerson>().Where(c => c.ZoneCode.StartsWith(zoneCode) && c.Status == virtualStatus).Select(s => new { s.ID }).ToList().Select(c => c.ID).ToList();
                entity = GetByVirtualPerson(compare, idsSelfAndSubs);
            }
            else
            {
                int count = DataSource.CreateQuery<LandVirtualPerson>().Where(c => c.ZoneCode.StartsWith(zoneCode) && c.ZoneCode != zoneCode && c.Status == virtualStatus).Count();//.Select(c => c.ID)
                if (count == 0)
                    return entity;
                var idsSubs = DataSource.CreateQuery<LandVirtualPerson>().Where(c => c.ZoneCode.StartsWith(zoneCode) && c.ZoneCode != zoneCode && c.Status == virtualStatus).Select(s => new { s.ID }).ToList().Select(c => c.ID).ToList();
                entity = GetByVirtualPerson(compare, idsSubs);
            }

            return entity;
        }

        /// <summary>
        /// 根据承包方状态获取承包地块集合
        /// </summary>
        /// <param name="compare">拼接Whrer语句最大个数(此数字过大会导致程序异常)</param>
        /// <param name="vpIds">查找的承包方id集合</param>
        private List<T> GetByVirtualPerson(int compare, List<Guid> vpIds)
        {
            List<T> listLand = new List<T>();
            StringBuilder b = new StringBuilder();
            if (vpIds.Count > compare)
            {
                List<T> lands = new List<T>();
                int count = vpIds.Count / compare;
                List<object> listObj = new List<object>();
                int index = 0;
                for (int i = 0; i < count; i++)
                {
                    int j = 0;
                    while (j < compare)
                    {
                        if (j == compare - 1)
                            b.AppendFormat("OwnerId == @{0}", j);
                        else
                            b.AppendFormat("OwnerId == @{0}" + " || ", j);
                        listObj.Add(vpIds[index]);
                        j++;
                        index++;
                    }
                    lands = DataSource.CreateQuery<T>().Where(b.ToString(), listObj.ToArray()).ToList();
                    listLand.AddRange(lands);
                    b.Clear();
                    listObj.Clear();
                    lands.Clear();
                }
                int other = vpIds.Count - (compare * count);
                if (other == 0)
                    return listLand;
                for (int i = 0; i < other; i++)
                {
                    listObj.Add(vpIds[index + i]);
                    if (i == other - 1)
                        b.AppendFormat("OwnerId == @{0}", i);
                    else
                        b.AppendFormat("OwnerId == @{0}" + " || ", i);
                }
                lands = DataSource.CreateQuery<T>().Where(b.ToString(), listObj.ToArray()).ToList();
                listLand.AddRange(lands);
            }
            else
            {
                for (int i = 0; i < vpIds.Count; i++)
                {
                    if (i == vpIds.Count - 1)
                        b.AppendFormat("OwnerId == @{0}", i);
                    else
                        b.AppendFormat("OwnerId == @{0}" + " || ", i);
                }
                listLand = DataSource.CreateQuery<T>().Where(b.ToString(), vpIds.Cast<object>().ToArray()).ToList();
            }

            return listLand;
        }

        /// <summary>
        /// 根据地域及地域级别查询指定承包方状态下没有界址信息地块
        /// </summary>
        /// <param name="zoneCode">地域信息</param>
        /// <param name="option">地域级别</param>
        /// <returns>查询结果</returns>
        public List<T> GetLandsWithoutSiteInfoByZone(string zoneCode, eVirtualPersonStatus virtualStatus, eLevelOption option = eLevelOption.Self)
        {
            if (!CheckRule.CheckStringNullOrEmpty(ref zoneCode))
                return null;

            var table = DataSource.CreateQuery<T>().Where(c => c.ZoneCode == zoneCode);
            if (eLevelOption.SelfAndSubs == option)
                table = DataSource.CreateQuery<T>().Where(l => l.ZoneCode.StartsWith(zoneCode));
            else if (eLevelOption.Subs == option)
                table = DataSource.CreateQuery<T>().Where(l => l.ZoneCode.StartsWith(zoneCode) && l.ZoneCode != zoneCode);

            var ids = (from dot in DataSource.CreateQuery<BuildLandBoundaryAddressDot>()
                       join coil in DataSource.CreateQuery<BuildLandBoundaryAddressCoil>()
                       on dot.LandID equals coil.LandID
                       where dot.ZoneCode == zoneCode
                       select new { DKID = dot.LandID }).ToList().Select(c => c.DKID);
            if (eLevelOption.SelfAndSubs == option)
                ids = (from dot in DataSource.CreateQuery<BuildLandBoundaryAddressDot>()
                       join coil in DataSource.CreateQuery<BuildLandBoundaryAddressCoil>()
                       on dot.LandID equals coil.LandID
                       where dot.ZoneCode.StartsWith(zoneCode)
                       select new { DKID = dot.LandID }).ToList().Select(c => c.DKID);
            else if (eLevelOption.Subs == option)
                ids = (from dot in DataSource.CreateQuery<BuildLandBoundaryAddressDot>()
                       join coil in DataSource.CreateQuery<BuildLandBoundaryAddressCoil>()
                       on dot.LandID equals coil.LandID
                       where dot.ZoneCode.StartsWith(zoneCode) && dot.ZoneCode != zoneCode
                       select new { DKID = dot.LandID }).ToList().Select(c => c.DKID);
            if (ids == null)
                return null;
            var arrayIds = ids.Distinct().ToArray();
            var queryNo = table.Where(c => !arrayIds.Contains(c.ID));
            if (queryNo.ToList() == null)
                return null;

            var ownerIds = (from obligee in DataSource.CreateQuery<LandVirtualPerson>()
                            where obligee.ZoneCode == zoneCode && obligee.Status == virtualStatus
                            select new { QLRID = obligee.ID }).ToList().Select(c => c.QLRID);
            if (eLevelOption.SelfAndSubs == option)
                ownerIds = (from obligee in DataSource.CreateQuery<LandVirtualPerson>()
                            where obligee.ZoneCode.StartsWith(zoneCode) && obligee.Status == virtualStatus
                            select new { QLRID = obligee.ID }).ToList().Select(c => c.QLRID);
            else if (eLevelOption.Subs == option)
                ownerIds = (from obligee in DataSource.CreateQuery<LandVirtualPerson>()
                            where obligee.ZoneCode.StartsWith(zoneCode) && obligee.ZoneCode != zoneCode && obligee.Status == virtualStatus
                            select new { QLRID = obligee.ID }).ToList().Select(c => c.QLRID);
            if (ownerIds == null)
                return null;
            var arrayOwnerIds = ownerIds.Distinct().ToArray();
            return queryNo.Where(a => arrayOwnerIds.Contains((Guid)a.OwnerId)).ToList();
        }

        /// <summary>
        /// 根据承包方标识集合获取地块集合
        /// </summary>
        /// <param name="obligeeIds">承包方标识集合</param>
        /// <returns>查询地块集合</returns>
        public List<T> GetLandsByObligeeIds(Guid[] obligeeIds)
        {
            if (obligeeIds == null)
                return null;
            if (obligeeIds.Length == 0)
                return null;

            var data = DataSource.CreateQuery<T>().Where(c => obligeeIds.Contains((Guid)c.OwnerId)).ToList();
            return data;
        }

        /// <summary>
        /// 按承包方ID统计地块
        /// </summary>
        /// <param name="ownerId">承包方ID</param>
        /// <returns>地块数量</returns>
        public int Count(Guid ownerId)
        {
            if (!CheckTableExist())
            {
                throw new ArgumentNullException("数据库不存在表："
                    + this.GetType().ToString().Substring(this.GetType().ToString().LastIndexOf('.') + 1).Replace("Repository", ""));
            }
            return Count(c => c.OwnerId.Equals(ownerId));
        }

        /// <summary>
        /// 根据地籍号获取以承包方名称排序的承包地地块集合对象
        /// </summary>
        /// <param name="cadastralNumber">地籍号</param>
        /// <param name="searchType">查找类型</param>
        /// <returns>以承包方名称排序的承包地地块集合对象</returns>
        public List<T> SearchByCadastralNumber(string cadastralNumber, eSearchOption searchType)
        {
            if (!CheckTableExist())
            {
                throw new ArgumentNullException("数据库不存在表："
                    + this.GetType().ToString().Substring(this.GetType().ToString().LastIndexOf('.') + 1).Replace("Repository", ""));
            }
            if (!CheckRule.CheckStringNullOrEmpty(ref cadastralNumber))
                return null;

            List<T> data = null;

            if (searchType == eSearchOption.Precision)
                data = Get(c => c.CadastralNumber.EndsWith(cadastralNumber)).OrderBy(c => c.OwnerName).ToList();
            else if (searchType == eSearchOption.Fuzzy)
                data = Get(c => c.CadastralNumber.Contains(cadastralNumber)).OrderBy(c => c.OwnerName).ToList();

            return data;
        }

        /// <summary>
        /// 根据承包方名称获取以承包方名称排序的承包地地块集合对象
        /// </summary>
        /// <param name="ownerName">承包方名称</param>
        /// <param name="searchType">查找类型</param>
        /// <returns>以承包方名称排序的承包地地块集合对象</returns>
        public List<T> SearchByVirtualPersonName(string ownerName, eSearchOption searchType)
        {
            if (!CheckTableExist())
            {
                throw new ArgumentNullException("数据库不存在表："
                    + this.GetType().ToString().Substring(this.GetType().ToString().LastIndexOf('.') + 1).Replace("Repository", ""));
            }
            if (!CheckRule.CheckStringNullOrEmpty(ref ownerName))
                return null;

            List<T> data = null;

            if (searchType == eSearchOption.Precision)
                data = Get(c => c.OwnerName.Equals(ownerName)).OrderBy(c => c.OwnerName).ToList();
            else if (searchType == eSearchOption.Fuzzy)
                data = Get(c => c.OwnerName.Contains(ownerName)).OrderBy(c => c.OwnerName).ToList();

            return data;
        }

        /// <summary>
        /// 根据地籍号获取指定区域下以承包方名称排序的承包地地块集合对象
        /// </summary>
        /// <param name="cadastralNumber">地籍号</param>
        /// <param name="searchType">查找类型</param>
        /// <param name="zoneCode">区域代码</param>
        /// <returns>以承包方名称排序的承包地地块集合对象；区域代码为空时，返回所有满足指定地籍号的并以承包方名称排序的地块集合</returns>
        public List<T> SearchByCadastralNumber(string cadastralNumber, eSearchOption searchType, string zoneCode)
        {
            if (!CheckTableExist())
            {
                throw new ArgumentNullException("数据库不存在表："
                    + this.GetType().ToString().Substring(this.GetType().ToString().LastIndexOf('.') + 1).Replace("Repository", ""));
            }
            if (!CheckRule.CheckStringNullOrEmpty(ref cadastralNumber))
                return null;

            if (string.IsNullOrEmpty(zoneCode))
                return SearchByCadastralNumber(cadastralNumber, searchType);

            List<T> data = null;

            if (searchType == eSearchOption.Precision)
                data = Get(c => c.CadastralNumber.EndsWith(cadastralNumber) && c.ZoneCode.StartsWith(zoneCode)).OrderBy(c => c.OwnerName).ToList();
            else if (searchType == eSearchOption.Fuzzy)
                data = Get(c => c.CadastralNumber.Contains(cadastralNumber) && c.ZoneCode.StartsWith(zoneCode)).OrderBy(c => c.OwnerName).ToList();
            return data;
        }

        /// <summary>
        /// 根据承包方名称获取指定区域下以承包方名称排序的承包地地块集合对象
        /// </summary>
        /// <param name="ownerName">承包方名称</param>
        /// <param name="searchType">查找类型</param>
        /// <param name="zoneCode">区域代码</param>
        /// <returns>以承包方名称排序的承包地地块集合对象；区域代码为空时，返回所有满足指定承包方名称的并以承包方名称排序的地块集合</returns>
        public List<T> SearchByVirtualPersonName(string ownerName, eSearchOption searchType, string zoneCode)
        {
            if (!CheckTableExist())
            {
                throw new ArgumentNullException("数据库不存在表："
                    + this.GetType().ToString().Substring(this.GetType().ToString().LastIndexOf('.') + 1).Replace("Repository", ""));
            }
            if (!CheckRule.CheckStringNullOrEmpty(ref ownerName))
                return null;

            if (string.IsNullOrEmpty(zoneCode))
                return SearchByVirtualPersonName(ownerName, searchType);

            List<T> data = null;

            if (searchType == eSearchOption.Precision)
                data = Get(c => c.OwnerName.Equals(ownerName) && c.ZoneCode.StartsWith(zoneCode)).OrderBy(c => c.OwnerName).ToList();
            else if (searchType == eSearchOption.Fuzzy)
                data = Get(c => c.OwnerName.Contains(ownerName) && c.ZoneCode.StartsWith(zoneCode)).OrderBy(c => c.OwnerName).ToList();

            return data;
        }

        /// <summary>
        /// 按承包经营权类型搜索指定区域的土地对象集合
        /// </summary>
        /// <param name="zoneCode">区域代码</param>
        /// <param name="constructType">承包经营权类型</param>
        /// <returns>土地对象集合；如果区域代码为null，返回空的承包地集合对象</returns>
        public List<T> SearchByLandType(string zoneCode, string constructType)
        {
            if (!CheckTableExist())
            {
                throw new ArgumentNullException("数据库不存在表："
                    + this.GetType().ToString().Substring(this.GetType().ToString().LastIndexOf('.') + 1).Replace("Repository", ""));
            }
            if (string.IsNullOrEmpty(zoneCode))
                return new List<T>();

            List<T> entity = Get(c => c.ZoneCode.Equals(zoneCode) && c.LandCategory.Equals(constructType));
            return entity;
        }

        /// <summary>
        /// 按承包经营权类型搜索指定承包地地籍区编号的土地对象集合
        /// </summary>
        /// <param name="cadastralZoneCode">地籍区编号</param>
        /// <param name="constructType">承包经营权类型</param>
        /// <returns>土地对象集合；如果区域代码为null，返回空的承包地集合对象</returns>
        public List<T> SearchByLandTypeAndCadaZone(string cadastralZoneCode, string constructType)
        {
            if (!CheckTableExist())
            {
                throw new ArgumentNullException("数据库不存在表："
                    + this.GetType().ToString().Substring(this.GetType().ToString().LastIndexOf('.') + 1).Replace("Repository", ""));
            }
            if (string.IsNullOrEmpty(cadastralZoneCode))
                return new List<T>();

            List<T> entity = Get(c => c.CadastralZoneCode.Equals(cadastralZoneCode) && c.LandCategory.Equals(constructType));
            return entity;
        }

        /// <summary>
        /// 按区域搜索指定地类名称的承包地块对象
        /// </summary>
        ///<param name="landName">地类名称</param>
        /// <param name="constructType">承包经营权类型</param>
        /// <returns>承包地对象</returns>
        public T SearchByLandNameAndZoneCode(string landName, string zoneCode)
        {
            if (!CheckTableExist())
            {
                throw new ArgumentNullException("数据库不存在表："
                    + this.GetType().ToString().Substring(this.GetType().ToString().LastIndexOf('.') + 1).Replace("Repository", ""));
            }
            if (!CheckRule.CheckStringNullOrEmpty(ref zoneCode) || !CheckRule.CheckStringNullOrEmpty(ref landName))
                return null;

            T entity = Get(c => c.ZoneCode.Equals(zoneCode) && c.LandName.Equals(landName)).FirstOrDefault();

            return entity;
        }

        public T GetByLandNumber(string landNumber)
        {
            if (!CheckTableExist())
            {
                throw new ArgumentNullException("数据库不存在表："
                    + this.GetType().ToString().Substring(this.GetType().ToString().LastIndexOf('.') + 1).Replace("Repository", ""));
            }
            if (!CheckRule.CheckStringNullOrEmpty(ref landNumber))
                return null;

            T entity = Get(c => c.LandNumber.Equals(landNumber)).FirstOrDefault();
            return entity;
        }
        public List<T> GetLandsBYGraph(Geometry graph)
        {
            if (!CheckTableExist())
            {
                throw new ArgumentNullException("数据库不存在表："
                    + this.GetType().ToString().Substring(this.GetType().ToString().LastIndexOf('.') + 1).Replace("Repository", ""));
            }
            if (graph == null)
                return new List<T>();

            List<T> entity = Get(c => c.Shape.Intersects(graph)).ToList();
            return entity;
        }

        #endregion
    }
}
