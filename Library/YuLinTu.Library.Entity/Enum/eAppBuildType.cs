using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Drawing.Design;
using YuLinTu;
using YuLinTu.Data;


namespace YuLinTu.Library.Entity
{
    /// <summary>
    /// 申建性质
    /// </summary>
    public enum eAppBuildType
    {
        /// <summary>
        /// 自建
        /// </summary>
        [EntityEnumName("key41011", IsLanguageName = true)]
        Private = 1,

        /// <summary>
        /// 其他
        /// </summary>
        [EntityEnumName("key41012", IsLanguageName = true)]
        Other = 2,

    }
}
