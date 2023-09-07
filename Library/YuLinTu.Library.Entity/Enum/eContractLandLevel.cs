using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Drawing.Design;

namespace YuLinTu.Library.Entity
{
    /// <summary>
    /// 土地级别
    /// </summary>
    public enum eContractLandLevel
    {
        /// <summary>
        /// 一等地
        /// </summary>
        [EntityEnumName("一等地", IsLanguageName = false)]
        OneLevel = 1,

        /// <summary>
        ///  二等地
        /// </summary>
        [EntityEnumName("二等地", IsLanguageName = false)]
        TwoLevel = 2,

        /// <summary>
        /// 三等地
        /// </summary>
        [EntityEnumName("三等地", IsLanguageName = false)]
        ThreeLevel = 3,

        /// <summary>
        /// 四等地
        /// </summary>
        [EntityEnumName("四等地", IsLanguageName = false)]
        FourLevel = 4,

        /// <summary>
        /// 五等地
        /// </summary>
        [EntityEnumName("五等地", IsLanguageName = false)]
        FiveLevel = 5,

        /// <summary>
        /// 六等地
        /// </summary>
        [EntityEnumName("六等地", IsLanguageName = false)]
        SixLevel = 6,

        /// <summary>
        /// 七等地
        /// </summary>
        [EntityEnumName("七等地", IsLanguageName = false)]
        SevenLevel = 7,

        /// <summary>
        /// 八等地
        /// </summary>
        [EntityEnumName("八等地", IsLanguageName = false)]
        EightLevel = 8,

        /// <summary>
        /// 九等地
        /// </summary>
        [EntityEnumName("九等地", IsLanguageName = false)]
        NineLevel = 9,

        /// <summary>
        /// 十等地
        /// </summary>
        [EntityEnumName("十等地", IsLanguageName = false)]
        TenLevel = 10,

        ///// <summary>
        ///// 特级
        ///// </summary>
        //[EntityEnumName("特级", IsLanguageName = false)]
        //SpecialLevel = 11,

        /// <summary>
        /// 未知
        /// </summary>
        [EntityEnumName("未知", IsLanguageName = false)]
        UnKnow = 900,
    }
}
