using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Drawing.Design;


namespace YuLinTu.Library.Entity
{
    /// <summary>
    /// 房屋结构
    /// </summary>
    public enum eHouseStructure
    {
        /// <summary>
        /// 钢结构
        /// </summary>
        [EntityEnumName("key41021", IsLanguageName = true)]
        Crock = 1,

        /// <summary>
        /// 钢、钢筋混凝土结构
        /// </summary>
        [EntityEnumName("key41022", IsLanguageName = true)]
        CrockAndConcrete,

        /// <summary>
        /// 钢筋混凝土结构
        /// </summary>
        [EntityEnumName("key41023", IsLanguageName = true)]
        Concrete,

        /// <summary>
        /// 混合结构
        /// </summary>
        [EntityEnumName("key41024", IsLanguageName = true)]
        Mix,

        /// <summary>
        /// 砖木结构
        /// </summary>
        [EntityEnumName("key41025", IsLanguageName = true)]
        BrickWood,

        /// <summary>
        /// 简易房
        /// </summary>
        [EntityEnumName("key41026", IsLanguageName = true)]
        Simple,

        /// <summary>
        /// 木
        /// </summary>
        [EntityEnumName("key41027", IsLanguageName = true)]
        Wood,

        /// <summary>
        /// 其他
        /// </summary>
        [EntityEnumName("key41028", IsLanguageName = true)]
        Other = 9,
    }
}
