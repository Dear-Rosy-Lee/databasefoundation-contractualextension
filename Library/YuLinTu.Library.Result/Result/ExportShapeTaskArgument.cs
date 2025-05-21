using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YuLinTu;

namespace YuLinTu.Library.Result
{
    [Serializable]
    public class ExportShapeTaskArgument : TaskArgument
    {
        public string DbFile { get; set; }
        public string ShapeOutPath { get; set; }
        public string ZoneCode { get; set; }
        public string ZoneYearCode { get; set; }
        public int LandNumber { get; set; }
        public int PointNumber { get; set; }
        public int LineNumber { get; set; }

        public string ESRIPrjStr { get; set; }

        public bool OnlyKey { get; set; }

        public bool UseUniteNumberExport { get; set; }

        /// <summary>
        /// 小数位数
        /// </summary>
        public int DataNumber { get; set; }

        public override string ToString()
        {
            return "DbFile:" + DbFile +
               " ShapeOutPath:" + ShapeOutPath +
               " ZoneCode:" + ZoneCode +
               " ZoneYearCode:" + ZoneYearCode +
               " LandNumber:" + LandNumber +
               " PointNumber:" + PointNumber +
               " LineNumber:" + LineNumber +
               " OnlyKey" + OnlyKey +
               " UseUniteNumberExport" + UseUniteNumberExport;
        }
    }
}
