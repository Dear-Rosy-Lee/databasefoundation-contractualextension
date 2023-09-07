using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Drawing.Design;


namespace YuLinTu.Library.Entity
{
    /// <summary>
    /// 经营方式
    /// </summary>
    /// <summary>
    /// 界线类型
    /// </summary>
    public enum eManageType
    {
        /// <summary>
        /// 自营
        /// </summary>
        [EntityEnumName("key41321", IsLanguageName = true)]
        SelfSupport = 1,

        /// <summary>
        /// 租赁
        /// </summary>
        [EntityEnumName("key41322", IsLanguageName = true)]
        Rent = 2,

        /// <summary>
        /// 转让
        /// </summary>
        [EntityEnumName("key41323", IsLanguageName = true)]
        Transfer = 3,

        /// <summary>
        /// 互换
        /// </summary>
        [EntityEnumName("key41324", IsLanguageName = true)]
        Interchange = 4,

        /// <summary>
        /// 其他
        /// </summary>
        [EntityEnumName("key41325", IsLanguageName = true)]
        Other = 9
    };
}
