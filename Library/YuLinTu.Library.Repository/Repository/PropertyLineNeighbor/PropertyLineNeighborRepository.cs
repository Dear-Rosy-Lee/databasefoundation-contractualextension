// (C) 2025 鱼鳞图公司版权所有，保留所有权利
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YuLinTu.Data;
using YuLinTu.Library.Entity;

namespace YuLinTu.Library.Repository
{
    /// <summary>
    /// 界址线四至的数据访问类
    /// </summary>
    public class PropertyLineNeighborRepository : RepositoryDbContext<PropertyLineNeighbor>,  IPropertyLineNeighborRepository
    {      
        #region Ctor
        
        private IDataSourceSchema m_DSSchema = null;

        public PropertyLineNeighborRepository(IDataSource ds)
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
            //    return m_DSSchema.AnyElement(m_DSSchema.GetElements()[0].Schema, "PropertyLineNeighbor");
            //}
            //catch (Exception)
            //{
            //    return false;
            //}
            return true;
        }

        /// <summary>
        /// 根据id获得界址线四至数据
        /// </summary>
        /// <param name="id">id</param>
        /// <returns>界址线四至数据</returns>
        public PropertyLineNeighbor Get(Guid id)
        {
            if (!CheckTableExist())
            {
                throw new ArgumentNullException("数据库不存在表："
                    + this.GetType().ToString().Substring(this.GetType().ToString().LastIndexOf('.') + 1).Replace("Repository", ""));
            }
            if (id == Guid.Empty)
                return null;

            object data = Get(c => c.ID.Equals(id));
            return data as PropertyLineNeighbor;
        }

        /// <summary>
        /// 根据id判断界址线四至数据是否存在
        /// </summary>
        /// <param name="id">id</param>
        /// <returns>true（存在）/false（不存在）</returns>
        public int Exist(Guid id)
        {
            if (!CheckTableExist())
            {
                throw new ArgumentNullException("数据库不存在表："
                    + this.GetType().ToString().Substring(this.GetType().ToString().LastIndexOf('.') + 1).Replace("Repository", ""));
            }
            if (id == Guid.Empty)
                return -1;

            return Count(c => c.ID.Equals(id));
        }

        /// <summary>
        /// 根据集体建设用地使用权ID获得界址线四至数据
        /// </summary>
        /// <param name="landID">集体建设用地使用权ID</param>
        /// <returns>界址线四至数据</returns>
        public PropertyLineNeighborCollection GetByLandID(Guid landID)
        {
            if (!CheckTableExist())
            {
                throw new ArgumentNullException("数据库不存在表："
                    + this.GetType().ToString().Substring(this.GetType().ToString().LastIndexOf('.') + 1).Replace("Repository", ""));
            }
            if (!CheckRule.CheckGuidNullOrEmpty(landID))
                return null;
            object data = Get(c => c.LandID.Equals(landID));
            return data as PropertyLineNeighborCollection;
        }

        /// <summary>
        /// 根据id删除界址线四至数据
        /// </summary>
        /// <param name="id">id</param>
        /// <returns>-1（参数错误）/0（失败）/1（成功）</returns>
        public int Delete(Guid id)
        {
            if (!CheckTableExist())
            {
                throw new ArgumentNullException("数据库不存在表："
                    + this.GetType().ToString().Substring(this.GetType().ToString().LastIndexOf('.') + 1).Replace("Repository", ""));
            }
            if (!CheckRule.CheckGuidNullOrEmpty(id))
                return -1;
            int cnt = 0;
            cnt = Delete(c => c.ID.Equals(id));
            return cnt;
        }
        /// <summary>
        /// 根据集体建设用地使用权ID删除界址线四至数据
        /// </summary>
        /// <param name="landID">集体建设用地使用权ID</param>
        /// <returns>-1（参数错误）/0（失败）/1（成功）</returns>
        public int DeleteByLand(Guid landID)
        {
            if (!CheckTableExist())
            {
                throw new ArgumentNullException("数据库不存在表："
                    + this.GetType().ToString().Substring(this.GetType().ToString().LastIndexOf('.') + 1).Replace("Repository", ""));
            }
            if (!CheckRule.CheckGuidNullOrEmpty(landID))
                return -1;
            int cnt = 0;
            cnt = Delete(c => c.LandID.Equals(landID));
            return cnt;
        }

        /// <summary>
        /// 更新界址线四至数据
        /// </summary>
        /// <param name="neighbor">实体</param>
        /// <returns>-1（参数错误）/0（失败）/1（成功）</returns>
        public int Update(PropertyLineNeighbor neighbor)
        {
            if (!CheckTableExist())
            {
                throw new ArgumentNullException("数据库不存在表："
                    + this.GetType().ToString().Substring(this.GetType().ToString().LastIndexOf('.') + 1).Replace("Repository", ""));
            }
            if (neighbor == null ||!CheckRule.CheckGuidNullOrEmpty(neighbor.ID))
                return -1;
            neighbor.ModifiedTime = DateTime.Now;
            int cnt = 0;
            cnt = Update(neighbor, c => c.ID.Equals(neighbor.ID));
            return cnt;
        }
        
		#endregion
	}
}