using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;


namespace YuLinTu.Library.Entity
{
    /// <summary>
    /// 指示地域的级别。
    /// </summary>
    public enum eZoneLevel
    {
        /// <summary>
        /// 组级
        /// </summary>
        [EntityEnumNameAttribute("key41411", IsLanguageName = true)]
        Group = 1,

        /// <summary>
        /// 村级
        /// </summary>
        [EntityEnumNameAttribute("key41412", IsLanguageName = true)]
        Village = 2,

        /// <summary>
        /// 乡镇级
        /// </summary>
        [EntityEnumNameAttribute("key41413", IsLanguageName = true)]
        Town = 3,

        /// <summary>
        /// 区县级
        /// </summary>
        [EntityEnumNameAttribute("key41414", IsLanguageName = true)]
        County = 4,

        /// <summary>
        /// 市地级
        /// </summary>
        [EntityEnumNameAttribute("key41415", IsLanguageName = true)]
        City = 5,

        /// <summary>
        /// 省级
        /// </summary>
        [EntityEnumNameAttribute("key41416", IsLanguageName = true)]
        Province = 6,

        /// <summary>
        /// 国家级
        /// </summary>
        [EntityEnumNameAttribute("key41417", IsLanguageName = true)]
        State = 7,
    }
}