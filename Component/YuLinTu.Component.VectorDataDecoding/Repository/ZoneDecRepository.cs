using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using YuLinTu;
using YuLinTu.Data;
using YuLinTu.DF;
using YuLinTu.DF.Data;
using YuLinTu.DF.Repositories;
using YuLinTu.DF.Zones;
using YuLinTu.Spatial;

namespace YuLinTu.Component.VectorDataDecoding.Repository
{
    public class ZoneDecRepository : YltRepository<XZQH_XZDY, Guid>, IZoneRepository
    {


        #region Methods


        #endregion
        public ZoneDecRepository(IDataSource ds) : base(ds)
        {
        }

        public bool AnyByFullName(string uplevelName, string zoneName)
        {
            throw new NotImplementedException();
        }

        public bool AnyByFullName(string uplevelName, string zoneName, Guid id)
        {
            throw new NotImplementedException();
        }

        public bool AnyChildren(string zoneCode, eLevelOption options = eLevelOption.SelfAndSubs)
        {
            throw new NotImplementedException();
        }

        public override Expression<Func<XZQH_XZDY, bool>> FilterExpression(string filter)
        {
            throw new NotImplementedException();
        }

        public List<XZQH_XZDY> GetByLevel(string zoneCode, bool friendly = true, params ZoneLevel[] levels)
        {
            throw new NotImplementedException();
        }

        public XZQH_XZDY GetByZoneCode(string zoneCode, bool friendly = true)
        {
            throw new NotImplementedException();
        }

        public List<XZQH_XZDY> GetChildrenByZoneCode(string zoneCode, eLevelOption options = eLevelOption.SelfAndSubs, bool friendly = true)
        {
            throw new NotImplementedException();
        }

        public XZQH_XZDY GetCountyZone()
        {
            throw new NotImplementedException();
        }

        public override Expression<Func<XZQH_XZDY, string>> GetKey()
        {
            throw new NotImplementedException();
        }

        public Dictionary<string, bool> GetParentAndChildrenCodeAndContainsShapeByZoneCode(string codeZone)
        {
            throw new NotImplementedException();
        }

        public List<XZQH_XZDY> GetParentsByZoneCode(string zoneCode, eLevelOption options = eLevelOption.SelfAndSubs, bool friendly = true)
        {
            throw new NotImplementedException();
        }

        public SpatialReference GetSpatialReference()
        {
            throw new NotImplementedException();
        }
    }
}
