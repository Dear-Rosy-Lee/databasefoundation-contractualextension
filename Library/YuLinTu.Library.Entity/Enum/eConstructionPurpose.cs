using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Drawing.Design;


namespace YuLinTu.Library.Entity
{
    /// <summary>
    /// 实际用途
    /// </summary>
    public enum ConstructionPurpose
    {
        /// <summary>
        /// 居住
        /// </summary>
        [EntityEnumName("key41331", IsLanguageName = true)]
        Inhabitation = 1,

        /// <summary>
        /// 养殖
        /// </summary>
        [EntityEnumName("key41332", IsLanguageName = true)]
        Breed = 2,

        /// <summary>
        /// 其他
        /// </summary>
        [EntityEnumName("key41333", IsLanguageName = true)]
        Pigsty = 9
    };
}
