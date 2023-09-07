using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YuLinTu.Data;

namespace YuLinTu.Component.MapFoundation
{
    public class LandTopologyTaskGroup : TaskGroup
    {
        #region Methods

        protected override void OnGo()
        {
            var args = Argument as LandTopologyTaskArgument;
            var db = args.DataSource as IDbContext;

            try
            {
                var schema = db.CreateSchema();
                var sr = schema.GetElementSpatialReference(null, ObjectContext.Create(typeof(Library.Entity.Zone)).TableName);

                if (!schema.AnyElement(null, ObjectContext.Create(typeof(Library.Entity.TopologyErrorPoint)).TableName))
                    schema.Export(typeof(Library.Entity.TopologyErrorPoint), sr.WKID);

                if (!schema.AnyElement(null, ObjectContext.Create(typeof(Library.Entity.TopologyErrorPolyline)).TableName))
                    schema.Export(typeof(Library.Entity.TopologyErrorPolyline), sr.WKID);

                if (!schema.AnyElement(null, ObjectContext.Create(typeof(Library.Entity.TopologyErrorPolygon)).TableName))
                    schema.Export(typeof(Library.Entity.TopologyErrorPolygon), sr.WKID);

                db.BeginTransaction();

                if (args.ClearAll)
                {
                    db.CreateQuery<YuLinTu.Library.Entity.TopologyErrorPoint>().Delete().Save();
                    db.CreateQuery<YuLinTu.Library.Entity.TopologyErrorPolygon>().Delete().Save();
                    db.CreateQuery<YuLinTu.Library.Entity.TopologyErrorPolyline>().Delete().Save();
                }

                base.OnGo();

                if (db.Queries.Count > 0)
                    db.Queries.Save();

                db.CommitTransaction();
            }
            catch (Exception ex)
            {
                this.ReportException(ex, string.Format("拓扑检查过程中发生错误，错误信息如下：{0}", ex.Message));
                db.RollbackTransaction();
            }
        }

        #endregion
    }
}
