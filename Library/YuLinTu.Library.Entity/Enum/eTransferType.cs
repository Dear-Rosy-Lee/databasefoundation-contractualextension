using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace YuLinTu.Library.Entity
{
    /// <summary>
    /// 流转类型
    /// </summary>
    public enum eTransferType
    {
        /// <summary>
        /// 出租
        /// </summary>
        [EntityEnumName("lang6001001", IsLanguageName = true)]
        Rent = 1,

        /// <summary>
        /// 入股
        /// </summary>
        [EntityEnumName("lang6001002", IsLanguageName = true)]
        Shares = 2,

        /// <summary>
        /// 转包
        /// </summary>
        [EntityEnumName("lang6001003", IsLanguageName = true)]
        Subcontracting = 3,

        /// <summary>
        /// 转让
        /// </summary>
        [EntityEnumName("lang6001004", IsLanguageName = true)]
        Tranfer = 4,

        /// <summary>
        /// 互换
        /// </summary>
        [EntityEnumName("lang6001005", IsLanguageName = true)]
        Exchange = 5,

        /// <summary>
        /// 其他
        /// </summary>
        [EntityEnumName("lang6001006", IsLanguageName = true)]
        Other = 6,

        /// <summary>
        ///  未知
        /// </summary>
        [EntityEnumName("lang6001007", IsLanguageName = true)]
        Other2 = 0
    }
}
