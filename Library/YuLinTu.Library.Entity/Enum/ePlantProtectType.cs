using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Drawing.Design;


namespace YuLinTu.Library.Entity
{
    /// <summary>
    /// 耕保种类
    /// </summary>
    public enum ePlantProtectType
    {
        /// <summary>
        /// 一类(粮食经济作物)
        /// </summary>
        [EntityEnumName("key41301", IsLanguageName = true)]
        FirstGrade = 1,

        /// <summary>
        /// 二类(林木花卉)
        /// </summary>
        [EntityEnumName("key41302", IsLanguageName = true)]
        SecondGrade = 2,

        /// <summary>
        /// 其他
        /// </summary>
        [EntityEnumName("key41303", IsLanguageName = true)]
        UnKnown = 3
    };
}
