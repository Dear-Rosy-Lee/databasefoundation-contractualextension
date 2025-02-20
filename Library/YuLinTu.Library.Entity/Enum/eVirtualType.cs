/*
 * (C) 2025  鱼鳞图公司版权所有,保留所有权利 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Drawing.Design;

namespace YuLinTu.Library.Entity
{
    /// <summary>
    /// 承包方类型
    /// </summary>
    public enum eVirtualType
    {
        /// <summary>
        /// 农用承包地经营权
        /// </summary>
        [EntityEnumName("key6005501", IsLanguageName = true)]
        Land = 1,

        /// <summary>
        /// 集体建设用地使用权
        /// </summary>
        [EntityEnumName("key6005502", IsLanguageName = true)]
        Yard = 2,

        /// <summary>
        /// 房屋所有权
        /// </summary>
        [EntityEnumName("key6005503", IsLanguageName = true)]
        House = 3,

        /// <summary>
        /// 林权
        /// </summary>
        [EntityEnumName("key6005504", IsLanguageName = true)]
        Wood = 4,

        /// <summary>
        /// 集体土地所有权
        /// </summary>
        [EntityEnumName("key6005505", IsLanguageName = true)]
        CollectiveLand = 5,

        /// <summary>
        /// 二轮台账
        /// </summary>
        [EntityEnumName("key6005507", IsLanguageName = true)]
        SecondTable=6,

        /// <summary>
        /// 其他
        /// </summary>
        [EntityEnumName("key6005506", IsLanguageName = true)]
        Other = 0
    }
}
