/*
 * (C) 2015  鱼鳞图公司版权所有,保留所有权利 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YuLinTu.Library.Business
{
    /// <summary>
    /// 导入地块裁剪的类型
    /// </summary>
    public enum eImportLandClipType
    {
        /// <summary>
        /// 按照距离裁剪-等宗裁剪
        /// </summary>
        ClipByDistence = 1,
        
        /// <summary>
        /// 按照面积等分裁剪
        /// </summary>
        ClipByAverageArea = 2,

        /// <summary>
        /// 按照导入text的面积进行裁剪
        /// </summary>
        ClipByText = 3,

        /// <summary>
        /// 按照比例进行裁剪
        /// </summary>
        ClipByTextAndProportion = 4,
    }
}
