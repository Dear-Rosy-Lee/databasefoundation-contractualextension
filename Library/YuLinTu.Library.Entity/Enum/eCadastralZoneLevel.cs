using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;


namespace YuLinTu.Library.Entity
{
    /// <summary>
    /// 指示地籍区的级别。
    /// </summary>
    public enum eCadastralZoneLevel
    {
        /// <summary>
        /// 地籍子区
        /// </summary>
        [EntityEnumNameAttribute("ZoneKey41412", IsLanguageName = true)]
        CadastralSubRegion = 2,

        /// <summary>
        /// 地籍区
        /// </summary>
        [EntityEnumNameAttribute("ZoneKey41413", IsLanguageName = true)]
        CadastralRegion = 3,

        /// <summary>
        /// 区县级
        /// </summary>
        [EntityEnumNameAttribute("ZoneKey41414", IsLanguageName = true)]
        County = 4,

        /// <summary>
        /// 市地级
        /// </summary>
        [EntityEnumNameAttribute("ZoneKey41415", IsLanguageName = true)]
        City = 5,

        /// <summary>
        /// 省级
        /// </summary>
        [EntityEnumNameAttribute("ZoneKey41416", IsLanguageName = true)]
        Province = 6,

        /// <summary>
        /// 国家级
        /// </summary>
        [EntityEnumNameAttribute("ZoneKey41417", IsLanguageName = true)]
        State = 7,
    }
}