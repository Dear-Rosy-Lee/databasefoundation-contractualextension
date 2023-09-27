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
        /// 备注
        /// </summary>
        [DisplayName("备注")]
        [DataColumn("BZXX")]
        public string Comment { get; set; }
    }
}