using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Drawing.Design;


namespace YuLinTu.Library.Entity
{
    /// <summary>
    /// 实体状态
    /// </summary>
    public enum eEntityStatus
    {
        /// <summary>
        /// 正常
        /// </summary>
        [EntityEnumName("正常", IsLanguageName = false)]
        Normal = 10,

        /// <summary>
        /// 变更
        /// </summary>
        [EntityEnumName("变更", IsLanguageName = false)]
        Exchange = 30,

        /// <summary>
        /// 注销
        /// </summary>
        [EntityEnumName("注销", IsLanguageName = false)]
        Deleted = 50,

        /// <summary>
        /// 结束
        /// </summary>
        [EntityEnumName("结束", IsLanguageName = false)]
        End = 80,

        /// <summary>
        /// 未知
        /// </summary>
        [EntityEnumName("未知", IsLanguageName = false)]
        UnKnown = 100
    }
}
