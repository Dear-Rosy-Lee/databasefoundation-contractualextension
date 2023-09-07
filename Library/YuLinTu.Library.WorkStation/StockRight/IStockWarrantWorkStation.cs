using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YuLinTu.Library.Entity;
using YuLinTu.Library.Repository;
using YuLinTu.Data;

namespace YuLinTu.Library.WorkStation
{
    public interface IStockWarrantWorkStation : IWorkstation<StockWarrant>
    {
        int Add(List<StockWarrant> warrants);

        int AddRang(List<StockWarrant> listRelation);

        int Count(string zoneCode, eLevelOption searchOption);

        int DeleteByZone(string zoneCode, eLevelOption searchOption);

        List<StockWarrant> GetByZoneCode(string zoneCode, eLevelOption searchOption);

        int Update(StockWarrant entity);
    }
}