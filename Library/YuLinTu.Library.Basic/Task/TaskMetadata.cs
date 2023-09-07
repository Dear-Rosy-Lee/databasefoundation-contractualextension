using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YuLinTu.Library.Basic
{
    [Serializable]
    public class TaskMetadata
    {
        public string TaskName { get; set; }
        public object UserState { get; set; }
    }
}
