using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Drawing.Design;


namespace YuLinTu.Library.Entity
{
    /// <summary>
    /// 耕地坡度级别
    /// </summary>
    public enum eLandSlopeLevel
    {
        /// <summary>
        /// ≤2°
        /// </summary>
        [EntityEnumName("key41311", IsLanguageName = true)]
        LetterTwo = 1,

        /// <summary>
        /// 2°～6°
        /// </summary>
        [EntityEnumName("key41312", IsLanguageName = true)]
        TwoToSix,

        /// <summary>
        /// 6°～15°
        /// </summary>
        [EntityEnumName("key41313", IsLanguageName = true)]
        SixToFifteen,

        /// <summary>
        /// 15°～25°
        /// </summary>
        [EntityEnumName("key41314", IsLanguageName = true)]
        FifteenToTwentyFive,

        /// <summary>
        /// ＞25°
        /// </summary>
        [EntityEnumName("key41315", IsLanguageName = true)]
        LargeTwentyFive,

        /// <summary>
        /// 未知
        /// </summary>
        [EntityEnumName("key41316", IsLanguageName = true)]
        UnKnown = 9
    }
}