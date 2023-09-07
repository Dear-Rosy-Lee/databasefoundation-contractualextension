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
    /// 农村土地承包经营权申请审批表的数据访问类
    /// </summary>
    public class ContractCheckTableRepository : RepositoryDbContext<ContractCheckTable>,  IContractCheckTableRepository
    { 
        #region Ctor
        
        private IDataSourceSchema m_DSSchema = null;

        public ContractCheckTableRepository(IDataSource ds)
            : base(ds) 
        {
            m_DSSchema = ds.CreateSchema();
        }
        #endregion

        #region Methods
        
		#endregion
	}
}