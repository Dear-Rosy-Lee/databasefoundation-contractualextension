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
    /// 集体建设用地线状地物的数据访问类
    /// </summary>
    public class XZDWRepository : RepositoryDbContext<XZDW>, IXZDWRepository
    {
          #region Ctor

        private IDataSourceSchema m_DSSchema = null;

        public XZDWRepository(IDataSource ds)
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
            //    return m_DSSchema.AnyElement(m_DSSchema.GetElements()[0].Schema, typeof(XZDW).GetAttribute<DataTableAttribute>().TableName);
            //}
            //catch (Exception)
            //{
            //    return false;
            //}
            return true;
        }

        /// <summary>
        /// 根据区域代码获取以标识码排序的线状地物集合
        /// </summary>
        /// <param name="zoneCode">区域代码</param>
        /// <returns>点状地物集合</returns>
        public List<XZDW> GetByZoneCode(string zoneCode)
        {
            if (!CheckTableExist())
            {
                throw new ArgumentNullException("数据库不存在表："
                    + this.GetType().ToString().Substring(this.GetType().ToString().LastIndexOf('.') + 1).Replace("Repository", ""));
            }
            if (!CheckRule.CheckStringNullOrEmpty(ref zoneCode))
                return null;

            object entity = (from q in DataSource.CreateQuery<XZDW>()
                             where q.ZoneCode.Equals(zoneCode)
                             orderby q.BSM
                             select q).ToList();
            return entity as List<XZDW>;
        }


        #endregion
    }

    }

