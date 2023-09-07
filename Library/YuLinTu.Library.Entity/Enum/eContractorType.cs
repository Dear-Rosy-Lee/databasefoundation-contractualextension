/*
 * (C) 2012-2016 鱼鳞图公司版权所有，保留所有权利
*/
using System;

namespace YuLinTu.Library.Entity
{
    /// <summary>
    /// 承包方类型
    /// </summary>
    [Serializable]
    public enum eContractorType
    {
        /// <summary>
        /// 农户
        /// </summary>
        [EntityEnumName("农户", IsLanguageName = false)]
        Farmer = 1,

        /// <summary>
        /// 个人
        /// </summary>
        [EntityEnumName("个人", IsLanguageName = false)]
        Personal = 2,

        /// <summary>
        /// 单位
        /// </summary>
        [EntityEnumName("单位", IsLanguageName = false)]
        Unit = 3
    }
}