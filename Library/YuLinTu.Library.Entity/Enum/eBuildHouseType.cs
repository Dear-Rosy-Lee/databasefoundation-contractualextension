using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Drawing.Design;


namespace YuLinTu.Library.Entity
{
    /// <summary>
    /// 建房属性
    /// </summary>
    public enum eBuildHouseType
    {
        /// <summary>
        /// 私有
        /// </summary>
        [EntityEnumName("key41041", IsLanguageName = true)]
        Private = 1,

        /// <summary>
        /// 集体所有
        /// </summary>
        [EntityEnumName("key41042", IsLanguageName = true)]
        Public = 2,

        /// <summary>
        /// 其他
        /// </summary>
        [EntityEnumName("key41043", IsLanguageName = true)]
        Other = 3,

    }
}
