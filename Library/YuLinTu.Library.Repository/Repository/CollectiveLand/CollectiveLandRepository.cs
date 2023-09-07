// (C) 2015 鱼鳞图公司版权所有，保留所有权利
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
    /// 集体土地所有权的数据访问类
    /// </summary>
    public class CollectiveLandRepository : RepositoryDbContext<CollectiveLand>,  ICollectiveLandRepository
    {
        #region Ctor
        
        private IDataSourceSchema m_DSSchema = null;

        public CollectiveLandRepository(IDataSource ds)
            : base(ds) 
        {
            m_DSSchema = ds.CreateSchema();
        }
        #endregion

        #region Method

        /// <summary>
        /// 检查表是否存在
        /// </summary>
        /// <returns></returns>
        private bool CheckTableExist()
        {
            //try
            //{
            //    return m_DSSchema.AnyElement(m_DSSchema.GetElements()[0].Schema, "CollectiveLand");
            //}
            //catch (Exception)
            //{
            //    return false;
            //}

            return true;
        }

        /// <summary>
        /// 获得与目标标识码相同的集体土地所有权对象
        /// </summary>
        /// <param name="collectiveLandID">标识码</param>
        /// <returns>集体土地所有权</returns>
        public CollectiveLand Get(Guid collectiveLandID)
        {
            if (!CheckTableExist())
            {
                throw new ArgumentNullException("数据库不存在表："
                    + this.GetType().ToString().Substring(this.GetType().ToString().LastIndexOf('.') + 1).Replace("Repository", ""));
            }
            if (!CheckRule.CheckGuidNullOrEmpty(collectiveLandID))
                return null;
            object data = Get(c => c.ID.Equals(collectiveLandID));
            return (CollectiveLand)data;
        }

        /// <summary>
        /// 删除与目标标识码相同的集体土地所有权对象
        /// </summary>
        /// <param name="collectiveLandID">标识码</param>
        /// <returns>-1（参数错误）/0（失败）/1（成功）</returns>
        public int Delete(Guid collectiveLandID)
        {
            if (!CheckTableExist())
            {
                throw new ArgumentNullException("数据库不存在表："
                    + this.GetType().ToString().Substring(this.GetType().ToString().LastIndexOf('.') + 1).Replace("Repository", ""));
            }
            if (!CheckRule.CheckGuidNullOrEmpty(collectiveLandID))
                return 0;
            return Delete(c => c.ID.Equals(collectiveLandID));
        }

        /// <summary>
        /// 删除与目标权属单位代码相同的集体土地所有权对象
        /// </summary>
        /// <param name="ownUnitCode">权属单位代码</param>
        /// <returns>-1（参数错误）/0（失败）/1（成功）</returns>
        public int DeleteByZoneCode(string ownUnitCode)
        {
            if (!CheckTableExist())
            {
                throw new ArgumentNullException("数据库不存在表："
                    + this.GetType().ToString().Substring(this.GetType().ToString().LastIndexOf('.') + 1).Replace("Repository", ""));
            }
            if (!CheckRule.CheckStringNullOrEmpty(ref ownUnitCode))
                return 0;
            return Delete(c => c.ZoneCode.Equals(ownUnitCode));
        }

        /// <summary>
        /// 更新集体土地所有权对象
        /// </summary>
        /// <param name="collectiveLand">集体土地所有权对象</param>
        /// <returns>-1（参数错误）/0（失败）/1（成功）</returns>
        public int Update(CollectiveLand collectiveLand)
        {
            if (!CheckTableExist())
            {
                throw new ArgumentNullException("数据库不存在表："
                    + this.GetType().ToString().Substring(this.GetType().ToString().LastIndexOf('.') + 1).Replace("Repository", ""));
            }
            if (collectiveLand == null || collectiveLand.ID == null)
                return 0;
            collectiveLand.ModifiedTime = DateTime.Now;
            return Update(collectiveLand, c => c.ID.Equals(collectiveLand.ID));
        }

        /// <summary>
        /// 根据承包方ID更新其土地所有权人名称
        /// </summary>
        /// <param name="houseHolderID">承包方ID</param>
        /// <param name="houseHolderName">土地所有权人</param>
        /// <returns>-1（参数错误）/0（失败）/1（成功）</returns>
        public int Update(Guid houseHolderID, string houseHolderName)
        {
            if (!CheckTableExist())
            {
                throw new ArgumentNullException("数据库不存在表："
                    + this.GetType().ToString().Substring(this.GetType().ToString().LastIndexOf('.') + 1).Replace("Repository", ""));
            }
            if (!CheckRule.CheckGuidNullOrEmpty(houseHolderID) || !CheckRule.CheckStringNullOrEmpty(ref houseHolderName))
                return 0;

            var q = (from qa in DataSource.CreateQuery<CollectiveLand>()
                     where qa.OwnUnitID.Equals(houseHolderID)
                     select qa).FirstOrDefault();
            if (q != null)
            {
                q.OwnUnitName = houseHolderName;
                return Update(q, c => c.ID == q.ID);
            }
            else
                return 0;
        }

        /// <summary>
        /// 根据权属单位代码获得集体土地所有权对象
        /// </summary>
        /// <param name="zoneCode">权属单位代码</param>
        /// <returns>集体土地所有权对象集合</returns>
        public CollectiveLand GetByZoneCode(string zoneCode)
        {
            if (!CheckTableExist())
            {
                throw new ArgumentNullException("数据库不存在表："
                    + this.GetType().ToString().Substring(this.GetType().ToString().LastIndexOf('.') + 1).Replace("Repository", ""));
            }
            if (!CheckRule.CheckStringNullOrEmpty(ref zoneCode))
                return null; 
            object data = Get(c => c.ZoneCode.Equals(zoneCode));
            return (CollectiveLand)data;
        }

        /// <summary>
        /// 根据权属单位代码获得集体土地所有权对象
        /// </summary>
        /// <param name="zoneCode">权属单位代码</param>
        /// <param name="levelOption">匹配等级</param>
        /// <returns>以权属单位代码排序的集体土地所有权对象集合</returns>
        public List<CollectiveLand> GetByZoneCode(string zoneCode, eLevelOption levelOption)
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
                data =(from q in DataSource.CreateQuery<CollectiveLand>()
                          where q.ZoneCode.Equals(zoneCode)
                         orderby q.ZoneCode
                        select q).ToList(); 
            else if (levelOption == eLevelOption.Subs)
                data = (from q in DataSource.CreateQuery<CollectiveLand>()
                        where q.ZoneCode.StartsWith(zoneCode)
                        orderby q.ZoneCode
                        select q).ToList();

            return (List<CollectiveLand>)data;
        }

        /// <summary>
        /// 根据地号获得集体土地所有权对象
        /// </summary>
        /// <param name="landNumber">地号</param>
        /// <returns>以地号排序的集体土地所有权对象集合</returns>
        public List<CollectiveLand> GetByLandNumber(string landNumber)
        {
            if (!CheckTableExist())
            {
                throw new ArgumentNullException("数据库不存在表："
                    + this.GetType().ToString().Substring(this.GetType().ToString().LastIndexOf('.') + 1).Replace("Repository", ""));
            }
            if (string.IsNullOrEmpty(landNumber))
                return null;
            object data = (from q in DataSource.CreateQuery<CollectiveLand>()
                           where q.LandNumber.EndsWith(landNumber)
                           orderby q.LandNumber
                           select q).ToList();
            return (List<CollectiveLand>)data;
        }

        /// <summary>
        /// 根据图号获得集体土地所有权对象
        /// </summary>
        /// <param name="imageNumber">图号</param>
        /// <returns>集体土地所有权对象集合</returns>
        public List<CollectiveLand> GetByImageNumber(string imageNumber)
        {
            if (!CheckTableExist())
            {
                throw new ArgumentNullException("数据库不存在表："
                    + this.GetType().ToString().Substring(this.GetType().ToString().LastIndexOf('.') + 1).Replace("Repository", ""));
            }
            if (string.IsNullOrEmpty(imageNumber))
                return null; 
            object data = Get(c => c.ImageNumber.Equals(imageNumber));
            return (List<CollectiveLand>)data;
        }

        /// <summary>
        /// 根据权属单位代码获得集体土地所有权对象的统计信息
        /// </summary>
        /// <param name="zoneCode">权属单位代码</param>
        /// <param name="levelOption">匹配等级</param>
        /// <returns>统计信息</returns>
        public CollectiveLandAreaInfo CountAreaByZoneCode(string zoneCode, eLevelOption levelOption)
        {
            if (!CheckTableExist())
            {
                throw new ArgumentNullException("数据库不存在表："
                    + this.GetType().ToString().Substring(this.GetType().ToString().LastIndexOf('.') + 1).Replace("Repository", ""));
            }
            if(string.IsNullOrEmpty(zoneCode))
                return null;            

            var qc =DataSource.CreateQuery<CollectiveLand>();
            CollectiveLandAreaInfo info = new CollectiveLandAreaInfo();
            if (levelOption == eLevelOption.Subs)
            {
                var q = (from qa in qc
                         where qa.ZoneCode.StartsWith(zoneCode)
                         select new
                         {
                             sunArea = qc.Sum(c => c.Area),
                             sumBuildLandArea = qc.Sum(c => c.BuildLandArea),
                             sumUndueArea = qc.Sum(c => c.UndueArea),
                             sumFarmArea = qc.Sum(c => c.FarmArea)
                         }).FirstOrDefault();
                info.CountArea = (double)q.sunArea;
                info.CountBuildLandArea = q.sumBuildLandArea;
                info.CountUndueArea = q.sumUndueArea;
                info.CountFarmArea = q.sumFarmArea;

            }
            else 
            {

                var q = (from qa in qc
                         where qa.ZoneCode.Equals(zoneCode)
                         select new
                         {
                             sunArea = qc.Sum(c => c.Area),
                             sumBuildLandArea = qc.Sum(c => c.BuildLandArea),
                             sumUndueArea = qc.Sum(c => c.UndueArea),
                             sumFarmArea = qc.Sum(c => c.FarmArea)
                         }).FirstOrDefault();
                info.CountArea = (double)q.sunArea;
                info.CountBuildLandArea = q.sumBuildLandArea;
                info.CountUndueArea = q.sumUndueArea;
                info.CountFarmArea = q.sumFarmArea;
            }
            return info;
        }
        #endregion
    }
}
