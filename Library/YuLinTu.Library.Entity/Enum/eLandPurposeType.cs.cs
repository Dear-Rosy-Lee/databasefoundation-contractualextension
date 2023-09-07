using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Drawing.Design;


namespace YuLinTu.Library.Entity
{
    /// <summary>
    /// 土地的实际用途
    /// </summary>
    public enum eLandPurposeType
    {
        /// <summary>
        /// 种植业
        /// </summary>
        [EntityEnumName("key41271", IsLanguageName = true)]
        Planting = 1,

        /// <summary>
        /// 林业
        /// </summary>
        [EntityEnumName("key41272", IsLanguageName = true)]
        WoodPlant = 2,

        /// <summary>
        /// 畜牧业
        /// </summary>
        [EntityEnumName("key41273", IsLanguageName = true)]
        Cultred = 3,

        /// <summary>
        /// 渔业
        /// </summary>
        [EntityEnumName("key41274", IsLanguageName = true)]
        Fish = 4,

        /// <summary>
        /// 非农业用途
        /// </summary>
        [EntityEnumName("key41275", IsLanguageName = true)]
        Other = 5
    }
}
