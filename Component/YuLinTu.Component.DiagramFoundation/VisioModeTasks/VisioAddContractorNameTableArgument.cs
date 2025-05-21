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
    ///  添加承包方表格，签章表参数
    /// </summary>
    public class VisioAddContractorNameTableArgument : TaskArgument
    {
        public ElementsDesigner VisiosView { set; get; }               

        //在制图模板上点击后获取的位置信息
        public double locationX { get; set; }
        public double locationY { get; set; }

        /// <summary>
        /// 出图设置
        /// </summary>
        public VisioMapLayoutSetInfo VisioMapLayoutSetInfo { get; set; }
    }
}
