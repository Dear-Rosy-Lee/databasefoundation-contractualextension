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
    /// 土地利用现状分类表的数据访问类
    /// </summary>
    public class LandTypeRepository : RepositoryDbContext<LandType>,  ILandTypeRepository
    {      
        #region Ctor
        
        private IDataSourceSchema m_DSSchema = null;

        public LandTypeRepository(IDataSource ds)
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
                return m_DSSchema.AnyElement(m_DSSchema.GetElements()[0].Schema, "LandType");
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// 更新土地利用现状分类表对象
        /// </summary>
        /// <returns>-1（参数错误）/0（失败）/1（成功）</returns>
        public int Update(LandType landType)
        {
            if (!CheckTableExist())
            {
                throw new ArgumentNullException("数据库不存在表："
                    + this.GetType().ToString().Substring(this.GetType().ToString().LastIndexOf('.') + 1).Replace("Repository", ""));
            }
            if (landType == null)
                return -1;

            int cnt = 0;
            cnt = Update(landType, c => c.ID.Equals(landType.ID));
            return cnt;
        }

        #endregion

        #region ExtendMethods
        /// <summary>
        /// 根据标识码删除土地利用现状分类表对象
        /// </summary>
        /// <returns>-1（参数错误）/0（失败）/1（成功）</returns>
        public int Delete(int id)
        {
            if (!CheckTableExist())
            {
                throw new ArgumentNullException("数据库不存在表："
                    + this.GetType().ToString().Substring(this.GetType().ToString().LastIndexOf('.') + 1).Replace("Repository", ""));
            }

            return Delete(c => c.ID.Equals(id));
        }

        /// <summary>
        /// 根据标识码获取土地利用现状分类表对象
        /// </summary>
        /// <returns>土地利用现状分类表</returns>
        public LandType Get(int id)
        {
            if (!CheckTableExist())
            {
                throw new ArgumentNullException("数据库不存在表："
                    + this.GetType().ToString().Substring(this.GetType().ToString().LastIndexOf('.') + 1).Replace("Repository", ""));
            }

            object data = Get(c => c.ID.Equals(id));
            return (LandType)data;
        }

        /// <summary>
        /// 根据标识码判断土地利用现状分类表对象是否存在
        /// </summary>
        /// <returns>true（存在）/false（不存在）</returns>
        public bool Exists(int id)
        {
            if (!CheckTableExist())
            {
                throw new ArgumentNullException("数据库不存在表："
                    + this.GetType().ToString().Substring(this.GetType().ToString().LastIndexOf('.') + 1).Replace("Repository", ""));
            }

            int cnt = Count(c => c.ID.Equals(id));
            return cnt == 0 ? false : true;
        }

        /// <summary>
        /// 选择所有二级实体对象
        /// </summary>
        /// <returns>土地利用现状分类表集合</returns>
        public LandTypeCollection SelectChildrenType()
        {
            if (!CheckTableExist())
            {
                throw new ArgumentNullException("数据库不存在表："
                    + this.GetType().ToString().Substring(this.GetType().ToString().LastIndexOf('.') + 1).Replace("Repository", ""));
            }
            object data = Get(c => c.UpLevelCode.Equals(null));

            return data as LandTypeCollection;
        }

        /// <summary>
        /// 选择一级下二级实体对象
        /// </summary>
        /// <param name="code">代码</param>
        /// <returns>土地利用现状分类表集合</returns>
        public LandTypeCollection SelectChildrenType(string code)
        {
            if (!CheckTableExist())
            {
                throw new ArgumentNullException("数据库不存在表："
                    + this.GetType().ToString().Substring(this.GetType().ToString().LastIndexOf('.') + 1).Replace("Repository", ""));
            }
            if (!CheckRule.CheckStringNullOrEmpty(ref code))
                return null;

            object data = Get(c => c.UpLevelCode.Equals(code));
            return data as LandTypeCollection;
        }

        /// <summary>
        /// 选择所有类型
        /// </summary>
        /// <returns>土地利用现状分类表集合</returns>
        public LandTypeCollection SelectDetailType()
        {
            if (!CheckTableExist())
            {
                throw new ArgumentNullException("数据库不存在表："
                    + this.GetType().ToString().Substring(this.GetType().ToString().LastIndexOf('.') + 1).Replace("Repository", ""));
            }
            object data = Get();
            return data as LandTypeCollection;
        }

        /// <summary>
        /// 选择主要类型
        /// </summary>
        /// <returns>土地利用现状分类表集合</returns>
        public LandTypeCollection SelectMainType()
        {
            if (!CheckTableExist())
            {
                throw new ArgumentNullException("数据库不存在表："
                    + this.GetType().ToString().Substring(this.GetType().ToString().LastIndexOf('.') + 1).Replace("Repository", ""));
            }
            object data = Get(c => c.UpLevelCode.Equals(null));

            return data as LandTypeCollection;
        }

        /// <summary>
        /// 根据代码选择土地利用现状分类表集合对象
        /// </summary>
        /// <param name="code">代码</param>
        /// <returns>土地利用现状分类表</returns>
        public LandType SelectByCode(string code)
        {
            if (!CheckTableExist())
            {
                throw new ArgumentNullException("数据库不存在表："
                    + this.GetType().ToString().Substring(this.GetType().ToString().LastIndexOf('.') + 1).Replace("Repository", ""));
            }
            if (!CheckRule.CheckStringNullOrEmpty(ref code))
                return null;

            object entity = null;
            object data = Get(c => c.Code.Equals(code));
            return entity as LandType;
        }

        /// <summary>
        /// 根据名称选择对象
        /// </summary>
        /// <param name="name">名称</param>
        /// <returns>土地利用现状分类表</returns>
        public LandType SelectByName(string name)
        {
            if (!CheckTableExist())
            {
                throw new ArgumentNullException("数据库不存在表："
                    + this.GetType().ToString().Substring(this.GetType().ToString().LastIndexOf('.') + 1).Replace("Repository", ""));
            }
            if (!CheckRule.CheckStringNullOrEmpty(ref name))
                return null;

            object entity = null;
            object data = Get(c => c.Name.Equals(name));
            return entity as LandType;
        }
        #endregion
    }
}