using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;


namespace YuLinTu.Library.Entity
{
    /* 修改于2016/8/30 根据农业部规范进行修改 */
    public enum eGender
    {
        [EntityEnumName("key41341", IsLanguageName = true)]
        [EnumName("男")]
        Male = 1,

        [EntityEnumName("key41342", IsLanguageName = true)]
        [EnumName("女")]
        Female = 2,

        [EnumName("")]
        [EntityEnumName("key41343", IsLanguageName = true)]
        Unknow = -1,
    }
}
