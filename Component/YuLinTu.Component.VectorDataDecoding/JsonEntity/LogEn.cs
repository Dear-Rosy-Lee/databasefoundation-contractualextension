using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YuLinTu.Component.VectorDataDecoding.JsonEntity
{
    internal class LogEn
    {
        public string scope { get; set; }

        public string user_id { get; set; }

        public string type { get; set; } = "business";

        public string sub_type { get; set; }
        public string logger_case { get; set; } = "Success";
        public string description { get; set; }
        public string owner { get; set; }
    }
}

 

