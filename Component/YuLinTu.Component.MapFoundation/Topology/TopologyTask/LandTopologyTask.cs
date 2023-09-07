using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YuLinTu.Data;
using YuLinTu.Library.Entity;
using YuLinTu.tGISCNet;
using YuLinTu.Library.Business;
using YuLinTu.Spatial;

namespace YuLinTu.Component.MapFoundation
{
    public class LandTopologyTask : TopologyTask
    {
        #region Fields

        protected Dictionary<int, ContractLand> dicLand = null;

        protected IDbContext db = null;
        protected SpatialReference sr = null;
        protected int cntBatchReport = 50;

        #endregion

        #region Methods

        protected override void OnGo()
        {
            dicLand = new Dictionary<int, ContractLand>();
            var args = Argument as LandTopologyTaskArgument;
            db = args.DataSource as IDbContext;
            sr = db.CreateSchema().GetElementSpatialReference(null, typeof(Zone).GetAttribute<DataTableAttribute>().TableName);

            base.OnGo();
        }

        protected Envelope GetLandsEnvelopeByZone(string codeZone)
        {
            var args = Argument as LandTopologyTaskArgument;
            var work = (args.DataSource as IDbContext).CreateContractLandWorkstation();

            Envelope env = new Envelope();

            work.ForEach((i, cnt, land) =>
            {
                env.Union(land.Shape.GetEnvelope());
                //var code = land.ID.GetHashCode();
                //dicLand[code] = land;
                dicLand[i] = land;

                return true;
            },
            c => c.LocationCode.StartsWith(args.ZoneCode) && c.Shape != null,
            c => new ContractLand() { ID = c.ID, Shape = c.Shape });

            return env;
        }

        protected void TrySave()
        {
            if (db.Queries.Count > 100)
                db.Queries.Save();
        }

        protected GeoAPI.Geometries.Envelope GetLandEnvelope(int oid)
        {
            return dicLand[oid].Shape.Instance.EnvelopeInternal as GeoAPI.Geometries.Envelope;
        }

        protected byte[] GetLandWkb(int oid)
        {
            return dicLand[oid].Shape.AsBinary();
        }

        protected Geometry GetLandGeometry(int oid)
        {
            return dicLand[oid].Shape;
        }

        protected Guid GetLandID(int oid)
        {
            return dicLand[oid].ID;
        }

        #endregion
    }
}
