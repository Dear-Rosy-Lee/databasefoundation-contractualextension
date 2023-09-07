using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Drawing.Design;


namespace YuLinTu.Library.Entity
{
    /// <summary>
    /// 界址点类型
    /// </summary>
    public enum eBoundaryPointType
    {
        /// <summary>
        /// 实测法界址点
        /// </summary>
        [EntityEnumName("key41121", IsLanguageName = true)]
        ResolvePoint = 1,

        /// <summary>
        /// 航测法界址点
        /// </summary>
        [EntityEnumName("key41122", IsLanguageName = true)]
        NavigationPoint = 2,

        /// <summary>
        /// 图解界址点
        /// </summary>
        [EntityEnumName("key41123", IsLanguageName = true)]
        FigurePoint = 3
    }
}
