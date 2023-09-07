using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Drawing.Design;


namespace YuLinTu.Library.Entity
{
    /// <summary>
    /// 界址线类别
    /// </summary>
    public enum eBoundaryLineCategory
    {
        /// <summary>
        /// 田(垄)埂
        /// </summary>
        [EntityEnumName("key41091", IsLanguageName = true)]
        Baulk = 1,

        /// <summary>
        /// 沟渠
        /// </summary>
        [EntityEnumName("key41095", IsLanguageName = true)]
        Kennel = 2,

        /// <summary>
        /// 道路
        /// </summary>
        [EntityEnumName("key41096", IsLanguageName = true)]
        Road = 3,

        /// <summary>
        /// 行树
        /// </summary>
        [EntityEnumName("key41094", IsLanguageName = true)]
        Linage = 4,

        /// <summary>
        /// 围墙
        /// </summary>
        [EntityEnumName("key41099", IsLanguageName = true)]
        Enclosure = 5,

        /// <summary>
        /// 墙壁
        /// </summary>
        [EntityEnumName("key41092", IsLanguageName = true)]
        Wall = 6,

        /// <summary>
        /// 栅栏
        /// </summary>
        [EntityEnumName("key41093", IsLanguageName = true)]
        Raster = 7,

        /// <summary>
        /// 两点连线
        /// </summary>
        [EntityEnumName("key41097", IsLanguageName = true)]
        LinkLine = 8,

        /// <summary>
        /// 其他界线
        /// </summary>
        [EntityEnumName("key41098", IsLanguageName = true)]
        Other = 99,
    }
}
