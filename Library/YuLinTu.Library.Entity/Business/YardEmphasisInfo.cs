using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YuLinTu.Library.Entity
{
    public class YardEmphasisInfo
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
        /// 总现状面积
        /// </summary>
        public double CountSelfArea { get; set; }

        /// <summary>
        /// 总确权面积
        /// </summary>
        public double CountPublicArea { get; set; }

        /// <summary>
        /// 总超占面积
        /// </summary>
        public double CountExceedArea { get; set; }

        /// <summary>
        /// 总违法面积
        /// </summary>
        public double CountIllegalArea { get; set; }
    }
}
