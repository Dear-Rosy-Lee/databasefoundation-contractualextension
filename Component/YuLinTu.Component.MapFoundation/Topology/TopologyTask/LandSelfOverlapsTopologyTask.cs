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
    public class LandSelfOverlapsTopologyTask : LandTopologyTask
    {
        #region Ctor

        public LandSelfOverlapsTopologyTask()
        {
            Name = "面要素边界自重叠";
            Description = "检查同一面状要素内边界是否有重叠的部分";
        }

        #endregion

        #region Methods

        protected override void OnGo()
        {
            base.OnGo();

            var args = Argument as LandTopologyTaskArgument;

            var nErrCount = 0;
            var nCount = 0;

            var work = (args.DataSource as IDbContext).CreateContractLandWorkstation();

            work.ForEach((i, cnt, land) =>
            {
                nCount++;
                LineSelfOverlapCheckForWKB.DoCheck(land.Shape.AsBinary(), args.SelfOverlapsDistanceTolerance, args.SelfOverlapsLengthTolerance * args.SelfOverlapsLengthTolerance, (x1, y1, x2, y2) =>
                 {
                     AddError(BuildError(x1, y1, x2, y2));
                     ++nErrCount;
                     if (nErrCount % cntBatchReport == 0)
                         this.ReportError(string.Format("发现 {0} 个错误。", nErrCount));
                 });
                //AreaRepeatPointCheckForWKB.DoCheck(land.Shape.AsBinary(), args.NodeRepeatTolerance2, args.NodeRepeatTolerance2 * args.NodeRepeatTolerance2, (x1, y1, x2, y2, len2) =>
                //{
                //    AddError(BuildError(x1, y1, x2, y2, len2));
                //    ++nErrCount;
                //    if (nErrCount % 1000 == 0)
                //        this.ReportError(string.Format("发现 {0} 个错误。", nErrCount));
                //});

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

        private TopologyError BuildError(double x1, double y1, double x2, double y2)
        {
            var args = Argument as LandTopologyTaskArgument;

            //var land1 = dicLand[r1];
            //var land2 = dicLand[r2];

            var error = new TopologyErrorPolyline();
            error.Description = string.Format("面要素边界自重叠");
            error.LayerName = args.LayerName;
            error.Type = Name;

            try
            {
                error.Shape = Geometry.CreatePolyline(new List<Spatial.Coordinate>() {
                    new Spatial.Coordinate(x1, y1),
                    new Spatial.Coordinate(x2, y2) }, sr);
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
