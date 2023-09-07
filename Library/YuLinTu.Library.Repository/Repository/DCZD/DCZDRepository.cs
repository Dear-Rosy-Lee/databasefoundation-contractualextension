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
    /// 集体建设用地调查宗地的数据访问类
    /// </summary>
    public class DCZDRepository : RepositoryDbContext<DCZD>, IDCZDRepository
    {
        #region Ctor

        private IDataSourceSchema m_DSSchema = null;

        public DCZDRepository(IDataSource ds)
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
            //    return m_DSSchema.AnyElement(m_DSSchema.GetElements()[0].Schema, typeof(DCZD).GetAttribute<DataTableAttribute>().TableName);
            //}
            //catch (Exception)
            //{
            //    return false;
            //}
            return true;
        }

        /// <summary>
        /// 根据标识码获取集体建设用地使用权调查宗地对象
        /// </summary>
        /// <param name="id">标识码</param>
        /// <returns>集体建设用地使用权调查宗地对象</returns>
        public DCZD Get(Guid id)
        {
            if (!CheckTableExist())
            {
                throw new ArgumentNullException("数据库不存在表："
                    + this.GetType().ToString().Substring(this.GetType().ToString().LastIndexOf('.') + 1).Replace("Repository", ""));
            }
            if (!CheckRule.CheckGuidNullOrEmpty(id))
                return null;

            var data = Get(c => c.ID == id).FirstOrDefault();
            return data;
        }

        /// <summary>
        /// 判断标识码对象是否存在？
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

            return Count(c => c.ID == id) > 0 ? true : false;
        }



        /// <summary>
        /// 更新集体建设用地使用权调查宗地对象
        /// </summary>
        /// <param name="dot">集体建设用地使用权调查宗地对象</param>
        /// <returns>-1（参数错误）/0（失败）/1（成功）</returns>
        public int Update(DCZD dot)
        {
            if (!CheckTableExist())
            {
                throw new ArgumentNullException("数据库不存在表："
                            + this.GetType().ToString().Substring(this.GetType().ToString().LastIndexOf('.') + 1).Replace("Repository", ""));
            }
            if (dot == null || !CheckRule.CheckGuidNullOrEmpty(dot.ID))
                return -1;

            int cnt = 0;
            cnt = Update(dot, c => c.ID == dot.ID);
            return cnt;
        }

        /// <summary>
        /// 根据标识码删除集体建设用地使用权调查宗地对象数量
        /// </summary>
        /// <param name="ID">标识码</param>
        /// <returns>-1（参数错误）/0（失败）/1（成功）</returns>
        public int Delete(Guid ID)
        {
            if (!CheckTableExist())
            {
                throw new ArgumentNullException("数据库不存在表："
                            + this.GetType().ToString().Substring(this.GetType().ToString().LastIndexOf('.') + 1).Replace("Repository", ""));
            }
            if (!CheckRule.CheckGuidNullOrEmpty(ID))
                return -1;
            int cnt = 0;
            cnt = Delete(c => c.ID == ID);
            return cnt;
        }

        #endregion
    }
}
