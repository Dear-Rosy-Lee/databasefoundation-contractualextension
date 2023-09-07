using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Drawing.Design;


namespace YuLinTu.Library.Entity
{
    /// <summary>
    /// 土地承包方式
    /// </summary>
    public enum eConstructMode
    {
        /// <summary>
        /// 家庭承包
        /// </summary>
        [EntityEnumName("key41261", IsLanguageName = true)]
        Family = 110,

        /// <summary>
        /// 招标
        /// </summary>
        [EntityEnumName("key41262", IsLanguageName = true)]
        Tenderee = 121,

        /// <summary>
        /// 拍卖
        /// </summary>
        [EntityEnumName("key41263", IsLanguageName = true)]
        Vendue = 122,

        /// <summary>
        /// 公开协商
        /// </summary>
        [EntityEnumName("key41264", IsLanguageName = true)]
        Consensus = 123,

        /// <summary>
        /// 其他承包方式（家庭）
        /// </summary>
        [EntityEnumName("key41265", IsLanguageName = true)]
        OtherContract = 129,

        /// <summary>
        /// 转让
        /// </summary>
        [EntityEnumName("key41266", IsLanguageName = true)]
        Transfer = 200,

        /// <summary>
        /// 互换
        /// </summary>
        [EntityEnumName("key41267", IsLanguageName = true)]
        Exchange = 300,

        /// <summary>
        /// 其他方式
        /// </summary>
        [EntityEnumName("key41268", IsLanguageName = true)]
        Other = 900,
    };
}
