// (C) 2015 鱼鳞图公司版权所有，保留所有权利
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YuLinTu.Data;
using YuLinTu.Library.Entity;

namespace YuLinTu.Library.Repository
{
    /// <summary>
    /// 共有人的数据访问类
    /// </summary>
    public class SharePersonRepository : RepositoryDbContext<SharePerson>,  ISharePersonRepository
    {
        #region Ctor
        
        private IDataSourceSchema m_DSSchema = null;

        public SharePersonRepository(IDataSource ds)
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
            //    return m_DSSchema.AnyElement(m_DSSchema.GetElements()[0].Schema, "SharePerson");
            //}
            //catch (Exception)
            //{
            //    return false;
            //}
            return true;
        }

        /// <summary>
        /// 根据ID删除共有人对象
        /// </summary>
        /// <param name="guid">id</param>
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
            return Delete(c => c.ID.Equals(guid));
        }

        /// <summary>
        /// 更新共有人对象
        /// </summary>
        /// <param name="entity">实体</param>
        /// <returns>-1（参数错误）/0（失败）/1（成功）</returns>
        public int Update(SharePerson entity)
        {
            if (!CheckTableExist())
            {
                throw new ArgumentNullException("数据库不存在表："
                    + this.GetType().ToString().Substring(this.GetType().ToString().LastIndexOf('.') + 1).Replace("Repository", ""));
            }
            if (entity == null||!CheckRule.CheckGuidNullOrEmpty(entity.ID))
                return -1;
            return Update(entity, c => c.ID.Equals(entity.ID));
        }

        /// <summary>
        /// 根据ID获取共有人对象
        /// </summary>
        public SharePerson Get(Guid guid)
        {
            if (!CheckTableExist())
            {
                throw new ArgumentNullException("数据库不存在表："
                    + this.GetType().ToString().Substring(this.GetType().ToString().LastIndexOf('.') + 1).Replace("Repository", ""));
            }
            if (!CheckRule.CheckGuidNullOrEmpty(guid))
                return null;
            object entity = Get(c => c.ID.Equals(guid));
            return entity as SharePerson;
        }

        /// <summary>
        /// 根据ID判断共有人对象是否存在
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
            return  Count(c=>c.ID.Equals(guid))> 0 ? true : false;
        }

        #endregion

        #region ExtendMethod

        /// <summary>
        /// 根据农村土地承包合同ID获取共有人
        /// </summary>
        /// <param name="concordID">农村土地承包合同ID</param>
        /// <returns>共有人对象集合</returns>
        public SharePersonCollection GetByConcordID(Guid concordID)
        {

            if (!CheckTableExist())
            {
                throw new ArgumentNullException("数据库不存在表："
                    + this.GetType().ToString().Substring(this.GetType().ToString().LastIndexOf('.') + 1).Replace("Repository", ""));
            }
            if (!CheckRule.CheckGuidNullOrEmpty(concordID))
                return null; 
            object sharePerson = Get(c => c.ConcordID.Equals(concordID));
            return sharePerson as SharePersonCollection;
        }

        /// <summary>
        /// 根据农村土地承包合同ID删除共有人对象
        /// </summary>
        /// <param name="concordID">农村土地承包合同ID</param>
        /// <returns>-1（参数错误）/0（失败）/1（成功）</returns>
        public int DeleteByConcordID(Guid concordID)
        {
            if (!CheckTableExist())
            {
                throw new ArgumentNullException("数据库不存在表："
                    + this.GetType().ToString().Substring(this.GetType().ToString().LastIndexOf('.') + 1).Replace("Repository", ""));
            }
            if (!CheckRule.CheckGuidNullOrEmpty(concordID))
                return -1;
            return Delete(c => c.ConcordID.Equals(concordID));
        }

        #endregion
	}
}