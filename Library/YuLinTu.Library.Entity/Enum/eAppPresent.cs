using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Drawing.Design;


namespace YuLinTu.Library.Entity
{
    /// <summary>
    /// 当前状态
    /// </summary>
    public enum eAppPersont
    {
        /// <summary>
        /// 申请
        /// </summary>
        [EntityEnumName("key41051", IsLanguageName = true)]
        Application = 1,

        /// <summary>
        /// 审核中
        /// </summary>
        [EntityEnumName("key41052", IsLanguageName = true)]
        Check = 2,

        /// <summary>
        /// 审核完成
        /// </summary>
        [EntityEnumName("key41053", IsLanguageName = true)]
        CheckOver = 3,

        /// <summary>
        /// 正在修建
        /// </summary>
        [EntityEnumName("key41054", IsLanguageName = true)]
        Building = 4,

        /// <summary>
        /// 竣工
        /// </summary>
        [EntityEnumName("key41055", IsLanguageName = true)]
        Complete = 5,

    }
}
