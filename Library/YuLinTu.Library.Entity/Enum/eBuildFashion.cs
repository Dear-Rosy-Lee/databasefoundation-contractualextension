using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Drawing.Design;


namespace YuLinTu.Library.Entity
{
    /// <summary>
    /// 建设方式
    /// </summary>
    public enum eBuildFashion
    {
        /// <summary>
        /// 统规统建
        /// </summary>
        [EntityEnumName("key41061", IsLanguageName = true)]
        TGTJ = 1, 

        /// <summary>
        /// 统规自建
        /// </summary>
        [EntityEnumName("key41062", IsLanguageName = true)]
        TGZJ = 2,

        /// <summary>
        /// 异址重建
        /// </summary>
        [EntityEnumName("key41063", IsLanguageName = true)]
        NCJ = 3,

        /// <summary>
        /// 原址重建
        /// </summary>
        [EntityEnumName("key41064", IsLanguageName = true)]
        OCJ = 4,

    }
}
