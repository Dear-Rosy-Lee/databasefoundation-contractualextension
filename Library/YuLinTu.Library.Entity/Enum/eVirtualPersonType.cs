using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace YuLinTu.Library.Entity
{
    /// <summary>
    /// 承包方类型。
    /// </summary>
    public enum eVirtualPersonType
    {
        /// <summary>
        /// 其它（针对暂时未知的“人”的类型）。
        /// </summary>
        [EntityEnumName("key6005501", IsLanguageName = true)]
        Other,

        /// <summary>
        /// 户。
        /// </summary>
        [EntityEnumName("key6005502", IsLanguageName = true)]
        Family,

        /// <summary>
        /// 集体经济组织。
        /// </summary>
        [EntityEnumName("key6005503", IsLanguageName = true)]
        CollectivityTissue,

        /// <summary>
        /// 自然人
        /// </summary>
        [EntityEnumName("key6005504", IsLanguageName = true)]
        RealityPerson,

        /// <summary>
        /// 公司法人
        /// </summary>
        [EntityEnumName("key6005505", IsLanguageName = true)]
        BusinessEntity
    }
}
