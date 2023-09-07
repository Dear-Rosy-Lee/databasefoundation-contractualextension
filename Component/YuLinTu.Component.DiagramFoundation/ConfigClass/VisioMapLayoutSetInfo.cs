/*
 * (C) 2015  鱼鳞图公司版权所有,保留所有权利 
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
    public class VisioMapLayoutSetInfo
    {        
      
        public VisioMapLayoutCBDXZQYSetInfo CbdxzqySetInfo { get; set; }

        public VisioMapLayoutDXMZDWSetInfo DxmzdwSetInfo { get; set; }

        public VisioMapLayoutQZBSetInfo QzbSetInfo { get; set; }

    }
}
