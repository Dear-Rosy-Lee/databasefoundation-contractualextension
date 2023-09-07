using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Drawing.Design;


namespace YuLinTu.Library.Entity
{
    /// <summary>
    /// 土地使用者性质
    /// </summary>
    public enum eLandUsePersonType
    {
        /// <summary>
        /// 个人
        /// </summary>
        [EntityEnumName("key41211", IsLanguageName = true)]
        Individual = 1,

        /// <summary>
        /// 户
        /// </summary>
        [EntityEnumName("key41212", IsLanguageName = true)]
        Family = 2,

        /// <summary>
        /// 其他
        /// </summary>
        [EntityEnumName("key41213", IsLanguageName = true)]
        Other = 3,

        /// <summary>
        /// 集体
        /// </summary>
        [EntityEnumName("key41214", IsLanguageName = true)]
        Collective = 4
    }
}