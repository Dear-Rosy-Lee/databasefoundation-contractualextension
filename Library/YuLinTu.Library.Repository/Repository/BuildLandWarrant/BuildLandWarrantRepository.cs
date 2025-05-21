// (C) 2025 鱼鳞图公司版权所有，保留所有权利
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YuLinTu.Library.Entity;
using System.Data;
using YuLinTu.Data;

namespace YuLinTu.Library.Repository
{
    /// <summary>
    /// 集体建设用地第字号表的数据访问类
    /// </summary>
    public class BuildLandWarrantRepository : RepositoryDbContext<BuildLandWarrant>,  IBuildLandWarrantRepository
    {
         #region Ctor
        
        private IDataSourceSchema m_DSSchema = null;

        public BuildLandWarrantRepository(IDataSource ds)
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
            //    return m_DSSchema.AnyElement(m_DSSchema.GetElements()[0].Schema, "BuildLandWarrant");
            //}
            //catch (Exception)
            //{
            //    return false;
            //}

            return true;
        }

        /// <summary>
        /// 根据权证编号统计与目标权证编号右相似的集体建设用地第字号对象的数量？
        /// </summary>
        /// <param name="number">权证编号</param>
        /// <returns>-1（参数错误）/int 对象数量</returns>
        public bool ExistByWarrantNumber(string number)
        {
            if (!CheckTableExist())
            {
                throw new ArgumentNullException("数据库不存在表："
                    + this.GetType().ToString().Substring(this.GetType().ToString().LastIndexOf('.') + 1).Replace("Repository", ""));
            }
            if (!CheckRule.CheckStringNullOrEmpty(ref number))
                return false;
            return Count(c => c.WarrantNumber.EndsWith(number))>0;
        }

        /// <summary>
        /// 根据权证编号统计集体建设用地第字号对象的数量？
        /// </summary>
        /// <param name="number">权证编号</param>
        /// <returns>-1（参数错误）/int 对象数量</returns>
        public bool ExistAllNumber(string number)
        {
            if (!CheckTableExist())
            {
                throw new ArgumentNullException("数据库不存在表："
                    + this.GetType().ToString().Substring(this.GetType().ToString().LastIndexOf('.') + 1).Replace("Repository", ""));
            }
            if (!CheckRule.CheckStringNullOrEmpty(ref number))
                return false;
            return Count(c => c.WarrantNumber.Equals(number))>0;
        }

        /// <summary>
        ///根据标识码获取集体建设用地第字号对象
        /// </summary>
        /// <param name="id">识码</param>
        /// <returns>集体建设用地第字号对象集合</returns>
        public BuildLandWarrant Get(Guid id)
        {
            if (!CheckTableExist())
            {
                throw new ArgumentNullException("数据库不存在表："
                    + this.GetType().ToString().Substring(this.GetType().ToString().LastIndexOf('.') + 1).Replace("Repository", ""));
            }
            if (!CheckRule.CheckGuidNullOrEmpty(id))
                return null;
            object obj = Get(c => c.ID.Equals(id)); 

            return (BuildLandWarrant)obj;

        }

        /// <summary>
        /// 获取与目标权证编号右相似的集体建设用地第字号对象
        /// </summary>
        /// <param name="warrant">目标权证编号</param>
        /// <returns>集体建设用地第字号对象集合</returns>
        public BuildLandWarrantCollection GetByWarrantNumbe_Right(string warrant)
        {
            if (!CheckTableExist())
            {
                throw new ArgumentNullException("数据库不存在表："
                    + this.GetType().ToString().Substring(this.GetType().ToString().LastIndexOf('.') + 1).Replace("Repository", ""));
            }
            if (!CheckRule.CheckStringNullOrEmpty(ref warrant))
                return null;
            object obj = Get(c => c.WarrantNumber.EndsWith(warrant));

            return (BuildLandWarrantCollection)obj;
        }

        /// <summary>
        /// 获取与目标权证编号相同的集体建设用地第字号对象
        /// </summary>
        /// <param name="warrant">目标权证编号</param>
        /// <returns>集体建设用地第字号对象集合</returns>
        public BuildLandWarrantCollection GetByWarrantNumbe_All(string warrant)
        {
            if (!CheckTableExist())
            {
                throw new ArgumentNullException("数据库不存在表："
                    + this.GetType().ToString().Substring(this.GetType().ToString().LastIndexOf('.') + 1).Replace("Repository", ""));
            }
            if (!CheckRule.CheckStringNullOrEmpty(ref warrant))
                return null;
            object obj = Get(c => c.WarrantNumber.Equals(warrant));

            return (BuildLandWarrantCollection)obj;
        }

        /// <summary>
        /// 根据标识码将集体建设用地第字号对象的打印次数加一
        /// </summary>
        /// <param name="landID">标识码</param>
        public void RecordPrintCount(Guid landID)
        {
            if (!CheckTableExist())
            {
                throw new ArgumentNullException("数据库不存在表："
                    + this.GetType().ToString().Substring(this.GetType().ToString().LastIndexOf('.') + 1).Replace("Repository", ""));
            }
            if (!CheckRule.CheckGuidNullOrEmpty(landID))
                return;

            var q = (from qa in DataSource.CreateQuery<BuildLandWarrant>()
                     where qa.ID.Equals(landID)
                     select qa).FirstOrDefault();
            if (q != null)
            {
                q.PrintCount++;
                Update(q, c => c.ID == q.ID);
            }

        }

        /// <summary>
        /// 更新集体建设用地第字号对象
        /// </summary>
        /// <param name="buildLandWarrant">集体建设用地第字号对象</param>
        /// <returns>-1（错误）/0（失败）/1（成功）</returns>
        public int Update(BuildLandWarrant buildLandWarrant)
        {
            if (!CheckTableExist())
            {
                throw new ArgumentNullException("数据库不存在表："
                    + this.GetType().ToString().Substring(this.GetType().ToString().LastIndexOf('.') + 1).Replace("Repository", ""));
            }
            if (buildLandWarrant == null||!CheckRule.CheckGuidNullOrEmpty(buildLandWarrant.ID))
                return -1;

            buildLandWarrant.ModifiedTime = DateTime.Now;
            int val = 0;
            try
            {
                val = Update(buildLandWarrant, c => c.ID.Equals(buildLandWarrant.ID));
            }
            catch
            {
                return -1;
            }
            return val;

        }

        /// <summary>
        /// 根据地域编号删除集体建设用地第字号表
        /// </summary>
        /// <param name="zoneCode">地域编号</param>
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
            int val = 0;
            try
            {
                val = Delete(c => c.ZoneCode.Equals(zoneCode));
            }
            catch
            {
                return -1;
            }
            return val;

        }

        /// <summary>
        /// 根据地标识码删除集体建设用地第字号表
        /// </summary>
        /// <param name="buildLandWarrantID">标识码</param>
        /// <returns>-1（参数错误）/0（失败）/1（成功）</returns>
        public int Delete(Guid buildLandWarrantID)
        {
            if (!CheckTableExist())
            {
                throw new ArgumentNullException("数据库不存在表："
                    + this.GetType().ToString().Substring(this.GetType().ToString().LastIndexOf('.') + 1).Replace("Repository", ""));
            }
            if (!CheckRule.CheckGuidNullOrEmpty(buildLandWarrantID))
                return -1;
            int val = 0;
            try
            {
                val = Delete(c => c.ID.Equals(buildLandWarrantID));
            }
            catch
            {
                return -1;
            }
            return val;

        }
        #endregion
    }
}