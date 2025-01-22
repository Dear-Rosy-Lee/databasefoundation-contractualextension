using System.Collections.Generic;

namespace YuLinTu.Component.CoordinateTransformTask
{
    public class ShapeSplitInfo
    {
        public ShapeSplitInfo()
        {
            FullName = string.Empty;
            PropertyNameList = null;
        }
        public ShapeSplitInfo(string fullName, List<string> proList)
        {
            this.FullName = fullName;
            this.PropertyNameList = proList;
        }
        public string FileName { get { return System.IO.Path.GetFileName(FullName); } }
        public string FullName { set; get; }
        public List<string> PropertyNameList { set; get; }
        public string SelectedSplitPropertyName { set; get; }
    }
}
