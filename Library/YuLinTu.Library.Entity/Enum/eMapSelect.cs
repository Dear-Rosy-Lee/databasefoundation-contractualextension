using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Drawing.Design;


namespace YuLinTu.Library.Entity
{
    /// <summary>
    /// 施工图选择
    /// </summary>
    public enum eMapSelect
    {
        /// <summary>
        /// 省通用图
        /// </summary>
        [EntityEnumName("key41071", IsLanguageName = true)]
        ProvinceMap = 1,

        /// <summary>
        /// 市通用图
        /// </summary>
        [EntityEnumName("key41072", IsLanguageName = true)]
        CityMap = 2,

        /// <summary>
        /// 自行设计图
        /// </summary>
        [EntityEnumName("key41073", IsLanguageName = true)]
        SelfMap = 3,

    }
}
