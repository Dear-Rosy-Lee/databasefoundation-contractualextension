using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Drawing.Design;


namespace YuLinTu.Library.Entity
{
    public enum LandUseRightType
    {
        /// <summary>
        /// 划拨
        /// </summary>
        [EntityEnumName("key41141", IsLanguageName = true)]
        Transfers = 11,

        /// <summary>
        /// 入股
        /// </summary>
        [EntityEnumName("key41142", IsLanguageName = true)]
        Shareholder = 13,

        /// <summary>
        /// 租赁
        /// </summary>
        [EntityEnumName("key41143", IsLanguageName = true)]
        Lease = 14,

        /// <summary>
        /// 授权经营
        /// </summary>
        [EntityEnumName("key41144", IsLanguageName = true)]
        Authorize = 15,

        /// <summary>
        /// 荒地拍卖
        /// </summary>
        [EntityEnumName("key41145", IsLanguageName = true)]
        Auction = 21,

        /// <summary>
        /// 拨用宅基地
        /// </summary>
        [EntityEnumName("key41146", IsLanguageName = true)]
        AssignHouse = 22,

        /// <summary>
        /// 拨用企业用地
        /// </summary>
        [EntityEnumName("key41147", IsLanguageName = true)]
        AssignEnterpriseLand = 23,

        /// <summary>
        /// 农用承包地
        /// </summary>
        [EntityEnumName("key41148", IsLanguageName = true)]
        ContractAgriculturalLand = 24,

        /// <summary>
        /// 集体土地入股
        /// </summary>
        [EntityEnumName("key41149", IsLanguageName = true)]
        ShareCollectiveLand = 25,

        /// <summary>
        /// 自留地
        /// </summary>
        [EntityEnumName("key41150", IsLanguageName = true)]
        SelfLand = 26,

        /// <summary>
        /// 林地
        /// </summary>
        [EntityEnumName("key41151", IsLanguageName = true)]
        ForestLand = 27,

        /// <summary>
        /// 出让
        /// </summary>
        [EntityEnumName("key41152", IsLanguageName = true)]
        Rent = 12,

        /// <summary>
        /// 其他
        /// </summary>
        [EntityEnumName("key41153", IsLanguageName = true)]
        Other = 99,
    }
}
