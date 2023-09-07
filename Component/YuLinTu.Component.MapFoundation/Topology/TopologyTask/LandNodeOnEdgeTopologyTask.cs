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

namespace YuLinTu.Component.MapFoundation
{
    public class LandNodeOnEdgeTopologyTask : LandTopologyTask
    {
        #region Ctor

        public LandNodeOnEdgeTopologyTask()
        {
            Name = "面要素公用节点检查";
            Description = "检查图层中面要素的节点是否在其他面要素的边上";
        }

        #endregion

        #region Methods

        protected override void OnGo()
        {
            base.OnGo();

            var args = Argument as LandTopologyTaskArgument;

            var chk = new AreaVertexOnEdgeCheckForWKB(args.NodeOnEdgeTolerance);

            using (chk)
            {
                GetLandsEnvelopeByZone(args.ZoneCode);

                foreach (var item in dicLand)
                {
                    var wkb = item.Value.Shape.AsBinary();
                    chk.AddGeometry(item.Key, wkb);
                }

                var nErrCount = 0;

                chk.DoCheck((oid, x, y) =>
                {
                    AddError(BuildError(oid, x, y));
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

        private TopologyError BuildError(int oid, double x, double y)
        {
            var args = Argument as LandTopologyTaskArgument;

            //var land1 = dicLand[r1];
            //var land2 = dicLand[r2];

            var error = new TopologyErrorPoint();
            error.Description = string.Format("标识为 {0} 的面要素节点在其他面要素的边上", GetLandID(oid));
            error.LayerName = args.LayerName;
            error.Type = Name;

            try
            {
                error.Shape = Spatial.Geometry.CreatePoint(new Spatial.Coordinate(x, y), sr);
            }
            catch
            {
            }

            return error;
        }

        #endregion
    }
}
