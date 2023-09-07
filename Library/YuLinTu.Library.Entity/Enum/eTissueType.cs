using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Drawing.Design;


namespace YuLinTu.Library.Entity
{
    /// <summary>
    /// 集体经济组织类型
    /// </summary>
    public enum eTissueType
    {
        /// <summary>
        /// 一般
        /// </summary>
        [EntityEnumName("key41231", IsLanguageName = true)]
        General = 1,

        /// <summary>
        /// 新型集体经济组织
        /// </summary>
        [EntityEnumName("key41232", IsLanguageName = true)]
        NewKind,

        /// <summary>
        /// 未知
        /// </summary>
        [EntityEnumName("key41233", IsLanguageName = true)]
        UnKnown
    }
}