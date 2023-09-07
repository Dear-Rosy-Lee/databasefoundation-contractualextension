// (C) 2015 鱼鳞图公司版权所有，保留所有权利
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
    /// 地籍调查表的数据访问类
    /// </summary>
    public class CadastralInvestigateRepository : RepositoryDbContext<CadastralInvestigate>,  ICadastralInvestigateRepository
    {
        #region Ctor
        
        private IDataSourceSchema m_DSSchema = null;

        public CadastralInvestigateRepository(IDataSource ds)
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
            //    return m_DSSchema.AnyElement(m_DSSchema.GetElements()[0].Schema, "CadastralInvestigate");
            //}
            //catch (Exception)
            //{
            //    return false;
            //}

            return true;
        }

        /// <summary>
        /// 获得与目标标识码相同的籍调查表对象
        /// </summary>
        /// <param name="id">标识码</param>
        /// <returns></returns>
        public CadastralInvestigate Get(Guid id)
        {
            if (!CheckTableExist())
            {
                throw new ArgumentNullException("数据库不存在表："
                    + this.GetType().ToString().Substring(this.GetType().ToString().LastIndexOf('.') + 1).Replace("Repository", ""));
            }
            if (!CheckRule.CheckGuidNullOrEmpty(id))
                return null;

            object data = Get(c => c.ID.Equals(id));
            return data as CadastralInvestigate;
        }

        /// <summary>
        /// 获得与目标标识码相同的籍调查表对象数量
        /// </summary>
        /// <param name="id">标识码</param>
        public bool Exist(Guid id)
        {
            if (!CheckTableExist())
            {
                throw new ArgumentNullException("数据库不存在表："
                    + this.GetType().ToString().Substring(this.GetType().ToString().LastIndexOf('.') + 1).Replace("Repository", ""));
            }
            if (!CheckRule.CheckGuidNullOrEmpty(id)) 
                return false;

            return Count(c => c.ID.Equals(id))>0;
        }


        /// <summary>
        /// 根据集体建设用地ID获得地籍调查信息
        /// </summary>
        /// <param name="houseHolderId">集体建设用地ID</param>
        /// <returns>地籍调查表对象</returns>
        public CadastralInvestigate GetByLandID(Guid landID)
        {
            if (!CheckTableExist())
            {
                throw new ArgumentNullException("数据库不存在表："
                    + this.GetType().ToString().Substring(this.GetType().ToString().LastIndexOf('.') + 1).Replace("Repository", ""));
            }
            if (!CheckRule.CheckGuidNullOrEmpty(landID)) 
                return null;
            object data = Get(c => c.LandID.Equals(landID));
            return data as CadastralInvestigate;
        }

        /// <summary>
        /// 更新地籍调查信息
        /// </summary>
        /// <param name="investigate">地籍调查信息</param>
        /// <returns>-1（参数错误）/0（失败）/1（成功）</returns>
        public int Update(CadastralInvestigate investigate)
        {
            if (!CheckTableExist())
            {
                throw new ArgumentNullException("数据库不存在表："
                    + this.GetType().ToString().Substring(this.GetType().ToString().LastIndexOf('.') + 1).Replace("Repository", ""));
            }
            if (investigate == null || !CheckRule.CheckGuidNullOrEmpty(investigate.ID))
            {
                return -1;
            }
            return Update(investigate, c => c.ID.Equals(investigate.ID));
        }
        /// <summary>
        /// 根据标识码删除地籍调查信息
        /// </summary>
        /// <param name="investigate">标识码</param>
        /// <returns>-1（参数错误）/0（失败）/1（成功）</returns>
        public int Delete(Guid investigateID)
        {
            if (!CheckTableExist())
            {
                throw new ArgumentNullException("数据库不存在表："
                    + this.GetType().ToString().Substring(this.GetType().ToString().LastIndexOf('.') + 1).Replace("Repository", ""));
            }
            if (!CheckRule.CheckGuidNullOrEmpty(investigateID))
            {
                return -1;
            }

            return Delete(c => c.ID.Equals(investigateID));
        }

        /// <summary>
        /// 根据集体建设用地ID删除地籍调查信息
        /// </summary>
        /// <param name="landID">集体建设用地ID</param>
        /// <returns>-1（参数错误）/0（失败）/1（成功）</returns>
        public int DeleteByLandID(Guid landID)
        {
            if (!CheckTableExist())
            {
                throw new ArgumentNullException("数据库不存在表："
                    + this.GetType().ToString().Substring(this.GetType().ToString().LastIndexOf('.') + 1).Replace("Repository", ""));
            }
            if (landID == null || landID == Guid.Empty)
            {
                return -1;
            }

            return Delete(c => c.LandID.Equals(landID));
        }

        /// <summary>
        /// 根据所在地域删除地籍调查信息
        /// </summary>
        /// <param name="zoneCode">所在地域</param>
        /// <param name="levelOption">匹配等级</param>
        /// <returns>-1（参数错误）/0（失败）/1（成功）</returns>
        public int DeleteByZoneCode(string zoneCode, eLevelOption levelOption)
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

            if(levelOption == eLevelOption.Self)
                return Delete(c=>c.ZoneCode.Equals(zoneCode));
            else
                return Delete(c => c.ZoneCode.StartsWith(zoneCode));
        }
        
		#endregion
	}
}