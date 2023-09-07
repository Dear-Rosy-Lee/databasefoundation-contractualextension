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
    public class LandAreaTopologyTask : LandTopologyTask
    {
        #region Ctor

        public LandAreaTopologyTask()
        {
            Name = "碎小图斑检查";
            Description = "检查面状要素面积是否小于碎小图斑容差";
        }

        #endregion

        #region Methods

        protected override void OnGo()
        {
            base.OnGo();

            var args = Argument as LandTopologyTaskArgument;

            //var chk = new AreaRepeatPointCheckForWKB();

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

                if (land.Shape.Area() < args.AreaTolerance)
                {
                    AddError(BuildError(land.Shape));
                    ++nErrCount;
                    if (nErrCount % cntBatchReport == 0)
                        this.ReportError(string.Format("发现 {0} 个错误。", nErrCount));
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

        private TopologyError BuildError(Geometry shape)
        {
            var args = Argument as LandTopologyTaskArgument;

            //var land1 = dicLand[r1];
            //var land2 = dicLand[r2];

            var error = new TopologyErrorPolygon();
            error.Description = string.Format("碎小图斑检查");
            error.LayerName = args.LayerName;
            error.Type = Name;

            try
            {
                error.Shape = shape;
            }
            catch
            {
            }

            return error;
        }

        private void AddError(TopologyError topologyError)
        {
            db.Queries.Add(db.CreateQuery<TopologyErrorPolygon>().Add(topologyError));
            TrySave();
        }

        #endregion
    }
}
