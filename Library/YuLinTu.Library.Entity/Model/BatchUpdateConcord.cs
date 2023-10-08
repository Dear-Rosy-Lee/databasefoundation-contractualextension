using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using YuLinTu.Data;

namespace YuLinTu.Library.Entity.Model
{
    public class BatchUpdateConcord
    {
      



        /// <summary>
        /// 耕地承包起始时间
        /// </summary>
        [DisplayName("承包起始日期")]
        [DataColumn("CBQXQ")]
        public DateTime? ArableLandStartTime { get; set; }

        /// <summary>
        ///耕地承包结束时间
        /// </summary>
        [DisplayName("承包结束日期")]
        [DataColumn("CBQXZ")]
        public DateTime? ArableLandEndTime { get; set; }

        /// <summary>
        ///承包期限
        /// </summary>
        [DisplayName("承包期限设置")]
        [DataColumn("JYQX")]
        [Enabled(false)]
        public string ManagementTime { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        [DisplayName("备注")]
        [DataColumn("BZXX")]
        public string Comment { get; set; }
    }
}