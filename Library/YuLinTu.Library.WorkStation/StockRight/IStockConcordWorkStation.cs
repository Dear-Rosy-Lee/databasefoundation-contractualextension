using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YuLinTu.Library.Entity;
using YuLinTu.Library.Repository;
using YuLinTu.Data;
namespace YuLinTu.Library.WorkStation
{
    public interface IStockConcordWorkStation :IWorkstation<StockConcord>
    {

        int Add(List<StockConcord> concords);

        List<StockConcord> GetByZoneCode(string zoneCode, eLevelOption searchOption);

        List<StockConcord> GetStockByFamilyID(Guid guid);

        int DeleteByZone(string zoneCode, eLevelOption searchOption);

        int AddRang(List<StockConcord> listRelation);
    }
}
