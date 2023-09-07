/*
 * (C) 2012-2016 鱼鳞图公司版权所有，保留所有权利
*/
using System;

namespace YuLinTu.Library.Entity
{
    /// <summary>
    /// 业务处理过程状态
    /// </summary>
    [Serializable]
    public enum eProgressType
    {
        /// <summary>
        /// 未知。
        /// </summary>
        [EntityEnumName("未知", IsLanguageName = false)]
        Unknown = 0,

        /// <summary>
        /// 等待提交。
        /// </summary>
        [EntityEnumName("等待提交", IsLanguageName = false)]
        WaitSubmitted = 1,

        /// <summary>
        /// 提交审核。
        /// </summary>
        [EntityEnumName("提交审核", IsLanguageName = false)]
        SubmitCheck = 2,

        /// <summary>
        /// 审核中
        /// </summary>
        [EntityEnumName("审核中", IsLanguageName = false)]
        Checking = 3,

        /// <summary>
        /// 审核通过。
        /// </summary>
        [EntityEnumName("审核通过", IsLanguageName = false)]
        CheckThrough = 4,

        /// <summary>
        /// 审核不通过。
        /// </summary>
        [EntityEnumName("审核不通过", IsLanguageName = false)]
        CheckNotThrough = 5

    }
}