using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YuLinTu.Library.Entity
{
    public class LandEmphasisInfo
    {
        /// <summary>
        /// 总户数
        /// </summary>
        public int CountFamily { get; set; }

        /// <summary>
        /// 总人数
        /// </summary>
        public int CountPerson { get; set; }

        /// <summary>
        /// 总实测面积
        /// </summary>
        public double CountActualArea { get; set; }

        /// <summary>
        /// 总确权面积
        /// </summary>
        public double CountAwareArea { get; set; }

        /// <summary>
        /// 总机动地面积
        /// </summary>
        public double CountMotorizeLandArea { get; set; }

        /// <summary>
        /// 总二轮台账面积
        /// </summary>
        public double CountTotalTableArea { get; set; }
    }
}
