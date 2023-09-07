using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Drawing.Design;


namespace YuLinTu.Library.Entity
{
    /// <summary>
    /// 集体经济组织成员类型
    /// </summary>
    public enum eTissueMemberType
    {
        /// <summary>
        /// 自然户
        /// </summary>
        [EntityEnumName("key41251",IsLanguageName = true)]
        ActualFamily = 1,

        /// <summary>
        /// 自然人。
        /// </summary>
        [EntityEnumName("key41252", IsLanguageName = true)]
        ActualPerson,

        /// <summary>
        /// 工商组织
        /// </summary>
        [EntityEnumName("key41253", IsLanguageName = true)]
        CommerceTissue,

        /// <summary>
        /// 未知
        /// </summary>
        [EntityEnumName("key41254", IsLanguageName = true)]
        UnKnown
    }
}