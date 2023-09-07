using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Drawing.Design;


namespace YuLinTu.Library.Entity
{
    /// <summary>
    /// 占地类型
    /// </summary>
    public enum eHavingLandType
    {
        /// <summary>
        /// 耕地
        /// </summary>
        [EntityEnumName("key41031",IsLanguageName=true)]
        Plough = 1,

        /// <summary>
        /// 非耕地
        /// </summary>
        [EntityEnumName("key41032", IsLanguageName = true)]
        Other = 2,

        /// <summary>
        /// 原宅基地
        /// </summary>
        [EntityEnumName("key41033", IsLanguageName = true)]
        HouseLand = 3,
    }
}
