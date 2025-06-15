using System;

namespace YuLinTu.Component.VectorDataLinkageTask.Core
{
    [Serializable]
    internal class PostGetAPIEntity
    {
        public string token { get; set; }

        public string AppID { get; set; }

        public string AppKey { get; set; }
    }
}
