using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YuLinTu.Data;
using YuLinTu.Library.Entity;

namespace YuLinTu.Library.Repository
{
    public interface IStockWarrantRespository : IRepository<StockWarrant>
    {
        int Count(string zoneCode, eLevelOption searchOption);

        int Update(StockWarrant entity);
    }
}