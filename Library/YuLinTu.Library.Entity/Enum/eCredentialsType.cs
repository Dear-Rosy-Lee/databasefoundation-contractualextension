using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Drawing.Design;


namespace YuLinTu.Library.Entity
{
    /// <summary>
    /// 证件类型
    /// </summary>
    public enum eCredentialsType
    {
        /// <summary>
        /// 居民身份证
        /// </summary>
        [EntityEnumName("key41221", IsLanguageName = true)]
        [EnumName("居民身份证")]
        IdentifyCard = 1,

        /// <summary>
        /// 军官证
        /// </summary>
        [EntityEnumName("key41222", IsLanguageName = true)]
        [EnumName("军官证")]
        OfficerCard = 2,

        /// <summary>
        /// 行政、企事业单位机构代码证或法人代码证
        /// </summary>
        [EntityEnumName("key41224", IsLanguageName = true)]
        [EnumName("行政、企事业单位机构代码证或法人代码证")]
        AgentCard = 3,

        /// <summary>
        /// 户口簿
        /// </summary>
        [EntityEnumName("key41225", IsLanguageName = true)]
        [EnumName("户口簿")]
        ResidenceBooklet = 4,

        /// <summary>
        /// 护照
        /// </summary>
        [EntityEnumName("key41223", IsLanguageName = true)]
        [EnumName("护照")]
        Passport = 5,

        /// <summary>
        /// 其他证件
        /// </summary>
        [EntityEnumName("key41226", IsLanguageName = true)]
        [EnumName("其他证件")]
        Other = 9,
    }
}
