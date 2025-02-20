/*
 * (C) 2025  鱼鳞图公司版权所有,保留所有权利 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YuLinTu.Library.Entity;

namespace YuLinTu.Component.DiagramFoundation
{
    /// <summary>
    /// 出图点线面状地物字体及颜色设置
    /// </summary>
    public class VisioMapLayoutDXMZDWSetInfo
    {
        /// <summary>
        /// 点状地物字体大小
        /// </summary>
        public int DZDWLabelSize { get; set; }

        /// <summary>
        /// 点状地物字体颜色
        /// </summary>
        public string DZDWLabelFontColor { get; set; }

        /// <summary>
        /// 线状地物字体大小
        /// </summary>
        public int XZDWLabelSize { get; set; }

        /// <summary>
        /// 线状地物字体颜色
        /// </summary>
        public string XZDWLabelFontColor { get; set; }

        /// <summary>
        /// 面状地物字体大小
        /// </summary>
        public int MZDWLabelSize { get; set; }

        /// <summary>
        /// 面状地物字体颜色
        /// </summary>
        public string MZDWLabelFontColor { get; set; }

    }
}
