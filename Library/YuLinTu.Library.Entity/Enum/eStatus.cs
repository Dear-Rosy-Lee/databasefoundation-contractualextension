using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Drawing.Design;


namespace YuLinTu.Library.Entity
{
    /// <summary>
    /// 业务流程状态
    /// </summary>
    public enum eStatus
    {
        /// <summary>
        /// 申请
        /// </summary>
        [EntityEnumName("key41241", IsLanguageName = true)]
        Require = 1,

        /// <summary>
        /// 登记
        /// </summary>
        [EntityEnumName("key41242", IsLanguageName = true)]
        Register,

        /// <summary>
        /// 审核
        /// </summary>
        [EntityEnumName("key41243", IsLanguageName = true)]
        Checked,

        /// <summary>
        /// 未知
        /// </summary>
        [EntityEnumName("key41244", IsLanguageName = true)]
        UnKnown
    }
}
