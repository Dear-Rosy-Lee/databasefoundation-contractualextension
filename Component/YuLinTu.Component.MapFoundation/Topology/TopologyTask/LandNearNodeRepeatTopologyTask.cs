using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoAPI.Geometries;
using YuLinTu;
using YuLinTu.Data;
using YuLinTu.Library.Entity;
using YuLinTu.Spatial;
using YuLinTu.tGISCNet;

namespace YuLinTu.Component.MapFoundation
{
    public class LandNearNodeRepeatTopologyTask : LandTopologyTask
    {
        #region Ctor

        public LandNearNodeRepeatTopologyTask()
        {
            Name = "相邻面要素节点重叠检查";
            Description = "检查图层中面要素与面要素之间是否节点重叠";
        }

        #endregion

        #region Methods

        protected override void OnGo()
        {
            base.OnGo();

            var args = Argument as LandTopologyTaskArgument;

            var chk = new AreaAdjacentPointCheckForWKB(
                oid =>
                {
                    return GetLandEnvelope(oid);
                },
                oid =>
                {
                    return GetLandWkb(oid);
                }, 0.0001);

            using (chk)
            {
                var env = GetLandsEnvelopeByZone(args.ZoneCode);
                chk.SetFullExtent(env.MinX, env.MinY, env.MaxX, env.MaxY);

                foreach (var item in dicLand)
                    chk.AddFeatureID(item.Key);

                var nErrCount = 0;

                chk.DoCheck(args.NearNodeRepeatTolerance, (r1, r2, x1, y1, x2, y2, len2) =>
                {
                    AddError(BuildError(r1, r2, x1, y1, x2, y2, len2));
                    //this.ReportError(string.Format("检查到标识为 {0} 与 {1} 的地块相互重叠。", GetLandID(r1), GetLandID(r2)));
                    ++nErrCount;
                    if (nErrCount % cntBatchReport == 0)
                        this.ReportError(string.Format("发现 {0} 个错误。", nErrCount));
                }, (i) =>
                {
                    this.ReportProgress(i);
                });

                if (nErrCount > 0)
                    this.ReportError("共检查记录条数：" + dicLand.Count + "，发现错误个数：" + nErrCount);
                else
                    this.ReportInfomation("共检查记录条数：" + dicLand.Count + "，发现错误个数：" + nErrCount);

            }

            this.ReportProgress(100);
        }

        private void AddError(TopologyError topologyError)
        {
            db.Queries.Add(db.CreateQuery<TopologyErrorPoint>().Add(topologyError));
            TrySave();
        }

        private TopologyError BuildError(int r1, int r2, double x1, double y1, double x2, double y2, double len2)
        {
            var args = Argument as LandTopologyTaskArgument;

            //var land1 = dicLand[r1];
            //var land2 = dicLand[r2];

            var error = new TopologyErrorPoint();
            error.Description = string.Format("标识为 {0} 与 {1} 的地块之间存在节点重叠", GetLandID(r1), GetLandID(r2));
            error.LayerName = args.LayerName;
            error.Type = Name;

            try
            {
                error.Shape = Geometry.CreateMultiPoint(new List<Spatial.Coordinate>() {
                    new Spatial.Coordinate(x1, y1),
                    new Spatial.Coordinate(x2, y2) }, sr);
            }
            catch
            {
            }

            return error;
        }

        #endregion
    }
}
