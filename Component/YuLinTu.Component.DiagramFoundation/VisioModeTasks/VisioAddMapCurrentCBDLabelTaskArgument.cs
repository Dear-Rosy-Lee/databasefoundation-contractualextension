/*
 * (C) 2025  鱼鳞图公司版权所有,保留所有权利 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YuLinTu.Appwork;
using YuLinTu.Diagrams;
using YuLinTu.Library.Entity;
using YuLinTu.tGIS.Client;
using YuLinTu.Visio.Designer;
using YuLinTu.Windows;

namespace YuLinTu.Component.DiagramFoundation
{
    /// <summary>
    ///  添加当前地域下承包地图面所有标注参数
    /// </summary>
    public class VisioAddMapCurrentCBDLabelTaskArgument : TaskArgument
    {
        public ElementsDesigner VisiosView { set; get; }

        /// <summary>
        /// 当前地域编码
        /// </summary>
        public string CurrentZoneCode { get; set; }

        /// <summary>
        /// 出图设置
        /// </summary>
        public VisioMapLayoutSetInfo VisioMapLayoutSetInfo { get; set; }
    }
}
