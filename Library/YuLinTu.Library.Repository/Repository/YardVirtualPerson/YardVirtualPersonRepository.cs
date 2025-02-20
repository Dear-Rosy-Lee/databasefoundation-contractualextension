// (C) 2025 鱼鳞图公司版权所有，保留所有权利
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YuLinTu.Data;
using YuLinTu.Library.Entity;

namespace YuLinTu.Library.Repository
{
    public class YardVirtualPersonRepository : VirtualPersonRepository<YardVirtualPerson>, IYardVirtualPersonRepository
    {
        #region Ctor

        private IDataSourceSchema m_DSSchema = null;

        public YardVirtualPersonRepository(IDataSource ds)
            : base(ds)
        {
            m_DSSchema = ds.CreateSchema();
        }

        #endregion
    }
}
