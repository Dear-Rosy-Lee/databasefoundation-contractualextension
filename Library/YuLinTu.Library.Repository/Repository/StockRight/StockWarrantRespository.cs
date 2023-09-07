using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YuLinTu.Data;
using YuLinTu.Library.Entity;

namespace YuLinTu.Library.Repository
{
    public class StockWarrantRespository : RepositoryDbContext<StockWarrant>, IStockWarrantRespository
    {
        public StockWarrantRespository(IDataSource ds) : base(ds)
        {
        }

        public int Count(string zoneCode, eLevelOption searchOption)
        {
            if (!CheckRule.CheckStringNullOrEmpty(ref zoneCode))
            {
                return -1;
            }
            if (searchOption == eLevelOption.Self)
            {
                return Count(c => c.ZoneCode.Equals(zoneCode));
            }
            return Count(c => c.ZoneCode.StartsWith(zoneCode));
        }

        public int Update(StockWarrant entity)
        {
            if (entity == null || !CheckRule.CheckGuidNullOrEmpty(entity.ID))
                return 0;
            entity.ModifiedTime = DateTime.Now;
            return Update(entity, c => c.ID.Equals(entity.ID));
        }
    }
}