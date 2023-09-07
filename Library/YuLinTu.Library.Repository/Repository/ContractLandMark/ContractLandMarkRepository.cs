// (C) 2015 鱼鳞图公司版权所有，保留所有权利
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YuLinTu.Library.Entity;
using System.Data;
using System.Data.SqlClient;
using YuLinTu.Data;
using YuLinTu;

namespace YuLinTu.Library.Repository
{
    /// <summary>
    /// 农村土地承包地宗地标注的数据访问类
    /// </summary>
    public class ContractLandMarkRepository : AgricultureLandRepository<ContractLandMark>, IContractLandMarkRepository
    {
        #region Ctor

        private IDataSourceSchema m_DSSchema = null;

        public ContractLandMarkRepository(IDataSource ds)
            : base(ds)
        {
            m_DSSchema = ds.CreateSchema();
        }
        #endregion
    }
}