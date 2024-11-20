using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YuLinTu.Data;
using YuLinTu.Library.Entity;

namespace YuLinTu.Library.Repository
{
    public class ContractLandDeleteRepository : RepositoryDbContext<ContractLand_Del>, IContractLandDeleteRepository
    {
  

        private IDataSourceSchema m_DSSchema = null;

        public ContractLandDeleteRepository(IDataSource ds)
            : base(ds)
        {
            m_DSSchema = ds.CreateSchema();
        }

        public List<ContractLand_Del> GetDelLandByZone(string zoneCode)
        {
            var entity = Get(c => c.DYBM.StartsWith(zoneCode));
            return entity.ToList();
        }

    }
}
