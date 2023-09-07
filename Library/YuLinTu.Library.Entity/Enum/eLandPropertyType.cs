using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Drawing.Design;

namespace YuLinTu.Library.Entity
{
    /// <summary>
    /// 土地所有权类型
    /// </summary>
    public enum eLandPropertyType
    {
        /// <summary>
        /// 国有土地所有权
        /// </summary>
        [EntityEnumName("key41131", IsLanguageName = true)]
        Stated = 10,

        /// <summary>
        /// 国有土地使用权
        /// </summary>
        [EntityEnumName("key41132", IsLanguageName = true)]
        UsageState = 20,

        /// <summary>
        /// 集体土地所有权
        /// </summary>
        [EntityEnumName("key41133", IsLanguageName = true)]
        Collectived = 30,

        /// <summary>
        /// 村民小组
        /// </summary>
        [EntityEnumName("key41134", IsLanguageName = true)]
        GroupOfPeople = 31,

        /// <summary>
        /// 村集体经济组织
        /// </summary>
        [EntityEnumName("key41135", IsLanguageName = true)]
        VillageCollective = 32,

        /// <summary>
        /// 乡集体经济组织
        /// </summary>
        [EntityEnumName("key41136", IsLanguageName = true)]
        TownCollective = 33,

        /// <summary>
        /// 其它农民集体经济组织
        /// </summary>
        [EntityEnumName("key41137", IsLanguageName = true)]
        OtherCollective = 34,

        /// <summary>
        /// 集体土地使用权
        /// </summary>
        [EntityEnumName("key41138", IsLanguageName = true)]
        UsageCollective = 40,

        /// <summary>
        /// 其它新型体经济组织
        /// </summary>
        [EntityEnumName("key41139", IsLanguageName = true)]
        Other = 0
    };
}
