
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YuLinTu.Library.WorkStation
{
    /// <summary>
    /// 文件
    /// </summary>
    public class ToolFile
    {
        /// <summary>
        /// 矢量文件shp、MapGis
        /// </summary>
        public const string ShapeFileter = "Shape文件(*.shp)|*.shp|MapGis区文件 (*.wp)|*.wp";

        /// <summary>
        /// 矢量文件xls
        /// </summary>
        public const string ExcelFileter = "Excel文件(*.xls)|*.xls";

        /// <summary>
        /// 矢量文件xls、xlsx
        /// </summary>
        public const string ExcelAllFileter = "Excel文件(*.xlsx,xls)|*.xlsx;*.xls";
    }
}
