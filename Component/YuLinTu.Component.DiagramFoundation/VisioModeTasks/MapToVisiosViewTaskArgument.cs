using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YuLinTu.Appwork;
using YuLinTu.Spatial;
using YuLinTu.tGIS.Client;
using YuLinTu.Windows;

namespace YuLinTu.Component.DiagramFoundation
{
    public class MapToVisiosViewTaskArgument : TaskArgument
    {
        #region Properties

        public ITheWorkspace Workspace { get; set; }
        public MapControl MapControl { get; set; }
        public string DestinationDatabaseFileName { get; set; }
        public string TemplateFileName { get; set; }

        /// <summary>
        /// 当前地域编码
        /// </summary>
        public string currentZoneCode { get; set; }
        
        /// <summary>
        /// 选择模式下出图几何范围-也是绘图模式，如果是行政地域导出，就没有值为null
        /// </summary>
        public Geometry ExportGeometryOfExtend
        {
            get;
            set;
        }

        #region 出图设置-字体大小、边框颜色


        #endregion


        #endregion
    }
}
