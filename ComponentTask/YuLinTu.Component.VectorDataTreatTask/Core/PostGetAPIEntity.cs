using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YuLinTu.Component.VectorDataTreatTask.Core
{
    [Serializable]
    internal class PostGetAPIEntity
    {
        public string token { get; set; }

        public string AppID { get; set; }

        public string AppKey { get; set; }
    }
}
