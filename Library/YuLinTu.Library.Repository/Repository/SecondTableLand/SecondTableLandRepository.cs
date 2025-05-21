// (C) 2025 鱼鳞图公司版权所有，保留所有权利
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
    /// 二轮台账地块数据访问类
    /// </summary>
    public class SecondTableLandRepository : AgricultureLandRepository<SecondTableLand>, ISecondTableLandRepository
    {
        #region Ctor

        private IDataSourceSchema m_DSSchema = null;

        public SecondTableLandRepository(IDataSource ds)
            : base(ds)
        {
            m_DSSchema = ds.CreateSchema();
        }

        #endregion
    }
}