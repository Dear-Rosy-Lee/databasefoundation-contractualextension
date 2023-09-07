using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Drawing.Design;

namespace YuLinTu.Library.Entity
{
    /// <summary>
    /// 界标类型
    /// </summary>
    public enum eLandMarkType
    {
        /// <summary>
        /// 钢钉
        /// </summary>
        [EntityEnumName("key41111", IsLanguageName = true)]
        FixTure = 1,

        /// <summary>
        /// 水泥桩
        /// </summary>
        [EntityEnumName("key41112", IsLanguageName = true)]
        Cement = 2,

        /// <summary>
        /// 石灰桩
        /// </summary>
        [EntityEnumName("key41113", IsLanguageName = true)]
        Lime = 3,

        /// <summary>
        /// 喷涂
        /// </summary>
        [EntityEnumName("key41114", IsLanguageName = true)]
        Shoot = 4,

        /// <summary>
        /// 瓷标志
        /// </summary>
        [EntityEnumName("key41115", IsLanguageName = true)]
        ChinaFlag = 5,

        /// <summary>
        /// 无标志
        /// </summary>
        [EntityEnumName("key41116", IsLanguageName = true)]
        NoFlag = 6,

        /// <summary>
        /// 木桩
        /// </summary>
        [EntityEnumName("key41117", IsLanguageName = true)]
        Piling = 7,

        /// <summary>
        /// 埋石
        /// </summary>
        [EntityEnumName("key41119", IsLanguageName = true)]
        BuriedStone = 8,

        ///// <summary>
        ///// 田埂
        ///// </summary>
        //[EntityEnumName("key41120", IsLanguageName = true)]
        //Baulk = 0,

        /// <summary>
        /// 其他界标
        /// </summary>
        [EntityEnumName("key41118", IsLanguageName = true)]
        Other = 9
    }
}
