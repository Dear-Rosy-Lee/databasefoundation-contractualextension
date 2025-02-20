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
    /// 出图签章表设置
    /// </summary>
    public class VisioMapLayoutQZBSetInfo
    {
        /// <summary>
        /// 签章表表头字体大小
        /// </summary>
        public int QZBTitleLabelSize { get; set; }

        /// <summary>
        /// 签章表表头字体颜色
        /// </summary>
        public string QZBTitleFontColor { get; set; }

        /// <summary>
        /// 签章表栏数设置
        /// </summary>
        public int QZBHNumBox { get; set; }

        /// <summary>
        /// 签章表字体大小
        /// </summary>
        public int QZBTableLabelSize { get; set; }

        /// <summary>
        /// 签章表字体颜色
        /// </summary>
        public string QZBTableLabelColor { get; set; }

        /// <summary>
        /// 签章表边框颜色
        /// </summary>
        public string QZBTableBorderColor { get; set; }

        /// <summary>
        /// 签章表单元格高
        /// </summary>
        public int QZBTableCellHeightSize { get; set; }

        /// <summary>
        /// 签章表单元格宽
        /// </summary>
        public int QZBTableCellWidthSize { get; set; }

    }
}
