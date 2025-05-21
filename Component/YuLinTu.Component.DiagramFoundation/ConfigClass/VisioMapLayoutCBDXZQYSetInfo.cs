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
    /// 制图承包地及行政地域标注设置
    /// </summary>
    public  class VisioMapLayoutCBDXZQYSetInfo
    {
        /// <summary>
        /// 承包地标注大小
        /// </summary>
        public int CbdLabelSize { get; set; }

        /// <summary>
        /// 承包地标注颜色
        /// </summary>
        public string CbdLabelFontColor { get; set; }

        /// <summary>
        /// 承包地标注间隔条高度
        /// </summary>
        public double CbdLabelSeparatorHeight { get; set; }

        /// <summary>
        /// 地域标注大小
        /// </summary>
        public int GroupZoneLabelSize { get; set; }

        /// <summary>
        /// 地域标注颜色
        /// </summary>
        public string GroupZoneLabelColor { get; set; }  
    }
}
