using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YuLinTu.Library.Entity
{
    public class CollectiveLandAreaInfo
    {
        /// <summary>
        /// 总面积
        /// </summary>
        public double CountArea { get; set; }

        /// <summary>
        /// 总建设用地面积
        /// </summary>
        public double CountBuildLandArea { get; set; }

        /// <summary>
        /// 总未利用地面积
        /// </summary>
        public double CountUndueArea { get; set; }

        /// <summary>
        /// 总农用地面积
        /// </summary>
        public double CountFarmArea { get; set; }
    }
}
