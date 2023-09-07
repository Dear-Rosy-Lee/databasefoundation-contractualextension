using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Drawing.Design;

namespace YuLinTu.Library.Entity
{
    /// <summary>
    /// 地块类别
    /// </summary>
    public enum eLandCategoryType
    {
        /// <summary>
        /// 承包地块
        /// </summary>
        [EntityEnumName("key41291", IsLanguageName = true)]
        ContractLand = 10,

        /// <summary>
        /// 自留地
        /// </summary>
        [EntityEnumName("key41294", IsLanguageName = true)]
        PrivateLand = 21,

        /// <summary>
        /// 机动地
        /// </summary>
        [EntityEnumName("key41292", IsLanguageName = true)]
        MotorizeLand = 22,

        /// <summary>
        /// 开荒地
        /// </summary>
        [EntityEnumName("key41296", IsLanguageName = true)]
        WasteLand = 23,

        /// <summary>
        /// 其他集体土地
        /// </summary>
        [EntityEnumName("key41295", IsLanguageName = true)]
        CollectiveLand = 99,

        /// <summary>
        /// 经济地
        /// </summary>
        [EntityEnumName("key41293", IsLanguageName = true)]
        EncollecLand = 3,

        /// <summary>
        /// 饲料地
        /// </summary>
        [EntityEnumName("key41297", IsLanguageName = true)]
        FeedLand = 7,

        /// <summary>
        /// 撂荒地
        /// </summary>
        [EntityEnumName("key41298", IsLanguageName = true)]
        AbandonedLand = 8,

        ///// <summary>
        ///// 其它
        ///// </summary>
        //[EntityEnumName("key41300", IsLanguageName = true)]
        //UnKnown
    };
}
