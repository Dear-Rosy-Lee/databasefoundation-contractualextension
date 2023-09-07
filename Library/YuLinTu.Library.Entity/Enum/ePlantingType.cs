using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Drawing.Design;


namespace YuLinTu.Library.Entity
{
    /// <summary>
    /// 种植类型
    /// </summary>
    public enum ePlantingType
    {
        /// <summary>
        /// 粮油
        /// </summary>
        [EntityEnumName("粮油", IsLanguageName = false)]
        GrainAndOil = 1,

        /// <summary>
        /// 蔬菜
        /// </summary>
        [EntityEnumName("蔬菜", IsLanguageName = false)]
        Vegetables = 2,

        /// <summary>
        /// 茶叶
        /// </summary>
        [EntityEnumName("茶叶", IsLanguageName = false)]
        Tea = 3,

        /// <summary>
        /// 果树
        /// </summary>
        [EntityEnumName("果树", IsLanguageName = false)]
        FruitTree = 4,

        /// <summary>
        /// 中草药
        /// </summary>
        [EntityEnumName("中草药", IsLanguageName = false)]
        HerbalMedicine = 5,
        
        /// <summary>
        /// 花卉苗木
        /// </summary>
        [EntityEnumName("花卉苗木", IsLanguageName = false)]
        FlowerAndWood = 6,

        /// <summary>
        /// 其它
        /// </summary>
        [EntityEnumName("其它", IsLanguageName = false)]
        Other = 0
    };
}
