/*
 * (C) 2025 鱼鳞图公司版权所有，保留所有权利
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YuLinTu;
using YuLinTu.Data;
using YuLinTu.Spatial;

namespace YuLinTu.Library.Entity
{
    /// <summary>
    /// 创建实体类——字典类
    /// </summary>
    [DataTable("TopologyError")]
    [Serializable]
    public class TopologyError : NotifyCDObject
    {
        #region Properties

        public string LayerName { get; set; }
        public string LayerAliasName { get; set; }
        public string Type { get; set; }
        public string Description { get; set; }

        [DataColumn(PrimaryKey = true)]
        public Guid ID { get; set; }

        public Geometry Shape { get; set; }

        #endregion

        #region Ctor

        public TopologyError()
        {
            ID = Guid.NewGuid();
        }

        #endregion
    }


    [DataTable("TopologyErrorPoint")]
    public class TopologyErrorPoint : TopologyError
    {
    }
    [DataTable("TopologyErrorPolyline")]
    public class TopologyErrorPolyline : TopologyError
    {
    }
    [DataTable("TopologyErrorPolygon")]
    public class TopologyErrorPolygon : TopologyError
    {
    }
}