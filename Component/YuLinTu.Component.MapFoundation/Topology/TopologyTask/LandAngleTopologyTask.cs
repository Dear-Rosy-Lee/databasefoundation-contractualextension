using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoAPI.Geometries;
using YuLinTu;
using YuLinTu.Data;
using YuLinTu.Library.Entity;
using YuLinTu.tGISCNet;
using YuLinTu.Library.Business;
using YuLinTu.Spatial;

namespace YuLinTu.Component.MapFoundation
{
    public class LandAngleTopologyTask : LandTopologyTask
    {
        #region Ctor

        public LandAngleTopologyTask()
        {
            Name = "面要素狭长角检查";
            Description = "检查面要素相邻界址线间夹角的角度是否小于容差";
        }

        #endregion

        #region Methods

        protected override void OnGo()
        {
            base.OnGo();

            var args = Argument as LandTopologyTaskArgument;

            var chk = new SmallAngleCheckWKB(args.AngleTolerance);

            //var env = GetLandsEnvelopeByZone(args.ZoneCode);
            //chk.SetFullExtent(env.MinX, env.MinY, env.MaxX, env.MaxY);

            //foreach (var item in dicLand)
            //    chk.AddFeatureID(item.Key);

            var nErrCount = 0;
            var nCount = 0;


            var work = (args.DataSource as IDbContext).CreateContractLandWorkstation();

            work.ForEach((i, cnt, land) =>
            {
                nCount++;
                var result = chk.DoCheck(land.Shape.AsBinary());
                if (result != null && result.Count > 0)
                {
                    foreach (var item in result)
                    {
                        AddError(BuildError(item));
                        ++nErrCount;
                        if (nErrCount % cntBatchReport == 0)
                            this.ReportError(string.Format("发现 {0} 个错误。", nErrCount));
                    }
                }

                return true;
            },
            c => c.LocationCode.StartsWith(args.ZoneCode) && c.Shape != null,
            c => new ContractLand() { ID = c.ID, Shape = c.Shape });

            if (nErrCount > 0)
                this.ReportError("共检查记录条数：" + nCount + "，发现错误个数：" + nErrCount);
            else
                this.ReportInfomation("共检查记录条数：" + nCount + "，发现错误个数：" + nErrCount);

            this.ReportProgress(100);
        }

        private TopologyError BuildError(SmallAngleCheckWKB.Result item)
        {
            var args = Argument as LandTopologyTaskArgument;

            //var land1 = dicLand[r1];
            //var land2 = dicLand[r2];

            var error = new TopologyErrorPolyline();
            error.Description = string.Format("面要素狭长角检查");
            error.LayerName = args.LayerName;
            error.Type = Name;

            try
            {
                error.Shape = Geometry.CreateMultiPolyline(new List<List<Spatial.Coordinate>>() {
                    new List<Spatial.Coordinate>() { new Spatial.Coordinate(item.x0,item.y0), new Spatial.Coordinate(item.x1,item.y1) },
                    new List<Spatial.Coordinate>() { new Spatial.Coordinate(item.x0,item.y0), new Spatial.Coordinate(item.x2,item.y2) },
                }, sr);
            }
            catch
            {
            }

            return error;
        }

        private void AddError(TopologyError topologyError)
        {
            db.Queries.Add(db.CreateQuery<TopologyErrorPolyline>().Add(topologyError));
            TrySave();
        }

        #endregion
    }
}
