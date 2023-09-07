using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Drawing.Design;


namespace YuLinTu.Library.Entity
{
    /// <summary>
    /// 承包人类型
    /// </summary>
    public enum eContracterType
    {
        /// <summary>
        /// 户
        /// </summary>
        [EntityEnumName("key41281", IsLanguageName = true)]
        Family = 1,

        /// <summary>
        /// 自然人
        /// </summary>
        [EntityEnumName("key41282", IsLanguageName = true)]
        Person,

        /// <summary>
        /// 集体经济组织
        /// </summary>
        [EntityEnumName("key41283", IsLanguageName = true)]
        Organization,

        /// <summary>
        /// 企业
        /// </summary>
        [EntityEnumName("key41284", IsLanguageName = true)]
        Company,

        /// <summary>
        /// 其它
        /// </summary>
        [EntityEnumName("key41285", IsLanguageName = true)]
        Other
    }
}
