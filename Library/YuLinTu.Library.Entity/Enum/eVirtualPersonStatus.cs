using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YuLinTu.Library.Entity
{
    public enum eVirtualPersonStatus
    {
        /// <summary>
        /// 正常
        /// </summary>
        [EntityEnumName("6006001", IsLanguageName = true)]
        Right = 0,

        /// <summary>
        /// 锁定
        /// </summary>
       [EntityEnumName("6006002", IsLanguageName = true)]
        Lock = 1,

        /// <summary>
        /// 注销
        /// </summary>
        [EntityEnumName("6006003", IsLanguageName = true)]
        Bad = -1
    }
}
