using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YuLinTu.Data;
using YuLinTu.Library.Entity;

namespace YuLinTu.Library.Repository
{
    public interface IContractLandDeleteRepository : IRepository<ContractLand_Del>
    {

        List<ContractLand_Del> GetDelLandByZone(string zoneCode);

    }
}
