using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Drawing.Design;


namespace YuLinTu.Library.Entity
{
    /// <summary>
    /// 界线性质
    /// </summary>
    public enum eBoundaryNatureType
    {
        /// <summary>
        /// 已定界
        /// </summary>
        [EntityEnumName("key41081", IsLanguageName = true)]
        FixBoundary = 600001,

        /// <summary>
        /// 未定界
        /// </summary>
        [EntityEnumName("key41082", IsLanguageName = true)]
        UnFixBoundary = 600002,

        /// <summary>
        /// 争议界
        /// </summary>
        [EntityEnumName("key41083", IsLanguageName = true)]
        ArgueBoundary = 600003,

        /// <summary>
        /// 工作界
        /// </summary>
        [EntityEnumName("key41084", IsLanguageName = true)]
        WorkBoundary = 600004,

        /// <summary>
        /// 其他界线
        /// </summary>
        [EntityEnumName("key41085", IsLanguageName = true)]
        Other = 600009

        ///// <summary>
        ///// 其他新型边界
        ///// </summary>
        //[EntityEnumName("key41086", IsLanguageName = true)]
        //OtherTemp = 0
    }
}
